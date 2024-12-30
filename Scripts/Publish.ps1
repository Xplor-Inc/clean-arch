$basePath = "../src/Presentation/WebApp/"

$fileName = $basePath+"appsettings.json"
$s = Get-Content $fileName |ConvertFrom-Json

$VersionArray =$s.Version.Split(".")
$Major = [int]$VersionArray[0];
$Minor = [int]$VersionArray[1];
$Bug = [int]$VersionArray[2]+1;

if ( $Bug -ge 10 )
{
    $Bug = 0
    $Minor = $Minor+1
}
if ( $Minor -ge 10 )
{
    $Minor = 0
    $Major = $Major+1
}

$OldValue = $s.Version
$NewValue = $Major.ToString()+"."+$Minor.ToString()+"."+$Bug.ToString();

(Get-Content -Path $fileName) |
    ForEach-Object {$_ -Replace $OldValue, $NewValue} |
        Set-Content -Path $fileName

$info = "`r`nUpdated appsettings.json with "+ $OldValue +" to "+ $NewValue +"`r`n"
Write-Output $info


$outputPath = "../Publish/"+$NewValue

Write-Output ("Publishing application to " + $outputPath  +" in Release mode with X64 Config  self-contained`r`n")

dotnet publish "../ShareMarket.sln" -c Release -r win-x64 --self-contained true --output $outputPath
Write-Output ("`r`n Published successfully to "+ $outputPath +"`r`n")

