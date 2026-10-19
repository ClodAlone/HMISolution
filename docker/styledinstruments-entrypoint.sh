#!/usr/bin/env bash
set -euo pipefail

if [ ! -f /data/nodes.json ]; then
    cp /opt/hmi/sample/nodes.json /data/nodes.json
    echo "[demo] Seeded StyledInstruments in /data/nodes.json"
fi

pids=()
cleanup() {
    trap '' TERM INT
    for pid in "${pids[@]}"; do
        if kill -0 "$pid" 2>/dev/null; then
            kill -TERM "$pid" 2>/dev/null || true
        fi
    done
    for pid in "${pids[@]}"; do
        wait "$pid" 2>/dev/null || true
    done
}
trap 'cleanup; exit 0' TERM INT

cd /opt/hmi/server
dotnet Server.dll /data/nodes.json &
pids+=("$!")

cd /opt/hmi/viewer
dotnet RuntimeViewer.dll /data/nodes.json &
pids+=("$!")

echo "[demo] Styled Instruments viewer: http://localhost:8081"
echo "[demo] Server and viewer logs follow. Both services stop if either exits."

status=0
wait -n "${pids[@]}" || status=$?
echo "[demo] A service exited (status $status); stopping the demo." >&2
cleanup
# Even an unexpected clean service exit means this container cannot serve the demo.
if [ "$status" -eq 0 ]; then
    status=1
fi
exit "$status"
