// <copyright file="IProductActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain
{
    internal interface IProductActor : IGrainWithStringKey
    {
        [ReadOnly]
        [Transaction(TransactionOption.Join)]
        public Task<ProductInfo?> GetData();

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> Create(string productName, string productDescription, decimal priceInEuro, int AvailableStock);

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> Update(string productName, string productDescription, decimal priceInEuro, int AvailableStock);

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> Delete();

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> ReduceStock(int amount);

        [ReadOnly]
        [Transaction(TransactionOption.Join)]
        public Task<int?> GetStock();
    }

    [GenerateSerializer]
    internal record ProductInfo(string ProductName, string Description, decimal PriceInEuro, int AvailableStock);
}
