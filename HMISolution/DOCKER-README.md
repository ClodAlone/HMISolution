# AI Core HMI - Docker Deployment Guide

## Quick Start - All-in-One Deployment

The easiest way to run AI Core HMI with all services (Server, Editor, Viewer, PostgreSQL, and Ollama) is using docker-compose:

### Option 1: Using Docker Compose (Recommended)

```bash
# Download the docker-compose file
curl -O https://raw.githubusercontent.com/ClodAlone/HMISolution/master/docker-compose.allinone.yml

# Start all services
docker-compose -f docker-compose.allinone.yml up -d

# View logs
docker-compose -f docker-compose.allinone.yml logs -f

# Stop all services
docker-compose -f docker-compose.allinone.yml down
```

After startup, access the services at:

| Service | URL | Description |
|---------|-----|-------------|
| Web Editor | http://localhost:8080 | Project editor with AI assistant |
| Runtime Viewer | http://localhost:8088 | Operator interface with NL query |
| OPC UA Server | opc.tcp://localhost:14840 | Industrial data server |
| REST API | http://localhost:14841 | Diagnostics and REST endpoints |
| Ollama | http://localhost:11434 | Local LLM service |
| PostgreSQL | localhost:5432 | Historical database |

### Option 2: Individual Services

If you prefer to run services separately:

```bash
# Pull images
docker pull clodprogea/hmi-server:latest
docker pull clodprogea/hmi-editor:latest
docker pull clodprogea/hmi-viewer:latest

# Run Server (OPC UA + REST API)
docker run -d --name hmi-server \
  -p 14840:14840 \
  -p 14841:14841 \
  -v hmi-data:/data \
  clodprogea/hmi-server:latest

# Run Editor (Web UI)
docker run -d --name hmi-editor \
  -p 8080:8080 \
  -v hmi-data:/data \
  clodprogea/hmi-editor:latest

# Run Viewer (Runtime UI)
docker run -d --name hmi-viewer \
  -p 8088:8088 \
  -v hmi-data:/data \
  clodprogea/hmi-viewer:latest
```

## Available Docker Images

All images are publicly available on Docker Hub:

- **`clodprogea/hmi-server:latest`** - OPC UA Server + Alarm Engine + Anomaly Detection (255MB)
- **`clodprogea/hmi-editor:latest`** - Blazor Web Editor + AI Assistant (133MB)
- **`clodprogea/hmi-viewer:latest`** - Runtime Viewer + NL Query Chat (116MB)

## Environment Variables

### HMI Server
- `POSTGRES_CONNECTION` - PostgreSQL connection string
- `OLLAMA_HOST` - Ollama service URL (default: http://ollama:11434)

### HMI Editor
- `ASPNETCORE_URLS` - Listening URLs (default: http://+:8080)
- `POSTGRES_CONNECTION` - PostgreSQL connection string
- `OLLAMA_HOST` - Ollama service URL
- `OPENAI_API_KEY` - OpenAI API key (optional)
- `GEMINI_API_KEY` - Google Gemini API key (optional)
- `CLAUDE_API_KEY` - Anthropic Claude API key (optional)

### HMI Viewer
- `ASPNETCORE_URLS` - Listening URLs (default: http://+:8088)
- `OPC_UA_SERVER` - OPC UA server endpoint
- `REST_API_URL` - REST API base URL
- `OLLAMA_HOST` - Ollama service URL

## Volumes

The docker-compose setup creates three persistent volumes:

- **`hmi-data`** - Shared application data (project files, configs)
- **`hmi-pgdata`** - PostgreSQL database files
- **`hmi-ollama`** - Ollama models and cache

## Upgrading

To update to the latest version:

```bash
# Using docker-compose
docker-compose -f docker-compose.allinone.yml pull
docker-compose -f docker-compose.allinone.yml up -d

# Or for individual images
docker pull clodprogea/hmi-server:latest
docker pull clodprogea/hmi-editor:latest
docker pull clodprogea/hmi-viewer:latest
```

## Troubleshooting

### Ollama model loading takes time
The first request to Ollama after container start will load the model into memory (30-90 seconds). Subsequent requests are instant.

### PostgreSQL connection errors
Ensure PostgreSQL is healthy before starting the HMI services. The docker-compose file includes health checks to manage this automatically.

### Port conflicts
If ports are already in use, modify the port mappings in docker-compose.allinone.yml or your docker run commands.

## Building from Source

```bash
# Clone the repository
git clone https://github.com/ClodAlone/HMISolution.git
cd HMISolution

# Build all images
docker-compose -f docker-compose.allinone.yml build

# Run
docker-compose -f docker-compose.allinone.yml up -d
```

## Support

- **Documentation**: https://clodalone.github.io/HMISolution
- **Issues**: https://github.com/ClodAlone/HMISolution/issues
- **Discussions**: https://github.com/ClodAlone/HMISolution/discussions

---

**Note**: The documentation previously referenced `clodprogea/aicorehmi:latest` which was a planned all-in-one image. We now recommend using docker-compose for the complete stack, or the individual service images listed above for more flexibility.
