#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Collection of nodes that can be transferred to and from the clipboard.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a specialized NodeCollection class that is used for storing nodes
    /// on the clipboard. It maintains a GUID (Globally Unique IDentifier) that
    /// identifies the object that placed the collection of nodes on the clipboard.
    /// </para>
    /// </remarks>
    [Serializable()]
    [Description("Collection of nodes that can be transferred to and from the clipboard.")]
    [DefaultProperty("Item")]
    public class ClipboardNodeCollection
        : NodeCollection
    {
        #region Class members
        private Guid srcGuid = Guid.Empty;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardNodeCollection"/> class.
        /// Constructs a ClipboardNodeCollection given a GUID.
        /// </summary>
        /// <param name="srcGuid">GUID that identifies the source of the nodes.</param>
        public ClipboardNodeCollection(Guid srcGuid)
        {
            this.srcGuid = srcGuid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardNodeCollection"/> class.
        /// Serialization constructor for a ClipboardNodeCollection.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected ClipboardNodeCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.srcGuid = (Guid)info.GetValue("srcGuid", typeof(Guid));
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Compares the given GUID to the source GUID of the collection.
        /// </summary>
        /// <param name="guid">GUID to compare.</param>
        /// <returns>True if the GUIDs match; otherwise False.</returns>
        public bool CompareSourceGuid(Guid guid)
        {
            return guid.Equals(srcGuid);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("srcGuid", this.srcGuid);
        }

        #endregion
    }
}
