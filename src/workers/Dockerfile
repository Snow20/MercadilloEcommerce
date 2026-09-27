FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ServiceNowWorker.csproj", "./"]
RUN dotnet restore "ServiceNowWorker.csproj"
COPY . .
RUN dotnet publish "ServiceNowWorker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServiceNowWorker.dll"]