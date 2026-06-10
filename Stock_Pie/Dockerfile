## Multi-stage Dockerfile for Stock_Pie (.NET 10)
## Build stage
#FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
#WORKDIR /src
#
## Copy csproj and restore as distinct layers
#COPY ["Stock_Pie/Stock_Pie.csproj", "Stock_Pie/"]
#RUN dotnet restore "Stock_Pie/Stock_Pie.csproj"
#
## Copy everything and publish
#COPY . .
#WORKDIR /src/Stock_Pie
#RUN dotnet publish "Stock_Pie.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
## Runtime stage
#FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
#WORKDIR /app
#ENV ASPNETCORE_URLS="http://+:80"
#EXPOSE 80
#
#COPY --from=build /app/publish .
#ENTRYPOINT ["dotnet", "Stock_Pie.dll"]
#

# Multi-stage Dockerfile for Stock_Pie (.NET 10)
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Stock_Pie.csproj", "."]
RUN dotnet restore "Stock_Pie.csproj"
COPY . .
RUN dotnet publish "Stock_Pie.csproj" -c Release -o /app/publish /p:UseAppHost=false
# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Stock_Pie.dll"]