# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SnipEx.Web/SnipEx.Web.csproj", "SnipEx.Web/"]
COPY ["SnipEx.Common/SnipEx.Common.csproj", "SnipEx.Common/"]
COPY ["SnipEx.Services.Data/SnipEx.Services.Data.csproj", "SnipEx.Services.Data/"]
COPY ["SnipEx.Data.Models/SnipEx.Data.Models.csproj", "SnipEx.Data.Models/"]
COPY ["SnipEx.Data/SnipEx.Data.csproj", "SnipEx.Data/"]
COPY ["SnipEx.Services.Mapping/SnipEx.Services.Mapping.csproj", "SnipEx.Services.Mapping/"]
COPY ["SnipEx.Services.Mediator/SnipEx.Services.Mediator.csproj", "SnipEx.Services.Mediator/"]
COPY ["SnipEx.Realtime/SnipEx.Realtime.csproj", "SnipEx.Realtime/"]
COPY ["SnipEx.Web.ViewModels/SnipEx.Web.ViewModels.csproj", "SnipEx.Web.ViewModels/"]
COPY ["SnipEx.Web.Infrastructure/SnipEx.Web.Infrastructure.csproj", "SnipEx.Web.Infrastructure/"]
RUN dotnet restore "./SnipEx.Web/SnipEx.Web.csproj"
COPY SnipEx.Web/. ./SnipEx.Web/
COPY SnipEx.Common/. ./SnipEx.Common/
COPY SnipEx.Services.Data/. ./SnipEx.Services.Data/
COPY SnipEx.Data.Models/. ./SnipEx.Data.Models/
COPY SnipEx.Data/. ./SnipEx.Data/
COPY SnipEx.Data/Datasets/ ./SnipEx.Data/Datasets/
COPY SnipEx.Services.Mapping/. ./SnipEx.Services.Mapping/
COPY SnipEx.Services.Mediator/. ./SnipEx.Services.Mediator/
COPY SnipEx.Realtime/. ./SnipEx.Realtime/
COPY SnipEx.Web.ViewModels/. ./SnipEx.Web.ViewModels/
COPY SnipEx.Web.Infrastructure/. ./SnipEx.Web.Infrastructure/
WORKDIR "/src/SnipEx.Web"
RUN dotnet build "./SnipEx.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SnipEx.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SnipEx.Web.dll"]