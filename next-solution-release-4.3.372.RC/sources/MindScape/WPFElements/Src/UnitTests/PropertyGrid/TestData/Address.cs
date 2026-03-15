namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class Address : Entity
  {
    public static readonly Address AndrewsAddress = new Address();

    static Address()
    {
      AndrewsAddress.Id = 1;
      AndrewsAddress.Country = "New Zealand";
      AndrewsAddress.City = "Wellington";
      AndrewsAddress.StreetNumber = 28;
      AndrewsAddress.StreetName = "Bolton";
      AndrewsAddress.PhoneNumber.CountryCode = "64";
      AndrewsAddress.PhoneNumber.RegionCode = "4";
      AndrewsAddress.PhoneNumber.Number = "1231234";
    }

    private int _streetNumber;
    private string _streetName;
    private string _city;
    private string _country;
    private PhoneNumber _phoneNumber = new PhoneNumber();

    public string Country
    {
      get { return _country; }
      set { Set(ref _country, value, "Country"); }
    }

    public string City
    {
      get { return _city; }
      set { Set(ref _city, value, "City"); }
    }

    public string StreetName
    {
      get { return _streetName; }
      set { Set(ref _streetName, value, "StreetName"); }
    }

    public int StreetNumber
    {
      get { return _streetNumber; }
      set { Set(ref _streetNumber, value, "StreetNumber"); }
    }

    public PhoneNumber PhoneNumber
    {
      get { return _phoneNumber; }
      set { _phoneNumber = value; }
    }

    public override string ToString()
    {
      return string.Format("{0} {1} {2}", StreetNumber, StreetName, City).Trim();
    }
  }
}