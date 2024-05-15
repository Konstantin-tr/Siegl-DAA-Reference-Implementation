// <copyright file="GenerateSalesReportQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;
using System.Text;
using static OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows.IGenerateSalesReportQueryActor;

namespace OnlineRetailSystem.Actors.OrderSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    public class GenerateSalesReportQueryActor : Grain, IGenerateSalesReportQueryActor
    {
        public async Task<GenerateSalesReportQueryResponse> Execute(DateTime startDate, DateTime endDate)
        {
            var orders = await GetOrders(startDate, endDate);

            var productsData = await GetProductsSummary(orders);

            var bytes = GetCsvFile(productsData);

            return new(bytes);
        }

        private static byte[] GetCsvFile(Dictionary<string, (string name, string id, int price, int quantity, int saleTotal)> productsData)
        {
            var csv = new StringBuilder();

            csv.AppendLine("Product;Id;Price;Quantity;SaleTotal;");

            foreach (var product in productsData.Values)
            {
                csv.AppendLine($"{product.name};{product.id};{(decimal)product.price / 100};{product.quantity};{(decimal)product.saleTotal / 100};");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return bytes;
        }

        private async Task<Dictionary<string, (string name, string id, int price, int quantity, int saleTotal)>> GetProductsSummary(IEnumerable<InvoiceInfo> filteredOrders)
        {
            var productQuery = GrainFactory.GetGrain<IGetProductInformationQueryActor>(0);

            var invoiceWithProducts = await Task.WhenAll(filteredOrders.Select(async (invoice) =>
            {
                return await ListInvoicesBaseActor.GetProducts(productQuery, invoice);
            }));

            var productsData = invoiceWithProducts.SelectMany(p => p).Aggregate(new Dictionary<string, (string name, string id, int price, int quantity, int saleTotal)>(), (agg, curr) =>
            {
                if (agg.TryGetValue(curr.ProductId, out var product))
                {
                    product.quantity += curr.Quantity;
                    product.saleTotal += int.CreateTruncating(curr.PriceInEuro * 100) * curr.Quantity;

                    agg[curr.ProductId] = product;

                    return agg;
                }

                agg.Add(curr.ProductId, (
                    curr.ProductName,
                    curr.ProductId,
                    int.CreateTruncating(curr.PriceInEuro * 100),
                    curr.Quantity,
                    int.CreateTruncating(curr.PriceInEuro * 100) * curr.Quantity)
                    );

                return agg;
            });
            return productsData;
        }

        private async Task<IEnumerable<InvoiceInfo>> GetOrders(DateTime startDate, DateTime endDate)
        {
            var orderIds = await GrainFactory.GetGrain<IOrderRepository>(0).GetAllOrders();

            var orders = await Task.WhenAll(orderIds.Select(async (o) =>
            {
                var invoice = await GrainFactory.GetGrain<IOrderActor>(o.orderId).GetInvoice();

                if (invoice is null || invoice.CreationDate < startDate || invoice.CreationDate > endDate)
                {
                    return null;
                }

                return invoice;
            }));

            return orders.Where(x => x is not null).Cast<InvoiceInfo>();
        }
    }
}
