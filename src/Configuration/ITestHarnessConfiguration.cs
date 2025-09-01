// <copyright file="ITestHarnessConfiguration.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Testing.Diagnostics.Abstractions;

namespace Zentient.Testing.Configuration
{
    public interface ITestHarnessConfiguration
    {
        IReadOnlyCollection<object> Dependencies { get; }
        IReadOnlyCollection<IProbe> Probes { get; }
    }
}
