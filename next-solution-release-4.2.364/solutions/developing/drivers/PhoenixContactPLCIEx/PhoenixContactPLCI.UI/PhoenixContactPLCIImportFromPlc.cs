using System;
using DriverCodeBaseEx.UI;

namespace PhoenixContactPLCI.UI
{
    public partial class PhoenixContactPLCIImportFromPlc : PhoenixContactPLCIImportBase, IDisposable
    {
        #region Properties
        #endregion

        #region Constructors
        public PhoenixContactPLCIImportFromPlc() : base()
        {
        }
        #endregion

        #region Methods
                
        public ImportDataModelPhoenixContactPlci Import(GetStationName readStationName, string strSettingPath, PhoenixContactPLCIChannelSettings ch, PhoenixContactPLCIStationSettings st)
        {
            using (PhoenixContactPLCIChannel channel = new PhoenixContactPLCIChannel(new PhoenixContactPLCIDriver(strSettingPath), ch))
            {
                PhoenixContactPLCIProtocol.ErrorCodes ret = channel.GetBinFileFromDevice(out string binFile, st.Name, out string errorMessage);
                if ((DriverCodeBaseEx.Enumerators.DriverErrorCodes)ret == DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
                    // base method to fill list of imported var using bin file
                    _ImportDataModel = ImportFromBin(readStationName, binFile);
                else
                    base.LastError = errorMessage;
            }                        
        
            return _ImportDataModel;
        }
               
        #region IDisposable Members        
        #endregion

    }
    #endregion
}
