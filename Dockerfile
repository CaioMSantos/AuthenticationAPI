FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os arquivos do projeto e faz restore para evitar baixar pacotes desnecessários
COPY ["Authentication/Authentication.csproj", "Authentication/"]
RUN dotnet restore "Authentication/Authentication.csproj"

# Copia todo o código do projeto para o container
COPY Authentication/. Authentication/
WORKDIR "/src/Authentication"

# Compila a aplicação
RUN dotnet build "Authentication.csproj" -c Release -o /app/build

# Publica a aplicação
FROM build AS publish
RUN dotnet publish "Authentication.csproj" -c Release -o /app/publish

# Imagem final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.dll"]
