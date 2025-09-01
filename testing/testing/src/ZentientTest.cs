// <copyright file="ZentientTest.cs" author="Ulf Bourelius">
// Copyright © 2025 Zentient Frameworks Team. All rights reserved. All rights reserved.
// </copyright>

using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Configuration.Builders;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.Common;
using Zentient.Testing.Configuration;
using Zentient.Testing.Harnesses;
using Zentient.Testing.Harnesses.Abstractions;
using Zentient.Testing.Configuration.Internal;
using Zentient.Testing.Mocks;

namespace Zentient.Testing
{
    public static class ZentientTest
    {
        private static readonly IMockFactory _mockFactory = new MockFactory(); // Assumes MockFactory is implemented and correctly located.

        public static IMockFactory Mocks => _mockFactory;

        public static IHandlerTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition> CreateHandlerTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>(
            Action<ITestConfigurationBuilder> configure)
            where TDefinition : ITypeDefinition
            where TCodeDefinition : ICodeDefinition
            where TErrorDefinition : IErrorDefinition
        {
            var configurationBuilder = new TestConfigurationBuilder();
            configure(configurationBuilder);
            var config = configurationBuilder.Build();
            return new HandlerTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>(config);
        }

        public static IPolicyTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition> CreatePolicyTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>(
            Action<ITestConfigurationBuilder> configure)
            where TDefinition : ITypeDefinition
            where TCodeDefinition : ICodeDefinition
            where TErrorDefinition : IErrorDefinition
        {
            var configurationBuilder = new TestConfigurationBuilder();
            configure(configurationBuilder);
            var config = configurationBuilder.Build();
            return new PolicyTestHarness<TInput, TOutput, TDefinition, TCodeDefinition, TErrorDefinition>(config);
        }
    }
}
