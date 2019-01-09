FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY . ./
WORKDIR ./Synchronization/Tenders.Synchronization.FtpMonitoring/
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/Synchronization/Tenders.Synchronization.FtpMonitoring/out .
ENTRYPOINT ["dotnet", "Tenders.Synchronization.FtpMonitoring.dll"]