# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY SharedModels/SharedModels.csproj                         SharedModels/
COPY RuntimeViewer.Shared/RuntimeViewer.Shared.csproj         RuntimeViewer.Shared/
COPY RuntimeViewer/RuntimeViewer.csproj                       RuntimeViewer/

# Copy source and publish
COPY SharedModels/          SharedModels/
COPY RuntimeViewer.Shared/  RuntimeViewer.Shared/
COPY RuntimeViewer/         RuntimeViewer/

RUN dotnet publish RuntimeViewer/RuntimeViewer.csproj -c Release -o /app/publish

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Data volume for nodes.json config
VOLUME /data

ENV ASPNETCORE_URLS=http://+:8081

EXPOSE 8081

# Pass nodes.json path as argument; mount your config at /data/nodes.json
ENTRYPOINT ["dotnet", "RuntimeViewer.dll"]
CMD ["/data/nodes.json"]