using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using Accon.AGLink;
using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;

namespace Simotion
{
    public class SimotionUISymbolicFileManagement
    {
        #region Methods use into .UI assembly to manage Symbolic file
        public static bool IsValidSymbolicFile(string projectFile)
        {
            if (string.IsNullOrEmpty(projectFile))
                return false;

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                return false;
            }

            IntPtr rootSchemaNodeHandle = new IntPtr();
            int ret = AGL4.Simotion_LoadSTISymbols(projectFile, ref rootSchemaNodeHandle);
            if (ret == AGL4.AGL40_SUCCESS)
            {
                ret = AGL4.Simotion_FreeHandle(rootSchemaNodeHandle);
                return true;
            }
            else
            {
                return false;
            }
        }
               
        private static string AddDriverName(string strConnectionString)
        {
            return CommunicationDriver.GetConnectionString(strConnectionString, "Drivers", SimotionProtocol.GetDriverName(), null);
        }
        public enum FilePath
        {
            SettingFileName,
            FileWithFullPath
        }

        public static string GetSymbolicFileName(string strConnectionString, SimotionStationSettings station, FilePath path)
        {
            string result = station.SymbolicFile;
            using (IDataLayer idl = CommunicationDriver.GetSpecificDataLayer(AddDriverName(strConnectionString), out string dummy, out InMemoryDataStore inMemorySubscriberID, out bool targetIsFile))
            {
                // verify if file was created with old driver version
                if (string.IsNullOrEmpty(result))
                {
                    if (targetIsFile)
                    {
                        result = SimotionProtocol.GetOldFileName(station.Name);
                    
                        if (!FileExist(Path.Combine(SimotionProtocol.GetDriverPath(strConnectionString), result)))
                            result = string.Empty;
                    }
                    else
                    {
                        // leave empty
                    }
                }

                if (!string.IsNullOrEmpty(result))
                {
                    if (path == FilePath.FileWithFullPath && SimotionProtocol.IsImportedSymbolicFile(result))
                    {
                        if (targetIsFile) // driver path + file name
                            result = Path.Combine(SimotionProtocol.GetDriverPath(strConnectionString), result);
                    }
                }
            }

            return result;
        }

        public static bool UpdatedSymbolicFileName(UnitOfWork ufw, SimotionStationSettings station, string sourceImportFile)
        {
            bool result = false;
            try
            {
                // update all station with the same symbolic file and the this station
                SimotionStationSettings s = (from st in new XPQuery<SimotionStationSettings>(ufw).AsParallel() where st.Name == station.Name select st).FirstOrDefault();
                if (s != null)
                {
                    s.SymbolicFile = sourceImportFile;
                    result = true;
                }
            }
            catch (Exception ex) {  }

            return result;
        }


