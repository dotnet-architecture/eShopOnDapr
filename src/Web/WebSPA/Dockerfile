#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG NODE_IMAGE=15.12.0-alpine3.12
ARG NET_IMAGE=5.0-buster-slim
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
WORKDIR /app
EXPOSE 80
# install node
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq

FROM node:${NODE_IMAGE} as node-build
WORKDIR /web
COPY src/Web/WebSPA/package.json .
COPY src/Web/WebSPA/package-lock.json .
COPY src/Web/WebSPA .
RUN npm i npm@6.14.11 -g && npm update && npm install && npm run build:prod

FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
# install node
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq
WORKDIR /src

# Create this "restore-solution" section by running ./Create-DockerfileSolutionRestore.ps1, to optimize build cache reuse
COPY ["src/ApiGateways/Aggregators/Web.Shopping.HttpAggregator/Web.Shopping.HttpAggregator.csproj", "src/ApiGateways/Aggregators/Web.Shopping.HttpAggregator/"]
COPY ["src/BuildingBlocks/EventBus/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/EventBus/"]
COPY ["src/BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "src/BuildingBlocks/EventBus/IntegrationEventLogEF/"]
COPY ["src/BuildingBlocks/WebHost/WebHost.Customization/WebHost.Customization.csproj", "src/BuildingBlocks/WebHost/WebHost.Customization/"]
COPY ["src/Services/Basket/Basket.API/Basket.API.csproj", "src/Services/Basket/Basket.API/"]
COPY ["src/Services/Catalog/Catalog.API/Catalog.API.csproj", "src/Services/Catalog/Catalog.API/"]
COPY ["src/Services/Identity/Identity.API/Identity.API.csproj", "src/Services/Identity/Identity.API/"]
COPY ["src/Services/Ordering/Ordering.API/Ordering.API.csproj", "src/Services/Ordering/Ordering.API/"]
COPY ["src/Services/Payment/Payment.API/Payment.API.csproj", "src/Services/Payment/Payment.API/"]
COPY ["src/Web/WebSPA/WebSPA.csproj", "src/Web/WebSPA/"]
COPY ["src/Web/WebStatus/WebStatus.csproj", "src/Web/WebStatus/"]
COPY ["docker-compose.dcproj", "./"]
COPY ["NuGet.config", "./"]
COPY ["eShopOnDapr.sln", "./"]
RUN dotnet restore "eShopOnDapr.sln"

COPY . .
COPY --from=node-build /web/wwwroot /src/src/Web/WebSPA/wwwroot/
WORKDIR /src/src/Web/WebSPA
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebSPA.dll"]
