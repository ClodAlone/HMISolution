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
using System.Runtime.Serialization;
using System.Collections;
using System.Drawing;

using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Tools.Controls.StatusBar;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class for managing status bar.
	/// </summary>
	[TypeConverter( typeof( StatusBarSettings.StatusBarSettingsConverter ) )]
	public class StatusBarSettings
	{
		#region Internal Classes
		/// <summary>
		/// Class used for work with property grid.
		/// </summary>
		internal class StatusBarSettingsConverter
			: ExpandableObjectConverter
		{
			/// <summary>
			/// Disables instantination.
			/// </summary>
			/// <param name="context">Current context, does not matter.</param>
			/// <returns>False.</returns>
			public override bool GetCreateInstanceSupported( ITypeDescriptorContext context )
			{
				return false;
			}
		}
		#endregion

		#region Class Members
		/// <summary>
		/// Underlying status bar.
		/// </summary>
		private StatusBarExt m_statusBar;
		/// <summary>
		/// Visibility of status bar sizing grip.
		/// </summary>
		private SizingGripVisibility m_gripVisibility = SizingGripVisibility.Visible;
		/// <summary>
		/// Settings of "Text" status bar panel.
		/// </summary>
		private StatusBarPanelSettings m_pnlText;
		/// <summary>
		/// Settings of "Status" status bar panel.
		/// </summary>
		private StatusBarPanelSettings m_pnlStatus;
		/// <summary>
		/// Settings of "Encoding" status bar panel.
		/// </summary>
		private StatusBarPanelSettings m_pnlEncoding;
		/// <summary>
		/// Settings of "Coords" status bar panel.
		/// </summary>
		private StatusBarPanelSettings m_pnlCoords;
		/// <summary>
		/// Settings of "Insert" status bar panel.
		/// </summary>
		private StatusBarPanelSettings m_pnlInsert;
		/// <summary>
		/// Settings of "FileName" status bar panel.
		/// </summary>
		private StatusBarPanelSettings m_pnlFileName;
		/// <summary>
		/// Index of status bar panel with AutoSize property forcibly set to Spring. -1 if no forced spring was set.
		/// </summary>
		private int m_springForcedIndex = -1;
		/// <summary>
		/// Old value of panel Width property forcially set to Spring.
		/// </summary>
		private int m_oldForcedSpringWidth;
        /// <summary>
        /// 
        /// </summary>
        private Tools.Controls.StatusBar.VisualStyle m_Style = Tools.Controls.StatusBar.VisualStyle.Default;
        /// <summary>
        /// 
        /// </summary>
        private Office2007Theme m_Office07ColorScheme = Office2007Theme.Blue;
        /// <summary>
        /// 
        /// </summary>
        private Office2010Theme m_Office10ColorScheme = Office2010Theme.Blue;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets bool indicating visibility of status bar.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies bool indicating visibility of status bar." )]
		[DefaultValue( false )]
		public bool Visible
		{
			get
			{
				return m_statusBar.Visible;
			}
			set
			{
				if( value != m_statusBar.Visible )
				{
					m_statusBar.Visible = value;

					if( null != VisibilityChanged )
					{
						ValueChangedEventArgs args = new ValueChangedEventArgs( !value, value );
						VisibilityChanged( this, args );
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets visibility of status bar sizing grip.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies visibility of status bar sizing grip." )]
		[DefaultValue( SizingGripVisibility.Visible )]
		public SizingGripVisibility GripVisibility
		{
			get
			{
				return m_gripVisibility;
			}
			set
			{
				if( value != m_gripVisibility )
				{
					SetGripVisibility( value );
				}
			}
		}
		/// <summary>
		/// Gets underlying status bar.
		/// </summary>
		[Browsable( false )]
		public StatusBarExt StatusBar
		{
			get
			{
				return m_statusBar;
			}
		}
        /// <summary>
        /// Gets or Sets the VisualStyle
        /// </summary>
        public Tools.Controls.StatusBar.VisualStyle VisualStyle
        {
            get
            {
                return m_Style;
            }
            set
            {
                m_Style = value;
                StatusBar.VisualStyle = value;
            }
        }
        /// <summary>
        /// Gets or Sets the Office2007color scheme
        /// </summary>
        public Office2007Theme Offcie2007ColorScheme
        {
            get
            {
                return m_Office07ColorScheme;
            }
            set
            {
                m_Office07ColorScheme = value;
                StatusBar.Office2007ColorScheme = value;
            }
        }
        /// <summary>
        /// Gets or Sets the Office2010color scheme
        /// </summary>
        public Office2010Theme Offcie2010ColorScheme
        {
            get
            {
                return m_Office10ColorScheme;
            }
            set
            {
                m_Office10ColorScheme = value;
                StatusBar.Office2010ColorScheme = value;
            }
        }
		/// <summary>
		/// Gets StatusBarPanelSettings object for "Text" panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies StatusBarPanelSettings object for \"Text\" panel." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarPanelSettings TextPanel
		{
			get
			{
				return m_pnlText;
			}
		}
		/// <summary>
		/// Gets StatusBarPanelSettings object for "Status" panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies StatusBarPanelSettings object for \"Status\" panel." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarPanelSettings StatusPanel
		{
			get
			{
				return m_pnlStatus;
			}
		}
		/// <summary>
		/// Gets StatusBarPanelSettings object for "Encoding" panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies StatusBarPanelSettings object for \"Encoding\" panel." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarPanelSettings EncodingPanel
		{
			get
			{
				return m_pnlEncoding;
			}
		}
		/// <summary>
		/// Gets StatusBarPanelSettings object for "Coords" panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies StatusBarPanelSettings object for \"Coords\" panel." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarPanelSettings CoordsPanel
		{
			get
			{
				return m_pnlCoords;
			}
		}
		/// <summary>
		/// Gets StatusBarPanelSettings object for "Insert" panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies StatusBarPanelSettings object for \"Insert\" panel." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarPanelSettings InsertPanel
		{
			get
			{
				return m_pnlInsert;
			}
		}
		/// <summary>
		/// Gets StatusBarPanelSettings object for "FileName" panel.
		/// </summary>
		[Browsable( true )]
		[Description( "Specifies StatusBarPanelSettings object for \"FileName\" panel." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarPanelSettings FileNamePanel
		{
			get
			{
				return m_pnlFileName;
			}
		}
        [Browsable(false)]
		/// <summary>
		/// Gets list of status bar panels settings.
		/// </summary>
		public StatusBarAdvPanel[] Panels
		{
			get
			{
                return StatusBar.Panels;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates and initializes new instance of StatusBarSettings.
		/// </summary>
		/// <param name="statusBar">Underling status bar.</param>
		public StatusBarSettings( StatusBarExt statusBar )
		{
			if( null == statusBar )
				throw new ArgumentNullException( "statusBar" );

			m_statusBar = statusBar;
            m_statusBar.Alignment = FlowAlignment.ChildConstraints;
            m_pnlText = CreatePanel(Syncfusion.Windows.Forms.Tools.HorzFlowAlign.Justify, 214, string.Empty);
            m_pnlFileName = CreatePanel(Syncfusion.Windows.Forms.Tools.HorzFlowAlign.Right, 100, string.Empty);
            m_pnlStatus = CreatePanel(Syncfusion.Windows.Forms.Tools.HorzFlowAlign.Right, 70, string.Empty);
			m_pnlEncoding =
                CreatePanel(Syncfusion.Windows.Forms.Tools.HorzFlowAlign.Right, 100, "Windows-1251");
            m_pnlCoords = CreatePanel(Syncfusion.Windows.Forms.Tools.HorzFlowAlign.Right, 150, string.Format(Localizer.GetString(Localizer.EditResourceIdentifiers.DEF_STATUSBAR_POSITION), 0, 0));
			m_pnlInsert =
                CreatePanel(Syncfusion.Windows.Forms.Tools.HorzFlowAlign.Right, 33, Localizer.GetString(Localizer.EditResourceIdentifiers.DEF_STATUSBAR_INSERT));

			SetGripVisibility( SizingGripVisibility.Visible );
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Event for retrieving bool indicating whether to show sizing grip with smart visibility.
		/// </summary>
		public event GetBoolEventHandler CheckSmartGripVisibility;
		/// <summary>
		/// Event is raised when status bar visiblity is changed.
		/// </summary>
		public event ValueChangedEventHandler VisibilityChanged;
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Creates new status bar panel with specified parameters and adds it to the collection.
		/// </summary>
		/// <param name="alignment">Panel alignment.</param>
		/// <param name="autoSize">Panel auto size.</param>
		/// <param name="width">Panel width.</param>
		/// <param name="minWidth">Panel minimal width.</param>
		/// <param name="text">Panel text.</param>
		/// <returns>Settings for created panel.</returns>
		private StatusBarPanelSettings CreatePanel(
            Syncfusion.Windows.Forms.Tools.HorzFlowAlign alignment, int width, string text)
		{
			StatusBarAdvPanel panel = new StatusBarAdvPanel();
			panel.Width = width;
            panel.HAlign = alignment;
			panel.Text = text;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BorderColor = SystemColors.ControlDark;
			StatusBarPanelSettings settings = new StatusBarPanelSettings( panel );
			settings.VisibilityChanged += new ValueChangedEventHandler( m_pnlVisibilityChanged );

            StatusBar.Controls.Add(panel);

			return settings;
		}
		/// <summary>
		/// Sets visibility of status bar sizing grip.
		/// </summary>
		/// <param name="visibility">Visibility of sizing grip.</param>
		private void SetGripVisibility( SizingGripVisibility visibility )
		{
			m_gripVisibility = visibility;

			switch( visibility )
			{
				case SizingGripVisibility.Visible:
                    StatusBar.SizingGrip = true;
					break;

				case SizingGripVisibility.Hidden:
                    StatusBar.SizingGrip = false;
					break;
			}
		}
		#endregion

		#region Class Evet Handlers
		/// <summary>
		/// Fills status bar if it is not expanded with forcibly springed panels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_pnlVisibilityChanged( object sender, ValueChangedEventArgs e )
		{
			StatusBarPanelSettings panelSettings = ( StatusBarPanelSettings )sender;

			bool bVisible = ( bool )e.newValue;

			// If panel became visible.
			if( bVisible )
			{
				// If status bar was empty - make single panel springed.

				// Number of visible panels.
				int nVisiblePanels = 0;
				// Index of visible panel.
				int iPanel = 0;

				for( int i = 0, len = StatusBar.Panels.Length; i < len; i++ )
				{
					if( ( ( StatusBarAdvPanel )this.Panels[ i ] ).Width != 0 )
					{
						nVisiblePanels++;
						iPanel = i;
					}

					if( nVisiblePanels > 1 ) break;
				}

				// If only 1 panel - make it springed.
				if( 1 == nVisiblePanels )
				{
					StatusBarAdvPanel panel = ( StatusBarAdvPanel )this.Panels[ iPanel ];
					m_oldForcedSpringWidth = panel.Width;
					m_springForcedIndex = iPanel;
					return;
				}

				// If no previous forced springs.
				if( -1 == m_springForcedIndex ) return;

			}
			// If panel became invisible.
			else
			{
				// If there is another springing visible panel, then everything's OK.
				// If forcibly springed panel became invisible.
				if( -1 != m_springForcedIndex )
				{
					panelSettings.Width = m_oldForcedSpringWidth;
				}

				for( int i = 0, len = StatusBar.Panels.Length; i < len; i++ )
				{
					StatusBarAdvPanel panel = ( StatusBarAdvPanel )this.Panels[i];

					// If panel visible - make it forcibly springed.
					if( 0 != panel.Width )
					{
						m_oldForcedSpringWidth = panel.Width;
						m_springForcedIndex = i;
						break;
					}
				}
			}
		}
		#endregion
	}
}