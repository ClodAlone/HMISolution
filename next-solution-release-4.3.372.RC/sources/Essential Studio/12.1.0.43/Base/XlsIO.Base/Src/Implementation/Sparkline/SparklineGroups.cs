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
  /// The <c> SparklineGroups</c> represents the collection of SparklineGroup objects.
  /// </summary>
  public class SparklineGroups :
    List<ISparklineGroup>,
    ISparklineGroups
  {
    #region Members
    private WorkbookImpl m_book;
    #endregion

    #region Implementation
    public SparklineGroups( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
    }
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
    public void Clear( ISparklineGroup sparklineGroup )
    {
      if( sparklineGroup == null )
        throw new ArgumentNullException( "SparklineGroup" );

      this.Remove( sparklineGroup );
    }

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
    public ISparklineGroup Add()
    {
      SparklineGroup sparklineGroup = new SparklineGroup( m_book );
      base.Add( sparklineGroup );
      return sparklineGroup;
    }

    #endregion
  }
}
