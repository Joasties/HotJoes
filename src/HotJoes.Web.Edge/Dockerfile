FROM node:24.15.0-bookworm AS web-build
WORKDIR /source/src/HotJoes.Web.Vendor
COPY src/HotJoes.Web.Vendor/package*.json ./
RUN npm ci
COPY src/HotJoes.Web.Vendor/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS edge-build
WORKDIR /source
COPY . .
RUN dotnet restore src/HotJoes.Web.Edge/HotJoes.Web.Edge.csproj
RUN dotnet publish src/HotJoes.Web.Edge/HotJoes.Web.Edge.csproj --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install --yes --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=edge-build /app .
COPY --from=web-build /source/src/HotJoes.Web.Vendor/dist/hotjoes-web-vendor/browser ./wwwroot
ENTRYPOINT ["dotnet", "HotJoes.Web.Edge.dll"]
