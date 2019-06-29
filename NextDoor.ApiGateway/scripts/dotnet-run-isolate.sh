#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd ../src/NextDoor.Services.ApiGateway
dotnet run --no-restore