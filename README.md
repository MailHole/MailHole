# MailHole

[![Build status](https://ci.appveyor.com/api/projects/status/u5hn3f0v3xfd5rya?svg=true)](https://ci.appveyor.com/project/PeterKurfer/mailhole)


## Docs

You can find the docs at the [GitHub Pages](https://baez90.github.io/MailHole) of this project.

At the moment the update process of the docs is not automated so they may be out of sync at any time.

To view the very latest version of the docs check out the repository, install [DocFX](https://dotnet.github.io/docfx/) (e.g. with [Chocolatey](https://chocolatey.org)) and run

```bash
docfx -s -f
```

to regenerate the docs and serve it at `http://localhost:8080`.

## Development setup

To start the whole application in development mode switch to the directory `src/docker/` and run start all containers at once by running the following command:

```bash
docker-compose -f MailHole-Debug-compose.yml up
```

It will start all necessary components:

* Traefik
* Redis
* Minio

the two services in development mode:

* MailHole.Api - serving the API of the project
* MailHole.Smtp - service which accepts mails through SMTP

and configure them as required.
Whenever you restart one of the microservices, the code is recompiled if necessary so the workflow is quite fast and easy.

To restart one of the containers pick the appropriate one of the following two commands:

```bash
# to restart the SMTP listener
docker restart docker_mailhole-smtp_1

# to restart the API service
docker restart docker_mailhole-api_1
```