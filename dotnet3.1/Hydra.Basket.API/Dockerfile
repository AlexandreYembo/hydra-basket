#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /dotnet3.1
COPY ["./src/dotnet3.1/Hydra.Basket.API/Hydra.Basket.API.csproj", "services/Hydra.Basket.API/"]
COPY ["./src/dotnet3.1/hydra-core/dotnet3.1/Hydra.WebAPI.Core/Hydra.WebAPI.Core.csproj", "building blocks/Hydra.WebAPI.Core/"]
COPY ["./src/dotnet3.1/hydra-core/dotnet3.1/Hydra.Core.Integration/Hydra.Core.Integration.csproj", "building blocks/Hydra.Core.Integration/"]
COPY ["./src/dotnet3.1/hydra-core/dotnet3.1/Hydra.Core.MessageBus/Hydra.Core.MessageBus.csproj", "building blocks/Hydra.Core.MessageBus/"]

RUN dotnet restore "Hydra.Basket.API/Hydra.Basket.API.csproj"
COPY . .
WORKDIR "src/dotnet3.1/Hydra.Basket.API"
RUN dotnet build "Hydra.Basket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hydra.Basket.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hydra.Basket.API.dll"]