// <copyright file="CommandResponse.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Actors.Core
{
    [GenerateSerializer]
    public record CommandResponse(bool IsSuccess, string? Message);


    [GenerateSerializer]
    public record CommandResponse<T>(bool IsSuccess, T? Result, string? Message);
}
