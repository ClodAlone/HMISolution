#Requires -Version 5.1
<#
.SYNOPSIS
    HMI Solution Windows Installer.

.DESCRIPTION
    Installs the desktop Release builds of the HMI Solution runtime, editor
    and server. Components can be selected interactively or through the
    -Components parameter.

    The script expects to find a "payload" directory next to it with the
    following layout (produced by Build-Installer.ps1):

        payload\
            Runtime\   RuntimeViewer.Desktop.exe + support files
            Editor\    ServerEditorWeb.Desktop.exe + support files
            Server\    Server.exe + support files

.PARAMETER Components
    Comma-separated list of components to install.
    Valid values: Runtime, Editor, Server, All. Default: interactive prompt.

.PARAMETER InstallDir
    Target install directory. Default: %ProgramFiles%\HMI Solution.

.PARAMETER NoShortcuts
    Skip Start Menu / Desktop shortcut creation.

.PARAMETER Silent
    Non-interactive: use provided parameters and defaults, do not prompt.

.EXAMPLE
    .\Install-HMISolution.ps1

.EXAMPLE
    .\Install-HMISolution.ps1 -Components Runtime,Editor -Silent
#>
[CmdletBinding()]
param(
    [string]$Components,
    [string]$InstallDir,
    [switch]$NoShortcuts,
    [switch]$Silent
)

$ErrorActionPreference = 'Stop'
$script:ScriptRoot     = Split-Path -Parent $MyInvocation.MyCommand.Path
$script:PayloadRoot    = Join-Path $script:ScriptRoot 'payload'
$script:ProductName    = 'HMI Solution'
$script:AllComponents  = @(
    [pscustomobject]@{ Key = 'Runtime'; Folder = 'Runtime'; Exe = 'RuntimeViewer.Desktop.exe';   Display = 'Runtime Viewer (Desktop)' }
    [pscustomobject]@{ Key = 'Editor';  Folder = 'Editor';  Exe = 'ServerEditorWeb.Desktop.exe'; Display = 'Server Editor (Desktop)'  }
    [pscustomobject]@{ Key = 'Server';  Folder = 'Server';  Exe = 'Server.exe';                  Display = 'HMI OPC UA Server'        }
)

function Write-Header {
    Write-Host ''
    Write-Host '======================================================' -ForegroundColor Cyan
    Write-Host "  $script:ProductName - Windows Installer"              -ForegroundColor Cyan
    Write-Host '======================================================' -ForegroundColor Cyan
    Write-Host ''
}

