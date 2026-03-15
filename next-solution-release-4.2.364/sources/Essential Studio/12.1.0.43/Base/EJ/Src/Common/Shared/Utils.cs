#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Configuration;
using System.Web.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace Syncfusion.JavaScript
{
   public static class Utils
   {
#region Fields
            static Dictionary<String, String> idJsonDataPair = new Dictionary<String, String>();
            public const String ej = "ej";
            public const String Grid = "Grid";
            public const String dataManager = "DataManager";
            public const String dataSource = "dataSource";
            public const String startDiv = "<div id=\"";
            public const String endDiv = "\"></div>";
            public const String startScript = "<script type='text/javascript'>$(function($){";
            public const String endScript = "});</script>";
            public const String handleEvent = @"([a-z:\,])";
            public const String replaceEvent = @"{*}*";
            public const String table = @"#([A-Za-z0-9_])";
            
#endregion
            public static String DataManager { get; set; } 
                public static String Query { get; set; }
            public static Dictionary<String, String> IdJsonPair
            {
                get { return idJsonDataPair; }
                set { idJsonDataPair = value; }
            }

           

            public static bool PropertyCompare(Object source, Object destination)
            {
                Type sourceType = source.GetType();

                Type destinationType = destination.GetType();

                IList<PropertyInfo> propertyList = source.GetType().GetProperties().ToList();

                String sourceValue=null, destinationValue=null;

                foreach (var property in propertyList)
                {
                    var sValue = sourceType.GetProperty(property.Name).GetValue(source, null);
                    var dValue = destinationType.GetProperty(property.Name).GetValue(destination, null);
                    if (sValue != null)
                        sourceValue = sValue.ToString();
                    if (dValue != null)
                        destinationValue = dValue.ToString();

                    if (property.PropertyType.IsGenericType)
                    {
                        ICollection iSource = sourceType.GetProperty(property.Name).GetValue(source, null) as ICollection;
                        ICollection iDestination = destinationType.GetProperty(property.Name).GetValue(destination, null) as ICollection;
                        if (iSource.Count != iDestination.Count)
                            return true;
                    }
                    else
                    {
                        if (sourceValue != destinationValue)
                            return true;
                    }
                }
                return false;
            
        }

            //Method to check for default value.

            public static bool DefaultValueHandler(PropertyInfo inputObject, object baseObject)
            {
                DefaultValueAttribute defaultAttribute = null;

                object[] defaultValueAttributes = inputObject.GetCustomAttributes(typeof(DefaultValueAttribute), false);

                defaultAttribute = defaultValueAttributes.Count() != 0 ? (DefaultValueAttribute)defaultValueAttributes.First() : null;

                string inputValue = null, defaultValue = "";
                ICollection collection = null;

                var propertyValue = inputObject.GetValue(baseObject, null);

                inputValue = propertyValue != null ? propertyValue.ToString() : "";

                if (inputObject.PropertyType.IsPrimitive || inputObject.PropertyType.IsEnum || propertyValue is String || propertyValue is decimal ||
                   (inputObject.PropertyType.IsAssignableFrom(typeof(object)) && propertyValue.GetType().IsPrimitive))
                {
                    if (defaultAttribute != null && defaultAttribute.Value != null)
                        defaultValue = defaultAttribute.Value.ToString();

                    if (defaultAttribute != null && defaultAttribute.Value == null)
                        return false;
                    return inputValue.Equals(defaultValue) ? true : false;
                }                
                else if (propertyValue is ICollection)
                {
                    collection = propertyValue as ICollection;
                    return collection.Count == 0 ? true : false;
                }
                else
                {
                    return true;
                }
            }


            public static bool IsComplexObject(PropertyInfo property, object propertyValue)
            {
                if (propertyValue == null || propertyValue is string || propertyValue.GetType().IsPrimitive || propertyValue is decimal || propertyValue is IEnumerable || propertyValue is ICollection || propertyValue == null || property.PropertyType.IsEnum)
                    return false;
                else
                    return true;
            }
    
            //Check for Unobtrusive settings in web.config

            public static bool IsUnObtrusive()
            {
                AppSettingsReader reader = new AppSettingsReader();
                var keys = ConfigurationSettings.AppSettings.Keys;
                object r = null;
                foreach (var k in keys)
                {
                    string ks = k.ToString();
                    if (ks.Equals("UnobtrusiveJavaScriptEnabled"))
                    {
                        r = reader.GetValue("UnobtrusiveJavaScriptEnabled", typeof(bool));
                        return Convert.ToBoolean(r);
                    }
                }
                return false;
            }
    }
}
