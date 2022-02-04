FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY email /app
COPY entrypoint.sh /entrypoint.sh
COPY KavitaEmail/config /tmpconfig

RUN apt-get update && \
    apt-get install -y curl nano rsync && \
    rm -rf /var/lib/apt/lists/*

EXPOSE 5003

WORKDIR /app

ENTRYPOINT [ "/bin/bash" ]
CMD [ "/entrypoint.sh" ]