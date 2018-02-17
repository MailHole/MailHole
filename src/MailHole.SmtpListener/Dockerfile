# Build stage
FROM microsoft/dotnet:2.0.5-sdk-2.1.4-stretch AS build-env

WORKDIR /app

# Add all sources as MailHole.Common is required to compile the SMTP listener
COPY . ./
RUN cd MailHole.SmtpListener && \
    dotnet publish -c Release -o out

# Runtime stage
FROM microsoft/dotnet:2.0.5-runtime-stretch

ARG BUILD_DATE
ARG VCS_REF

LABEL org.label-schema.build-date=$BUILD_DATE \
      org.label-schema.name="MailHole SMTP listener" \
      org.label-schema.description="SMTP listener microservice of the MailHole application" \
      org.label-schema.url="https://github.com/baez90/MailHole" \
      org.label-schema.vcs-ref=$VCS_REF \
      org.label-schema.vcs-url="https://github.com/baez90/MailHole" \
      org.label-schema.vendor="Peter Kurfer" \
      org.label-schema.version="Alpha" \
      org.label-schema.schema-version="1.0" \
      maintainer="peter.kurfer@googlemail.com"

WORKDIR /app
COPY --from=build-env /app/MailHole.Api/out .
ENTRYPOINT ["dotnet", "MailHole.SmtpListener.dll"]