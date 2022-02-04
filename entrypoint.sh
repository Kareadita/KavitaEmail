#! /bin/bash

if [ -d /app/config ]; then
  if [ -f /app/config/appsettings.json ] && [ -d /app/config/templates ]; then
    #Directory is not empty, do nothing
    echo "Config directory exists, skipping copy..."
    #Makes sure the templates folder is up to date without wiping out appsettings
    rsync -azP /tmpconfig/templates/ /app/config/templates/
  else
    #Directory is empty, copy files in from the temp config
    rsync -azP /tmpconfig/ /app/config/
    mv /app/config/appsettings.Development.json /app/config/appsettings.json
    rm -r /tmpconfig
  fi
fi

chmod +x KavitaEmail

./KavitaEmail