using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using ScreenSettings;
using UFInterfaces;
using DocumentManager.ComponentService;
using System.Globalization;
using System.Text;
#if !NET_STANDARD
using STRL;
using Microsoft.Expression.Shapes;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Data;
using Newtonsoft.Json.Linq;
using Utilities.WPF;
using Utilities;
using System.Reflection;
using Newtonsoft.Json;
using OPCUAViewModel;
using UFUAEditor.ComponentService;
using AnimationManager;
using CommandManager;
using System.Xml.Linq;
using ScreenSettings.Documents;
using ScreenSettings.Entities;
using WPFUtilities;
using log4net;
using DevExpress.Xpf.Editors.Helpers;
using ThemeHelper = WPFUtilities.ThemeHelper;
using MSSchedulerSettings.ComponentService;
#endif

namespace SVGHelper
{
    internal class IOsEntity
    {
        public string entity;
        public string x;
        public string y;
        public string width;
        public string height;
        public string nestingFobj;
        public bool forceEncapsulation;
        public bool replaceForeign;
        public bool removeViewbox;
    }
#if !NET_STANDARD

    internal class ElementBoundRectangle
    {
        public Rect rect;
        public bool intersect;
        public bool encapsule;
        public bool cliccable;
    }
#endif
    public class DocSVGHelper: IDisposable
    {
        #region declaration
        static readonly string foreignObject = "foreignObject";
        static readonly string groupObject = "g";
        static readonly String innerEntitySVGNameFormat = "_";
        static readonly String shapeType = "Shape";
        OPCUAEntityReferenceMapList entityMapList = new OPCUAEntityReferenceMapList();
        int styleConter = 0;
        Dictionary<string, string> styleMap = new Dictionary<string, string>();
        Dictionary<string, string> styleXMLMap = new Dictionary<string, string>();
        List<string> styleSymbolsXMLList = new List<string>();
        Dictionary<string, string> referenceDefsXMLNameMap = new Dictionary<string, string>();
        Dictionary<string, string> referenceDefsXMLStyleNameMap = new Dictionary<string, string>();
        Dictionary<string, string> referenceDefsXMLContentMap = new Dictionary<string, string>();
        Dictionary<string, string> referenceDefsXMLStyleContentMap = new Dictionary<string, string>();
        Dictionary<string, string> eumap = new Dictionary<string, string>();
        Dictionary<string, string> filterMapList = new Dictionary<string, string>();
        string cssPrefix;
#if !NET_STANDARD
        private static readonly ILog logDeploy = LogManager.GetLogger(Properties.Resources.SvgGenerator);
        List<string> innercontrolList = new List<string>();
        entities foreignObjectList = new entities();
        Canvas ActiveLayer;
        SVGHelperList sVGHelperList = new SVGHelperList();
        Dictionary<string, SVGHelper> sVGHelpermap = new Dictionary<string, SVGHelper>();
        Dictionary<string, ElementBoundRectangle> overlappedOnCanvas = new Dictionary<string, ElementBoundRectangle>();
#endif
        List<string> referenceBrushList = new List<string>();
        Dictionary<string, string> referenceBrushListMap = new Dictionary<string, string>();
        Dictionary<string, string> referenceClipDefMap = new Dictionary<string, string>();
        Dictionary<string, string> referenceClipStyleMap = new Dictionary<string, string>();
        List<string> entityList = new List<string>();
        Dictionary<string, IOsEntity> iOsEntityList = new Dictionary<string, IOsEntity>();
        XmlNode styleElement;
        XmlDocument xmlDoc;
        XmlDocument iOSxmlDoc;
        ScreenDocument Document;
        IDocument Parent;
        readonly string tagClipStyle = Properties.Settings.Default.ClipStyleTag;
        readonly string tagClipDef = Properties.Settings.Default.ClipDefTag;
        readonly string tagProjectSymbol = Properties.Settings.Default.ProjectSymbolTag;
        readonly string tagSymbol = Properties.Settings.Default.SymbolTag;
        readonly string tagSymbolStyle = Properties.Settings.Default.SymbolStyleTag;
        readonly string tagBaseStyle = Properties.Settings.Default.BaseStyleTag;
        readonly string tagFlatStyle = Properties.Settings.Default.FlatStyleTag;
        XmlElement rootElement;
        XmlElement iOSrootElement;
        XmlNode defsElement;
        List<string> visibleHMIScreenControls;
        bool makeIOSFile;
        #endregion

        #region ctor
        public DocSVGHelper(ScreenDocument screenDocument, List<string> visibleHMIScreenControls, Dictionary<string, string> eumap, string cssPrefix)
        {
#if !NET_STANDARD
            this.visibleHMIScreenControls = visibleHMIScreenControls;
            this.eumap = eumap;
            this.cssPrefix = $"{cssPrefix}_";
            Document = screenDocument ?? throw new ArgumentNullException("ScreenDocument");
            makeIOSFile = true; // Document.FitInWindow;
            Parent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(Document, true);
            InitActiveLayer();

            string webExportExtension = Properties.Settings.Default.WebExportExtension;
            xmlDoc = new XmlDocument();
            iOSxmlDoc = new XmlDocument();
            //XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xmlDoc.DocumentElement;
            var versionLabel = Utilities.AssemblyInfo.FileFormatVersion;
            var revisionLabel = Utilities.AssemblyInfo.FilePrivatePart;
            var productLabel = Utilities.AssemblyInfo.Product;
            //XmlComment newComment = xmlDoc.CreateComment(string.Format(Properties.Resources.SvgGeneratorComment, productLabel, versionLabel, revisionLabel));
            //xmlDoc.InsertBefore(newComment, root);
            //xmlDoc.InsertBefore(xmlDeclaration, newComment);
            rootElement = CreateRootElement();
            
            UpdateEntityList();


            referenceClipDefMap.Values.ToList().ForEach(def =>
            {
                try
                {
                    var node = xmlDoc.ImportNode(def.FromXml<XmlNode>(), true);
                    defsElement.AppendChild(node);
                }
                catch (Exception ex)
                {
                }
            });

            referenceClipStyleMap.Keys.ToList().ForEach(key =>
            {
                var element = $".{key}{{{referenceClipStyleMap[key]}}}";
                styleElement.InnerText = $"{styleElement.InnerText}{Environment.NewLine}\t{element}";
            });

            filterMapList.Values.ToList().ForEach(def =>
            {
                try
                {
                    var node = xmlDoc.ImportNode(def.FromXml<XmlNode>(), true);
                    defsElement.AppendChild(node);
                }
                catch (Exception ex)
                {
                }
            });

            styleXMLMap.Keys.ToList().ForEach(key =>
            {
                var element = $".{key}{{{styleXMLMap[key]}}}";
                styleElement.InnerText = $"{styleElement.InnerText}{Environment.NewLine}\t{element}";
            });
            styleSymbolsXMLList.ForEach(style =>
            {
                styleElement.InnerText = $"{styleElement.InnerText}{Environment.NewLine}\t{style}";
            });
            referenceBrushList.ForEach(element =>
            {
                if (!referenceBrushListMap.ContainsValue(element))
                {
                    var node = xmlDoc.ImportNode(element.FromXml<XmlElement>(), true) as XmlElement;
                    rootElement.AppendChild(node);
                }
            });

            referenceBrushListMap.Values.ToList().ForEach(element =>
            {
                var node = xmlDoc.ImportNode(element.FromXml<XmlNode>(), true);
                defsElement.AppendChild(node);
            });


            var el = rootElement.ToXml();
            if (makeIOSFile)
            {
                iOSrootElement = (XmlElement)iOSxmlDoc.ImportNode(el.FromXml<XmlNode>(), true);
                overlappedOnCanvas.Keys.ToList().ForEach(k =>
                    {
                        Rect rect = overlappedOnCanvas[k].rect; 
                        var list = (from o in overlappedOnCanvas.Keys
                                    where k != o && (!iOsEntityList.ContainsKey(o) ||
                                    (iOsEntityList.ContainsKey(o) && iOsEntityList[o].forceEncapsulation)) &&
                                    rect.IntersectsWith(overlappedOnCanvas[o].rect)
                                    select o ).ToList();
                        list.ForEach(o => overlappedOnCanvas[o].intersect = true);
                        if (list.Count > 0)
                            overlappedOnCanvas[k].intersect = true;
                    });

            }
            entityList.ForEach(element =>
            {
                try
                {
                    var node = xmlDoc.ImportNode(element.FromXml<XmlNode>(), true);
                    rootElement.AppendChild(node);

                    if (makeIOSFile)
                    {
                        if (!iOsEntityList.ContainsKey(element) || 
                        (!iOsEntityList[element].forceEncapsulation && overlappedOnCanvas.ContainsKey(element) && !overlappedOnCanvas[element].intersect && !iOsEntityList[element].replaceForeign))
                            iOSrootElement.AppendChild(iOSxmlDoc.ImportNode(element.FromXml<XmlNode>(), true));
                        else
                        {
                            var cliccable = !overlappedOnCanvas.ContainsKey(element) || overlappedOnCanvas.ContainsKey(element) && 
                                            overlappedOnCanvas[element].cliccable;

                            XmlElement xmlElement = (XmlElement)iOSxmlDoc.ImportNode(iOsEntityList[element].entity.FromXml<XmlNode>(), true);
                            var iOSnode = (overlappedOnCanvas.ContainsKey(element) && overlappedOnCanvas[element].encapsule) || 
                            (iOsEntityList.ContainsKey(element) && iOsEntityList[element].forceEncapsulation && !iOsEntityList[element].replaceForeign) 
                            ? GetEncapsulatedNode(iOsEntityList[element], iOSxmlDoc, cliccable) 
                            : xmlElement;
                            if (iOSnode != null)
                                iOSrootElement.AppendChild(iOSnode);
                            else
                                iOSrootElement.AppendChild(iOSxmlDoc.ImportNode(element.FromXml<XmlNode>(), true));
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }); 

            xmlDoc.AppendChild(rootElement);
            if (makeIOSFile)
                iOSxmlDoc.AppendChild(iOSrootElement);
#endif
        }
#if !NET_STANDARD
        private XmlNode GetEncapsulatedNode(IOsEntity element, XmlDocument xmlDocument, bool cliccable)
        {
            if (element == null || element.entity == null)
                return null;
            XmlElement xmlElement = (XmlElement)xmlDocument.ImportNode(element.entity.FromXml<XmlNode>(), true);
            if (xmlElement.Name == foreignObject && !element.forceEncapsulation)
                return xmlElement;
            XmlNode foreignElement = xmlDocument.CreateElement(string.Empty, foreignObject, string.Empty);
            foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("isComposed", "true", xmlDocument));
            foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", element.x, xmlDocument));
            foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", element.y, xmlDocument));
            foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", element.width, xmlDocument));
            foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", element.height, xmlDocument));
            if(cliccable)
                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("style", "pointer-events: auto;", xmlDocument));
            else
                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("style", "pointer-events: none;", xmlDocument));

            XmlNode div = xmlDocument.CreateElement(string.Empty, "div", string.Empty);
            div.Attributes.Append(JSONHelper.CreateNewAttribute("style", "position:relative; width: 100%; height:100%", xmlDocument));

            string transform = xmlElement.GetAttribute("transform");
            var rotate = string.Empty;
            var style = "overflow: visible; will-change: opacity;";
            if (!string.IsNullOrEmpty(transform) && transform.Contains("rotate"))
            {
                var tlist = transform.Split(')').ToList();
                transform = string.Empty;
                tlist.ForEach(t =>
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        if (!t.Contains("rotate"))
                            transform = $"{transform} {t})";
                        else if(string.IsNullOrEmpty(rotate))
                        {
                            rotate = t.Split('(').LastOrDefault();
                        }
                    }
                });
                if (!string.IsNullOrEmpty(transform))
                    xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute("transform", transform, iOSxmlDoc));
                else
                    xmlElement.Attributes.RemoveNamedItem("transform");

                if (!string.IsNullOrEmpty(rotate))
                {
                    double number;
                    var value = rotate.Split(' ').Where(x => double.TryParse(x, out number)).Select(x => x).FirstOrDefault();
                    if (value != null)
                    {
                        style = $"{style} transform: rotate({value}deg);";
                    }
                };
            }

            XmlNode svg = xmlDocument.CreateElement(string.Empty, "svg", string.Empty);
            svg.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns", "http://www.w3.org/2000/svg", xmlDocument));
            svg.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink", xmlDocument));
            
            if (!element.removeViewbox)
                svg.Attributes.Append(JSONHelper.CreateNewAttribute("viewBox", $"0 0 {element.width} {element.height}", xmlDocument));
            svg.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", xmlDocument));
            if (!element.removeViewbox)
            {
                svg.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", xmlDocument));
                svg.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", xmlDocument));
            }
            svg.Attributes.Append(JSONHelper.CreateNewAttribute("style", style, xmlDocument));

            XmlElement nestingFobj = null;
            if (element.nestingFobj != null) {
                nestingFobj = (XmlElement)xmlDocument.ImportNode(element.nestingFobj.FromXml<XmlNode>(), true);
                nestingFobj.AppendChild(xmlElement);
            }
            svg.AppendChild(nestingFobj ?? xmlElement);
            div.AppendChild(svg);
            foreignElement.AppendChild(div);

            return foreignElement;
        }
