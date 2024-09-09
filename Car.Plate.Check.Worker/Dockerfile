FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Car.Plate.Check.Worker/Car.Plate.Check.Worker.csproj", "Car.Plate.Check.Worker/"]
RUN dotnet restore "Car.Plate.Check.Worker/Car.Plate.Check.Worker.csproj"
COPY . .
WORKDIR "/src/Car.Plate.Check.Worker"
RUN dotnet build "Car.Plate.Check.Worker.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Car.Plate.Check.Worker.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Car.Plate.Check.Worker.dll"]
