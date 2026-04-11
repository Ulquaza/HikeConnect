FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HikeConnect.WebApp/HikeConnect.WebApp.csproj HikeConnect.WebApp/
RUN dotnet restore HikeConnect.WebApp/HikeConnect.WebApp.csproj

COPY . .
RUN dotnet publish HikeConnect.WebApp/HikeConnect.WebApp.csproj -c Release -o /app/publish

COPY --from=build /app/publish/wwwroot .

EXPOSE 7029
