using mongoBackup.MongoDb;
using System;
using System.Threading.Tasks;

namespace mongoBackup {
    class Program {
        static async Task Main(string[] args) {

            Console.WriteLine("MongoDb Backup Utility" + Environment.NewLine);

            if (AppData.Settings.Loaded()) {

                // Swap comments on these lines in order to create or restore a backup
                await MongoDbBackup.CreateBackupAsync();
                //await MongoDbBackup.RestoreBackupAsync("backup-25-05-2020-d1f5cd77-6dd4-4586-9675-a46eb0db5ad6.json", "azurerestore");
            }
            else {
                Console.WriteLine("Settings have not been loaded. Please try again.");
            }
        }
    }
}
