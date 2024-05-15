// <copyright file="IViewProductQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows
{
    public interface IViewProductQueryActor : IGrainWithIntegerKey
    {
        [ReadOnly]
        [Transaction(TransactionOption.Create)]
        public Task<ViewProductResponse> Execute(string productId);
    }

    [GenerateSerializer]
    public record ViewProductResponse(ProductViewData? Product);

    [GenerateSerializer]
    public record ProductViewData(string ProductId, string ProductName, string Description, decimal PriceInEuro, int AvailableStock);
}
