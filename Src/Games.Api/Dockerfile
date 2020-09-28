#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/Games.Api/Games.Api.csproj", "Src/Games.Api/"]
COPY ["Src/Games.Application/Games.Application.csproj", "Src/Games.Application/"]
COPY ["Src/Games.Infrastructure/Games.Infrastructure.csproj", "Src/Games.Infrastructure/"]
COPY ["Src/Games.Domain/Games.Domain.csproj", "Src/Games.Domain/"]
COPY ["Src/Games.Common/Games.Common.csproj", "Src/Games.Common/"]
RUN dotnet restore "Src/Games.Api/Games.Api.csproj"
COPY . .
WORKDIR "/src/Src/Games.Api"
RUN dotnet build "Games.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Games.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Games.Api.dll"]