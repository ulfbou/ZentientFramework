//
// File: BoolExtensions.cs
//
// Description: Provides extension methods for working with bools.
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
/// Provides extension methods for working with bools.
/// </summary>
public static class BoolExtensions
{
    /// <summary>
    /// Toggles the boolean value.
    /// </summary>
    /// <param name="value">The boolean value to toggle.</param>
    /// <returns>The toggled boolean value.</returns>
    public static bool Toggle(this bool value)
    {
        return !value;
    }

    /// <summary>
    /// Converts the boolean value to an integer.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <returns>0 if false; 1 if true.</returns>
    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }

    /// <summary>
    /// Converts the boolean value to "Yes" or "No".
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <returns>"Yes" if true; "No" if false.</returns>
    public static string ToStringYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }

    /// <summary>
    /// Converts the boolean value to "On" or "Off".
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <returns>"On" if true; "Off" if false.</returns>
    public static string ToStringOnOff(this bool value)
    {
        return value ? "On" : "Off";
    }
}
