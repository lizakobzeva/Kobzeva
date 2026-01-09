# stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /gymBackendC--.WebApi

# install deps
COPY gymBackendC--.WebApi/gymBackendC--.WebApi.csproj .
RUN dotnet restore

# copy source code
COPY gymBackendC--.WebApi .

# build and publish release
RUN dotnet publish -c Release -o /app/publish


# stage 2: run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "gymBackendC--.WebApi.dll"]
