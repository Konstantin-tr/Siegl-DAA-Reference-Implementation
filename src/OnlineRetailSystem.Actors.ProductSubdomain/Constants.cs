// <copyright file="Constants.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Actors.ProductSubdomain
{
    public static class Constants
    {
        public const string ORDER_NAME = "name";
        public const string ORDER_PRICE = "price";

        public readonly static string[] ORDER_OPTIONS = [ORDER_NAME, ORDER_PRICE];
    }
}
