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
RUN apt install -y ffmpeg yt-dlp

# Install app
WORKDIR /app
COPY --from=publish /app/publish .

# Configure & run
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "Ytdlp.Web.dll"]
