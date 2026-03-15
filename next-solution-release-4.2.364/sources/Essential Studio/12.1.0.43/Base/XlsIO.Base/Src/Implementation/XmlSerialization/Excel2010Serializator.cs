#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;

using Syncfusion.Compression.Zip;

using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.CompoundFile.XlsIO;
using System.Diagnostics;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Parser;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
  /// <summary>
  /// Class used for Excel 2007 Serialization.
  /// </summary>
  public class Excel2010Serializator : Excel2007Serializator
  {

    #region Constants
    private const string VersionValue = "14.0300";
    /// <summary>
    /// Uri for data bar properties
    /// </summary>
    public const string DataBarUri = "{B025F937-C7B1-47D3-B67F-A62EFF666E3E}";
    public const string DataBarExtUri = "{78C0D931-6437-407d-A8EE-F0AAD7539E65}";
    #endregion

    /// <summary>
    /// Gets version that is supported by this serializator.
    /// </summary>
    public override ExcelVersion Version
    {
      get
      {
        return ExcelVersion.Excel2010;
      }
    }

    public Excel2010Serializator( WorkbookImpl book ) :
      base( book )
    {
    }

    protected override void SerilaizeExtensions( XmlWriter writer, WorksheetImpl sheet )
    {
        if (sheet.Version == ExcelVersion.Excel2010 && sheet.SparklineGroups.Count > 0)
            SerializeSparklineGroups( writer, sheet );
        SerializeConditionalFormattings(writer, sheet);
    }
  
    /// <summary>
    /// Serializes the sparkline groups.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sheet">The sheet.</param>
    public void SerializeSparklineGroups(XmlWriter writer, WorksheetImpl sheet)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (sheet.SparklineGroups.Count == 0)
            return;

        writer.WriteStartElement(Extensionlist, XmlNamespaceMain);
        writer.WriteStartElement(Vml.Ext, XmlNamespaceMain);
        writer.WriteAttributeString( SparkConstants.UriAttribute, SparklineUri );
        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, X14Prefix, null, X14Namespace);
        writer.WriteStartElement( SparkConstants.SparklineGroupsTag, X14Namespace );
        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, MSPrefix, null, MSNamespaceMain);

        foreach (SparklineGroup sparklineGroup in sheet.SparklineGroups)
        {
            SerializeSparklineGroup(writer, sheet, sparklineGroup);
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes the sparkline group.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sheet">The sheet.</param>
    /// <param name="sparklineGroup">The sparkline group.</param>
    private void SerializeSparklineGroup(XmlWriter writer, WorksheetImpl sheet, SparklineGroup sparklineGroup)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (sparklineGroup == null)
            throw new ArgumentNullException("SparklineGroup");

        ColorObject colorObject;

        writer.WriteStartElement( SparkConstants.SparklineGroupTag, X14Namespace );

        //Vertical Axis Maximum Value
        if (sparklineGroup.VerticalAxisMaximum != null)
        {
            if (sparklineGroup.VerticalAxisMaximum.VerticalAxisOptions == SparklineVerticalAxisOptions.Custom)
              writer.WriteAttributeString( SparkConstants.VerticalMaxAttribute, sparklineGroup.VerticalAxisMaximum.CustomValue.ToString() );
        }
        //Vertical Axis Minimum Value
        if (sparklineGroup.VerticalAxisMinimum != null)
        {
            if (sparklineGroup.VerticalAxisMinimum.VerticalAxisOptions == SparklineVerticalAxisOptions.Custom)
              writer.WriteAttributeString( SparkConstants.VerticalMinAttribute, sparklineGroup.VerticalAxisMinimum.CustomValue.ToString() );
        }

        //Line Weight for the Sparkline Line type
        if (sparklineGroup.SparklineType == SparklineType.Line)
          writer.WriteAttributeString( SparkConstants.LineWeightAttribute, XmlConvert.ToString( sparklineGroup.LineWeight ) );

        //Sparkline Type
        switch (sparklineGroup.SparklineType)
        {
            case SparklineType.Line:
                //default Sparkline Type
                break;
            case SparklineType.Column:
                writer.WriteAttributeString( SparkConstants.SparklineTypeAttribute, SparklineColumnValue );
                break;
            case SparklineType.ColumnStacked100:
                writer.WriteAttributeString( SparkConstants.SparklineTypeAttribute, SparklineWinLossValue );
                break;
        }

        //Date Axis 
        if ((sparklineGroup.HorizontalDateAxis) && (sparklineGroup.HorizontalDateAxisRange != null))
          writer.WriteAttributeString( SparkConstants.DateAxisAttribute, TrueValue );

        //Display Empty Cells
        switch (sparklineGroup.DisplayEmptyCellsAs)
        {
            case SparklineEmptyCells.Gaps:
            writer.WriteAttributeString( SparkConstants.DisplayEmptyCellsAttribute, EmptyCellsGapValue );
                break;
            case SparklineEmptyCells.Line:
                if (sparklineGroup.SparklineType == SparklineType.Line)
                  writer.WriteAttributeString( SparkConstants.DisplayEmptyCellsAttribute, EmptyCellsLineValue );
                else
                  writer.WriteAttributeString( SparkConstants.DisplayEmptyCellsAttribute, EmptyCellsZeroValue );
                break;
            case SparklineEmptyCells.Zero:
                //default Display of Empty cells
                break;
        }

        //Show markers
        if ((sparklineGroup.ShowMarkers) && (sparklineGroup.SparklineType == SparklineType.Line))
          writer.WriteAttributeString( SparkConstants.MarkersAttribute, TrueValue );

        //Show High point
        if (sparklineGroup.ShowHighPoint)
            writer.WriteAttributeString(SparkConstants.HighAttribute, TrueValue);

        //Show Low point
        if (sparklineGroup.ShowLowPoint)
          writer.WriteAttributeString( SparkConstants.LowAttribute, TrueValue );

        //Show First point
        if (sparklineGroup.ShowFirstPoint)
          writer.WriteAttributeString( SparkConstants.FirstAttribute, TrueValue );

        //Show Last point
        if (sparklineGroup.ShowLastPoint)
          writer.WriteAttributeString( SparkConstants.LastAttribute, TrueValue );

        //Show Negative point
        if (sparklineGroup.ShowNegativePoint)
          writer.WriteAttributeString( SparkConstants.NegativeAttribute, TrueValue );

        //Display Axis
        if (sparklineGroup.DisplayAxis)
          writer.WriteAttributeString( SparkConstants.DisplayAxisAttribute, TrueValue );

        //Display Hidden Row and Columns
        if (sparklineGroup.DisplayHiddenRC)
          writer.WriteAttributeString( SparkConstants.DisplayHiddenAttribute, TrueValue );

        //Vertical Axis Maximum type
        if (sparklineGroup.VerticalAxisMaximum != null)
        {
            switch (sparklineGroup.VerticalAxisMaximum.VerticalAxisOptions)
            {
                case SparklineVerticalAxisOptions.Custom:
                writer.WriteAttributeString( SparkConstants.VerticalMaxAxisTypeAttr, VerticalCustomTypeValue );
                    break;
                case SparklineVerticalAxisOptions.Same:
                    writer.WriteAttributeString( SparkConstants.VerticalMaxAxisTypeAttr, VerticalSameTypeValue );
                    break;
                case SparklineVerticalAxisOptions.Automatic:
                    //default Sparkline VerticalAxis Maximum Type
                    break;
            }
        }
        //Vertical Axis Minimum Type.
        if (sparklineGroup.VerticalAxisMinimum != null)
        {
            switch (sparklineGroup.VerticalAxisMinimum.VerticalAxisOptions)
            {
                case SparklineVerticalAxisOptions.Custom:
                writer.WriteAttributeString( SparkConstants.VerticalMinAxisTypeAttr, VerticalCustomTypeValue );
                    break;
                case SparklineVerticalAxisOptions.Same:
                    writer.WriteAttributeString( SparkConstants.VerticalMinAxisTypeAttr, VerticalSameTypeValue );
                    break;
                case SparklineVerticalAxisOptions.Automatic:
                    //default Sparkline VerticalAxis Minimum Type
                    break;
            }
        }

        //Plot Right To Left.
        if (sparklineGroup.PlotRightToLeft)
            writer.WriteAttributeString(RightToLeft, TrueValue);

        //Sparkline Color
        writer.WriteStartElement( SparkConstants.ColorSeriesTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.SparklineColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //Negative Color
        writer.WriteStartElement( SparkConstants.ColorNegativeTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.NegativePointColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //Axis Color
        writer.WriteStartElement( SparkConstants.ColorAxisTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.AxisColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //Markers Color
        if (sparklineGroup.SparklineType == SparklineType.Line)
        {
          writer.WriteStartElement( SparkConstants.ColorMarkersTag, X14Namespace );
            colorObject = new ColorObject(sparklineGroup.MarkersColor);
            writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
            writer.WriteEndElement();
        }

        //First Point Color
        writer.WriteStartElement( SparkConstants.ColorFirstTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.FirstPointColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //Last Point Color
        writer.WriteStartElement( SparkConstants.ColorLastTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.LastPointColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //High Point Color
        writer.WriteStartElement( SparkConstants.ColorHighTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.HighPointColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //Low Point Color
        writer.WriteStartElement( SparkConstants.
          ColorLowTag, X14Namespace );
        colorObject = new ColorObject(sparklineGroup.LowPointColor);
        writer.WriteAttributeString(ColorRgbAttribute, colorObject.Value.ToString("X6"));
        writer.WriteEndElement();

        //Data Axis Range
        if (sparklineGroup.HorizontalDateAxisRange != null)
        {
            writer.WriteStartElement(FormulaTagName, MSNamespaceMain);
            writer.WriteString(sparklineGroup.HorizontalDateAxisRange.Address.Replace("'", ""));
            writer.WriteEndElement();
        }
        foreach (Sparklines sparklines in sparklineGroup)
        {
            SerializeSparklines(writer, sheet, sparklines);
        }
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes the sparklines.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sheet">The sheet.</param>
    /// <param name="sparklines">The sparklines.</param>
    private void SerializeSparklines(XmlWriter writer, WorksheetImpl sheet, Sparklines sparklines)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (sparklines == null)
            throw new ArgumentNullException("Sparlines");

        writer.WriteStartElement( SparkConstants.SparklinesTag, X14Namespace );
        foreach (Sparkline sparkline in sparklines)
        {
            SerializeSparkline(writer, sheet, sparkline);
        }
        writer.WriteEndElement();
    }

    /// <summary>
    /// Serializes the sparkline.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sheet">The sheet.</param>
    /// <param name="sparkline">The sparkline.</param>
    private void SerializeSparkline(XmlWriter writer, WorksheetImpl sheet, Sparkline sparkline)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (sparkline == null)
            throw new ArgumentNullException("sparkline");

        writer.WriteStartElement( SparkConstants.SparklineTag, X14Namespace );
        writer.WriteStartElement(FormulaTagName, MSNamespaceMain);
        writer.WriteString(sparkline.DataRange.AddressGlobal);
        writer.WriteEndElement();
        writer.WriteStartElement(RangeReferenceAttribute, MSNamespaceMain);
        writer.WriteString(sparkline.ReferenceRange.AddressLocal);
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    protected override void SerializeAppVersion( XmlWriter writer )
    {
      SerializeElementString( writer, DocProp.AppVersion, VersionValue, null );
    }

    /// <summary>
    /// Serializes conditional formattings.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="sheet">The sheet.</param>
    public void SerializeConditionalFormattings(XmlWriter writer, WorksheetImpl sheet)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (sheet.UsedRange.ConditionalFormats.Count == 0 && sheet.ConditionalFormats.Count == 0)
            return;       

            bool hasExtensionList = false;
            int iPriority = 1;
            string strRange = string.Empty;

            foreach (IConditionalFormats conditions in sheet.ConditionalFormats)
            {
                IConditionalFormat condition = conditions[conditions.Count-1];
                if (condition.DataBar != null && (condition.DataBar as DataBarImpl).HasExtensionList)
                {
                    hasExtensionList = true;
                    break;
                }
                else if ((condition as ConditionalFormatImpl).CFHasExtensionList && condition.FormatType== ExcelCFType.SpecificText)
                {
                    hasExtensionList = true;
                    break;
                }
            }

            if (hasExtensionList)
        {
            writer.WriteStartElement(Extensionlist, XmlNamespaceMain);
            writer.WriteStartElement(Vml.Ext, XmlNamespaceMain);
            writer.WriteAttributeString(SparkConstants.UriAttribute, DataBarExtUri);
            writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, X14Prefix, null, X14Namespace);

            writer.WriteStartElement(X14Prefix, CF.ConditionalFormattingsTagName, null);

                foreach (IConditionalFormats conditions in sheet.ConditionalFormats)
                {
                    IConditionalFormat condition = conditions[conditions.Count-1];
                    ConditionalFormatImpl format=(condition as ConditionalFormatImpl);
                    format.RangeRefernce = (conditions as ConditionalFormats).Address.ToString();

                    if (condition.FormatType == ExcelCFType.SpecificText && condition.FirstFormula!=null && (condition.Operator == ExcelComparisonOperator.BeginsWith ||
                          condition.Operator == ExcelComparisonOperator.EndsWith || condition.Operator == ExcelComparisonOperator.ContainsText
                          || condition.Operator == ExcelComparisonOperator.NotContainsText))
                    {
                        writer.WriteStartElement(X14Prefix, CF.ConditionalFormattingTagName,null);
                        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, MSPrefix,null, MSNamespaceMain);
                        writer.WriteStartElement(X14Prefix, CF.RuleTagName, null);
                        writer.WriteAttributeString(CF.TypeAttributeName, GetCFType(condition.FormatType,condition.Operator));
                        writer.WriteAttributeString(CF.PriorityAttributeName, iPriority.ToString ());
                        iPriority++;
                        writer.WriteAttributeString(CF.OperatorAttributeName, GetCFComparisonOperatorName(condition.Operator));
                        writer.WriteAttributeString(IdAttributeName, format.ST_GUID .ToString());
                        string str_formula = condition.FirstFormula;

                        if (condition.FirstFormula != null && condition.FirstFormula != string.Empty)
                        {
                            writer.WriteElementString(MSPrefix,FormulaTagName,null, str_formula.Replace("\"", string.Empty).Trim());
                        }
                        
                       if(format.Range!=null)
                           writer.WriteElementString(MSPrefix, FormulaTagName, null, (format.Range as RangeImpl).AddressGlobalWithoutSheetName);
                        else
                           writer.WriteElementString(MSPrefix, FormulaTagName, null, format.AsteriskRange);
                        
                        Excel2007Serializator serializator = new Excel2007Serializator(format.Workbook as WorkbookImpl);
                       
                      
                        writer.WriteStartElement(X14Prefix, DxfFormattingTagName,null );
                        SerializeDxfFont(writer, condition as IInternalConditionalFormat);
                        SerializeDxfFill(writer, condition as IInternalConditionalFormat);
                        SerializeDxfBorders(writer, condition as IInternalConditionalFormat);
                       
                        writer.WriteEndElement();
                        
                        writer.WriteEndElement();                       
                        writer.WriteElementString(MSPrefix, RangeReferenceAttribute, null, format.RangeRefernce);
                      
                        writer.WriteEndElement();

                    }
                    if (condition.DataBar != null && (condition.DataBar as DataBarImpl).HasExtensionList)
                    {
                        ExcelCFType cfType = condition.FormatType;
                    IDataBar dataBar = condition.DataBar;
                    DataBarImpl dataBarImpl = (dataBar as DataBarImpl);

                    writer.WriteStartElement(X14Prefix, CF.ConditionalFormattingTagName, null);
                    writer.WriteStartElement(X14Prefix, CF.RuleTagName, null);
                    writer.WriteAttributeString(CF.TypeAttributeName, CF.TypeDataBar);
                    writer.WriteAttributeString(IdAttributeName, dataBarImpl.ST_GUID.ToString());

                    writer.WriteStartElement(X14Prefix, CF.DataBarTag, null);
                    writer.WriteAttributeString(CF.BorderAttributeName, (dataBar.HasBorder) ? "1" : "0");
                    writer.WriteAttributeString(CF.GradientAttributeName, (dataBar.HasGradientFill) ? "1" : "0");
                    writer.WriteAttributeString(CF.DirectionAttributeName, dataBar.DataBarDirection.ToString());
                    writer.WriteAttributeString(CF.NegativeBarColorSameAsPositiveAttributeName, (dataBarImpl.HasDiffNegativeBarColor) ? "0" : "1");
                    writer.WriteAttributeString(CF.NegativeBarBorderColorSameAsPositiveAttributeName, (dataBarImpl.HasDiffNegativeBarBorderColor) ? "0" : "1");
                    writer.WriteAttributeString(CF.AxisPositionAttributeName, dataBar.DataBarAxisPosition.ToString());

                    SerializeConditionValueObject(writer, dataBar.MinPoint, false);
                    SerializeConditionValueObject(writer, dataBar.MaxPoint, false);
                    SerializeRgbColor(writer, CF.BorderColorTagName, dataBar.BorderColor);
                    SerializeRgbColor(writer, CF.NegativeFillColorTagName, dataBar.NegativeFillColor);
                    SerializeRgbColor(writer, CF.NegativeBorderColorTagName, dataBar.NegativeBorderColor);
                    SerializeRgbColor(writer, CF.AxisColorTagName, dataBar.BarAxisColor);

                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    //writer.WriteStartElement(X14Prefix, CF.DataBarTag, MSNamespaceMain);

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        else if (sheet.DataHolder.m_cfsStream != null && sheet.DataHolder.m_cfsStream.Length != 0)
        {
            writer.WriteStartElement(Extensionlist, XmlNamespaceMain);
            writer.WriteStartElement(Vml.Ext, XmlNamespaceMain);
            writer.WriteAttributeString(SparkConstants.UriAttribute, Excel2010Serializator.DataBarExtUri);
            writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, X14Prefix, null, X14Namespace);

            Excel2007Serializator.SerializeStream(writer, sheet.DataHolder.m_cfsStream, "root");

            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Serializes conditional value object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="conditionValue">Object to serialize.</param>
    public void SerializeConditionValueObject(XmlWriter writer, IConditionValue conditionValue, bool isIconSet)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        writer.WriteStartElement(X14Prefix, CF.ValueObjectTag, null);
        
        int index = (int)conditionValue.Type;
        string strType = CF.ValueTypes[index];
        writer.WriteAttributeString(CF.TypeAttributeName, strType);
        writer.WriteAttributeString(ValueAttributeName, conditionValue.Value);
        if (isIconSet)
            writer.WriteAttributeString(CF.GreaterAttribute, ((int)conditionValue.Operator).ToString());
        writer.WriteEndElement();
    }

    /// <summary>
    /// Serializes rgb color value.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Xml tag name to use for color serialization.</param>
    /// <param name="color">Color to serialize.</param>
    public void SerializeRgbColor(XmlWriter writer, string tagName, ColorObject color)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (tagName == null || tagName.Length == 0)
            throw new ArgumentOutOfRangeException("tagName");

        int value = color.Value;
        writer.WriteStartElement(X14Prefix, tagName, null);
        writer.WriteAttributeString(ColorRgbAttribute, value.ToString("X8"));
        SerializeAttribute(writer, ColorTintAttributeName, color.Tint, 0);
        writer.WriteEndElement();
    }

  }
}
