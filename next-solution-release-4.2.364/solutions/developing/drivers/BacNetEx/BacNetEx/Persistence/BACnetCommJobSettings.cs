////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\BACnetCommJobSettings.cs
//
// summary:	Implements the driver BACnet communications job settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace BACnet
{
    /// <summary>   Settings for the protocol's task(BACnetCommJob). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BACnetCommJobSettings : CommJobSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from BACnetCommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetCommJobSettings(Session session, BACnetCommJob job)
            : base(session, job)
        {
            _BACnetObjectType = job.BACnetObjectType;
            _ObjectName = job.ObjectName;
            _PropertyIdentifier = job.PropertyIdentifier;
            _COVEnable = job.COVEnable;
            _DataSize = job.DataSize;
            _ArrayIndex = job.ArrayIndex;
            _DataLogMode = job.DataLogMode;
            _PriorityLevel = job.PriorityLevel;
            _InstanceNumber = job.InstanceNumber;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from BACnetCommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected BACnetCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _BACnetObjectType = BACnetEnums.ObjectTypes.DEVICE;
            _ObjectName = "";
            _PropertyIdentifier = BACnetEnums.PropertyIdentifier.PRESENT_VALUE;
            _COVEnable = false;
            _DataSize = 0;
            _ArrayIndex = 1;
            _DataLogMode = DataLogModes.None;
            _PriorityLevel = BACnetEnums.DefaultPriorityLevel;
            _InstanceNumber = -1;
        }

        #region Properties

        /// <summary>   The Object Type. </summary>
        private BACnetEnums.ObjectTypes _BACnetObjectType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Object Type. </summary>
        ///
        /// <value> The Object Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetEnums.ObjectTypes BACnetObjectType
        {
            get
            {
                return _BACnetObjectType;
            }
            set
            {
                SetPropertyValue("BACnetObjectType", ref _BACnetObjectType, value);
            }
        }
        /// <summary>   The object name. </summary>
        private string _ObjectName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   object name. </summary>
        ///
        /// <value> The object name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Size(SizeAttribute.Unlimited)]
        public string ObjectName
        {
            get
            {
                return _ObjectName;
            }
            set
            {
                SetPropertyValue("ObjectName", ref _ObjectName, value);
            }
        }

        /// <summary>   The Property Identifier. </summary>
        private BACnetEnums.PropertyIdentifier _PropertyIdentifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Property Identifier. </summary>
        ///
        /// <value> The Property Identifier </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetEnums.PropertyIdentifier PropertyIdentifier
       {
            get
            {
                return _PropertyIdentifier;
            }
            set
            {
                SetPropertyValue("PropertyIdentifier", ref _PropertyIdentifier, value);
            }
        }
        /// <summary>   The COV Enable. </summary>
        private bool _COVEnable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   COV Enable. </summary>
        ///
        /// <value> The COV Enable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool COVEnable
        {
            get
            {
                return _COVEnable;
            }
            set
            {
                SetPropertyValue("COVEnable", ref _COVEnable, value);
            }
        }

        /// <summary>   The Data Size. </summary>
        private ushort _DataSize;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data Size. </summary>
        ///
        /// <value> The Data Size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort DataSize
        {
            get
            {
                return _DataSize;
            }
            set
            {
                SetPropertyValue("DataSize", ref _DataSize, value);
            }
        }

        /// <summary>   The Array Index. </summary>
        private ushort _ArrayIndex;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Array Index. </summary>
        ///
        /// <value> The Array Index. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort ArrayIndex
        {
            get
            {
                return _ArrayIndex;
            }
            set
            {
                SetPropertyValue("ArrayIndex", ref _ArrayIndex, value);
            }
        }

        /// <summary>   The Data Log Mode. </summary>
        private DataLogModes _DataLogMode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data Log Mode. </summary>
        ///
        /// <value> The Data Log Mode. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataLogModes DataLogMode
        {
            get
            {
                return _DataLogMode;
            }
            set
            {
                SetPropertyValue("DataLogMode", ref _DataLogMode, value);
            }
        }
        /// <summary>   The PriorityLevel. </summary>
        private PriorityLevels _PriorityLevel;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Priority Level. </summary>
        ///
        /// <value> The Priority Level. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public PriorityLevels PriorityLevel
        {
            get { return _PriorityLevel; }
            set
            {
                SetPropertyValue("PriorityLevel", ref _PriorityLevel, value);
            }
        }

        /// <summary>   The Instance Number. </summary>
        private Int32 _InstanceNumber = -1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Instance Number. </summary>
        ///
        /// <value> The Instance Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Int32 InstanceNumber
        {
            get
            {
                return _InstanceNumber;
            }
            set
            {
                SetPropertyValue("InstanceNumber", ref _InstanceNumber, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            return null;
        }

        #endregion
    
    }
}
