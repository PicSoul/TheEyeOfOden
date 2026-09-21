<#
    Build and optionally install The Eye of Oden for local testing.

        .\build.ps1            build only
        .\build.ps1 -Install   build, then copy into the r2modman profile
        .\build.ps1 -Disable   stop BepInEx loading it, without deleting it
        .\build.ps1 -Enable    load it again

    -Disable and -Enable exist because this mod is copied into the profile rather
    than installed by r2modman, so r2modman does not list it and has no toggle for
    it. They rename the DLL to .old and back, which is the same trick r2modman uses
    on the mods it does manage.
#>

[CmdletBinding()]
param(
    [switch]$Install,
    [switch]$Disable,
    [switch]$Enable,
    [string]$Profile = "1.0 Release Client Mods"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

function Fail($msg) { Write-Host "FAIL: $msg" -ForegroundColor Red; exit 1 }
function Ok($msg)   { Write-Host "  ok   $msg" -ForegroundColor Green }

function PluginRoot {
    $path = "$env:APPDATA\com.kesomannen.gale\valheim\profiles\$Profile\BepInEx\plugins"
    if (-not (Test-Path $path)) { Fail "profile not found: $path" }
    return $path
}

function RequireValheimClosed {
    if (Get-Process -Name "valheim" -ErrorAction SilentlyContinue) {
        Fail "Valheim is running; close it first"
    }
}

function SetLoaded([bool]$loaded) {
    RequireValheimClosed
    $dir = Join-Path (PluginRoot) "PICS0UL-TheEyeofOden"
    if (-not (Test-Path $dir)) { Fail "not installed; run .\build.ps1 -Install first" }

    $live = Join-Path $dir "TheEyeofOden.dll"
    $off = "$live.old"

    if ($loaded) {
        if (Test-Path $live) { Ok "already enabled"; return }
        if (-not (Test-Path $off)) { Fail "nothing to enable in $dir" }
        Move-Item $off $live -Force
        Ok "enabled - BepInEx will load it on next launch"
        return
    }

    if (-not (Test-Path $live)) {
        if (Test-Path $off) { Ok "already disabled"; return }
        Fail "nothing to disable in $dir"
    }

    Move-Item $live $off -Force
    Ok "disabled - the DLL is kept as $(Split-Path $off -Leaf)"
}

if ($Disable -and $Enable) { Fail "pick one of -Disable or -Enable" }

if ($Disable) {
    Write-Host "`ndisabling..." -ForegroundColor Cyan
    SetLoaded $false
    Write-Host "`ndone.`n" -ForegroundColor Cyan
    exit 0
}

if ($Enable) {
    Write-Host "`nenabling..." -ForegroundColor Cyan
    SetLoaded $true
    Write-Host "`ndone.`n" -ForegroundColor Cyan
    exit 0
}

$csproj = [xml](Get-Content "$root\TheEyeofOden.csproj")
$version = $csproj.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
Write-Host "`nThe Eye of Oden $version" -ForegroundColor Cyan

Write-Host "`nbuilding..." -ForegroundColor Cyan
$log = & dotnet build "$root\TheEyeofOden.csproj" -c Release -v minimal --nologo 2>&1
if ($LASTEXITCODE -ne 0) { $log; Fail "build failed" }
Ok "compiled"

$dll = "$root\bin\Release\TheEyeofOden.dll"
if (-not (Test-Path $dll)) { Fail "expected output missing: $dll" }

if (-not $Install) { Write-Host "`ndone.`n" -ForegroundColor Cyan; exit 0 }

Write-Host "`ninstalling to profile '$Profile'..." -ForegroundColor Cyan

RequireValheimClosed
$pluginRoot = PluginRoot

$target = Join-Path $pluginRoot "PICS0UL-TheEyeofOden"
if (-not (Test-Path $target)) { New-Item -ItemType Directory -Path $target | Out-Null }
Copy-Item $dll $target -Force

# A stale .old beside a fresh DLL would leave -Enable and -Disable disagreeing
# about which file is the real one; installing always means enabled.
Remove-Item (Join-Path $target "TheEyeofOden.dll.old") -Force -ErrorAction SilentlyContinue
Ok "installed to $(Split-Path $target -Leaf)"

Write-Host "`ndone.`n" -ForegroundColor Cyan
