using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using Accon.AGLink;
using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Linq;
using System.Collections;

namespace S7TIASymbolic
{
    public class S7TIASymbolicUISymbolicFileManagement
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
            int ret = AGL4.Symbolic_LoadTIAProjectSymbols(projectFile, ref rootSchemaNodeHandle);
            if (ret == AGL4.AGL40_SUCCESS)
            {
                ret = AGL4.Symbolic_FreeHandle(rootSchemaNodeHandle);
                return true;
            }
            else
            {
                return false;
            }
        }
               
        private static string AddDriverName(string strConnectionString)
        {
            return CommunicationDriver.GetConnectionString(strConnectionString, "Drivers", S7TIAProtocol.GetDriverName(), null);
        }
        public enum FilePath
        {
            SettingFileName,
            FileWithFullPath
        }
        public static bool DeleteFileName(string strConnectionString, S7TIAStationSettings station/*, FilePath path*/)
        {
            string result = string.Empty;
            using (IDataLayer idl = CommunicationDriver.GetSpecificDataLayer(AddDriverName(strConnectionString), out string dummy, out InMemoryDataStore inMemorySubscriberID, out bool targetIsFile))
            {
                if (idl == null)
                    return false;

                if (targetIsFile)
                {
                    result = S7TIAProtocol.GetOldFileName(station.Name);
                    string path = Path.Combine(S7TIAProtocol.GetDriverPath(strConnectionString), result);
                    if (FileExist(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch (Exception ex)
                        { 
                        }
                    }
                }
                else
                {
                    // leave empty
                }
            }

            return (true);
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

        #endregion
    }
}
