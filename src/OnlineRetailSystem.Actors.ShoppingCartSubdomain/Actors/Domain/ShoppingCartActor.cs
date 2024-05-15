// <copyright file="ShoppingCartActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using System.Collections.Immutable;

namespace OnlineRetailSystem.Actors.CustomerSubdomain.Actors.Domain
{
    [Reentrant]
    internal class ShoppingCartActor
   (
       [TransactionalState(stateName: "shopping-cart", storageName: "online-retail")]
        ITransactionalState<ShoppingCartState> state
   ) : Grain, IShoppingCartActor
    {
        private readonly ITransactionalState<ShoppingCartState> _state = state;

        public async Task<CommandResponse<ShoppingCartAddProductResponse>> AddProduct(string productId, int quantity)
        {
            return await _state.PerformUpdate<CommandResponse<ShoppingCartAddProductResponse>>(state =>
            {
                var id = $"item_{productId}";

                var items = state.Items;

                if (items.ContainsKey(id))
                {
                    throw new ActorCommandException("Product was already added to shopping cart.");
                }

                if (quantity < 1)
                {
                    throw new ActorCommandException("Quantity cannot be less than 1.");
                }

                state.Items = items.Add(id, new() { ProductId = productId, OrderItemId = id, Quantity = quantity });

                return new(true, new(id), null);
            });
        }

        public Task Clear()
        {
            return _state.PerformUpdate(s =>
            {
                s.Items = s.Items.Clear();
            });
        }

        public Task<ShoppingCartItemDomainData[]> GetItems()
        {
            return _state.PerformRead(state =>
            {
                return state.Items.Values.Select(i => new ShoppingCartItemDomainData(i.OrderItemId, i.ProductId, i.Quantity)).ToArray();
            });
        }

        public async Task<CommandResponse> RemoveProduct(string productId)
        {
            return await _state.PerformUpdate<CommandResponse>(state =>
            {
                var id = $"item_{productId}";

                var items = state.Items;

                if (!items.ContainsKey(id))
                {
                    throw new ActorCommandException("Product does not exsit in shopping cart.");
                }

                state.Items = items.Remove(id);

                return new(true, null);
            });
        }

        public async Task<CommandResponse> UpdateProductQuantity(string productId, int quantity)
        {
            return await _state.PerformUpdate<CommandResponse>(state =>
            {
                var id = $"item_{productId}";

                var items = state.Items;

                if (!items.TryGetValue(id, out ShoppingCartItem? value))
                {
                    throw new ActorCommandException("Product does not exsit in shopping cart.");
                }

                if (quantity < 1)
                {
                    throw new ActorCommandException("Quantity cannot be less than 1.");
                }

                value.Quantity = quantity;

                return new(true, null);
            });
        }
    }

    [GenerateSerializer]
    internal class ShoppingCartState()
    {
        [Id(0)]
        public ImmutableDictionary<string, ShoppingCartItem> Items
        {
            get;
            set;
        } = ImmutableDictionary<string, ShoppingCartItem>.Empty;
    }

    [GenerateSerializer]
    internal class ShoppingCartItem
    {
        [Id(0)]
        public required string OrderItemId { get; set; }

        [Id(1)]
        public required string ProductId { get; set; }

        [Id(2)]
        public required int Quantity { get; set; }
    }
}


