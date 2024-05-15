// <copyright file="IListProductsQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows
{
    public interface IListProductsQueryActor : IGrainWithIntegerKey
    {
        [ReadOnly]
        [Transaction(TransactionOption.Create)]
        public Task<ListProductsResponse> Execute(string? nameFilter, string? orderBy, bool? orderDescending);
    }

    [GenerateSerializer]
    public record ListProductsResponse(ProductListData[] Products);

    [GenerateSerializer]
    public record ProductListData(string ProductId, string ProductName, decimal PriceInEuro, int AvailableStock);
}
