
//
//      FILE:   EncryptedLicenseProvider.cs.
//
// COPYRIGHT:   Copyright 2008 
//              Infralution
//
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Security;
namespace Infralution.Licensing
{

    /// <summary>
    /// The parameters used to generate and validate <see cref="EncryptedLicense">EncryptedLicenses</see>
    /// using an <see cref="EncryptedLicenseProvider"/>
    /// </summary>
    /// <seealso cref="EncryptedLicenseProvider"/>
#if PUBLIC_LICENSE_CLASS  // if true allows enum to be visible outside library
    public
#endif
  [System.Runtime.CompilerServices.CompilerGenerated]  // suppress FxCop warnings on external code that we don't want to modify
  class EncryptedLicenseParameters
    {
        private string _productName;
        private string _productPassword;
        private int _keyStrength = 7;
        private bool _checksumProductInfo = false;
        private TextEncoding _textEncoding = TextEncoding.Hex;
        private bool _shortSerialNo = true;
        private RSACryptoServiceProvider _rsaProvider = LicenseUtilities.CreateRSACryptoServiceProvider();
        private byte[] _designSignature;
        private byte[] _runtimeSignature;

        /// <summary>
        /// The name of the product being licensed
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        /// <summary>
        /// The password used to encrypt the license data
        /// </summary>
        /// <remarks>
        /// The <see cref="KeyStrength"/> determines the number of characters of the ProductPassword that are
        /// actually used in generating keys.   If the ProductPassword is shorter than the <see cref="KeyStrength"/> then 
        /// it is padded.
        /// </remarks>
        public string ProductPassword
        {
            get { return _productPassword; }
            set
            {
                if (value != _productPassword)
                {
                    _productPassword = value;

                    // if the password is changed then also use a new RSA key for validating and
                    // force the signatures to be recreated
                    //
                    _rsaProvider = LicenseUtilities.CreateRSACryptoServiceProvider();
                    _designSignature = null;
                    _runtimeSignature = null;
                }
            }
        }

        /// <summary>
        /// The strength of the key to generate.   
        /// </summary>
        /// <remarks>
        /// The KeyStrength determines the number of characters of the <see cref="ProductPassword"/> that are
        /// actually used in generating keys.    The smaller the KeyStrength the shorter the generated keys.
        /// If the <see cref="ProductPassword"/> is shorter than the KeyStrength then it is padded.
        /// </remarks>
        public int KeyStrength
        {
            get { return _keyStrength; }
            set
            {
                if (value < 7 || value > 63) throw new ArgumentOutOfRangeException("KeyStrength", "KeyStrength must be in the range 7 to 63");
                if (value != _keyStrength)
                {
                    // force the signatures to be recreated
                    //
                    _designSignature = null;
                    _runtimeSignature = null;
                    _keyStrength = value;
                }
            }
        }

        /// <summary>
        /// The encoding used to convert the binary key to text
        /// </summary>
        public TextEncoding TextEncoding
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        /// <summary>
        /// Should a checksum of the <see cref="EncryptedLicense.ProductInfo"/> be included in the key  
        /// </summary>
        /// <remarks>
        /// If true a checksum is included in generated keys to check that the contents of the 
        /// <see cref="EncryptedLicense.ProductInfo"/> are valid.  This is only necessary if the 
        /// ProductInfo is potentially more than 6 characters long.  For ProductInfo of less than 7 
        /// characters the block encryption algorithm used to encrypt the overall key guarantees the validity
        /// of the ProductInfo.  
        /// </remarks>
        public bool ChecksumProductInfo
        {
            get { return _checksumProductInfo; }
            set { _checksumProductInfo = value; }
        }

        /// <summary>
        /// If true serial numbers must be less than <see cref="UInt16.MaxValue"/>.  
        /// </summary>
        /// <remarks>
        /// Setting this to true enables the generated key to be kept as short as possible.   The default
        /// value for backward compatibility with previous versions is true.
        /// </remarks>
        public bool ShortSerialNo
        {
            get { return _shortSerialNo; }
            set { _shortSerialNo = value; }
        }

        /// <summary>
        /// Return the maximum serial no.
        /// </summary>
        /// <remarks>
        /// This returns the maximum allowed serial no based on the value of the <see cref="ShortSerialNo"/>
        /// property.
        /// </remarks>
        public int MaxSerialNo
        {
            get 
            { 
                return (_shortSerialNo) ? UInt16.MaxValue : Int32.MaxValue; 
            }
        }

        /// <summary>
        /// Return the RSA Provider used to validate RSA signatures
        /// </summary>
        internal RSACryptoServiceProvider RSAProvider
        {
            get { return _rsaProvider; }
        }

        /// <summary>
        /// The RSA signature for the product password at design time
        /// </summary>
        internal byte[] DesignSignature
        {
            get 
            {
                if (_designSignature == null)
                {
                    EncryptedLicenseProvider.CreateSignatures(this);
                }
                return _designSignature; 
            }
            set { _designSignature = value; }
        }

        /// <summary>
        /// The RSA signature for the product password at runtime
        /// </summary>
        internal byte[] RuntimeSignature
        {
            get 
            {
                if (_runtimeSignature == null)
                {
                    EncryptedLicenseProvider.CreateSignatures(this);
                }
                return _runtimeSignature; 
            }
            set { _runtimeSignature = value; }
        }

