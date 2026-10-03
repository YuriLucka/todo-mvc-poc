FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/TodoMvc/TodoMvc.csproj src/TodoMvc/
RUN dotnet restore src/TodoMvc/TodoMvc.csproj
COPY src/ src/
RUN dotnet publish src/TodoMvc/TodoMvc.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoMvc.dll"]
