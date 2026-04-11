#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# HMI Solution — All-in-One Container Entrypoint
# Always starts: TimescaleDB (PostgreSQL 16), Ollama, ServerEditorWeb.
# Optionally starts: HMI Server (ENABLE_SERVER=true),
#                    RuntimeViewer (ENABLE_VIEWER=true).
# All running processes are supervised; if any critical process exits the
# container stops.
# ─────────────────────────────────────────────────────────────────────────────
set -e

# ── Configurable environment variables ──────────────────────────────────────
POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-hmipass}"
POSTGRES_DB="${POSTGRES_DB:-hmi}"
PGDATA="${PGDATA:-/var/lib/postgresql/data}"

OLLAMA_MODEL="${OLLAMA_MODEL:-mistral}"
OLLAMA_HOST="${OLLAMA_HOST:-127.0.0.1:11434}"

HMI_DATA="${HMI_DATA:-/data}"

# Service toggles (only the web editor runs by default)
ENABLE_SERVER="${ENABLE_SERVER:-false}"
ENABLE_VIEWER="${ENABLE_VIEWER:-false}"

# PostgreSQL 16 binaries (not on default PATH for the postgres user)
PGBIN="/usr/lib/postgresql/16/bin"

# ── 1. Initialize & start PostgreSQL / TimescaleDB ──────────────────────────
echo "[entrypoint] Starting TimescaleDB …"

if [ ! -s "$PGDATA/PG_VERSION" ]; then
    echo "[entrypoint] Initializing PostgreSQL data directory …"
    chown -R postgres:postgres "$PGDATA"
    su postgres -c "$PGBIN/initdb -D '$PGDATA' --auth-local=trust --auth-host=scram-sha-256"

    # Configure for TimescaleDB
    echo "shared_preload_libraries = 'timescaledb'" >> "$PGDATA/postgresql.conf"
    echo "listen_addresses = '*'" >> "$PGDATA/postgresql.conf"
    echo "host all all 0.0.0.0/0 scram-sha-256" >> "$PGDATA/pg_hba.conf"
fi

su postgres -c "$PGBIN/pg_ctl -D '$PGDATA' -l /var/log/postgresql/postgresql.log start -w"

# Create user / database if first run
su postgres -c "$PGBIN/psql -tc \"SELECT 1 FROM pg_roles WHERE rolname='$POSTGRES_USER'\"" \
    | grep -q 1 || su postgres -c "$PGBIN/psql -c \"CREATE ROLE $POSTGRES_USER WITH LOGIN PASSWORD '$POSTGRES_PASSWORD' SUPERUSER;\""
su postgres -c "$PGBIN/psql -tc \"SELECT 1 FROM pg_database WHERE datname='$POSTGRES_DB'\"" \
    | grep -q 1 || su postgres -c "$PGBIN/createdb -O '$POSTGRES_USER' '$POSTGRES_DB'"
su postgres -c "$PGBIN/psql -d '$POSTGRES_DB' -c 'CREATE EXTENSION IF NOT EXISTS timescaledb;'" 2>/dev/null || true

echo "[entrypoint] TimescaleDB ready (port 5432)"

# ── 2. Start Ollama in background ──────────────────────────────────────────
echo "[entrypoint] Starting Ollama …"
export OLLAMA_HOST
ollama serve > /var/log/ollama.log 2>&1 &
OLLAMA_PID=$!

# Wait for Ollama to become ready
for i in $(seq 1 30); do
    if curl -sf "http://${OLLAMA_HOST}/api/tags" > /dev/null 2>&1; then break; fi
    sleep 1
done

# Pull the default model in background if not already present
if ! ollama list 2>/dev/null | grep -q "$OLLAMA_MODEL"; then
    echo "[entrypoint] Pulling Ollama model '$OLLAMA_MODEL' in background (first run only) …"
    (ollama pull "$OLLAMA_MODEL" && echo "[entrypoint] Ollama model '$OLLAMA_MODEL' ready" \
        || echo "[entrypoint] WARNING: Could not pull model. Pull it manually later with: docker exec <container> ollama pull $OLLAMA_MODEL") &
else
    echo "[entrypoint] Ollama model '$OLLAMA_MODEL' already present"
fi

echo "[entrypoint] Ollama ready (${OLLAMA_HOST})"

# ── 3. Prepare shared data directory ───────────────────────────────────────
mkdir -p "$HMI_DATA" /var/log/hmi

