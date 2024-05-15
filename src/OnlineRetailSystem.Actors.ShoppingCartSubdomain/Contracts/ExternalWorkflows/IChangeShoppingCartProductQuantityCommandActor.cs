// <copyright file="IChangeShoppingCartProductQuantityCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using System.Transactions;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{

    public interface IChangeShoppingCartProductQuantityCommandActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        public Task<CommandResponse> Execute(string shoppingCartId, string productId, int quantity);
    }
}
