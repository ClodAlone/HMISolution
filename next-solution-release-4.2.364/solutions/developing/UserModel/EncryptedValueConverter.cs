using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace UFUserModel
{
   public class EncryptedValueConverter : DevExpress.Xpo.Metadata.ValueConverter
   {
        public override object ConvertFromStorageType(object value)
        {
            if (!(value is String) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return WPFUtilities.CryptString.CryptString.DecryptString(value.ToString());
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
