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
-------------------------------------------------------
## [dotnet run error](http://www.waynethompson.com.au/blog/dotnet-dev-certs-https/)
### Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found.
- Steps to fix the problem
	1. Close your browsers so that they do not cache the certificate because that will cause other issues. 
	2. On the commandline run
	```
	dotnet dev-certs https --clean
	```
	3. run
	```
	dotnet dev-certs https -t
	```
	4. You can then check the certificate with dotnet dev-certs https --check but this returned nothing which I assume means everything is ok.
	5. then ran dotnet run the project ran as expected.
-------------------------------------------------------
## [Git Command](https://confluence.atlassian.com/bitbucketserver/basic-git-commands-776639767.html)
### Show all remote and local branches
```
git branch -a
```
### Figure out what branches are on your remote by actually using the remote related commands
```
git remote show origin
```
### Get all branches on remote which are not in local
```
git fetch
git fetch origin
```
### Switch from one branch to another/Checkout a branch
```
git checkout --track <branchname>
```
### Create a new branch and switch to it:
```
git checkout -b <branchname>
```
### Rename a branch
- Rename your local branch.

- If you are on the branch you want to rename:
```
git branch -m new-name
```
- If you are on a different branch:
```
git branch -m old-name new-name
```
- Delete the old-name remote branch and push the new-name local branch.
```
git push origin :old-name new-name
```
- Reset the upstream branch for the new-name local branch.

* Switch to the branch and then:
```
git push origin -u new-name
```
-------------------------------------------------------