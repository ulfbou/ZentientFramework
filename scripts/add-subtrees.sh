#!/bin/bash

# This script adds multiple repositories as git subtrees under the packages/ folder
# Usage: bash add-subtrees.sh
# Make sure you are in the root of your ZentientFramework repo and it is initialized

REPO_OWNER="ulfbou"
TARGET_BRANCH="main" # Change if your repos use a different branch name
PACKAGES_DIR="packages"

# List of repo names to add as subtrees
REPOS=(
  "Zentient.Abstractions"
  "Zentient.Core"
  "Zentient.Results"
  "Zentient.Endpoints"
  "Zentient.Telemetry"
  "Zentient.Policies"
  "Zentient.Formatters"
  # Add more repo names here as needed
)

for REPO in "${REPOS[@]}"; do
  REMOTE_URL="https://github.com/${REPO_OWNER}/${REPO}.git"
  PREFIX="${PACKAGES_DIR}/${REPO}"
  
  echo "Adding subtree for $REPO at $PREFIX from $REMOTE_URL"
  git subtree add --prefix="$PREFIX" "$REMOTE_URL" "$TARGET_BRANCH" --squash
done

echo "All subtrees have been added. Don't forget to push your changes!"
