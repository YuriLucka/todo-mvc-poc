FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/TodoApp.Shared/TodoApp.Shared.csproj src/TodoApp.Shared/
COPY src/TodoApp.Api/TodoApp.Api.csproj src/TodoApp.Api/
RUN dotnet restore src/TodoApp.Api/TodoApp.Api.csproj
COPY src/TodoApp.Shared/ src/TodoApp.Shared/
COPY src/TodoApp.Api/ src/TodoApp.Api/
RUN dotnet publish src/TodoApp.Api/TodoApp.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoApp.Api.dll"]
