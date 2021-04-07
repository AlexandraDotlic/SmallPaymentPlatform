FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebClient/WebClient.csproj", "WebClient/"]
COPY ["Core/Infrastructure/DataAccess/EfCoreDataAccess/Core.Infrastructure.DataAccess.EfCoreDataAccess.csproj", "Core/Infrastructure/DataAccess/EfCoreDataAccess/"]
COPY ["Core/Domain/Core.Domain.csproj", "Core/Domain/"]
COPY ["Common/Utils/Common.Utils.csproj", "Common/Utils/"]
COPY ["Core/ApplicationServices/Core.ApplicationServices.csproj", "Core/ApplicationServices/"]
COPY ["Core/Infrastructure/Services/MockBankService/Core.Infrastructure.Services.MockBankService.csproj", "Core/Infrastructure/Services/MockBankService/"]
RUN dotnet restore "WebClient/WebClient.csproj"
COPY . .
WORKDIR "/src/WebClient"
RUN dotnet build "WebClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WebClient.dll"]