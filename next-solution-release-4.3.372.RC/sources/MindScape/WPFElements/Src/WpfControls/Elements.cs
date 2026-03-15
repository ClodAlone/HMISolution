using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides assembly-level helper methods.
  /// </summary>
  public static class Elements
  {
    /// <summary>
    /// Performs manual licensing for specialized deployment scenarios.
    /// </summary>
    /// <param name="licenseKey">A runtime license key.</param>
    /// <remarks>In normal deployment scenarios, WPF Elements controls are
    /// automatically licensed.  Install a license key manually only under advisement
    /// from Mindscape support.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void InstallLicense(string licenseKey)
    {
      LicenseHelper.SetLicenseKeyToUse(licenseKey);
    }
  }
}
