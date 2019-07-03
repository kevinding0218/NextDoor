## Port and Services
### List of Micro services (continuing) and port number
| Micro Service | Port |
|--|--|
| API Gateway | 5200 |
| Identity | 5201 |
| Customer | 5202 |
| Admin | 5203 |
| Notification | 5209 |
| SignalR | 5210 |
| Operation(unused) | 5207 |
## Create a web api project and solution
### 1. Create web api project under _src_ subfolder
```
mkdir NextDoor.Services.Customers
cd .\NextDoor.Services.Customers\
mkdir src
dotnet new webapi -n NextDoor.Services.Customers -o src\NextDoor.Services.Customers
```
### 2. Create new solution file and add above project inside solution
```
dotnet new sln --name NextDoor.Services.Customers
dotnet sln add src/NextDoor.Services.Customers/NextDoor.Services.Customers.csproj
```
### 3. repeat above for additional micro services
	- NextDoor.Services.Customers
	- NextDoor.Services.Identity
	- NextDoor.ApiGateway (Done)
	- NextDoor.Services.Notifications (Done)
	- NextDoor.Services.Accounts (Later)
	- NextDoor.Services.Billbooks (Later)
	- NextDoor.Services.Transactions (Later)

### Re-organize a solution by adding all projects inside using Powershell script
- Listing the projects in a solution file
```
dotnet sln list
```
## Create one Solution to include all projects
- Run in Powershell Individually
```
dotnet new sln -n NextDoor
dotnet sln NextDoor.sln add H:\MyGithub\NextDoor\NextDoor.Core\src\NextDoor.Core\NextDoor.Core.csproj
```
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
## Docker
### Run development docker tool
- Go into `NextDoor\NextDoor\Docker Compose`
```
docker-compose -f .\mongo-rabbit-redis.yml up -d
```
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
## Create a bash script to run all instances
- Turn Windows features On or Off
	- Windows sub system for Linux
- Go to root directory and run `.\dotnet-run-all.sh`
-------------------------------------------------------
## [Use Docker for Sql Server 2017](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-2017&pivots=cs1-bash)

### Using powershell
- Pull the SQL Server 2017 Linux container image from Microsoft Container Registry.
	```
	docker pull mcr.microsoft.com/mssql/server:2017-latest
	```
	- If you want to pull a specific image, you add a colon and the tag name, for example, `mcr.microsoft.com/mssql/server:2017-GA-ubuntu`
- Run the container image with Docker
	```
	docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=<YourStrong!Passw0rd>' -p 1433:1433 --name sql1 -d mcr.microsoft.com/mssql/server:2017-latest
	```
	- **-e 'ACCEPT_EULA=Y'**: (Required) Set the ACCEPT_EULA variable to any value to confirm your acceptance of the End-User Licensing Agreement.
	- **-e 'SA_PASSWORD=<YourStrong!Passw0rd>'**: (Required) Specify your own strong password that is at least 8 characters and meets the SQL Server password requirements.
	- **-p 1433:1433**: Map a TCP port on the host environment (first value) with a TCP port in the container (second value). In this example, SQL Server is listening on TCP 1433 in the container and this is exposed to the port, 1433, on the host.
	- **--name sql1**: Specify a custom name for the container rather than a randomly generated one. If you run more than one container, you cannot reuse this same name.
	- **mcr.microsoft.com/mssql/server:2017-latest**: The SQL Server 2017 Linux container image.
- To view your Docker containers
	```
	docker ps -a
	```
- Status
	- If the STATUS column shows a status of Up, then SQL Server is running in the container and listening on the port specified in the PORTS column.
	- If the STATUS column for your SQL Server container shows Exited, need to troubleshoot
- Troubleshoot
	- make sure Docker service is running
	- error: failed to create endpoint CONTAINER_NAME on network bridge. Error starting proxy: listen tcp 0.0.0.0:1433 bind: address already in use
		- This can happen if you're running SQL Server locally on the host machine.
		- It can also happen if you start two SQL Server containers and try to map them both to the same host port.
		- If this happens, use the -p parameter to map the container port 1433 to a different host port.
		- For example:
			```
			docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong!Passw0rd>" -p 1400:1433 -d mcr.microsoft.com/mssql/server:2017-latest
			```
	- check container log	
		```
		docker logs YOUR_CONTAINER_ID
		```
- Connect to local Azure Data Studio or SSMS using `192.168.1.79,1433` and SA/Password
- Check local ip address through `ipconfig` --> Wireless LAN IPv4 Address
### Using docker-compose
- Create a docker-compose.yml file
	- This file defines the `web` and `db` micro-services, their relationship, the ports they are using, and their specific environment variables.
		```docker
		version: "3"
		services:
		    web:
		        build: .
		        ports:
		            - "8000:80"
		        depends_on:
		            - db
		    db:
		        image: "mcr.microsoft.com/mssql/server:2017-latest"
		        ports:
			        - "1433:1433"
		        environment:
		            SA_PASSWORD: "Your_password123"
		            ACCEPT_EULA: "Y"
		        container_name: db1
		```
- Use correct [version](https://docs.docker.com/compose/compose-file/)
- Check validity using `docker-compose config`
- Run docker-compose.yml file using `docker-compose up -d`
	- **-d**: start in detach mode
- Stop the container using `docker-compose down`
- Scale services: 
	- Scale four database services using`docker-compose up -d --scale database=4`