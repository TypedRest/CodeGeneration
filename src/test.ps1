$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

$zeroInstall = "$PSScriptRoot\..\0install.ps1"

function Run-DotNet {
    & $zeroInstall run --batch --version 10.0..!10.1 https://apps.0install.net/dotnet/sdk.xml @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

function Run-Gradle {
    if (Get-Command gradle -ErrorAction SilentlyContinue) {
        gradle @args
    } else {
        & $zeroInstall run --batch https://apps.0install.net/java/gradle.xml @args
    }
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
Run-DotNet test --no-build --configuration Release

$cli = "..\artifacts\Release\net10.0\TypedRest.CodeGeneration.Cli.dll"

# JVM smoke test
if (Test-Path SmokeTest.Jvm\generated) {Remove-Item SmokeTest.Jvm\generated -Recurse -Force}
Run-DotNet $cli generate -l java -f UnitTests\sample-v3.yml -o SmokeTest.Jvm\generated\java -s Sample -n net.typedrest.smoketest.java --generate-dtos
Run-DotNet $cli generate -l java -f UnitTests\sample-v3.yml -o SmokeTest.Jvm\generated\java -s Sample -n net.typedrest.smoketest.interfaces.java --generate-dtos --generate-interfaces
Run-DotNet $cli generate -l java -f UnitTests\sample-nested.yml -o SmokeTest.Jvm\generated\java -s NestedSample -n net.typedrest.smoketest.nested.java --generate-dtos
Run-DotNet $cli generate -l java -f UnitTests\sample-nested.yml -o SmokeTest.Jvm\generated\java -s NestedSample -n net.typedrest.smoketest.interfaces.nested.java --generate-dtos --generate-interfaces
Run-DotNet $cli generate -l kotlin -f UnitTests\sample-v3.yml -o SmokeTest.Jvm\generated\kotlin -s Sample -n net.typedrest.smoketest.kotlin --generate-dtos
Run-DotNet $cli generate -l kotlin -f UnitTests\sample-v3.yml -o SmokeTest.Jvm\generated\kotlin -s Sample -n net.typedrest.smoketest.interfaces.kotlin --generate-dtos --generate-interfaces
Run-DotNet $cli generate -l kotlin -f UnitTests\sample-nested.yml -o SmokeTest.Jvm\generated\kotlin -s NestedSample -n net.typedrest.smoketest.nested.kotlin --generate-dtos
Run-DotNet $cli generate -l kotlin -f UnitTests\sample-nested.yml -o SmokeTest.Jvm\generated\kotlin -s NestedSample -n net.typedrest.smoketest.interfaces.nested.kotlin --generate-dtos --generate-interfaces
pushd SmokeTest.Jvm
Run-Gradle --quiet --no-daemon compileKotlin compileJava
popd

# TypeScript smoke test
if (Test-Path SmokeTest.TypeScript\generated) {Remove-Item SmokeTest.TypeScript\generated -Recurse -Force}
Run-DotNet $cli generate -l typescript -f UnitTests\sample-v3.yml -o SmokeTest.TypeScript\generated\sample -s Sample --generate-dtos
Run-DotNet $cli generate -l typescript -f UnitTests\sample-nested.yml -o SmokeTest.TypeScript\generated\nested-sample -s NestedSample --generate-dtos
pushd SmokeTest.TypeScript
Run-Npm ci
Run-Npm run check
popd

popd
