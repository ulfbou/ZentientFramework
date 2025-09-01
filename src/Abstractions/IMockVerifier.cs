// <copyright file="IMockVerifier.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Testing.Abstractions
{
    public interface IMockVerifier<TService> where TService : class
    {
        void ShouldHaveBeenCalledTimes(int times);
        IReadOnlyCollection<ICallInfo> GetCallHistory();
    }
}
