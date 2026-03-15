using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !NET_STANDARD
using System.Windows.Data;
#else
using Utilities.Converters;
#endif

namespace DataLoggerManager
{
    public class DataColumnValue : ICloneable
    {
#region Declarations
        readonly List<NodeId> nodeIds;
        readonly Array values;
        readonly Array sourceTimeStamps;
        readonly Array serverTimeStamps;
        readonly Array statusCodes;
        readonly IValueConverter converter;
#endregion

#region Constructors
        public DataColumnValue(NodeId nodeId) : 
            this(new List<NodeId>() { nodeId })
        { }

        public DataColumnValue(IList<NodeId> nodes) :
            this(nodes, null)
        { }

        public DataColumnValue(IList<NodeId> nodes, IValueConverter valueConverter)
        {
            nodeIds = new List<NodeId>(nodes);
            converter = valueConverter;
            values = Array.CreateInstance(typeof(object), nodeIds.Count);
            sourceTimeStamps = Array.CreateInstance(typeof(DateTime), nodeIds.Count);
            serverTimeStamps = Array.CreateInstance(typeof(DateTime), nodeIds.Count);
            statusCodes = Array.CreateInstance(typeof(uint), nodeIds.Count);
            for (int ii = 0; ii < statusCodes.Length; ii++)
                statusCodes.SetValue(StatusCodes.Uncertain, ii);
        }
#endregion

#region Properties

        public object Value
        {
            get
            {
                if (values.Length == 0)
                    return null;
                else if (values.Length == 1)
                {
                    var value = values.GetValue(0);
                    if (value is Array)
                        return new Variant(value).ToString();
                    else
                        return values.GetValue(0);
                }
                else
                    return new Variant(values).ToString(null, CultureInfo.InvariantCulture);
            }
        }

        public object SourceTimeStamp
        {
            get
            {
                if (sourceTimeStamps.Length == 0)
                    return null;
                else if (sourceTimeStamps.Length == 1)
                    return sourceTimeStamps.GetValue(0);
                else
                    return new Variant(sourceTimeStamps).ToString(null, CultureInfo.InvariantCulture);
            }
        }

        public object ServerTimeStamp
        {
            get
            {
                if (serverTimeStamps.Length == 0)
                    return null;
                else if (serverTimeStamps.Length == 1)
                    return serverTimeStamps.GetValue(0);
                else
                    return new Variant(serverTimeStamps).ToString(null, CultureInfo.InvariantCulture);
            }
        }

        public object StatusCode
        {
            get
            {
                if (statusCodes.Length == 0)
                    return null;
                else if (statusCodes.Length == 1)
                    return statusCodes.GetValue(0);
                else
                    return new Variant(statusCodes).ToString(null, CultureInfo.InvariantCulture);
            }
        }
#endregion

#region Methods
        public bool UpdateValue(NodeId nodeId, DataValue newValue, Boolean serverTimeStampEnabled, Boolean sourceTimeStampEnabled, Boolean statusEnabled)
        {
            var index = nodeIds.IndexOf(nodeId);
            if (index != -1)
            {
                var prevValue = values.GetValue(index);
                var prevSourceTimeStamp = sourceTimeStamps.GetValue(index);
                var prevServerTimeStamp = serverTimeStamps.GetValue(index);
                var prevStatusCode = statusCodes.GetValue(index);

                var wrappedValue = newValue.WrappedValue;
                if (converter != null)
                {
                    try
                    {
                        bool isNumeric = TypeInfo.IsNumericType(wrappedValue.TypeInfo.BuiltInType);
                        Type targetType = isNumeric ? typeof(Double) : typeof(String);
                        var exprValue = converter.Convert(wrappedValue.Value, targetType, null, null);
                        if (isNumeric)
                            wrappedValue.Value = Convert.ToDouble(exprValue);
                        else
                            wrappedValue.Value = exprValue;
                    }
                    catch (Exception ex)
                    {
                        throw new ExpressionValueConverterException("See the inner exception for more details.", ex);
                    }

                    prevSourceTimeStamp = newValue.SourceTimestamp;
                    prevServerTimeStamp = newValue.ServerTimestamp;
                }

                values.SetValue(wrappedValue.Value, index);
                sourceTimeStamps.SetValue(newValue.SourceTimestamp, index);
                serverTimeStamps.SetValue(newValue.ServerTimestamp, index);
                statusCodes.SetValue(newValue.StatusCode.Code, index);

                return !object.Equals(prevValue, wrappedValue.Value) ||
                    (DateTime)prevSourceTimeStamp != newValue.SourceTimestamp && sourceTimeStampEnabled ||
                    (DateTime)prevServerTimeStamp != newValue.ServerTimestamp && serverTimeStampEnabled ||
                    (uint)prevStatusCode != newValue.StatusCode.Code && statusEnabled;
            }

            return false;
        }

        public bool Exists(NodeId nodeId)
        {
            return nodeIds.IndexOf(nodeId) != -1;
        }

        /// <summary>
        /// Return a flag which inform if the quality is good. In case the statusCodes is an array
        /// with length > 0, the quality is good only if all the status codes are good.
        /// </summary>
        /// <returns>True if quality is good, false otherwise</returns>
        public bool IsQualityGood()
        {
            foreach (var statusCode in statusCodes) 
            {
                try
                {
                    if (UInt64.Parse(statusCode.ToString()) != StatusCodes.Good)
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
#endregion

#region ICloneable
        public object Clone()
        {
            var ret = new DataColumnValue(nodeIds);
            Array.Copy(values, ret.values, nodeIds.Count);
            Array.Copy(sourceTimeStamps, ret.sourceTimeStamps, nodeIds.Count);
            Array.Copy(serverTimeStamps, ret.serverTimeStamps, nodeIds.Count);
            Array.Copy(statusCodes, ret.statusCodes, nodeIds.Count);

            return ret;
        }
#endregion
    }
}
