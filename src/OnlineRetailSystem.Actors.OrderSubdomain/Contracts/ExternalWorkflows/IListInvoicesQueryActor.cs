// <copyright file="IListInvoicesQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface IListInvoicesQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        [ReadOnly]
        public Task<ListInvoicesQueryResponse> Execute();
    }

    [GenerateSerializer]
    public record ListInvoicesQueryResponse(ListInvoicesData[] Invoices);

    [GenerateSerializer]
    public record ListInvoicesData(string InvoiceId, string OrderId, string CustomerName, DateTime CreationDate, decimal TotalOrderPriceInEuro, bool IsPaid, ListInvoicesItemData[] Items);

    [GenerateSerializer]
    public record ListInvoicesItemData(string ItemdId, string ProductId, string ProductName, int Quantity, decimal PriceInEuro);
}
