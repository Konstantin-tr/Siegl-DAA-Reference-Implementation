// <copyright file="ListInvoicesBaseActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    internal abstract class ListInvoicesBaseActor : Grain
    {
        protected async Task<ListInvoicesQueryResponse> GetInvoicesFromOrders(IEnumerable<(string orderId, string customerId)> orders)
        {
            var invoices = await Task.WhenAll(orders.Select(async o =>
            {
                return await GetInvoiceData(o);
            }));

            var fullInvoices = invoices.Where(x => x is not null).Cast<ListInvoicesData>().ToArray();

            return new(fullInvoices);
        }



        private async Task<ListInvoicesData?> GetInvoiceData((string orderId, string customerId) o)
        {
            var productQuery = GrainFactory.GetGrain<IGetProductInformationQueryActor>(0);
            var nameQuery = GrainFactory.GetGrain<IGetCustomerNameQueryActor>(0);

            var invoice = await GetInvoice(o);

            if (invoice is null)
            {
                return null;
            }

            string? name = null;
            ListInvoicesItemData[] result = [];

            var getProducts = async () =>
            {
                result = await GetProducts(productQuery, invoice);
            };

            var getName = async () =>
            {
                var response = await nameQuery.Execute(o.customerId);

                name = response.Name;
            };

            await Task.WhenAll(getProducts(), getName());

            if (result.Length == 0 || name is null)
            {
                return null;
            }

            return new ListInvoicesData(invoice.InvoiceId, o.orderId, name, invoice.CreationDate, invoice.TotalOrderPriceInEuro, invoice.IsPaid, result);
        }

        private async Task<InvoiceInfo?> GetInvoice((string orderId, string customerId) o)
        {
            var order = GrainFactory.GetGrain<IOrderActor>(o.orderId);

            return await order.GetInvoice();
        }

        public static async Task<ListInvoicesItemData[]> GetProducts(IGetProductInformationQueryActor productQuery, InvoiceInfo invoice)
        {
            var productInfos = await Task.WhenAll(
                invoice.items.Select(async item =>
                    (
                    item,
                    product: await productQuery.Execute(item.ProductId)
                    )
                ));

            return productInfos
                .Select(p => (p.item, product: p.product.Product))
                .Where(p => p.product is not null)
                .Cast<(InvoiceItemInfo item, ProductResult product)>()
                .Select(p =>
                    new ListInvoicesItemData(
                        p.item.ItemId,
                        p.product.ProductId,
                        p.product.ProductName,
                        p.item.Quantity,
                        p.item.priceInEuro
                    )
                )
                .ToArray();
        }
    }
}
