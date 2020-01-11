FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out -r linux-arm

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim-arm64v8
WORKDIR /app
COPY --from=build-env /app/out .
VOLUME [ "/config", "/credentials" ]
ENTRYPOINT ["dotnet", "google-calendar-copier.dll", "/config/config.json", "/credentials/"]
