// <copyright file="IShoppingCartRepositoryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.Demo
{
    public interface IShoppingCartRepositoryActor : IGrainWithIntegerKey
    {
        [ReadOnly]
        public Task<ShoppingCartInfo[]> GetIds();

        public Task Add(string id, string customerId);
    }

    [GenerateSerializer]
    public record ShoppingCartInfo(string Id, string CustomerId);
}
