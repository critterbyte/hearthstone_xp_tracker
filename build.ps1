# Builds the solution against the newest local Hearthstone Deck Tracker installation and runs the tests.
# Usage: .\build.ps1 [-Configuration Release] [-HdtDir <folder containing HearthstoneDeckTracker.exe>]
param(
    [string]$Configuration = 'Release',
    [string]$HdtDir
)
$ErrorActionPreference = 'Stop'

if (-not $HdtDir) {
    # HDT installs each version into its own app-x.y.z folder; pick the highest version.
    $root = Join-Path $env:LOCALAPPDATA 'HearthstoneDeckTracker'
    $HdtDir = Get-ChildItem $root -Directory -Filter 'app-*' -ErrorAction SilentlyContinue |
        Sort-Object { [version]($_.Name -replace '^app-', '') } -Descending |
        Select-Object -First 1 -ExpandProperty FullName
    if (-not $HdtDir) { throw "No Hearthstone Deck Tracker installation found under $root. Pass -HdtDir." }
}
Write-Host "Using HDT: $HdtDir"

dotnet build (Join-Path $PSScriptRoot 'HearthstoneXpTracker.slnx') -c $Configuration "-p:HdtDir=$HdtDir"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet test (Join-Path $PSScriptRoot 'tests\XpTracker.Core.Tests') -c $Configuration --no-build
exit $LASTEXITCODE
