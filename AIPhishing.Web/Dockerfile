FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AIPhishing.Common/AIPhishing.Common.csproj", "AIPhishing.Common/"]
COPY ["AIPhishing.Database/AIPhishing.Database.csproj", "AIPhishing.Database/"]
COPY ["AIPhishing.Business/AIPhishing.Business.csproj", "AIPhishing.Business/"]
COPY ["AIPhishing.Web/AIPhishing.Web.csproj", "AIPhishing.Web/"]
RUN dotnet restore "AIPhishing.Web/AIPhishing.Web.csproj"
COPY . .
WORKDIR "/src/AIPhishing.Web"
RUN dotnet build "AIPhishing.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AIPhishing.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIPhishing.Web.dll"]
