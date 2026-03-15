using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Demo
{
    public class DemoProtocol
    {
        public enum DemoTypes : int
        {
            Sin = 0,
            Cos,
            Ramp,
            Random,
            SquareWave,
            UpDownCounter
        }
        
        public enum Direction : int
        {
            Undefined,
            Up,
            Down
        }

        public const uint SIMULATION_INTERVAL_DEFAULT_VALUE = 500;
        public const double SIN_NR_STEPS_PER_CYCLE = 60;

        public static DateTime GetDateTimeUtcNowNoMSec(int secondCorrect = 0)
        {
            DateTime dt = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second, DateTime.UtcNow.Kind);
            if (secondCorrect > 0)
                dt = dt.AddSeconds(secondCorrect);

            return dt;
        }

        public static bool ParseData(List<Tag> receivebuffer, ref DemoCommJob job, ref List<object> items)
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
           
            job.SetJobData(receivebuffer, ref changed);

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

        public static bool AreParametersInRange(UFUAModel.DataType type, int demoType, double minValue, double maxValue, double deltaValue, double factorSinCos)
        {
            return AreParametersInRange(GetBuiltInTypeFromDataType(type), demoType, minValue, maxValue, deltaValue, factorSinCos);
        }

        public static bool AreParametersInRange(uint identifier, int demoType, double minValue, double maxValue, double deltaValue, double factorSinCos)
        {
            bool result = false;

            switch ((DemoProtocol.DemoTypes)demoType)
            {
                case DemoTypes.Sin:
                case DemoTypes.Cos:
                    result = IsParameterInTagDataTypeRange(identifier, factorSinCos);
                    break;
                case DemoTypes.Random:
                case DemoTypes.SquareWave:
                    result = IsParameterInTagDataTypeRange(identifier, minValue) && IsParameterInTagDataTypeRange(identifier, maxValue);
                    break;

                case DemoTypes.Ramp:
                case DemoTypes.UpDownCounter:
                    result = IsParameterInTagDataTypeRange(identifier, minValue) && IsParameterInTagDataTypeRange(identifier, maxValue) && IsParameterInTagDataTypeRange(identifier, deltaValue);
                    break;
            }

            return result;
        }

        private static bool IsParameterInTagDataTypeRange(uint identifier, double value)
        {
            bool result = false;

            switch (identifier)
            {
                case 0: // struct
                    result = true;
                    break;
                case (uint)BuiltInType.Boolean:
                    {
                        result = true;// Boolean.TryParse(value.ToString(), out bool m1);                        
                        break;
                    }
                case (uint)BuiltInType.Byte:
                    {
                        result = Byte.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out byte m1);
                        break;
                    }
                case (uint)BuiltInType.SByte:
                    {
                        result = SByte.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out sbyte m1);
                        break;
                    }
                case (uint)BuiltInType.Int16:
                    {
                        result = Int16.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Int16 m1);
                        break;
                    }
                case (uint)BuiltInType.UInt16:
                    {
                        result = UInt16.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out UInt16 m1);
                        break;
                    }
                case (uint)BuiltInType.Int32:
                    {
                        result = Int32.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Int32 m1);
                        break;
                    }
                case (uint)BuiltInType.UInt32:
                    {
                        result = UInt32.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out UInt32 m1);
                        break;
                    }
                case (uint)BuiltInType.Int64:
                    {
                        result = Int64.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Int64 m1);
                        break;
                    }
                case (uint)BuiltInType.UInt64:
                    {
                        result = UInt64.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out UInt64 m1);
                        break;
                    }                    
                case (uint)BuiltInType.Float:
                    {
                        result = Single.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Single m1);
                        break;
                    }
                case (uint)BuiltInType.Double:
                    {
                        result = Double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Double m1);
                        break;
                    }
                case (uint)BuiltInType.String:
                    result = true;
                    break;
            }

            return result;
        }

        public static uint GetBuiltInTypeFromDataType(UFUAModel.DataType type)
        {
            switch (type)
            {
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
                case UFUAModel.DataType.SByte:
                    return (uint)BuiltInType.SByte;
                case UFUAModel.DataType.Int16:
                    return (uint)BuiltInType.Int16;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Int32:
                    return (uint)BuiltInType.Int32;
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.Int64:
                    return (uint)BuiltInType.Int64;
                case UFUAModel.DataType.UInt64:
                    return (uint)BuiltInType.UInt64;
                case UFUAModel.DataType.Float:
                    return (uint)BuiltInType.Float;
                case UFUAModel.DataType.Double:
                    return (uint)BuiltInType.Double;
                case UFUAModel.DataType.String:
                    return (uint)BuiltInType.String;
                default:
                    return 0;
            }
        }

        public static object CreateArrayFromBuiltInType(uint type, uint arraySize)
        {
            switch (type)
            {
                case (uint)BuiltInType.Boolean:
                    return new Boolean[arraySize];
                case (uint)BuiltInType.Byte:
                    return new Byte[arraySize];
                case (uint)BuiltInType.SByte:
                    return new SByte[arraySize];
                case (uint)BuiltInType.Int16:
                    return new Int16[arraySize];
                case (uint)BuiltInType.UInt16:
                    return new UInt16[arraySize];
                case (uint)BuiltInType.Int32:
                    return new Int32[arraySize];
                case (uint)BuiltInType.UInt32:
                    return new UInt32[arraySize];
                case (uint)BuiltInType.Int64:
                    return new Int64[arraySize];
                case (uint)BuiltInType.UInt64:
                    return new UInt64[arraySize];
                case (uint)BuiltInType.Float:
                    return new float[arraySize];
                case (uint)BuiltInType.Double:
                    return new Double[arraySize];
                case (uint)BuiltInType.String:
                    return new String[arraySize];
                default:
                    return new Boolean[arraySize];
            }
        }

        public static UFUAModel.DataType GetUFUAModelFromBuiltInType(uint type)
        {
            switch (type)
            {
                case (uint)BuiltInType.Boolean:
                    return UFUAModel.DataType.Boolean;
                case (uint)BuiltInType.Byte:
                    return UFUAModel.DataType.Byte;
                case (uint)BuiltInType.SByte:
                    return UFUAModel.DataType.SByte;
                case (uint)BuiltInType.Int16:
                    return UFUAModel.DataType.Int16;
                case (uint)BuiltInType.UInt16:
                    return UFUAModel.DataType.UInt16;
                case (uint)BuiltInType.Int32:
                    return UFUAModel.DataType.Int32;
                case (uint)BuiltInType.UInt32:
                    return UFUAModel.DataType.UInt32;
                case (uint)BuiltInType.Int64:
                    return UFUAModel.DataType.Int64;
                case (uint)BuiltInType.UInt64:
                    return UFUAModel.DataType.UInt64;
                case (uint)BuiltInType.Float:
                    return UFUAModel.DataType.Float;
                case (uint)BuiltInType.Double:
                    return UFUAModel.DataType.Double;
                case (uint)BuiltInType.String:
                    return UFUAModel.DataType.String;
                default:
                    return UFUAModel.DataType.Boolean;
            }
        }

        public static bool IsTagBoolean(Tag tag)
        {
            return (tag.TagNode.DataType.IdType == IdType.Numeric && (uint)tag.TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean);
        }

        public static bool IsDemoTypeCompatibleWithVarType(UFUAModel.DataType type, int demoType)
        {
            bool result = true;
            switch ((DemoProtocol.DemoTypes)demoType)
            {
                case DemoProtocol.DemoTypes.Sin:
                    result = (type != UFUAModel.DataType.Boolean);
                    break;
                case DemoProtocol.DemoTypes.Cos:
                    result = (type != UFUAModel.DataType.Boolean);
                    break;
                case DemoProtocol.DemoTypes.Ramp:
                    result = (type != UFUAModel.DataType.Boolean);
                    break;
                case DemoProtocol.DemoTypes.Random:
                    break;
                case DemoProtocol.DemoTypes.SquareWave:
                    break;
                case DemoProtocol.DemoTypes.UpDownCounter:
                    result = (type != UFUAModel.DataType.Boolean);
                    break;
            }

            return result;
        }
    }
}

