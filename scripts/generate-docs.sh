#!/bin/bash

# Documentation Generation and Validation Script
# This script automates the complete documentation generation process

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

echo "📚 Starting documentation generation process..."

# Function to check if a tool is installed
check_tool() {
    local tool="$1"
    if ! command -v "$tool" &> /dev/null; then
        echo "❌ $tool is not installed"
        return 1
    fi
    echo "✅ $tool is available"
    return 0
}

# Function to install DocFX if not available
install_docfx() {
    echo "🔧 Installing DocFX..."
    if ! dotnet tool list -g | grep -q docfx; then
        dotnet tool install -g docfx
        echo "✅ DocFX installed successfully"
    else
        echo "✅ DocFX is already installed"
    fi
}

# Function to validate XML documentation
validate_xml_docs() {
    echo "🔍 Validating XML documentation coverage..."
    
    local xml_files=$(find "$PROJECT_ROOT" -name "*.xml" -path "*/bin/Release/*" 2>/dev/null)
    
    if [ -z "$xml_files" ]; then
        echo "⚠️  No XML documentation files found. Run 'dotnet build -c Release' first."
        return 1
    fi
    
    for xml_file in $xml_files; do
        echo "  📄 Found documentation: $(basename "$xml_file")"
        
        # Check if XML file is not empty
        if [ ! -s "$xml_file" ]; then
            echo "  ⚠️  Documentation file is empty: $xml_file"
        else
            # Count documented members
            local member_count=$(grep -c "<member " "$xml_file" 2>/dev/null || echo "0")
            echo "  📊 Documented members: $member_count"
        fi
    done
    
    echo "✅ XML documentation validation complete"
}

# Function to generate DocFX documentation
generate_docfx() {
    echo "📖 Generating DocFX documentation..."
    
    cd "$PROJECT_ROOT"
    
    # Check if docfx.json exists
    if [ ! -f "docfx.json" ]; then
        echo "❌ docfx.json not found in project root"
        return 1
    fi
    
    # Clean previous build
    if [ -d "_site" ]; then
        echo "🧹 Cleaning previous documentation build..."
        rm -rf _site
    fi
    
    # Generate documentation
    echo "🔨 Building documentation..."
    docfx docfx.json
    
    if [ $? -eq 0 ]; then
        echo "✅ Documentation generated successfully"
        echo "📁 Documentation available in: $PROJECT_ROOT/_site"
        
        # Calculate documentation size
        local site_size=$(du -sh _site 2>/dev/null | cut -f1 || echo "unknown")
        echo "📊 Documentation size: $site_size"
        
        return 0
    else
        echo "❌ Documentation generation failed"
        return 1
    fi
}

# Function to serve documentation locally
serve_docs() {
    echo "🌐 Starting documentation server..."
    cd "$PROJECT_ROOT"
    
    if [ ! -d "_site" ]; then
        echo "❌ No documentation site found. Run generation first."
        return 1
    fi
    
    echo "📡 Serving documentation at http://localhost:8080"
    echo "Press Ctrl+C to stop the server"
    
    docfx serve _site --port 8080
}

# Function to validate documentation links
validate_links() {
    echo "🔗 Validating documentation links..."
    
    cd "$PROJECT_ROOT"
    
    if [ ! -d "_site" ]; then
        echo "⚠️  No documentation site found. Skipping link validation."
        return 0
    fi
    
    # Simple link validation - check for broken internal links
    local broken_links=0
    
    find _site -name "*.html" -exec grep -l "href.*\.html" {} \; | while read -r file; do
        local relative_path=${file#$PROJECT_ROOT/_site/}
        echo "  🔍 Checking links in: $relative_path"
        
        # Extract internal HTML links and validate they exist
        grep -o 'href="[^"]*\.html[^"]*"' "$file" | while read -r link; do
            local href=$(echo "$link" | sed 's/href="//;s/".*//')
            local target_file="$PROJECT_ROOT/_site/$(dirname "$relative_path")/$href"
            
            if [[ "$href" == /* ]]; then
                target_file="$PROJECT_ROOT/_site$href"
            fi
            
            if [ ! -f "$target_file" ]; then
                echo "    ⚠️  Broken link: $href in $relative_path"
                broken_links=$((broken_links + 1))
            fi
        done
    done
    
    if [ $broken_links -eq 0 ]; then
        echo "✅ All documentation links are valid"
    else
        echo "⚠️  Found $broken_links potential broken links"
    fi
}

# Function to generate documentation report
generate_report() {
    echo "📊 Generating documentation report..."
    
    local report_file="$PROJECT_ROOT/documentation-report.md"
    
    cat > "$report_file" << EOF
# Documentation Report

Generated on: $(date)

## Project Structure

\`\`\`
$(find "$PROJECT_ROOT" -name "*.md" -o -name "*.yml" -o -name "docfx.json" | sort)
\`\`\`

## XML Documentation Files

\`\`\`
$(find "$PROJECT_ROOT" -name "*.xml" -path "*/bin/Release/*" 2>/dev/null | sort)
\`\`\`

## Documentation Site

EOF

    if [ -d "$PROJECT_ROOT/_site" ]; then
        echo "- ✅ Site generated successfully" >> "$report_file"
        echo "- 📁 Location: \`_site/\`" >> "$report_file"
        echo "- 📊 Size: $(du -sh _site 2>/dev/null | cut -f1 || echo "unknown")" >> "$report_file"
        echo "- 📄 Files: $(find _site -type f | wc -l) files" >> "$report_file"
    else
        echo "- ❌ No documentation site found" >> "$report_file"
    fi
    
    echo "📄 Documentation report saved to: $report_file"
}

# Main execution
main() {
    local command="${1:-generate}"
    
    case "$command" in
        "check")
            echo "🔍 Checking documentation prerequisites..."
            check_tool "dotnet" || exit 1
            install_docfx || exit 1
            validate_xml_docs
            ;;
        "generate")
            echo "🚀 Running complete documentation generation..."
            check_tool "dotnet" || exit 1
            install_docfx || exit 1
            validate_xml_docs
            generate_docfx
            validate_links
            generate_report
            ;;
        "serve")
            serve_docs
            ;;
        "validate")
            validate_xml_docs
            validate_links
            ;;
        "report")
            generate_report
            ;;
        *)
            echo "Usage: $0 {check|generate|serve|validate|report}"
            echo ""
            echo "Commands:"
            echo "  check     - Check prerequisites and XML documentation"
            echo "  generate  - Full documentation generation (default)"
            echo "  serve     - Serve documentation locally"
            echo "  validate  - Validate documentation and links"
            echo "  report    - Generate documentation report"
            exit 1
            ;;
    esac
}

main "$@"
