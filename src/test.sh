#!/bin/sh
set -e
cd `dirname $0`

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="../0install.sh run --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml"
fi

# Unit tests
$dotnet test --no-build --logger trx --configuration Release UnitTests/UnitTests.csproj

# TypeScript smoke test
if command -v npm > /dev/null 2> /dev/null; then
    cli="../artifacts/Release/net10.0/TypedRest.CodeGeneration.Cli.dll"

    # Generate clients
    rm -rf SmokeTest.TypeScript/generated
    $dotnet "$cli" generate -l typescript -f UnitTests/sample-v3.yml -o SmokeTest.TypeScript/generated/sample -s Sample --generate-dtos
    $dotnet "$cli" generate -l typescript -f UnitTests/sample-nested.yml -o SmokeTest.TypeScript/generated/nested-sample -s NestedSample --generate-dtos

    cd SmokeTest.TypeScript
    npm ci
    npm run check
else
    echo "Skipping TypeScript smoke test: npm not found"
fi
