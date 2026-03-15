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
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Inherit from ConnectionPoint. Used as shape port.
	/// </summary>
	[ Serializable ]
	[ TypeConverter( typeof( ExpandableObjectConverter ) ) ]
	public class CentralPort
		: ConnectionPoint
	{
		#region Members
		/// <summary>
		/// Indication whether port will draw on model.
		/// </summary>
		private bool m_bDrawPort;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets a value indicating whether port is visible.
		/// </summary>
		/// <value><c>true</c> if port is visible ; otherwise, <c>false</c>.</value>
		[ Browsable( true ) ]
		[ EditorBrowsable( EditorBrowsableState.Never ) ]
		public bool DrawCentralPort
		{
			get
			{
				return m_bDrawPort;
			}
			set
			{
				if( m_bDrawPort != value && OnPropertyChanging( DPN.DrawCentralPort, value ) )
				{
					// make history entry
					RecordPropertyChanged( DPN.DrawCentralPort );
					// set new value
					m_bDrawPort = value;
					// raise property changed event
					OnPropertyChanged( DPN.DrawCentralPort );
				}
			}
		}
		#endregion

		#region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CentralPort"/> class.
        /// </summary>
		public CentralPort()
			: base()
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralPort"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="gpPortShape">The gp port shape.</param>
		public CentralPort( Node container, GraphicsPath gpPortShape )
			: base( container, gpPortShape )
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralPort"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
		public CentralPort( CentralPort src )
			: base( src )
		{
            m_bDrawPort = src.m_bDrawPort;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralPort"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
		public CentralPort( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
            foreach (SerializationEntry entry in info)
            {
              if(entry.Name=="drawCentralPort")
                m_bDrawPort = bool.Parse(info.GetString("drawCentralPort"));           
            }  
        }
		#endregion

		#region Class overrides
        /// <summary>
        /// Gets the object data to serialize instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("drawCentralPort", m_bDrawPort);

        }
		/// <summary>
		/// Renders the specified graphics.
		/// </summary>
		/// <param name="gfx">Graphics to draw on.</param>
		protected override void Render(Graphics gfx)
		{
			if( this.DrawCentralPort )
			{
				base.Render( gfx );
			}
		}
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
		public override object Clone()
		{
			return new CentralPort( this );
		}
		#endregion
	}
}
