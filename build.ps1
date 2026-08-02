Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

src\build.ps1 $Version
src\test.ps1
doc\build.ps1
.\0install.ps1 run --batch https://apps.0install.net/0install/0template.xml typedrest-codegen.xml.template version=$Version

popd
