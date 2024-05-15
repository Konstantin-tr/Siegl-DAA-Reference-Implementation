// <copyright file="CustomerActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.CustomerSubdomain.Contracts.Domain;
using Orleans.Runtime;

namespace OnlineRetailSystem.Actors.CustomerSubdomain.Actors.Domain
{

    public class CustomerActor
   (
       [PersistentState(stateName: "customer", storageName: "online-retail")]
        IPersistentState<CustomerData> data
   ) : Grain, ICustomerActor
    {
        private readonly IPersistentState<CustomerData> _data = data;

        public Task<string?> GetName()
        {
            return _data.RecordExists ? Task.FromResult<string?>(BuildName(_data.State)) : Task.FromResult<string?>(null);
        }

        private static string BuildName(CustomerData data)
        {
            var middleName = data.MiddleName is not null ? $" {data.MiddleName} " : " ";

            return $"{data.FirstName}{middleName}{data.LastName}";
        }

        public async Task Create(string firstName, string? middleName, string lastName)
        {
            _data.State = new(firstName, middleName, lastName);

            await _data.WriteStateAsync();
        }
    }

    [Serializable]
    public record CustomerData(string FirstName, string? MiddleName, string LastName);
}
