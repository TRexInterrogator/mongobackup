using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace mongoBackup.MongoDb {
    public class MongoDbBackup {

        public DateTime datetime { get; set; } = DateTime.UtcNow;
        public List<MongoDbCollection> collections { get; set; } = new List<MongoDbCollection>();

        public MongoDbBackup() { }


        /// <summary>
        /// Creates a full backup of the database
        /// </summary>
        public static async Task CreateBackupAsync() {
            try {
                var backup = new MongoDbBackup();

                using (var db = new MongoContext()) {
                    foreach (var collection in await db.database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>()) {
                        Console.WriteLine($"Collection: {collection["name"].AsString}");

                        backup.collections.Add(new MongoDbCollection() {
                            name = collection["name"].AsString
                        });
                    }

                    foreach (var collection in backup.collections) {
                        var dbCollection = db.database.GetCollection<BsonDocument>(collection.name);
                        var items = await dbCollection.Find(new BsonDocument()).ToListAsync();

                        items.ForEach((i) => collection.elements.Add(i.ToString()));
                    }
                }

                var data = await Task.Factory.StartNew(() => Newtonsoft.Json.JsonConvert.SerializeObject(backup, Newtonsoft.Json.Formatting.Indented));
                var filename = $"./backup-{DateTime.UtcNow.ToString("dd-MM-yyy")}-{Guid.NewGuid()}.json";
                await File.WriteAllTextAsync(filename, data);
            }
            catch (Exception ex) {
                Console.WriteLine($"!** Failed creating backup: {ex.Message} **!");
            }
        }


        /// <summary>
        /// Restores database backup into given database
        /// </summary>
        /// <param name="filename">Backup file</param>
        /// <param name="database_name">Database name</param>
        public static async Task RestoreBackupAsync(string filename, string database_name) {
            try {
                if (!string.IsNullOrEmpty(filename)) {
                    if (File.Exists(filename)) {

                        var backupData = await File.ReadAllTextAsync(filename);
                        if (!string.IsNullOrEmpty(backupData)) {

                            var backup = await Task.Factory.StartNew(() => Newtonsoft.Json.JsonConvert.DeserializeObject<MongoDbBackup>(backupData));
                            using (var db = new MongoContext(database_name)) {

                                foreach (var collection in backup.collections) {
                                    var dbCollection = db.database.GetCollection<BsonDocument>(collection.name);

                                    foreach (var element in collection.elements) {
                                        var dbElement = BsonSerializer.Deserialize<BsonDocument>(element);
                                        await dbCollection.InsertOneAsync(dbElement);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"!** Error: {ex.Message} **!");
            }
        }
    }
}
