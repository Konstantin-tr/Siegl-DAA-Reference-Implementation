// <copyright file="GetShoppingCartItemsQuery.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;
using ShoppingCartItemData = OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows.ShoppingCartItemData;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class GetShoppingCartItemsQuery : Grain, IGetShoppingCartItemsQueryActor
    {
        public async Task<GetShoppingCartItemsQueryResponse> Execute(string shoppingCartId)
        {
            var cart = GrainFactory.GetGrain<IShoppingCartActor>(shoppingCartId);

            var items = await cart.GetItems();

            return new(items.Select(i => new ShoppingCartItemData(i.ItemId, i.ProductId, i.Quantity)).ToArray());
        }
    }
}
