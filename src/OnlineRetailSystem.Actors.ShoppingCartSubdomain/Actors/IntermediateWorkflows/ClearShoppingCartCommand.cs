// <copyright file="ClearShoppingCartCommand.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class ClearShoppingCartCommand : Grain, IClearShoppingCartCommandActor
    {
        public async Task<CommandResponse> Execute(string shoppingCartId)
        {
            var cart = GrainFactory.GetGrain<IShoppingCartActor>(shoppingCartId);

            await cart.Clear();

            return new(true, null);
        }
    }
}
