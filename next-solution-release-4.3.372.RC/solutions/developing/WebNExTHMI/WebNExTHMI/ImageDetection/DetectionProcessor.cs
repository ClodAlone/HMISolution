using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;

namespace WebNExTHMI.ImageDetection
{
    public class DetectionProcessStatus
    {
        public bool isOnTimeout;
        public int nProcessedImages;
        public int nExpiredImages;
        public int nLostImages;
        public String lastError;

        public double lastExecutionProcessedMs;
        public String lastSentImageTime;
    }

    public static class DetectionProcessor
    {
        static RequestSocket reqSocket = new RequestSocket(); //Sender (send request)
        static ResponseSocket resSocket = new ResponseSocket(); //Receiver (receive response)
        static PublisherSocket pubSocket = new PublisherSocket(); //Sender (publish message)
        static SubscriberSocket subSocket = new SubscriberSocket(); //Receiver (take message)

        public class ImageReq
        { 
            //Image received from the client
            byte[] image_;
            string connectionId_;
            int id_;
            string pageId_;
            IClientProxy _caller;
            DateTime creationTime;

            public ImageReq(byte[] image, string connectionId, IClientProxy caller, int id, string pageId)
            {
                image_ = image;
                connectionId_ = connectionId;
                id_ = id;
                pageId_ = pageId;
                _caller = caller;
                creationTime = DateTime.UtcNow;
            }

            public ImageReq(ImageReq im)
            {
                image_ = im.image();
                connectionId_ = im.connectionId();
                id_ = im.id();
                pageId_ = im.pageId();
            }
            public int id()
            {
                return id_;
            }
            public string connectionId()
            {
                return connectionId_;
            }
            public byte[] image()
            {
                return image_;
            }
            public string pageId()
            {
                return pageId_;
            }

            public bool IsExpired()
            {
                var currTime = DateTime.UtcNow;
                var timespan = currTime - creationTime;

                return timespan.TotalSeconds > PlatformComponents.PlatformComponents.TensorFlowTimeoutSec;
            }
        };

        static Dictionary<String, ImageReq> imageQueueClients = new Dictionary<String, ImageReq>();
        static Dictionary<String, IClientProxy> CallerIds = new Dictionary<String, IClientProxy>();
        static Dictionary<String, DetectionProcessStatus> mapStatus = new Dictionary<String, DetectionProcessStatus>();
        static AutoResetEvent mut_clients = new AutoResetEvent(false);

        static internal void RemoveImageProcessing(String connectionId)
        {
            lock (CallerIds)
            {
                if (CallerIds.ContainsKey(connectionId))
                    CallerIds.Remove(connectionId);
            }
            lock (imageQueueClients)
            {
                if (imageQueueClients.ContainsKey(connectionId))
                    imageQueueClients.Remove(connectionId);
            }
            lock(mapStatus)
            {
                if (mapStatus.ContainsKey(connectionId))
                    mapStatus.Remove(connectionId);
            }
        }

        static internal DetectionProcessStatus ProcessImage(byte[] binData, String connectionId, IClientProxy caller, int index, string pageId)
        {
            DetectionProcessStatus ret = null;
            lock (mapStatus)
            {
                if (!mapStatus.ContainsKey(connectionId))
                    mapStatus.Add(connectionId, new DetectionProcessStatus());
                ret = mapStatus[connectionId];
            }

            try
            {
                var sentTime = DateTime.Parse(ret.lastSentImageTime);
                ret.isOnTimeout = DateTime.UtcNow - sentTime > defaultTimeout;
            }
            catch { }

            Init(ret);

            lock (CallerIds)
            {
                if (!CallerIds.ContainsKey(connectionId))
                    CallerIds.Add(connectionId, caller);
            }

            var temp_req = new ImageReq(binData, connectionId, caller, index, pageId);
            lock (imageQueueClients)
            {
                if (imageQueueClients.ContainsKey(connectionId))
                {
                    imageQueueClients.Remove(connectionId);
                    ret.nLostImages++;
                }
                imageQueueClients.Add(connectionId, temp_req);
            }

            mut_clients.Set();

            /////////////////////////////////////////////////////////////////////////////////////
            // SIMULATION
            //var detectedObject = new JObject();

            //var screen = PlatformComponents.PlatformComponents.GetConfigurationStartupScreen();

            //detectedObject.Add("screen", screen);
            //detectedObject.Add("class_name", screen);

            //if (++demopos.Xmax > 300)
            //    demopos.Xmax = 200;
            //if (++demopos.Ymax > 300)
            //    demopos.Ymax = 200;

            //detectedObject.Add("coordinates", JToken.FromObject(demopos));

            //caller.SendAsync("ImageProcessed", detectedObject.ToString());
            //////////////////////////////////////////////////////////////////////////////////////
            ///
            return ret;
        }

        //struct coordinates
        //{
        //    public int Xmin;
        //    public int Ymin;
        //    public int Xmax;
        //    public int Ymax;
        //}

        //static coordinates demopos = new coordinates()
        //{
        //    Xmin = 100,
        //    Ymin = 100,
        //    Xmax = 200,
        //    Ymax = 200
        //};

