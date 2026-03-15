////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\TagSettings.cs
//
// summary:	Implements the tag settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Opc.Ua;
using System.Threading;

namespace DriverCodeBase
{
    /// <summary>   tag settings of the base communication driver. </summary>
    [DeferredDeletion(false)]
    [Persistent("TagSettings_1")]
    public class TagSettings : XPObject
    {

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        /// <param name="tag" type="Tag">           The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TagSettings(Session session, Tag tag)
            : this(session)
        {
            _NodeId = tag.TagNode.NodeId;
            _DataType = tag.TagNode.DataType;
            _DynamicSettings = tag.TagNode.DynamicSettings;
            _SamplingInterval = tag.TagNode.SamplingInterval;
            _ByteOffset = tag.ByteOffset;
            _BitOffset = tag.BitOffset;
            _ArrayDimension = tag.TagNode.ArrayDimension;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TagSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected TagSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Properties

        /// <summary>   NodeId for linking to tag setting informations. </summary>
        private NodeId _NodeId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the identifier of the node. </summary>
        ///
        /// <value> The identifier of the node. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [ValueConverter(typeof(ConvertNodeId))]
        [Size(SizeAttribute.Unlimited)]
        public NodeId NodeId
        {
            get { return _NodeId; }
            set
            {
                SetPropertyValue("NodeId", ref _NodeId, value);
            }
        }

        /// <summary>   Type of the data. </summary>
        private NodeId _DataType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
         [ValueConverter(typeof(ConvertDataType))]
        public NodeId DataType
        {
            get
            {
                return _DataType;
            }
            set
            {
                SetPropertyValue("DataType", ref _DataType, value);
            }
        }

        /// <summary>   The dynamic settings. </summary>
        private String _DynamicSettings;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Size(SizeAttribute.Unlimited)]
        public String DynamicSettings
        {
            get
            {
                return _DynamicSettings;
            }
            set
            {
                SetPropertyValue("DynamicSettings", ref _DynamicSettings, value);
            }
        }

        /// <summary>   The sampling interval. </summary>
        private double _SamplingInterval;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the sampling interval. </summary>
        ///
        /// <value> The sampling interval. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public double SamplingInterval
        {
            get
            {
                return _SamplingInterval;
            }
            set
            {
                SetPropertyValue("SamplingInterval", ref _SamplingInterval, value);
            }
        }

        /// <summary>   Bytes offset value in the job. </summary>
        private uint _ByteOffset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the byte offset. </summary>
        ///
        /// <value> The byte offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ByteOffset
        {
            get { return _ByteOffset; }
            set
            {
                SetPropertyValue("ByteOffset", ref _ByteOffset, value);
            }
        }

        /// <summary>   Bit offset value in the job (should be useful for bit exchanges) </summary>
        private uint _BitOffset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the bit offset. </summary>
        ///
        /// <value> The bit offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint BitOffset
        {
            get { return _BitOffset; }
            set
            {
                SetPropertyValue("BitOffset", ref _BitOffset, value);
            }
        }

        /// <summary>   array dimension of the server tag. </summary>
        private uint _ArrayDimension;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the array dimension. </summary>
        ///
        /// <value> The array dimension. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ArrayDimension
        {
            get { return _ArrayDimension; }
            set
            {
                SetPropertyValue("ArrayDimension", ref _ArrayDimension, value);
            }
        }

        /// <summary>   The communications job settings. </summary>
        private CommJobSettings _CommJobSettings;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications job settings. </summary>
        ///
        /// <value> The communications job settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Association("CommJobSettings-Tags")]
        public CommJobSettings CommJobSettings
        {
            get
            {
                return _CommJobSettings;
            }
            set
            {
                SetPropertyValue("CommJobSettings", ref _CommJobSettings, value);
            }
        }

        #endregion
    }

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

    /// <summary>   A convert data type. </summary>
    public class ConvertDataType : ValueConverter
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes this object from the given convert from storage type. </summary>
        ///
        /// <param name="value" type="object">  The value. </param>
        ///
        /// <returns>   from converted storage type. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override object ConvertFromStorageType(object value)
        {
            
            

            object n = null;
            try
            {
                if (value.GetType() == typeof(int))
                {
                    n = new NodeId((uint)Convert.ToInt32(value));
                }
                else if (value.GetType() == typeof(Guid))
                {
                    n = new NodeId((Guid)value);
                }
            }
            catch
            {
            }
            return n;
            //return TypeInfo.GetDataTypeId(value);
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
            object n = null;
            try
            {
                NodeId nodo = (NodeId)value;
                if (nodo.IdType == IdType.Numeric)
                {
                    n = TypeInfo.GetBuiltInType(nodo);
                }
                else if (nodo.IdType == IdType.Guid)
                {
                    n = nodo.Identifier;
                }
            }
            catch
            { 
            }
            return n;
            //return TypeInfo.GetBuiltInType((NodeId)value);
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
                return typeof(BuiltInType);
            }
        }
    }

    #endregion
    
}
