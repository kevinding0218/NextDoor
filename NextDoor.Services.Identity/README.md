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