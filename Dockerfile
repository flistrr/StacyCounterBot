FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /bin
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /bin
COPY --from=build /bin/out ./
ENTRYPOINT ["dotnet", "CounterStacy.dll"]
