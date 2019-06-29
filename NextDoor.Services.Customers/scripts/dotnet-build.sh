#!/bin/bash
MYGET_ENV=""
case "$TRAVIS_BRANCH" in
  "develop")
    MYGET_ENV="-dev"
    ;;
esac

cd ../src/NextDoor.Services.Customers
dotnet build -c Release --no-cache