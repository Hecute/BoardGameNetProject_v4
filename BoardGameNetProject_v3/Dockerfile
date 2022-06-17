FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BoardGameNetProject_v3/BoardGameNetProject_v3.csproj", "BoardGameNetProject_v3/"]
RUN dotnet restore "BoardGameNetProject_v3/BoardGameNetProject_v3.csproj"
COPY . .
WORKDIR "/src/BoardGameNetProject_v3"
RUN dotnet build "BoardGameNetProject_v3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BoardGameNetProject_v3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BoardGameNetProject_v3.dll"]
