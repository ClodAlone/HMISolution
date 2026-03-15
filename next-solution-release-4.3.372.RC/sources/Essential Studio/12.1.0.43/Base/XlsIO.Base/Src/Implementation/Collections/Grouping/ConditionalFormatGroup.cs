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


namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for ConditionalFormatGroup.
	/// </summary>
	public class ConditionalFormatGroup
    : CommonObject
    , IConditionalFormat
	{
    #region Class members
    /// <summary>
    /// Index of the format in the collection.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Parent formats group.
    /// </summary>
    private ConditionalFormatsGroup m_formats;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    /// <param name="index">Format index.</param>
    public ConditionalFormatGroup( IApplication application, object parent, int index )
      : base( application, parent )
    {
      FindParents();
      m_iIndex = index;
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Synchronizes all conditional formats collection.
    /// </summary>
    private void SynchronizeParentCollection()
    {
      throw new NotImplementedException();
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
        return m_formats[ 0 ][ m_iIndex ].FormatType;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.FormatType setter implementation
      }
    }

    /// <summary>
    /// Type of the comparison operator.
    /// </summary>
    public ExcelComparisonOperator Operator
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].Operator;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.Operator setter implementation
      }
    }
    /// <summary>
    /// Type of time period.
    /// </summary>
    public CFTimePeriods TimePeriodType
    {
        get
        {
            return m_formats[0][m_iIndex].TimePeriodType;
        }
        set
        {
            
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
            return m_formats[0][m_iIndex].Text;
        }
        set
        {

        }
    }

    /// <summary>
    /// Indicates whether the font is bold.
    /// </summary>
    public bool IsBold
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsBold;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsBold setter implementation
      }
    }

    /// <summary>
    /// Indicates whether font is italic.
    /// </summary>
    public bool IsItalic
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsItalic;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsItalic setter implementation
      }
    }

    /// <summary>
    /// Font color.
    /// </summary>
    public ExcelKnownColors FontColor
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].FontColor;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.FontColor setter implementation
      }
    }

    /// <summary>
    /// Font color.
    /// </summary>
    public Color FontColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].FontColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.FontColorRGB setter implementation
      }
    }

    /// <summary>
    /// Underline type.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].Underline;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.Underline setter implementation
      }
    }

    /// <summary>
    /// Indicates whether font is struck through.
    /// </summary>
    public bool IsStrikeThrough
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsStrikeThrough;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsStrikeThrough setter implementation
      }
    }

    /// <summary>
    /// Color of the left line.
    /// </summary>
    public ExcelKnownColors LeftBorderColor
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].LeftBorderColor;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.LeftBorderColor setter implementation
      }
    }

    /// <summary>
    /// Color of the left line.
    /// </summary>
    public Color LeftBorderColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].LeftBorderColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.LeftBorderColorRGB setter implementation
      }
    }

    /// <summary>
    /// Left border line style.
    /// </summary>
    public ExcelLineStyle LeftBorderStyle
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].LeftBorderStyle;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.LeftBorderStyle setter implementation
      }
    }

    /// <summary>
    /// Color of the right line.
    /// </summary>
    public ExcelKnownColors RightBorderColor
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].RightBorderColor;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.RightBorderColor setter implementation
      }
    }

    /// <summary>
    /// Color of the right line.
    /// </summary>
    public Color RightBorderColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].RightBorderColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.RightBorderColorRGB setter implementation
      }
    }

    /// <summary>
    /// Right border line style.
    /// </summary>
    public ExcelLineStyle RightBorderStyle
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].RightBorderStyle;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.RightBorderStyle setter implementation
      }
    }

    /// <summary>
    /// Color of the top line.
    /// </summary>
    public ExcelKnownColors TopBorderColor
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].TopBorderColor;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.TopBorderColor setter implementation
      }
    }

    /// <summary>
    /// Color of the top line
    /// </summary>
    public Color TopBorderColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].TopBorderColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.TopBorderColorRGB setter implementation
      }
    }

    /// <summary>
    /// Top border line style.
    /// </summary>
    public ExcelLineStyle TopBorderStyle
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].TopBorderStyle;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.TopBorderStyle setter implementation
      }
    }

    /// <summary>
    /// Color of the bottom line.
    /// </summary>
    public ExcelKnownColors BottomBorderColor
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].BottomBorderColor;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.BottomBorderColor setter implementation
      }
    }

    /// <summary>
    /// Color of the bottom line
    /// </summary>
    public Color BottomBorderColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].BottomBorderColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.BottomBorderColorRGB setter implementation
      }
    }

    /// <summary>
    /// Bottom border line style.
    /// </summary>
    public ExcelLineStyle BottomBorderStyle
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].BottomBorderStyle;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.BottomBorderStyle setter implementation
      }
    }

    /// <summary>
    /// First formula.
    /// </summary>
    public string FirstFormula
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].FirstFormula;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.FirstFormula setter implementation
      }
    }

    /// <summary>
    /// First formula in R1C1 notation. Read-only.
    /// </summary>
    public string FirstFormulaR1C1
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].FirstFormulaR1C1;
      }
        set
        {
            m_formats[0][m_iIndex].FirstFormulaR1C1 = value;
        }
    }
    /// <summary>
    /// Second formula.
    /// </summary>
    public string SecondFormula
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].SecondFormula;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.SecondFormula setter implementation
      }
    }

    /// <summary>
    /// Second formula in R1C1 notation. Read-only.
    /// </summary>
    public string SecondFormulaR1C1
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].SecondFormulaR1C1;
      }
        set
        {
            m_formats[0][m_iIndex].SecondFormulaR1C1 = value;
        }
    }
    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].Color;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.Color setter implementation
      }
    }

    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    public Color ColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].ColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.ColorRGB setter implementation
      }
    }

    /// <summary>
    /// Pattern background color.
    /// </summary>
    public ExcelKnownColors BackColor
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].BackColor;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.BackColor setter implementation
      }
    }

    /// <summary>
    /// Pattern background color.
    /// </summary>
    public Color BackColorRGB
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].BackColorRGB;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.BackColorRGB setter implementation
      }
    }

    /// <summary>
    /// Fill pattern style.
    /// </summary>
    public ExcelPattern FillPattern
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].FillPattern;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.FillPattern setter implementation
      }
    }

    /// <summary>
    /// Indicates whether font is superscript.
    /// </summary>
    public bool IsSuperScript
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsSuperScript;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSuperScript setter implementation
      }
    }

    /// <summary>
    /// Indicates whether font is subscript.
    /// </summary>
    public bool IsSubScript
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsSubScript;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// True if contains font formatting.
    /// </summary>
    public bool IsFontFormatPresent
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsFontFormatPresent;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// True if contains border formatting.
    /// </summary>
    public bool IsBorderFormatPresent
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsBorderFormatPresent;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }

    /// <summary>
    /// True if contains pattern formatting.
    /// </summary>
    public bool IsPatternFormatPresent
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsPatternFormatPresent;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }

    /// <summary>
    /// If true - format color present. otherwise - false.
    /// </summary>
    public bool IsFontColorPresent
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsFontColorPresent;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// If true - pattern color present, otherwise - false.
    /// </summary>
    public bool IsPatternColorPresent
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsBackgroundColorPresent;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// If true - background color present. otherwise - false.
    /// </summary>
    public bool IsBackgroundColorPresent
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsBackgroundColorPresent;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// True if left border style and color are modified.
    /// </summary>
    public bool IsLeftBorderModified
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsLeftBorderModified;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }

    /// <summary>
    /// True if right border style and color modified.
    /// </summary>
    public bool IsRightBorderModified
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsRightBorderModified;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// True if top border style and color are modified.
    /// </summary>
    public bool IsTopBorderModified
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsTopBorderModified;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// True if bottom border style and color are modified.
    /// </summary>
    public bool IsBottomBorderModified
    {
      get
      {
        return m_formats[ 0 ][ m_iIndex ].IsBottomBorderModified;
      }
      set
      {
        // TODO:  Add ConditionalFormatGroup.IsSubScript setter implementation
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IDataBar DataBar
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IIconSet IconSet
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IColorScale ColorScale
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Number format.
    /// </summary>
    public string NumberFormat
    {
        get
        {
            return m_formats[0][m_iIndex].NumberFormat;
        }
        set
        {
            // TODO:  Add ConditionalFormatGroup.Number format setter implementation
        }
    }
    ///<summary>
    /// Lower priority conditional formatting rules are evaluated.
    ///</summary>
    public bool StopIfTrue
    {
        get
        {
            return m_formats[0][m_iIndex].StopIfTrue;
        }
        set
        {
            // TODO:  Add ConditionalFormatGroup.StopIfTrue setter implementation
        }
    }
    #endregion

    #region IOptimizedUpdate methods
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
