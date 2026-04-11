docker rm -f hmi

dotnet build Server/Server.csproj -c Release -p:BuildDockerImage=true

docker save -o hmi-allinone.tar hmi-allinone:latest

docker load -i hmi-allinone.tar

docker run -d --name hmi -p 14840:14840 -p 14841:14841 -p 8080:8080 -p 8081:8081 -p 8088:8088 -p 5432:5432 -p 11434:11434 -v hmi-data:/data -v hmi-pgdata:/var/lib/postgresql/data hmi-allinone:latest

