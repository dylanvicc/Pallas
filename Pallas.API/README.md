# Run commands from solution root.
# Build
```
docker build -t pallas-api .
```
# Run Development Environment
```
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8080:8080 pallas-api
```
# Run Production Environment
```
docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8080:8080 pallas-api
```
# Swagger
```
http://localhost:8080/swagger
```