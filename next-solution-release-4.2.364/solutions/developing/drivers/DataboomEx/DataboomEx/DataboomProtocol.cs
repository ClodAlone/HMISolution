using DriverCodeBaseEx;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;

namespace Databoom
{
    public class DataboomProtocol
    {
        public enum DataboomErrorCodes : int
        {
            ErrorCodeErrorPreparingPublishMessage = 1000,
            ErrorCodeFailedGettingServiceDateTime = 1001,
            ErrorCodeErrorPublish = 1002,
            ErrorCodeNoInternetConnection = 1003,
            //ErrorCodeExceptionThreadSending = 1003,
        }

        public const int JOBS_AGGREGATION_LIMIT = 20;
        
        public class Credentials
        {
            // all properties name are case sensitive !!! don't change
            public string device { get; set; }
            public string date { get; set; }
            public List<Signal> signals { get; set; }

            public Credentials()
            {
                device = string.Empty;
                date = string.Empty;
                signals = new List<Signal>();
            }
        }

        public class Signal
        {
            public string name { get; set; }

            public string value { get; set; }
        }

        public class DataToSend
        {
            public string Message { get; set; }

            public DateTime CurrentExecutionTime { get; set; }

            public DataToSend()
            {
                Message = string.Empty;
                CurrentExecutionTime = DateTime.UtcNow;
            }

            public StringContent GetHttpContent()
            {
                return new StringContent(Message, Encoding.UTF8, "application/json");
            }
        };

        public class DataValue
        {            
            public DateTime SamplingTime { get; set; }

            public string Value { get; set; }

            public DateTime StartSampling { get; set; }
            public DateTime EndSampling { get; set; }

            public DataValue()
            {
                SamplingTime = DateTime.MinValue;                
                Value = string.Empty;                
            }

            public DataValue(Tag tag, DateTime dt)
            {
                SamplingTime = dt;
                if (tag == null)
                {
                    Value = String.Empty;
                }
                else
                {
                    if (((uint)tag.TagNode.DataType.Identifier) == ((uint)BuiltInType.Boolean))
                    {
                        if (tag.Value.Value.ToString() == "False")
                            Value = "0";
                        else
                            Value = "1";
                    }
                    else
                    {
                        Value = tag.Value.Value.ToString();
                    }
                }
            }
        };

        public static bool CheckInternetConnection()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var gatewayAddr in adapter.GetIPProperties().GatewayAddresses)
                {
                    // if gateway address is NOT 0.0.0.0 and the network card status is UP then we've found the main network card
                    if (gatewayAddr.Address.ToString() != "0.0.0.0" && adapter.OperationalStatus == OperationalStatus.Up)
                        return true;
                }
            }

            return false;
        }

        public static DateTime GetDateTimeUtcNowNoMSec()
        {
            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second, DateTime.UtcNow.Kind);
        }
    }
}

