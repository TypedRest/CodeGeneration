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

# Find gradle
if command -v gradle > /dev/null 2> /dev/null; then
    gradle="gradle"
else
    gradle="$zeroinstall run https://apps.0install.net/java/gradle.xml"
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

# JVM smoke test
rm -rf SmokeTest.Jvm/generated
$dotnet "$cli" generate -l kotlin -f UnitTests/sample-v3.yml -o SmokeTest.Jvm/generated/kotlin -s Sample -n net.typedrest.smoketest.kotlin --generate-dtos
$dotnet "$cli" generate -l java -f UnitTests/sample-v3.yml -o SmokeTest.Jvm/generated/java -s Sample -n net.typedrest.smoketest.java --generate-dtos
$dotnet "$cli" generate -l kotlin -f UnitTests/sample-nested.yml -o SmokeTest.Jvm/generated/kotlin -s NestedSample -n net.typedrest.smoketest.nested.kotlin --generate-dtos
$dotnet "$cli" generate -l java -f UnitTests/sample-nested.yml -o SmokeTest.Jvm/generated/java -s NestedSample -n net.typedrest.smoketest.nested.java --generate-dtos
(
    cd SmokeTest.Jvm
    $gradle --quiet --no-daemon compileKotlin compileJava
)
