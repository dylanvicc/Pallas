# Run commands from solution root.
# Build
```
docker build -t pallas -f Pallas.API/Dockerfile .
```
# Run Development Environment
```
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8080:8080 pallas
```
# Run Production Environment
```
docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8080:8080 pallas
```
# Swagger
```
http://localhost:8080/swagger
```