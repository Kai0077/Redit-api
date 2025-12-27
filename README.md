
## Setup Development Environment
```
docker compose down -v
docker compose up --build
```

## Migration
```
docker compose run api migrate
```