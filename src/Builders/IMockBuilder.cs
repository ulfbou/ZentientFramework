// <copyright file="IMockBuilder.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Linq.Expressions;

using Zentient.Abstractions.Common.Definitions;
using Zentient.Testing.Abstractions;

namespace Zentient.Testing.Builders
{
    public interface IMockBuilder<TDefinition, TService>
        where TDefinition : ITypeDefinition, IMockDefinition<TService>
        where TService : class
    {
        TService Build();
        TService Build(out IMockVerifier<TService> verifier);
        IMockBuilder<TDefinition, TService> When(Expression<Func<TService, bool>> predicate);
        IMockBuilder<TDefinition, TService> OnExecute(Func<object[], object> action);
        IMockBuilder<TDefinition, TService> Returns(object value);
        IMockBuilder<TDefinition, TService> Throws(Exception exception);
    }
}
