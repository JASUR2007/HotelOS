FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["services/room-service/HotelOS.RoomService.csproj", "services/room-service/"]
COPY ["shared/HotelOS.Shared/HotelOS.Shared.csproj", "shared/HotelOS.Shared/"]
RUN dotnet restore "services/room-service/HotelOS.RoomService.csproj"
COPY . .
RUN dotnet publish "services/room-service/HotelOS.RoomService.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*
EXPOSE 8084
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelOS.RoomService.dll"]