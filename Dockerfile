# 1. Futtatási környezet (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Build környezet (SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Másoljuk át a .csproj fájlt és töltsük le a csomagokat
COPY ["BlogApi/BlogApi.csproj", "./"]
RUN dotnet restore "BlogApi.csproj"

# Másoljuk át az összes többi kódfájlt és buildeljük a projektet
COPY . .
RUN dotnet build "BlogApi.csproj" -c Release -o /app/build

# 3. Publikálás
FROM build AS publish
RUN dotnet publish "BlogApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Végső konténer összerakása
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApi.dll"]
