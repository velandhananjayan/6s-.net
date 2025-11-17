# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore the main API project
RUN dotnet restore "ApplicationTrackingSystem.API/ApplicationTrackingSystem.API.csproj"

# Build and publish
RUN dotnet publish "ApplicationTrackingSystem.API/ApplicationTrackingSystem.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ApplicationTrackingSystem.API.dll"]
