# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source


# copy csproj different layers

#this sln file need only for the docker

# Core layer
COPY src/Core/*.csproj Core/

# Infrastructure layer
COPY src/Infrastructure/*.csproj Infrastructure/

# Web layer
COPY src/Web/*.csproj Web/

#sln file to restore all projects
COPY src/*.sln .

#restore
RUN dotnet restore

COPY src/ .



# copy everything else and build app

RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./



ENTRYPOINT ["dotnet", "Web.dll"]