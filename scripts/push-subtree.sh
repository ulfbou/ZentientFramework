#!/bin/bash

# Pushes local subtree changes back to their repos.
# Usage: bash push-subtrees.sh

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

for REPO in "${REPOS[@]}"; do
  REMOTE_URL="https://github.com/${REPO_OWNER}/${REPO}.git"
  PREFIX="${PACKAGES_DIR}/${REPO}"

  if [ -d "$PREFIX" ]; then
    echo "📤 Pushing subtree for $REPO..."
    git subtree push --prefix="$PREFIX" "$REMOTE_URL" "$TARGET_BRANCH" || {
      echo "⚠️ Push failed for $REPO — try pulling first."
    }
  else
    echo "⏭ Skipping $REPO (not present locally)."
  fi
done

echo "✅ Push completed for all present subtrees."
