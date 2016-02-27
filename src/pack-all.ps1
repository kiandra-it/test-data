param (
    [string]$version,
    [string]$apiKey,
    [switch]$push = $false
)

if (!(Test-Path ".\pack"))
{
  mkdir pack
}

$nuget = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath(".\pack\nuget.exe")

if (!(Test-Path "$nuget")){
  $webclient = New-Object System.Net.WebClient
  $webclient.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", $nuget)
}
& $nuget pack .\TestData.Interface\TestData.Interface.csproj -build -properties "configuration=Release;TargetFrameworkVersion=v4.5" -msbuildversion 14 -IncludeReferencedProjects  -output .\pack -version $version
& $nuget pack .\TestData.Interface.MediatR\TestData.Interface.MediatR.csproj -build -properties "configuration=Release;TargetFrameworkVersion=v4.5" -msbuildversion 14 -IncludeReferencedProjects -output .\pack -version $version
& $nuget pack .\TestData.Interface.Web\TestData.Interface.Web.csproj -build -properties "configuration=Release;TargetFrameworkVersion=v4.5" -msbuildversion 14 -IncludeReferencedProjects -output .\pack -version $version


if ($push) {
  & $nuget push ".\pack\TestData.Interface.$version.nupkg" -ApiKey $apiKey
  & $nuget push ".\pack\TestData.Interface.MediatR.$version.nupkg" -ApiKey $apiKey
  & $nuget push ".\pack\TestData.Interface.Web.$version.nupkg" -ApiKey $apiKey
}
