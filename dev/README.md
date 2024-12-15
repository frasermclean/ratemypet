# Local development dependencies

This directory contains a Docker Compose file that can be used to run the
local development environment. 

## Pre-requisites

Before you can start the local development environment, you need to install the following tools:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [Mkcert](https://github.com/FiloSottile/mkcert)

## Create certificates

To create the certificates, run the following command:

```bash
mkcert --cert-file certs/cert.pem --key-file certs/key.pem "localhost" "127.0.0.1" "::1"
```

This will create a `cert.pem` and `key.pem` file in the `certs` directory.

## Start the local development environment

To start the local development environment, run the following command:

```bash
docker compose up -d
```
