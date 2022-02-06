#! /bin/bash
set -e

outputFolder='_output'

CheckRequirements()
{
    if ! command -v npm &> /dev/null
    then
        echo "Warning!!! npm not found, it is required for building Kavita!"
    fi
    if ! command -v dotnet &> /dev/null
    then
        echo "Warning!!! dotnet not found, it is required for building Kavita!"
    fi
}

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
    ProgressStart 'Build'

    rm -rf $outputFolder

    slnFile=Kavita.sln

    dotnet clean $slnFile -c Release

    if [[ -z "$RID" ]];
    then
        dotnet msbuild -restore $slnFile -p:Configuration=Release -p:Platform="Any CPU"
    else
        dotnet msbuild -restore $slnFile -p:Configuration=Release -p:Platform="Any CPU" -p:RuntimeIdentifiers=$RID
    fi

    ProgressEnd 'Build'
}

Package()
{
    local framework="$1"
    local runtime="$2"
    local lOutputFolder=../_output/"$runtime"/Kavita

    ProgressStart "Creating $runtime Package for $framework"

    # TODO: Use no-restore? Because Build should have already done it for us
    echo "Building"
    cd API
    echo dotnet publish -c Release --self-contained --runtime $runtime -o "$lOutputFolder" --framework $framework
    dotnet publish -c Release --self-contained --runtime $runtime -o "$lOutputFolder" --framework $framework

    echo "Copying LICENSE"
    cp ../LICENSE "$lOutputFolder"/LICENSE.txt

    echo "Copying appsettings.json"
    cp config/appsettings.Development.json $lOutputFolder/config/appsettings.json

    echo "Creating tar"
    cd ../$outputFolder/"$runtime"/
    tar -czvf ../kavita-$runtime.tar.gz Kavita


    ProgressEnd "Creating $runtime Package for $framework"


}

RID="$1"

CheckRequirements
Build

dir=$PWD

if [[ -z "$RID" ]];
then
    Package "net6.0" "win-x64"
    cd "$dir"
    Package "net6.0" "win-x86"
    cd "$dir"
    Package "net6.0" "linux-x64"
    cd "$dir"
    Package "net6.0" "linux-arm"
    cd "$dir"
    Package "net6.0" "linux-arm64"
    cd "$dir"
    Package "net6.0" "linux-musl-x64"
    cd "$dir"
    Package "net6.0" "osx-x64"
    cd "$dir"
else
    Package "net6.0" "$RID"
    cd "$dir"
fi
