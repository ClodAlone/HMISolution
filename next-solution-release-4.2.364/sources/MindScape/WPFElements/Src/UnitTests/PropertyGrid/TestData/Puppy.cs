using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class Puppy : Entity, IDataErrorInfo
  {
    private readonly Person _owner;

    public Puppy(Person owner)
    {
      _owner = owner;
    }

    public Puppy(string name, int cuteness)
    {
      _owner = null;
      _name = name;
      _alive = true;
      _cuteness = cuteness;
    }

    private string _name;

    public string Name
    {
      get { return _name; }
      set { Set(ref _name, value, "Name"); }
    }


    private bool _alive;

    public bool Alive
    {
      get { return _alive; }
      set { Set(ref _alive, value, "Alive"); }
    }

    private int _cuteness;

    public int Cuteness
    {
      get { return _cuteness; }
      set { Set(ref _cuteness, value, "Cuteness"); }
    }

    [Browsable(false)]
    public Person Owner { get { return _owner; } }

    #region IDataErrorInfo Members

    string IDataErrorInfo.Error
    {
      get { return Alive ? null : "puppy is dead"; }
    }

    string IDataErrorInfo.this[string columnName]
    {
      get
      {
        switch (columnName)
        {
          case "Alive": return Alive ? null : "puppy is dead";
          case "Cuteness": return Cuteness > 50 ? "too cute" : null;
        }

        return null;
      }
    }

    #endregion
  }
}
