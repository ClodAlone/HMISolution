using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using System.Xml;
#if !WINDOWS_UWP
using System.Windows.Markup;
using System.Runtime.Serialization;
using Microsoft.Xaml.Tools.XamlDom;
using System.Windows.Controls;
using System.Windows.Baml2006;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Controls;
#endif

namespace Utilities.WPF
{
    public static class XmlHelper
    {
        const String tagString = "ProblematicXamlWriter";
#if !WINDOWS_UWP
        const String tagAssembly = "assembly=";

        public static IEnumerable<String> GetAssemblyListInXaml(String xamlCode)
        {
            List<String> list = new List<String>();

            String fileTemp = Path.GetTempFileName();
            File.WriteAllText(fileTemp, xamlCode);
            XamlDomObject rootObject = XamlDomServices.Load(fileTemp);

            foreach (var v in rootObject.GetNamespacePrefixes())
            {
                if (v.Namespace.Contains(tagAssembly))
                {
                    int nPos = v.Namespace.LastIndexOf(tagAssembly);
                    list.Add(v.Namespace.Substring(nPos + tagAssembly.Length));
                }
            }
            File.Delete(fileTemp);

            return list;
        }
#endif
        public static UIElement ReadUIElement(this String xamlCode, String Folder = null)
        {
#if !WINDOWS_UWP
            using (var reader = new StringReader(xamlCode))
            {
                using (var textReader = new XmlTextReader(reader))
                {
                    if (String.IsNullOrEmpty(Folder))
                        return XamlReader.Load(textReader) as UIElement;
                    else
                    {
                        var ms = new MemoryStream(xamlCode.Length);
                        var sw = new StreamWriter(ms);
                        sw.Write(xamlCode);
                        sw.Flush();

                        ms.Seek(0, SeekOrigin.Begin);

                        ParserContext pc = new ParserContext
                        {
                            // System.IO.Packaging.PackUriHelper.Create()
                            BaseUri = new Uri(String.Format("{0}/", Folder))
                        };

                        return XamlReader.Load(ms, pc) as UIElement;
                    }
                }
            }
#else
            return XamlReader.Load(xamlCode) as UIElement;
#endif
        }

#if !WINDOWS_UWP
        public static T LoadBaml<T>(String bamlFile)
        {
            using (var fs = new FileStream(bamlFile, FileMode.Open, FileAccess.Read))
            {
                var reader = new Baml2006Reader(fs);
                using (var writer = new System.Xaml.XamlObjectWriter(reader.SchemaContext))
                {
                    while (reader.Read())
                    {
                        writer.WriteNode(reader);
                    }
                    return (T)writer.Result;
                }
            }
        }

        public static T LoadBaml<T>(byte[] bamlData)
        {
            using (var fs = new MemoryStream(bamlData))
            {
                var reader = new Baml2006Reader(fs);
                using (var writer = new System.Xaml.XamlObjectWriter(reader.SchemaContext))
                {
                    while (reader.Read())
                    {
                        writer.WriteNode(reader);
                    }
                    return (T)writer.Result;
                }
            }
        }
#endif
        public static bool IsProblematicXamlWriter(UIElement uie)
        {
            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as UIElement;

            if (uie is FrameworkElement && (uie as FrameworkElement).Tag is String && ((uie as FrameworkElement).Tag as String) == tagString)
            {
                return true;
            }

            return false;
        }

        public static bool SetProblematicXamlWriter(UIElement uie, String xamlCode)
        {
            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as UIElement;

            if (uie is FrameworkElement)
            {
                (uie as FrameworkElement).Tag = tagString;
                (uie as FrameworkElement).Resources[tagString] = xamlCode;
                return true;
            }

            return false;
        }

        public static bool CheckProblematicXamlWriter(UIElement uie, String xamlCode)
        {
            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as UIElement;

            if (uie is FrameworkElement && (uie as FrameworkElement).Tag is String && ((uie as FrameworkElement).Tag as String) == tagString)
            {
                (uie as FrameworkElement).Resources[tagString] = xamlCode;
                return true;
            }

            return false;
        }

#if !WINDOWS_UWP
        public static String XamlWriterFormatted(this Object uie)
        {
            /*
            using (var stopwatcher2 = new StopWatcher("Cloning using XamlWriter SLOW took : {0}"))
            {
                string xml = null;
                if (uie is FrameworkElement && (uie as FrameworkElement).Tag is String && ((uie as FrameworkElement).Tag as String) == tagString)
                    xml = (uie as FrameworkElement).Resources[tagString] as String;

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Fragment
                };
                XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(sb, settings));
                dsm.XamlWriterMode = XamlWriterMode.Expression;
                XamlWriter.Save(uie, dsm);
                xml = sb.ToString();
            }
            */
#if DEBUG
            // using (var stopwatcher = new StopWatcher("Cloning using XamlWriter took : {0}"))
#endif
            {
                if (uie is FrameworkElement && (uie as FrameworkElement).Tag is String && ((uie as FrameworkElement).Tag as String) == tagString)
                    return (uie as FrameworkElement).Resources[tagString] as String;

                Cloners.InitXamlBinding();
                // return XamlWriterEx.XamlWriter.Save(uie);
                
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Fragment
                };
                XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(sb, settings));
                dsm.XamlWriterMode = XamlWriterMode.Expression;
                XamlWriter.Save(uie, dsm);
                var ret = sb.ToString();
                if (uie is UserControl) // XamlWriter save all the UserControl tree : WRONG !
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(ret);
                    var root = doc.FirstChild;
                    if (root.HasChildNodes)
                    {
                        var nodes = root.ChildNodes;
                        foreach (XmlNode node in nodes)
                            root.RemoveChild(node);
                        // root.RemoveAll();
                    }

                    ret = doc.OuterXml;
                }
                return ret;
            }
        }
#endif
    }
}
