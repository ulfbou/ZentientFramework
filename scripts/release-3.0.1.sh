#!/bin/bash
# Zentient.Abstractions 3.0.1 Release Script
# This script prepares and executes the 3.0.1 release

set -e

VERSION="3.0.1"
BRANCH="release/3.0.1"

echo "🚀 Preparing Zentient.Abstractions $VERSION Release"
echo "=================================================="

# Verify we're on the correct branch
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [ "$CURRENT_BRANCH" != "$BRANCH" ]; then
    echo "❌ Error: Must be on $BRANCH branch (currently on $CURRENT_BRANCH)"
    exit 1
fi

# Verify working directory is clean
if [ -n "$(git status --porcelain)" ]; then
    echo "❌ Error: Working directory must be clean"
    git status --short
    exit 1
fi

# Verify version has been updated
VERSION_IN_CSPROJ=$(grep -o '<Version>[^<]*' src/Zentient.Abstractions.csproj | sed 's/<Version>//')
if [ "$VERSION_IN_CSPROJ" != "$VERSION" ]; then
    echo "❌ Error: Version in .csproj ($VERSION_IN_CSPROJ) doesn't match expected ($VERSION)"
    exit 1
fi

echo "✅ Pre-release checks passed"
echo ""

# Build and test
echo "🔨 Building project..."
dotnet build src/Zentient.Abstractions.csproj --configuration Release

echo "🧪 Running tests..."
# Add test runs here when tests are available
echo "  (No tests to run for abstractions library)"

echo "📦 Creating NuGet package..."
dotnet pack src/Zentient.Abstractions.csproj --configuration Release --output ./artifacts

echo "📚 Building documentation..."
if command -v docfx &> /dev/null; then
    docfx metadata
    docfx build
    echo "  Documentation built successfully"
else
    echo "  ⚠️  DocFX not installed - skipping documentation build"
fi

echo ""
echo "🏷️  Ready to create release tag $VERSION"
echo "   Next steps:"
echo "   1. Review the changes above"
echo "   2. Run: git tag v$VERSION"
echo "   3. Run: git push origin v$VERSION"
echo "   4. Create GitHub release from tag"
echo "   5. Upload artifacts to NuGet"
echo ""
echo "✅ Release preparation completed!"
