FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Bemásoljuk a projektfájlt a BlogApi almappába
COPY ["BlogApi/BlogApi.csproj", "BlogApi/"]
RUN dotnet restore "BlogApi/BlogApi.csproj"

# 2. Bemásoljuk a teljes forráskódot
COPY . .

# 3. Átlépünk a BlogApi almappába (ez szünteti meg a duplikációt!)
WORKDIR "/src/BlogApi"
RUN dotnet build "BlogApi.csproj" -c Release -o /app/build

# 4. Publikálás
FROM build AS publish
RUN dotnet publish "BlogApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 5. Végső futtatási környezet
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApi.dll"]
