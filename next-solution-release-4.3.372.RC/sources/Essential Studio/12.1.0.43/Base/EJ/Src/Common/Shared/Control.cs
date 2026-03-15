#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Web.Script.Serialization;



namespace Syncfusion.JavaScript
{

    public abstract class Control
    {
        public string ID { get; set; }

        public virtual string TagName
        {
            get { return "div"; }
        }

        public abstract string PluginName
        {
            get;
        }

        protected abstract object Model { get; }

        protected string Data { get; set; }

        public virtual HtmlString Render()
        {

            if (!EssentialJavaScript.UnObtrusive)
            {
                RenderJson();
                return this.CreateContainer(this.ID);
            }
            else
                return this.CreateUnObtrusiveContainer(this.ID);

        }

        //Serialize the object to json string.

        public string Serialize(object model)
        {            
            SerializeObject convert = new SerializeObject();
            return convert.SerializeToJson(model, this.ID);
        }

        //Render the json string 

        protected String RenderJson()
        {

            String jsonResult, initialScriptContent;
            jsonResult = Serialize(this.Model);
            initialScriptContent = this.BuildScriptContent(jsonResult);
            ScriptManager.RegisterControl(ID, initialScriptContent);
            return initialScriptContent;
        }

        //Method to build the script content.

        protected String BuildScriptContent(String jsonContent)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("$(\"#" + this.ID + "\")." + this.PluginName + "(");
            builder.Append(jsonContent);
            builder.Append(");");

            return builder.ToString();
        }

        //Create the Control Container.

        public virtual HtmlString CreateContainer(string controlId)
        {
            return new HtmlString("");
        }

        public virtual HtmlString CreateUnObtrusiveContainer(string controlId)
        {
            StringBuilder tag = new StringBuilder();
            string pluginName = PluginName.Substring(2).ToLower();
            string pluginString = "data-ej-";

            tag.Append("<")
               .Append(TagName)
               .Append(" id=\"")
               .Append(controlId + "\"")
               .Append(" data-role=\"" + PluginName.ToLower() + "\" ")
               .Append(CreateUnObtrusiveDataAttributes(this.Model,controlId,pluginString))
               .Append("></")
               .Append(TagName)
               .Append(">")
               .Append(this.Data);
            
            return new HtmlString(tag.ToString());
        }

        // Method to create the Unobtrusive Wrapper container.

