// <copyright file="ICustomerActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain
{
    public interface ICustomerActor : IGrainWithStringKey
    {
        [Transaction(TransactionOption.Supported)]
        public Task<string?> GetName();

        [Transaction(TransactionOption.Supported)]
        public Task Create(string firstName, string? middleName, string lastName);
    }
}
