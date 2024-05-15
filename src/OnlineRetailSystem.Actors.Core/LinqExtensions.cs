// <copyright file="LinqExtensions.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static IOrderedEnumerable<T> DirectionalOrderBy<T, TKey>(this IEnumerable<T> ts, bool orderDescending, Func<T, TKey> keySelector)
        {
            return orderDescending ? ts.OrderByDescending(keySelector) : ts.OrderBy(keySelector);
        }
    }
}
