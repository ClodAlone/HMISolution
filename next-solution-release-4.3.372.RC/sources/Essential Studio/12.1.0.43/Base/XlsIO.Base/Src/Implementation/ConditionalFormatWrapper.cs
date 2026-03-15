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

#region file using directives
using System;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;


#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for ConditionalFormatWrapper.
	/// </summary>
	public class ConditionalFormatWrapper
    : CommonWrapper
    , IInternalConditionalFormat
    , IConditionalFormat
	{
    #region Class members
    /// <summary>
    /// Parent conditional formats wrapper.
    /// </summary>
    private CondFormatCollectionWrapper m_formats;
    /// <summary>
    /// Condition index.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Wrapper over data bar object.
    /// </summary>
    private DataBarWrapper m_dataBar;
    /// <summary>
    /// Wrapper over icon set object.
    /// </summary>
    private IconSetWrapper m_iconSet;
    /// <summary>
    /// Wrapper over color scale object.
    /// </summary>
    private ColorScaleWrapper m_colorScale;
    private IRange m_range;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent user from creation instances of this class without arguments.
    /// </summary>
    private ConditionalFormatWrapper()
    {
    }
    /// <summary>
    /// Creates new instance of the wrapper.
    /// </summary>
    /// <param name="formats">Parent formats collection.</param>
    /// <param name="iIndex">Condition index.</param>
    public ConditionalFormatWrapper( CondFormatCollectionWrapper formats, int iIndex )
    {
      if( formats == null )
        throw new ArgumentNullException( "formats" );

      m_formats = formats;

      if( iIndex < 0 || iIndex >= formats.Count )
        throw new ArgumentOutOfRangeException( "iIndex" );

      m_iIndex = iIndex;
    }
    #endregion

    #region IConditionalFormat Members
    /// <summary>
    /// Type of the conditional format.
    /// </summary>
    public ExcelCFType FormatType
    {
      get
      {
        return GetCondition().FormatType;
      }
      set
      {
        BeginUpdate();
        GetCondition().FormatType = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Represents the type of time period.
    /// </summary>
    public CFTimePeriods TimePeriodType
    {
        get
        {
            return GetCondition().TimePeriodType; ;
        }
        set
        {
            BeginUpdate();
            GetCondition().TimePeriodType = value;
            EndUpdate();
        }
    }

    /// <summary>
    /// Type of the comparison operator.
    /// </summary>
    public ExcelComparisonOperator Operator
    {
      get
      {
        return GetCondition().Operator;
      }
      set
      {
        BeginUpdate();
        GetCondition().Operator = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Indicates whether the font is bold.
    /// </summary>
    public bool IsBold
    {
      get
      {
        return GetCondition().IsBold;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsBold = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Indicates whether font is italic.
    /// </summary>
    public bool IsItalic
    {
      get
      {
        return GetCondition().IsItalic;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsItalic = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Font color.
    /// </summary>
    public ExcelKnownColors FontColor
    {
      get
      {
        return GetCondition().FontColor;
      }
      set
      {
        BeginUpdate();
        GetCondition().FontColor = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Font color.
    /// </summary>
    public Color FontColorRGB
    {
      get
      {
        return GetCondition().FontColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().FontColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Underline type.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        return GetCondition().Underline;
      }
      set
      {
        BeginUpdate();
        GetCondition().Underline = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Indicates whether font is struck through.
    /// </summary>
    public bool IsStrikeThrough
    {
      get
      {
        return GetCondition().IsStrikeThrough;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsStrikeThrough = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the left line.
    /// </summary>
    public ExcelKnownColors LeftBorderColor
    {
      get
      {
        return GetCondition().LeftBorderColor;
      }
      set
      {
        BeginUpdate();
        GetCondition().LeftBorderColor = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the left line.
    /// </summary>
    public Color LeftBorderColorRGB
    {
      get
      {
        return GetCondition().LeftBorderColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().LeftBorderColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Left border line style.
    /// </summary>
    public ExcelLineStyle LeftBorderStyle
    {
      get
      {
        return GetCondition().LeftBorderStyle;
      }
      set
      {
        BeginUpdate();
        GetCondition().LeftBorderStyle = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the right line.
    /// </summary>
    public ExcelKnownColors RightBorderColor
    {
      get
      {
        return GetCondition().RightBorderColor;
      }
      set
      {
        BeginUpdate();
        GetCondition().RightBorderColor = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the right line.
    /// </summary>
    public Color RightBorderColorRGB
    {
      get
      {
        return GetCondition().RightBorderColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().RightBorderColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Right border line style.
    /// </summary>
    public ExcelLineStyle RightBorderStyle
    {
      get
      {
        return GetCondition().RightBorderStyle;
      }
      set
      {
        BeginUpdate();
        GetCondition().RightBorderStyle = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the top line.
    /// </summary>
    public ExcelKnownColors TopBorderColor
    {
      get
      {
        return GetCondition().TopBorderColor;
      }
      set
      {
        BeginUpdate();
        GetCondition().TopBorderColor = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the top line
    /// </summary>
    public Color TopBorderColorRGB
    {
      get
      {
        return GetCondition().TopBorderColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().TopBorderColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Top border line style.
    /// </summary>
    public ExcelLineStyle TopBorderStyle
    {
      get
      {
        return GetCondition().TopBorderStyle;
      }
      set
      {
        BeginUpdate();
        GetCondition().TopBorderStyle = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the bottom line.
    /// </summary>
    public ExcelKnownColors BottomBorderColor
    {
      get
      {
        return GetCondition().BottomBorderColor;
      }
      set
      {
        BeginUpdate();
        GetCondition().BottomBorderColor = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Color of the bottom line
    /// </summary>
    public Color BottomBorderColorRGB
    {
      get
      {
        return GetCondition().BottomBorderColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().BottomBorderColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Bottom border line style.
    /// </summary>
    public ExcelLineStyle BottomBorderStyle
    {
      get
      {
        return GetCondition().BottomBorderStyle;
      }
      set
      {
        BeginUpdate();
        GetCondition().BottomBorderStyle = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// First formula.
    /// </summary>
    public string FirstFormula
    {
      get
      {
        GetCondition().Range = m_range;
        return GetCondition().FirstFormula;
      }
      set
      {
        BeginUpdate();
        GetCondition().FirstFormula = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// First formula in R1C1 notation. Read-only.
    /// </summary>
    public string FirstFormulaR1C1
    {
      get
      {
        return GetCondition().FirstFormulaR1C1;
      }
        set
        {
            BeginUpdate();
            GetCondition().Range= m_range;
            GetCondition().FirstFormulaR1C1 = value;
            EndUpdate();
        }
    }

    /// <summary>
    /// Second formula.
    /// </summary>
    public string SecondFormula
    {
      get
      {
        return GetCondition().SecondFormula;
      }
      set
      {
        BeginUpdate();
        GetCondition().SecondFormula = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Second formula in R1C1 notation. Read-only.
    /// </summary>
    public string SecondFormulaR1C1
    {
      get
      {
        return GetCondition().SecondFormulaR1C1;
      }
        set
        {
            BeginUpdate();
            GetCondition().Range = m_range;
            GetCondition().SecondFormulaR1C1 = value;
            EndUpdate();
        }
    }

    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return GetCondition().Color;
      }
      set
      {
        BeginUpdate();
        GetCondition().Color = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        return GetCondition().ColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().ColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Pattern background color.
    /// </summary>
    public ExcelKnownColors BackColor
    {
      get
      {
        return GetCondition().BackColor;
      }
      set
      {
        BeginUpdate();
        GetCondition().BackColor = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Pattern background color.
    /// </summary>
    public Color BackColorRGB
    {
      get
      {
        return GetCondition().BackColorRGB;
      }
      set
      {
        BeginUpdate();
        GetCondition().BackColorRGB = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Fill pattern style.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        return GetCondition().FillPattern;
      }
      set
      {
        BeginUpdate();
        GetCondition().FillPattern = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Indicates whether font is superscript.
    /// </summary>
    public bool IsSuperScript
    {
      get
      {
        return GetCondition().IsSuperScript;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsSuperScript = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// Indicates whether font is subscript.
    /// </summary>
    public bool IsSubScript
    {
      get
      {
        return GetCondition().IsSubScript;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsSubScript = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// True if contains font formatting.
    /// </summary>
    public bool IsFontFormatPresent
    {
      get
      {
        return GetCondition().IsFontFormatPresent;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsFontFormatPresent = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// True if contains border formatting.
    /// </summary>
    public bool IsBorderFormatPresent
    {
      get
      {
        return GetCondition().IsBorderFormatPresent;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsBorderFormatPresent = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// True if contains pattern formatting.
    /// </summary>
    public bool IsPatternFormatPresent
    {
      get
      {
        return GetCondition().IsPatternFormatPresent;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsPatternFormatPresent = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// If true - format color present. otherwise - false.
    /// </summary>
    public bool IsFontColorPresent
    {
      get
      {
        return GetCondition().IsFontColorPresent;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsFontColorPresent = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// If true - pattern color present, otherwise - false.
    /// </summary>
    public bool IsPatternColorPresent
    {
      get
      {
        return GetCondition().IsPatternColorPresent;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsPatternColorPresent = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// If true - background color present. otherwise - false.
    /// </summary>
    public bool IsBackgroundColorPresent
    {
      get
      {
        return GetCondition().IsBackgroundColorPresent;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsBackgroundColorPresent = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// If true - number format present. otherwise - false.
    /// </summary>
    public bool HasNumberFormatPresent
    {
        get
        {
            return GetCondition().HasNumberFormatPresent;
        }
        set
        {
            BeginUpdate();
            GetCondition().HasNumberFormatPresent = value;
            EndUpdate();
        }
    }

    /// <summary>
    /// True if left border style and color are modified.
    /// </summary>
    public bool IsLeftBorderModified
    {
      get
      {
        return GetCondition().IsLeftBorderModified;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsLeftBorderModified = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// True if right border style and color modified.
    /// </summary>
    public bool IsRightBorderModified
    {
      get
      {
        return GetCondition().IsRightBorderModified;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsRightBorderModified = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// True if top border style and color are modified.
    /// </summary>
    public bool IsTopBorderModified
    {
      get
      {
        return GetCondition().IsTopBorderModified;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsTopBorderModified = value;
        EndUpdate();
      }
    }

    /// <summary>
    /// True if bottom border style and color are modified.
    /// </summary>
    public bool IsBottomBorderModified
    {
      get
      {
        return GetCondition().IsBottomBorderModified;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsBottomBorderModified = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Number format index.
    /// </summary>
    public ushort NumberFormatIndex
    {
        get
        {
            return GetCondition().NumberFormatIndex;
        }
        set
        {
            BeginUpdate();
            GetCondition().NumberFormatIndex = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Returns or sets the format code for the object. Read / write String.
    /// </summary>
    public string NumberFormat
    {
        get
        {
            return GetCondition().NumberFormat;
        }
        set
        {            
            BeginUpdate();
            GetCondition().NumberFormat = value;
            EndUpdate();            
        }
    }

    /// <summary>
    /// The text value in a Specific Text conditional formatting rule. 
    /// Valid only for type =contains Text, notContainsText,beginsWith,endsWith. The default value is null.
    /// </summary>
    public string Text
    {
        get
        {
            return GetCondition().Text;
        }
        set
        {
            BeginUpdate();
            GetCondition().Text = value;
            EndUpdate();
        }
    }
    ///<summary>
    /// Lower priority conditional formatting rules are evaluated.
    ///</summary>
    public bool StopIfTrue
    {
        get
        {
            return GetCondition().StopIfTrue;
        }
        set
        {
            GetCondition().StopIfTrue = value;
        }
    }
    ///<summary>
    /// Conditionl format template.
    ///</summary>
    public ConditionalFormatTemplate Template
    {
        get
        {
            return GetCondition().Template;
        }
        set
        {
            GetCondition().Template = value;
        }
    }
    /// <summary>
    /// Returns data bar settings. Valid only if FormatType is set to DataBar. Read-only.
    /// </summary>
    public IDataBar DataBar
    {
      get
      {
        if( FormatType == ExcelCFType.DataBar )
        {
          if( m_dataBar == null )
          {
            m_dataBar = new DataBarWrapper( GetCondition().DataBar as DataBarImpl, this );
          }
        }
        else
        {
          m_dataBar = null;
        }

        return m_dataBar;
      }
    }
    /// <summary>
    /// Returns iconset settings. Valid only if FormatType is set to IconSet. Read-only.
    /// </summary>
    public IIconSet IconSet
    {
      get
      {
        if( FormatType == ExcelCFType.IconSet )
        {
          if( m_iconSet == null )
          {
            m_iconSet = new IconSetWrapper( this );
          }
        }
        else
        {
          m_iconSet = null;
        }

        return m_iconSet;
      }
    }
    /// <summary>
    /// Returns color scale settings. Valid only if FormatType is set to ColorScale. Read-only.
    /// </summary>
    public IColorScale ColorScale
    {
      get
      {
        if( FormatType == ExcelCFType.ColorScale )
        {
          if( m_colorScale == null )
          {
            m_colorScale = new ColorScaleWrapper( this );
          }
        }
        else
        {
          m_colorScale = null;
        }

        return m_colorScale;
      }
    }

    internal IRange Range
    {
        get
        {
            return m_range;
        }
        set
        {
            m_range = value;
        }

    }
    #endregion

    #region IParentApplication Members
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_formats.Application;
      }
    }

    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_formats;
      }
    }

    #endregion

    #region IOptimizedUpdate Members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {
        m_formats.BeginUpdate();
      }

      base.BeginUpdate();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        m_formats.EndUpdate();
      }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Returns unwrapped condition.
    /// </summary>
    /// <returns>Unwrapped condition.</returns>
    internal ConditionalFormatImpl GetCondition()
    {
      return m_formats.GetCondition( m_iIndex );
    }
    #endregion

    #region IInternalConditionalFormat Members
    /// <summary>
    /// Conditional format color. Read-only.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Conditional format background color. Read-only.
    /// </summary>
    public ColorObject BackColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Conditional format top border color. Read-only.
    /// </summary>
    public ColorObject TopBorderColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Conditional format bottom border color. Read-only.
    /// </summary>
    public ColorObject BottomBorderColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Conditional format left border color. Read-only.
    /// </summary>
    public ColorObject LeftBorderColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Conditional format right border color. Read-only.
    /// </summary>
    public ColorObject RightBorderColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Conditional format font color. Read-only.
    /// </summary>
    public ColorObject FontColorObject
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates whether pattern style was modified.
    /// </summary>
    public bool IsPatternStyleModified
    {
      get
      {
        return GetCondition().IsPatternStyleModified;
      }
      set
      {
        BeginUpdate();
        GetCondition().IsPatternStyleModified = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns parsed tokens of the first formula.
    /// </summary>
    Ptg[] IInternalConditionalFormat.FirstFormulaPtgs
    {
      get
      {
        return ( GetCondition() as IInternalConditionalFormat ).FirstFormulaPtgs;
      }
    }
    /// <summary>
    /// Returns parsed tokens of the second formula.
    /// </summary>
    Ptg[] IInternalConditionalFormat.SecondFormulaPtgs
    {
      get
      {
        return ( GetCondition() as IInternalConditionalFormat ).SecondFormulaPtgs;
      }
    }
    #endregion
  }
}
