# Function to publish the Dotnet project
function Publish-DotnetProject {
    param (
        [string]$projectPath
    )

    # Change to the project directory
    Set-Location -Path $projectPath

    # Check if bin\Release folder exists
    $releaseFolder = Join-Path -Path $projectPath -ChildPath 'bin\Release'
    
    if (Test-Path -Path $releaseFolder -PathType Container) {
      
            Remove-Item -Path $releaseFolder -Recurse -Force -ErrorAction SilentlyContinue  

    } else {
        Write-Host "The 'bin\Release\' folder does not exist in $($projectPath). Continuing without removing it."
    }

    # Publish the project
    $null = dotnet publish -c Release

    # Change to the bin\Release\ folder
    Set-Location -Path $releaseFolder

    # Find the .nupkg file
    $nupkgFile = Get-ChildItem -Filter *.nupkg

    # Upload the .nupkg file to NuGet
    if ($nupkgFile -ne $null) {
        $nugetSource = 'https://api.nuget.org/v3/index.json'
        $nugetApiKey = 'oy2aij2alwq4weh2jt4qvudaxz3xdnau3iptflwni4bo5i'

        # Use NuGet API or command line to upload the package
        $nugetPushCommand = "nuget push `"$($nupkgFile.FullName)`" -Source $nugetSource -ApiKey $nugetApiKey"

        
         try {
             $null = Invoke-Expression -Command $nugetPushCommand
    
#              # Check if the response contains a non-successful status code
#              if ($null -match "already exists and cannot be modified" ) {
#                  Write-Host "Error: A package with the same ID and version already exists on NuGet." 
#              } else {
                 Write-Host "Successfully uploaded $($nupkgFile.Name) to NuGet." -ForegroundColor Green
#              }
         } catch {
             Write-Host "Error uploading $($nupkgFile.Name) to NuGet. $_" -ForegroundColor Red
         }
     
         
    } else {
        Write-Host "No .nupkg file found in $($releaseFolder)." 
    }
  
}

dotnet test 

# Change to the 'Src' folder
Set-Location -Path .\Frameworks\src

# Get all subdirectories in 'Src'
$subdirectories = Get-ChildItem -Directory

# Iterate through each subdirectory and perform actions
foreach ($subdir in $subdirectories) {
    $subdirPath = $subdir.FullName
    
    # Change to the current subdirectory
    Set-Location -Path $subdirPath
    
    # Check if the current directory contains a Dotnet project (e.g., *.csproj)
    $dotnetProject = Get-ChildItem -Filter *.csproj

    if ($dotnetProject -ne $null) {
#         $userResponse = Read-Host "Do you want to perform actions in $($subdir.Name)? (Y/N)"
        
#         if ($userResponse -eq 'Y' -or $userResponse -eq 'y') {
            # Perform actions for Dotnet project in the current subdirectory
            Publish-DotnetProject -projectPath $subdirPath
#         }
    } else {
        Write-Host "No Dotnet project found in $($subdir.Name)."
    }
}

Set-Location -Path  ../../../../..
