#!/bin/sh
set -e
cd `dirname $0`

zeroinstall="`cd .. && pwd`/0install.sh"

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="$zeroinstall run --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml"
fi

# Find npm
if command -v npm > /dev/null 2> /dev/null; then
    npm="npm"
else
    npm="$zeroinstall run --command=npm https://apps.0install.net/javascript/node.xml"
fi

# Unit tests
$dotnet test --no-build --logger trx --configuration Release UnitTests/UnitTests.csproj

cli="../artifacts/Release/net10.0/TypedRest.CodeGeneration.Cli.dll"

# TypeScript smoke test
rm -rf SmokeTest.TypeScript/generated
$dotnet "$cli" generate -l typescript -f UnitTests/sample-v3.yml -o SmokeTest.TypeScript/generated/sample -s Sample --generate-dtos
$dotnet "$cli" generate -l typescript -f UnitTests/sample-nested.yml -o SmokeTest.TypeScript/generated/nested-sample -s NestedSample --generate-dtos
(
    cd SmokeTest.TypeScript
    $npm ci
    $npm run check
)
