//
//      FILE:   EvaluationMonitor.cs
//
// COPYRIGHT:   Copyright 2008 
//              Infralution
//
using System;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Globalization;
namespace Infralution.Licensing
{
	/// <summary>
	/// Provides a mechanism for managing time/usage limited evaluations of products.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Instantiate an instance of this class to read/write the evaluation parameters for the   
	/// given product.  The <see cref="FirstUseDate"/> is set the first time that
    /// the class is instantiated.  The <see cref="LastUseDate"/> and <see cref="UsageCount"/> 
    /// properties are set each time the class is instantiated (or once per day if
    /// the <see cref="CountUsageOncePerDay"/> property is set to true).
    /// </para>
    /// <para>
    /// Note that evaluation data must be stored somewhere on the users 
    /// hard disk.  It is therefore not too difficult for a sophisticated user to determine the 
    /// changes made either to registry keys or files (using file/registry monitoring software) 
    /// and restore the state of these to their pre-installation state (thus resetting the 
    /// evaluation period).  For this reason it is recommended that you don't rely on this 
    /// mechanism alone.  You should also consider limiting the functionality of your product 
    /// in some way or adding nag screens to discourage long term use of evaluation versions.
    /// </para>
    /// <para>
    /// If you have a data oriented application you can increase the security of evaluations by
    /// storing the current <see cref="UsageCount"/> somewhere in your database each time the 
    /// application runs and cross checking this with the number returned by the EvaluationMonitor.
    /// </para>
    /// </remarks>
#if PUBLIC_LICENSE_CLASS  // if true allows class to be visible outside library  
    public
#endif
  [System.Runtime.CompilerServices.CompilerGenerated]  // suppress FxCop warnings on external code that we don't want to modify
  class EvaluationMonitor : IDisposable
    {
        #region Member Variables

        private byte[] _productData;
        private int _usageCount = 1;
       
        private DateTime _firstUseDate = DateTime.MinValue;
        private DateTime _lastUseDate = DateTime.MinValue;
        private bool _invalid = false;

        private bool _countUsageOncePerDay = false;

        private RegistryKey _rootKey;
        private RegistryKey _baseKey;
        private string _usageKeyName;
        private string _firstUseKeyName;
        private string _lastUseKeyName;

        // Sub field names for saving data.  Designed to
        // blend in with their surroundings
        //
        private const string classUsageKey = "TypeLib";
        private const string classFirstUseKey = "InprocServer32";
        private const string classLastUseKey = "Control";

        private const string userUsageKey = "Software\\Microsoft\\WAB\\WAB4";
        private const string userFirstUseKey = "Software\\Microsoft\\WAB\\WAB Sort State";
        private const string userLastUseKey = "Software\\Microsoft\\WAB\\WAB4\\LastFind";

        // parameters for encrypting evaluation data
        //
        private static byte[] _desKey = new byte []  { 0x12, 0x75, 0xA8, 0xF1, 0x32, 0xED, 0x13, 0xF2 };
        private static byte[] _desIV = new byte []  { 0xA3, 0xEF, 0xD6, 0x21, 0x37, 0x80, 0xCC, 0xB1 };
       

        #endregion

        #region Public Interface

        /// <summary>
        /// Initialize a new instance of the evaluation monitor.
        /// </summary>
        /// <param name="countUsageOncePerDay">Should the usage count only be incremented once per day</param>
        /// <param name="password">A unique password for this product</param>
        /// <remarks>
        /// If countUsageOncePerDay is set to true then the <see cref="UsageCount"/> is only incremented once
        /// for each day that the product is actually used.   If countUsageOncePerDay is false then the <see cref="UsageCount"/>
        /// is incremented each time a new evaluation monitor is instantiated for a product
        /// </remarks>
        public EvaluationMonitor(string password, bool countUsageOncePerDay)
        {
            _countUsageOncePerDay = countUsageOncePerDay;
            _productData = Encrypt(password);
            RegistryKey parentKey;

            // test whether we can write to HKEY_CLASSES_ROOT
            //
            try
            {
              // TODO: is this key variable used/needed?
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(password);
                Registry.ClassesRoot.DeleteSubKey(password, false);

                // if that succeeded then set up the keys appropriately
                //
                _rootKey = Registry.ClassesRoot;
                parentKey = _rootKey.OpenSubKey("CLSID", true);
                _usageKeyName = classUsageKey;
                _firstUseKeyName = classFirstUseKey;
                _lastUseKeyName = classLastUseKey;
            }
            catch
            {
                // revert to using HKEY_CURRENT_USER
                //
                _rootKey = Registry.CurrentUser;
                parentKey = _rootKey.OpenSubKey("Identities", true);
                if (parentKey == null)
                {
                    parentKey = _rootKey.CreateSubKey("Identities");
                }
                _usageKeyName = userUsageKey;
                _firstUseKeyName = userFirstUseKey;
                _lastUseKeyName = userLastUseKey;
            }

            try
            {
                // find the base key
                //
                _baseKey = FindBaseKey(parentKey);

                // OK we couldn't find a key so create it
                //
                if (_baseKey == null)
                {
                    _usageCount = 1;
                    _firstUseDate = DateTime.UtcNow;
                    _lastUseDate = DateTime.UtcNow;
                    CreateBaseKey(parentKey);
                }
                else
                {
                    UpdateData();
                }
                parentKey.Close();
            }
            catch
            {
                _invalid = true;
            }
        }

