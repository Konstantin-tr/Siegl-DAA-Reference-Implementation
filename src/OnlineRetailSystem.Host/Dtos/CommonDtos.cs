// <copyright file="CommonDtos.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Host.Dtos
{
    public record CommandResponseDto(bool isSuccess, string? Message);

    public record CommandResponseResultDto<T>(bool isSuccess, T? Result, string? Message);
}
