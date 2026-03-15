//-------------------------------------------------------------------------------------------------
// <copyright file="GridUserCanceledException.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Use GridUserCanceledException when the user cancels an operation.
    /// </summary>
    [Serializable]
    public class GridUserCanceledException : GridException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GridUserCanceledException"/>.
        /// </summary>
        public GridUserCanceledException()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GridUserCanceledException"/> with a specified message.
        /// </summary>
        /// <param name="message">The message text.</param>
        public GridUserCanceledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GridUserCanceledException"/> with a specified message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="inner">The source of the exception.</param>
        public GridUserCanceledException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridUserCanceledException"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridUserCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <override/>
        /// <override/>
        /// <summary>
        /// Sets the <see
        /// cref="SerializationInfo" /> with information
        /// about the exception.
        /// </summary>
        /// <param name="info">The <see
        /// cref="SerializationInfo" /> that holds the
        /// serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see
        /// cref="StreamingContext" /> that contains
        /// contextual information about the source or destination. </param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <override/>
        /// <override/>
        /// <summary>Gets a message that represents the current exception.</summary>
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
