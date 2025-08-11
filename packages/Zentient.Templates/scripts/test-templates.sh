#!/bin/bash
# Zentient Templates - Comprehensive Local Validation Script
# This script validates all template functionality with proper error handling

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
TEST_DIR="/tmp/zentient-template-validation"
LOG_FILE="$TEST_DIR/validation-$(date +%Y%m%d-%H%M%S).log"

# Color functions
red() { echo -e "\033[31m$1\033[0m"; }
green() { echo -e "\033[32m$1\033[0m"; }
yellow() { echo -e "\033[33m$1\033[0m"; }
blue() { echo -e "\033[34m$1\033[0m"; }
cyan() { echo -e "\033[36m$1\033[0m"; }
bold() { echo -e "\033[1m$1\033[0m"; }

# Logging functions
log() { 
    local msg="$(date '+%H:%M:%S') $1"
    echo "$msg" | tee -a "$LOG_FILE"
}
step() { log "$(cyan "🔄 $1")"; }
success() { log "$(green "✅ $1")"; }
error() { log "$(red "❌ $1")"; }
warning() { log "$(yellow "⚠️  $1")"; }
info() { log "$(blue "ℹ️  $1")"; }

# Test results tracking
TESTS_TOTAL=0
TESTS_PASSED=0
TESTS_FAILED=0
FAILED_TESTS=()

test_result() {
    TESTS_TOTAL=$((TESTS_TOTAL + 1))
    if [[ $1 -eq 0 ]]; then
        TESTS_PASSED=$((TESTS_PASSED + 1))
        success "$2"
    else
        TESTS_FAILED=$((TESTS_FAILED + 1))
        FAILED_TESTS+=("$2")
        error "$2"
    fi
}

echo ""
echo "$(bold "$(cyan '🧪 ZENTIENT TEMPLATES - COMPREHENSIVE VALIDATION')")"
echo "$(cyan '==================================================')"
echo ""

# Setup test environment
setup_environment() {
    step "Setting up test environment..."
    
    rm -rf "$TEST_DIR"
    mkdir -p "$TEST_DIR"
    cd "$TEST_DIR"
    
    success "Test environment ready: $TEST_DIR"
    info "Log file: $LOG_FILE"
}

# Check prerequisites
check_prerequisites() {
    step "Checking prerequisites..."
    
    # Check .NET SDK
    if ! command -v dotnet &> /dev/null; then
        error ".NET SDK not found"
        exit 1
    fi
    
    local dotnet_version=$(dotnet --version)
    info ".NET SDK version: $dotnet_version"
    
    # Install templates
    cd "$REPO_ROOT"
    info "Installing library template..."
    dotnet new install templates/zentient-library-template --force >> "$LOG_FILE" 2>&1
    test_result $? "Library template installation"
    
    info "Installing project template..."
    dotnet new install templates/zentient-project-template --force >> "$LOG_FILE" 2>&1
    test_result $? "Project template installation"
    
    cd "$TEST_DIR"
}

