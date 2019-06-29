#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
DOTNET_RUN=./scripts/dotnet-run.sh
PREFIX=NextDoor
SERVICE=$PREFIX.Services
REPOSITORIES=($PREFIX.ApiGateway $SERVICE.Admin $SERVICE.Customers $SERVICE.Identity $SERVICE.Notifications)

for REPOSITORY in ${REPOSITORIES[*]}
do
	 echo ========================================================
	 echo Starting a service: $REPOSITORY
	 echo ========================================================
	 echo cd $REPOSITORY
     cd $REPOSITORY
	 echo $DOTNET_RUN &
     $DOTNET_RUN &
     cd ..
done