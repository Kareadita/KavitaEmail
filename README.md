# Kavita Email
This is a simple email relay server that handles emails from Kavita application. By default, Kavita installations 
will use the Kavita hosted email service. However, if a user wants to setup their own SMTP service, then they can run 
their own instance of this microService.

# Installation

### With Docker Run

`docker run --name kavita-email -p 5003:5003 -v ${PWD}/config:/app/config -d kizaing/kavitaemail:latest`

### With Docker Compose

```
version: '3'
services:
     email:
        image: kizaing/kavitaemail:latest
        container_name: kavita-email
        volumes:
           - ./config:/app/config
        ports:
           - "5003:5003"
        restart: unless-stopped
```

After the first run, shut down the container and edit the appsettings.json file inside the config folder. When the settings are to your liking, restart and it should apply your SMTP settings.
