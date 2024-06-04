//
// File: StringExtensions.cs
//
// Description: Provides extension methods for working with strings.
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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Zentient.Extensions
{
    /// <summary>
    /// Provides extension methods for working with IQueryables.
    /// </summary>

    /// <summary>
    /// Provides extension methods for working with strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary> Determines whether the string is null, empty, or consists only of white-space characters. </summary>
        /// <param name="value"> The string value to check. </param>
        /// <returns> True if the string is null, empty, or contains only white-space characters; otherwise, false. </returns>
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary> Converts the string to title case. </summary>
        /// <param name="value"> The string value to convert. </param>
        /// <returns> The string converted to title case. </returns>
        public static string ToTitleCase(this string value)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());

        /// <summary> Removes all white-space characters from the string. </summary>
        /// <param name="value"> The string value to process. </param>
        /// <returns> The string with all white-space characters removed. </returns>
        public static string RemoveWhitespace(this string value)
            => new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray());

        /// <summary> Checks if the string is a valid email address. </summary>
        /// <param name="value"> The string value to check. </param>
        /// <returns> True if the string is a valid email address; otherwise, false. </returns>
        public static bool IsEmail(this string value)
            => Regex.IsMatch(value,
                @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)[a-z0-9]+(?<!\.)\.)[a-z0-9]+(?<!\.))$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary> Checks if the string represents a numeric value. </summary>
        /// <param name="value"> The string value to check. </param>
        /// <returns> True if the string represents a numeric value; otherwise, false. </returns>
        public static bool IsNumeric(this string value) => double.TryParse(value, out _);

        /// <summary> Truncates the string to a specified length. </summary>
        /// <param name="value"> The string value to truncate. </param>
        /// <param name="length"> The maximum length of the truncated string. </param>
        /// <returns> The truncated string. </returns>
        public static string Truncate(this string value, int length)
            => value.Length <= length ? value : value.Substring(0, length);

        /// <summary> Checks if the string is a palindrome. </summary>
        /// <param name="value"> The string value to check. </param>
        /// <returns> True if the string is a palindrome; otherwise, false. </returns>
        public static bool IsPalindrome(this string value) => value.SequenceEqual(value.Reverse());

        /// <summary> Counts the occurrences of a substring within the string. </summary>
        /// <param name="value"> The string value to search. </param>
        /// <param name="substring"> The substring to count occurrences of. </param>
        /// <returns> The number of occurrences of the substring within the string. </returns>
        public static int CountOccurrences(this string value, string substring)
            => Regex.Matches(value, substring).Count;

        /// <summary> Reverses the characters in the string. </summary>
        /// <param name="value"> The string value to reverse. </param>
        /// <returns> The reversed string. </returns>
        public static string Reverse(this string value) => new string(value.Reverse().ToArray());

        /// <summary> Encodes the string to Base64. </summary>
        /// <param name="value"> The string value to encode. </param>
        /// <returns> The Base64-encoded string. </returns>
        public static string EncodeBase64(this string value)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        /// <summary> Decodes a Base64-encoded string. </summary>
        /// <param name="value"> The Base64-encoded string to decode. </param>
        /// <returns> The decoded string. </returns>
        public static string DecodeBase64(this string value)
            => Encoding.UTF8.GetString(Convert.FromBase64String(value));

        /// <summary> Checks if the string is a valid URL. </summary>
        /// <param name="value"> The string value to check. </param>
        /// <returns> True if the string is a valid URL; otherwise, false. </returns>
        public static bool IsUrl(this string value) => Uri.TryCreate(value, UriKind.Absolute, out _);

        /// <summary> Masks sensitive information within the string. </summary>
        /// <param name="value"> The string value to mask. </param>
        /// <param name="start"> The starting index of the sensitive information. </param>
        /// <param name="length"> The length of the sensitive information to mask. </param>
        /// <param name="maskChar"> The character used for masking. Default is '*'. </param>
        /// <returns> The string with sensitive information masked. </returns>
        public static string Mask(this string value, int start, int length, char maskChar = '*')
            => value.Substring(0, start) + new string(maskChar, Math.Min(length, value.Length - start));

        /// <summary> Formats the string using placeholders and arguments. </summary>
        /// <param name="format"> The format string. </param>
        /// <param name="args"> The arguments to format. </param>
        /// <returns> The formatted string. </returns>
        public static string FormatWith(this string format, params object[] args) => string.Format(format, args);
    }
}
