# HMI Solution - Deployment Scripts

This folder contains automated build and deployment scripts for the HMI Solution platform.

## 🚀 Quick Start

### Complete Pipeline (Recommended)
Run the complete build and deployment pipeline:
```batch
deploy-all.bat
```

This will:
1. ✅ Build the solution in Release mode
2. ✅ Run all tests
3. ✅ Build Docker images
4. ✅ Push images to Docker Hub

### Individual Steps

#### 1️⃣ Build Release Platform
```batch
build-release.bat
```
- Cleans and builds the solution in Release configuration (uses `HMISolution.slnx`)
- ~~Runs all tests~~ Tests currently skipped (see note below)
- Publishes Server, Editor, and RuntimeViewer to `publish\` folder
- Output: `C:\Users\cfior\source\repos\publish\{Server|ServerEditorWeb|RuntimeViewer}\`

**Note on Tests**: Tests are currently skipped due to a known compatibility issue between .NET 10 preview and xunit.v3. The test code is valid but the test host cannot be loaded. This will be resolved when .NET 10 RTM is released. See `TEST-FAILURES-ANALYSIS.md` for details.

#### 2️⃣ Build Docker Images
```batch
build-docker.bat
```
- Builds Docker images for all components
- Tags images with version number and `latest`
- Images: `hmi-server`, `hmi-editor`, `hmi-viewer`

#### 3️⃣ Push to Docker Hub
```batch
push-docker.bat
```
- Pushes all Docker images to Docker Hub
- **Prerequisites**: You must be logged in (`docker login`)

## 📦 Docker Images

The deployment creates three Docker images for individual components:

| Component | Image | Port | Description |
|-----------|-------|------|-------------|
| **Server** | `clodprogea/hmi-server` | 14840, 14841 | OPC-UA Server with REST API |
| **Editor** | `clodprogea/hmi-editor` | 8080 | Blazor Web Editor UI |
| **RuntimeViewer** | `clodprogea/hmi-viewer` | 8081 | Runtime Monitoring UI |

These individual components are building blocks for the complete **AI Core HMI** platform.

### All-in-One Images

The complete platform is available as all-in-one Docker images:

| Image | Size | Description |
|-------|------|-------------|
| `clodprogea/aicorehmi:latest` | ~12 GB | Full stack with AI (Ollama + Mistral) |
| `clodprogea/hmi-allinone:latest` | ~4 GB | Lightweight without AI features |

**Note**: The all-in-one images require separate Dockerfiles that combine all components with PostgreSQL and Ollama. The batch scripts in this folder build the individual components only.

### Pull from Docker Hub

**Individual Components:**
```bash
docker pull clodprogea/hmi-server:latest
docker pull clodprogea/hmi-editor:latest
docker pull clodprogea/hmi-viewer:latest
```

**All-in-One (Complete Platform):**
```bash
# Full stack with AI
docker pull clodprogea/aicorehmi:latest

# Lightweight without AI
docker pull clodprogea/hmi-allinone:latest
```

### Run Locally

**Individual Components:**
```bash
# Server (OPC-UA + REST API)
docker run -p 14840:14840 -p 14841:14841 -v ./data:/data clodprogea/hmi-server:latest

# Editor (Blazor UI)
docker run -p 8080:8080 -v ./data:/data clodprogea/hmi-editor:latest

# RuntimeViewer
docker run -p 8081:8081 -v ./data:/data clodprogea/hmi-viewer:latest
```

**Complete Platform (All-in-One):**
```bash
# Full AI Core HMI with local LLM
docker run -d --name aicorehmi \
  -p 14840:14840 -p 14841:14841 \
  -p 8080:8080 -p 8081:8081 -p 8088:8088 \
  -p 5432:5432 -p 11434:11434 \
  -v hmi-data:/data \
  -v hmi-pgdata:/var/lib/postgresql/data \
  clodprogea/aicorehmi:latest
```

## ⚙️ Configuration

### Docker Hub Username

The scripts are pre-configured for the `clodprogea` Docker Hub organization (as documented in the GitHub README).

If you need to change it, update the `DOCKER_USERNAME` variable in both `build-docker.bat` and `push-docker.bat`:
```batch
set "DOCKER_USERNAME=clodprogea"
```

### Version Tag

Update the `VERSION` variable for new releases:
```batch
set "VERSION=1.0.0"
```

Both version-tagged and `latest` images will be built and pushed.

## 📋 Prerequisites

### For Release Build
- ✅ .NET 10 SDK installed
- ✅ Visual Studio 2026 or VS Code (optional)

### For Docker Build/Push
- ✅ Docker Desktop installed and running
- ✅ Docker Hub account
- ✅ Logged in to Docker Hub (`docker login`)

## 🔧 Troubleshooting

### Build Fails
```batch
# Clean everything and try again
dotnet clean HMISolution.slnx
dotnet restore HMISolution.slnx
build-release.bat
```

**Note**: The solution uses the `.slnx` format (Visual Studio 2022+ solution format) located at `C:\Users\cfior\source\repos\HMISolution.slnx`

### Docker Build Fails
```batch
# Check Docker is running
docker info

# Verify Docker context
docker build --file Server\Dockerfile .
```

### Docker Push Fails
```batch
# Login to Docker Hub with clodprogea credentials
docker login -u clodprogea

# Verify images exist locally
docker images | grep hmi

# Try pushing individually
docker push clodprogea/hmi-server:latest
docker push clodprogea/hmi-editor:latest
docker push clodprogea/hmi-viewer:latest
```

## 📁 Output Structure

After running `build-release.bat`:
```
publish/
├── Server/          # OPC-UA Server binaries
├── ServerEditorWeb/ # Blazor Editor binaries
└── RuntimeViewer/   # RuntimeViewer binaries
```

## 🐳 Docker Compose

For orchestrated deployment, use `docker-compose.yml` (if available in the solution):
```bash
docker-compose up -d
```

## 📚 Additional Resources

- [HMI Solution Repository](https://github.com/ClodAlone/HMISolution)
- [Docker Hub - AI Core HMI (Full)](https://hub.docker.com/r/clodprogea/aicorehmi)
- [Docker Hub - HMI All-in-One (Lightweight)](https://hub.docker.com/r/clodprogea/hmi-allinone)
- [Docker Hub - Individual Components](https://hub.docker.com/u/clodprogea)
- [Deployment Documentation](./docs/DEPLOYMENT.md)
- [AI Core HMI Landing Page](https://clodalon.github.io/HMISolution/)

## 🏗️ Architecture Notes

The **AI Core HMI** platform consists of multiple layers:

1. **Individual Components** (built by these scripts):
   - `hmi-server`: OPC-UA Server with 13 industrial drivers
   - `hmi-editor`: Blazor Web Editor with AI assistant
   - `hmi-viewer`: Runtime Viewer with NL query interface

2. **All-in-One Images** (separate Dockerfiles):
   - `aicorehmi`: Full stack including PostgreSQL (TimescaleDB) + Ollama (Mistral)
   - `hmi-allinone`: Lightweight version without AI features

The batch scripts in this folder build and push the **individual components** only. The all-in-one images require multi-stage Dockerfiles that orchestrate all services together.

## 🔐 Security Notes

- The scripts use Release configuration for optimized builds
- Docker images use official Microsoft .NET 10 base images
- Volume mounts (`/data`) are used for persistent configuration
- Ensure API keys and sensitive data are not included in Docker images

## 📝 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | Initial | Base deployment automation |

---

**Need Help?** Open an issue on [GitHub](https://github.com/ClodAlone/HMISolution/issues)
