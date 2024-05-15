// <copyright file="DeleteProductCommandActor.cs" company="Konstantin Siegl">
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
    internal class DeleteProductCommandActor : Grain, IDeleteProductCommandActor
    {
        public async Task<CommandResponse> Execute(string productId)
        {
            var productRepo = GrainFactory.GetGrain<IProductRepository>(0);
            var product = GrainFactory.GetGrain<IProductActor>(productId);

            CommandResponse result = null!;

            async Task Delete()
            {
                result = await product.Delete();
            }

            await Task.WhenAll(
                productRepo.RemoveProduct(productId),
                Delete()
            );

            return new(result.IsSuccess, result.Message);
        }
    }
}
