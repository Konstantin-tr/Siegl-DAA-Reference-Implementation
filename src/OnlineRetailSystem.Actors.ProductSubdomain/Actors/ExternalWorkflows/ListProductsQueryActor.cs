// <copyright file="ListProductsQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class ListProductsQueryActor : Grain, IListProductsQueryActor
    {
        public async Task<ListProductsResponse> Execute(string? nameFilter, string? orderBy, bool? orderDescending)
        {
            var order = orderBy is null || !Constants.ORDER_OPTIONS.Contains(orderBy) ? Constants.ORDER_NAME : orderBy;

            var orderDesc = orderDescending == true;

            var productRepo = GrainFactory.GetGrain<IProductRepository>(0);

            var productIds = await productRepo.GetAllProducts();

            var tasks = productIds.Select(async (id) =>
            {
                var data = await GrainFactory.GetGrain<IProductActor>(id).GetData();

                if (data is null || (nameFilter is not null && !data.ProductName.ToLower().Contains(nameFilter.ToLower())))
                {
                    return null;
                }

                return new ProductListData(id, data.ProductName, data.PriceInEuro, data.AvailableStock);
            });

            var products = await Task.WhenAll(tasks);

            var filteredProducts = products.Where(x => x is not null).Cast<ProductListData>();

            var orderedProducts = order switch
            {
                Constants.ORDER_NAME => filteredProducts.DirectionalOrderBy(orderDesc, p => p.ProductName),
                Constants.ORDER_PRICE => filteredProducts.DirectionalOrderBy(orderDesc, p => p.PriceInEuro),
                _ => filteredProducts
            };

            return new(orderedProducts.ToArray());
        }
    }
}
