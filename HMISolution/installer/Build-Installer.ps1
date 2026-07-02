#Requires -Version 5.1
<#
.SYNOPSIS
    Builds HMI Solution installer packages for Windows and Linux.

.DESCRIPTION
    Publishes the three desktop Release components (RuntimeViewer.Desktop,
    ServerEditorWeb.Desktop, Server) as self-contained binaries for the
    selected runtime identifiers, arranges them into a payload directory
    alongside the platform installer, and produces distributable archives:

        HMISolution-Installer-Windows-<version>.zip
        HMISolution-Installer-Linux-<version>.tar.gz

    Requires the .NET 10 SDK on PATH.

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

# Read version from Version.props if not provided
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

$projects = @(
    [pscustomobject]@{ Key='Runtime'; Path = Join-Path $srcRoot 'RuntimeViewer.Desktop\RuntimeViewer.Desktop.csproj' }
    [pscustomobject]@{ Key='Editor';  Path = Join-Path $srcRoot 'ServerEditorWeb.Desktop\ServerEditorWeb.Desktop.csproj' }
    [pscustomobject]@{ Key='Server';  Path = Join-Path $srcRoot 'Server\Server.csproj' }
)

foreach ($p in $projects) {
    if (-not (Test-Path $p.Path)) { throw "Project not found: $($p.Path)" }
}

function Publish-Project {
    param($Project, [string]$Rid, [string]$OutDir)

    if ($SkipPublish -and (Test-Path $OutDir) -and (Get-ChildItem $OutDir -File -ErrorAction SilentlyContinue)) {
        Write-Host "  [skip] $($Project.Key) ($Rid) - existing output" -ForegroundColor DarkGray
        return
    }
    Write-Host "  publishing $($Project.Key) -> $Rid" -ForegroundColor Green
    New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
    & dotnet publish $Project.Path `
        -c $Configuration `
        -r $Rid `
        --self-contained true `
        -p:PublishSingleFile=false `
        -o $OutDir `
        --nologo `
        -v minimal
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed for $($Project.Key) / $Rid" }
}

function New-Payload {
    param([string]$Rid, [string]$StageRoot)

    $payload = Join-Path $StageRoot 'payload'
    if (Test-Path $payload) { Remove-Item -Recurse -Force $payload }
    New-Item -ItemType Directory -Force -Path $payload | Out-Null

    foreach ($p in $projects) {
        $pubDir = Join-Path $here "publish\$Rid\$($p.Key)"
        Publish-Project -Project $p -Rid $Rid -OutDir $pubDir
        $dest = Join-Path $payload $p.Key
        New-Item -ItemType Directory -Force -Path $dest | Out-Null
        Copy-Item -Path (Join-Path $pubDir '*') -Destination $dest -Recurse -Force
    }
    return $payload
}

function Build-WindowsPackage {
    $rid       = 'win-x64'
    $stage     = Join-Path $here "stage\$rid"
    if (Test-Path $stage) { Remove-Item -Recurse -Force $stage }
    New-Item -ItemType Directory -Force -Path $stage | Out-Null

    Copy-Item (Join-Path $here 'Install-HMISolution.ps1') $stage
    if (Test-Path (Join-Path $here 'README.md')) {
        Copy-Item (Join-Path $here 'README.md') $stage
    }
    # Convenience .cmd wrapper so end-users can double-click
    @'
@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Install-HMISolution.ps1" %*
pause
'@ | Set-Content -Path (Join-Path $stage 'Install.cmd') -Encoding ASCII

    New-Payload -Rid $rid -StageRoot $stage | Out-Null

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

    New-Payload -Rid $rid -StageRoot $stage | Out-Null

    # Make Linux executables + install script executable inside the archive
    $tar = Get-Command tar -ErrorAction SilentlyContinue
    if (-not $tar) { throw "tar is required to build the Linux package (available in Windows 10+)." }

    $archive = Join-Path $OutputDir "HMISolution-Installer-Linux-$Version.tar.gz"
    if (Test-Path $archive) { Remove-Item -Force $archive }

    # Normalize the install script to LF and add executable bit inside the tar
    $shPath = Join-Path $stage 'install-hmi-solution.sh'
    $sh = [IO.File]::ReadAllText($shPath) -replace "`r`n", "`n"
    [IO.File]::WriteAllText($shPath, $sh)

    Write-Host "  packing $archive" -ForegroundColor Green
    Push-Location $stage
    try {
        # Use tar's --mode to ensure executables retain +x on extraction
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
