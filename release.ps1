# Builds the release ZIP for GitHub: the app, its libraries, the licenses and a short install note.
# It builds and tests first, checks the version and the package content, and prints the SHA-256 checksum.
# It does not tag, push or publish anything.
#
# Usage: .\release.ps1 [-AllowDirty]
param(
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
& (Join-Path $root 'build.ps1') -Configuration Release
if ($LASTEXITCODE -ne 0) {
    throw "Build or tests failed."
}

# 3. The app and the calculation engine must carry the version from Directory.Build.props.
$bin = Join-Path $root 'src\XpTracker.App\bin\Release'
foreach ($name in @('HearthstoneXpTracker.exe', 'XpTracker.Core.dll')) {
    $built = [Reflection.AssemblyName]::GetAssemblyName((Join-Path $bin $name)).Version
    if ("$($built.Major).$($built.Minor).$($built.Build)" -ne $version) {
        throw "$name has version $built, expected $version."
    }
}

# 4. Put the package together in a folder that can be extracted anywhere.
$artifacts = Join-Path $root 'artifacts'
$staging = Join-Path $artifacts 'staging'
$package = Join-Path $staging 'HearthstoneXpTracker'
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory -Force $package | Out-Null

$binaries = @(
    'HearthstoneXpTracker.exe',
    'HearthstoneXpTracker.exe.config',
    'XpTracker.Core.dll',
    'LiveCharts.dll',
    'LiveCharts.Wpf.dll',
    'Newtonsoft.Json.dll'
)
foreach ($name in $binaries) {
    Copy-Item (Join-Path $bin $name) (Join-Path $package $name)
}
Copy-Item (Join-Path $root 'LICENSE') (Join-Path $package 'LICENSE.txt')
Copy-Item (Join-Path $root 'THIRD-PARTY-NOTICES.txt') (Join-Path $package 'THIRD-PARTY-NOTICES.txt')

# Links to the translated install guides; the language names need this file to be saved as UTF-8 with BOM.
$docs = 'https://github.com/critterbyte/hearthstone_xp_tracker/blob/main/docs'
$install = @"
Hearthstone XP Tracker $version
A small Windows program that shows the reward track XP of the current Hearthstone game.
It only reads Hearthstone's log file; it does not change the game.

Install
1. Right-click the ZIP file, choose Properties, and select "Unblock" if it is shown.
2. Extract the ZIP into a folder of your choice, for example Documents.
   This creates the folder "HearthstoneXpTracker".
3. Start HearthstoneXpTracker.exe in that folder.
   If Windows shows "Windows protected your PC", choose "More info" > "Run anyway".
   The program is not signed yet, so Windows does not know it.
4. The dashboard opens. Closing it keeps the tracker running: the gold "XP" icon in the
   notification area (bottom right, maybe behind the ^ arrow) opens it again.
   During a game the overlay appears over Hearthstone.
If Hearthstone was already running at the first start, restart it once,
so that it writes the game log the tracker reads.

Coming from the Hearthstone Deck Tracker plugin
HDT stops supporting plugins on 30 November 2026; this program replaces the plugin.
It takes over your game history and settings automatically.
Disable the plugin in HDT (Settings > Tracker > Plugins), otherwise two overlays are shown.

Update
Exit the tracker (right-click the icon > Exit) and extract the new ZIP into the same folder,
replacing the old files. Your game history and settings are kept.

Uninstall
Exit the tracker and delete its folder.
The game history and settings are in %APPDATA%\HearthstoneXpTracker; delete it to remove them too.

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

License: MIT, see LICENSE.txt. Included libraries: see THIRD-PARTY-NOTICES.txt.
This is an unofficial program. It is not affiliated with Blizzard Entertainment or HearthSim.
"@
[IO.File]::WriteAllText((Join-Path $package 'INSTALL.txt'), $install.Replace("`r`n", "`n").Replace("`n", "`r`n"), (New-Object Text.UTF8Encoding($false)))

# 5. Only these files may be in the package.
$expected = @($binaries + @('INSTALL.txt', 'LICENSE.txt', 'THIRD-PARTY-NOTICES.txt') | Sort-Object)
$actual = @(Get-ChildItem $package -Recurse -File | ForEach-Object { $_.Name } | Sort-Object)
if (Compare-Object $expected $actual) {
    throw "Unexpected package content: $($actual -join ', ')"
}

# 6. Create the ZIP with the folder inside.
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
