#!/bin/bash

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