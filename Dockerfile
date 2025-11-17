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

# Copy published app from build stage
COPY --from=build /app/publish .

# Bind to the port provided by Render
ENV PORT 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Expose the port for the container
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "ApplicationTrackingSystem.API.dll"]
