FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HikeConnect.WebApp/HikeConnect.WebApp.csproj HikeConnect.WebApp/
RUN dotnet restore HikeConnect.WebApp/HikeConnect.WebApp.csproj

COPY . .
RUN dotnet publish HikeConnect.WebApp/HikeConnect.WebApp.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish/wwwroot .

ENV ASPNETCORE_URLS=https://+:7029
EXPOSE 7029

ENTRYPOINT ["dotnet", "HikeConnect.WebApp.dll"]
