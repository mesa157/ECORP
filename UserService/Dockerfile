# ======= Build UserService =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-user
WORKDIR /app
COPY *.csproj ./UserService/
WORKDIR /app/UserService
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish
# UserService Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS user-runtime
WORKDIR /app
COPY --from=build-user /app/publish .
ENTRYPOINT ["dotnet", "userService.dll"]