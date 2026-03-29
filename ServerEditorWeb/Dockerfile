# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY SharedModels/SharedModels.csproj         SharedModels/
COPY ServerEditorWeb/ServerEditorWeb.csproj   ServerEditorWeb/

# Copy source and publish
COPY SharedModels/     SharedModels/
COPY ServerEditorWeb/  ServerEditorWeb/

RUN dotnet publish ServerEditorWeb/ServerEditorWeb.csproj -c Release -o /app/publish

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

LABEL org.opencontainers.image.source="https://github.com/ClodAlone/HMISolution"
LABEL org.opencontainers.image.description="HMI Server Editor (Blazor Web UI)"

WORKDIR /app

COPY --from=build /app/publish .

# Data volume for projects, symbol libraries, etc.
VOLUME /data

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ServerEditorWeb.dll"]