# ======= Build ShoppingBasket =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-basket
WORKDIR /app
COPY *.csproj ./ShoppingBasket/
WORKDIR /app/ShoppingBasket
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish
# ShoppingBasket Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS basket-runtime
WORKDIR /app
COPY --from=build-basket /app/publish .
ENTRYPOINT ["dotnet", "ShoppingBasket.dll"]