// <copyright file="src/Zentient.Analyzers/Abstractions/StubMode.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Analyzers.Abstractions
{
    /// <summary>
    /// Specifies the mode in which a stub is generated.
    /// </summary>
    public enum StubMode
    {
        /// <summary>Stub is a partial class.</summary>
        PartialClass,
        /// <summary>Stub is a full class.</summary>
        FullClass,
        /// <summary>Stub is an interface.</summary>
        Interface,
        /// <summary>Stub is a record.</summary>
        Record,
        /// <summary>Stub is a struct.</summary>
        Struct,
        /// <summary>Stub is an enum.</summary>
        Enum
    }
}
