using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A wrapper for setting the items source of a <see cref="DataGrid"/> to be a <see cref="DataTable"/>.
  /// </summary>
  public class DataTableWrapper : IEnumerable, INotifyCollectionChanged
  {
    private readonly DataTable _dataTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTableWrapper"/> class.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to wrap.</param>
    public DataTableWrapper(DataTable dataTable)
    {
      _dataTable = dataTable;
      if (_dataTable != null)
      {
        _dataTable.DefaultView.ListChanged += new System.ComponentModel.ListChangedEventHandler(DefaultView_ListChanged);
      }
    }

    private void DefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemMoved || e.ListChangedType == ListChangedType.Reset || e.ListChangedType == ListChangedType.ItemChanged)
      {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }
    }

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      NotifyCollectionChangedEventHandler handler = CollectionChanged;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    /// <summary>
    /// Gets the <see cref="DataTable"/> that this <see cref="DataTableWrapper"/> uses.
    /// </summary>
    public DataTable DataTable { get { return _dataTable; } }

    /// <summary>
    /// Gets the enumerator that enumerates the rows of this <see cref="DataTableWrapper"/>.
    /// </summary>
    /// <returns>The collection enumerator.</returns>
    public IEnumerator GetEnumerator()
    {
      return DataTable != null ? DataTable.DefaultView.GetEnumerator() : new List<object>().GetEnumerator();
    }

    /// <summary>
    /// Raised when this <see cref="DataTableWrapper"/> collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}
