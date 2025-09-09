FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore templates.sln
RUN dotnet build templates.sln --configuration Release --no-restore
RUN dotnet test templates.sln --no-build --verbosity normal
RUN dotnet pack templates.sln --configuration Release --no-build --output /artifacts

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /artifacts ./artifacts
# This image is for CI/CD and packing, not for running an app
CMD ["/bin/bash"]
