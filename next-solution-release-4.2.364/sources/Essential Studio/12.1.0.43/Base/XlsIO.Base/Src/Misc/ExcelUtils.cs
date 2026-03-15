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
using System.IO;

using Syncfusion.XlsIO;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// ExcelUtils is a helper class that is used for easy usage of direct XlsIO API in performing
  /// repetitive tasks.[The usage of this class is not recommended, Please refer to the documentation
  /// for updated sample code.]
  /// </summary>
  public class ExcelUtils
  {
   /// <summary>
   /// ExcelEngine Object
   /// </summary>
   private static ExcelEngine excelEngine = null;
   /// <summary>
   /// Represents the application object 
   /// </summary>
   private static IApplication application = null;
   /// <summary>
   /// Static Constructor
   /// </summary>
    static ExcelUtils()
    {
      ExcelUtils.excelEngine = new ExcelEngine();
      ExcelUtils.application = excelEngine.Excel;
    }
    /// <summary>
    /// Dispose excelEngine member.
    /// </summary>
    ~ExcelUtils()
    {
      ExcelUtils.excelEngine.Dispose();
    }
    /// <summary>
    /// Determines if an exception is thrown when there are unsaved workbook objects. Default is 
    /// True.
    /// </summary>
    public static bool ThrowNotSavedOnDestroy
    {
      get
      {
        return ExcelUtils.excelEngine.ThrowNotSavedOnDestroy;
      }
      set
      {
        ExcelUtils.excelEngine.ThrowNotSavedOnDestroy = value;
      }
    }
    /// <summary>
    /// Creates a new workbook. The new workbook becomes the active Workbook.
    /// </summary>
    /// <param name="numberOfSheets">Number of Worksheets in the workbook.</param>
    /// <returns>Added Workbook object.</returns>
    public static IWorkbook CreateWorkbook(int numberOfSheets)
    {
      ExcelUtils.application.SheetsInNewWorkbook = numberOfSheets;
      IWorkbook workBook = ExcelUtils.application.Workbooks.Add( null );
      return workBook;
    }
    /// <summary>
    /// Creates a new workbook and the worksheets are named using the names supplied. 
    /// The new workbook becomes the active workbook.
    /// </summary>
    /// <param name="names">Names of the Worksheets in the Workbook.</param>
    /// <returns>Added Workbook object.</returns>
    public static IWorkbook CreateWorkbook(string[] names)
    {
      int numberOfSheets = names.GetLength(0);
      IWorkbook workBook = ExcelUtils.CreateWorkbook(numberOfSheets);
      
      int index = 0;
      foreach(IWorksheet sheet in workBook.Worksheets)
      {
        sheet.Name = names[index];
        index++;
      }

      return workBook;
    }
    /// <summary>
    /// Closes the Workbook object.
    /// </summary>
    public static void CloseWorkBook()
    {
      ExcelUtils.excelEngine.Excel.Workbooks.Close();
    }
    /// <summary>
    /// Creates a workbook using the template file.
    /// </summary>
    /// <param name="templateLocation"></param>
    /// <returns>Workbook created from template.</returns>
    public static IWorkbook CreateWorkBookUsingTemplate(string templateLocation)
    {
      IWorkbook workBook = ExcelUtils.excelEngine.Excel.Workbooks.Open(templateLocation);
      return workBook;
    }
    /// <summary>
    /// Creates a workbook using the template file stream.
    /// </summary>
    /// <param name="stream">Base stream to create.</param>
    /// <returns>Workbook created from template.</returns>
    public static IWorkbook CreateWorkBookUsingTemplate( Stream stream )
    {
      IWorkbook workBook = ExcelUtils.excelEngine.Excel.Workbooks.Open(stream);
      return workBook;
    }
    /// <summary>
    /// Opens the workbook in the specified location.
    /// </summary>
    /// <param name="fileLocation"></param>
    /// <returns>Opened workbook object.</returns>
    public static IWorkbook Open(string fileLocation)
    {  
      IWorkbook workBook = ExcelUtils.excelEngine.Excel.Workbooks.Open(fileLocation);
      return workBook;
    }
    /// <summary>
    /// Opens a workbook in the form of a stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>Opened workbook object.</returns>
    public static IWorkbook Open (Stream stream)
    {  
      IWorkbook workBook = ExcelUtils.excelEngine.Excel.Workbooks.Open(stream);
      return workBook;
    }
    /// <summary>
    /// Not required. Obsolete.
    /// </summary>
    public static void Close()
    {
      // no implementation
    }
    /// <summary>
    /// Copies workbook from the clipboard.
    /// </summary>
    /// <returns>Pasted workbook object.</returns>
    public static IWorkbook PasteWorkbook()
    {
      IWorkbook workBook = ExcelUtils.excelEngine.Excel.Workbooks.PasteWorkbook();
      return workBook;
    }

  }
}