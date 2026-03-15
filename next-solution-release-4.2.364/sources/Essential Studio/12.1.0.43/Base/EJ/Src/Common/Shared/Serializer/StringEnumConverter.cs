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
using System.Runtime.Serialization;
using System.Reflection;

namespace Syncfusion.JavaScript.Shared.Serializer
{
    public class StringEnumConverter:Converter
    {
        protected internal override IDictionary<string, object> BuildJsonDictionary(object value)
        {
            EnumMemberAttribute enumMember;
            IDictionary<string, object> jsonDictionary = new Dictionary<string, object>();
            Type objectType = value.GetType();

            MemberInfo[] members = objectType.GetMember(value.ToString());
            MemberInfo member = members.First();

            object[] attrList = member.GetCustomAttributes(typeof(EnumMemberAttribute), true);
            enumMember = (EnumMemberAttribute)attrList.First();

            string val = enumMember.Value;
            jsonDictionary.Add(value.GetType().Name, val);

            return jsonDictionary;
        }
        public override string SerializeToJson(object inputObject)
        {
            object[] attrList = inputObject.GetType().GetCustomAttributes(false);
            List<object> listAttr = attrList.ToList();
            FlagsAttribute flagAttr = attrList.Count() != 0 ? (FlagsAttribute)listAttr.Find(item => item.GetType() == typeof(FlagsAttribute)) : null;
            bool flag = (flagAttr != null) ? true : false;
            if (flag) {
                int value = (int)inputObject;
                return value.ToString();
            }
            else
            {
                IDictionary<string, object> enumDictionary = BuildJsonDictionary(inputObject);
                object enumValue = enumDictionary.First().Value;
                string enumstring = "\"" + enumValue.ToString() + "\"";
                return enumstring;
            }
        }
        }
    }

