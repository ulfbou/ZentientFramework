// <copyright file="IHarnessContext.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Abstractions.DependencyInjection;
using Zentient.Testing.Configuration;

namespace Zentient.Testing.Harnesses.Abstractions
{
    public interface IHarnessContext
    {
        IServiceResolver Resolver { get; }
        ITestHarnessConfiguration Configuration { get; }
    }
}
