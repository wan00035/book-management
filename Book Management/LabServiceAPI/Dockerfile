# Runtime image for running the ASP.NET Core app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy and restore project dependencies
COPY ["LabServiceAPI.csproj", "./"]
RUN dotnet restore "LabServiceAPI.csproj"

# Copy remaining source code and publish
COPY . .
RUN dotnet publish "LabServiceAPI.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LabServiceAPI.dll"]