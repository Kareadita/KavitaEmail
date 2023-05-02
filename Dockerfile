FROM ubuntu:focal AS copytask

ARG TARGETPLATFORM

#Moves the files over
RUN mkdir /files
COPY _output/*.tar.gz /files/
COPY copy_runtime.sh /copy_runtime.sh
RUN /copy_runtime.sh

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY --from=copytask /KavitaEmail /app

COPY entrypoint.sh /entrypoint.sh
COPY KavitaEmail/config/templates /app/config/templates
COPY KavitaEmail/config/appsettings.json /tmp/appsettings.json

RUN apt-get update && \
    apt-get install -y curl nano rsync && \
    rm -rf /var/lib/apt/lists/*

EXPOSE 5003

WORKDIR /app

ENTRYPOINT [ "/bin/bash" ]
CMD [ "/entrypoint.sh" ]