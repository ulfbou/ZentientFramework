#!/bin/bash
# Move Internal Tools to Secure Location
# This script moves all internal-only files to /tmp/internal to prevent accidental commits

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
INTERNAL_DIR="/tmp/internal"

# Color functions
red() { echo -e "\033[31m$1\033[0m"; }
green() { echo -e "\033[32m$1\033[0m"; }
yellow() { echo -e "\033[33m$1\033[0m"; }
blue() { echo -e "\033[34m$1\033[0m"; }
cyan() { echo -e "\033[36m$1\033[0m"; }

log() { echo "$(date '+%H:%M:%S') $1"; }
step() { log "$(cyan "🔄 $1")"; }
success() { log "$(green "✅ $1")"; }
error() { log "$(red "❌ $1")"; }
info() { log "$(blue "ℹ️  $1")"; }

echo ""
echo "$(cyan '🔒 SECURING INTERNAL TOOLS')"
echo "$(cyan '===========================')"
echo ""

# Create internal directory
setup_internal_dir() {
    step "Setting up secure internal directory..."
    
    rm -rf "$INTERNAL_DIR"
    mkdir -p "$INTERNAL_DIR/scripts"
    
    success "Internal directory created: $INTERNAL_DIR"
}

# Move internal scripts
move_internal_scripts() {
    step "Moving internal scripts..."
    
    cd "$REPO_ROOT"
    
    # List of files that should NOT be committed
    local internal_files=(
        "scripts/local-template-validation.sh"
        "scripts/test-cicd-pipeline.sh" 
        "scripts/commit-and-validate.sh"
        "cicd-readiness-report.md"
    )
    
    # Move files that exist
    for file in "${internal_files[@]}"; do
        if [[ -f "$file" ]]; then
            mv "$file" "$INTERNAL_DIR/$file"
            success "Moved: $file"
        else
            info "Not found: $file"
        fi
    done
    
    # Move any log files
    find . -maxdepth 1 -name "*.log" -exec mv {} "$INTERNAL_DIR/" \; 2>/dev/null || true
}

# Create quick access script
create_access_script() {
    step "Creating quick access script..."
    
    cat > "$INTERNAL_DIR/run-tools.sh" << 'EOF'
#!/bin/bash
# Quick access to internal tools

INTERNAL_DIR="/tmp/internal"

echo "🔒 Zentient Templates - Internal Tools"
echo ""
echo "1. CI/CD Testing:     bash $INTERNAL_DIR/scripts/test-cicd-pipeline.sh"
echo "2. Commit & Validate: bash $INTERNAL_DIR/scripts/commit-and-validate.sh"  
echo "3. Local Validation:  bash $INTERNAL_DIR/scripts/local-template-validation.sh"
echo ""

case "$1" in
    cicd) bash "$INTERNAL_DIR/scripts/test-cicd-pipeline.sh" ;;
    commit) bash "$INTERNAL_DIR/scripts/commit-and-validate.sh" "${@:2}" ;;
    local) bash "$INTERNAL_DIR/scripts/local-template-validation.sh" ;;
    *) echo "Usage: $0 [cicd|commit|local]" ;;
esac
EOF

    chmod +x "$INTERNAL_DIR/run-tools.sh"
    success "Created quick access script"
}

# Update .gitignore
update_gitignore() {
    step "Updating .gitignore..."
    
    cd "$REPO_ROOT"
    
    # Add patterns to .gitignore
    cat >> .gitignore << 'EOF'

# Internal tools (secured in /tmp/internal)
scripts/local-template-validation.sh
scripts/test-cicd-pipeline.sh
scripts/commit-and-validate.sh
cicd-readiness-report.md
*.log
EOF

    success "Updated .gitignore"
}

# Generate documentation
generate_docs() {
    step "Generating documentation..."
    
    cat > "$INTERNAL_DIR/README.md" << EOF
# Internal Tools - Secured Location

**Location**: \`$INTERNAL_DIR\`

## Tools Available

- **CI/CD Testing**: \`bash $INTERNAL_DIR/scripts/test-cicd-pipeline.sh\`
- **Commit & Validate**: \`bash $INTERNAL_DIR/scripts/commit-and-validate.sh\`
- **Local Validation**: \`bash $INTERNAL_DIR/scripts/local-template-validation.sh\`

## Quick Access

\`\`\`bash
bash $INTERNAL_DIR/run-tools.sh [cicd|commit|local]
\`\`\`

## Repository Scripts (Safe to Commit)

- \`scripts/maintenance-automation.sh\`
- \`scripts/generate-docs.sh\`  
- \`scripts/validate-nuget-metadata.sh\`

Internal tools are secured and won't be committed! 🔒
EOF

    success "Documentation created"
}

# Main execution
main() {
    setup_internal_dir
    move_internal_scripts
    create_access_script
    update_gitignore
    generate_docs
    
    echo ""
    success "🔒 Internal tools secured!"
    info "Access: bash $INTERNAL_DIR/run-tools.sh"
    echo ""
}

main "$@"
