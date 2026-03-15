using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Hosting;
using Opc.Ua;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Diagnostics;
using WebApplication1.Simulator;

namespace ServerSignalR.Simulator
{
    static public class Simulator
    {
        static uint numTags;
        static string pathDataFile;

        static Tag[] tags;

        static public void Init(uint numtags, string pathdatafile)
        {
            numTags = numtags;
            pathDataFile = pathdatafile;

            tags = new Tag[numTags];
            for(int i = 0; i < numTags; i++)
            {
                tags[i] = new Tag();
                tags[i].id = i;
                tags[i].Value = new DataValue(new Variant(i), Opc.Ua.StatusCodes.Good, DateTime.UtcNow);
            }
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            string jsonString = JsonSerializer.Serialize(tags);
            Directory.CreateDirectory(Path.GetDirectoryName(pathDataFile));
            File.WriteAllText(pathDataFile, jsonString);
            stopwatch.Stop();
            Console.WriteLine("Number of Tags {0}, Memory footprint {1} kb", tags.Length, jsonString.Length / 1024);
            Console.WriteLine("Elapsed Time serializing datasets {0} ms", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var dbsettings = new LiteDB.Engine.EngineSettings { Filename = pathDataFile + ".db" };
            var dblite = new LiteDB.Engine.LiteEngine(dbsettings);
            // Open database (or create if doesn't exist)
            using (var db = new LiteDB.LiteDatabase(dblite))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Tag>("tags");

                col.Upsert(tags);
                //for (int i = 0; i < numTags; i++)
                //{
                //    dataValues[i] = new DataValue(new Variant(i), Opc.Ua.StatusCodes.Good, DateTime.UtcNow);
                //    col.Upsert(i, dataValues[i]);
                //}
            }
            stopwatch.Stop();
            Console.WriteLine("Elapsed Time updating liteDB {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public static string GetData()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var ret = JsonSerializer.Serialize(tags);
            stopwatch.Stop();
            Console.WriteLine("Time to serialize datasets {0} ms", stopwatch.ElapsedMilliseconds);

            return ret;
        }

        static public void Start()
        {
        }

        static public void Stop()
        {
        }
    }
}
