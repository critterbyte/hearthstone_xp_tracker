# Builds the solution and runs the tests.
# Usage: .\build.ps1 [-Configuration Release]
param(
    [string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'

dotnet build (Join-Path $PSScriptRoot 'HearthstoneXpTracker.slnx') -c $Configuration
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet test (Join-Path $PSScriptRoot 'tests\XpTracker.Core.Tests') -c $Configuration --no-build
exit $LASTEXITCODE
