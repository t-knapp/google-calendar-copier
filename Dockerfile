FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
COPY --from=build-env /app/out .
VOLUME /config
ENTRYPOINT ["dotnet", "google-calendar-copier.dll", "/config/config.json"]
