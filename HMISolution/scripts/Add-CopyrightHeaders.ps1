# Adds a copyright header to source files across the HMI Solution.
# Usage: pwsh -File .\Add-CopyrightHeaders.ps1 [-DryRun]
[CmdletBinding()]
param(
    [switch]$DryRun,
    [string]$Owner = 'Claudio Fiorani',
    [int]$Year = (Get-Date).Year
)

$ErrorActionPreference = 'Stop'
$repoRoot = 'C:\Users\cfior\source\repos'
$slnx     = Join-Path $repoRoot 'HMISolution\HMISolution.slnx'

[xml]$sln = Get-Content $slnx
$slnDir = Split-Path $slnx -Parent
$projectDirs = @()
foreach ($p in $sln.SelectNodes('//Project')) {
    $full = [IO.Path]::GetFullPath((Join-Path $slnDir $p.Path))
    $projectDirs += (Split-Path $full -Parent)
}
# Also include the HMISolution folder itself (Program.cs, scripts, installer, etc.)
$projectDirs += $slnDir
$projectDirs = $projectDirs | Sort-Object -Unique

$excludeDirs = @('bin','obj','publish','node_modules','.git','.vs',
                 'BenchmarkDotNet.Artifacts','TestResults','packages','dist','stage')

$extensions = @('.cs','.razor','.ps1','.sh')

$marker = "Copyright (c) "  # any year followed by owner name in first lines counts as present

function Is-Excluded([string]$path) {
    foreach ($e in $excludeDirs) {
        if ($path -match "[\\/]$([regex]::Escape($e))[\\/]") { return $true }
        if ($path -match "[\\/]$([regex]::Escape($e))$")     { return $true }
    }
    return $false
}

function Get-Files([string]$root) {
    if (-not (Test-Path $root)) { return @() }
    Get-ChildItem -LiteralPath $root -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $extensions -contains $_.Extension.ToLower() } |
        Where-Object { -not (Is-Excluded $_.FullName) } |
        Where-Object {
            # Skip auto-generated designer / assembly info files
            $n = $_.Name
            -not ($n -like '*.Designer.cs' -or
                  $n -like '*.g.cs'        -or
                  $n -like '*.g.i.cs'      -or
                  $n -like 'GlobalUsings.g.cs' -or
                  $n -like 'AssemblyAttributes.cs' -or
                  $n -like '*.AssemblyInfo.cs' -or
                  $n -like '*.GlobalUsings.g.cs' -or
                  $n -like '*.AssemblyAttributes.cs' -or
                  $n -like '*.NuGet.dgspec.json')
        }
}

function Get-HeaderFor([string]$ext) {
    switch ($ext.ToLower()) {
        '.cs'    { return @("// Copyright (c) $Year $Owner","// All rights reserved.") }
        '.razor' { return @("@* Copyright (c) $Year $Owner - All rights reserved. *@") }
        '.ps1'   { return @("# Copyright (c) $Year $Owner","# All rights reserved.") }
        '.sh'    { return @("# Copyright (c) $Year $Owner","# All rights reserved.") }
    }
}

function Detect-Newline([string]$text) {
    if ($text -match "`r`n") { return "`r`n" } else { return "`n" }
}

function Has-Header([string]$text) {
    # Look for existing copyright by us in the first ~1KB
    $head = if ($text.Length -gt 1024) { $text.Substring(0,1024) } else { $text }
    return ($head -match [regex]::Escape($Owner) -and $head -match 'Copyright')
}

function Split-PreservePreamble([string]$ext, [string[]]$lines) {
    # Returns [preamble[], rest[]] where preamble is kept at top
    # (shebang for .sh, #Requires for .ps1)
    $preamble = @()
    $i = 0
    if ($ext -eq '.sh') {
        if ($lines.Count -gt 0 -and $lines[0].StartsWith('#!')) {
            $preamble += $lines[0]; $i = 1
        }
    }
    elseif ($ext -eq '.ps1') {
        while ($i -lt $lines.Count -and $lines[$i] -match '^\s*#Requires\b') {
            $preamble += $lines[$i]; $i++
        }
    }
    $rest = if ($i -lt $lines.Count) { $lines[$i..($lines.Count-1)] } else { @() }
    return @{ Preamble = $preamble; Rest = $rest }
}

$processed = 0
$skipped   = 0
$modified  = 0
$errors    = @()

foreach ($dir in $projectDirs) {
    $files = Get-Files $dir
    foreach ($f in $files) {
        $processed++
        try {
            $text = [IO.File]::ReadAllText($f.FullName)
            if (Has-Header $text) { $skipped++; continue }

            $nl = Detect-Newline $text

            # Preserve BOM if present
            $bytes = [IO.File]::ReadAllBytes($f.FullName)
            $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF

            $ext = $f.Extension
            $lines = $text -split "`r?`n"
            $split    = Split-PreservePreamble $ext $lines
            $preamble = @($split.Preamble)
            $rest     = @($split.Rest)

            $header = @(Get-HeaderFor $ext)
            $blank  = @('')

            $newLines = New-Object System.Collections.Generic.List[string]
            foreach ($l in $preamble) { $newLines.Add([string]$l) }
            foreach ($l in $header)   { $newLines.Add([string]$l) }
            foreach ($l in $blank)    { $newLines.Add([string]$l) }
            foreach ($l in $rest)     { $newLines.Add([string]$l) }

            $newText = ($newLines -join $nl)
            if ($DryRun) {
                Write-Host "would modify: $($f.FullName)"
            } else {
                $enc = New-Object System.Text.UTF8Encoding($hasBom)
                [IO.File]::WriteAllText($f.FullName, $newText, $enc)
            }
            $modified++
        }
        catch {
            $errors += "$($f.FullName): $($_.Exception.Message)"
        }
    }
}

Write-Host ""
Write-Host "Processed : $processed"
Write-Host "Modified  : $modified"
Write-Host "Skipped   : $skipped (already have header)"
if ($errors.Count) {
    Write-Host "Errors    : $($errors.Count)" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}
