# Updating GitHub Pages with Docker Instructions

## Current Status

✅ **Docker Images Published**:
- `clodprogea/hmi-server:latest` (255MB) - OPC UA Server
- `clodprogea/hmi-editor:latest` (133MB) - Web Editor  
- `clodprogea/hmi-viewer:latest` (116MB) - Runtime Viewer
- `clodprogea/aicorehmi:latest` (4.12GB) - All-in-One ⏳ **Pushing in progress...**

## How to Update GitHub Pages

Your GitHub Pages site is at: https://clodalone.github.io/HMISolution

### Option 1: Update via GitHub Web Interface

1. Go to your GitHub Pages repository (likely `clodalone.github.io` or a `gh-pages` branch)
2. Find the section with Docker instructions (the one showing `docker pull clodprogea/aicorehmi:latest`)
3. Replace the Docker section with the updated content below

### Option 2: Update Locally

If your GitHub Pages is generated from this repository:

```bash
# Navigate to your repository
cd C:\Users\cfior\source\repos\HMISolution

# Create/update the README.md (already done)
# The README.md file now contains correct Docker instructions

# Commit and push
git add README.md DOCKER-README.md
git commit -m "Update Docker deployment instructions with published images"
git push origin master
```

## Updated Docker Section for GitHub Pages

Replace the current Docker section in your GitHub Pages with:

```markdown
## 🐳 Docker Quick Start

The fastest way to try AI Core HMI is the all-in-one Docker image. It bundles the OPC UA server, web editor, runtime viewer, PostgreSQL, and Ollama (local AI) into a single container.

### Pull & Run (All-in-One)

\`\`\`bash
# Pull the all-in-one image from Docker Hub (4.12GB)
docker pull clodprogea/aicorehmi:latest

# Run with persistent volumes
docker run -d --name aicorehmi \\
  -p 14840:14840 \\
  -p 14841:14841 \\
  -p 8080:8080 \\
  -p 8081:8081 \\
  -p 8088:8088 \\
  -p 5432:5432 \\
  -p 11434:11434 \\
  -v hmi-data:/data \\
  -v hmi-pgdata:/var/lib/postgresql/data \\
  clodprogea/aicorehmi:latest
\`\`\`

### Access Points

After the container starts, access:

- **Web Editor**: http://localhost:8080
- **Runtime Viewer**: http://localhost:8088  
- **OPC UA Server**: opc.tcp://localhost:14840
- **REST API**: http://localhost:14841
- **Ollama AI**: http://localhost:11434
- **PostgreSQL**: localhost:5432

### Individual Images (Recommended for Production)

For better flexibility and smaller downloads, use individual services:

\`\`\`bash
# Download docker-compose file
curl -O https://raw.githubusercontent.com/ClodAlone/HMISolution/master/docker-compose.allinone.yml

# Start all services
docker-compose -f docker-compose.allinone.yml up -d
\`\`\`

Or pull individual images:

| Image | Size | Description |
|-------|------|-------------|
| `clodprogea/hmi-server:latest` | 255MB | OPC UA Server + REST API |
| `clodprogea/hmi-editor:latest` | 133MB | Blazor Web Editor |
| `clodprogea/hmi-viewer:latest` | 116MB | Runtime Viewer |

### Port Reference

| Port | Service |
|------|---------|
| 14840 | OPC UA Server |
| 14841 | Diagnostics / REST API |
| 8080 | Web Editor (HTTP) |
| 8081 | Web Editor (HTTPS) |
| 8088 | Runtime Viewer |
| 5432 | PostgreSQL (TimescaleDB) |
| 11434 | Ollama (Local LLM) |
```

## Verification

After pushing `clodprogea/aicorehmi:latest`, verify it's available:

```bash
# Search for your images on Docker Hub
docker search clodprogea --limit 10

# Try pulling it
docker pull clodprogea/aicorehmi:latest
```

Expected output:
```
NAME                       DESCRIPTION   STARS     OFFICIAL
clodprogea/aicorehmi                     0         
clodprogea/hmi-server                    0         
clodprogea/hmi-editor                    0         
clodprogea/hmi-viewer                    0         
```

## Next Steps

1. ✅ Wait for `docker push clodprogea/aicorehmi:latest` to complete
2. ✅ Update GitHub Pages documentation with the new Docker section above
3. ✅ Test the pull command to ensure it works
4. ✅ Update any other documentation references (README, wiki, etc.)
5. ✅ Announce the Docker availability to users

## Notes

- The all-in-one image (4.12GB) includes everything bundled together
- For production, we recommend using docker-compose with individual images
- First run of Ollama will take 30-90 seconds to load the model
- All images are public and require no authentication

