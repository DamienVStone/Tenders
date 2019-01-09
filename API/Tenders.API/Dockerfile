FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY . ./
WORKDIR ./API/Tenders.API/
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/API/Tenders.API/out .
ENTRYPOINT ["dotnet", "Tenders.API.dll"]