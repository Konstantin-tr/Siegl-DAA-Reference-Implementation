// <copyright file="IDeleteProductCommandActor.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using OnlineRetailSystem.Actors.Core;

namespace OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows
{
    public interface IDeleteProductCommandActor : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        public Task<CommandResponse> Execute(string productId);
    }
}
