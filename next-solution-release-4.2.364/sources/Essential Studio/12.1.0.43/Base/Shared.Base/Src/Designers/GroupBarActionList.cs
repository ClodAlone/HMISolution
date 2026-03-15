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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using System.Globalization;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
	public class GroupBarActionList : SyncActionListBase<GroupBar>
	{
        DesignerActionUIService UIservice;
		public GroupBarActionList( IComponent component )
			: base( component )
		{
            UIservice = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
            this.AutoShow = true;
		}

		protected override void InitializeActionList()
		{
			this.AddDesignerActionHeaderItem( "Essential Tools - GroupBar" );
			this.AddDesignerActionPropertyItem( "Name", "Name", "Misc", "Specifies the name of the control." );
			this.AddDesignerActionPropertyItem( "GroupBarItems", "Items Collection", "Misc", "Specifies the collection of groupBarItems." );

			this.AddDesignerActionMethodItem( "AddGroup", "Add Group", "Misc", "Add a new groupBarItem." );
			this.AddDesignerActionMethodItem( "RemoveGroup", "Remove Group", "Misc", "Remove a groupBarItem." );

			this.AddDesignerActionHeaderItem( "Layout" );
			this.AddDesignerActionPropertyItem( "Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container." );

			this.AddDesignerActionHeaderItem( "Appearance" );
			this.AddDesignerActionPropertyItem( "VisualStyle", "Visual Style", "Appearance", "Specifies the stlye to be used." );
            
            if (VisualStyle == VisualStyle.Office2007)
            {
                this.AddDesignerActionPropertyItem("Office2007Theme", "Office 2007 Color Scheme", "Appearance", "Specifies the Office 2007 Color Scheme if Office 2007 Visual Style selected.");
            }

			this.AddDesignerActionPropertyItem( "TextAlign", "Text Alignment", "Appearance", "Specifies the alignment of the text." );
			this.AddDesignerActionPropertyItem( "BorderStyle", "Border Style", "Appearance", "Specifies the borderStyle for GroupBar." );            
			this.AddDesignerActionPropertyItem( "FlatLook", "Flat Look", "Appearance", "Specifies whether groupBarItems should look flat." );
			this.AddDesignerActionPropertyItem( "ThemesEnabled", "Themes Enabled", "Appearance", "Specifies whether themes should be enabled." );            

			this.AddDesignerActionHeaderItem( "Behavior" );
            this.AddDesignerActionPropertyItem("PopupResizeMode", "Popup Resize Mode", "Behavior", "Specifies the Popup Resize Mode for GroupBar.");			
            this.AddDesignerActionPropertyItem("AllowCollapse", "Allow Collapse", "Behavior", "Specifies whether collapsing should be enabled.");
            if (AllowCollapse == true)
            {
                this.AddDesignerActionPropertyItem("Collapsed", "Collapsed", "Behavior", "Specifies whether groupBar should be collapsed by default.");
            }
			this.AddDesignerActionPropertyItem( "IntegratedScrolling", "Integrated Scrolling", "Behavior", "Specifies whether integrated scrolling should be enabled." );
			this.AddDesignerActionPropertyItem( "StackedMode", "Stacked Mode", "Behavior", "Specifies whether Stacked Mode should be set." );
		}

		public Syncfusion.Windows.Forms.Tools.GroupBar.GroupBarItemCollection GroupBarItems
		{
			get
			{
				return ( Syncfusion.Windows.Forms.Tools.GroupBar.GroupBarItemCollection )GetValue( "GroupBarItems" );
			}
			set
			{
				SetValue( "GroupBarItems", value );
			}
		}

		public string Name
		{
			get
			{
				return ( string )GetValue( "Name" );
			}
			set
			{
				SetValue( "Name", value );
			}
		}

		public void AddGroup()
		{
			IDesignerHost idh = ( IDesignerHost )this.GetService( typeof( IDesignerHost ) );
			GroupBarDesigner gbd = idh.GetDesigner( this.Component ) as GroupBarDesigner;

			gbd.OnVerbAddGroup( this, EventArgs.Empty );

			UpdateRemoveGroupVerbState( idh );
		}

		public void RemoveGroup()
		{
			IDesignerHost idh = (IDesignerHost)this.GetService( typeof( IDesignerHost ) );
			GroupBarDesigner gbd = idh.GetDesigner( this.Component ) as GroupBarDesigner;

			gbd.OnVerbRemoveGroup( this, EventArgs.Empty );

			UpdateRemoveGroupVerbState( idh );
		}

		private void UpdateRemoveGroupVerbState( IDesignerHost idh )
		{
			foreach( DesignerVerb verb in idh.GetDesigner( this.Component ).Verbs )
			{
				if( verb.Text == "Remove Group" && !verb.Enabled )
				{
					verb.Enabled = this.GroupBarItems.Count > 0;
					break;
				}
			}
		}

		public VisualStyle VisualStyle
		{
			get
			{
				return ( VisualStyle )GetValue( "VisualStyle" );
			}
			set
			{
				SetValue( "VisualStyle", value );
                this.UIservice.Refresh(this.Component);
			}
		}   

        public Office2007Theme Office2007Theme
        {
            get 
            {
                return (Office2007Theme)GetValue("Office2007Theme"); 
            }
            set
            {
                SetValue("Office2007Theme", value);
            }
        }

        public PopupResizeMode PopupResizeMode
        {
            get
            {
                return (PopupResizeMode)GetValue("PopupResizeMode"); 
            }
            set
            {
                SetValue("PopupResizeMode", value);
            }
        }

        public bool AllowCollapse
        {
            get
            {
                return (bool)GetValue("AllowCollapse");
            }
            set
            {
                SetValue("AllowCollapse", value);
                this.UIservice.Refresh(this.Component);
            }
        }

        public bool Collapsed
        {
            get
            {
                return (bool)GetValue("Collapsed");
            }
            set
            {
                SetValue("Collapsed", value);
            }
        }
        

		public TextAlignment TextAlign
		{
			get
			{
				return ( TextAlignment )GetValue( "TextAlign" );
			}
			set
			{
				SetValue( "TextAlign", value );
			}
		}

		public BorderStyle BorderStyle
		{
			get
			{
				return ( BorderStyle )GetValue( "BorderStyle" );
			}
			set
			{
				SetValue( "BorderStyle", value );
			}
		}

		public bool FlatLook
		{
			get
			{
				return ( bool )GetValue( "FlatLook" );
			}
			set
			{
				SetValue( "FlatLook", value );
			}
		}

		public bool ThemesEnabled
		{
			get
			{
				return ( bool )GetValue( "ThemesEnabled" );
			}
			set
			{
				SetValue( "ThemesEnabled", value );
			}
		}
       
		public bool IntegratedScrolling
		{
			get
			{
				return ( bool )GetValue( "IntegratedScrolling" );
			}
			set
			{
				SetValue( "IntegratedScrolling", value );
			}
		}

		public bool StackedMode
		{
			get
			{
				return ( bool )GetValue( "StackedMode" );
			}
			set
			{
				SetValue( "StackedMode", value );
			}
		}

		public DockStyle Dock
		{

			get
			{
				return ( DockStyle )GetValue( "Dock" );
			}
			set
			{
				SetValue( "Dock", value );
			}

		}

	}
}
#endif