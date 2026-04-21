# Publishing Docker Images to Docker Hub (Public)

## Step 1: Create Docker Hub Account
1. Go to https://hub.docker.com/
2. Sign up for a free account
3. Verify your email

## Step 2: Create Public Repository on Docker Hub
1. Log in to Docker Hub
2. Click **"Create Repository"**
3. Repository name: `aicorehmi-server` (or your preferred name)
4. Visibility: **Public** ✅
5. Description: "AI Core HMI - AI-Powered Open-Source Industrial HMI/SCADA Server"
6. Click **"Create"**

Repeat for other images:
- `aicorehmi-editor` (web editor)
- `aicorehmi-runtime` (runtime viewer)

## Step 3: Login to Docker Hub from Command Line

```bash
docker login
# Enter your Docker Hub username and password
```

## Step 4: Tag Your Images

```bash
# Replace 'yourusername' with your Docker Hub username

# Server image
docker tag aicorehmi-server:latest yourusername/aicorehmi-server:latest
docker tag aicorehmi-server:latest yourusername/aicorehmi-server:1.0

# Editor image
docker tag aicorehmi-editor:latest yourusername/aicorehmi-editor:latest
docker tag aicorehmi-editor:latest yourusername/aicorehmi-editor:1.0

# Runtime image (if you have one)
docker tag aicorehmi-runtime:latest yourusername/aicorehmi-runtime:latest
docker tag aicorehmi-runtime:latest yourusername/aicorehmi-runtime:1.0
```

## Step 5: Push Images to Docker Hub

```bash
# Push server
docker push yourusername/aicorehmi-server:latest
docker push yourusername/aicorehmi-server:1.0

# Push editor
docker push yourusername/aicorehmi-editor:latest
docker push yourusername/aicorehmi-editor:1.0

# Push runtime
docker push yourusername/aicorehmi-runtime:latest
docker push yourusername/aicorehmi-runtime:1.0
```

## Step 6: Update Landing Page Documentation

Update your landing page with the public Docker Hub commands:

```bash
# Pull server image
docker pull yourusername/aicorehmi-server:latest

# Pull editor image
docker pull yourusername/aicorehmi-editor:latest

# Run server
docker run -d -p 8080:8080 --name aicorehmi-server yourusername/aicorehmi-server:latest

# Run editor
docker run -d -p 5000:5000 --name aicorehmi-editor yourusername/aicorehmi-editor:latest
```

## Docker Hub Free Tier Limits
- ✅ **Unlimited public repositories**
- ✅ **Unlimited public image pulls**
- ✅ 1 private repository
- ✅ 200 container pulls per 6 hours (for anonymous users)
- ✅ Unlimited pulls for authenticated users

---

## Alternative: GitHub Container Registry (GHCR) with Public Visibility

You can also use GitHub Container Registry and make images public while keeping the repo private.

### Setup GHCR:

```bash
# 1. Create a Personal Access Token (PAT)
# Go to: GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
# Create token with 'write:packages' and 'read:packages' scopes

# 2. Login to GHCR
echo YOUR_PAT | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin

# 3. Tag images for GHCR
docker tag aicorehmi-server:latest ghcr.io/clodalone/aicorehmi-server:latest
docker tag aicorehmi-server:latest ghcr.io/clodalone/aicorehmi-server:1.0

# 4. Push to GHCR
docker push ghcr.io/clodalone/aicorehmi-server:latest
docker push ghcr.io/clodalone/aicorehmi-server:1.0

# 5. Make Package Public
# Go to: https://github.com/users/ClodAlone/packages
# Click on your package → Package settings → Change visibility → Public
```

### Users pull with:
```bash
docker pull ghcr.io/clodalone/aicorehmi-server:latest
```

---

## Recommended Approach

**Use Docker Hub** because:
- ✅ Easier for users (no GitHub authentication needed)
- ✅ Better for public discovery
- ✅ Standard for open-source projects
- ✅ Unlimited public image pulls
- ✅ Shows up in Docker Hub search

**Use GHCR** if:
- You want to keep everything in the GitHub ecosystem
- You plan to use GitHub Actions for automated builds

---

## Automation with GitHub Actions

You can automate Docker image publishing even with a private repo:

`.github/workflows/docker-publish.yml`:

```yaml
name: Publish Docker Images

on:
  push:
    tags:
      - 'v*'
  workflow_dispatch:

jobs:
  build-and-push:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3

    - name: Login to Docker Hub
      uses: docker/login-action@v3
      with:
        username: ${{ secrets.DOCKERHUB_USERNAME }}
        password: ${{ secrets.DOCKERHUB_TOKEN }}

    - name: Build and push Server
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ./ServerOpcUa/Dockerfile
        push: true
        tags: |
          yourusername/aicorehmi-server:latest
          yourusername/aicorehmi-server:${{ github.ref_name }}

    - name: Build and push Editor
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ./ServerEditorWeb/Dockerfile
        push: true
        tags: |
          yourusername/aicorehmi-editor:latest
          yourusername/aicorehmi-editor:${{ github.ref_name }}
```

Then add Docker Hub credentials as GitHub Secrets:
- `DOCKERHUB_USERNAME`
- `DOCKERHUB_TOKEN` (create at https://hub.docker.com/settings/security)

---

## Summary

**Quick Start (Manual):**
1. Create Docker Hub account
2. Run: `docker login`
3. Tag: `docker tag aicorehmi-server:latest yourusername/aicorehmi-server:latest`
4. Push: `docker push yourusername/aicorehmi-server:latest`
5. Update landing page with new pull commands

**Your repository stays private, but Docker images are publicly accessible!** ✅
