# MailHole documentation

## Features

* Keep mails for a specified time or until they are deleted
* Store whole mail including headers, text and HTML body
* Store mail attachements including some file hashes (planned are MD5, SHA1 and SHA256)
* Dashboard to browse all the received mails, attachements and so on
* Dashboard to have a look at the processing queue
* Dashboard to get some metrics of the service like how many mails were received, ...
* API to check if an attachement was transmitted correctly by passing a prebuilt hash to API or by fetching the attachement details including its hashes

## Architecture

### Application parts

* MailHole.SmtpListener - Service which receives mails and stores them for further processing
* MailHole.Api - WebAPI project to access receiver and attachement informations and statistics

### External tools and frameworks

* [Redis](https://redis.io) - used to store all metadata and text parts of the mails
* [Minio](https://minio.io) - Amazon S3 replacement to store all attachements
* [Traefik](https://traefik.io) - API Gateway and load balancer used to serve a consistent API
* [Hangfire](https://www.hangfire.io) - used to delete mails after specific time and for asynchronous processing of incoming mails
* [App-Metrics](https://www.app-metrics.io) - used to collect performance indicators and health state of the custom components