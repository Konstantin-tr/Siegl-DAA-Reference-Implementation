// <copyright file="Extensions.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.Extensions.Hosting;
using OnlineRetailSystem.Host.StartupTasks;

namespace OnlineRetailSystem.Actors.Orleans
{
    public static class Extensions
    {
        public static IHostBuilder AddDistributedActorsArchitecture(this IHostBuilder host)
        {
            host.UseOrleans((_, siloBuilder) =>
            {

                siloBuilder
                .UseLocalhostClustering()
                .UseTransactions()
                .AddMemoryGrainStorage("online-retail")
                .AddStartupTask<SeedDataTask>();

            });

            return host;
        }
    }
}
