# Deployment Automation - Quick Reference

## 🎯 What These Scripts Do

These batch files automate the build and deployment pipeline for the **AI Core HMI** platform components.

## 📦 Docker Images Structure

### Individual Components (Built by these scripts)
```
clodprogea/hmi-server:latest   - OPC-UA Server with REST API
clodprogea/hmi-editor:latest   - Blazor Web Editor
clodprogea/hmi-viewer:latest   - Runtime Viewer
```

### All-in-One Images (Separate builds)
```
clodprogea/aicorehmi:latest      - Full stack with AI (12 GB)
clodprogea/hmi-allinone:latest   - Lightweight without AI (4 GB)
```

## 🚀 Quick Commands

```batch
REM Complete pipeline (build + docker + push)
deploy-all.bat

REM Individual steps
build-release.bat    # .NET Release build
build-docker.bat     # Docker images (3 components)
push-docker.bat      # Push to clodprogea on Docker Hub
```

## 🔑 Prerequisites

1. **Docker Hub Login**
   ```bash
   docker login -u clodprogea
   ```

2. **.NET 10 SDK** installed

3. **Docker Desktop** running

## 📊 Build Output

### build-release.bat
- Publishes to: `C:\Users\cfior\source\repos\publish\`
  - `Server\` - OPC-UA Server binaries
  - `ServerEditorWeb\` - Blazor Editor binaries
  - `RuntimeViewer\` - Runtime Viewer binaries
- Uses `HMISolution.slnx` (Visual Studio 2022+ solution format)

### build-docker.bat
- Creates 3 Docker images (each tagged with version + latest)
- Total size: ~500 MB compressed

### push-docker.bat
- Pushes to Docker Hub: `https://hub.docker.com/u/clodprogea`
- 6 tags total (3 images × 2 tags each)

## 🌐 Public Access

After pushing, users can deploy with:

```bash
# Pull individual components
docker pull clodprogea/hmi-server:latest
docker pull clodprogea/hmi-editor:latest
docker pull clodprogea/hmi-viewer:latest

# Or use the complete all-in-one image
docker pull clodprogea/aicorehmi:latest
```

## ⚙️ Configuration Variables

Both `build-docker.bat` and `push-docker.bat` use:

```batch
set "DOCKER_USERNAME=clodprogea"  # Matches GitHub docs
set "VERSION=1.0.0"                # Update for releases
```

## 🔗 Links

- **GitHub**: https://github.com/ClodAlone/HMISolution
- **Docker Hub**: https://hub.docker.com/u/clodprogea
- **Landing Page**: https://clodalon.github.io/HMISolution/
- **Full Documentation**: See `DEPLOYMENT-SCRIPTS.md`

## ⚠️ Important Notes

1. These scripts build **individual components** only
2. The all-in-one images (`aicorehmi`, `hmi-allinone`) require separate Dockerfiles that combine components with PostgreSQL and Ollama
3. All image names match the official GitHub documentation
4. Docker Hub organization is `clodprogea` (not `clodalon`)

## 🎬 Typical Workflow

```batch
REM 1. Make code changes in Visual Studio
REM 2. Run complete pipeline
C:\Users\cfior\source\repos\deploy-all.bat

REM 3. Verify on Docker Hub
REM    https://hub.docker.com/r/clodprogea/hmi-server
REM    https://hub.docker.com/r/clodprogea/hmi-editor
REM    https://hub.docker.com/r/clodprogea/hmi-viewer
```

---
**Ready to deploy!** 🚀
