FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /usr/local/team07/tokengram/backend

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release --property:PublishDir=out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /usr/local/team07/tokengram/backend
COPY --from=build-env /usr/local/team07/tokengram/backend/out .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "Tokengram.dll"]
