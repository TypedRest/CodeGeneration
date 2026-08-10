$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

function Run-DotNet {
    ..\0install.ps1 run --batch --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

function Run-Npm {
    npm @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

# Unit tests
Run-DotNet test --no-build --logger trx --configuration Release UnitTests\UnitTests.csproj

# TypeScript smoke test
if (Get-Command npm -ErrorAction SilentlyContinue) {
    $cli = "..\artifacts\Release\net10.0\TypedRest.CodeGeneration.Cli.dll"

    # Generate clients
    if (Test-Path SmokeTest.TypeScript\generated) {Remove-Item SmokeTest.TypeScript\generated -Recurse -Force}
    Run-DotNet $cli generate -l typescript -f UnitTests\sample-v3.yml -o SmokeTest.TypeScript\generated\sample -s Sample --generate-dtos
    Run-DotNet $cli generate -l typescript -f SmokeTest\nested.yml -o SmokeTest.TypeScript\generated\nested -s Nested --generate-dtos

    pushd SmokeTest.TypeScript
    Run-Npm install
    Run-Npm run check
    popd
} else {
    Write-Host "Skipping TypeScript smoke test: npm not found"
}

popd
