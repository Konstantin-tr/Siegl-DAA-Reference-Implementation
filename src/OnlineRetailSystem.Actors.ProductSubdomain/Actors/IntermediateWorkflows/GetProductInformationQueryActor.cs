// <copyright file="GetProductInformationQueryActor.cs" company="Konstantin Siegl">
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
    internal class GetProductInformationQueryActor : Grain, IGetProductInformationQueryActor
    {
        public async Task<GetProductInformationQueryResult> Execute(string productId)
        {
            var product = GrainFactory.GetGrain<IProductActor>(productId);

            var data = await product.GetData();

            if (data is null)
            {
                return new(null);
            }

            return new(new(productId, data.ProductName, data.PriceInEuro, data.AvailableStock));
        }
    }
}