        protected string CreateUnObtrusiveDataAttributes(object inputObject,string id,string argPluginString)
        {
            SerializeObject convert = new SerializeObject();
            StringBuilder tag = new StringBuilder();
            StringBuilder tempTag = new StringBuilder();

            Type inputType = inputObject.GetType();
            var properties = inputType.GetProperties().ToList();

            string query = GetQuery(properties, inputObject);
            string pluginString = argPluginString;
            string complexObjectPluginString = null;
            string propertyName = null;

            foreach (PropertyInfo property in properties)
            {
                object[] attrList = property.GetCustomAttributes(false);
                List<object> listAttr = attrList.ToList();

                JsonIgnoreAttribute jIgnore = listAttr.Count() != 0 ? (JsonIgnoreAttribute)listAttr.Find(item => item.GetType() == typeof(JsonIgnoreAttribute)) : null;

                bool propertyIgnore = (jIgnore != null) ? true : false;
                if (!propertyIgnore)
                {
                    object propertyValue = property.GetValue(inputObject, null);
                    Type propertyValueType = propertyValue!=null?propertyValue.GetType():property.PropertyType;

                    JsonPropertyAttribute jsonAttribute = attrList.Count() != 0 ? (JsonPropertyAttribute)listAttr.Find(item => item.GetType() == typeof(JsonPropertyAttribute)) : null;
                    propertyName = (jsonAttribute != null ? jsonAttribute.PropertyName : property.Name).ToLower();
                    JsonConverterAttribute jsonConverter = attrList.Count() != 0 ? (JsonConverterAttribute)listAttr.Find(item => item.GetType() == typeof(JsonConverterAttribute)) : null;
                    DataManagerAttribute dataManager = attrList.Count() != 0 ? (DataManagerAttribute)listAttr.Find(item => item.GetType() == typeof(DataManagerAttribute)) : null;
                    EJDateAttribute dateAttr = attrList.Count() != 0 ? (EJDateAttribute)listAttr.Find(item => item.GetType() == typeof(EJDateAttribute)) : null;
                    object objectInstance = (jsonConverter != null && jsonConverter.ConverterType != null) ? Activator.CreateInstance(jsonConverter.ConverterType) : null;
                    MethodInfo method = jsonConverter != null ? jsonConverter.ConverterType.GetMethod("SerializeToJson"): null;

                    if (Utils.IsComplexObject(property, propertyValue) && jsonConverter == null && !propertyValue.GetType().IsAssignableFrom(typeof(object)) && dateAttr == null)
                    {
                        complexObjectPluginString = argPluginString + propertyName + "-";
                        if (propertyValue.GetType().IsPrimitive || propertyValue.GetType().IsEnum || propertyValue is String)
                            tag.Append(complexObjectPluginString.Substring(0,complexObjectPluginString.Length-1)).Append("='").Append(propertyValue is string ? propertyValue.ToString() : propertyValue.ToString().ToLower()).Append("' ");
                        else
                            tag.Append(CreateUnObtrusiveDataAttributes(propertyValue,id,complexObjectPluginString));
                       
                    }
                    else
                    {
                        tempTag.Append(pluginString)
                               .Append(propertyName)
                               .Append("='");
                        if (jsonConverter != null && typeof(QueryConverter).IsAssignableFrom(jsonConverter.ConverterType) && propertyValue != null)
                        {
                            tempTag.Append("ej.dataSources.").Append(id).Append(".query' ");
                        }
                        else if (jsonConverter != null && typeof(DataManagerConverter).IsAssignableFrom(jsonConverter.ConverterType) && propertyValue != null && !propertyValueType.IsAssignableFrom(typeof(object)))
                        {
                            if (method != null)
                            {
                                tempTag.Append("ej.dataSources.").Append(id).Append(".data' ");
                               CreateDataVariable(method.Invoke(objectInstance, new object[] { propertyValue }), query);
                            }
                            else
                            {
                                tempTag.Clear();
                            }
                        }
                        else if (dateAttr != null && propertyValue is DateTime) {
                            tempTag.Append(new JavaScriptSerializer().Serialize(propertyValue).Replace("\"","").Replace("\\","")).Append("' ");
                        }
                        else if (propertyValue is ICollection && !Utils.DefaultValueHandler(property, inputObject))
                        {
                            
                            tempTag.Append(Serialize(propertyValue))
                                   .Append("' ");
                            if (EssentialJavaScript.UnObtrusive && Utils.DataManager != null)
                                { 
                                CreateDataVariable(Utils.DataManager, Utils.Query);
                                Utils.DataManager = null;
                                    Utils.Query=null;
                            }
                            
                        }
                        else if ((property.PropertyType.IsPrimitive || property.PropertyType.IsEnum || (propertyValue != null && (propertyValueType.IsPrimitive || propertyValue is String))) && !Utils.DefaultValueHandler(property, inputObject))
                        {
                            tempTag.Append(propertyValue is string ? propertyValue.ToString() : propertyValue.ToString().ToLower())
                                   .Append("' ");
                        }
                        else
                        {
                            tempTag.Clear();
                        }

                        tag.Append(tempTag.ToString());
                        tempTag.Clear();
                    }

                }
            }

            return tag.ToString();
        }

