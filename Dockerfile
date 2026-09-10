FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT
WORKDIR /source
COPY . .
RUN dotnet restore "${PROJECT}"
RUN dotnet publish "${PROJECT}" --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
ARG ENTRYPOINT_DLL
ENV ENTRYPOINT_DLL=${ENTRYPOINT_DLL}
RUN apt-get update && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["sh", "-c", "dotnet ${ENTRYPOINT_DLL}"]
