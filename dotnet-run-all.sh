#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
DOTNET_RUN=./scripts/dotnet-run.sh
PREFIX=NextDoor
SERVICE=$PREFIX.Services
REPOSITORIES=($PREFIX.ApiGateway $SERVICE.Identity)

for REPOSITORY in ${REPOSITORIES[*]}
do
	 echo ========================================================
	 echo Starting a service: $REPOSITORY
	 echo ========================================================
	 echo cd $REPOSITORY
     cd $REPOSITORY
	 echo bash $DOTNET_RUN &
     bash $DOTNET_RUN &
     cd ..
done