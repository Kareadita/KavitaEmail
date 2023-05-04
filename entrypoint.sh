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

#Checks if the templates folder exists, and creates it if it does not
if [ ! -d "/app/config/templates" ]; then
    echo "Templates folder does not exist, copying from temp..."
    cp -r /tmp/templates /app/config/templates
    if [ -d "/app/config/templates" ]; then
        echo "Copy completed successfully, starting app..."
    else
        echo "Copy failed, check folder permissions. Exiting..."
        exit
    fi
fi

chmod +x KavitaEmail

./KavitaEmail