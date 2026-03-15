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
using System.Xml;
using System.Xml.Linq;
#if WINRT
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Html;
using Windows.Foundation;
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    internal class ShapeFileKmlReader : DependencyObject
    {
        #region Constructor

        internal ShapeFileKmlReader()
        {
            PlacemarkList = new List<KmlPlacemark>();
            PolygonList = new List<KmlPolygon>();
            PointList = new List<KmlPoint>();
            StyleMapList = new Dictionary<string, KmlStyleMap>();
            StyleList = new Dictionary<string, KmlStyle>();
        }

        #endregion

        #region Dependency Properties

        #region PlacemarkList
        public List<KmlPlacemark> PlacemarkList
        {
            get { return (List<KmlPlacemark>)GetValue(PlacemarkListProperty); }
            set { SetValue(PlacemarkListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlacemarkList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacemarkListProperty =
            DependencyProperty.Register("PlacemarkList", typeof(List<KmlPlacemark>), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #region BoundingBox
        public KmlBoundingBox BoundingBox
        {
            get { return (KmlBoundingBox)GetValue(BoundingBoxProperty); }
            set { SetValue(BoundingBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoundingBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoundingBoxProperty =
            DependencyProperty.Register("BoundingBox", typeof(KmlBoundingBox), typeof(ShapeFileKmlReader), new PropertyMetadata(new KmlBoundingBox()));
        #endregion

        #region PolygonList
        public List<KmlPolygon> PolygonList
        {
            get { return (List<KmlPolygon>)GetValue(PolygonListProperty); }
            set { SetValue(PolygonListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PolygonList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PolygonListProperty =
            DependencyProperty.Register("PolygonList", typeof(List<KmlPolygon>), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #region PointList
        public List<KmlPoint> PointList
        {
            get { return (List<KmlPoint>)GetValue(PointListProperty); }
            set { SetValue(PointListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointListProperty =
            DependencyProperty.Register("PointList", typeof(List<KmlPoint>), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #region StyleMapList
        public Dictionary<string, KmlStyleMap> StyleMapList
        {
            get { return (Dictionary<string, KmlStyleMap>)GetValue(StyleMapListProperty); }
            set { SetValue(StyleMapListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyleMapList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleMapListProperty =
            DependencyProperty.Register("StyleMapList", typeof(Dictionary<string, KmlStyleMap>), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #region StyleList
        public Dictionary<string, KmlStyle> StyleList
        {
            get { return (Dictionary<string, KmlStyle>)GetValue(StyleListProperty); }
            set { SetValue(StyleListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyleList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleListProperty =
            DependencyProperty.Register("StyleList", typeof(Dictionary<string, KmlStyle>), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #region NormalStyle
        public KmlStyle NormalStyle
        {
            get { return (KmlStyle)GetValue(NormalStyleProperty); }
            set { SetValue(NormalStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalStyleProperty =
            DependencyProperty.Register("NormalStyle", typeof(KmlStyle), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #region HighlightStyle
        public KmlStyle HighlightStyle
        {
            get { return (KmlStyle)GetValue(HighlightStyleProperty); }
            set { SetValue(HighlightStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightStyleProperty =
            DependencyProperty.Register("HighlightStyle", typeof(KmlStyle), typeof(ShapeFileKmlReader), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Internal Fields

        internal ShapeFileLayer baseLayer;

        #endregion

        #region Implementation

        internal void ReadKMLStream(Stream kmlStream, ShapeFileLayer shapeFileLayer, bool isBaseLayer)
        {
            if (isBaseLayer)
                baseLayer = shapeFileLayer;

            var xDocument = XDocument.Load(XmlReader.Create(kmlStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            }));
            XElement rootNode = null;
            foreach (var xNode in xDocument.Nodes())
            {
                if (xNode is XElement)
                {
                    rootNode = (XElement)xNode;
                    break;
                }
            }

            if (rootNode != null)
            {
                SetKmlStyles(rootNode);
                SetKmlBoundingBox(rootNode);

                #region PlacemarkList

                var placemarkNodeList = new List<XElement>();
                GetKmlChildNodes(ref placemarkNodeList, rootNode, "Placemark");
                foreach (var placemarkNode in placemarkNodeList)
                {
                    var placemark = new KmlPlacemark();

                    #region Name & Description

                    var nameNode = GetKmlChildNode(placemarkNode, "name");
                    if (nameNode != null)
                    {
#if WINRT
                        string html = HtmlFormatHelper.GetStaticFragment(HtmlFormatHelper.CreateHtmlFormat(nameNode.Value));
                        placemark.Name = HtmlUtilities.ConvertToText(html);
#else
                        placemark.Name = nameNode.Value;
#endif
                    }
                    var descriptionNode = GetKmlChildNode(placemarkNode, "description");
                    if (descriptionNode != null)
                    {
#if WINRT
                        string html = HtmlFormatHelper.GetStaticFragment(HtmlFormatHelper.CreateHtmlFormat(descriptionNode.Value));
                        placemark.Description = HtmlUtilities.ConvertToText(html);
#endif
                    }

                    #endregion

                    #region ExtendedData

                    var extendedDataNode = GetKmlChildNode(placemarkNode, "ExtendedData");
                    if (extendedDataNode != null)
                    {
                        var dataNodes = new List<XElement>();
                        GetKmlChildNodes(ref dataNodes, extendedDataNode, "Data");
                        if (dataNodes.Count > 0)
                        {
                            placemark.ExtendedData = new Dictionary<string, KmlData>();
                            foreach (XElement dataNode in dataNodes)
                            {
                                if (dataNode.HasAttributes && dataNode.Attribute("name") != null)
                                {
                                    var name = dataNode.Attribute("name").Value;
                                    var data = new KmlData();
                                    var displayNameNode = GetKmlChildNode(dataNode, "displayName");
                                    if (displayNameNode != null)
                                        data.DisplayName = displayNameNode.Value;
                                    var valueNode = GetKmlChildNode(dataNode, "value");
                                    if (valueNode != null)
                                        data.Value = valueNode.Value;
                                    placemark.ExtendedData.Add(name, data);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Styles

                    XElement styleNode = GetKmlChildNode(placemarkNode, "Style");
                    if (styleNode != null)
                    {
                        string kmlStyleId = GetKmlStyleId(styleNode);
                        if (string.IsNullOrEmpty(kmlStyleId))
                        {
                            string uniqueId = styleNode.GetHashCode().ToString();
                            placemark.NormalStyle = CreateKmlStyle(styleNode, uniqueId, placemark);
                        }
                        else
                        {
                            placemark.NormalStyle = GetKmlPredefinedStyle(kmlStyleId, false, placemark);
                        }
                    }
                    else
                    {
                        XElement styleUrlNode = GetKmlChildNode(placemarkNode, "styleUrl");
                        if (styleUrlNode != null)
                        {
                            placemark.NormalStyle = GetKmlPredefinedStyle(styleUrlNode.Value.Substring(1), false, placemark);
                            placemark.HighlightStyle = GetKmlPredefinedStyle(styleUrlNode.Value.Substring(1), true, placemark);
                        }
                    }

                    #endregion

                    #region Polygons

                    var polygonNodeList = new List<XElement>();
                    GetKmlChildNodes(ref polygonNodeList, placemarkNode, "Polygon");
                    foreach (var polygonNode in polygonNodeList)
                    {
                        var polygon = new KmlPolygon();

                        var outerBoundaryNode = GetKmlChildNode(polygonNode, "outerBoundaryIs");
                        if (outerBoundaryNode != null)
                            polygon.OuterBoundary = CreateKmlBoundary(outerBoundaryNode);

                        var innnerBoundaryNodeList = new List<XElement>();
                        GetKmlChildNodes(ref innnerBoundaryNodeList, polygonNode, "innerBoundaryIs");
                        if (innnerBoundaryNodeList != null)
                        {
                            polygon.InnerBoundaryList = new List<KmlBoundary>();
                            foreach (var innerBoundaryNode in innnerBoundaryNodeList)
                            {
                                polygon.InnerBoundaryList.Add(CreateKmlBoundary(innerBoundaryNode));
                            }
                        }
                        polygon.Placemark = placemark;
                        placemark.Polygons.Add(polygon);
                    }

                    #endregion

                    #region Points

                    var pointNodeList = new List<XElement>();
                    GetKmlChildNodes(ref pointNodeList, placemarkNode, "Point");
                    foreach (var pointNode in pointNodeList)
                    {
                        string coordinate = pointNode.Value;
                        if (coordinate.Contains(","))
                        {
                            string[] points = coordinate.Split(new[] { ',' }, StringSplitOptions.None);
                            if (points != null && points.Length > 1)
                            {
                                double x = Double.Parse(points[0]);
                                double y = Double.Parse(points[1]);
                                var kmlPoint = new KmlPoint
                                {
                                    Point = new Point(x, y),
                                    Placemark = placemark
                                };
                                placemark.Points.Add(kmlPoint);
                            }
                        }
                    }

                    #endregion

                    #region Balloon

                    XElement balloonVisibilityNode = GetKmlChildNode(placemarkNode, "balloonVisibility");
                    if (balloonVisibilityNode != null)
                    {
                        placemark.BalloonVisibility = (Double.Parse(balloonVisibilityNode.Value).Equals(1))
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                    }
                    placemark.SetBalloonStyle();

                    #endregion

                    PolygonList.AddRange(placemark.Polygons);
                    PointList.AddRange(placemark.Points);
                    PlacemarkList.Add(placemark);
                }

                #endregion
            }
        }

        private void SetKmlBoundingBox(XElement parentNode)
        {
            var regionNode = GetKmlChildNode(parentNode, "Region");
            if (regionNode != null)
            {
                BoundingBox.hasBoundingValues = true;

                var eastNode = GetKmlChildNode(regionNode, "East");
                var westNode = GetKmlChildNode(regionNode, "West");
                var northNode = GetKmlChildNode(regionNode, "North");
                var southNode = GetKmlChildNode(regionNode, "South");

                if (eastNode != null)
                    BoundingBox.East = Double.Parse(eastNode.Value);
                if (westNode != null)
                    BoundingBox.West = Double.Parse(westNode.Value);
                if (northNode != null)
                    BoundingBox.North = Double.Parse(northNode.Value);
                if (southNode != null)
                    BoundingBox.South = Double.Parse(southNode.Value);
            }
        }

        private void SetKmlStyles(XElement rootNode)
        {
            var styleNodes = new List<XElement>();
            GetKmlChildNodes(ref styleNodes, rootNode, "Style");

            foreach (var styleNode in styleNodes)
            {
                string kmlStyleId = GetKmlStyleId(styleNode);
                if (!String.IsNullOrEmpty(kmlStyleId))
                {
                    CreateKmlStyle(styleNode, kmlStyleId, null);
                }
            }

            var styleMapNodes = new List<XElement>();
            GetKmlChildNodes(ref styleMapNodes, rootNode, "StyleMap");
            foreach (var styleMapNode in styleMapNodes)
            {
                var pairNodes = new List<XElement>();
                GetKmlChildNodes(ref pairNodes, styleMapNode, "Pair");
                var styleMap = new KmlStyleMap();
                foreach (var pairNode in pairNodes)
                {
                    string[] keyValues = pairNode.Value.Split(new[] { '#' }, StringSplitOptions.None);
                    if (keyValues.Count() > 1)
                    {
                        if (keyValues[0] == "normal" && StyleList.ContainsKey(keyValues[1]))
                            styleMap.NormalStyle = GetKmlPredefinedStyle(keyValues[1], false, null);
                        if (keyValues[0] == "highlight" && StyleList.ContainsKey(keyValues[1]))
                            styleMap.HighlightStyle = GetKmlPredefinedStyle(keyValues[1], true, null);
                    }
                }
                StyleMapList.Add(GetKmlStyleId(styleMapNode), styleMap);
            }
        }

        internal XElement GetKmlChildNode(XElement rootNode, string nodeName)
        {
            if (rootNode.Name.LocalName.Equals(nodeName))
            {
                return rootNode;
            }
            foreach (var xNode in rootNode.Nodes())
            {
                if (xNode is XElement && (xNode as XElement).Name.LocalName == nodeName)
                {
                    return (xNode as XElement);
                }
            }
            return null;
        }

        internal void GetKmlChildNodes(ref List<XElement> nodeList, XElement rootNode, string nodeName)
        {
            if (rootNode != null && rootNode.NodeType == XmlNodeType.Element)
            {
                if (rootNode.Name.LocalName == nodeName)
                {
                    nodeList.Add(rootNode);
                }
                else if (rootNode.HasElements)
                {
                    foreach (var xNode in rootNode.Nodes())
                    {
                        GetKmlChildNodes(ref nodeList, xNode as XElement, nodeName);
                    }
                }
            }
        }

        private string GetKmlStyleId(XElement styleNode)
        {
            if (styleNode.HasAttributes && styleNode.Attribute("id") != null)
            {
                return styleNode.Attribute("id").Value;
            }
            return string.Empty;
        }

        private KmlStyle GetKmlPredefinedStyle(string key, bool isHighlightStyle, KmlPlacemark placemark)
        {
            KmlStyle kmlStyle = null;
            if (StyleList.ContainsKey(key))
                StyleList.TryGetValue(key, out kmlStyle);
            else if (StyleMapList.ContainsKey(key))
            {
                KmlStyleMap kmlStyleMap;
                StyleMapList.TryGetValue(key, out kmlStyleMap);
                kmlStyle = isHighlightStyle ? kmlStyleMap.HighlightStyle : kmlStyleMap.NormalStyle;
                if (placemark != null)
                    placemark.hasStyleMapStyle = true;
            }
            if (kmlStyle != null && placemark != null)
                kmlStyle.Placemark = placemark;
            return kmlStyle;
        }

        private KmlStyle CreateKmlStyle(XElement rootNode, string styleId, KmlPlacemark placemark)
        {
            var kmlStyle = new KmlStyle { Id = styleId, Placemark = placemark, styleNode = rootNode, kmlReader = this };
            StyleList.Add(styleId, kmlStyle);
            return kmlStyle;
        }

        private KmlBoundary CreateKmlBoundary(XElement rootNode)
        {
            var boundary = new KmlBoundary { LinearString = rootNode.Value };
            boundary.SetCoordinates();
            return boundary;
        }

        #endregion
    }
}
