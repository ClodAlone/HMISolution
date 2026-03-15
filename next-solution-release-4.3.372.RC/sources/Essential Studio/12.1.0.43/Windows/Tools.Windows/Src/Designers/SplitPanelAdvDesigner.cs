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

#region file using directives
using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Tools;
#endregion

namespace Syncfusion.Windows.Forms.Design
{
	/// <summary>
	/// This class implements specific for nested controls design-time behaviour
	/// to split panels. 
	/// </summary>
	class SplitPanelAdvDesigner:
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		ScrollableControlDesigner
#else
		ParentControlDesigner
#endif
    {
		#region Class Constants
		private static readonly string[] DEF_PROPERTIES_TO_REMOVE = new string[]
		{
			"Modifiers",
			"Locked"
		};

		/// <summary>
		/// Offset, used for selection frame drawing.
		/// </summary>
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )		
        private const int DEF_BORDER_OFFSET =  4;
#else
		private const int DEF_BORDER_OFFSET =  3;
#endif

        /// <summary>
        /// Used to get brigtness per cent in panel's background color
        /// to determine selection frame color.
        /// </summary>
        private const float DEF_MIDDLE_BRIGHTNESS = 0.5f;
        /// <summary>
        /// used for mathematic float comparision operations.
        /// </summary>
        private const float DEF_EPS = 0.000000001f;
        #endregion

		#region Class members
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Used to deactivate context menu items only first time context
		/// menu is shown.
		/// </summary>
		private bool m_bCommandsDesibled = false;
#endif
		/// <summary>
		/// Split panel, this designer is associated with.
		/// </summary>
		private SplitPanelAdv m_panel = null;
		#endregion

		#region Class overrides
		protected override void PreFilterProperties( IDictionary properties )
		{
			base.PreFilterProperties( properties );
            
			// remove some properties from properties window
			for( int i = 0, len = DEF_PROPERTIES_TO_REMOVE.Length; i < len; i++ )
			{
				properties.Remove( DEF_PROPERTIES_TO_REMOVE[ i ] );
				properties.Remove( DEF_PROPERTIES_TO_REMOVE[ i ] );
			}

			PropertyDescriptor propDescriptor = null;

			// remove Name property from properties window
			foreach( DictionaryEntry entry in properties )
			{
				propDescriptor = entry.Value as PropertyDescriptor;

				// remove name property from properties window and disable it's serialization
				if( propDescriptor.Name.Equals( "Name" ) )
				{
					Attribute[] arrAttributes = new Attribute[]{ BrowsableAttribute.No, new EditorBrowsableAttribute( EditorBrowsableState.Never ),
																	  DesignerSerializationVisibilityAttribute.Hidden } ;

					properties[ entry.Key ] = TypeDescriptor.CreateProperty( propDescriptor.ComponentType, 
						propDescriptor, arrAttributes );
					return;
				}
			}
		}

		protected override void OnContextMenu( int x, int y )
		{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// disable such menu items as Cut, Copy, Delete, etc.
			DisableMenuCommands();
#endif

			base.OnContextMenu( x, y );
		}

		/// <summary>
		/// Gets selection rules allowed to this component at design-time.
		/// </summary>
		public override SelectionRules SelectionRules
		{
			get
			{
				return SelectionRules.Locked;
			}
		}

		/// <summary>
		/// Initialize designer.
		/// </summary>
		/// <param name="component"> Component, designer is associated with. </param>
		public override void Initialize( IComponent component )
		{
			base.Initialize( component );

			m_panel = component as SplitPanelAdv;
		}

		/// <summary>
		/// Additional drawing logic is implemented here.
		/// </summary>
		/// <param name="pe"></param>
		protected override void OnPaintAdornments( PaintEventArgs pe )
		{
			// if panel associated with this designer is selected, draw 
			// it selected.
			DrawSelectionFrame();
                        
			base.OnPaintAdornments( pe );
		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Disables such menu items as Cut, Copy, Delete, etc.
		/// </summary>
		protected virtual void DisableMenuCommands()
		{
			if( !m_bCommandsDesibled )
			{
				IMenuCommandService menuService = ( IMenuCommandService )base.GetService( typeof( IMenuCommandService ) );

				// disable such menu items as Cut, Copy, Delete, etc.
				if( menuService != null )
				{
					DisabeMenuCommand( menuService, StandardCommands.Cut );
					DisabeMenuCommand( menuService, StandardCommands.Copy );
					DisabeMenuCommand( menuService, StandardCommands.Delete );
					DisabeMenuCommand( menuService, StandardCommands.Paste );
					DisabeMenuCommand( menuService, StandardCommands.AlignToGrid );

					m_bCommandsDesibled = true;
				}
			}
		}
#endif		
		#endregion

		#region Class utility methods
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Disables specified menu item in ContextMenu at design-time.
		/// </summary>
		/// <param name="menuService"> Service, used to find menu items </param>
		/// <param name="commandID"> command to disable </param>
		protected void DisabeMenuCommand( IMenuCommandService menuService, CommandID commandID )
		{
			if( menuService == null )
				throw new ArgumentNullException( "menuService" );

			if( commandID == null )
				throw new ArgumentNullException( "commandID" );

			MenuCommand command = menuService.FindCommand( commandID );
			if( command != null )
			{
				command.Enabled = false;
			}
		}
#endif
		/// <summary>
		/// Draws selection frame around panel.
		/// </summary>
		private void DrawSelectionFrame()
		{
			if( m_panel != null )
			{
				using (Graphics g = m_panel.CreateGraphics())
				{
					// get reverse colors to panel's colors for selection drawing
					Color panelBackColor = m_panel.BackColor;
					bool isBrightBackColor = (panelBackColor.GetBrightness() - DEF_MIDDLE_BRIGHTNESS) < DEF_EPS;

					Color borderColor = (isBrightBackColor ) ? ControlPaint.Light(panelBackColor) :
						ControlPaint.Dark(panelBackColor);

					Rectangle borderRect = m_panel.ClientRectangle;

					ButtonBorderStyle borderStyle = ButtonBorderStyle.Dashed;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    borderStyle = ButtonBorderStyle.Dotted;
#endif

					// draw border
					ControlPaint.DrawBorder( g, borderRect, borderColor,
						borderStyle );

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					// draw inner border, if this panel is selected.
					if( m_panel.DrawSelected )
					{
						borderRect.Inflate( -DEF_BORDER_OFFSET, -DEF_BORDER_OFFSET );

						ControlPaint.DrawBorder( g, borderRect, borderColor,
							borderStyle );
					}
#endif
				}
			}
		}
		#endregion
    }
}
