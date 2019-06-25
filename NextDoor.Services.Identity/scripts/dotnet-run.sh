#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd src/NextDoor.Services.Identity
dotnet run --no-restore