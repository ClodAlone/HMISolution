using DataProtection.Common;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAHistorianModel;

namespace DataProtection.Historian
{
    class CrypData
    {
        #region Methods

        #region UFUAAuditDataItem
        internal static string CryptData(UFUAAuditDataItem item)
        {
            var expandoObject = GetExpandoObject<UFUAAuditDataItem>(item);
            var values = expandoObject as IDictionary<string, object>;
            var clearText = CryptCommon.GetFlatStringValues(values);
            return WPFUtilities.CryptString.CryptString.EncryptString(clearText, CryptCommon.key);
        }

        internal static bool ValidateData(UFUAAuditDataItem item, string encryptedText, TimeSpan dateTimeTolerance)
        {
            var dictionary = GetColumnValues<UFUAAuditDataItem>(encryptedText);
            if (dictionary == null)
                return false;

            XPClassInfo classInfo = item.Session.GetClassInfo(item.GetType());
            foreach (var key in dictionary.Keys)
            {
                if (classInfo.GetPersistentMember(key) == null)
                {
                    item.SetMemberValue(key, dictionary[key]);
                    continue;
                }

                var value1 = item.GetMemberValue(key);
                if (!CryptCommon.ValidateValue(value1, dictionary[key], dateTimeTolerance))
                    return false;
            }

            return true;
        }
        #endregion

        #region UFUAAuditDataLog
        internal static string CryptData(UFUAAuditDataLog item)
        {
            var expandoObject = GetExpandoObject<UFUAAuditDataLog>(item);
            var values = expandoObject as IDictionary<string, object>;
            var clearText = CryptCommon.GetFlatStringValues(values);
            return WPFUtilities.CryptString.CryptString.EncryptString(clearText, CryptCommon.key);
        }

        internal static bool ValidateData(UFUAAuditDataLog item, string encryptedText, TimeSpan dateTimeTolerance)
        {
            var dictionary = GetColumnValues<UFUAAuditDataLog>(encryptedText);
            if (dictionary == null)
                return false;

            XPClassInfo classInfo = item.Session.GetClassInfo(item.GetType());
            foreach (var key in dictionary.Keys)
            {
                if (classInfo.GetPersistentMember(key) == null)
                {
                    item.SetMemberValue(key, dictionary[key]);
                    continue;
                }

                var value1 = item.GetMemberValue(key);
                if (!CryptCommon.ValidateValue(value1, dictionary[key], dateTimeTolerance))
                    return false;
            }

            return true;
        }
        #endregion

        #region UFUAAuditLogItem
        internal static string CryptData(UFUAAuditLogItem item)
        {
            var expandoObject = GetExpandoObject<UFUAAuditLogItem>(item);
            var values = expandoObject as IDictionary<string, object>;
            var clearText = CryptCommon.GetFlatStringValues(values);
            return WPFUtilities.CryptString.CryptString.EncryptString(clearText, CryptCommon.key);
        }

        internal static bool ValidateData(UFUAAuditLogItem item, string encryptedText, TimeSpan dateTimeTolerance)
        {
            var dictionary = GetColumnValues<UFUAAuditLogItem>(encryptedText);
            if (dictionary == null)
                return false;

            XPClassInfo classInfo = item.Session.GetClassInfo(item.GetType());
            foreach (var key in dictionary.Keys)
            {
                if (classInfo.GetPersistentMember(key) == null)
                {
                    item.SetMemberValue(key, dictionary[key]);
                    continue;
                }

                var value1 = item.GetMemberValue(key);
                if (!CryptCommon.ValidateValue(value1, dictionary[key], dateTimeTolerance))
                    return false;
            }

            return true;
        }
        #endregion

        internal static dynamic GetExpandoObject<T>(T item, bool bOid = false, bool bSkipConvertValue = false) where T : XPObject
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            if (bOid)
                expandoObject.Add("OID", item.Oid);

            var fileds = UFUAHistorianModel.Helpers.HistorianHelper.GetEncryptionFields<T>();
            foreach (var key in fileds.Keys)
            {
                var value = item.GetMemberValue(key);
                    expandoObject.Add(key, bSkipConvertValue ? value : CryptCommon.GetValue(value));
            }

            return expandoObject;
        }

        static IDictionary<string, object> GetColumnValues<T>(string encryptedText)
        {
            var clearText = WPFUtilities.CryptString.CryptString.DecryptString(encryptedText, CryptCommon.key);
            var expandoObject = new Dictionary<string, object>();
            var values = clearText.Split(CryptCommon.SeparatorCryptedValuesChar);

            int ii = 0;
            var fileds = UFUAHistorianModel.Helpers.HistorianHelper.GetEncryptionFields<T>();
            foreach (var key in fileds.Keys)
            {
                if (values.Length <= ii)
                    break;

                expandoObject.Add(key, CryptCommon.GetValue(values[ii++], fileds[key]));
            }
            return expandoObject;
        }
        #endregion
    }
}
