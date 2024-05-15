// <copyright file="SeedDataTask.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Bogus;
using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.Demo;
using Orleans.Runtime;

namespace OnlineRetailSystem.Host.StartupTasks
{
    public class SeedDataTask(IGrainFactory grainFactory) : IStartupTask
    {
        async Task IStartupTask.Execute(CancellationToken cancellationToken)
        {
            var cartId = Guid.NewGuid().ToString();
            var customerId = Guid.NewGuid().ToString();

            await SeedCart(grainFactory, cartId, customerId);
            await SeedCustomer(grainFactory, customerId);
            await SeedProducts(grainFactory);
        }

        private static async Task SeedCart(IGrainFactory grainFactory, string cartId, string customerId)
        {
            var repo = grainFactory.GetGrain<IShoppingCartRepositoryActor>(0);

            await repo.Add(cartId, customerId);
        }

        private static async Task SeedCustomer(IGrainFactory grainFactory, string customerId)
        {
            var customer = grainFactory.GetGrain<ICustomerActor>(customerId);

            var nameFaker = new Faker();

            var firstName = nameFaker.Name.FirstName();
            var lastName = nameFaker.Name.LastName();

            await customer.Create(firstName, null, lastName);
        }

        private async Task SeedProducts(IGrainFactory grainFactory)
        {
            var productFaker = GetBogusProductFaker();

            var tasks = new List<Task>();

            var prods = productFaker.Generate(100);

            foreach (var product in prods)
            {
                var productGrain = grainFactory.GetGrain<ICreateProductCommandActor>(0);

                tasks.Add(productGrain.Execute(product.Name, product.Description, (decimal)product.PriceInEuro / 100, product.AvailableStock));
            }

            await Task.WhenAll(tasks);
        }

        private Faker<ProductCreationData> GetBogusProductFaker() =>
                new Faker<ProductCreationData>()
        .StrictMode(true)
        .RuleFor(p => p.Name, (f, p) => f.Commerce.ProductName())
        .RuleFor(p => p.Description, (f, p) => f.Commerce.ProductDescription())
        .RuleFor(p => p.AvailableStock, (f, p) => f.Random.Number(20, 300))
        .RuleFor(p => p.PriceInEuro, (f, p) => f.Random.Number(1, 100 * 299));

        private class ProductCreationData
        {
            public required string Name { get; set; }

            public required string Description { get; set; }

            public int AvailableStock { get; set; }

            public int PriceInEuro { get; set; }
        }
    }
}