        /// <summary>
        /// Read the parameters from an XML string
        /// </summary>
        /// <param name="xmlParameters"></param>
        public virtual void ReadFromString(string xmlParameters)
        {
            XmlReader reader = new XmlTextReader(xmlParameters, XmlNodeType.Element, null);
            Read(reader);
            reader.Close();
        }

        /// <summary>
        /// Write the parameters to an XML string
        /// </summary>
        /// <param name="includeGenerationParameters">Should parameters required for generating keys be included</param>
        /// <returns>The parameters in a formatted XML string</returns>
        public virtual string WriteToString(bool includeGenerationParameters)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            Write(xmlWriter, includeGenerationParameters);
            xmlWriter.Close();
            string result = stringWriter.ToString();
            stringWriter.Close();
            return result;
        }


        /// <summary>
        /// Read the parameters from an XML Reader
        /// </summary>
        /// <param name="reader"></param>
        public virtual void Read(XmlReader reader)
        {
            reader.ReadStartElement();
            while (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "ProductName":
                        _productName = reader.ReadElementString();
                        break;
                    case "ProductPassword":
                        _productPassword = reader.ReadElementString();
                        break;
                    case "RSAKeyValue":
                        LicenseUtilities.ReadRSAParameters(_rsaProvider, reader, "RSAKeyValue");
                        break;
                    case "DesignSignature":
                        _designSignature = LicenseUtilities.ReadElementBase64(reader, "DesignSignature");
                        break;
                    case "RuntimeSignature":
                        _runtimeSignature = LicenseUtilities.ReadElementBase64(reader, "RuntimeSignature");
                        break;
                    case "KeyStrength":
                        _keyStrength = int.Parse(reader.ReadElementString(), CultureInfo.InvariantCulture);
                        break;
                    case "ChecksumProductInfo":
                        _checksumProductInfo = bool.Parse(reader.ReadElementString());
                        break;
                    case "TextEncoding":
                        _textEncoding = (TextEncoding)Enum.Parse(typeof(TextEncoding), reader.ReadElementString());
                        break;
                    case "ShortSerialNo":
                        _shortSerialNo = bool.Parse(reader.ReadElementString());
                        break;
                    default:
                        string error = "Unexpected XML Element: {0}";
                        throw new XmlSyntaxException(string.Format(error, reader.Name));
                }
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// Write the parameters to an XML Writer
        /// </summary>
        /// <param name="writer">The XML Writer to write to</param>
        /// <param name="includeGenerationParameters">Should parameters required for generating keys be included</param>
        public virtual void Write(XmlWriter writer, bool includeGenerationParameters)
        {
            writer.WriteStartElement("EncryptedLicenseParameters");
            writer.WriteElementString("ProductName", ProductName);
            if (includeGenerationParameters)
            {
                writer.WriteElementString("ProductPassword", ProductPassword);
            }
            LicenseUtilities.WriteRSAParameters(RSAProvider, writer, "RSAKeyValue", includeGenerationParameters);
            writer.WriteElementString("DesignSignature", Convert.ToBase64String(DesignSignature));
            writer.WriteElementString("RuntimeSignature", Convert.ToBase64String(RuntimeSignature));
            writer.WriteElementString("KeyStrength", KeyStrength.ToString(CultureInfo.InvariantCulture));
            if (ChecksumProductInfo)
            {
                writer.WriteElementString("ChecksumProductInfo", ChecksumProductInfo.ToString(CultureInfo.InvariantCulture));
            }
            if (TextEncoding != TextEncoding.Hex)
            {
                writer.WriteElementString("TextEncoding", TextEncoding.ToString());
            }
            if (!ShortSerialNo)
            {
                writer.WriteElementString("ShortSerialNo", ShortSerialNo.ToString(CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Defines a .NET LicenseProvider that generates and validates simple, secure 
    /// <see cref="EncryptedLicense">EncryptedLicenses</see>.
    /// </summary>
    /// <remarks>
    /// The EncryptedLicenseProvider generates simple license keys which are validated using
    /// a public key encryption algorithm to minimize the possibility of cracking.  See 
    /// <see href="XtraGettingStarted.html">Getting Started</see> for detailed information on using
    /// the EncryptedLicenseProvider to license applications and components.
    /// </remarks>
    /// <seealso cref="EncryptedLicense"/>
#if PUBLIC_LICENSE_CLASS  // if true allows class to be visible outside library  
    public
#endif
  [System.Runtime.CompilerServices.CompilerGenerated]  // suppress FxCop warnings on external code that we don't want to modify
  class EncryptedLicenseProvider : LicenseProvider
	{

        #region Member Variables

    private const string Base32Chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"; // TODO: what is this for? Is it used?

        private static byte[] _desKey = new byte []  { 0x92, 0x15, 0x38, 0xA1, 0x12, 0xED, 0xB3, 0xC2 };
        private static byte[] _desIV = new byte []  { 0xAD, 0x3F, 0xC6, 0x11, 0x47, 0x90, 0xDD, 0xA1 };
       
        /// <summary>
        /// The current parameters for validating licenses
        /// </summary>
        private static EncryptedLicenseParameters _parameters;

        #endregion

        #region Public Interface

        /// <summary>
        /// Set the parameters used to validate licenses created by this provider.
        /// </summary>
        /// <remarks>
        /// This must be called by the client software prior to obtaining licenses using the EncryptedLicenseProvider.
        /// The XML parameter string is generated using the Infralution License Key Generator and pasted into the calling client code
        /// or by calling <see cref="EncryptedLicenseParameters.WriteToString"/>
        /// </remarks>
        /// <param name="xmlParameters">An XML string containing parameters used to validate licenses</param>
        public static void SetParameters(string xmlParameters)
        {
            EncryptedLicenseParameters parameters = new EncryptedLicenseParameters();
            parameters.ReadFromString(xmlParameters);
            _parameters = parameters;
        }

        /// <summary>
        /// Set/Get Parameters for validating <see cref="EncryptedLicense">EncryptedLicenses</see>  
        /// </summary>
        public static EncryptedLicenseParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        /// Generate a new encrypted license using the given parameters
        /// </summary>
        /// <param name="parameters">The license parameters to use to generate the key</param>
        /// <param name="productInfo">User defined data to be included in license key</param>
        /// <param name="serialNo">The unique license serial number for the</param>
        /// <returns>An encrypted license key</returns>
        /// <remarks>
        /// If there is no installed license for the Infralution Licensing System then the only 
        /// allowed password is "TEST" and the only allowed serial numbers are 1 or 0.  
        /// </remarks>
        public virtual string GenerateKey(EncryptedLicenseParameters parameters, 
                                          string productInfo, 
                                          Int32 serialNo)
        {
            if (parameters == null) throw new ArgumentNullException("parameters");
            if (parameters.ProductPassword == null) throw new InvalidOperationException("Parameters.ProductPassword MUST be non-null");
            if (productInfo == null) productInfo = "";

            byte[] passwordData = GetPasswordData(parameters.ProductPassword, parameters.KeyStrength); 
            byte[] productInfoData = ASCIIEncoding.UTF8.GetBytes(productInfo);
 
            #if CHECK_ILS_LICENSE

            // if the Licensing System is not licensed then we need to check the password and serial no
            //
            if (LicenseUtilities.ILSLicense == null)
            {
                const string passwordErrorMsg = "The only allowable password in evaluation mode is 'TEST'";
                const string serialNoErrorMsg = "The only allowable serial numbers in evaluation mode are '0' or '1'";

                if (parameters.ProductPassword != "TEST")
                    throw new LicenseException(typeof(EncryptedLicenseProvider), this, passwordErrorMsg);
                if (serialNo < 0 || serialNo > 1)
                    throw new LicenseException(typeof(EncryptedLicenseProvider), this, serialNoErrorMsg);
            }

            #endif

            return GenerateKey(parameters, passwordData, productInfoData, serialNo);
        }

        /// <summary>
        /// Generate a runtime license key from the given design time license key
        /// </summary>
        /// <param name="designTimeLicenseKey">The design time license key to use</param>
        /// <returns>A runtime license key (or null if the designTimeLicenseKey can't be validated)</returns>
        /// <remarks>
        /// The <see cref="SetParameters"/> method MUST be called before using this method.  
        /// </remarks>
        public virtual string GenerateRuntimeKey(string designTimeLicenseKey)
        {
            string runtimeLicenseKey = null;
            ValidateLicenseKey(designTimeLicenseKey, LicenseUsageMode.Designtime, true, ref runtimeLicenseKey);
            return runtimeLicenseKey;
        }
        
        /// <summary>
        /// Install a license key for the given component or control type.
        /// </summary>
        /// <remarks>
        /// This method is used to install licenses for components and controls.  The <see cref="InstallLicense(string, EncryptedLicense)"/>
        /// method is typically better for installing application licenses because it provides more control over the 
        /// license key file name.  This license key file used by this method is the full type name followed by a ".lic" suffix. 
        /// </remarks>
        /// <param name="type">The type to install the license for</param>
        /// <param name="license">The license to install</param>
        public virtual void InstallLicense(Type type, EncryptedLicense license)
        {
            if (license == null) throw new ArgumentNullException("license");
            string licenseFile = GetLicenseFilePath(LicenseManager.CurrentContext, type);
            WriteKeyToFile(licenseFile, license.LicenseKey);
        }

        /// <summary>
        /// Install a license key for the given component or control type.
        /// </summary>
        /// <remarks>
        /// Validates the given license key and then installs the license.
        /// This method is an alternative to calling <see cref="ValidateLicenseKey(string)"/> and then
        /// <see cref="InstallLicense(Type, EncryptedLicense)"/>.
        /// </remarks>
        /// <param name="type">The type to install the license for</param>
        /// <param name="licenseKey">The license key to install</param>
        /// <returns>A license if succesful or null/nothing if not</returns>
        public virtual EncryptedLicense InstallLicense(Type type, string licenseKey)
        {
            EncryptedLicense license = ValidateLicenseKey(licenseKey);
            if (license != null)
            {
                InstallLicense(type, license);
            }
            return license;
        }

        /// <summary>
        /// Install a license key for an application in the given file.
        /// </summary>
        /// <remarks>
        /// This method is used to install licenses for applications.  Use the <see cref="InstallLicense(Type, EncryptedLicense)"/>
        /// method to install licenses for components or controls.  If a full path is not specified for licenseFile then
        /// the file will be created relative to the entry executable directory.
        /// </remarks>
        /// <param name="licenseFile">The name of the file to install the license key in</param>
        /// <param name="license">The license to install</param>
        public virtual void InstallLicense(string licenseFile, EncryptedLicense license)
        {
            if (license == null) throw new ArgumentNullException("license");
            string baseDir = GetLicenseDirectory(LicenseManager.CurrentContext, null);
            string path = System.IO.Path.Combine(baseDir, licenseFile);
            WriteKeyToFile(path, license.LicenseKey);
        }

        /// <summary>
        /// Install a license key for an application in the given file.
        /// </summary>
        /// <remarks>
        /// Validates the given license key and then installs the license.
        /// This method is an alternative to calling <see cref="ValidateLicenseKey(string)"/> and then
        /// <see cref="InstallLicense(string, EncryptedLicense)"/>.
        /// </remarks>
        /// <param name="licenseFile">The name of the file to install the license key in</param>
        /// <param name="licenseKey">The license key to install</param>
        /// <returns>A license if succesful or null/nothing if not</returns>
        public virtual EncryptedLicense InstallLicense(string licenseFile, string licenseKey)
        {
            EncryptedLicense license = ValidateLicenseKey(licenseKey);
            if (license != null)
            {
                InstallLicense(licenseFile, license);
            }
            return license;
        }

        /// <summary>
        /// Uninstall a license key for the given component or control type.
        /// </summary>
        /// <remarks>
        /// Deletes the license file for the given type
        /// </remarks>
        /// <param name="type">The type to uninstall the license for</param>
        public virtual void UninstallLicense(Type type)
        {
            string licenseFile = GetLicenseFilePath(LicenseManager.CurrentContext, type);
            LicenseUtilities.UninstallLicenseFile(licenseFile);
        }

        /// <summary>
        /// Uninstall the license key in the given file.
        /// </summary>
        /// <remarks>
        /// Deletes the license file
        /// </remarks>
        /// <param name="licenseFile">The name of the file the license key is in</param>
        public virtual void UninstallLicense(string licenseFile)
        {
            string baseDir = GetLicenseDirectory(LicenseManager.CurrentContext, null);
            string path = System.IO.Path.Combine(baseDir, licenseFile);
            LicenseUtilities.UninstallLicenseFile(path);
        }

        /// <summary>
        /// Check  that the given license key is valid
        /// </summary>
        /// <param name="licenseKey">The license key to validate</param>
        /// <param name="context">The current licensing context</param>
        /// <param name="type">The type to be licensed</param>
        /// <returns>An <see cref="EncryptedLicense"/> or null if licenseKey is not valid</returns>
        /// <remarks>
        /// <para>
        /// This method is called to validate the license key for a type.  If the license context is a design
        /// time context then it generates a runtime license key and saves it in the context.
        /// </para>
        /// <para>
        /// The <see cref="SetParameters"/> method MUST be called before using this method.  
        /// </para>
        /// </remarks>
        public virtual EncryptedLicense ValidateLicenseKey(string licenseKey, LicenseContext context, Type type)
        {
            string runtimeLicenseKey = null;
            bool generateRuntimeLicenseKey = (context.UsageMode == LicenseUsageMode.Designtime && type != null);
            EncryptedLicense license = ValidateLicenseKey(licenseKey, context.UsageMode, generateRuntimeLicenseKey, ref runtimeLicenseKey);
            if (runtimeLicenseKey != null)
            {
                // save the runtime key into the context
                //
                context.SetSavedLicenseKey(type, runtimeLicenseKey);
            }
            return license;
        }

        /// <summary>
        /// Validate that the given license key is valid for the current licensing parameters
        /// </summary>
        /// <param name="licenseKey">The license key to validate</param>
        /// <returns>The encrypted license if the key is valid otherwise null</returns>
        /// <remarks>
        /// <para>
        /// This method provides a mechanism to validate that a given license key is valid
        /// prior to attempting to install it.   This can be useful if you want to check
        /// the <see cref="EncryptedLicense.ProductInfo"/> before installing the license.
        /// </para>
        /// <para>
        /// The <see cref="SetParameters"/> method MUST be called before using this method.  
        /// </para>
        /// </remarks>
        /// <seealso cref="ValidateLicenseKey(string, string)"/> 
        public EncryptedLicense ValidateLicenseKey(string licenseKey)
        {
            string runtimeLicenseKey = null;
            return ValidateLicenseKey(licenseKey, LicenseManager.CurrentContext.UsageMode, false, ref runtimeLicenseKey);
        }

        /// <summary>
        /// Validate that the given license key is valid for the given licensing parameters
        /// </summary>
        /// <param name="licenseParameters">An XML string containing parameters used to validate the license key</param>
        /// <param name="licenseKey">The license key to validate</param>
        /// <returns>The encrypted license if the key is valid otherwise null</returns>
        /// <remarks>
        /// <para>
        /// This method provides a mechanism to validate that a given license key is valid
        /// prior to attempting to install it.   This can be useful if you want to check
        /// the <see cref="EncryptedLicense.ProductInfo"/> before installing the license.  
        /// </para>
        /// <para>
        /// This method is an alternative to calling <see cref="SetParameters"/> followed by 
        /// <see cref="ValidateLicenseKey(string)"/>.
        /// </para>
        /// </remarks>
        public EncryptedLicense ValidateLicenseKey(string licenseParameters,
                                                   string licenseKey)
        {
            SetParameters(licenseParameters);
            return ValidateLicenseKey(licenseKey);
        }

        /// <summary>
        /// Get a license (if installed) from the given license file.
        /// </summary>
        /// <param name="licenseFile">The name of the license file containing the license key</param>
        /// <returns>The installed license if any</returns>
        /// <remarks>
        /// <para>
        /// This method is used to read licenses for applications.  Components and controls should use the 
        /// <see cref="LicenseManager"/> methods to load and validate licenses.  If a full path is not specified 
        /// for licenseFile then the file loaded will be relative to the directory containing the application 
        /// executable (for Window Forms applications) or aspx files (for ASP.NET applications).
        /// </para>
        /// <para>
        /// The <see cref="SetParameters"/> method MUST be called before using this method.  
        /// </para>
        /// </remarks>
        public virtual EncryptedLicense GetLicense(string licenseFile)
        {
            string dir = GetLicenseDirectory(LicenseManager.CurrentContext, null);
            string path = System.IO.Path.Combine(dir, licenseFile);             
            string licenseKey = ReadKeyFromFile(path);
            return ValidateLicenseKey(licenseKey);
        }
 
        /// <summary>
        /// Get a license (if installed) from the given license file.
        /// </summary>
        /// <param name="licenseParameters">An XML string containing parameters used to validate the license key</param>
        /// <param name="licenseFile">The name of the license file containing the license key</param>
        /// <returns>The installed license if any</returns>
        /// <remarks>
        /// <para>
        /// This method is used to read licenses for applications.  Components and controls should use the 
        /// <see cref="LicenseManager"/> methods to read and validate licenses.  If a full path is not specified 
        /// for licenseFile then the file loaded will be relative to the directory containing the application 
        /// executable (for Window Forms applications) or aspx files (for ASP.NET applications).
        /// </para>
        /// <para>
        /// This method is an alternative to calling <see cref="SetParameters"/> followed by 
        /// <see cref="GetLicense(string)"/>.
        /// </para>
        /// </remarks>
        public EncryptedLicense GetLicense(string licenseParameters,
                                           string licenseFile)
        {
            SetParameters(licenseParameters);
            return GetLicense(licenseFile);
        }
                                        
        /// <summary>
        /// Get a license (if installed) for the given component/control type 
        /// </summary>
        /// <param name="context">The context (design or runtime)</param>
        /// <param name="type">The type to get the license for</param>
        /// <param name="instance">The object the license is for</param>
        /// <param name="allowExceptions">If true a <see cref="LicenseException"/> is thrown if a valid license cannot be loaded</param>
        /// <returns>An encrypted license</returns>
        /// <remarks>
        /// <para>
        /// This method is used to get licenses for components and controls.  Applications should generally
        /// use the <see cref="GetLicense(string, string)"/> method as it provides more control over the license file
        /// that keys are stored in.  This method is not typically called directly by application code.  
        /// Instead the component or control uses the <see cref="LicenseManager.IsValid(Type)"/> or
        /// <see cref="LicenseManager.Validate(Type)"/> methods which find the <see cref="LicenseProvider"/> for the type
        /// and call this method.
        /// </para>
        /// <para>
        /// You must call <see cref="SetParameters"/> before calling this method either directly or
        /// indirectly by via a call to <see cref="LicenseManager.IsValid(Type)"/>
        /// </para>
        /// </remarks>
        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            EncryptedLicense license = null;
            if (context.UsageMode == LicenseUsageMode.Runtime)
            {
                try
                {
                    string key = context.GetSavedLicenseKey(type, null);
                    license = ValidateLicenseKey(key, context, type);
                }
                catch
                {
                    // if something goes wrong retrieving the saved license key then just ignore it
                    // and try reading from file                    
                }
            }

            if (license == null)
            {
                // if we're in design mode or a suitable license key wasn't found in 
                // the runtime context try to read a license from file
                //
                string key = ReadKeyFromFile(GetLicenseFilePath(context, type));
                license = ValidateLicenseKey(key, context, type);
            }

            if (license == null && allowExceptions)
            {
                throw new LicenseException(type, instance, "No License Installed");
            }
            return license;
        }

        /// <summary>
        /// Return the license for the given type from a given DLL assembly resources
        /// </summary>
        /// <param name="context">The license context to validate the license in</param>
        /// <param name="assembly">The assembly containing the license</param>
        /// <param name="type">The type to get the license for</param>
        /// <returns>The license key if any</returns>
        /// <remarks>
        /// This method can be used to check the given DLL assembly for a license.  By default the .NET licensing
        /// framework only checks the entry assembly (ie typically executables) for licenses.  This means
        /// that if a licensed control is wrapped in another control, the customer of the wrapped control will
        /// still required a design time license key for the original control.  This is generally the behavior
        /// that control authors would like.  If however you want to provide a license that enables a customer
        /// to create new component/controls using your control/component then you can achieve this by using 
        /// this method to check for a license in the CallingAssembly that created the control/component.
        /// </remarks>
        public virtual EncryptedLicense GetLicense(LicenseContext context, Assembly assembly, Type type)
        {
            if (assembly == null) return null;
            string licenseKey = LicenseUtilities.GetSavedLicenseKey(assembly, type);
            return ValidateLicenseKey(licenseKey, context, type);
        }

        #endregion

        #region Local Methods

        /// <summary>
        /// Converts a byte array into a text representation.
        /// </summary>
        /// <param name="data">The byte data to convert</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>Text representation of the data</returns>
        internal protected virtual string EncodeToText(byte[] data, TextEncoding encoding)
        {
            // encrypt the overall license key using the preset encryption key to obscure the password
            //
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey;
            des.IV = _desIV;
            byte[] encData = des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            return LicenseUtilities.EncodeToText(encData, encoding);
        }

        /// <summary>
        /// Converts a string into a byte array.
        /// </summary>
        /// <param name="text">The text to convert</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>The converted byte data</returns>
        internal protected virtual byte[] DecodeFromText(string text, TextEncoding encoding)
        {
             byte[] encData = LicenseUtilities.DecodeFromText(text, encoding);

             //  decrypt the overall license key using the preset encryption key 
             //
             DESCryptoServiceProvider des = new DESCryptoServiceProvider();
             des.Key = _desKey;
             des.IV = _desIV;
             return des.CreateDecryptor().TransformFinalBlock(encData, 0, encData.Length);
        }

        /// <summary>
        /// Generate the password data used to verify and decrypt the license
        /// </summary>
        /// <param name="password">The password used to generate the key</param>
        /// <param name="keyStrength">The strength of the key to create</param>
        /// <returns>The password data used to verify and decrypt the license</returns> 
        private static byte[] GetPasswordData(string password, int keyStrength)
        {
            byte[] key = new byte []  { 0xF2, 0xA1, 0x03, 0x9D, 0x63, 0x87, 0x35, 0x5E };
            byte[] iv = new byte []  { 0xAB, 0xB8, 0x94, 0x7E, 0x1D, 0xE5, 0xD1, 0x33 };

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = key;
            des.IV = iv;

            int padLength = Math.Max(keyStrength, 8); 
            if (password.Length < padLength) 
                password = password.PadRight(padLength, '*');
            byte[] data = ASCIIEncoding.ASCII.GetBytes(password);
            byte[] encPassword = des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            byte[] result = new byte[LicenseUtilities.ArraySize(keyStrength)];
            Array.Copy(encPassword, 0, result, 0, keyStrength);
            return result;
        }

        /// <summary>
        /// Pad the given password if required.
        /// </summary>
        /// <param name="passwordData">The password data to pad</param>
        /// <returns>The padded password data</returns> 
        /// <remarks>
        /// This function is required for backward compatibility with 7 byte passwords which were
        /// padded before being signed
        /// </remarks>
        private static byte[] PadPassword(byte[] passwordData)
        {
            if (passwordData.Length == 7)
            {
                byte[] result = new byte[LicenseUtilities.ArraySize(8)];
                Array.Copy(passwordData, 0, result, 0, passwordData.Length);
                return result;
            }
            return passwordData;
        }

        /// <summary>
        /// Create the signatures based on the current parameters
        /// </summary>
        internal static void CreateSignatures(EncryptedLicenseParameters parameters)
        {
            byte[] designPassword = PadPassword(GetPasswordData(parameters.ProductPassword, parameters.KeyStrength));
            parameters.DesignSignature = LicenseUtilities.SignData(parameters.RSAProvider, designPassword);

            // encrypt the password using itself to produce the runtime password 
            //
            byte[] encryptionKey = new byte[LicenseUtilities.ArraySize(8)];
            Array.Copy(designPassword, 0, encryptionKey, 0, 7);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey;
            des.IV = encryptionKey;
            byte[] encKey = des.CreateEncryptor().TransformFinalBlock(designPassword, 0, designPassword.Length);

            byte[] runtimePassword = new byte[LicenseUtilities.ArraySize(parameters.KeyStrength)];
            Array.Copy(encKey, 0, runtimePassword, 0, parameters.KeyStrength);
            runtimePassword = PadPassword(runtimePassword);

            // sign the runtime key
            //
            parameters.RuntimeSignature = LicenseUtilities.SignData(parameters.RSAProvider, runtimePassword);

        }

        /// <summary>
        /// Generate a new encrypted license 
        /// </summary>
        /// <param name="parameters">The license parameters to use to generate the key</param>
        /// <param name="productPassword">The password used to encrypted the license data</param>
        /// <param name="productInfo">User defined data to be included in license key</param>
        /// <param name="serialNo">The unique license serial number</param>
        /// <returns>An encrypted license key</returns>
        /// <remarks>
        /// If there is no installed license for the Infralution Licensing System then the only 
        /// allowed password is "TEST" and the only allowed serial numbers are 1 or 0.  To use the
        /// licensed version of this method ensure that the file Infralution.Licensing.EncryptedLicenseProvider.lic
        /// exists in the same directory as the Infralution.Licensing.dll and contains a valid
        /// license key for the Licensing System.
        /// </remarks>
        private string GenerateKey(EncryptedLicenseParameters parameters, 
                                   byte[] productPassword, 
                                   byte[] productInfo, 
                                   Int32 serialNo)
        {

            // Public Key token for the Infralution signed assemblies
            //            
          // TODO: What are these for? Are they needed?
            byte[] requiredToken = { 0x3E, 0x7E, 0x8E, 0x37, 0x44, 0xA5, 0xC1, 0x3F };
            byte[] publicKeyToken = Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken();

            #if CHECK_PUBLIC_KEY

            // Validate this assembly - if it isn't signed with the correct public key
            // then copy rubbish into the key.  This is to make it just a little more
            // difficult for the casual hacker.
            //
            if (!LicenseUtilities.ArrayEqual(publicKeyToken, requiredToken))
            {
                _desKey.CopyTo(productPassword, 0);
            }
            #endif

            int checksumLength = (parameters.ChecksumProductInfo) ? sizeof(UInt16) : 0;
            int serialNoLength = (parameters.ShortSerialNo) ? sizeof(UInt16) : sizeof(Int32);

            byte[] clientData;
            if (serialNo < 0) throw new ArgumentOutOfRangeException("serialNo", "serialNo must be non-negative");
            if (parameters.ShortSerialNo)
            {
                if (serialNo > UInt16.MaxValue) throw new ArgumentOutOfRangeException("serialNo", "serialNo must be less than 65536");
                UInt16 userialNo = (UInt16)serialNo;
                clientData = BitConverter.GetBytes(userialNo);
            }
            else
            {
                clientData = BitConverter.GetBytes(serialNo);
            }

            byte[] payload = new byte[LicenseUtilities.ArraySize(productInfo.Length + serialNoLength + checksumLength)];

            clientData.CopyTo(payload, 0);
            productInfo.CopyTo(payload, serialNoLength);

            // calculate the product data checksum and add to the payload
            //
            if (parameters.ChecksumProductInfo)
            {
                UInt16 checksum = LicenseUtilities.Checksum(productInfo);
                byte[] checksumData = BitConverter.GetBytes(checksum);
                checksumData.CopyTo(payload, payload.Length - checksumLength);
            }

            // Encrypt the payload. The key used for encrypting the payload is just the first 7 bytes of the password data
            //
            byte[] encryptionKey = new byte[LicenseUtilities.ArraySize(8)];
            Array.Copy(productPassword, 0, encryptionKey, 0, 7);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey;
            des.IV = encryptionKey;
            byte[] encPayload = des.CreateEncryptor().TransformFinalBlock(payload, 0, payload.Length);

            // Combine the password data and encrypted payload 
            //
            byte[] data = new byte[LicenseUtilities.ArraySize(productPassword.Length + encPayload.Length)];

            // For key strengths greater than 8 we swap the order of the payload and password data
            // This ensures that the first 8 bytes do not always end up hex encoded the same
            //
            if (parameters.KeyStrength < 8)
            {
                productPassword.CopyTo(data, 0);
                encPayload.CopyTo(data, productPassword.Length);
            }
            else
            {
                encPayload.CopyTo(data, 0);
                productPassword.CopyTo(data, encPayload.Length);
            }

            // return the data encoded as a string
            //
            return EncodeToText(data, parameters.TextEncoding);
        }

        /// <summary>
        /// Check that the given license key is valid and optionally generate a runtime license key
        /// </summary>
        /// <param name="licenseKey">The license key to validate</param>
        /// <param name="usageMode">The usage mode that we want to validate the license key for</param>
        /// <param name="generateRuntimeLicenseKey">Should a runtime license be generated from the license - usageMode must also be DesignTime</param>
        /// <param name="runtimeLicenseKey">The generated runtime license (if any)</param>
        /// <returns>An <see cref="EncryptedLicense"/> or null if licenseKey is not valid</returns>
        /// <remarks>
        /// <para>
        /// This method implements the core validation logic (other ValidateLicenseKey methods call it) and optionally
        /// generates a runtime license key.
        /// </para>
        /// <para>
        /// The <see cref="SetParameters"/> method MUST be called before using this method.  
        /// </para>
        /// </remarks>
        protected virtual EncryptedLicense ValidateLicenseKey(string licenseKey, LicenseUsageMode usageMode, bool generateRuntimeLicenseKey, ref string runtimeLicenseKey)
        {

            // check that validation parameters have been set by the client
            //
            if (_parameters == null)
                throw new InvalidOperationException("EncryptedLicenseProvider.SetParameters must be called prior to using the EncryptedLicenseProvider");
            if (licenseKey == null || licenseKey.Trim().Length == 0) return null;

            try
            {
                byte[] data = DecodeFromText(licenseKey, _parameters.TextEncoding);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Key = _desKey;
                des.IV = _desIV;

                // extract the password data and encrypted product data 
                //
                byte[] passwordData = new byte[LicenseUtilities.ArraySize(_parameters.KeyStrength)];
                byte[] encPayload = new byte[LicenseUtilities.ArraySize(data.Length - _parameters.KeyStrength)];

                // for key strengths greater than 8 the order of payload and key is swapped
                //
                if (_parameters.KeyStrength < 8)
                {
                    Array.Copy(data, 0, passwordData, 0, _parameters.KeyStrength);
                    Array.Copy(data, _parameters.KeyStrength, encPayload, 0, encPayload.Length);
                }
                else
                {
                    Array.Copy(data, 0, encPayload, 0, encPayload.Length);
                    Array.Copy(data, encPayload.Length, passwordData, 0, _parameters.KeyStrength);
                }

                // the key used to encrypt payload is just the first 7 bytes of the password data
                //
                byte[] encryptionKey = new byte[LicenseUtilities.ArraySize(8)];
                Array.Copy(passwordData, 0, encryptionKey, 0, 7);

                // validate that the password matches what the client is expecting
                //
                byte[] paddedPasswordData = PadPassword(passwordData);
                if (usageMode == LicenseUsageMode.Designtime)
                {
                    // if design time license requested then the license MUST be a design license
                    //
                    if (!LicenseUtilities.VerifyData(_parameters.RSAProvider, paddedPasswordData, _parameters.DesignSignature))
                        return null;
                }
                else
                {
                    // if runtime license requested then first check if the license is a runtime license
                    // also allow design licenses to work at runtime
                    //
                    if (!LicenseUtilities.VerifyData(_parameters.RSAProvider, paddedPasswordData, _parameters.RuntimeSignature))
                    {
                        if (!LicenseUtilities.VerifyData(_parameters.RSAProvider, paddedPasswordData, _parameters.DesignSignature))
                            return null;
                    }
                }

                // decrypt the payload using the encryption key
                //
                des.IV = encryptionKey;
                byte[] payload = des.CreateDecryptor().TransformFinalBlock(encPayload, 0, encPayload.Length);

                int checksumLength = (_parameters.ChecksumProductInfo) ? sizeof(UInt16) : 0;
                int serialNoLength = (_parameters.ShortSerialNo) ? sizeof(UInt16) : sizeof(Int32);

                byte[] productData = new byte[LicenseUtilities.ArraySize(payload.Length - serialNoLength - checksumLength)];
                Array.Copy(payload, serialNoLength, productData, 0, productData.Length);

                Int32 serialNo;
                if (_parameters.ShortSerialNo)
                {
                    serialNo = BitConverter.ToUInt16(payload, 0);
                }
                else
                {
                    serialNo = BitConverter.ToInt32(payload, 0);
                }

                string productInfo = System.Text.ASCIIEncoding.UTF8.GetString(productData);

                // validate the product data checksum
                //
                if (_parameters.ChecksumProductInfo)
                {
                    UInt16 requiredChecksum = BitConverter.ToUInt16(payload, payload.Length - checksumLength);
                    UInt16 actualChecksum = LicenseUtilities.Checksum(productData);
                    if (requiredChecksum != actualChecksum)
                        return null;
                }

                // optionally generate a runtime license key (for design time licenses only)
                //
                if (usageMode == LicenseUsageMode.Designtime && generateRuntimeLicenseKey)
                {

                    // create the runtime password by encrypting the design password
                    //
                    byte[] encPassword = des.CreateEncryptor().TransformFinalBlock(paddedPasswordData, 0, paddedPasswordData.Length);
                    byte[] runtimePasswordData = new byte[LicenseUtilities.ArraySize(_parameters.KeyStrength)];
                    Array.Copy(encPassword, 0, runtimePasswordData, 0, _parameters.KeyStrength);

                    runtimeLicenseKey = GenerateKey(_parameters, runtimePasswordData, productData, serialNo);
                }
                return new EncryptedLicense(licenseKey, serialNo, productInfo);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read a license key from the given file
        /// </summary>
        /// <param name="licenseFile">The path to the license file to read the key from</param>
        /// <returns>The license key if any</returns>
        protected virtual string ReadKeyFromFile(string licenseFile)
        {
            string key = null;
            try
            {
                if (File.Exists(licenseFile))
                {
                    Stream stream = new FileStream(licenseFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader reader = new StreamReader(stream);
                    key = reader.ReadLine();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                if (!LicenseUtilities.HandleIOExceptions) throw;
                string msg = string.Format(LicenseResources.ReadErrorMsg, ex.Message, licenseFile);
                ShowError(LicenseResources.ReadErrorTitle, msg);
            }
            return key;
        }

        /// <summary>
        /// Write a license key to the given file
        /// </summary>
        /// <param name="licenseFile">The path to the license file to write the key to</param>
        /// <param name="licenseKey">The license key to write</param>
        protected virtual void WriteKeyToFile(string licenseFile, string licenseKey)
        {
            try
            {
                // create the directory containing the file if required
                //
                string dir = Path.GetDirectoryName(licenseFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                Stream stream = new FileStream(licenseFile, FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(licenseKey);
                writer.Close();
            }
            catch (Exception ex)
            {
                if (!LicenseUtilities.HandleIOExceptions) throw;
                string msg = string.Format(LicenseResources.WriteErrorMsg, ex.Message, licenseFile);
                ShowError(LicenseResources.WriteErrorTitle, msg);
            }
        }

        /// <summary>
        /// Return the directory used to store license files
        /// </summary>
        /// <param name="context">The license context</param>
        /// <param name="type">The type being licensed</param>
        /// <returns>The directory to look for license files</returns>
        protected virtual string GetLicenseDirectory(LicenseContext context, Type type)
        {
            return LicenseUtilities.DefaultLicenseDirectory(context, type);
        }

        /// <summary>
        /// Called by <see cref="GetLicense(string)"/> to get the file path to obtain the license from (if there is no runtime license saved in the context)
        /// </summary>
        /// <remarks>
        /// This can be overridden to change the file used to store the design time license for the provider.   By default the
        /// the license file is stored in the same directory as the component executable with the name based on the fully
        /// qualified type name eg MyNamespace.MyControl.lic
        /// </remarks>
        /// <param name="context">The license context</param>
        /// <param name="type">The type to get the license for</param>
        /// <returns>The path of the license file</returns>
        protected virtual string GetLicenseFilePath(LicenseContext context, Type type)
        {
            string dir = GetLicenseDirectory(context, type);
            return String.Format(@"{0}\{1}.lic", dir, type.FullName);
        }

        /// <summary>
        /// Display an error to a message box or the trace output
        /// </summary>
        /// <param name="title">The title for the error</param>
        /// <param name="message">The error message</param>
        protected virtual void ShowError(string title, string message)
        {
            string productName = (_parameters != null) ? _parameters.ProductName : LicenseResources.UnknownProductTxt;
            LicenseUtilities.ShowError(string.Format(title, productName), message);
        }

        #endregion
	}

}
