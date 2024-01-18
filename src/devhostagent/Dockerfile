# Build container
ARG TAG=ltsc2022
FROM --platform=windows/amd64 mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-$TAG AS build

ARG Configuration=Release
ARG TelemetryType=TELEMETRY_DEVELOPMENT
ARG MindaroBuildNumber=0.0

WORKDIR /src/devhostagent
COPY /src/devhostagent/devhostAgent.csproj .
COPY /src/common/common.csproj /src/common/
RUN dotnet restore

# USER ContainerAdministrator

COPY /src/devhostagent/ /src/devhostagent/
COPY /src/common/ /src/common/
COPY /build/ /build/
ENV TelemetryType=${TelemetryType}
ENV MINDARO_BUILD_NUMBER=${MindaroBuildNumber}

USER ContainerUser

RUN dotnet publish -c ${Configuration} --self-contained false --no-restore -o /src/publish devhostAgent.csproj

# Final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-$TAG as final

#RUN Install-Module -Name PowerShellGet -Force -AllowClobber -Scope CurrentUser; \
 #   Install-Script -Name Invoke-RestMethod -Force -AllowClobber -Scope CurrentUser

WORKDIR /src/devhostagent
COPY --from=build /src/publish /src/devhostagent
ENTRYPOINT ["dotnet", "/src/devhostagent/Microsoft.BridgeToKubernetes.DevHostAgent.dll"]
