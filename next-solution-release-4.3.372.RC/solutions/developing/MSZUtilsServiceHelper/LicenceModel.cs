using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MSZUtilsServiceHelper
{
    public class LicenceType
    {
        #region Private Members;
        private int id = 0;
        private String name;
        private String description;
        private bool hidden;
        #endregion

        #region Properties
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public String Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public String Description
        {
            get { return description; }
            set
            {
                description = value;
            }
        }

        public bool Hidden
        {
            get { return hidden; }
            set
            {
                hidden = value;
            }
        }

        public override string ToString()
        {
            return ModelHelper.GetString<LicenceType>(this);
        }

        public static LicenceType FromString(string value)
        {
            return ModelHelper.GetInstance<LicenceType>(value);
        }
        #endregion
    }
    public class BoolOption
    {
        #region Private Members;
        private int id = 0;
        private String name;
        private String mszparameter;
        private bool defvalue;
        private bool enabled;
        private String addInfo;
        private int orderInfo;
        #endregion

        #region Properties
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public String Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public String MszParameter
        {
            get { return mszparameter; }
            set
            {
                mszparameter = value;
            }
        }

        public bool Value
        {
            get { return defvalue; }
            set
            {
                defvalue = value;
            }
        }
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
            }
        }

        public String AddInfo
        {
            get { return addInfo; }
            set
            {
                addInfo = value;
            }
        }

        public int OrderInfo
        {
            get { return orderInfo; }
            set
            {
                orderInfo = value;
            }
        }

        #endregion
        public BoolOption Clone()
        {
            return ModelHelper.GetInstance<BoolOption>(this.ToString());
        }

        public override string ToString()
        {
            return ModelHelper.GetString<BoolOption>(this);
        }
        public static BoolOption FromString(string value)
        {
            return ModelHelper.GetInstance<BoolOption>(value);
        }
    }
    public class IntOption
    {
        #region Private Members;
        private int id = 0;
        private String name;
        private String mszparameter;
        private uint defvalue;
        private bool enabled;
        private String addInfo;
        private int orderInfo;
        #endregion

        #region Properties
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public String Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public String MszParameter
        {
            get { return mszparameter; }
            set
            {
                mszparameter = value;
            }
        }

        public uint Value
        {
            get { return defvalue; }
            set
            {
                defvalue = value;
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
            }
        }

        public String AddInfo
        {
            get { return addInfo; }
            set
            {
                addInfo = value;
            }
        }

        public int OrderInfo
        {
            get { return orderInfo; }
            set
            {
                orderInfo = value;
            }
        }

        #endregion
        public IntOption Clone()
        {
           return ModelHelper.GetInstance<IntOption>(this.ToString());
        }

        public override string ToString()
        {
            return ModelHelper.GetString<IntOption>(this);
        }
        public static IntOption FromString(string value)
        {
            return ModelHelper.GetInstance<IntOption>(value);
        }
    }
    public class AreaGeoID
    {
        #region Private Members;
        private int id = 0;
        private String name;
        #endregion

        #region Properties
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public String Area
        {
            get { return name; }
            set
            {
                name = value;
            }
        }
        #endregion

        public override string ToString()
        {
            return ModelHelper.GetString<AreaGeoID>(this);
        }
        public static AreaGeoID FromString(string value)
        {
            return ModelHelper.GetInstance<AreaGeoID>(value);
        }
    }
    public class Customer
    {
        #region Private Members;
        private int id = -1;
        private String code;
        private String name;
        private string shortname;
        private String piva;
        private int areageoid;
        private String area;
        private bool editable;
        #endregion

        #region Properties
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public String Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public String ShortName
        {
            get { return shortname; }
            set
            {
                shortname = value;
            }
        }

        public String Code
        {
            get { return code; }
            set
            {
                code = value;
            }
        }

        public String PIva
        {
            get { return piva; }
            set
            {
                piva = value;
            }
        }

        public String Area
        {
            get { return area; }
            set
            {
                area = value;
            }
        }

        public int AreaGeoID
        {
            get { return areageoid; }
            set
            {
                areageoid = value;
            }
        }

        public bool Editable
        {
            get { return editable; }
            set
            {
                editable = value;
            }
        }
        #endregion

        public override string ToString()
        {
            return ModelHelper.GetString<Customer>(this);
        }
        public static Customer FromString(string value)
        {
            return ModelHelper.GetInstance<Customer>(value);
        }
    }
    public enum LicType
    {
        HW = 1,
        UnlimitedSW = 2,
        TemporarySW = 3
    }
    public class LicenceInfo
    {
        #region Private Members;
        private int id = 0;
        private int productId;
        private int serialNumber;
        private int instanceNumber;
        private int idType;
        private bool generateSWCode;
        private string sWCodeOptions;
        private LicType idLicType;
        private int customerID;
        private String customerCode;
        private String removedCode;
        private String order;
        private String siteCode;
        private String bill;
        private String note;
        private byte[] fileKey;
        private DateTime sKGenerationDate;
        private DateTime sKExpiredDate;
        private string sKUserName;
        private string customerName;
        private string typeDescr;
        private string typeName;
        private string licDescr;
        private DateTime lastChangesDate;
        private string description;
        private string price;
        private string finalCustomer;
        private string lastChangesUser;
        private string areaGeoID;
        #endregion

        #region Properties
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }
        public int ProductID
        {
            get { return productId; }
            set
            {
                productId = value;
            }
        }
        public int IDType
        {
            get { return idType; }
            set
            {
                idType = value;
            }
        }
        public string TypeName
        {
            get { return typeName; }
            set
            {
                typeName = value;
            }
        }
        public string LicDescr
        {
            get { return licDescr; }
            set
            {
                licDescr = value;
            }
        }
        public string TypeDescr
        {
            get { return typeDescr; }
            set
            {
                typeDescr = value;
            }
        }
        public LicType IDLicType
        {
            get { return idLicType; }
            set
            {
                idLicType = value;
            }
        }
        public String CustomerCode
        {
            get { return customerCode; }
            set
            {
                customerCode = value;
            }
        }
        public int CustomerID
        {
            get { return customerID; }
            set
            {
                customerID = value;
            }
        }
        public string CustomerDescription
        {
            get { return customerName; }
            set
            {
                customerName = value;
            }
        }
        public DateTime LastChangesDate
        {
            get { return lastChangesDate; }
            set
            {
                lastChangesDate = value;
            }
        }
        public DateTime SKGenerationDate
        {
            get { return sKGenerationDate; }
            set
            {
                sKGenerationDate = value;
            }
        }
        public DateTime SKExpiredDate
        {
            get { return sKExpiredDate; }
            set
            {
                sKExpiredDate = value;
            }
        }
        public int SerialNumber
        {
            get { return serialNumber; }
            set
            {
                serialNumber = value;
            }
        }
        public int InstanceNumber
        {
            get { return instanceNumber; }
            set
            {
                instanceNumber = value;
            }
        }

        public String SiteCode
        {
            get { return siteCode; }
            set
            {
                siteCode = value;
            }
        }

        public byte[] FileKey
        {
            get { return fileKey; }
            set
            {
                fileKey = value;
            }
        }

        public bool GenerateSWCode
        {
            get { return generateSWCode; }
            set
            {
                generateSWCode = value;
            }
        }
        public string SWCodeOptions
        {
            get { return sWCodeOptions; }
            set
            {
                sWCodeOptions = value;
            }
        }
        public string SKUserName
        {
            get { return sKUserName; }
            set
            {
                sKUserName = value;
            }
        }
        public String RemovedCode
        {
            get { return removedCode; }
            set
            {
                removedCode = value;
            }
        }
        public String Order
        {
            get { return order; }
            set
            {
                order = value;
            }
        }
        public String Bill
        {
            get { return bill; }
            set
            {
                bill = value;
            }
        }
        public String Price
        {
            get { return price; }
            set
            {
                price = value;
            }
        }
        public String FinalCustomer
        {
            get { return finalCustomer; }
            set
            {
                finalCustomer = value;
            }
        }
        public String Note
        {
            get { return note; }
            set
            {
                note = value;
            }
        }
        public String Description
        {
            get { return description; }
            set
            {
                description = value;
            }
        }
        public String LastChangesUser
        {
            get { return lastChangesUser; }
            set
            {
                lastChangesUser = value;
            }
        }
        public String AreaGeoDescr
        {
            get { return areaGeoID; }
            set
            {
                areaGeoID = value;
            }
        }
        
        #endregion


        public override string ToString()
        {
            return ModelHelper.GetString<LicenceInfo>(this);
        }
        public static LicenceInfo FromString(string value)
        {
            return ModelHelper.GetInstance<LicenceInfo>(value);
        }
    }

    public class LogLicenceInfo
    {
        public string LicenceOption { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime DateOfChange { get; set; }
        public string ChangesUser { get; set; }

        public override string ToString()
        {
            return ModelHelper.GetString<LogLicenceInfo>(this);
        }
        public static LogLicenceInfo FromString(string value)
        {
            return ModelHelper.GetInstance<LogLicenceInfo>(value);
        }
    }

    internal static class ModelHelper
    {
        const char itemSeparator = '§';
        const char propSeparator = '☻';
        public static string GetString<T>(T customObject)
        {
            var res = String.Join($"{itemSeparator}", GetPropValues<T>(customObject).ToList());
            return res;
        }
        private static IEnumerable<string> GetPropValues<T>(T customObject)
        {
            var properties = (from prop in typeof(T).GetProperties().Where(p => p.CanWrite && p.GetIndexParameters().Length == 0) select prop).ToList();
            foreach (PropertyInfo p in properties)
            {
                var prop = p.GetValue(customObject, null);
                if (prop == null)
                    yield return $"{p.Name}{propSeparator}{string.Empty}";
                else
                {
                    object _prop;
                    if (p.PropertyType == typeof(byte[]))
                        _prop = Encoding.ASCII.GetString((byte[])prop);
                    else if (p.PropertyType == typeof(DateTime))
                        _prop = ((DateTime)prop).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else
                        _prop = prop;

                    yield return $"{p.Name}{propSeparator}{_prop}";
                }
            }
        }
        public static T GetInstance<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            string[] values = value.Split(itemSeparator);
            T ret = Activator.CreateInstance<T>();
            var properties = (from prop in typeof(T).GetProperties().Where(p => p.CanWrite && p.GetIndexParameters().Length == 0) select prop).ToList();
            foreach (PropertyInfo p in properties)
            {
                try
                {
                    uint newUIntValue;
                    int newIntValue;
                    bool newBoolValue;
                    LicType newLicTypeValue;
                    DateTime newDateTimeValue;
                    var stringValue = (from v in values where v.StartsWith($"{p.Name}{propSeparator}") select v).FirstOrDefault()?.Split(propSeparator)[1];
                    if (p.PropertyType == typeof(uint) && uint.TryParse(stringValue, out newUIntValue))
                        p.SetValue(ret, newUIntValue);
                    if (p.PropertyType == typeof(int) && int.TryParse(stringValue, out newIntValue))
                        p.SetValue(ret, newIntValue);
                    else if (p.PropertyType == typeof(LicType) && Enum.TryParse<LicType>(stringValue, out newLicTypeValue))
                        p.SetValue(ret, newLicTypeValue);
                    else if (p.PropertyType == typeof(byte[]) && !string.IsNullOrEmpty(stringValue))
                        p.SetValue(ret, Encoding.ASCII.GetBytes(stringValue));
                    else if (p.PropertyType == typeof(bool) && bool.TryParse(stringValue, out newBoolValue))
                        p.SetValue(ret, newBoolValue);
                    else if (p.PropertyType == typeof(DateTime) && DateTime.TryParse(stringValue, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out newDateTimeValue))
                        p.SetValue(ret, newDateTimeValue);
                    else if (p.PropertyType == typeof(string))
                        p.SetValue(ret, stringValue);
                }
                catch (Exception)
                {
                }
            }

            return ret;
        }
    }
}
