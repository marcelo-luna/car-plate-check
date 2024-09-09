FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Car.Plate.Chek.Device.Worker/Car.Plate.Chek.Device.Worker.csproj", "Car.Plate.Chek.Device.Worker/"]
RUN dotnet restore "Car.Plate.Chek.Device.Worker/Car.Plate.Chek.Device.Worker.csproj"
COPY . .
WORKDIR "/src/Car.Plate.Chek.Device.Worker"
RUN dotnet build "Car.Plate.Chek.Device.Worker.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Car.Plate.Chek.Device.Worker.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Car.Plate.Chek.Device.Worker.dll"]
