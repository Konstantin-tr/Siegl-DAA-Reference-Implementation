// <copyright file="GetCustomerNameQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Actors.ExternalWorkflows
{
    [StatelessWorker]
    internal class GetCustomerNameQueryActor : Grain, IGetCustomerNameQueryActor
    {
        async Task<GetCustomerNameQueryResponse> IGetCustomerNameQueryActor.Execute(string customerId)
        {
            var customer = GrainFactory.GetGrain<ICustomerActor>(customerId);

            var name = await customer.GetName();

            if (name is null)
            {
                return new(null);
            }

            return new(new(name));
        }
    }
}
