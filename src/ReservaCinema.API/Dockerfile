# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the necessary project files
COPY ["src/ReservaCinema.API/ReservaCinema.API.csproj", "src/ReservaCinema.API/"]
COPY ["src/ReservaCinema.Application/ReservaCinema.Application.csproj", "src/ReservaCinema.Application/"]
COPY ["src/ReservaCinema.Domain/ReservaCinema.Domain.csproj", "src/ReservaCinema.Domain/"]
COPY ["src/ReservaCinema.Infrastructure/ReservaCinema.Infrastructure.csproj", "src/ReservaCinema.Infrastructure/"]

# Restore dependencies for API only
RUN dotnet restore "src/ReservaCinema.API/ReservaCinema.API.csproj"

# Copy all source files
COPY src/ ./src/

# Build
RUN dotnet build "src/ReservaCinema.API/ReservaCinema.API.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "src/ReservaCinema.API/ReservaCinema.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080

# Run application
ENTRYPOINT ["dotnet", "ReservaCinema.API.dll"]
