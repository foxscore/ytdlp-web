FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /
VOLUME /data
EXPOSE 8080

#############
##  BUILD  ##
#############

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./Ytdlp.Web.csproj"
RUN dotnet build "Ytdlp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ytdlp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

##############
##  RUNTIME ##
##############

FROM base AS final

# Install dependencies
RUN apt update
RUN apt install -y ffmpeg yt-dlp bzip2

# Install PhantomJS
WORKDIR /tmp
RUN apt install -y wget
RUN wget -O phantomjs.tar.bz2 https://bitbucket.org/ariya/phantomjs/downloads/phantomjs-2.1.1-linux-x86_64.tar.bz2
RUN tar -xjf phantomjs.tar.bz2
RUN mv phantomjs-2.1.1-linux-x86_64 /usr/local/share
RUN ln -sf /usr/local/share/phantomjs-2.1.1-linux-x86_64/bin/phantomjs /usr/local/bin
RUN rm phantomjs.tar.bz2

# Install app
COPY --from=publish /app/publish .

# Configure & run
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "Ytdlp.Web.dll"]
