FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MinimalApi/MinimalApi.csproj", "src/MinimalApi/"]
RUN dotnet restore "src/MinimalApi/MinimalApi.csproj"
COPY . .
WORKDIR "/src/src/MinimalApi"
RUN dotnet build "MinimalApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinimalApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApi.dll"]
