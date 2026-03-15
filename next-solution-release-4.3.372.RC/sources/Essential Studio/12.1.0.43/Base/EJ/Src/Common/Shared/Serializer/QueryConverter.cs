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

namespace Syncfusion.JavaScript.Shared.Serializer
{
    public class QueryConverter:Converter
    {
        protected internal override IDictionary<string, object> BuildJsonDictionary(object value)
        {
            IDictionary<string, object> jDictionary = new Dictionary<string, object>();
            if (value is string)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(value.ToString()).Replace("'","\"");
                jDictionary.Add(value.GetType().Name, sb.ToString());
            }
            return jDictionary;
        }

        public override string SerializeToJson(object inputObject)
        {
           
                IDictionary<string, object> jDictionary = BuildJsonDictionary(inputObject);
                object value = null;
                foreach (KeyValuePair<string, object> k in jDictionary)
                {
                    value = k.Value;
                }
                return value.ToString();
            
        }
    }
}
