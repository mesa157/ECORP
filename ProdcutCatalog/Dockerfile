# ======= Build ProductCatalog =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-product
WORKDIR /app
COPY *.csproj ./ProductCatalog/
WORKDIR /app/ProductCatalog
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish
# ProductCatalog Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS product-runtime
WORKDIR /app
COPY --from=build-product /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.dll"]