        static bool bInit;
        static TimeSpan defaultTimeout;
        static void Init(DetectionProcessStatus status)
        {
            if (bInit)
                return;
            bInit = true;

            defaultTimeout = TimeSpan.FromSeconds(PlatformComponents.PlatformComponents.TensorFlowTimeoutSec);

            try
            {
                reqSocket.Connect(PlatformComponents.PlatformComponents.TensorFlowConnectTo); //To send images to tensorflow
                resSocket.Bind(PlatformComponents.PlatformComponents.TensorFlowBindTo); //To receive response from tensorflow
            }
            catch(Exception e)
            {
                status.lastError = e.Message;
                return;
            }

            Task t1 = Task.Run(() => ProcessImgQueue());
            Task t2 = Task.Run(() => ProcessResQueue());
        }

        /***Every 30 ms this task control if there is an image in the image queue,
        takes the first element and pushes it to the server, then waits the ack.***/
        static void ProcessImgQueue()
        {
            while (true)
            {
                mut_clients.WaitOne();

                Dictionary<String, ImageReq> tempimageQueueClients = null;
                lock (imageQueueClients)
                {
                    tempimageQueueClients = new Dictionary<string, ImageReq>(imageQueueClients);
                    imageQueueClients.Clear();
                }

                foreach(var pair in tempimageQueueClients)
                {
                    DetectionProcessStatus ret = null;
                    lock (mapStatus)
                    {
                        if (!mapStatus.ContainsKey(pair.Key))
                            mapStatus.Add(pair.Key, new DetectionProcessStatus());
                        ret = mapStatus[pair.Key];
                    }

                    var value = pair.Value;
                    lock (imageQueueClients)
                    {
                        if (imageQueueClients.ContainsKey(pair.Key))
                        {
                            value = imageQueueClients[pair.Key];
                            imageQueueClients.Remove(pair.Key);
                            ret.nLostImages++;
                        }
                    }

                    if (!value.IsExpired())
                    {
                        try
                        {
                            ret.lastSentImageTime = DateTime.UtcNow.ToString();

                            reqSocket.SendMoreFrame(value.image())
                                  .SendMoreFrame(value.connectionId())
                                  .SendFrame(value.pageId());
                            //wait the acknowledge from tensorflow
                            var ackRes = reqSocket.ReceiveFrameString();
                        }
                        catch (Exception e)
                        {
                            ret.lastError = e.Message;
                            ret.isOnTimeout = true;
                            ret.nLostImages++;
                        }
                    }
                    else
                        ret.nExpiredImages++;
                }
            }
        }

        /***Every 30 ms this task ask to the server a response. The response that come from the server
        is inserted into the response queue, and this client sends an ack to the server.***/
        static void ProcessResQueue()
        {
            while (true)
            {
                //File.AppendAllText("log.txt", "Waiting for JSON from python... \n"); 
                //wait response from tensorflow

                var reply = resSocket.ReceiveMultipartStrings();
                //File.AppendAllText("log.txt", "JSON: \n\n"); 
                //File.AppendAllText("log.txt", "JSON: " + reply[0] + "\n" + "CLIENT" + reply[1] + "\n\n"); 
                resSocket.SendFrame("Response received ACK");

                var connectionId = reply[1];
                IClientProxy caller = null;
                lock (CallerIds)
                {
                    if (CallerIds.ContainsKey(connectionId))
                        caller = CallerIds[connectionId];
                }

                if (caller != null)
                {
                    var detectedObject = JObject.Parse(reply[0]);

                    foreach (var o in detectedObject["detections"])
                    {
                        var classname = (string)o["class_name"];

                        var screen = PlatformComponents.PlatformComponents.GetConfigurationDetectedScreen(classname);
                        o["screen"] = screen;
                    }

                    //foreach (JObject o in detectedObject.Children<JObject>())
                    //{
                    //    var classname = (string)o["class_name"];

                    //    var screen = PlatformComponents.PlatformComponents.GetConfigurationDetectedScreen(classname);
                    //    o.Add("screen", screen);

                    //    /*
                    //    foreach (JProperty p in o.Properties())
                    //    {
                    //        string name = p.Name;
                    //        string value = (string)p.Value;
                    //        Console.WriteLine(name + " -- " + value);
                    //    }
                    //    */
                    //}

                    /*
                    var num_detections = (int)detectedObject["num_detections"];
                    for (int i = 0; i < num_detections; ++i)
                    {
                        detections

                        JToken
                        // TODO
                        var classname = (string)detectedObject["class_name"];

                        var screen = PlatformComponents.PlatformComponents.GetConfigurationDetectedScreen(classname);
                        detectedObject.Add("screen", screen);
                    }
                    */

                    caller.SendAsync("ImageProcessed", detectedObject.ToString());
                }

                DetectionProcessStatus ret = null;
                lock (mapStatus)
                {
                    if (mapStatus.ContainsKey(connectionId))
                        ret = mapStatus[connectionId];
                }
                if (ret != null)
                {
                    try
                    {
                        var sentTime = DateTime.Parse(ret.lastSentImageTime);
                        ret.lastExecutionProcessedMs = (sentTime - DateTime.UtcNow).TotalMilliseconds;
                    }
                    catch { }
                }
            }
        }
    }
}
