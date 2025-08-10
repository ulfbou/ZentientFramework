# Zentient Library Template

This template provides a comprehensive, enterprise-grade foundation for developing libraries within the Zentient ecosystem. It includes complete automation for all development tasks through specialized Directory.*.* files.

## Features

### 🏗️ **Complete Development Automation**
- **Build System**: Comprehensive MSBuild configuration with multi-targeting support
- **Code Signing**: Automated assembly signing with strong naming and certificate management  
- **Testing**: Full test automation with coverage reporting, benchmarking, and validation
- **Quality**: Code analysis, StyleCop, security scanning, and quality gates
- **Security**: Vulnerability scanning, cryptographic validation, and security compliance
- **Documentation**: API documentation generation, DocFX integration, and sample validation
- **Performance**: Benchmarking, profiling, memory analysis, and regression detection

### 📁 **Directory Infrastructure Files**
- `Directory.Build.props/.targets` - Core build configuration and automation
- `Directory.Pack.props/.targets` - NuGet packaging and distribution
- `Directory.Sign.props/.targets` - Code signing and strong naming
- `Directory.Test.props/.targets` - Comprehensive testing infrastructure  
- `Directory.Quality.props/.targets` - Code quality analysis and enforcement
- `Directory.Security.props/.targets` - Security analysis and compliance validation
- `Directory.Documentation.props/.targets` - Documentation generation and validation
- `Directory.Performance.props/.targets` - Performance monitoring and benchmarking

### 🛠️ **Developer Experience**
- **VS Code Integration**: Complete debugging, tasks, and extension configuration
- **Zero Setup**: No manual configuration required - everything works out of the box
- **Standards Compliance**: Follows Zentient coding standards and best practices
- **Enterprise Ready**: Suitable for production use with comprehensive automation

## Quick Start

### 1. Install the Template
```bash
dotnet new install /path/to/zentient-library-template
```

### 2. Create a New Library
```bash
# Basic library
dotnet new zentient-lib -n MyLibrary

# With all features enabled
dotnet new zentient-lib -n MyLibrary \
  --EnableSigning true \
  --EnableTesting true \
  --EnableQuality true \
  --EnableSecurity true \
  --EnableDocumentation true \
  --EnablePerformance true

# Specific library type
dotnet new zentient-lib -n Zentient.MyValidation \
  --LibraryType Validation \
  --Description "Custom validation library for business rules"
```

### 3. Build and Test
```bash
cd MyLibrary

# Build with all automation
dotnet build

# Run tests with coverage
dotnet test

# Generate documentation
dotnet build -p:GenerateDocumentation=true

# Run performance benchmarks (if enabled)
dotnet build -p:PerformanceBenchmarkingEnabled=true
```

## Template Parameters

| Parameter | Description | Default | Options |
|-----------|-------------|---------|---------|
| `ProjectName` | Library project name | `Zentient.NewLibrary` | Any valid name |
| `Framework` | Target framework | `net8.0` | `net8.0`, `net7.0`, `net6.0`, `netstandard2.1`, `netstandard2.0` |
| `Description` | Library description | `A Zentient framework library` | Any description |
| `Author` | Library author | `Zentient` | Any author name |
| `Company` | Company name | `Zentient` | Any company name |
| `LibraryType` | Type of library | `Custom` | `Core`, `DependencyInjection`, `Validation`, `Configuration`, `Caching`, `Messaging`, `Diagnostics`, `Policies`, `Observability`, `Custom` |
| `EnableSigning` | Enable assembly signing | `true` | `true`, `false` |
| `EnableTesting` | Include testing setup | `true` | `true`, `false` |
| `EnableQuality` | Enable quality analysis | `true` | `true`, `false` |
| `EnableSecurity` | Enable security analysis | `true` | `true`, `false` |
| `EnableDocumentation` | Enable documentation | `true` | `true`, `false` |
| `EnablePerformance` | Enable performance monitoring | `false` | `true`, `false` |

## Project Structure

```
YourLibrary/
├── .vscode/                    # VS Code configuration
│   ├── extensions.json        # Recommended extensions
│   ├── launch.json            # Debug configuration
│   ├── settings.json          # Workspace settings
│   └── tasks.json             # Build tasks
├── analyzers/                  # Code analysis rules
│   ├── Zentient.ruleset       # Main ruleset
│   └── Zentient.Tests.ruleset # Test ruleset
├── docs/                      # Documentation
│   └── README.md              # Documentation index
├── src/                      # Library source code
├── tests/                    # Unit tests
├── docs/                     # Documentation
├── .vscode/                  # VS Code configuration
├── .github/                  # GitHub workflows and templates
│   └── BasicUsage.cs          # Basic usage example
├── src/                       # Source code
│   ├── YourLibrary.csproj     # Main project
│   └── ... your code ...      # Library implementation
├── tests/                     # Test projects
│   ├── YourLibrary.Tests.csproj # Test project
│   └── ... test code ...      # Unit tests
├── Directory.*.props          # Build configuration files
├── Directory.*.targets        # Build automation files
├── global.json               # .NET SDK version
├── .editorconfig             # Code formatting
├── .gitignore                # Git ignore rules
├── docfx.json                # Documentation config
├── YourLibrary.sln           # Solution file
├── Zentient.snk              # Strong name key
├── CHANGELOG.md              # Change log
├── LICENSE                   # License file
└── README.md                 # Project README
```

## Build Targets

The template provides numerous build targets for different development tasks:

### Quality & Analysis
```bash
dotnet build -p:RunCodeAnalysis=true
dotnet build -p:GenerateQualityReport=true
```

### Security
```bash
dotnet build -p:RunSecurityCodeAnalysis=true
dotnet build -p:ScanNuGetPackages=true
```

