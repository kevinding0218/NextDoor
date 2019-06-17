# Creae one Solution to include all projects
- Create a shell script (one_solution.sh)
```
#!/bin/bash
PROJECT_PREFIX=NextDoor
PROJECT_PATH_PREFIX=H:\\MyGithub
SOLUTION_NAME=NextDoor_Test

if [ -f $SOLUTION_NAME.sln ]
then 
	echo ========================================================
	echo Deleting Solution: $SOLUTION_NAME.sln
	echo ========================================================
    rm $SOLUTION_NAME.sln
fi

echo ========================================================
echo Recreating Solution: $SOLUTION_NAME.sln
echo ========================================================
dotnet new sln -n $SOLUTION_NAME

#e.g $CSPROJ=NextDoor.Core
CORE_NAME=$PROJECT_PREFIX.Core
CORE_CSPROJ_FULL_PATH=$PROJECT_PATH_PREFIX\\$PROJECT_PREFIX\\$CORE_NAME\\src\\$CORE_NAME\\$CORE_NAME.csproj

echo ========================================================
echo Running: dotnet sln $SOLUTION_NAME.sln add $CORE_CSPROJ_FULL_PATH
echo ========================================================
dotnet sln $SOLUTION_NAME.sln add $CORE_CSPROJ_FULL_PATH
	 
SERVICE_PREFIX=$PROJECT_PREFIX.Services
REPOSITORIES=(Customers Identity)

for REPOSITORY in ${REPOSITORIES[*]}
do
	 SERVICE_NAME=$SERVICE_PREFIX.$REPOSITORY
	 SERVICE_CSPROJ_FULL_PATH=$PROJECT_PATH_PREFIX\\$PROJECT_PREFIX\\$SERVICE_NAME\\src\\$SERVICE_NAME\\$SERVICE_NAME.csproj
	 echo ========================================================
	 echo Running: dotnet sln $SOLUTION_NAME.sln add $SERVICE_CSPROJ_FULL_PATH
	 echo ========================================================
	 dotnet sln $SOLUTION_NAME.sln add $SERVICE_CSPROJ_FULL_PATH
done

$SHELL
```
- Run in Powershell Individually
```
dotnet new sln -n NextDoor
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Core\src\NextDoor.Core\NextDoor.Core.csproj
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Services.Customers\src\NextDoor.Services.Customers\NextDoor.Services.Customers.csproj
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Services.Identity\src\NextDoor.Services.Identity\NextDoor.Services.Identity.csproj
```
- Create a powershell script
```
$projects = Get-ChildItem -Recurse | Where-Object { $_.Name -match '^.+\.(csproj|vbproj)$' }
$uniqueProjects = $projects | Group-Object -Property Name | Where Count -EQ 1 | select -ExpandProperty Group | % { $_.FullName }
Invoke-Expression -Command "dotnet new sln -n NextDoor"
$uniqueProjects | % { Invoke-Expression -Command "dotnet sln NextDoor.sln add ""$_""" }
```
