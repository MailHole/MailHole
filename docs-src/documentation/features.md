# Features

* Mass mails (e.g. newsletters): create a job (pass a template to matching including some variables and get a JobId), receive all mails containing a job header (e.g. `X-MailHole-JobId`) and check if all mails are matching the template
* SpamAssasin integration: pass the mails to SpamAssasin and calculate the spam score
* Registration of accounts to get only mails associated to that account
* Received mails without registration/authentication are passed to a public space (lower retention policy e.g. 10 minutes)
* Integration of a service that shows the mails in different client simulations like Outlook, Apple Mail, Android, ...