// <copyright file="ReduceProductStockCommand.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class ReduceProductStockCommand : Grain, IReduceProductStockCommand
    {
        public async Task<CommandResponse> Execute(string productId, int amount)
        {
            var product = GrainFactory.GetGrain<IProductActor>(productId);

            return await product.ReduceStock(amount);

        }
    }
}
