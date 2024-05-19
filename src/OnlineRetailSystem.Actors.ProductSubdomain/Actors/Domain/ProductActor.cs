// <copyright file="ProductActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.Extensions.Logging;
using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.Domain;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.Domain
{
    [Reentrant]
    internal class ProductActor
    (
        [TransactionalState(stateName: "product", storageName: "online-retail")]
        ITransactionalState<ProductData> data,
        ILogger<ProductActor> logger,
        ILocalSiloDetails siloInfo
    ) : Grain, IProductActor
    {
        private readonly ITransactionalState<ProductData> _data = data;

        public Task<ProductInfo?> GetData()
        {
            logger.LogInformation("Getting product data for product {0} from silo {1}", this.GetGrainId().Key.ToString(), siloInfo.Name);

            return _data.PerformRead((state) =>
            {
                if (!state.IsCreated || state.IsDeleted)
                {
                    return null;
                }

                var info = new ProductInfo(state.Name, state.Description, (decimal)state.Price / 100, state.Stock);

                return info;
            });
        }

        public async Task<CommandResponse> Create(string productName, string productDescription, decimal priceInEuro, int availableStock)
        {
            return await UpdateState(productName, productDescription, priceInEuro, availableStock, false, "Cannot create existing product.");
        }

        public Task<CommandResponse> Delete()
        {
            return _data.PerformUpdate(s =>
            {

                if (!s.IsCreated)
                {
                    throw new ActorCommandException("Cannot delete non-existing product.");
                }

                if (s.IsDeleted)
                {
                    throw new ActorCommandException("Cannot delete already deleted product.");
                }

                s.IsDeleted = true;

                return new CommandResponse(true, null);
            });
        }

        public async Task<int?> GetStock()
        {
            var (isCreated, isDeleted, stock) = await _data.PerformRead(s => (s.IsCreated, s.IsDeleted, s.Stock));

            return isCreated && !isDeleted ? stock : null;
        }

        public async Task<CommandResponse> Update(string productName, string productDescription, decimal priceInEuro, int availableStock)
        {
            return await UpdateState(productName, productDescription, priceInEuro, availableStock, true, "Cannot update non-existing product.");
        }

        private Task<CommandResponse> UpdateState(string productName, string productDescription, decimal priceInEuro, int availableStock, bool shouldBeCreated, string createdError)
        {
            return _data.PerformUpdate(data =>
            {
                if (data.IsCreated != shouldBeCreated)
                {
                    throw new ActorCommandException(createdError);
                }

                if (data.IsDeleted)
                {
                    throw new ActorCommandException("Cannot modify deleted product.");
                }

                var priceInCents = priceInEuro * 100;

                var priceInt = int.CreateTruncating(priceInCents);

                var validation = ValidatInput(priceInCents, priceInt, availableStock);

                if (validation.Length > 0)
                {
                    throw new ActorCommandException(string.Join(' ', validation));
                }

                data.Name = productName;
                data.Description = productDescription;
                data.Stock = availableStock;
                data.Price = priceInt;
                data.IsCreated = true;

                return new CommandResponse(true, null);
            });
        }

        private static string[] ValidatInput(decimal priceInCents, int priceInt, int availableStock)
        {
            var errors = new List<string>(3);

            if (priceInCents < 0)
            {
                errors.Add("Cannot have negative price.");
            }

            if (priceInt != priceInCents)
            {
                errors.Add("Price is not a valid euro value.");
            }

            if (availableStock < 0)
            {
                errors.Add("Cannot have negative available stock.");
            }

            return errors.ToArray();
        }

        public Task<CommandResponse> ReduceStock(int amount)
        {
            return _data.PerformUpdate(data =>
            {
                if (!data.IsCreated)
                {
                    throw new ActorCommandException("Product does not exist.");
                }

                if (data.IsDeleted)
                {
                    throw new ActorCommandException("Product is deleted.");
                }

                var currentAmount = data.Stock;

                if (currentAmount < amount)
                {
                    throw new ActorCommandException("Available product stock is not high enough.");
                }

                data.Stock -= amount;

                return new CommandResponse(true, null);
            });
        }
    }

    [GenerateSerializer]
    public class ProductData
    {
        [Id(0)]
        public bool IsCreated { get; set; }

        [Id(1)]
        public string Name { get; set; } = string.Empty;

        [Id(2)]
        public string Description { get; set; } = string.Empty;

        [Id(3)]
        public int Stock { get; set; }

        [Id(4)]
        public int Price { get; set; }

        [Id(5)]
        public bool IsDeleted { get; set; }
    }
}
