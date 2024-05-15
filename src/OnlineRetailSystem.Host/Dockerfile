#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnlineRetailSystem.Host/OnlineRetailSystem.Host.csproj", "OnlineRetailSystem.Host/"]
COPY ["OnlineRetailSystem.Actors.CustomerSubdomain/OnlineRetailSystem.Actors.CustomerSubdomain.csproj", "OnlineRetailSystem.Actors.CustomerSubdomain/"]
COPY ["OnlineRetailSystem.Actors.Core/OnlineRetailSystem.Actors.Core.csproj", "OnlineRetailSystem.Actors.Core/"]
COPY ["OnlineRetailSystem.Actors.OrderSubdomain/OnlineRetailSystem.Actors.OrderSubdomain.csproj", "OnlineRetailSystem.Actors.OrderSubdomain/"]
COPY ["OnlineRetailSystem.Actors.ProductSubdomain/OnlineRetailSystem.Actors.ProductSubdomain.csproj", "OnlineRetailSystem.Actors.ProductSubdomain/"]
COPY ["OnlineRetailSystem.Actors.ShoppingCartSubdomain/OnlineRetailSystem.Actors.ShoppingCartSubdomain.csproj", "OnlineRetailSystem.Actors.ShoppingCartSubdomain/"]
COPY ["OnlineRetailSystem.Actors.Orleans/OnlineRetailSystem.Actors.Orleans.csproj", "OnlineRetailSystem.Actors.Orleans/"]
RUN dotnet restore "./OnlineRetailSystem.Host/OnlineRetailSystem.Host.csproj"
COPY . .
WORKDIR "/src/OnlineRetailSystem.Host"
RUN dotnet build "./OnlineRetailSystem.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnlineRetailSystem.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineRetailSystem.Host.dll"]