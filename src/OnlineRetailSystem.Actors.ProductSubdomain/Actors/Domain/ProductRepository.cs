// <copyright file="ProductRepository.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;
using System.Collections.Immutable;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.Domain
{
    [Reentrant]
    internal class ProductRepository
    (
        [TransactionalState(stateName: "product-repository", storageName: "online-retail")]
        ITransactionalState<RepositoryData> state
    ) : Grain, IProductRepository
    {
        private readonly ITransactionalState<RepositoryData> _state = state;

        public Task AddProduct(string productId)
        {
            return _state.PerformUpdate(state =>
            {
                if (state.ProductIds.Contains(productId))
                {
                    return;
                }

                state.ProductIds = state.ProductIds.Add(productId);
            });
        }

        public Task<string[]> GetAllProducts()
        {
            return _state.PerformRead(state => state.ProductIds.ToArray());
        }

        public Task RemoveProduct(string productId)
        {
            return _state.PerformUpdate(state =>
            {
                if (!state.ProductIds.Contains(productId))
                {
                    return;
                }

                state.ProductIds = state.ProductIds.Remove(productId);
            });
        }
    }

    [GenerateSerializer]
    internal class RepositoryData
    {
        [Id(0)]
        public ImmutableHashSet<string> ProductIds
        {
            get;
            set;
        } = ImmutableHashSet<string>.Empty;
    }
}
