FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8
ARG configuration=Release
WORKDIR /src
COPY ["src/AddressManager.API/AddressManager.API.csproj", "src/AddressManager.API/"]
RUN dotnet restore "src/AddressManager.API/AddressManager.API.csproj"
COPY . .
WORKDIR "/src/src/AddressManager.API"
RUN dotnet build "AddressManager.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AddressManager.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AddressManager.API.dll"]
