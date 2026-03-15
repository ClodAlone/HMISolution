#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Map
{
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
    using System.Xml;
    using System.Xml.Serialization;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Markup;
    using System.Xml.Linq;
    using System.Linq;
    using System.IO;
    using System.ComponentModel;
    using System.Windows.Media.Imaging;
    using System.Diagnostics;


    /// <summary>
    ///  This helps to Write a Xaml file of the Map
    /// </summary>
    public class MapXamlWriter
    {
        private XmlWriter xmlWriter;
        private void WriteAttribute(string attrName, object getval)
        {

            var objectValue = getval.ToString();
            if (!objectValue.Contains("Brush") && !objectValue.Contains("Core"))
            {
                var objType = getval.GetType();
                if (objType == typeof(System.Double))
                {
                    objectValue = ((double)getval).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                if (objType == typeof(System.Windows.Point))
                {
                    // objectValue.ToString().Replace(';', ',');
                    objectValue = ((Point)getval).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                this.xmlWriter.WriteAttributeString(attrName, objectValue);
            }
        }

        private void writeFrameworkProperties(DependencyObject source, DependencyObject sourceobj)
        {
            try
            {
                if ((sourceobj != null) && (source.GetType() != sourceobj.GetType()))
                {
                    sourceobj = null;
                }
                if (sourceobj == null)
                {
                    sourceobj = Activator.CreateInstance(source.GetType()) as DependencyObject;
                }
                PropertyInfo[] properties = source.GetType().GetProperties();

                foreach (PropertyInfo propinfo in properties)
                {

                    if (source is TextBox)
                    {
                        if (propinfo.Name.Contains("InputScope") || propinfo.Name.Contains("Watermark"))
                        {
                            continue;
                        }
                    }

                    if (propinfo.Name != "Parent")
                    {
                        object getval = propinfo.GetValue(source, null);

                        if (propinfo.Name == "Name")
                        {
                            if ((getval != null) && !string.IsNullOrEmpty(getval.ToString()) && !(source is System.Windows.Shapes.Path))
                            {
                                this.xmlWriter.WriteAttributeString("x", propinfo.Name, "http://schemas.microsoft.com/winfx/2006/xaml", getval.ToString());
                            }
                        }
                        else if (getval != null && !propinfo.Name.Equals("Content"))
                        {
                            if (!propinfo.Name.Contains("ID") && !propinfo.Name.Equals("Content") && !propinfo.Name.Contains("Matrix") && !getval.ToString().Contains("Collection") && (!getval.ToString().Contains("Windows") || getval.ToString().Contains("Brush")) && !propinfo.Name.ToString().Contains("PathGeometry") || propinfo.Name == "ReferenceID")
                            {
                                if ((propinfo.CanWrite && propinfo.GetGetMethod() != null && (propinfo.GetSetMethod() != null)))
                                {
                                    if (propinfo.Name != "GradientStops" && !propinfo.PropertyType.ToString().Contains("Brush") && !propinfo.Name.Contains("Transform"))
                                    {
                                        this.WriteAttribute(propinfo.Name, getval);
                                    }
                                }
                            }
                            else if (getval.ToString().Contains("Value"))
                            {
                                this.WriteAttribute(propinfo.Name, getval);
                            }
                        }
                        else if (propinfo.Name == "Content" && getval != null && getval.GetType().GetProperties().Where(e => (e.CanWrite == true)).Count() == 0)
                        {
                            this.WriteAttribute(propinfo.Name, getval);
                        }
                    }
                }
            }
            catch
            {
            }
        }


        private void writeAttachedProperties(DependencyObject source, DependencyObject sourceobj)
        {
            if (source is FrameworkElement)
            {
                if (((FrameworkElement)source).Parent is Canvas)
                {
                    if (source.GetValue(Canvas.LeftProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Canvas.Left", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Canvas.LeftProperty).ToString());
#else
                           this.xmlWriter.WriteAttributeString("x", "Canvas.Left", "http://schemas.microsoft.com/client/2007", source.GetValue(Canvas.LeftProperty).ToString());

#endif

                    }
                    if (source.GetValue(Canvas.TopProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Canvas.Top", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Canvas.TopProperty).ToString());
#else
                         this.xmlWriter.WriteAttributeString("x", "Canvas.Top", "http://schemas.microsoft.com/client/2007", source.GetValue(Canvas.TopProperty).ToString());

#endif

                    }
                    if (source.GetValue(Canvas.ZIndexProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Canvas.ZIndex", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Canvas.ZIndexProperty).ToString());
#else
                         this.xmlWriter.WriteAttributeString("x", "Canvas.ZIndex", "http://schemas.microsoft.com/client/2007", source.GetValue(Canvas.ZIndexProperty).ToString());

#endif
                    }
                }
                if (((FrameworkElement)source).Parent is Grid)
                {
                    if (source.GetValue(Grid.RowProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Grid.Row", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Grid.RowProperty).ToString());
#else
                        this.xmlWriter.WriteAttributeString("x", "Grid.Row", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.RowProperty).ToString());
#endif

                    }
                    if (source.GetValue(Grid.ColumnProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Grid.Column", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Grid.ColumnProperty).ToString());
#else
                             this.xmlWriter.WriteAttributeString("x", "Grid.Column", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.ColumnProperty).ToString());

#endif
                    }
                    if (source.GetValue(Grid.RowSpanProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Grid.RowSpan", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Grid.RowSpanProperty).ToString());
#else
                        this.xmlWriter.WriteAttributeString("x", "Grid.RowSpan", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.RowSpanProperty).ToString());
#endif
                    }
                    if (source.GetValue(Grid.ColumnSpanProperty).ToString() != "0")
                    {
#if WPF
                        this.xmlWriter.WriteAttributeString("x", "Grid.ColumnSpan", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", source.GetValue(Grid.ColumnSpanProperty).ToString());
#else
                        this.xmlWriter.WriteAttributeString("x", "Grid.ColumnSpan", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.ColumnSpanProperty).ToString());
#endif
                    }
                }
           
                if (((FrameworkElement)source).Parent is ShapeFileCanvas)
                {
                    if (source.GetValue(ShapeFileCanvas.LatitudeProperty).ToString() != string.Empty)
                    {
                        this.xmlWriter.WriteAttributeString("ShapeFileCanvas.Latitude", source.GetValue(ShapeFileCanvas.LatitudeProperty).ToString());
                    }
                    if (source.GetValue(ShapeFileCanvas.LongitudeProperty).ToString() != string.Empty)
                    {
                        this.xmlWriter.WriteAttributeString("ShapeFileCanvas.Longitude", source.GetValue(ShapeFileCanvas.LongitudeProperty).ToString());
                    }
                }
            }
            if ((source is Timeline) && !(source is Storyboard))
            {
                this.WriteAttribute("Storyboard.TargetName", source.GetValue(Storyboard.TargetNameProperty));
                PropertyPath path = source.GetValue(Storyboard.TargetPropertyProperty) as PropertyPath;
                if (path != null)
                {
                    this.xmlWriter.WriteAttributeString("Storyboard.TargetProperty", path.Path);
                }
            }
        }

        private void WriteStartElement(Type type, string ext)
        {
            string localName = type.IsSubclassOf(typeof(UserControl)) ? "UserControl" : type.Name;
            if (string.IsNullOrEmpty(ext))
            {
                if (type.Namespace.Contains("Syncfusion"))
                {
#if WPF
                    this.xmlWriter.WriteStartElement(localName, "clr-namespace:Syncfusion.Windows.Controls.Map;assembly=Syncfusion.Maps.Wpf");
#else
                    this.xmlWriter.WriteStartElement(localName, "clr-namespace:Syncfusion.Windows.Controls.Map;assembly=Syncfusion.Maps.Silverlight");
#endif
                }
                else
                {
                    object o = null;
                    try
                    {
#if WPF
                        var stream = new StringReader("<" + type.Name + " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />");
                        var xmlfile = XmlReader.Create(stream);
                        o = XamlReader.Load(xmlfile);
#else
                          o = XamlReader.Load("<" + type.Name + " xmlns=\"http://schemas.microsoft.com/client/2007\" />");
#endif

                    }
                    catch { }
                    if (o != null)
                    {
                        this.xmlWriter.WriteStartElement("x", localName, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                        return;
                    }
                    try
                    {
#if WPF
                        var stream = new StringReader("<x:" + type.Name + " xmlns:x = \"clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(',')) + "\"/>");
                        var xmlfile = XmlReader.Create(stream);
                        o = XamlReader.Load(xmlfile);
#else
                          o = XamlReader.Load("<x:" + type.Name + " xmlns:x = \"clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(',')) + "\"/>");
#endif
                    }
                    catch { }
                    this.xmlWriter.WriteStartElement("x", localName, "clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(',')));
                }
            }
            else
            {
                if (type.Namespace.Contains("Syncfusion"))
                {
#if WPF
                    this.xmlWriter.WriteStartElement(localName + "." + ext, "clr-namespace:Syncfusion.Windows.Controls.Map;assembly=Syncfusion.Maps.Wpf");
#else
                    this.xmlWriter.WriteStartElement(localName + "." + ext, "clr-namespace:Syncfusion.Windows.Controls.Map;assembly=Syncfusion.Maps.Silverlight");
#endif
                }
                else
                {
                    this.xmlWriter.WriteStartElement("x", localName + "." + ext, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                }
            }
        }

        private void WriteInnerXaml(object root)
        {
            if ((root != null) || (root is DependencyObject))
            {
                Type type = root.GetType();
                DependencyObject rootobj = null;

                string str = type.Module.Name;
                if (type.Module.Name.Contains("Unknown"))
                {
                    str = type.Module.ToString();
                }
                str = " xmlns:p=\"clr-namespace:" + type.Namespace + ";assembly=" + str.Substring(0, str.Length - 4) + "\"";
                string str2 = " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"";
                string str3 = "<p:" + type.Name + str2 + str + "/>";
                if (root is UserControl)
                {
                    str3 = "<UserControl" + str2 + "/>";
                }
                try
                {
#if WPF
                    var stream = new StringReader(str3);
                    var xmlfile = XmlReader.Create(stream);
                     rootobj = XamlReader.Load(xmlfile) as DependencyObject;
#else

                    rootobj = XamlReader.Load(str3) as DependencyObject;
#endif
                }
                catch
                {
                    rootobj = Activator.CreateInstance(root.GetType()) as DependencyObject;
                }

                this.WriteXaml(root, rootobj);
            }
        }

        /// <summary>
        ///  This methos Writes the object as xaml
        /// </summary>
        /// <param name="root">Object</param>
        public string WriteXaml(object root)
        {
            if ((root == null) && !(root is DependencyObject))
            {
                return "";
            }
            StringBuilder XamlString = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            using (this.xmlWriter = XmlWriter.Create(XamlString, settings))
            {
                this.WriteInnerXaml(root);
            }

            return XamlString.ToString();
        }

        private void WriteXaml(object root, DependencyObject rootobj)
        {
            if ((root != null) && (root is DependencyObject))
            {
                this.WriteStartElement(root.GetType(), "");
                this.writeFrameworkProperties((DependencyObject)root, rootobj);
                this.writeAttachedProperties((DependencyObject)root, rootobj);
                this.writeCollections((DependencyObject)root, rootobj);
                this.xmlWriter.WriteEndElement();
            }
            else if (root is Point)
            {
                double x = 0;
                double y = 0;
                PropertyInfo[] propinfo1 = root.GetType().GetProperties();
                foreach (PropertyInfo info in propinfo1)
                {
                    if (info.Name == "X")
                    {
                        x = (double)info.GetValue(root, null);
                    }
                    if (info.Name == "Y")
                    {
                        y = (double)info.GetValue(root, null);
                    }
                }
                this.xmlWriter.WriteRaw("<x:Point xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation" + "\" X=" + "\"" + x.ToString() + "\"" + " Y=" + "\"" + y.ToString() + "\">" + "" + "</x:Point>");
            }
            else
            {
                this.xmlWriter.WriteRaw("<x:Int32 xmlns:x=\"clr-namespace:System;assembly=mscorlib\">" + root.ToString() + "</x:Int32>");
            }
        }

        private void WriteTag(PropertyInfo prop, object obj, object objBlank)
        {
            if (((obj != null) && (obj is DependencyObject)) && ((objBlank == null) || (objBlank is DependencyObject)) || (objBlank is INotifyPropertyChanged))
            {
                this.WriteStartElement(prop.ReflectedType, prop.Name);
                if (prop.PropertyType == typeof(DataTemplate))
                {
                    this.WriteStartElement(prop.PropertyType, "");
                }
                if (objBlank == null)
                {
                    this.WriteInnerXaml(obj);
                }
                else
                {
                    this.WriteXaml(obj, objBlank as DependencyObject);
                }
                if (prop.PropertyType == typeof(DataTemplate))
                {
                    this.xmlWriter.WriteEndElement();
                }
                this.xmlWriter.WriteEndElement();
            }
        }
        private void writeCollections(DependencyObject source, DependencyObject sourceobj)
        {
            if (source != null)
            {
                PropertyInfo[] properties = source.GetType().GetProperties();
                int count1 = 0;
                object depobj2;
                object depobj3;
                foreach (PropertyInfo propinfo in properties)
                {
                    if (source is TextBox)
                    {
                        if (propinfo.Name.Contains("InputScope") || propinfo.Name.Contains("Watermark"))
                        {
                            continue;
                        }
                    }

                    if (!propinfo.Name.Contains("Parent") && propinfo.Name != "StrokeDashArray")
                    {
                        if ((propinfo.PropertyType.BaseType != null) && (propinfo.PropertyType.BaseType.Name == "PresentationFrameworkCollection`1" || propinfo.Name == "GradientStops"))
                        {
                            int count = (int)propinfo.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, propinfo.GetValue(source, null), null);
                            count1 = ((sourceobj == null) || (source.GetType() != sourceobj.GetType())) ? 0 : ((int)propinfo.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, propinfo.GetValue(sourceobj, null), null));
                            if ((((count > 0) && (propinfo.Name != "Children")) && ((propinfo.Name != "Inlines"))) && (propinfo.Name != "Items") && (propinfo.Name != "Setter"))
                            {
                                this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                            }
                            for (int i = 0; i < count; i++)
                            {
                                depobj2 = propinfo.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, propinfo.GetValue(source, null), new object[] { i });
                                if ((sourceobj != null) && (i < count1))
                                {
                                    depobj3 = propinfo.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, propinfo.GetValue(sourceobj, null), new object[] { i });
                                    this.WriteInnerXaml(depobj2);
                                }
                                else
                                {
                                    this.WriteInnerXaml(depobj2);
                                }
                            }
                            if ((((count > 0) && (propinfo.Name != "Children")) && ((propinfo.Name != "Inlines") && (propinfo.Name != "Setter"))) && (propinfo.Name != "Items"))
                            {
                                this.xmlWriter.WriteEndElement();
                            }

                        }
                        else if (propinfo.PropertyType == typeof(Storyboard))
                        {
                            this.WriteInnerXaml(propinfo.GetValue(source, null));
                        }
                        else if (propinfo.Name == "RenderTransform" && !source.ToString().Contains("Brush"))
                        {
                            depobj2 = propinfo.GetValue(source, null);
                            depobj3 = (sourceobj != null) ? (propinfo.GetValue(sourceobj, null) as DependencyObject) : null;
                            this.WriteTag(propinfo, depobj2, depobj3);
                        }
                        else if (propinfo.PropertyType.Name.EndsWith("Brush"))
                        {
                            depobj2 = propinfo.GetValue(source, null);
                            depobj3 = (sourceobj != null) ? (propinfo.GetValue(sourceobj, null) as DependencyObject) : null;
                            this.WriteTag(propinfo, depobj2, depobj3);
                        }
                        else
                        {
                            if (!((propinfo.Name.Contains("Parent") )))
                            {
                                if (propinfo.Name == "Child")
                                {
                                    this.WriteInnerXaml(propinfo.GetValue(source, null));
                                }
                                else if (propinfo.PropertyType.Name == "ResourceDictionary")
                                {
                                    ResourceDictionary dictionary = propinfo.GetValue(source, null) as ResourceDictionary;
                                    if ((dictionary != null) && (dictionary.Count > 0))
                                    {
                                        this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                                        foreach (object depobj4 in dictionary)
                                        {
                                            this.WriteXaml(depobj4, (DependencyObject)null);
                                        }
                                        this.xmlWriter.WriteEndElement();
                                    }
                                }
                                else
                                {
                                    depobj2 = propinfo.GetValue(source, null);

                                    if (depobj2 != null)
                                    {
                                        depobj3 = ((sourceobj != null) && (source.GetType() == sourceobj.GetType())) ? propinfo.GetValue(sourceobj, null) : null;
                                        if ((propinfo.PropertyType == typeof(DataTemplate)) && (depobj2 is DataTemplate))
                                        {
                                            this.WriteTag(propinfo, ((DataTemplate)depobj2).LoadContent(), depobj3);
                                        }
                                        else if (depobj2 is FrameworkElement)
                                        {
                                            this.WriteTag(propinfo, depobj2, depobj3);
                                        }
                                        else if (propinfo.Name == "Content" && depobj2.GetType().GetProperties().Where(e => (e.CanWrite == true)).Count() > 0)
                                        {
                                            this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                                            this.WriteInnerXaml(depobj2);
                                            this.xmlWriter.WriteEndElement();
                                        }
                                        else if ((propinfo.PropertyType.BaseType != null && propinfo.PropertyType.BaseType.Name == "Collection`1") || propinfo.Name == "GroupChildrenRef")
                                        {
                                           
                                            int count = (int)propinfo.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, propinfo.GetValue(source, null), null);
                                            if (count > 0)
                                            {
                                                count1 = ((sourceobj == null) || (source.GetType() != sourceobj.GetType())) ? 0 : ((int)propinfo.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, propinfo.GetValue(sourceobj, null), null));
                                                this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                                                for (int i = 0; i < count; i++)
                                                {
                                                    depobj2 = propinfo.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, propinfo.GetValue(source, null), new object[] { i });
                                                    if ((sourceobj != null) && (i < count1))
                                                    {
                                                        depobj3 = propinfo.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, propinfo.GetValue(sourceobj, null), new object[] { i });
                                                        if (!depobj2.Equals(depobj3))
                                                        {
                                                            this.WriteInnerXaml(depobj2);
                                                        }
                                                    }
                                                    else if (propinfo.Name != "Children")
                                                    {
                                                        this.WriteInnerXaml(depobj2);
                                                    }
                                                }
                                                this.xmlWriter.WriteEndElement();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                if (source is UserControl)
                {
                    for (int j = 0; j < VisualTreeHelper.GetChildrenCount(source); j++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(source, j);
                        if (child is FrameworkElement)
                        {
                            this.WriteInnerXaml(child);
                        }
                    }
                }
            }
        }


    }
}




