#!/usr/bin/env bash
# Copyright (c) 2026 Claudio Fiorani
# All rights reserved.

# HMI Solution Linux installer.
#
# Installs the desktop Release builds of the HMI Solution runtime, editor
# and server. Components can be selected interactively or via --components.
#
# The script expects a "payload" directory next to it with layout:
#   payload/
#     Runtime/  -> RuntimeViewer.Desktop + support files
#     Editor/   -> ServerEditorWeb.Desktop + support files
#     Server/   -> Server + support files
#
# Produced by Build-Installer.ps1.

set -euo pipefail

PRODUCT_NAME="HMI Solution"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PAYLOAD_DIR="$SCRIPT_DIR/payload"

COMPONENTS_ARG=""
INSTALL_DIR=""
MODE=""                 # system | user
NO_SHORTCUTS=0
INSTALL_SERVICE=0
SILENT=0

# component_key:folder:executable:display
ALL_COMPONENTS=(
    "Runtime:Runtime:RuntimeViewer.Desktop:Runtime Viewer (Desktop)"
    "Editor:Editor:ServerEditorWeb.Desktop:Server Editor (Desktop)"
    "Server:Server:Server:HMI OPC UA Server"
)

usage() {
    cat <<EOF
Usage: $0 [options]

Options:
  --components LIST    Comma-separated components: Runtime,Editor,Server,All
  --install-dir DIR    Target directory. Default: /opt/hmi-solution (system)
                       or ~/.local/share/hmi-solution (user)
  --user               Install for current user only (no root required)
  --system             Install system-wide (default; requires root)
  --no-shortcuts       Do not create .desktop launcher entries
  --service            Install a systemd service for the Server component
  --silent             Non-interactive: use defaults / provided flags
  -h, --help           Show this help
EOF
}

log()  { printf '\033[36m%s\033[0m\n' "$*"; }
ok()   { printf '\033[32m%s\033[0m\n' "$*"; }
warn() { printf '\033[33m%s\033[0m\n' "$*" >&2; }
err()  { printf '\033[31mERROR: %s\033[0m\n' "$*" >&2; }

# ------------------------- arg parsing -------------------------
while [[ $# -gt 0 ]]; do
    case "$1" in
        --components)   COMPONENTS_ARG="$2"; shift 2 ;;
        --install-dir)  INSTALL_DIR="$2";    shift 2 ;;
        --user)         MODE="user";         shift ;;
        --system)       MODE="system";       shift ;;
        --no-shortcuts) NO_SHORTCUTS=1;      shift ;;
        --service)      INSTALL_SERVICE=1;   shift ;;
        --silent)       SILENT=1;            shift ;;
        -h|--help)      usage; exit 0 ;;
        *)              err "Unknown option: $1"; usage; exit 2 ;;
    esac
done

header() {
    echo
    echo "======================================================"
    echo "  $PRODUCT_NAME - Linux Installer"
    echo "======================================================"
    echo
}

# ------------------------- helpers -----------------------------
comp_field() { echo "$1" | cut -d: -f"$2"; }

