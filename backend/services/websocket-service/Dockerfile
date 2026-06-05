FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/websocket-service/HotelOS.WebsocketService.csproj", "services/websocket-service/"]
COPY ["shared/HotelOS.Shared/HotelOS.Shared.csproj", "shared/HotelOS.Shared/"]
RUN dotnet restore "services/websocket-service/HotelOS.WebsocketService.csproj"
COPY . .
RUN dotnet publish "services/websocket-service/HotelOS.WebsocketService.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelOS.WebsocketService.dll"]