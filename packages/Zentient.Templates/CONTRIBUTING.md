# Contributing to Zentient.Templates

Thank you for your interest in contributing to Zentient.Templates! This guide will help you get started with developing, testing, and improving our .NET project templates.

## Table of Contents

- [Development Setup](#development-setup)
- [Template Development](#template-development)
- [Testing Templates](#testing-templates)
- [Code Standards](#code-standards)
- [Submitting Changes](#submitting-changes)
- [Template Architecture](#template-architecture)

## Development Setup

### Prerequisites

- **.NET SDK 8.0 or later** - Required for template engine and testing
- **Git** - For version control
- **Visual Studio Code** (recommended) - With C# Dev Kit extension
- **Docker** (optional) - For testing containerized scenarios

### Getting Started

1. **Fork and Clone**
   ```bash
   git fork https://github.com/ulfbou/Zentient.Templates
   git clone https://github.com/YOUR_USERNAME/Zentient.Templates.git
   cd Zentient.Templates
   ```

2. **Install Templates Locally**
   ```bash
   # Install both templates for testing
   dotnet new install ./templates/zentient-library-template
   dotnet new install ./templates/zentient-project-template
   
   # Verify installation
   dotnet new list zentient
   ```

3. **Verify Setup**
   ```bash
   # Create test instances
   mkdir test-workspace && cd test-workspace
   
   # Test library template
   dotnet new zentient-lib -n TestLibrary
   cd TestLibrary && dotnet build && dotnet test
   cd ..
   
   # Test project template
   dotnet new zentient -n TestProject
   cd TestProject && dotnet build
   cd ..
   
   # Clean up
   cd .. && rm -rf test-workspace
   ```

## Template Development

### Template Structure

```
templates/
├── zentient-library-template/
│   ├── .template.config/
│   │   └── template.json              # Template configuration
│   ├── Directory.*.props/.targets     # MSBuild automation
│   ├── src/                          # Source code examples
│   ├── tests/                        # Test examples
│   └── docs/                         # Documentation examples
└── zentient-project-template/
    ├── .template.config/
    │   └── template.json              # Template configuration
    └── src/                          # Multi-project structure
```

### Key Files and Their Purpose

#### Template Configuration (`template.json`)
Defines template metadata, parameters, and behavior:

```json
{
  "$schema": "http://json.schemastore.org/template",
  "identity": "Zentient.Library.Template",
  "shortName": "zentient-lib",
  "symbols": {
    "ProjectName": {
      "type": "parameter",
      "dataType": "string",
      "defaultValue": "Zentient.NewLibrary",
      "replaces": "Zentient.LibraryTemplate"
    }
  }
}
```

#### Directory Build Files
MSBuild automation files that provide enterprise-grade features:

- **Directory.Build.props** - Core build properties
- **Directory.Pack.props** - NuGet packaging configuration
- **Directory.Quality.props** - Code analysis and quality gates
- **Directory.Test.props** - Testing infrastructure
- **Directory.Security.props** - Security scanning and compliance
- **Directory.Documentation.props** - Documentation generation

### Adding New Parameters

1. **Define in template.json**
   ```json
   "symbols": {
     "MyNewParameter": {
       "type": "parameter",
       "dataType": "bool",
       "description": "Enable my new feature",
       "defaultValue": "false"
     }
   }
   ```

2. **Use in template files**
   ```xml
   <!-- In .props files -->
   <PropertyGroup Condition="'$(MyNewParameter)' == 'true'">
     <EnableMyFeature>true</EnableMyFeature>
   </PropertyGroup>
   ```

3. **Add conditional content**
   ```json
   "sources": [
     {
       "modifiers": [
         {
           "condition": "(!MyNewParameter)",
           "exclude": ["optional-feature/**"]
         }
       ]
     }
   ]
   ```

## Testing Templates

### Manual Testing

1. **Create Test Instance**
   ```bash
   # Test with default parameters
   dotnet new zentient-lib -n ManualTest
   cd ManualTest
   
   # Verify all features work
   dotnet build
   dotnet test
   dotnet pack
   ```

2. **Test Parameter Variations**
   ```bash
   # Test different configurations
   dotnet new zentient-lib -n TestMinimal \
     --EnableSigning false \
     --EnableTesting false \
     --EnableDocumentation false
   
   dotnet new zentient-lib -n TestMaximal \
     --EnablePerformance true \
     --LibraryType Validation
   ```

3. **Test Multi-Framework**
   ```bash
   # Test different target frameworks
   dotnet new zentient-lib -n TestNet6 --Framework net6.0
   dotnet new zentient-lib -n TestNet9 --Framework net9.0
   ```

### Template Validation Checklist

Before submitting changes, verify:

- [ ] Template installs without errors
- [ ] All parameter combinations work
- [ ] Generated projects build successfully
- [ ] Tests run and pass
- [ ] NuGet packages generate correctly
- [ ] Documentation builds without warnings
- [ ] Security scans complete successfully
- [ ] Performance benchmarks execute (if enabled)

## Code Standards

### MSBuild Files (.props/.targets)

```xml
<!-- Good: Clear structure with comments -->
<Project>
  <!-- Feature Configuration -->
  <PropertyGroup Label="MyFeature Settings">
    <EnableMyFeature Condition="'$(EnableMyFeature)' == ''">true</EnableMyFeature>
    <MyFeatureLevel Condition="'$(MyFeatureLevel)' == ''">strict</MyFeatureLevel>
  </PropertyGroup>

  <!-- Feature Dependencies -->
  <ItemGroup Condition="'$(EnableMyFeature)' == 'true'">
    <PackageReference Include="MyFeature.Package" Version="1.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

### Template.json Structure

```json
{
  // Required metadata
  "$schema": "http://json.schemastore.org/template",
  "identity": "Unique.Template.Identity",
  "shortName": "template-shortname",
  
  // Clear parameter definitions
  "symbols": {
    "ParameterName": {
      "type": "parameter",
      "dataType": "bool|string|choice",
      "description": "Clear description of what this parameter does",
      "defaultValue": "sensible-default"
    }
  },
  
  // Conditional file inclusion
  "sources": [
    {
      "modifiers": [
        {
          "condition": "(!FeatureEnabled)",
          "exclude": ["feature-specific-files/**"]
        }
      ]
    }
  ]
}
```

### C# Code Standards

- **Follow .NET coding conventions**
- **Use XML documentation for all public APIs**
- **Include comprehensive examples in template code**
- **Write tests that demonstrate template usage**

## Submitting Changes

### Pull Request Process

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/my-improvement
   git commit -m "feat: add improved documentation generation"
   ```

2. **Test Thoroughly**
   ```bash
   # Manual verification
   dotnet new zentient-lib -n TestMyChanges
   cd TestMyChanges && dotnet build && dotnet test
   ```

3. **Update Documentation**
   - Update README.md if adding new features
   - Update template parameter tables
   - Add examples for new functionality

4. **Submit Pull Request**
   - Clear description of changes
   - Link to related issues
   - Include testing evidence

### Commit Message Convention

We use conventional commits:

- `feat:` - New features or enhancements
- `fix:` - Bug fixes
- `docs:` - Documentation changes
- `test:` - Test improvements
- `refactor:` - Code refactoring
- `chore:` - Build/tooling changes

Examples:
```
feat: add performance benchmarking to library template
fix: resolve CS1591 warning inconsistency in Directory.*.props
docs: update README with new template parameters
```

### Code Review Criteria

Pull requests are evaluated on:

- **Developer Experience Impact** - Does this improve DX?
- **Template Quality** - Are generated projects production-ready?
- **Testing Coverage** - Are changes properly tested?
- **Documentation** - Is new functionality documented?
- **Backward Compatibility** - Do existing templates still work?

## Template Architecture

### Design Principles

1. **Developer Experience First** - Every decision prioritizes developer productivity
2. **Zero Configuration** - Templates should work immediately without setup
3. **Enterprise Ready** - Generated projects are production-ready
4. **Modular Features** - Users can opt-in/out of specific capabilities
5. **Best Practices** - Templates enforce industry best practices

### Directory.*.props Pattern

We use a modular MSBuild approach:

```
Directory.Build.props          # Core build settings
Directory.Pack.props           # NuGet packaging
Directory.Quality.props        # Code analysis
Directory.Security.props       # Security scanning
Directory.Test.props           # Testing infrastructure
Directory.Documentation.props  # Documentation generation
Directory.Performance.props    # Benchmarking
```

This allows:
- **Focused responsibility** per file
- **Easy maintenance** and updates
- **Conditional inclusion** based on parameters
- **Clear separation** of concerns

## Getting Help

- **GitHub Issues** - For bugs, feature requests, and questions
- **Pull Request Reviews** - Maintainers provide detailed feedback

Thank you for contributing to Zentient.Templates! Your efforts help create better developer experiences for the entire .NET community.
    * Provide a clear title and detailed description for your PR.
    * Reference any related issues (e.g., `Closes #123` or `Fixes #123`).
    * Explain the changes you've made, why they are necessary, and how they align with Zentient's conventions.
    * Ensure all CI checks (formatting, analyzers, tests) pass.
    * Be prepared for constructive feedback and discussions during the review process.

## License

By contributing to the Zentient Framework, you agree that your contributions will be licensed under the [MIT License](https://github.com/ulfbou/Zentient.Framework/blob/main/LICENSE) (adjust path if different).

## Support & Contact

If you have any questions or need clarification on the contributing process, please don't hesitate to open a [GitHub Issue](https://github.com/ulfbou/Zentient.Framework/issues) or join our [Discussions board](https://github.com/ulfbou/Zentient.Framework/discussions) (if applicable).

Thank you for helping us build the future of predictable .NET outcomes!

---

## 📚 Resources & Related Documents

* [Zentient.Results NuGet](https://www.nuget.org/packages/Zentient.Results)
* [Zentient.Endpoints NuGet](https://www.nuget.org/packages/Zentient.Endpoints)
* [GitVersion Documentation](https://gitversion.net/docs/)
* [dotnet format Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format)
* `/docs/conventions/async-guidelines.md`
* `/docs/conventions/developer-first.md`
* `/docs/conventions/naming-conventions.md`
* `/docs/architecture/transport-pipeline.md`
* `/docs/analyzers/rules.md`
