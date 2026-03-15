#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Implements the data store for the <see cref="ChartStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    /// <internalonly/>
    [Serializable, DocumentationExclude()]
    public class ChartStyleInfoStore : StyleInfoStore
    {
        #region Constants
        private static StaticData sd = new StaticData(typeof(ChartStyleInfoStore), typeof(ChartStyleInfo), false);

        // Objects - more frequently used fields should come first because of memory performance reasons.
        // Data will be allocated per style object on a slot basis, 4 object references at a time.
        internal static readonly StyleInfoProperty TextColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "TextColor");
        internal static readonly StyleInfoProperty TextShapeProperty = sd.CreateStyleInfoProperty(typeof(ChartCustomShapeInfo), "TextShapeProperty");
        internal static readonly StyleInfoProperty BaseStyleProperty = sd.CreateStyleInfoProperty(typeof(string), "BaseStyle");
        internal static readonly StyleInfoProperty AltTagFormatProperty = sd.CreateStyleInfoProperty(typeof(string), "AltTagFormat");
        internal static readonly StyleInfoProperty FontProperty = sd.CreateStyleInfoProperty(typeof(ChartFontInfo), "Font");

        internal static readonly StyleInfoProperty InteriorProperty = sd.CreateStyleInfoProperty(typeof(BrushInfo), "Interior");        
        internal static readonly StyleInfoProperty TextProperty = sd.CreateStyleInfoProperty(typeof(string), "Text");
        internal static readonly StyleInfoProperty ToolTipProperty = sd.CreateStyleInfoProperty(typeof(string), "ToolTip");
        internal static readonly StyleInfoProperty ImagesProperty = sd.CreateStyleInfoProperty(typeof(ChartImageCollection), "Images");
        internal static readonly StyleInfoProperty ImageIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "ImageIndex");
        internal static readonly StyleInfoProperty SymbolProperty = sd.CreateStyleInfoProperty(typeof(ChartSymbolInfo), "Symbol");
        internal static readonly StyleInfoProperty SystemProperty = sd.CreateStyleInfoProperty(typeof(bool), "System");
        internal static readonly StyleInfoProperty NameProperty = sd.CreateStyleInfoProperty(typeof(string), "Name");

        internal static readonly StyleInfoProperty TextOrientationProperty = sd.CreateStyleInfoProperty(typeof(ChartTextOrientation), "TextOrientation");
        internal static readonly StyleInfoProperty DisplayShadowProperty = sd.CreateStyleInfoProperty(typeof(bool), "DisplayShadow");
        internal static readonly StyleInfoProperty ShadowOffsetProperty = sd.CreateStyleInfoProperty(typeof(Size), "ShadowOffset");
        internal static readonly StyleInfoProperty ShadowInteriorProperty = sd.CreateStyleInfoProperty(typeof(BrushInfo), "ShadowInterior");
        internal static readonly StyleInfoProperty HighlightInteriorProperty = sd.CreateStyleInfoProperty(typeof(BrushInfo), "HighlightInterior");
        internal static readonly StyleInfoProperty DimmedInteriorProperty = sd.CreateStyleInfoProperty(typeof(BrushInfo), "DimmedInterior");
        internal static readonly StyleInfoProperty HighlightOnMouseOverProperty = sd.CreateStyleInfoProperty(typeof(bool), "HighlightOnMouseOver");
        internal static readonly StyleInfoProperty HitTestRadiusProperty = sd.CreateStyleInfoProperty(typeof(float), "HitTestRadius");
        internal static readonly StyleInfoProperty LabelProperty = sd.CreateStyleInfoProperty(typeof(string), "Label");
        internal static readonly StyleInfoProperty PointWidthProperty = sd.CreateStyleInfoProperty(typeof(float), "PointWidth");

        internal static readonly StyleInfoProperty TextOffsetProperty = sd.CreateStyleInfoProperty(typeof(float), "TextOffset");
        internal static readonly StyleInfoProperty BorderProperty = sd.CreateStyleInfoProperty(typeof(ChartLineInfo), "Border");

        internal static readonly StyleInfoProperty DisplayTextProperty = sd.CreateStyleInfoProperty(typeof(bool), "DisplayText");
        internal static readonly StyleInfoProperty DrawTextShapeProperty = sd.CreateStyleInfoProperty(typeof(bool), "DrawTextShapeProperty");
        internal static readonly StyleInfoProperty TextFormatProperty = sd.CreateStyleInfoProperty(typeof(string), "TextFormat");
        internal static readonly StyleInfoProperty FormatProperty = sd.CreateStyleInfoProperty(typeof(StringFormat), "Format");
        internal static readonly StyleInfoProperty ToolTipFormatProperty = sd.CreateStyleInfoProperty(typeof(string), "ToolTipFormat");

        internal static readonly StyleInfoProperty ElementBordersProperty = sd.CreateStyleInfoProperty(typeof(ChartBordersInfo), "ElementBorders");

        internal static readonly StyleInfoProperty RelatedPointsProperty = sd.CreateStyleInfoProperty(typeof(ChartRelatedPointInfo), "RelatedPoints");
        internal static readonly StyleInfoProperty UrlProperty = sd.CreateStyleInfoProperty(typeof(String), "Url");
        #endregion

        #region Properties
        /// <summary>
        /// Gets the static data.
        /// </summary>
        /// <value>The static data.</value>
        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <value></value>
        /// <internalonly/>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="ChartStyleInfoStore"/> class.
        /// </summary>
        static ChartStyleInfoStore()
        {
            FontProperty.CreateObject = new CreateSubObjectHandler(ChartFontInfo.CreateObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoStore"/> class.
        /// </summary>
        /// <internalonly/>
        public ChartStyleInfoStore()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoStore"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <internalonly/>
        protected ChartStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (sd.IsEmpty)
            {
                new ChartStyleInfo();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns clone object of StyleInfoStore.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new ChartStyleInfoStore();
            CopyTo(target);
            return target;
        }

        /// <summary>
        /// Returns the <see cref="XmlSchema"/> specifies the correct store.
        /// </summary>
        /// <returns>Returns new instance of XmlSchema object.</returns>
        /// <internalonly/>
        public static new XmlSchema GetSchema()
        {
            ////XmlSchema schema = base.GetSchema();
            XmlSchema schema = new XmlSchema();

            #region sip Arrays
            StyleInfoProperty[] chartStyleInfoStoreSipArray = 
                {
                TextColorProperty,
                BaseStyleProperty,
                FontProperty,
                InteriorProperty,                
                TextProperty,
                ToolTipProperty,
                ImagesProperty,
                ImageIndexProperty,
                SymbolProperty,                
                SystemProperty,
                NameProperty,
                TextOrientationProperty,
                DisplayShadowProperty,
                ShadowOffsetProperty,
                ShadowInteriorProperty,
                HighlightInteriorProperty,
                DimmedInteriorProperty,
                HighlightOnMouseOverProperty,
                HitTestRadiusProperty,
                LabelProperty,
                PointWidthProperty,
                TextOffsetProperty,
                BorderProperty,
                DisplayTextProperty,
                TextFormatProperty,
                FormatProperty,
                ToolTipFormatProperty,
                ElementBordersProperty,
                RelatedPointsProperty
                };

            StyleInfoProperty[] chartLineInfoStoreSipArray = 
                {
                    ChartLineInfoStore.AlignmentProperty,
                    ChartLineInfoStore.ColorProperty,
                    ChartLineInfoStore.DashPatternProperty,
                    ChartLineInfoStore.DashStyleProperty,
                    ChartLineInfoStore.WidthProperty
                };

            StyleInfoProperty[] chartFontInfoStoreSipArray = 
                {
                    ChartFontInfoStore.BoldProperty,
                    ChartFontInfoStore.FacenameProperty,
                    ChartFontInfoStore.FontFamilyTemplateProperty,
                    ChartFontInfoStore.ItalicProperty,
                    ChartFontInfoStore.OrientationProperty,
                    ChartFontInfoStore.SizeProperty,
                    ChartFontInfoStore.StrikeoutProperty,
                    ChartFontInfoStore.UnderlineProperty,
                    ChartFontInfoStore.UnitProperty
                };

            StyleInfoProperty[] chartSymbolInfoStoreSipArray = 
                {
                    ChartSymbolInfoStore.ColorProperty,
                    ChartSymbolInfoStore.HighlightColorProperty,
                    ChartSymbolInfoStore.DimmedColorProperty,
                    ChartSymbolInfoStore.ImageIndexProperty,
                    ChartSymbolInfoStore.MarkerProperty,
                    ChartSymbolInfoStore.OffsetProperty,
                    ChartSymbolInfoStore.ShapeProperty,
                    ChartSymbolInfoStore.SizeProperty
                };

            StyleInfoProperty[] chartBordersInfoStoreSipArray = 
                {
                    ChartBordersInfoStore.InnerProperty,
                    ChartBordersInfoStore.OuterProperty
                };

            StyleInfoProperty[] chartRelatedPointInfoStoreSipArray = 
                {
                    ChartRelatedPointInfoStore.AlignmentProperty,
                    ChartRelatedPointInfoStore.BorderProperty,
                    ChartRelatedPointInfoStore.ColorProperty,
                    ChartRelatedPointInfoStore.DashPatternProperty,
                    ChartRelatedPointInfoStore.DashStyleProperty,
                    ChartRelatedPointInfoStore.EndSymbolProperty,
                    ChartRelatedPointInfoStore.PointsProperty,
                    ChartRelatedPointInfoStore.StartSymbolProperty,
                    ChartRelatedPointInfoStore.WidthProperty
                };

            ArrayList allSipArray = new ArrayList();
            Hashtable names = new Hashtable(100);
            StyleInfoProperty[][] styleArAr = {  chartStyleInfoStoreSipArray,
                                            chartLineInfoStoreSipArray,
                                            chartFontInfoStoreSipArray,    
                                            chartSymbolInfoStoreSipArray,
                                            chartBordersInfoStoreSipArray,
                                            chartRelatedPointInfoStoreSipArray
                                         };

            for (int i = 0; i < styleArAr.Length; i++)
                for (int j = 0; j < styleArAr[i].Length; j++)
                {
                    if ((!allSipArray.Contains(styleArAr[i][j])) && (!names.Contains(styleArAr[i][j].PropertyName)))
                    {
                        allSipArray.Add(styleArAr[i][j]);
                        names.Add(styleArAr[i][j].PropertyName, styleArAr[i][j].PropertyName);
                    }
                }

            #endregion

            // <xs:element name="ChartStyleInfo" type="xs:string"/>
            XmlSchemaElement csi = new XmlSchemaElement();
            schema.Items.Add(csi);
            csi.Name = typeof(ChartStyleInfo).Name;

            // <xs:complexType>
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            csi.SchemaType = complexType;

            // <xs:complexType>
            // <xs:all>
            XmlSchemaAll all = new XmlSchemaAll();
            all.MinOccurs = 0;
            complexType.Particle = all;

            #region all elements
            for (int i = 0; i < chartStyleInfoStoreSipArray.Length; i++)
            {
                XmlSchemaElement el = new XmlSchemaElement();
                el.Name = chartStyleInfoStoreSipArray[i].PropertyName;
                el.MinOccurs = 0;
                el.SchemaTypeName = new XmlQualifiedName(chartStyleInfoStoreSipArray[i].PropertyName);
                all.Items.Add(el);
            }

            XmlSchemaAll chartLineInfoStoreAll = new XmlSchemaAll();
            chartLineInfoStoreAll.MinOccurs = 0;
            for (int i = 0; i < chartLineInfoStoreSipArray.Length; i++)
            {
                XmlSchemaElement el = new XmlSchemaElement();
                el.Name = chartLineInfoStoreSipArray[i].PropertyName;
                el.MinOccurs = 0;
                el.SchemaTypeName = new XmlQualifiedName(chartLineInfoStoreSipArray[i].PropertyName);
                chartLineInfoStoreAll.Items.Add(el);
            }

            XmlSchemaAll chartFontInfoStoreAll = new XmlSchemaAll();
            chartFontInfoStoreAll.MinOccurs = 0;
            for (int i = 0; i < chartFontInfoStoreSipArray.Length; i++)
            {
                XmlSchemaElement el = new XmlSchemaElement();
                el.Name = chartFontInfoStoreSipArray[i].PropertyName;
                el.MinOccurs = 0;
                el.SchemaTypeName = new XmlQualifiedName(chartFontInfoStoreSipArray[i].PropertyName);
                chartFontInfoStoreAll.Items.Add(el);
            }

            XmlSchemaAll chartSymbolInfoStoreAll = new XmlSchemaAll();
            chartSymbolInfoStoreAll.MinOccurs = 0;
            for (int i = 0; i < chartSymbolInfoStoreSipArray.Length; i++)
            {
                XmlSchemaElement el = new XmlSchemaElement();
                el.Name = chartSymbolInfoStoreSipArray[i].PropertyName;
                el.MinOccurs = 0;
                el.SchemaTypeName = new XmlQualifiedName(chartSymbolInfoStoreSipArray[i].PropertyName);
                chartSymbolInfoStoreAll.Items.Add(el);
            }

            XmlSchemaAll chartBordersInfoStoreAll = new XmlSchemaAll();
            chartBordersInfoStoreAll.MinOccurs = 0;
            for (int i = 0; i < chartBordersInfoStoreSipArray.Length; i++)
            {
                XmlSchemaElement el = new XmlSchemaElement();
                el.Name = chartBordersInfoStoreSipArray[i].PropertyName;
                el.MinOccurs = 0;
                el.SchemaTypeName = new XmlQualifiedName(chartBordersInfoStoreSipArray[i].PropertyName);
                chartBordersInfoStoreAll.Items.Add(el);
            }

            XmlSchemaAll chartRelatedPointInfoStoreAll = new XmlSchemaAll();
            chartRelatedPointInfoStoreAll.MinOccurs = 0;
            for (int i = 0; i < chartRelatedPointInfoStoreSipArray.Length; i++)
            {
                XmlSchemaElement el = new XmlSchemaElement();
                el.Name = chartRelatedPointInfoStoreSipArray[i].PropertyName;
                el.MinOccurs = 0;
                el.SchemaTypeName = new XmlQualifiedName(chartRelatedPointInfoStoreSipArray[i].PropertyName);
                chartRelatedPointInfoStoreAll.Items.Add(el);
            }
            #endregion

            for (int i = 0; i < allSipArray.Count; i++)
            {
                StyleInfoProperty sip = (StyleInfoProperty)allSipArray[i];

                if (sip.IsExpandable)
                {
                    XmlSchemaComplexType complType = new XmlSchemaComplexType();
                    complType.Name = sip.PropertyName;

                    if (sip.PropertyType == typeof(ChartLineInfo))
                    {
                        complType.Particle = chartLineInfoStoreAll;
                    }
                    else if (sip.PropertyType == typeof(ChartFontInfo))
                    {
                        complType.Particle = chartFontInfoStoreAll;
                    }
                    else if (sip.PropertyType == typeof(ChartSymbolInfo))
                    {
                        complType.Particle = chartSymbolInfoStoreAll;
                    }
                    else if (sip.PropertyType == typeof(ChartBordersInfo))
                    {
                        complType.Particle = chartBordersInfoStoreAll;
                    }
                    else if (sip.PropertyType == typeof(ChartRelatedPointInfoStore))
                    {
                        complType.Particle = chartRelatedPointInfoStoreAll;
                    }
                    else
                    {
                        complType.Particle = all;
                    }
                    schema.Items.Add(complType);
                }
                else
                {
                    if (sip.SerializeXmlBehavior == SerializeXmlBehavior.Skip)
                        continue;

                    if ((sip.SerializeXmlBehavior == SerializeXmlBehavior.SerializeAsString) ||
                            (sip.ObjectStoreKey == -1 ||
                            sip.PropertyType == typeof(Color) ||
                            sip.PropertyType == typeof(string) ||
                            sip.PropertyType.IsPrimitive) ||
                            (sip.PropertyType == typeof(Type))
                            )
                    {
                        XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
                        simpleType.Name = sip.PropertyName;

                        XmlSchemaSimpleTypeRestriction stringRestriction = new XmlSchemaSimpleTypeRestriction();
                        stringRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                        simpleType.Content = stringRestriction;

                        schema.Items.Add(simpleType);
                    }
                    else
                    {
                        XmlSchemaComplexType complType = new XmlSchemaComplexType();
                        complType.Name = sip.PropertyName;

                        XmlSchemaSequence seq = new XmlSchemaSequence();
                        XmlSchemaAny any = new XmlSchemaAny();
                        any.MinOccurs = 0;
                        any.ProcessContents = XmlSchemaContentProcessing.Skip;
                        seq.Items.Add(any);

                        complType.Particle = seq;
                        schema.Items.Add(complType);
                    }
                }
            }

            return schema;
        }
        #endregion
    }
}