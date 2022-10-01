# Kavita Email
This is a simple email relay server that handles emails from Kavita application. By default, Kavita installations 
will use the Kavita hosted email service. However, if a user wants to setup their own SMTP service, then they can run 
their own instance of this microService.

# Installation

### With Docker Run

`docker run --name kavita-email -p 5003:5003 -e SMTP_HOST="" -e SMTP_PORT="" -e SMTP_USER="" -e SMTP_PASS="" -e SEND_ADDR="" -e DISP_NAME="" -e ALLOW_SENDTO="true" -d kizaing/kavitaemail:latest`

### With Docker Compose

```
version: '3'
services:
     email:
        image: kizaing/kavitaemail:latest
        container_name: kavita-email
        environment:
           - SMTP_HOST=<your smtp hostname here>
           - SMTP_PORT=<smtp port>
           - SMTP_USER=<smtp username>
           - SMTP_PASS=<smtp password>
           - SEND_ADDR=<address you are sending emails from>
           - DISP_NAME=<display name to use>
           - ALLOW_SENDTO=<true/false if you want the service to email files for Kavita>
        ports:
           - "5003:5003"
        restart: unless-stopped
```

Simply fill in the variables with your SMTP settings and then in Kavita put in the address of this email service. If running with Kavita in a docker-compose stack you can use the service name you defined for this email relay. 
