#run when vs code is closed
Get-ChildItem .\ -include bin,obj,".ionide",".fake" -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
rm .\build.fsx.lock
