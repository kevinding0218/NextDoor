#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd ../src/NextDoor.Services.Notifications
dotnet run --no-restore