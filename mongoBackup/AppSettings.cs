using Newtonsoft.Json;
using System;
using System.IO;

namespace mongoBackup {
    public class AppSettings {

        public string database { get; set; } = string.Empty;
        public string connection_string { get; set; } = string.Empty;
        private string _settings_path { get; set; } = "./appsettings.json";


        public AppSettings() { }

        public AppSettings(bool loading) {
            if (loading) {
                try {
                    if (File.Exists(this._settings_path)) {
                        var data = File.ReadAllText(this._settings_path);

                        if (!string.IsNullOrEmpty(data)) {

                            var setting = JsonConvert.DeserializeObject<AppSettings>(data);
                            if (setting != null) {
                                this.database = setting.database;
                                this.connection_string = setting.connection_string;
                            }
                        }
                    }
                    else {
                        var data = JsonConvert.SerializeObject(this, Formatting.Indented);
                        File.WriteAllText(this._settings_path, data);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"!** AppSettings loading error: {ex.Message} **!");
                }
            }
        }


        /// <summary>
        /// Indicated if the settings have been loaded
        /// </summary>
        /// <returns>Boolean</returns>
        public bool Loaded() {
            return !string.IsNullOrEmpty(this.database) ? true : false;
        }
    }
}
