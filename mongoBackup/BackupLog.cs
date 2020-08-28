using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using mongoBackup.MongoTools;
using Newtonsoft.Json;

namespace mongoBackup {

    public class BackupLog {

        public string backup_path { get; set; }
        public string database { get; set; }
        public DateTime created { get; set; } = DateTime.UtcNow;
        public bool stored { get; set; } = true;

        private string _loglocation { get; set; } = "./backupLogs.json";


        public BackupLog() { }


        /// <summary>
        /// Creates a new backuplog file if not present
        /// </summary>
        /// <param name="init">Init needed?</param>
        public BackupLog(bool init) {

            if (init == true) {
                try {
                    if (!File.Exists(this._loglocation)) {

                        var json = JsonConvert.SerializeObject(new List<BackupLog>(), Formatting.Indented);
                        File.WriteAllText(this._loglocation, json);
                    }
                }
                catch (Exception ex) {
                    Console.Write($"Backup-Log INIT failed: {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Adds a new backup log item
        /// </summary>
        /// <param name="backupPath">Path to the file</param>
        /// <param name="dataBase">Name of database</param>
        public void Add(string backupPath, string dataBase) {

            try {   
                if (File.Exists(this._loglocation)) {
                    var log_list_json = File.ReadAllText(this._loglocation);

                    if (!string.IsNullOrEmpty(log_list_json)) {
                        var logs = JsonConvert.DeserializeObject<List<BackupLog>>(log_list_json);

                        if (logs != null) {
                            logs.Add(new BackupLog() { backup_path = backupPath, database = dataBase });
                            
                            var logs_json = JsonConvert.SerializeObject(logs, Formatting.Indented);
                            File.WriteAllText(this._loglocation, logs_json);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed adding new backuplog item: {ex.Message}");
            }
        }


        /// <summary>
        /// Returns all BackupLogs as list
        /// </summary>
        /// <returns>List of Backup logs</returns>
        public List<BackupLog> ToList() {

            var logs = new List<BackupLog>();

            try {
                if (File.Exists(this._loglocation)) {
                    var log_list_json = File.ReadAllText(this._loglocation);

                    if (!string.IsNullOrEmpty(log_list_json)) {

                        var converted_logs = JsonConvert.DeserializeObject<List<BackupLog>>(log_list_json);
                        if (converted_logs != null) logs = converted_logs;
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed listing all backup logs: {ex.Message}");
            }

            return logs;
        }


        /// <summary>
        /// Removes all old backup files from local storage and cloud
        /// </summary>
        public async Task<bool> RemoveOldBackupsAsync() {
            
            var finished = false;

            if (AppData.Settings.auto_delete.enabled) {
                var days = AppData.Settings.auto_delete.keep_last_x_days;
                var logs = await Task.Run(() => this.ToList());
                var date = DateTime.UtcNow;

                // Filter out all backups to keep
                var to_remove = logs.Where(x => x.stored).ToList();

                for (var d = 0; d < days; d++) {
                    to_remove.RemoveAll(x => x.created.Year == date.Year && x.created.Month == date.Month && x.created.Day == date.Day);
                    date.AddDays(-1);
                }

                // Loop remaining backups and delete them
                foreach(var log in to_remove) {

                    // Local
                    if (File.Exists(log.backup_path)) {
                        await Task.Run(() => File.Delete(log.backup_path));

                        log.stored = false;
                        logs[logs.FindIndex(x => x.backup_path == log.backup_path)] = log;
                    }

                    // Azure blob
                    if (AppData.Settings.azureblob_enabled) {
                        await AzureBlobStorage.DeleteBlobAsync(log.backup_path);
                    }
                }


                // Save updated backup logs
                var logs_json = await Task.Run(() => JsonConvert.SerializeObject(logs, Formatting.Indented));
                await File.WriteAllTextAsync(this._loglocation, logs_json);
                finished = true;
            }

            return finished;
        }
    }
}