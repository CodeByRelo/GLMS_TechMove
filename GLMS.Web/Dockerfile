FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .

COPY GLMS.Web/ GLMS.Web/
COPY GLMS.Core/ GLMS.Core/
COPY GLMS.Infrastructure/ GLMS.Infrastructure/

RUN dotnet restore GLMS.Web/GLMS.Web.csproj

WORKDIR /src/GLMS.Web
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GLMS.Web.dll"]