# ======= Build PaymentService =======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-payment
WORKDIR /app
COPY *.csproj ./PaymentService/
WORKDIR /app/PaymentService
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish
# PaymentService Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS payment-runtime
WORKDIR /app
COPY --from=build-payment /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]
