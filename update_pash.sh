#!/bin/bash
if [ -z "$1" ]; then
	echo "Provide a Path to the Pash sources"
	exit 1
fi

pashsrc="$1"

# Build Pash in release mode
pushd $pashsrc
xbuild /p:Configuration=Release
popd

# Copy all dlls
cp $pashsrc/Source/PashConsole/bin/Release/*.dll .

echo "Updated Pash"

