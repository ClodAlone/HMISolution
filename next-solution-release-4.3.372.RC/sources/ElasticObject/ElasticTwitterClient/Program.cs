using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AmazedSaint.Elastic;
using System.Xml.Linq;
using System.IO;

namespace ElasticTwitterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient cl=new WebClient();
            Console.WriteLine("Reading public time line");
            using (StreamReader r = new StreamReader
                (cl.OpenRead(@"http://twitter.com/statuses/user_timeline/amazedsaint.xml")))
            {
                var data = r.ReadToEnd();
                IterateTweets(data);
            }
            Console.ReadLine();

        }

        static void IterateTweets(string data)
        {
            dynamic root = XElement.Parse(data).ToElastic();
            foreach (var s in root["status"])
            {
                Console.WriteLine(~s.user.screen_name + " - " + ~s.text);
                Console.WriteLine();
            }
        }
    }
}
