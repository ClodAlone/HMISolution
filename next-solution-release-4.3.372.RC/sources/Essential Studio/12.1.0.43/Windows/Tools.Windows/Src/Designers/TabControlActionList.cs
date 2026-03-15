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
	public class TabControlActionList : SyncActionListBase<TabControlAdv>
	{

		public TabControlActionList (IComponent component)
			: base(component)
		{
			
		}

		public enum TabStyles
		{
			TabRenderer3D,
			TabRenderer2D,
			TabRendererWorkbookMode,
			OneNoteStyleRenderer,
			OneNoteStyleFlatTabsRenderer,
			TabRendererOffice2003,
			TabRendererWhidbey,
            TabRendererDockingWhidbey,
			TabRendererDockingWhidbeyBeta,
			TabRendererIE7,
			TabRendererOffice2007,
            TabRendererVS2008,
            TabRendererBlendDark,
            TabRendererBlendLight,
            TabRendererVS2010,
            TabRendererMetro,
		}

		protected override void InitializeActionList ( )
		{
			this.AddDesignerActionHeaderItem("Essential Tools - TabControlAdv");
			
			// Misc
			this.AddDesignerActionPropertyItem("Name", "Name", "Misc", "Specifies the name of the control");
			this.AddDesignerActionPropertyItem("Alignment", "Alignment", "Misc", "Specifies the alignment of tabs.");
			this.AddDesignerActionPropertyItem("ImageList", "ImageList", "Misc", "Specifies the imageList to be associated.");
			this.AddDesignerActionPropertyItem("TabPages", "TabPagesCollection", "Misc", "Indicates the collection of tabPages");
			this.AddDesignerActionMethodItem("AddTab", "Add Tab", "Misc", "Adds a new tabPage.");
			this.AddDesignerActionMethodItem("RemoveTab", "Remove Tab", "Misc", "Removes selected tabPage.");

			// Style
			this.AddDesignerActionHeaderItem("Style");
			this.AddDesignerActionPropertyItem("TabStyle", "TabStyle", "Style", "Specifies the style for the tabs.");

			// Color
			this.AddDesignerActionHeaderItem("Color");
			this.AddDesignerActionPropertyItem("TabPanelBackColor", "TabPanelBackColor", "Color", "Specifies the BackColor for the tabPanel.");
			this.AddDesignerActionPropertyItem("ActiveTabColor", "ActiveTabColor", "Color", "Specifies the ActiveTabColor for the tabs.");
			this.AddDesignerActionPropertyItem("InactiveTabColor", "InactiveTabColor", "Color", "Specifies the InactiveTabColor for the tabs.");

			// Appearance
			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("ImageAlignmentR", "ImageAlignment", "Appearance", "Specifies alignment of image with respect to tab text.");
			this.AddDesignerActionPropertyItem("ThemesEnabled", "Themes Enabled", "Appearance", "Specifies whether themes should be enabled.");
			this.AddDesignerActionPropertyItem("RotateTextWhenVertical", "RotateTextWhenVertical", "Appearance", "Specifies whether to rotate the tabs.");
			this.AddDesignerActionPropertyItem("ShowScroll", "Show Scroll", "Appearance", "Specifies whether the scroll buttons should be visible.");
			this.AddDesignerActionPropertyItem("HotTrack", "HotTrack", "Appearance", "Specifies whether hotTrack should be enabled.");
			
			// Behavior
   			this.AddDesignerActionHeaderItem("Behavior");
			this.AddDesignerActionPropertyItem("LabelEdit", "Label Edit", "Behavior", "Specifies whether Label Edit should be enabled.");

			// Layout
			this.AddDesignerActionHeaderItem("Layout");
			this.AddDesignerActionPropertyItem("Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container.");

		}

		public string Name
		{
			get
			{
				string name = string.Empty;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					name = Control.Name;
				}
				return name;
			}
			set
			{
				SetValue("Name", value);
			}
		}

		public Syncfusion.Windows.Forms.Tools.TabPageAdvCollection TabPages
		{
			get
			{
				Syncfusion.Windows.Forms.Tools.TabPageAdvCollection tpCollection = null;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					tpCollection = Control.TabPages;
				}
				return tpCollection;
			}
			set
			{
				SetValue("TabPages", value);
			}
		}
		

		public TabStyles TabStyle
		{
			get
			{
				TabStyles style = TabStyles.TabRenderer3D;
				Type styles = typeof(TabStyles);
				if (this.Control != null)
				{
					TabControlAdv bControl = this.Control as TabControlAdv;
					string s = bControl.TabStyle.ToString();
					s = s.Replace("Syncfusion.Windows.Forms.Tools.","");
					style = (TabStyles)Enum.Parse(typeof(TabStyles), s) ;
				}
				return style;
			}
			set
			{
                if (value == TabStyles.TabRenderer3D)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRenderer3D));
                else if (value == TabStyles.TabRenderer2D)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRenderer2D));
                else if (value == TabStyles.TabRendererWorkbookMode)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererWorkbookMode));
                else if (value == TabStyles.OneNoteStyleRenderer)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.OneNoteStyleRenderer));
                else if (value == TabStyles.OneNoteStyleFlatTabsRenderer)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.OneNoteStyleFlatTabsRenderer));
                else if (value == TabStyles.TabRendererOffice2003)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2003));
                else if (value == TabStyles.TabRendererWhidbey)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererWhidbey));
                else if (value == TabStyles.TabRendererDockingWhidbeyBeta)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererDockingWhidbeyBeta));
                else if (value == TabStyles.TabRendererDockingWhidbey)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererDockingWhidbey));
                else if (value == TabStyles.TabRendererIE7)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererIE7));          
				else if (value == TabStyles.TabRendererOffice2007)
					SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2007));
                else if (value == TabStyles.TabRendererVS2008)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererVS2008));
                else if( value == TabStyles.TabRendererBlendDark)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererBlendDark));
                else if (value == TabStyles.TabRendererBlendLight)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererBlendLight));
                else if (value == TabStyles.TabRendererVS2010)
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererVS2010));
                else if (value == TabStyles.TabRendererMetro  )
                    SetValue("TabStyle", typeof(Syncfusion.Windows.Forms.Tools.TabRendererMetro));
            }
		}

		public void AddTab()
		{
			if (this.Control != null)
			{
				TabControlAdv control = this.Control as TabControlAdv;
				TabControlAdv tabControl;
				System.ComponentModel.MemberDescriptor memberDescriptor;
				System.ComponentModel.Design.IDesignerHost iDesignerHost;
				System.ComponentModel.Design.DesignerTransaction designerTransaction;
				System.ComponentModel.Design.CheckoutException checkoutException;
				TabPageAdv tabPage;
				string tabName;
				System.ComponentModel.PropertyDescriptor propertyDescriptor;

				tabControl = (TabControlAdv)this.Component;
				memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)@"Controls"];
				iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
				if (iDesignerHost != null)
				{
					designerTransaction = null;
					// Raise the RaiseComponentChanging and RaiseComponentChanged events
					// Also call CreateComponent and parent the tabpage to the tabcontrol
					try
					{
						try
						{
							designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"Add tab to ", this.Component.Site.Name));
						//	this.RaiseComponentChanging(memberDescriptor);
						}
						catch (System.ComponentModel.Design.CheckoutException exception)
						{
							checkoutException = exception;
							if (checkoutException == CheckoutException.Canceled)
								throw checkoutException;
						}
						tabPage = (TabPageAdv)iDesignerHost.CreateComponent(this.GetChildType());
						tabName = null;
						propertyDescriptor = TypeDescriptor.GetProperties((object)tabPage)[(string)@"Name"];
						if (propertyDescriptor != null)
						{
							if (propertyDescriptor.PropertyType == typeof(System.String))
								tabName = (string)(System.String)propertyDescriptor.GetValue((object)tabPage);
						}
						if ((tabName != null))
							tabPage.Text = tabName;

						tabControl.Controls.Add((Control)tabPage);
                        this.RaiseComponentChanged( memberDescriptor, null, null );
					}
					finally
					{
						if (designerTransaction != null)
							designerTransaction.Commit();
					}
				}
			}
		}

        /// <summary>
        /// Notifies the IComponentChangeService that this component has been changed.
        /// </summary>
        /// <param name="member">A MemberDescriptor that indicates the member that has been changed.</param>
        /// <param name="oldValue">The old value of the member.</param>
        /// <param name="newValue">The new value of the member. </param>
        protected void RaiseComponentChanged( MemberDescriptor member, object oldValue, object newValue )
        {
            IComponentChangeService service = ( IComponentChangeService ) this.GetService( typeof( IComponentChangeService ) );
            if( service != null )
            {
                service.OnComponentChanged( this.Component, member, oldValue, newValue );
            }
        }

		private Type GetChildType ( )
		{
			Type childType = typeof(TabPageAdv);
			object[] atts = this.Component.GetType().GetCustomAttributes(typeof(DefaultChildTypeAttribute), true);
			if (atts.Length > 0)
			{
				DefaultChildTypeAttribute attribute = atts[0] as DefaultChildTypeAttribute;

				if (!typeof(TabPageAdv).IsAssignableFrom(attribute.ChildType))
					MessageBox.Show("Specified custom type " + attribute.ChildType.ToString() + " is not derived from TabPageAdv. Creating a TabPageAdv instance.", "TabControlAdv designer warning:");
				else
					childType = attribute.ChildType;
			}
			return childType;
		}

		public void RemoveTab ( )
		{
			TabControlAdv tabControl;
			System.ComponentModel.MemberDescriptor memberDescriptor;
			TabPageAdv tabPage;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;
			System.ComponentModel.Design.DesignerTransaction designerTransaction;
			System.ComponentModel.Design.CheckoutException checkoutException;
			tabControl = (TabControlAdv)this.Component;
			if (tabControl == null || tabControl.TabPages.Count == 0)
				return;

			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)@"Controls"];
			tabPage = tabControl.SelectedTab;
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			if (iDesignerHost != null)
			{
				designerTransaction = null;
				try
				{
					try
					{
						designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"Remove ", tabPage.Site.Name, (string)@" from ", this.Component.Site.Name));
					//	this.RaiseComponentChanging(memberDescriptor);
					}
					catch (System.ComponentModel.Design.CheckoutException exception)
					{
						checkoutException = exception;
						if (checkoutException != CheckoutException.Canceled)
							throw checkoutException;
					}
					iDesignerHost.DestroyComponent((IComponent)tabPage);
					this.RaiseComponentChanged( memberDescriptor, null, null );
				}
				finally
				{
					if (designerTransaction != null)
						designerTransaction.Commit();
				}
			}
		}

		public TabAlignment Alignment
		{
			get
			{
				TabAlignment alignment = TabAlignment.Top;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					alignment = control.Alignment;
				}
				return alignment;
			}
			set
			{
				SetValue("Alignment", value);
			}
		}

		public ImageList ImageList
		{
			get
			{
				ImageList imageList = null;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					imageList = control.ImageList;
				}
				return imageList;
			}
			set
			{
				SetValue("ImageList", value);
			}
		}

		public bool ThemesEnabled
		{
			get
			{
				bool themesEnabled = false;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					themesEnabled = control.ThemesEnabled;
				}
				return themesEnabled;
			}
			set
			{
				SetValue("ThemesEnabled", value);
			}
		}

		public bool LabelEdit
		{
			get
			{
				bool labelEdit = false;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					labelEdit = control.LabelEdit;
				}
				return labelEdit;
			}
			set
			{
				SetValue("LabelEdit", value);
			}
		}

		
		public bool ShowScroll
		{
			get
			{
				bool showScroll = false;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					showScroll = control.ShowScroll;
				}
				return showScroll;
			}
			set
			{
				SetValue("ShowScroll", value);
			}
		}

		public bool HotTrack
		{
			get
			{
				bool hotTrack = false;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					hotTrack = control.HotTrack;
				}
				return hotTrack;
			}
			set
			{
				SetValue("HotTrack", value);
			}
		}

		public Color TabPanelBackColor
		{
			get
			{
				Color panelColor = Color.Empty;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					panelColor = control.TabPanelBackColor;
				}
				return panelColor;
			}
			set
			{
				SetValue("TabPanelBackColor", value);
			}
		}

		public Color ActiveTabColor
		{
			get
			{
				Color activeTabColor = Color.Empty;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					activeTabColor = control.ActiveTabColor;
				}
				return activeTabColor;
			}
			set
			{
				SetValue("ActiveTabColor", value);
			}
		}

		public Color InactiveTabColor
		{
			get
			{
				Color inactiveTabColor = Color.Empty;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					inactiveTabColor = control.InactiveTabColor;
				}
				return inactiveTabColor;
			}
			set
			{
				SetValue("InactiveTabColor", value);
			}
		}

		public RelativeImageAlignment ImageAlignmentR
		{
			get
			{
				RelativeImageAlignment imageAlignment = RelativeImageAlignment.LeftOfText;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					imageAlignment = control.ImageAlignmentR;
				}
				return imageAlignment;
			}
			set
			{
				SetValue("ImageAlignmentR", value);
			}
		}

		public bool RotateTextWhenVertical
		{
			get
			{
				bool rotateText = false;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					rotateText = control.RotateTextWhenVertical;
				}
				return rotateText;
			}
			set
			{
				SetValue("RotateTextWhenVertical", value);
			}
		}

		public DockStyle Dock
		{

			get
			{
				DockStyle dock = DockStyle.None;
				if (this.Control != null)
				{
					TabControlAdv control = this.Control as TabControlAdv;
					dock = control.Dock;
				}
				return dock;
			}
			set
			{
				SetValue("Dock", value);
			}

		}
	}

}
#endif