#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8081:80
EXPOSE 443:443

# Set environment variables for certificate path and password
ENV CERTIFICATE_PATH="/app/path_to_your_certificate.pfx"
ENV CERTIFICATE_PASSWORD="certificate_password"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["mockTecoAPI/mockTecoAPI.csproj", "mockTecoAPI/"]
RUN dotnet restore "mockTecoAPI/mockTecoAPI.csproj"
COPY . .
WORKDIR "/src/mockTecoAPI"
RUN dotnet build "mockTecoAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "mockTecoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mockTecoAPI.dll"]