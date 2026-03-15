using System;
using System.Globalization;
using System.Windows;
using Mindscape.WpfElements.Properties;

namespace Mindscape.WpfElements
{
  internal static class AssemblyLicense
  {
    internal const string ValidationParameters = @"<LicenseParameters><RSAKeyValue><Modulus>w1Z5vkOxz8dwN6YLkbyxfpycbwGwFeaVTGXWmsUfi24K3sLgz5OqXsd+4SmL01hL2AKaILIyw7gKzG2lXx1GyPzqK/vg7nckxM6t9wmaGvKluYfFLO6cr5uiPSK1Nh08XtfXcHOUbYx0Xj0n9/8Yn6xtnOk+Q/1nefIuudvQ54U=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue><DesignSignature>RgqcWPBwAJkmrC/lFiMAzPc+Ac+3S1yBhgiLm3ZHpmznJkRvDXrLSQp0zIDz3UNM57qjGnIjvnYuyumTOXjE+HqDY3zBBQJMYPrK34PN9baWtoo6V7rZh/AdweE8p60j3uQjlNOJUGqq+D/yzeUls04DddcvGBkste/0GaqzE34=</DesignSignature><RuntimeSignature>jPFkXdEWVb4k/t+vFFxWaoYZ2bA8WKpcOgWJu6fUJmsX6TxEmkl8F5A7G+3QXSbU/qnoZpp4kSgDqLwgI8WhG5crbMPCvH5HZRVbgnRV425xHtdlsJ57tkLjGfMMV93o3QbsXXlENK44HxpF0r2v7qWXHqDvgS1EsBDsp05ZAgc=</RuntimeSignature><KeyStrength>8</KeyStrength><ChecksumProductInfo>True</ChecksumProductInfo><TextEncoding>Base32</TextEncoding></LicenseParameters>";

    internal static void ShowGracePeriodMessage(DateTime expiryDate)
    {
      string expiringSoonMessage = String.Format(CultureInfo.CurrentCulture, Resources.ExpiringSoon, expiryDate);
      MessageBox.Show(expiringSoonMessage, Resources.TrialLicenseMessageCaption);
    }

    internal static void ShowExpiredMessage(DateTime expiryDate)
    {
      string expiredMessage = String.Format(CultureInfo.CurrentCulture, Resources.ExpiredOn, expiryDate);
      MessageBox.Show(expiredMessage, Resources.TrialLicenseMessageCaption);
    }
  }
}
