﻿//
// Class: AssertionBuilderTests
// 
// Description:
// Provides a set of test methods for the classes <see chref="AssertionBuilder"/> and <see chref="AssertionBuilderBase"/>. 
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
using Zentient.Tests;

namespace Tests;
[TestClass]
public class AssertionBuilderTests
{
    public Assert Assert { get => Assert.Instance; }

    [TestMethod]
    public void AssertThatString_IsEqualTo_ShouldPass_WhenObjectsAreEqual()
    {
        // Arrange
        string subject = "Same".ToString();
        string expected = new String("Same");

        // Act & Assert
        Assert.Pass(() => Assert
            .That<string>(subject).IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatString_IsEqualTo_ShouldFail_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Different");

        // Act & Assert
        Assert.Fail(() => Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotEqualTo_ShouldPass_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new MyClass();
        var different = new MyClass();

        // Act & Assert
        Assert.Pass(() => Assert
            .That<MyClass>(subject)
            .IsNotEqualTo(different));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotEqualTo_ShouldFail_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new MyClass();
        var same = subject;

        // Act & Assert
        Assert.Fail(() => Assert
            .That<MyClass>(subject)
            .IsNotEqualTo(same));
    }

    [TestMethod]
    public void AssertThatMyClass_IsSameAs_ShouldPass_WhenObjectsAreSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = subject;

        // Act & Assert
        Assert.Pass(() => Assert
            .That<MyClass>(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatMyClass_IsSameAs_ShouldFail_WhenObjectsAreNotSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = new MyClass();

        Assert.That<ICollection<int>>(new List<int>());
        // Act & Assert
        Assert.Fail(() => Assert
            .That<object>(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatMyClass_IsNull_ShouldPass_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Assert.Pass(() => Assert
            .That<MyClass>(subject!)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatMyClass_IsNull_ShouldFail_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Assert.Fail(() => Assert
            .That<MyClass>(subject)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotNull_ShouldPass_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Assert.Pass(() => Assert
            .That<MyClass>(subject)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatMyClass_IsNotNull_ShouldFail_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Assert.Fail(() => Assert
            .That<MyClass>(subject!)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatObject_IsEqualTo_ShouldPass_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Same");

        // Act & Assert
        Assert.Pass(() => Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsEqualTo_ShouldFail_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new String("Same");
        var expected = new String("Different");

        // Act & Assert
        Assert.Fail(() => Assert
            .That<string>(subject)
            .IsEqualTo(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsNotEqualTo_ShouldPass_WhenObjectsAreNotEqual()
    {
        // Arrange
        var subject = new MyClass();
        var different = new MyClass();

        // Act & Assert
        Assert.Pass(() => Assert
            .That(subject)
            .IsNotEqualTo(different));
    }

    [TestMethod]
    public void AssertThatObject_IsNotEqualTo_ShouldFail_WhenObjectsAreEqual()
    {
        // Arrange
        var subject = new MyClass();
        var same = subject;

        // Act & Assert
        Assert.Fail(() => Assert
            .That(subject)
            .IsNotEqualTo(same));
    }

    [TestMethod]
    public void AssertThatObject_IsSameAs_ShouldPass_WhenObjectsAreSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = subject;

        // Act & Assert
        Assert.Pass(() => Assert
            .That(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsSameAs_ShouldFail_WhenObjectsAreNotSameInstance()
    {
        // Arrange
        var subject = new MyClass();
        var expected = new MyClass();

        // Act & Assert
        Assert.Fail(() => Assert
            .That(subject)
            .IsSameAs(expected));
    }

    [TestMethod]
    public void AssertThatObject_IsNull_ShouldPass_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Assert.Pass(() => Assert
            .That(subject!)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNull_ShouldFail_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Assert.Fail(() => Assert
            .That(subject)
            .IsNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNotNull_ShouldPass_WhenObjectIsNotNull()
    {
        // Arrange
        var subject = new MyClass();

        // Act & Assert
        Assert.Pass(() => Assert
            .That(subject)
            .IsNotNull());
    }

    [TestMethod]
    public void AssertThatObject_IsNotNull_ShouldFail_WhenObjectIsNull()
    {
        // Arrange
        MyClass subject = null!;

        // Act & Assert
        Assert.Fail(() => Assert
            .That(subject!)
            .IsNotNull());
    }
}

internal class MyClass
{
    public MyClass()
    {
    }
}
