// <copyright file="IClearShoppingCartCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface IClearShoppingCartCommandActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> Execute(string shoppingCartId);
    }
}
