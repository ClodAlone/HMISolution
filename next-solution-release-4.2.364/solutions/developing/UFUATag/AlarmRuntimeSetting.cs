using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel
{
    /// <summary>   Change setting for an alarm threshold. </summary>
    [DeferredDeletion(false)]
    public class AlarmRuntimeSetting : XPObject
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public AlarmRuntimeSetting(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected AlarmRuntimeSetting()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Properties
        /// <summary>   The AlarmStatus NodeId. </summary>
        private NodeId _AlarmStatusNodeId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the AlarmStatus NodeId. </summary>
        ///
        /// <value> The AlarmStatus NodeId. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [ValueConverter(typeof(ConvertNodeId))]
        [Size(SizeAttribute.Unlimited)]
        public NodeId AlarmStatusNodeId
        {
            get
            {
                return _AlarmStatusNodeId;
            }
            set
            {
                SetPropertyValue("AlarmStatusNodeId", ref _AlarmStatusNodeId, value);
            }
        }

        /// <summary>   The Alarm new text. </summary>
        private String _AlarmText;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Alarm text. </summary>
        ///
        /// <value> The Alarm text. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Size(SizeAttribute.Unlimited)]
        public String AlarmText
        {
            get
            {
                return _AlarmText;
            }
            set
            {
                SetPropertyValue("AlarmText", ref _AlarmText, value);
            }
        }

        #endregion

        #region Converters for XPObject

        /// <summary>   A convert node identifier. </summary>
        public class ConvertNodeId : ValueConverter
        {

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes this object from the given convert from storage type. </summary>
            ///
            /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
            ///                                         illegal values. </exception>
            ///
            /// <param name="value" type="object">  The value. </param>
            ///
            /// <returns>   from converted storage type. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            public override object ConvertFromStorageType(object value)
            {
                var id = value as String;
                if (id == null)
                    throw new ArgumentException("Parameters must be a string value");

                return NodeId.Parse((String)value);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Converts a value to a storage type. </summary>
            ///
            /// <param name="value" type="object">  The value. </param>
            ///
            /// <returns>   The given data converted to a storage type. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            public override object ConvertToStorageType(object value)
            {
                return value.ToString();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the type of the storage. </summary>
            ///
            /// <value> The type of the storage. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            public override Type StorageType
            {
                get
                {
                    return typeof(string);
                }
            }
        }
        #endregion
    }
}
