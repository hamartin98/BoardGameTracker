FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/BoardGameTrackerApi/BoardGameTrackerApi.csproj", "./"]
RUN dotnet restore "BoardGameTrackerApi.csproj"
COPY . .
RUN dotnet build "src/BoardGameTrackerApi/BoardGameTrackerApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish 
ARG BUILD_CONFIGURATION=Release 
RUN dotnet publish "src/BoardGameTrackerApi/BoardGameTrackerApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

FROM base AS final 
WORKDIR /app 
COPY --from=publish /app/publish . 

RUN useradd -ms /bin/bash appuser
USER appuser

ENTRYPOINT ["dotnet", "BoardGameTrackerApi.dll"] 