#!/bin/bash
set -e
cd `dirname $0`

echo "Downloading references to other documentation..."
curl -sS -o nanobyte-code-generation.tag https://code-generation.nano-byte.net/nanobyte-code-generation.tag

rm -rf ../artifacts/Documentation
mkdir -p ../artifacts/Documentation

VERSION=${1:-1.0-dev} ./_0install.sh run https://apps.0install.net/devel/doxygen.xml
