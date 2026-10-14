# Increments the build number (third segment) in Version.props.
# Called automatically before every build (Debug and Release) via HMISolution.csproj.
param(
    [string]$VersionPropsPath = (Join-Path $PSScriptRoot '..\Version.props')
)

$VersionPropsPath = (Resolve-Path $VersionPropsPath).Path
[xml]$xml = Get-Content -Path $VersionPropsPath -Raw

$versionNode = $xml.Project.PropertyGroup.HMIVersion
$parts = $versionNode.Trim().Split('.')

if ($parts.Length -ge 3) {
    $parts[2] = ([int]$parts[2] + 1).ToString()
} else {
    $parts += '1'
}

$newVersion = [string]::Join('.', $parts)
$xml.Project.PropertyGroup.HMIVersion = $newVersion

$xml.Save($VersionPropsPath)

Write-Host "Incremented HMIVersion: $newVersion"
