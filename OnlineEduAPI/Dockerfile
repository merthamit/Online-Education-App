FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnlineEduAPI/OnlineEduAPI.csproj", "OnlineEduAPI/"]
COPY ["OnlineEdu.Business/OnlineEdu.Business.csproj", "OnlineEdu.Business/"]
COPY ["OnlineEdu.DataAccess/OnlineEdu.DataAccess.csproj", "OnlineEdu.DataAccess/"]
COPY ["OnlineEdu.DTO/OnlineEdu.DTO.csproj", "OnlineEdu.DTO/"]
COPY ["OnlineEdu.Entity/OnlineEdu.Entity.csproj", "OnlineEdu.Entity/"]
RUN dotnet restore "./OnlineEduAPI/OnlineEduAPI.csproj"
COPY . .
WORKDIR "/src/OnlineEduAPI"
RUN dotnet build "./OnlineEduAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnlineEduAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineEduAPI.dll"]
