#!/bin/bash

# Script to validate GitHub Actions workflow files
# Usage: ./validate-workflows.sh

set -e

echo "🔍 Validating GitHub Actions Workflows..."
echo ""

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WORKFLOWS_DIR="$SCRIPT_DIR/../workflows"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Counter for results
total=0
passed=0
failed=0

# Function to validate YAML syntax
validate_yaml() {
    local file=$1
    local filename=$(basename "$file")
    
    total=$((total + 1))
    
    echo -n "Validating $filename... "
    
    # Check if file exists
    if [ ! -f "$file" ]; then
        echo -e "${RED}✗ File not found${NC}"
        failed=$((failed + 1))
        return 1
    fi
    
    # Validate YAML syntax using Python
    if python3 -c "
import yaml
import sys
try:
    with open('$file', 'r', encoding='utf-8') as f:
        yaml.safe_load(f)
    sys.exit(0)
except Exception as e:
    print(f'Error: {e}', file=sys.stderr)
    sys.exit(1)
" 2>/dev/null; then
        echo -e "${GREEN}✓ Valid${NC}"
        passed=$((passed + 1))
        return 0
    else
        echo -e "${RED}✗ Invalid YAML${NC}"
        failed=$((failed + 1))
        return 1
    fi
}

# Function to check for required fields
check_required_fields() {
    local file=$1
    local filename=$(basename "$file")
    
    echo -n "Checking required fields in $filename... "
    
    # Check for 'name' field
    if ! grep -q "^name:" "$file"; then
        echo -e "${YELLOW}⚠ Missing 'name' field${NC}"
        return 1
    fi
    
    # Check for 'on' field
    if ! grep -q "^on:" "$file"; then
        echo -e "${YELLOW}⚠ Missing 'on' field${NC}"
        return 1
    fi
    
    # Check for 'jobs' field
    if ! grep -q "^jobs:" "$file"; then
        echo -e "${YELLOW}⚠ Missing 'jobs' field${NC}"
        return 1
    fi
    
    echo -e "${GREEN}✓ All required fields present${NC}"
    return 0
}

# Main validation
echo "Found workflows:"
ls -1 "$WORKFLOWS_DIR"/*.yml 2>/dev/null || {
    echo -e "${RED}No workflow files found!${NC}"
    exit 1
}
echo ""

# Validate each workflow file
for file in "$WORKFLOWS_DIR"/*.yml; do
    validate_yaml "$file"
    check_required_fields "$file"
    echo ""
done

# Print summary
echo "================================================"
echo "Summary:"
echo "  Total workflows: $total"
echo -e "  ${GREEN}Passed: $passed${NC}"
if [ $failed -gt 0 ]; then
    echo -e "  ${RED}Failed: $failed${NC}"
fi
echo "================================================"

# Exit with error if any validation failed
if [ $failed -gt 0 ]; then
    echo -e "${RED}❌ Validation failed!${NC}"
    exit 1
else
    echo -e "${GREEN}✅ All workflows are valid!${NC}"
    exit 0
fi
