#!/bin/bash
lib_version=$1
output_path='outputs'
rm -r $output_path
dotnet clean
dotnet pack -c Release -o $output_path /p:Version=$lib_version # /p:FileVersion=1.0.0.119 /p:AssemblyVersion=1.0.0.119

