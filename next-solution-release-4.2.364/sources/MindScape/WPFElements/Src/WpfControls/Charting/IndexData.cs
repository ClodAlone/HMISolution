using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mindscape.WpfElements.Charting
{
  internal struct IndexData
  {
    private object _data;
    private int _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexData"/> struct.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="data">The data object.</param>
    public IndexData(int index, object data)
    {
      _index = index;
      _data = data;
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    public int Index { get { return _index; } }

    /// <summary>
    /// Gets the data object.
    /// </summary>
    public object Data { get { return _data; } }
  }
}
