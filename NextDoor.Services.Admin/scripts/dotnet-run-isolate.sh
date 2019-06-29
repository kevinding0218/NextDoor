#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd ../src/NextDoor.Services.Admin
dotnet run --no-restore