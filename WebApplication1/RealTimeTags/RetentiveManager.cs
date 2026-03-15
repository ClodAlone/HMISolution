using RealTimeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTags
{
    internal class RetentiveManager : IDisposable
    {
        static readonly String retentiveCollectionTags = "tags";

        String PasswordRetentive;
        String RetentivePath;
        internal RetentiveManager(String retentivePath, String password)
        {
            RetentivePath = retentivePath;
            PasswordRetentive = password;
        }

        internal IEnumerable<Tag> ReadRetentive()
        {
            var dbsettings = new LiteDB.Engine.EngineSettings { Password = PasswordRetentive, Filename = RetentivePath };
            var dblite = new LiteDB.Engine.LiteEngine(dbsettings);
            using (var db = new LiteDB.LiteDatabase(dblite))
            {
                return db.GetCollection<Tag>(retentiveCollectionTags).FindAll();
            }
        }

        internal void UpdateRetentive(IEnumerable<Tag> tags)
        {
            var dbsettings = new LiteDB.Engine.EngineSettings { Password = PasswordRetentive, Filename = RetentivePath };
            var dblite = new LiteDB.Engine.LiteEngine(dbsettings);
            using (var db = new LiteDB.LiteDatabase(dblite))
            {
                var col = db.GetCollection<Tag>(retentiveCollectionTags);

                col.Upsert(tags);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
