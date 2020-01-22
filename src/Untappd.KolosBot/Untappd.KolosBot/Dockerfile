FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

CMD ls

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

CMD ls

COPY ["Untappd.KolosBot/Untappd.KolosBot.csproj", "Untappd.KolosBot/"]
COPY ["Untappd.KolosBot.Infrastucture/Untappd.KolosBot.Infrastucture.csproj", "Untappd.KolosBot.Infrastucture/"]
RUN dotnet restore "Untappd.KolosBot/Untappd.KolosBot.csproj"
COPY . .
WORKDIR "/src/Untappd.KolosBot"
RUN dotnet build "Untappd.KolosBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Untappd.KolosBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Untappd.KolosBot.dll"]
