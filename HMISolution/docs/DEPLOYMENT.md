# HMI Solution — Deployment Guide

This document covers building Docker images for the server-side components
and publishing the native desktop viewer.

---

## Architecture Overview

| Component | Type | Docker? | Default Port |
|---|---|---|---|
| **Server** | OPC UA server (console) | ✅ | 4840 (OPC UA), 14841 (diagnostics) |
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
| OPC UA Server | `opc.tcp://localhost:4840` |
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
  -p 4840:4840 -p 14841:14841 \
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
| Editor cannot reach Server | Containers share a Docker network; use `server:4840` as the OPC UA endpoint inside the editor |
| Static assets not loading in RuntimeViewer | The Dockerfile publishes in Release mode; `UseStaticWebAssets()` is handled automatically |
| Desktop cannot find RuntimeViewer.exe | Publish both projects to the same output directory (see §5.3) |
| Port conflict | Change the host port in `docker-compose.yml` (e.g., `"9090:8080"`) |
