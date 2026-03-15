#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO
{
  public interface ISparklines : IList<ISparkline>
  {
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
    Sparkline Add();
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
    void Add( IRange dataRange, IRange referenceRange );
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
    void RefreshRanges( IRange dataRange, IRange referenceRange );
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
    void Add( IRange dataRange, IRange referenceRange, bool isVertical );
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
    void RefreshRanges( IRange dataRange, IRange referenceRange, bool isVertical );
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
    void Clear( Sparkline sparkline );

  }
}
