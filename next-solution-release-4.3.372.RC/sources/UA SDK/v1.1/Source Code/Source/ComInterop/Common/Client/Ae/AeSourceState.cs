/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;

namespace Opc.Ua.Com.Client
{        
    /// <summary>
    /// A object which maps a segment to a UA object.
    /// </summary>
    public partial class AeSourceState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AeSourceState"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="areaId">The area id.</param>
        /// <param name="qualifiedName">The qualified name for the source.</param>
        /// <param name="name">The name of the source.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        public AeSourceState(
            ISystemContext context,
            string areaId,
            string qualifiedName,
            string name,
            ushort namespaceIndex)
            :
                base(null)
        {
            m_areaId = areaId;
            m_qualifiedName = qualifiedName;

            this.TypeDefinitionId = Opc.Ua.ObjectTypeIds.BaseObjectType;
            this.NodeId = AeModelUtils.ConstructIdForSource(m_areaId, name, namespaceIndex);
            this.BrowseName = new QualifiedName(name, namespaceIndex);
            this.DisplayName = this.BrowseName.Name;
            this.Description = null;
            this.WriteMask = 0;
            this.UserWriteMask = 0;
            this.EventNotifier = EventNotifiers.None;

            this.AddReference(ReferenceTypeIds.HasNotifier, true, AeModelUtils.ConstructIdForArea(m_areaId, namespaceIndex));
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the qualified name for the source.
        /// </summary>
        /// <value>The qualified name for the source.</value>
        public string QualifiedName
        {
            get { return m_qualifiedName; }
        }
        #endregion

        #region Private Fields
        private string m_areaId;
        private string m_qualifiedName;
        #endregion
    }
}
