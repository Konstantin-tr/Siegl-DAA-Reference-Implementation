// <copyright file="ActorCommandException.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Actors.Core
{
    [GenerateSerializer]
    public class ActorCommandException : Exception
    {

        public ActorCommandException(string message) : base(message)
        {
        }
    }
}
