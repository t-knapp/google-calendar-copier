FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out -r linux-arm

# Build runtime image
# ... see tags to use other image https://hub.docker.com/_/microsoft-dotnet-runtime/
FROM mcr.microsoft.com/dotnet/runtime:5.0.7-buster-slim-arm32v7
WORKDIR /app
COPY --from=build-env /app/out .
VOLUME [ "/config", "/credentials" ]
ENTRYPOINT ["dotnet", "google-calendar-copier.dll", "/config/config.json", "/credentials/"]
