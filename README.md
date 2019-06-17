# Creae one Solution to include all projects
- Create a powershell script
```
$projects = Get-ChildItem -Recurse | Where-Object { $_.Name -match '^.+\.(csproj|vbproj)$' }
$uniqueProjects = $projects | Group-Object -Property Name | Where Count -EQ 1 | select -ExpandProperty Group | % { $_.FullName }
Invoke-Expression -Command "dotnet new sln -n NextDoor"
$uniqueProjects | % { Invoke-Expression -Command "dotnet sln NextDoor.sln add ""$_""" }
```
- Run in Powershell Individually
```
dotnet new sln -n NextDoor
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Core\src\NextDoor.Core\NextDoor.Core.csproj
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Services.Customers\src\NextDoor.Services.Customers\NextDoor.Services.Customers.csproj
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Services.Identity\src\NextDoor.Services.Identity\NextDoor.Services.Identity.csproj
```