// <copyright file="IViewShoppingCartQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface IViewShoppingCartQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        [ReadOnly]
        public Task<ViewShoppingCartQueryResponse> Execute(string shoppingCartId);
    }

    [GenerateSerializer]
    public record ViewShoppingCartQueryResponse(ShoppingCartInfo? ShoppingCart);

    [GenerateSerializer]
    public record ShoppingCartInfo(decimal TotalPriceInEuro, ShoppingCartItemInfo[] Items);

    [GenerateSerializer]
    public record ShoppingCartItemInfo(string ItemdId, string ProductId, string ProductName, int Quantity, decimal PriceInEuro);
}
