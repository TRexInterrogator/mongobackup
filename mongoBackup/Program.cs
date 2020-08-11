using mongoBackup.MongoTools;
using System;
using System.Threading.Tasks;


namespace mongoBackup {

    class Program {
        static void Main(string[] args) {

            Console.WriteLine("MongoDb Backup Task" + Environment.NewLine);

            if (AppData.Settings.Loaded()) {
                
                MongoToolsManager.Init();

                foreach (var db in AppData.Settings.databases) {
                    if (!string.IsNullOrEmpty(db)) {

                        Console.WriteLine($"Start backup for: {db}");

                        var mongotools = new MongoToolsManager(db);
                        var backup_path = mongotools.CreateBackup();

                        if (!string.IsNullOrEmpty(backup_path)) {

                            if (AppData.Settings.azureblob_enabled) {
                                if (Task.Run(async () => await AzureBlobStorage.UploadAsync(backup_path)).Result) Console.WriteLine("Finished and uploaded backup");
                            }
                            else {
                                Console.WriteLine("Created backup locally");
                            }
                        }
                        else {
                            Console.WriteLine("!** Failed backup. Backup file not found. **!");
                        }
                    }
                }
            }
            else {
                Console.WriteLine("Settings have not been loaded. Please try again.");
            }

            Console.WriteLine("FINISHED all backup tasks!");
        }
    }
}
