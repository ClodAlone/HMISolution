# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files first for layer caching
COPY SharedModels/SharedModels.csproj                         SharedModels/
COPY Drivers/Drivers.Abstractions/Drivers.Abstractions.csproj Drivers/Drivers.Abstractions/
COPY Drivers/Drivers.Csv/Drivers.Csv.csproj                   Drivers/Drivers.Csv/
COPY Drivers/Drivers.EtherNetIP/Drivers.EtherNetIP.csproj     Drivers/Drivers.EtherNetIP/
COPY Drivers/Drivers.Knx/Drivers.Knx.csproj                   Drivers/Drivers.Knx/
COPY Drivers/Drivers.Modbus/Drivers.Modbus.csproj             Drivers/Drivers.Modbus/
COPY Drivers/Drivers.Mqtt/Drivers.Mqtt.csproj                 Drivers/Drivers.Mqtt/
COPY Drivers/Drivers.OpcUaClient/Drivers.OpcUaClient.csproj   Drivers/Drivers.OpcUaClient/
COPY Drivers/Drivers.Rest/Drivers.Rest.csproj                 Drivers/Drivers.Rest/
COPY Drivers/Drivers.S7/Drivers.S7.csproj                     Drivers/Drivers.S7/
COPY Drivers/Drivers.Simulation/Drivers.Simulation.csproj     Drivers/Drivers.Simulation/
COPY Drivers/Drivers.Sql/Drivers.Sql.csproj                   Drivers/Drivers.Sql/
COPY Drivers/Drivers.Tcp/Drivers.Tcp.csproj                   Drivers/Drivers.Tcp/
COPY Server/Server.csproj                                     Server/

# Copy source
COPY SharedModels/ SharedModels/
COPY Drivers/      Drivers/
COPY Server/       Server/

RUN dotnet publish Server/Server.csproj -c Release -o /app/publish

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Default config / data volume mount point
VOLUME /data

# OPC UA default port (4840) + diagnostics port (14841)
EXPOSE 4840
EXPOSE 14841

# Pass nodes.json path as argument; mount your config at /data/nodes.json
ENTRYPOINT ["dotnet", "Server.dll"]
CMD ["/data/nodes.json"]