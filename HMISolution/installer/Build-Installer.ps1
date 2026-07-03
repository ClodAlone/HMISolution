#Requires -Version 5.1
<#
.SYNOPSIS
    Builds HMI Solution installer packages for Windows and Linux.

.DESCRIPTION
    Publishes the desktop Release components (RuntimeViewer.Desktop,
    ServerEditorWeb.Desktop, Server) as self-contained binaries for the
    selected runtime identifiers, plus:

      * the RuntimeViewer web app that RuntimeViewer.Desktop hosts,
      * the ServerEditorWeb web app that ServerEditorWeb.Desktop hosts,
      * the ServerEditorWeb SymbolLibrary (SVG symbols) and wwwroot,
      * all 12 driver DLLs (Modbus, S7, MQTT, OPC UA Client, EtherNet/IP,
        KNX, CSV, REST, TCP, SQL, SparkplugB, Simulation) plus their
        dependencies into the Server payload's drivers/ subfolder.

    Layout produced inside each archive:
        payload/
            Runtime/  RuntimeViewer.Desktop + RuntimeViewer web + wwwroot
            Editor/   ServerEditorWeb.Desktop + ServerEditorWeb web +
                      SymbolLibrary + wwwroot
            Server/   Server executable + drivers/ + runtimes/

.PARAMETER Platforms
    Which platforms to build. Values: Windows, Linux, Both. Default: Both.

.PARAMETER Configuration
    MSBuild configuration. Default: Release.

.PARAMETER OutputDir
    Where the packaged archives are written. Default: .\dist

.PARAMETER Version
    Version string used in the archive names. Default: read from ..\Version.props.

.PARAMETER SkipPublish
    Reuse existing publish output (development iteration).

.EXAMPLE
    .\Build-Installer.ps1

.EXAMPLE
    .\Build-Installer.ps1 -Platforms Windows
#>
[CmdletBinding()]
param(
    [ValidateSet('Windows','Linux','Both')]
    [string]$Platforms = 'Both',
    [string]$Configuration = 'Release',
    [string]$OutputDir,
    [string]$Version,
    [switch]$SkipPublish
)

$ErrorActionPreference = 'Stop'
$here    = Split-Path -Parent $MyInvocation.MyCommand.Path
$repo    = Resolve-Path (Join-Path $here '..')          # HMISolution folder
$srcRoot = Resolve-Path (Join-Path $repo '..')          # sibling projects root

if (-not $OutputDir) { $OutputDir = Join-Path $here 'dist' }
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

if (-not $Version) {
    $vp = Join-Path $repo 'Version.props'
    if (Test-Path $vp) {
        [xml]$xml = Get-Content $vp
        $Version = $xml.Project.PropertyGroup.HMIVersion
    }
    if (-not $Version) { $Version = '0.0.0' }
}

Write-Host "HMI Solution installer builder  (version $Version)" -ForegroundColor Cyan
Write-Host "  Repo root      : $srcRoot"
Write-Host "  Configuration  : $Configuration"
Write-Host "  Output         : $OutputDir"
Write-Host ""

# ---------- project catalogue ----------
$desktopShells = @{
    Runtime = Join-Path $srcRoot 'RuntimeViewer.Desktop\RuntimeViewer.Desktop.csproj'
    Editor  = Join-Path $srcRoot 'ServerEditorWeb.Desktop\ServerEditorWeb.Desktop.csproj'
}
$webApps = @{
    Runtime = Join-Path $srcRoot 'RuntimeViewer\RuntimeViewer.csproj'
    Editor  = Join-Path $srcRoot 'ServerEditorWeb\ServerEditorWeb.csproj'
}
$serverProject = Join-Path $srcRoot 'Server\Server.csproj'

$driverProjects = @(
    'Drivers.Csv','Drivers.Modbus','Drivers.S7','Drivers.Mqtt',
    'Drivers.EtherNetIP','Drivers.Sql','Drivers.OpcUaClient',
    'Drivers.Rest','Drivers.Tcp','Drivers.Knx','Drivers.Simulation',
    'Drivers.SparkplugB'
) | ForEach-Object { Join-Path $srcRoot "Drivers\$_\$_.csproj" }

# Sanity: all projects exist
foreach ($p in @($desktopShells.Values + $webApps.Values + $serverProject + $driverProjects)) {
    if (-not (Test-Path $p)) { throw "Project not found: $p" }
}

