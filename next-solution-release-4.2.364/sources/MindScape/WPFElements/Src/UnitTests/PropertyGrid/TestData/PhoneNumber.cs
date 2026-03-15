using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public class PhoneNumber : Entity
  {
    private string _countryCode;
    private string _regionCode;
    private string _number;

    public string CountryCode
    {
      get { return _countryCode; }
      set { Set(ref _countryCode, value, "CountryCode"); }
    }

    public string RegionCode
    {
      get { return _regionCode; }
      set { Set(ref _regionCode, value, "RegionCode"); }
    }

    public string Number
    {
      get { return _number; }
      set { Set(ref _number, value, "Number"); }
    }
  }
}
