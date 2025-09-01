// <copyright file="src/Zentient.Analyzers/Abstractions/IEnumerableExtensions.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Zentient.Analyzers.Extensions
{
    internal static class EnumerableExtensions
    {
#if NETSTANDARD2_0
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            if (source is null)
            {
                return true;
            }
            using var enumerator = source.GetEnumerator();
            return !enumerator.MoveNext();
        }

        // ToHashSet is not available in .NET Standard 2.0

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return new HashSet<T>(source, comparer);
        }
#endif
    }
}
