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
using System.ComponentModel;
using System.Collections;
using System.Web.Script.Serialization;
 

namespace Syncfusion.JavaScript.Shared.Serializer
{
    public class SerializeObject : Converter
    {
        public string ID { get; set; }

        protected internal override IDictionary<string, object> BuildJsonDictionary(object obj)
        {
            IDictionary<string, object> jsonData = new Dictionary<string, object>();
            IDictionary<string, object> tempDict = new Dictionary<string, object>();

            var propertyList = obj.GetType().GetProperties().ToList();

            foreach (PropertyInfo property in propertyList)
            {
                object[] attrList = property.GetCustomAttributes(false);
                List<object> listAttr = attrList.ToList();
                
                //To ignore property with 'JsonIgnore' Attribute
                JsonIgnoreAttribute jIgnore = listAttr.Count() != 0 ? (JsonIgnoreAttribute)listAttr.Find(item => item.GetType() == typeof(JsonIgnoreAttribute)) : null;
                bool propertyIgnore = (jIgnore != null) ? true : false;

                if (!propertyIgnore)
                {
                    //Get Property Attributes
                    JsonPropertyAttribute jsonName = attrList.Count() != 0 ? (JsonPropertyAttribute)listAttr.Find(item => item.GetType() == typeof(JsonPropertyAttribute)) : null;
                    DefaultValueAttribute defaultAttrValue = attrList.Count() != 0 ? (DefaultValueAttribute)listAttr.Find(item => item.GetType() == typeof(DefaultValueAttribute)) : null;
                    JsonConverterAttribute jsonConverter = attrList.Count() != 0 ? (JsonConverterAttribute)listAttr.Find(item => item.GetType() == typeof(JsonConverterAttribute)) : null;
                    DataManagerAttribute dataManager = attrList.Count() != 0 ? (DataManagerAttribute)listAttr.Find(item => item.GetType() == typeof(DataManagerAttribute)) : null;
                    //Get Property Value

                    object propertyValue = property.GetValue(obj, null);
                    Type type = propertyValue != null ? propertyValue.GetType() : null;

                    //Create Custom Converter Instance

                    object objectInstance = (jsonConverter != null && jsonConverter.ConverterType != null) ? Activator.CreateInstance(jsonConverter.ConverterType) : null;
                    MethodInfo method = jsonConverter != null ? jsonConverter.ConverterType.GetMethod("SerializeToJson") : null;

                    
                    string defaultValue = defaultAttrValue != null && defaultAttrValue.Value != null ? defaultAttrValue.Value.ToString() : null;
                    string value = propertyValue != null ? propertyValue.ToString() : defaultValue;

                    if (jsonConverter != null && jsonConverter.ConverterType != null && method != null && propertyValue != null && !type.IsEnum && !type.IsAssignableFrom(typeof(object)))
                    {
                        object dataValue = null;
                        if (EssentialJavaScript.UnObtrusive && dataManager != null)
                        {
                            dataValue = "\"ej.dataSources." + this.ID + "." + jsonName.PropertyName + "\"";
                            if (!typeof(QueryConverter).IsAssignableFrom(jsonConverter.ConverterType))
                            {
                                Utils.DataManager = method.Invoke(objectInstance, new object[] { propertyValue }).ToString();
                                Utils.Query = GetQuery(propertyList, obj);
                            }
                        }
                        else
                            dataValue = method.Invoke(objectInstance, new object[] { propertyValue }); //Invoke DataManagerConverter and QueryConverter
                        jsonData.Add(jsonName.PropertyName, jsonName.PropertyName == "dataSource" && propertyValue is ICollection ? "ej.isJSON(" + dataValue + ")" : dataValue);
                    }
                    else if (defaultAttrValue != null && value != defaultValue)
                    {
                        if (type != null && type.IsEnum && method != null)
                        {
                            jsonData.Add(jsonName.PropertyName, method.Invoke(objectInstance, new object[] { propertyValue }));  //Invoke StringEnumConverter
                        }
                        else if (propertyValue is string)
                        {
                            jsonData.Add(jsonName.PropertyName, "\"" + propertyValue + "\"");
                        }
                        else if (propertyValue is DateTime)
                        {
                            if (listAttr.Find(item => item.GetType() == typeof(EJDateAttribute)) != null)
                            {

                                jsonData.Add(jsonName.PropertyName, new JavaScriptSerializer().Serialize(propertyValue));
                            }
                            else
                                jsonData.Add(jsonName.PropertyName, "\"" + ((DateTime)propertyValue).ToUniversalTime()
                                                                               .ToString(
                                                                                   "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") +
                                                                    "\"");
                        }
                        else
                        {
                            if (propertyValue != null && Utils.IsComplexObject(property, propertyValue))
                                jsonData.Add(jsonName.PropertyName, BuildJsonDictionary(propertyValue));
                            else
                                jsonData.Add(jsonName.PropertyName, propertyValue);

                        }
                    }
                    else if (propertyValue != null && Utils.IsComplexObject(property, propertyValue))
                    {
                        if (propertyValue.GetType().IsPrimitive || propertyValue.GetType().IsEnum || propertyValue is String)
                            jsonData.Add(jsonName.PropertyName, propertyValue);
                        if (Utils.PropertyCompare(propertyValue, Activator.CreateInstance(propertyValue.GetType())))
                            jsonData.Add(jsonName.PropertyName, BuildJsonDictionary(propertyValue));
                    }
                    else if (propertyValue is ICollection)
                    {
                        if (propertyValue is IDictionary)
                        {
                            IDictionary<string, object> iDictionary = propertyValue as IDictionary<string, object>;
                            if (iDictionary != null && iDictionary.Count != 0)
                                jsonData.Add(jsonName.PropertyName, Json.GetJson(iDictionary, this.ID));
                        }
                        else
                        {
                            ICollection icol = propertyValue as ICollection;

                            if (icol.Count != 0)
                            {
                                jsonData.Add(jsonName.PropertyName, SerializeCollection(propertyValue));
                            }
                        }
                    }
                }
            }
            return jsonData;
        }

        public override string SerializeToJson(object inputObject)
        {

            IDictionary<string, object> jDictionary = null;
            string json = null;
            if (inputObject is string || inputObject.GetType().IsPrimitive)
            {
                json = inputObject.ToString();
            }
            else if (inputObject is ICollection)
            {
                json = Json.GetJson((ICollection)inputObject,this.ID);
            }
            else
            {
                jDictionary = BuildJsonDictionary(inputObject);
                json = Json.GetJson(jDictionary,this.ID);
            }
            return json;
        }

        public string SerializeToJson(object input, string ID)
        {
            this.ID = ID;
            return SerializeToJson(input);
        }

        private List<object> SerializeCollection(object collection)
        {
            List<object> list = new List<object>();
            ICollection iCollection = collection != null ? collection as ICollection : null;

            foreach (var item in iCollection)
            {
                if (item is string || item.GetType().IsPrimitive || item is decimal)
                    list.Add(item);
                else
                    list.Add(BuildJsonDictionary(item));
            }
            return list;
        }
        private string GetQuery(List<PropertyInfo> propertyList, object baseObject)
        {
            PropertyInfo queryObject = null;
            IEnumerable<PropertyInfo> queryValue = from p in propertyList where p.Name == "Query" select p;
            queryObject = queryValue.Count() != 0 ? queryValue.First() : null;
            object value = queryObject != null ? queryObject.GetValue(baseObject, null) : null;
            return value != null ? value.ToString().Replace("'", "\"") : null;
        }
    }
}
