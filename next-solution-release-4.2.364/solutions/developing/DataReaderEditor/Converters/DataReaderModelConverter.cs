using System;
using System.Windows.Data;
using DevExpress.Xpo.DB.Helpers;

namespace DataReaderEditor.Converters
{
    [ValueConversion(typeof(DataReader.DataReaderModel), typeof(String))]
    public class DataReaderModelConverter : IValueConverter
    {
        #region Declarations
        DataReader.DataReaderModel lastValidDataReaderViewModel;
        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (targetType != typeof(String))
            //    throw new InvalidOperationException("The target must be a string");

            var dataReaderViewModel = value as DataReader.DataReaderModel;
            if (dataReaderViewModel == null)
                return null;

            lastValidDataReaderViewModel = dataReaderViewModel;
            if (String.IsNullOrEmpty(dataReaderViewModel.DataProvider))
                return dataReaderViewModel.Connection;
            else if (String.IsNullOrEmpty(dataReaderViewModel.Connection))
                return String.Format("DataProvider={0}", dataReaderViewModel.DataProvider);
            else
                return String.Format("DataProvider={0};{1}", dataReaderViewModel.DataProvider, dataReaderViewModel.Connection);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(DataReader.DataReaderModel))
                throw new InvalidOperationException("The target must be a DataReader.DataReaderModel");

            var connectionString = value as String;
            if (String.IsNullOrEmpty(connectionString))
                return null;
            try
            {
                ConnectionStringParser helper = new ConnectionStringParser(connectionString);
                var dataProvider = helper.GetPartByName("DataProvider");
                var connection = connectionString;
                if (!String.IsNullOrEmpty(dataProvider))
                {
                    helper.RemovePartByName("DataProvider");
                    connection = helper.GetConnectionString();
                }
                if (lastValidDataReaderViewModel == null || lastValidDataReaderViewModel.DataProvider != dataProvider || lastValidDataReaderViewModel.Connection != connection)
                {
                    lastValidDataReaderViewModel = new DataReader.DataReaderModel(lastValidDataReaderViewModel);
                    lastValidDataReaderViewModel.DataProvider = dataProvider;
                    lastValidDataReaderViewModel.Connection = connection;
                }

                return lastValidDataReaderViewModel;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