        public static bool ImportSymbolicFile(string strConnectionString, UnitOfWork ufw, string stationName, SimotionProtocol.SaveMode saveMode, ref string sourceImportFile)
        {            
            try
            {
                using (IDataLayer idl = CommunicationDriver.GetSpecificDataLayer(AddDriverName(strConnectionString), out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile))
                {
                    if (idl == null)
                        return false;

                    // imported file contain only file name
                    string destFile = SimotionProtocol.GetSymbolicFileImportName(sourceImportFile);

                    if (targetIsFile)
                    {
                        string driverPath = SimotionProtocol.GetDriverPath(strConnectionString);
                        switch (saveMode)
                        {
                            case SimotionProtocol.SaveMode.SaveOverride:
                                // do nothing specific
                                break;

                            case SimotionProtocol.SaveMode.AutoGenerateAlternativeName:                                
                                string newFileName = Path.GetFileNameWithoutExtension(sourceImportFile);
                                string newFileExt = Path.GetExtension(sourceImportFile);
                                int progr = 0;
                                while (true)
                                {
                                    progr++;
                                    destFile = string.Format("{0}{1}{2}", newFileName, progr, newFileExt);
                                    // if file don't exit --> Ok
                                    if (!FileExist(Path.Combine(driverPath, destFile)))
                                        break;
                                }
                                break;
                        }

                        if (sourceImportFile.ToLower() != Path.Combine(driverPath, destFile).ToLower())
                        {
                            // force overwrite when copy
                            File.Copy(sourceImportFile, Path.Combine(driverPath, destFile), true);
                        }
                    }
                    else // to db
                    {
                        if (ufw!= null)
                        {
                            // update all station with the same symbolic file and the this station
                            List<SimotionStationSettings> stations = (from s in new XPQuery<SimotionStationSettings>(ufw).AsParallel() where s.Name == stationName || s.SymbolicFile.ToLower() == destFile.ToLower() select s).ToList();
                            if (stations.Count > 0)
                            {
                                foreach (var f in stations)
                                {
                                    f.FileBody = File.ReadAllBytes(sourceImportFile);
                                    f.LastUpdate = DateTime.UtcNow;
                                }
                            }
                        }
                    }
                    sourceImportFile = destFile;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool IsSymbolicFileUsedByOtherStations(string strConnectionString, string sourceStationName, string sourceFile, out string othetStationName)
        {
            othetStationName = null;

            using (IDataLayer idl = CommunicationDriver.GetSpecificDataLayer(AddDriverName(strConnectionString), out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile))
            {
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get all configutarion of Simotion Driver
                    SimotionDriverSettings configuration = null;
                    try
                    {
                        configuration = (from tag in new XPQuery<SimotionDriverSettings>(ufw).AsParallel() select tag).Single();
                    }
                    catch (Exception ex)
                    {
                        configuration = new SimotionDriverSettings(ufw);
                        configuration.DefaultSettings();
                    }
                    if ((configuration != null) && (configuration.StationSettings.Count > 0))
                    {
                        foreach (SimotionStationSettings s in configuration.StationSettings)
                        {
                            if (s.Name != sourceStationName && s.SymbolicFile.ToLower() == SimotionProtocol.GetSymbolicFileImportName(sourceFile).ToLower())
                            {
                                othetStationName = s.SymbolicFile;
                                break;
                            }
                        }
                    }
                }
            }


            return !string.IsNullOrEmpty(othetStationName);
        }

        public static bool FileExist(string fileTia)
        {
            bool exist = false;

            try
            {
                exist = File.Exists(fileTia);
            }
            catch (Exception ex) { }

            return exist;
        }

        public static bool CopySymbolicFile(string sourceconn, SimotionStationSettings sourceStation, string targetconn)
        {
            bool result = false;

            using (IDataLayer sourceIdl = CommunicationDriver.GetSpecificDataLayer(AddDriverName(sourceconn), out string sourceFilebase, out InMemoryDataStore sourceInMemory, out bool sourceIsFile))
            {
                if (sourceIsFile)
                {
                    // check if is driver file configuration
                    if (string.IsNullOrEmpty(sourceStation.SymbolicFile))
                    {
                        if (FileExist(Path.Combine(SimotionProtocol.GetDriverPath(sourceconn), SimotionProtocol.GetOldFileName(sourceStation.Name))))
                            sourceStation.SymbolicFile = SimotionProtocol.GetOldFileName(sourceStation.Name);
                    }
                }

                if (!string.IsNullOrEmpty(sourceStation.SymbolicFile) && SimotionProtocol.IsImportedSymbolicFile(sourceStation.SymbolicFile))
                {
                    byte[] fileBody = null;
                    try
                    {
                        if (sourceIsFile)
                            fileBody = File.ReadAllBytes(Path.Combine(SimotionProtocol.GetDriverPath(sourceconn), sourceStation.SymbolicFile));
                        else
                            fileBody = sourceStation.FileBody;
                    }
                    catch (Exception ex) { }

                    using (IDataLayer Targetidl = CommunicationDriver.GetSpecificDataLayer(AddDriverName(targetconn), out string targetFilebase, out InMemoryDataStore targetInMemory, out bool targetIsFile))
                    {
                        using (UnitOfWork ufw = new UnitOfWork(Targetidl))
                        {
                            SimotionStationSettings station = (from s in new XPQuery<SimotionStationSettings>(ufw).AsParallel() where s.Name == sourceStation.Name select s).ToList().FirstOrDefault();
                            if (station != null)
                            {
                                station.SymbolicFile = sourceStation.SymbolicFile;
                                // write sti file into driver's directory
                                try
                                {
                                    if (fileBody != null && fileBody.Length > 0)
                                    {
                                        if (targetIsFile)
                                        {
                                            using (BinaryWriter binWriter = new BinaryWriter(File.Open(Path.Combine(SimotionProtocol.GetDriverPath(targetconn), sourceStation.SymbolicFile), FileMode.Create)))
                                            {
                                                binWriter.Write(fileBody);
                                            }
                                        }
                                        else // to DB
                                        {
                                            station.FileBody = fileBody;
                                            station.LastUpdate = DateTime.UtcNow;
                                            ufw.CommitChanges();
                                        }
                                        result = true;
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                }
                else
                {
                    // no file name or external file --> no operation is required
                    result = true;
                }
            }

            return result;
        }
        #endregion
    }
}
