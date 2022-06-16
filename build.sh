#! /bin/bash
set -e

outputFolder='_output'

ProgressStart()
{
    echo "Start '$1'"
}

ProgressEnd()
{
    echo "Finish '$1'"
}

Build()
{
	local RID="$1"

    ProgressStart "Build for $RID"

    slnFile=KavitaEmail.sln

    dotnet clean $slnFile -c Release

	dotnet msbuild -restore $slnFile -p:Configuration=Release -p:Platform="Any CPU" -p:RuntimeIdentifiers=$RID

    ProgressEnd "Build for $RID"
}

Package()
{
    local framework="$1"
    local runtime="$2"
    local lOutputFolder=../_output/"$runtime"/KavitaEmail

    ProgressStart "Creating $runtime Package for $framework"

    # TODO: Use no-restore? Because Build should have already done it for us
    echo "Building"
    cd KavitaEmail
    echo dotnet publish -c Release --no-restore --self-contained --runtime $runtime -o "$lOutputFolder" --framework $framework
    dotnet publish -c Release --no-restore --self-contained --runtime $runtime -o "$lOutputFolder" --framework $framework

    echo "Copying LICENSE"
    cp ../LICENSE "$lOutputFolder"/LICENSE.txt

    echo "Show KavitaEmail structure"
    find

	  echo "Copying appsettings.json"
    cp ./config/appsettings.json $lOutputFolder/config/appsettings.json

    echo "Creating tar"
    cd ../$outputFolder/"$runtime"/
    tar -czvf ../kavitaemail-$runtime.tar.gz KavitaEmail

    ProgressEnd "Creating $runtime Package for $framework"

}

dir=$PWD

if [ -d _output ]
then
	rm -r _output/
fi

#Build for x64
Build "linux-x64"
Package "net6.0" "linux-x64"
cd "$dir"

#Build for arm
Build "linux-arm"
Package "net6.0" "linux-arm"
cd "$dir"

#Build for arm64
Build "linux-arm64"
Package "net6.0" "linux-arm64"
cd "$dir"

Build "win-x64"
Package "net6.0" "win-x64"
cd "$dir"

Build "win-x86"
Package "net6.0" "win-x86"
cd "$dir"

Build "osx-x64"
Package "net6.0" "osx-x64"
cd "$dir"

Build "linux-musl-x64"
Package "net6.0" "linux-musl-x64"
cd "$dir"
