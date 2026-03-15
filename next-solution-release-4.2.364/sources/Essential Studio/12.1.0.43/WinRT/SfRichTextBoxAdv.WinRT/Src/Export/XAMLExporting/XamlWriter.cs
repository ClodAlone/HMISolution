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
using System.Windows.Input;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
#if WPF
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class XamlWriter
    {
        private static XmlWriter LocalXmlWriter
        {
            get;
            set;
        }

        private static DefaultValueDictionary dictionary
        {
            get;
            set;
        }
        public XamlWriter()
        {
               
        }

        static XamlWriter()
        {
            
        }

        public static string Write(object element)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            StringBuilder xamlbuilder = new StringBuilder();
            using(LocalXmlWriter=XmlWriter.Create(xamlbuilder,settings))
            {
                WriteXaml(element, LocalXmlWriter,"");
            }
            return xamlbuilder.ToString();
        }

        private static void WriteXaml(object RootObj,XmlWriter xmlwriter,string Text)
        {
            WriteStartElement(RootObj.GetType(), Text);
            dictionary = new DefaultValueDictionary(RootObj);
            WriteAttributes(RootObj, xmlwriter);
            WriteAttachedProperties(RootObj as DependencyObject);
            WriteBrushes(RootObj as DependencyObject, xmlwriter);
            WriteInnerTags(RootObj as DependencyObject, xmlwriter);
            xmlwriter.WriteEndElement();
        }

        private static void WriteStartElement(Type type,string Text)
        {
            if (type == null)
                return;
            string prefix = type.Namespace.ToLower().Split('.')[0];
#if WPF
            string assemblyname = type.Module.Name.ToLower().Contains("unknown") ? type.Module.ToString().Substring(0, type.Module.ToString().Length - 4) : type.Module.Name;
#else
            string assemblyname = type.GetTypeInfo().Module.Name.ToLower().Contains("unknown") ? type.GetTypeInfo().Module.ToString().Substring(0, type.GetTypeInfo().Module.ToString().Length - 4) : type.GetTypeInfo().Module.Name;
#endif
            if (prefix.ToLower().Contains("system"))
                prefix = string.Empty;
            if (string.IsNullOrEmpty(Text))
                LocalXmlWriter.WriteStartElement(prefix, type.Name, string.IsNullOrEmpty(prefix) ? prefix : "clr-namespace:" + type.Namespace + ";assembly=" + assemblyname );
            else
                LocalXmlWriter.WriteStartElement(prefix, type.Name + "." + Text, string.IsNullOrEmpty(prefix) ? prefix : "clr-namespace:" + type.Namespace + ";assembly=" + assemblyname);
        }

        private static void WriteAttributes(object element, XmlWriter writer)
        {
            PropertyInfo[] properties = element.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "Parent")
                {
                    object getval = property.GetValue(element, null);
                    if (property.Name == "Name")
                    {
                        if ((getval != null) && !string.IsNullOrEmpty(getval.ToString()) && !(element is Path))
                            writer.WriteAttributeString("x", property.Name, "http://schemas.microsoft.com/winfx/2006/xaml", getval.ToString());
                    }
                    else if (getval != null)
                    {
                        if (!property.Name.Contains("ID") && !property.Name.Contains("Matrix") && !getval.ToString().Contains("Collection") && (!getval.ToString().Contains("Windows") || getval.ToString().Contains("Brush")) && !property.Name.ToString().Contains("PathGeometry"))
                        {
#if WPF
                            if ((property.CanWrite && property.GetGetMethod() != null && (property.GetSetMethod() != null)))
#else
                            if ((property.CanWrite && property.GetMethod != null && (property.SetMethod != null)))
#endif
                                if(!(dictionary.HasDefaultValue(property)))
                                    WriteAttribute(property.Name, getval);
                        }
                        else if (getval.ToString().Contains("Value") || (property.Name == "Content" && getval is string))
                        {
                            if (!(dictionary.HasDefaultValue(property)))
                                WriteAttribute(property.Name, getval);
                        }
                    }
                }
            }
        }

        private static void WriteAttribute(string attrName, object getval)
        {
            if ((!getval.ToString().Contains("Brush") && !getval.ToString().Contains("Core")))
                if(!getval.ToString().Contains("Infinity") && !getval.ToString().Contains("NaN"))
                    LocalXmlWriter.WriteAttributeString(attrName, getval.ToString());
        }
        
        private static void WriteAttachedProperties(DependencyObject sourcevalue)
        {
            if (sourcevalue is FrameworkElement)
            {
                if (((FrameworkElement)sourcevalue).Parent is Canvas)
                {
                    if (sourcevalue.GetValue(Canvas.LeftProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Canvas.Left", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Canvas.LeftProperty).ToString());
                    if (sourcevalue.GetValue(Canvas.TopProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Canvas.Top", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Canvas.TopProperty).ToString());
                    if (sourcevalue.GetValue(Canvas.ZIndexProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Canvas.ZIndex", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Canvas.ZIndexProperty).ToString());
                }
                if (((FrameworkElement)sourcevalue).Parent is Grid)
                {
                    if (sourcevalue.GetValue(Grid.RowProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Grid.Row", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Grid.RowProperty).ToString());
                    if (sourcevalue.GetValue(Grid.ColumnProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Grid.Column", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Grid.ColumnProperty).ToString());
                    if (sourcevalue.GetValue(Grid.RowSpanProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Grid.RowSpan", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Grid.RowSpanProperty).ToString());
                    if (sourcevalue.GetValue(Grid.ColumnSpanProperty).ToString() != "0")
                        LocalXmlWriter.WriteAttributeString("x", "Grid.ColumnSpan", "http://schemas.microsoft.com/client/2007", sourcevalue.GetValue(Grid.ColumnSpanProperty).ToString());
                }
            }
            if ((sourcevalue is Timeline) && !(sourcevalue is Storyboard))
            {
                WriteAttribute("Storyboard.TargetName", sourcevalue.GetValue(Storyboard.TargetNameProperty));
                PropertyPath path = sourcevalue.GetValue(Storyboard.TargetPropertyProperty) as PropertyPath;
                if (path != null)
                {
                    LocalXmlWriter.WriteAttributeString("Storyboard.TargetProperty", path.Path);
                }
            }

        }

        private static void WriteBrushes(DependencyObject brushobj,XmlWriter brushwriter)
        {
            PropertyInfo[] properties = brushobj.GetType().GetProperties();
            object value = null;
            foreach (PropertyInfo brushproperty in properties)
            {
                value = brushproperty.GetValue(brushobj, null);
                if (brushproperty.PropertyType.Name == "Brush" && brushproperty.GetValue(brushobj, null) != null)
                {
                    WriteStartElement(brushproperty.DeclaringType, brushproperty.Name);//WriteStartElement(brushproperty.ReflectedType, brushproperty.Name);
                    WriteStartElement(value.GetType(),value.ToString().Split('.')[3]);
                    WriteAttributes(value, brushwriter);
                    brushwriter.WriteEndElement();
                    brushwriter.WriteEndElement();
                }
            }
        }

        private static void WriteInnerTags(DependencyObject InnerTagsObj, XmlWriter tagswriter)
        {
            if (InnerTagsObj != null)
            {
                PropertyInfo[] properties = InnerTagsObj.GetType().GetProperties();
                object item,exobj = null; int itemscount;
                foreach (PropertyInfo prop in properties)
                {
                    exobj = prop.GetValue(InnerTagsObj, null);
#if WPF
                    if ((prop.PropertyType.BaseType != null) && (prop.PropertyType.BaseType.Name == "PresentationFrameworkCollection`1"))
                    {
                        WriteStartElement(prop.ReflectedType, prop.Name);
                        itemscount = (int)prop.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, prop.GetValue(InnerTagsObj, null), null);
                        for (int i = 0; i < itemscount; i++)
                        {
                            item = prop.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, prop.GetValue(InnerTagsObj, null), new object[] { i });
                            WriteXaml(item, tagswriter, "");
                        }
                        tagswriter.WriteEndElement();
                    }
#else
                    if ((prop.PropertyType.GetTypeInfo().BaseType != null) && (prop.PropertyType.GetTypeInfo().BaseType.Name == "PresentationFrameworkCollection`1"))
                    {
                        WriteStartElement(prop.DeclaringType, prop.Name); //WriteStartElement(prop.ReflectedType, prop.Name);
                        //itemscount = (int)prop.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, prop.GetValue(InnerTagsObj, null), null);
                        //for (int i = 0; i < itemscount; i++)
                        //{
                        //    //System.Runtime.CompilerServices.CallSiteBinder binder = Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.InvokeSimpleName, "get_Item", null, prop.PropertyType, null);
                        //    //item = prop.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, prop.GetValue(InnerTagsObj, null), new object[] { i });
                        //    //WriteXaml(item, tagswriter, "");
                        //}
                        tagswriter.WriteEndElement();
                    }
#endif
                    else if (prop.Name == "RenderTransform" && exobj != null)
                    {
                        WriteStartElement(prop.DeclaringType, prop.Name);//WriteStartElement(prop.ReflectedType, prop.Name);
                        WriteXaml(exobj, tagswriter, "");
                        tagswriter.WriteEndElement();
                    }
                    else if (prop.Name == "Content" && exobj != null & !(exobj is string))
                    {
                        WriteStartElement(prop.DeclaringType, prop.Name);//WriteStartElement(prop.ReflectedType, prop.Name);
                        WriteXaml(exobj, tagswriter, "");
                        tagswriter.WriteEndElement();
                    }
                    else if (prop.Name == "ResourceDictionary" && exobj != null)
                    {
                        ResourceDictionary dictionary = prop.GetValue(exobj, null) as ResourceDictionary;
                        if ((dictionary != null) && (dictionary.Keys.Count > 0))
                        {
                            WriteStartElement(prop.DeclaringType, prop.Name); //WriteStartElement(prop.ReflectedType, prop.Name);
                            foreach (object depobj4 in dictionary.Keys)
                            {
                                WriteXaml(depobj4 as DependencyObject, tagswriter, "");
                            }
                            tagswriter.WriteEndElement();
                        }
                    }
                }
            }
        }
        
    }
}
