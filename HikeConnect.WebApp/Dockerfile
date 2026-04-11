FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HikeConnect.WebApp/HikeConnect.WebApp.csproj HikeConnect.WebApp/
RUN dotnet restore HikeConnect.WebApp/HikeConnect.WebApp.csproj

COPY . .
RUN dotnet publish HikeConnect.WebApp/HikeConnect.WebApp.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=build /app/publish/wwwroot .

COPY HikeConnect.WebApp/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
