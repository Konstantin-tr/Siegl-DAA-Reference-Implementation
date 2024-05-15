// <copyright file="IProductRepository.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain
{
    internal interface IProductRepository : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        public Task AddProduct(string productId);

        [Transaction(TransactionOption.CreateOrJoin)]
        public Task RemoveProduct(string productId);

        [ReadOnly]
        [Transaction(TransactionOption.CreateOrJoin)]
        public Task<string[]> GetAllProducts();
    }
}
