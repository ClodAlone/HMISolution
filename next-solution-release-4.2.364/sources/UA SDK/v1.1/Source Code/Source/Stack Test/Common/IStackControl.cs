/* ========================================================================
 * Copyright (c) 2005-2009 The OPC Foundation, Inc. All rights reserved.
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
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Opc.Ua.StackTest
{
    #region Interface IStackControl
    /// <summary>
    /// A interface that is used recieve events from the stack.
    /// </summary>
    public interface IStackEventSink
    {
        /// <summary>
        /// A certificate provided in an open secure channel message was rejected.
        /// </summary>
        void CertificateRejected(uint channelId, X509Certificate2 certificate);

        /// <summary>
        /// Reports an error during message processing.
        /// </summary>
        void MessageError(uint channelId, ServiceResult result);
    }
    #endregion

    #region Interface IStackControl
    /// <summary>
    /// A interface that is used to control the stack during testing.
    /// </summary>
    public interface IStackControl
    {
        /// <summary>
        /// Sets the event sink for the stack.
        /// </summary>
        void SetEventSink(IStackEventSink sink);

        /// <summary>
        /// Queues an action.
        /// </summary>
        void QueueAction(StackAction action);
    }
    #endregion

    #region Class StackAction
    /// <summary>
    /// An action which the stack should take.
    /// </summary>
    /// <remarks>
    /// Actions with a RepeatCount >= 1 are placed in the action queue. Each time a request arrives an action
    /// is removed from the queue, applied and the repeat count is decremented. When the repeat count reaches 0
    /// it is removed from the head of the action queue.
    /// 
    /// Actions with a RepeatCount = 0 are applied immediately.    
    /// </remarks>
    public class StackAction
    {
        #region Public Properties
        /// <summary>
        /// The action to take.
        /// </summary>
        public StackActionType ActionType
        {
            get { return m_actionType;  } 
            set { m_actionType = value; }
        }

        /// <summary>
        /// How long the action should last in milliseconds.
        /// </summary>
        public int Duration
        {
            get { return m_duration;  } 
            set { m_duration = value; }
        }
        #endregion

        #region Private Fields
        private StackActionType m_actionType;
        private int m_duration;
        #endregion
    }
    #endregion

    #region Enums StackActionType
    /// <summary>
    /// The types of stack actions.
    /// </summary>
    public enum StackActionType
    {
        /// <summary>
        /// The socket for the connection should be forcably closed.
        /// </summary>
        CloseConnectionSocket,

        /// <summary>
        /// The listening socket for the server should be forceably closed.
        /// </summary>
        CloseListeningSocket,

        /// <summary>
        /// The listening socket for the server should be re-opened.
        /// </summary>
        ReOpenListeningSocket,

        /// <summary>
        /// Corrupt message chunk
        /// </summary>
        CorruptMessageChunk,

        /// <summary>
        /// Re-use a sequence number
        /// </summary>
        ReuseSequenceNumber
    }
    #endregion
}
