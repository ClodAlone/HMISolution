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
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Com;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// An interface to an object that parses item ids.
    /// </summary>
    public interface IItemIdParser
    {
        /// <summary>
        /// Parses the specified item id.
        /// </summary>
        /// <param name="server">The COM server that provided the item id.</param>
        /// <param name="configuration">The COM wrapper configuration.</param>
        /// <param name="itemId">The item id to parse.</param>
        /// <param name="browseName">The name of the item.</param>
        /// <returns>True if the item id could be parsed.</returns>
        bool Parse(
            ComObject server,
            ComClientConfiguration configuration,
            string itemId,
            out string browseName);
    }

    /// <summary>
    /// The default item id parser that uses the settings in the configuration.
    /// </summary>
    public class ComItemIdParser : IItemIdParser
    {
        #region IItemIdParser Members
        /// <summary>
        /// Parses the specified item id.
        /// </summary>
        /// <param name="server">The COM server that provided the item id.</param>
        /// <param name="configuration">The COM wrapper configuration.</param>
        /// <param name="itemId">The item id to parse.</param>
        /// <param name="browseName">The name of the item.</param>
        /// <returns>True if the item id could be parsed.</returns>
        public bool Parse(ComObject server, ComClientConfiguration configuration, string itemId, out string browseName)
        {
            browseName = null;

            if (configuration == null || itemId == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(configuration.SeperatorChars))
            {                
                return false;
            }

            for (int ii = 0; ii < configuration.SeperatorChars.Length; ii++)
            {
                int index = itemId.LastIndexOf(configuration.SeperatorChars[ii]);

                if (index >= 0)
                {
                    browseName = itemId.Substring(index + 1);
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
