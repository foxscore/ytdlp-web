FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /
VOLUME /data
EXPOSE 80

#############
##  BUILD  ##
#############

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["yt-dlp-web/yt-dlp-web.csproj", "yt-dlp-web/"]
RUN dotnet restore "yt-dlp-web/yt-dlp-web.csproj"
COPY . .
WORKDIR "/src/yt-dlp-web"
RUN dotnet build "yt-dlp-web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "yt-dlp-web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

##############
##  RUNTIME ##
##############

FROM base AS final

# Install dependencies
RUN apt update
RUN apt install -y ffmpeg yt-dlp

# Install app
WORKDIR /app
COPY --from=publish /app/publish .

# Configure & run
USER $APP_UID
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Ytdlp.Web.dll"]
