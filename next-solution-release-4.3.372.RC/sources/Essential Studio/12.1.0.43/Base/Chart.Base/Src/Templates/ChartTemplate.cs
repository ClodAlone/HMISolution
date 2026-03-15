#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Windows.Forms;
using Syncfusion.Drawing;
using System.Drawing;
using System.Globalization;
using System.Drawing.Drawing2D;


namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Provides the storing of chart properties.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ChartTemplate
    {
        #region Constants
        private const char DEF_SPLITER = '.';
        private const string DEF_XML_ROOT = "Chart";
        private const string DEF_VALUE_ATTR = "value";
        #endregion

        #region Members
        private static Type s_ctaType = typeof(ChartTemplateAttribute);        
        private ArrayList m_setters = new ArrayList();
        private ArrayList m_serPointVal = new ArrayList();   
        private static string DEF_SER_ROOT = "ChartSeries";
        private static string DEF_SERCOLLEC_ROOT = "SeriesCollection";
        private static string DEF_SER_NAME = "";
        private static int ser_pointsCount = 0;
        private static int ser_Count;
        private static IChartAreaHost m_chart;
     
        private  static XmlNode rootNode;
        private static XmlNode root_Parent;
        private  static XmlNode copy;
        private  static int count=0;
        #endregion

        public static bool StoreAllProperties = false;
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTemplate"/> class.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        public ChartTemplate(Type chartType)
        {
            m_setters = Generate(chartType);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Loads the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns ChartTemplate with stored properties.</returns>
        public static ChartTemplate Load(IChartAreaHost chart, Stream stream)
        {
            ChartTemplate template = new ChartTemplate(chart.GetType());

            template.Load(stream);
            template.Apply(chart);

            return template;
        }

        /// <summary>
        /// Loads the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Returns ChartTemplate with stored properties.</returns>
        public static ChartTemplate Load(IChartAreaHost chart, string filename)
        {
            
                ChartTemplate template = new ChartTemplate(chart.GetType());
               
                 m_chart = chart;

                template.Load(filename);
               
                template.Apply(chart);
               
                return template;
            
        }

        /// <summary>
        /// Saves the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns ChartTemplate with stored properties.</returns>
        public static ChartTemplate Save(IChartAreaHost chart, Stream stream)
        {
            bool templateAll = false;
            string msg = "Do you want store only the appearance?";

            if (MessageBox.Show(msg, "Template Storing Option...", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                templateAll = true;
            }

            ChartTemplate.StoreAllProperties = templateAll;

            ChartTemplate template = new ChartTemplate(chart.GetType());

            template.Scan(chart);
            template.Save(stream);

            return template;
        }

        /// <summary>
        /// Saves the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Returns ChartTemplate with stored properties.</returns>
        public static ChartTemplate Save(IChartAreaHost chart, string filename)
        {
            using (Stream stream = File.Create(filename))
            {
                bool templateAll = false;
                string msg = "Do you want store only the appearance?";

                if (MessageBox.Show(msg, "Template Storing Option...", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    templateAll = true;
                }
                ChartTemplate.StoreAllProperties = templateAll;
                ChartTemplate template = new ChartTemplate(chart.GetType());            
                template.Scan(chart);
                template.Save(stream);
                if (StoreAllProperties)
                {                  
                    ser_Count = chart.Series.Count;
                    for (int i = 0; i < chart.Series.Count; i++)
                    {
                        ser_pointsCount = chart.Series[i].Points.Count;
                        DEF_SER_NAME = chart.Series[i].Name;
                        template = new ChartTemplate(chart.Series[i].GetType());
                        template.ScanSeries(chart.Series[i]);
                        template.SaveSeries(stream);
                    }
                }
                return template;
            }
        }
       


        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string path, object value)
        {
            ChartSetter setter = SelectBy(path);

            if (setter != null)
            {
                SelectBy(path).Value = value;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns object.</returns>
        public object GetValue(string path)
        {
            return SelectBy(path).Value;
        }

        /// <summary>
        /// Applies the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void Apply(IChartAreaHost chart)
        {
            int i = 0;
            foreach (ChartSetter setter in m_setters)
            {
                setter.Apply(chart);
                i++;
            }
        }

        /// <summary>
        /// Scans the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void Scan(IChartAreaHost chart)
        {
            foreach (ChartSetter setter in m_setters)
            {
                setter.Scan(chart);
            }
        }

        /// <summary>
        /// Scans the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public void ScanSeries(ChartSeries series)
        {
            foreach (ChartSetter setter in m_setters)
            {
                setter.Scan(series);
            }
        }
        

        /// <summary>
        /// Resets the properties of the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public void Reset(IChartAreaHost chart)
        {
            foreach (ChartSetter setter in m_setters)
            {
                setter.Reset(chart);
            }
        }

       

        /// <summary>
        /// Saves the series in specified filename.
        /// </summary>
        /// <param name="s">The stream.</param>
        public void SaveSeries(Stream s)
        {
            XmlDocument xmlDoc = new XmlDocument();       
            if (root_Parent == null)
                root_Parent = xmlDoc.CreateElement(DEF_SERCOLLEC_ROOT);
            else
                root_Parent = xmlDoc.ImportNode(root_Parent, true);
            XmlNode root_Child = xmlDoc.CreateElement(DEF_SER_ROOT);
            XmlElement element= (XmlElement)root_Child;
          
            element.SetAttribute("Name", DEF_SER_NAME);
            if (copy == null)
                copy = xmlDoc.ImportNode(rootNode, true);
            else
                copy = xmlDoc.ImportNode(copy, true);
            foreach (ChartSetter setter in m_setters)
            {
                XmlElement elem;
                if(setter.PropertyName=="Points")
                elem = xmlDoc.CreateElement(setter.PropertyName+"Collection");
                else
                elem = xmlDoc.CreateElement(setter.PropertyName);           
                setter.Write(elem);
                root_Child.AppendChild(elem);
                root_Parent.AppendChild(root_Child);             
            }
                     
            count++;
            if (count == ser_Count )
            {
                copy.AppendChild(root_Parent);
                xmlDoc.AppendChild(copy);
                xmlDoc.Save(s);
                count = 0;
                copy = null;
                root_Parent = null;
            }          
                    
        }

        /// <summary>
        /// Loads the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Load(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                this.Load(stream);
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Load(Stream stream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            XmlNode rootNode = xmlDoc.SelectSingleNode(DEF_XML_ROOT);

            if (rootNode == null)
            {
                throw new FileLoadException("Root element doesn't exist.");
            }

            foreach (XmlElement elem in rootNode.ChildNodes)
            {
                if (elem.Name.Equals("SeriesCollection"))
                {                  
                    foreach (XmlElement child in elem)
                    {
                        int sPoint = 4;
                        Read(child);
                        ChartSeries newSeries = new ChartSeries();
                        while (sPoint < (m_serPointVal.IndexOf("Style")) - 1)
                        {
                            string[] xValue = m_serPointVal[sPoint].ToString().Split(',');
                            string[] yValue = m_serPointVal[sPoint + 1].ToString().Split(',');
                            string[] pEmpty = m_serPointVal[sPoint + 2].ToString().Split(',');
                            newSeries.Points.Add(Convert.ToDouble(xValue[1]), Convert.ToDouble(yValue[1]));
                            newSeries.Points[newSeries.Points.Count - 1].IsEmpty = Convert.ToBoolean(pEmpty[1]);
                            sPoint = sPoint + 5;
                        }
                        SetPropertiesValue(newSeries);
                        m_chart.Series.Add(newSeries);
                        m_serPointVal.Clear();
                    }               
                }
             
                else
                {
                    ChartSetter setter = GetSetter(elem.Name);
                    if (setter != null)
                    {
                        setter.Read(elem);
                    }
                }
            }
        }
        /// <summary>
        /// Setting Properties value for series.
        /// </summary>
        /// <param name="series">series.</param>
        private void SetPropertiesValue(ChartSeries series)
        {
                   
            bool bText=true;
            string sAlign = "BAlign";
            string propVal;         
            PropertyDescriptorCollection styleProperties = TypeDescriptor.GetProperties(series.Style.GetType());
            PropertyDescriptorCollection tooltipProperties = TypeDescriptor.GetProperties(series.FancyToolTip.GetType());
            PropertyDescriptorCollection borderProperties = TypeDescriptor.GetProperties(series.Style.Border.GetType());
            PropertyDescriptorCollection seriesProperties = TypeDescriptor.GetProperties(series.GetType());
            foreach (object item in m_serPointVal)
            {
                
                string[] pValue = item.ToString().Split(new char[]{','},2);
                if (pValue.Length > 1)
                    propVal = pValue[1].ToString();
                else
                    propVal = pValue[0].ToString();
                switch (pValue[0])
                {
                    case "Style": bText = false;
                        if (pValue.Length > 1)
                            series.FancyToolTip.Style = (MarkerStyle)Enum.Parse(typeof(MarkerStyle), propVal);
                        break;
                    case "FancyToolTip": sAlign = "FAlign";
                        break;
                    case"Type": case"ExplodedIndex": case "ExplodedAll": case"ExplosionOffset": case "ResetStyles": case "Rotate":case "SmartLabels": case "SmartLabelsBorderWidth": case "SmartLabelsBorderColor": case "EnableAreaToolTip": case "Name":case "RequireAxes":case "RequireInvertedAxes":case "OriginDependent": case "ZOrder": case "PointsToolTipFormat" : case "Compatible": case "OptimizePiePointPositions":  case "DrawColumnSeparatingLines": case"ShowTicks": case "ScatterConnectType": case "ScatterSplineTension": case"LegendItemUseSeriesStyle":

                        PropertyDescriptor propertyTypeDesc = seriesProperties.Find(pValue[0], false);
                        if (propertyTypeDesc != null)
                            propertyTypeDesc.SetValue(series, ChangeValueType(propertyTypeDesc, propVal));
                        break;
                    case "Text":
                        if (bText)
                            series.Text = propVal;
                        else
                            series.Style.Text = propVal;
                        break;
                    case "Color": case "Width" : case "DashStyle":

                        PropertyDescriptor propertyBorderDesc = borderProperties.Find(pValue[0], false);
                        if (propertyBorderDesc != null)
                            propertyBorderDesc.SetValue(series.Style.Border, ChangeValueType(propertyBorderDesc, propVal));
                        break;
                    case "Alignment":
                        if (sAlign == "FAlign")
                            series.FancyToolTip.Alignment = (TabAlignment)Enum.Parse(typeof(TabAlignment), propVal);
                        else
                            series.Style.Border.Alignment = (PenAlignment)Enum.Parse(typeof(PenAlignment), propVal);
                        break;

                    case "Visible": case "Spacing":  case "Angle" :  case "BackColor" : case "ForeColor": case "Font" :  case "SymbolSize": case "Symbol":
                        PropertyDescriptor propertyTooltipDesc = tooltipProperties.Find(pValue[0], false);
                        if (propertyTooltipDesc != null)
                            propertyTooltipDesc.SetValue(series.FancyToolTip, ChangeValueType(propertyTooltipDesc, propVal));
                        break;
                    case "Interior" : case "TextColor" : case "ToolTip": case "ToolTipFormat": case "TextOrientation" :case "DisplayShadow": case "ShadowInterior" : case "Label" : case "TextFormat":  case "DisplayText": case "PointWidth":  case "TextOffset" :  case "RelatedPoints":
                        PropertyDescriptor propertyStyleDesc = styleProperties.Find(pValue[0], false);
                        if (propertyStyleDesc != null)
                            propertyStyleDesc.SetValue(series.Style, ChangeValueType(propertyStyleDesc, propVal));
                        break;


                }                                                         
            }
        }
        /// <summary>
        /// Changing the valuetype for SeriesProperties.
        /// </summary>
        /// <param name="propertyDesc">The PropertyDescriptor.</param>
        /// <param name="propertyValue">The propertyValue.</param>
        private object ChangeValueType(PropertyDescriptor propertyDesc, string propertyValue)
        {
            object m_value=null;
            TypeConverter converter = propertyDesc.Converter;

            if (converter.CanConvertFrom(typeof(string)))
            {
               
              m_value = converter.ConvertFromString(null, CultureInfo.InvariantCulture, propertyValue);
            }
            return m_value;
         
        }
        /// <summary>
        /// Reads the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        private void Read(XmlElement element)
        {     
            if (element.HasAttribute(DEF_VALUE_ATTR))
            {                
                string value = element.GetAttribute(DEF_VALUE_ATTR);
                string elementVal = element.Name + "," + value;
                m_serPointVal.Add(elementVal);             
            }
            else
            {
                m_serPointVal.Add(element.Name);
            }
         
            foreach (XmlElement elem in element.ChildNodes)
            {
               
                Read(elem);                
            }
        }
        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            using (Stream stream = File.Create(filename))
            {
                this.Save(stream);
            }
        }
        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
           XmlDocument xmlDoc = new XmlDocument();        
           rootNode = xmlDoc.CreateElement(DEF_XML_ROOT);

            foreach (ChartSetter setter in m_setters)
            {               
             XmlElement elem = xmlDoc.CreateElement(setter.PropertyName);
             if (setter.PropertyName != "Series")
             {
                 setter.Write(elem);
                 rootNode.AppendChild(elem);
             }
            }

            xmlDoc.AppendChild(rootNode);
            if(!StoreAllProperties)
            xmlDoc.Save(stream);
           
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Generates tree of setters by the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns ArrayList object.</returns>
        private ArrayList Generate(Type type)
        {
            ArrayList result = new ArrayList();
          
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);

            for (int i = 0; i < properties.Count; i++)
            {
             
                ChartTemplateAttribute cta = properties[i].Attributes[s_ctaType] as ChartTemplateAttribute;               
                
                if (cta != null)
                {
                    ChartSetter setter = new ChartSetter(cta.SetType, properties[i]);
                                     
                    if (cta.SetType == ChartTemplateSet.SimpleBehavior)
                    {
                        if (StoreAllProperties)
                        {
                            result.Add(setter);
                        }
                    }
                    else 
                    {
                        result.Add(setter);
                    }

                    if (setter.Type == ChartTemplateSet.Content)
                    {
                        ArrayList children = Generate(properties[i].PropertyType);

                        foreach (ChartSetter child in children)
                        {
                            setter.Add(child);
                        }
                    }                        
                    else if (setter.Type == ChartTemplateSet.Collection)
                    {
                        ArrayList children = Generate(cta.ItemType);

                        foreach (ChartSetter child in children)
                        {
                            setter.Add(child);
                        }
                    }
                    else if (setter.Type == ChartTemplateSet.SimpleAndCollection)
                    {
                        if (setter.PropertyName == "Points" || setter.PropertyName == "Styles")
                        {
                            for (int j = 0; j < ser_pointsCount; j++)
                            {
                                ArrayList children = Generate(cta.ItemType);
                                setter.m_serPoints.Add(children);
                            }
                        }
                        else
                        {
                            ArrayList children = Generate(cta.ItemType);

                            foreach (ChartSetter child in children)
                            {
                                setter.Add(child);
                            }
                        }
                    }  
                    else if(StoreAllProperties)
                    {
                        if (setter.Type == ChartTemplateSet.ContentBehavior)
                        {
                            ArrayList children = Generate(properties[i].PropertyType);

                            foreach (ChartSetter child in children)
                            {
                                setter.Add(child);
                            }
                        }
                        else if (setter.Type == ChartTemplateSet.CollectionBehavior)
                        {
                            ArrayList children = Generate(cta.ItemType);

                            foreach (ChartSetter child in children)
                            {
                                setter.Add(child);
                            }
                        }
                        else if (setter.Type == ChartTemplateSet.SimpleAndCollectionBehavior)
                        {
                            ArrayList children = Generate(cta.ItemType);

                            foreach (ChartSetter child in children)
                            {
                                setter.Add(child);
                            }
                        }  
                    }
             

                }
            }

            return result;
        }

        /// <summary>
        /// Selects the <see cref="ChartSetter"/> by property path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The ChartSetter object.</returns>
        private ChartSetter SelectBy(string path)
        {
            string[] pathParts = path.Split(DEF_SPLITER);
            ChartSetter result = GetSetter(pathParts[0]);

            for (int i = 1; i < pathParts.Length; i++)
            {
                if (result != null)
                {
                    result = result[pathParts[i]];
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the <see cref="ChartSetter"/> by the property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The ChartSetter object.</returns>
        private ChartSetter GetSetter(string propertyName)
        {
            ChartSetter result = null;

            foreach (ChartSetter setter in m_setters)
            {
                if (setter.PropertyName == propertyName)
                {
                    result = setter;
                    break;
                }
            }

            return result;
        }
        #endregion
    }
}
