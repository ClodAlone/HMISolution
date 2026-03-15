using System;
using System.Linq;
using DevExpress.Xpo;
using Opc.Ua;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace UFUAModel
{
    public class ArrayConverter : DevExpress.Xpo.Metadata.ValueConverter
    {
        static readonly char separator = ',';
        public override object ConvertToStorageType(object value)
        {
            int[] array = value as int[];
            if (array == null)
                return String.Empty;
            var builder = new StringBuilder();
            foreach (var i in array)
            {
                if (builder.Length > 0)
                    builder.Append(separator);
                builder.Append(i);
            }
            return builder.ToString();
        }
        public override object ConvertFromStorageType(object value)
            {
                var str = value as String;
                if (String.IsNullOrEmpty(str))
                    return null;
                var split = str.Split(separator);
                if (split.Length == 0)
                    return null;
                int[] array = new int[split.Length];
                for (int i = 0; i < split.Length; ++i)
                {
                    try
                    {
                        array[i] = Convert.ToInt32(split[i]);
                    }
                    catch (Exception ex)
                    {
                        array[i] = 0;
                    }
                }
                return array;
            }
        public override Type StorageType
        {
            get { return typeof(String); }
        }
    }
}
