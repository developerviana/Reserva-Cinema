# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ReservaCinema.API/ReservaCinema.API.csproj", "src/ReservaCinema.API/"]
COPY ["src/ReservaCinema.Application/ReservaCinema.Application.csproj", "src/ReservaCinema.Application/"]

RUN dotnet restore "src/ReservaCinema.API/ReservaCinema.API.csproj"

COPY . .
WORKDIR "/src/src/ReservaCinema.API"
RUN dotnet build "ReservaCinema.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ReservaCinema.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ReservaCinema.API.dll"]
