FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN mkdir -p /src
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o out
RUN dotnet dev-certs https -ep /src/out/certificate.pfx -p password

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
RUN mkdir -p /app
WORKDIR /app
COPY --from=build /src/out .