        /// <summary>
        /// Creates the un obtrusive data dictionary.
        /// </summary>
        /// <param name="inputObject">The input object.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="argPluginString">The argument plugin string.</param>
        /// <returns></returns>
        protected Dictionary<string, object> CreateUnObtrusiveDataDictionary(object inputObject, string id, string argPluginString)
        {
            SerializeObject convert = new SerializeObject();
            Dictionary<string, object> tag = new Dictionary<string, object>(); 

            Type inputType = inputObject.GetType();
            var properties = inputType.GetProperties().ToList();

            string query = GetQuery(properties, inputObject);
            string pluginString = argPluginString;
            string complexObjectPluginString = null;
            string propertyName = null;

            foreach (PropertyInfo property in properties)
            {
                object[] attrList = property.GetCustomAttributes(false);
                List<object> listAttr = attrList.ToList();

                JsonIgnoreAttribute jIgnore = listAttr.Count() != 0 ? (JsonIgnoreAttribute)listAttr.Find(item => item.GetType() == typeof(JsonIgnoreAttribute)) : null;

                bool propertyIgnore = (jIgnore != null) ? true : false;
                if (!propertyIgnore)
                {
                    object propertyValue = property.GetValue(inputObject, null);
                    Type propertyValueType = propertyValue != null ? propertyValue.GetType() : property.PropertyType;

                    JsonPropertyAttribute jsonAttribute = attrList.Count() != 0 ? (JsonPropertyAttribute)listAttr.Find(item => item.GetType() == typeof(JsonPropertyAttribute)) : null;
                    propertyName = (jsonAttribute != null ? jsonAttribute.PropertyName : property.Name).ToLower();
                    JsonConverterAttribute jsonConverter = attrList.Count() != 0 ? (JsonConverterAttribute)listAttr.Find(item => item.GetType() == typeof(JsonConverterAttribute)) : null;
                    DataManagerAttribute dataManager = attrList.Count() != 0 ? (DataManagerAttribute)listAttr.Find(item => item.GetType() == typeof(DataManagerAttribute)) : null;
                    object objectInstance = (jsonConverter != null && jsonConverter.ConverterType != null) ? Activator.CreateInstance(jsonConverter.ConverterType) : null;
                    MethodInfo method = jsonConverter != null ? jsonConverter.ConverterType.GetMethod("SerializeToJson") : null;

                    if (Utils.IsComplexObject(property, propertyValue) && jsonConverter == null && !propertyValue.GetType().IsAssignableFrom(typeof(object)))
                    {
                        complexObjectPluginString = argPluginString + propertyName + "-";
                        if (propertyValue.GetType().IsPrimitive || propertyValue.GetType().IsEnum || propertyValue is String)
                            tag.Add(complexObjectPluginString.Substring(0, complexObjectPluginString.Length - 1), (propertyValue is string ? propertyValue.ToString() : propertyValue.ToString().ToLower()));
                        else
                            tag.Merge(CreateUnObtrusiveDataDictionary(propertyValue, id, complexObjectPluginString));

                    }
                    else
                    {
                        if (jsonConverter != null && typeof(QueryConverter).IsAssignableFrom(jsonConverter.ConverterType) && propertyValue != null)
                        {
                            tag.Add(pluginString + propertyName, "ej.dataSources." + id + ".query");
                             
                        }
                        else if (jsonConverter != null && typeof(DataManagerConverter).IsAssignableFrom(jsonConverter.ConverterType) && propertyValue != null && !propertyValueType.IsAssignableFrom(typeof(object)))
                        {
                            if (method != null)
                            {
                                tag.Add(pluginString + propertyName, "ej.dataSources." + id + ".data"); 
                                CreateDataVariable(method.Invoke(objectInstance, new object[] { propertyValue }), query);
                            }
                        }
                        else if (propertyValue is ICollection && !Utils.DefaultValueHandler(property, inputObject))
                        {
                            tag.Add(pluginString + propertyName, Serialize(propertyValue)); 
                            if (EssentialJavaScript.UnObtrusive && Utils.DataManager != null)
                            {
                                CreateDataVariable(Utils.DataManager, Utils.Query);
                                Utils.DataManager = null;
                                Utils.Query = null;
                            }

                        }

                        else if ((property.PropertyType.IsPrimitive || property.PropertyType.IsEnum || propertyValue is String) && !Utils.DefaultValueHandler(property, inputObject))
                        {
                            tag.Add(pluginString + propertyName, propertyValue is string ? propertyValue.ToString() : propertyValue.ToString().ToLower());

                        }
                    }

                }
            } 
            return tag;
        }


        
        protected String CreateDataVariable(object value, string queryValue)
        {
            var isCollection = value is ICollection;
            StringBuilder script = new StringBuilder();
            script.Append("<script>ej.createObject(\"ej.dataSources.")
                .Append(this.ID)
                .Append("\",{ data: ")
                .Append(isCollection ? "ej.isJSON(" : "")
                .Append(value)
                .Append(isCollection ? ")" : "");

            if (queryValue != null)
                script.Append(",").Append("query:")
                      .Append(queryValue);
                      
            script.Append("});</script>");
            this.Data = script.ToString();
            return this.Data;
        }

        //Method to get Query Value during Unobtrusive wrapper creation

        private string GetQuery(List<PropertyInfo> propertyList, object baseObject)
        {
            PropertyInfo queryObject = null;
            IEnumerable<PropertyInfo> queryValue = from p in propertyList where p.Name == "Query" select p;
            queryObject = queryValue.Count() != 0 ? queryValue.First() : null;
            object value = queryObject != null ? queryObject.GetValue(baseObject, null) : null;
            return value != null ? value.ToString().Replace("'", "\"") : null;
        }

        


        public HtmlString ToString()
        {
            return Render();
        }
    }
}
