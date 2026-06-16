# API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution first
COPY *.sln .

# Copy all projects
COPY GLMS.API/ GLMS.API/
COPY GLMS.Core/ GLMS.Core/
COPY GLMS.Infrastructure/ GLMS.Infrastructure/

# Restore API project
RUN dotnet restore GLMS.API/GLMS.API.csproj

# Build & publish
WORKDIR /src/GLMS.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GLMS.API.dll"]