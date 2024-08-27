#!/bin/bash
output_file='ls ./outputs/*.nupkg'

for file in $output_file
do
echo $file
dotnet nuget push --api-key vsts --source shared-library  $file
done

