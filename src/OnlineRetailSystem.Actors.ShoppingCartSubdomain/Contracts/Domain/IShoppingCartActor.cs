// <copyright file="IShoppingCartActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain
{
    internal interface IShoppingCartActor : IGrainWithStringKey
    {
        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse<ShoppingCartAddProductResponse>> AddProduct(string productId, int quantity);

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> UpdateProductQuantity(string productId, int quantity);

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> RemoveProduct(string productId);

        [Transaction(TransactionOption.Join)]
        [ReadOnly]
        public Task<ShoppingCartItemDomainData[]> GetItems();

        [Transaction(TransactionOption.Join)]
        public Task Clear();
    }

    [GenerateSerializer]
    internal record ShoppingCartItemDomainData(string ItemId, string ProductId, int Quantity);


    [GenerateSerializer]
    internal record ShoppingCartAddProductResponse(string ItemId);
}
