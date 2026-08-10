$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

$zeroInstall = "$PSScriptRoot\..\0install.ps1"

function Run-DotNet {
    & $zeroInstall run --batch --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

function Run-Npm {
    if (Get-Command npm -ErrorAction SilentlyContinue) {
        npm @args
    } else {
        & $zeroInstall run --batch --command=npm https://apps.0install.net/javascript/node.xml @args
    }
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

# Unit tests
Run-DotNet test --no-build --logger trx --configuration Release UnitTests\UnitTests.csproj

$cli = "..\artifacts\Release\net10.0\TypedRest.CodeGeneration.Cli.dll"

# TypeScript smoke test
if (Test-Path SmokeTest.TypeScript\generated) {Remove-Item SmokeTest.TypeScript\generated -Recurse -Force}
Run-DotNet $cli generate -l typescript -f UnitTests\sample-v3.yml -o SmokeTest.TypeScript\generated\sample -s Sample --generate-dtos
Run-DotNet $cli generate -l typescript -f UnitTests\sample-nested.yml -o SmokeTest.TypeScript\generated\nested-sample -s NestedSample --generate-dtos
pushd SmokeTest.TypeScript
Run-Npm ci
Run-Npm run check
popd

popd
