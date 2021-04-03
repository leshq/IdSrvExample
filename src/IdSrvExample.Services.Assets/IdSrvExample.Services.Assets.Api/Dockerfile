#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IdSrvExample.Services.Assets.Api/IdSrvExample.Services.Assets.Api.csproj", "IdSrvExample.Services.Assets.Api/"]
RUN dotnet restore "IdSrvExample.Services.Assets.Api/IdSrvExample.Services.Assets.Api.csproj"
COPY . .
WORKDIR "/src/IdSrvExample.Services.Assets.Api"
RUN dotnet build "IdSrvExample.Services.Assets.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdSrvExample.Services.Assets.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdSrvExample.Services.Assets.Api.dll"]