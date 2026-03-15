//
//      FILE:   EncryptedLicense.cs
//
// COPYRIGHT:   Copyright 2008 
//              Infralution
//
using System;
using System.ComponentModel;
using System.Globalization;
namespace Infralution.Licensing
{
	/// <summary>
	/// Defines an encrypted license for an application or component generated using the Infralution
	/// Licensing System.
	/// </summary>
	/// <remarks>
	/// The Infralution Licensing System provides a secure way of licensing .NET controls,
	/// components and applications.   Licenses are protected using public key encryption to
	/// minimize possibility of cracking.
	/// </remarks>
	/// <seealso cref="EncryptedLicenseProvider"/>
#if PUBLIC_LICENSE_CLASS  // if true allows class to be visible outside library  
    public
#endif
  [System.Runtime.CompilerServices.CompilerGenerated]  // suppress FxCop warnings on external code that we don't want to modify
	class EncryptedLicense : License
	{
        #region Member Variables

        private string _key;
        private Int32 _serialNo;
        private string _productInfo;

        #endregion

        #region Public Interface

        /// <summary>
        /// Create a new Infralution Encrypted License
        /// </summary>
        /// <param name="key">The key for the license</param>
        /// <param name="serialNo">The serial number of the license</param>
        /// <param name="productInfo">The product data associated with the license</param>
        public EncryptedLicense(string key, Int32 serialNo, string productInfo)
        {
            _key = key;
            _serialNo = serialNo;
            _productInfo = productInfo;
        }

        /// <summary>
        /// The license key for the license
        /// </summary>
        public override string LicenseKey
        {
            get { return _key; }
        }

        /// <summary>
        /// The product data associated with the license
        /// </summary>
        public string ProductInfo
        {
            get { return _productInfo; }
        }

        /// <summary>
        /// The unique serial no for the license
        /// </summary>
        public Int32 SerialNo
        {
            get { return _serialNo; }
        }

        /// <summary>
        /// Cleans up any resources held by the license
        /// </summary>
        public override void Dispose()
        {
        }

        #endregion

	}

}
