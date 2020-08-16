﻿Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

echo "Downloading references to other documentation..."
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]'Tls11,Tls12'
Invoke-WebRequest https://code-generation.nano-byte.net/nanobyte-code-generation.tag -OutFile nanobyte-code-generation.tag

if (Test-Path ..\artifacts\Documentation) {rm -Recurse -Force ..\artifacts\Documentation}
mkdir ..\artifacts\Documentation | Out-Null

$env:VERSION = $Version
..\0install.ps1 run --batch https://apps.0install.net/devel/doxygen.xml
if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}

popd
