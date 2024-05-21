//
// File: CharExtensions.cs
//
// Description: Provides extension methods for working with chars.
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

using System;

namespace Zentient.Extensions;

/// <summary>
/// Provides extension methods for working with chars.
/// </summary>
public static class CharExtensions
{
    /// <summary>
    /// Determines whether the character is a digit.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a digit; otherwise, false.</returns>
    public static bool IsDigit(this char c) => char.IsDigit(c);

    /// <summary>
    /// Determines whether the character is a letter.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a letter; otherwise, false.</returns>
    public static bool IsLetter(this char c) => char.IsLetter(c);

    /// <summary>
    /// Determines whether the character is a letter or a digit.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a letter or a digit; otherwise, false.</returns>
    public static bool IsLetterOrDigit(this char c) => char.IsLetterOrDigit(c);

    /// <summary>
    /// Determines whether the character is a lowercase letter.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a lowercase letter; otherwise, false.</returns>
    public static bool IsLower(this char c) => char.IsLower(c);

    /// <summary>
    /// Determines whether the character is an uppercase letter.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is an uppercase letter; otherwise, false.</returns>
    public static bool IsUpper(this char c) => char.IsUpper(c);

    /// <summary>
    /// Determines whether the character is a white-space character.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a white-space character; otherwise, false.</returns>
    public static bool IsWhiteSpace(this char c) => char.IsWhiteSpace(c);

    /// <summary>
    /// Converts the character to title case.
    /// </summary>
    /// <param name="c">The character to convert.</param>
    /// <returns>The character converted to title case.</returns>
    public static char ToTitleCase(this char c) => char.ToUpper(c);

    /// <summary>
    /// Determines whether the character is a punctuation mark.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a punctuation mark; otherwise, false.</returns>
    public static bool IsPunctuation(this char c) => char.IsPunctuation(c);

    /// <summary>
    /// Determines whether the character is a symbol.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a symbol; otherwise, false.</returns>
    public static bool IsSymbol(this char c) => char.IsSymbol(c);

    /// <summary>
    /// Determines whether the character is a control character.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a control character; otherwise, false.</returns>
    public static bool IsControl(this char c) => char.IsControl(c);

    /// <summary>
    /// Converts a character representing a digit to its equivalent integer value.
    /// </summary>
    /// <param name="c">The character representing a digit.</param>
    /// <returns>The integer value of the digit.</returns>
    public static int ToInt(this char c)
        => char.IsDigit(c) ? (int)(c - '0') : throw new ArgumentException("Character is not a digit.", nameof(c));
}
