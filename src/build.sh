#!/usr/bin/env bash
set -e
cd `dirname $0`

# Find dotnet
if command -v dotnet > /dev/null 2> /dev/null; then
    dotnet="dotnet"
else
    dotnet="../0install.sh run --version 11.0..!11.1 https://apps.0install.net/dotnet/sdk.xml"
fi

echo "Build binaries"
$dotnet msbuild -v:Quiet -restore -t:Build -p:Configuration=Release -p:Version=${1:-1.0.0-pre} ${CI+-p:ContinuousIntegrationBuild=True}

echo "Build single-file executables"
mkdir -p ../artifacts/Executables
for rid in win-x64 win-arm64 linux-x64 linux-arm64 osx-arm64; do
    case $rid in win-*) ext=".exe" ;; *) ext="" ;; esac
    $dotnet publish TypedRest.CodeGeneration.Cli --verbosity quiet --configuration Release --runtime $rid --output ../artifacts/Release/publish/$rid -p:Version=${1:-1.0.0-pre} ${CI+-p:ContinuousIntegrationBuild=True}
    cp ../artifacts/Release/publish/$rid/typedrest-codegen$ext ../artifacts/Executables/typedrest-codegen-$rid$ext
done