# ---------- helpers ----------
function Publish-One {
    param([string]$Project, [string]$Rid, [string]$OutDir, [switch]$NoSelfContained)

    if ($SkipPublish -and (Test-Path $OutDir) -and (Get-ChildItem $OutDir -File -ErrorAction SilentlyContinue)) {
        Write-Host "  [skip] $(Split-Path $Project -Leaf) ($Rid)" -ForegroundColor DarkGray
        return
    }
    Write-Host "  publishing $(Split-Path $Project -Leaf) -> $Rid" -ForegroundColor Green
    New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

    $selfArg = if ($NoSelfContained) { 'false' } else { 'true' }
    & dotnet publish $Project `
        -c $Configuration `
        -r $Rid `
        --self-contained $selfArg `
        -p:PublishSingleFile=false `
        -p:UseAppHost=true `
        -o $OutDir `
        --nologo `
        -v minimal
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed for $Project / $Rid" }
}

function Copy-Tree {
    param([string]$Src, [string]$Dst)
    if (-not (Test-Path $Src)) { throw "Source missing: $Src" }
    New-Item -ItemType Directory -Force -Path $Dst | Out-Null
    Copy-Item -Path (Join-Path $Src '*') -Destination $Dst -Recurse -Force
}

function Build-Payload {
    param([string]$Rid, [string]$StageRoot)

    $payload = Join-Path $StageRoot 'payload'
    if (Test-Path $payload) { Remove-Item -Recurse -Force $payload }
    New-Item -ItemType Directory -Force -Path $payload | Out-Null

    # ----- Runtime -----
    $runtimeDest = Join-Path $payload 'Runtime'
    # Web viewer first (self-contained so no runtime dependency)
    $runtimeWebOut = Join-Path $here "publish\$Rid\RuntimeViewerWeb"
    Publish-One -Project $webApps.Runtime -Rid $Rid -OutDir $runtimeWebOut
    Copy-Tree -Src $runtimeWebOut -Dst $runtimeDest
    # Desktop shell overlaid on top (adds Photino + RuntimeViewer.Desktop.exe)
    $runtimeShellOut = Join-Path $here "publish\$Rid\RuntimeViewerDesktop"
    Publish-One -Project $desktopShells.Runtime -Rid $Rid -OutDir $runtimeShellOut
    Copy-Item -Path (Join-Path $runtimeShellOut '*') -Destination $runtimeDest -Recurse -Force

    # ----- Editor -----
    $editorDest = Join-Path $payload 'Editor'
    $editorWebOut = Join-Path $here "publish\$Rid\ServerEditorWeb"
    Publish-One -Project $webApps.Editor -Rid $Rid -OutDir $editorWebOut
    Copy-Tree -Src $editorWebOut -Dst $editorDest
    # SymbolLibrary is included via <Content Update="SymbolLibrary\**\*" CopyToPublishDirectory="PreserveNewest"/>
    # but double-check and copy from source if missing (older SDKs sometimes miss it)
    $symbolDest = Join-Path $editorDest 'SymbolLibrary'
    $symbolSrc  = Join-Path $srcRoot 'ServerEditorWeb\SymbolLibrary'
    if ((-not (Test-Path $symbolDest) -or -not (Get-ChildItem $symbolDest -Recurse -File -EA SilentlyContinue)) `
        -and (Test-Path $symbolSrc)) {
        Write-Host "  copying SymbolLibrary from source" -ForegroundColor Yellow
        Copy-Tree -Src $symbolSrc -Dst $symbolDest
    }
    $editorShellOut = Join-Path $here "publish\$Rid\ServerEditorWebDesktop"
    Publish-One -Project $desktopShells.Editor -Rid $Rid -OutDir $editorShellOut
    Copy-Item -Path (Join-Path $editorShellOut '*') -Destination $editorDest -Recurse -Force

    # ----- Server -----
    $serverDest = Join-Path $payload 'Server'
    $serverOut  = Join-Path $here "publish\$Rid\Server"
    Publish-One -Project $serverProject -Rid $Rid -OutDir $serverOut
    Copy-Tree -Src $serverOut -Dst $serverDest

    # Publish each driver individually and copy its DLLs (+ dependencies) into Server/drivers
    $driversDest = Join-Path $serverDest 'drivers'
    New-Item -ItemType Directory -Force -Path $driversDest | Out-Null
    foreach ($drv in $driverProjects) {
        $name   = [IO.Path]::GetFileNameWithoutExtension($drv)
        $drvOut = Join-Path $here "publish\$Rid\$name"
        # Drivers are class libs -> publish framework-dependent to grab their transitive deps
        Publish-One -Project $drv -Rid $Rid -OutDir $drvOut -NoSelfContained
        # Copy only DLLs the driver actually needs, skipping those already in the Server payload
        $serverDlls = @{}
        Get-ChildItem $serverDest -Filter *.dll -File | ForEach-Object { $serverDlls[$_.Name] = $true }
        Get-ChildItem $drvOut -Filter *.dll -File | Where-Object { -not $serverDlls.ContainsKey($_.Name) } |
            Copy-Item -Destination $driversDest -Force
        # Copy the driver's own DLL unconditionally (must live in drivers/ folder)
        $ownDll = Join-Path $drvOut "$name.dll"
        if (Test-Path $ownDll) { Copy-Item $ownDll -Destination $driversDest -Force }
    }

    Write-Host ""
    Write-Host "  Payload summary for ${Rid}:" -ForegroundColor Cyan
    foreach ($sub in 'Runtime','Editor','Server') {
        $dir = Join-Path $payload $sub
        if (Test-Path $dir) {
            $c = (Get-ChildItem $dir -Recurse -File).Count
            $sz = [math]::Round(((Get-ChildItem $dir -Recurse -File | Measure-Object Length -Sum).Sum / 1MB), 1)
            Write-Host ("    {0,-8} {1,6} files, {2,7} MB" -f $sub, $c, $sz)
        }
    }
    $drvCount = (Get-ChildItem $driversDest -Filter '*Drivers.*.dll' -File -EA SilentlyContinue).Count
    Write-Host ("    drivers   {0} driver DLLs in Server/drivers/" -f $drvCount)
    Write-Host ""
    return $payload
}

function Build-WindowsPackage {
    $rid   = 'win-x64'
    $stage = Join-Path $here "stage\$rid"
    if (Test-Path $stage) { Remove-Item -Recurse -Force $stage }
    New-Item -ItemType Directory -Force -Path $stage | Out-Null

    Copy-Item (Join-Path $here 'Install-HMISolution.ps1') $stage
    if (Test-Path (Join-Path $here 'README.md')) {
        Copy-Item (Join-Path $here 'README.md') $stage
    }
    @'
@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Install-HMISolution.ps1" %*
pause
'@ | Set-Content -Path (Join-Path $stage 'Install.cmd') -Encoding ASCII

    Build-Payload -Rid $rid -StageRoot $stage | Out-Null

    $zip = Join-Path $OutputDir "HMISolution-Installer-Windows-$Version.zip"
    if (Test-Path $zip) { Remove-Item -Force $zip }
    Write-Host "  packing $zip" -ForegroundColor Green
    Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -Force
    Write-Host "  -> $zip" -ForegroundColor Cyan
}

function Build-LinuxPackage {
    $rid   = 'linux-x64'
    $stage = Join-Path $here "stage\$rid"
    if (Test-Path $stage) { Remove-Item -Recurse -Force $stage }
    New-Item -ItemType Directory -Force -Path $stage | Out-Null

    Copy-Item (Join-Path $here 'install-hmi-solution.sh') $stage
    if (Test-Path (Join-Path $here 'README.md')) {
        Copy-Item (Join-Path $here 'README.md') $stage
    }

    Build-Payload -Rid $rid -StageRoot $stage | Out-Null

    $tar = Get-Command tar -ErrorAction SilentlyContinue
    if (-not $tar) { throw "tar is required to build the Linux package (available in Windows 10+)." }

    $archive = Join-Path $OutputDir "HMISolution-Installer-Linux-$Version.tar.gz"
    if (Test-Path $archive) { Remove-Item -Force $archive }

    # Normalise install script to LF
    $shPath = Join-Path $stage 'install-hmi-solution.sh'
    $sh = [IO.File]::ReadAllText($shPath) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($shPath, $sh)

    Write-Host "  packing $archive" -ForegroundColor Green
    Push-Location $stage
    try {
        & tar --format=ustar -czf $archive `
            --mode='a+rX,u+w' `
            --owner=0 --group=0 `
            .
        if ($LASTEXITCODE -ne 0) { throw "tar failed with exit code $LASTEXITCODE" }
    }
    finally { Pop-Location }
    Write-Host "  -> $archive" -ForegroundColor Cyan
}

switch ($Platforms) {
    'Windows' { Build-WindowsPackage }
    'Linux'   { Build-LinuxPackage }
    'Both'    { Build-WindowsPackage; Build-LinuxPackage }
}

Write-Host ""
Write-Host "Done. Artifacts in: $OutputDir" -ForegroundColor Green
Get-ChildItem $OutputDir | Format-Table Name, Length, LastWriteTime
