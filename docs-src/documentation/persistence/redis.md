# Redis

## Entities

### ReceiverInfo

Every receiver is stored in Redis as a JSON value with the mail address as key.
The JSON value has the following structure:

```json
{
    "mailStorageGuid": "b8187230-ced6-48f9-957a-6a76b4250d8a",
    "attachementStorageGuid": "33250880-d59e-4513-8f54-d5edb0ea3957",
    "knownNames": [
        "Hans Dampf",
        "Rainer Zufall"
    ],
    "lastMailReceived": ".NET DateTime format",
    "totalMailsReceived": 42
}
```

The property `mailStorageGuid` is used to resolve a hashset containing all mails the user has already received.
See [ReceivedMail](#receivemMail) for further information.

The property `attachementStorageGuid` is used to resolve a hashset containing all attachements informations the user has already received.
See [AttachementsInfo](#attachementsinfo) for further information.

### ReceivedMail

Every received mail is stored in the mail storage of the receiver.
Every time _MailHole_ received a mail it assigns a GUID to the mail and stores the mail under this GUID in the mail storage of the receiver.

The received mail has the following structure

```json
{
    "sender": "any@one.com",
    "bcc": [
        "another@one.com"
    ],
    "cc": [
        "andAnother@one.com"
    ],
    "subject": "subject of the mail",
    "htmlBody": "<html><body>you know...HTML</body></html>",
    "textBody": "yeah...the text body of the mail",
    "headers": {
        "header1": "and its value"
    },
    "utcReceivedTime": ".NET DateTime format",
    "attachementsGuid": "f068d581-3ff3-478a-8681-b0c203416be0"
}
```

If the mail had attachements the property `attachementsGuid` will be set to a GUID that can be used to lookup the attachements information in the attachements storage of a receiver.
See [AttachementsInfo](#attachementsinfo) for further information.

### AttachementsInfo

The attachements information entity contains all information about a single mail attachement.
It is stored as a JSON array in the attachements storage hashset of a receiver at the `attachementsGuid` key.

The entity has the following structure:

```json
[
    {
        "originalFileName": "LICENSE",
        "mimeType": "text/plain",
        "fileSize": 35821,
        "md5": "3C34AFDC3ADF82D2448F12715A255122",
        "sha1": "7713A1753CE88F2C7E6B054ECC8E4C786DF76300",
        "sha256": "0B383D5A63DA644F628D99C33976EA6487ED89AAA59F0B3257992DEAC1171E6B"
    },
    {
        "originalFileName": "docfx.json",
        "mimeType": "text/plain",
        "fileSize": 1131,
        "md5": "37378F0021B80D47B0EBF621B5FB97CC",
        "sha1": "92F26C0B07AF25FFBFEEAF04AFDF8C11AAA755BC",
        "sha256": "294F1C714150053A4EF1783590F01CD05924B77AFBC45518929D0902F960E982"
    }
]
```

Where `fileSize` is the size in bytes.
The other properties should be clear.

## Relationships

_TODO: Summarize relationships with PlantUML class diagram_.