using DriverSettingsInterfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using UFInterfaces.Editors;

namespace UFUAModel.Helpers
{
    public class CommunicationDriverInfo
    {
        #region Declarations
        readonly string assemblyName;
        Assembly assemblyDriver;
        ICommunicationDriverWpfEditing driverWpfEditing;
        DriverXmlInfo driverXmlInfo;
        #endregion

        #region Constructors
        public CommunicationDriverInfo(string assemblyName)
        {
            this.assemblyName = assemblyName;

            LoadAssemblyDriver();
            LoadDriverXmlInfo();
        }
        #endregion

        #region Private Methods
        void LoadAssemblyDriver()
        {
            if (assemblyDriver == null)
            {
                try
                {
                    var assemblyPath = String.Format("{0}\\{1}", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), assemblyName);
                    assemblyDriver = Assembly.LoadFile(assemblyPath);

                    assemblyPath = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(assemblyPath);
                    var types = Assembly.LoadFile(assemblyPath).GetTypes();
                    driverWpfEditing = (from t in types
                                        where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                        select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                }
            }
        }

        void LoadDriverXmlInfo()
        {
            if (driverXmlInfo == null)
            {
                try
                {
                    var doc = XElement.Load(UFUAServerInfo.UFUAServerInfo.GetDriverListFile());
                    driverXmlInfo = (from item in doc.Descendants("Driver")
                                     where item.Attribute("AssemblyName").Value.ToLower() == assemblyName.ToLower()
                                     select new DriverXmlInfo
                                     {
                                         Factory = item.Attribute("Factory").Value,
                                         FriendlyName = item.Attribute("FriendlyName").Value,
                                         Help = item.Attribute("Help").Value,
                                         AssemblyName = item.Attribute("AssemblyName").Value,
                                         PackageType = item.Attribute("PackageType").Value
                                     }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                }
            }
        }
        #endregion

        #region Public Properties
        public string Version
        {
            get
            {
                if (assemblyDriver != null)
                {
                    try
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assemblyDriver.Location);
                        return fvi.FileVersion;
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                    }
                }

                return null;
            }
        }

        public string Help
        {
            get
            {
                if (driverXmlInfo != null)
                {
                    return driverXmlInfo.Help;
                }

                return null;
            }
        }

        public string PackageType
        {
            get
            {
                if (driverXmlInfo != null)
                {
                    return driverXmlInfo.PackageType;
                }

                return null;
            }
        }

        public bool CanImportTags
        {
            get
            {
                if (driverWpfEditing != null)
                {
                    try
                    {
                        return driverWpfEditing.IsImportTagsSupported;
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                    }
                }

                return false;
            }
        }

        string lastError;
        public string LastError
        {
            get
            {
                return lastError;
            }
            set
            {
                if (lastError == value)
                    return;

                lastError = value;
            }
        }
        #endregion
    }
}
