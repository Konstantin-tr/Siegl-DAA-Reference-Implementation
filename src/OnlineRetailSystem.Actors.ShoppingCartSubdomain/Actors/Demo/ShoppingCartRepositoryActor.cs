// <copyright file="ShoppingCartRepositoryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.CodeAnalysis;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.Demo;
using Orleans.Runtime;
using System.Collections.Immutable;

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Actors.Demo
{

    public class ShoppingCartRepositoryActor
   (
       [PersistentState(stateName: "shopping-cart-repository", storageName: "online-retail")]
        IPersistentState<ShoppingCartRepositoryState> repo
   ) : Grain, IShoppingCartRepositoryActor
    {
        private readonly IPersistentState<ShoppingCartRepositoryState> _repo = repo;

        public async Task Add(string id, string customerId)
        {
            var state = _repo.State;

            if (state.Carts.ContainsKey(id))
            {
                return;
            }

            _repo.State = state with
            {
                Carts = state.Carts.Add(id, new(id, customerId))
            };

            await _repo.WriteStateAsync();
        }

        public Task<ShoppingCartInfo[]> GetIds()
        {
            return Task.FromResult(_repo.State.Carts.Values.Select(v => new ShoppingCartInfo(v.CartId, v.CustomerId)).ToArray());
        }
    }

    [Serializable]
    public record ShoppingCartRepositoryState(ImmutableDictionary<string, ShoppingCartRepositoryItem> Carts)
    {
        public ShoppingCartRepositoryState() : this(ImmutableDictionary<string, ShoppingCartRepositoryItem>.Empty)
        {
        }
    };

    [Serializable]
    public record ShoppingCartRepositoryItem(string CartId, string CustomerId);
}


