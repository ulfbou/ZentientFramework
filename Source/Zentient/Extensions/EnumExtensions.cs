//
// File: EnumExtensions.cs
//
// Description: Provides extension methods for working with enums.
//
// MIT License
//
// Copyright (c) 2024 Ulf Bourelius
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Zentient.Extensions
{
    /// <summary>
    /// Provides extension methods for working with enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the name of the specified enum value as a string.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The name of the enum value as a string, or null if the enum value is invalid.</returns>
        public static string GetName<T>(this T value) where T : Enum => Enum.GetName(typeof(T), value)!;

        /// <summary>
        /// Attempts to parse a string representation of an enum value into an enum value of the specified type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumRepresentation">The string representation of the enum value.</param>
        /// <returns>
        /// The enum value of type <typeparamref name="T"/> if parsing succeeds; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This method returns null if the parsing fails. To determine whether parsing was successful, 
        /// use the nullable result or check for null.
        /// </remarks>
        public static T? TryParse<T>(this string enumRepresentation) where T : struct, Enum
        {
            return Enum.TryParse(enumRepresentation, out T result) ? result : null;
        }

        /// <summary>
        /// Determines whether the current enum value includes the specified flag.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if the enum value includes the specified flag; otherwise, false.</returns>
        public static bool HasFlag<T>(this T enumValue, T flag) where T : Enum
        {
            int enumIntValue = Convert.ToInt32(enumValue);
            int flagIntValue = Convert.ToInt32(flag);
            return (enumIntValue & flagIntValue) == flagIntValue;
        }

        /// <summary>
        /// Retrieves the display name of the specified enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The display name of the enum value.</returns>
        public static string GetDisplayName<T>(this T enumValue) where T : Enum
        {
            MemberInfo memberInfo = typeof(T).GetMember(enumValue.ToString())[0];
            DisplayAttribute displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? enumValue.ToString();
        }

        /// <summary>
        /// Retrieves the description of the specified enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The description of the enum value.</returns>
        public static string GetDescription<T>(this T enumValue) where T : Enum
        {
            MemberInfo memberInfo = typeof(T).GetMember(enumValue.ToString())[0];
            DescriptionAttribute descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? enumValue.ToString();
        }
    }

    public static class EnumHelper<T> where T : Enum
    {
        /// <summary>
        /// Retrieves an array containing all the values of the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>An array containing all the values of the enum type.</returns>
        public static IEnumerable<T> GetValues()
        {
            var iterator = Enum.GetValues(typeof(T)).GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return (T)iterator.Current;
            }
        }

        /// <summary>
        /// Retrieves an array containing all the names of the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>An array containing all the names of the enum type.</returns>
        public static IEnumerable<string> GetNames()
        {
            var values = EnumHelper<T>.GetValues();

            foreach (var value in values)
            {
                yield return value.GetName();
            }
        }
    }

    public static class EnumHelper
    {
        /// <summary>
        /// Retrieves an array containing all the values of the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>An array containing all the values of the enum type.</returns>
        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            var iterator = Enum.GetValues(typeof(T)).GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return (T)iterator.Current;
            }
        }

        /// <summary>
        /// Retrieves an array containing all the names of the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>An array containing all the names of the enum type.</returns>
        public static IEnumerable<string> GetNames<T>() where T : Enum
        {
            var values = EnumHelper<T>.GetValues();

            foreach (var value in values)
            {
                yield return value.GetName();
            }
        }
    }
}