#endif
        private void InitActiveLayer()
        {
#if !NET_STANDARD
            ActiveLayer = Document.GetCurrentXamlDocument();
            bool bDirty = Document.Width == 0 || Document.Height == 0;
            if (Document.Width == 0)
                Document.Width = ActiveLayer.Width;
            if (Document.Height == 0)
                Document.Height = ActiveLayer.Height;
            if (bDirty)
                Document.SaveToFile();
            var theme = Document.Theme;
            if (theme == "None")
                theme = DeployHelper.DefaultDeployTheme;
            ThemeHelper.SetTheme(ActiveLayer, theme);
            Document.LoadResources(ActiveLayer);
            ResourceDictionaryExtensions.AddCommonResources(ActiveLayer);
            Document.RefreshEntityStyleBinding(ActiveLayer, true);
            Document.RemoveDeadEntities(ActiveLayer);
            Document.UpdateRepositoryItems(ActiveLayer, ActiveLayer, bSync: true);
            Document.SetImagesBaseUri(ActiveLayer, true);
#endif
        }
        #endregion

        #region methods

        public static SVGHelperList ImportSVGTagUsed(ScreenDocument document)
        {
            if (document == null)
                return new SVGHelperList();
            var folderPath = $"{System.IO.Path.GetDirectoryName(document.FullPath)}";
            var fileName = $"{document.Title}{ Properties.Settings.Default.EntityReferenceExportExtension}";
            var filePath = System.IO.Path.Combine(folderPath, fileName);
            SVGHelperList sVGHelperList = Utilities.XmlHelper.ImportFromFile<SVGHelperList>(filePath, document.Title);
            return sVGHelperList;
        }
        bool errorMessages = false;
        public bool Save(string fullPath)
        {
#if !NET_STANDARD
            string webExportExtension = Properties.Settings.Default.WebExportExtension;
            string folderPath = System.IO.Path.GetDirectoryName(fullPath);
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            var fileName = $"{Document.Title}{webExportExtension}";
            var filePath = System.IO.Path.Combine(folderPath, fileName);

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            if (xmlDoc != null)
            {
                CreateResourceFiles(folderPath);
                xmlDoc.PreserveWhitespace = true;
                iOSxmlDoc.PreserveWhitespace = true;
                foreach (XmlElement el in xmlDoc.SelectNodes("descendant::*[not(*) and not(normalize-space())]"))
                {
                    el.IsEmpty = false;
                }
                foreach (XmlElement el in iOSxmlDoc.SelectNodes("descendant::*[not(*) and not(normalize-space())]"))
                {
                    el.IsEmpty = false;
                }
                string content = XDocument.Parse(xmlDoc.InnerXml.ToString()).ToString();

                File.WriteAllText(filePath, content);

                var iOSfilePath = System.IO.Path.ChangeExtension(filePath, Properties.Settings.Default.iOSExtension);
                if (makeIOSFile)
                {
                    var doc = XDocument.Parse(iOSxmlDoc.InnerXml.ToString());
                    foreach (var fobj in doc.Descendants(foreignObject))
                    {
                        if (fobj.Ancestors(foreignObject).Count() == 0) //not nested fobj
                        {
                            double x;
                            double y;
                            if (fobj.Attribute("translatedx") != null && double.TryParse(fobj.Attribute("translatedx").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                                fobj.SetAttributeValue("x", x);
                            if (fobj.Attribute("translatedy") != null && double.TryParse(fobj.Attribute("translatedy").Value, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                                fobj.SetAttributeValue("y", y);
                        }
                        else
                            fobj.SetAttributeValue("isNested", "true");
                        fobj.Attribute("translatedx")?.Remove();
                        fobj.Attribute("translatedy")?.Remove();
                    }
                    string iOScontent = doc.ToString().Replace("xmlns=\"\"", "");
                    File.WriteAllText(iOSfilePath, iOScontent);
                }
                else if (File.Exists(iOSfilePath))
                    File.Delete(iOSfilePath);

                filePath = System.IO.Path.ChangeExtension(filePath, Properties.Settings.Default.ControlStyles);
                if (referenceDefsXMLStyleContentMap.Count > 0)
                {
                    xmlDoc.RemoveAll();
                    rootElement.RemoveAll();
                    referenceDefsXMLStyleContentMap.Values.ToList().ForEach(element =>
                    {
                        if (string.IsNullOrEmpty(element))
                            return;
                        XmlElement child = xmlDoc.ImportNode(element.FromXml<XmlElement>(), true) as XmlElement;
                        rootElement.AppendChild(child);
                    });
                    xmlDoc.AppendChild(rootElement);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                        xmlDoc.Save(fs);
                }
                else if (File.Exists(filePath))
                    File.Delete(filePath);

                xmlDoc.RemoveAll();
                rootElement.RemoveAll();
                rootElement = null;
                xmlDoc = null;
            }

#endif
            return errorMessages;
        }

#if !NET_STANDARD
        public class ControlRect
        {
            public Rect rect { get; set; }
            public Brush brush { get; set; }
            public Control control { get; set; }
        }
        List<ControlRect> controlRectList = new List<ControlRect>();
        private void UpdateEntityList()
        {
            (from c in ActiveLayer.GetVisualChildrenOfType<Control>()
             select c).ToList().ForEach(c =>
             {
                 controlRectList.Add(new ControlRect()
                 {
                     rect = new Rect() 
                     {
                         Width = c.Width,
                         Height = c.Height,
                         X = (double)c.GetValue(Canvas.LeftProperty),
                         Y = (double)c.GetValue(Canvas.TopProperty),
                     },
                     brush = c.Background,
                     control = c
                 });
             });

            foreach (FrameworkElement child in ActiveLayer.Children)
            {
                string key = string.IsNullOrEmpty(child?.Name) ? child.Uid : child.Name;
                if(Document.MapScreenEntities.ContainsKey(key))
                    UpdateElements(key);
            }
        }

        private bool IsDeploySupported(Type _type, string key)
        {
            //if (Document.ProjectType != ProjectType.WebHMI.ToString())
            //    return true;
            if (Document.MapScreenEntities.ContainsKey(key) && Document.MapScreenEntities[key].Is3DElement)
                _type = typeof(Viewport3D);

            string typeName = _type.Name;
            var customObjectSvgAttribute = (Utilities.SvgValueConverterAttribute)_type.GetCustomAttributes(typeof(Utilities.SvgValueConverterAttribute), true).FirstOrDefault() as Utilities.SvgValueConverterAttribute;
            if (customObjectSvgAttribute != null && !string.IsNullOrEmpty(customObjectSvgAttribute.TypeName))
                typeName = customObjectSvgAttribute.TypeName;
            bool isDeploySupported = visibleHMIScreenControls.Contains(typeName);
            if (!isDeploySupported)
            {
                logDeploy.Error(string.Format(Properties.Resources.WebHMIControlNotSupportedWarning, Document.CleanInnerName(key, innerEntitySVGNameFormat), Document.FilePath));
                errorMessages = true;
            }
            return isDeploySupported;
        }

        private void UpdateElements(string elementName)
        {
            //if (Document.IsInnerEntity(elementName) && !innercontrolList.Contains(elementName))
            //{
            //    AddInnerControl(elementName);
            //}
            //else if (!innercontrolList.Contains(elementName))
            {
                var list = GetListInners(elementName, 1);
                if(list.Count > 0 && !Document.MapScreenEntities[elementName].SourceSymbolLinked)
                    AddInnerControl(elementName);
                else
                    UpdateElementItems(elementName);
            }
        }

        private void AddInnerControl(string elementName)
        {
            var parent = Document.FindParentInnerControl(ActiveLayer, elementName);
            int deepSearch = 1;
            bool hasIosChild = false;
            bool mustBeReplaced = false;
            if (parent != null && Document.MapScreenEntities.ContainsKey(parent.Name))
            {
                if (!innercontrolList.Contains(parent.Name))
                {
                    var _uieElement = GetUpdatedElementItems(parent.Name);
                    XmlDocument doc = new XmlDocument();
                    if (_uieElement == null)
                        return;
                    XmlNode uieElement = doc.ImportNode(_uieElement, true);
                    XmlNode iOSuieElement = (XmlNode)iOSxmlDoc.ImportNode(uieElement.ToXml().FromXml<XmlNode>(), true);
                    Point translationPoint = GetTranslationPoint(iOSuieElement as XmlElement);
                    var list = GetListInners(parent.Name, deepSearch);
                    deepSearch += 1;
                    if(list.Count > 0)
                    {
                        var checkinner = String.Format("{0}{1}", parent.Name, ScreenDocument.innerEntityNameFormat);
                        List<FrameworkElement> controlList = GetControlMap(parent);
                        controlList.ForEach(control =>
                        {
                            string controlName = string.IsNullOrEmpty(control.Name) ? control.Uid : control.Name;
                            string name = $"{checkinner}{controlName}";
                            if (innercontrolList.Contains(name))
                                return;
                            if (list.Contains(name))
                            {
                                var _list = GetListInners(name, deepSearch);
                                if (_list.Count() == 0)
                                {
                                    XmlNode childElement = GetElement(name);
                                    if (childElement != null)
                                    {
                                        var childentity = Document.MapScreenEntities[name];
                                        uieElement.AppendChild(doc.ImportNode(childElement, true));
                                        bool _hasChild;
                                        var iosChild = GetIosChild(childElement, name, translationPoint, out _hasChild, out mustBeReplaced);
                                        if(iosChild != null)
                                        {
                                            iOSuieElement.AppendChild(iOSxmlDoc.ImportNode(iosChild, true));
                                            if (_hasChild)
                                                hasIosChild = true;
                                        }
                                    }
                                } 
                                else
                                {
                                    bool _hasChild;
                                    XmlNode childElement = AddInners(control, name, deepSearch, iOSuieElement, translationPoint, out _hasChild);
                                    if (childElement != null)
                                    {
                                        var childentity = Document.MapScreenEntities[name];
                                        uieElement.AppendChild(doc.ImportNode(childElement, true));
                                        if (_hasChild)
                                            hasIosChild = true;
                                    }
                                }
                                if (!innercontrolList.Contains(name))
                                    innercontrolList.Add(name);
                            }
                        });
                    }

                    if (!innercontrolList.Contains(parent.Name))
                        innercontrolList.Add(parent.Name);

                    entityList.Add(uieElement.ToXml());

                    AddBoundRectangle(uieElement, parent, false);
                    string el = uieElement.ToXml();
                    if (hasIosChild || mustBeReplaced)
                        iOsEntityList.Add(el, new IOsEntity()
                        {
                            entity = iOSuieElement.ToXml(),
                            x = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Left, true),
                            y = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Top, true),
                            width = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Width, true),
                            height = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Height, true),
                            replaceForeign = mustBeReplaced
                        });
                }
            }
        }

        Point GetTranslationPoint(XmlElement xmlElement)
        {
            Point point = new Point(0, 0);
            string transform = xmlElement.GetAttribute("transform");
            if (!string.IsNullOrEmpty(transform) && transform.Contains("translate"))
            {
                var tlist = transform.Split(')').ToList();
                transform = string.Empty;
                var translate = string.Empty;
                tlist.ForEach(t =>
                {
                    if (string.IsNullOrEmpty(translate) && !string.IsNullOrEmpty(t))
                    {
                        if (t.Contains("translate"))
                        {
                            translate = t.Split('(').LastOrDefault();
                            return;
                        }
                    }
                });

                if (!string.IsNullOrEmpty(translate))
                {
                    double number;
                    var values = translate.Split(' ').Where(x => double.TryParse(x, out number)).Select(x => x);
                    if(values.Count() == 2)
                    {
                        point.X = double.Parse(values.First(), CultureInfo.InvariantCulture);
                        point.Y = double.Parse(values.Last(), CultureInfo.InvariantCulture);
                    }
                };
            }
            return point;
        }

        private XmlNode GetIosChild(XmlNode childElement, string entityName, Point translationPoint, out bool hasIosChild, out bool mustBeReplaced)
        {
            hasIosChild = false;
            mustBeReplaced = false;
            ScreenEntity childentity = null;
            
            if(Document.MapScreenEntities.ContainsKey(entityName))
                childentity = Document.MapScreenEntities[entityName];
            if (childentity == null)
                return null;

            if (iOsEntityList.ContainsKey(childElement.ToXml()))
            {
                var fe = childentity.Element as FrameworkElement;
                var cliccable = GetCliccableForiOS(fe, childentity);
                var iOSnode = GetEncapsulatedNode(iOsEntityList[childElement.ToXml()], iOSxmlDoc, cliccable);
                if (iOSnode != null)
                {
                    //if (addNestedAttribute)
                    //    iOSnode.Attributes.Append(JSONHelper.CreateNewAttribute($"isNested", $"true", iOSxmlDoc));
                    //else 
                    {
                        if (translationPoint.X != 0 || translationPoint.Y != 0)
                        {
                            double x;
                            double y;
                            if (double.TryParse((iOSnode as XmlElement).GetAttribute("x"), NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                                translationPoint.X += x;

                            if (double.TryParse((iOSnode as XmlElement).GetAttribute("y"), NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                                translationPoint.Y += y;
                            iOSnode.Attributes.Append(JSONHelper.CreateNewAttribute($"translatedx", $"{JSONHelper.GetValue(translationPoint.X, true)}", iOSxmlDoc));
                            iOSnode.Attributes.Append(JSONHelper.CreateNewAttribute($"translatedy", $"{JSONHelper.GetValue(translationPoint.Y, true)}", iOSxmlDoc));
                        }
                    }
                    hasIosChild = true;
                    return iOSnode;
                }
                else
                    return iOSxmlDoc.ImportNode(childElement.ToXml().FromXml<XmlNode>(), true); 
            }
            else
            {
                var iosChild = iOSxmlDoc.ImportNode(childElement.ToXml().FromXml<XmlNode>(), true);
                if ((iosChild.Name == foreignObject || iosChild.Name == groupObject))
                {
                    if (translationPoint.X != 0 || translationPoint.Y != 0)
                    {
                        double x;
                        double y;
                        if (double.TryParse((iosChild as XmlElement).GetAttribute("x"), NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                            translationPoint.X += x;

                        if (double.TryParse((iosChild as XmlElement).GetAttribute("y"), NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                            translationPoint.Y += y;
                        iosChild.Attributes.Append(JSONHelper.CreateNewAttribute($"x", $"{JSONHelper.GetValue(translationPoint.X, true)}", iOSxmlDoc));
                        iosChild.Attributes.Append(JSONHelper.CreateNewAttribute($"y", $"{JSONHelper.GetValue(translationPoint.Y, true)}", iOSxmlDoc));
                    }

                    mustBeReplaced = true;
                }

                return iosChild;
            }

            return null;
        }

        private void AddBoundRectangle(XmlNode uieElement, UIElement parent, bool encapsule = true)
        {
            if (!overlappedOnCanvas.ContainsKey(uieElement.ToXml()))
            {
                Rect rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { parent }, ActiveLayer);
                FrameworkElement fe = parent as FrameworkElement;
                if(iOsEntityList.ContainsKey(uieElement.ToXml()))
                {
                    rect.Width = double.Parse(iOsEntityList[uieElement.ToXml()].width, NumberFormatInfo.InvariantInfo);
                    rect.Height = double.Parse(iOsEntityList[uieElement.ToXml()].height, NumberFormatInfo.InvariantInfo);
                }
                else
                {
                    if (double.IsNaN(fe.Width) && (uieElement as XmlElement).GetAttributeNode("width") != null)
                        rect.Width = double.Parse((uieElement as XmlElement).GetAttributeNode("width").Value, NumberFormatInfo.InvariantInfo);
                    else
                        rect.Width = fe.Width;

                    if (double.IsNaN(fe.Height) && (uieElement as XmlElement).GetAttributeNode("height") != null)
                        rect.Height = double.Parse((uieElement as XmlElement).GetAttributeNode("height").Value, NumberFormatInfo.InvariantInfo);
                    else
                        rect.Height = fe.Height;
                }

                string name = Document.GetEntityName(fe, false);
                bool cliccable = GetCliccableForiOS(fe, name);
                overlappedOnCanvas.Add(uieElement.ToXml(), new ElementBoundRectangle() { rect = rect, encapsule = encapsule, cliccable = cliccable });
            }
        }

        bool GetCliccableForiOS(FrameworkElement fe,string name)
        {
            bool cliccable = fe.IsHitTestVisible;
            if (!string.IsNullOrEmpty(name) && Document.MapScreenEntities.ContainsKey(name))
            {
                var entity = Document.MapScreenEntities[name];
                cliccable = GetCliccableForiOS(fe, entity);
            }
            return cliccable;
        }

        bool GetCliccableForiOS(FrameworkElement fe, ScreenEntity entity)
        {
            bool cliccable = fe.IsHitTestVisible;
            if (entity != null)
            {
                if (entity.SourceSymbolLinked && fe is ContentControl && !(fe is UserControl) && (fe as ContentControl).Content is FrameworkElement)
                    fe = (fe as ContentControl).Content as FrameworkElement;
                cliccable = fe.IsHitTestVisible && (entity?.CommandList as CommandManagerList).Count() > 0;
            }
            return cliccable;
        }

        private List<FrameworkElement> GetControlMap(FrameworkElement parent)
        {
            List<FrameworkElement> map = new List<FrameworkElement>();
            if (parent is Panel)
            {
                Panel g = parent as Panel;
                foreach (FrameworkElement c in g.Children)
                {
                    map.Add(c);
                }
            }
            else if (parent is Expander || parent is GroupBox)
            {
                ContentControl g = parent as ContentControl;
                FrameworkElement child = g.Content as FrameworkElement;
                if (child == null)
                    return map;
                return GetControlMap(child);
            }
            else if (parent is Decorator && !(parent is Viewbox))
            {
                Decorator g = parent as Decorator;
                FrameworkElement child = g.Child as FrameworkElement;
                if (child == null)
                    return map;
                return GetControlMap(child);
            }
            else if(parent is Viewbox)
            {
                Viewbox g = parent as Viewbox;
                Canvas gc = g.Child as Canvas;
                if (gc == null && (g.Child as FrameworkElement) != null)
                    map.Add(g.Child as FrameworkElement);
                else
                    foreach (FrameworkElement c in gc.Children)
                    {
                        map.Add(c);
                    }
            }
            else if (parent is ContentControl)
            {
                ContentControl g = parent as ContentControl;
                if (g.Content is FrameworkElement)
                    map.Add(g.Content as FrameworkElement);
            }
            return map;
        }

        private List<String> GetListInners(string elementName, int deepSearch)
        {
            var checkinner = String.Format("{0}{1}", elementName, ScreenDocument.innerEntityNameFormat);
            if(deepSearch == -1)
                return (from c in Document.MapScreenEntities.Keys.AsParallel()
                        where c.StartsWith(checkinner) 
                        select c).ToList();
            else
                return (from c in Document.MapScreenEntities.Keys.AsParallel()
                              where c.StartsWith(checkinner) && CountStringOccurrences(c,ScreenDocument.innerEntityNameFormat) == deepSearch
                              select c).ToList();
        }
        public int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
        private XmlNode AddInners(FrameworkElement parent, string elementName, int deepSearch, XmlNode iOSrootElement, Point translationPoint, out bool hasIosChild)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode uieElement = null;
            XmlNode iOSuieElement = null;
            bool _hasIosChild = false;
            bool isChild = false;
            hasIosChild = false;
            if (Document.MapScreenEntities.ContainsKey(elementName))
            {
                if (!innercontrolList.Contains(elementName))
                {
                    var _uieElement = GetElement(elementName);
                    if (_uieElement != null)
                    {
                        uieElement = doc.ImportNode(_uieElement, true);
                        Point _translationPoint = GetTranslationPoint(uieElement as XmlElement);
                        translationPoint.X += _translationPoint.X;
                        translationPoint.Y += _translationPoint.Y;

                        bool _mustBeReplaced;
                        iOSuieElement = GetIosChild(uieElement, elementName, translationPoint, out isChild, out _mustBeReplaced);
                    }
                    var list = GetListInners(elementName, deepSearch);
                    deepSearch += 1;
                    bool mustBeReplaced = false;
                    if (list.Count > 0)
                    {
                        var checkinner = String.Format("{0}{1}", parent.Name, ScreenDocument.innerEntityNameFormat);
                        List<FrameworkElement> controlList = GetControlMap(parent);
                        controlList.ForEach(control =>
                        {
                            string controlName = string.IsNullOrEmpty(control.Name) ? control.Uid : control.Name;
                            string name = $"{elementName}{checkinner}{controlName}";
                            if (innercontrolList.Contains(name))
                                return;
                            if (list.Contains(name))
                            {
                                var _list = GetListInners(name, deepSearch);
                                if (_list.Count() == 0)
                                {
                                    XmlNode childElement = GetElement(name);
                                    if (childElement != null)
                                    {
                                        var childentity = Document.MapScreenEntities[name];
                                        uieElement.AppendChild(doc.ImportNode(childElement, true));

                                        bool _hasChild;
                                        var iosChild = GetIosChild(childElement, name, translationPoint, out _hasChild, out mustBeReplaced);
                                        if (iosChild != null && iOSuieElement != null)
                                        {
                                            iOSuieElement.AppendChild(iOSxmlDoc.ImportNode(iosChild, true));
                                            if (_hasChild || mustBeReplaced)
                                                _hasIosChild = true;
                                        }
                                    }
                                }
                                else
                                {
                                    bool _hasChild;
                                    XmlNode childElement = AddInners(control, name, deepSearch, iOSuieElement, translationPoint, out _hasChild);
                                    if (childElement != null)
                                    {
                                        var childentity = Document.MapScreenEntities[name];
                                        uieElement.AppendChild(doc.ImportNode(childElement, true));
                                        if (_hasChild)
                                            _hasIosChild = true;
                                    }
                                }
                                if (!innercontrolList.Contains(name))
                                    innercontrolList.Add(name);
                            }
                        });
                    }
                    if (_hasIosChild)
                        hasIosChild = true;
                    if (uieElement != null)
                    {
                        if (iOSuieElement != null)
                            iOSrootElement.AppendChild(iOSxmlDoc.ImportNode(iOSuieElement, true));

                        AddBoundRectangle(uieElement, parent);
                        string el = uieElement.ToXml();
                        if (hasIosChild || mustBeReplaced)
                            iOsEntityList.Add(el, new IOsEntity()
                            {
                                entity = iOSuieElement.ToXml(),
                                x = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Left, true),
                                y = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Top, true),
                                width = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Width, true),
                                height = JSONHelper.GetValue(overlappedOnCanvas[el].rect.Height, true),
                                replaceForeign = mustBeReplaced
                            });
                    }
                }
            }
            return uieElement;
        }

        private XmlNode GetUpdatedElementItems(string elementName)
        {
            XmlNode uieElement = GetElement(elementName);
            //if (uieElement != null)
            //    entityList.Add(uieElement.ToXml());
            return uieElement;
        }

        private void UpdateElementItems(string elementName)
        {
            XmlNode uieElement = GetElement(elementName);
            if (uieElement != null)
                entityList.Add(uieElement.ToXml());
        }

        private XmlNode CreateNestingFobj(FrameworkElement fe, string elementName, IOsEntity iOSentity, string customType = null, XmlDocument doc = null)
        {
            XmlNode uieElement = null;
            string clearedName = Document.CleanInnerName(elementName, innerEntitySVGNameFormat);
            string typeName;

            if (customType != null)
                typeName = customType;
            else
            {
                Type type = fe.GetType();
                if (!IsDeploySupported(type, elementName))
                    return uieElement;
                typeName = type.Name;
                var customObjectSvgAttribute = (Utilities.SvgValueConverterAttribute)type.GetCustomAttributes(typeof(Utilities.SvgValueConverterAttribute), true).FirstOrDefault() as Utilities.SvgValueConverterAttribute;
                if (customObjectSvgAttribute != null && !string.IsNullOrEmpty(customObjectSvgAttribute.TypeName))
                    typeName = customObjectSvgAttribute.TypeName;
            }

            if (doc == null)
                doc = new XmlDocument();
            //create foreignobject    
            uieElement = doc.CreateElement(string.Empty, foreignObject, string.Empty);

            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("class", $"{typeName}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{clearedName}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", "0", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", "0", doc));

            jsonobject fobj = new jsonobject()
            {
                SVGItemId = clearedName,
                SVGItem = fe,
                parameters = new Dictionary<string, object>()
            };

            foreignObjectList.Add(fobj);
            if (iOSentity != null)
                iOSentity.nestingFobj = uieElement.ToXml();

            return uieElement;
        }

        private XmlNode GetElement(string elementName, FrameworkElement fe,bool deepLoad = false, bool sourceSymbolLinked = false)
        {
            string name = string.IsNullOrEmpty(fe.Name) ? string.IsNullOrEmpty(elementName) ? fe.Uid : elementName : fe.Name;
            string clearedName = Document.CleanInnerName(name, innerEntitySVGNameFormat);
            //Document.UpdateProblematicXamlWriterProperties(fe, elementName);
            bool bProblematic = Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, "");
            if (bProblematic)
                Document.UpdateProblematicXamlWriterProperties(fe, elementName);

            XmlDocument doc = new XmlDocument();
            XmlNode uieElement = null;
            IOsEntity iOSentity = null;
            bool skipId = false;
            if (deepLoad)
            {
                uieElement = GetElement(elementName, fe);
                if ((uieElement as XmlElement).GetAttributeNode("transform") != null)
                    (uieElement as XmlElement).RemoveAttribute("transform");

                (from c in fe.GetVisualChildrenOfType<FrameworkElement>()
                 where c.Opacity != 0
                 select c).ToList().ForEach(child =>
                 {
                     string cname = child.Name;
                     if (string.IsNullOrEmpty(cname))
                         cname = child.Uid;
                     XmlNode childElement = GetElement(cname, child, deepLoad);
                     if (childElement != null)
                         uieElement.AppendChild(doc.ImportNode(childElement, true));
                 });
            }
            else if (sourceSymbolLinked)
            {
                skipId = true;
                uieElement = GetSymbolLinkedElement(fe, elementName, out iOSentity);
            }
            else if ((fe is Border || fe is Shape))
            {
                var shapeDoc = new XmlDocument();
                var innerShape = GetShapeElement(fe, clearedName, out iOSentity, shapeDoc);
                uieElement = CreateNestingFobj(fe, clearedName, iOSentity, shapeType, shapeDoc);
                uieElement.AppendChild(innerShape);
            }
            else if ((fe is Viewbox || fe is Panel || fe is Expander || fe is GroupBox || fe is Decorator))
                uieElement = GetGroupElement(fe);
            else if (fe is Canvas)
                uieElement = GetCanvasElement(fe);
            else if (fe is Control)
            {
                skipId = true;
                uieElement = GetControlElement(fe, elementName);
            }
            else if (fe is ContentControl)
                uieElement = GetContentControlElement(fe, elementName);
            if (uieElement != null)
            {
                XmlNode _uieElement = doc.ImportNode(uieElement, true);
                if (!skipId && !string.IsNullOrEmpty(clearedName))
                    _uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{clearedName}", doc));
                //if ((fe.Tag as string) == tagBackground)
                //    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("tag", $"{svgTagBackground}",xmlDoc));
                //if ((fe.Tag as string) == tagProblematic)
                //    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("tag", $"{tagProblematic}",xmlDoc));

                var opacity = fe.Opacity;
                if (sourceSymbolLinked && fe is ContentControl && !(fe is UserControl) && (fe as ContentControl).Content is FrameworkElement)
                    opacity = ((fe as ContentControl).Content as FrameworkElement).Opacity;
                if (opacity != 1)
                {
                    _uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("opacity", $"{JSONHelper.GetValue(opacity)}", doc));
                }

                if (iOSentity != null)
                    iOsEntityList.Add(_uieElement.ToXml(), iOSentity);
                return _uieElement;
            }

            return uieElement;

        }
        private XmlNode GetElement(string elementName)
        {
            if (!Document.MapScreenEntities.ContainsKey(elementName))
                return null;
            var entity = Document.MapScreenEntities[elementName];
            var fe = entity.Element as FrameworkElement;
            if(fe == null)
                fe = Document.FindInnerControl(ActiveLayer, elementName);
            XmlNode uieElement = null;
            uieElement = GetElement(elementName, fe, false, Document.MapScreenEntities[elementName].SourceSymbolLinked);
            if (uieElement != null && !overlappedOnCanvas.ContainsKey(uieElement.ToXml()))
            {
                AddBoundRectangle(uieElement, fe);
            }

            return uieElement;
        }

        private XmlNode GetGroupElement(FrameworkElement fe)
        {
            XmlNode uieElement = null;
            if (!(fe is Viewbox) & !(fe is Panel) && !(fe is Expander) && !(fe is GroupBox) && !(fe is Decorator))
                return uieElement;
            XmlDocument doc = new XmlDocument();
            uieElement = doc.CreateElement(string.Empty, groupObject, string.Empty);
            if (fe is Viewbox)
            {
                Point scaleFactor = new Point(1, 1);
                Viewbox control = fe as Viewbox;
                if ((fe as Viewbox).Child is Canvas)
                {
                    Canvas canvas = (fe as Viewbox).Child as Canvas;
                    double childWidth = canvas.Width;
                    double childHeight = canvas.Height;
                    double kwidth = childWidth > 0 ? fe.Width / childWidth : 1;
                    double kheight = childHeight > 0 ? fe.Height / childHeight : 1;
                    scaleFactor = new Point(kwidth, kheight);
                }
                var attribute = GetTransform(fe, scaleFactor, true, true, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);
            }
            
            UpdateEffect(doc, uieElement, fe.Effect);
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));

            return uieElement;
        }

        private XmlNode GetCanvasElement(FrameworkElement fe)
        {
            XmlNode uieElement = null;
            if (fe == null || !(fe is Canvas))
                return uieElement;

            XmlDocument doc = new XmlDocument();
            Canvas canvas = fe as Canvas;
            string name = !string.IsNullOrEmpty(canvas.Uid) ? canvas.Uid : canvas.Name;
            uieElement = doc.CreateElement(string.Empty, groupObject, string.Empty);
            if(!string.IsNullOrEmpty(name))
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{name}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));

            var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
            if (attribute != null)
                uieElement.Attributes.Append(attribute);
            UpdateEffect(doc, uieElement, fe.Effect);

            return uieElement;
        }

        private XmlAttribute GetTransform(FrameworkElement fe, Point scaleFactor, bool bTranslate, bool bScale, bool bRotate, XmlDocument doc, bool bEncapsuled = false, double viewboxW = 0, double viewboxH = 0)
        {
            double left = (double)fe.GetValue(Canvas.LeftProperty);
            double top = (double)fe.GetValue(Canvas.TopProperty);

            string translate = string.Empty;
            if (bTranslate && !double.IsNaN(left) && !double.IsNaN(top) && !(left == 0 && top == 0))
                translate = $"translate({JSONHelper.GetValue(left, true)} {JSONHelper.GetValue(top, true)})";
            string scale = string.Empty;
            var scaleFactorX = 1d;
            var scaleFactorY = 1d;
            if (bScale && !double.IsNaN(scaleFactor.X) && !double.IsNaN(scaleFactor.Y) && !(scaleFactor.X == 1 && scaleFactor.Y == 1))
            {
                scaleFactorX = scaleFactor.X;
                scaleFactorY = scaleFactor.Y;
            }
            if (fe.RenderTransform != null && fe.RenderTransform is TransformGroup)
            {
                TransformGroup t = fe.RenderTransform as TransformGroup;
                var scalet = (from _t in t.Children
                               where _t is ScaleTransform
                               select _t).FirstOrDefault() as ScaleTransform;
                if (scalet != null)
                {
                    double deltaTX = -left - fe.Width;
                    double deltaTY = -top - fe.Height;
                    if (scalet.ScaleX == -1 && scalet.ScaleY == 1)
                    {
                        translate = null;
                        if (!bEncapsuled)
                            scale = $"translate({JSONHelper.GetValue(left, true)} {0}) scale({-1 * scaleFactorX} {1 * scaleFactorY}) translate({JSONHelper.GetValue(deltaTX, true)} {0})";
                        else
                            scale = $"scale({JSONHelper.GetValue(-1 * scaleFactorX, true)} {JSONHelper.GetValue(1 * scaleFactorY, true)}) translate({JSONHelper.GetValue(-viewboxW, true)} {0})";
                    }
                    else if (scalet.ScaleX == -1 && scalet.ScaleY == -1)
                    {
                        translate = null;
                        if (!bEncapsuled)
                            scale = $"translate({JSONHelper.GetValue(left, true)} {JSONHelper.GetValue(top, true)}) scale({JSONHelper.GetValue(-1 * scaleFactorX, true)} {JSONHelper.GetValue(-1 * scaleFactorY, true)}) translate({JSONHelper.GetValue(deltaTX, true)} {JSONHelper.GetValue(deltaTY, true)})";
                        else
                            scale = $"scale({JSONHelper.GetValue(-1 * scaleFactorX, true)} {JSONHelper.GetValue(-1 * scaleFactorY, true)}) translate({JSONHelper.GetValue(-viewboxW, true)} {JSONHelper.GetValue(-viewboxH, true)})";
                    }
                    else if (scalet.ScaleX == 1 && scalet.ScaleY == -1)
                    {
                        translate = null;
                        if (!bEncapsuled)
                            scale = $"translate({0} {JSONHelper.GetValue(top, true)}) scale({JSONHelper.GetValue(1 * scaleFactorX, true)} {JSONHelper.GetValue(-1 * scaleFactorY, true)}) translate({0} {JSONHelper.GetValue(deltaTY, true)})";
                        else
                            scale = $"scale({JSONHelper.GetValue(1 * scaleFactorX, true)} {JSONHelper.GetValue(-1 * scaleFactorY, true)}) translate({0} {JSONHelper.GetValue(-viewboxH, true)})";
                    }
                }
            }
            if (bScale && scale == string.Empty && scaleFactorX != 1 && scaleFactorY != 1)
            {
                scale = $"scale({JSONHelper.GetValue(scaleFactorX, true)} {JSONHelper.GetValue(scaleFactorY, true)})";
            }

            string rotate = string.Empty;
            if(bRotate && fe.RenderTransform != null && fe.RenderTransform is TransformGroup)
            {
                TransformGroup t = fe.RenderTransform as TransformGroup;
                var rotatet = (from _t in t.Children
                             where _t is RotateTransform
                             select _t).FirstOrDefault() as RotateTransform;
                if (rotatet != null)
                    rotate = $"rotate({JSONHelper.GetValue(rotatet.Angle)} {JSONHelper.GetValue(left + fe.Width / 2)} {JSONHelper.GetValue(top + fe.Height / 2)})";
            }
            string transform = string.Empty;
            transform = AddAttribute(transform, rotate);
            transform = AddAttribute(transform, translate);
            transform = AddAttribute(transform, scale);
            if (!string.IsNullOrEmpty(transform))
                return JSONHelper.CreateNewAttribute("transform", $"{transform}", doc);
            else return null;
        }

        string AddAttribute(string source, string attribute)
        {
            if (string.IsNullOrEmpty(attribute))
                return source;
            if(string.IsNullOrEmpty(source))
                source = $"{attribute}";
            else
                source = $"{source} {attribute}";
            return source;
        }
        Dictionary<string, Thickness> styledObjectOffsetMap = new Dictionary<string, Thickness>();
        private XmlNode GetSymbolLinkedElement(FrameworkElement fe, string elementName, out IOsEntity iOSentity)
        {
            iOSentity = null;
            XmlNode uieElement = null;
            XmlDocument doc = new XmlDocument();
            if (fe is ContentControl && !innercontrolList.Contains(elementName))
            {
                string name = string.IsNullOrEmpty(fe.Name) ? fe.Uid : fe.Name;
                string clearedName = Document.CleanInnerName(elementName, innerEntitySVGNameFormat);
                ContentControl control = fe as ContentControl;
                if (Document.MapScreenEntities.ContainsKey(elementName) && Document.MapScreenEntities[elementName].SourceSymbolLinked)
                {
                    Type type = control.Content.GetType();
                    if (!IsDeploySupported(type, elementName))
                        return uieElement;

                    var feOpacity = (control.Content as FrameworkElement).Opacity;
                    var entry = Document.MapScreenEntities[elementName];
                    var key = STRL.STRL.GetSymbolStyleKey(entry.SourceSymbolProvider,
                                                            entry.SourceSymbolPath);
                    if (!string.IsNullOrEmpty(entry.SourceSymbolPath) && !String.IsNullOrEmpty(key))
                    {
                        string sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(entry.SourceSymbolPath);
                        List<string> details = sourceSymbolPath?.Split('@').ToList();
                        string baseObject = details?.FirstOrDefault();
                        if (!String.IsNullOrEmpty(baseObject))
                        {
                            try
                            {
                                //Document.UpdateProblematicXamlWriterProperties(basecontrol, name);

                                string symbolPath = details?.LastOrDefault().Replace($"?{key}", "");
                                string symbolON = string.Empty;
                                string symbolOFF = string.Empty;
                                string symbolNULL = string.Empty;
                                if (control.Content is ProgressBar)
                                    symbolPath = symbolPath.Replace($".xaml", $"_{(control.Content as ProgressBar).Orientation.ToString().Substring(0, 1)}.xaml");

                                GetSymbol(symbolPath, key, out symbolON, out symbolOFF, out symbolNULL);

                                XmlNode foreignElement = doc.CreateElement(string.Empty, foreignObject, string.Empty);
                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("class", $"{type.Name}", doc));
                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{clearedName}", doc));
                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"0px", doc));
                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"0px", doc));
                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("style", "pointer-events: auto;", doc));

                                double left = (double)fe.GetValue(Canvas.LeftProperty);
                                double top = (double)fe.GetValue(Canvas.TopProperty);
                                if (!double.IsNaN(left))
                                    foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                                if (!double.IsNaN(left))
                                    foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));

                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
                                foreignElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));

                                uieElement = foreignElement;
                                
                                var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                                if (attribute != null)
                                    uieElement.Attributes.Append(attribute);

                                jsonobject fobj = new jsonobject()
                                {
                                    SVGItemId = clearedName,
                                    SVGItem = control.Content,
                                    parameters = new Dictionary<string, object>() {
                                            { JSONHelper.styleON, $"{System.IO.Path.GetFileNameWithoutExtension(symbolON)}" },
                                            { JSONHelper.styleOFF, $"{System.IO.Path.GetFileNameWithoutExtension(symbolOFF)}" },
                                            { JSONHelper.styleNULL, $"{System.IO.Path.GetFileNameWithoutExtension(symbolNULL)}" },
                                            { JSONHelper.styleLinked, $"true" },
                                            /*{ JSONHelper.styleGuid, $"{key}"}*/ }
                                };

                                if (control.Content is ProgressBar && !string.IsNullOrEmpty(symbolOFF))
                                {
                                    var path = (from p in referenceDefsXMLStyleNameMap.Keys where referenceDefsXMLStyleNameMap[p] == symbolOFF select p).FirstOrDefault();
                                    bool hasDefinition = referenceDefsXMLStyleContentMap.ContainsKey(path);
                                    string xml = referenceDefsXMLStyleContentMap[path];
                                    bool showValue = hasDefinition && xml.Contains(JSONHelper.stylePartValue);
                                    bool showEUnit = hasDefinition && xml.Contains(JSONHelper.stylePartEUnit);
                                    bool forceOrientation = hasDefinition && xml.Contains(JSONHelper.progressBarOrientation);
                                    bool forceAnimation = hasDefinition && xml.Contains(JSONHelper.progressBarAnimation);
                                    if (forceAnimation)
                                    {
                                        var keyElem = $"{JSONHelper.progressBarAnimation}=\"{Animation.Rotate}\"";
                                        Animation animation = xml.Contains(keyElem) ? Animation.Rotate : Animation.Fill;
                                        fobj.parameters.Add("Animation", animation);
                                    }
                                    else
                                        fobj.parameters.Add("Animation", Animation.Fill);

                                    if (forceOrientation)
                                    {
                                        var keyElem = $"{JSONHelper.progressBarOrientation}=\"{Orientation.Vertical}\"";
                                        (control.Content as ProgressBar).Orientation = xml.Contains(keyElem) ? Orientation.Vertical : Orientation.Horizontal;
                                    }

                                    if (showEUnit)
                                    {
                                        var keyElem = $"{path}{JSONHelper.stylePartEUnit}";
                                        Thickness margin = styledObjectOffsetMap.ContainsKey(keyElem) ? styledObjectOffsetMap[keyElem] : GetElementOffset(doc, xml, JSONHelper.stylePartEUnit);
                                        if (!styledObjectOffsetMap.ContainsKey(keyElem))
                                            styledObjectOffsetMap.Add(keyElem, margin);
                                        fobj.parameters.Add("EngeneeringOffset", margin);
                                    }
                                    if (showValue)
                                    {
                                        var keyElem = $"{path}{JSONHelper.stylePartValue}";
                                        Thickness margin = styledObjectOffsetMap.ContainsKey(keyElem) ? styledObjectOffsetMap[keyElem] : GetElementOffset(doc, xml, JSONHelper.stylePartValue);
                                        if (!styledObjectOffsetMap.ContainsKey(keyElem))
                                            styledObjectOffsetMap.Add(keyElem, margin);
                                        fobj.parameters.Add("ValueOffset", margin);
                                    }
                                    fobj.parameters.Add("ShowValue", showValue);
                                    fobj.parameters.Add("ShowEngeneeringUnit", showEUnit);
                                }

                                Dictionary<string, Dictionary<string, object>> urldictNames = new Dictionary<string, Dictionary<string, object>>();
                                if ((control.Content is Control))
                                {
                                    var brush = (control.Content as Control).Background;
                                    if (((control.Content as Control).ReadLocalValue(Control.BackgroundProperty) == DependencyProperty.UnsetValue ||
                                        brush == null))
                                    {
                                        urldictNames.Add("Background", new Dictionary<string, object>() { { "Color", null } });
                                        fobj.parameters.Add(JSONHelper.svgBackground, new Dictionary<string, object>() { { "Color", null } });
                                    }
                                    else
                                    {
                                        if (brush is VisualBrush && (brush as VisualBrush).Visual is MediaElement)
                                        {
                                            string source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                                            var dict = new Dictionary<string, object>() { { "MediaSource", source } };
                                            fobj.parameters.Add(JSONHelper.svgBackground, dict);
                                        }
                                        else
                                        {
                                            var colorname = GetCssClassStyle(brush, ColorMode.Fill, new Size(fe.Width, fe.Height));
                                            if (styleXMLMap.ContainsKey(colorname))
                                            {
                                                try
                                                {
                                                    colorname = styleXMLMap[colorname].Split(';')[0].Split(':').LastOrDefault();
                                                    var dict = new Dictionary<string, object>() { { "Color", colorname } };
                                                    fobj.parameters.Add(JSONHelper.svgBackground, dict);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                    }
                                    
                                    brush = (control.Content as Control).BorderBrush;
                                    if (((control.Content as Control).ReadLocalValue(Control.BorderBrushProperty) == DependencyProperty.UnsetValue ||
                                        brush == null))
                                    {
                                        urldictNames.Add("BorderBrush", new Dictionary<string, object>() { { "Color", null } });
                                    }

                                    fobj.parameters.Add(JSONHelper.isSolidColorBorder, ((control.Content as Control).BorderBrush is SolidColorBrush));


                                    //if ((control.Content is Buttons.CheckBoxControl) && ((control.Content as Buttons.CheckBoxControl).ReadLocalValue(Buttons.CheckBoxControl.BackgroundOnProperty) == DependencyProperty.UnsetValue ||
                                    //    (control.Content as Buttons.CheckBoxControl).BackgroundOn == null))
                                    //{
                                    //    urldictNames.Add("BackgroundOn", new Dictionary<string, object>() { { "Color", null } });
                                    //}

                                    Dictionary<string, Brush> urldict = JSONHelper.GetUrlBrushes((control.Content as FrameworkElement), Document);
                                    urldict?.Keys.ToList().ForEach(urlkey =>
                                    {
                                        if (urldict[urlkey] == null)
                                        {
                                            var dict = new Dictionary<string, object>() { { "Color", null } };
                                            if (!urldictNames.ContainsKey(urlkey))
                                                urldictNames.Add($"{urlkey}", dict);
                                            else
                                                urldictNames[urlkey] = dict;
                                        }
                                        else if (urldict[urlkey] != null)
                                        {
                                            var urlbrush = urldict[urlkey];
                                            if (urlbrush is VisualBrush && (urlbrush as VisualBrush).Visual is MediaElement)
                                            {
                                                string source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                                                var dict = new Dictionary<string, object>() { { "MediaSource", source } };
                                                if (!urldictNames.ContainsKey(urlkey))
                                                    urldictNames.Add(urlkey, dict);
                                                else
                                                    urldictNames[urlkey] = dict;
                                            }
                                            else
                                            {
                                                var colorname = GetCssClassStyle(urldict[urlkey], ColorMode.Fill, new Size(fe.Width, fe.Height));
                                                if (styleXMLMap.ContainsKey(colorname))
                                                {
                                                    try
                                                    {
                                                        colorname = styleXMLMap[colorname].Split(';')[0].Split(':').LastOrDefault();
                                                        var dict = new Dictionary<string, object>() { { "Color", colorname } };
                                                        if (!urldictNames.ContainsKey(urlkey))
                                                            urldictNames.Add(urlkey, dict);
                                                        else
                                                            urldictNames[urlkey] = dict;
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }

                                fobj.urlbrushes = urldictNames;
                                foreignObjectList.Add(fobj);
                            }
                            catch (Exception ex)
                            {
                                Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                                string msg = string.Format(Properties.Resources.MsgWithException,string.Format(Properties.Resources.ErrorExportingSymbolLinked, name, Document.Title),
                                    e.Message);
                                logDeploy.Error(msg);
                                errorMessages = true;
                            }
                        }
                    }
                    else
                    {
                        //static symbols
                        uieElement = doc.CreateElement(string.Empty, groupObject, string.Empty);
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{clearedName}", doc));

                        var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                        if (attribute != null)
                            uieElement.Attributes.Append(attribute);

                        double left = (double)fe.GetValue(Canvas.LeftProperty);
                        double top = (double)fe.GetValue(Canvas.TopProperty);
                        string sourcePath = WPFUtilities.CryptString.CryptString.DecryptString(entry.SourceSymbolPath);
                        if (sourcePath.StartsWith(Utilities.SymbolLibraryPath.ProjectSymbolTag))
                        {
                            string symbolFile = $"{Document.rootBase}\\{Utilities.SymbolLibraryPath.RootSymbolFolder}{sourcePath.Remove(0,Utilities.SymbolLibraryPath.ProjectSymbolTag.Length)}";
                            string svgFile = System.IO.Path.ChangeExtension(symbolFile, Properties.Settings.Default.WebExportExtension);
                            if (System.IO.File.Exists(svgFile))
                            {
                                string symbolname = $"{cssPrefix}{tagProjectSymbol}";
                                XmlElement childElement = GetSymbol(svgFile, symbolname, left, top, fe, out iOSentity);
                                if (childElement != null)
                                {
                                    if (makeIOSFile && iOSentity != null && !string.IsNullOrEmpty(iOSentity.entity))
                                    {
                                        XmlNode iOSuieElement = (XmlNode)iOSxmlDoc.ImportNode(uieElement.ToXml().FromXml<XmlNode>(), true);
                                        XmlNode childNode = iOSxmlDoc.ImportNode(iOSentity.entity.FromXml<XmlNode>(), true);
                                        iOSuieElement.AppendChild(childNode);
                                        UpdateUids(iOSuieElement, name);
                                        if (!string.IsNullOrEmpty(entry.TagBrush) || !string.IsNullOrEmpty(entry.TagPen))
                                            UpdateTagBrush(entry, fe, childNode, iOSxmlDoc);
                                        UpdateEffect(iOSxmlDoc, iOSuieElement, fe.Effect);
                                        if (feOpacity != 1)
                                            iOSuieElement.Attributes.Append(JSONHelper.CreateNewAttribute("opacity", $"{JSONHelper.GetValue(feOpacity)}", iOSxmlDoc));
                                        iOSentity.entity = iOSuieElement.ToXml();
                                    }

                                    XmlNode node = doc.ImportNode(childElement, true);
                                    uieElement.AppendChild(node);
                                    UpdateUids(uieElement, name);
                                    if (!string.IsNullOrEmpty(entry.TagBrush) || !string.IsNullOrEmpty(entry.TagPen))
                                        UpdateTagBrush(entry, fe, node, doc);
                                }
                                else
                                {
                                    logDeploy.Error(string.Format(Properties.Resources.ErrorExportingSymbolLinked, name, Document.Title));
                                    errorMessages = true;
                                }
                            }
                            else
                            {
                                string element = STRL.STRL.GetSymbolElement(entry.SourceSymbolProvider, entry.SourceSymbolPath, Document.rootBase);
                                var basecontrol = element.ReadUIElement(Document.rootBase) as FrameworkElement;
                                //control.Content = basecontrol;

                                XmlElement childElement = ComposeElement(name, control.Content as FrameworkElement,
                                                                         basecontrol.Width, basecontrol.Height,
                                                                         fe.Width, fe.Height,
                                                                         left, top, true);
                                if (childElement != null)
                                {
                                    uieElement.AppendChild(doc.ImportNode(childElement, true));
                                    double dwidth = double.IsNaN(basecontrol.Width) ? fe.Width : basecontrol.Width;
                                    double dheight = double.IsNaN(basecontrol.Height) ? fe.Height : basecontrol.Height;
                                    iOSentity = GetIosElement(childElement, feOpacity, left, top, dwidth, dheight);
                                    if (feOpacity != 1)
                                    {
                                        XmlElement iOSuieElement = (XmlElement)iOSxmlDoc.ImportNode(iOSentity.entity.FromXml<XmlNode>(), true);
                                        iOSuieElement.Attributes.Append(JSONHelper.CreateNewAttribute("opacity", $"{JSONHelper.GetValue(feOpacity)}", iOSxmlDoc));
                                        iOSentity.entity = iOSuieElement.ToXml();
                                    }
                                }
                                else
                                {
                                    logDeploy.Error(string.Format(Properties.Resources.ErrorExportingSymbolLinked, name, Document.Title));
                                    errorMessages = true;
                                }
                            }
                        }
                        else
                        {
                            string rootFolder = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                            string startingOldPath = $"{rootFolder}\\{Properties.Settings.Default.SymbolCommonFolder}\\";
                            string startingPath = $"{rootFolder}\\{Properties.Settings.Default.Symbol4CommonFolder}\\";
                            string svgStartingPath = $"{rootFolder}\\{Properties.Settings.Default.SVGSymbolCommonFolder}\\";
                            sourcePath = sourcePath.Replace(startingOldPath, svgStartingPath);
                            sourcePath = sourcePath.Replace(startingPath, svgStartingPath);

                            sourcePath = System.IO.Path.ChangeExtension(sourcePath, Properties.Settings.Default.WebExportExtension);
                            string filePath = sourcePath;
                            string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
                            string symbolname = $"{cssPrefix}{tagSymbol}"; // UFUAModel.Helpers.NameValidator.EnsureValidName(filename);

#if DEBUG
                            if (!System.IO.File.Exists(filePath))
                            {
                                LogMissingFile(filePath);
                                return null;
                            }
#endif

                            if (System.IO.File.Exists(filePath))
                            {
                                XmlElement childElement = GetSymbol(filePath, symbolname, left, top, fe, out iOSentity);
                                if(childElement != null)
                                {
                                    if (makeIOSFile && iOSentity != null && !string.IsNullOrEmpty(iOSentity.entity))
                                    {
                                        XmlNode iOSuieElement = (XmlNode)iOSxmlDoc.ImportNode(uieElement.ToXml().FromXml<XmlNode>(), true);
                                        XmlNode childNode = iOSxmlDoc.ImportNode(iOSentity.entity.FromXml<XmlNode>(), true);
                                        iOSuieElement.AppendChild(childNode);
                                        UpdateUids(iOSuieElement, name);
                                        if (!string.IsNullOrEmpty(entry.TagBrush) || !string.IsNullOrEmpty(entry.TagPen))
                                            UpdateTagBrush(entry, fe, childNode, iOSxmlDoc);
                                        UpdateEffect(iOSxmlDoc, iOSuieElement, fe.Effect);
                                        if (feOpacity != 1)
                                            iOSuieElement.Attributes.Append(JSONHelper.CreateNewAttribute("opacity", $"{JSONHelper.GetValue(feOpacity)}", iOSxmlDoc));
                                        iOSentity.entity = iOSuieElement.ToXml();
                                    }

                                    XmlNode node = doc.ImportNode(childElement, true);
                                    uieElement.AppendChild(node);
                                    UpdateUids(uieElement, name);
                                    if (!string.IsNullOrEmpty(entry.TagBrush) || !string.IsNullOrEmpty(entry.TagPen))
                                        UpdateTagBrush(entry, fe, node, doc);
                                }
                                else
                                {
                                    logDeploy.Error(string.Format(Properties.Resources.ErrorExportingSymbolLinked, name, Document.Title));
                                    errorMessages = true;
                                }
                            }
                        }
                    }
                    if(uieElement != null)
                        UpdateEffect(doc, uieElement, fe.Effect);
                }
                else
                {
                    fe = (fe as ContentControl).Content as FrameworkElement;
                    if (fe != null)
                        uieElement = GetElement(name, fe);
                }

                //var parent = Document.FindParentInnerControl(ActiveLayer, name);
                //var list = GetListInners(parent.Name, -1);
                //foreach (var el in list)
                //    innercontrolList.Add(el);
                //innercontrolList.Add(parent.Name);
                if (!innercontrolList.Contains(elementName))
                    innercontrolList.Add(elementName);
            }

            return uieElement;
        }

        private Thickness GetElementOffset(XmlDocument doc, string content, string path)
        {
            var fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = JSONHelper.ManageClassIDs(content.FromXml<XmlNode>());
            XmlElement tmpchildElement = null;
            Thickness margin = new Thickness(0);
            foreach (XmlNode child in fragment.ChildNodes)
            {
                if (child.Name == "svg")
                {
                    foreach (XmlNode node in child.ChildNodes)
                    {
                        if (node.Name == "rect")
                        {
                            tmpchildElement = node as XmlElement;
                            if (tmpchildElement.Attributes != null && tmpchildElement.Attributes.GetNamedItem("id") != null &&
                                tmpchildElement.Attributes.GetNamedItem("id").Value == path)
                            {
                                if (tmpchildElement.Attributes.GetNamedItem("x") != null)
                                    margin.Left = double.Parse(tmpchildElement.Attributes.GetNamedItem("x").Value);
                                if (tmpchildElement.Attributes.GetNamedItem("y") != null)
                                    margin.Top = double.Parse(tmpchildElement.Attributes.GetNamedItem("y").Value);
                                break;
                            }
                        }
                    }
                    break;
                }               
            }
            return margin;
        }

        private void UpdateTagBrush(ScreenEntity entry, FrameworkElement fe, XmlNode node, XmlDocument doc)
        {
            string style = null;
            var backBrush = GetTagBrush(entry.TagBrush);
            var strokeBrush = GetTagBrush(entry.TagPen);
            var fillName = backBrush == null ? "none" : GetCssClassStyle(backBrush, ColorMode.Fill, new Size(fe.Width, fe.Height));
            var strokeName = strokeBrush == null ? "none" : GetCssClassStyle(strokeBrush, ColorMode.Stroke, new Size(fe.Width, fe.Height));

            if (styleXMLMap.ContainsKey(fillName))
                style = styleXMLMap[fillName];
            if (styleXMLMap.ContainsKey(strokeName))
                style = style + styleXMLMap[strokeName];
            List<string> styles = style?.Split(';').ToList();
            if (styles != null)
            {
                try
                {
                    Dictionary<string, string> brushMap = new Dictionary<string, string>();
                    foreach (var l in styles)
                    {
                        if (string.IsNullOrEmpty(l))
                            continue;
                        if (l.Split(':').Count() < 2)
                            continue;
                        var k = l.Split(':')[0];
                        var value = l.Split(':')[1];

                        if (!brushMap.ContainsKey(k))
                            brushMap.Add(k, value);
                    }
                    SetTagBrush(doc, node.ChildNodes, brushMap);
                }
                catch (Exception)
                {
                }
            }

        }

        private void SetTagBrush(XmlDocument doc, XmlNodeList childNodes, Dictionary<string, string> brushMap)
        {
            for (int i = 0; i < childNodes.Count; i++)
            {
                var node = childNodes[i];
                if (node.Attributes != null && node.Attributes.GetNamedItem("Tag") != null && 
                    node.Attributes.GetNamedItem("Tag").Value == Properties.Settings.Default.TagBackground)
                {
                    brushMap.Keys.ToList().ForEach(key =>
                    {
                        node.Attributes.RemoveNamedItem(key);
                        node.Attributes.Append(JSONHelper.CreateNewAttribute($"{key}", $"{brushMap[key]}", doc));
                    });
                }
                SetTagBrush(doc, node.ChildNodes, brushMap);
            }
        }

        Brush GetTagBrush(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;
            using (var reader = new StringReader(tag))
            {
                // object obj = s.Deserialize(reader);
                using (var textReader = new XmlTextReader(reader))
                {
                    return System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                }
            }
        }
#if DEBUG
        void LogMissingFile(string missinFile)
        {
            logDeploy.Warn(string.Format(Properties.Resources.ErrorExportingSymbolLinked, missinFile, Document.Title));
        }
#endif
        private XmlElement  GetSymbol(string filePath, string symbolname, double left, double top, FrameworkElement fe, out IOsEntity iOSentity)
        {
            iOSentity = null;
            if (!referenceDefsXMLNameMap.ContainsKey(filePath))
            {
                while (referenceDefsXMLNameMap.Values.Contains(symbolname))
                {
                    symbolname = $"{symbolname}{++styleConter}";
                }
                referenceDefsXMLNameMap.Add(filePath, symbolname);
            }

            XmlElement childElement = null; 
            XmlDocument doc = new XmlDocument();
            if (referenceDefsXMLContentMap.ContainsKey(filePath))
            {
                childElement = doc.ImportNode(referenceDefsXMLContentMap[filePath].FromXml<XmlElement>(), true) as XmlElement;
            }
            else
            {
                try
                {
                    if (!System.IO.File.Exists(filePath))
                    {
#if DEBUG
                        LogMissingFile(filePath);
#endif
                    }
                    string content = File.ReadAllText(filePath);
                    try
                    {
                        content = WPFUtilities.CryptString.CryptString.DecryptString(content);
                    }
                    catch (Exception ex) { }

                    content = JSONHelper.SetStyleIds(content, symbolname);


                    var fragment = doc.CreateDocumentFragment();
                    fragment.InnerXml = JSONHelper.ManageClassIDs(content.FromXml<XmlNode>(), symbolname);
                    XmlElement tmpchildElement = null;
                    foreach (XmlNode child in fragment.ChildNodes)
                    {
                        if (child.Name == "svg")
                        {
                            tmpchildElement = child as XmlElement;
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", doc));
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", doc));
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", doc));
                            break;
                        }
                    }
                    if (tmpchildElement != null)
                    {
                        //   defsElement.AppendChild(childElement);
                        XmlNodeList styles = tmpchildElement.GetElementsByTagName("style");
                        for (int i = 0; i < styles.Count; i++)
                        {
                            styleSymbolsXMLList.Add(styles[i].InnerText);
                        }
                        XmlNodeList gradients = tmpchildElement.GetElementsByTagName("linearGradient");
                        for (int i = 0; i < gradients.Count; i++)
                        {
                            referenceBrushList.Add((gradients[i] as XmlElement).ToXml());
                        }
                        gradients = tmpchildElement.GetElementsByTagName("radialGradient");
                        for (int i = 0; i < gradients.Count; i++)
                        {
                            referenceBrushList.Add((gradients[i] as XmlElement).ToXml());
                        }

                        ResolveUids(tmpchildElement, innerSymbolName);
                        referenceDefsXMLContentMap.Add(filePath, tmpchildElement.ToXml());
                        childElement = tmpchildElement;
                    }
                }
                catch (Exception ex)
                {
                    Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                    string msg = string.Format(Properties.Resources.MsgWithException,
                        string.Format(Properties.Resources.ErrorGettingSymbol, Document.Title, symbolname, filePath),
                        e.Message);
                    logDeploy.Error(msg);
                    errorMessages = true;
                }

            }

            if (childElement != null)
            {
                if (childElement.Attributes != null)
                    childElement.Attributes.RemoveNamedItem("id");
                if (!double.IsNaN(left))
                    childElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                if (!double.IsNaN(top))
                    childElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));

                childElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
                childElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));

                iOSentity = GetIosElement(childElement, fe.Opacity, left, top, fe.Width, fe.Height); 
            }

            return childElement;
        }

        private IOsEntity GetIosElement(XmlElement childElement, double opacity, double left, double top, double width, double height, Dictionary<string, string> attributes = null, string elementName = null, bool updateCoord = false)
        {
            if (!makeIOSFile)
                return null;
            XmlElement xmlElement = (XmlElement)iOSxmlDoc.ImportNode(childElement.ToXml().FromXml<XmlNode>(), true);
            
            xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"0", iOSxmlDoc));
            xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"0", iOSxmlDoc));
            
            string transform = xmlElement.GetAttribute("transform");
            if (!string.IsNullOrEmpty(transform) && transform.Contains("translate"))
            {
                var tlist = transform.Split(')').ToList();
                transform = string.Empty;
                tlist.ForEach(t =>
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        if (!t.Contains("translate"))
                            transform = $"{transform} {t})";
                    }
                });
                if (!string.IsNullOrEmpty(transform))
                    xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute("transform", transform, iOSxmlDoc));
                else
                    xmlElement.Attributes.RemoveNamedItem("transform");
            }

            if (!string.IsNullOrEmpty(elementName))
                xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{elementName}", iOSxmlDoc));

            attributes?.Keys.ToList().ForEach(a =>
            {
                xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute($"{a}", $"{attributes[a]}", iOSxmlDoc));
            });


            if (opacity != 1)
                xmlElement.Attributes.Append(JSONHelper.CreateNewAttribute("opacity", $"{JSONHelper.GetValue(opacity)}", iOSxmlDoc));
            const double deltaY = 14;
            IOsEntity iOSentity = new IOsEntity()
            {
                entity = xmlElement.ToXml(),
                x = JSONHelper.GetValue(left, true),
                y = updateCoord && height <= deltaY ? JSONHelper.GetValue(top - (deltaY - height), true) : JSONHelper.GetValue(top, true),
                width = JSONHelper.GetValue(width, true),
                height = JSONHelper.GetValue(height, true),
            };

            return iOSentity;
        }

        private XmlElement ComposeElement(string name, FrameworkElement basecontrol,
                                            double basecontrolWidth, double basecontrolHeight,
                                            double width, double height, 
                                            double left, double top, bool addViewbox = false)
        {
            if (basecontrol == null && !innercontrolList.Contains(name))
                return null;
            double dwidth = double.IsNaN(basecontrolWidth) ? width : basecontrolWidth;
            double dheight = double.IsNaN(basecontrolHeight) ? height : basecontrolHeight;
            string bwidth = JSONHelper.GetValue(dwidth);
            string bheight = JSONHelper.GetValue(dheight);
            XmlDocument doc = new XmlDocument();
            XmlElement childElement = doc.CreateElement(string.Empty, "svg", string.Empty);
            childElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(width, true)}", doc));
            childElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(height, true)}", doc));

            if (!double.IsNaN(left))
                childElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
            if (!double.IsNaN(left))
                childElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));

            if (addViewbox)
            {
                childElement.Attributes.Append(JSONHelper.CreateNewAttribute("viewbox", $"0 0 {JSONHelper.GetValue(basecontrolWidth, true)} {JSONHelper.GetValue(basecontrolHeight, true)}", doc));
                childElement.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", doc));
            }

            int deepSearch = CountStringOccurrences(name, ScreenDocument.innerEntityNameFormat) + 2;
            var list = GetListInners(name, deepSearch);
            if (list.Count > 0)
            {
                var checkinner = $"{name}{ScreenDocument.innerEntityNameFormat}";
                string basecontrolName = !string.IsNullOrEmpty(basecontrol.Uid) ? basecontrol.Uid : basecontrol.Name;
                string baseInnerName = $"{name}{ScreenDocument.innerEntityNameFormat}{basecontrolName}";
                if (!string.IsNullOrEmpty(basecontrolName))
                    checkinner = $"{checkinner}{basecontrolName}{ScreenDocument.innerEntityNameFormat}";
                List<FrameworkElement> controlList = GetControlMap(basecontrol);
                deepSearch += 1;
                controlList.ForEach(control =>
                {
                    string controlName = string.IsNullOrEmpty(control.Name) ? control.Uid : control.Name;
                    string cname = $"{checkinner}{controlName}";
                    if (list.Contains(cname) && !innercontrolList.Contains(cname))
                    {
                        var _list = GetListInners(cname, deepSearch);
                        if (_list.Count() == 0)
                        {
                            XmlNode cElement = GetElement(cname);
                            if (cElement != null)
                                childElement.AppendChild(doc.ImportNode(cElement, true));
                        }
                        else
                        {
                            bool hasIosChild;
                            XmlNode cElement = AddInners(control, cname, deepSearch, null, new Point(0,0), out hasIosChild);
                            if (cElement != null)
                                childElement.AppendChild(doc.ImportNode(cElement, true));
                        }
                        if (!innercontrolList.Contains(cname))
                            innercontrolList.Add(cname);
                        if (!innercontrolList.Contains(baseInnerName))
                            innercontrolList.Add(baseInnerName);
                    }
                });
            }
            else
            {
                if(basecontrol is Viewbox && (basecontrol as Viewbox).Child is Canvas)
                {
                    Canvas canvas = (basecontrol as Viewbox).Child as Canvas;
                    double cwidth = double.IsNaN(canvas.Width) ? width : canvas.Width;
                    double cheight = double.IsNaN(canvas.Height) ? height : canvas.Height;
                    childElement.Attributes.Append(JSONHelper.CreateNewAttribute("viewBox", $"0 0 {JSONHelper.GetValue(cwidth)} {JSONHelper.GetValue(cheight)}", doc));
                }
                string basecontrolName = !string.IsNullOrEmpty(name) ? name : !string.IsNullOrEmpty(basecontrol.Uid) ? basecontrol.Uid : basecontrol.Name;
                var checkinner = !string.IsNullOrEmpty(basecontrolName) ? $"{basecontrolName}{ScreenDocument.innerEntityNameFormat}" : string.Empty;

                var controlList = GetControlMap(basecontrol);
                controlList.ForEach(el =>
                {
                    string controlName = /*!string.IsNullOrEmpty(el.Uid) ? el.Uid :*/ el.Name;
                    string newName = string.IsNullOrEmpty(controlName) ? string.Empty : $"{checkinner}{controlName}";
                    if(!string.IsNullOrEmpty(newName))
                    {
                        bool sourceSymbolLinked = Document.MapScreenEntities.ContainsKey(newName) && Document.MapScreenEntities[newName].SourceSymbolLinked;
                        XmlNode _childElement = GetElement(newName, el, true, sourceSymbolLinked);
                        if (_childElement != null)
                            childElement.AppendChild(doc.ImportNode(_childElement, true));
                    }
                });
            }
            if(!innercontrolList.Contains(name))
                innercontrolList.Add(name);
            return childElement;
        }

        private void GetSymbol(string symbolPath, string key, out string symbolON, out string symbolOFF, out string symbolNULL)
        {
            symbolON = string.Empty;
            symbolOFF = string.Empty;
            symbolNULL = string.Empty;
            if (string.IsNullOrEmpty(symbolPath))
                return;
            try
            {
                symbolPath = symbolPath.UpdateStyleFolder().Replace(STRL.STRL.styleFolder, "SVGStyles");
                string symbolPathON = symbolPath.Replace($".xaml", $"{key}_ON.svg");
                string symbolPathOFF = symbolPath.Replace($".xaml", $"{key}_OFF.svg");
                string symbolPathNULL = symbolPath.Replace($".xaml", $"{key}_NULL.svg");
                symbolON = GetSymbol(symbolPathON);
                symbolOFF = GetSymbol(symbolPathOFF);
                symbolNULL = GetSymbol(symbolPathNULL);
            }
            catch
            {
            }
        }

        private string GetSymbol(string symbolPath)
        {
            if (!System.IO.File.Exists(symbolPath))
            {
#if DEBUG
                LogMissingFile(symbolPath);
#endif
                return string.Empty;
            }
            string symbolName = string.Empty; 
            try
            {
                
                if (!referenceDefsXMLStyleNameMap.ContainsKey(symbolPath) && System.IO.File.Exists(symbolPath))
                {
                    string filename = System.IO.Path.GetFileNameWithoutExtension(symbolPath);
                    symbolName = $"{cssPrefix}{tagSymbolStyle}"; //UFUAModel.Helpers.NameValidator.EnsureValidName(filename);
//#if DEBUG
//                    symbolName = $"{symbolName}_{filename}";
//#endif
                    int i = 0;
                    while (referenceDefsXMLStyleNameMap.Values.Contains(symbolName))
                    {
                        symbolName = $"{symbolName}{++styleConter}";
                    }

                    referenceDefsXMLStyleNameMap.Add(symbolPath, symbolName);
                }
                if (!referenceDefsXMLStyleContentMap.ContainsKey(symbolPath))
                {
                    string content = File.ReadAllText(symbolPath);
                    try
                    {
                        content = WPFUtilities.CryptString.CryptString.DecryptString(content);
                    }
                    catch (Exception ex) { }
                    content = JSONHelper.SetStyleIds(content, referenceDefsXMLStyleNameMap[symbolPath]);

                    XmlDocument doc = new XmlDocument();
                    var fragment = doc.CreateDocumentFragment();
                    fragment.InnerXml = JSONHelper.ManageClassIDs(content.FromXml<XmlNode>(), referenceDefsXMLStyleNameMap[symbolPath]);
                    XmlElement tmpchildElement = null;
                    foreach (XmlNode child in fragment.ChildNodes)
                    {
                        if (child.Name == "svg")
                        {
                            tmpchildElement = child as XmlElement;
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", doc));
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", doc));
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", doc));
                            tmpchildElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", referenceDefsXMLStyleNameMap[symbolPath], doc));
                            break;
                        }
                    }
                    referenceDefsXMLStyleContentMap.Add(symbolPath, tmpchildElement.ToXml());
                }
                return referenceDefsXMLStyleNameMap[symbolPath];
            }
            catch(Exception ex)
            {
                Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                string msg = string.Format(Properties.Resources.MsgWithException,
                    string.Format(Properties.Resources.ErrorGettingSymbol, Document.Title, symbolName, symbolPath),
                    e.Message);
                logDeploy.Error(msg);
                errorMessages = true;
            }
            return string.Empty;
        }

        internal static readonly String innerSymbolName = "InnerSymbolName";
        List<string> skipNodeTypeForUid = new List<string>() { "svg", "linearGradient", "radialGradient", "style", "#significant-whitespace" };
        private void ResolveUids(XmlNode element, string parent = null)
        {
            var checkinner = !string.IsNullOrEmpty(parent) ? String.Format("{0}{1}", parent, innerEntitySVGNameFormat) : parent;
            string newValue = checkinner;

            if (element.HasChildNodes)
            {
                List<XmlNode> nodetoremove = new List<XmlNode>();
                foreach (XmlNode child in element.ChildNodes)
                {
                    try
                    {
                        if (child.Attributes != null && child.Attributes.GetNamedItem("id") != null && !skipNodeTypeForUid.Contains(child.Name))
                        {
                            child.Attributes.GetNamedItem("id").Value = newValue = $"{checkinner}{child.Attributes.GetNamedItem("id").Value}";
                        }
                        if (child.Name == "linearGradient" || child.Name == "radialGradient" || child.Name == "style" )
                            nodetoremove.Add(child);
                        if (!skipNodeTypeForUid.Contains(child.Name))
                            ResolveUids(child, newValue);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                nodetoremove.ForEach(child => element.RemoveChild(child));
            }
        }
        private void UpdateUids(XmlNode element, string newValue = null)
        {
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child.Attributes != null && child.Attributes.GetNamedItem("id") != null)
                {
                    child.Attributes.GetNamedItem("id").Value = child.Attributes.GetNamedItem("id").Value.Replace(innerSymbolName, newValue);
                }
                UpdateUids(child, newValue);
            }
        }
        private XmlNode GetContentControlElement(FrameworkElement fe, string elementName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode uieElement = null;
            string fillName = null;
            double left = (double)fe.GetValue(Canvas.LeftProperty);
            double top = (double)fe.GetValue(Canvas.TopProperty);
            if (fe is ContentControl)
            {
                string name = string.IsNullOrEmpty(fe?.Name) ? string.IsNullOrEmpty(elementName) ? fe?.Uid : elementName : fe.Name;
                string clearedName = Document.CleanInnerName(elementName, innerEntitySVGNameFormat);

                ContentControl control = fe as ContentControl;
                //Document.UpdateProblematicXamlWriterProperties(fe, elementName);
                uieElement = doc.CreateElement(string.Empty, groupObject, string.Empty);
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{clearedName}", doc));

                if (!double.IsNaN(left))
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                if (!double.IsNaN(top))
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));

                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));

                var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);

                if (control.Content != null)
                {
                    XmlElement childElement = ComposeElement(name, control.Content as FrameworkElement,
                                                                     double.NaN, double.NaN,
                                                                     fe.Width, fe.Height,
                                                                     left, top);
                    if (childElement != null)
                        uieElement.AppendChild(doc.ImportNode(childElement, true));
                }
                
                fillName = GetCssClassStyle(control.Background, ColorMode.Fill, new Size(fe.Width,fe.Height));
            }
            if (uieElement != null)
            {
                string style = null;
                if (styleXMLMap.ContainsKey(fillName))
                    style = styleXMLMap[fillName];
                if(style != null)
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("class", $"{style}", doc));
                UpdateEffect(doc, uieElement, fe.Effect);
            }
            return uieElement;
        }

        private XmlNode GetControlElement(FrameworkElement fe, string elementName)
        {
            XmlNode uieElement = null;
            string fillName = null;
            if (fe is Control)
            {
                string name = string.IsNullOrEmpty(fe?.Name) ? string.IsNullOrEmpty(elementName) ? fe?.Uid : elementName : fe.Name;
                string clearedName = Document.CleanInnerName(elementName, innerEntitySVGNameFormat);

                Control control = fe as Control;
                //Document.UpdateProblematicXamlWriterProperties(control, elementName);
                Type type = fe.GetType();
                if (!IsDeploySupported(type, elementName))
                    return uieElement;
                XmlDocument doc = new XmlDocument();
                //create foreignobject    
                uieElement = doc.CreateElement(string.Empty, foreignObject, string.Empty);
                string typeName = type.Name;
                var customObjectSvgAttribute = (Utilities.SvgValueConverterAttribute)type.GetCustomAttributes(typeof(Utilities.SvgValueConverterAttribute), true).FirstOrDefault() as Utilities.SvgValueConverterAttribute;
                if (customObjectSvgAttribute != null && !string.IsNullOrEmpty(customObjectSvgAttribute.TypeName))
                    typeName = customObjectSvgAttribute.TypeName;

                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("class", $"{typeName}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{clearedName}", doc));

                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"0px", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"0px", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("style", "pointer-events: auto;", doc));

                double left = (double)fe.GetValue(Canvas.LeftProperty);
                double top = (double)fe.GetValue(Canvas.TopProperty);
                if (!double.IsNaN(left))
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                if (!double.IsNaN(left))
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));

                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));

                var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);


                Dictionary<string, Brush> overriddenBrushes = JSONHelper.GetOverrideBrushProperties(fe, Document);
                Dictionary<string, Brush> dict = JSONHelper.GetBrushes(fe, Document);
                Dictionary<string, string> dictNames = new Dictionary<string, string>();
                dict.Keys.ToList().ForEach(key =>
                {
                    var colorname = GetCssClassStyle(dict[key], ColorMode.Fill, new Size(fe.Width, fe.Height));
                    dictNames.Add(key, colorname);
                });

                Dictionary<string, Brush> urldict = JSONHelper.GetUrlBrushes(fe, Document);
                Dictionary<string, Dictionary<string, object>> urldictNames = new Dictionary<string, Dictionary<string, object>>();
                urldict?.Keys.ToList().ForEach(key =>
                {
                    if (urldict[key] == null)
                    {
                        urldictNames.Add(key, new Dictionary<string, object>() { { "Color", null } });
                    }
                    if (urldict[key] != null && !(urldict[key] is SolidColorBrush))
                    {
                        var brush = urldict[key];
                        if (brush is VisualBrush && (brush as VisualBrush).Visual is MediaElement)
                        {
                            string source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                            urldictNames.Add(key, new Dictionary<string, object>() { { "MediaSource", source } });
                        }
                        else
                        {
                            var colorname = GetCssClassStyle(urldict[key], ColorMode.Fill, new Size(fe.Width, fe.Height));
                            if (styleXMLMap.ContainsKey(colorname))
                            {
                                try
                                {
                                    colorname = styleXMLMap[colorname].Split(';')[0].Split(':').LastOrDefault();
                                    urldictNames.Add(key, new Dictionary<string, object>() { { "Color", colorname } });
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                });

                if (control.ReadLocalValue(Control.BorderBrushProperty) == DependencyProperty.UnsetValue || control.BorderBrush == null)
                    urldictNames.Add(Control.BorderBrushProperty.Name, new Dictionary<string, object>() { { "Color", null } });

                //fillName = GetCssClassStyle(control.Background, ColorMode.Fill);
                //string style = null;
                //if (styleXMLMap.ContainsKey(fillName))
                //    style = styleXMLMap[fillName];
                //if (style != null)
                //    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("style", $"{style}", xmlDoc));
                jsonobject fobj = new jsonobject()
                {
                    SVGItemId = clearedName,
                    SVGItem = control,
                    brushes = dictNames,
                    urlbrushes = urldictNames,
#if !NET_STANDARD
                    overriddenBrushes = overriddenBrushes,
#endif
                    parameters = new Dictionary<string, object>()
                };

                fobj.parameters.Add(JSONHelper.isSolidColorBorder, (control.BorderBrush is SolidColorBrush));


                if (fe is ListBox || fe is ComboBox)
                {
                    if (Document.MapScreenEntities.ContainsKey(elementName))
                    {
                        var entity = Document.MapScreenEntities[elementName];
                        if (entity != null && entity.IsItemSourceEntity())
                        {
                            if(entity.ReaderItemSources != null)
                            {
                                fobj.parameters.Add("ReaderItemSources", new DataReader.DataReaderModel(entity.ReaderItemSources));
                            }
                            else
                            {
                                var itemList = entity.GetItemSources();
                                if (itemList?.Count > 0)
                                {
                                    fobj.parameters.Add("ItemsSource", $"{String.Join($"|", itemList.ToList())}");
                                }
                            }
                        }
                    }
                }
                if (fe is Buttons.CheckBoxControl && Utilities.WPF.XmlHelper.IsProblematicXamlWriter(fe))
                {
                    if (Document.MapScreenEntities.ContainsKey(elementName))
                    {
                        var entity = Document.MapScreenEntities[elementName];
                        if (entity != null)
                        {
                            Buttons.CheckBoxControl checkBoxControl = (fe as Buttons.CheckBoxControl);
                            if (entity.ProblematicXaml.Contains(tagFlatStyle)) 
                                fobj.parameters.Add("IsFlatStyle", true);
                            var k = fe.Height / 16;
                            try
                            {
                                Thickness thickness = new Thickness(Math.Round(checkBoxControl.BorderThickness.Left * k,2),
                                                    Math.Round(checkBoxControl.BorderThickness.Top * k, 2),
                                                    Math.Round(checkBoxControl.BorderThickness.Right * k, 2),
                                                    Math.Round(checkBoxControl.BorderThickness.Bottom * k, 2));
                            
                                fobj.parameters.Add("BorderThickness", thickness);
                            }
                            catch 
                            {
                            }
                        }
                    }
                }
                if (fe is TextBox)
                {
                    var alignment = (fe as TextBox).HorizontalContentAlignment;

                    if ((fe as TextBox).ReadLocalValue(TextBox.TextAlignmentProperty) != DependencyProperty.UnsetValue)
                    {
                        switch ((fe as TextBox).TextAlignment)
                        {
                            case TextAlignment.Left:
                                alignment = HorizontalAlignment.Left;
                                break;
                            case TextAlignment.Right:
                                alignment = HorizontalAlignment.Right;
                                break;
                            case TextAlignment.Center:
                                alignment = HorizontalAlignment.Center;
                                break;
                            case TextAlignment.Justify:
                                alignment = HorizontalAlignment.Stretch;
                                break;
                            default:
                                break;
                        }
                    }
                    fobj.parameters.Add("WebHMITextAlignment", alignment);
                }
                foreignObjectList.Add(fobj);
                UpdateEffect(doc, uieElement, fe.Effect);
            }
            return uieElement;
        }

        private XmlNode GetShapeElement(FrameworkElement fe, string elementName, out IOsEntity iOSentity, XmlDocument doc = null)
        {
            iOSentity = null;
            string name = string.IsNullOrEmpty(fe.Name) ? fe.Uid : fe.Name;
            if (!(fe is Shape) && !(fe is Border))
                return null;
            XmlNode uieElement = null;
            string strokeName = null;
            string fillName = null;
            double left = (double)fe.GetValue(Canvas.LeftProperty);
            double top = (double)fe.GetValue(Canvas.TopProperty);
            
            if (double.IsNaN(left))
                left = 0;
            if (double.IsNaN(top))
                top = 0;

            double iosLeft = left;
            double iosTop = top;
            double iosWidth = fe.Width;
            double iosHeight = fe.Height;
            bool encapsule = false;

            if (doc == null)
                doc = new XmlDocument();
            XmlNode xmlNode = null;
            if (fe is Border)
            {
                Border rect = fe as Border;

                VisualBrush brush = rect.Background as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                Dictionary<string, string> attributes = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(source))
                {
                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                    XmlNode uieDef = doc.CreateElement(string.Empty, "path", string.Empty);
                    var borderPath = DescribeBorder(rect, 0, 0,
                                    (double)fe.Width, (double)fe.Height);
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("d", $"{borderPath}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(Math.Max(rect.BorderThickness.Top, Math.Max(rect.BorderThickness.Bottom, Math.Max(rect.BorderThickness.Left, rect.BorderThickness.Right))),true)}", doc));
                    if (rect.BorderBrush != null && rect.BorderBrush is SolidColorBrush)
                    {
                        SolidColorBrush stroke = rect.BorderBrush as SolidColorBrush;
                        uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke", $"{stroke}", doc));
                    }
                    uieClipDef.AppendChild(uieDef);
                   // var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    string videoClass = AddClipPathStyle($"path('{borderPath}')");

                    uieElement = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass);
                    fillName = "none";
                    strokeName = "none";
                    encapsule = true;
                }
                else
                {
                    uieElement = doc.CreateElement(string.Empty, "path", string.Empty);
                    var borderPath = DescribeBorder(rect, left, top,
                                    (double)fe.Width, (double)fe.Height);

                    var iOsBorderPath = DescribeBorder(rect, 0, 0,
                                    (double)fe.Width, (double)fe.Height);

                    attributes.Add("d", $"{iOsBorderPath}");

                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("d", $"{borderPath}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(Math.Max(rect.BorderThickness.Top, Math.Max(rect.BorderThickness.Bottom, Math.Max(rect.BorderThickness.Left, rect.BorderThickness.Right))),true)}", doc));

                    fillName = rect.Background == null ? "none" : GetCssClassStyle(rect.Background, ColorMode.Fill, new Size(fe.Width, fe.Height));
                    strokeName = rect.BorderBrush == null ? "none" : GetCssClassStyle(rect.BorderBrush, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                }

                var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);
            
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, attributes, elementName: elementName);
                iOSentity.removeViewbox = false;
            }
            else if (fe is Rectangle)
            {
                Rectangle rect = fe as Rectangle;

                VisualBrush brush = rect.Fill as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);

                Dictionary<string, string> attributes = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(source))
                {
                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                    XmlNode uieDef = doc.CreateElement(string.Empty, "rect", string.Empty);
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(rect.StrokeThickness,true)}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{JSONHelper.GetValue(rect.RadiusX, true)}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{JSONHelper.GetValue(rect.RadiusY, true)}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue((double)fe.Width, true)}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue((double)fe.Height, true)}", doc));

                    if (rect.Stroke != null && rect.Stroke is SolidColorBrush)
                    {
                        SolidColorBrush stroke = rect.Stroke as SolidColorBrush;
                        uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke", $"{stroke}", doc));
                    }
                    uieClipDef.AppendChild(uieDef);
                   // var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    string videoClass = AddClipPathStyle($"inset(0 round {JSONHelper.GetValue(Math.Max(rect.RadiusX, rect.RadiusY), true)}px)");

                    uieElement = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass);
                    fillName = "none";
                    strokeName = "none";
                    encapsule = true;
                }
                else
                {
                    uieElement = doc.CreateElement(string.Empty, "rect", string.Empty);
                    fillName = rect.Fill == null ? "none" : GetCssClassStyle(rect.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                    strokeName = rect.Stroke == null ? "none" : GetCssClassStyle(rect.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(rect.StrokeThickness,true)}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{JSONHelper.GetValue(rect.RadiusX, true)}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{JSONHelper.GetValue(rect.RadiusY, true)}", doc));


                    attributes.Add("x", $"0");
                    attributes.Add("y", $"0");
                }
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue((double)fe.Width, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue((double)fe.Height, true)}", doc));

                var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);
         
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, elementName: elementName, updateCoord: true);
                iOSentity.removeViewbox = false;
            }
            else if (fe is Ellipse)
            {
                Ellipse ell = fe as Ellipse;
                VisualBrush brush = ell.Fill as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);

                Dictionary<string, string> attributes = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(source))
                {
                    var cx = JSONHelper.GetValue((double)fe.Width / 2);
                    var cy = JSONHelper.GetValue((double)fe.Height / 2);
                    var rx = JSONHelper.GetValue(fe.Width / 2 - ell.StrokeThickness);
                    var ry = JSONHelper.GetValue(fe.Height / 2 - ell.StrokeThickness);

                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                    XmlNode uieDef = doc.CreateElement(string.Empty, "ellipse", string.Empty);
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("cx", $"{cx}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("cy", $"{cy}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("rx", $"{rx}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{ry}", doc));
                    uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness,true)}", doc));

                    if (ell.Stroke != null && ell.Stroke is SolidColorBrush)
                    {
                        SolidColorBrush stroke = ell.Stroke as SolidColorBrush;
                        uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke", $"{stroke}", doc));
                    }
                    uieClipDef.AppendChild(uieDef);
                   // var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    string videoClass = AddClipPathStyle($"ellipse({rx}px {ry}px)"); 


                    uieElement = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass);
                    fillName = "none";
                    strokeName = "none";
                    encapsule = true;
                }
                else
                {
                    double cxd = (double)fe.GetValue(Canvas.LeftProperty) + fe.Width / 2;
                    double cyd = (double)fe.GetValue(Canvas.TopProperty) + fe.Height / 2;
                    attributes.Add("cx", $"{JSONHelper.GetValue(cxd - iosLeft, true)}");
                    attributes.Add("cy", $"{JSONHelper.GetValue(cyd - iosTop,true)}");

                    var cx = JSONHelper.GetValue(cxd, true);
                    var cy = JSONHelper.GetValue(cyd, true);
                    var rx = JSONHelper.GetValue(fe.Width / 2 - ell.StrokeThickness, true);
                    var ry = JSONHelper.GetValue(fe.Height / 2 - ell.StrokeThickness, true);

                    uieElement = doc.CreateElement(string.Empty, "ellipse", string.Empty);
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("cx", $"{cx}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("cy", $"{cy}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("rx", $"{rx}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{ry}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness,true)}", doc));

                    fillName = ell.Fill == null ? "none" : GetCssClassStyle(ell.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                    strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                }

                var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);
      
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, attributes, elementName: elementName);
                iOSentity.removeViewbox = true;
            }
            else if (fe is Polygon)
            {
                Polygon pol = fe as Polygon;

                VisualBrush brush = pol.Fill as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                if (!string.IsNullOrEmpty(source))
                {
                    string points = GetPoints(pol.Points);
                    var xmin = pol.Points.Min(p => p.X);
                    var xmax = pol.Points.Max(p => p.X);
                    var ymin = pol.Points.Min(p => p.Y);
                    var ymax = pol.Points.Max(p => p.Y);
                    Size size = new Size(Math.Abs(xmax - xmin), Math.Abs(ymax - ymin)); 
                    if (double.IsNaN(left))
                        iosLeft = xmin;
                    if (double.IsNaN(top))
                        iosTop = ymin;

                    iosWidth = size.Width;
                    iosHeight = size.Height;
                    InfoLine infoLine = new InfoLine()
                    { 
                        Shades = 1,
                        StrokeThickness = pol.StrokeThickness,
                        Points = points,
                        Stroke = pol.Stroke,
                        Fill = pol.Fill,
                        Left = left,
                        Top = top,
                        Width = size.Width,
                        Height = size.Height,
                        UseRound = false,
                        UseFill = true,
                        StrokeDashArray = pol.StrokeDashArray,
                        LineJoin = pol.StrokeLineJoin.ToString().ToLower()
                    };

                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                    XmlNode uieDef = DrawLine(1, infoLine, doc, false);

                    uieClipDef.AppendChild(uieDef);
                    //var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    var pointl = String.Join(" ", pol.Points.Select(p => $"{JSONHelper.GetValue(p.X, true)} {JSONHelper.GetValue(p.Y, true)}").ToList());
                    string videoClass = AddClipPathStyle($"path('M{pointl}Z')");

                    uieElement = GetVideoElement(doc, left, top, size, source, videoClass);
                    fillName = "none";
                    strokeName = "none";
                    encapsule = true;
                }
                else
                {
                    var xmin = pol.Points.Min(p => p.X);
                    var xmax = pol.Points.Max(p => p.X);
                    var ymin = pol.Points.Min(p => p.Y);
                    var ymax = pol.Points.Max(p => p.Y);

                    iosLeft += xmin;
                    iosTop += ymin;
                    var _points = pol.Points.ToList();
                    pol.Points = new PointCollection();
                    _points.ForEach(p => pol.Points.Add(new Point() { X = p.X - xmin, Y = p.Y - ymin }));

                    top = iosTop;
                    left = iosLeft;
                    fe.SetValue(Canvas.TopProperty, top);
                    fe.SetValue(Canvas.LeftProperty, left);

                    string points = GetPoints(pol.Points);
                    uieElement = doc.CreateElement(string.Empty, "polygon", string.Empty);
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("points", $"{points}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(pol.StrokeThickness,true)}", doc));
                    Size size = new Size(Math.Abs(xmax - xmin), Math.Abs(ymax - ymin));
                    iosWidth = size.Width;
                    iosHeight = size.Height;
                    fillName = pol.Fill == null ? "none" : GetCssClassStyle(pol.Fill, ColorMode.Fill, size);
                    strokeName = pol.Stroke == null ? "none" : GetCssClassStyle(pol.Stroke, ColorMode.Stroke, size);
                    var attribute = GetTransform(fe, new Point(1, 1), true, false, true, doc);
                    if (attribute != null)
                        uieElement.Attributes.Append(attribute);
                }
  
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, elementName: elementName);
                iOSentity.removeViewbox = true;
            }
            else if (fe is System.Windows.Shapes.Path)
            {
                System.Windows.Shapes.Path path = fe as System.Windows.Shapes.Path;
                string points = path.Data.ToString().Replace(',', '.').Replace(';', ',');

                VisualBrush brush = path.Fill as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                if (!string.IsNullOrEmpty(source))
                {
                    
                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                    XmlNode uieDef = doc.CreateElement(string.Empty, "path", string.Empty);
                    uieClipDef.Attributes.Append(JSONHelper.CreateNewAttribute("d", $"{points}", doc));
                    uieClipDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(path.StrokeThickness,true)}", doc));

                    if (path.Stroke != null && path.Stroke is SolidColorBrush)
                    {
                        SolidColorBrush stroke = path.Stroke as SolidColorBrush;
                        uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("stroke", $"{stroke}", doc));
                    }
                    uieClipDef.AppendChild(uieDef);
                    //var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    string videoClass = AddClipPathStyle($"path('{points}')");


                    uieElement = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass);
                    fillName = "none";
                    strokeName = "none";
                    encapsule = true;
                }
                else
                {
                    uieElement = doc.CreateElement(string.Empty, "path", string.Empty);
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("d", $"{points}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(path.StrokeThickness,true)}", doc));
                    fillName = path.Fill == null ? "none" : GetCssClassStyle(path.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                    strokeName = path.Stroke == null ? "none" : GetCssClassStyle(path.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                }

                var attribute = GetTransform(fe, new Point(1, 1), true, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);

                iosWidth = iosWidth + path.StrokeThickness;
                iosHeight = iosHeight + path.StrokeThickness;
  
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, elementName: elementName);
                iOSentity.removeViewbox = false;
            }
            else if (fe is System.Windows.Shapes.Line)
            {
                Line line = fe as Line;

                uieElement = doc.CreateElement(string.Empty, "line", string.Empty);
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x1", $"{JSONHelper.GetValue(line.X1, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y1", $"{JSONHelper.GetValue(line.Y1, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x2", $"{JSONHelper.GetValue(line.X2, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y2", $"{JSONHelper.GetValue(line.Y2, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(line.StrokeThickness,true)}", doc));
                var attribute = GetTransform(fe, new Point(1, 1), true, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);

                var xmin = line.X1;
                var xmax = line.X2;
                var ymin = line.Y1;
                var ymax = line.Y2;
                Size size = new Size(Math.Abs(xmax - xmin), Math.Abs(ymax - ymin));
                iosLeft += xmin;
                line.X1 = line.X1 - xmin;
                line.X2 = line.X2 - xmin;

                iosTop += ymin;
                line.Y1 = line.Y1 - ymin;
                line.Y2 = line.Y2 - ymin;

                top = iosTop;
                left = iosLeft;
                fe.SetValue(Canvas.TopProperty, top);
                fe.SetValue(Canvas.LeftProperty, left);


                iosWidth = size.Width + line.StrokeThickness;
                iosHeight = size.Height + line.StrokeThickness;
                fillName = line.Fill == null ? "none" : GetCssClassStyle(line.Fill, ColorMode.Fill, size);
                strokeName = line.Stroke == null ? "none" : GetCssClassStyle(line.Stroke, ColorMode.Stroke, size);
     
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                Dictionary<string, string> attributes = new Dictionary<string, string>();
                attributes.Add("x1", $"{JSONHelper.GetValue(line.X1, true)}");
                attributes.Add("x2", $"{JSONHelper.GetValue(line.X2, true)}");
                attributes.Add("y1", $"{JSONHelper.GetValue(line.Y1, true)}");
                attributes.Add("y2", $"{JSONHelper.GetValue(line.Y2, true)}");
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, attributes, elementName: elementName);
                iOSentity.removeViewbox = true;
            }
            else if (fe is Pipeline.Pipeline)
            {
                Pipeline.Pipeline line = fe as Pipeline.Pipeline;
                if (line.Points == null || line.Points.Count < 2 || line.Stroke == null)
                    return uieElement;
                var xmin = line.Points.Min(p => p.X);
                var xmax = line.Points.Max(p => p.X);
                var ymin = line.Points.Min(p => p.Y);
                var ymax = line.Points.Max(p => p.Y);
                Size size = new Size(Math.Abs(xmax - xmin), Math.Abs(ymax - ymin));

                iosLeft += xmin;
                iosTop += ymin;
                var points = line.Points.ToList();
                line.Points = new PointCollection();
                points.ForEach(p => line.Points.Add(new Point() { X = p.X - xmin, Y = p.Y - ymin }));

                top = iosTop;
                left = iosLeft;
                fe.SetValue(Canvas.TopProperty, top);
                fe.SetValue(Canvas.LeftProperty, left);

                iosWidth = size.Width + line.StrokeThickness;
                iosHeight = size.Height + line.StrokeThickness;

                uieElement = doc.CreateElement(string.Empty, groupObject, string.Empty);
                InfoLine infoLine = new InfoLine()
                {
                    Shades = line.Shades,
                    StrokeThickness = line.StrokeThickness,
                    Points = GetPoints(line.Points),
                    Stroke = line.Stroke,
                    Fill = line.Fill,
                    Left = left,
                    Top = top,
                    Width = fe.Width,
                    Height = fe.Height,
                    UseRound = line.StrokeEndLineCap == PenLineCap.Round || line.StrokeStartLineCap == PenLineCap.Round,
                    UseFill = false,
                    StrokeDashArray = line.StrokeDashArray,
                    LineJoin = "round"
                };

                Color color1 = Colors.White;
                Color color2 = Colors.Black;

                GetLineColors(infoLine, out color1, out color2);
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("useTagBGForStroke", $"{true}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("strokeColor", $"{color2}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("fillColor", $"{color1}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("backAnimationProperty", $"stroke", doc));


                int shades = Math.Max(infoLine.Shades, 1);
                for (int shade = line.Shades; shade >= 0; --shade)
                {
                    XmlNode path = DrawLine(shade,  infoLine, doc);
                    var percentage = (float)shade / (float)shades;
                    path.Attributes.Append(JSONHelper.CreateNewAttribute("tag", $"BG", doc)); 
                    path.Attributes.Append(JSONHelper.CreateNewAttribute("colorInterpolationPercentiage", $"{JSONHelper.GetValue(percentage, true)}", doc));
                    uieElement.AppendChild(doc.ImportNode(path, true));
                }

                var attribute = GetTransform(fe, new Point(1, 1), true, false, true, doc);
                if (attribute != null)
                    uieElement.Attributes.Append(attribute);

                fillName = "none";
                strokeName = "none";
   
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);

                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, elementName: elementName);
                iOSentity.removeViewbox = true;
            }
            else if (fe is System.Windows.Shapes.Polyline)
            {
                Polyline line = fe as Polyline;
                var xmin = line.Points.Min(p => p.X);
                var xmax = line.Points.Max(p => p.X);
                var ymin = line.Points.Min(p => p.Y);
                var ymax = line.Points.Max(p => p.Y);
                Size size = new Size(Math.Abs(xmax - xmin), Math.Abs(ymax - ymin));

                iosLeft += xmin;
                iosTop += ymin;
                var points = line.Points.ToList();
                line.Points = new PointCollection();
                points.ForEach(p => line.Points.Add(new Point() { X = p.X - xmin, Y = p.Y - ymin }));

                top = iosTop;
                left = iosLeft;
                fe.SetValue(Canvas.TopProperty, top);
                fe.SetValue(Canvas.LeftProperty, left);

                iosWidth = size.Width + line.StrokeThickness;
                iosHeight = size.Height + line.StrokeThickness;
                InfoLine infoLine = new InfoLine()
                {
                    Shades = 1,
                    StrokeThickness = line.StrokeThickness,
                    Points = GetPoints(line.Points),
                    Stroke = line.Stroke,
                    Fill = line.Fill,
                    Left = left,
                    Top = top,
                    Width = size.Width,
                    Height = size.Height,
                    UseRound = line.StrokeEndLineCap == PenLineCap.Round || line.StrokeStartLineCap == PenLineCap.Round,
                    UseFill = true,
                    StrokeDashArray = line.StrokeDashArray,
                    LineJoin = line.StrokeLineJoin.ToString().ToLower()
                };
                VisualBrush brush = line.Fill as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                if (!string.IsNullOrEmpty(source))
                {
                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                    XmlNode uieDef = DrawLine(1, infoLine, doc, false);

                    uieClipDef.AppendChild(uieDef);
                    //var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    var pointl = String.Join(" ", line.Points.Select(p => $"{JSONHelper.GetValue(p.X, true)} {JSONHelper.GetValue(p.Y, true)}").ToList());
                    string videoClass = AddClipPathStyle($"path('M{pointl}Z')");

                    uieElement = GetVideoElement(doc, left, top, size, source, videoClass);
                    encapsule = true;
                }
                else
                {
                    infoLine.Points = GetPoints(line.Points);
                    uieElement = DrawLine(1, infoLine, doc);
                    var attribute = GetTransform(fe, new Point(1, 1), true, false, true, doc);
                    if (attribute != null)
                        uieElement.Attributes.Append(attribute);
                }

                fillName = "none";
                strokeName = "none";
  
                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, elementName: elementName);
                iOSentity.removeViewbox = true;
            }
            else if (fe is Microsoft.Expression.Shapes.Arc)
            {
                Microsoft.Expression.Shapes.Arc ell = fe as Microsoft.Expression.Shapes.Arc;

                VisualBrush brush = ell.Fill as VisualBrush;
                string source = null;
                if (brush?.Visual is MediaElement)
                    source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
                Dictionary<string, string> attributes = new Dictionary<string, string>();
                XmlNode iOsuieElement = null;

                Rect fe_rect = new Rect()
                {
                    Width = iosWidth,
                    Height = iosHeight,
                    X = iosLeft,
                    Y = iosTop,
                };
                Brush _brush = ActiveLayer.Background;
                var intersect = controlRectList != null && !controlRectList.TrueForAll(controlRect => !controlRect.rect.IntersectsWith(fe_rect));
                if (intersect)
                    _brush = (from c in controlRectList where c.rect.IntersectsWith(fe_rect) select c).LastOrDefault()?.brush;


                if (!string.IsNullOrEmpty(source))
                {
                    XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);

                    bool addArc = false;
                    if (Math.Abs(ell.StartAngle - ell.EndAngle) >= 360 && !(ell.ArcThicknessUnit == Microsoft.Expression.Media.UnitType.Percent || ell.ArcThickness <= 1))
                    {
                        addArc = true;
                    }

                    XmlNode uieDef = DrawArc(ell, left, top, fe, doc, false, out fillName, out strokeName, onlyOuter: addArc);

                    uieClipDef.AppendChild(uieDef);
                    var id = AddDef(uieClipDef, doc);
                    string cssClass = string.Empty;
                    string videoClass = null;
                    //AddStyle(id);
                    if(uieDef.Name == "ellipse")
                    {
                        videoClass = AddClipPathStyle($"ellipse({uieDef.Attributes.GetNamedItem("rx").Value}px {uieDef.Attributes.GetNamedItem("ry").Value}px)"); 
                    }
                    else if (uieDef.Name == "path")
                    {
                        videoClass = AddClipPathStyle($"path('{uieDef.Attributes.GetNamedItem("d").Value}')");
                    }

                    //if (addArc)
                    //{
                    //    uieElement = doc.CreateElement(string.Empty, "g", string.Empty);
                    //    xmlNode = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass);
                    //    XmlNode innerArc = DrawArc(ell, left, top, fe, doc, true, out fillName, out strokeName, onlyInner: true);
                    //    uieElement.AppendChild(xmlNode);
                    //    uieElement.AppendChild(innerArc);
                    //}
                    //else
                        uieElement = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass);

                    fillName = "none";
                    strokeName = "none";
                    
                    var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                    if (attribute != null)
                        uieElement.Attributes.Append(attribute);
                    encapsule = true;

                    UpdateEffect(doc, uieElement, fe.Effect);
                    xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);

                    iOSentity = GetIosElement(uieElement as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, attributes, elementName: elementName);
                    iOSentity.removeViewbox = true;
                }
                else
                {
                    uieElement = DrawArc(ell, left, top, fe, doc, true, out fillName, out strokeName, innerBrush: _brush);
                    iOsuieElement = DrawArc(ell, 0, 0, fe, doc, true, out fillName, out strokeName, innerBrush: _brush);
                    iOsuieElement = UpdateStyle(doc, iOsuieElement, fillName, strokeName, 0, 0, fe.Width, fe.Height);
                    iOSentity = GetIosElement(iOsuieElement as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, attributes, elementName: elementName);
                    if (Math.Abs(ell.StartAngle - ell.EndAngle) < 360)
                        iOSentity.removeViewbox = false;
                    else
                        iOSentity.removeViewbox = true;

                    UpdateEffect(doc, uieElement, fe.Effect);
                    xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                }
            }
            else if (fe is Microsoft.Expression.Shapes.RegularPolygon)
            {
                Microsoft.Expression.Shapes.RegularPolygon ell = fe as Microsoft.Expression.Shapes.RegularPolygon;
                if (ell.PointCount > 0)
                {
                    Point scalefactor = new Point(1, 1);
                    var cxd = fe.Width / 2;
                    var cyd = fe.Height / 2;
                    var radius = Math.Min(cxd, cyd);
                    bool HasInnerPoints = ell.InnerRadius < 1;
                    double pointCount = HasInnerPoints ? ell.PointCount * 2 : ell.PointCount;
                    var angleDelta = 2 * Math.PI / pointCount;
                    StringBuilder points = new StringBuilder();
                    List<Point> pointList = new List<Point>();
                    Point center = new Point(radius, radius); //we are now building a 1:1 regular polygon. Eventually we will stretch it later using a SVG viewbox
                    bool skipPoint = true;
                    var startAngle = Math.PI / 2; // RegularPolygon vertices always start from center-top
                    for (int i = 0; i < pointCount; i++)
                    {
                        var k = HasInnerPoints && !skipPoint ? 1 - ell.InnerRadius + 0.1 : 0;
                        if (k < 0)
                            k = 0;
                        
                        var x = center.X - Math.Cos(startAngle + angleDelta * i) * (radius - radius * k);
                        var y = center.Y - Math.Sin(startAngle + angleDelta * i) * (radius - radius * k);

                        pointList.Add(new Point(x, y));
                        skipPoint = !skipPoint;
                    }

                    var xmin = pointList.Min(p => p.X);
                    var xmax = pointList.Max(p => p.X);
                    var ymin = pointList.Min(p => p.Y);
                    var ymax = pointList.Max(p => p.Y);

                    //iosLeft += xmin;
                    //iosTop += ymin;
                    var _points = pointList.ToList();
                    pointList = new List<Point>();
                    _points.ForEach(p => pointList.Add(new Point() { X = p.X - xmin, Y = p.Y - ymin }));

                    top = iosTop;
                    left = iosLeft;
                    fe.SetValue(Canvas.TopProperty, top);
                    fe.SetValue(Canvas.LeftProperty, left);

                    scalefactor.X = (fe.Width - ell.StrokeThickness) / fe.Width;
                    scalefactor.Y = (fe.Height - ell.StrokeThickness) / fe.Height;

                    pointList.ForEach(p => points.Append($" {JSONHelper.GetValue(p.X)}, {JSONHelper.GetValue(p.Y)}"));

                    VisualBrush brush = ell.Fill as VisualBrush;
                    string source = null;
                    if (brush?.Visual is MediaElement)
                        source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);

                    XmlNode poly;
                    var viewboxW = pointList.Max(p => p.X);
                    var viewboxH = pointList.Max(p => p.Y);
                    if (!string.IsNullOrEmpty(source))
                    {
                        XmlNode uieClipDef = doc.CreateElement(string.Empty, "clipPath", string.Empty);
                        poly = doc.CreateElement(string.Empty, "polygon", string.Empty);
                        poly.Attributes.Append(JSONHelper.CreateNewAttribute("points", $"{points}", doc));
                        poly.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness, true)}", doc));

                        if (ell.Stroke != null && ell.Stroke is SolidColorBrush)
                        {
                            SolidColorBrush stroke = ell.Stroke as SolidColorBrush;
                            poly.Attributes.Append(JSONHelper.CreateNewAttribute("stroke", $"{stroke}", doc));
                        }

                        uieClipDef.AppendChild(poly);
                        //var id = AddDef(uieClipDef, doc);
                        string cssClass = string.Empty;
                        var pointl = String.Join(" ", pointList.Select(p => $"{JSONHelper.GetValue(p.X, true)} {JSONHelper.GetValue(p.Y, true)}").ToList());
                        string videoClass = AddClipPathStyle($"path('M{pointl}Z')");


                        uieElement = GetVideoElement(doc, left, top, new Size(fe.Width, fe.Height), source, videoClass, false);
                        fillName = "none";
                        strokeName = "none";
                        encapsule = true;
                    }
                    else
                    {
                        poly = doc.CreateElement(string.Empty, "polygon", string.Empty);
                        poly.Attributes.Append(JSONHelper.CreateNewAttribute("points", $"{points}", doc));
                        poly.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness, true)}", doc));

                        uieElement = doc.CreateElement(string.Empty, "svg", string.Empty);
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns", "http://www.w3.org/2000/svg", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(fe.Width, true)}", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(fe.Height, true)}", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", "none", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("style", "overflow: visible;", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("class", "skip_me", doc));
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("viewBox", $"0 0 {JSONHelper.GetValue(viewboxW, true)} {JSONHelper.GetValue(viewboxH, true)}", doc));
                        uieElement.AppendChild(poly);

                        fillName = ell.Fill == null ? "none" : GetCssClassStyle(ell.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                        strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                    }

                    //var deltax = (fe.Width - (xmax - xmin) - 2 * ell.StrokeThickness) / 2;
                    //var deltay = (fe.Height - (ymax - ymin) - 2 * ell.StrokeThickness) / 2;


                    //double cleft = iosLeft - deltax;
                    //double ctop = iosTop - deltay;

                    //fe.SetValue(Canvas.LeftProperty, Math.Round(cleft, 2));
                    //fe.SetValue(Canvas.TopProperty, Math.Round(ctop, 2));

                    var attribute = GetTransform(fe, scalefactor, false, !encapsule, true, doc, true, viewboxW, viewboxH);
                    if (attribute != null)
                        poly.Attributes.Append(attribute);
                }

                UpdateEffect(doc, uieElement, fe.Effect);
                xmlNode = UpdateStyle(doc, uieElement, fillName, strokeName, left, top, fe.Width, fe.Height);
                iOSentity = GetIosElement(xmlNode as XmlElement, fe.Opacity, iosLeft, iosTop, iosWidth, iosHeight, elementName: elementName);
                iOSentity.removeViewbox = true;
            }

            if (iOSentity != null)
            {
                Rect fe_rect = new Rect()
                {
                    Width = iosWidth,
                    Height = iosHeight,
                    X = iosLeft,
                    Y = iosTop,
                };

                var intersect = controlRectList != null && !controlRectList.TrueForAll(controlRect => !controlRect.rect.IntersectsWith(fe_rect));

                if (intersect || (styleXMLMap.ContainsKey(fillName) && referenceBrushListMap.ContainsKey(styleXMLMap[fillName]) && referenceBrushListMap[styleXMLMap[fillName]].Contains("image")))
                    encapsule = true;
                iOSentity.forceEncapsulation = encapsule;
            }

            if (xmlNode != null)
            {
                    return xmlNode;
            }
            else
            {
                logDeploy.Error(string.Format(Properties.Resources.ErrorExportingShape, name, Document.Title));
                errorMessages = true;
            }
            return uieElement;
        }

        private XmlNode GetVideoElement(XmlDocument doc, double left, double top, Size size, string source, string videoClass, bool skipPosition = false)
        {
            XmlNode uieElement = doc.CreateElement(string.Empty, foreignObject, string.Empty);
            XmlNode content = doc.CreateElement(string.Empty, "video", string.Empty);
            content.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", doc));
            content.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", doc));
            content.Attributes.Append(JSONHelper.CreateNewAttribute("src", source, doc));
            content.Attributes.Append(JSONHelper.CreateNewAttribute("autoplay", $"autoplay", doc));
            content.Attributes.Append(JSONHelper.CreateNewAttribute("loop", $"loop", doc));
            if(Properties.Settings.Default.UnMuteVideos)
                content.Attributes.Append(JSONHelper.CreateNewAttribute("muted", $"muted", doc));
            double width = size.Width;
            double height = size.Height;

            if(!skipPosition)
            {
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"{JSONHelper.GetValue(left, true)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"{JSONHelper.GetValue(top, true)}", doc));
            }
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(width, true)}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(height, true)}", doc));

            content.Attributes.Append(JSONHelper.CreateNewAttribute("class", $"{videoClass}", doc));
            uieElement.AppendChild(content);
            return uieElement;
        }

        private XmlNode DrawArc(Arc ell,double left, double top, FrameworkElement fe, XmlDocument doc, bool updateEffects, out string fillName, out string strokeName, bool onlyInner = false, bool onlyOuter = false, Brush innerBrush = null)
        {
            XmlNode uieElement = null;
            var cxd = updateEffects ? left + fe.Width / 2 : fe.Width / 2;
            var cyd = updateEffects ? top + fe.Height / 2 : fe.Height / 2;
            var cx = JSONHelper.GetValue(cxd);
            var cy = JSONHelper.GetValue(cyd);
            var rx = JSONHelper.GetValue(fe.Width / 2 - ell.StrokeThickness);
            var ry = JSONHelper.GetValue(fe.Height / 2 - ell.StrokeThickness);


            if (Math.Abs(ell.StartAngle - ell.EndAngle) >= 360)
            {
                //var ry = rx;
                if (ell.ArcThicknessUnit == Microsoft.Expression.Media.UnitType.Percent || ell.ArcThickness <= 1)
                {
                    uieElement = doc.CreateElement(string.Empty, "ellipse", string.Empty);
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("cx", $"{cx}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("cy", $"{cy}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("rx", $"{rx}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{ry}", doc));
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness,true)}", doc));

                    fillName = ell.Fill == null ? "none" : GetCssClassStyle(ell.ArcThickness <= 1 ? Brushes.Transparent : ell.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                    strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                }
                else
                {
                    if (!onlyInner && !onlyOuter)
                        uieElement = doc.CreateElement(string.Empty, groupObject, string.Empty);
                    if (!onlyInner)
                    {
                        var uieElement1 = doc.CreateElement(string.Empty, "ellipse", string.Empty);
                        uieElement1.Attributes.Append(JSONHelper.CreateNewAttribute("cx", $"{cx}", doc));
                        uieElement1.Attributes.Append(JSONHelper.CreateNewAttribute("cy", $"{cy}", doc));
                        uieElement1.Attributes.Append(JSONHelper.CreateNewAttribute("rx", $"{rx}", doc));
                        uieElement1.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{ry}", doc));
                        uieElement1.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness,true)}", doc));

                        fillName = ell.Fill == null ? "none" : GetCssClassStyle(ell.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                        strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));

                        var node1 = UpdateStyle(doc, uieElement1, fillName, strokeName, left, top, fe.Width, fe.Height);
                        if (!onlyOuter)
                            uieElement.AppendChild(node1);
                        else
                            uieElement = node1;
                    }
                    if (!onlyOuter)
                    {
                        var rxd = fe.Width / 2 - ell.StrokeThickness;
                        var ryd = fe.Height / 2 - ell.StrokeThickness;
                        var uieElement2 = doc.CreateElement(string.Empty, "ellipse", string.Empty);
                        uieElement2.Attributes.Append(JSONHelper.CreateNewAttribute("cx", $"{cx}", doc));
                        uieElement2.Attributes.Append(JSONHelper.CreateNewAttribute("cy", $"{cy}", doc));
                        uieElement2.Attributes.Append(JSONHelper.CreateNewAttribute("rx", $"{JSONHelper.GetValue(rxd - ell.ArcThickness)}", doc));
                        uieElement2.Attributes.Append(JSONHelper.CreateNewAttribute("ry", $"{JSONHelper.GetValue(ryd - ell.ArcThickness)}", doc));
                        uieElement2.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness,true)}", doc));

                        fillName = ell.Fill == null ? "none" : GetCssClassStyle(innerBrush ?? ActiveLayer.Background, ColorMode.Fill, new Size(fe.Width, fe.Height));
                        strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));

                        var node2 = UpdateStyle(doc, uieElement2, fillName, strokeName, left, top, fe.Width, fe.Height);
                        if (!onlyInner)
                            uieElement.AppendChild(node2);
                        else
                            uieElement = node2;
                    }

                    fillName = ell.Fill == null ? "none" : GetCssClassStyle(ell.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                    strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
                }

                if (updateEffects)
                {
                    var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                    if (attribute != null)
                        uieElement.Attributes.Append(attribute);
                }
            }
            else
            {
                var rxd = Math.Min(fe.Width / 2, fe.Height / 2);
                uieElement = doc.CreateElement(string.Empty, "path", string.Empty);
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("d", $"{DescribeArc(cxd, cyd, rxd, ell)}", doc));
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{JSONHelper.GetValue(ell.StrokeThickness,true)}", doc));

                if (updateEffects)
                {
                    var attribute = GetTransform(fe, new Point(1, 1), false, false, true, doc);
                    if (attribute != null)
                        uieElement.Attributes.Append(attribute);
                }

                fillName = ell.Fill == null ? "none" : GetCssClassStyle(ell.Fill, ColorMode.Fill, new Size(fe.Width, fe.Height));
                strokeName = ell.Stroke == null ? "none" : GetCssClassStyle(ell.Stroke, ColorMode.Stroke, new Size(fe.Width, fe.Height));
            }

            if(!updateEffects && ell.Stroke != null && ell.Stroke is SolidColorBrush)
            {
                SolidColorBrush stroke = ell.Stroke as SolidColorBrush;
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke", $"{stroke}", doc));
            }


            return uieElement;
        }

        private string AddStyle(string id)
        {
            string styleName = $"{cssPrefix}{tagClipStyle}";
            int i = 0;
            while (referenceClipStyleMap.Keys.Contains(styleName))
            {
                styleName = $"{styleName}{++styleCounter}";
            }
            referenceClipStyleMap.Add(styleName, $"-webkit-clip-path: url(#{id}); clip-path: url(#{id}); object-fit:cover;");
            return styleName;
        }

        private string AddClipPathStyle(string clipPath)
        {
            string styleName = $"{cssPrefix}{tagClipStyle}";
            int i = 0;
            while (referenceClipStyleMap.Keys.Contains(styleName))
            {
                styleName = $"{styleName}{++styleCounter}";
            }
            referenceClipStyleMap.Add(styleName, $"-webkit-clip-path: {clipPath}; clip-path: {clipPath}; object-fit:cover;");
            return styleName;
        }

        int styleCounter;
        int defCounter;
        private string AddDef(XmlNode uieDef, XmlDocument doc)
        {
            string defName = $"{cssPrefix}{tagClipDef}";
            int i = 0;
            while (referenceClipDefMap.Keys.Contains(defName))
            {
                defName = $"{defName}{++defCounter}";
            }
            uieDef.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{defName}", doc));
            referenceClipDefMap.Add(defName, (uieDef).ToXml());
            return defName;
        }

        private string GetPoints(PointCollection points)
        {
            var ret = String.Join(" ", points.Select(p => $"{JSONHelper.GetValue(p.X, true)},{JSONHelper.GetValue(p.Y, true)}").ToList());
            return ret;
        }

        void UpdateEffect(XmlDocument doc, XmlNode uieElement, System.Windows.Media.Effects.Effect effect )
        {
            if (effect == null)
                return;
            XmlNode filterNode = null;
            XmlDocument document = new XmlDocument();
            string filter = null;
            if (effect is System.Windows.Media.Effects.BlurEffect)
            {
                var radius = (effect as System.Windows.Media.Effects.BlurEffect).Radius;
                filter = $"defsFilterBlur_{radius}";
                if (filterMapList.ContainsKey(filter))
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute($"filter", $"url(#{filter})", doc));
                else
                {
                    filterNode = document.CreateElement(string.Empty, "filter", string.Empty);
                    filterNode.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{filter}", document));
                    filterNode.Attributes.Append(JSONHelper.CreateNewAttribute("filterUnits", $"userSpaceOnUse", document));
                    XmlNode feNode = document.CreateElement(string.Empty, "feGaussianBlur", string.Empty);
                    feNode.Attributes.Append(JSONHelper.CreateNewAttribute("in", $"SourceGraphic", document));
                    feNode.Attributes.Append(JSONHelper.CreateNewAttribute("stdDeviation", $"{JSONHelper.GetValue(radius/3)}", document));
                    filterNode.AppendChild(feNode);
                    filterMapList.Add(filter, filterNode.ToXml());
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute($"filter", $"url(#{filter})", doc));
                }
            }
        }

        private XmlNode UpdateStyle(XmlDocument doc, XmlNode uieElement, string fillName, string strokeName, double left, double top, double width, double height)
        {
            string style = null;
            if (styleXMLMap.ContainsKey(fillName))
            {
                style = styleXMLMap[fillName];
                if (!styleXMLMap.ContainsKey(strokeName))
                {
                    uieElement.Attributes.Append(JSONHelper.CreateNewAttribute($"class", fillName, doc));
                    return uieElement;
                }
            }
            else if(styleXMLMap.ContainsKey(strokeName))
            {
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute($"class", strokeName, doc));
                return uieElement;
            }

            if (styleXMLMap.ContainsKey(strokeName))
                style = style + styleXMLMap[strokeName];
            if (string.IsNullOrEmpty(style))
                return uieElement;
            List<string> styles = style?.Split(';').ToList();
            if (styles != null)
                styles.ForEach(s =>
                {
                    string[] nstyle = s.Split(':');
                    if (nstyle.Count() == 2)
                        uieElement.Attributes.Append(JSONHelper.CreateNewAttribute($"{nstyle[0]}", $"{nstyle[1]}", doc));
                });
            return uieElement;
        }
        class InfoLine
        {
            public int Shades { get; set; }
            public double StrokeThickness { get; set; }
            public string Points { get; set; }
            public Brush Stroke { get; set; }
            public Brush Fill { get; set; }
            public double Left { get; set; }
            public double Top { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public bool UseRound { get; set; }
            public bool UseFill { get; set; }
            public DoubleCollection StrokeDashArray { get; set; }
            public string LineJoin { get; set; }
        }
        void GetLineColors(InfoLine infoLine, out Color color1, out Color color2)
        {
            color1 = Colors.White;
            color2 = Colors.Black;

            if (infoLine.Stroke is SolidColorBrush)
            {
                if (infoLine.Stroke.HasAnimatedProperties)
                    color2 = (Color)(infoLine.Stroke as SolidColorBrush).GetAnimationBaseValue(SolidColorBrush.ColorProperty);
                else
                    color2 = (infoLine.Stroke as SolidColorBrush).Color;
            }
            if (infoLine.Fill is SolidColorBrush)
            {
                if (infoLine.Fill.HasAnimatedProperties)
                    color1 = (Color)(infoLine.Fill as SolidColorBrush).GetAnimationBaseValue(SolidColorBrush.ColorProperty);
                else
                    color1 = (infoLine.Fill as SolidColorBrush).Color;
            }
        }
        private XmlNode DrawLine(int shade, InfoLine infoLine, XmlDocument doc, bool updateStyle = true)
        {
            Color color1 = Colors.White;
            Color color2 = Colors.Black;

            GetLineColors(infoLine, out color1, out color2);

            int shades = Math.Max(infoLine.Shades, 1);
            var percentage = (float)shade / (float)shades;
            SolidColorBrush brush = new SolidColorBrush(Pipeline.Pipeline.InterpolateColors(color1, color2, percentage));
            string thickness = JSONHelper.GetValue(infoLine.StrokeThickness * percentage, true);

            XmlNode uieElement = doc.CreateElement(string.Empty, "polyline", string.Empty);
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("points", $"{infoLine.Points}", doc));
            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-width", $"{thickness}", doc));

            var strokeName = !updateStyle || infoLine.Stroke == null ? "none" : GetCssClassStyle(brush, ColorMode.Stroke, new Size(infoLine.Width, infoLine.Height));
            var fillName = !updateStyle || infoLine.Fill == null || !infoLine.UseFill ? "none" : GetCssClassStyle(infoLine.Fill, ColorMode.Fill, new Size(infoLine.Width, infoLine.Height));
            if(!infoLine.UseFill || fillName == "none")
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("fill", $"none", doc));
            if(infoLine.UseRound)
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-linecap", $"round", doc));

            uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-linejoin", $"{infoLine.LineJoin}", doc));

            if(infoLine.StrokeDashArray != null && infoLine.StrokeDashArray.Count > 0)
            {
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute("stroke-dasharray", $"{infoLine.StrokeDashArray.ToString()}", doc));
            }
            if(updateStyle)
                return UpdateStyle(doc, uieElement, fillName, strokeName, infoLine.Left, infoLine.Top, infoLine.Width, infoLine.Height);
            else
            {
                uieElement.Attributes.Append(JSONHelper.CreateNewAttribute($"stroke", ColorConverterExtensions.ToHexString(brush.Color), doc));
                return uieElement;
            }

        }

        private object DescribeBorder(Border border, double left, double top, double width, double height)
        {
            var xs1 = JSONHelper.GetValue(left + border.CornerRadius.TopLeft);
            var ys1 = JSONHelper.GetValue(top);
            var topLeft = border.CornerRadius.TopLeft;
            var topRight = border.CornerRadius.TopRight;
            var bottomLeft = border.CornerRadius.BottomLeft;
            var bottomRight = border.CornerRadius.BottomRight;

            var d1 = width - topLeft - topRight;
            var d2 = height - topRight - bottomRight;
            var d3 = width - bottomRight - bottomLeft;
            var d4 = height - topLeft - bottomLeft;

            var ds1 = JSONHelper.GetValue(d1);
            var ds2 = JSONHelper.GetValue(d2);
            var ds3 = JSONHelper.GetValue(d3);
            var ds4 = JSONHelper.GetValue(d4);



            var topLeftR = JSONHelper.GetValue(topLeft);
            var topRightR = JSONHelper.GetValue(topRight);
            var bottomLeftR = JSONHelper.GetValue(bottomLeft);
            var bottomRightR = JSONHelper.GetValue(bottomRight);

            return $"M {xs1} {ys1} " +
                   $"h{ds1} " +
                   $"a{topRightR},{topRightR} 0 0 1 {topRightR},{topRightR}" +
                   $"v{ds2} " +
                   $"a{bottomRightR},{bottomRightR} 0 0 1 -{bottomRightR},{bottomRightR}" +
                   $"h-{ds3} " +
                   $"a{bottomLeftR},{bottomLeftR} 0 0 1 -{bottomLeftR},-{bottomLeftR}" +
                   $"v-{ds4} " +
                   $"a{topLeftR},{topLeftR} 0 0 1 {topLeftR},-{topLeftR}" +
                   $" z";
        }

        private string DescribeArc(double centerX, double centerY, double radius, Microsoft.Expression.Shapes.Arc arc)
        {
            Point startArc = new Point();
            Point endArc = new Point();

            Point startInternalArc = new Point();
            Point endInternalArc = new Point();
            double internalRadius = radius - arc.ArcThickness;
            if (arc.FlowDirection == FlowDirection.LeftToRight)
            {
                PolarToCartesian(centerX, centerY, radius, arc.StartAngle, out startArc);
                PolarToCartesian(centerX, centerY, radius, arc.EndAngle, out endArc);


                PolarToCartesian(centerX, centerY, internalRadius, arc.StartAngle, out startInternalArc);
                PolarToCartesian(centerX, centerY, internalRadius, arc.EndAngle, out endInternalArc);
            }
            else
            {
                PolarToCartesian(centerX, centerY, radius, 360 - arc.StartAngle, out endArc);
                PolarToCartesian(centerX, centerY, radius, 360 - arc.EndAngle, out startArc);

                PolarToCartesian(centerX, centerY, internalRadius, 360 - arc.StartAngle, out endInternalArc);
                PolarToCartesian(centerX, centerY, internalRadius, 360 - arc.EndAngle, out startInternalArc);
            }

            if (internalRadius > 0 && arc.ArcThicknessUnit == Microsoft.Expression.Media.UnitType.Pixel)
            {
                return $"M{(int)startArc.X} {(int)startArc.Y} " +
                       $"A {(int)radius} {(int)radius} 0 1 1 {(int)endArc.X} {(int)endArc.Y}" +
                       $"L {(int)endInternalArc.X} {(int)endInternalArc.Y}" +
                       $"A {(int)internalRadius} {(int)internalRadius} 0 1 0 {(int)startInternalArc.X} {(int)startInternalArc.Y}" +
                       $"Z";
            }
            else
            {
                return $"M{(int)startArc.X} {(int)startArc.Y} " +
                               $"A {(int)radius} {(int)radius} 0 1 1 {(int)endArc.X} {(int)endArc.Y}" +
                               $"L {(int)centerX} {(int)centerY}" +
                               $"Z";
            }
        }

        private void PolarToCartesian(double centerX, double centerY, double radius, double angleInDegrees, out Point point)
        {
            point = new Point(0, 0);
            var angleInRadians = (angleInDegrees - 90) * Math.PI / 180.0;
            point.X = centerX + (radius * Math.Cos(angleInRadians));
            point.Y = centerY + (radius * Math.Sin(angleInRadians));
        }

        internal XmlElement CreateRootElement()
        {
            XmlElement rootElement = xmlDoc.CreateElement(string.Empty, "svg", string.Empty);
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns", "http://www.w3.org/2000/svg", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink", xmlDoc));

            defsElement = xmlDoc.CreateElement(string.Empty, "defs", string.Empty);
            rootElement.AppendChild(defsElement);

            styleElement = xmlDoc.CreateElement(string.Empty, "style", string.Empty);
            styleElement.Attributes.Append(JSONHelper.CreateNewAttribute("type", "text/css", xmlDoc));
            rootElement.AppendChild(styleElement);

            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", "{0}", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("image-rendering", "optimizeSpeed", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("baseProfile", "basic", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("version", "1.1", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"0px", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"0px", xmlDoc));
            if(Document.FitInWindow)
            {
                rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", xmlDoc));
                rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", xmlDoc));
                if (!Document.KeepAspectRatio)
                    rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", xmlDoc));
            }
            else
            {
                rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(Document.Width, true)}", xmlDoc));
                rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(Document.Height, true)}", xmlDoc));
            }

            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("viewBox", $"0 0 {JSONHelper.GetValue(Document.Width)} {JSONHelper.GetValue(Document.Height)}", xmlDoc));
            rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("enable-background", $"new 0 0 {JSONHelper.GetValue(Document.Width)} {JSONHelper.GetValue(Document.Height)}", xmlDoc));

            string cssClassName = GetCssClassStyle(ActiveLayer.Background, ColorMode.Both, new Size(Document.Width, Document.Height));
            if (!string.IsNullOrEmpty(cssClassName))
            {
                string style = null;
                if (styleXMLMap.ContainsKey(cssClassName))
                    style = styleXMLMap[cssClassName];
                XmlDocument doc = new XmlDocument();
                //if (!string.IsNullOrEmpty(style) && referenceBrushListMap.ContainsKey(style))
                //{
                //    XmlElement image = doc.ImportNode(referenceBrushListMap[style].FromXml<XmlElement>(), true) as XmlElement;
                //    image.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", doc));
                //    image.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", doc));
                //    image.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", doc));
                //    entityList.Add(image.ToXml());
                //    image = null;
                //}
                //else
                {
                    XmlElement backgroundElement = doc.CreateElement(string.Empty, "rect", string.Empty);
                    backgroundElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", "100%", doc));
                    backgroundElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", "100%", doc));
                    backgroundElement.Attributes.Append(JSONHelper.CreateNewAttribute("class", $"{cssClassName}", doc));
                    entityList.Add(backgroundElement.ToXml());
                    backgroundElement = null;
                }
            }

            return rootElement;
        }

        string GetCssClassStyle(Brush brush, ColorMode mode, Size size)
        {
            if (brush == null)
                brush = Brushes.Transparent;
            string keyPrefix = $"{mode}{tagBaseStyle}_";
            string xmlBrush = GetXamlBrush(brush, size);
            if (xmlBrush == null)
                xmlBrush = GetXamlBrush(Brushes.Transparent, size);
            string styleName = (from key in styleMap.Keys 
                                where styleMap[key] == xmlBrush && key.StartsWith(keyPrefix)
                                select key).FirstOrDefault();
            if (!string.IsNullOrEmpty(styleName))
                return styleName;
            else
            {
                styleName = $"{keyPrefix}{cssPrefix}{styleMap.Count}";
                string newStyle = GetNewStyle(brush, styleName, mode, size);
                if (!string.IsNullOrEmpty(newStyle))
                {
                    styleMap.Add(styleName, xmlBrush);
                    styleXMLMap.Add(styleName, newStyle);
                    //styleElement.InnerText = $"{styleElement.InnerText}{Environment.NewLine}{newStyle}";
                    return styleName;
                }
                else
                    return null;
            }
        }

        private string GetXamlBrush(Brush brush, Size size)
        {
            if (brush is VisualBrush)
            {
                if ((brush as VisualBrush).Visual is MediaElement)
                {
                    MediaElement image = ((brush as VisualBrush).Visual as MediaElement);
                    if(image.Source != null)
                        return XamlWriter.Save(image.Source) + size.ToString();
                    else
                    {
                        BindingExpression sourceBinding = image.GetBindingExpression(MediaElement.SourceProperty);
                        if (sourceBinding != null && sourceBinding.ParentBinding != null)
                        {
                            return XamlWriter.Save(sourceBinding.ParentBinding) + size.ToString();
                        }
                        else
                            return null;
                    }
                }
                else if ((brush as VisualBrush).Visual is Image)
                {
                    Image image = ((brush as VisualBrush).Visual as Image);
                    if (image.Source != null)
                    {
                        if(image.Source is System.Windows.Media.Imaging.BitmapImage && (image.Source as System.Windows.Media.Imaging.BitmapImage).UriSource != null)
                            return XamlWriter.Save((image.Source as System.Windows.Media.Imaging.BitmapImage).UriSource);
                        else
                            return XamlWriter.Save(image.Source.ToString()) + size.ToString();
                    }
                    else
                    {
                        BindingExpression sourceBinding = image.GetBindingExpression(System.Windows.Controls.Image.SourceProperty);
                        if (sourceBinding != null && sourceBinding.ParentBinding != null)
                        {
                            return XamlWriter.Save(sourceBinding.ParentBinding) + size.ToString();
                        }
                        else
                            return null;
                    }
                }
                else
                    return null;
            }
            else if (brush is ImageBrush && (brush as ImageBrush).ImageSource != null)
            {
                ImageBrush image = (brush as ImageBrush);
                if (image.ImageSource is System.Windows.Media.Imaging.BitmapImage && (image.ImageSource as System.Windows.Media.Imaging.BitmapImage).UriSource != null)
                    return XamlWriter.Save((image.ImageSource as System.Windows.Media.Imaging.BitmapImage).UriSource);
                else
                    return XamlWriter.Save(image.ImageSource.ToString());
            }
            else
                return XamlWriter.Save(brush);
        }

        private string GetNewStyle(Brush brush, string styleName, ColorMode mode, Size size)
        {
            if (brush == null)
                brush = Brushes.Transparent;

            XmlElement brushElement;
            if (brush is SolidColorBrush)
            {
                SolidColorBrush solidBrush = brush as SolidColorBrush;
                string color = ColorConverterExtensions.ToHexString(solidBrush.Color);
                var alpha = (double)solidBrush.Color.A / 255;
                return ComposeStyle(color, alpha, mode);
            }
            else if (brush is LinearGradientBrush)
            {
                XmlDocument doc = new XmlDocument();
                LinearGradientBrush linearBrush = brush as LinearGradientBrush;
                brushElement = doc.CreateElement(string.Empty, "linearGradient", string.Empty);
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{styleName}_", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("gradientUnits", $"objectBoundingBox", doc));
                Point start = linearBrush.StartPoint;
                Point end = linearBrush.EndPoint;
                UpdatePoints(start, end, size);
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("x1", $"{JSONHelper.GetValue(start.X * 100)}%", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("y1", $"{JSONHelper.GetValue(start.Y * 100)}%", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("x2", $"{JSONHelper.GetValue(end.X * 100)}%", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("y2", $"{JSONHelper.GetValue(end.Y * 100)}%", doc));
                linearBrush.GradientStops.OrderBy(g => g.Offset).ToList().ForEach(g =>
                {
                    XmlElement gradientElement = doc.CreateElement(string.Empty, "stop", string.Empty);
                    gradientElement.Attributes.Append(JSONHelper.CreateNewAttribute("offset", $"{Math.Round(g.Offset * 100)}%", doc));
                    gradientElement.Attributes.Append(JSONHelper.CreateNewAttribute("stop-color", $"{ColorConverterExtensions.ToHexString(g.Color)}", doc));
                    gradientElement.Attributes.Append(JSONHelper.CreateNewAttribute("stop-opacity", $"{Math.Round((double)g.Color.A / 255 * 100)}%", doc));
                    brushElement.AppendChild(gradientElement);
                });

                referenceBrushList.Add(brushElement.ToXml());
                string color = $"url(#{styleName}_)";
                var alpha = (double)linearBrush.Opacity;
                return ComposeStyle(color,alpha,mode);
            }
            else if (brush is RadialGradientBrush)
            {
                XmlDocument doc = new XmlDocument();
                RadialGradientBrush radialBrush = brush as RadialGradientBrush;
                brushElement = doc.CreateElement(string.Empty, "radialGradient", string.Empty);
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{styleName}_", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("gradientUnits", $"objectBoundingBox", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("cx", $"{JSONHelper.GetValue(radialBrush.GradientOrigin.X)}", doc));
                brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("cy", $"{JSONHelper.GetValue(radialBrush.GradientOrigin.Y)}", doc));
                //brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("fx", $"{JSONHelper.GetValue(radialBrush.RadiusX)}", doc));
                //brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("fy", $"{JSONHelper.GetValue(radialBrush.RadiusY)}", doc));
                radialBrush.GradientStops.OrderBy(g =>g.Offset).ToList().ForEach(g =>
                {
                    XmlElement gradientElement = doc.CreateElement(string.Empty, "stop", string.Empty);
                    gradientElement.Attributes.Append(JSONHelper.CreateNewAttribute("offset", $"{Math.Round(g.Offset * 100)}%", doc));
                    gradientElement.Attributes.Append(JSONHelper.CreateNewAttribute("stop-color", $"{ColorConverterExtensions.ToHexString(g.Color)}", doc));
                    gradientElement.Attributes.Append(JSONHelper.CreateNewAttribute("stop-opacity", $"{Math.Round((double)g.Color.A / 255 * 100)}%", doc));
                    brushElement.AppendChild(gradientElement);
                });

                referenceBrushList.Add(brushElement.ToXml());
                string color = $"url(#{styleName}_)";
                var alpha = (double)radialBrush.Opacity;
                return ComposeStyle(color, alpha, mode);
            }
            else if (brush is VisualBrush)
            {
                VisualBrush visualBrush = brush as VisualBrush;
                XmlDocument doc = new XmlDocument();
                brushElement = GetPattern(styleName, doc, size, "none", brush);
                if (brushElement == null)
                    return null;
                referenceBrushList.Add(brushElement.ToXml());
                string color = $"url(#{styleName}_)";
                var alpha = (double)visualBrush.Opacity;
                string style = ComposeStyle(color, alpha, mode);
                referenceBrushListMap.Add(style, brushElement.ToXml());
                return style;
            }
            else if (brush is ImageBrush)
            {
                ImageBrush imageBrush = brush as ImageBrush;
                string aspectRatio = GetAspectRatio(imageBrush.Stretch);
                XmlDocument doc = new XmlDocument();
                brushElement = GetPattern(styleName, doc, size, $"{aspectRatio}", brush);
                if (brushElement == null)
                    return null;
                referenceBrushList.Add(brushElement.ToXml());
                string color = $"url(#{styleName}_)";
                var alpha = (double)imageBrush.Opacity;
                string style = ComposeStyle(color, alpha, mode);
                referenceBrushListMap.Add(style, brushElement.ToXml());
                return style;
            }
            else
                return null;
        }

        XmlElement GetPattern(string styleName, XmlDocument doc, Size uieSize, string aspectRatio, Brush brush)
        {
            string source = WPFUtilities.ImageHelper.BrushToImagePath(Document, brush);
            if (source == null)
                return null;

            Uri sourceUri = WPFUtilities.ImageHelper.BrushToImageUri(Document, brush);
            if (sourceUri == null)
                return null;

            XmlElement pattern = doc.CreateElement(string.Empty, "pattern", string.Empty);
            pattern.Attributes.Append(JSONHelper.CreateNewAttribute("id", $"{styleName}_", doc));
            pattern.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"1", doc));
            pattern.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"1", doc));
            pattern.Attributes.Append(JSONHelper.CreateNewAttribute("patternContentUnits", $"userSpaceOnUse", doc));

            XmlElement brushElement = doc.CreateElement(string.Empty, "image", string.Empty);
            brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"0", doc));
            brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"0", doc));

            try
            {
                //using (var imageStream = File.OpenRead(sourceUri.GetPathString()))
                //{
                //    var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(imageStream,
                //        System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreColorProfile,
                //        System.Windows.Media.Imaging.BitmapCacheOption.Default);
                //    var iheight = decoder.Frames[0].PixelHeight;
                //    var iwidth = decoder.Frames[0].PixelWidth;
                //    var height = JSONHelper.GetValue(iheight, true);
                //    var width = JSONHelper.GetValue(iwidth, true);
                //    var scaleX = JSONHelper.GetValue(uieSize.Width / iwidth, true);
                //    var scaleY = JSONHelper.GetValue(uieSize.Height / iheight, true);
                    brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"{JSONHelper.GetValue(uieSize.Width,true)}", doc));
                    brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"{JSONHelper.GetValue(uieSize.Height, true)}", doc));
                    pattern.Attributes.Append(JSONHelper.CreateNewAttribute("viewbox", $"0 0 {JSONHelper.GetValue(uieSize.Width, true)} {JSONHelper.GetValue(uieSize.Height, true)}", doc));

                    //brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("transform", $"scale({scaleX} {scaleY})", doc));
