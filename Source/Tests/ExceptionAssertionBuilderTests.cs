//
// File: ExceptionAssertionBuilderTests.cs
// 
// Description:
// Defines a set of fluent assertion methods for evaluating equality, type compatibility, and other logical conditions. These methods empower developers to confirm the expected behavior and results of the code under test. The IAssertBuilder interface promotes a more intuitive and sustainable testing practice by supporting a chainable method syntax.
// 
// Usage:
// The IAssertBuilder interface is implemented to facilitate unit testing assertions on object states. Upon instantiation with the target test object, it offers a sequence of chainable methods designed to throw exceptions upon assertion failure, thereby indicating test failures.
// 
// Purpose:
// The IAssertBuilder interface aims to provide a straightforward and articulate means for crafting test assertions. Leveraging a fluent interface pattern, it simplifies test code structure and enhances the clarity of test assertions, aiding developers in grasping the test's purpose and the standards for its success.
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
public class ExceptionAssertionBuilderTests
{
    public Assert Assert { get => Assert.Instance; }

    public static readonly string BaseMessage = "Base exception";
    public static readonly string DerivedMessage = "Derived exception";
    public static readonly string OtherDerivedMessage = "Other Derived exception";
    public static readonly string NotDerivedMessage = "Not Derived exception";

    public static Action ThrowsBase = () => throw new Exceptions(BaseMessage);
    public static Action ThrowsDerived = () => throw new DerivedException(DerivedMessage);
    public static Action ThrowsOtherDerived = () => throw new OtherDerivedException(OtherDerivedMessage);
    public static Action ThrowsNotDerived => () => throw new NotDerivedException(NotDerivedMessage);
    public static Action DoesNotThrowAny = () => {};

    // Validate Throws<T>
    [TestMethod]
    public void AssertThatThrowsBase_IsValidatedBy_ThrowsBaseException()
    {
        Validate.Pass(() => Assert
            .That(ThrowsBase)
            .Throws<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsDerived_IsValidatedBy_ThrowsBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsDerived)
        .Throws<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsOtherDerived_IsValidatedBy_ThrowsBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsOtherDerived)
        .Throws<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsNotDerived_IsInvalidatedBy_ThrowsBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsNotDerived)
        .Throws<Exceptions>());
    }

    [TestMethod]
    public void AssertThatDoesNotThrowAny_IsInvalidatedBy_ThrowsBaseException()
    {
        Validate.Fail(() => Assert
        .That(DoesNotThrowAny)
        .Throws<Exceptions>());
    }

    // Validate ThrowsExactly<T>
    [TestMethod]
    public void AssertThatThrowsBase_IsValidatedBy_ThrowsExactlyBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsBase)
        .ThrowsExactly<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsDerived_IsInvalidatedBy_ThrowsExactlyBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsDerived)
        .ThrowsExactly<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_ThrowsExactlyDerivedException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .ThrowsExactly<DerivedException>());
    }

    [TestMethod]
    public void AssertThatThrowsNotDerived_IsInvalidatedBy_ThrowsExactlyBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsNotDerived)
        .ThrowsExactly<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_ThrowsDerivedBaseException()
    {
        // Validates ThrowsDerived<T>
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .ThrowsDerived<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsDerived_IsValidatedBy_ThrowsDerivedBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsDerived)
        .ThrowsDerived<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsOtherDerived_IsValidatedBy_ThrowsDerivedBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsOtherDerived)
        .ThrowsDerived<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_ThrowsDerivedDerivedException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .ThrowsDerived<DerivedException>());
    }

    [TestMethod]
    public void AssertThatThrowsNotDerived_IsInvalidatedBy_ThrowsDerivedBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsNotDerived)
        .ThrowsDerived<Exceptions>());
    }

    // ThrowsAny
    [TestMethod]
    public void AssertThatThrowsBase_IsValidatedBy_ThrowsAny()
    {
        Validate.Pass(() => Assert
            .That(ThrowsBase)
            .ThrowsAny());
    }

    [TestMethod]
    public void AssertThatDoesNotThrowAny_IsInvalidatedBy_ThrowsAny()
    {
        Validate.Fail(() => Assert
        .That(DoesNotThrowAny)
        .ThrowsAny());
    }

    // Validates DoesNotThrow<T>
    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_DoesNotThrowBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .DoesNotThrow<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsDerived_IsInvalidatedBy_DoesNotThrowBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsDerived)
        .DoesNotThrow<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsOtherDerived_IsInvalidatedBy_DoesNotThrowBaseException()
    {
        Validate.Fail(() => Assert
        .That(ThrowsOtherDerived)
        .DoesNotThrow<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsNotDerived_IsValidatedBy_DoesNotThrowBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsNotDerived)
        .DoesNotThrow<Exceptions>());
    }

    [TestMethod]
    public void AssertThatDoesNotThrowAny_IsValidatedBy_DoesNotThrowBaseException()
    {
        Validate.Pass(() => Assert
        .That(DoesNotThrowAny)
        .DoesNotThrow<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_DoesNotThrowExactlyBaseException()
    {
        // Validate DoesNotThrowExactly<T>
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .DoesNotThrowExactly<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsDerived_IsValidatedBy_DoesNotThrowExactlyBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsDerived)
        .DoesNotThrowExactly<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsValidatedBy_DoesNotThrowExactlyDerivedException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsBase)
        .DoesNotThrowExactly<DerivedException>());
    }

    [TestMethod]
    public void AssertThatThrowsNotDerived_IsValidatedBy_DoesNotThrowExactlyBaseException()
    {
        Validate.Pass(() => Assert
        .That(ThrowsNotDerived)
        .DoesNotThrowExactly<Exceptions>());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_DoesNotThrow()
    {
        // ThrowsAny
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .DoesNotThrow());
    }

    [TestMethod]
    public void AssertThatThrowsBase_IsInvalidatedBy_DoesNotThrowAny()
    {
        Validate.Fail(() => Assert
        .That(ThrowsBase)
        .DoesNotThrowAny());
    }

    [TestMethod]
    public void AssertThatDoesNotThrowAny_IsValidatedBy_DoesNotThrow()
    {
        Validate.Pass(() => Assert
        .That(DoesNotThrowAny)
        .DoesNotThrow());
    }

    [TestMethod]
    public void AssertThatDoesNotThrowAny_IsValidatedBy_DoesNotThrowAny()
    {
        Validate.Pass(() => Assert
        .That(DoesNotThrowAny)
        .DoesNotThrowAny());
    }

    // Validate WithMessage
    [TestMethod]
    public void AssertThatThrowsBase_IsValidatedBy_ThrowsBaseException_WithMessageBaseMessage()
    {
        Validate.Pass(() => Assert
        .That(ThrowsBase)
        .Throws<Exceptions>()
        .WithMessage(BaseMessage));
    }

    [TestMethod]
    public void AssertThatThrowsDerived_IsValidatedBy_ThrowsBaseException_WithMessageContaining_Substring()
    {
        Validate.Pass(() => Assert
        .That(ThrowsDerived)
        .Throws<Exceptions>()
        .WithMessageContaining(DerivedMessage.Substring(0, 1)));
    }

    [TestMethod]
    public void AssertThatThrowsOtherDerived_IsValidatedBy_ThrowsBaseException_WithMessageOtherDerivedMessage()
    {
        Validate.Pass(() => Assert
        .That(ThrowsOtherDerived)
        .Throws<Exceptions>()
        .WithMessage(OtherDerivedMessage));
    }
}
