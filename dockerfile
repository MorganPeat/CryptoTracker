# Final runtime image
FROM microsoft/dotnet:2.1.2-aspnetcore-runtime AS base
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://*:8080
EXPOSE 8080


# Build image
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

# Run restore first to help docker image caching
COPY src/CryptoTracker.MarketData/CryptoTracker.MarketData.csproj /src/CryptoTracker.MarketData/
WORKDIR /src/CryptoTracker.MarketData
RUN dotnet restore

# Copy all source and build
ADD common /common/
ADD src/CryptoTracker.MarketData/ /src/CryptoTracker.MarketData/
RUN dotnet build --no-restore -c Release -o /app
RUN dotnet publish --no-restore -c Release -o /app

# Copy build output to final image
FROM base AS final
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "CryptoTracker.MarketData.dll"]
