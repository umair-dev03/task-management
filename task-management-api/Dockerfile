# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TaskManagement.sln ./
COPY src/TaskManagement.Application/*.csproj ./TaskManagement.Application/
COPY src/TaskManagement.Domain/*.csproj ./TaskManagement.Domain/
COPY src/TaskManagement.Infrastructure/*.csproj ./TaskManagement.Infrastructure/
COPY src/TaskManagement.Api/*.csproj ./TaskManagement.Api/

# Use retry logic for restore (NuGet source is already present in official images)
RUN dotnet restore TaskManagement.Api/TaskManagement.Api.csproj --disable-parallel --interactive --no-cache || \
    dotnet restore TaskManagement.Api/TaskManagement.Api.csproj --disable-parallel --interactive --no-cache

# Copy the rest of the source code
COPY src/TaskManagement.Application/ ./TaskManagement.Application/
COPY src/TaskManagement.Domain/ ./TaskManagement.Domain/
COPY src/TaskManagement.Infrastructure/ ./TaskManagement.Infrastructure/
COPY src/TaskManagement.Api/ ./TaskManagement.Api/

WORKDIR /src/TaskManagement.Api

# Build and publish
RUN dotnet publish -c Release -o /app/out

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose port
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]
