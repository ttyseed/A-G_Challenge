# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# New stage (Build runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# Set environment variable to avoid interactive prompts during package installations
ENV DEBIAN_FRONTEND=noninteractive

WORKDIR /app

EXPOSE 80   
#for local docker desktop testing
EXPOSE 443  
#for local docker desktop testing
EXPOSE 3000 
#for http alb to container routing
EXPOSE 3080 
#for https alb to container routing

ENV ASPNETCORE_URLS=http://+:3000;https://+:3080

ENV TZ=Asia/Singapore
ENV ENVIRONMENT="LOCAL"
#ENV ENVIRONMENT="DEV"
#ENV ENVIRONMENT="UAT"
#ENV ENVIRONMENT="PRD"

# New stage (Build stage)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Install tools
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy all project files
COPY ["Challenge.Application.API/Challenge.Application.API.csproj", "Challenge.Application.API/"]
COPY ["Challenge.Application.Bll/Challenge.Application.Bll.csproj", "Challenge.Application.Bll/"]
COPY ["Challenge.Application.ViewModel/Challenge.Application.ViewModel.csproj", "Challenge.Application.ViewModel/"]
COPY ["Challenge.Common.EmailService/Challenge.Common.EmailService.csproj", "Challenge.Common.EmailService/"]
COPY ["Challenge.Common.Extension/Challenge.Common.Extension.csproj", "Challenge.Common.Extension/"]
COPY ["Challenge.Common.Logging/Challenge.Common.Logging.csproj", "Challenge.Common.Logging/"]
COPY ["Challenge.Common.Mapper/Challenge.Common.Mapper.csproj", "Challenge.Common.Mapper/"]
COPY ["Challenge.Common.Util/Challenge.Common.Util.csproj", "Challenge.Common.Util/"]
COPY ["Challenge.Database.Database/Challenge.Database.Repositories.csproj", "Challenge.Database.Database/"]
COPY ["Challenge.Database.Models/Challenge.Database.Models.csproj", "Challenge.Database.Models/"]

# Restore dependencies
RUN dotnet restore "Challenge.Application.API/Challenge.Application.API.csproj"

# Copy all source files
COPY . .

# Build and publish (no restore needed since already restored)
# Using /p:UseAppHost=false to create framework-dependent deployment to reduce image size (AppHost is not needed in container, only in self-contained deployments)
RUN dotnet publish "Challenge.Application.API/Challenge.Application.API.csproj" -c Release -o /app/out --no-restore /p:UseAppHost=false

# New stage (final stage - Runtime image will actually be run)
FROM base AS final

# Create user and set permissions
RUN groupadd -r oshd && useradd --no-log-init -r -g oshd oshd_user && \
    chown -R oshd_user:oshd /app && \
    chmod -R 755 /app && \
    mkdir -p /app/wwwroot/img && \
    chown -R oshd_user:oshd /app/wwwroot/img && \
    chmod -R 755 /app/wwwroot/img

# Copy everything from /app/out in the stage named build into the current working directory of this stage.
COPY --from=build /app/out .

# Copy self signed certificate and set environment variable for password
COPY cert.pfx .
ENV CERT_PASSWORD=

USER oshd_user

# Run the application
ENTRYPOINT ["dotnet", "Challenge.Application.API.dll"]