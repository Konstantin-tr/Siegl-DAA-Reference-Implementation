// <copyright file="ViewShoppingCartQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class ViewShoppingCartQueryActor : Grain, IViewShoppingCartQueryActor
    {
        public async Task<ViewShoppingCartQueryResponse> Execute(string shoppingCartId)
        {
            var cart = GrainFactory.GetGrain<IShoppingCartActor>(shoppingCartId);

            var items = await cart.GetItems();

            if (items.Length == 0)
            {
                return new(new(0, []));
            }

            var getProductInfoQuery = GrainFactory.GetGrain<IGetProductInformationQueryActor>(0);

            var results = await Task.WhenAll(
                items.Select(p => GetProduct(p, getProductInfoQuery))
            );

            var productResults = results.Where(x => x is not null).Cast<ShoppingCartItemInfo>().ToArray();

            var totalPrice = productResults.Sum(p => p.Quantity * p.PriceInEuro);

            return new(new(totalPrice, productResults));
        }

        private static async Task<ShoppingCartItemInfo?> GetProduct(ShoppingCartItemDomainData item, IGetProductInformationQueryActor query)
        {
            var response = await query.Execute(item.ProductId);

            var product = response.Product;

            if (product is null)
            {
                return null;
            }

            return new ShoppingCartItemInfo(item.ItemId, product.ProductId, product.ProductName, item.Quantity, product.PriceInEuro);
        }
    }
}
