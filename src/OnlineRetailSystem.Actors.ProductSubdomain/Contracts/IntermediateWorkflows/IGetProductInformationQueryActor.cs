// <copyright file="IGetProductInformationQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows
{

    public interface IGetProductInformationQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        [ReadOnly]
        public Task<GetProductInformationQueryResult> Execute(string productId);
    }

    [GenerateSerializer]
    public record GetProductInformationQueryResult(ProductResult? Product);

    [GenerateSerializer]
    public record ProductResult(string ProductId, string ProductName, decimal PriceInEuro, int AvailableStock);
}