        /// <summary>
        /// Initialize a new instance of the evaluation monitor.
        /// </summary>
        /// <param name="password">A unique password for this product</param>
        /// <remarks>
        /// For backward compatibility this overloaded constructor sets the <see cref="CountUsageOncePerDay"/>
        /// property to false.
        /// </remarks>
        public EvaluationMonitor(string password)
            : this(password, false)
        {
        }

        /// <summary>
        /// Set/Get whether the <see cref="UsageCount"/> should be incremented only once per day 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <remarks>
        /// If CountUsageOncePerDay is set to true then the <see cref="UsageCount"/> is only incremented once
        /// for each day that the product is actually used.   If CountUsageOncePerDay is false then 
        /// the <see cref="UsageCount"/> is incremented each time a new evaluation monitor is instantiated for 
        /// a given product id. 
        /// </remarks>
        public bool CountUsageOncePerDay
        {
            get { return _countUsageOncePerDay; }
            set { _countUsageOncePerDay = value; }
        }

        /// <summary>
        /// Return the number of times the product has been used 
        /// </summary>
        /// <remarks>
        /// If <see cref="CountUsageOncePerDay"/> is set to true then the UsageCount is only incremented once
        /// for each day that the product is actually used.   If <see cref="CountUsageOncePerDay"/> is false then 
        /// the UsageCount is incremented each time a new evaluation monitor is instantiated for 
        /// a given product id.  Typically you should instantiate an EvaluationMonitor object just once in your 
        /// software.
        /// </remarks>
        public int UsageCount
        {
            get { return _usageCount; }
        }

        /// <summary>
        /// Return the date/time the product was first used
        /// </summary>
        public DateTime FirstUseDate
        {
            get { return _firstUseDate; }
        }

        /// <summary>
        /// Return the date/time the product was last used
        /// </summary>
        /// <remarks>
        /// If <see cref="CountUsageOncePerDay"/> is set to true then this is the date
        /// at which the usage count was last updated - otherwise it is the date at 
        /// which the EvaluationMonitor was last created.
        /// </remarks>
        public DateTime LastUseDate
        {
            get { return _lastUseDate; }
        }

        /// <summary>
        /// Return the number of days since the product was first run.
        /// </summary>
        public int DaysInUse
        {
            get { return DateTime.UtcNow.Subtract(_firstUseDate).Days + 1; }
        }

        /// <summary>
        /// Returns true if the evaluation monitor detects attempts to circumvent
        /// evaluation limits by tampering with the hidden evaluation data or winding
        /// the PC clock backwards 
        /// </summary>
        public bool Invalid
        {
            get { return _invalid; }
        }

        /// <summary>
        /// Allows you to reset the evaluation period.
        /// </summary>
        /// <remarks>
        /// This may be useful if a customer needs an extension or if somehow they
        /// invalidate their evaluation data by attempting to fiddle
        /// </remarks>
        public void Reset()
        {
            if (_baseKey != null)
            {
                string name = _baseKey.Name;
                int i = name.IndexOf("\\") + 1;
                name = name.Substring(i);
                _baseKey.Close();
                _baseKey = null;
                try
                {
                    _rootKey.DeleteSubKeyTree(name);
                }
                catch {} // ignore possible failures
            }
            _usageCount = 1;
            _firstUseDate = DateTime.UtcNow;
            _lastUseDate = _firstUseDate;
            _invalid = false;
        }

        #endregion

        #region Local Methods


