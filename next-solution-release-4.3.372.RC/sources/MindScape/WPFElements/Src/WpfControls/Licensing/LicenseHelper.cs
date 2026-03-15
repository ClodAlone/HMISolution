using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;
using System.Globalization;
using Mindscape.WpfElements.Properties;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  internal static class LicenseHelper
  {
    private static string _licenseKeyToUse;

    private const string TrialLicenseIdentifier = "trial";
    private const string GracePeriodIdentifier = "grace";
    private const string ExpiryDateOverrideIdentifier = "expiry";
    private const int DefaultTrialPeriodDays = 60;
    private const int DefaultGracePeriodDays = 5;

    private static License _license;
    private static Type _type;
    private static object _instance;

    private static bool _doneOneOffCheck = false;

    private static readonly EncryptedLicenseProvider _provider = new EncryptedLicenseProvider();

    internal static void Attach(FrameworkElement instance, Assembly assembly)
    {
      _type = instance.GetType();
      _instance = instance;
      PerformLicenseChecks(assembly);
      _instance = null;

      instance.Unloaded += CleanupLicense;
    }

    internal static void SetLicenseKeyToUse(string licenseKey)
    {
      _licenseKeyToUse = licenseKey;
    }

    private static Dictionary<string, string> ParseProductInfo(string productInfo)
    {
      Dictionary<string, string> parsed = new Dictionary<string, string>();

      string[] elements = productInfo.Split(';');
      foreach (string element in elements)
      {
        string[] contents = element.Split('=');
        if (contents.Length == 2)
        {
          parsed[contents[0]] = contents[1];
        }
      }

      return parsed;
    }

    private class LicenseActions
    {
      public static Action Trial(int daysRemaining)
      {
        return () => Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, Resources.TrialLicenseTraceMessage, daysRemaining));
      }

      public static Action Grace(int daysRemaining, DateTime expiryDate)
      {
        return () =>
        {
          Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, Resources.TrialLicenseTraceMessage_GracePeriod, daysRemaining));
          AssemblyLicense.ShowGracePeriodMessage(expiryDate);
        };
      }

      public static Action Expired(DateTime expiryDate, Type type)
      {
        return () =>
        {
          Trace.WriteLine(Resources.TrialLicenseTraceMessage_Expired);
          AssemblyLicense.ShowExpiredMessage(expiryDate);
          throw new LicenseException(type);
        };
      }

      public static readonly Action Unrestricted = () => { return; };
    }

    private static int GetIntProductInfo(Dictionary<string, string> productInfo, string key, int defaultValue)
    {
      string s;
      if (productInfo.TryGetValue(key, out s))
      {
        return Int32.Parse(s, CultureInfo.InvariantCulture);
      }
      return defaultValue;
    }

    private static DateTime? GetDateTimeProductInfo(Dictionary<string, string> productInfo, string key)
    {
      string s;
      if (productInfo.TryGetValue(key, out s))
      {
        return DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture);
      }
      return null;
    }

    private static Action GetLicenseAction(Dictionary<string, string> productInfo)
    {
      if (productInfo.ContainsKey(TrialLicenseIdentifier) || productInfo.ContainsKey(ExpiryDateOverrideIdentifier))
      {
        int trialPeriodDays = GetIntProductInfo(productInfo, TrialLicenseIdentifier, DefaultTrialPeriodDays);
        int gracePeriodDays = GetIntProductInfo(productInfo, GracePeriodIdentifier, DefaultGracePeriodDays);
        DateTime? expiryDateOverride = GetDateTimeProductInfo(productInfo, ExpiryDateOverrideIdentifier);

        EvaluationMonitor em = new EvaluationMonitor(_type.FullName);
        int daysInUse = em.DaysInUse;

        if (expiryDateOverride.HasValue && DateTime.Today <= expiryDateOverride.Value.AddDays(-gracePeriodDays))
        {
          int daysRemaining = (int)((expiryDateOverride.Value - DateTime.Today).TotalDays);
          return LicenseActions.Trial(daysRemaining);
        }
        else if (expiryDateOverride.HasValue && DateTime.Today <= expiryDateOverride.Value)
        {
          int daysRemaining = (int)((expiryDateOverride.Value - DateTime.Today).TotalDays);
          return LicenseActions.Grace(daysRemaining, expiryDateOverride.Value);
        }
        else if (daysInUse <= trialPeriodDays)
        {
          int daysRemaining = trialPeriodDays + gracePeriodDays - daysInUse;
          return LicenseActions.Trial(daysRemaining);
        }
        else if (daysInUse <= trialPeriodDays + gracePeriodDays)
        {
          int daysRemaining = trialPeriodDays + gracePeriodDays - daysInUse;
          DateTime expiryDate = em.FirstUseDate.AddDays(trialPeriodDays + gracePeriodDays);
          return LicenseActions.Grace(daysRemaining, expiryDate);
        }
        else
        {
          DateTime expiryDate = expiryDateOverride ?? em.FirstUseDate.AddDays(trialPeriodDays + gracePeriodDays);
          return LicenseActions.Expired(expiryDate, _type);
        }
      }
      else
      {
        return LicenseActions.Unrestricted;
      }
    }

    private static void PerformLicenseChecks(Assembly assembly)
    {
      EncryptedLicenseProvider.SetParameters(AssemblyLicense.ValidationParameters);

      if (_license == null)
      {
        if (_licenseKeyToUse != null)
        {
          _license = _provider.ValidateLicenseKey(AssemblyLicense.ValidationParameters, _licenseKeyToUse);
        }
        else
        {
          bool isValid = LicenseManager.IsValid(_type, _instance, out _license);
          if (!isValid)
          {
            _license = _provider.GetLicense(LicenseManager.CurrentContext, assembly, _type);
            if (_license == null)
            {
              throw new LicenseException(_type);
            }
          }
        }
      }

      EncryptedLicense el = _license as EncryptedLicense;
      if (el != null)
      {
        if (!_doneOneOffCheck)
        {
          _doneOneOffCheck = true;
          Dictionary<string, string> productInfo = ParseProductInfo(el.ProductInfo);
          Action licenseAction = GetLicenseAction(productInfo);
          licenseAction();
        }
      }
      else
      {
        throw new LicenseException(_type);
      }

#if DEBUG
      if (el == null)
      {
        Debug.WriteLine("*****************************************************************************************");
        Debug.WriteLine("*** License for " + _type.Name + " was unexpectedly null");
        Debug.WriteLine("*** Have you forgotten to apply LicenseProviderAttribute?");
        Debug.WriteLine("*****************************************************************************************");
        Debugger.Break();
      }
#endif
    }

    private static void CleanupLicense(object sender, RoutedEventArgs e)
    {
      FrameworkElement instance = sender as FrameworkElement;
      if (instance != null)
      {
        instance.Unloaded += CleanupLicense;
      }
      if (_license != null)
      {
        _license.Dispose();
        _license = null;
      }
    }

    internal static string GenerateRuntimeLicense(string designTimeLicense)
    {
      EncryptedLicenseProvider.SetParameters(AssemblyLicense.ValidationParameters);
      string runtimeLicenseKey = new LicenseGenerationHelper().CreateRuntimeLicense(designTimeLicense);
      return runtimeLicenseKey;
    }

    private class LicenseGenerationHelper : Infralution.Licensing.EncryptedLicenseProvider
    {
      internal string CreateRuntimeLicense(string designTimeLicense)
      {
        string runtimeKey = null;
        var license = ValidateLicenseKey(designTimeLicense, LicenseUsageMode.Designtime, true, ref runtimeKey);
        if (license == null)
        {
          throw new LicenseException(null);
        }
        return runtimeKey;
      }
    }
  }

  /// <summary>
  /// The core licensing object of WPF Elements.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class WpfElementsCore : FrameworkElement
  {
    /// <summary>
    /// This class is not for intended use outside the WPF Elements framework.
    /// </summary>
    public WpfElementsCore()
    {
      LicenseHelper.Attach(this, null);
    }

    internal WpfElementsCore(Assembly assembly)
    {
      LicenseHelper.Attach(this, assembly);
    }
  }
}
