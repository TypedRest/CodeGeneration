Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

function Run-DotNet {
    ..\0install.ps1 run --batch --version 11.0..!11.1 https://apps.0install.net/dotnet/sdk.xml @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

echo "Build binaries"
if ($env:CI) { $ci = "/p:ContinuousIntegrationBuild=True" }
Run-DotNet msbuild /v:Quiet /Restore /t:Build /p:Configuration=Release /p:Version=$Version $ci

echo "Build single-file executables"
foreach ($rid in "win-x64", "win-arm64", "linux-x64", "linux-arm64", "osx-arm64") {
    $ext = if ($rid.StartsWith("win-")) { ".exe" } else { "" }
    Run-DotNet publish TypedRest.CodeGeneration.Cli --verbosity quiet --configuration Release --runtime $rid --output ..\artifacts\Release\publish\$rid /p:Version=$Version $ci
    New-Item -ItemType Directory -Force ..\artifacts\Executables | Out-Null
    Copy-Item ..\artifacts\Release\publish\$rid\typedrest-codegen$ext ..\artifacts\Executables\typedrest-codegen-$rid$ext
}

popd
