//-------------------------------------------------------------------------------------------------
// <copyright file="GridException.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Base class for exceptions thrown inside the grid. 
    /// </summary>
    [Serializable]
    public class GridException : ApplicationException
    {
        GridControlBase grid = null;

        /// <overloaded>
        /// Initializes a new instance of the <see cref="GridException"/> class.
        /// </overloaded>
        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class.
        /// </summary>
        public GridException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GridException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class with a specified error message
        /// and a reference to the inner exception that is the root cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception. If the innerException parameter is not a NULL reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
        public GridException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class with a specified error message
        /// and a reference to a grid.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="grid">A reference to a grid.</param>
        public GridException(string message, GridControlBase grid)
            : base(message)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridException"/> class with a specified error message,
        /// a reference to a grid, and a reference to the inner exception that is the root cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="grid">A reference to a grid.</param>
        /// <param name="inner">The exception that is the cause of the current exception. If the innerException parameter is not a NULL reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
        public GridException(string message, GridControlBase grid, Exception inner)
            : base(message, inner)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Initializes a new <see cref="GridException"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridException(SerializationInfo info, StreamingContext context)
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

        // Called by the frameworks during serialization
        // to fetch the data from an object.
        
        /// <override/>
        /// <summary>
        /// When overridden in a derived class, sets the <see
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

        // Overridden Message property. This will give the
        // proper textual representation of the exception,
        // with the added field value.
        
        /// <override/>
        /// <summary>Gets a message that represents the current exception.</summary>
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        /// <summary>
        /// Gets a reference to a <see cref="GridControlBase"/>.
        /// </summary>
        public GridControlBase Grid
        {
            get
            {
                return this.grid;
            }
        }
    }
}
