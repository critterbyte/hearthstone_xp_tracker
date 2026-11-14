# Builds the release ZIP for GitHub: the two plugin DLLs, the license and a short install note.
# It builds and tests first, checks the version and the package content, and prints the SHA-256 checksum.
# It does not tag, push or publish anything.
#
# Usage: .\release.ps1 [-HdtDir <folder containing HearthstoneDeckTracker.exe>] [-AllowDirty]
param(
    [string]$HdtDir,
    # Build even if there are uncommitted changes (for trying the script; not for a real release).
    [switch]$AllowDirty
)
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

# 1. A release must match a committed state of the repository.
$dirty = git -C $root status --porcelain
if ($dirty -and -not $AllowDirty) {
    throw "There are uncommitted changes. Commit them first, or use -AllowDirty to try the script."
}
if ($dirty) {
    Write-Warning "Uncommitted changes are included. Do not publish this ZIP."
}

$props = [xml](Get-Content (Join-Path $root 'Directory.Build.props') -Raw)
$version = @($props.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ })[0]
if ($version -notmatch '^\d+\.\d+\.\d+$') {
    throw "Version '$version' in Directory.Build.props is not of the form X.Y.Z."
}
Write-Host "Version: $version"

# 2. Build and test.
$buildArgs = @{ Configuration = 'Release' }
if ($HdtDir) { $buildArgs.HdtDir = $HdtDir }
& (Join-Path $root 'build.ps1') @buildArgs
if ($LASTEXITCODE -ne 0) {
    throw "Build or tests failed."
}

# 3. The built DLLs must carry the version from Directory.Build.props.
$sources = [ordered]@{
    'HearthstoneXpTracker.dll' = Join-Path $root 'src\XpTracker.Plugin\bin\Release\HearthstoneXpTracker.dll'
    'XpTracker.Core.dll'       = Join-Path $root 'src\XpTracker.Core\bin\Release\net472\XpTracker.Core.dll'
}
foreach ($name in $sources.Keys) {
    $built = [Reflection.AssemblyName]::GetAssemblyName($sources[$name]).Version
    if ("$($built.Major).$($built.Minor).$($built.Build)" -ne $version) {
        throw "$name has version $built, expected $version."
    }
}

# 4. Put the package together in a folder named like the plugin folder in HDT.
$artifacts = Join-Path $root 'artifacts'
$staging = Join-Path $artifacts 'staging'
$package = Join-Path $staging 'HearthstoneXpTracker'
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory -Force $package | Out-Null

foreach ($name in $sources.Keys) {
    Copy-Item $sources[$name] (Join-Path $package $name)
}
Copy-Item (Join-Path $root 'LICENSE') (Join-Path $package 'LICENSE.txt')

# Links to the translated install guides; the language names need this file to be saved as UTF-8 with BOM.
$docs = 'https://github.com/critterbyte/hearthstone_xp_tracker/blob/main/docs'
$install = @"
Hearthstone XP Tracker $version
A plugin for Hearthstone Deck Tracker (HDT) that shows the reward track XP of the current game.

Install
1. Close HDT.
2. Right-click the ZIP file, choose Properties, and select "Unblock" if it is shown.
   Otherwise Windows may block the plugin.
3. Extract the ZIP into the HDT plugins folder:
   %APPDATA%\HearthstoneDeckTracker\Plugins
   This creates the folder "HearthstoneXpTracker" with both DLL files.
4. Start HDT. Open Settings (the gear icon on the right of the top bar) > Tracker > Plugins
   and enable "Hearthstone XP Tracker".

Update
Close HDT and extract the new ZIP the same way, replacing the old files.
Your game history and settings are kept.

Documentation: https://github.com/critterbyte/hearthstone_xp_tracker
Language of the overlay and the dashboard: open the dashboard and choose "Language".

Install guide in other languages:
  Deutsch          $docs/de/INSTALL.md
  Español (AL)     $docs/es-MX/INSTALL.md
  Español (EU)     $docs/es-ES/INSTALL.md
  Français         $docs/fr/INSTALL.md
  Italiano         $docs/it/INSTALL.md
  日本語           $docs/ja/INSTALL.md
  Polski           $docs/pl/INSTALL.md
  Português        $docs/pt-BR/INSTALL.md
  Русский          $docs/ru/INSTALL.md
  ภาษาไทย          $docs/th/INSTALL.md
  한국어           $docs/ko/INSTALL.md
  繁體中文         $docs/zh-TW/INSTALL.md
  简体中文         $docs/zh-CN/INSTALL.md

License: MIT, see LICENSE.txt.
This is an unofficial plugin. It is not affiliated with HearthSim or Blizzard Entertainment.
"@
[IO.File]::WriteAllText((Join-Path $package 'INSTALL.txt'), $install.Replace("`r`n", "`n").Replace("`n", "`r`n"), (New-Object Text.UTF8Encoding($false)))

# 5. Only these files may be in the package. HDT files must never be shipped.
$expected = @('HearthstoneXpTracker.dll', 'INSTALL.txt', 'LICENSE.txt', 'XpTracker.Core.dll')
$actual = @(Get-ChildItem $package -Recurse -File | ForEach-Object { $_.Name } | Sort-Object)
if (Compare-Object $expected $actual) {
    throw "Unexpected package content: $($actual -join ', ')"
}

# 6. Create the ZIP with the folder inside, so extracting it into the plugins folder is enough.
$zip = Join-Path $artifacts "HearthstoneXpTracker-$version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem
# Entries are added one by one with "/" as the separator. ZipFile.CreateFromDirectory on .NET Framework
# (Windows PowerShell 5.1) writes "\", which some unzip tools do not understand.
$archive = [IO.Compression.ZipFile]::Open($zip, [IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($name in $expected) {
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, (Join-Path $package $name), "HearthstoneXpTracker/$name", [IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
}
finally {
    $archive.Dispose()
}

$archive = [IO.Compression.ZipFile]::OpenRead($zip)
try {
    $entries = @($archive.Entries | ForEach-Object { $_.FullName } | Sort-Object)
}
finally {
    $archive.Dispose()
}
$expectedEntries = @($expected | ForEach-Object { "HearthstoneXpTracker/$_" })
if (Compare-Object $expectedEntries $entries) {
    throw "Unexpected ZIP entries: $($entries -join ', ')"
}

# 7. Checksum for the release notes.
$hash = (Get-FileHash $zip -Algorithm SHA256).Hash.ToLowerInvariant()
Set-Content -Path "$zip.sha256" -Value "$hash  $(Split-Path $zip -Leaf)" -Encoding Ascii
Remove-Item $staging -Recurse -Force

Write-Host ""
Write-Host "Package: $zip"
$entries | ForEach-Object { Write-Host "  $_" }
Write-Host "SHA-256: $hash"
Write-Host ""
Write-Host "To publish (after committing, merging and testing):"
Write-Host "  git tag v$version"
Write-Host "  git push origin v$version"
Write-Host "  gh release create v$version `"$zip`" --prerelease --title `"v$version`" --notes-file <release notes>"
