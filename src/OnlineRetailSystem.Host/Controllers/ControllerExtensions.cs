// <copyright file="ControllerExtensions.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.AspNetCore.Mvc;
using OnlineRetailSystem.Actors.Core;
using OnlineRetailSystem.Host.Dtos;
using Orleans.Transactions;

namespace OnlineRetailSystem.Host.Controllers
{
    public static class ControllerExtensions
    {
        public static async Task<ActionResult<CommandResponseDto>> ExecuteCommand(this ControllerBase _, Func<Task<CommandResponseDto>> func)
        {
            try
            {
                return await func();
            }
            catch (OrleansTransactionException ex)
            {
                if (ex.InnerException is not ActorCommandException actEx)
                {
                    throw new InvalidOperationException("Unknown transaction abortion", ex);
                }

                return new CommandResponseDto(false, actEx.Message);
            }
        }

        public static async Task<ActionResult<CommandResponseResultDto<T>>> ExecuteCommand<T>(this ControllerBase _, Func<Task<CommandResponseResultDto<T>>> func)
        {
            try
            {
                return await func();
            }
            catch (OrleansTransactionException ex)
            {
                if (ex.InnerException is not ActorCommandException actEx || actEx.Message is null)
                {
                    throw new InvalidOperationException("Unknown transaction abortion", ex);
                }

                return new CommandResponseResultDto<T>(false, default, actEx.Message);
            }
        }
    }
}

