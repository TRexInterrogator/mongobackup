# Register service on linux

1. Create a directory where the service files should be located
2. Give your user access rights to this folder `sudo chown -R user:user foldername`
3. Copy the contents of the linux release version to this folder
4. Run the application `./mongoBackup`
5. Configure the `backupsettings.json` file


## Register service

1. Open and edit the `mongobackup.service` 
2. Replace `WorkingDirectory=` with your path to the folder you previously created
3. Copy the .service file to systemd `sudo cp mangobackup.service /etc/systemd/system/mongobackup.service`
4. Copy the .timer file to systemd `sudo cp mangobackup.service /etc/systemd/system/mongobackup.timer`
5. Enable the timer: `sudo systemctl enable mongobackup.timer`
6. Start the timer: `sudo systemctl start mongobackup.timer`

<br>

To check if the timer is enabled or running type:
```bash
# sudo systemctl is-enabled mongobackup.timer
enabled

# sudo systemctl is-active mongobackup.timer
active
```

If you want to change the backup schedule in **mongobackup.timer**, don't forget to reload the systemd deamon.
```bash
# sudo systemctl deamon-reload
```

<br>

You can list all timers by typing:
```bash
# sudo systemctl list-timers
NEXT                        LEFT          LAST                        PASSED       UNIT                         ACTIVATES                     
Tue 2020-08-18 08:06:10 UTC 1h 13min left Mon 2020-08-17 21:00:06 UTC 9h ago       fwupd-refresh.timer          fwupd-refresh.service         
               
Wed 2020-08-19 00:00:00 UTC 17h left      n/a                         n/a          mongobackup.timer            mongobackup.service  
```