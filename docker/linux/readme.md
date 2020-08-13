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
4. Enable the service `sudo systemctl enable mongobackup.service`
5. Start the service `sudo systemctl start mongobackup.service`
6. Check the service `sudo systemctl status mongobackup.service`

The mongobackup service should now be running each day and 5min after reboot.
