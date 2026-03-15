using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class Entity : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private int _id;

    [Browsable(false)]
    public int Id
    {
      get { return _id; }
      set { Set(ref _id, value, "Id"); }
    }

    protected void Set<T>(ref T field, T value, string propertyName)
    {
      if (!Equals(field, value))
      {
        field = value;

        OnPropertyChanged(propertyName);
      }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}