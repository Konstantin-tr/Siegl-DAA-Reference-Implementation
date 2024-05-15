// <copyright file="IListOwnInvoicesQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface IListOwnInvoicesQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        [ReadOnly]
        public Task<ListInvoicesQueryResponse> Execute(string customerId);
    }
}
