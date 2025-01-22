# ======= Build UserService =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-user
WORKDIR /app
COPY UserService/*.csproj ./UserService/
WORKDIR /app/UserService
RUN dotnet restore
COPY UserService/. .
RUN dotnet publish -c Release -o /app/publish

# ======= Build ProductCatalog =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-product
WORKDIR /app
COPY ProductCatalog/*.csproj ./ProductCatalog/
WORKDIR /app/ProductCatalog
RUN dotnet restore
COPY ProductCatalog/. .
RUN dotnet publish -c Release -o /app/publish

# ======= Build PaymentService =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-payment
WORKDIR /app
COPY PaymentService/*.csproj ./PaymentService/
WORKDIR /app/PaymentService
RUN dotnet restore
COPY PaymentService/. .
RUN dotnet publish -c Release -o /app/publish

# ======= Build ShoppingBasket =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-basket
WORKDIR /app
COPY ShoppingBasket/*.csproj ./ShoppingBasket/
WORKDIR /app/ShoppingBasket
RUN dotnet restore
COPY ShoppingBasket/. .
RUN dotnet publish -c Release -o /app/publish

# ======= Build UnifiedFrontend =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-frontend
WORKDIR /app
COPY UnifiedFrontend/*.csproj ./UnifiedFrontend/
WORKDIR /app/UnifiedFrontend
RUN dotnet restore
COPY UnifiedFrontend/. .
RUN dotnet publish -c Release -o /app/publish

# ======= Runtime Images =======

# UserService Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS user-runtime
WORKDIR /app
COPY --from=build-user /app/publish .
ENTRYPOINT ["dotnet", "userService.dll"]

# ProductCatalog Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS product-runtime
WORKDIR /app
COPY --from=build-product /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.dll"]

# PaymentService Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS payment-runtime
WORKDIR /app
COPY --from=build-payment /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]

# ShoppingBasket Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS basket-runtime
WORKDIR /app
COPY --from=build-basket /app/publish .
ENTRYPOINT ["dotnet", "ShoppingBasket.dll"]

# UnifiedFrontend Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS frontend-runtime
WORKDIR /app
COPY --from=build-frontend /app/publish .
ENTRYPOINT ["dotnet", "UnifiedFrontend.dll"]
