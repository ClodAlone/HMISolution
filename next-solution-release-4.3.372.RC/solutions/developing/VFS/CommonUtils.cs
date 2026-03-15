using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Globalization;

namespace VFS
{
    public static class CommonUtils
    {
        public const string CustomJSPropertyPrefix = "cp";
        private static readonly object ClientDateFormatInfoKey = new object();
        public static bool AreEqualsArrays(string[] array1, string[] array2)
        {
            if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
        public static bool AreEqual(object value1, object value2, bool convertEmptyStringToNull)
        {
            bool equal = false;
            if (convertEmptyStringToNull)
            {
                equal = (value1 == null && value2 == null ||
                    value1 != null && value1.ToString() == "" && value2 == null ||
                    value1 == null && value2 != null && value2.ToString() == "" ||
                    value1 != null && value2 != null && value1.ToString() == "" && value2.ToString() == "");
            }
            return equal || AreEqual(value1, value2);
        }
        private static bool AreEqual(object value1, object value2)
        {
            return Object.Equals(value1, value2) ||
                value1 is ValueType && value2 is ValueType && value1.Equals(value2);
        }
        public static object ConvertToType(object value, Type type, bool positive)
        {
            if (type == null)
                return value;
            object result = GetDefaultValue(type, positive);
            try
            {
                result = CommonUtils.GetConvertedArgumentValue(value, type, "");
            }
            catch
            {
            }
            return result;
        }
        public static object GetDefaultValue(Type type, bool positive)
        {
            object result = null;
            TypeCode code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Boolean:
                    result = positive ? true : false;
                    break;
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Int32:
                    result = positive ? 1 : 0;
                    break;
                case TypeCode.String:
                    result = positive ? "True" : "";
                    break;
                case TypeCode.DateTime:
                    result = DateTime.Now;
                    break;
            }
            return result;
        }
        public static string GetObjectText(object obj)
        {
            return GetObjectText(obj, false);
        }
        public static string GetObjectText(object obj, bool includeSubObjects)
        {
            string res = string.Empty;
            try
            {
                PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
                foreach (PropertyDescriptor pd in coll)
                {
                    if (!pd.IsBrowsable || pd.SerializationVisibility == DesignerSerializationVisibility.Hidden)
                        continue;
                    if (pd.SerializationVisibility == DesignerSerializationVisibility.Content)
                    {
                        if (includeSubObjects)
                        {
                            object val = pd.GetValue(obj);
                            string s = (val != null) ? val.ToString() : string.Empty;
                            if (!string.IsNullOrEmpty(s))
                            {
                                if (res.Length > 0)
                                    res += ", ";
                                res += string.Format("{0} = {{ {1} }}", pd.Name, s);
                            }
                        }
                    }
                    else if (!pd.IsReadOnly && pd.ShouldSerializeValue(obj))
                    {
                        if (res.Length > 0)
                            res += ", ";
                        res += pd.Name;
                        object val = pd.GetValue(obj);
                        if (pd.PropertyType.Equals(typeof(string)))
                            res += string.Format(" = '{0}'", val);
                        else
                            res += string.Format(" = {0}", val);
                    }
                }
            }
            catch
            {
            }
            return res;
        }
        #region B132204: String Array Deserialization
        private const char SerializedStringArraySeparator = '|';
        public static string SerializeStringArray(string[] array)
        {
            if (array == null || array.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                string item = array[i];
                sb.Append(item.Length);
                sb.Append(SerializedStringArraySeparator);
                sb.Append(item);
            }
            return sb.ToString();
        }
        public static List<string> DeserializeStringArray(string serializedData)
        {
            List<string> items = new List<string>();
            if (!string.IsNullOrEmpty(serializedData))
            {
                int currentPos = 0;
                int dataLength = serializedData.Length;
                while (currentPos < dataLength)
                {
                    string item = DeserializeStringArrayItem(serializedData, ref currentPos);
                    items.Add(item);
                }
            }
            return items;
        }
        private static string DeserializeStringArrayItem(string serializedData, ref int currentPos)
        {
            int indexOfFirstSeparator = serializedData.IndexOf(SerializedStringArraySeparator, currentPos);
            string itemLengthString = serializedData.Substring(currentPos, indexOfFirstSeparator - currentPos);
            int itemLength = Int32.Parse(itemLengthString);
            currentPos += itemLengthString.Length + 1;
            string item = serializedData.Substring(currentPos, itemLength);
            currentPos += itemLength;
            return item;
        }
        #endregion
        public static string ValueToString(object value)
        {
            return value == null ? "" : value.ToString();
        }
        public static bool IsNullValue(object value)
        {
            return value == null || (value is DBNull);
        }
        public static object GetConvertedArgumentValue(object value, Type targetType, string argumentName)
        {
            if (value == null)
                return null;
            if (targetType == null)
                return value;
            if (targetType != typeof(String) && value.ToString() == "")
                return null;
            try
            {
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value.ToString());
                else if (targetType == typeof(String))
                    return value.ToString();
                else if (targetType == typeof(Guid))
                    return new Guid(value.ToString());
                else if (targetType == typeof(TimeSpan))
                    return TimeSpan.Parse(value.ToString());
                else
                    return Convert.ChangeType(value, targetType);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(String.Format(Properties.Resources.UnableToCast, argumentName, targetType), e);
            }
        }
        public static object GetHtmlTextWriterTagObject(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return null;
            try
            {
                return (HtmlTextWriterTag)Enum.Parse(typeof(HtmlTextWriterTag), tagName, true);
            }
            catch (Exception)
            {
            }
            return null;
        }
        public static string GetFormatString(string formatString)
        {
            bool shouldModifyFormatString = (formatString.IndexOf('{') == -1);
            return (!shouldModifyFormatString ? formatString : ("{0" + ((!string.IsNullOrEmpty(formatString)) ? ":" + formatString : string.Empty) + "}"));
        }
        public static string GetDefaultTextFormatString(int placeHolderCount, bool rtl)
        {
            if (placeHolderCount < 1)
                return String.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < placeHolderCount; i++)
            {
                if (i > 0)
                    sb.Append(rtl ? " " : "; ");
                sb.Append('{');
                sb.Append(rtl ? placeHolderCount - i - 1 : i);
                sb.Append('}');
            }
            return sb.ToString();
        }
        public static string GetDayName(DayOfWeek day, CultureInfo culture, DayNameFormat dayNameFormat)
        {
            if (dayNameFormat == DayNameFormat.Short)
            {
                return culture.DateTimeFormat.GetAbbreviatedDayName(day);
            }
            else
            {
                string dayName = culture.DateTimeFormat.GetDayName(day);
                dayName = Capitalize(dayName);
                switch (dayNameFormat)
                {
                    case DayNameFormat.FirstLetter:
                    case DayNameFormat.Shortest:
                        return dayName[0].ToString();
                    case DayNameFormat.FirstTwoLetters:
                        return dayName[0].ToString() + dayName[1].ToString();
                    default:
                        return dayName;
                }
            }
        }
        public static string GetDayName(DayOfWeek day, DayNameFormat dayNameFormat)
        {
            return GetDayName(day, CultureInfo.CurrentCulture, dayNameFormat);
        }
        public static DateTime GetFirstDateOfMonthView(int year, int month, DayOfWeek firstDay)
        {
            DateTime date = new DateTime(year, month, 1);
            int offset = (int)date.DayOfWeek - (int)firstDay;
            if (offset < 0)
                offset += 7;
            return date.AddDays(-offset);
        }
        public static string Capitalize(string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
        public static void CheckCustomPropertyName(string name)
        {
            if (!name.StartsWith(CustomJSPropertyPrefix))
                throw new ArgumentException(string.Format("Wrong custom property name '{0}'. Should start with the '{1}' prefix.", name, CustomJSPropertyPrefix));
        }
        private static void RaiseArgumentOutOfRangeException(string message)
        {
            throw new ArgumentOutOfRangeException("value", message);
        }
        public static void CheckMinimumValue(double value, double min, string propertyName)
        {
            if (min > value)
                throw new ArgumentOutOfRangeException("value",
                    string.Format(Properties.Resources.InvalidMinimumValue, propertyName, min));
        }
        public static void CheckValueRange(double value, double min, double max, string propertyName)
        {
            if (min > value || value > max)
                throw new ArgumentOutOfRangeException("value",
                    string.Format(Properties.Resources.InvalidValueRange, propertyName, min, max));
        }
        public static void CheckNegativeValue(double value, string propertyName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value",
                    string.Format(Properties.Resources.InvalidNegativeValue, propertyName));
        }
        public static void CheckNegativeOrZeroValue(double value, string propertyName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("value",
                    string.Format(Properties.Resources.InvalidNonPositiveValue, propertyName));
        }
        public static void CheckGreaterOrEqual(double greaterValue, double lessValue, string greaterPropertyName, string lessPropertyName)
        {
            if (greaterValue < lessValue)
                throw new ArgumentOutOfRangeException("value",
                    string.Format(Properties.Resources.InvalidValuesRatio, lessPropertyName, greaterPropertyName));
        }
        public static string FormatXmlDocumentText(XmlDocument xmlDocument)
        {
            string text = "";
            using (MemoryStream stream = new MemoryStream())
            {
                xmlDocument.Save(stream);
                using (StreamReader sr = new StreamReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    text = sr.ReadToEnd();
                }
            }
            return text;
        }
        private static List<int> ArrangePartInCategory(int partCount, int columnCount)
        {
            List<int> ret = new List<int>();
            int columnCountActualForPart = GetColumnCountForPart(partCount, columnCount);
            int rowCount = GetRowCount(partCount, columnCountActualForPart);
            for (int i = 0; i < rowCount; i++)
            {
                if (i == rowCount - 1)
                    ret.Add(partCount - i * columnCountActualForPart);
                else
                    ret.Add(columnCountActualForPart);
            }
            return ret;
        }
        private static int GetColumnCountForPart(int partCount, int columnCount)
        {
            int ret = columnCount;
            if (ret >= partCount)
                ret = partCount;
            if (ret < 1)
                ret = 1;
            return ret;
        }
        private static int GetRowCount(int partCount, int columnCount)
        {
            if (partCount > columnCount)
            {
                if (partCount % columnCount != 0)
                    return (partCount / columnCount) + 1;
                else
                    return partCount / columnCount;
            }
            else
                return 1;
        }
        public static int[] ArrangeParts(int[] partHeights, int columnCount)
        {
            int[] ret = null;
            if (partHeights.Length == 0)
                ret = new int[0];
            else
                if (partHeights.Length <= columnCount)
                    ret = ArrangePartInEachColumn(partHeights.Length);
                else
                {
                    if (IsArrayContaintOnlyNumber(partHeights, 1))
                        ret = ArrangePartsWithOneHeightInColumns(partHeights, columnCount);
                    else
                        ret = ArrangePartsInColumns(partHeights, columnCount);
                    if (ret.Length != columnCount)
                        ret = CorrectionPartsInColumn(ret, columnCount);
                }
            return ret;
        }
        public static int[] ArrangePartsByStartingNumber(int[] partNumbers, int partCount, int columnCount)
        {
            int[] ret = new int[columnCount];
            if (partNumbers[0] != 0)
                partNumbers[0] = 0;
            for (int i = 0; i < partNumbers.Length - 1; i++)
            {
                ret[i] = partNumbers[i + 1] - partNumbers[i];
            }
            ret[partNumbers.Length - 1] = partCount == partNumbers[partNumbers.Length - 1] ? 1 : partCount - partNumbers[partNumbers.Length - 1];
            return ret;
        }
        public static int[] ArrangePartInEachColumn(int columnCount)
        {
            int[] ret = new int[columnCount];
            for (int i = 0; i < columnCount; i++)
                ret[i] = 1;
            return ret;
        }
        public static int[] ArrangePartsInColumns(int[] partHeights, int columnCount)
        {
            int sum = 0;
            for (int i = 0; i < partHeights.Length; i++)
                sum += partHeights[i];
            int a = 0;
            int b = sum / 2;
            int c = sum;
            int newColumnCount = 0;
            int lastHeight = 0;
            while (Math.Abs(c - b) > 1)
            {
                newColumnCount = CalcColumnCount(partHeights, b);
                if (newColumnCount > columnCount)
                {
                    int newB = SeekForward(b, c);
                    a = b;
                    b = newB;
                }
                else
                {
                    if (newColumnCount == columnCount)
                        lastHeight = b;
                    int newB = SeekBack(a, b);
                    c = b;
                    b = newB;
                }
            }
            if (lastHeight == 0)
                lastHeight = b;
            return CalcPartCountInColumns(partHeights, columnCount, lastHeight);
        }
        public static int[] ArrangePartsWithOneHeightInColumns(int[] partHeights, int columnCount)
        {
            List<int> partCountInColumns = new List<int>(columnCount);
            int maxPartCountInColumn = (int)Math.Ceiling((double)partHeights.Length / columnCount);
            int partCount = partHeights.Length;
            int curPartCount = 0;
            int curColumn = 1;
            while (curPartCount < partCount)
            {
                if (partCount - curPartCount - maxPartCountInColumn >= columnCount - curColumn)
                {
                    partCountInColumns.Add(maxPartCountInColumn);
                    curPartCount += maxPartCountInColumn;
                    curColumn++;
                }
                else
                {
                    if (curColumn < columnCount)
                    {
                        partCountInColumns.Add(1);
                        curColumn++;
                        curPartCount++;
                    }
                    else
                    {
                        partCountInColumns.Add(partCount - curPartCount);
                        curPartCount += partCountInColumns[columnCount - 1];
                    }
                }
            }
            return partCountInColumns.ToArray();
        }
        public static int CalcColumnCount(int[] partHeights, int height)
        {
            int sum = 0;
            int ret = 0;
            int partIndex = 0;
            int prevIndex = 0;
            while (partIndex < partHeights.Length)
            {
                sum += partHeights[partIndex];
                if (sum > height)
                {
                    ret++;
                    sum = 0;
                    if (partIndex != prevIndex)
                    {
                        prevIndex = partIndex;
                        partIndex--;
                    }
                    else
                        prevIndex = partIndex + 1;
                }
                partIndex++;
            }
            if (sum != 0)
                ret++;
            return ret;
        }
        public static int SeekForward(int currentHeight, int endRange)
        {
            int ret = 0;
            ret = currentHeight + (endRange - currentHeight) / 2;
            return ret;
        }
        public static int SeekBack(int currentHeight, int startRange)
        {
            int ret = 0;
            ret = startRange + (currentHeight - startRange) / 2;
            return ret;
        }
        public static int[] CorrectionPartsInColumn(int[] partsInColumns, int columnCount)
        {
            List<int> retList = new List<int>(partsInColumns);
            if (partsInColumns.Length > columnCount)
            {
                for (int i = partsInColumns.Length - 1; i >= columnCount; i--)
                {
                    if (i - 2 >= 0)
                    {
                        int partsCountForReplace = Math.Max(retList[i], retList[i - 1]);
                        retList[i - 1] -= partsCountForReplace;
                        retList[i - 2] += partsCountForReplace;
                        retList[i - 1] += retList[i];
                    }
                    else
                        retList[i - 1] += retList[i];
                    retList.RemoveAt(i);
                }
            }
            else
            {
                for (int i = partsInColumns.Length; i < columnCount; i++)
                {
                    retList.Add(1);
                    int d = columnCount - i;
                    retList[partsInColumns.Length - d]--;
                    while (retList[partsInColumns.Length - d] == 0 && d <= partsInColumns.Length)
                    {
                        retList[partsInColumns.Length - d]++;
                        d++;
                        retList[partsInColumns.Length - d]--;
                    }
                }
            }
            return retList.ToArray();
        }
        public static int[] CalcPartCountInColumns(int[] partHeights, int columnCount, int height)
        {
            int sum = 0;
            List<int> retList = new List<int>();
            int partIndex = 0;
            int prevIndex = 0;
            int columnIndex = 0;
            while (partIndex < partHeights.Length)
            {
                sum += partHeights[partIndex];
                if (sum > height)
                {
                    sum = 0;
                    if (partIndex != prevIndex)
                    {
                        retList.Add(partIndex - prevIndex);
                        prevIndex = partIndex;
                        partIndex--;
                    }
                    else
                    {
                        retList.Add(1);
                        prevIndex = partIndex + 1;
                    }
                    columnIndex++;
                }
                partIndex++;
            }
            if (partIndex - prevIndex != 0)
                retList.Add(partIndex - prevIndex);
            return retList.ToArray();
        }
        public static bool IsArrayContaintOnlyNumber(int[] array, int number)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] != number)
                    return false;
            return true;
        }
        public static byte[] GetBytesFromStream(Stream stream)
        {
            byte[] ret = null;
            if ((stream != null) && (stream != Stream.Null))
            {
                if (stream.Length > 0x7fffffffL)
                    throw new Exception(Properties.Resources.UploadControl_StreamTooLong);
                if (!stream.CanSeek)
                    throw new Exception(Properties.Resources.UploadControl_StreamNotSeekable);
                int position = (int)stream.Position;
                int count = (int)stream.Length;
                try
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        stream.Seek(0L, SeekOrigin.Begin);
                        ret = reader.ReadBytes((int)stream.Length);
                    }
                }
                finally
                {
                    stream.Seek(position, SeekOrigin.Begin);
                }
                if (ret.Length != count)
                    throw new Exception(Properties.Resources.UploadControl_StreamLengthNotReached);
            }
            else
                ret = new byte[0];
            return ret;
        }
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
