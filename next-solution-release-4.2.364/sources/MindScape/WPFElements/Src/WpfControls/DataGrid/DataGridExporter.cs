using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements
{
  /// <summary>
  ///  Exports a DataGrid's content to a particular format
  /// </summary>
  public abstract class DataGridExporter
  {
    /// <summary>
    /// Exports a DataGrid to the CSV format
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly DataGridExporter Csv = new DataGridCsvExporter();

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridExporter"/> class. 
    /// </summary>
    protected DataGridExporter()
    {
    }

    /// <summary>
    /// Exports a DataGrid to a file
    /// </summary>
    /// <param name="dataGrid">The DataGrid to be exported</param>
    /// <param name="path">The file in which to write the formatted data</param>
    /// <remarks>If the file exists, it is overwritten</remarks>
    public void WriteFile(DataGrid dataGrid, string path)
    {
      using (FileStream fileStream = File.Create(path))
      {
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
          WriteTextWriter(dataGrid, streamWriter);
        }
      }
    }

    /// <summary>
    /// Exports a DataGrid and returns the content formatted as the specified file format as a string
    /// </summary>
    /// <param name="dataGrid">The DataGrid to export</param>
    /// <returns>A string with the DataGrid's content formatted as the specified file type</returns>
    public string WriteString(DataGrid dataGrid)
    {
      string result = string.Empty;
      using (StringWriter stringWriter = new StringWriter())
      {
        WriteTextWriter(dataGrid, stringWriter);
        result = stringWriter.ToString();
      }
      return result;
    }

    /// <summary>
    /// Exports a DataGrid to a TextWriter
    /// </summary>
    /// <param name="dataGrid">The DataGrid to export</param>
    /// <param name="textWriter">The TextWriter that is to have the contents written to</param>
    public void WriteTextWriter(DataGrid dataGrid, TextWriter textWriter)
    {
      if (dataGrid.ItemsSource != null && textWriter != null)
      {
        WriteCore(dataGrid, textWriter);
      }
    }

    /// <summary>
    /// Override this with specific logic to format a DataGrid's ItemsContent to a particular file format.
    /// </summary>
    /// <param name="dataGrid">The DataGrid to export</param>
    /// <param name="textWriter">The TextWriter that will contain the resulting formatted data</param>
    protected abstract void WriteCore(DataGrid dataGrid, TextWriter textWriter);        
  }
}