function Test-Elevated {
    $id = [Security.Principal.WindowsIdentity]::GetCurrent()
    (New-Object Security.Principal.WindowsPrincipal($id)).IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Resolve-Components {
    param([string]$Requested)

    if ($Requested) {
        $items = $Requested -split '[,;\s]+' | Where-Object { $_ }
        if ($items -contains 'All') { return $script:AllComponents }
        $sel = @()
        foreach ($n in $items) {
            $c = $script:AllComponents | Where-Object { $_.Key -ieq $n }
            if (-not $c) { throw "Unknown component: '$n'. Valid: Runtime, Editor, Server, All." }
            $sel += $c
        }
        return $sel
    }

    if ($Silent) { return $script:AllComponents }

    Write-Host 'Select components to install:' -ForegroundColor Yellow
    for ($i = 0; $i -lt $script:AllComponents.Count; $i++) {
        $c = $script:AllComponents[$i]
        Write-Host ("  [{0}] {1,-30} ({2})" -f ($i + 1), $c.Display, $c.Key)
    }
    Write-Host '  [A] All components'
    Write-Host ''
    $ans = Read-Host 'Enter numbers separated by comma (e.g. 1,3) or A for all'
    if ([string]::IsNullOrWhiteSpace($ans) -or $ans -match '^[Aa]$') {
        return $script:AllComponents
    }
    $sel = @()
    foreach ($tok in ($ans -split '[,;\s]+' | Where-Object { $_ })) {
        $idx = 0
        if (-not [int]::TryParse($tok, [ref]$idx) -or $idx -lt 1 -or $idx -gt $script:AllComponents.Count) {
            throw "Invalid selection: '$tok'."
        }
        $sel += $script:AllComponents[$idx - 1]
    }
    return $sel
}

function Resolve-InstallDir {
    param([string]$Requested)

    if ($Requested) { return $Requested }
    $default = Join-Path $env:ProgramFiles $script:ProductName
    if ($Silent) { return $default }
    $ans = Read-Host "Install directory [$default]"
    if ([string]::IsNullOrWhiteSpace($ans)) { return $default }
    return $ans
}

function Install-Component {
    param($Component, [string]$Root)

    $src = Join-Path $script:PayloadRoot $Component.Folder
    if (-not (Test-Path $src)) {
        throw "Payload folder missing for $($Component.Key): $src"
    }
    $dst = Join-Path $Root $Component.Folder
    Write-Host "  -> Installing $($Component.Display)..." -ForegroundColor Green
    New-Item -ItemType Directory -Force -Path $dst | Out-Null
    Copy-Item -Path (Join-Path $src '*') -Destination $dst -Recurse -Force
}

function New-Shortcut {
    param([string]$LnkPath, [string]$Target, [string]$WorkDir, [string]$Description)

    $shell = New-Object -ComObject WScript.Shell
    $sc = $shell.CreateShortcut($LnkPath)
    $sc.TargetPath       = $Target
    $sc.WorkingDirectory = $WorkDir
    $sc.Description      = $Description
    $sc.Save()
}

function Add-Shortcuts {
    param($Components, [string]$Root)

    $menuDir = Join-Path ([Environment]::GetFolderPath('CommonPrograms')) $script:ProductName
    New-Item -ItemType Directory -Force -Path $menuDir | Out-Null

    foreach ($c in $Components) {
        $exe = Join-Path (Join-Path $Root $c.Folder) $c.Exe
        if (-not (Test-Path $exe)) {
            Write-Warning "Executable not found for shortcut: $exe"
            continue
        }
        $lnk = Join-Path $menuDir ("$($c.Display).lnk")
        New-Shortcut -LnkPath $lnk -Target $exe -WorkDir (Split-Path $exe) -Description $c.Display
        Write-Host "  -> Shortcut: $lnk" -ForegroundColor DarkGray
    }
}

function Write-Uninstaller {
    param($Components, [string]$Root)

    $uninstall = Join-Path $Root 'Uninstall-HMISolution.ps1'
    $installedKeys = ($Components | ForEach-Object { "'$($_.Key)'" }) -join ','
    $content = @"
#Requires -Version 5.1
`$ErrorActionPreference = 'Continue'
`$root = Split-Path -Parent `$MyInvocation.MyCommand.Path
Write-Host 'Uninstalling $script:ProductName from' `$root
`$menu = Join-Path ([Environment]::GetFolderPath('CommonPrograms')) '$script:ProductName'
if (Test-Path `$menu) { Remove-Item -Recurse -Force `$menu }
foreach (`$k in @($installedKeys)) {
    `$p = Join-Path `$root `$k
    if (Test-Path `$p) { Remove-Item -Recurse -Force `$p }
}
Remove-Item -Force (Join-Path `$root 'Uninstall-HMISolution.ps1') -ErrorAction SilentlyContinue
Write-Host 'Done. You may now delete' `$root
"@
    Set-Content -Path $uninstall -Value $content -Encoding UTF8
}

# ---------------- main ----------------
try {
    Write-Header

    if (-not (Test-Path $script:PayloadRoot)) {
        throw "Payload directory not found: $script:PayloadRoot`nRun Build-Installer.ps1 first to produce it."
    }

    $components = Resolve-Components -Requested $Components
    $installDir = Resolve-InstallDir -Requested $InstallDir

    Write-Host ''
    Write-Host 'Summary:' -ForegroundColor Yellow
    Write-Host "  Install directory : $installDir"
    Write-Host "  Components        : $(($components | ForEach-Object Key) -join ', ')"
    Write-Host "  Create shortcuts  : $([bool](-not $NoShortcuts))"
    Write-Host ''

    if (-not $Silent) {
        $confirm = Read-Host 'Proceed with installation? [Y/n]'
        if ($confirm -and $confirm -notmatch '^[Yy]') {
            Write-Host 'Aborted.' -ForegroundColor Yellow
            return
        }
    }

    $needsElevation = $installDir -like "$env:ProgramFiles*" -or $installDir -like "$env:ProgramFiles(x86)*"
    if ($needsElevation -and -not (Test-Elevated)) {
        throw "Installing to '$installDir' requires an elevated PowerShell (Run as Administrator)."
    }

    New-Item -ItemType Directory -Force -Path $installDir | Out-Null
    foreach ($c in $components) { Install-Component -Component $c -Root $installDir }

    if (-not $NoShortcuts) { Add-Shortcuts -Components $components -Root $installDir }

    Write-Uninstaller -Components $components -Root $installDir

    Write-Host ''
    Write-Host 'Installation complete.' -ForegroundColor Green
    Write-Host "Location: $installDir"
    Write-Host "Uninstall: powershell -ExecutionPolicy Bypass -File `"$installDir\Uninstall-HMISolution.ps1`""
}
catch {
    Write-Host ''
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
