#! /bin/bash

#Checks if the config file exists, and creates it if it does not
if [ ! -f "/app/config/appsettings.json" ]; then
    echo "Kavita configuration file does not exist, copying from temp..."
    cp /tmp/appsettings.json /app/config/appsettings.json
    if [ -f "/app/config/appsettings.json" ]; then
        echo "Copy completed successfully, starting app..."
    else
        echo "Copy failed, check folder permissions. Exiting..."
        exit
    fi
fi

chmod +x KavitaEmail

./KavitaEmail