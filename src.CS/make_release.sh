#! /bin/sh
#
# Create a zip(1) file from the current Release binaries,
# plus installation support.

BINSRC="SWA.Ariadne.App/bin/Release"
VERSION=`grep AssemblyVersion ./SharedAssemblyInfo.cs | sed -e 's/.*("//' -e 's/\\.[0-9]*").*//'`
RELEASE="Ariadne_$VERSION"

# Create the new release's directory.
mkdir -p ./Releases
cd       ./Releases
rm -rf   $RELEASE $RELEASE.zip
mkdir -p $RELEASE/Install

# Copy all required files into the release's directory.
rm -f    ../$BINSRC/SWA.*.config
cp -alLt $RELEASE          ../$BINSRC/.  ../INSTALL.txt
cp -alLt $RELEASE/Install  ../Install/*.tmpl  ../Install/*.sh

# Create the new zipfile.
zip -r -o $RELEASE.zip $RELEASE

echo "Done."
cd ..
ls -l "./Releases/$RELEASE.zip"
