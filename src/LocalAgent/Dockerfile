#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:789045ecae51d62d07877994d567eff4442b7bbd4121867898ee7bf00b7241ea AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0.203@sha256:249a78aa4ce22ab872d0dff0490a6389e7bc087d2c080c4ffc7569b49cf0e23b AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["LocalAgent/localagent.csproj", "LocalAgent/"]
COPY ["library/library.csproj", "library/"]
COPY ["common/common.csproj", "common/"]
RUN dotnet restore "LocalAgent/localagent.csproj"
COPY . .
WORKDIR "/src/LocalAgent"
RUN dotnet build "localagent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "localagent.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update
RUN apt-get install iptables
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microsoft.BridgeToKubernetes.LocalAgent.dll"]