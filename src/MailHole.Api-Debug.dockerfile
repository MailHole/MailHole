# Build stage
FROM microsoft/dotnet:2.0.5-sdk-2.1.4-stretch AS build-env

WORKDIR /app

# Add all sources as MailHole.Common is required to compile the API
COPY . ./
RUN dotnet restore

VOLUME [ "/app" ]

ENTRYPOINT ["dotnet", "run", "-p", "MailHole.Api"]