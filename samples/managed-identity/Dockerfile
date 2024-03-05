#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-cbl-mariner2.0@sha256:5494f5b2eec5bbbae426168bff879624374d945406b2fdfdfb0cc2e88a4b327c as base
ARG TARGETARCH
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0-cbl-mariner2.0@sha256:9e054d0663d07e09127d114f753c1068d0bf681eab188352d06f111ce68f050f AS build
ARG TARGETARCH
WORKDIR /src
COPY "mi-webapp.csproj" .

RUN dotnet restore "mi-webapp.csproj" -a ${TARGETARCH}

COPY . .

WORKDIR "/src/."
RUN dotnet build "mi-webapp.csproj" -c Release -o /app/build -a ${TARGETARCH}

FROM build AS publish
RUN dotnet publish "mi-webapp.csproj" -c Release -o /app/publish -a ${TARGETARCH}

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mi-webapp.dll"]