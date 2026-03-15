#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    public class CssPropertiesBox
    {
        #region Properties

        /// <summary>
        /// Properties from the Style Tag
        /// </summary>
        internal Dictionary<string, string> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Property value from the Style Tag
        /// </summary>
        internal Dictionary<PropertyInfo, object> PropertyValues
        {
            get;
            set;
        }

        #endregion
        
        #region Construtors

        public CssPropertiesBox()
        {
            Properties = new Dictionary<string, string>();
            PropertyValues = new Dictionary<PropertyInfo, object>();
        }

        public CssPropertiesBox(string stylestring)
            :this()
        {
            MatchCollection matches = HtmlTagsParser.Match(HtmlTagsParser.CssProperties, stylestring);
            foreach (Match match in matches)
            {
                if (match!=null)
                {
                    string[] splitted = match.Value.Split(':');

                    if (splitted.Length != 2) continue;

                    string propName = splitted[0].Trim().ToLower();
                    string propValue = splitted[1].Trim().ToLower();

                    if(propName.StartsWith("'"))
                        propName=propName.Substring(1,propName.Length-1);

                    if (propValue.EndsWith(";'") || propValue.EndsWith("';"))
                        propValue = propValue.Substring(0, propValue.Length - 2);
                    else if (propValue.EndsWith("'") || propValue.EndsWith(";"))
                        propValue = propValue.Substring(0, propValue.Length - 1);
                    if (propName.ToLower() == "margin" && propValue.Contains("px"))
                    {
                        string[] propNames = new string[] { "before-spacing", "right-indent", "after-spacing", "left-indent" };
                        string[] values = propValue.Trim().Split(new string[] { "px" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] propValues = new string[4];
                     
                        switch(values.Length)
                        {
                            case 1:
                                propValues[0] = propValues[1] = propValues[2] = propValues[3] = values[0];
                                break;
                            case 2:
                                propValues[0] = propValues[2] = values[0];
                                propValues[1] = propValues[3] = values[1];
                                break;
                            case 3:
                                propValues[0] = values[0];
                                propValues[1] = propValues[3] = values[1];
                                propValues[2] = values[0];
                                break;
                            case 4:
                                propValues[0] = values[0];
                                propValues[1] = values[1];
                                propValues[2] = values[2];
                                propValues[3] = values[3];
                                break;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            if (!(Properties.ContainsKey(propNames[i])))
                                Properties.Add(propNames[i], propValues[i]);

                            if (HtmlNode.Properties.ContainsKey(propNames[i]))
                                if (!(PropertyValues.ContainsKey(HtmlNode.Properties[propNames[i]])))
                                    PropertyValues.Add(HtmlNode.Properties[propNames[i]], propValues[i]);
                        }
                    }
                    else
                    {
                        if (!(Properties.ContainsKey(propName)))
                            Properties.Add(propName, propValue);

                        if (HtmlNode.Properties.ContainsKey(propName))
                            if (!(PropertyValues.ContainsKey(HtmlNode.Properties[propName])))
                                PropertyValues.Add(HtmlNode.Properties[propName], propValue);
                    }
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Assign the Style tag properties to the Box
        /// </summary>
        /// <param name="Box"></param>
        internal void AssignProperties(HtmlNode Box)
        {
            if (Box!=null)
            {
                foreach (PropertyInfo property in PropertyValues.Keys)
                {
                    object value = PropertyValues[property];
                    if (value.ToString()=="inherit" && Box.ParentNode!=null)
                    {
                        value = (object)property.GetValue(Box.ParentNode, null);
                    }
                    if (property.PropertyType.Name == "Double")
                    {
                        double dValue;
                        if (double.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dValue))
                            property.SetValue(Box, dValue, null);
                    }
                    else
                        property.SetValue(Box, value, null);
                }
            }
        }

        /// <summary>
        /// Updates the property values of Boxes
        /// </summary>
        internal void UpdatePropertyValues()
        {
            PropertyValues.Clear();

            foreach (string prop in Properties.Keys)
            {
                if (HtmlNode.Properties.ContainsKey(prop))
                {
                     PropertyValues.Add(HtmlNode.Properties[prop], Properties[prop]);
                }
            }
        }



        #endregion
    }
}
