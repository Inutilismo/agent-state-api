# Simple Dockerfile - uses single image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# Copy project files
COPY . ./

# Restore dependencies and build
RUN dotnet restore
RUN dotnet build -c Release

EXPOSE 5000

# Run application directly
ENTRYPOINT ["dotnet", "run", "--project", ".", "--urls", "http://0.0.0.0:5000"]
