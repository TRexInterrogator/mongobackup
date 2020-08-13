# /bin/bash

# Create backup dump of database
docker exec mongodb mongodump --db $2 --out ./backups/$1
docker cp mongodb:./backups/$1 ./backups/

# Delete backup in container
docker exec mongodb rm -r ./backups/$1