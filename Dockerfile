FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8090

FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MovieApp.csproj", "MovieApp/"]
RUN dotnet restore "./MovieApp.csproj"
COPY . .
WORKDIR "/src/MovieApp"
RUN dotnet build "./MovieApp.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MovieApp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieApp.dll"]







































































# # This stage is used when running from VS in fast mode (Default for Debug configuration)
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID
# WORKDIR /app
# EXPOSE 8080

# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
# WORKDIR /app

# COPY *.csproj ./
# RUN dotnet restore
# COPY . ./
# RUN dotnet publish -c Release -o out
# FROM mcr.microsoft.com/dotnet/aspnet:9.0
# WORKDIR /app
# COPY --from=build-env /app/out .
# EXPOSE 8090
# ENTRYPOINT ["dotnet", "MovieApp.dll"]

# # See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

