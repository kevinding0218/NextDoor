#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd src/NextDoor.ApiGateway
dotnet run --no-restore