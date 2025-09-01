// <copyright file="IDiagnosticContainer.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

using Zentient.Testing.Probes;

namespace Zentient.Testing.Diagnostics.Abstractions
{
    public interface IDiagnosticContainer
    {
        ICacheDiagnostics? Cache { get; }
        IRetryDiagnostics? Retry { get; }
    }
}
