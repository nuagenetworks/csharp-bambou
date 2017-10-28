#!/bin/sh

# This script will:
#   - build the project using mono on linux. 
#   - Create a release on github
#   - push the package on nuget
#
# Variables that need to be declared before invoking this script:
#   HTTPS_PROXY (optional
#   TAG: the github tag for this build
#   GITHUBTOKEN: token for github API
#   NUGETTOKEN: token for nuget API
#

set -x
trap failscript ERR

failscript()
{
    echo "Build failed"
    exit 1
}

# Artifact to be released
FILE=net.nuagenetworks.bambou.dll

# Build
nuget restore csharp-bambou.sln
sed -i "s/^.*AssemblyFileVersion.*$/[assembly: AssemblyFileVersion(\"$TAG\")]/g" csharp-bambou/Properties/AssemblyInfo.cs
xbuild /p:Configuration="Release" csharp-bambou.sln

#Create release on github
RESPONSE=$(curl -H "Authorization: token $GITHUBTOKEN" -X POST -b -c curlcookies -d '{"tag_name": "'$TAG'","target_commitish": "master", "name": "Bambou .NET '$TAG'","body": "Bambou library for .NET"}' https://api.github.com/repos/nuagenetworks/csharp-bambou/releases)

ID=$(echo $RESPONSE | jq -r ".id")

UPLOAD_RESPONSE=$(curl -H "Authorization: token $GITHUBTOKEN" -X POST -H "Content-type: application/x-dosexec" --data-binary @csharp-bambou/bin/Release/$FILE https://uploads.github.com/repos/nuagenetworks/csharp-bambou/releases/$ID/assets?name=$FILE)

# Build nuget package
sed -i "s/VERSION_VAR/$TAG/g" package.nuspec
nuget pack package.nuspec

# Push to nuget
nuget push net.nuagenetworks.bambou.dll.1.0.1.nupkg $NUGET_TOKENTnuget push net.nuagenetworks.bambou.dll.$TAG.nupkg $NUGETTOKEN -Source https://www.nuget.org/api/v2/package -Source https://www.nuget.org/api/v2/package