# Seed sample projects — copy any missing sample folders from the image
if [ -d /opt/hmi/samples ]; then
    SAMPLE_COUNT=0
    for sample_dir in /opt/hmi/samples/*/; do
        sample_name="$(basename "$sample_dir")"
        if [ ! -d "$HMI_DATA/$sample_name" ]; then
            cp -r "$sample_dir" "$HMI_DATA/$sample_name"
            SAMPLE_COUNT=$((SAMPLE_COUNT + 1))
            echo "[entrypoint] Seeded sample project: $sample_name"
        fi
    done
    if [ "$SAMPLE_COUNT" -gt 0 ]; then
        echo "[entrypoint] Seeded $SAMPLE_COUNT new sample project(s)"
    fi
fi

# ── 4. Start persistent service instances from config ──────────────────────
# The editor saves /data/.hmi-services.json listing server/viewer instances
# that should auto-start. We launch them here before the editor starts.
SERVICE_CONFIG="${HMI_DATA}/.hmi-services.json"
SERVICE_PIDS=""

if [ -f "$SERVICE_CONFIG" ]; then
    echo "[entrypoint] Loading persistent service config: $SERVICE_CONFIG"
    # Parse JSON array — each entry has kind, projectPath, port, autoStart
    # Use python3 (available in ubuntu:24.04) for reliable JSON parsing
    python3 -c "
import json, sys
with open('$SERVICE_CONFIG') as f:
    entries = json.load(f)
for e in entries:
    if e.get('autoStart', False):
        kind = e.get('kind', 'Server')
        path = e.get('projectPath', '')
        port = e.get('port', 0)
        print(f'{kind}|{path}|{port}')
" 2>/dev/null | while IFS='|' read -r kind project_path port; do
        if [ -z "$project_path" ] || [ ! -f "$project_path" ]; then
            echo "[entrypoint] Skipping $kind — project not found: $project_path"
            continue
        fi

        if [ "$kind" = "Server" ]; then
            echo "[entrypoint] Starting Server for $(basename "$(dirname "$project_path")") …"
            cd /opt/hmi/server
            dotnet Server.dll "$project_path" > /var/log/hmi/server-$(basename "$(dirname "$project_path")").log 2>&1 &
            SERVICE_PIDS="$SERVICE_PIDS $!"
        elif [ "$kind" = "Viewer" ]; then
            echo "[entrypoint] Starting Viewer for $(basename "$(dirname "$project_path")") on port $port …"
            cd /opt/hmi/viewer
            ASPNETCORE_URLS="http://+:${port}" dotnet RuntimeViewer.dll "$project_path" > /var/log/hmi/viewer-$(basename "$(dirname "$project_path")").log 2>&1 &
            SERVICE_PIDS="$SERVICE_PIDS $!"
        fi
    done
    echo "[entrypoint] Persistent service instances launched"
else
    echo "[entrypoint] No persistent service config found — editor-only mode"
    # Legacy support: ENABLE_SERVER / ENABLE_VIEWER env vars
    # (deprecated — use the editor to manage instances instead)
    if [ "$ENABLE_SERVER" = "true" ]; then
        # Find a nodes.json to use
        NODES_JSON="${HMI_DATA}/nodes.json"
        if [ ! -f "$NODES_JSON" ]; then
            FIRST_NODES="$(find "$HMI_DATA" -maxdepth 2 -name "nodes.json" -type f 2>/dev/null | head -1)"
            [ -n "$FIRST_NODES" ] && NODES_JSON="$FIRST_NODES"
        fi
        if [ -f "$NODES_JSON" ]; then
            echo "[entrypoint] Starting HMI Server (legacy ENABLE_SERVER) …"
            cd /opt/hmi/server
            dotnet Server.dll "$NODES_JSON" > /var/log/hmi/server.log 2>&1 &
            SERVICE_PIDS="$SERVICE_PIDS $!"
        fi
    fi
    if [ "$ENABLE_VIEWER" = "true" ]; then
        NODES_JSON="${HMI_DATA}/nodes.json"
        if [ ! -f "$NODES_JSON" ]; then
            FIRST_NODES="$(find "$HMI_DATA" -maxdepth 2 -name "nodes.json" -type f 2>/dev/null | head -1)"
            [ -n "$FIRST_NODES" ] && NODES_JSON="$FIRST_NODES"
        fi
        if [ -f "$NODES_JSON" ]; then
            echo "[entrypoint] Starting RuntimeViewer (legacy ENABLE_VIEWER) …"
            cd /opt/hmi/viewer
            ASPNETCORE_URLS="http://+:8081" dotnet RuntimeViewer.dll "$NODES_JSON" > /var/log/hmi/viewer.log 2>&1 &
            SERVICE_PIDS="$SERVICE_PIDS $!"
        fi
    fi
fi

# ── 5. Start ServerEditorWeb (Blazor) ─────────────────────────────────────
echo "[entrypoint] Starting ServerEditorWeb …"
cd /opt/hmi/editor
export ASPNETCORE_URLS="http://+:8080"
export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Production}"
dotnet ServerEditorWeb.dll > /var/log/hmi/editor.log 2>&1 &
EDITOR_PID=$!

# ── 6. Supervise – keep container alive while editor runs ────────────────
echo ""
echo "════════════════════════════════════════════════════════════════"
echo "  HMI Solution — All-in-One Container"
echo "  ────────────────────────────────────"
echo "  Web Editor         : http://localhost:8080"
echo "  TimescaleDB        : postgresql://localhost:5432/$POSTGRES_DB"
echo "  Ollama             : http://${OLLAMA_HOST}"
if [ -f "$SERVICE_CONFIG" ]; then
    echo "  Services           : loaded from $SERVICE_CONFIG"
else
    echo "  Services           : none (use the editor to start servers)"
fi
echo ""
echo "  Use the editor's Server Panel to start/stop HMI Server and"
echo "  RuntimeViewer instances. Configuration is saved automatically"
echo "  and persists across container restarts."
echo "════════════════════════════════════════════════════════════════"
echo ""

cleanup() {
    echo "[entrypoint] Shutting down …"
    # Kill all child dotnet processes (servers, viewers started by editor or entrypoint)
    pkill -P $$ dotnet 2>/dev/null || true
    kill "$EDITOR_PID" 2>/dev/null || true
    kill "$OLLAMA_PID" 2>/dev/null || true
    su postgres -c "$PGBIN/pg_ctl -D '$PGDATA' stop -m fast" 2>/dev/null || true
    exit 0
}
trap cleanup SIGTERM SIGINT

# Only the editor is critical — server/viewer instances are managed dynamically
# and their lifecycle is handled by the DockerServiceManager in the editor.
while true; do
    if ! kill -0 "$EDITOR_PID" 2>/dev/null; then
        echo "[entrypoint] Editor process exited — stopping container"
        cleanup
    fi
    sleep 5
done