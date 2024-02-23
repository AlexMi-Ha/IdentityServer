FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IdentityServer.UI/IdentityServer.UI.csproj", "IdentityServer.UI/"]
COPY ["IdentityServer.Core/IdentityServer.Core.csproj", "IdentityServer.Core/"]
COPY ["IdentityServer.Data/IdentityServer.Data.csproj", "IdentityServer.Data/"]
COPY ["IdentityServer.Validation/IdentityServer.Validation.csproj", "IdentityServer.Validation/"]
RUN dotnet restore "IdentityServer.UI/IdentityServer.UI.csproj"
COPY . .
WORKDIR "/src/IdentityServer.UI"
RUN dotnet build "IdentityServer.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.UI.dll"]
