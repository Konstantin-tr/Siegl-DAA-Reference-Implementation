// <copyright file="HasSufficientProductStockQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class HasSufficientProductStockQueryActor : Grain, IHasSufficientProductStockQueryActor
    {
        public async Task<HasSufficientProductStockQueryResult> Execute(string productId, int amount)
        {
            var product = GrainFactory.GetGrain<IProductActor>(productId);

            var stock = await product.GetStock();

            if (stock is null)
            {
                return new(null);
            }

            return new(new(stock >= amount));
        }
    }
}
