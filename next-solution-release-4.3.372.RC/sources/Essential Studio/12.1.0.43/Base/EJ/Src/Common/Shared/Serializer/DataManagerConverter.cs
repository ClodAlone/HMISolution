#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Syncfusion.JavaScript.DataSources;
using System.Collections;
using System.Web.Script.Serialization;

namespace Syncfusion.JavaScript.Shared.Serializer
{
    public class DataManagerConverter : Converter
    {
        private IDataSourceSerializer serializer = null;

        public IDataSourceSerializer Serializer
        {
            get
            {
                if (serializer == null)
                    serializer = new DataSourceSerializer();
                return serializer;
            }
            set { serializer = value; }
        }

        public DataManagerConverter()
        { }

        SerializeObject convert = new SerializeObject();
        protected internal override IDictionary<string, object> BuildJsonDictionary(object value)
        {
            Type objType = value.GetType();
            IDictionary<string, object> outputDictionary = new Dictionary<string, object>();
            IDictionary<string, object> tempDictionary = new Dictionary<string, object>();
            if (typeof(DataSource).IsAssignableFrom(objType))
            {
                tempDictionary = convert.BuildJsonDictionary(value);
                outputDictionary.Add(value.GetType().Name, tempDictionary);
            }
            return outputDictionary;
        }

        public override string SerializeToJson(object value)
        {
            Type valType = value.GetType();
            StringBuilder sb = new StringBuilder();
            if (value is ICollection && !typeof(ICollection<KeyValuePair<string, object>>).IsAssignableFrom(valType))
            {
                return Serializer.Serialize(value);
            }
            else
            {
                sb.Append("ej.DataManager(");
                if (value is string)
                    sb.Append("\"").Append(value).Append("\"");
                else if ((typeof(DataSource).IsAssignableFrom(valType)))
                {
                    IDictionary<string, object> dataDictionary = BuildJsonDictionary(value);
                    string jValue = null;

                    foreach (KeyValuePair<string, object> k in dataDictionary)
                    {
                        jValue = Json.GetJson((IDictionary<string, object>)k.Value,"");
                        sb.Append(jValue);
                    }
                }
                sb.Append(")");
            }

            return sb.ToString();
        }
    }

    public interface IDataSourceSerializer
    {
        string Serialize(object obj);
    }

    public class DataSourceSerializer : IDataSourceSerializer
    {
        public string Serialize(object obj)
        {
            JavaScriptSerializer jSerializer = new JavaScriptSerializer() { };
            var str = jSerializer.Serialize(obj);
            return EssentialJavaScript.UnObtrusive ? str.Replace("'", "\"") : str;
        }
    }
}
