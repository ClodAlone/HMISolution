using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;

namespace MTConnect
{
    public class MTConnectProtocol
    {
        public enum MTConnectErrorCodes : int
        {
            //ErrorCodeErrorPreparingPublishMessage = 1000,
            //ErrorCodeFailedGettingServiceDateTime = 1001,
            //ErrorCodeErrorPublish = 1002,
            ErrorCodeNoInternetConnection = 1000,
            ErrorCodeEthernetCardConnection = 1001,
            ErrorCodeConvertValue = 1003,
            ErrorCodeTagNoPresent = 1004,
        }
        public enum Category_E :int
        {
            Sample,
            Event,
            Condition,
            None
        }
        public enum FormatDataType_E : int
        {
            Double,
            DWord,
            Signed_DWord,
            String
        }

        public const int MaxItemForMessage = 50;
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
        public static Category_E GetCategory(string cat)
        {
            MTConnectProtocol.Category_E catType = MTConnectProtocol.Category_E.Event;
            if (String.IsNullOrWhiteSpace(cat))
            {
                return catType;
            }
            cat = cat.ToUpper();
            switch (cat)
            {
                case "SAMPLE":
                    catType =Category_E.Sample;
                    break;
                case "CONDITION":
                    catType = Category_E.Condition;
                    break;
                case "EVENT":
                    catType = Category_E.Event;
                    break;
                default:
                    catType = Category_E.None;
                    break;
            }
            return catType;
        }
        public static FormatDataType_E GetFormatMTConnect(string dt)
        {
            MTConnectProtocol.FormatDataType_E formType = MTConnectProtocol.FormatDataType_E.String;
            if (String.IsNullOrWhiteSpace(dt))
            {
                return formType;
            }
            dt = dt.ToLower();
            switch (dt)
            {
                case "double":
                    formType = MTConnectProtocol.FormatDataType_E.Double;
                    break;
                case "signed_dword":
                    formType = MTConnectProtocol.FormatDataType_E.Signed_DWord;
                    break;
                case "dword":
                    formType = MTConnectProtocol.FormatDataType_E.DWord;
                    break;
                default:
                    formType = MTConnectProtocol.FormatDataType_E.String;
                    break;
            }
            return formType;
        }
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

        public static bool ParseData(byte[] receivebuffer, ref MTConnectCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
            {
                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                items[0] = DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverErrorCodes.ErrorNoError;
            }

            byte[] tempBuffer = new byte[receivebuffer.Length];

            Array.Copy(receivebuffer, tempBuffer, receivebuffer.Length);

            job.SetJobData(tempBuffer, ref changed);

            if (areArguments)
            {
                if (job.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < job.TagsList.Count; k++)
                    {
                        items[k + 1] = job.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }

            return true;
        }
    }
}

