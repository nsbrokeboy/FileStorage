FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY FileStorage.Domain/FileStorage.Domain.csproj FileStorage.Domain/
COPY FileStorage.Application/FileStorage.Application.csproj FileStorage.Application/
COPY FileStorage.Infrastructure/FileStorage.Infrastructure.csproj FileStorage.Infrastructure/
COPY FileStorage.Api/FileStorage.Api.csproj FileStorage.Api/

RUN dotnet restore FileStorage.Api/FileStorage.Api.csproj

COPY . . 

RUN dotnet publish FileStorage.Api/FileStorage.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80

ENTRYPOINT ["dotnet", "FileStorage.Api.dll"]