#!/bin/sh
set -e
cd `dirname $0`

src/build.sh ${1:-1.0-dev}
src/test.sh
doc/build.sh
./0install.sh run https://apps.0install.net/0install/0template.xml typedrest-codegen.xml.template version=${1:-1.0-dev}
