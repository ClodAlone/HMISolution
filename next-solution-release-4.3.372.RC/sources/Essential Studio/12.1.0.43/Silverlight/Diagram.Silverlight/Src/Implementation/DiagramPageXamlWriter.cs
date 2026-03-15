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

namespace Syncfusion.Windows.Diagram
{
    internal class DiagramPageXamlWriter
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
                if (objType == typeof(DoubleCollection))
                {
                    if (getval != null)
                    {
                        string sb = "";

                        foreach (double d in getval as DoubleCollection)
                        {
                            sb = sb + d.ToString();
                            if ((getval as DoubleCollection).IndexOf(d) != (getval as DoubleCollection).Count - 1)
                            {
                                sb = sb + ",";
                            }
                        }
                        objectValue = sb.ToString();
                    }
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
                            if (!propinfo.Name.Contains("ID") && !propinfo.Name.Equals("Content") && !propinfo.Name.Contains("Matrix") && !getval.ToString().Contains("Collection") && (!getval.ToString().Contains("Windows") || getval.ToString().Contains("Brush")) && !propinfo.Name.ToString().Contains("PathGeometry") || propinfo.Name == "ReferenceID" || propinfo.Name == "SerializationData" || propinfo.Name == "StrokeDashArray")
                            {
                                if ((propinfo.CanWrite && propinfo.GetGetMethod() != null && (propinfo.GetSetMethod() != null)))
                                {
                                    this.WriteAttribute(propinfo.Name, getval);
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
                   
                    if (!(propinfo.Name == "Node" && source.ToString().Contains("ConnectionPort")))
                        if (!propinfo.Name.Contains("Parent") )
                        {
                            if ((propinfo.PropertyType.BaseType != null) && (propinfo.PropertyType.BaseType.Name == "PresentationFrameworkCollection`1") && propinfo.Name != "StrokeDashArray")
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
                            else if (propinfo.Name.Equals("Data") && propinfo.PropertyType==typeof(Geometry))
                            {
                                depobj2 = propinfo.GetValue(source, null);
                                if (depobj2!=null)
                                {
                                    if (depobj2.ToString().EndsWith("Geometry"))
                                    {
                                        depobj3 = (sourceobj != null) ? (propinfo.GetValue(sourceobj, null) as DependencyObject) : null;
                                        this.WriteTag(propinfo, depobj2, depobj3);
                                    } 
                                }
                            }
                            else
                            {
                                if (!((propinfo.Name.Contains("Parent") || propinfo.Name.Contains("Node")) && source is Node))
                                {
                                    if (propinfo.Name == "Child")
                                    {
                                        this.WriteInnerXaml(propinfo.GetValue(source, null));
                                    }
                                    else if (propinfo.PropertyType.Name == "ResourceDictionary")
                                    {
                                        ResourceDictionary dictionary =
                                            propinfo.GetValue(source, null) as ResourceDictionary;
                                        if ((dictionary != null) && (dictionary.Count > 0))
                                        {
                                            this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                                            foreach (object depobj4 in dictionary)
                                            {
                                                this.WriteXaml(depobj4, (DependencyObject) null);
                                            }
                                            this.xmlWriter.WriteEndElement();
                                        }
                                    }
                                    else
                                    {
                                        depobj2 = propinfo.GetValue(source, null);

                                        if (depobj2 != null)
                                        {
                                            depobj3 = ((sourceobj != null) && (source.GetType() == sourceobj.GetType()))
                                                ? propinfo.GetValue(sourceobj, null)
                                                : null;
                                            if ((propinfo.PropertyType == typeof (DataTemplate)) &&
                                                (depobj2 is DataTemplate))
                                            {
                                                this.WriteTag(propinfo, ((DataTemplate) depobj2).LoadContent(), depobj3);
                                            }
                                            else if (depobj2 is FrameworkElement)
                                            {
                                                if (!(depobj2 is Node && source is Node) && propinfo.Name != "Page" &&
                                                    propinfo.Name != "DataContext" && propinfo.Name != "NodeID" &&
                                                    propinfo.Name != "Rectangle" && !propinfo.Name.Contains("Tail") &&
                                                    !propinfo.Name.Contains("Head") && !propinfo.Name.Contains("Owner"))
                                                {
                                                    Type type = null;
                                                    if (propinfo.Name == "Content")
                                                    {
                                                        if ((source as Node).Content != null)
                                                        {
                                                            type = ((source as Node).Content.GetType());
                                                        }
                                                        if (type != null)
                                                        {
                                                            object o = null;
                                                            try
                                                            {
                                                                o =
                                                                    XamlReader.Load("<" + type.Name +
                                                                                    " xmlns=\"http://schemas.microsoft.com/client/2007\" />");
                                                            }
                                                            catch
                                                            {

                                                            }
                                                            if (o != null)
                                                            {

                                                                this.WriteTag(propinfo, depobj2, depobj3);
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        this.WriteTag(propinfo, depobj2, depobj3);
                                                    }
                                                }
                                            }
                                            else if (source is Node && !(depobj2 is Node) && propinfo.Name == "Content" &&
                                                     depobj2.GetType()
                                                         .GetProperties()
                                                         .Where(e => (e.CanWrite == true))
                                                         .Count() > 0)
                                            {
                                                this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                                                this.WriteInnerXaml(depobj2);
                                                this.xmlWriter.WriteEndElement();
                                            }
                                            else if ((propinfo.PropertyType.BaseType != null &&
                                                      propinfo.PropertyType.BaseType.Name == "Collection`1") ||
                                                     propinfo.Name == "GroupChildrenRef")
                                            {
                                                this.WriteStartElement(propinfo.ReflectedType, propinfo.Name);
                                                int count =
                                                    (int)
                                                        propinfo.PropertyType.InvokeMember("get_Count",
                                                            BindingFlags.InvokeMethod, null,
                                                            propinfo.GetValue(source, null), null);
                                                count1 = ((sourceobj == null) ||
                                                          (source.GetType() != sourceobj.GetType()))
                                                    ? 0
                                                    : ((int)
                                                        propinfo.PropertyType.InvokeMember("get_Count",
                                                            BindingFlags.InvokeMethod, null,
                                                            propinfo.GetValue(sourceobj, null), null));

                                                for (int i = 0; i < count; i++)
                                                {
                                                    depobj2 = propinfo.PropertyType.InvokeMember("get_Item",
                                                        BindingFlags.InvokeMethod, null,
                                                        propinfo.GetValue(source, null), new object[] {i});
                                                    if ((sourceobj != null) && (i < count1))
                                                    {
                                                        depobj3 = propinfo.PropertyType.InvokeMember(
                                                            "get_Item", BindingFlags.InvokeMethod, null,
                                                            propinfo.GetValue(sourceobj, null), new object[] {i});
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
                                            else if (depobj2 is PortStyle || depobj2 is BitmapImage ||
                                                     depobj2 is CustomPathStyle || depobj2 is LineStyle ||
                                                     depobj2 is DecoratorStyle)
                                            {
                                                this.WriteTag(propinfo, depobj2, depobj3);
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

        private void writeAttachedProperties(DependencyObject source, DependencyObject sourceobj)
        {
            
            if (source is FrameworkElement)
            {
                if (((FrameworkElement)source).Parent is Canvas)
                {
                    if (source.GetValue(Canvas.LeftProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Canvas.Left", "http://schemas.microsoft.com/client/2007", source.GetValue(Canvas.LeftProperty).ToString());

                    }
                    if (source.GetValue(Canvas.TopProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Canvas.Top", "http://schemas.microsoft.com/client/2007", source.GetValue(Canvas.TopProperty).ToString());

                    }
                    if (source.GetValue(Canvas.ZIndexProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Canvas.ZIndex", "http://schemas.microsoft.com/client/2007", source.GetValue(Canvas.ZIndexProperty).ToString());

                    }
                }
                if (((FrameworkElement)source).Parent is Grid)
                {
                    if (source.GetValue(Grid.RowProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Grid.Row", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.RowProperty).ToString());

                    }
                    if (source.GetValue(Grid.ColumnProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Grid.Column", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.ColumnProperty).ToString());
                    }
                    if (source.GetValue(Grid.RowSpanProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Grid.RowSpan", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.RowSpanProperty).ToString());
                    }
                    if (source.GetValue(Grid.ColumnSpanProperty).ToString() != "0")
                    {
                        this.xmlWriter.WriteAttributeString("x", "Grid.ColumnSpan", "http://schemas.microsoft.com/client/2007", source.GetValue(Grid.ColumnSpanProperty).ToString());
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
                    this.xmlWriter.WriteStartElement(localName, "clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight");
                }
                else
                {
                    object o = null;
                    try
                    {
                        o = XamlReader.Load("<" + type.Name + " xmlns=\"http://schemas.microsoft.com/client/2007\" />");
                    }
                    catch { }
                    if (o != null)
                    {
                        this.xmlWriter.WriteStartElement("x", localName, "http://schemas.microsoft.com/client/2007");
                        return;
                    }
                    try
                    {
                        o = XamlReader.Load("<x:" + type.Name + " xmlns:x = \"clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(',')) + "\"/>");
                    }
                    catch { }
                    this.xmlWriter.WriteStartElement("x", localName, "clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(',')));
                }
            }
            else
            {
                if (type.Namespace.Contains("Syncfusion"))
                {
                    this.xmlWriter.WriteStartElement(localName + "." + ext, "clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight");
                }
                else
                {
                    this.xmlWriter.WriteStartElement("x", localName + "." + ext, "http://schemas.microsoft.com/client/2007");
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
                string str2 = " xmlns=\"http://schemas.microsoft.com/client/2007\"";
                string str3 = "<p:" + type.Name + str2 + str + "/>";
                if (root is UserControl)
                {
                    str3 = "<UserControl" + str2 + "/>";
                }
                try
                {
                    rootobj = XamlReader.Load(str3) as DependencyObject;
                }
                catch
                {
                    rootobj = Activator.CreateInstance(root.GetType()) as DependencyObject;
                }

                this.WriteXaml(root, rootobj);
            }
        }

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

    }
}




