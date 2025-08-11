#!/bin/bash

# Syncs all subtrees: adds if missing, pulls if existing.
# Usage: bash sync-subtrees.sh

set -e

REPO_OWNER="ulfbou"
TARGET_BRANCH="main"
PACKAGES_DIR="packages"

REPOS=(
  "Zentient.Abstractions"
  "Zentient.Core"
  "Zentient.DependencyInjection"
  "Zentient.Diagnostics"
  "Zentient.Observability"
  "Zentient.Policies"
  "Zentient.Metadata"
)

# Check for uncommitted changes
if ! git diff --quiet || ! git diff --cached --quiet; then
  echo "⚠️ You have uncommitted changes. Commit or stash them before running this script."
  exit 1
fi

for REPO in "${REPOS[@]}"; do
  REMOTE_URL="https://github.com/${REPO_OWNER}/${REPO}.git"
  PREFIX="${PACKAGES_DIR}/${REPO}"

  if [ -d "$PREFIX" ]; then
    echo "🔄 Updating subtree for $REPO..."
    git subtree pull --prefix="$PREFIX" "$REMOTE_URL" "$TARGET_BRANCH" --squash
  else
    echo "➕ Adding subtree for $REPO..."
    git subtree add --prefix="$PREFIX" "$REMOTE_URL" "$TARGET_BRANCH" --squash
  fi
done

echo "✅ All subtrees are now in sync."
