#!/bin/bash

SCRIPT_DIR="$(cd $(dirname "$0"); pwd)"
dotnet build "${SCRIPT_DIR}/../src/LibVLCSharp/LibVLCSharp.csproj" /p:UNITY=true -c Release
