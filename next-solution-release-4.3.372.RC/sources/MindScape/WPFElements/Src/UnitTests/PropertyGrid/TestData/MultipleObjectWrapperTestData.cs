using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class Animal : Entity
  {
    public static readonly Animal Fang = CreateAnimal("Cat", 4);
    public static readonly Animal Kiki = CreateAnimal("Cat", 4);
    public static readonly Animal Butch = CreateAnimal("Dog", 4);
    public static readonly Animal Fido = CreateAnimal("Dog", 4, new DateTime(2001, 12, 31));
    public static readonly Animal Boris = CreateAnimal("Spider", 8);

    private string _species;

    [Browsable(true)]
    public string Species
    {
      get { return _species; }
      set { Set(ref _species, value, "Species"); }
    }

    private int _legCount;

    public int LegCount
    {
      get { return _legCount; }
      set { Set(ref _legCount, value, "LegCount"); }
    }

    private readonly DateTime _birthDate = new DateTime(2000, 1, 1);

    public DateTime BirthDate
    {
      get { return _birthDate; }
    }

    public Animal()
    {

    }

    public Animal(DateTime birthDate) : this()
    {
      _birthDate = birthDate;
    }

    private static Animal CreateAnimal(string species, int legCount)
    {
      return new Animal { Species = species, LegCount = legCount };
    }

    private static Animal CreateAnimal(string species, int legCount, DateTime birthDate)
    {
      return new Animal(birthDate) { Species = species, LegCount = legCount };
    }

    public override string ToString()
    {
      return String.Format("Animal: Species={0}, LegCount={1}, BirthDate={2}", Species, LegCount, BirthDate);
    }
  }

  public class Alien : Entity
  {
    public static readonly Alien Bill = CreateAlien("Vogon", 2);
    public static readonly Alien Anastasia = CreateAlien("Dalek", 0);

    private string _species;

    public string Species
    {
      get { return _species; }
      set { Set(ref _species, value, "Species"); }
    }

    private long _legCount;

    public long LegCount
    {
      get { return _legCount; }
      set { Set(ref _legCount, value, "LegCount"); }
    }

    private DateTime _birthDate = new DateTime(2000, 1, 1);

    public DateTime BirthDate
    {
      get { return _birthDate; }
      set { Set(ref _birthDate, value, "BirthDate"); }
    }

    private Color _tentacleColor = Colors.GreenYellow;

    public Color TentacleColor
    {
      get { return _tentacleColor; }
      set { Set(ref _tentacleColor, value, "TentacleColor"); }
    }

    private PhoneNumber _phoneNumber = new PhoneNumber();

    public PhoneNumber PhoneNumber
    {
      get { return _phoneNumber; }
      set { Set(ref _phoneNumber, value, "PhoneNumber"); }
    }

    private string _firstName;

    [Category("Plogdar! Spragichi!")]  // they're aliens, dude
    public string FirstName
    {
      get { return _firstName; }
      set { Set(ref _firstName, value, "FirstName"); }
    }

    private static Alien CreateAlien(string species, long legCount)
    {
      return new Alien { Species = species, LegCount = legCount };
    }

    public override string ToString()
    {
      return String.Format("Alien: Species={0}, LegCount={1}, BirthDate={2}, TentacleColor={3}, FirstName={4}", Species, LegCount, BirthDate, TentacleColor, FirstName);
    }
  }

  [TypeConverter(typeof(PlantConverter))]
  public class Plant : Entity
  {
    public static readonly Plant ZogTheDevourer = CreatePlant("Triffid", false);

    private string _species;

    public string Species
    {
      get { return _species; }
      set { Set(ref _species, value, "Species"); }
    }

    private bool _isFlowering;

    public bool IsFlowering
    {
      get { return _isFlowering; }
      set { Set(ref _isFlowering, value, "IsFlowering"); }
    }

    private Animal _favouriteFood;

    public Animal FavouriteFood
    {
      get { return _favouriteFood; }
      set { Set(ref _favouriteFood, value, "FavouriteFood"); }
    }

    private string _firstName;

    [Category("Identity")]
    public string FirstName
    {
      get { return _firstName; }
      set { Set(ref _firstName, value, "FirstName"); }
    }

    private static Plant CreatePlant(string species, bool isFlowering)
    {
      return new Plant { Species = species, IsFlowering = isFlowering };
    }

    public override string ToString()
    {
      return String.Format("Plant: Species={0}, IsFlowering={1}", Species, IsFlowering);
    }
  }

  public class Herbivore : Entity
  {
    private string _species;

    [Browsable(false)]
    public string Species
    {
      get { return _species; }
      set { Set(ref _species, value, "Species"); }
    }

    private Plant _favouriteFood;

    public Plant FavouriteFood
    {
      get { return _favouriteFood; }
      set { Set(ref _favouriteFood, value, "FavouriteFood"); }
    }

    public override string ToString()
    {
      return String.Format("Herbivore: Species={0}, FavouriteFood={1}", Species, (FavouriteFood == null ? "(null)" : FavouriteFood.Species));
    }
  }

  public class TheAntiMergeOTron1
  {
    public int SomeInt { get; set; }
    public string SomeString { get; set; }
    public int MergeableOnlyIn1 { get; set; }
  }

  public class TheAntiMergeOTron2
  {
    public int SomeInt { get; set; }
    public string SomeString { get; set; }
    [MergableProperty(false)]
    public int MergeableOnlyIn1 { get; set; }
  }

  public class PlantConverter : TypeConverter
  {
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
    {
      return new Plant { Species = "Triffid", IsFlowering = true };
    }
  }
}
