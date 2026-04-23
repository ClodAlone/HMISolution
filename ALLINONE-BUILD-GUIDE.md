# 🐳 All-in-One Docker Image - Build & Deploy Guide

## Overview

The **`clodprogea/aicorehmi`** all-in-one image is a complete, self-contained HMI platform in a single Docker container (~12-13 GB).

## 📦 What's Included

The all-in-one image contains:

| Component | Description |
|-----------|-------------|
| **Server** | OPC-UA Server with 13 industrial drivers |
| **Editor** | Blazor Web Editor with AI assistant |
| **Viewer** | Runtime monitoring UI with NL query |
| **PostgreSQL** | TimescaleDB (time-series database) |
| **Ollama** | Local LLM inference (Mistral) |
| **ffmpeg** | Camera capture & video recording |
| **YOLOv8n** | ONNX model for object detection |

## 🛠️ Build Scripts

### Build All-in-One Image
```cmd
cd C:\Users\cfior\source\repos
build-allinone-docker.bat
```

**What it does:**
- Uses `Dockerfile.allinone` to build a unified container
- Compiles all .NET projects
- Installs PostgreSQL 16 + TimescaleDB
- Installs Ollama
- Bundles ffmpeg and YOLOv8n model
- Creates a ~12-13 GB image

**Time:** 10-15 minutes  
**Requirements:** Docker Desktop, ~20 GB free disk space

### Push to Docker Hub
```cmd
push-allinone-docker.bat
```

**Prerequisites:**
```bash
docker login -u clodprogea
```

**Time:** 15-30 minutes (uploads ~12 GB)

## 🎯 Complete Pipeline

Build everything (individual components + all-in-one) and push to Docker Hub:

```cmd
deploy-all-complete.bat
```

This runs:
1. ✅ `build-release.bat` - .NET Release build
2. ✅ `build-docker.bat` - Individual images (server, editor, viewer)
3. ✅ `build-allinone-docker.bat` - All-in-one image
4. ✅ `push-docker.bat` - Push individual images
5. ✅ `push-allinone-docker.bat` - Push all-in-one image

**Total time:** ~30-45 minutes (build + upload)

## 📋 Script Inventory

| Script | Purpose | Output |
|--------|---------|--------|
| `build-release.bat` | .NET Release build | Publish folders |
| `build-docker.bat` | Individual components | 3 images (~1.7 GB) |
| **`build-allinone-docker.bat`** | **All-in-one image** | **1 image (~12 GB)** |
| `push-docker.bat` | Push components | To Docker Hub |
| **`push-allinone-docker.bat`** | **Push all-in-one** | **To Docker Hub** |
| `deploy-all.bat` | Components only | Components pipeline |
| **`deploy-all-complete.bat`** | **Everything** | **Complete pipeline** |

## 🚀 Using the All-in-One Image

### Pull from Docker Hub
```bash
docker pull clodprogea/aicorehmi:latest
```

### Run
```bash
docker run -d --name aicorehmi \
  -p 14840:14840 \
  -p 14841:14841 \
  -p 8080:8080 \
  -p 8081:8081 \
  -p 8088:8088 \
  -p 5432:5432 \
  -p 11434:11434 \
  -v hmi-data:/data \
  -v hmi-pgdata:/var/lib/postgresql/data \
  clodprogea/aicorehmi:latest
```

### Access Points
| Service | URL | Port |
|---------|-----|------|
| Web Editor | http://localhost:8080 | 8080 |
| Runtime Viewer | http://localhost:8088 | 8088 |
| OPC-UA Server | opc.tcp://localhost:14840 | 14840 |
| REST API | http://localhost:14841 | 14841 |
| PostgreSQL | localhost:5432 | 5432 |
| Ollama AI | http://localhost:11434 | 11434 |
| Camera Stream | http://localhost:8088 | 8088 |

## 🔍 Technical Details

### Dockerfile Location
```
C:\Users\cfior\source\repos\Dockerfile.allinone
```

### Build Process
The `Dockerfile.allinone` uses a multi-stage build:

