#! /bin/bash

#Applies environment variables
sed -i "s/\"Host\": \"\"/\"Host\": \"$SMTP_HOST\"/" /app/config/appsettings.json
sed -i "s/\"Port\": \"\"/\"Port\": \"$SMTP_PORT\"/" /app/config/appsettings.json
sed -i "s/\"UserName\": \"\"/\"UserName\": \"$SMTP_USER\"/" /app/config/appsettings.json
sed -i "s/\"Password\": \"\"/\"Password\": \"$SMTP_PASS\"/" /app/config/appsettings.json
sed -i "s/\"SenderAddress\": \"\"/\"SenderAddress\": \"$SEND_ADDR\"/" /app/config/appsettings.json
sed -i "s/\"SenderDisplayName\": \"\"/\"SenderDisplayName\": \"$DISP_NAME\"/" /app/config/appsettings.json

chmod +x KavitaEmail

./KavitaEmail