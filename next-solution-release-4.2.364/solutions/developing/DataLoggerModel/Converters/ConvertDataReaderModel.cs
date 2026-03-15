using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReader;
using DevExpress.Xpo.Metadata;

namespace DataLoggerModel.Converters
{
    /// <summary>
    /// Convert the DataReaderModel in a suitable format for XPObject data store.
    /// </summary>
    /// <remarks>
    /// 'DataProvider' and 'Connection' are mandatory parameters in order to create a new DataReaderModel.
    /// 'DataProviderDescription', 'DataProviderDisplayName', 'DataProviderShortDisplayName', 
    /// 'DataSourceName' and 'DataSourceDisplayName' are optional parameters.
    /// DataReaderModel is serialized as String using the pipe character for delimit the following members :
    /// DataProvider|Connection|DataSourceName|DataSourceDisplayName|DataProviderDisplayName|DataProviderShortDisplayName|DataProviderDescription
    /// </remarks>
    /// <exception cref="ArgumentException"></exception>
    internal class ConvertDataReaderModel : ValueConverter
    {
        public override object ConvertFromStorageType(object value)
        {
            var id = value as String;
            if (id == null)
                throw new ArgumentException("Parameters must be a string value");

            var values = id.Split('|');

            // Check for mandatory parameters
            if (values.Length < 2)
                return new DataReaderModel();
                //throw new ArgumentException("Missing mandatory parameters in the string value");

            // Set mandatory parameters
            var model = new DataReaderModel()
            {
                DataProvider = values[0],
                Connection = values[1]
            };

            // Set optional parameters
            if (values.Length > 2)
                model.DataSourceName = values[2];
            if (values.Length > 3)
                model.DataSourceDisplayName = values[3];
            if (values.Length > 4)
                model.DataProviderDisplayName = values[4];
            if (values.Length > 5)
                model.DataProviderShortDisplayName = values[5];
            if (values.Length > 6)
                model.DataProviderDescription = values[6];

            return model;
        }

        public override object ConvertToStorageType(object value)
        {
            var model = value as DataReaderModel;
            if (model == null)
                return String.Empty;

            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                                    model.DataProvider, model.Connection,
                                    model.DataSourceName, model.DataSourceDisplayName,
                                    model.DataProviderDisplayName, model.DataProviderShortDisplayName, model.DataProviderDescription);
        }

        public override Type StorageType
        {
            get
            {
                return typeof(String);
            }
        }
    }
}
