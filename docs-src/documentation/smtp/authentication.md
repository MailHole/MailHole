# Authentication

Pass `.json` file of the structure

```json
{
    "username": "cleartext password"
}
```

or with encryption (highly recommended!):

```json
{
    "username": "PBKDF2 or BCrypt hash"
}
```

it's also highly recommended to use salts when hashig the passwords!
The salt can be passed to the **MailHole.SmtpListener** by defining the environment variable `SMTP_AUTH_SALT`.