        /// <summary>
        /// Find the base key for this product
        /// </summary>
        /// <param name="parent">The key to search under</param>
        /// <returns>The base registry key used to store the data</returns>
        private RegistryKey FindBaseKey(RegistryKey parent)
        {
            string[] classIDs = parent.GetSubKeyNames();
            foreach (string classID in classIDs)
            {
                try
                {
                    RegistryKey key = parent.OpenSubKey(classID);
                    object keyValue = key.GetValue(null);
                    if (keyValue is byte[])
                    {
                        if (Equals((keyValue as byte[]), _productData))
                        {
                            return key;
                        }
                    }
                    key.Close();
                }
                catch   // if we can't open the key for some reason just go to the next key
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Create the base key for this product
        /// </summary>
        /// <param name="parent">The key to place the information under</param>
        private void CreateBaseKey(RegistryKey parent)
        {
            // create the registry key with a unique name each time - this makes it a little
            // more difficult for people to find the key
            //
            string guid = Guid.NewGuid().ToString().ToUpper();

            // force the GUID to start with a zero - this potentially improves the performance
            // when locating the key by up to a factor of sixteen
            //
            guid = "0" + guid.Substring(1);
            string baseKeyName = string.Format("{{{0}}}", guid);

            _baseKey = parent.CreateSubKey(baseKeyName);
            _baseKey.SetValue(null, _productData);
            RegistryKey dateKey = _baseKey.CreateSubKey(_firstUseKeyName);
            dateKey.SetValue(null, EncryptDate(_firstUseDate));
            dateKey.Close();

            // create the usage key and set the initial value
            //
            RegistryKey usageKey = _baseKey.CreateSubKey(_usageKeyName);
            usageKey.SetValue(null, Encrypt(_usageCount.ToString(CultureInfo.InvariantCulture)));
            usageKey.Close();   

            // create the last use key and set the initial value
            //
            RegistryKey lastUseKey = _baseKey.CreateSubKey(_lastUseKeyName);
            lastUseKey.SetValue(null,  EncryptDate(_lastUseDate));
            lastUseKey.Close();   
        }

        /// <summary>
        /// Update the registry data
        /// </summary>
        private void UpdateData()
        {
            RegistryKey firstUseKey = _baseKey.OpenSubKey(_firstUseKeyName, true);
            _firstUseDate = DecryptDate((byte[]) firstUseKey.GetValue(null));

            RegistryKey lastUseKey = _baseKey.OpenSubKey(_lastUseKeyName, true);
            _lastUseDate = DecryptDate((byte[]) lastUseKey.GetValue(null));

            double hoursSinceLastUse = DateTime.UtcNow.Subtract(_lastUseDate).TotalHours;

            // if we are incrementing the usage count everytime OR if it is a day since the last use then
            // set the usage count and lastUseDate
            //
            bool updateUsage = (!_countUsageOncePerDay || hoursSinceLastUse > 24);

            // detect winding the clock back on the PC - give them six hours of grace to allow for
            // daylight saving adjustments etc
            //
            if (hoursSinceLastUse < -6.0)
            {
                _invalid = true;
            }
            else
            {
                if (updateUsage)
                {
                    // set the first use date again to ensure that is saved using invariant culture
                    //
                    firstUseKey.SetValue(null, EncryptDate(_firstUseDate));
                    lastUseKey.SetValue(null, EncryptDate(DateTime.UtcNow));
                }
            }

            // get the previous usage count
            //
            RegistryKey usageKey = _baseKey.OpenSubKey(_usageKeyName, true);
            string countString = Decrypt((byte[])usageKey.GetValue(null));
            _usageCount = int.Parse(countString, CultureInfo.InvariantCulture);

            if (updateUsage)
            {
                _usageCount++;
                usageKey.SetValue(null, Encrypt(_usageCount.ToString(CultureInfo.InvariantCulture)));
            }

            usageKey.Close();            
            lastUseKey.Close();
            firstUseKey.Close();
        
        }

        /// <summary>
        /// Encrypt the given text
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <returns>Encrypted byte array</returns>
        private byte[] Encrypt(string text)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey; 
            des.IV = _desIV;
            byte[] data = ASCIIEncoding.ASCII.GetBytes(text);
            return des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// Decrypt the given byte data to text
        /// </summary>
        /// <param name="data">The byte data to decrypt</param>
        /// <returns>The decrypted text</returns>
        private string Decrypt(byte[] data)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey; 
            des.IV = _desIV;
            byte[] decryptedData = des.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            return ASCIIEncoding.ASCII.GetString(decryptedData);
        }

        /// <summary>
        /// Encrypt a date
        /// </summary>
        /// <param name="date">The date to encrypt</param>
        /// <returns>The encrypted date data</returns>
        private byte[] EncryptDate(DateTime date)
        {
            // add "I" to the date to indicate that the date is encrypted using an invariant culture
            //
            string dateString = "I" + date.ToString(CultureInfo.InvariantCulture);
            return Encrypt(dateString);
        }

        /// <summary>
        /// Decrypt a date
        /// </summary>
        /// <param name="data">The data to decrypt</param>
        /// <returns>The descryped date</returns>
        private DateTime DecryptDate(byte[] data)
        {
            string dateString = Decrypt(data);
            CultureInfo culture = CultureInfo.CurrentCulture;

            // earlier versions of EvaluationMonitor did not use an invariant culture to
            // to encrypt the date - check if the date was encrypted using invariant culture
            //
            if (dateString[0] == 'I')
            {
                dateString = dateString.Substring(1);
                culture = CultureInfo.InvariantCulture;
            }
            return DateTime.Parse(dateString, culture);
        }

        /// <summary>
        /// Are the contents of the two byte arrays equal
        /// </summary>
        /// <param name="a1">The first array</param>
        /// <param name="a2">The second array </param>
        /// <returns>True if the contents of the arrays is equal</returns>
        private bool Equals(byte[] a1, byte[] a2)
        {
            if (a1 == a2) return true;
            if ((a1 == null) || (a2 == null)) return false;
            if (a1.Length != a2.Length) return false;
            for (int i=0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i]) return false;
            }
            return true;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Free resources used by the EvaluationMonitor
        /// </summary>
        public void Dispose()
        {
            if (_baseKey != null)
            {
                _baseKey.Close();
                _baseKey = null;
            }
        }

        #endregion
    }
}
