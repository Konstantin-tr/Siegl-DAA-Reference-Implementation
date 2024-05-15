// <copyright file="Extensions.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OnlineRetailSystem.Host.StartupTasks;
using Orleans.Configuration;
using Orleans.Storage;

namespace OnlineRetailSystem.Actors.Orleans
{
    public static class Extensions
    {
        public static IHostBuilder AddDistributedActorsArchitecture(this IHostBuilder host, string? dbConnection, bool? skipSeeding)
        {
            host.UseOrleans((_, siloBuilder) =>
            {
                siloBuilder.UseTransactions();

                if(skipSeeding is not true)
                {
                    siloBuilder.AddStartupTask<SeedDataTask>();
                }

                if (dbConnection is not null)
                {
                    siloBuilder.AddAdoNetGrainStorage("online-retail", new Action<AdoNetGrainStorageOptions>(a =>
                    {
                        a.Invariant = "Npgsql";
                        a.ConnectionString = dbConnection;
                    }));

                    siloBuilder.UseAdoNetClustering(a =>
                    {
                        a.Invariant = "Npgsql";
                        a.ConnectionString = dbConnection;
                    });

                    return;
                }

                siloBuilder
                .UseLocalhostClustering()
                .UseTransactions()
                .AddMemoryGrainStorage("online-retail");

            });

            return host;
        }
    }
}