1. **Stage 1**: Build all .NET projects (Server, Editor, Viewer, all drivers)
2. **Stage 2**: Export YOLOv8n ONNX model
3. **Stage 3**: Create runtime image with:
   - Ubuntu 24.04 base
   - .NET 10 ASP.NET Core runtime
   - PostgreSQL 16 + TimescaleDB
   - Ollama
   - ffmpeg
   - All compiled applications

### Environment Variables
```bash
POSTGRES_USER=postgres
POSTGRES_PASSWORD=hmipass
POSTGRES_DB=hmi
PGDATA=/var/lib/postgresql/data
OLLAMA_MODEL=mistral
OLLAMA_HOST=127.0.0.1:11434
HMI_DATA=/data
ENABLE_SERVER=false
ENABLE_VIEWER=false
ASPNETCORE_ENVIRONMENT=Production
```

### Entrypoint Script
Located at: `docker/entrypoint.sh`

The entrypoint:
- Initializes PostgreSQL database
- Seeds sample projects on first run
- Starts Ollama and downloads Mistral model
- Launches Server, Editor, and Viewer processes

## 📊 Image Comparison

| Type | Images | Total Size | Use Case |
|------|--------|------------|----------|
| **Individual** | 3 separate | ~1.7 GB | Microservices, Kubernetes, custom orchestration |
| **All-in-One** | 1 unified | ~12 GB | Quick deployment, demos, single-server installs |

## 🎯 When to Use What

### Use Individual Components When:
- ✅ Running in Kubernetes
- ✅ Need to scale services independently
- ✅ Want minimal images
- ✅ Using custom orchestration

### Use All-in-One When:
- ✅ Quick deployment needed
- ✅ Single-command setup
- ✅ Demos or testing
- ✅ Self-contained environments
- ✅ Offline deployment (everything bundled)

## ⚙️ Updating the All-in-One Image

To rebuild and push an updated version:

```cmd
cd C:\Users\cfior\source\repos

# 1. Build new image
build-allinone-docker.bat

# 2. Test locally
docker run -d --name test-aicorehmi \
  -p 8080:8080 -p 8088:8088 \
  -v hmi-data:/data \
  clodprogea/aicorehmi:latest

# 3. Verify it works
# Open http://localhost:8080 and http://localhost:8088

# 4. Stop test
docker stop test-aicorehmi
docker rm test-aicorehmi

# 5. Push to Docker Hub
push-allinone-docker.bat
```

## 🔧 Troubleshooting

### Build Fails
```cmd
# Clean Docker build cache
docker builder prune -a

# Rebuild
build-allinone-docker.bat
```

### Push Fails
```cmd
# Login again
docker login -u clodprogea

# Verify image exists
docker images clodprogea/aicorehmi

# Retry push
push-allinone-docker.bat
```

### Image Too Large
The 12 GB size includes:
- .NET 10 Runtime (~200 MB)
- PostgreSQL + TimescaleDB (~150 MB)
- Ollama (~500 MB)
- Mistral model (~4 GB when pulled)
- All drivers and applications (~500 MB)
- System dependencies (~1 GB)
- Layers overhead (~5-6 GB)

This is expected for an all-in-one container with AI capabilities.

## 📚 Documentation

- **Build Guide**: This file
- **Individual Components**: `DEPLOYMENT-SCRIPTS.md`
- **Docker Build Success**: `DOCKER-BUILD-SUCCESS.md`
- **GitHub README**: Contains pull/run instructions for users

## 🎉 Summary

✅ **`build-allinone-docker.bat`** - Builds the complete aicorehmi image  
✅ **`push-allinone-docker.bat`** - Pushes to Docker Hub  
✅ **`deploy-all-complete.bat`** - Complete automation (components + all-in-one)  

Your batch file automation now fully supports building and pushing the **`clodprogea/aicorehmi`** all-in-one image! 🚀

---

**Last Updated**: After adding all-in-one automation  
**Image**: `clodprogea/aicorehmi:latest`  
**Size**: ~12-13 GB  
**Status**: ✅ Ready to build and deploy
