// <copyright file="ChangeShoppingCartProductQuantityCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class ChangeShoppingCartProductQuantityCommandActor : Grain, IChangeShoppingCartProductQuantityCommandActor
    {
        public async Task<CommandResponse> Execute(string shoppingCartId, string productId, int quantity)
        {
            var cart = GrainFactory.GetGrain<IShoppingCartActor>(shoppingCartId);
            var hasSufficientProductStockQuery = GrainFactory.GetGrain<IHasSufficientProductStockQueryActor>(0);

            var response = await hasSufficientProductStockQuery.Execute(productId, quantity);

            if (response.Result is null)
            {
                throw new ActorCommandException("Product could not be found.");
            }

            if (!response.Result.HasSufficientStock)
            {
                throw new ActorCommandException("Product does not have sufficient stock.");
            }

            return await cart.UpdateProductQuantity(productId, quantity);
        }
    }
}
