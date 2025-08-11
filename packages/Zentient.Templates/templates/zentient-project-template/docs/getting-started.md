# Getting Started with PROJECT_NAME

This guide will help you get up and running with your new Zentient project.

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio Code (recommended) or Visual Studio
- Git (for version control)
#if (UseDocker)
- Docker (for containerization)
#endif
#if (UseRedisCache)
- Redis (for caching)
#endif
#if (UseEntityFramework)
- SQL Server or compatible database
#endif

## Quick Start

### 1. Clone and Setup

```bash
# If you created this from the template, you're already ready!
# Otherwise, clone your repository:
git clone REPOSITORY_URL
cd PROJECT_NAME

# Restore dependencies
dotnet restore
```

### 2. Build and Test

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Run the application
dotnet run --project src/PROJECT_NAME.Core
```

### 3. Development Workflow

```bash
# Start development with file watching
dotnet watch run --project src/PROJECT_NAME.Core

# Run tests continuously
dotnet watch test
```

#if (UseDocker)
### 4. Docker Development

```bash
# Build Docker image
docker build -t PROJECT_NAME:latest .

# Run with Docker Compose
docker-compose up -d
```
#endif

## Project Structure

```
PROJECT_NAME/
├── src/                          # Source code
│   └── PROJECT_NAME.Core/        # Main application
├── tests/                        # Test projects
├── docs/                         # Documentation
├── analyzers/                    # Code analysis rules
├── Directory.*.props            # MSBuild configuration
└── docfx.json                   # Documentation generation
```

## Next Steps

- Review the [Architecture](architecture.md) documentation
- Check out the [Contributing Guidelines](../CONTRIBUTING.md)
- Explore the example code in `src/`
- Run the test suite to ensure everything works
