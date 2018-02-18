FROM microsoft/dotnet:2.0.5-sdk-2.1.4-stretch

WORKDIR /app

# Add all sources as MailHole.Common is required to compile the SMTP listener
COPY . ./
RUN dotnet restore

VOLUME /app

EXPOSE 25
EXPOSE 587

ENTRYPOINT ["dotnet", "run", "-p", "MailHole.SmtpListener"]