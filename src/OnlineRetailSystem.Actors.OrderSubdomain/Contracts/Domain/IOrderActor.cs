// <copyright file="IOrderActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain
{
    public interface IOrderActor : IGrainWithStringKey
    {
        [Transaction(TransactionOption.Join)]
        [ReadOnly]
        public Task<InvoiceInfo?> GetInvoice();

        [Transaction(TransactionOption.Join)]
        public Task<CommandResponse> Create(DateTime creationDate, OrderItemData[] items);
    }

    [GenerateSerializer]
    public record InvoiceInfo(string InvoiceId, DateTime CreationDate, decimal TotalOrderPriceInEuro, bool IsPaid, InvoiceItemInfo[] items);

    [GenerateSerializer]
    public record InvoiceItemInfo(string ItemId, string ProductId, decimal priceInEuro, int Quantity);

    [GenerateSerializer]
    public record OrderItemData(string ItemId, string ProductId, decimal PriceInEuro, int Quantity);
}
