// <copyright file="ICreateProductCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows
{
    public interface ICreateProductCommandActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        public Task<CommandResponse<CreateProductResponse>> Execute(string productName, string productDescription, decimal priceInEuro, int availableStock);
    }

    [GenerateSerializer]
    public record CreateProductResponse(string ProductId);
}
