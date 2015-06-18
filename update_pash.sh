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

libdir="Libraries/Pash"

mkdir -p $libdir

pashdlls="$pashsrc/Source/PashConsole/bin/Release/*.dll"
# Copy all dlls
cp $pashdlls $libdir/

echo "Updated Pash"

pstesting="Extras/PSTesting"
if  [ ! -d "$pstesting" ]; then
	echo "PSTesing not found to update Pash. Did you (re-)initialize and update your git submodules?"
	exit 1
fi

pstestinglib="$pstesting/$libdir"
mkdir -p $pstestinglib

cp $pashdlls $pstestinglib/

