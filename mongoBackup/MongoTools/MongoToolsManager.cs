using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;


namespace mongoBackup.MongoTools {

    public class MongoToolsManager {

        protected static readonly string _backup_folder = "./backups";
        private string _database { get; set; }



        public MongoToolsManager() { }

        public MongoToolsManager(string database) {
            this._database = database;
        }



        public static void Init() {
            try {
                Directory.CreateDirectory(_backup_folder);
            }
            catch (Exception ex) {
                Console.WriteLine($"!** Failed MongoToolsManager init: {ex.Message} **!");
            }
        }



        /// <summary>
        /// Creates the backup file zip for the current database
        /// </summary>
        /// <returns>Path to backup zip file</returns>
        public string CreateBackup() {
            
            if (AppData.Settings.system == "windows") return this.CreateBackupWindowsServer();
            else if (AppData.Settings.system == "linux-docker") return this.CreateBackupLinuxDocker();
            else return string.Empty;
        }



        /// <summary>
        /// Created a backup on linux-docker configuration
        /// </summary>
        /// <returns>Path to backup zip file</returns>
        private string CreateBackupLinuxDocker() {

            var created_path = string.Empty;

            try {
                if (!string.IsNullOrEmpty(this._database) && !string.IsNullOrEmpty(AppData.Settings.bash_location)) {

                    var folder_id = $"{DateTime.UtcNow.ToString("dd-MM-yyyy")}-{Guid.NewGuid().ToString()}";

                    var mongoscript_command = $"bash ./scripts/create_mongobackup.sh {folder_id} {this._database}";
                    var escaped_agrs = mongoscript_command.Replace("\"", "\\\"");


                    var mongodump_process = new Process() {
                        StartInfo = new ProcessStartInfo {
                            FileName = AppData.Settings.bash_location,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = $"-c \"{escaped_agrs}\""
                        }
                    };

                    mongodump_process.Start();
                    mongodump_process.WaitForExit();


                    var zip_path = $"{_backup_folder}/{folder_id}.zip";
                    var folder_path = $"{_backup_folder}/{folder_id}";
                    ZipFile.CreateFromDirectory(folder_path, zip_path);

                    // Delete raw backup data
                    Directory.Delete(folder_path, true);

                    created_path = zip_path;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"!** Failed creating backup dump: {ex.Message} **!");
            }

            return created_path;
        }




        private string CreateBackupWindowsServer() {

            var created_path = string.Empty;

            try {
                if (!string.IsNullOrEmpty(this._database) && !string.IsNullOrEmpty(AppData.Settings.bash_location)) {

                    var folder_id = $"{DateTime.UtcNow.ToString("dd-MM-yyyy")}-{Guid.NewGuid().ToString()}";
                    var folder_path = $"{_backup_folder}/{folder_id}";

                    var mongodump_command = $"mongodump --db {this._database} --out {folder_path}";

                    var mongodump_process = new Process() {
                        StartInfo = new ProcessStartInfo {
                            FileName = AppData.Settings.bash_location,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = $"/C \"{mongodump_command}\""
                        }
                    };

                    mongodump_process.Start();
                    mongodump_process.WaitForExit();


                    var zip_path = $"{_backup_folder}/{folder_id}.zip";
                    ZipFile.CreateFromDirectory(folder_path, zip_path);

                    // Delete raw backup data
                    Directory.Delete(folder_path, true);

                    created_path = zip_path;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"!** Failed creating backup dump: {ex.Message} **!");
            }

            return created_path;
        }



        /// <summary>
        /// Deletes local backup file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool DeleteBackup(string path) {

            var deleted = false;

            try {
                if (File.Exists(path)) {

                    File.Delete(path);
                    deleted = true;
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"!** Failed deleting backup: {path} - {ex.Message} **!");
            }

            return deleted;
        }
    }
}