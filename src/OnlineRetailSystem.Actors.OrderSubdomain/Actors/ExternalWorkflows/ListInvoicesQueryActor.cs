// <copyright file="ListInvoicesQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class ListInvoicesQueryActor : ListInvoicesBaseActor, IListInvoicesQueryActor
    {
        public async Task<ListInvoicesQueryResponse> Execute()
        {
            var orders = await GetOrders();

            return await GetInvoicesFromOrders(orders);
        }

        private async Task<(string orderId, string customerId)[]> GetOrders()
        {
            var orderRepo = GrainFactory.GetGrain<IOrderRepository>(0);

            return await orderRepo.GetAllOrders();
        }
    }
}
