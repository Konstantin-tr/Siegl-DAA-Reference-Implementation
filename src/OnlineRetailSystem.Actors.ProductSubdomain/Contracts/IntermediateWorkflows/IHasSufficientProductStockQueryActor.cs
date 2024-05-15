// <copyright file="IHasSufficientProductStockQueryActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Orleans.Concurrency;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.IntermediateWorkflows
{

    public interface IHasSufficientProductStockQueryActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Join)]
        [ReadOnly]
        public Task<HasSufficientProductStockQueryResult> Execute(string productId, int amount);
    }

    [GenerateSerializer]
    public record HasSufficientProductStockQueryResult(ResultValue? Result);

    [GenerateSerializer]
    public record ResultValue(bool HasSufficientStock);
}
