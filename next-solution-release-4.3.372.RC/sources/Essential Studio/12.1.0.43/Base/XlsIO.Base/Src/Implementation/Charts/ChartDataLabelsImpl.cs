#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.IO;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Interfaces.Charts;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#if ( WINRT )
using Windows.UI;
using Rectanlge = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Class used for Chart Data Labels implementation.
	/// </summary>
	public class ChartDataLabelsImpl
    : CommonObject
    , IChartDataLabels
    , ISerializable
    , IInternalChartTextArea
	{
    #region Class members
    /// <summary>
    /// Parent Series.
    /// </summary>
    private ChartSerieImpl m_serie;
    /// <summary>
    /// Text area.
    /// </summary>
    private ChartTextAreaImpl m_textArea;
    /// <summary>
    /// Parent data point;
    /// </summary>
    private ChartDataPointImpl m_dataPoint;
    ///// <summary>
    ///// Language used to display text.
    ///// </summary>
    //private string m_strLanguage;
    /// <summary>
    /// Represents Excel 2007 layout data
    /// </summary>
    private IChartLayout m_layout;
    /// <summary>
    /// boolean containing delete value
    /// </summary>
    private bool m_isDelete;
    /// <summary>
    /// Represents the TextArea Paragraph 
    /// </summary>
    private ChartParagraphType m_paraType;
    /// <summary>
    /// Represents the Default Fontname
    /// </summary>
    internal const string DEFAULT_FONTNAME = "Tahoma";
    /// <summary>
    /// Represents the Default Language
    /// </summary>
    internal const string DEFAULT_LANGUAGE = "en-US";
    /// <summary>
    /// Represents the Default size
    /// </summary>
    internal const double DEFAULT_FONTSIZE = 10;    
    /// <summary>
    /// Indicats whether to show text properties or not
    /// </summary>
    private bool m_bShowTextProperties = true;
    /// <summary>
    /// Indicats whether to show text size properties or not
    /// </summary>
    private bool m_bShowSizeProperties;
    /// <summary>
    /// Indicats whether to show text bold properties or not
    /// </summary>
    private bool m_bShowBoldProperties;
    internal bool m_bHasValueOption, m_bHasSeriesOption, m_bHasCategoryOption, m_bHasPercentageOption, m_bHasLegendKeyOption, m_bHasBubbleSizeOption;
    private string m_numberFormat;
    private bool m_bIsFormula;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of data label and sets its parent and application objects.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object</param>
    /// <param name="index">Data point index.</param>
    public ChartDataLabelsImpl( IApplication application, object parent, int index )
      : base( application, parent )
    {
      SetParents();
      m_textArea = new ChartWrappedTextAreaImpl(Application, this);
      //m_textArea = new ChartTextAreaImpl( Application, this );
      m_textArea.ObjectLink.DataPointNumber = ( ushort )index;
      m_textArea.TextRecord.IsAutoText = true;
      m_textArea.ChartAI.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;
      m_paraType = ChartParagraphType.Default;
      ChartSerieDataFormatImpl dataFormat = m_dataPoint.InnerDataFormat;

      if( dataFormat != null )
      {
        // We have to force formats creation to let MS Excel open resulting file without problems.
//        ChartMarkerFormatRecord marker = dataFormat.MarkerFormat;
//        IChartBorder border = dataFormat.LineProperties;
//        IChartInterior interior = dataFormat.AreaProperties;
//        ChartPieFormatRecord pie = dataFormat.PieFormat;

        // TODO: check whether we can comment the next line out
        //dataFormat.UpdateSerieFormat();
      }
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      object[] arrParents = FindParents(
        new Type[]{ typeof( ChartSerieImpl ), typeof( ChartDataPointImpl ) } );

      m_serie = arrParents[ 0 ] as ChartSerieImpl;

      if( m_serie == null )
      {
        throw new ArgumentNullException( "parent", "Can't find parent serie." );
      }

      m_dataPoint = arrParents[ 1 ] as ChartDataPointImpl;

      if( m_dataPoint == null )
      {
        throw new ArgumentNullException( "parent", "Can't find data point." );
      }
    }
    #endregion

    #region IChartDataLabels Members
    /// <summary>
    /// Indicates whether series name is in data labels.
    /// </summary>
    public bool IsSeriesName
    {
      get
      {
        return (m_bHasSeriesOption ? m_textArea.IsSeriesName :
               (m_serie.ParentBook as WorkbookImpl).Saving ? m_textArea.IsSeriesName :
               (this.Application.DefaultVersion != ExcelVersion.Excel2013) ? m_textArea.IsSeriesName : true);
      }
      set
      {
        m_textArea.IsSeriesName = value;
        m_bHasSeriesOption = true;
      }
    }
    /// <summary>
    /// Indicates whether category name is in data labels.
    /// </summary>
    public bool IsCategoryName
    {
      get
      {
        return (m_bHasCategoryOption ? m_textArea.IsCategoryName : 
               (m_serie.ParentBook as WorkbookImpl).Saving ? m_textArea.IsCategoryName : 
               (this.Application.DefaultVersion != ExcelVersion.Excel2013) ? m_textArea.IsCategoryName : true);
      }
      set
      {
        m_textArea.IsCategoryName = value;
        m_bHasCategoryOption = true;
        ChartSerieDataFormatImpl format = Format;

        if( format != null )
        {
          //if( m_serie.IsPie )
            format.AttachedLabel.ShowCategoryLabel = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether value is in data labels.
    /// </summary>
    public bool IsValue
    {
      get
      {
        return (m_bHasValueOption ? m_textArea.IsValue :
               (m_serie.ParentBook as WorkbookImpl).Saving ? m_textArea.IsValue :
               (this.Application.DefaultVersion != ExcelVersion.Excel2013) ? m_textArea.IsValue : true);
      }
      set
      {
        ChartSerieDataFormatImpl format = Format;
        m_bHasValueOption = true;
        if( format != null )//&& !IsBubble )
        {
          //if( !m_serie.IsPie )
            format.AttachedLabel.ShowActiveValue = value;
        }

        m_textArea.IsValue = value;
      }
    }

    /// <summary>
    /// Indicates whether percentage is in data labels.
    /// </summary>
    public bool IsPercentage
    {
      get
      {
        return (m_bHasPercentageOption ? m_textArea.IsPercentage :
               (m_serie.ParentBook as WorkbookImpl).Saving ? m_textArea.IsPercentage :
               (this.Application.DefaultVersion != ExcelVersion.Excel2013) ? m_textArea.IsPercentage : true);
      }
      set
      {
        m_textArea.IsPercentage = value;
        m_bHasPercentageOption = true;
        ChartSerieDataFormatImpl format = Format;

        if( format != null )
        {
          format.AttachedLabel.ShowPieInPercents = true;
          // Create attached labels:
          //ChartAttachedLabelRecord record = format.AttachedLabel;
        }
      }
    }

    /// <summary>
    /// Indicates whether bubble size is in data labels.
    /// </summary>
    public bool IsBubbleSize
    {
      get
      {
        return m_textArea.IsBubbleSize;
      }
      set
      {
        m_textArea.IsBubbleSize = value;
        m_bHasBubbleSizeOption = true;
        ChartSerieDataFormatImpl format = Format;

        if( format != null )
        {
          //format.AttachedLabel.ShowBubble = value;
        }
      }
    }

    /// <summary>
    /// Delimiter.
    /// </summary>
    public string Delimiter
    {
      get
      {
        return m_textArea.Delimiter;
      }
      set
      {
        m_textArea.Delimiter = value;
      }
    }

    /// <summary>
    /// Indicates whether legend key is in data labels.
    /// </summary>
    public bool IsLegendKey
    {
      get
      {
        return (m_bHasLegendKeyOption ? m_textArea.IsLegendKey :
               (m_serie.ParentBook as WorkbookImpl).Saving ? m_textArea.IsLegendKey :
               (this.Application.DefaultVersion != ExcelVersion.Excel2013) ? m_textArea.IsLegendKey : true);
      }
      set
      {
        m_textArea.IsLegendKey = value;
        m_bHasLegendKeyOption = true;
      }
    }
    /// <summary>
    /// Indicates whether Leader Lines is in data labels.
    /// </summary>
    public bool ShowLeaderLines
    {
      get
      {
        return m_serie.InnerChart.ChartFormat.ShowLeaderLines;
      }
      set
      {
          //if (!(IsSeriesName || IsPercentage || IsValue || IsCategoryName))
          //    throw new ApplicationException("Set IsCategoryName, IsPercentage, IsSeriesName or IsValue to enable this property");

        ChartFormatImpl format = m_serie.InnerChart.ChartFormat;
        format.ShowLeaderLines = value;
      }
    }
    /// <summary>
    /// Represents data labels position.
    /// </summary>
    public ExcelDataLabelPosition Position
    {
      get
      {
        return m_textArea.Position;
      }
      set
      {
        m_textArea.Position = value;
      }
    }
    #endregion

    #region IChartTextArea Members
    /// <summary>
    /// Display mode of the background.
    /// </summary>
    public ExcelChartBackgroundMode BackgroundMode
    {
      get
      {
        return m_textArea.BackgroundMode;
      }
      set
      {
        m_textArea.BackgroundMode = value;
      }
    }
    /// <summary>
    /// True if background is set to automatic.
    /// </summary>
    public bool IsAutoMode
    {
      get
      {
        return m_textArea.IsAutoMode;
      }
      set
      {
        m_textArea.IsAutoMode = value;
      }
    }
    /// <summary>
    /// Area's text.
    /// </summary>
    public string Text
    {
      get
      {
        return m_textArea.Text;
      }
      set
      {
        m_textArea.Text = value;
      }
    }
    /// <summary>
    /// Gets rich text.
    /// </summary>
    public IChartRichTextString RichText
    {
        get
        {
            return TextArea.RichText;
        }
    }
    /// <summary>
    /// Text rotation angle.
    /// </summary>
    public int TextRotationAngle
    {
      get
      {
        return m_textArea.TextRotationAngle;
      }
      set
      {
        m_textArea.TextRotationAngle = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// Return format of the text area.
    /// </summary>
    public IChartFrameFormat FrameFormat
    {
      get
      {
        return m_textArea.FrameFormat;
      }
    }

    #endregion

    #region IFont Members
    /// <summary>
    /// True if the font is bold. Read / write Boolean.
    /// </summary>
    public bool Bold
    {
      get
      {
        return m_textArea.Bold;
      }
      set
      {
        m_textArea.Bold = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read / write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return m_textArea.Color;
      }
      set
      {
        m_textArea.Color = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// Gets / sets font color. Searches for the closest color in 
    /// the workbook palette.
    /// </summary>
    public Color RGBColor
    {
      get
      {
        return m_textArea.RGBColor;
      }
      set
      {
        m_textArea.RGBColor = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// True if the font style is italic. Read / write Boolean.
    /// </summary>
    public bool Italic
    {
      get
      {
        return m_textArea.Italic;
      }
      set
      {
        m_textArea.Italic = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// True if the font is an outline font. Read / write Boolean.
    /// </summary>
    public bool MacOSOutlineFont
    {
      get
      {
        return m_textArea.MacOSOutlineFont;
      }
      set
      {
        m_textArea.MacOSOutlineFont = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// True if the font is a shadow font or if the object has
    /// a shadow. Read / write Boolean.
    /// </summary>
    public bool MacOSShadow
    {
      get
      {
        return m_textArea.MacOSShadow;
      }
      set
      {
        m_textArea.MacOSShadow = value;
      }
    }

    /// <summary>
    /// Returns or sets the size of the font. Read / write Variant.
    /// </summary>
    public double Size
    {
      get
      {
        return m_textArea.Size;
      }
      set
      {
        m_textArea.Size = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// True if the font is struck through with a horizontal line.
    /// Read / write Boolean
    /// </summary>
    public bool Strikethrough
    {
      get
      {
        return m_textArea.Strikethrough;
      }
      set
      {
        m_textArea.Strikethrough = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// True if the font is formatted as subscript.
    /// False by default. Read / write Boolean.
    /// </summary>
    public bool Subscript
    {
      get
      {
        return m_textArea.Subscript;
      }
      set
      {
        m_textArea.Subscript = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// True if the font is formatted as superscript. False by default.
    /// Read/write Boolean
    /// </summary>
    public bool Superscript
    {
      get
      {
        return m_textArea.Superscript;
      }
      set
      {
        m_textArea.Superscript = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// Returns or sets the type of underline applied to the font. Can
    /// be one of the following ExcelUnderlineStyle constants.
    /// Read / write ExcelUnderline.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        return m_textArea.Underline;
      }
      set
      {
        m_textArea.Underline = value;
        this.ShowTextProperties = true;
      }
    }

    /// <summary>
    /// Returns or sets the font name. Read / write string.
    /// </summary>
    public string FontName
    {
      get
      {
        return m_textArea.FontName;
      }
      set
      {
        m_textArea.FontName = value;
        this.ShowTextProperties = true;
      }
    }
    /// <summary>
    /// Gets / sets font vertical alignment.
    /// </summary>
    public ExcelFontVertialAlignment VerticalAlignment
    {
      get
      {
        return m_textArea.VerticalAlignment;
      }
      set
      {
        m_textArea.VerticalAlignment = value;
        this.ShowTextProperties = true;
      }
    }
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont()
    {
      return m_textArea.GenerateNativeFont();
    }
    /// <summary>
    /// Indicates whether color is automatically selected. Read-only.
    /// </summary>
    public bool IsAutoColor
    {
      get
      {
        return m_textArea.IsAutoColor;
      }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Saves object into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_textArea.ContainDataLabels )
        m_textArea.IsShowLabelPercent = ( m_serie.IsPie && IsPercentage && IsCategoryName && !IsValue && !IsSeriesName );

      SetObjectLink();

      bool bCondition = !IsValue && IsCategoryName;

      if( bCondition )
      {
        m_textArea.TextRecord.IsShowLabel = true;
      }

      m_textArea.Serialize( records );

      if( bCondition )
      {
        m_textArea.TextRecord.IsShowLabel = false;
      }
    }
    /// <summary>
    /// Fill object link record.
    /// </summary>
    private void SetObjectLink()
    {
      ChartObjectLinkRecord objectLink = m_textArea.ObjectLink;
      objectLink.DataPointNumber = ( ushort )m_dataPoint.Index;
      objectLink.LinkObject = ExcelObjectTextLink.DataLabel;

      ChartSerieImpl series = FindParent( typeof( ChartSerieImpl ) ) as ChartSerieImpl;

      if( series == null )
      {
        throw new NotImplementedException( "Can't find parent series" );
      }

      objectLink.SeriesNumber = ( ushort )series.Index;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets text area.
    /// </summary>
    public ChartTextAreaImpl TextArea
    {
      get
      {
        return m_textArea;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_textArea = value;
      }
    }
    /// <summary>
    /// Gets corresponding data format. Read-only.
    /// </summary>
    public ChartSerieDataFormatImpl Format
    {
      get
      {
        return m_dataPoint.InnerDataFormat;
      }
    }
    /// <summary>
    /// Gets or sets Excel 2007 layout data
    /// </summary>
    public IChartLayout Layout
    {
        get
        {
            if (m_layout == null)
                m_layout = new ChartLayoutImpl(Application, this, this.Parent);

            return m_layout;
        }
        set
        {
            m_layout = value;
        }
    }
    /// <summary>
    /// Gets or sets whether to delete or not.
    /// </summary>
    internal bool IsDelete
    {
        get
        {
            return m_isDelete;
        }
        set
        {
            m_isDelete = value;
        }
    }
    /// <summary>
    /// Gets value indicating whether TextRotation was changed. Read-only.
    /// </summary>
    public bool HasTextRotation
    {
      get
      {
        return TextArea.HasTextRotation;
      }
    }
    /// <summary>
    /// Represents the Legend Paragraph 
    /// </summary>
    public ChartParagraphType ParagraphType
    {
        get
        {
            if(m_paraType != ChartParagraphType .CustomDefault )
            {
                ChartParserCommon.CheckDefaultSettings(TextArea);
                m_paraType = TextArea.ParagraphType;
            }
            return m_paraType;
        }
        set
        {
            m_paraType = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is formula.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is formula; otherwise, <c>false</c>.
    /// </value>
    public string NumberFormat
    {
        get
        {
            return m_numberFormat;
        }
        set
        {
            m_numberFormat = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is formula.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is formula; otherwise, <c>false</c>.
    /// </value>
    public bool IsFormula
    {
        get
        {
            return m_bIsFormula;
        }
        set
        {
            m_bIsFormula = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show text properties or not
    /// </summary>
    internal bool ShowTextProperties
    {
        get
        {
            return m_bShowTextProperties;
        }
        set
        {
            m_bShowTextProperties = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show text size properties or not
    /// </summary>
    internal bool ShowSizeProperties
    {
        get
        {
            return m_bShowSizeProperties;
        }
        set
        {
            m_bShowSizeProperties = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show text bold properties or not
    /// </summary>
    internal bool ShowBoldProperties
    {
        get
        {
            return m_bShowBoldProperties;
        }
        set
        {
            m_bShowBoldProperties = value;
        }
    }
    /// <summary>
    /// Gets or sets the Text Rotation
    /// </summary>
    public Excel2007TextRotation TextRotation
    {
        get
        {
            return m_textArea.TextRotation;
        }
        set
        {
            m_textArea.TextRotation = value;
        }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Updates Series index.
    /// </summary>
    public void UpdateSerieIndex()
    {
      m_textArea.UpdateSerieIndex( m_serie.Index );
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <param name="dicFontIndexes">New font indexes.</param>
    /// <param name="dicNewSheetNames">New worksheet names.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent, Dictionary<int, int> dicFontIndexes, Dictionary<string, string> dicNewSheetNames )
    {
      ChartDataLabelsImpl result = ( ChartDataLabelsImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_textArea = ( ChartTextAreaImpl )m_textArea.Clone( result, dicFontIndexes, dicNewSheetNames );

      return result;
    }
    #endregion

    #region IOptimizedUpdate Members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      m_textArea.BeginUpdate();
    }

    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      m_textArea.EndUpdate();
    }

    #endregion

    #region IInternalChartTextArea Members
    /// <summary>
    /// Data labels color. Read-only.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_textArea.ColorObject;
      }
    }
    /// <summary>
    /// Returns font index. Read-only.
    /// </summary>
    public int Index
    {
      get
      {
        return m_textArea.Index;
      }
    }
    /// <summary>
    /// Returns FontImpl for current font. Read-only.
    /// </summary>
    public FontImpl Font
    {
      get
      {
        return m_textArea.Font;
      }
    }
    #endregion
  }
}
