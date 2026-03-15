using System;
using System.ComponentModel;
using Mindscape.WpfElements;

namespace Infralution.Licensing
{
  /// <summary>
  /// Forwards licensing requests to the internal provider.
  /// </summary>
  public class PublicEncryptedLicenseProvider : LicenseProvider
  {
    private readonly EncryptedLicenseProvider _impl;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicEncryptedLicenseProvider"/> class.
    /// </summary>
    public PublicEncryptedLicenseProvider()
    {
      _impl = new EncryptedLicenseProvider();
    }

    /// <summary>
    /// Get a license (if installed) for the given component/control type 
    /// </summary>
    /// <param name="context">The context (design or runtime)</param>
    /// <param name="type">The type to get the license for</param>
    /// <param name="instance">The object the license is for</param>
    /// <param name="allowExceptions">If true a <see cref="LicenseException"/> is thrown if a valid license cannot be loaded</param>
    /// <returns>An encrypted license</returns>
    public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
    {
      return _impl.GetLicense(context, type, instance, allowExceptions);
    }
  }
}
