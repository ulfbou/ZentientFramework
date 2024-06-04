//
// File: DoubleExtensions.cs
//
// Description: Provides extension methods for working with doubles.
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

namespace Zentient.Extensions
{
    /// <summary>
    /// Provides extension methods for working with doubles.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Determines whether the double is an integer.
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the double is an integer; otherwise, false.</returns>
        public static bool IsInteger(this double value) => Math.Abs(value % 1) < double.Epsilon;

        /// <summary>
        /// Determines whether the double is positive.
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the double is positive; otherwise, false.</returns>
        public static bool IsPositive(this double value) => value > 0;

        /// <summary>
        /// Determines whether the double is negative.
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the double is negative; otherwise, false.</returns>
        public static bool IsNegative(this double value) => value < 0;

        /// <summary>
        /// Rounds the double to the specified number of decimal places.
        /// </summary>
        /// <param name="value">The double value to round.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>The rounded double value.</returns>
        public static double Round(this double value, int decimalPlaces) => Math.Round(value, decimalPlaces);

        /// <summary>
        /// Converts the double from radians to degrees.
        /// </summary>
        /// <param name="value">The double value to convert.</param>
        /// <returns>The double value converted to degrees.</returns>
        public static double ToDegrees(this double value) => value * (180.0 / Math.PI);

        /// <summary>
        /// Converts the double from degrees to radians.
        /// </summary>
        /// <param name="value">The double value to convert.</param>
        /// <returns>The double value converted to radians.</returns>
        public static double ToRadians(this double value) => value * (Math.PI / 180.0);

        /// <summary>
        /// Determines whether the double is NaN (Not a Number).
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the double is NaN; otherwise, false.</returns>
        public static bool IsNaN(this double value) => double.IsNaN(value);

        /// <summary>
        /// Determines whether the double is positive or negative infinity.
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the double is positive or negative infinity; otherwise, false.</returns>
        public static bool IsInfinity(this double value) => double.IsInfinity(value);

        /// <summary>
        /// Determines whether the double is a finite number (not NaN, not positive or negative infinity).
        /// </summary>
        /// <param name="value">The double value to check.</param>
        /// <returns>True if the double is a finite number; otherwise, false.</returns>
        public static bool IsFinite(this double value) => !double.IsNaN(value) && !double.IsInfinity(value);


        /// <summary>
        /// Calculates the percentage of another double.
        /// </summary>
        /// <param name="value">The double value representing the percentage.</param>
        /// <param name="of">The double value to calculate the percentage of.</param>
        /// <returns>The percentage of the specified double.</returns>
        public static double PercentageOf(this double value, double of) => (value / of) * 100.0;

    }
}
