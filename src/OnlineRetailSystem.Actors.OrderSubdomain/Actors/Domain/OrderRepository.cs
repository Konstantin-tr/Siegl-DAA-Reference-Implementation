// <copyright file="OrderRepository.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using System.Collections.Immutable;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.Domain
{
    [Reentrant]
    internal class OrderRepository
    (
        [TransactionalState(stateName: "order-repository", storageName: "online-retail")]
        ITransactionalState<RepositoryData> state
    ) : Grain, IOrderRepository
    {
        private readonly ITransactionalState<RepositoryData> _state = state;

        public Task AddOrder(string orderId, string customerId)
        {
            return _state.PerformUpdate(state =>
            {
                if (state.Orders.Any(s => s.orderId == orderId))
                {
                    return;
                }

                state.Orders = state.Orders.Add((orderId, customerId));
            });
        }

        public Task<(string orderId, string customerId)[]> GetAllOrders()
        {
            return _state.PerformRead(state => state.Orders.ToArray());
        }
    }

    [GenerateSerializer]
    internal class RepositoryData
    {
        [Id(0)]
        public ImmutableList<(string orderId, string customerId)> Orders
        {
            get;
            set;
        } = ImmutableList<(string orderId, string customerId)>.Empty;
    }
}
