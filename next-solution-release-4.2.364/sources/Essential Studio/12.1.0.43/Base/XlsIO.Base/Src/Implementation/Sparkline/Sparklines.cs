#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region File Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents the sparklines.The Sparklines object is a member of the SparklineGroup collection.
  /// The SparklineGroup collection contains all the Sparklines object of the worksheet.
  /// </summary>
  public class Sparklines :
    List<ISparkline>,
    ISparklines
  {
    #region Properties
    internal SparklineGroup ParentGroup;
    #endregion

    #region Implementation
    /// <summary>
    /// Adds Sparkline instance.
    /// </summary>
    /// <example>
    /// The following example demonstrates the Add method.
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups.Add();
    /// 
    /// //Initialize the Saprklines
    /// Sparklines sparklines=spGroup.Add(); 
    /// 
    /// //returns the sparkline object.
    /// Sparkline sparkline=sparklines.Add();
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    /// <returns>the Saprkline object</returns>
    public Sparkline Add()
    {
      Sparkline sparkline = new Sparkline();
      base.Add( sparkline );
      return sparkline;
    }

    /// <summary>
    /// Adds the sparkline.
    /// </summary>
    /// <param name="dataRange">The data range.<see cref="IRange"/></param>
    /// <param name="referenceRange">The reference range.<see cref=" IRange"/></param>
    /// <example>
    /// The following example demonstrates the Add method.
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups.Add();
    /// 
    /// //Initialize the Sparklines
    /// Sparklines sparklines=spGroup.Add(); 
    /// 
    /// //Add the Sparklines data range and reference range.
    /// sparklines.Add(sheet.Range["A1:B2"],sheet.Range["C1"],true);
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    public void Add( IRange dataRange, IRange referenceRange )
    {
      this.Add( dataRange, referenceRange, false );
    }
    /// <summary>
    /// Adds the sparkline.
    /// </summary>
    /// <param name="dataRange">The data range.<see cref="IRange"/></param>
    /// <param name="referenceRange">The reference range.<see cref=" IRange"/></param>
    /// <example>
    /// The following example demonstrates the Add method.
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups[0];
    /// 
    /// //Initialize the Sparklines
    /// Sparklines sparklines=spGroup[0]; 
    /// 
    /// //Add the Sparklines data range and reference range.
    /// sparklines.RefreshRanges(sheet.Range["A1:B2"],sheet.Range["C1"]);
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    public void RefreshRanges( IRange dataRange, IRange referenceRange )
    {
      this.RefreshRanges( dataRange, referenceRange, false );
    }
    /// <summary>
    /// Adds the sparkline.
    /// </summary>
    /// <param name="dataRange">The data range.<see cref="IRange"/></param>
    /// <param name="referenceRange">The reference range.<see cref=" IRange"/></param>
    /// <param name="isVertical" >Specifies whether to plot the sparklines from the new data range by row or by column.</param>
    /// <example>
    /// The following example demonstrates the Add method.
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups.Add();
    /// 
    /// //Initialize the Sparklines
    /// Sparklines sparklines=spGroup.Add(); 
    /// 
    /// //Add the Sparklines data range and reference range.
    /// sparklines.Add(sheet.Range["A1:B2"],sheet.Range["C1"],true);
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    public void Add( IRange dataRange, IRange referenceRange, bool isVertical )
    {
        if (isVertical)
        {
            if (referenceRange.Rows.Length != dataRange.Columns.Length)
                throw new ArgumentOutOfRangeException("range", "The reference for the location or data range is not valid.");
        }
        else if (!(referenceRange.Rows.Length <= dataRange.Rows.Length && referenceRange.Columns.Length <= dataRange.Columns.Length))
            throw new ArgumentOutOfRangeException("range", "The reference for the location or data range is not valid.");

      this.UpdateSparklines( dataRange, referenceRange, isVertical );
    }
    /// <summary>
    /// Adds the sparkline.
    /// </summary>
    /// <param name="dataRange">The data range.<see cref="IRange"/></param>
    /// <param name="referenceRange">The reference range.<see cref=" IRange"/></param>
    /// <example>
    /// The following example demonstrates the Add method.
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups[0];
    /// 
    /// //Initialize the Sparklines
    /// Sparklines sparklines=spGroup[0]; 
    /// 
    /// //Add the Sparklines data range and reference range.
    /// sparklines.RefreshRanges(sheet.Range["A1:B2"],sheet.Range["C1"],true);
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    public void RefreshRanges( IRange dataRange, IRange referenceRange, bool isVertical )
    {
        if (isVertical)
        {
            if (referenceRange.Rows.Length != dataRange.Columns.Length)
                throw new ArgumentOutOfRangeException("range", "The reference for the location or data range is not valid.");
        }
        else if (!(referenceRange.Rows.Length <= dataRange.Rows.Length && referenceRange.Columns.Length <= dataRange.Columns.Length))
            throw new ArgumentOutOfRangeException("range", "The reference for the location or data range is not valid.");

      this.Clear();
      this.UpdateSparklines( dataRange, referenceRange, isVertical );
    }
    /// <summary>
    /// Clears the sparkline.
    /// </summary>
    /// <param name="sparklineRange">The sparkline range.<see cref=" IRange"/></param>
    /// <example>
    /// The following example demonstrates the Clear Method
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups.Add();
    /// 
    /// //Initialize the Saprklines
    /// Sparklines sparklines=spGroup.Add(); 
    /// 
    /// //Clears the Sparkline from the specified range.
    /// sparklines.Clear(sparklines[0]);
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    public void Clear( Sparkline sparkline )
    {
      this.Remove( sparkline );
    }


    #endregion

    #region Class Helper methods

    /// <summary>
    /// Updates the sparklines.
    /// </summary>
    /// <param name="dataRange">The data range.</param>
    /// <param name="referenceRange">The reference range.</param>
    /// <param name="isVertical">if set to <c>true</c> [is vertical].</param>
    internal void UpdateSparklines( IRange dataRange, IRange referenceRange, bool isVertical )
    {
      Sparkline sparkline;
      if( isVertical )
      {
        if( ( referenceRange.Columns.Length > 1 ) && ( referenceRange.Rows.Length == 1 ) )
        {
          for( int i = 0, j = 0; i < dataRange.Columns.Length && j < referenceRange.Columns.Length; i++, j++ )
          {
            sparkline = new Sparkline();
            sparkline.DataRange = dataRange.Columns[ i ];
            sparkline.ReferenceRange = referenceRange.Columns[ j ];
            base.Add( sparkline );
          }
        }
        else
        {
          for( int i = 0, j = 0; i < dataRange.Columns.Length && j < referenceRange.Rows.Length; i++, j++ )
          {
            sparkline = new Sparkline();
            sparkline.DataRange = dataRange.Columns[ i ];
            sparkline.ReferenceRange = referenceRange.Rows[ j ];
            base.Add( sparkline );
          }
        }
      }
      else
      {
        if( ( referenceRange.Columns.Length > 1 ) && ( referenceRange.Rows.Length == 1 ) )
        {
          for( int i = 0, j = 0; i < dataRange.Rows.Length && j < referenceRange.Columns.Length; i++, j++ )
          {
            sparkline = new Sparkline();
            sparkline.DataRange = dataRange.Rows[ i ];
            sparkline.ReferenceRange = referenceRange.Columns[ j ];
            base.Add( sparkline );
          }
        }
        else
        {
          for( int i = 0, j = 0; i < dataRange.Rows.Length && j < referenceRange.Rows.Length; i++, j++ )
          {
            sparkline = new Sparkline();
            sparkline.DataRange = dataRange.Rows[ i ];
            sparkline.ReferenceRange = referenceRange.Rows[ j ];
            base.Add( sparkline );
          }
        }
      }
    }
    #endregion
  }
}
