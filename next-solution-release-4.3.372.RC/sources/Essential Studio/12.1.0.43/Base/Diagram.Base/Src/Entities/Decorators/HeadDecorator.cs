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
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Head decorator.
    /// </summary>
	[ Serializable ]
	public class HeadDecorator
		: Decorator
	{
		#region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadDecorator"/> class.
        /// </summary>
		public HeadDecorator()
			: base()
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadDecorator"/> class.
        /// </summary>
        /// <param name="grfxPath">The GRFX path.</param>
		public HeadDecorator( GraphicsPath grfxPath )
			: base( grfxPath )
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadDecorator"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
		public HeadDecorator( HeadDecorator src )
			: base( src )
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadDecorator"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
		protected HeadDecorator( SerializationInfo info, StreamingContext context )
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
			return "HeadDecorator";
		}
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public override object Clone()
		{
			return new HeadDecorator( this );
		}
		#endregion
	}
}
