// <copyright file="Unit.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Zentient.Abstractions
{
    /// <summary>
    /// Represents a type with a single value, used to indicate the absence of a specific value
    /// in generic contexts, similar to <c>void</c> or <c>null</c> but as a concrete type.
    /// </summary>
    /// <remarks>
    /// This type is commonly used in functional programming paradigms and can be useful
    /// when a generic type parameter is required but no meaningful value needs to be conveyed.
    /// </remarks>
    [DataContract]
    [JsonConverter(typeof(Serialization.UnitJsonConverter))]
    public readonly struct Unit : IEquatable<Unit>, IComparable, IComparable<Unit>
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="Unit"/> type.
        /// </summary>
        public static Unit Value
        {
            get
            {
                _value ??= new Unit();
                return _value.Value;
            }
        }
        private static Unit? _value;

        /// <summary>
        /// Compares two <see cref="Unit"/> instances for less than. Always returns <c>false</c>.
        /// </summary>
        /// <param name="left">The left <see cref="Unit"/> instance.</param>
        /// <param name="right">The right <see cref="Unit"/> instance.</param>
        /// <returns><c>false</c>.</returns>
        public static bool operator <(Unit left, Unit right)
        {
            _ = left;
            _ = right;
            return false;
        }

        /// <summary>
        /// Compares two <see cref="Unit"/> instances for greater than. Always returns <c>false</c>.
        /// </summary>
        /// <param name="left">The left <see cref="Unit"/> instance.</param>
        /// <param name="right">The right <see cref="Unit"/> instance.</param>
        /// <returns><c>false</c>.</returns>
        public static bool operator >(Unit left, Unit right)
        {
            _ = left;
            _ = right;
            return false;
        }

        /// <summary>
        /// Compares two <see cref="Unit"/> instances for less than or equal to. Always returns <c>true</c>.
        /// </summary>
        /// <param name="left">The left <see cref="Unit"/> instance.</param>
        /// <param name="right">The right <see cref="Unit"/> instance.</param>
        /// <returns><c>true</c>.</returns>
        public static bool operator <=(Unit left, Unit right)
        {
            _ = left;
            _ = right;
            return true;
        }

        /// <summary>
        /// Compares two <see cref="Unit"/> instances for greater than or equal to. Always returns <c>true</c>.
        /// </summary>
        /// <param name="left">The left <see cref="Unit"/> instance.</param>
        /// <param name="right">The right <see cref="Unit"/> instance.</param>
        /// <returns><c>true</c>.</returns>
        public static bool operator >=(Unit left, Unit right)
        {
            _ = left;
            _ = right;
            return true;
        }

        /// <summary>
        /// Compares two <see cref="Unit"/> instances for equality. Always returns <c>true</c>.
        /// </summary>
        /// <param name="left">The left <see cref="Unit"/> instance.</param>
        /// <param name="right">The right <see cref="Unit"/> instance.</param>
        /// <returns><c>true</c>.</returns>
        public static bool operator ==(Unit left, Unit right)
        => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="Unit"/> instances for inequality. Always returns <c>false</c>.
        /// </summary>
        /// <param name="left">The left <see cref="Unit"/> instance.</param>
        /// <param name="right">The right <see cref="Unit"/> instance.</param>
        /// <returns><c>false</c>.</returns>
        public static bool operator !=(Unit left, Unit right)
        => !(left == right);

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified object is a <see cref="Unit"/> instance; otherwise, <c>false</c>.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Unit;

        /// <summary>
        /// Determines whether this instance and another specified <see cref="Unit"/> instance are equal.
        /// </summary>
        /// <param name="other">The <see cref="Unit"/> instance to compare with the current instance.</param>
        /// <returns><c>true</c>.</returns>
        public bool Equals(Unit other)
        => true;

        /// <summary>
        /// Returns the hash code for this instance. Always returns 0.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        => 0;

        /// <summary>
        /// Returns the string representation of the <see cref="Unit"/> instance.
        /// </summary>
        /// <returns>The string "Unit".</returns>
        public override string ToString()
        => "Unit";

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// <list type="table">
        /// <listheader><term>Value</term><description>Meaning</description></listheader>
        /// <item><term>Less than zero</term><description>This instance precedes <paramref name="other"/> in the sort order.</description></item>
        /// <item><term>Zero</term><description>This instance occurs in the same position in the sort order as <paramref name="other"/>.</description></item>
        /// <item><term>Greater than zero</term><description>This instance follows <paramref name="other"/> in the sort order.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="other"/> is not of type <see cref="Unit"/>.
        /// </exception>
        public int CompareTo(object? other)
        {
            if (other == null)
            {
                return 1;
            }

            if (other is Unit unit)
            {
                return this.CompareTo(unit);
            }

            throw new ArgumentException($"Object must be of type {nameof(Unit)}.", nameof(other));
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Unit"/> instance. Always returns 0.
        /// </summary>
        /// <param name="other">A <see cref="Unit"/> instance to compare with this instance.</param>
        /// <returns>0.</returns>
        public int CompareTo(Unit other) => 0;
    }
}
