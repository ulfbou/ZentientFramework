# Start from official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install dependencies for .NET, Docker CLI, and other tools
RUN apt-get update && \
    apt-get install -y wget apt-transport-https ca-certificates curl gnupg lsb-release bash pwsh && \
    # Install Docker CLI
    curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null && \
    apt-get update && \
    apt-get install -y docker-ce-cli && \
    rm -rf /var/lib/apt/lists/*

# Install .NET 9 SDK (version 9.0.304)
ENV DOTNET9_VERSION=9.0.304
RUN wget https://dotnetcli.azureedge.net/dotnet/Sdk/9.0.304/dotnet-sdk-9.0.304-linux-x64.tar.gz -O dotnet9.tar.gz && \
    mkdir -p /usr/share/dotnet9 && \
    tar -zxf dotnet9.tar.gz -C /usr/share/dotnet9 && \
    rm dotnet9.tar.gz

# Set environment variables
ENV PATH="/usr/share/dotnet9:${PATH}"
ENV DOTNET_ROOT="/usr/share/dotnet9"

# Ensure bash and pwsh available
SHELL ["/bin/bash", "-c"]
