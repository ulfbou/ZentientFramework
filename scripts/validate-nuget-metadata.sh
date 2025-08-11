#!/bin/bash

# Template NuGet Package Validation Script
# This script validates that templates generate projects with proper NuGet metadata

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
TEMPLATES_DIR="$REPO_ROOT/templates"
TEMP_DIR="/tmp/template-validation"

echo "🔍 Starting template NuGet package validation..."

# Create temp directory if it doesn't exist
mkdir -p "$TEMP_DIR"

# Function to validate a template
validate_template() {
    local template_name="$1"
    local template_path="$2"
    
    echo "📦 Validating template: $template_name"
    
    # Install template first
    echo "  📥 Installing template..."
    dotnet new install "$template_path" > /dev/null 2>&1 || true
    
    # Clean up any existing temp directory
    rm -rf "$TEMP_DIR/$template_name"
    mkdir -p "$TEMP_DIR/$template_name"
    
    # Create project from template
    echo "  ⚡ Creating project from template..."
    dotnet new "$template_name" \
        --name "TestProject" \
        --Author "Test Author" \
        --Company "Test Company" \
        --Description "Test project description for validation" \
        --RepositoryUrl "https://github.com/test/test-project" \
        --Tags "test;validation;template" \
        --output "$TEMP_DIR/$template_name" \
        --force
    
    cd "$TEMP_DIR/$template_name"
    
    # Restore packages
    echo "  📥 Restoring NuGet packages..."
    dotnet restore --verbosity quiet
    
    # Build project
    echo "  🔨 Building project..."
    dotnet build --no-restore --verbosity quiet
    
    # Validate package can be created
    echo "  📦 Validating NuGet package creation..."
    dotnet pack --no-build --dry-run --verbosity normal > pack-output.log 2>&1
    
    # Check for validation errors
    if grep -q "error" pack-output.log; then
        echo "  ❌ Package validation failed:"
        cat pack-output.log
        return 1
    fi
    
    # Check for template placeholders in output
    if grep -q "LIBRARY_\|PROJECT_\|REPOSITORY_URL" pack-output.log; then
        echo "  ⚠️  Template placeholders found in package metadata:"
        grep "LIBRARY_\|PROJECT_\|REPOSITORY_URL" pack-output.log
        return 1
    fi
    
    echo "  ✅ Template validation successful"
    cd - > /dev/null
    
    # Uninstall template to clean up
    dotnet new uninstall "$template_path" > /dev/null 2>&1 || true
}

# Validate zentient-lib template
if [ -d "$TEMPLATES_DIR/zentient-library-template" ]; then
    validate_template "zentient-lib" "$TEMPLATES_DIR/zentient-library-template"
else
    echo "❌ zentient-library-template directory not found"
    exit 1
fi

# Validate zentient template
if [ -d "$TEMPLATES_DIR/zentient-project-template" ]; then
    validate_template "zentient" "$TEMPLATES_DIR/zentient-project-template"
else
    echo "❌ zentient-project-template directory not found"
    exit 1
fi

# Clean up
echo "🧹 Cleaning up temporary files..."
rm -rf "$TEMP_DIR"

echo "✅ All template NuGet package validation tests passed!"
