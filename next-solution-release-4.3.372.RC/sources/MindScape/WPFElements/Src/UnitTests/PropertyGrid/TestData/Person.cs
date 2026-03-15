using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
  public class Person : Entity
  {
    public static readonly Person Alice = new Person();
    public static readonly Person Bob = new Person();

    private static Random _random = new Random();

    static Person()
    {
      Department department1 = new Department();
      department1.Name = "Sinister Projects";
      department1.Location = Location.Christchurch;

      Department department2 = new Department();
      department2.Name = "Enigmatic Projects";
      department2.Location = null;

      Alice.Id = 1;
      Alice.FirstName = "Alice";
      Alice.Surname = "Turing";
      Alice.DateOfBirth = new DateTime(1976, 10, 11);
      Alice.Address = Address.AndrewsAddress;
      Alice.Status = CitizenshipStatus.Citizen;
      Alice.Alive = true;
      Alice.Partner = Bob;
      Alice.VeryImportantFact = "woo hoo";
      Alice.Puppy = new Puppy(Alice);
      Alice.FavoriteColor = Colors.AliceBlue; // duh
      Alice.NullableProperty = null;
      Alice.ImmutableStruct = new TimeSpan(1, 2, 3);
      Alice.Department = department1;

      Alice.Friends.Add("Eve");
      Alice.Friends.Add(null);
      Alice.Friends.Add("Mallory");
      Alice.Friends.Add("Trent");

      Alice.FavouriteNumbers.Add(16384);
      Alice.FavouriteNumbers.Add(256);
      Alice.FavouriteNumbers.Add(32768);

      // Do type editors play nicely with decorators?
      Alice.PhoneNumbers.Add("Home", new PhoneNumber());
      Alice.PhoneNumbers.Add("Work", new PhoneNumber());
      Alice.PhoneNumbers.Add("Mobile", new PhoneNumber());

      Alice.HatDimensions = new HatDimensions(123, 456, "mfr", Colors.Bisque);

      Bob.Id = 2;
      Bob.FirstName = "Bob";
      Bob.Surname = "Schneier";
      Bob.DateOfBirth = new DateTime(1923, 3, 17);
      Bob.Address = Address.AndrewsAddress;
      Bob.Status = CitizenshipStatus.Alien;
      Bob.Alive = false;
      Bob.Partner = Alice;
      Bob.FavoriteColor = Colors.DarkGoldenrod;  // everybody loves DarkGoldenrod
      Bob.Department = department2;
    }

    public Person()
    {
      _unalterableFacts = new List<string> { "Says 'Fie!' a lot", "Is a Person", "Fie!" }.AsReadOnly();
      _puppies = new List<Puppy>
      {
        new Puppy("Puppy 1", 1),
        new Puppy("Puppy 2", 2)
      }.AsReadOnly();
      if (_random == null)
      {
        _random = new Random();
      }
      int gender = _random.Next(2);
      if (gender == 0)
      {
        _gender = "Female";
      }
      else
      {
        _gender = "Male";
      }
      Address = new Address();
      HatDimensions = new HatDimensions();
      Department = new Department();
    }

    private string _firstName;
    private string _surname;
    private DateTime _dateOfBirth;
    private double _age;
    private Address _address;
    private CitizenshipStatus _status;
    private bool _alive;
    private Person _partner;
    private string _gender = "Female";
    private ObservableCollection<string> _friends = new ObservableCollection<string>();
    private bool _tall;
    private string _nullTest;
    private string _veryImportantFact;
    private Puppy _puppy;
    private Int32Collection _favouriteNumbers = new Int32Collection();
    private int _evilNonGettableProperty;
    private readonly ObservableDictionary<string, PhoneNumber> _phoneNumbers = new ObservableDictionary<string, PhoneNumber>();
    private Color _favoriteColor;
    private int? _nullableProperty;
    private PrintOrientation _favouriteOrientation;
    private TimeSpan _immutableStruct;
    private Department _department;
    private HatDimensions _hatDimensions;
    private readonly ReadOnlyCollection<string> _unalterableFacts;
    private readonly ReadOnlyCollection<Puppy> _puppies;

    // Temporary test code:
    private ObservableCollection<Person> _children = new ObservableCollection<Person>();

    [Browsable(false)]
    public ObservableCollection<Person> Children
    {
      get { return _children; }
      set { Set(ref _children, value, "Children"); }
    }
    //

    [Category("Identity")]
    [Description("The person's first name")]
    [DisplayName("First Name")]
    public string FirstName
    {
      get { return _firstName; }
      set { if (value != null && value.Length > 15) throw new ArgumentException("first name too long"); Set(ref _firstName, value, "FirstName"); }
    }

    [Category("Identity")]
    [Description("The person's family name")]
    public string Surname
    {
      get { return _surname; }
      set { if (value != null && value.Length > 15) throw new ArgumentException("surname too long"); Set(ref _surname, value, "Surname"); }
    }

    [Category("Identity")]
    public DateTime DateOfBirth
    {
      get { return _dateOfBirth; }
      set { if (value > DateTime.Now) throw new ArgumentException("born in the future"); Set(ref _dateOfBirth, value, "DateOfBirth"); }
    }

    public double Age
    {
      get { return _age; }
      set
      {
        if (value < 0)
        {
          throw new InvalidOperationException("Age can not be negative");
        }
        Set(ref _age, value, "Age");
      }
    }

    public Address Address
    {
      get { return _address; }
      set { Set(ref _address, value, "Address"); }
    }

    //[TypeConverter(typeof(CitizenshipStatusConverter))]
    public CitizenshipStatus Status
    {
      get { return _status; }
      set { Set(ref _status, value, "Status"); }
    }

    [TypeConverter(typeof(YesNoConverter))]
    public bool Alive
    {
      get { return _alive; }
      set { Set(ref _alive, value, "Alive"); }
    }

    public Person Partner
    {
      get { return _partner; }
      set { Set(ref _partner, value, "Partner"); }
    }

    [Category("Identity")]
    public string Gender
    {
      get { return _gender; }
    }

    [Category("Likes & Dislikes")]
    public ICollection<string> Friends
    {
      get { return _friends; }
    }

    public bool Tall
    {
      get { return _tall; }
      set { Set(ref _tall, value, "Tall"); }
    }

    // This is to ensure that we don't go haywire if a value is null
    public string NullTest
    {
      get { return _nullTest; }
      set { Set(ref _nullTest, value, "NullTest"); }
    }

    public string VeryImportantFact
    {
      get { return _veryImportantFact; }
      set { Set(ref _veryImportantFact, value, "VeryImportantFact"); }
    }

    [Category("Likes & Dislikes")]
    public Puppy Puppy
    {
      get { return _puppy; }
      set { Set(ref _puppy, value, "Puppy"); }
    }

    [Category("Likes & Dislikes")]
    public ICollection<int> FavouriteNumbers
    {
      get { return _favouriteNumbers; }
    }

    public int EvilNonGettableProperty
    {
      set { _evilNonGettableProperty = value; }
    }

    public ObservableDictionary<string, PhoneNumber> PhoneNumbers
    {
      get { return _phoneNumbers; }
    }

    [Category("Likes & Dislikes")]
    public Color FavoriteColor
    {
      get { return _favoriteColor; }
      set { Set(ref _favoriteColor, value, "FavoriteColor"); }
    }

    [DisplayName("Nullable Integer")]
    public int? NullableProperty
    {
      get { return _nullableProperty; }
      set { Set(ref _nullableProperty, value, "NullableProperty"); }
    }

    public PrintOrientation FavouriteOrientation
    {
      get { return _favouriteOrientation; }
      set { Set(ref _favouriteOrientation, value, "FavouriteOrientation"); }
    }

    public TimeSpan ImmutableStruct
    {
      get { return _immutableStruct; }
      set { Set(ref _immutableStruct, value, "ImmutableStruct"); }
    }

    public Department Department
    {
      get { return _department; }
      set { Set(ref _department, value, "Department"); }
    }

    private CustomNamedEnum _mysteryEnum;

    public CustomNamedEnum MysteryEnum
    {
      get { return _mysteryEnum; }
      set { Set(ref _mysteryEnum, value, "MysteryEnum"); }
    }

    public HatDimensions HatDimensions
    {
      get { return _hatDimensions; }
      set
      {
        _hatDimensions.PropertyChanged -= new PropertyChangedEventHandler(HatDimensions_PropertyChanged);
        Set(ref _hatDimensions, value, "HatDimensions");
        _hatDimensions.PropertyChanged += new PropertyChangedEventHandler(HatDimensions_PropertyChanged);
      }
    }

    void HatDimensions_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged("HatDimensions");
    }

    public void EnlargeHat(int amount)
    {
      _hatDimensions.BrimSize = _hatDimensions.BrimSize + amount;
    }

    public ReadOnlyCollection<string> UnalterableFacts
    {
      get { return _unalterableFacts; }
    }

    public ReadOnlyCollection<Puppy> Puppies
    {
      get { return _puppies; }
    }
  }

  public class Int32Collection : ObservableCollection<Int32> { }

  public enum PrintOrientation
  {
    Landscape,
    Portrait,
    Origami
  }

  public class YesNoConverter : TypeConverter
  {
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(new bool[] { true, false });
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (value == null)
      {
        return value;
      }

      if (value is bool && destinationType == typeof(string))
      {
        return ((bool)value) ? "Si, Barone!" : "No";
      }

      return value;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      string valueText = value as string;
      if (valueText != null)
      {
        return valueText == "Si, Barone!";
      }

      throw new NotSupportedException("unknown text");
    }
  }

  public class CitizenshipStatusConverter : TypeConverter
  {
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(new CitizenshipStatus[] { CitizenshipStatus.Alien, CitizenshipStatus.Citizen, CitizenshipStatus.Resident, CitizenshipStatus.WorkerVisa });
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (value == null)
      {
        return value;
      }

      if (value is CitizenshipStatus && destinationType == typeof(string))
      {
        return ((CitizenshipStatus)value == CitizenshipStatus.WorkerVisa) ? "Worker Visa" : value.ToString();
      }

      return value;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      Debug.WriteLine("CONVERTING FROM");
      string valueText = value as string;
      switch (valueText)
      {
        case "Citizen":
          return CitizenshipStatus.Citizen;
        case "Resident":
          return CitizenshipStatus.Resident;
        case "Worker Visa":
          return CitizenshipStatus.WorkerVisa;
        case "Alien":
          return CitizenshipStatus.Alien;
      }

      throw new NotSupportedException("unknown text");
    }
  }
}