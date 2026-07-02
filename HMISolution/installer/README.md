# HMI Solution — Desktop Installer

This folder produces platform installers that deploy the desktop **Release**
builds of the HMI Solution components:

| Component | Project                            | Notes                        |
|-----------|-------------------------------------|------------------------------|
| Runtime   | `RuntimeViewer.Desktop`             | Photino.NET native viewer    |
| Editor    | `ServerEditorWeb.Desktop`           | Photino.NET native editor    |
| Server    | `Server`                            | OPC UA server (console/svc)  |

The end-user installers let the user pick which components to install.

---

## Files

| File                          | Role                                                    |
|-------------------------------|---------------------------------------------------------|
| `Build-Installer.ps1`         | Builds & packages Windows `.zip` and Linux `.tar.gz`.   |
| `Install-HMISolution.ps1`     | Windows installer (bundled inside the `.zip`).          |
| `install-hmi-solution.sh`     | Linux installer  (bundled inside the `.tar.gz`).        |

Both installers expect a `payload/` directory sitting next to them with
`Runtime/`, `Editor/` and `Server/` subfolders. The build script arranges
this automatically.

---

## Building the installers

Requires the **.NET 10 SDK** on `PATH`.

```powershell
cd HMISolution\installer

# Build both platforms
.\Build-Installer.ps1

# Windows only
.\Build-Installer.ps1 -Platforms Windows

# Linux only (needs tar; ships with Windows 10+)
.\Build-Installer.ps1 -Platforms Linux
```

Output is written to `HMISolution\installer\dist\`:

```
HMISolution-Installer-Windows-<version>.zip
HMISolution-Installer-Linux-<version>.tar.gz
```

Each archive contains the appropriate installer script plus a `payload/`
directory with self-contained Release binaries for the three components.

---

## End-user: Windows

1. Extract `HMISolution-Installer-Windows-<version>.zip`.
2. Right-click **`Install.cmd`** → **Run as administrator**
   *(or run `Install-HMISolution.ps1` from an elevated PowerShell).*
3. When prompted, select the components to install:
   - `1` Runtime Viewer
   - `2` Server Editor
   - `3` HMI OPC UA Server
   - `A` All

Non-interactive example:

```powershell
powershell -ExecutionPolicy Bypass -File .\Install-HMISolution.ps1 `
    -Components Runtime,Editor `
    -InstallDir "C:\HMI" `
    -Silent
```

Options:

| Parameter        | Description                                              |
|------------------|----------------------------------------------------------|
| `-Components`    | `Runtime`, `Editor`, `Server`, `All` (comma-separated).  |
| `-InstallDir`    | Target folder. Default: `%ProgramFiles%\HMI Solution`.   |
| `-NoShortcuts`   | Skip Start Menu shortcut creation.                       |
| `-Silent`        | No prompts; use provided flags / defaults.               |

The installer writes an `Uninstall-HMISolution.ps1` alongside the app.

---

## End-user: Linux

```bash
tar -xzf HMISolution-Installer-Linux-<version>.tar.gz
cd <extracted-folder>

# System-wide (default, needs root):
sudo ./install-hmi-solution.sh

# User install (no root):
./install-hmi-solution.sh --user

# Non-interactive:
sudo ./install-hmi-solution.sh --components Runtime,Server --silent --service
```

Options:

| Flag              | Description                                              |
|-------------------|----------------------------------------------------------|
| `--components`    | `Runtime`, `Editor`, `Server`, `All`.                    |
| `--install-dir`   | Target folder. Default: `/opt/hmi-solution` (system)     |
|                   | or `~/.local/share/hmi-solution` (user).                 |
| `--user`          | Install for current user only.                           |
| `--system`        | Install system-wide (default when root).                 |
| `--no-shortcuts`  | Skip `.desktop` launcher creation.                       |
| `--service`       | Install a `hmi-server.service` systemd unit (root only). |
| `--silent`        | No prompts.                                              |

`.desktop` launcher entries are created for **Runtime** and **Editor** only
(Server is a headless console). With `--service`, the installer registers
`/etc/systemd/system/hmi-server.service`; enable and start with:

```bash
sudo systemctl enable --now hmi-server
```

An `uninstall-hmi-solution.sh` is written to the install directory.

---

## Notes

- Components are published **self-contained** for `win-x64` / `linux-x64`,
  so end-users do not need a .NET runtime installed.
- The `Server` binary reads a `nodes.json` in its working directory (see
  the deployment guide in `HMISolution/docs/DEPLOYMENT.md`).
- The `Editor` desktop shell launches the web editor process; both apps
  discover their sibling web process from the same folder.
