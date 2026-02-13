# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy project files
COPY ["src/TaskManager.Api/TaskManager.Api.csproj", "src/TaskManager.Api/"]
RUN dotnet restore "src/TaskManager.Api/TaskManager.Api.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "src/TaskManager.Api/TaskManager.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "src/TaskManager.Api/TaskManager.Api.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose port 8080 (standard for containers)
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]
