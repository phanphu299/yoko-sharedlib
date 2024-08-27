# cd to the directory
# current version
**NOTE**: You can use `publish-nuget.ps1` instead
**NOTE**: You should change default version on `azure-pipeline.yaml` also. 


```powershell
# e.g ./publish-nuget.ps1 Audit
$current_version = "2.1.5-rc58"
./publish-nuget.ps1 $current_version
```