// <copyright file="IMockVerifier.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace Zentient.Testing.Mocks
{
    public interface IMockVerifier
    {
        void TimesCalled(int expectedCount);
        void WasCalledWith<TInput>(TInput expectedInput);
    }
}
