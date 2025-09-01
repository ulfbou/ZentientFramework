// <copyright file="IMockBehavior.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Linq.Expressions;

namespace Zentient.Testing.Abstractions
{
    public interface IMockBehavior
    {
        Expression<Func<object[], bool>> Predicate { get; }
        Func<object[], object> Action { get; }
        object ReturnValue { get; }
        Exception ExceptionToThrow { get; }
    }
}
