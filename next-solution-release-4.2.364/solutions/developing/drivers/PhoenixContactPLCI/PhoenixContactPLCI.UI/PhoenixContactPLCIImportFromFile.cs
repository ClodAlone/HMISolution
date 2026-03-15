using DriverCodeBase.UI;
using System;
using System.IO;

namespace PhoenixContactPLCI.UI
{
    public class PhoenixContactPLCIImportFromFile : PhoenixContactPLCIImportBase, IDisposable
    {
        private const string ADDPATHBIN = "\\C\\STD_CNF\\R\\STD_RES\\image.bin";


        public PhoenixContactPLCIImportFromFile() : base()
        {
        }

        #region Phoenix Contact PLCI

        public string GetBinFileFromMwtProject(string file)
        {
            int lastOfFullStop = file.LastIndexOf(".");
            string namePathFile = file.Substring(0, lastOfFullStop);

            return string.Format("{0}{1}", namePathFile, ADDPATHBIN);
        }

        /// <summary>
        /// Test if selected file is a valid Mwt project file
        /// </summary>
        /// <returns></returns>
        public bool IsMwtProject(string file)
        {
            bool isValid = false;

            try
            {
                string namePathFile = GetBinFileFromMwtProject(file);

                isValid = File.Exists(namePathFile);
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            
            return (isValid);
        }

        public ImportDataModelPhoenixContactPlci ImportFromMwt(GetStationName readStationName, string file)
        {
            base.Init();

            if (IsMwtProject(file))
            {
                base.LastError = Properties.Resources.ErrorDriverTheProjectDoesNotContainTheFile;
                return _ImportDataModel;
            }
            else
            {
                _ImportDataModel = ImportFromBin(readStationName, GetBinFileFromMwtProject(file));
            }

            return _ImportDataModel;
        }
        #endregion                        
    }
}
