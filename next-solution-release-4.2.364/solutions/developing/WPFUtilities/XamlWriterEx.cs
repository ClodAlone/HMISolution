
using System;

using System.IO;

using System.Xml;

using System.Windows.Markup.Primitives;

using System.Text;

using System.Windows.Markup;

using System.Collections.Generic;

using System.Collections;

using System.Diagnostics;

namespace XamlWriterEx
{

    // Summary:

    // Provides a single static Overload:SXamlWriter.Save method

    // that can be used for limited Extensible Application

    // Markup Language (XAML) serialization of provided runtime objects. This class

    // cannot be inherited, and only has static methods.

    public static class XamlWriter
    {

        // Summary:
        // Returns a Extensible Application Markup Language (XAML) string that serializes
        // the provided object.
        //
        // Parameters:
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // Returns:
        // Extensible Application Markup Language (XAML) string that can be written
        // to a stream or file. The logical tree of all elements that fall under the
        // provided obj element will be serialized.
        //
        // Exceptions:
        // System.Security.SecurityException:
        // the application is not running in full trust.
        //
        // System.ArgumentNullException:
        // obj is null.
        public static string Save(object obj)
        {
            StringBuilder sb = new StringBuilder();
            WriteObject(obj, sb, true);
            return sb.ToString();
        }

        //WriteObject - 3 params (used primarily when isRoot is true or by the 2 param version)
        private static void WriteObject(object obj, StringBuilder sb, bool isRoot)
        {
            WriteObjectWithKey(null, obj, sb, isRoot);
        }

        //WriteObject - 2 param version
        private static void WriteObject(object obj, StringBuilder sb)
        {
            WriteObjectWithKey(null, obj, sb, false);
        }

        //WriteObject - 3 param version
        private static void WriteObjectWithKey(object key, object obj, StringBuilder sb)
        {
            WriteObjectWithKey(key, obj, sb, false);
        }

        private static Dictionary<Type, string> contentProperties = new Dictionary<Type, string>();

        //WriteObject - 4 params (used primarily when isRoot is true or by the 3 param version)
        private static void WriteObjectWithKey(object key, object obj, StringBuilder sb, bool isRoot)
        {
            List<MarkupProperty> propertyElements = new List<MarkupProperty>();
            //If the value is a string
            string s = obj as string;
            if (s != null)
            {
                //TODO: in a dictionary, this should be serialized as a <s:String />
                sb.Append(s);
                return;
            }
            MarkupProperty contentProperty = null;
            string contentPropertyName = null;
            MarkupObject markupObj = MarkupWriter.GetMarkupObjectFor(obj);
            Type objectType = obj.GetType();
            sb.Append("<" + markupObj.ObjectType.Name);
            if (isRoot)
            {
                sb.Append(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            }

            if (key != null)
            {
                string keyString = key.ToString();
                if (keyString.Length > 0)
                    sb.Append(" x:Key=\"" + keyString + "\"");
                else
                    //TODO: key may not be a string, what about x:Type...
                    throw new NotImplementedException("Sample XamlWriter cannot yet handle keys that aren't strings");
            }

            //Look for CPA info in our cache that keeps contentProperty names per Type
            //If it doesn't have an entry, go get the info and store it.
            if (!contentProperties.ContainsKey(objectType))
            {
                string lookedUpContentProperty = string.Empty;
                foreach (Attribute attr in markupObj.Attributes)
                {
                    ContentPropertyAttribute cpa = attr as ContentPropertyAttribute;
                    if (cpa != null)
                        lookedUpContentProperty = cpa.Name;
                }
                contentProperties.Add(objectType, lookedUpContentProperty);
            }
            contentPropertyName = contentProperties[objectType];

            string contentString = string.Empty;
            foreach (MarkupProperty markupProperty in markupObj.Properties)
            {
                if (markupProperty.Name != contentPropertyName)
                {
                    if (markupProperty.IsValueAsString)
                        contentString = markupProperty.Value as string;
                    else if (!markupProperty.IsComposite)
                        sb.Append(" " + markupProperty.Name + "=\"" + markupProperty.Value + "\"");
                    else if (markupProperty.Value.GetType() == typeof(NullExtension))
                        sb.Append(" " + markupProperty.Name + "=\"{x:Null}\"");
                    else
                    {
                        propertyElements.Add(markupProperty);
                    }
                }
                else
                    contentProperty = markupProperty;
            }

            if (contentProperty != null || propertyElements.Count > 0 || contentString != string.Empty)
            {
                sb.Append(">");
                foreach (MarkupProperty markupProp in propertyElements)
                {
                    string propElementName = markupObj.ObjectType.Name + "." + markupProp.Name;
                    sb.Append("<" + propElementName + ">");
                    WriteChildren(sb, markupProp);
                    sb.Append("</" + propElementName + ">");
                }
                if (contentString != string.Empty)
                    sb.Append(contentString);
                else if (contentProperty != null)
                    WriteChildren(sb, contentProperty);
                sb.Append("</" + markupObj.ObjectType.Name + ">");
            }
            else
            {
                sb.Append("/>");
            }
        }

        private static void WriteChildren(StringBuilder sb, MarkupProperty markupProp)
        {
            if (!markupProp.IsComposite)
            {
                XamlWriter.WriteObject(markupProp.Value, sb);
            }
            else
            {
                IList collection = markupProp.Value as IList;
                IDictionary dictionary = markupProp.Value as IDictionary;
                if (collection != null)
                {
                    foreach (object o in collection)
                        XamlWriter.WriteObject(o, sb);
                }
                else if (dictionary != null)
                {
                    foreach (object key in dictionary.Keys)
                    {
                        XamlWriter.WriteObjectWithKey(key, dictionary[key], sb);
                    }
                }
                else
                    XamlWriter.WriteObject(markupProp.Value, sb);
            }
        }


        //
        // Summary:
        // Saves Extensible Application Markup Language (XAML) information into a provided
        // stream to serialize the provided object.
        //
        // Parameters:
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // stream:
        // Destination stream for the serialized XAML information.
        //
        // Exceptions:
        // System.Security.SecurityException:
        // the application is not running in full trust.
        //
        // System.ArgumentNullException:
        // obj is null -or- stream is null.
        public static void Save(object obj, Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            stream.Seek(0, SeekOrigin.Begin); //this line may not be needed.
            writer.Write(Save(obj));
            writer.Flush();
        }
        //
        // Summary:
        // Saves Extensible Application Markup Language (XAML) information as the source
        // for a provided text writer object. The output of the text writer can then
        // be used to serialize the provided object.
        //
        // Parameters:
        // writer:
        // TextWriter instance to use to write the serialized XAML information.
        //
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // Exceptions:
        // System.ArgumentNullException:
        // obj is null -or- writer is null.
        //
        // System.Security.SecurityException:
        // the application is not running in full trust.

        public static void Save(object obj, TextWriter writer) { }
        //
        // Summary:
        // Saves Extensible Application Markup Language (XAML) information as the source
        // for a provided XML writer object. The output of the XML writer can then be
        // used to serialize the provided object.
        //
        // Parameters:
        // obj:
        // The element to be serialized. Typically, this is the root element of a page
        // or application.
        //
        // xmlWriter:
        // Writer to use to write the serialized XAML information.
        //
        // Exceptions:
        // System.ArgumentNullException:
        // obj is null -or- manager is null.
        //
        // System.Security.SecurityException:
        // the application is not running in full trust.

        public static void Save(object obj, XmlWriter xmlWriter) { }

    }

}
