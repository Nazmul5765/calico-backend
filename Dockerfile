FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["lofi-backend/lofi-backend.csproj", "lofi-backend/"]
RUN dotnet restore "lofi-backend/lofi-backend.csproj"

COPY . .

WORKDIR "/src/lofi-backend"
RUN dotnet publish "lofi-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

CMD ["sh", "-c", "dotnet lofi-backend.dll --urls http://0.0.0.0:${PORT:-8080}"]
