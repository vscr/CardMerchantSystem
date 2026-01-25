FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["CardMerchantSystem.sln", "./"]

# Copy all csproj files (basit yaklaþým - tüm csproj'leri kopyala)
COPY ["CardMerchantSystem.API/*.csproj", "CardMerchantSystem.API/"]
COPY ["CardMerchantSystem.Shared/*.csproj", "CardMerchantSystem.Shared/"]

# Restore
RUN dotnet restore "CardMerchantSystem.API/CardMerchantSystem.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/CardMerchantSystem.API"
RUN dotnet build "CardMerchantSystem.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CardMerchantSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CardMerchantSystem.API.dll"]