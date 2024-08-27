$lib_version=$args[0]
$output_path = "outputs"
# $project_path="src/AHI.Infrastructure.$lib_name/AHI.Infrastructure.$lib_name.csproj"
# $nuget_path="./nugets/AHI.Infrastructure.$lib_name.*.nupkg"
# Write-Output $project_path
# Write-Output $nuget_path

# if (Test-Path -Path $nuget_path -PathType Leaf) {
#     Write-Output "Deleting... $nuget_path"
#     Remove-Item $nuget_path
# }
rm -r $output_path
dotnet clean
dotnet pack -c Release -o $output_path /p:Version=$lib_version # /p:FileVersion=1.0.0.119 /p:AssemblyVersion=1.0.0.119

Get-ChildItem $output_path | ForEach-Object {
    $fullPath = $_.FullName
    dotnet nuget push --api-key vsts --source shared-library  $fullPath
}
