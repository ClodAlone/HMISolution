#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Drawing;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Summary description for HeadEndPoint.
	/// </summary>
	[ Serializable ]
	public class HeadEndPoint
		: EndPoint
	{
		#region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadEndPoint"/> class.
        /// </summary>
        /// <param name="container">The <see cref="Syncfusion.Windows.Forms.Diagram.PathNode"/> container.</param>
        /// <param name="ptLocation">The handle location in parent coordinates.</param>
		public HeadEndPoint( PathNode container, PointF ptLocation )
			: base( container, ptLocation )
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadEndPoint"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
		public HeadEndPoint( HeadEndPoint src )
			: base( src )
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadEndPoint"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
		protected HeadEndPoint( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{}
		#endregion
		
		#region Class overrides
        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return "HeadEndPoint";
		}

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public override object Clone()
		{
			return new HeadEndPoint( this );
		}
		#endregion
	}
}
