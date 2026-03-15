using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;
using System.IO;
using System.Collections;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Implementation class for exporting a DataGrid to CSV format
  /// </summary>
  public class DataGridCsvExporter : DataGridExporter
  {
    private string _delimiter = ",";
    private bool _isQuotingEnabled = true;

    /// <summary>
    /// Gets or sets the character(s) used to separate cells in the CSV file
    /// </summary>
    public String Delimiter
    {
      get { return _delimiter; }
      set { _delimiter = value; }
    }

    private DataGridExportMode _exportMode = DataGridExportMode.All;

    /// <summary>
    /// Property that selects which set of items will be exported from a DataGrid
    /// </summary>
    public DataGridExportMode ExportMode
    {
      get { return _exportMode; }
      set { _exportMode = value; }
    }

    /// <summary>
    /// Gets or sets whether or not values are wrapped in quote marks. The default is true.
    /// </summary>
    public bool IsQuotingEnabled
    {
      get { return _isQuotingEnabled; }
      set
      {
        _isQuotingEnabled = value;
      }
    }

    private bool _isColumnHeaderExported = true;

    /// <summary>
    /// Enables/disables the writing of a DataGrid's column headers as the first line
    /// of a CSV exporter's output
    /// </summary>
    public bool IsColumnHeaderExported
    {
      get { return _isColumnHeaderExported; }
      set { _isColumnHeaderExported = value; }
    }

    /// <summary>
    /// Functionality to write the contents of a datagrid to an output destination
    /// </summary>
    /// <param name="dataGrid">The DataGrid to export</param>
    /// <param name="textWriter">The destination to export to</param>
    protected override void WriteCore(DataGrid dataGrid, TextWriter textWriter)
    {
      string quoteMark = IsQuotingEnabled ? "\"" : "";
      if (_isColumnHeaderExported)
      {
        foreach (DataGridColumn column in dataGrid.EffectiveColumns)
        {
          if (column.PropertyInfo != null)
          {
            if (column != dataGrid.EffectiveColumns[dataGrid.EffectiveColumns.Count - 1])
            {
              textWriter.Write(quoteMark + column.Header.ToString() + quoteMark + _delimiter);
            }
            else
            {
              textWriter.Write(quoteMark + column.Header.ToString() + quoteMark);
            }
          }
        }
        textWriter.WriteLine();
      }      

      IEnumerable rows;
      if (_exportMode == DataGridExportMode.All)
      {
        rows = dataGrid.ItemsSource;
      }
      else
      {
        rows = dataGrid.SelectedItems;
      }

      foreach (Object row in rows)
      {
        foreach (DataGridColumn column in dataGrid.EffectiveColumns)
        {
          if (column.PropertyInfo != null)
          {
            Object o = column.PropertyInfo.AsPropertyInfo.GetValue(row, null);
            if (o != null && o.ToString().StartsWith("\"") && o.ToString().EndsWith("\""))
            {
              textWriter.Write(quoteMark);
              textWriter.Write(quoteMark);
              textWriter.Write(o.ToString());
              textWriter.Write(quoteMark);
              textWriter.Write(quoteMark);
            }
            else
            {
              textWriter.Write(quoteMark);
              textWriter.Write(o == null ? "" : o.ToString());
              textWriter.Write(quoteMark);
            }
            if (column != dataGrid.EffectiveColumns[dataGrid.EffectiveColumns.Count - 1])
            {
              textWriter.Write(_delimiter);
            }
          }
        }
        textWriter.WriteLine();
      }
    }
  }

  /// <summary>
  /// Selects what items in the DataGrid will be exported
  /// </summary>
  public enum DataGridExportMode
  {
    /// <summary>
    /// Exports the entire collection the DataGrid is bound to
    /// </summary>
    All,
    /// <summary>
    /// Exports just the rows the user has selected from the DataGrid's ItemSource
    /// </summary>
    SelectedRows
  }
}
