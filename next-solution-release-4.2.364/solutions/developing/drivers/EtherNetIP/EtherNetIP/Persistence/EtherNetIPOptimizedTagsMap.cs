using DevExpress.Xpo;
using System;

namespace EtherNetIP
{
    [Persistent("DriverEtherNetIPOptimizedTagsMap")]  
    [DeferredDeletion(false)]
    public class EtherNetIPOptimizedTagsMap : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public EtherNetIPOptimizedTagsMap(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.

            //session.UpdateSchema(typeof(BACnetSubscribeId));
            //session.CreateObjectTypeRecords(typeof(BACnetSubscribeId));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected EtherNetIPOptimizedTagsMap()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }       
        #endregion

        //public void DefaultSettings()
        //{
        //    _subcribeId = 0;
        //    _name = string.Empty;
        //}
        //public void SaveSubcribeIdSettings(BACnetSubscribeId settings)
        //{
        //    settings.SubcribeId = SubcribeId;
        //    settings.Name = Name;
        //}

        //public void CopyProperties(BACnetSubscribeId st)
        //{
        //    _subcribeId = st.SubcribeId;
        //    _name = st.Name;
        //}

        // station name
        private string _StationName;        
        public string StationName
        {
            get
            {
                return _StationName;
            }
            set
            {
                SetPropertyValue("StationName", ref _StationName, value);
            }
        }
       
        /// <summary>
        /// Internal PLC parameter used to indentify when program was downloaded (changed every time program was updated)
        /// </summary>
        private ushort attribute1;
        public ushort Attribute1
        {
            get
            {
                return attribute1;
            }
            set
            {
                SetPropertyValue("Attribute1", ref attribute1, value);
            }
        }

        /// <summary>
        /// Internal PLC parameter used to indentify when program was downloaded (changed every time program was updated)
        /// </summary>
        private ushort attribute2;
        public ushort Attribute2
        {
            get
            {
                return attribute2;
            }
            set
            {
                SetPropertyValue("Attribute2", ref attribute2, value);
            }
        }

        /// <summary>
        /// Internal PLC parameter used to indentify when program was downloaded (changed every time program was updated)
        /// </summary>
        private uint attribute3;
        public uint Attribute3
        {
            get
            {
                return attribute3;
            }
            set
            {
                SetPropertyValue("Attribute3", ref attribute3, value);
            }
        }

        /// <summary>
        /// Internal PLC parameter used to indentify when program was downloaded (changed every time program was updated)
        /// </summary>
        private uint attribute4;
        public uint Attribute4
        {
            get
            {
                return attribute4;
            }
            set
            {
                SetPropertyValue("Attribute4", ref attribute4, value);
            }
        }

        /// <summary>
        /// Internal PLC parameter used to indentify when program was downloaded (changed every time program was updated)
        /// </summary>
        private uint attribute10;
        public uint Attribute10
        {
            get
            {
                return attribute10;
            }
            set
            {
                SetPropertyValue("Attribute10", ref attribute10, value);
            }
        }

        /// <summary>
        /// PLC firmware version
        /// </summary>
        private byte firmwareVersion;
        public byte FirmwareVersion
        {
            get
            {
                return firmwareVersion;
            }
            set
            {
                SetPropertyValue("FirmwareVersion", ref firmwareVersion, value);
            }
        }

        /// <summary>
        /// Identify last time information was updated
        /// </summary>
        private DateTime lastUpdate = DateTime.UtcNow;
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            set
            {
                SetPropertyValue("LastUpdate", ref lastUpdate, value);
            }
        }

        /// <summary>
        /// Internal PLC parameters contain list of instance number associated to program (used to manage station.m_listProgramsTrueIstances into storage)
        /// </summary>
        private byte[] listProgramsTrueIstances = null;
        public byte[] ListProgramsTrueIstances
        {
            get
            {
                return listProgramsTrueIstances;
            }
            set
            {
                SetPropertyValue("ListProgramsTrueIstances", ref listProgramsTrueIstances, value);
            }
        }

        /// <summary>
        /// Internal PLC list contain running programs (used to manage station.m_programList into storage)
        /// </summary>
        private byte[] programList = null;
        public byte[] ProgramList
        {
            get
            {
                return programList;
            }
            set
            {
                SetPropertyValue("ProgramList", ref programList, value);
            }
        }

        /// <summary>
        /// Internal PLC list contain PLC program id associated to PLC program name (used to manage station.m_mapProgramsInstances into storage)
        /// </summary>
        private byte[] mapProgramsInstances = null;
        public byte[] MapProgramsInstances
        {
            get
            {
                return mapProgramsInstances;
            }
            set
            {
                SetPropertyValue("MapProgramsInstances", ref mapProgramsInstances, value);
            }
        }

        /// <summary>
        /// Internal PLC list contain PLC program addrss associated to PLC program ID (used to manage station.m_mapProgramsAddresses into storage)
        /// </summary>
        private byte[] mapProgramsAddresses = null;
        public byte[] MapProgramsAddresses
        {
            get
            {
                return mapProgramsAddresses;
            }
            set
            {
                SetPropertyValue("MapProgramsAddresses", ref mapProgramsAddresses, value);
            }
        }

        /// <summary>
        /// Internal PLC list contain direct address of simbolic PLC variable (used to manage station.m_mapPlcTagInstanceInfo into storage)
        /// </summary>
        private byte[] mapPlcTagInstanceInfo = null;
        public byte[] MapPlcTagInstanceInfo
        {
            get
            {
                return mapPlcTagInstanceInfo;
            }
            set
            {
                SetPropertyValue("MapPlcTagInstanceInfo", ref mapPlcTagInstanceInfo, value);
            }
        }

        /// <summary>
        /// Internal PLC list contain direct address of simbolic PLC struct (used to manage station.m_mapPlcTagInstanceInfo into storage)
        /// </summary>
        private byte[] mapPlcTemplateInfo = null;
        public byte[] MapPlcTemplateInfo
        {
            get
            {
                return mapPlcTemplateInfo;
            }
            set
            {
                SetPropertyValue("MapPlcTemplateInfo", ref mapPlcTemplateInfo, value);
            }
        }
    }
}
