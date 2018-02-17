# Build stage
FROM microsoft/dotnet:2.0.5-sdk-2.1.4-stretch AS build-env

WORKDIR /app

# Add all sources as MailHole.Common is required to compile the API
COPY . ./
RUN cd MailHole.Api && \
    dotnet publish -c Release -o out

# Runtime stage
FROM microsoft/aspnetcore:2.0.5-stretch
WORKDIR /app
COPY --from=build-env /app/MailHole.Api/out .
ENTRYPOINT ["dotnet", "MailHole.Api.dll"]