FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app/vault
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/vault/release

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app/vault
COPY --from=build /app/vault/release /app/vault
ENTRYPOINT [ "dotnet", "/app/vault/EctoTech.EctoPass.Backend.Vault.dll" ]
