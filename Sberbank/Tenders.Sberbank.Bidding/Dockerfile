FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY . ./
WORKDIR ./Sberbank/Tenders.Sberbank.Bidding/
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/Sberbank/Tenders.Sberbank.Bidding/out .
ENTRYPOINT ["dotnet", "Tenders.Sberbank.Bidding.dll"]