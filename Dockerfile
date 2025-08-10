{
  "name": "Zentient.Templates Dev Container",
  "build": {
    "dockerfile": "Dockerfile"
  },
  "features": {
    "ghcr.io/devcontainers/features/node:1": {
      "version": "lts"
    },
    "ghcr.io/devcontainers/features/github-cli:1": {},
    "ghcr.io/devcontainers/features/docker-in-docker:2": {}  // Enables Docker CLI in the container
  },
  "postCreateCommand": "dotnet tool install -g docfx dotnet-format coverlet.console reportgenerator && dotnet restore && dotnet build",
  "customizations": {
    "vscode": {
      "settings": {
        "terminal.integrated.defaultProfile.linux": "pwsh",
        "terminal.integrated.profiles.linux": {
          "bash": {
            "path": "/bin/bash"
          },
          "pwsh": {
            "path": "/usr/bin/pwsh"
          }
        }
      },
      "extensions": [
        "ms-dotnettools.csharp",
        "ms-azuretools.vscode-docker",
        "eamodio.gitlens",
        "yzhang.markdown-all-in-one"
      ]
    }
  },
  "remoteUser": "vscode"
}
