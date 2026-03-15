using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace MQTTClient
{
    public enum QualityOfServiceLevels : byte
    {
        AtMostOnce_0,
        AtLeastOnce_1,
        ExactlyOnce_2
    }

    public enum JSonTimestampFormats : byte
    {
        tf_ISO,
        tf_en_US,
        tf_it_IT,
        tf_EPOCH
    }

    public class MQTTClientProtocol
    {

        public static uint ParseData(string receivedValue, ref MQTTClientCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool writeOperation = false;
            //if ((job.Status == MQTTClientCommJobStatus.PublishReplyReceived) ||
            //   (job.Status == MQTTClientCommJobStatus.PublishError))
            if(String.IsNullOrWhiteSpace(receivedValue))
            {
                writeOperation = true;
            }

            if (writeOperation)
            {
                // Done
                return (StatusCodes.Good);
            }

            uint returnValue = job.SetJobValue(receivedValue, ref changed);
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ParseData 1 - Job {1} - processed subscription data for var {2}, Json Path = {3}, returnValue = {4}, receivedValue = {5}",
                                               currentTime, job.TagsList[0].TagNode.NodeId, job.TagName, job.JsonMessageFormat, returnValue, receivedValue));
#endif
            items.AddRange(changed);

            return (returnValue);
        }

        public static bool ParseData(byte[] receivebuffer, ref MQTTClientCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            bool writeOperation = false;
            if ((job.Status == MQTTClientCommJobStatus.PublishReplyReceived) ||
               (job.Status == MQTTClientCommJobStatus.PublishError))
            {
                writeOperation = true;
            }

            if (areArguments)
            {
                if(items.Count == 0)
                {
                    return (false);
                }

                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger && bt != BuiltInType.String)
                {
                    return (false);
                }

                if (items.Count < (writeOperation ? 1 : 2))
                {

                    items[0] = 11; // Invalid number of arguments
                    return (false);
                }
                else
                {
                    items[0] = 0; // No error
                }
            }

            if (writeOperation)
            {
                // Done
                return (true);
            }

            job.SetJobData(receivebuffer, ref changed);
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ParseData 2 - Job {1} - processed subscription data for var {2}",
                                               currentTime, job.TagsList[0].TagNode.NodeId, job.TagName));
#endif
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

            return (true);
        }

    }
}

