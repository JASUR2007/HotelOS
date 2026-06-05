FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/maintenance-service/HotelOS.MaintenanceService.csproj", "services/maintenance-service/"]
COPY ["shared/HotelOS.Shared/HotelOS.Shared.csproj", "shared/HotelOS.Shared/"]
RUN dotnet restore "services/maintenance-service/HotelOS.MaintenanceService.csproj"
COPY . .
RUN dotnet publish "services/maintenance-service/HotelOS.MaintenanceService.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*
EXPOSE 8085
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelOS.MaintenanceService.dll"]