select_components() {
    if [[ -n "$COMPONENTS_ARG" ]]; then
        echo "$COMPONENTS_ARG"
        return
    fi
    if (( SILENT )); then echo "All"; return; fi

    echo "Select components to install:" >&2
    local i=1
    for c in "${ALL_COMPONENTS[@]}"; do
        printf "  [%d] %-30s (%s)\n" "$i" "$(comp_field "$c" 4)" "$(comp_field "$c" 1)" >&2
        i=$((i+1))
    done
    echo "  [A] All components" >&2
    echo >&2
    read -rp "Enter numbers separated by comma (e.g. 1,3) or A for all: " ans
    [[ -z "$ans" || "$ans" =~ ^[Aa]$ ]] && { echo "All"; return; }

    local out=""
    IFS=',; ' read -ra toks <<<"$ans"
    for t in "${toks[@]}"; do
        [[ -z "$t" ]] && continue
        if ! [[ "$t" =~ ^[0-9]+$ ]] || (( t < 1 || t > ${#ALL_COMPONENTS[@]} )); then
            err "Invalid selection: '$t'"; exit 2
        fi
        local key
        key="$(comp_field "${ALL_COMPONENTS[$((t-1))]}" 1)"
        out+="${out:+,}$key"
    done
    echo "$out"
}

resolve_selected() {
    local requested="$1"
    if [[ "$requested" == "All" ]]; then
        for c in "${ALL_COMPONENTS[@]}"; do echo "$c"; done
        return
    fi
    IFS=',; ' read -ra toks <<<"$requested"
    for t in "${toks[@]}"; do
        [[ -z "$t" ]] && continue
        local found=0
        for c in "${ALL_COMPONENTS[@]}"; do
            if [[ "$(comp_field "$c" 1)" == "$t" ]]; then
                echo "$c"; found=1; break
            fi
        done
        (( found )) || { err "Unknown component: '$t'"; exit 2; }
    done
}

install_component() {
    local c="$1" root="$2"
    local folder key display
    key="$(comp_field "$c" 1)"
    folder="$(comp_field "$c" 2)"
    display="$(comp_field "$c" 4)"
    local src="$PAYLOAD_DIR/$folder"
    local dst="$root/$folder"
    [[ -d "$src" ]] || { err "Payload folder missing for $key: $src"; exit 1; }
    ok "  -> Installing $display..."
    mkdir -p "$dst"
    cp -a "$src/." "$dst/"
    # Ensure the main executable is executable
    local exe
    exe="$(comp_field "$c" 3)"
    [[ -f "$dst/$exe" ]] && chmod +x "$dst/$exe" || true
}

write_desktop_entry() {
    local c="$1" root="$2" appdir="$3"
    local key folder exe display
    key="$(comp_field "$c" 1)"
    folder="$(comp_field "$c" 2)"
    exe="$(comp_field "$c" 3)"
    display="$(comp_field "$c" 4)"
    # Skip .desktop for headless Server component
    [[ "$key" == "Server" ]] && return

    local target="$root/$folder/$exe"
    local entry="$appdir/hmi-${key,,}.desktop"
    mkdir -p "$appdir"
    cat > "$entry" <<EOF
[Desktop Entry]
Type=Application
Name=$display
Comment=$PRODUCT_NAME - $display
Exec="$target" %U
Path=$root/$folder
Terminal=false
Categories=Development;Engineering;
StartupNotify=true
EOF
    chmod 644 "$entry"
    echo "  -> Launcher: $entry"
}

write_systemd_unit() {
    local root="$1"
    local unit_path="/etc/systemd/system/hmi-server.service"
    local exec="$root/Server/Server"
    [[ -f "$exec" ]] || { warn "Server not installed; skipping systemd unit."; return; }
    if [[ $EUID -ne 0 ]]; then
        warn "Systemd unit requires root; skipping. Re-run with sudo and --service."
        return
    fi
    cat > "$unit_path" <<EOF
[Unit]
Description=$PRODUCT_NAME - OPC UA Server
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
WorkingDirectory=$root/Server
ExecStart=$exec
Restart=on-failure
RestartSec=5

[Install]
WantedBy=multi-user.target
EOF
    chmod 644 "$unit_path"
    systemctl daemon-reload
    echo "  -> systemd unit: $unit_path (enable with: sudo systemctl enable --now hmi-server)"
}

write_uninstaller() {
    local root="$1" selected="$2"
    local script="$root/uninstall-hmi-solution.sh"
    local folders=""
    while IFS= read -r c; do
        [[ -z "$c" ]] && continue
        folders+="\"$(comp_field "$c" 2)\" "
    done <<<"$selected"

    cat > "$script" <<EOF
#!/usr/bin/env bash
set -e
ROOT="\$(cd "\$(dirname "\${BASH_SOURCE[0]}")" && pwd)"
echo "Uninstalling $PRODUCT_NAME from \$ROOT"
for f in $folders; do
    [[ -d "\$ROOT/\$f" ]] && rm -rf "\$ROOT/\$f"
done
# Remove desktop entries (system + user)
for d in /usr/share/applications "\$HOME/.local/share/applications"; do
    rm -f "\$d"/hmi-runtime.desktop "\$d"/hmi-editor.desktop 2>/dev/null || true
done
# Remove systemd unit if present
if [[ -f /etc/systemd/system/hmi-server.service ]]; then
    systemctl disable --now hmi-server 2>/dev/null || true
    rm -f /etc/systemd/system/hmi-server.service
    systemctl daemon-reload 2>/dev/null || true
fi
rm -f "\$ROOT/uninstall-hmi-solution.sh"
echo "Done. You may now delete \$ROOT"
EOF
    chmod +x "$script"
}

# ------------------------- main --------------------------------
header

[[ -d "$PAYLOAD_DIR" ]] || { err "Payload directory not found: $PAYLOAD_DIR
Run Build-Installer.ps1 first to produce it."; exit 1; }

# Determine install mode
if [[ -z "$MODE" ]]; then
    if [[ $EUID -eq 0 ]]; then MODE="system"; else MODE="user"; fi
fi

# Default install dir
if [[ -z "$INSTALL_DIR" ]]; then
    if [[ "$MODE" == "system" ]]; then
        INSTALL_DIR="/opt/hmi-solution"
    else
        INSTALL_DIR="${XDG_DATA_HOME:-$HOME/.local/share}/hmi-solution"
    fi
fi

if (( ! SILENT )); then
    read -rp "Install directory [$INSTALL_DIR]: " ans
    [[ -n "$ans" ]] && INSTALL_DIR="$ans"
fi

REQUESTED="$(select_components)"
SELECTED="$(resolve_selected "$REQUESTED")"

echo
log "Summary:"
echo "  Mode              : $MODE"
echo "  Install directory : $INSTALL_DIR"
echo -n "  Components        : "
echo "$SELECTED" | while read -r c; do [[ -n "$c" ]] && printf '%s ' "$(comp_field "$c" 1)"; done
echo
echo "  Create shortcuts  : $([[ $NO_SHORTCUTS -eq 0 ]] && echo yes || echo no)"
echo "  Install service   : $([[ $INSTALL_SERVICE -eq 1 ]] && echo yes || echo no)"
echo

if (( ! SILENT )); then
    read -rp "Proceed with installation? [Y/n] " confirm
    if [[ -n "$confirm" && ! "$confirm" =~ ^[Yy]$ ]]; then
        warn "Aborted."; exit 0
    fi
fi

if [[ "$MODE" == "system" && $EUID -ne 0 ]]; then
    err "System-wide install requires root. Re-run with sudo, or pass --user."
    exit 1
fi

mkdir -p "$INSTALL_DIR"
while IFS= read -r c; do
    [[ -z "$c" ]] && continue
    install_component "$c" "$INSTALL_DIR"
done <<<"$SELECTED"

if (( ! NO_SHORTCUTS )); then
    if [[ "$MODE" == "system" ]]; then
        APPDIR="/usr/share/applications"
    else
        APPDIR="${XDG_DATA_HOME:-$HOME/.local/share}/applications"
    fi
    while IFS= read -r c; do
        [[ -z "$c" ]] && continue
        write_desktop_entry "$c" "$INSTALL_DIR" "$APPDIR"
    done <<<"$SELECTED"
fi

if (( INSTALL_SERVICE )); then
    write_systemd_unit "$INSTALL_DIR"
fi

write_uninstaller "$INSTALL_DIR" "$SELECTED"

echo
ok "Installation complete."
echo "Location: $INSTALL_DIR"
echo "Uninstall: $INSTALL_DIR/uninstall-hmi-solution.sh"
