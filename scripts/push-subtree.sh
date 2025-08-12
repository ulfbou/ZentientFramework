#!/bin/bash

<<<<<<< HEAD
# Check if a subtree name was provided as an argument
if [ -z "$1" ]; then
  echo "Error: Please provide the name of the subtree."
  echo "Usage: ./push-subtree.sh <SubtreeName>"
  exit 1
fi

SUBTREE_NAME="Zentient.$1"
PREFIX="packages/$SUBTREE_NAME"
REMOTE_NAME="$1"
BRANCH="main"

# Check if the subtree directory exists
if [ ! -d "$PREFIX" ]; then
  echo "fatal: '$PREFIX' does not exist; please check your directory structure."
  exit 1
fi

# Push the subtree
echo "git subtree push --prefix='$PREFIX' '$REMOTE_NAME' '$BRANCH'"
git subtree push --prefix="$PREFIX" "$REMOTE_NAME" "$BRANCH"

# Check the exit status of the push command
if [ $? -eq 0 ]; then
  echo "Subtree '$SUBTREE_NAME' pushed successfully to $REMOTE_NAME/$BRANCH."
else
  echo "Failed to push subtree '$SUBTREE_NAME'."
fi
=======
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
>>>>>>> 2296645ac4d3d1f6b1514311d8a85130076cc4ed
