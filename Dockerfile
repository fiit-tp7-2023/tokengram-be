FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /app

COPY --link . ./
ENV HUSKY=0

RUN dotnet restore
RUN dotnet publish -c Release --property:PublishDir=out


FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app
COPY --link --from=build-env /app/out ./

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "Tokengram.dll"]
