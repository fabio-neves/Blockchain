FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# copy all and restore 
COPY . ./

# build (note: the command "dotnet restore" will run implicit)
RUN dotnet publish ./Blockchain/Blockchain.csproj -c Release -o ../../out/

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build-env out ./
ENTRYPOINT ["dotnet", "Blockchain.dll"]