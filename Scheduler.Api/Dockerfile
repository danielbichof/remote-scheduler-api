FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Scheduler.Api/Scheduler.Api.csproj", "Scheduler.Api/"]
COPY ["Scheduler.Application/Scheduler.Application.csproj", "Scheduler.Application/"]
COPY ["Scheduler.Domain/Scheduler.Domain.csproj", "Scheduler.Domain/"]
COPY ["Scheduler.Infrastructure/Scheduler.Infrastructure.csproj", "Scheduler.Infrastructure/"]
RUN dotnet restore "Scheduler.Api/Scheduler.Api.csproj"
COPY . .
WORKDIR "/src/Scheduler.Api"
RUN dotnet build "Scheduler.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Scheduler.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scheduler.Api.dll"]
