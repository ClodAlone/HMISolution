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
  public interface ISparklineGroups : IList<ISparklineGroup>
  {
    /// <summary>
    /// Clears the sparkline group.
    /// </summary>
    /// <param name="sparklineGroupRange">The sparkline group range.</param>
    /// <example> This follwing is the example for Clear Method
    /// <code>
    /// ExcelEngine engine=new ExcelEngine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroups spGroups= sheet.SparklineGroups;      
    /// 
    /// //Clears the SparklineGroups from the Specified range.
    /// spGroups.Clear(spGroups[0]);
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// 
    /// </code>
    /// </example>
    void Clear( ISparklineGroup sparklineGroup );
    /// <summary>
    /// Adds the SparklineGroup instance.
    /// </summary>
    /// <example>The follwoing is the sample for Add method
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
    /// SparklineGroups spGroups= sheet.SparklineGroups;      
    /// 
    /// //Returns the SparklineGroup instance.
    /// SparklineGroup spGroup= spGroups.Add();
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// </code>
    /// </example>
    /// <returns>SparklineGroup instance</returns>
    ISparklineGroup Add();
  }
}
