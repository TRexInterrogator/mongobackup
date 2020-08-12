# /bin/bash

# Create backup dump of database
docker exec mongodb mongodump --db sample --out ./backups/sampledb
docker cp mongodb:./backups/sampledb ./backups/

# Delete backup in container
docker exec mongodb rm -r ./backups/sampledb