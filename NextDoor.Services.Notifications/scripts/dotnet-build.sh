#!/bin/bash
MYGET_ENV=""
case "$TRAVIS_BRANCH" in
  "develop")
    MYGET_ENV="-dev"
    ;;
esac

cd ../src/NextDoor.Services.Notifications
dotnet build -c Release --no-cache