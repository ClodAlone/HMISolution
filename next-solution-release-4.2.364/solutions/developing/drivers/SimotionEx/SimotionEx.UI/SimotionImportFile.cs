using DevExpress.Xpo;
using System.Windows;

namespace Simotion.UI
{
    public sealed class SimotionImportFile 
    {
        #region members
        string connString;
        UnitOfWork ufw;
        #endregion

        public SimotionImportFile(string strConnString, UnitOfWork ufW)
        {
            connString = strConnString;
            ufw = ufW;
        }

        public string ImportRequest(string sourceFile, SimotionStationSettings stationSetting)
        {
            string resultFile = null;
            
            // some file configured before ?
            if (MessageBox.Show(string.Format(Properties.Resources.ImportSelectedSymbolicFileRequestFromStation, sourceFile), Properties.Resources.ImportSymbolicFile, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MessageBoxResult result = MessageBoxResult.Yes;

                if (SimotionUISymbolicFileManagement.IsSymbolicFileUsedByOtherStations(connString, stationSetting.Name, sourceFile, out string otherStationName))
                {
                    result = MessageBox.Show(string.Format(Properties.Resources.ImportSelectedSymbolicFileImportOptions, otherStationName), Properties.Resources.ImportSymbolicFile, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                }

                switch (result)
                {
                    case MessageBoxResult.Yes:  // save / override                                    
                        SimotionUISymbolicFileManagement.ImportSymbolicFile(connString, ufw, stationSetting.Name, SimotionProtocol.SaveMode.SaveOverride, ref sourceFile);
                        resultFile = sourceFile;
                        break;
                    case MessageBoxResult.No:   // generate an alternative name                                    
                        SimotionUISymbolicFileManagement.ImportSymbolicFile(connString, ufw, stationSetting.Name, SimotionProtocol.SaveMode.AutoGenerateAlternativeName, ref sourceFile);
                        resultFile = sourceFile;
                        break;
                    default:    // exit without doing nothing
                        resultFile = null;
                        break;
                }                
            }
            else
            {
                // external file
                resultFile = sourceFile;
            }

            return resultFile;
        }
    }
}