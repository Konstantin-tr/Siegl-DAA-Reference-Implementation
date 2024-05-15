// <copyright file="CreateOrderFromShoppingCartCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class CreateOrderFromShoppingCartCommandActor : Grain, ICreateOrderFromShoppingCartCommandActor
    {
        public async Task<CommandResponse<CreateOrderFromShoppingCartResponse>> Execute(string shoppingCartId, string customerId)
        {
            var getItemsQuery = GrainFactory.GetGrain<IGetShoppingCartItemsQueryActor>(0);

            var response = await getItemsQuery.Execute(shoppingCartId);

            if (response.Items is null)
            {
                throw new ActorCommandException("Cart does not exist");
            }

            if (response.Items.Length == 0)
            {
                throw new ActorCommandException("Cart has no items");
            }

            await ReduceStock(response.Items);

            OrderItemData[] items = await ProcessParallelActionsAndGetProducts(shoppingCartId, response.Items);

            return await CreateOrder(customerId, items);
        }

        private async Task ReduceStock(ShoppingCartItemData[] cartItems)
        {
            var reduceStockCommand = GrainFactory.GetGrain<IReduceProductStockCommand>(0);

            await Task.WhenAll(cartItems.Select(i => reduceStockCommand.Execute(i.ProductId, i.Quantity)));
        }

        private async Task<OrderItemData[]> ProcessParallelActionsAndGetProducts(string shoppingCartId, ShoppingCartItemData[] cartItems)
        {
            var getProductInfoQuery = GrainFactory.GetGrain<IGetProductInformationQueryActor>(0);

            OrderItemData[] items = null!;

            var loadItems = async () =>
            {
                OrderItemData?[] orderItems = await GetOrderItems(cartItems, getProductInfoQuery);

                items = orderItems.Where(p => p is not null).Cast<OrderItemData>().ToArray();
            };

            var clearCart = () => ClearCart(shoppingCartId);

            await Task.WhenAll(loadItems(), clearCart());

            return items;
        }

        private async Task ClearCart(string shoppingCartId)
        {
            var clearCartCommand = GrainFactory.GetGrain<IClearShoppingCartCommandActor>(0);

            await clearCartCommand.Execute(shoppingCartId);
        }

        private static async Task<OrderItemData?[]> GetOrderItems(ShoppingCartItemData[] cartItems, IGetProductInformationQueryActor getProductInfoQuery)
        {
            return await Task.WhenAll(cartItems.Select(async (item) =>
            {
                return await GetOrderItem(getProductInfoQuery, item);
            }));
        }

        private static async Task<OrderItemData?> GetOrderItem(IGetProductInformationQueryActor getProductInfoQuery, ShoppingCartItemData item)
        {
            var response = await getProductInfoQuery.Execute(item.ProductId);

            if (response.Product is null)
            {
                return null;
            }

            return new OrderItemData(item.ItemdId, item.ProductId, response.Product.PriceInEuro, item.Quantity);
        }

        private async Task<CommandResponse<CreateOrderFromShoppingCartResponse>> CreateOrder(string customerId, OrderItemData[] items)
        {
            var orderId = Guid.NewGuid().ToString();

            var order = GrainFactory.GetGrain<IOrderActor>(orderId);
            var orderRepository = GrainFactory.GetGrain<IOrderRepository>(0);

            CommandResponse result = null!;

            async Task Create()
            {
                result = await order.Create(DateTime.Now, items);
            }

            await Task.WhenAll(
                orderRepository.AddOrder(orderId, customerId),
                Create()
            );

            return new(result.IsSuccess, result.IsSuccess ? new(orderId) : null, result.Message);
        }
    }
}
