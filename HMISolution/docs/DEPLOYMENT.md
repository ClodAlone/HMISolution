# HMI Solution — Deployment Guide

This document covers building Docker images for the server-side components
and publishing the native desktop viewer.

---

## Architecture Overview

| Component | Type | Docker? | Default Port |
|---|---|---|---|
| **Server** | OPC UA server (console) | ✅ | 14840 (OPC UA), 14841 (diagnostics) |
| **ServerEditorWeb** | Blazor Server web app | ✅ | 8080 |
| **RuntimeViewer** | Blazor Server web app | ✅ | 8081 |
| **RuntimeViewer.Desktop** | Photino.NET native app | ❌ | _(local)_ |

> **RuntimeViewer.Desktop** wraps the RuntimeViewer web app inside a native
> window (Photino.NET). It requires a display server and cannot run inside a
> Docker container. See [Desktop Deployment](#5-desktop-deployment) below.

---

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) ≥ 24
- [Docker Compose](https://docs.docker.com/compose/) ≥ 2
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ≥ 10.0.200
  (for Desktop publishing and local development)

---

## 1. Repository Layout

All projects live as sibling directories under the repository root:

```
<repo-root>/
├── SharedModels/
├── Drivers/
│   ├── Drivers.Abstractions/
│   ├── Drivers.Csv/
│   ├── ... (11 driver projects)
├── Server/
│   └── Dockerfile
├── ServerEditorWeb/
│   └── Dockerfile
├── RuntimeViewer/
│   └── Dockerfile
├── RuntimeViewer.Shared/
├── RuntimeViewer.Desktop/
├── docker-compose.yml
└── .dockerignore
```

The Docker build context is the **repo root** so every project can resolve
its sibling `ProjectReference` paths.

---

## 2. Build & Run with Docker Compose (Recommended)

From the **repository root**:

```bash
# Build all three images
docker compose build

# Start all services (detached)
docker compose up -d

# View logs
docker compose logs -f

# Stop everything
docker compose down
```

After startup the services are available at:

| Service | URL |
|---|---|
| OPC UA Server | `opc.tcp://localhost:14840` |
| Server Diagnostics | `http://localhost:14841` |
| Web Editor | `http://localhost:8080` |
| Runtime Viewer | `http://localhost:8081` |

---

## 3. Build Individual Images

If you prefer to build images one at a time, run from the **repo root**:

```bash
# Server
docker build -f Server/Dockerfile -t hmi-server .

# Editor
docker build -f ServerEditorWeb/Dockerfile -t hmi-editor .

# Runtime Viewer
docker build -f RuntimeViewer/Dockerfile -t hmi-runtime .
```

### Run individually

```bash
# Server — mount your nodes.json from the host
docker run -d --name hmi-server \
  -p 14840:14840 -p 14841:14841 \
  -v /path/to/config:/data \
  hmi-server /data/nodes.json

# Editor
docker run -d --name hmi-editor \
  -p 8080:8080 \
  -v /path/to/projects:/data \
  hmi-editor

# Runtime Viewer
docker run -d --name hmi-runtime \
  -p 8081:8081 \
  -v /path/to/config:/data \
  hmi-runtime /data/nodes.json
```

---

## 4. Configuration

### 4.1 Node Configuration (nodes.json)

The **Server** and **RuntimeViewer** both read a `nodes.json` configuration
file. Place it inside the mounted `/data` volume:

```bash
# Copy your config into the named volume
docker cp ./my-project/nodes.json hmi-server:/data/nodes.json

# Or bind-mount a host directory in docker-compose.yml:
#   volumes:
#     - ./my-project:/data
```

### 4.2 Environment Variables

| Variable | Default | Description |
|---|---|---|
| `ASPNETCORE_URLS` | `http://+:8080` (editor), `http://+:8081` (viewer) | Listening URL |
| `ASPNETCORE_ENVIRONMENT` | `Production` | ASP.NET Core environment |

### 4.3 Ports

Override default port mappings in `docker-compose.yml` or with `-p`:

```yaml
# Example: expose editor on port 80
servereditor:
  ports:
    - "80:8080"
```

### 4.4 Volumes

| Named Volume | Used By | Purpose |
|---|---|---|
| `server-data` | Server, RuntimeViewer | nodes.json, logs, data files |
| `editor-data` | ServerEditorWeb | Project files, symbol libraries |

---

## 5. Desktop Deployment

**RuntimeViewer.Desktop** is a native windowing application using
[Photino.NET](https://github.com/nicholasrice/nicholasrice-photino.net) and
**cannot be containerized**. Deploy it via `dotnet publish`.

### 5.1 Publish for Windows

```powershell
cd RuntimeViewer.Desktop
dotnet publish -c Release -r win-x64 --self-contained -o ./publish/win-x64
```

The output in `./publish/win-x64/` contains a self-contained executable
with all dependencies.

### 5.2 Publish for Linux

```bash
cd RuntimeViewer.Desktop
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish/linux-x64
```

> **Note:** The Desktop app starts the RuntimeViewer web process internally.
> You must also publish RuntimeViewer (web) and place its output adjacent to
> the Desktop executable, or in a sibling `RuntimeViewer/` directory.

### 5.3 Bundle Desktop + Web Viewer Together

```powershell
# 1. Publish the web viewer
cd RuntimeViewer
dotnet publish -c Release -r win-x64 --self-contained -o ../RuntimeViewer.Desktop/publish/win-x64

# 2. Publish the desktop shell into the same folder
cd ../RuntimeViewer.Desktop
dotnet publish -c Release -r win-x64 --self-contained -o ./publish/win-x64
```

The Desktop executable discovers the `RuntimeViewer` executable in the same
directory automatically.

### 5.4 Run

```powershell
# Default
./publish/win-x64/RuntimeViewer.Desktop.exe "C:\path\to\nodes.json"

# Kiosk mode (fullscreen, no title bar)
./publish/win-x64/RuntimeViewer.Desktop.exe "C:\path\to\nodes.json" --kiosk
```

---

## 6. Production Tips

### 6.1 HTTPS / Reverse Proxy

In production, place an **nginx** or **Traefik** reverse proxy in front of
the Blazor apps to handle TLS termination:

```yaml
# Add to docker-compose.yml
services:
  nginx:
    image: nginx:alpine
    ports:
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./certs:/etc/nginx/certs:ro
    depends_on:
      - servereditor
      - runtimeviewer
```

### 6.2 Health Checks

Add health checks to `docker-compose.yml`:

```yaml
servereditor:
  healthcheck:
    test: ["CMD", "curl", "-f", "http://localhost:8080"]
    interval: 30s
    timeout: 10s
    retries: 3
```

### 6.3 Logging

- **Server** writes logs to `Logs/` inside its working directory (mapped to
  the `/data` volume).
- **ServerEditorWeb** and **RuntimeViewer** use default ASP.NET Core console
  logging, visible via `docker compose logs`.

### 6.4 Updating

```bash
# Pull latest code, rebuild, and restart
git pull
docker compose build
docker compose up -d
```

---

## 7. Troubleshooting

| Issue | Solution |
|---|---|
| `nodes.json not found` | Ensure the file is mounted at `/data/nodes.json` |
| Editor cannot reach Server | Containers share a Docker network; use `server:14840` as the OPC UA endpoint inside the editor |
| Static assets not loading in RuntimeViewer | The Dockerfile publishes in Release mode; `UseStaticWebAssets()` is handled automatically |
| Desktop cannot find RuntimeViewer.exe | Publish both projects to the same output directory (see §5.3) |
| Port conflict | Change the host port in `docker-compose.yml` (e.g., `"9090:8080"`) |


---

## 8. All-in-One Docker Image

The **all-in-one** image bundles every component into a single container.
By default only the **Web Editor** (and its infrastructure: TimescaleDB,
Ollama) starts. HMI Server and RuntimeViewer instances are managed
**dynamically from the editor UI** — no environment variables needed.

| Component | Default | Details |
|---|---|---|
| **ServerEditorWeb** | ✅ on | Blazor Server web editor — always starts |
| **HMI Server** | ❌ off | Start from the editor's Server Panel |
| **RuntimeViewer** | ❌ off | Start from the editor's Server Panel |
| **TimescaleDB** | ✅ on | PostgreSQL 16 + TimescaleDB extension for historical data |
| **Ollama** | ✅ on | Local LLM inference (default model: `mistral`) |
| **ffmpeg** | ✅ on | RTSP camera capture and video recording |
| **YOLOv8n** | ✅ on | ONNX model for object detection on camera streams |
| **All drivers** | ✅ on | Modbus, S7, MQTT, OPC UA Client, EtherNet/IP, KNX, CSV, REST, TCP, SQL, SparkplugB, Simulation |

### 8.1 Dynamic Service Management

The container starts in **editor-only mode**. From the editor's **Server
Panel** (🐳 Docker Service Instances), you can:

- **Start** one or more HMI Server instances, each with a different project
- **Start** RuntimeViewer instances on configurable ports
- **Stop** / **Remove** individual instances
- Toggle **Auto-start** per instance — controls whether it starts on next
  container boot

Configuration is saved to `/data/.hmi-services.json` and **persists across
container restarts**. The entrypoint reads this file on boot and
automatically starts all instances marked as auto-start.

### 8.2 Sample Projects

The image ships with sample projects that are seeded into `/data` on first
run. On subsequent starts, any **new** samples added in a newer image are
copied automatically (existing samples are not overwritten).

### 8.3 Port Map

| Port | Service |
|---|---|
| `14840` | OPC UA Server |
| `14841` | Server diagnostics / REST API |
| `8080` | ServerEditorWeb (Blazor) |
| `8081+` | RuntimeViewer (configurable per instance) |
| `8088` | Camera MJPEG streaming |
| `5432` | TimescaleDB (PostgreSQL) |
| `11434` | Ollama API |

### 8.4 Build the Image

From the **repository root**:

```bash
# Option A: Build directly
docker build -f Dockerfile.allinone -t hmi-allinone .

# Option B: Build via docker compose
docker compose --profile allinone build

# Option C: Auto-build on Release compile (from Visual Studio or CLI)
#   Set BuildDockerImage=true when building Server in Release mode:
dotnet build Server/Server.csproj -c Release -p:BuildDockerImage=true
```

> **Auto-build:** The `Server.csproj` imports `docker/DockerAllInOne.targets` which
> automatically runs `docker build` after a **Release** build when the
> MSBuild property `BuildDockerImage=true` is set. In Visual Studio, you can
> set this in your `.csproj.user` file or via the command line.

### 8.5 Export the Image

To transfer the image to another machine without a registry:

```bash
# Save to a tar archive (works on Windows, macOS, and Linux)
docker save -o hmi-allinone.tar hmi-allinone:latest

# Or via MSBuild target:
dotnet msbuild docker/DockerAllInOne.targets /t:ExportDockerAllInOne
```

The resulting `hmi-allinone.tar` file contains the complete image (~4 GB,
expands to ~13 GB when loaded).

> **Tip:** Compress the archive before transferring to reduce file size and
> speed up network transfers:
>
> ```bash
> # Linux / macOS
> gzip hmi-allinone.tar            # produces hmi-allinone.tar.gz
>
> # Windows (PowerShell)
> Compress-Archive -Path hmi-allinone.tar -DestinationPath hmi-allinone.tar.zip
> ```
>
> To load a gzipped archive directly:
> ```bash
> # Linux / macOS
> docker load < hmi-allinone.tar.gz
> ```

---

## 9. End-User: Import & Run the All-in-One Image

These instructions are for the **end user** who receives the `hmi-allinone.tar` file.

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) >= 24 installed and running
- At least **12 GB RAM** allocated to Docker (recommended: 16 GB)
  - Docker Desktop → Settings → Resources → Memory
- At least **30 GB free disk** (image ~13 GB uncompressed + data + Ollama models)
- Free Docker storage before loading (optional but recommended):
  ```bash
  docker system prune          # remove stopped containers and dangling images
  ```

### Step 1 — Load the Image

> **⚠ This step takes a long time (5–20 minutes).** The archive is ~4 GB
> compressed and expands to ~13 GB of image layers. `docker load` shows
> **no progress** until the very last layer is written, so it will appear to
> hang — this is normal. Do **not** cancel it prematurely.
>
> If it seems stuck for more than 30 minutes, ensure Docker Desktop has
> enough **RAM (≥ 12 GB)** and **free disk space (≥ 30 GB)** in Settings →
> Resources.

```bash
# Option A — stdin pipe (may show per-layer progress on some Docker versions)
docker load < hmi-allinone.tar

# Option B — file path
docker load -i hmi-allinone.tar
```

Verify it was loaded:

```bash
docker images hmi-allinone
# Should show:  hmi-allinone   latest   <id>   ~13 GB
```

### Step 2 — Run the Container

> **Note:** If a container named `hmi` already exists from a previous run, remove it first:
> ```bash
> docker rm -f hmi
> ```

```bash
docker run -d --name hmi \
  -p 14840:14840 -p 14841:14841 -p 8080:8080 -p 8081:8081 \
  -p 8088:8088 -p 5432:5432 -p 11434:11434 \
  -v hmi-data:/data -v hmi-pgdata:/var/lib/postgresql/data \
  hmi-allinone:latest
```

> **Tip:** Expose ports for all services you may use. The HMI Server and
> RuntimeViewer won't consume resources until you start them from the editor.

### Step 3 — Open the Editor & Start Services

Once the container is running, open a browser:

| Service | URL | Notes |
|---|---|---|
| **Web Editor** | [http://localhost:8080](http://localhost:8080) | ✅ always on |
| **TimescaleDB** | `postgresql://postgres:hmipass@localhost:5432/hmi` | ✅ always on |
| **Ollama API** | [http://localhost:11434](http://localhost:11434) | ✅ always on |

In the editor, open the **Server Panel** and use the **🐳 Docker Service
Instances** section to:

1. Select a sample project from the dropdown
2. Choose **Server** or **Viewer** type
3. Click **➕ Start**

Each instance you start will appear in the table with its PID and status.
Check the **Auto** checkbox to have it auto-start on the next container boot.
The configuration is persisted to `/data/.hmi-services.json`.

Once a server is running:

| Service | URL |
|---|---|
| **OPC UA Server** | `opc.tcp://localhost:14840` |
| **Camera Streams** | `http://localhost:8088/camera/{id}/stream` |
| **Server Diagnostics** | [http://localhost:14841](http://localhost:14841) |
| **Runtime Viewer** | `http://localhost:{port}` (as configured) |

### Step 4 — Sample Projects & Custom Projects

The image ships with **19 sample projects** that are automatically seeded
into `/data` on first run. Open the editor and browse the project list to
explore them.

To add your own project, copy it into the container:

```bash
# Copy a project folder from host to container
docker cp ./my-project hmi:/data/my-project

# Or bind-mount a host directory at startup
docker run -d --name hmi \
  -p 14840:14840 -p 8080:8080 -p 8081:8081 \
  -p 8088:8088 -p 14841:14841 -p 5432:5432 -p 11434:11434 \
  -v /path/to/projects:/data \
  -v hmi-pgdata:/var/lib/postgresql/data \
  hmi-allinone:latest
```

### Step 5 — View Logs

```bash
# All combined stdout
docker logs -f hmi

# Individual service logs inside the container
docker exec hmi cat /var/log/hmi/server.log
docker exec hmi cat /var/log/hmi/editor.log
docker exec hmi cat /var/log/hmi/viewer.log
docker exec hmi cat /var/log/ollama.log
docker exec hmi cat /var/log/postgresql.log
```

### Step 6 — Manage Ollama Models

```bash
# List installed models
docker exec hmi ollama list

# Pull an additional model
docker exec hmi ollama pull llama3.1

# Remove a model
docker exec hmi ollama rm mistral
```

### Step 7 — Stop & Remove

```bash
# Stop the container (data is preserved in volumes)
docker stop hmi

# Start it again
docker start hmi

# Remove the container (volumes/data survive)
docker rm hmi

# Remove everything including data
docker rm -f hmi
docker volume rm hmi-data hmi-pgdata
```

### Environment Variables

| Variable | Default | Description |
|---|---|---|
| `POSTGRES_USER` | `postgres` | TimescaleDB superuser name |
| `POSTGRES_PASSWORD` | `hmipass` | TimescaleDB superuser password |
| `POSTGRES_DB` | `hmi` | Default database name |
| `OLLAMA_MODEL` | `mistral` | LLM model to pull on first start |
| `OLLAMA_HOST` | `127.0.0.1:11434` | Ollama listen address |
| `HMI_DATA` | `/data` | Mount point for project files |
| `ASPNETCORE_ENVIRONMENT` | `Production` | ASP.NET Core environment |
| `ENABLE_SERVER` | `false` | _(deprecated)_ Legacy: start a server on boot. Use the editor instead. |
| `ENABLE_VIEWER` | `false` | _(deprecated)_ Legacy: start a viewer on boot. Use the editor instead. |

> **Note:** `ENABLE_SERVER` and `ENABLE_VIEWER` are kept for backward
> compatibility but are **deprecated**. When `/data/.hmi-services.json`
> exists (created by the editor), the persistent config takes priority.
> Use the editor's Server Panel to manage services instead.

### Customizing Ports

Change the **host** port (left side of `-p`) to avoid conflicts:

```bash
# Example: Editor on port 80, Viewer on port 443
docker run -d --name hmi \
  -p 14840:14840 \
  -p 80:8080 \
  -p 443:8081 \
  -p 8088:8088 \
  -p 5432:5432 \
  hmi-allinone:latest
```

### Windows Users

On Windows, replace the `\` line continuations with `  ` (backtick) in PowerShell,
or use a single line:

```powershell
docker run -d --name hmi -p 14840:14840 -p 14841:14841 -p 8080:8080 -p 8081:8081 -p 8088:8088 -p 5432:5432 -p 11434:11434 -v hmi-data:/data -v hmi-pgdata:/var/lib/postgresql/data hmi-allinone:latest
```
