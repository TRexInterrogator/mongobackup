# mongobackup
C# .NET Core MongoDb Backup Console Application

This application uses the mongo tools in order to create multiple database backups of the specified db's.

## Dependencies
In order for this application to work properly you need to have a mongoDb instance installed as well as the [mongoDatabaseTools](https://www.mongodb.com/try/download/database-tools).

## Runing the app
First you will need to open the latest release folder for your plattform `./release/*_latest`. 
Locate the executable and run it.

## Configuration
The first time you run the application a config file will be created (backupsettings.json).

Sample configuration for Windows:
```json
{
  "databases": [
    "toolsetapp",
    "defaultdb"
  ],
  "bash_location": "cmd.exe",
  "azureblob_enabled": false,
  "azureblob_settings": {
    "azureblob_endpoint": null,
    "azureblob_key": null,
    "azureblob_name": null
  }
}
```

Sample configuration for Linux:
```json
{
  "databases": [
    "toolsetapp",
    "defaultdb"
  ],
  "bash_location": "/bin/bash",
  "azureblob_enabled": false,
  "azureblob_settings": {
    "azureblob_endpoint": null,
    "azureblob_key": null,
    "azureblob_name": null
  }
}
```

## Azure blob storage
This application allows uploading the backup.zip files to an Azure blob storage container.
You will need to provide the neccessary connection details in order for this to work. Don't forget to set `"azureblob_enabled": true`!

## Windows Server automation (Task schedular)
**Create a new Task**

1. General
- Change `When running this task, use the following user account` to the user with permission to edit the file directory where your application files are located.
- Check `Run whether user is logged in or not`
- Check `Run with highest priviliges`
- Select `Configire for: Windows Server 2019`

2. Triggers
- Begin the task `On schedule`
- Select `Daily`
- Select start datetime
- Set `Recur every` (1 days)
- Check `Enabled`

3. Actions
- Action: `Start a program`
- Program /script: `C:\mongoBackup\mongoBackup.exe`
- Start in: `C:\mongoBackup`

4. Settings
- [x] Allow the task to be run on demand
- [x] Stop the task if it runs longer than `1 hour`
- [x] If the running task does not end when requested, force it to stop
- If the task is alread running: `Do not start a new instance`

