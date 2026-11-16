FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY sistemaagendamento.csproj ./
RUN dotnet restore sistemaagendamento.csproj
COPY . ./
RUN dotnet publish sistemaagendamento.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "sistemaagendamento.dll"]
