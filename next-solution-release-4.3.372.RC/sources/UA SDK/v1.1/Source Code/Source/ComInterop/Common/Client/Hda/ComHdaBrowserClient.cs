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
using Opc.Ua.Com;
using OpcRcw.Hda;
using OpcRcw.Comn;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// Browses areas and sources in the AE server.
    /// </summary>
    public class ComHdaBrowserClient : ComObject
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComHdaBrowserClient"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="itemId">The qualified area name.</param>
        public ComHdaBrowserClient(
            ComHdaClient client,
            string itemId)
        {
            m_client = client;
            m_itemId = itemId; 
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Utils.SilentDispose(m_enumerator);
                m_enumerator = null;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the next AE area or source.
        /// </summary>
        /// <returns>A DA element. Null if nothing left to browse.</returns>
        public BaseInstanceState Next(ISystemContext context, ushort namespaceIndex)
        {
            // check if already completed.
            if (m_completed)
            {
                return null;
            }

            // create the browser.
            if (base.Unknown == null)
            {
                base.Unknown = m_client.CreateBrowser();

                if (base.Unknown == null)
                {
                    return null;
                }

                if (!String.IsNullOrEmpty(m_itemId))
                {
                    if (!ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_DIRECT, m_itemId))
                    {
                        return null;
                    }
                }
            }

            // create the enumerator if not already created.
            if (m_enumerator == null)
            {
                m_enumerator = CreateEnumerator(true);
                m_branches = true;

                // a null indicates an error.
                if (m_enumerator == null)
                {
                    m_completed = true;
                    return null;
                }
            }

            // need a loop in case errors occur fetching element metadata.
            BaseInstanceState node = null;

            do
            {
                // fetch the next name.
                string name = m_enumerator.Next();

                // a null indicates the end of list.
                if (name == null)
                {
                    if (m_branches)
                    {
                        m_enumerator.Dispose();
                        m_enumerator = CreateEnumerator(false);
                        m_branches = false;

                        // a null indicates an error.
                        if (m_enumerator != null)
                        {
                            continue;
                        }
                    }

                    m_completed = true;
                    return null;
                }

                // create the node.
                if (m_branches)
                {
                    string itemId = GetBranchPosition(m_itemId, name);
                    node = new HdaBranchState(itemId, name, namespaceIndex);

                }
                else
                {
                    string itemId = GetItemId(name);
                    node = new HdaItemState(itemId, name, namespaceIndex);
                }

                break;
            }
            while (node == null);

            // return node.
            return node;
        }

        /// <summary>
        /// Finds the branch.
        /// </summary>
        public BaseObjectState FindBranch(ISystemContext context, string itemId, ushort namespaceIndex)
        {
            // create the browser.
            if (base.Unknown == null)
            {
                base.Unknown = m_client.CreateBrowser();

                if (base.Unknown == null)
                {
                    return null;
                }
            }

            // goto branch.
            if (!ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_DIRECT, itemId))
            {
                return null;
            }

            // find browse name via parent.
            if (!ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_UP, String.Empty))
            {
                return null;
            }

            // find the branch.
            return FindBranch(context,  GetBranchPosition(null, null), itemId, namespaceIndex);
        }

        /// <summary>
        /// Recusively finds the branch.
        /// </summary>
        private BaseObjectState FindBranch(ISystemContext context, string parentId, string itemId, ushort namespaceIndex)
        {
            // remove the enumerator.
            if (m_enumerator != null)
            {
                m_enumerator.Dispose();
                m_enumerator = null;
            }

            // find item at current level.
            m_enumerator = CreateEnumerator(true);
            List<string> children = new List<string>();

            do
            {
                // fetch the next name.
                string name = m_enumerator.Next();

                // a null indicates the end of list.
                if (name == null)
                {
                    break;
                }

                // create the node.
                string targetId = GetBranchPosition(parentId, name);

                if (itemId == targetId)
                {
                    return new HdaBranchState(itemId, name, namespaceIndex);
                }

                children.Add(targetId);
            }
            while (true);

            // recursively search for item ids.
            for (int ii = 0; ii < children.Count; ii++)
            {
                if (!ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_DIRECT, children[ii]))
                {
                    continue;
                }

                BaseObjectState node = FindBranch(context, children[ii], itemId, namespaceIndex);

                if (node != null)
                {
                    return node;
                }
            }

            m_completed = true;
            return null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Changes the browse position.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="itemId">The target.</param>
        private bool ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION direction, string itemId)
        {
            string methodName = "IOPCHDA_Browser.ChangeBrowsePosition";

            try
            {
                IOPCHDA_Browser server = BeginComCall<IOPCHDA_Browser>(methodName, true);
                server.ChangeBrowsePosition(direction, itemId);
                return true;
            }
            catch (Exception e)
            {
                if (ComUtils.IsUnknownError(e, ResultIds.E_FAIL))
                {
                    ComCallError(methodName, e);
                }

                return false;
            }
            finally
            {
                EndComCall(methodName);
            }
        }

        /// <summary>
        /// Gets the branch position.
        /// </summary>
        private string GetBranchPosition(string parentId, string name)
        {
            string methodName = "IOPCHDA_Browser.GetBranchPosition";

            // need to move down to get the browse position.
            if (!String.IsNullOrEmpty(name))
            {
                if (!ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_DOWN, name))
                {
                    return null;
                }
            }

            string itemId = null;

            try
            {
                IOPCHDA_Browser server = BeginComCall<IOPCHDA_Browser>(methodName, true);
                server.GetBranchPosition(out itemId);
            }
            catch (Exception e)
            {
                ComCallError(methodName, e);
                return null;
            }
            finally
            {
                EndComCall(methodName);

                // restore browse position.
                if (!String.IsNullOrEmpty(name))
                {
                    ChangeBrowsePosition(OPCHDA_BROWSEDIRECTION.OPCHDA_BROWSE_DIRECT, parentId);
                }
            }

            return itemId;
        }

        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        private string GetItemId(string name)
        {
            string methodName = "IOPCHDA_Browser.GetItemID";

            string itemId = null;

            try
            {
                IOPCHDA_Browser server = BeginComCall<IOPCHDA_Browser>(methodName, true);
                server.GetItemID(name, out itemId);
            }
            catch (Exception e)
            {
                ComCallError(methodName, e);
                return null;
            }
            finally
            {
                EndComCall(methodName);
            }

            return itemId;
        }

        /// <summary>
        /// Creates an enumerator for the current browse position.
        /// </summary>
        /// <param name="branches">if set to <c>true</c> then browse branches.</param>
        /// <returns>The wrapped enumerator.</returns>
        private EnumString CreateEnumerator(bool branches)
        {
            IEnumString unknown = null;

            string methodName = "IOPCHDA_Browser.GetEnum";

            try
            {
                IOPCHDA_Browser server = BeginComCall<IOPCHDA_Browser>(methodName, true);

                OPCHDA_BROWSETYPE browseType = OPCHDA_BROWSETYPE.OPCHDA_ITEMS;

                if (branches)
                {
                    browseType = OPCHDA_BROWSETYPE.OPCHDA_BRANCH;
                }

                server.GetEnum(browseType, out unknown);
            }
            catch (Exception e)
            {
                ComCallError(methodName, e);
                return null;
            }
            finally
            {
                EndComCall(methodName);
            }

            // wrapper the enumrator. hardcoding a buffer size of 256.
            return new EnumString(unknown, 256);
        }
        #endregion

        #region Private Fields
        private ComHdaClient m_client;
        private string m_itemId;
        private Opc.Ua.Com.Client.EnumString m_enumerator;
        private bool m_completed;
        private bool m_branches;
        #endregion
    }
}
