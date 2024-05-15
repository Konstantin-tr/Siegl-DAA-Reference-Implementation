// <copyright file="ICreateOrderFromShoppingCartCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface ICreateOrderFromShoppingCartCommandActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        public Task<CommandResponse<CreateOrderFromShoppingCartResponse>> Execute(string shoppingCartId, string customerId);
    }

    [GenerateSerializer]
    public record CreateOrderFromShoppingCartResponse(string OrderId);
}
