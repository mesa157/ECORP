# ======= Build UnifiedFrontend =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-frontend
WORKDIR /app
COPY *.csproj ./UnifiedFrontend/
WORKDIR /app/UnifiedFrontend
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish
# UnifiedFrontend Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS frontend-runtime
WORKDIR /app
COPY --from=build-frontend /app/publish .
ENTRYPOINT ["dotnet", "UnifiedFrontend.dll"]