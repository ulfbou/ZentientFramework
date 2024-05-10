//
// File: TestInfo.cs
//
// Description: Struct for storing an instance, setup and test methods.
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

namespace Zentient.Tests

/// <summary>
/// Struct for storing an instance, setup and test methods.
/// </summary>
internal struct TestInfo(
    object instance,
    MethodInfo? setup,
    IEnumerable<MethodInfo> tests)
{
    private readonly object _instance = instance;
    private readonly MethodInfo? _setup = setup;
    private readonly IEnumerable<MethodInfo> _tests = tests;
    
    internal object Instance { get => _instance; }
    internal MethodInfo? Setup { get => _setup; }
    internal IEnumerable<MethodInfo> Tests {get => _tests; }
}
