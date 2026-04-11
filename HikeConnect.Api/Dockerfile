FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HikeConnect.Api/HikeConnect.Api.csproj HikeConnect.Api/
RUN dotnet restore HikeConnect.Api/HikeConnect.Api.csproj

COPY . .
RUN dotnet publish HikeConnect.Api/HikeConnect.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HikeConnect.Api.dll"]
