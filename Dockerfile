# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy the solution file and restore dependencies
COPY *.sln .
COPY TheBench.Api/*.csproj TheBench.Api/
RUN dotnet restore

# Copy the rest of the source code and build the application
COPY . .
WORKDIR /source/TheBench.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "TheBench.Api.dll"]

# Expose the port the app runs on
EXPOSE 8080