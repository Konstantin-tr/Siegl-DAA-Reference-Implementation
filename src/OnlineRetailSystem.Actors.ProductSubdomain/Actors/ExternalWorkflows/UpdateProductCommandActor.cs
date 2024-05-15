// <copyright file="UpdateProductCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class UpdateProductCommandActor : Grain, IUpdateProductCommandActor
    {
        public async Task<CommandResponse> Execute(string productId, string productName, string productDescription, decimal priceInEuro, int availableStock)
        {
            var product = GrainFactory.GetGrain<IProductActor>(productId);

            var result = await product.Update(productName, productDescription, priceInEuro, availableStock);

            return new(result.IsSuccess, result.Message);
        }
    }
}
