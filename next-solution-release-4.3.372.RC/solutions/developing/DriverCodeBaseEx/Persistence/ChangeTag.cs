////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\ChangeSettings.cs
//
// summary:	Implements the Change settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Opc.Ua;

namespace DriverCodeBaseEx
{
    /// <summary>   Change setting of the base communication driver. </summary>
    [DeferredDeletion(false)]
    public class ChangeTag : XPObject
    {

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        /// <param name="tag" type="Tag">           The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChangeTag(Session session, Tag tag)
            : this(session)
        {
            _NodeId = tag.TagNode.NodeId;
            _DynamicSettings = tag.TagNode.DynamicSettings;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected ChangeTag()
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


        #endregion
    }


}



