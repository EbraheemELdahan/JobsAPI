# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy only the project file first (speeds cache)
COPY ["JobsAPI/JobsAPI.csproj", "JobsAPI/"]
RUN dotnet restore "JobsAPI/JobsAPI.csproj"

# copy everything and publish
COPY . .
RUN dotnet publish "JobsAPI/JobsAPI.csproj" -c Release -o /app --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "JobsAPI.dll"]