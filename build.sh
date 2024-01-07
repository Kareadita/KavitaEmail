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
    local runtime="$1"
    local lOutputFolder=../_output/"$runtime"/KavitaEmail

    ProgressStart "Creating $runtime Package"

    # TODO: Use no-restore? Because Build should have already done it for us
    echo "Building"
    cd KavitaEmail
    echo dotnet publish -c Release --no-restore --self-contained --runtime $runtime -o "$lOutputFolder"
    dotnet publish -c Release --no-restore --self-contained --runtime $runtime -o "$lOutputFolder"

    echo "Copying LICENSE"
    cp ../LICENSE "$lOutputFolder"/LICENSE.txt

    echo "Show KavitaEmail structure"
    find

	  echo "Copying appsettings.json"
    cp ./config/appsettings.json $lOutputFolder/config/appsettings.json
    
    echo "Removing appsettings.Development.json if exists"
    if [ -e "$lOutputFolder/config/appsettings.Development.json" ]; then
        rm "$lOutputFolder/config/appsettings.Development.json"
        echo "File removed successfully"
    fi
    
    echo "Copying templates"
    cp -a ./config/templates/ $lOutputFolder/config/templates/

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
Package "linux-x64"
cd "$dir"

#Build for arm
Build "linux-arm"
Package "linux-arm"
cd "$dir"

#Build for arm64
Build "linux-arm64"
Package "linux-arm64"
cd "$dir"

Build "win-x64"
Package "win-x64"
cd "$dir"

Build "win-x86"
Package "win-x86"
cd "$dir"

Build "osx-x64"
Package "osx-x64"
cd "$dir"

Build "linux-musl-x64"
Package "linux-musl-x64"
cd "$dir"
