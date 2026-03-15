using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Hosting;
using Opc.Ua;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Diagnostics;

namespace ServerSignalR.Simulator
{
    static public class Simulator
    {
        static uint numTags;
        static string pathDataFile;

        static DataValue[] dataValues;

        static public void Init(uint numtags, string pathdatafile)
        {
            numTags = numtags;
            pathDataFile = pathdatafile;

            dataValues = new DataValue[numTags];
            for(int i = 0; i < numTags; i++)
            {
                dataValues[i] = new DataValue(new Variant(i), Opc.Ua.StatusCodes.Good, DateTime.UtcNow);
            }
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            string jsonString = JsonSerializer.Serialize(dataValues);
            File.WriteAllText(pathDataFile, jsonString);
            stopwatch.Stop();
            Console.WriteLine("Elapsed Time serializing datasets {0} ms", stopwatch.ElapsedMilliseconds);
        }

        public static DataValue[] GetData()
        {
            return dataValues;
        }

        static public void Start()
        {
        }

        static public void Stop()
        {
        }
    }
}
