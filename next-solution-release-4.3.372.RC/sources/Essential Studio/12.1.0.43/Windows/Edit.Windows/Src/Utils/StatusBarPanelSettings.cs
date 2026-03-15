#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class for managing status bar panel.
	/// </summary>
	[TypeConverter( typeof( StatusBarPanelSettings.StatusBarPanelSettingsConverter ) )]
	public class StatusBarPanelSettings
	{
		#region Internal Classes
		/// <summary>
		/// Class used for work with property grid.
		/// </summary>
		internal class StatusBarPanelSettingsConverter : ExpandableObjectConverter
		{
			public override bool GetCreateInstanceSupported( ITypeDescriptorContext context )
			{
				return false;
			}
		}
		#endregion

		#region Class Membes
		/// <summary>
		/// Underlying status bar panel.
		/// </summary>
		private StatusBarAdvPanel m_panel = null;
        /// <summary>
        /// Default width of the panel
        /// </summary>
        private int DefWidth = 80;
        /// <summary>
        /// Default value of AutoSize property
        /// </summary>
        private bool m_autoSize = false;
        /// <summary>
        /// Default value of Visible property
        /// </summary>
        private bool m_visible = true;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets visibility of the panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies visibility of the panel." )]
		[DefaultValue( true )]
		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				if( m_visible != value )
				{
					m_visible = value;
					Panel.Visible = m_visible;

					if( null != VisibilityChanged )
					{
						ValueChangedEventArgs args = new ValueChangedEventArgs( !value, value );
						VisibilityChanged( this, args );
					}
				}
			}
		}
		/// <summary>
		/// Gets underlying panel.
		/// </summary>
		[Browsable( false )]
		public StatusBarAdvPanel Panel
		{
			get
			{
				return m_panel;
			}
		}
        /// <summary>
        /// Sets the default width of the panel
        /// </summary>
        /// <param name="sizetocontent"></param>
        private void OnPanelWidthChanged(bool sizetocontent)
        {
            if (!m_autoSize)
            {
                m_panel.Width = DefWidth;
            }
        }
		/// <summary>
		/// Gets or sets panel width.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies panel width." )]
		public int Width
		{
			get
			{
                return m_panel.Width;
			}
			set
			{
                if (!m_autoSize)
                {
                    m_panel.Width = value;
                }
			}
		}
		/// <summary>
		/// Gets or sets panel auto size mode.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies panel auto size mode." )]
		[DefaultValue( false)]
		public bool AutoSize
		{
			get
			{
				return m_autoSize;
			}
			set
			{
                m_autoSize = value;
                Panel.SizeToContent = m_autoSize;
                OnPanelWidthChanged(m_autoSize);

			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates and initializes new instance of StatusBarPanelSettings.
		/// </summary>
		/// <param name="panel">Underlying status bar panel.</param>
		public StatusBarPanelSettings( StatusBarAdvPanel panel )
		{
			if( null == panel )
				throw new ArgumentNullException( "panel" );

			m_panel = panel;
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Raised when Visibility of panel is changed.
		/// </summary>
		public event ValueChangedEventHandler VisibilityChanged;
		#endregion
	}
}