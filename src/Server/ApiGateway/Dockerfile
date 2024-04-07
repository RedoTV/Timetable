FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish ApiGateway.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app .
ENTRYPOINT [ "dotnet", "ApiGateway.dll" ]