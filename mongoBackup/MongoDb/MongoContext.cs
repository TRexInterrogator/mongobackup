using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace mongoBackup.MongoDb {
    public class MongoContext : IDisposable {

        public IMongoClient client;
        public IMongoDatabase database;


        public MongoContext() {
            try {
                this.client = new MongoClient(AppData.Settings.connection_string);
                this.database = this.client.GetDatabase(AppData.Settings.database);
            }
            catch(Exception ex) {
                Console.WriteLine($"!** MongoDb Context error: {ex.Message} **!");
            }
        }

        public MongoContext(string database_name) {
            try {
                this.client = new MongoClient(AppData.Settings.connection_string);
                this.database = this.client.GetDatabase(database_name);
            }
            catch (Exception ex) {
                Console.WriteLine($"!** MongoDb Context error: {ex.Message} **!");
            }
        }



        public void Dispose() {

            this.client = null;
            this.database = null;
        }
    }
}
