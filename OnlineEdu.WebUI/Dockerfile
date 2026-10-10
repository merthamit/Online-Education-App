FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnlineEdu.WebUI/OnlineEdu.WebUI.csproj", "OnlineEdu.WebUI/"]
COPY ["OnlineEdu.DTO/OnlineEdu.DTO.csproj", "OnlineEdu.DTO/"]
RUN dotnet restore "./OnlineEdu.WebUI/OnlineEdu.WebUI.csproj"
COPY . .
WORKDIR "/src/OnlineEdu.WebUI"
RUN dotnet build "./OnlineEdu.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnlineEdu.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER root
RUN mkdir -p /keys && chown app /keys
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineEdu.WebUI.dll"]
