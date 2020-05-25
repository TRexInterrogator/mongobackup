using System.Collections.Generic;


namespace mongoBackup.MongoDb {
    public class MongoDbCollection {

        public string name { get; set; }
        public List<string> elements { get; set; } = new List<string>();
    }
}
