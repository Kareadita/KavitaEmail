#! /bin/bash

#Checks if the config file exists, and creates it if it does not
if [ ! -f "/app/config/appsettings.json" ]; then
    echo "Kavita configuration file does not exist, copying from temp..."
    cp /tmp/config/appsettings.json /app/config/appsettings.json
    if [ -f "/app/config/appsettings.json" ]; then
        echo "Copy completed successfully, starting app..."
    else
        echo "Copy failed, check folder permissions. Exiting..."
        exit
    fi
fi

#Checks if the templates folder exists, and creates it if it does not
#Also will sync new files if they are added
DIFF=$(diff /tmp/config/templates /app/config/templates)
if [ ! -d "/app/config/templates" ]; then
    echo "Templates folder does not exist, copying from temp..."
    cp -r /tmp/config/templates /app/config/templates
    if [ -d "/app/config/templates" ]; then
        echo "Copy completed successfully, starting app..."
    else
        echo "Copy failed, check folder permissions. Exiting..."
        exit
    fi
elif [ "$DIFF" != "" ]; then
    echo "Template folder out of sync, copying new files..."
    rsync -azP /tmp/config/templates/ /app/config/templates
    #Doing a second check to make sure it copied properly
    DIFF=$(diff /tmp/config/templates /app/config/templates)
    if [ "$DIFF" == "" ]; then
        echo "Files synced"
    else
        echo "File sync failed, check folder permissions"
        exit
    fi
fi

chmod +x KavitaEmail

./KavitaEmail
