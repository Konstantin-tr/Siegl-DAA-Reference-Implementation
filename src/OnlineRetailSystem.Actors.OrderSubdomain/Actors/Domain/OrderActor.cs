// <copyright file="OrderActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using System.Collections.Immutable;

namespace OnlineRetailSystem.Actors.CustomerSubdomain.Actors.Domain
{
    [Reentrant]
    public class OrderActor
   (
       [TransactionalState(stateName: "order", storageName: "online-retail")]
        ITransactionalState<OrderState> order
   ) : Grain, IOrderActor
    {
        private readonly ITransactionalState<OrderState> _state = order;

        public async Task<CommandResponse> Create(DateTime creationDate, OrderItemData[] items)
        {
            return await _state.PerformUpdate<CommandResponse>((state) =>
            {
                if (state.IsCreated)
                {
                    throw new ActorCommandException("Order already exists");
                }

                var validation = ValidateInput(items);

                if (validation.Length > 0)
                {
                    throw new ActorCommandException(string.Join(' ', validation));
                }

                var orderItems = items.Select(i =>
                    new OrderItem
                    {
                        OrderItemId = i.ItemId,
                        ProductId = i.ProductId,
                        OrderPrice = int.CreateTruncating(i.PriceInEuro * 100),
                        Quantity = i.Quantity
                    }
                )
                .ToImmutableDictionary(s => s.OrderItemId);

                var totalPrice = orderItems.Values.Aggregate(0, (acc, curr) => acc + curr.OrderPrice * curr.Quantity);

                var invoice = new Invoice { InvoiceId = $"inv_{this.GetGrainId().Key}", CreationDate = creationDate, TotalPrice = totalPrice, IsPaid = false };

                state.Invoice = invoice;
                state.Items = orderItems;
                state.IsCreated = true;

                return new(true, null);
            });
        }

        public Task<InvoiceInfo?> GetInvoice()
        {
            return _state.PerformRead(state =>
            {
                if (!state.IsCreated)
                {
                    return null;
                }

                var invoice = state.Invoice;

                var items = state.Items.Values.Select(i => new InvoiceItemInfo(i.OrderItemId, i.ProductId, (decimal)i.OrderPrice / 100, i.Quantity)).ToArray();

                return new InvoiceInfo(invoice.InvoiceId, invoice.CreationDate, (decimal)invoice.TotalPrice / 100, invoice.IsPaid, items);
            });
        }

        private string[] ValidateInput(OrderItemData[] items)
        {
            var errors = new List<string>(10);

            if (items.Any(i => items.Count(x => x.ItemId == i.ItemId || x.ProductId == i.ProductId) > 1))
            {
                errors.Add("Duplicate items discovered in order");
            }

            if (items.Any(i => i.Quantity < 1))
            {
                errors.Add("Items with less than 1 quantity discovered in order");
            }

            if (items.Any(i => i.PriceInEuro < 0))
            {
                errors.Add("Items with negative price discovered in order");
            }

            return [.. errors];
        }
    }

    [GenerateSerializer]
    public class OrderState
    {
        [Id(0)]
        public bool IsCreated { get; set; }

        [Id(1)]
        public Invoice Invoice { get; set; } = new();

        [Id(2)]
        public ImmutableDictionary<string, OrderItem> Items { get; set; } = ImmutableDictionary<string, OrderItem>.Empty;
    }

    [GenerateSerializer]
    public class Invoice
    {
        [Id(0)]
        public string InvoiceId { get; set; } = string.Empty;

        [Id(1)]
        public DateTime CreationDate { get; set; }

        [Id(2)]
        public int TotalPrice { get; set; }

        [Id(3)]
        public bool IsPaid { get; set; }
    }

    [GenerateSerializer]
    public class OrderItem
    {
        [Id(0)]
        public string OrderItemId { get; set; } = string.Empty;

        [Id(1)]
        public string ProductId { get; set; } = string.Empty;

        [Id(2)]
        public int OrderPrice { get; set; }

        [Id(3)]
        public int Quantity { get; set; }
    }
}


