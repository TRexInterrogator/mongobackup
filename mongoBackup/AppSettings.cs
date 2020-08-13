using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace mongoBackup {
    public class AppSettings {

        public List<string> databases { get; set; } = new List<string>();
        public string bash_location { get; set; } = "/bin/bash";
        public bool azureblob_enabled { get; set; } = false;
        public string system { get; set; } = "linux-docker";
        public AzureBlobSetting azureblob_settings { get; set; } = new AzureBlobSetting();
        
        
        private string _settings_path { get; set; } = "./backupsettings.json";


        public AppSettings() { }


        /// <summary>
        /// New AppSettings Class instance with settings file builder
        /// </summary>
        /// <param name="loading"></param>
        public AppSettings(bool loading) {
            if (loading) {
                try {
                    if (File.Exists(this._settings_path)) {
                        var data = File.ReadAllText(this._settings_path);

                        if (!string.IsNullOrEmpty(data)) {

                            var setting = JsonConvert.DeserializeObject<AppSettings>(data);
                            if (setting != null) {
                                this.databases = setting.databases;
                                this.bash_location = setting.bash_location;
                                this.azureblob_enabled = setting.azureblob_enabled;
                                this.azureblob_settings = setting.azureblob_settings;
                                this.system = setting.system;
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
            
            var loaded = false;

            if (!string.IsNullOrEmpty(this.bash_location)) loaded = true;


            if (this.azureblob_enabled) {
                if (this.azureblob_settings != null) {
                    if (!string.IsNullOrEmpty(this.azureblob_settings.azureblob_endpoint) && !string.IsNullOrEmpty(this.azureblob_settings.azureblob_key) && !string.IsNullOrEmpty(this.azureblob_settings.azureblob_name)) loaded = true;
                    else loaded = false;
                }
                else loaded = false;
            }

            return loaded;
        }
    }


    public class AzureBlobSetting {

        public string azureblob_endpoint { get; set; }
        public string azureblob_key { get; set; }
        public string azureblob_name { get; set; }
    }
}
