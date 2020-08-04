#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
EXPOSE 1000
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ConnectionServer/ConnectionServer.csproj", "ConnectionServer/"]
COPY ["Lib/STDLib/STDLib.csproj", "Lib/STDLib/"]
RUN dotnet restore "ConnectionServer/ConnectionServer.csproj"
COPY . .
WORKDIR "/src/ConnectionServer"
RUN dotnet build "ConnectionServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConnectionServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConnectionServer.dll"]