using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCodeBaseEx.EncryptString
{
    public class EncryptedValueConverter : DevExpress.Xpo.Metadata.ValueConverter
    {
        public override object ConvertFromStorageType(object value)
        {
            if (!(value is String) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            //the try catch is necessary because if the string has not been encrypted the decryption method throws an exception,
            //in this case the string is passed without being decrypted
            try
            {
                return WPFUtilities.CryptString.CryptString.DecryptString(value.ToString());
            }
            catch
            {
                return value.ToString();
            }
        }

        public override object ConvertToStorageType(object value)
        {
            if (!(value is String) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return WPFUtilities.CryptString.CryptString.EncryptString(value.ToString());
        }

        public override System.Type StorageType
        {
            get { return typeof(String); }
        }
    }
}
