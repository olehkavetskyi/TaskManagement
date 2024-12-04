# Stage 1: Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Api/Api.csproj", "Api/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Api/Api.csproj"


# Set the working directory to the Api folder
WORKDIR "/src/Api"
# Copy the rest of the application code
COPY . .


# Build the application
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the application
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Runtime image
FROM base AS final
WORKDIR /app

# Copy the published application from the previous stage
COPY --from=publish /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Api.dll"]
