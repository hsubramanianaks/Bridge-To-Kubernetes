FROM mcr.microsoft.com/dotnet/aspnet:8.0-cbl-mariner2.0@sha256:5494f5b2eec5bbbae426168bff879624374d945406b2fdfdfb0cc2e88a4b327c AS base
ARG TARGETARCH
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0-cbl-mariner2.0@sha256:69e18e87d68a6653550c1eed320cdddc3ff39296bb9d04f3317eca6e500daea9 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["databaseApi.csproj", ""]
RUN dotnet restore "./databaseApi.csproj" -a ${TARGETARCH}
COPY . .
WORKDIR "/src/."
RUN dotnet build "databaseApi.csproj" -c Release -o /app/build -a ${TARGETARCH}

FROM build AS publish
RUN dotnet publish "databaseApi.csproj" -c Release -a ${TARGETARCH} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "databaseApi.dll"]