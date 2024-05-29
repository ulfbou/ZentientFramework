//
// File: IntegerExtensions.cs
//
// Description: Provides extension methods for working with integers.
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

namespace Zentient.Extensions;

/// <summary>
/// Provides extension methods for working with integers.
/// </summary>
public static class IntegerExtensions
{
    /// <summary>
    /// Determines whether the integer is an even number.
    /// </summary>
    /// <param name="value">The integer value to check.</param>
    /// <returns>True if the integer is even; otherwise, false.</returns>
    public static bool IsEven(this int value)
    {
        return value % 2 == 0;
    }

    /// <summary>
    /// Determines whether the integer is an odd number.
    /// </summary>
    /// <param name="value">The integer value to check.</param>
    /// <returns>True if the integer is odd; otherwise, false.</returns>
    public static bool IsOdd(this int value)
    {
        return value % 2 != 0;
    }

    /// <summary>
    /// Determines whether the integer is a prime number.
    /// </summary>
    /// <param name="value">The integer value to check.</param>
    /// <returns>True if the integer is prime; otherwise, false.</returns>
    public static bool IsPrime(this int value)
    {
        if (value <= 1)
            return false;

        if (value % 2 == 0) return false;

        for (int i = 3; i <= Math.Sqrt(value); i+=2)
        {
            if (value % i == 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Converts the integer to its ordinal representation.
    /// </summary>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The ordinal representation of the integer.</returns>
    public static string ToOrdinal(this int value)
    {
        if (value <= 0)
            return value.ToString();

        string suffix;
        switch (value % 100)
        {
            case 11:
            case 12:
            case 13:
                suffix = "th";
                break;
            default:
                switch (value % 10)
                {
                    case 1:
                        suffix = "st";
                        break;
                    case 2:
                        suffix = "nd";
                        break;
                    case 3:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
                break;
        }
        return value.ToString() + suffix;
    }
}
