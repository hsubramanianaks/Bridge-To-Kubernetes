# Build container
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0-cbl-mariner2.0@sha256:9e054d0663d07e09127d114f753c1068d0bf681eab188352d06f111ce68f050f AS build
ARG TARGETARCH
ARG Configuration=Release
ARG TelemetryType=TELEMETRY_DEVELOPMENT
ARG MindaroBuildNumber=0.0


WORKDIR /src/routingmanager
COPY /src/routingmanager/routingmanager.csproj .
COPY /src/common/common.csproj /src/common/
RUN dotnet restore -a ${TARGETARCH}
COPY /src/routingmanager/ /src/routingmanager/
COPY /src/common/ /src/common/
COPY /build/ /build/
ENV TelemetryType=${TelemetryType}
ENV MINDARO_BUILD_NUMBER=${MindaroBuildNumber}

RUN dotnet publish -c ${Configuration} -a ${TARGETARCH} --self-contained false --no-restore -o /src/publish

# Final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0-cbl-mariner2.0@sha256:5494f5b2eec5bbbae426168bff879624374d945406b2fdfdfb0cc2e88a4b327c as final
ARG TARGETARCH
ARG KUBECTL_VERSION=v1.27.3
ARG INSTALL_LOCATION=/app/kubectl/linux

# Setup common tools
RUN tdnf clean all && \
  tdnf check-update && \
  tdnf upgrade -y

RUN tdnf install -y \
  procps \
  bind-utils

COPY /build/setup-kubectl.sh .
RUN chmod +x ./setup-kubectl.sh \
    && ./setup-kubectl.sh ${KUBECTL_VERSION} ${INSTALL_LOCATION} ${TARGETARCH} \
    && rm -f ./setup-kubectl.sh

WORKDIR /src/routingmanager
COPY --from=build /src/publish /src/routingmanager
ENTRYPOINT ["dotnet", "/src/routingmanager/Microsoft.BridgeToKubernetes.RoutingManager.dll"]
