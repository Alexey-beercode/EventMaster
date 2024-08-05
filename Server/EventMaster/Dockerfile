FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EventMaster.Presentation/EventMaster.Presentation.csproj", "EventMaster.Presentation/"]
COPY ["EventMaster.BLL/EventMaster.BLL.csproj", "EventMaster.BLL/"]
COPY ["EventMaster.Domain/EventMaster.Domain.csproj", "EventMaster.Domain/"]
COPY ["EventMaster.DAL/EventMaster.DAL.csproj", "EventMaster.DAL/"]
RUN dotnet restore "EventMaster.Presentation/EventMaster.Presentation.csproj"
COPY . .
WORKDIR "/src/EventMaster.Presentation"
RUN dotnet build "EventMaster.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventMaster.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventMaster.Presentation.dll"]
