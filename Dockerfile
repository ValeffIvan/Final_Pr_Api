# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Final_Pr_Api/*.csproj ./Final_Pr_Api/
RUN dotnet restore

# copy everything else and build app
COPY Final_Pr_Api/. ./Final_Pr_Api/
WORKDIR /source/Final_Pr_Api
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Final_Pr_Api.dll"]

ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 80 
