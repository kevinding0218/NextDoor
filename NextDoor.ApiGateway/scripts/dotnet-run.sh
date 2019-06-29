#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd ./src
cd ./NextDoor.ApiGateway
dotnet run --no-restore