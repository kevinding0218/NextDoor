#!/bin/bash
export ASPNETCORE_ENVIRONMENT=local
cd src/NextDoor.Services.Customers
dotnet run --no-restore