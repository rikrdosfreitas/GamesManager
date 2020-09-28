#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /app

COPY . /app
WORKDIR /app/Tests/Games.Application.IntegrationTests
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/wait_for_it.sh \
    && dotnet restore Games.Application.IntegrationTests.csproj