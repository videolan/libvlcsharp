#!/bin/bash
dotnet build ../LibVLCSharp/LibVLCSharp.csproj /p:UNITY_ANDROID=true -f netstandard2.0 -c Release
