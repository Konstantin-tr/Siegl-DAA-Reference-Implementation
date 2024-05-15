// <copyright file="IGetShoppingCartItemsQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface IGetShoppingCartItemsQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        [ReadOnly]
        public Task<GetShoppingCartItemsQueryResponse> Execute(string shoppingCartId);
    }

    [GenerateSerializer]
    public record GetShoppingCartItemsQueryResponse(ShoppingCartItemData[]? Items);

    [GenerateSerializer]
    public record ShoppingCartItemData(string ItemdId, string ProductId, int Quantity);
}