### Testing
```bash
dotnet test -p:CollectCoverage=true
dotnet test -p:RunBenchmarks=true
```

### Documentation
```bash
dotnet build -p:GenerateDocumentation=true
dotnet build -p:ValidateDocumentationLinks=true
```

### Performance
```bash
dotnet build -p:PerformanceBenchmarkingEnabled=true
dotnet build -p:EnableMemoryAnalysis=true
```

### Packaging
```bash
dotnet pack -p:IncludeSymbols=true
dotnet pack -p:IncludeSource=true
```

## Advanced Configuration

### Custom Analyzers
Add custom analyzers to your project by modifying `Directory.Quality.props`:

```xml
<PackageReference Include="YourCustomAnalyzer" Version="1.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

### Performance Thresholds
Configure performance regression thresholds in `Directory.Performance.props`:

```xml
<PropertyGroup>
  <PerformanceRegressionThreshold>5</PerformanceRegressionThreshold>
  <MaxAllowedMemoryIncrease>10</MaxAllowedMemoryIncrease>
</PropertyGroup>
```

### Security Policies
Customize security policies in `Directory.Security.props`:

```xml
<PropertyGroup>
  <MinimumRSAKeySize>4096</MinimumRSAKeySize>
  <AllowedHashAlgorithms>SHA256;SHA384;SHA512</AllowedHashAlgorithms>
</PropertyGroup>
```

## Integration with Zentient.Abstractions

This template is designed to work seamlessly with Zentient.Abstractions:

```csharp
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Results;

namespace YourLibrary
{
    public class YourService : IIdentifiable
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        
        public IResult<string> ProcessData(string input)
        {
            // Your implementation here
            return Results.Success(input.ToUpper());
        }
    }
}
```

## 🗺️ Roadmap

### Current Status: MVP Complete ✅
The template currently provides a fully functional minimal viable product with:
- ✅ Core library structure with multi-framework targeting (.NET 6.0-9.0)
- ✅ Complete Directory.*.* automation infrastructure
- ✅ Sample code demonstrating Zentient.Abstractions patterns
- ✅ Working test framework with xUnit, FluentAssertions, and Moq
- ✅ VS Code integration with debugging and tasks
- ✅ Strong name signing and basic security

### Phase 1: Enhanced Features (Future)

#### 🔧 Advanced Template Configuration
- [ ] **Template Parameters**: Add dotnet template parameters for customization
  - `--EnableSigning`, `--EnableTesting`, `--EnableQuality` options
  - Author, company, and copyright template variables
  - Custom namespace and project name substitution
- [ ] **Template Metadata**: Complete template.json configuration
  - Parameter validation and constraints
  - Conditional file inclusion based on parameters
  - Post-creation scripts for setup automation

#### 📊 Enhanced Quality & Analysis
- [ ] **Code Coverage**: Integrate coverlet for test coverage reporting
  - Coverage thresholds and quality gates
  - Integration with CI/CD pipelines
  - Coverage reports in multiple formats
- [ ] **Advanced Code Analysis**: Enhanced static analysis rules
  - Custom analyzers for Zentient patterns
  - Architecture compliance validation
  - Dependency analysis and metrics

#### 🔒 Advanced Security Features
- [ ] **Security Scanning**: Comprehensive vulnerability detection
  - SAST (Static Application Security Testing) integration
  - Dependency vulnerability scanning
  - Security policy enforcement
- [ ] **Cryptographic Validation**: Certificate-based signing
  - Code signing certificates integration
  - Timestamping and validation
  - Supply chain security measures

#### 📖 Documentation & Samples
- [ ] **API Documentation**: Complete DocFX integration
  - Automated API reference generation
  - Code example validation
  - Documentation deployment pipeline
- [ ] **Enhanced Samples**: More comprehensive examples
  - Advanced patterns using Zentient.Abstractions
  - Integration examples with common frameworks
  - Best practices and architectural guidance

#### ⚡ Performance & Monitoring
- [ ] **Benchmarking**: BenchmarkDotNet integration
  - Performance regression detection
  - Memory allocation tracking
  - Automated performance reporting
- [ ] **Observability**: Telemetry and monitoring
  - OpenTelemetry integration
  - Performance counters
  - Health check endpoints

#### 🚀 DevOps & Deployment
- [ ] **CI/CD Templates**: Ready-to-use pipeline configurations
  - GitHub Actions workflows
  - Azure DevOps pipelines
  - Docker containerization support
- [ ] **Package Management**: Enhanced NuGet packaging
  - Automated version management
  - Symbol packages and source linking
  - Multi-target package optimization

### Phase 2: Ecosystem Integration (Long-term)

#### 🌐 Framework Integration
- [ ] **ASP.NET Core**: Web API project templates
- [ ] **Blazor**: Component library templates
- [ ] **MAUI**: Cross-platform application templates
- [ ] **Worker Services**: Background service templates

#### 🏗️ Architecture Templates
- [ ] **Microservices**: Service mesh integration
- [ ] **Event-Driven**: Message broker patterns
- [ ] **Clean Architecture**: Layered solution templates
- [ ] **Domain-Driven Design**: DDD pattern templates

### Contributing to Roadmap

Priority is given to:
1. **Developer Experience**: Features that reduce setup time and improve productivity
2. **Quality Assurance**: Tools that prevent bugs and ensure code quality
3. **Security**: Features that enhance application security
4. **Performance**: Tools that optimize runtime performance and resource usage

## Contributing

1. Follow the established patterns in existing Directory.*.* files
2. Ensure all changes maintain backward compatibility
3. Add appropriate tests for new functionality
4. Update documentation for any new features

## License

This template is licensed under the MIT License. See LICENSE file for details.

---

**Enterprise-Grade Development, Zero Configuration Required** ✨
