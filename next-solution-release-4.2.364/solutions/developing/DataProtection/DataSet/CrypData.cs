using DataProtection.Common;
using DataReader.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DataProtection.DataSet
{
    class CrypData
    {
        #region Declarations
        static int maxAllowedExtensionValues = 1;
        #endregion

        #region Methods
        internal static string CryptData(DataRow row, string[] skipColumnNames, params object[] extensionValues)
        {
            var expandoObject = SqlDataRowToExpando(row, false, skipColumnNames);
            var values = expandoObject as IDictionary<string, object>;
            var clearText = CryptCommon.GetFlatStringValues(values);
            if (extensionValues != null)
            {
                foreach (var value in extensionValues)
                    clearText = string.Format("{0}{1}{2}", clearText, CryptCommon.SeparatorCryptedValuesChar, string.Format(CultureInfo.InvariantCulture, "{0}", value));
            }
            return WPFUtilities.CryptString.CryptString.EncryptString(clearText, CryptCommon.key);
        }

        internal static bool ValidateData(DataRow row, string encryptedText, TimeSpan dateTimeTolerance, params string[] skipColumnNames)
        {
            string[] extensionValues;
            return ValidateData(row, encryptedText, dateTimeTolerance, skipColumnNames, out extensionValues);
        }

        internal static bool ValidateData(DataRow row, string encryptedText, TimeSpan dateTimeTolerance, string[] skipColumnNames, out string[] extensionValues)
        {
            var dictionary = GetColumnValues(row, encryptedText, skipColumnNames, out extensionValues);
            if (dictionary == null)
                return false;

            foreach (var columnName in dictionary.Keys)
            {
                if (!CryptCommon.ValidateValue(row[columnName], dictionary[columnName], dateTimeTolerance))
                    return false;
            }

            return true;
        }

        internal static dynamic SqlDataRowToExpando(DataRow row, bool bSkipConvertValue, params string[] skipColumnNames)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var ii = 0; ii < row.Table.Columns.Count; ii++)
            {
                if(skipColumnNames != null && skipColumnNames.Contains(row.Table.Columns[ii].ColumnName))
                    continue;

                expandoObject.Add(row.Table.Columns[ii].ColumnName, bSkipConvertValue ? row[ii] : CryptCommon.GetValue(row[ii]));
            }

            return expandoObject;
        }

        static IDictionary<string, object> GetColumnValues(DataRow row, string encryptedText, string[] skipColumnNames, out string[] extensionValues)
        {
            var clearText = WPFUtilities.CryptString.CryptString.DecryptString(encryptedText, CryptCommon.key);
            var expandoObject = new Dictionary<string, object>();
            var values = clearText.Split(CryptCommon.SeparatorCryptedValuesChar);

            int ii = 0;
            foreach (DataColumn column in row.Table.Columns)
            {
                if (skipColumnNames != null && skipColumnNames.Contains(column.ColumnName))
                    continue;

                if (values.Length <= ii)
                    break;

                expandoObject.Add(column.ColumnName, CryptCommon.GetValue(values[ii++], column.DataType));
            }

            var listExtensionValues = new List<string>();
            for (; ii < values.Length; ii++)
                listExtensionValues.Add(values[ii]);

            extensionValues = null;
            if (listExtensionValues.Count > maxAllowedExtensionValues)
                throw new InvalidOperationException("Too many extension values.");
            else if (listExtensionValues.Count > 0)
                extensionValues = listExtensionValues.ToArray();

            return expandoObject;
        }
        #endregion
    }
}
