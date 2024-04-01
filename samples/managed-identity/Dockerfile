#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-cbl-mariner2.0@sha256:ea1751dfd2defec87508aa979088741935d1fd4c870bfd53e7645642fc9ead13 as base
ARG TARGETARCH
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0-cbl-mariner2.0@sha256:17c634c9734df912b2744313c392e35c3bcb13d1f899c5029efaba663e977e03 AS build
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