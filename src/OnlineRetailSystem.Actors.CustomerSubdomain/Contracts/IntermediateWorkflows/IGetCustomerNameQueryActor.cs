// <copyright file="IGetCustomerNameQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows
{
    public interface IGetCustomerNameQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Supported)]
        public Task<GetCustomerNameQueryResponse> Execute(string customerId);
    }

    [GenerateSerializer]
    public record GetCustomerNameQueryResponse(string? Name);
}