//                }
            }
            catch
            {
            }

            brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("xlink:href", $"{source}", doc));
            brushElement.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"{aspectRatio}", doc));

            pattern.AppendChild(brushElement);
            return pattern;
        }

        private void UpdatePoints(Point start, Point end, Size size)
        {
            double angle = JSONHelper.GetDoubleDegree(start, end, size);
            var m = Math.Tan(angle);
            if (double.IsInfinity(m))
            {
                start.X = 0;
                start.Y = 0;
                end.X = 0;
                end.Y = 1;
            }
            else if (double.IsNaN(m))
            {
                start.X = 0;
                start.Y = 0;
                end.X = 1;
                end.Y = 0;
            }
            else
            {
                end.X = 2;
                end.Y = m*(2- start.X)+start.Y;
            }
        }

        private string ComposeStyle(string color, double alpha, ColorMode mode)
        {
            switch (mode)
            {
                case ColorMode.Fill:
                    return $"fill:{color};fill-opacity:{JSONHelper.GetValue(alpha)};";
                case ColorMode.Stroke:
                    return $"stroke:{color};stroke-opacity:{JSONHelper.GetValue(alpha)};";
                case ColorMode.Both:
                    return $"fill:{color};stroke:{color};fill-opacity:{JSONHelper.GetValue(alpha)};stroke-opacity:{JSONHelper.GetValue(alpha)};";
                default:
                    return $"fill:{color};stroke:{color};fill-opacity:{JSONHelper.GetValue(alpha)};stroke-opacity:{JSONHelper.GetValue(alpha)};";
            }
        }

        private string GetAspectRatio(Stretch stretch)
        {
            if (stretch == Stretch.None)
                return "false";
            else
                return "true";
        }
        void AddReference(ScreenSettings.Entities.ScreenEntity entity, FrameworkElement fe,
                                string item, 
                                Dictionary<string, List<string>> expressionVariables,
                                Dictionary<string, OPCUAEntityReference> resolvedVariables,
                                out int? entityrefID, OPCUAEntityReference entityreference = null)
        {
            OPCUAEntityReference tagRef = GetItemTagReferece(entity, entityreference);
            SVGHelper SVGHelper = null;
            if (fe != null && fe is IStatisticTagAware && 
                (StatDef.StatProps)(fe as IStatisticTagAware).GetStatisticType() != StatDef.StatProps.None)
                SVGHelper = GetStatisticTagAwareHelper(item, (fe as IStatisticTagAware), tagRef);
            else
                SVGHelper = GetTagHelper(tagRef, entity?.Expression, entity?.ReverseExpression);

            entityrefID = SVGHelper?.SVGReferenceId;
            if (SVGHelper != null)
            {
                entityMapList.Add(new OPCUAEntityReferenceMap()
                {
                    SVGReferenceId = SVGHelper.SVGReferenceId,
                    ScreenEntityName = item,
                    Kind = ReferenceType.Tag
                });
            }
            /*
            if (!string.IsNullOrEmpty(entity.Expression))
            {
                PopulateExpressionVariables(entity.Expression, expressionVariables);
                expressionVariables[entity.Expression].ForEach(variable =>
                {
                    SVGHelper expSVGHelper = GetTagHelper(null, GetResolvedReference(variable, resolvedVariables));
                    if(expSVGHelper != null)
                    {
                        entityMapList.Add(new OPCUAEntityReferenceMap()
                        {
                            SVGReferenceId = expSVGHelper.SVGReferenceId,
                            ScreenEntityName = item,
                            Kind = ReferenceType.Expression
                        });
                    }
                });
            }
            if (!string.IsNullOrEmpty(entity.ReverseExpression))
            {
                PopulateExpressionVariables(entity.ReverseExpression, expressionVariables);
                expressionVariables[entity.ReverseExpression].ForEach(variable =>
                {
                    SVGHelper expSVGHelper = GetTagHelper(null, GetResolvedReference(variable, resolvedVariables));
                    if (expSVGHelper != null)
                    {
                        entityMapList.Add(new OPCUAEntityReferenceMap()
                        {
                            SVGReferenceId = expSVGHelper.SVGReferenceId,
                            ScreenEntityName = item,
                            Kind = ReferenceType.ReverseExpression
                        });
                    }
                });
            }
            */
            if (entity.SourceSymbolLinked && fe is ContentControl && !(fe is UserControl) && (fe as ContentControl).Content is FrameworkElement)
                fe = (fe as ContentControl).Content as FrameworkElement;

            if (fe != null && fe is IDynamicTagAware)
            {                
                //get IDynamicTagAware references
                var dynamicMap = (fe as IDynamicTagAware).GetMapDynamics();

                foreach (var k in dynamicMap.Keys)
                {
                    OPCUAEntityReference newreference = dynamicMap[k].FromXml<OPCUAEntityReference>();
                    if(IsReferenceValid(newreference))
                    {
                        SVGHelper expSVGHelper = GetTagHelper(null, newreference);
                        if (expSVGHelper != null)
                        {
                            entityMapList.Add(new OPCUAEntityReferenceMap()
                            {
                                SVGReferenceId = expSVGHelper.SVGReferenceId,
                                ScreenEntityName = item,
                                Kind = ReferenceType.IDynamicTag,
                                RefName = k
                            });
                            UpdateContextDynamicTagAwareReference(item, k, expSVGHelper.SVGReferenceId);
                        }
                    }
                    else
                    {
                        logDeploy.Error(string.Format(Properties.Resources.ErrorTagReferenceNotValid, item, Document.Title));
                        errorMessages = true;
                    }
                }
            }
        }

        private SVGHelper GetStatisticTagAwareHelper(string item, IStatisticTagAware statisticTagAware, OPCUAEntityReference tag)
        {
            if (!IsReferenceValid(tag))
                return null;
            StatDef.StatProps stype = (StatDef.StatProps)statisticTagAware.GetStatisticType();
            if (stype != StatDef.StatProps.None)
            {
                if (StatDef.StatisticsDefinitions.statParams.ContainsKey(stype))
                {
                    var statName = StatDef.StatisticsDefinitions.statParams[stype];
                    if(!string.IsNullOrEmpty(statName))
                    {
                        string project = tag.HumanReadable.Replace(tag.HumanReadableNoProject, "");
                        OPCUAEntityReference newreference = new OPCUAEntityReference(
                            tag.HostName, 
                            tag.AppName,
                            tag.EndpointUrl, 
                            $"{tag.RelativePath}/{statName}", 
                            new Opc.Ua.NodeId($"{tag.ResolvedNodeId.Identifier}?{statName}"), 
                            $"{tag.HumanReadableNoProject}\\{statName}{project}", 
                            Opc.Ua.VariableTypeIds.PropertyType);
                        newreference.ReadablePath = $"{tag.RelativePath}/{statName}";
                        SVGHelper statSVGHelper = GetTagHelper(null, newreference);
                        return statSVGHelper;
                    }
                }
            }
            return null;
        }

        private OPCUAEntityReference GetItemTagReferece(ScreenEntity entity, OPCUAEntityReference parentEntityReference)
        {
            bool isEntityRefValid = IsReferenceValid(entity?.OpcuaEntityReference);
            bool isParentEntityRefValid = IsReferenceValid(parentEntityReference);

            if (isEntityRefValid)
                return entity?.OpcuaEntityReference;
            else if (isParentEntityRefValid)
                return parentEntityReference;

            return null;
        }

        private SVGHelper GetItemTagHelper(ScreenEntity entity, OPCUAEntityReference parentEntityReference)
        {
            bool isEntityRefValid = IsReferenceValid(entity?.OpcuaEntityReference);
            bool isParentEntityRefValid = IsReferenceValid(parentEntityReference);
            SVGHelper helper = null;

            if (isEntityRefValid)
                return GetTagHelper(entity?.OpcuaEntityReference, entity?.Expression, entity?.ReverseExpression);
            else if (isParentEntityRefValid)
                return GetTagHelper(parentEntityReference, entity?.Expression, entity?.ReverseExpression);

            return helper;
        }

        private SVGHelper GetAnimationTagHelper(AnimationManager.AnimationManager animation, OPCUAEntityReference parentEntityReference,
                                ScreenEntity sentity = null)
        {
            bool isEAnimationntityRefValid = IsReferenceValid(animation?.OpcuaEntityReference);
            bool isParentEntityRefValid = IsReferenceValid(parentEntityReference);
            SVGHelper helper = null;

            if (isEAnimationntityRefValid)
                return GetTagHelper(animation?.OpcuaEntityReference, animation?.Expression, null);
            else if (isParentEntityRefValid)
                return GetTagHelper(parentEntityReference, animation?.Expression ?? sentity?.Expression, null);

            return helper;
        }

        private SVGHelper GetCommandTagHelper(CommandManager.CommandManager command, OPCUAEntityReference parentEntityReference,
                                ScreenEntity sentity = null)
        {
            bool isCommandEntityRefValid = IsReferenceValid(command?.OpcuaEntityReference);
            bool isParentEntityRefValid = IsReferenceValid(parentEntityReference);
            SVGHelper helper = null;

            if (isCommandEntityRefValid)
                return GetTagHelper(command?.OpcuaEntityReference, command?.Expression, null);
            else if (isParentEntityRefValid)
                return GetTagHelper(parentEntityReference, command?.Expression ?? sentity?.Expression, null);

            return helper;
        }
        private SVGHelper GetTagHelper(OPCUAEntityReference entityReference, OPCUAEntityReference parentEntityReference)
        {
            bool isEntityRefValid = IsReferenceValid(entityReference);
            bool isParentEntityRefValid = IsReferenceValid(parentEntityReference);
            SVGHelper helper = null;
            string humanReadable = string.Empty;
            OPCUAEntityReference tag = null;
            if (isEntityRefValid)
                return GetTagHelper(entityReference, null, null);
            else if (isParentEntityRefValid)
                return GetTagHelper(parentEntityReference, null, null); 

            return helper;
        }
        private SVGHelper GetTagHelper(OPCUAEntityReference tag, string expression, string revExpression)
        {
            if (tag == null)
                return null;

            SVGHelper helper = null;

            if (IsReferenceValid(tag))
            {
                string key = $"{tag.HumanReadable}|{tag.ReadablePath}|{tag.ParentTypeDefinitionName}|{expression}|{revExpression}";
                if (sVGHelpermap.ContainsKey(key))
                    helper = sVGHelpermap[key];
                else
                {
                    int refId = sVGHelperList.Count + 1;
                    SVGHelper sVGHelper = new SVGHelper()
                    {
                        Tag = tag,
                        SVGReferenceId = refId
                    };

                    if (!string.IsNullOrEmpty(expression))
                        sVGHelper.Expression = expression;
                    if (!string.IsNullOrEmpty(revExpression))
                        sVGHelper.ReverseExpression = revExpression;

                    if(tag.ResolvedNodeId != null && eumap.ContainsKey(tag.ResolvedNodeId.Identifier.ToString()))
                    {
                        var eu = eumap[tag.ResolvedNodeId.Identifier?.ToString()].Split(';');
                        if (eu.Count() >= 3)
                        {
                            double min;
                            double max;
                            sVGHelper.EUnit = eu[0];
                            if (!string.IsNullOrEmpty(eu[1]) && double.TryParse(eu[1], out min))
                                sVGHelper.EUMin = min;
                            if (!string.IsNullOrEmpty(eu[1]) && double.TryParse(eu[2], out max))
                                sVGHelper.EUMax = max;
                        }
                    }
                    sVGHelperList.Add(sVGHelper);
                    helper = sVGHelper;
                    sVGHelpermap.Add(key, helper);
                }
            }

            return helper;
        }
        private void UpdateContextDynamicTagAwareReference(string item, string id, int? svgKeyCodeNumber)
        {
            var foreign = (from f in foreignObjectList where f.SVGItemId == item select f).FirstOrDefault();
            if (foreign != null)
            {
                if (foreign.parameters == null)
                    foreign.parameters = new Dictionary<string, object>();
                if (foreign.parameters.ContainsKey(id))
                    foreign.parameters[id] = svgKeyCodeNumber;
                else
                    foreign.parameters.Add(id, svgKeyCodeNumber);
            }
        }

        void AddCommandReference(string item,
                                CommandManager.CommandManager command,
                                Dictionary<string, List<string>> expressionVariables,
                                Dictionary<string, OPCUAEntityReference> resolvedVariables,
                                OPCUAEntityReference entityreference = null,
                                ScreenEntity sentity = null)
        {
            if (command != null)
            {
                command.SVGItemId = item;
                SVGHelper SVGHelper = GetCommandTagHelper(command, entityreference, sentity);
                if (SVGHelper != null)
                {
                    command.SVGReferenceId = SVGHelper.SVGReferenceId;
                    entityMapList.Add(new OPCUAEntityReferenceMap()
                    {
                        SVGReferenceId = SVGHelper.SVGReferenceId,
                        ScreenEntityName = item,
                        Kind = ReferenceType.Command,
                        RefName = command.Name
                    });
                }

                if (command is ValueCommand)
                {
                    (command as ValueCommand).SVGTransferReferenceId = GetReferenceID((command as ValueCommand)?.TransferToTag, command.Name, item);
                    (command as ValueCommand).SVGMaxValueReferenceId = GetReferenceID((command as ValueCommand)?.TagMaxValue, command.Name, item);
                    (command as ValueCommand).SVGMinValueReferenceId = GetReferenceID((command as ValueCommand)?.TagMinValue, command.Name, item);
                }
                else if (command is ReportCommand)
                {
                    (command as ReportCommand).Parameters?.ForEach(p =>
                    {
                        p.SVGReferenceId = GetReferenceID(p.TagRef, command.Name, item);
                    });                       
                }
                /*
                if (command["Expression"] && !string.IsNullOrEmpty(command.Expression))
                {
                    PopulateExpressionVariables(command.Expression, expressionVariables);
                    expressionVariables[command.Expression].ForEach(variable =>
                    {
                        SVGHelper expSVGHelper = GetTagHelper(null, GetResolvedReference(variable, resolvedVariables));
                        if (expSVGHelper != null)
                        {
                            entityMapList.Add(new OPCUAEntityReferenceMap()
                            {
                                SVGReferenceId = expSVGHelper.SVGReferenceId,
                                ScreenEntityName = item,
                                Kind = ReferenceType.CommandExpresion,
                                RefName = command.Name
                            });
                        }
                    });
                }*/
            }
        }
        int? GetReferenceID(OPCUAEntityReference tag, string key, string name)
        {
            if (IsReferenceValid(tag))
            {
                SVGHelper transferSVGHelper = GetTagHelper(tag, null, null);
                if (transferSVGHelper != null)
                {
                    entityMapList.Add(new OPCUAEntityReferenceMap()
                    {
                        SVGReferenceId = transferSVGHelper.SVGReferenceId,
                        ScreenEntityName = name,
                        Kind = ReferenceType.CommandTags,
                        RefName = key
                    });
                    return transferSVGHelper.SVGReferenceId;
                }
            }
            return null;
        }
        void AddAnimationReference(string item, 
                                AnimationManager.AnimationManager animation,
                                Dictionary<string, List<string>> expressionVariables,
                                Dictionary<string, OPCUAEntityReference> resolvedVariables,
                                OPCUAEntityReference entityreference = null,
                                ScreenEntity sentity = null)
        {
            if (animation != null)
            {
                animation.SVGItemId = item;
                SVGHelper SVGHelper = GetAnimationTagHelper(animation, entityreference, sentity);
                if (SVGHelper != null)
                {
                    animation.SVGReferenceId = SVGHelper.SVGReferenceId;
                    entityMapList.Add(new OPCUAEntityReferenceMap()
                    {
                        SVGReferenceId = SVGHelper.SVGReferenceId,
                        ScreenEntityName = item,
                        Kind = ReferenceType.Animation,
                        RefName = animation.Name
                    });
                }/*
                if (animation["Expression"] && !string.IsNullOrEmpty(animation.Expression))
                {
                    PopulateExpressionVariables(animation.Expression, expressionVariables);
                    expressionVariables[animation.Expression].ForEach(variable =>
                    {
                        SVGHelper expSVGHelper = GetTagHelper(null, GetResolvedReference(variable, resolvedVariables));
                        if (expSVGHelper != null)
                        {
                            entityMapList.Add(new OPCUAEntityReferenceMap()
                            {
                                SVGReferenceId = expSVGHelper.SVGReferenceId,
                                ScreenEntityName = item,
                                Kind = ReferenceType.AnimationExpression,
                                RefName = animation.Name
                            });
                        }
                    });
                }*/
            }
        }

        private void UpdateAnimatedEntityClassID()
        {
            List<string> elements = styleElement?.InnerText.Split('.').ToList();
            List<string> list = (List<string>)(from e in entityMapList where e.Kind == ReferenceType.Animation select e.ScreenEntityName).ToList();
            UpdateAnimatedEntityClassID(list, rootElement, elements);
        }

        private void UpdateAnimatedEntityClassID(List<string> list, XmlElement rootElement, List<string> elements)
        {

            foreach (XmlNode child in rootElement.ChildNodes)
            {
                if (child is XmlElement)
                {
                    XmlElement element = (child as XmlElement);
                    if(element.HasAttributes)
                    {
                        var attribute = element.GetAttributeNode("id");
                        if (attribute != null && list.Contains(attribute.Value))
                        {
                            if(element.Name != foreignObject)
                                ReplaceClassAttribute(element, elements);
                        }
                        else
                            UpdateAnimatedEntityClassID(list, element, elements);
                    }
                    else
                        UpdateAnimatedEntityClassID(list, element, elements);
                }
            }
        }

        private void ReplaceClassAttribute(XmlElement child, List<string> elements)
        {
            if(child.HasAttributes && elements != null && elements.Count > 0)
            {
                var attribute = child.GetAttributeNode("class");
                if (attribute != null)
                {
                    try
                    {
                        string fillUrl = (from e in elements where e.StartsWith($"{attribute.Value}{{") select e).FirstOrDefault();
                        if (string.IsNullOrEmpty(fillUrl))
                            return;
                        fillUrl = fillUrl.Substring(($"{attribute.Value}{{").Length);
                        fillUrl = fillUrl.Substring(0, fillUrl.IndexOf('}'));
                        child.RemoveAttribute("class");
                        List<string> styles = fillUrl.Split(';').ToList();
                        styles.ForEach(s =>
                        {
                            string[] style = s.Split(':');
                            if(style.Count() == 2)
                                child.Attributes.Append(JSONHelper.CreateNewAttribute($"{style[0]}", $"{style[1]}", xmlDoc));
                        });
                    }
                    catch
                    {
                    }
                }
            }
        }

        OPCUAEntityReference GetResolvedReference(string variable, Dictionary<string, OPCUAEntityReference> resolvedVariables)
        {
            OPCUAEntityReference entityReference = null;
            if (resolvedVariables.ContainsKey(variable))
                entityReference = resolvedVariables[variable];
            else
            {
                var sanitizedTagName = NamespaceTableConverter.GetSanitizedReadableValue(variable);
                string instance;
                string name;

                WPFUtilities.SmartControlHelper.GetInstanceName(sanitizedTagName, out instance, out name);
                IUFUAEditorManager uFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                var xml = uFUAEditor.GetTagEntityReference(Document, name, instance);
                if (!String.IsNullOrEmpty(xml))
                    entityReference = xml.FromXml<OPCUAEntityReference>();
            }
            return entityReference;
        }

        void CreateResourceFiles(string folderPath)
        {
            if (entityMapList == null)
                entityMapList = new OPCUAEntityReferenceMapList();
            entityMapList.Clear();
            if (sVGHelperList == null)
                sVGHelperList = new SVGHelperList();
            sVGHelperList.Clear();

            Dictionary<string, OPCUAEntityReference> resolvedVariables = new Dictionary<string, OPCUAEntityReference>();
            Dictionary<string, List<string>> expressionVariables = new Dictionary<string, List<string>>();
            AnimationManagerList animationList = new AnimationManagerList();
            CommandManagerList commandList = new CommandManagerList();
            entities entitiesList = new entities();

            Dictionary<string, Dictionary<string, List<string>>> expressionAnimationCommandVariables = new Dictionary<string, Dictionary<string, List<string>>>();
            //create entityreferencelist and entitypersistence
            List<string> resolveditems = new List<string>();
            List<string> canvasItems = new List<string>();
            var root = xmlDoc.SelectSingleNode("svg");
            var res = root.SelectNodes("//*[@id]");
            foreach (XmlNode node in res)
            {
                var nodeValue = node.Attributes.GetNamedItem("id").Value;
                if (!canvasItems.Contains(nodeValue))
                    canvasItems.Add(nodeValue);
            }

            foreach (var key in Document.MapScreenEntities.Keys)
            {
                var sentity = Document.MapScreenEntities[key];
                   var fe = sentity.Element as FrameworkElement;
                if (fe == null)
                    fe = Document.FindInnerControl(ActiveLayer, key);
                
                sentity.ReplaceAllAliasInOnce();
                
                if (!Document.IsInnerEntity(key) || sentity.SourceSymbolLinked)
                    Document.ResolveRelativeItems(fe, key, ActiveLayer);

                sentity.RefreshItemSource(true);
            }

            var listAlarm = new List<FrameworkElement>();
            var listConnectionString = new List<FrameworkElement>();
            var listEventConnectionString = new List<FrameworkElement>();
            var listScheduler = new List<FrameworkElement>();

            ScreenDocument.CheckPreBinding(listAlarm,
                    listConnectionString,
                    listEventConnectionString,
                    listScheduler,
                    ActiveLayer);
            if (listAlarm.Count > 0)
            {
                IUFUAEditorManager ufuaEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                var server = ufuaEditor.GetServerEntityReference(Document);
                if (server != null)
                    try
                    {
                        Document.PreBindAlarmSource(listAlarm, server);
                    }
                    catch (Exception ex)
                    {
                        Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                        string msg = string.Format(Properties.Resources.MsgWithException,
                            string.Format(Properties.Resources.ErrorBindingAlarms, Document.Title),
                            e.Message);
                        logDeploy.Error(msg);
                        errorMessages = true;
                    }
            }

            if (listScheduler.Count > 0)
            {
                ISchedulerEditorManager schedulerEditor = Document.GetService(typeof(ISchedulerEditorManager)) as ISchedulerEditorManager;
                var server = schedulerEditor.GetServerEntityReference(Document);
                if (server != null)
                    try
                    {
                        Document.PreBindSchedulerSource(listScheduler, server);
                    }
                    catch(Exception ex)
                    {
                        Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                        string msg = string.Format(Properties.Resources.MsgWithException,
                            string.Format(Properties.Resources.ErrorBindingSchedulers, Document.Title),
                            e.Message);
                        logDeploy.Error(msg);
                        errorMessages = true;
                    }
            }

            foreach (var item in Document.MapScreenEntities.Keys)
            {
                string clearedName = Document.CleanInnerName(item, innerEntitySVGNameFormat);
                if (canvasItems.Contains(clearedName))
                    try
                    {
                        AddEntityReferences(item, entitiesList, animationList, commandList,
                                                            expressionVariables, resolvedVariables, resolveditems);
                    }
                    catch(Exception ex)
                    {
                        Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                        string msg = string.Format(Properties.Resources.MsgWithException,
                            string.Format(Properties.Resources.ErrorAddingReferences, Document.Title),
                            e.Message);
                        logDeploy.Error(msg);
                        errorMessages = true;
                    }
            }
            
            var fileName = $"{Document.Title}{Properties.Settings.Default.EntityReferenceExportExtension}";
            var filePath = System.IO.Path.Combine(folderPath, fileName);

            if(sVGHelperList.Count > 0)
                //export opcuaentityreferene list
                Utilities.XmlHelper.ExportToFile<SVGHelperList>(sVGHelperList, filePath, Document.Title);
            else if (File.Exists(filePath))
                File.Delete(filePath);

            //export command list to json
            filePath = System.IO.Path.ChangeExtension(filePath, Properties.Settings.Default.CommandToJson);
            if (commandList.Count > 0)
            {
                try
                {
                    commandList.ForEach(command =>
                    {
                        command.OpcuaEntityReference = null;
                        if (command is ValueCommand)
                        {
                            (command as ValueCommand).TransferToTag = null;
                            (command as ValueCommand).TagMinValue = null;
                            (command as ValueCommand).TagMaxValue = null;
                        }
                        else if (command is ReportCommand)
                        {
                            (command as ReportCommand).Parameters?.ForEach(p =>
                            {
                                p.TagRef = null;
                            });
                        }
                    });
                    String ret = JSONHelper.IndentJSon(commandList.ToJSON());
                    File.WriteAllText(filePath, ret.Replace(":#CommandManager", "").Replace(":http://progea.com", ""));
                }
                catch (Exception ex)
                {
                    Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                    string msg = string.Format(Properties.Resources.MsgWithException,
                        string.Format(Properties.Resources.ErrorExportingCommands, Document.Title),
                        e.Message);
                    logDeploy.Error(msg);
                    errorMessages = true;
                }
            }
            else if (File.Exists(filePath))
                File.Delete(filePath);

            //export animation list to json
            filePath = System.IO.Path.ChangeExtension(filePath, Properties.Settings.Default.AnimationsToJson);
            if (animationList.Count > 0)
            {
                try
                {
                    animationList.ForEach(animation =>
                    {
                        animation.OpcuaEntityReference = null;
                        if (animation is BackColorAnimation)
                        {
                            (animation as BackColorAnimation).ListColors = (animation as BackColorAnimation).ListColors.OrderBy(x => x.Value).ToList();
                        }
                        else if (animation is BorderColorAnimation)
                        {
                            (animation as BorderColorAnimation).ListColors = (animation as BorderColorAnimation).ListColors.OrderBy(x => x.Value).ToList();
                        }
                    });
                    String ret = JSONHelper.ClearProperties(animationList.ToJSON());
                    File.WriteAllText(filePath, ret.Replace(":#AnimationManager", ""));
                }
                catch(Exception ex)
                {
                    Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                    string msg = string.Format(Properties.Resources.MsgWithException,
                        string.Format(Properties.Resources.ErrorExportingAnimations, Document.Title),
                        e.Message);
                    logDeploy.Error(msg);
                    errorMessages = true;
                }
            }
            else if (File.Exists(filePath))
                File.Delete(filePath);

            //export entities main properties to json
            filePath = System.IO.Path.ChangeExtension(filePath, Properties.Settings.Default.EnitiesToJson);
            if (entitiesList != null)
            {
                try
                {
                    String ret = JSONHelper.ToJSONArray(Parent, Document, entitiesList, null, null);
                    File.WriteAllText(filePath, ret);
                }
                catch(Exception ex)
                {
                    Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                    string msg = string.Format(Properties.Resources.MsgWithException,
                        string.Format(Properties.Resources.ErrorExportingEntities, Document.Title),
                        e.Message);
                    logDeploy.Error(msg);
                    errorMessages = true;
                }
            }
            else if (File.Exists(filePath))
                File.Delete(filePath);

            //export foreignObjectList to json
            filePath = System.IO.Path.ChangeExtension(filePath, Properties.Settings.Default.ControlsToJson);
            if (foreignObjectList != null)
            {
                try
                {
                    var elementList = (from fo in foreignObjectList 
                     where fo.SVGItem != null && fo.SVGItem is UIElement 
                     select fo.SVGItem as UIElement).ToList();
                    Document.SetImagesBaseUri(elementList);
                    String ret = JSONHelper.ToJSONArray(Parent, Document, foreignObjectList, referenceDefsXMLStyleContentMap, referenceDefsXMLStyleNameMap);
                    File.WriteAllText(filePath, ret);
                }
                catch(Exception ex)
                {
                    Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                    string msg = string.Format(Properties.Resources.MsgWithException,
                        string.Format(Properties.Resources.ErrorExportingControls, Document.Title),
                        e.Message);
                    logDeploy.Error(msg);
                    errorMessages = true;
                }
            }
            else if (File.Exists(filePath))
                File.Delete(filePath);

            try
            {
                UpdateAnimatedEntityClassID();
            }
            catch(Exception ex)
            {
                Exception e = (ex.InnerException != null ? ex.InnerException : ex);
                string msg = string.Format(Properties.Resources.MsgWithException,
                    string.Format(Properties.Resources.ErrorUpdatingAnimatedClassIDs, Document.Title),
                    e.Message);
                logDeploy.Error(msg);
                errorMessages = true;
            }
        }

        private void AddEntityReferences(string item, entities entitiesList,
            AnimationManagerList animationList, CommandManagerList commandList, Dictionary<string, List<string>> expressionVariables, 
            Dictionary<string, OPCUAEntityReference> resolvedVariables, List<string> resolveditems)
        {

            if (resolveditems.Contains(item))
                return;

            string clearedName = Document.CleanInnerName(item, innerEntitySVGNameFormat);
            var sentity = Document.MapScreenEntities[item];
            var fe = sentity.Element as FrameworkElement;
            if (fe == null)
                fe = Document.FindInnerControl(ActiveLayer, item);

            OPCUAEntityReference entityReference = GetParentTag(item, sentity.OpcuaEntityReference, entitiesList,
            animationList, commandList, expressionVariables, resolvedVariables, resolveditems);

            bool isHitTestVisible = fe.IsHitTestVisible;
            if (sentity.SourceSymbolLinked && fe is ContentControl && !(fe is UserControl) && (fe as ContentControl).Content is FrameworkElement)
                isHitTestVisible = ((fe as ContentControl).Content as FrameworkElement).IsHitTestVisible;

            var fList = sentity.FontSettingList?.ToDictionary(el => el.Key, el => el.Value.ToDictionary());

            jsonobject fobj = new jsonobject()
            {
                SVGItemId = clearedName,
                parameters = new Dictionary<string, object>() { { "AccessLevelFromTag", sentity.AccessLevelFromTag },
                                                                    { "AccessLevel", sentity.AccessLevel },
                                                                    { "AccessRole", sentity.AccessRole },
                                                                    { "ReadableAccessMask", sentity.ReadableAccessMask },
                                                                    { "WritableAccessMask", sentity.WritableAccessMask },
                                                                    { "ZoomLevelVisibilityX", sentity.ZoomLevelVisibilityX },
                                                                    { "ZoomLevelVisibilityY", sentity.ZoomLevelVisibilityY },
                                                                    { "EnableManipulation", sentity.EnableManipulation },
                                                                    { "FontSettingList", fList },
                                                                    { "IsHitTestVisible", isHitTestVisible }}
            };
            entitiesList.Add(fobj);

            int? entityrefID;
            AddReference(sentity, fe, clearedName, expressionVariables, resolvedVariables,out entityrefID, entityReference);
            //update context reference for foreignobjects
            UpdateContextReference(clearedName, entityrefID);
            //get animation references
            var listAnimation = sentity.AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
                AddAnimationReference(clearedName, animation, expressionVariables, resolvedVariables, entityReference, sentity);
            animationList.AddRange(listAnimation);
            //get command references
            var listCommand = sentity.CommandList as CommandManagerList;
            foreach (var command in listCommand)
                AddCommandReference(clearedName, command, expressionVariables, resolvedVariables, entityReference, sentity);
            commandList.AddRange(listCommand);

            resolveditems.Add(item);
        }

        private void UpdateContextReference(string item, int? svgKeyCodeNumber)
        {
            var foreign = (from f in foreignObjectList where f.SVGItemId == item select f).FirstOrDefault();
            if(foreign != null)
            {
                if (foreign.parameters == null)
                    foreign.parameters = new Dictionary<string, object>();
                if (foreign.parameters.ContainsKey(JSONHelper.sVGReferenceId))
                    foreign.parameters[JSONHelper.sVGReferenceId] = svgKeyCodeNumber;
                else
                    foreign.parameters.Add(JSONHelper.sVGReferenceId, svgKeyCodeNumber);
            }
        }

        private OPCUAEntityReference GetParentTag(string name, OPCUAEntityReference entityreference, entities entitiesList,
            AnimationManagerList animationList, CommandManagerList commandList, Dictionary<string, List<string>> expressionVariables,
            Dictionary<string, OPCUAEntityReference> resolvedVariables, List<string> resolveditems)
        {
            if (IsReferenceValid(entityreference))
                return entityreference;
            if (string.IsNullOrEmpty(name))
                return null;
            if (Document.IsInnerEntity(name))
            {
                OPCUAEntityReference newReference;
                var names = name?.Split(ScreenDocument.innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                if (names != null && names.Count > 1)
                {
                    string parent = name;
                    while (names.Count > 0)
                    {
                        names.RemoveAt(names.Count - 1);
                        parent = String.Join(ScreenDocument.innerEntityNameFormat, names);
                        if (Document.MapScreenEntities.ContainsKey(parent) && IsReferenceValid(Document.MapScreenEntities[parent].OpcuaEntityReference))
                        {
                            AddEntityReferences(parent, entitiesList, animationList, commandList,
                                                       expressionVariables, resolvedVariables, resolveditems);
                            newReference = Document.MapScreenEntities[parent].OpcuaEntityReference;

                            return newReference;
                        }
                    }
                }
                else
                    return null;
            }

            return null;
        }
        private ScreenSettings.Entities.ScreenEntity GetParentEntity(string name)
        {
            if (Document.IsInnerEntity(name))
            {
                var names = name?.Split(ScreenDocument.innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                if (names != null && names.Count > 1)
                {
                    string parent = name;
                    while (names.Count > 0)
                    {
                        parent = names[0];
                        names.RemoveAt(0);
                        if (Document.MapScreenEntities.ContainsKey(parent))
                            return Document.MapScreenEntities[parent];
                    }
                }
            }
            else
                return null;

            return null;
        }

        private bool IsReferenceValid(OPCUAEntityReference entityreference)
        {
            return (entityreference != null && (entityreference.IsValid /*|| AliasHelper.ContainsAlias(entityreference.RelativePath)*/));
        }
        
        private void GetStringValue<T>(string name, JToken jtoken)
        {
            try
            {
                T newvalue = JsonConvert.DeserializeObject<T>(jtoken[name].ToString());
                jtoken[name] = new JProperty(name, newvalue.ToString());
            }
            catch (Exception ex)
            {
            }
        }

        void PopulateExpressionVariables(string expr, Dictionary<string, List<string>> expressionVariables)
        {
            if (expressionVariables == null)
                expressionVariables = new Dictionary<string, List<string>>();
            if (expressionVariables.ContainsKey(expr))
                return;
            else
                expressionVariables.Add(expr, new List<string>());

            if (expr != null)
            {
                var expConv = new Utilities.Converters.ExpressionValueConverter(expr);
                expConv.ReverseFormula = string.Empty;
                expConv.ParseFormula();
                foreach (var tag in expConv.GetAllParsedVariables())
                {
                    expressionVariables[expr].Add(tag);
                }
            }
        }
#endif

        #endregion

        #region dispose
        public void Dispose()
        {
            styleMap.Clear();
            entityList.Clear();
            styleXMLMap.Clear();
            referenceDefsXMLNameMap.Clear();
            referenceDefsXMLContentMap.Clear();
            entityMapList.Clear();
            iOsEntityList.Clear();
#if !NET_STANDARD
            overlappedOnCanvas.Clear();
            innercontrolList.Clear();
            foreignObjectList.Clear();
            referenceDefsXMLStyleContentMap.Clear();
            referenceDefsXMLStyleNameMap.Clear();
            referenceBrushListMap.Clear();
            referenceClipDefMap.Clear();
            referenceClipStyleMap.Clear();
            ActiveLayer.Dispose();
            sVGHelpermap.Clear();
            sVGHelperList.Clear();
#endif
            rootElement = null;
            defsElement = null;
            styleElement = null;
            iOsEntityList = null;
            xmlDoc = null;
            iOSxmlDoc = null;
        }
        #endregion
    }

    [DataContract(Name = "foreignObject", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class jsonobject
    {
        [DataMember]
        public string SVGItemId { get; set; }
        [DataMember]
        public object SVGItem { get; set; }
        [DataMember]
        public Dictionary<string, object> parameters { get; set; }
        [DataMember]
        public Dictionary<string, object> commandTagList { get; set; }
        [DataMember]
        public Dictionary<string, string> brushes { get; set; }
        [DataMember]
        public Dictionary<string, Dictionary<string, object>> urlbrushes { get; set; }
#if !NET_STANDARD
        [DataMember]
        public Dictionary<string, Brush> overriddenBrushes { get; set; }
#endif
    }

#if !NET_STANDARD
    [CollectionDataContract(Name = "entities", ItemName = "foreignobject", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class entities : List<jsonobject>
    {
        #region Constructors
        public entities(List<jsonobject> collection)
            : base(collection)
        {
        }

        public entities()
            : base()
        {
        }
        #endregion
    }
    public static class ColorConverterExtensions
    {
        public static string ToHexString(this System.Windows.Media.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        public static string ToRgbString(this System.Windows.Media.Color c) => $"rgba({c.R}, {c.G}, {c.B}, {JSONHelper.GetValue((double)c.A/255,true)})";
    }
    public static class JSONHelper
    {
        static string images = $"{SpecialFolders.Images.ToString().First<char>().ToString().ToLower()}{SpecialFolders.Images.ToString().Substring(1)}";
        public static List<JToken> FindTokens(this JToken containerToken, string name)
        {
            List<JToken> matches = new List<JToken>();
            FindTokens(containerToken, name, matches);
            return matches;
        }

        private static void FindTokens(JToken containerToken, string name, List<JToken> matches)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    if (child.Name == name)
                    {
                        matches.Add(child.Value);
                    }
                    FindTokens(child.Value, name, matches);
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, name, matches);
                }
            }
        }

        public static string IndentJSon(string jsonvalue, bool usArray = true)
        {
            if(usArray)
            {
                JArray json = JArray.Parse(jsonvalue);
                return json.ToString();
            }
            else
            {
                JObject json = JObject.Parse(jsonvalue);
                return json.ToString();
            }
        }

        public static string ClearProperties(string jsonvalue, Dictionary<string,object> parameters = null)
        {
            try
            {
                JArray json = JArray.Parse(jsonvalue);
                foreach (JObject parsedObject in json.Children<JObject>())
                {
                    List<JProperty> propList = new List<JProperty>();
                    parsedObject.Remove("ID");
                    parsedObject.Remove("ColuName");
                    parsedObject.Remove("PointSettings");
                    parsedObject.Remove("TagReference");
                    parsedObject.Remove("TagReferenceXml");
                    List<JProperty> newPropertiesToAdd = new List<JProperty>();
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        string propertyName = parsedProperty.Name;
                        if (propertyName.Equals("listColors"))
                        {
                            if (parsedProperty.HasValues)
                                foreach (JObject parsedColors in parsedProperty.Value.Children<JObject>())
                                {
                                    List<JProperty> propertiesToAdd = new List<JProperty>();
                                    foreach (JProperty parsedColorsProperty in parsedColors.Properties())
                                    {
                                        string parsedColorsPropertyName = parsedColorsProperty.Name;
                                        if ((parsedColorsPropertyName.Equals("colorBlink") || parsedColorsPropertyName.Equals("color")))
                                        {
                                            if(parsedColorsProperty.HasValues)
                                            {
                                                byte r = 0;
                                                byte g = 0;
                                                byte b = 0;
                                                var propertyValue = parsedColorsProperty.Value;
                                                foreach (JProperty property in (propertyValue as JObject).Properties())
                                                {
                                                    if (property.Name == "ScA")
                                                        //a = (byte)property.Value;
                                                        propertiesToAdd.Add(new JProperty($"{parsedColorsPropertyName}Opacity", property.Value));
                                                    if (property.Name == "R")
                                                        r = (byte)property.Value;
                                                    if (property.Name == "G")
                                                        g = (byte)property.Value;
                                                    if (property.Name == "B")
                                                        b = (byte)property.Value;
                                                }

                                                parsedColorsProperty.Value = $"#{r.ToString("X2")}{g.ToString("X2")}{b.ToString("X2")}";
                                            }
                                            else
                                                propertiesToAdd.Add(new JProperty($"{parsedColorsPropertyName}Opacity", 0));
                                        }
                                    }
                                    propertiesToAdd.ForEach(p => parsedColors.Add(p));
                                }
                        }
                        else if(propertyName.Equals("LColor") || propertyName.Equals("StartColor") || propertyName.Equals("EndColor"))
                        {
                            if (parsedProperty.HasValues)
                            {
                                string newValue = parsedProperty.Value.ToString();
                                byte a = 255;
                                byte r = 0;
                                byte g = 0;
                                byte b = 0;
                                foreach (JProperty parsedColorsProperty in parsedProperty.Value)
                                {
                                    if (parsedColorsProperty.Name == "A")
                                        a = (byte)parsedColorsProperty.Value;
                                    if (parsedColorsProperty.Name == "R")
                                        r = (byte)parsedColorsProperty.Value;
                                    if (parsedColorsProperty.Name == "G")
                                        g = (byte)parsedColorsProperty.Value;
                                    if (parsedColorsProperty.Name == "B")
                                        b = (byte)parsedColorsProperty.Value;
                                    if (parsedColorsProperty.Name == "ScA")
                                        newPropertiesToAdd.Add(new JProperty($"{propertyName}Opacity", parsedColorsProperty.Value));
                                }
                                newValue = $"#{r.ToString("X2")}{g.ToString("X2")}{b.ToString("X2")}";
                                parsedProperty.Value = newValue;
                            }
                            else
                                newPropertiesToAdd.Add(new JProperty($"{propertyName}Opacity", 0));
                        }
                        else if (propertyName.Equals("NodeId") && parameters != null)
                        {
                            if (parsedProperty.HasValues)
                            {
                                if (parameters.ContainsKey(parsedProperty.Value.ToString()))
                                    propList.Add(new JProperty(sVGReferenceId, parameters[parsedProperty.Value.ToString()]));
                            }
                        }

                    }
                    newPropertiesToAdd.ForEach(p => parsedObject.Add(p));
                    parsedObject.Remove("NodeId");
                    foreach (JProperty p in propList)
                    {
                        parsedObject.Add(p);
                    }
                }
                return json.ToString();
            }
            catch (Exception)
            {
            }
            return jsonvalue;
        }
        public static string sVGReferenceId = Properties.Settings.Default.SVGReferenceId;
        public static string styleON = Properties.Settings.Default.StyleON;
        public static string styleOFF = Properties.Settings.Default.StyleOFF;
        public static string styleNULL = Properties.Settings.Default.StyleNULL;
        public static string stylePartValue = Properties.Settings.Default.StylePartValue;
        public static string stylePartEUnit = Properties.Settings.Default.StylePartEUnit;
        public static string stylePartIndicator = Properties.Settings.Default.StylePartIndicator;
        public static string styleLinked = Properties.Settings.Default.SourceSymbolLinked;
        public static string styleGuid = Properties.Settings.Default.GUIDStyle;
        public static string svgBackground = Properties.Settings.Default.SvgBackground;
        public static string isSolidColorBorder = Properties.Settings.Default.IsSolidColorBorder;
        public static string progressBarOrientation = Properties.Settings.Default.ProgressBarOrientation;
        public static string progressBarAnimation = Properties.Settings.Default.ProgressBarAnimation;
        static Dictionary<Type, List<PropertyInfo>> propertyCache = new Dictionary<Type, List<PropertyInfo>>();
        static Dictionary<Type, List<DependencyProperty>> dependencyPropertyCache = new Dictionary<Type, List<DependencyProperty>>();
        public static void ClearPropertyCache()
        {
            propertyCache.Clear();
            dependencyPropertyCache.Clear();
        }
        public static string ToJSONArray(IDocument parent, IDocument document, entities entitiesList, Dictionary<string, string> referenceDefsXMLStyleContentMap, Dictionary<string, string> referenceDefsXMLStyleNameMap)
        {
            JArray rss = new JArray();
            entitiesList.ForEach(p =>
            {
                bool addElement = false;
                JObject element = new JObject(new JProperty("SVGItemId", p.SVGItemId));
                if (p.SVGItem != null)
                {
                    FrameworkElement fe = p.SVGItem as FrameworkElement;
                    fe.ApplyTemplate();

                    Type type = p.SVGItem.GetType();
                    if (p.parameters != null)
                    {
                        if (p.parameters.ContainsKey(sVGReferenceId))
                        {
                            AddElement(element, sVGReferenceId, p.parameters[sVGReferenceId]);
                            p.parameters.Remove(sVGReferenceId);
                        }
                        if (p.parameters.ContainsKey(styleON))
                        {
                            AddElement(element, styleON, p.parameters[styleON]);
                            p.parameters.Remove(styleON);
                        }
                        if (p.parameters.ContainsKey(styleOFF))
                        {
                            AddElement(element, styleOFF, p.parameters[styleOFF]);
                            p.parameters.Remove(styleOFF);
                        }
                        if (p.parameters.ContainsKey(styleNULL))
                        {
                            AddElement(element, styleNULL, p.parameters[styleNULL]);
                            p.parameters.Remove(styleNULL);
                        }
                        if (p.parameters.ContainsKey(styleGuid))
                        {
                            AddElement(element, styleGuid, p.parameters[styleGuid]);
                            p.parameters.Remove(styleGuid);
                        }
                        if (p.parameters.ContainsKey(isSolidColorBorder))
                        {
                            AddElement(element, isSolidColorBorder, p.parameters[isSolidColorBorder]);
                            p.parameters.Remove(isSolidColorBorder);
                        }
                        if (p.parameters.ContainsKey(styleLinked))
                        {
                            AddElement(element, styleLinked, p.parameters[styleLinked]);
                            p.parameters.Remove(styleLinked);
                        }
                        if (p.parameters.ContainsKey(svgBackground))
                        {
                            if(!p.urlbrushes.ContainsKey(svgBackground))
                            {
                                var value = p.parameters[svgBackground];
                                try
                                {
#if !NET_STANDARD
                                    if (!AddKnownElement(parent, document, element, svgBackground, value, null, fe))
#endif
                                        AddElement(element, svgBackground, value);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        element.Add(new JProperty(svgBackground, value));
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            p.parameters.Remove(svgBackground);
                        }
                    }

                    var svgItemStyles = JSONHelper.GetStyles(fe, propertyCache);

                    Dictionary<string, string> newStyleRefIDs = new Dictionary<string, string>();
                    svgItemStyles?.Keys.ToList().ForEach(k =>
                    {
                        string typeName = type.Name;
                        var customObjectSvgAttribute = (Utilities.SvgValueConverterAttribute)type.GetCustomAttributes(typeof(Utilities.SvgValueConverterAttribute), true).FirstOrDefault() as Utilities.SvgValueConverterAttribute;
                        if (customObjectSvgAttribute != null && !string.IsNullOrEmpty(customObjectSvgAttribute.StyleTypeName))
                            typeName = customObjectSvgAttribute.StyleTypeName;

                        string newKey = $"_{typeName}_{svgItemStyles[k]}";
                        if (!referenceDefsXMLStyleNameMap.ContainsKey(newKey))
                        {
                            var styleName = newKey;

                            int i = 0;
                            while (referenceDefsXMLStyleNameMap.Values.Contains(styleName))
                            {
                                styleName = styleName + ++i;
                            }

                            referenceDefsXMLStyleNameMap.Add(newKey, styleName);
                        }

                        if (!referenceDefsXMLStyleContentMap.ContainsKey(newKey))
                        {
                            string rootFolder = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                            string svgStartingPath = $"{rootFolder}\\{Properties.Settings.Default.SVGControlStylesCommonFolder}\\{typeName}\\";
                            string sourcepath = $"{System.IO.Path.Combine(svgStartingPath, k)}{Properties.Settings.Default.WebExportExtension}";

                            if (System.IO.File.Exists(sourcepath))
                            {
                                string content = File.ReadAllText(sourcepath);
                                try
                                {
                                    content = WPFUtilities.CryptString.CryptString.DecryptString(content);
                                }
                                catch (Exception ex) { }
                                //content = SetStyleIds(content, referenceDefsXMLStyleNameMap[newKey]);
                                XmlDocument doc = new XmlDocument();
                                var fragment = doc.CreateDocumentFragment();
                                fragment.InnerXml = XDocument.Parse(content).ToString();
                                XmlElement child = (from XmlNode c in fragment.ChildNodes[0].ChildNodes
                                                    where c is XmlElement && (c as XmlElement).Name == "svg" && (c as XmlElement).Attributes?.GetNamedItem("id")?.Value == svgItemStyles[k]
                                                    select (c as XmlElement)).FirstOrDefault();
                                if (child != null)
                                {
                                    child.InnerXml = SetStyleIds(child.InnerXml, referenceDefsXMLStyleNameMap[newKey]);

                                    child.Attributes.Append(JSONHelper.CreateNewAttribute("x", $"0px", doc));
                                    child.Attributes.Append(JSONHelper.CreateNewAttribute("y", $"0px", doc));
                                    child.Attributes.Append(JSONHelper.CreateNewAttribute("width", $"100%", doc));
                                    child.Attributes.Append(JSONHelper.CreateNewAttribute("height", $"100%", doc));

                                    if (string.IsNullOrEmpty(child.GetAttribute("preserveAspectRatio")))
                                        child.Attributes.Append(JSONHelper.CreateNewAttribute("preserveAspectRatio", $"none", doc));
                                    child.Attributes.Append(JSONHelper.CreateNewAttribute("id", referenceDefsXMLStyleNameMap[newKey], doc));

                                    referenceDefsXMLStyleContentMap.Add(newKey, JSONHelper.ManageClassIDs(child, newKey));
                                }
                                else
                                    referenceDefsXMLStyleContentMap.Add(newKey, null);
                            }
                            else
                                referenceDefsXMLStyleContentMap.Add(newKey, null);
                        }

                        newStyleRefIDs.Add(svgItemStyles[k], referenceDefsXMLStyleNameMap[newKey]);
                    });

                    var parameters = JSONHelper.GetParameters(type, fe, propertyCache, document);
                    parameters?.Keys.ToList().ForEach(k =>
                    {
                        object value = parameters[k];
                        if (p.urlbrushes != null && p.urlbrushes.ContainsKey(k))
                            value = p.urlbrushes[k]; 
                        else if (p.brushes != null && p.brushes.ContainsKey(k))
                            value = p.brushes[k];
#if !NET_STANDARD
                        else if (p.overriddenBrushes != null && p.overriddenBrushes.ContainsKey(k))
                            value = p.overriddenBrushes[k];
#endif
                        else if (value != null)
                            try
                            {
                                if (newStyleRefIDs.ContainsKey(value.ToString()))
                                    value = newStyleRefIDs[value.ToString()];
                            }
                            catch (Exception ex)
                            {
                            }
                        try
                        {
#if !NET_STANDARD
                            if (!AddKnownElement(parent, document, element, k, value, p.parameters, fe))
#endif
                                AddElement(element, k, value);
                            else if (p.parameters.ContainsKey(k))
                                p.parameters.Remove(k);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                element.Add(new JProperty(k, value));
                            }
                            catch
                            {
                            }
                        }
                        addElement = true;
                    });
                    p.parameters.Keys.ToList().ForEach(k =>
                    {
                        object value = p.parameters[k];
                        AddElement(element, k, value);
                        addElement = true;
                    });
                }
                else if (p.parameters != null)
                {
                    p.parameters.Keys.ToList().ForEach(k =>
                    {
                        object value = p.parameters[k];
                        try
                        {
#if !NET_STANDARD
                            if (!AddKnownElement(parent, document, element, k, value, null, null))
#endif
                                AddElement(element, k, value);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                element.Add(new JProperty(k, value));
                            }
                            catch
                            {
                            }
                        }
                        addElement = true;
                    });
                }
                
                if(addElement)
                    rss.Add(element);
            });

            var json = rss.ToString();
            return json;
        }

        public static string SetStyleIds(string content, string newValue)
        {
            if (string.IsNullOrEmpty(content))
                return content;
            //render distinct style names    
            //content = content.Replace("class=\"st", $"class=\"{newValue}st");

            content = content.Replace(".st", $".{newValue}st");
            //render distinct brush names    
            content = content.Replace("url(#", $"url(#{newValue}");
            content = content.Replace("id=\"", $"id=\"{newValue}");
            content = content.Replace($"xlink:href=\"#", $"xlink:href=\"#{newValue}");
            content = content.Replace($"id=\"{newValue}{JSONHelper.stylePartEUnit}\"", $"id=\"{JSONHelper.stylePartEUnit}\"");
            content = content.Replace($"id=\"{newValue}{JSONHelper.stylePartValue}\"", $"id=\"{JSONHelper.stylePartValue}\"");
            content = content.Replace($"id=\"{newValue}{JSONHelper.stylePartIndicator}\"", $"id=\"{JSONHelper.stylePartIndicator}\"");
            //render distinct background names  
            content = content.Replace("(#background_", $"(#{newValue}background_");
            content = content.Replace("id=\"background_", $"id=\"{newValue}background_");
            return content;
        }

        private static void AddElement(JContainer element, string parameter, object value)
        {
            try
            {
                if (value == null)
                    element.Add(new JProperty(parameter, value));
                else
                {
                    if (value is string)
                    {
                        if ((value as string).Contains("\\{"))
                            value = (value as string).Replace("\\{", "{");
                        if ((value as string).Contains("\\}"))
                            value = (value as string).Replace("\\}", "}");
                    } 

                    string svalue = value.ToJSON();
                    try
                    {
                        JToken jsonvalue = JToken.Parse(svalue);
                        element.Add(new JProperty(parameter, jsonvalue));
                    }
                    catch
                    {
                        element.Add(new JProperty(parameter, svalue));
                    }
                }
            }
            catch
            {
                try
                {
                    element.Add(new JProperty(parameter, value));
                }
                catch
                {
                }
            }
        }

        private static void AddElement(JContainer element, object value)
        {
            try
            {
                if (value == null)
                    return;
                else
                {
                    string svalue = value.ToJSON();
                    try
                    {
                        JToken jsonvalue = JToken.Parse(svalue);
                        element.Add(jsonvalue);
                    }
                    catch
                    {
                        element.Add(value);
                    }
                }
            }
            catch
            {
            }
        }

        private static bool AddKnownElement(IDocument parent, IDocument document, JContainer element, string parameter, object value, Dictionary<string, object> knownvalues, FrameworkElement fe = null)
        {
            bool useKnownValues = knownvalues != null && knownvalues.Count > 0;
            if (value == null)
            {
                element.Add(new JProperty(parameter, value));
                return true;
            }
            else if (value is Dictionary<string, object>)
            {
                Dictionary<string, object> val = value as Dictionary<string, object>;
                JObject jsonvalue = new JObject();
                val.Keys.ToList().ForEach(v =>
                {
                    if (useKnownValues && val[v] is string && !string.IsNullOrEmpty(val[v] as string) && knownvalues.Keys.Contains(val[v] as string))
                        AddElement(jsonvalue, v, knownvalues[val[v] as string]);
                    else if(!AddKnownElement(parent, document, jsonvalue, v, val[v], knownvalues, fe))
                        AddElement(jsonvalue, v, val[v]);
                });
                element.Add(new JProperty(parameter, jsonvalue));
                return true;
            }
            else if (value is Dictionary<string, Dictionary<string, object>>)
            {
                Dictionary<string, Dictionary<string, object>> val = value as Dictionary<string, Dictionary<string, object>>;
                JObject jsonvalue = new JObject();
                val.Keys.ToList().ForEach(v =>
                {
                    if (!AddKnownElement(parent, document, jsonvalue, v, val[v], knownvalues, fe))
                        AddElement(jsonvalue, v, val[v]);
                });
                element.Add(new JProperty(parameter, jsonvalue));
                return true;
            }
            else if (value is List<object>)
            {
                List<object> val = value as List<object>;
                JArray jsonvalue = new JArray();
                val.ForEach(obj =>
                {
                    if (obj is Dictionary<string, object>)
                    {
                        Dictionary<string, object> objval = obj as Dictionary<string, object>;
                        JObject jsonobjvalue = new JObject();
                        objval.Keys.ToList().ForEach(keyvalue =>
                        {
                            if (useKnownValues && objval[keyvalue] is string && !string.IsNullOrEmpty(objval[keyvalue] as string) && knownvalues.Keys.Contains(objval[keyvalue] as string))
                                AddElement(jsonobjvalue, keyvalue, knownvalues[objval[keyvalue] as string]);
                            else if (!AddKnownElement(parent, document, jsonobjvalue, keyvalue, objval[keyvalue], knownvalues, fe))
                                AddElement(jsonobjvalue, keyvalue, objval[keyvalue]);
                        });
                        jsonvalue.Add(jsonobjvalue);
                    }
                    else if (!AddKnownElement(parent, document, jsonvalue, parameter, obj, knownvalues, fe))
                        AddElement(jsonvalue, parameter, obj);
                });
                element.Add(new JProperty(parameter, jsonvalue));
                return true;
            }
            else if (value is Color)
            {
                Color neval = (Color)value;
                element.Add(new JProperty(parameter, ColorConverterExtensions.ToRgbString(neval)));
                return true;
            }
            else if (value is Brush && fe != null)
            {
                element.Add(new JProperty(parameter, GetBrush(parent, document, value as Brush, fe)));
                return true;
            }
            else if (value is FontWeight || value is FontFamily || value is FontStyle)
            {
                element.Add(new JProperty(parameter, value.ToString()));
                return true;
            }
            else if (value is ReportParameters.ParameterCollection)
            {
                if (!useKnownValues)
                    return false;
                ReportParameters.ParameterCollection repParamaterCollection = value as ReportParameters.ParameterCollection;
                JArray jsonvalue = new JArray();

                repParamaterCollection.ToList().ForEach(repParamater =>
                {
                    if (knownvalues.Keys.Contains(repParamater.NodeId))
                    {
                        repParamater.SVGReferenceId = (int)knownvalues[repParamater.NodeId];
                        repParamater.TagRef = null;
                    }
                    AddElement(jsonvalue, repParamater);
                });

                element.Add(new JProperty(parameter, jsonvalue));
            }
            else
            {
                if (!useKnownValues)
                    return false;
                else
                {
                    if (knownvalues.Keys.Contains(parameter))
                    {
                        JObject jsonvalue = new JObject();
                        jsonvalue.Add(new JProperty(JSONHelper.sVGReferenceId, knownvalues[parameter]));
                        element.Add(new JProperty(parameter, jsonvalue));
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetValue(double v, bool bRound = false)
        {
            if (double.IsNaN(v))
                return null;
            if (bRound)
                v = Math.Round(v, Properties.Settings.Default.ExportDecimalNumders);
            return (v.ToString(CultureInfo.InvariantCulture));
        }

        private static JContainer GetBrush(IDocument parent, IDocument document, Brush brush, FrameworkElement fe)
        {
            if (brush == null)
                return null;

            JObject jsonobjvalue = new JObject();
            if (brush is SolidColorBrush)
            {
                SolidColorBrush color = (brush as SolidColorBrush);
                AddElement(jsonobjvalue, "Color", ColorConverterExtensions.ToRgbString(color.Color));
            }
            else if (brush is LinearGradientBrush)
            {
                LinearGradientBrush color = (brush as LinearGradientBrush);
                string linearColor = $"linear-gradient({GetDegree(color.StartPoint, color.EndPoint, new Size(fe.Width, fe.Height))}";
                if (color.GradientStops.Count > 0)
                {
                    var gradients = color.GradientStops.OrderBy(x => x.Offset).ToList();
                    for (int i = 0; i < gradients.Count; i++)
                    {
                        Color gcolor = gradients[i].Color;
                        linearColor = $"{linearColor}, {ColorConverterExtensions.ToRgbString(gcolor)} {Math.Round(gradients[i].Offset * 100)}%";
                    }
                }
                linearColor = $"{linearColor})";
                AddElement(jsonobjvalue, "Color", $"{linearColor}");
            }
            else if (brush is RadialGradientBrush)
            {
                RadialGradientBrush color = (brush as RadialGradientBrush);
                string linearColor = $"radial-gradient(ellipse {Math.Round(color.RadiusX * 100)}% {Math.Round(color.RadiusY * 100)}% at {Math.Round((color.GradientOrigin.X * 100))}% {Math.Round(color.GradientOrigin.Y * 100)}%";
                if (color.GradientStops.Count > 0)
                {
                    var gradients = color.GradientStops.OrderBy(x => x.Offset).ToList();
                    for (int i = 0; i < gradients.Count; i++)
                    {
                        Color gcolor = gradients[i].Color;
                        linearColor = $"{linearColor}, {ColorConverterExtensions.ToRgbString(gcolor)} {Math.Round(gradients[i].Offset * 100)}%";
                    }
                }
                linearColor = $"{linearColor})";
                AddElement(jsonobjvalue, "Color", $"{linearColor}");
            }
            else if (brush is VisualBrush)
            {
                VisualBrush color = (brush as VisualBrush);
                string source = WPFUtilities.ImageHelper.BrushToImagePath(document, brush);
                if(color.Visual is MediaElement)
                    AddElement(jsonobjvalue, "MediaSource", $"{source}");
                else
                {
                    AddElement(jsonobjvalue, "Opacity", $"{JSONHelper.GetValue(color.Opacity, true)}");
                    AddElement(jsonobjvalue, "Source", $"{source}");
                }
            }
            else if (brush is ImageBrush)
            {
                string source = WPFUtilities.ImageHelper.BrushToImagePath(document, brush);
                AddElement(jsonobjvalue, "Source", $"{source}");
            }
            return jsonobjvalue;
        }

        static string GetDegree(Point startPoint, Point endPoint, Size size)
        {
            return $"{GetValue(GetDoubleDegree(startPoint, endPoint, size))}deg";
        }

        public static double GetDoubleDegree(Point startPoint, Point endPoint, Size size)
        {
            var x2 = endPoint.X;
            var x1 = startPoint.X;
            var y2 = endPoint.Y;
            var y1 = startPoint.Y;
            if (Math.Round(x2, 1) == Math.Round(x1,1))
            {
                if(y1 >= y2)
                    return 0d;
                else
                    return 180d;
            }
            if (Math.Round(y2, 1) == Math.Round(y1, 1))
            {
                if (x1 >= x2)
                    return -90d;
                else
                    return 90d;
            }
            double m1 = (y1 - y2) / (x1 - x2);
            double k;
            var w = size.Width;
            var h = size.Height;
            if (h == 0)
                return 0d;
            if (Math.Round(w) == Math.Round(h))
                k = 1;
            else
            {
                if (w > h)
                {
                    k = 90;
                }
                else
                {
                    k = w / h;
                    k = 45 * k;
                }
            }

            double angle = Math.Atan(m1);
            double angleR = k + angle;
            return angleR;
        }

        public static string ManageClassIDs(XmlNode child, string prefix = null)
        {
            var xDoc = XDocument.Parse(child.ToXml());
            var list = xDoc.Descendants().Where(n => n.HasAttributes && /*n.Attribute("Tag")?.Value == "BG" &&*/ n.Attribute("class") != null);
            if (list.Count() > 0)
            {
                var defStyles = xDoc.Descendants().Where(n => n.HasAttributes && n.Attribute("type")?.Value == "text/css").Select(n => n.Value);
                List<string> styleElements = new List<string>();
                defStyles.ToList().ForEach(d => styleElements.AddRange(d.Replace("\r", "").Replace("\n", "").Replace("\t", "").Split('}')));
                styleElements = styleElements.Select(x => x.Trim()).ToList();
                list.ToList().ForEach(xelement =>
                {
                    XAttribute attr = xelement.Attribute("class");
                    if (attr != null && !string.IsNullOrEmpty(attr.Value))
                    {
                        bool updateTagBG = xelement.Attribute("Tag")?.Value == "BG";
                        string classID = attr.Value;
                        string[] classIDs = classID.Split(' ');
                        try
                        {
                            classID = string.Empty;
                            classIDs.ToList().ForEach(id =>
                            {
                                if (string.IsNullOrEmpty(id))
                                    return;

                                string newKey = $"{prefix}{id}";

                                if (updateTagBG)
                                {
                                    string fillUrl = (from e in styleElements where e.StartsWith($".{newKey}{{") select e).FirstOrDefault();
                                    if (string.IsNullOrEmpty(fillUrl))
                                    {
                                        if (!string.IsNullOrEmpty(classID))
                                            classID = $"{classID} {newKey}";
                                        else
                                            classID = $"{newKey}";
                                        return;
                                    }

                                    fillUrl = fillUrl.Substring(($".{newKey}{{").Length);
                                    //fillUrl = fillUrl.Substring(0, fillUrl.IndexOf('}'));
                                    //attr.Remove();
                                    List<string> styles = fillUrl.Split(';').ToList();
                                    styles.ForEach(s =>
                                    {
                                        string[] style = s.Split(':');
                                        if (style.Count() == 2)
                                            xelement.Add(new XAttribute($"{style[0]}", $"{style[1]}"));
                                    });
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(classID))
                                        classID = $"{classID} {newKey}";
                                    else
                                        classID = $"{newKey}";
                                }

                            });
                            attr.Remove();
                            if (!string.IsNullOrEmpty(classID))
                                xelement.Add(new XAttribute("class", classID));
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
                return xDoc.ToString();
            }
            else return child.ToXml();
        }
        public static XmlAttribute CreateNewAttribute(string id, string value, XmlDocument xmlDoc)
        {
            XmlAttribute attribute = xmlDoc.CreateAttribute(id);
            attribute.Value = value;
            return attribute;
        }

        private static object GetLayout(object svalue)
        {
            try
            {
                string value = svalue as string;
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDoc.DocumentElement;
                XmlElement rootElement = xmlDoc.CreateElement(string.Empty, "svg", string.Empty);
                rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns", "http://www.w3.org/2000/svg", xmlDoc));
                rootElement.Attributes.Append(JSONHelper.CreateNewAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink", xmlDoc));
                var fragment = xmlDoc.CreateDocumentFragment();
                fragment.InnerXml = value;
                rootElement.AppendChild(fragment);
                var nodelist = rootElement.SelectNodes("descendant::property/property/property");
                JArray jcolumns = new JArray();

                Dictionary<string, column> objectMap = new Dictionary<string,column>();
                column ncolumn = new column();
                string colName = string.Empty;
                foreach (XmlElement node in nodelist)
                {
                    bool visible;
                    int visibleindex;
                    double actualwidth;
                    XmlAttribute attribute = node.Attributes.GetNamedItem("name") as XmlAttribute;
                    if (attribute == null)
                        continue;
                    if(attribute.Value.StartsWith("Item"))
                    {
                        ncolumn = new column();
                        ncolumn.Visible = true;
                        colName = attribute.Value;
                        objectMap.Add(colName, ncolumn);
                    }
                    else if (attribute.Value.StartsWith("FieldName") && objectMap.ContainsKey(colName))
                        objectMap[colName].FieldName = node.InnerText;
                    else if (attribute.Value.StartsWith("ActualWidth") && objectMap.ContainsKey(colName))
                    {
                        if (double.TryParse(node.InnerText, out actualwidth))
                            objectMap[colName].ActualWidth = actualwidth;
                    }
                    else if (attribute.Value.Equals("Visible") && objectMap.ContainsKey(colName))
                    {
                        if (bool.TryParse(node.InnerText, out visible))
                            objectMap[colName].Visible = visible;
                    }
                    else if (attribute.Value.StartsWith("VisibleIndex") && objectMap.ContainsKey(colName))
                    {
                        if (int.TryParse(node.InnerText, out visibleindex))
                            objectMap[colName].VisibleIndex = visibleindex;
                    }
                }

                int i = 1;
                objectMap.Values.ToList().Where(o => o.Visible).OrderBy(o => o.VisibleIndex).ToList().ForEach(c =>
                {
                    JObject jcolumn = new JObject();
                    jcolumn.Add(new JProperty("FieldName", c.FieldName));
                    jcolumn.Add(new JProperty("ActualWidth", c.ActualWidth));
                    jcolumns.Add(jcolumn);
                });
                
                return jcolumns;
            }
            catch
            {
            }
            return svalue;
        }
        class column
        {
            public int VisibleIndex { get; set; }
            public double ActualWidth { get; set; }
            public string FieldName { get; set; }
            public bool Visible { get; set; }
        }
        static List<string> mustAddToList = Properties.Settings.Default.SvgFrameworkProperties.Split('|').ToList();
        public static IEnumerable<PropertyInfo> GetPropertiesInfo(Type type)
        {
            var properties = (from prop in type.GetProperties(System.Reflection.BindingFlags.Public
                                    | BindingFlags.Instance
                                    | System.Reflection.BindingFlags.DeclaredOnly).Where(p =>
                                    (p.CanWrite &&
                                    (!Attribute.IsDefined(p, typeof(Utilities.SvgValueConverterAttribute)) ||
                                    (Attribute.IsDefined(p, typeof(Utilities.SvgValueConverterAttribute)) && (p.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as Utilities.SvgValueConverterAttribute).RequiredKey == true)))
                                    ||
                                    (p.CanRead &&
                                    Attribute.IsDefined(p, typeof(Utilities.SvgValueConverterAttribute)) && (p.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as Utilities.SvgValueConverterAttribute).RequiredKey == true))
                               select prop);
            if (type.BaseType != null && type.BaseType != typeof(UserControl) && type.BaseType != typeof(Control))
                properties = properties.Union(GetPropertiesInfo(type.BaseType));
            return properties;
        }
        public static List<PropertyInfo> GetSetedProperties(DependencyObject obj)
        {
            Type type = obj.GetType();
            if (propertiesMap.ContainsKey(type))
                return propertiesMap[type];

            var properties = GetPropertiesInfo(type).ToList();

            mustAddToList.ForEach(pname =>
            {
                var prop = type.GetProperty(pname);
                if (prop != null && !properties.Contains(prop))
                    properties.Add(prop);
            });

            propertiesMap.Add(type, properties);
            return properties;
        }
        public static Dictionary<Type, List<PropertyInfo>> propertiesMap = new Dictionary<Type, List<PropertyInfo>>();
        public static Dictionary<Type, SvgValueConverterAttribute> styleConverterMap = new Dictionary<Type, SvgValueConverterAttribute>();
        public static Dictionary<Type, SvgValueConverterAttribute> brushConverterMap = new Dictionary<Type, SvgValueConverterAttribute>();
        public static Dictionary<Type, SvgValueConverterAttribute> urlBrushConverterMap = new Dictionary<Type, SvgValueConverterAttribute>();
        public static Dictionary<Type, SvgValueConverterAttribute> urlOverriddenBrushConverterMap = new Dictionary<Type, SvgValueConverterAttribute>();
        public static Dictionary<string, object> GetParameters(Type t, FrameworkElement customObject, Dictionary<Type, List<PropertyInfo>> propertyCache, IDocument document)
        {
            Type type = customObject.GetType();
            Dictionary<string, object> propertymap = new Dictionary<string, object>();
            List<PropertyInfo> properties = new List<PropertyInfo>();
            properties = GetSetedProperties(customObject);

            foreach (PropertyInfo p in properties)
            {
                try
                {
                    string pName = p.Name;
                    object value = ClearValue(p.GetValue(customObject, null));
                    bool hasConverterType = false;
                    if (Attribute.IsDefined(p, typeof(Utilities.SvgValueConverterAttribute)))
                    {
                        var attribute = p.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as Utilities.SvgValueConverterAttribute;
                        if (!string.IsNullOrEmpty(attribute.PropertyName))
                            pName = attribute.PropertyName;

                        if (attribute.ConverterType != null)
                        {
                            hasConverterType = true;
                            Type converterType = attribute.ConverterType;
                            CustomValueConverter converter = (CustomValueConverter)Activator.CreateInstance(converterType);
                            var dproperty = System.ComponentModel.DependencyPropertyDescriptor.FromName(
                                                    p.Name, type, type)?.DependencyProperty;
                            value = converter.ConvertToStorageType(value, customObject, document, dproperty);
                            converter = null;
                        }
                    }
                    
                    if(!hasConverterType)
                    {
                        if (value == null && p.PropertyType == typeof(Brush))
                            value = Brushes.Transparent;
                        else if (value == null && p.PropertyType == typeof(Color))
                            value = Colors.Transparent;
                    }

                    if (!propertymap.ContainsKey(pName))
                        propertymap.Add(pName, value);
                }
                catch (Exception ex)
                {
                }
            }
            return propertymap;
        }

        public static Dictionary<string, string> GetStyles(FrameworkElement customObject, Dictionary<Type, List<PropertyInfo>> propertyCache)
        {
            Type type = customObject.GetType();
            Utilities.SvgValueConverterAttribute customObjectSvgAttribute = null;
            if (styleConverterMap.ContainsKey(type))
                customObjectSvgAttribute = styleConverterMap[type];
            else
            {
                customObjectSvgAttribute = (Utilities.SvgValueConverterAttribute)type.GetCustomAttributes(typeof(Utilities.SvgValueConverterAttribute), true).FirstOrDefault() as Utilities.SvgValueConverterAttribute;
                styleConverterMap.Add(type, customObjectSvgAttribute);
            }

            if (customObjectSvgAttribute != null && customObjectSvgAttribute.HasStyles)
            {
                Type converterType = customObjectSvgAttribute.ConverterType;
                CustomValueConverter converter = (CustomValueConverter)Activator.CreateInstance(converterType);
                var styles = converter.ConvertToStorageType(null, customObject, null, null) as Dictionary<string, string>;
                if(styles == null)
                    return new Dictionary<string, string>();
                return styles;
            }
            return new Dictionary<string, string>();
        }

        public static Dictionary<string, Brush> GetBrushes(FrameworkElement customObject, object document)
        {
            Type type = customObject.GetType();
            Utilities.SvgValueConverterAttribute customObjectSvgAttribute = null;
            if (brushConverterMap.ContainsKey(type))
                customObjectSvgAttribute = brushConverterMap[type];
            else
            {
                customObjectSvgAttribute = (Utilities.SvgValueConverterAttribute)type.GetCustomAttributes(typeof(Utilities.SvgValueConverterAttribute), true).FirstOrDefault() as Utilities.SvgValueConverterAttribute;
                brushConverterMap.Add(type, customObjectSvgAttribute);
            }

            if (customObjectSvgAttribute != null && customObjectSvgAttribute.HasBrushes)
            {
                Type converterType = customObjectSvgAttribute.ConverterType;
                CustomValueConverter converter = (CustomValueConverter)Activator.CreateInstance(converterType);
                var styles = converter.ConvertFromStorageType(null, customObject, document) as Dictionary<string, Brush>;
                if (styles == null)
                    return new Dictionary<string, Brush>();
                return styles;
            }
            return new Dictionary<string, Brush>();
        }

        public static Dictionary<string, Brush> GetUrlBrushes(FrameworkElement customObject, object document)
        {
            Type type = customObject.GetType();
            Utilities.SvgValueConverterAttribute customPropSvgAttribute = null;
            if (urlBrushConverterMap.ContainsKey(type))
                customPropSvgAttribute = urlBrushConverterMap[type];
            else
            {
                var property = (from prop in type.GetProperties(System.Reflection.BindingFlags.Public | BindingFlags.Instance |
                               System.Reflection.BindingFlags.DeclaredOnly)
                                where Attribute.IsDefined(prop, typeof(Utilities.SvgValueConverterAttribute)) &&
                                (prop.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as
                                Utilities.SvgValueConverterAttribute).NeedSVGUrlBrushes == true
                                select prop).FirstOrDefault();

                customPropSvgAttribute = property?.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as Utilities.SvgValueConverterAttribute;
                urlBrushConverterMap.Add(type, customPropSvgAttribute);
            }

            if (customPropSvgAttribute != null && customPropSvgAttribute.NeedSVGUrlBrushes)
            {
                Type converterType = customPropSvgAttribute.ConverterType;
                CustomValueConverter converter = (CustomValueConverter)Activator.CreateInstance(converterType);
                var styles = converter.ConvertFromStorageType(null, customObject, document) as Dictionary<string, Brush>;
                if (styles == null)
                    return new Dictionary<string, Brush>();
                return styles;
            }
            return new Dictionary<string, Brush>();
        }

        public static Dictionary<string, Brush> GetOverrideBrushProperties(FrameworkElement customObject, object document)
        {
            Type type = customObject.GetType();
            Utilities.SvgValueConverterAttribute customPropSvgAttribute = null;
            if (urlOverriddenBrushConverterMap.ContainsKey(type))
                customPropSvgAttribute = urlOverriddenBrushConverterMap[type];
            else
            {
                var property = (from prop in type.GetProperties(System.Reflection.BindingFlags.Public | BindingFlags.Instance |
                               System.Reflection.BindingFlags.DeclaredOnly)
                                where Attribute.IsDefined(prop, typeof(Utilities.SvgValueConverterAttribute)) &&
                                (prop.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as
                                Utilities.SvgValueConverterAttribute).HasOverrideBrushProperties == true
                                select prop).FirstOrDefault();

                customPropSvgAttribute = property?.GetCustomAttribute(typeof(Utilities.SvgValueConverterAttribute), true) as Utilities.SvgValueConverterAttribute;
                urlOverriddenBrushConverterMap.Add(type, customPropSvgAttribute);
            }

            if (customPropSvgAttribute != null && customPropSvgAttribute.HasOverrideBrushProperties)
            {
                Type converterType = customPropSvgAttribute.ConverterType;
                CustomValueConverter converter = (CustomValueConverter)Activator.CreateInstance(converterType);
                var styles = converter.ConvertFromStorageType(null, customObject, document) as Dictionary<string, Brush>;
                if (styles == null)
                    return new Dictionary<string, Brush>();
                return styles;
            }
            return new Dictionary<string, Brush>();
        }

        static object ClearValue(object value)
        {
            if (value is string)
            {
                var newvalue = (value as string).Replace("{", "\\{")
                    .Replace("}", "\\}");
                return newvalue;
            }
            else
                return value;
        }

    internal static XmlAttribute CreateNewAttribute(string v1, string v2, double v3, double v4, CornerRadius cornerRadius)
    {
        throw new NotImplementedException();
    }
}

#endif
    public enum ColorMode
    {
        Fill,
        Stroke,
        Both
    }
    public enum Animation
    {
        Fill,
        Rotate
    }

    [DataContract(Name = "OPCUAEntityReferenceMap", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OPCUAEntityReferenceMap
    {
        [DataMember]
        public int? SVGReferenceId { get; set; }
        [DataMember]
        public string ScreenEntityName { get; set; }
        [DataMember]
        public ReferenceType Kind { get; set; }
        [DataMember]
        public string RefName { get; set; }
    }
    [CollectionDataContract(Name = "OPCUAEntityReferenceMapList", ItemName = "OPCUAEntityReferenceMap", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OPCUAEntityReferenceMapList : List<OPCUAEntityReferenceMap>
    {
        #region Constructors
        public OPCUAEntityReferenceMapList(List<OPCUAEntityReferenceMap> collection)
            : base(collection)
        {
        }

        public OPCUAEntityReferenceMapList()
            : base()
        {
        }
        #endregion
    }


    [DataContract(Name = "SVGHelper", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class SVGHelper
    {
        [DataMember]
        public int SVGReferenceId { get; set; }
        [DataMember]
        public OPCUAViewModel.OPCUAEntityReference Tag { get; set; }
        [DataMember]
        public string Expression { get; set; }
        [DataMember]
        public string ReverseExpression { get; set; }
        [DataMember]
        public string EUnit { get; set; }
        [DataMember]
        public double? EUMin { get; set; }
        [DataMember]
        public double? EUMax { get; set; }
        public SVGHelper()
        {
        }
    }
    [CollectionDataContract(Name = "SVGHelperList", ItemName = "SVGHelper", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class SVGHelperList : List<SVGHelper>
    {
        #region Constructors
        public SVGHelperList(List<SVGHelper> collection)
            : base(collection)
        {
        }

        public SVGHelperList()
            : base()
        {
        }
        #endregion
    }

    public enum ReferenceType
    {
        Expression,
        ReverseExpression,
        Tag,
        Animation,
        Command,
        AnimationExpression,
        CommandExpresion,
        CommandTags,
        IDynamicTag
    }
}
