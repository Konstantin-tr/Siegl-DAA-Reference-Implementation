// <copyright file="CreateProductCommandActor.cs" company="Konstantin Siegl">
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
    internal class CreateProductCommandActor : Grain, ICreateProductCommandActor
    {
        public async Task<CommandResponse<CreateProductResponse>> Execute(string productName, string productDescription, decimal priceInEuro, int availableStock)
        {
            var newId = Guid.NewGuid().ToString();

            var productRepo = GrainFactory.GetGrain<IProductRepository>(0);
            var product = GrainFactory.GetGrain<IProductActor>(newId);

            var result = await product.Create(productName, productDescription, priceInEuro, availableStock);

            await productRepo.AddProduct(newId);

            return new(result.IsSuccess, result.IsSuccess ? new(newId) : null, result.Message);
        }
    }
}
