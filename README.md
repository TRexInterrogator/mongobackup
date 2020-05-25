# mongobackup
Simple C# MongoDb Backup Console Application

This application exports all data inside of a mongodb database into one single .json File.
You can use this File to restore a backup or clone it into a new database.
Use the .appsettings file to configure the target database for backups.

Other usecases are possible as well =>
- run the application with windows server task scheduler
- upload the backup file to a blob storage
- export the database data for BI´s / data science