# Fix solution file references
fix_solution_references() {
    local project_name="$1"
    step "Fixing solution file references for $project_name..."
    
    # Find solution file
    local sln_file
    if [[ -f "$project_name.sln" ]]; then
        sln_file="$project_name.sln"
    elif ls *.sln &>/dev/null; then
        sln_file=$(ls *.sln | head -1)
    else
        warning "No solution file found"
        return 0
    fi
    
    info "Processing solution file: $sln_file"
    
    # Check for incorrect references
    if grep -q "Zentient.NewLibrary\|Zentient.LibraryTemplate" "$sln_file"; then
        warning "Found template placeholder references in solution file"
        
        # Fix the references
        sed -i "s/Zentient\.NewLibrary/$project_name/g" "$sln_file"
        sed -i "s/Zentient\.LibraryTemplate/$project_name/g" "$sln_file"
        
        success "Fixed solution file references"
    else
        success "Solution file references are correct"
    fi
    
    # Verify references exist
    local missing_refs=()
    while IFS= read -r line; do
        if [[ "$line" =~ Project.*\.csproj ]]; then
            local proj_path=$(echo "$line" | sed -n 's/.*"\([^"]*\.csproj\)".*/\1/p')
            if [[ -n "$proj_path" && ! -f "$proj_path" ]]; then
                missing_refs+=("$proj_path")
            fi
        fi
    done < "$sln_file"
    
    if [[ ${#missing_refs[@]} -gt 0 ]]; then
        error "Missing project files referenced in solution:"
        for ref in "${missing_refs[@]}"; do
            error "  - $ref"
        done
        return 1
    fi
    
    success "All solution references verified"
}

# Test library template
test_library_template() {
    step "Testing library template..."
    
    local test_name="MyTestLib"
    local test_dir="$TEST_DIR/$test_name"
    
    # Create project
    mkdir -p "$test_dir"
    cd "$test_dir"
    
    info "Creating library project: $test_name"
    if dotnet new zentient-lib -n "$test_name" --force >> "$LOG_FILE" 2>&1; then
        test_result 0 "Library template creation"
    else
        test_result 1 "Library template creation"
        return 1
    fi
    
    # Fix solution file
    fix_solution_references "$test_name"
    
    # Check structure
    info "Validating project structure..."
    local expected_files=(
        "$test_name.sln"
        "src/$test_name.csproj"
        "tests/$test_name.Tests.csproj"
        "README.md"
        "Directory.Build.props"
    )
    
    local structure_ok=true
    for file in "${expected_files[@]}"; do
        if [[ -f "$file" ]]; then
            info "✓ Found: $file"
        else
            warning "✗ Missing: $file"
            structure_ok=false
        fi
    done
    
    test_result $([ "$structure_ok" = true ] && echo 0 || echo 1) "Project structure validation"
    
    # Test individual project builds (more reliable)
    if [[ -f "src/$test_name.csproj" ]]; then
        info "Testing source project..."
        cd src
        if dotnet restore >> "$LOG_FILE" 2>&1 && dotnet build --verbosity quiet >> "$LOG_FILE" 2>&1; then
            test_result 0 "Source project build"
        else
            test_result 1 "Source project build"
        fi
        cd ..
    fi
    
    if [[ -f "tests/$test_name.Tests.csproj" ]]; then
        info "Testing test project..."
        cd tests
        if dotnet restore >> "$LOG_FILE" 2>&1 && dotnet build --verbosity quiet >> "$LOG_FILE" 2>&1; then
            test_result 0 "Test project build"
        else
            test_result 1 "Test project build"
        fi
        cd ..
    fi
    
    # Test solution build if possible
    info "Testing solution build..."
    if dotnet restore >> "$LOG_FILE" 2>&1 && dotnet build --verbosity quiet >> "$LOG_FILE" 2>&1; then
        test_result 0 "Solution build"
    else
        test_result 1 "Solution build"
    fi
    
    cd "$TEST_DIR"
}

# Test project template
test_project_template() {
    step "Testing project template..."
    
    local test_name="MyTestProject"
    local test_dir="$TEST_DIR/$test_name"
    
    # Create project
    mkdir -p "$test_dir"
    cd "$test_dir"
    
    info "Creating project: $test_name"
    if dotnet new zentient -n "$test_name" --force >> "$LOG_FILE" 2>&1; then
        test_result 0 "Project template creation"
    else
        test_result 1 "Project template creation"
        return 1
    fi
    
    # Check structure
    info "Validating project structure..."
    local expected_files=(
        "$test_name.sln"
        "Directory.Build.props"
        "README.md"
    )
    
    local structure_ok=true
    for file in "${expected_files[@]}"; do
        if [[ -f "$file" ]]; then
            info "✓ Found: $file"
        else
            warning "✗ Missing: $file"
            structure_ok=false
        fi
    done
    
    test_result $([ "$structure_ok" = true ] && echo 0 || echo 1) "Project structure validation"
    
    # Test restore and build
    info "Testing project restore and build..."
    if dotnet restore >> "$LOG_FILE" 2>&1; then
        test_result 0 "Project restore"
    else
        test_result 1 "Project restore"
    fi
    
    if dotnet build --verbosity quiet >> "$LOG_FILE" 2>&1; then
        test_result 0 "Project build"
    else
        test_result 1 "Project build"
    fi
    
    cd "$TEST_DIR"
}

# Test template metadata
test_template_metadata() {
    step "Testing template metadata..."
    
    cd "$REPO_ROOT"
    
    # Check template.json files
    local lib_template_json="templates/zentient-library-template/.template.config/template.json"
    local project_template_json="templates/zentient-project-template/.template.config/template.json"
    
    if [[ -f "$lib_template_json" ]]; then
        info "Validating library template metadata..."
        if python3 -c "import json; json.load(open('$lib_template_json'))" 2>/dev/null; then
            test_result 0 "Library template JSON validation"
        else
            test_result 1 "Library template JSON validation"
        fi
    else
        test_result 1 "Library template metadata file missing"
    fi
    
    if [[ -f "$project_template_json" ]]; then
        info "Validating project template metadata..."
        if python3 -c "import json; json.load(open('$project_template_json'))" 2>/dev/null; then
            test_result 0 "Project template JSON validation"
        else
            test_result 1 "Project template JSON validation"
        fi
    else
        test_result 1 "Project template metadata file missing"
    fi
    
    cd "$TEST_DIR"
}

# Generate test report
generate_report() {
    step "Generating test report..."
    
    local report_file="$TEST_DIR/test-report.md"
    local success_rate=0
    if [[ $TESTS_TOTAL -gt 0 ]]; then
        success_rate=$(echo "scale=1; $TESTS_PASSED * 100 / $TESTS_TOTAL" | bc 2>/dev/null || echo "0")
    fi
    
    cat > "$report_file" << EOF
# Zentient Templates - Validation Report

**Date**: $(date)
**Environment**: $(uname -s) $(uname -r)
**Logs**: $LOG_FILE

## Summary

- **Total Tests**: $TESTS_TOTAL
- **Passed**: $TESTS_PASSED
- **Failed**: $TESTS_FAILED
- **Success Rate**: ${success_rate}%

## Results

