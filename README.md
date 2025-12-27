
## Setup Development Environment
```
docker compose down -v
docker compose up --build
```

## Migration
```
docker compose run api migrate
```

## Database Users
```
docker compose exec postgres psql -U redit -d redit_db
docker compose exec postgres psql -U redit_community_reader -d redit_db
```