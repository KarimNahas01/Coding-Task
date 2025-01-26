FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend.csproj ./

RUN dotnet restore "backend.csproj"

COPY . ./

WORKDIR /src
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend.dll"]
