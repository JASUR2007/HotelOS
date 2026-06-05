FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/payment-service/HotelOS.PaymentService.csproj", "services/payment-service/"]
COPY ["shared/HotelOS.Shared/HotelOS.Shared.csproj", "shared/HotelOS.Shared/"]
RUN dotnet restore "services/payment-service/HotelOS.PaymentService.csproj"
COPY . .
RUN dotnet publish "services/payment-service/HotelOS.PaymentService.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*
EXPOSE 8087
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelOS.PaymentService.dll"]