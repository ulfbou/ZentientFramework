# Zentient.Abstractions 3.0.1

🔧 **DocFX Documentation & Workflow Improvements**

> A patch release focused on fixing documentation deployment issues and optimizing CI/CD workflows discovered after the 3.0.0 release.

## 🔧 Fixed

- **📚 DocFX Documentation Deployment**: Fixed DocFX not deploying to expected GitHub Pages URL (`https://ulfbou.github.io/Zentient.Abstractions/api/Zentient.Abstractions.html`)
- **📱 .NET 9.0 Badge Display**: Restored missing .NET 9.0 badge in README.md and documentation
- **⚙️ NUGET_API_KEY Validation**: Added proper secret validation in release workflow to prevent deployment failures  
- **🏗️ Workflow Optimization**: Specialized docs.yml to use .NET 9.0 only while maintaining .NET 6.0-9.0 support in release/docker workflows
- **📁 DocFX Configuration**: Enhanced DocFX setup with proper GitHub Pages integration and modern schema

## 🚀 Improved

- **🧭 Documentation Navigation**: Added comprehensive API index page and table of contents structure
- **🔗 URL Compatibility**: Added redirect page for expected API documentation URLs
- **📋 Build Process**: Improved DocFX metadata generation with better error handling
- **⚡ CI/CD Performance**: Optimized documentation workflow to use single .NET version for faster builds

## 🏗️ Framework Support

- ✅ .NET 6.0
- ✅ .NET 7.0  
- ✅ .NET 8.0
- ✅ .NET 9.0

## 📦 Installation

```bash
dotnet add package Zentient.Abstractions --version 3.0.1
```

Or via Package Manager:
```powershell
Install-Package Zentient.Abstractions -Version 3.0.1
```

## 🔗 Links

- 📚 [API Documentation](https://ulfbou.github.io/Zentient.Abstractions/)
- 📝 [Full Changelog](https://github.com/ulfbou/Zentient.Abstractions/blob/main/CHANGELOG.md)
- 🐛 [Report Issues](https://github.com/ulfbou/Zentient.Abstractions/issues)

---

**Full Changelog**: https://github.com/ulfbou/Zentient.Abstractions/compare/v3.0.0...v3.0.1
