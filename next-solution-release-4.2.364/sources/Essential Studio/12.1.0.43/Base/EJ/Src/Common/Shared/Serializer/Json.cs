#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Syncfusion.JavaScript.Shared.Serializer
{
    public static class Json
    {

        public static string GetJson(IDictionary<string, object> input,string id)
        {
            IDictionary<string, object> tempDictionary = new Dictionary<string, object>();
            KeyValuePair<string, object> tempPair = new KeyValuePair<string, object>();
            List<string> jData = new List<string>();
            List<string> collectionList = null;
            StringBuilder final = new StringBuilder();
            StringBuilder collectionstring = null;
            string keystring = null;
            object value = null;
            string temp = null;

            final.Append("{");

            foreach (KeyValuePair<string, object> k in input)
            {
                StringBuilder sb = new StringBuilder();
                keystring = k.Key;
                value = k.Value;
                string key = null;
                key = EssentialJavaScript.UnObtrusive ? "\"" + keystring + "\"" : keystring;
                if (value is string)
                {
                    if (!Regex.IsMatch(value.ToString(), @"([[[{""])") && !value.ToString().StartsWith("ej"))
                        value = "\"" + value.ToString() + "\"";
                    sb.Append(key).Append(":").Append(value);
                    jData.Add(sb.ToString());
                }
                else if (value is bool)
                {
                    sb.Append(key).Append(":").Append(value.ToString().ToLower());
                    jData.Add(sb.ToString());
                }
                else if (value is IDictionary)
                {
                    sb.Append(key).Append(":").Append(Json.GetJson((IDictionary<string, object>)value,id));
                    jData.Add(sb.ToString());
                }
                else if (value is ICollection)
                {
                    ICollection iCollection = value as ICollection;
                    collectionList = new List<string>();

                    sb.Append(key).Append(":").Append("[");
                    foreach (var v in iCollection)
                    {
                        collectionstring = new StringBuilder();

                        if (v is string)
                        {
                            collectionstring.Append("\"" + v + "\"");
                            collectionList.Add(collectionstring.ToString());
                        }
                        else if (v.GetType().IsPrimitive || v is decimal)
                        {
                            collectionstring.Append(v);
                            collectionList.Add(collectionstring.ToString());
                        }
                        else if (v is IDictionary)
                        {
                            collectionstring.Append(Json.GetJson((IDictionary<string, object>)v,id));
                            collectionList.Add(collectionstring.ToString());
                        }
                        else
                        {
                            tempPair = (KeyValuePair<string, object>)v;
                            tempDictionary.Add(tempPair.Key, tempPair.Value);
                            collectionstring.Append(Json.GetJson(tempDictionary,id));
                            collectionList.Add(collectionstring.ToString());
                        }
                    }
                    temp = string.Join(",", collectionList.ToArray());
                    sb.Append(temp.ToString())
                      .Append("]");
                    jData.Add(sb.ToString());

                }
                else
                {
                    sb.Append(key).Append(":").Append(value);
                    jData.Add(sb.ToString());
                }
            }

            temp = string.Join(",", jData.ToArray());
            final.Append(temp).Append("}");

            return final.ToString();
        }

        public static string GetJson(ICollection input,string id)
        {
            ICollection iCollection = input as ICollection;
            List<string> collectionList = new List<string>();
            StringBuilder sb = new StringBuilder();
            SerializeObject convert = new SerializeObject();
            StringBuilder collectionString = null;
            sb.Append("[");
            foreach (var v in iCollection)
            {
                collectionString = new StringBuilder();

                if (v is string)
                {
                    collectionString.Append("\"" + v + "\"");
                    collectionList.Add(collectionString.ToString());
                }
                else if (v.GetType().IsPrimitive || v is decimal)
                {
                    collectionString.Append(v);
                    collectionList.Add(collectionString.ToString());
                }
                else
                {
                    convert.ID = id;
                    collectionString.Append(convert.SerializeToJson(v));
                    collectionList.Add(collectionString.ToString());
                }
            }
            sb.Append(String.Join(",",collectionList.ToArray())).Append("]");
            return sb.ToString();
        }

    }
}
