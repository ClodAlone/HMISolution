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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Specifies the UserControl that will be used in the XP Menus Customization dialog to allow the end user
	/// to customize the application's menu structure.
	/// </summary>
	/// <remarks>
	/// Derive from this UserControl and customize by adding more controls, if necessary, during design-time.
	/// Then use the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.CustomizationPanel"/> property
	/// to let the framework use your custom control.
	/// </remarks>
	[
	Syncfusion.Documentation.DocumentationExclude(),
	ToolboxItem(false)
	]
	public class CustomizationPanel : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private BarManager localBarManager, customizingBarManager;
		private BarCustomizationDialog custDlg;
		private IDesignerHost designerHost;
		private System.ComponentModel.IContainer components;
		private MenuGridControlCustomizable menuGridBase;
		private PopupMenu barItemModifyMenu, catModifyMenu;
		protected System.Windows.Forms.TabControl tabControl;
		protected System.Windows.Forms.TabPage tabPage1;
		protected System.Windows.Forms.TabPage tabPage2;
		protected System.Windows.Forms.TabPage tabPage3;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.Button newButton;
		protected System.Windows.Forms.Button deleteButton;
		protected System.Windows.Forms.Button resetButton;
		protected internal System.Windows.Forms.Button closeButton;
		protected System.Windows.Forms.Label label2;
		protected System.Windows.Forms.Label label3;
		protected System.Windows.Forms.Button cmdModify;
		protected System.Windows.Forms.CheckedListBox toolbarList;
		protected System.Windows.Forms.TreeView catView;
		protected System.Windows.Forms.Button catMoveDown;
		protected System.Windows.Forms.Button catMoveUp;
		protected System.Windows.Forms.Panel gridHostPanel;
		protected System.Windows.Forms.CheckBox alwaysFullMenus;
		protected System.Windows.Forms.Label label4;
		protected System.Windows.Forms.CheckBox expandAfterDelay;
		protected System.Windows.Forms.Label label5;
		protected System.Windows.Forms.CheckBox largeIcons;
		protected ToolTip catViewTooltip;
		protected System.Windows.Forms.Button resetPartialMenusBtn;
		protected System.Windows.Forms.Label label6;
		protected System.Windows.Forms.Label label7;
		protected System.Windows.Forms.Button resetCustomizationButton;
		private System.Windows.Forms.ImageList imageList1;
		protected System.Windows.Forms.HScrollBar hScrollBar1;
		protected System.Windows.Forms.Panel panel1;
		protected System.Windows.Forms.Splitter splitter1;

		#region Constants
		private const int c_nTEXT_COLUMN_WIDTH = 84;
		private const int c_nTEXT_WITH_ICON_WIDTH = 86;
		private const int c_nBORDERS = 5;
		#endregion Constants

		private string GetLocalizedString(string Key,System.Resources.ResourceManager resources,string name)
		{
			string result = SR.GetString(Key, this);

			if(result == string.Empty)
				return resources.GetString(name);

			return result;
		}
		
		// Custom members
		private ArrayList barsList;

		/// <summary>
		/// Creates a new instance of the CustomizationPanel class.
		/// </summary>
		public CustomizationPanel()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				new Syncfusion.Core.Licensing.LicensedComponent(typeof(CustomizationPanel));
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            this.label5.AutoSize = true; 
			this.barsList = new ArrayList();

			this.alwaysFullMenus.Click += new EventHandler(AlwaysFullMenus_Clicked);
			this.largeIcons.Click += new EventHandler(LargeIcons_Clicked);
			this.expandAfterDelay.Click += new EventHandler(ExpandAfterDelay_Clicked);
			
			this.catView.Scrollable = true;
            this.catView.BringToFront();
			this.catMoveDown.Hide();
			this.catMoveUp.Hide();
			this.cmdModify.Hide();
		}

		/// <summary>
		/// Initializes the CustomizationPanel object.
		/// </summary>
		/// <param name="dlg">The dialog where this control is hosted.</param>
		/// <param name="customizingManager">The BarManager that this control is customizing.</param>
		/// <remarks>
		/// This method will be called to initialize the CustomizationPanel
		/// and pass its references to the above objects.
		/// <para>Inheritors could override this function and perform custom
		/// initialization at this time. When you override this function make
		/// sure to call the base class for proper initialization.</para>
		/// </remarks>
		public virtual void InitCustomizationPanel(BarCustomizationDialog dlg, 
			BarManager customizingManager)
		{
			this.custDlg = dlg;
			this.customizingBarManager = customizingManager;

			this.customizingBarManager.PropertyChanged += new SyncfusionPropertyChangedEventHandler( customizingBarManager_PropertyChanged );

			this.customizingBarManager.Bars.ItemPropertyChanged
				+= new SyncfusionPropertyChangedEventHandler( this.BarPropertyChanged );

			if( this.customizingBarManager.DesignMode )
			{
				this.catView.CheckBoxes = true;
				this.catViewTooltip	= new ToolTip();
				this.catViewTooltip.SetToolTip(this.catView, "Checked nodes indicate that the Category will appear in the User Customization dialog.");
			}
			else
			{
				this.catView.CheckBoxes = false;
			}

			InitPopups();
		}

		/// <summary>
		/// The IDesignerHost interface in which the BarManager is hosted
		/// during design time.
		/// </summary>
		/// <value>A reference to an object implementing the IDesignerHost interface.</value>
		/// <remarks>
		/// This will be called only when the BarManager is customized
		/// in design mode.
		/// </remarks>
		public virtual IDesignerHost DesignerHost
		{
			set
			{
				if(this.designerHost != value)
				{
					if(this.designerHost != null)
						this.SetupSelectionListenerHandlers(false);

					this.designerHost = value;
					
					if(this.designerHost != null)
						this.SetupSelectionListenerHandlers(true);

					if(this.designerHost == null)
					{
						this.catMoveDown.Hide();
						this.catMoveUp.Hide();
						this.cmdModify.Hide();
					}
					else
					{
						this.catMoveDown.Show();
						this.catMoveUp.Show();
						this.cmdModify.Show();
					}
				}
			}
		}

		/// <summary>
		/// Called when the dialog in which this control is hosted is closing.
		/// </summary>
		/// <remarks>
		/// Inheritors can override this function to perform custom clean up.
		/// Make sure to call the base class when you override this function
		/// for proper clean up.
		/// </remarks>
		public virtual void OnDialogClosing()
		{
			if(this.menuGridBase.ParentItem != null)
				this.menuGridBase.ParentItem.Items.CollectionChanged
					-= new CollectionChangeEventHandler(this.BarItemsCollectionChanged);
		}

		private void ItemPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			if(e.PropertyName == "CategoryIndex")
			{
				int selIndex = this.menuGridBase.SelectedIndex;
				this.menuGridBase.SelectedIndex = -1;
				this.RefreshBarItemsList();
				if(selIndex < this.menuGridBase.ParentItem.Items.Count)
				{
					this.menuGridBase.SelectedIndex = selIndex;
					this.customizingBarManager.CustomizingItem = this.menuGridBase.SelectedItem;
				}
			}
		}

		private void InitPopups()
		{
			this.menuGridBase = new MenuGridControlCustomizable(this.customizingBarManager.DesignMode);

			this.menuGridBase.DragDrop += new DragEventHandler( OnMenuGridDragDrop );
			this.menuGridBase.fixedWidth = true;
			this.menuGridBase.TextColumnWidth += 
				(this.menuGridBase.IconColumnWidth /*+ this.menuGridBase.ShortcutColumnWidth*/);
			//this.menuGridBase.ShortcutColumnWidth = 0;
			//			this.menuGridBase.allowedDragEffects |= DragDropEffects.Move;
			this.menuGridBase.dndHelper.AllowRecord = false;
			this.menuGridBase.Parent = this.gridHostPanel;
			this.menuGridBase.Dock = DockStyle.Top;
			this.gridHostPanel.BackColor = this.menuGridBase.BackColor;

			this.menuGridBase.MouseUp += new MouseEventHandler(this.MenuGrid_MouseUp);
			//			this.menuGridBase.AllowDrop = false;
			//			this.menuGridBase.AutoScrolling = ScrollBars.Vertical;

			this.catView.MouseUp += new MouseEventHandler(this.CatView_MouseUp);
			this.catView.MouseDown += new MouseEventHandler(this.CatView_MouseDown);
			this.catView.AfterCheck 
				+= new TreeViewEventHandler(this.CatView_AfterCheck);

			// Popup menus for Category Modify and BarItem Modify
			localBarManager = new BarManager();
			localBarManager.ImageList = this.imageList1;

			catModifyMenu = new PopupMenu();
			barItemModifyMenu = new PopupMenu();
			catModifyMenu.ParentBarItem = new ParentBarItem();
			barItemModifyMenu.ParentBarItem = new ParentBarItem();

			catModifyMenu.ParentBarItem.Manager = this.localBarManager;
			barItemModifyMenu.ParentBarItem.Manager = this.localBarManager;
			this.localBarManager.SetUseHooksForMenus(true);

			catModifyMenu.ParentBarItem.Popup += new EventHandler(this.CatModifyMenu_Popup);
			barItemModifyMenu.ParentBarItem.Popup += new EventHandler(this.BarItemModifyMenu_Popup);

			// Cat Menu
			catModifyMenu.ParentBarItem.Items.Add(new BarItem(SR.GetString(SR.Add,this .customizingBarManager ), new EventHandler(OnCatAdd)));
			catModifyMenu.ParentBarItem.Items[0].ImageIndex = 3;
			this.localBarManager.Items.Add(catModifyMenu.ParentBarItem.Items[0]);

			catModifyMenu.ParentBarItem.Items.Add(new BarItem(SR.GetString(SR.Delete, this.customizingBarManager), new EventHandler(OnCatDelete)));
			catModifyMenu.ParentBarItem.Items[1].ImageIndex = 2;
			this.localBarManager.Items.Add(catModifyMenu.ParentBarItem.Items[1]);

			//			catModifyMenu.ParentBarItem.AddItem(new BarItem("Insert...", new EventHandler(OnCatInsert)));
			catModifyMenu.ParentBarItem.Items.Add(new BarItem(SR.GetString(SR.Rename, this.customizingBarManager), new EventHandler(OnCatRename)));
			this.localBarManager.Items.Add(catModifyMenu.ParentBarItem.Items[2]);


			barItemModifyMenu.ParentBarItem.Items.Add(new BarItem(SR.GetString(SR.Add,this .customizingBarManager ), new EventHandler(OnItemAdd)));
			barItemModifyMenu.ParentBarItem.Items[0].ImageIndex = 3;
			this.localBarManager.Items.Add(barItemModifyMenu.ParentBarItem.Items[0]);

			barItemModifyMenu.ParentBarItem.Items.Add(new BarItem(SR.GetString(SR.Delete, this.customizingBarManager), new EventHandler(OnItemDelete)));
			barItemModifyMenu.ParentBarItem.Items[1].ImageIndex = 2;
			this.localBarManager.Items.Add(barItemModifyMenu.ParentBarItem.Items[1]);
			barItemModifyMenu.ParentBarItem.Items.Add(new BarItem(SR.GetString(SR.DeleteAll, this.customizingBarManager), new EventHandler(OnDeleteItems)));
			this.localBarManager.Items.Add(barItemModifyMenu.ParentBarItem.Items[2]);
		}

		/// <summary> 
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}

				if( this.customizingBarManager != null )
				{
					this.customizingBarManager.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( customizingBarManager_PropertyChanged );

					this.customizingBarManager.Items.ItemPropertyChanged
						-= new SyncfusionPropertyChangedEventHandler
						( this.ItemPropertyChanged );
					this.customizingBarManager.Bars.ItemPropertyChanged
						-= new SyncfusionPropertyChangedEventHandler( this.BarPropertyChanged );

					this.customizingBarManager = null;
				}

                if (catModifyMenu != null)
                {
                    catModifyMenu.ParentBarItem.Items.Clear();
                    catModifyMenu.ParentBarItem = null;
                    catModifyMenu.Dispose();
                    catModifyMenu = null;
                }

                if (barItemModifyMenu != null)
                {
                    barItemModifyMenu.ParentBarItem.Items.Clear();
                    barItemModifyMenu.ParentBarItem = null;
                    barItemModifyMenu.Dispose();
                    barItemModifyMenu = null;
                }

                if (localBarManager != null)
                {
                    localBarManager.Items.Clear();
                    localBarManager.Dispose();
                    localBarManager = null;
                }

                if (menuGridBase != null)
                {
                    menuGridBase.Dispose();
                    menuGridBase = null;
                }

				if( this.designerHost != null )
				{
					this.DesignerHost = null;
				}
			}

			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CustomizationPanel));
			this.toolbarList = new System.Windows.Forms.CheckedListBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.resetButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.newButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.catView = new System.Windows.Forms.TreeView();
			this.gridHostPanel = new System.Windows.Forms.Panel();
			this.catMoveUp = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.catMoveDown = new System.Windows.Forms.Button();
			this.cmdModify = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.resetCustomizationButton = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.resetPartialMenusBtn = new System.Windows.Forms.Button();
			this.largeIcons = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.expandAfterDelay = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.alwaysFullMenus = new System.Windows.Forms.CheckBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolbarList
			// 
			this.toolbarList.AccessibleDescription = resources.GetString("toolbarList.AccessibleDescription");
			this.toolbarList.AccessibleName = resources.GetString("toolbarList.AccessibleName");
			this.toolbarList.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("toolbarList.Anchor")));
			this.toolbarList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolbarList.BackgroundImage")));
			this.toolbarList.ColumnWidth = ((int)(resources.GetObject("toolbarList.ColumnWidth")));
			this.toolbarList.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("toolbarList.Dock")));
			this.toolbarList.Enabled = ((bool)(resources.GetObject("toolbarList.Enabled")));
			this.toolbarList.Font = ((System.Drawing.Font)(resources.GetObject("toolbarList.Font")));
			this.toolbarList.HorizontalExtent = ((int)(resources.GetObject("toolbarList.HorizontalExtent")));
			this.toolbarList.HorizontalScrollbar = ((bool)(resources.GetObject("toolbarList.HorizontalScrollbar")));
			this.toolbarList.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("toolbarList.ImeMode")));
			this.toolbarList.IntegralHeight = ((bool)(resources.GetObject("toolbarList.IntegralHeight")));
			this.toolbarList.Location = ((System.Drawing.Point)(resources.GetObject("toolbarList.Location")));
			this.toolbarList.Name = "toolbarList";
			this.toolbarList.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("toolbarList.RightToLeft")));
			this.toolbarList.ScrollAlwaysVisible = ((bool)(resources.GetObject("toolbarList.ScrollAlwaysVisible")));
			this.toolbarList.Size = ((System.Drawing.Size)(resources.GetObject("toolbarList.Size")));
			this.toolbarList.TabIndex = ((int)(resources.GetObject("toolbarList.TabIndex")));
			this.toolbarList.ThreeDCheckBoxes = true;
			this.toolbarList.Visible = ((bool)(resources.GetObject("toolbarList.Visible")));
			this.toolbarList.SelectedIndexChanged += new System.EventHandler(this.toolbarList_SelectedIndexChanged);
			this.toolbarList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.toolbarList_ItemCheck);
			// 
			// tabControl
			// 
			this.tabControl.AccessibleDescription = resources.GetString("tabControl.AccessibleDescription");
			this.tabControl.AccessibleName = resources.GetString("tabControl.AccessibleName");
			this.tabControl.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tabControl.Alignment")));
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabControl.Anchor")));
			this.tabControl.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tabControl.Appearance")));
			this.tabControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabControl.BackgroundImage")));
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Controls.Add(this.tabPage3);
			this.tabControl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabControl.Dock")));
			this.tabControl.Enabled = ((bool)(resources.GetObject("tabControl.Enabled")));
			this.tabControl.Font = ((System.Drawing.Font)(resources.GetObject("tabControl.Font")));
			this.tabControl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabControl.ImeMode")));
			this.tabControl.ItemSize = ((System.Drawing.Size)(resources.GetObject("tabControl.ItemSize")));
			this.tabControl.Location = ((System.Drawing.Point)(resources.GetObject("tabControl.Location")));
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = ((System.Drawing.Point)(resources.GetObject("tabControl.Padding")));
			this.tabControl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabControl.RightToLeft")));
			this.tabControl.SelectedIndex = 0;
			this.tabControl.ShowToolTips = ((bool)(resources.GetObject("tabControl.ShowToolTips")));
			this.tabControl.Size = ((System.Drawing.Size)(resources.GetObject("tabControl.Size")));
			this.tabControl.TabIndex = ((int)(resources.GetObject("tabControl.TabIndex")));
            this.tabControl.Text = SR.GetString(SR.BarCustomizationDialogClose);
			this.tabControl.Visible = ((bool)(resources.GetObject("tabControl.Visible")));
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.AccessibleDescription = resources.GetString("tabPage1.AccessibleDescription");
			this.tabPage1.AccessibleName = resources.GetString("tabPage1.AccessibleName");
			this.tabPage1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPage1.Anchor")));
			this.tabPage1.AutoScroll = ((bool)(resources.GetObject("tabPage1.AutoScroll")));
			this.tabPage1.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPage1.AutoScrollMargin")));
			this.tabPage1.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPage1.AutoScrollMinSize")));
			this.tabPage1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage1.BackgroundImage")));
			this.tabPage1.Controls.Add(this.resetButton);
			this.tabPage1.Controls.Add(this.deleteButton);
			this.tabPage1.Controls.Add(this.newButton);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.toolbarList);
			this.tabPage1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPage1.Dock")));
			this.tabPage1.Enabled = ((bool)(resources.GetObject("tabPage1.Enabled")));
			this.tabPage1.Font = ((System.Drawing.Font)(resources.GetObject("tabPage1.Font")));
			this.tabPage1.ImageIndex = ((int)(resources.GetObject("tabPage1.ImageIndex")));
			this.tabPage1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPage1.ImeMode")));
			this.tabPage1.Location = ((System.Drawing.Point)(resources.GetObject("tabPage1.Location")));
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPage1.RightToLeft")));
			this.tabPage1.Size = ((System.Drawing.Size)(resources.GetObject("tabPage1.Size")));
			this.tabPage1.TabIndex = ((int)(resources.GetObject("tabPage1.TabIndex")));
            this.tabPage1.Text = SR.GetString(SR.BarCustomizationDialogTabToolbars,this);
			this.tabPage1.ToolTipText = resources.GetString("tabPage1.ToolTipText");
			this.tabPage1.Visible = ((bool)(resources.GetObject("tabPage1.Visible")));
			// 
			// resetButton
			// 
			this.resetButton.AccessibleDescription = resources.GetString("resetButton.AccessibleDescription");
			this.resetButton.AccessibleName = resources.GetString("resetButton.AccessibleName");
			this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("resetButton.Anchor")));
			this.resetButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("resetButton.BackgroundImage")));
			this.resetButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("resetButton.Dock")));
			this.resetButton.Enabled = ((bool)(resources.GetObject("resetButton.Enabled")));
			this.resetButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("resetButton.FlatStyle")));
			this.resetButton.Font = ((System.Drawing.Font)(resources.GetObject("resetButton.Font")));
			this.resetButton.Image = ((System.Drawing.Image)(resources.GetObject("resetButton.Image")));
			this.resetButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("resetButton.ImageAlign")));
			this.resetButton.ImageIndex = ((int)(resources.GetObject("resetButton.ImageIndex")));
			this.resetButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resetButton.ImeMode")));
			this.resetButton.Location = ((System.Drawing.Point)(resources.GetObject("resetButton.Location")));
			this.resetButton.Name = "resetButton";
			this.resetButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("resetButton.RightToLeft")));
			this.resetButton.Size = ((System.Drawing.Size)(resources.GetObject("resetButton.Size")));
			this.resetButton.TabIndex = ((int)(resources.GetObject("resetButton.TabIndex")));
			this.resetButton.Text = GetLocalizedString(SR.BarCustomizationDialogButtonReset,resources,"resetButton.Text");
			this.resetButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("resetButton.TextAlign")));
			this.resetButton.Visible = ((bool)(resources.GetObject("resetButton.Visible")));
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.AccessibleDescription = resources.GetString("deleteButton.AccessibleDescription");
			this.deleteButton.AccessibleName = resources.GetString("deleteButton.AccessibleName");
			this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("deleteButton.Anchor")));
			this.deleteButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("deleteButton.BackgroundImage")));
			this.deleteButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("deleteButton.Dock")));
			this.deleteButton.Enabled = ((bool)(resources.GetObject("deleteButton.Enabled")));
			this.deleteButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("deleteButton.FlatStyle")));
			this.deleteButton.Font = ((System.Drawing.Font)(resources.GetObject("deleteButton.Font")));
			this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
			this.deleteButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("deleteButton.ImageAlign")));
			this.deleteButton.ImageIndex = ((int)(resources.GetObject("deleteButton.ImageIndex")));
			this.deleteButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("deleteButton.ImeMode")));
			this.deleteButton.Location = ((System.Drawing.Point)(resources.GetObject("deleteButton.Location")));
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("deleteButton.RightToLeft")));
			this.deleteButton.Size = ((System.Drawing.Size)(resources.GetObject("deleteButton.Size")));
			this.deleteButton.TabIndex = ((int)(resources.GetObject("deleteButton.TabIndex")));
			this.deleteButton.Text = GetLocalizedString(SR.BarCustomizationDialogDelete,resources,"deleteButton.Text");
			this.deleteButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("deleteButton.TextAlign")));
			this.deleteButton.Visible = ((bool)(resources.GetObject("deleteButton.Visible")));
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// newButton
			// 
			this.newButton.AccessibleDescription = resources.GetString("newButton.AccessibleDescription");
			this.newButton.AccessibleName = resources.GetString("newButton.AccessibleName");
			this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("newButton.Anchor")));
			this.newButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("newButton.BackgroundImage")));
			this.newButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("newButton.Dock")));
			this.newButton.Enabled = ((bool)(resources.GetObject("newButton.Enabled")));
			this.newButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("newButton.FlatStyle")));
			this.newButton.Font = ((System.Drawing.Font)(resources.GetObject("newButton.Font")));
			this.newButton.Image = ((System.Drawing.Image)(resources.GetObject("newButton.Image")));
			this.newButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("newButton.ImageAlign")));
			this.newButton.ImageIndex = ((int)(resources.GetObject("newButton.ImageIndex")));
			this.newButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("newButton.ImeMode")));
			this.newButton.Location = ((System.Drawing.Point)(resources.GetObject("newButton.Location")));
			this.newButton.Name = "newButton";
			this.newButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("newButton.RightToLeft")));
			this.newButton.Size = ((System.Drawing.Size)(resources.GetObject("newButton.Size")));
			this.newButton.TabIndex = ((int)(resources.GetObject("newButton.TabIndex")));
			this.newButton.Text =GetLocalizedString(SR.BarCustomizationDialogNew,resources,"newButton.Text");
			this.newButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("newButton.TextAlign")));
			this.newButton.Visible = ((bool)(resources.GetObject("newButton.Visible")));
			this.newButton.Click += new System.EventHandler(this.newButton_Click);
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = GetLocalizedString(SR.BarCustomizationDialogToolbars,resources,"label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// tabPage2
			// 
			this.tabPage2.AccessibleDescription = resources.GetString("tabPage2.AccessibleDescription");
			this.tabPage2.AccessibleName = resources.GetString("tabPage2.AccessibleName");
			this.tabPage2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPage2.Anchor")));
			this.tabPage2.AutoScroll = ((bool)(resources.GetObject("tabPage2.AutoScroll")));
			this.tabPage2.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPage2.AutoScrollMargin")));
			this.tabPage2.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPage2.AutoScrollMinSize")));
			this.tabPage2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage2.BackgroundImage")));
			this.tabPage2.Controls.Add(this.panel1);
			this.tabPage2.Controls.Add(this.catMoveUp);
			this.tabPage2.Controls.Add(this.catMoveDown);
			this.tabPage2.Controls.Add(this.cmdModify);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPage2.Dock")));
			this.tabPage2.Enabled = ((bool)(resources.GetObject("tabPage2.Enabled")));
			this.tabPage2.Font = ((System.Drawing.Font)(resources.GetObject("tabPage2.Font")));
			this.tabPage2.ImageIndex = ((int)(resources.GetObject("tabPage2.ImageIndex")));
			this.tabPage2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPage2.ImeMode")));
			this.tabPage2.Location = ((System.Drawing.Point)(resources.GetObject("tabPage2.Location")));
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPage2.RightToLeft")));
			this.tabPage2.Size = ((System.Drawing.Size)(resources.GetObject("tabPage2.Size")));
			this.tabPage2.TabIndex = ((int)(resources.GetObject("tabPage2.TabIndex")));
			this.tabPage2.Text = GetLocalizedString(SR.BarCustomizationDialogTabCommands,resources,"tabPage2.Text");
			this.tabPage2.ToolTipText = resources.GetString("tabPage2.ToolTipText");
			this.tabPage2.Visible = ((bool)(resources.GetObject("tabPage2.Visible")));
			// 
			// panel1
			// 
			this.panel1.AccessibleDescription = resources.GetString("panel1.AccessibleDescription");
			this.panel1.AccessibleName = resources.GetString("panel1.AccessibleName");
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("panel1.Anchor")));
			this.panel1.AutoScroll = ((bool)(resources.GetObject("panel1.AutoScroll")));
			this.panel1.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("panel1.AutoScrollMargin")));
			this.panel1.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("panel1.AutoScrollMinSize")));
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.Controls.Add(this.splitter1);
			this.panel1.Controls.Add(this.catView);
			this.panel1.Controls.Add(this.gridHostPanel);
			this.panel1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("panel1.Dock")));
			this.panel1.Enabled = ((bool)(resources.GetObject("panel1.Enabled")));
			this.panel1.Font = ((System.Drawing.Font)(resources.GetObject("panel1.Font")));
			this.panel1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("panel1.ImeMode")));
			this.panel1.Location = ((System.Drawing.Point)(resources.GetObject("panel1.Location")));
			this.panel1.Name = "panel1";
			this.panel1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("panel1.RightToLeft")));
			this.panel1.Size = ((System.Drawing.Size)(resources.GetObject("panel1.Size")));
			this.panel1.TabIndex = ((int)(resources.GetObject("panel1.TabIndex")));
			this.panel1.Text = resources.GetString("panel1.Text");
			this.panel1.Visible = ((bool)(resources.GetObject("panel1.Visible")));
			// 
			// splitter1
			// 
			this.splitter1.AccessibleDescription = resources.GetString("splitter1.AccessibleDescription");
			this.splitter1.AccessibleName = resources.GetString("splitter1.AccessibleName");
			this.splitter1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("splitter1.Anchor")));
			this.splitter1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("splitter1.BackgroundImage")));
			this.splitter1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("splitter1.Dock")));
			this.splitter1.Enabled = ((bool)(resources.GetObject("splitter1.Enabled")));
			this.splitter1.Font = ((System.Drawing.Font)(resources.GetObject("splitter1.Font")));
			this.splitter1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("splitter1.ImeMode")));
			this.splitter1.Location = ((System.Drawing.Point)(resources.GetObject("splitter1.Location")));
			this.splitter1.MinExtra = ((int)(resources.GetObject("splitter1.MinExtra")));
			this.splitter1.MinSize = ((int)(resources.GetObject("splitter1.MinSize")));
			this.splitter1.Name = "splitter1";
			this.splitter1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("splitter1.RightToLeft")));
			this.splitter1.Size = ((System.Drawing.Size)(resources.GetObject("splitter1.Size")));
			this.splitter1.TabIndex = ((int)(resources.GetObject("splitter1.TabIndex")));
			this.splitter1.TabStop = false;
			this.splitter1.Visible = ((bool)(resources.GetObject("splitter1.Visible")));
			// 
			// catView
			// 
			this.catView.AccessibleDescription = resources.GetString("catView.AccessibleDescription");
			this.catView.AccessibleName = resources.GetString("catView.AccessibleName");
			this.catView.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("catView.Anchor")));
			this.catView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("catView.BackgroundImage")));
			this.catView.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("catView.Dock")));
			this.catView.Enabled = ((bool)(resources.GetObject("catView.Enabled")));
			this.catView.Font = ((System.Drawing.Font)(resources.GetObject("catView.Font")));
			this.catView.HideSelection = false;
			this.catView.ImageIndex = ((int)(resources.GetObject("catView.ImageIndex")));
			this.catView.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("catView.ImeMode")));
			this.catView.Indent = ((int)(resources.GetObject("catView.Indent")));
			this.catView.ItemHeight = ((int)(resources.GetObject("catView.ItemHeight")));
			this.catView.Location = ((System.Drawing.Point)(resources.GetObject("catView.Location")));
			this.catView.Name = "catView";
			this.catView.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("catView.RightToLeft")));
			this.catView.Scrollable = false;
			this.catView.SelectedImageIndex = ((int)(resources.GetObject("catView.SelectedImageIndex")));
			this.catView.Size = ((System.Drawing.Size)(resources.GetObject("catView.Size")));
			this.catView.TabIndex = ((int)(resources.GetObject("catView.TabIndex")));
			this.catView.Text = resources.GetString("catView.Text");
			this.catView.Visible = ((bool)(resources.GetObject("catView.Visible")));
			this.catView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.catView_SelectionChanged);
			// 
			// gridHostPanel
			// 
			this.gridHostPanel.AccessibleDescription = resources.GetString("gridHostPanel.AccessibleDescription");
			this.gridHostPanel.AccessibleName = resources.GetString("gridHostPanel.AccessibleName");
			this.gridHostPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("gridHostPanel.Anchor")));
			this.gridHostPanel.AutoScroll = ((bool)(resources.GetObject("gridHostPanel.AutoScroll")));
			this.gridHostPanel.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("gridHostPanel.AutoScrollMargin")));
			this.gridHostPanel.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("gridHostPanel.AutoScrollMinSize")));
			this.gridHostPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("gridHostPanel.BackgroundImage")));
			this.gridHostPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridHostPanel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("gridHostPanel.Dock")));
			this.gridHostPanel.Enabled = ((bool)(resources.GetObject("gridHostPanel.Enabled")));
			this.gridHostPanel.Font = ((System.Drawing.Font)(resources.GetObject("gridHostPanel.Font")));
			this.gridHostPanel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("gridHostPanel.ImeMode")));
			this.gridHostPanel.Location = ((System.Drawing.Point)(resources.GetObject("gridHostPanel.Location")));
			this.gridHostPanel.Name = "gridHostPanel";
			this.gridHostPanel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("gridHostPanel.RightToLeft")));
			this.gridHostPanel.Size = ((System.Drawing.Size)(resources.GetObject("gridHostPanel.Size")));
			this.gridHostPanel.TabIndex = ((int)(resources.GetObject("gridHostPanel.TabIndex")));
			this.gridHostPanel.Text = resources.GetString("gridHostPanel.Text");
			this.gridHostPanel.Visible = ((bool)(resources.GetObject("gridHostPanel.Visible")));
			this.gridHostPanel.SizeChanged += new System.EventHandler(this.gridHostPanel_SizeChanged);
			this.gridHostPanel.MouseUp +=new MouseEventHandler(gridHostPanel_MouseUp);
			// 
			// catMoveUp
			// 
			this.catMoveUp.AccessibleDescription = resources.GetString("catMoveUp.AccessibleDescription");
			this.catMoveUp.AccessibleName = resources.GetString("catMoveUp.AccessibleName");
			this.catMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("catMoveUp.Anchor")));
			this.catMoveUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("catMoveUp.BackgroundImage")));
			this.catMoveUp.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("catMoveUp.Dock")));
			this.catMoveUp.Enabled = ((bool)(resources.GetObject("catMoveUp.Enabled")));
			this.catMoveUp.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("catMoveUp.FlatStyle")));
			this.catMoveUp.Font = ((System.Drawing.Font)(resources.GetObject("catMoveUp.Font")));
			this.catMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("catMoveUp.Image")));
			this.catMoveUp.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("catMoveUp.ImageAlign")));
			this.catMoveUp.ImageIndex = ((int)(resources.GetObject("catMoveUp.ImageIndex")));
			this.catMoveUp.ImageList = this.imageList1;
			this.catMoveUp.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("catMoveUp.ImeMode")));
			this.catMoveUp.Location = ((System.Drawing.Point)(resources.GetObject("catMoveUp.Location")));
			this.catMoveUp.Name = "catMoveUp";
			this.catMoveUp.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("catMoveUp.RightToLeft")));
			this.catMoveUp.Size = ((System.Drawing.Size)(resources.GetObject("catMoveUp.Size")));
			this.catMoveUp.TabIndex = ((int)(resources.GetObject("catMoveUp.TabIndex")));
			this.catMoveUp.Text = resources.GetString("catMoveUp.Text");
			this.catMoveUp.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("catMoveUp.TextAlign")));
			this.catMoveUp.Visible = ((bool)(resources.GetObject("catMoveUp.Visible")));
			this.catMoveUp.Click += new System.EventHandler(this.OnCatMoveUp);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageList1.ImageSize")));
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// catMoveDown
			// 
			this.catMoveDown.AccessibleDescription = resources.GetString("catMoveDown.AccessibleDescription");
			this.catMoveDown.AccessibleName = resources.GetString("catMoveDown.AccessibleName");
			this.catMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("catMoveDown.Anchor")));
			this.catMoveDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("catMoveDown.BackgroundImage")));
			this.catMoveDown.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("catMoveDown.Dock")));
			this.catMoveDown.Enabled = ((bool)(resources.GetObject("catMoveDown.Enabled")));
			this.catMoveDown.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("catMoveDown.FlatStyle")));
			this.catMoveDown.Font = ((System.Drawing.Font)(resources.GetObject("catMoveDown.Font")));
			this.catMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("catMoveDown.Image")));
			this.catMoveDown.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("catMoveDown.ImageAlign")));
			this.catMoveDown.ImageIndex = ((int)(resources.GetObject("catMoveDown.ImageIndex")));
			this.catMoveDown.ImageList = this.imageList1;
			this.catMoveDown.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("catMoveDown.ImeMode")));
			this.catMoveDown.Location = ((System.Drawing.Point)(resources.GetObject("catMoveDown.Location")));
			this.catMoveDown.Name = "catMoveDown";
			this.catMoveDown.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("catMoveDown.RightToLeft")));
			this.catMoveDown.Size = ((System.Drawing.Size)(resources.GetObject("catMoveDown.Size")));
			this.catMoveDown.TabIndex = ((int)(resources.GetObject("catMoveDown.TabIndex")));
			this.catMoveDown.Text = resources.GetString("catMoveDown.Text");
			this.catMoveDown.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("catMoveDown.TextAlign")));
			this.catMoveDown.Visible = ((bool)(resources.GetObject("catMoveDown.Visible")));
			this.catMoveDown.Click += new System.EventHandler(this.OnCatMoveDown);
			// 
			// cmdModify
			// 
			this.cmdModify.AccessibleDescription = resources.GetString("cmdModify.AccessibleDescription");
			this.cmdModify.AccessibleName = resources.GetString("cmdModify.AccessibleName");
			this.cmdModify.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("cmdModify.Anchor")));
			this.cmdModify.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdModify.BackgroundImage")));
			this.cmdModify.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("cmdModify.Dock")));
			this.cmdModify.Enabled = ((bool)(resources.GetObject("cmdModify.Enabled")));
			this.cmdModify.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("cmdModify.FlatStyle")));
			this.cmdModify.Font = ((System.Drawing.Font)(resources.GetObject("cmdModify.Font")));
			this.cmdModify.Image = ((System.Drawing.Image)(resources.GetObject("cmdModify.Image")));
			this.cmdModify.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("cmdModify.ImageAlign")));
			this.cmdModify.ImageIndex = ((int)(resources.GetObject("cmdModify.ImageIndex")));
			this.cmdModify.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("cmdModify.ImeMode")));
			this.cmdModify.Location = ((System.Drawing.Point)(resources.GetObject("cmdModify.Location")));
			this.cmdModify.Name = "cmdModify";
			this.cmdModify.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("cmdModify.RightToLeft")));
			this.cmdModify.Size = ((System.Drawing.Size)(resources.GetObject("cmdModify.Size")));
			this.cmdModify.TabIndex = ((int)(resources.GetObject("cmdModify.TabIndex")));
			this.cmdModify.Text = GetLocalizedString(SR.BarCustomizationDialogModify,resources,"cmdModify.Text");
			this.cmdModify.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("cmdModify.TextAlign")));
			this.cmdModify.Visible = ((bool)(resources.GetObject("cmdModify.Visible")));
			this.cmdModify.Click += new System.EventHandler(this.cmdModify_Click);
			// 
			// label3
			// 
			this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
			this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
			this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
			this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
			this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
			this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
			this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
			this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
			this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
			this.label3.Name = "label3";
			this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
			this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
			this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
			this.label3.Text = GetLocalizedString(SR.BarCustomizationDialogCommands,resources,"label3.Text");
			this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
			this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = GetLocalizedString(SR.BarCustomizationDialogCategories,resources,"label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// tabPage3
			// 
			this.tabPage3.AccessibleDescription = resources.GetString("tabPage3.AccessibleDescription");
			this.tabPage3.AccessibleName = resources.GetString("tabPage3.AccessibleName");
			this.tabPage3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPage3.Anchor")));
			this.tabPage3.AutoScroll = ((bool)(resources.GetObject("tabPage3.AutoScroll")));
			this.tabPage3.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPage3.AutoScrollMargin")));
			this.tabPage3.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPage3.AutoScrollMinSize")));
			this.tabPage3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage3.BackgroundImage")));
			this.tabPage3.Controls.Add(this.resetCustomizationButton);
			this.tabPage3.Controls.Add(this.label7);
			this.tabPage3.Controls.Add(this.label6);
			this.tabPage3.Controls.Add(this.resetPartialMenusBtn);
			this.tabPage3.Controls.Add(this.largeIcons);
			this.tabPage3.Controls.Add(this.label5);
			this.tabPage3.Controls.Add(this.expandAfterDelay);
			this.tabPage3.Controls.Add(this.label4);
			this.tabPage3.Controls.Add(this.alwaysFullMenus);
			this.tabPage3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPage3.Dock")));
			this.tabPage3.Enabled = ((bool)(resources.GetObject("tabPage3.Enabled")));
			this.tabPage3.Font = ((System.Drawing.Font)(resources.GetObject("tabPage3.Font")));
			this.tabPage3.ImageIndex = ((int)(resources.GetObject("tabPage3.ImageIndex")));
			this.tabPage3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPage3.ImeMode")));
			this.tabPage3.Location = ((System.Drawing.Point)(resources.GetObject("tabPage3.Location")));
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPage3.RightToLeft")));
			this.tabPage3.Size = ((System.Drawing.Size)(resources.GetObject("tabPage3.Size")));
			this.tabPage3.TabIndex = ((int)(resources.GetObject("tabPage3.TabIndex")));
			this.tabPage3.Text = GetLocalizedString(SR.BarCustomizationDialogTabOptions,resources,"tabPage3.Text");
			this.tabPage3.ToolTipText = resources.GetString("tabPage3.ToolTipText");
			this.tabPage3.Visible = ((bool)(resources.GetObject("tabPage3.Visible")));
			// 
			// resetCustomizationButton
			// 
			this.resetCustomizationButton.AccessibleDescription = resources.GetString("resetCustomizationButton.AccessibleDescription");
			this.resetCustomizationButton.AccessibleName = resources.GetString("resetCustomizationButton.AccessibleName");
			this.resetCustomizationButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("resetCustomizationButton.Anchor")));
			this.resetCustomizationButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("resetCustomizationButton.BackgroundImage")));
			this.resetCustomizationButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("resetCustomizationButton.Dock")));
			this.resetCustomizationButton.Enabled = ((bool)(resources.GetObject("resetCustomizationButton.Enabled")));
			this.resetCustomizationButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("resetCustomizationButton.FlatStyle")));
			this.resetCustomizationButton.Font = ((System.Drawing.Font)(resources.GetObject("resetCustomizationButton.Font")));
			this.resetCustomizationButton.Image = ((System.Drawing.Image)(resources.GetObject("resetCustomizationButton.Image")));
			this.resetCustomizationButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("resetCustomizationButton.ImageAlign")));
			this.resetCustomizationButton.ImageIndex = ((int)(resources.GetObject("resetCustomizationButton.ImageIndex")));
			this.resetCustomizationButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resetCustomizationButton.ImeMode")));
			this.resetCustomizationButton.Location = ((System.Drawing.Point)(resources.GetObject("resetCustomizationButton.Location")));
			this.resetCustomizationButton.Name = "resetCustomizationButton";
			this.resetCustomizationButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("resetCustomizationButton.RightToLeft")));
			this.resetCustomizationButton.Size = ((System.Drawing.Size)(resources.GetObject("resetCustomizationButton.Size")));
			this.resetCustomizationButton.TabIndex = ((int)(resources.GetObject("resetCustomizationButton.TabIndex")));
			this.resetCustomizationButton.Text = GetLocalizedString(SR.BarCustomizationDialogResetCustomization,resources,"resetCustomizationButton.Text");
			this.resetCustomizationButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("resetCustomizationButton.TextAlign")));
			this.resetCustomizationButton.Visible = ((bool)(resources.GetObject("resetCustomizationButton.Visible")));
			this.resetCustomizationButton.Click += new System.EventHandler(this.resetCustomizationButton_Click);
			// 
			// label7
			// 
			this.label7.AccessibleDescription = resources.GetString("label7.AccessibleDescription");
			this.label7.AccessibleName = resources.GetString("label7.AccessibleName");
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label7.Anchor")));
			this.label7.AutoSize = ((bool)(resources.GetObject("label7.AutoSize")));
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label7.Dock")));
			this.label7.Enabled = ((bool)(resources.GetObject("label7.Enabled")));
			this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label7.Font = ((System.Drawing.Font)(resources.GetObject("label7.Font")));
			this.label7.Image = ((System.Drawing.Image)(resources.GetObject("label7.Image")));
			this.label7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label7.ImageAlign")));
			this.label7.ImageIndex = ((int)(resources.GetObject("label7.ImageIndex")));
			this.label7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label7.ImeMode")));
			this.label7.Location = ((System.Drawing.Point)(resources.GetObject("label7.Location")));
			this.label7.Name = "label7";
			this.label7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label7.RightToLeft")));
			this.label7.Size = ((System.Drawing.Size)(resources.GetObject("label7.Size")));
			this.label7.TabIndex = ((int)(resources.GetObject("label7.TabIndex")));
			this.label7.Text = resources.GetString("label7.Text");
			this.label7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label7.TextAlign")));
			this.label7.Visible = ((bool)(resources.GetObject("label7.Visible")));
			// 
			// label6
			// 
			this.label6.AccessibleDescription = resources.GetString("label6.AccessibleDescription");
			this.label6.AccessibleName = resources.GetString("label6.AccessibleName");
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label6.Anchor")));
			this.label6.AutoSize = ((bool)(resources.GetObject("label6.AutoSize")));
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label6.Dock")));
			this.label6.Enabled = ((bool)(resources.GetObject("label6.Enabled")));
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label6.Font = ((System.Drawing.Font)(resources.GetObject("label6.Font")));
			this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
			this.label6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.ImageAlign")));
			this.label6.ImageIndex = ((int)(resources.GetObject("label6.ImageIndex")));
			this.label6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label6.ImeMode")));
			this.label6.Location = ((System.Drawing.Point)(resources.GetObject("label6.Location")));
			this.label6.Name = "label6";
			this.label6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label6.RightToLeft")));
			this.label6.Size = ((System.Drawing.Size)(resources.GetObject("label6.Size")));
			this.label6.TabIndex = ((int)(resources.GetObject("label6.TabIndex")));
			this.label6.Text = resources.GetString("label6.Text");
			this.label6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.TextAlign")));
			this.label6.Visible = ((bool)(resources.GetObject("label6.Visible")));
			// 
			// resetPartialMenusBtn
			// 
			this.resetPartialMenusBtn.AccessibleDescription = resources.GetString("resetPartialMenusBtn.AccessibleDescription");
			this.resetPartialMenusBtn.AccessibleName = resources.GetString("resetPartialMenusBtn.AccessibleName");
			this.resetPartialMenusBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("resetPartialMenusBtn.Anchor")));
			this.resetPartialMenusBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("resetPartialMenusBtn.BackgroundImage")));
			this.resetPartialMenusBtn.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("resetPartialMenusBtn.Dock")));
			this.resetPartialMenusBtn.Enabled = ((bool)(resources.GetObject("resetPartialMenusBtn.Enabled")));
			this.resetPartialMenusBtn.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("resetPartialMenusBtn.FlatStyle")));
			this.resetPartialMenusBtn.Font = ((System.Drawing.Font)(resources.GetObject("resetPartialMenusBtn.Font")));
			this.resetPartialMenusBtn.Image = ((System.Drawing.Image)(resources.GetObject("resetPartialMenusBtn.Image")));
			this.resetPartialMenusBtn.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("resetPartialMenusBtn.ImageAlign")));
			this.resetPartialMenusBtn.ImageIndex = ((int)(resources.GetObject("resetPartialMenusBtn.ImageIndex")));
			this.resetPartialMenusBtn.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("resetPartialMenusBtn.ImeMode")));
			this.resetPartialMenusBtn.Location = ((System.Drawing.Point)(resources.GetObject("resetPartialMenusBtn.Location")));
			this.resetPartialMenusBtn.Name = "resetPartialMenusBtn";
			this.resetPartialMenusBtn.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("resetPartialMenusBtn.RightToLeft")));
			this.resetPartialMenusBtn.Size = ((System.Drawing.Size)(resources.GetObject("resetPartialMenusBtn.Size")));
			this.resetPartialMenusBtn.TabIndex = ((int)(resources.GetObject("resetPartialMenusBtn.TabIndex")));
			this.resetPartialMenusBtn.Text = GetLocalizedString(SR.BarCustomizationDialogResetPartialMenus,resources,"resetPartialMenusBtn.Text");
			this.resetPartialMenusBtn.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("resetPartialMenusBtn.TextAlign")));
			this.resetPartialMenusBtn.Visible = ((bool)(resources.GetObject("resetPartialMenusBtn.Visible")));
			this.resetPartialMenusBtn.Click += new System.EventHandler(this.resetPartialMenusBtn_Click);
			// 
			// largeIcons
			// 
			this.largeIcons.AccessibleDescription = resources.GetString("largeIcons.AccessibleDescription");
			this.largeIcons.AccessibleName = resources.GetString("largeIcons.AccessibleName");
			this.largeIcons.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("largeIcons.Anchor")));
			this.largeIcons.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("largeIcons.Appearance")));
			this.largeIcons.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("largeIcons.BackgroundImage")));
			this.largeIcons.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("largeIcons.CheckAlign")));
			this.largeIcons.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("largeIcons.Dock")));
			this.largeIcons.Enabled = ((bool)(resources.GetObject("largeIcons.Enabled")));
			this.largeIcons.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("largeIcons.FlatStyle")));
			this.largeIcons.Font = ((System.Drawing.Font)(resources.GetObject("largeIcons.Font")));
			this.largeIcons.Image = ((System.Drawing.Image)(resources.GetObject("largeIcons.Image")));
			this.largeIcons.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("largeIcons.ImageAlign")));
			this.largeIcons.ImageIndex = ((int)(resources.GetObject("largeIcons.ImageIndex")));
			this.largeIcons.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("largeIcons.ImeMode")));
			this.largeIcons.Location = ((System.Drawing.Point)(resources.GetObject("largeIcons.Location")));
			this.largeIcons.Name = "largeIcons";
			this.largeIcons.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("largeIcons.RightToLeft")));
			this.largeIcons.Size = ((System.Drawing.Size)(resources.GetObject("largeIcons.Size")));
			this.largeIcons.TabIndex = ((int)(resources.GetObject("largeIcons.TabIndex")));
			this.largeIcons.Text =GetLocalizedString(SR.BarCustomizationDialogLargeIcons,resources,"largeIcons.Text");
			this.largeIcons.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("largeIcons.TextAlign")));
			this.largeIcons.Visible = ((bool)(resources.GetObject("largeIcons.Visible")));
			// 
			// label5
			// 
			this.label5.AccessibleDescription = resources.GetString("label5.AccessibleDescription");
			this.label5.AccessibleName = resources.GetString("label5.AccessibleName");
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label5.Anchor")));
			this.label5.AutoSize = ((bool)(resources.GetObject("label5.AutoSize")));
			this.label5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label5.Dock")));
			this.label5.Enabled = ((bool)(resources.GetObject("label5.Enabled")));
			this.label5.Font = ((System.Drawing.Font)(resources.GetObject("label5.Font")));
			this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
			this.label5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.ImageAlign")));
			this.label5.ImageIndex = ((int)(resources.GetObject("label5.ImageIndex")));
			this.label5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label5.ImeMode")));
			this.label5.Location = ((System.Drawing.Point)(resources.GetObject("label5.Location")));
			this.label5.Name = "label5";
			this.label5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label5.RightToLeft")));
			this.label5.Size = ((System.Drawing.Size)(resources.GetObject("label5.Size")));
			this.label5.TabIndex = ((int)(resources.GetObject("label5.TabIndex")));
			this.label5.Text = resources.GetString("label5.Text");
			this.label5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.TextAlign")));
			this.label5.Visible = ((bool)(resources.GetObject("label5.Visible")));
			// 
			// expandAfterDelay
			// 
			this.expandAfterDelay.AccessibleDescription = resources.GetString("expandAfterDelay.AccessibleDescription");
			this.expandAfterDelay.AccessibleName = resources.GetString("expandAfterDelay.AccessibleName");
			this.expandAfterDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("expandAfterDelay.Anchor")));
			this.expandAfterDelay.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("expandAfterDelay.Appearance")));
			this.expandAfterDelay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("expandAfterDelay.BackgroundImage")));
			this.expandAfterDelay.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("expandAfterDelay.CheckAlign")));
			this.expandAfterDelay.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("expandAfterDelay.Dock")));
			this.expandAfterDelay.Enabled = ((bool)(resources.GetObject("expandAfterDelay.Enabled")));
			this.expandAfterDelay.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("expandAfterDelay.FlatStyle")));
			this.expandAfterDelay.Font = ((System.Drawing.Font)(resources.GetObject("expandAfterDelay.Font")));
			this.expandAfterDelay.Image = ((System.Drawing.Image)(resources.GetObject("expandAfterDelay.Image")));
			this.expandAfterDelay.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("expandAfterDelay.ImageAlign")));
			this.expandAfterDelay.ImageIndex = ((int)(resources.GetObject("expandAfterDelay.ImageIndex")));
			this.expandAfterDelay.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("expandAfterDelay.ImeMode")));
			this.expandAfterDelay.Location = ((System.Drawing.Point)(resources.GetObject("expandAfterDelay.Location")));
			this.expandAfterDelay.Name = "expandAfterDelay";
			this.expandAfterDelay.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("expandAfterDelay.RightToLeft")));
			this.expandAfterDelay.Size = ((System.Drawing.Size)(resources.GetObject("expandAfterDelay.Size")));
			this.expandAfterDelay.TabIndex = ((int)(resources.GetObject("expandAfterDelay.TabIndex")));
			this.expandAfterDelay.Text = GetLocalizedString(SR.BarCustomizationDialogExpandAfterDelay,resources,"expandAfterDelay.Text");
			this.expandAfterDelay.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("expandAfterDelay.TextAlign")));
			this.expandAfterDelay.Visible = ((bool)(resources.GetObject("expandAfterDelay.Visible")));
			// 
			// label4
			// 
			this.label4.AccessibleDescription = resources.GetString("label4.AccessibleDescription");
			this.label4.AccessibleName = resources.GetString("label4.AccessibleName");
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label4.Anchor")));
			this.label4.AutoSize = ((bool)(resources.GetObject("label4.AutoSize")));
			this.label4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label4.Dock")));
			this.label4.Enabled = ((bool)(resources.GetObject("label4.Enabled")));
			this.label4.Font = ((System.Drawing.Font)(resources.GetObject("label4.Font")));
			this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
			this.label4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.ImageAlign")));
			this.label4.ImageIndex = ((int)(resources.GetObject("label4.ImageIndex")));
			this.label4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label4.ImeMode")));
			this.label4.Location = ((System.Drawing.Point)(resources.GetObject("label4.Location")));
			this.label4.Name = "label4";
			this.label4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label4.RightToLeft")));
			this.label4.Size = ((System.Drawing.Size)(resources.GetObject("label4.Size")));
			this.label4.TabIndex = ((int)(resources.GetObject("label4.TabIndex")));
			this.label4.Text = GetLocalizedString(SR.BarCustomizationDialogPersonalizedMenus,resources,"label4.Text");
			this.label4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.TextAlign")));
			this.label4.Visible = ((bool)(resources.GetObject("label4.Visible")));
			// 
			// alwaysFullMenus
			// 
			this.alwaysFullMenus.AccessibleDescription = resources.GetString("alwaysFullMenus.AccessibleDescription");
			this.alwaysFullMenus.AccessibleName = resources.GetString("alwaysFullMenus.AccessibleName");
			this.alwaysFullMenus.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("alwaysFullMenus.Anchor")));
			this.alwaysFullMenus.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("alwaysFullMenus.Appearance")));
			this.alwaysFullMenus.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("alwaysFullMenus.BackgroundImage")));
			this.alwaysFullMenus.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("alwaysFullMenus.CheckAlign")));
			this.alwaysFullMenus.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("alwaysFullMenus.Dock")));
			this.alwaysFullMenus.Enabled = ((bool)(resources.GetObject("alwaysFullMenus.Enabled")));
			this.alwaysFullMenus.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("alwaysFullMenus.FlatStyle")));
			this.alwaysFullMenus.Font = ((System.Drawing.Font)(resources.GetObject("alwaysFullMenus.Font")));
			this.alwaysFullMenus.Image = ((System.Drawing.Image)(resources.GetObject("alwaysFullMenus.Image")));
			this.alwaysFullMenus.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("alwaysFullMenus.ImageAlign")));
			this.alwaysFullMenus.ImageIndex = ((int)(resources.GetObject("alwaysFullMenus.ImageIndex")));
			this.alwaysFullMenus.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("alwaysFullMenus.ImeMode")));
			this.alwaysFullMenus.Location = ((System.Drawing.Point)(resources.GetObject("alwaysFullMenus.Location")));
			this.alwaysFullMenus.Name = "alwaysFullMenus";
			this.alwaysFullMenus.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("alwaysFullMenus.RightToLeft")));
			this.alwaysFullMenus.Size = ((System.Drawing.Size)(resources.GetObject("alwaysFullMenus.Size")));
			this.alwaysFullMenus.TabIndex = ((int)(resources.GetObject("alwaysFullMenus.TabIndex")));
			this.alwaysFullMenus.Text = GetLocalizedString(SR.BarCustomizationDialogAlwaysFullMenu,resources,"alwaysFullMenus.Text");
			this.alwaysFullMenus.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("alwaysFullMenus.TextAlign")));
			this.alwaysFullMenus.Visible = ((bool)(resources.GetObject("alwaysFullMenus.Visible")));
			// 
			// closeButton
			// 
			this.closeButton.AccessibleDescription = resources.GetString("closeButton.AccessibleDescription");
			this.closeButton.AccessibleName = resources.GetString("closeButton.AccessibleName");
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("closeButton.Anchor")));
			this.closeButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("closeButton.BackgroundImage")));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("closeButton.Dock")));
			this.closeButton.Enabled = ((bool)(resources.GetObject("closeButton.Enabled")));
			this.closeButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("closeButton.FlatStyle")));
			this.closeButton.Font = ((System.Drawing.Font)(resources.GetObject("closeButton.Font")));
			this.closeButton.Image = ((System.Drawing.Image)(resources.GetObject("closeButton.Image")));
			this.closeButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("closeButton.ImageAlign")));
			this.closeButton.ImageIndex = ((int)(resources.GetObject("closeButton.ImageIndex")));
			this.closeButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("closeButton.ImeMode")));
			this.closeButton.Location = ((System.Drawing.Point)(resources.GetObject("closeButton.Location")));
			this.closeButton.Name = "closeButton";
			this.closeButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("closeButton.RightToLeft")));
			this.closeButton.Size = ((System.Drawing.Size)(resources.GetObject("closeButton.Size")));
			this.closeButton.TabIndex = ((int)(resources.GetObject("closeButton.TabIndex")));
			this.closeButton.Text = GetLocalizedString(SR.BarCustomizationDialogClose,resources,"closeButton.Text");
			this.closeButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("closeButton.TextAlign")));
			this.closeButton.Visible = ((bool)(resources.GetObject("closeButton.Visible")));
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.AccessibleDescription = resources.GetString("hScrollBar1.AccessibleDescription");
			this.hScrollBar1.AccessibleName = resources.GetString("hScrollBar1.AccessibleName");
			this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("hScrollBar1.Anchor")));
			this.hScrollBar1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("hScrollBar1.BackgroundImage")));
			this.hScrollBar1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("hScrollBar1.Dock")));
			this.hScrollBar1.Enabled = ((bool)(resources.GetObject("hScrollBar1.Enabled")));
			this.hScrollBar1.Font = ((System.Drawing.Font)(resources.GetObject("hScrollBar1.Font")));
			this.hScrollBar1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("hScrollBar1.ImeMode")));
			this.hScrollBar1.Location = ((System.Drawing.Point)(resources.GetObject("hScrollBar1.Location")));
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("hScrollBar1.RightToLeft")));
			this.hScrollBar1.Size = ((System.Drawing.Size)(resources.GetObject("hScrollBar1.Size")));
			this.hScrollBar1.TabIndex = ((int)(resources.GetObject("hScrollBar1.TabIndex")));
			this.hScrollBar1.Visible = ((bool)(resources.GetObject("hScrollBar1.Visible")));
			// 
			// CustomizationPanel
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Controls.Add(this.hScrollBar1);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.closeButton);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "CustomizationPanel";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Called when the dialog in which this control is hosted is loaded.
		/// </summary>
		/// <remarks>
		/// This virtual is available for inheritors to override and
		/// perform custom initialization. Make sure to call the base
		/// class when you override.
		/// </remarks>
		protected virtual internal void OnDialogLoad()
		{
			if(this.customizingBarManager.DesignMode)
			{
				this.tabControl.Controls.Remove(this.tabPage3);
			}
		}
		/// <summary>
		/// Called when the dialog in which this control is hosted is
		/// either made visible or hidden.
		/// </summary>
		/// <remarks>
		/// This virtual is available for inheritors to override and 
		/// perform custom operations. Make sure to call the base class
		/// when you override.
		/// </remarks>
		protected virtual internal void OnDialogVisibiltyChanged()
		{
			if(this.custDlg.Visible)
			{
				this.RefreshToolbarList();
				this.RefreshCategoryList();
				this.RefreshOptionsTab();

				this.customizingBarManager.CustomizingItemChanged 
					+= new EventHandler(this.CustomizingItemChanged);

				this.tabControl_SelectedIndexChanged(this.tabControl, EventArgs.Empty);

				if(!this.customizingBarManager.DesignMode)
				{
					// The Commands label's left has to align with the menu list below it.
					this.label3.Left = this.gridHostPanel.Left;
				}
			}
			else
			{
				this.customizingBarManager.CustomizingItemChanged 
					-= new EventHandler(this.CustomizingItemChanged);
				this.customizingBarManager.commandBarManager.SetDesigntimeSelectedBar(null);
			}
		}
		private bool IsBarVisible(Bar bar)
		{
			//			bool visible = (bar.BarStyle & BarStyle.Visible) > 0;
			//			if(this.customizingBarManager.commandBarManager.IsUserPrefAvailable(bar))
			//				visible = this.customizingBarManager.commandBarManager.GetUserVisibilityPrefOfBar(bar);
			//
			//			return visible;
			if(this.customizingBarManager.DesignMode)
				return (bar.BarStyle & BarStyle.Visible) > 0;
			else
				return this.customizingBarManager.commandBarManager.IsBarVisible(bar);
		}

		private void BarPropertyChanged(object sender, SyncfusionPropertyChangedEventArgs args)
		{
			if(this.Visible && args.PropertyName == "BarName")
			{
				this.RefreshToolbarList();
			}
		}
		private void RefreshToolbarList()
		{
			int curSelection = this.toolbarList.SelectedIndex;

			this.toolbarList.Items.Clear();
			this.barsList.Clear();

			this.toolbarList.ItemCheck -= new ItemCheckEventHandler(this.toolbarList_ItemCheck);
			foreach(Bar bar in this.customizingBarManager.Bars)
			{
				this.barsList.Add(bar);

				string text = ( bar.Caption == null ) ? bar.BarName : bar.Caption;

				int index = this.toolbarList.Items.Add( text, this.IsBarVisible(bar));
				if(!bar.AllowHiding && !this.customizingBarManager.DesignMode)
					this.toolbarList.SetItemCheckState(index, CheckState.Indeterminate);
			}
			// Also show the Bars in the Child Managers
			if(!this.customizingBarManager.DesignMode)
			{
				ArrayList managers = ((MainFrameBarManager)this.customizingBarManager).ChildManagers;
				foreach(BarManager manager in managers)
				{
					foreach(Bar bar in manager.Bars)
					{
						bool visible = this.IsBarVisible(bar);
						if((bar.BarStyle & BarStyle.IsMainMenu) == 0)
						{
							this.barsList.Add(bar);
							int index = this.toolbarList.Items.Add(bar.BarName, visible);
							if(!bar.AllowHiding)
								this.toolbarList.SetItemCheckState(index, CheckState.Indeterminate);
						}
					}
				}
			}

			this.toolbarList.ItemCheck += new ItemCheckEventHandler(this.toolbarList_ItemCheck);

			if(curSelection != -1 && curSelection < this.toolbarList.Items.Count)
				this.toolbarList.SelectedIndex = curSelection;
		}
		#region OPERATIONS
		private void AddToolbar(string toolbarName)
		{
			if(toolbarName == String.Empty)
				return;

			// Check for duplicate entry
			foreach(Bar bar in this.customizingBarManager.Bars)
			{
				if(bar.BarName == toolbarName)
				{
					MessageBox.Show(SR.GetString(SR.DuplicateNameWarning, toolbarName));
					return;
				}
			}
			if(this.customizingBarManager.DesignMode)
			{
				//this.customizingBarManager.Bars.Add(new Bar(this.customizingBarManager, toolbarName));
				Bar bar = (Bar)this.designerHost.CreateComponent(typeof(Bar));
				bar.Manager = this.customizingBarManager;
				bar.BarName = toolbarName;
				this.customizingBarManager.Bars.Add(bar);
			}
			else
				this.customizingBarManager.MainFrameBarManager.RecordAddCustomBar(toolbarName);

			RefreshToolbarList();
		}

		private void DeleteSelectedToolbar()
		{
			if(this.toolbarList.SelectedIndex == -1)
				return;

			if(this.customizingBarManager.DesignMode)
			{
				//this.customizingBarManager.Bars.RemoveAt(this.toolbarList.SelectedIndex);
				Bar bar = this.barsList[this.toolbarList.SelectedIndex] as Bar;
				if(this.designerHost != null)
					this.designerHost.DestroyComponent(bar);
			}
			else
			{
				this.customizingBarManager.MainFrameBarManager.RemoveCustomBar
					((Bar)this.barsList[this.toolbarList.SelectedIndex]);

				this.RefreshToolbarList();
			}
		}
		#endregion OPERATIONS
		#region TOOLBARS_PAGE
		private void newButton_Click(object sender, System.EventArgs e)
		{
			this.customizingBarManager.HidePopups();
			
			GenericNewDlg newDialog = new GenericNewDlg();
            newDialog.Text = SR.GetString(SR.ToolbarNameEntryDialogCaption, this.customizingBarManager);
            newDialog.NameLabel = SR.GetString(SR.NewToolbarName, this.customizingBarManager);
            newDialog.OKButtonLabel = SR.GetString(SR.GenericOKButtonText, this.customizingBarManager);
            newDialog.CancelButtonLabel = SR.GetString(SR.GenericCancelButtonText, this.customizingBarManager);
			if(newDialog.ShowDialog() == DialogResult.OK)
				this.AddToolbar(newDialog.NewName);
		}
		
		private void toolbarList_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			Bar bar = this.barsList[e.Index] as Bar;
			if(!bar.AllowHiding
				&& !this.customizingBarManager.DesignMode)
			{
				e.NewValue = CheckState.Indeterminate;
				return;
			}

			if(e.NewValue == CheckState.Unchecked)
				this.customizingBarManager.commandBarManager.SetBarVisibility(bar, false);
			else
				this.customizingBarManager.commandBarManager.SetBarVisibility(bar, true);
		}
		// Populate the PropertyGrid with the appropriate object
		private void toolbarList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.customizingBarManager.HidePopups();

			if(this.toolbarList.SelectedIndex != -1 && this.designerHost != null)
				this.SelectComponentInDesigner(this.barsList[this.toolbarList.SelectedIndex]);

			if(!this.customizingBarManager.DesignMode)
			{
				if(this.toolbarList.SelectedIndex == -1
					|| !this.customizingBarManager.MainFrameBarManager.IsBarUserDeletable
					(this.barsList[this.toolbarList.SelectedIndex] as Bar))
					this.deleteButton.Enabled = false;
				else
					this.deleteButton.Enabled = true;

				if(this.toolbarList.SelectedIndex == -1)
					this.resetButton.Enabled = false;
				else
					this.resetButton.Enabled = true;
			}
			else
			{
				this.deleteButton.Enabled = true;
				this.resetButton.Enabled = false;
				if(this.toolbarList.SelectedIndex != -1)
					this.customizingBarManager.commandBarManager.SetDesigntimeSelectedBar(
						this.barsList[this.toolbarList.SelectedIndex] as Bar);
				else
					this.customizingBarManager.commandBarManager.SetDesigntimeSelectedBar(null);
			}
		}
		
		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			if(this.toolbarList.SelectedIndex == -1)
				MessageBox.Show(SR.GetString(SR.InvalidToolbarDeleteWarning));
			else
			{
				if(MessageBox.Show(SR.GetString(SR.ToolbarDeleteConfirmation), SR.GetString(SR.DeleteConfirmDlgCaption), MessageBoxButtons.YesNo) == DialogResult.Yes)
					this.DeleteSelectedToolbar();
			}
		}
		private void resetButton_Click(object sender, System.EventArgs e)
		{
			// Reset changes in the selected Bar.
			if(this.customizingBarManager is MainFrameBarManager
				&& !this.customizingBarManager.DesignMode)
			{
				if(this.toolbarList.SelectedIndex == -1)
					return;

				Bar bar = this.barsList[this.toolbarList.SelectedIndex] as Bar;

				this.customizingBarManager.MainFrameBarManager.ResetContainer(bar);

                MessageBox.Show(SR.GetString(SR.SuccesfulToolbarResetMessage, this.customizingBarManager.MainFrameBarManager), SR.GetString(SR.SuccesfulResetMessageBoxTitle, this.customizingBarManager.MainFrameBarManager));
			}
		}
		private void OnToolbarsPageActivated()
		{
			//			if(this.toolbarList.SelectedIndex == -1 && this.toolbarList.Items.Count > 0)
			//			{
			//				this.toolbarList.SelectedIndex = 0;
			//				this.customizingBarManager.CustomizingItem = null;
			//			}
		}
		#endregion TOOLBARS_PAGE
		#region COMMANDS_PAGE
		private void catView_SelectionChanged(object sender, TreeViewEventArgs e)
		{
			this.RefreshBarItemsList();

			int catIndex = -1;
			BarManager manager = null;
			this.GetSelectedCategoryInTree(ref manager, ref catIndex);

			// Update the Up/Down buttons' Enabled property
			this.catMoveUp.Enabled = true;
			this.catMoveDown.Enabled = true;
			if(catIndex == -1)
			{
				this.catMoveUp.Enabled = false;
				this.catMoveDown.Enabled = false;
			}
			if(catIndex == 0)
				this.catMoveUp.Enabled = false;
			else if(catIndex == manager.Categories.Count - 1)
				this.catMoveDown.Enabled = false;

		}
		private void GetSelectedCategoryInTree(ref BarManager manager, ref int categoryIndex)
		{
			TreeNode selectedNode = this.catView.SelectedNode;
			if(selectedNode == null)
				return;

			if(selectedNode.Tag is BarManager)
			{
				manager = selectedNode.Tag as BarManager;
				categoryIndex = -1;
			}
			else
			{
				TreeNode parentNode = selectedNode.Parent;
				manager = parentNode.Tag as BarManager;
				if(selectedNode.Tag is int)
					categoryIndex = (int)selectedNode.Tag;
				else
					categoryIndex = parentNode.Nodes.IndexOf(selectedNode);
			}
			return;
		}
		private void SelectLastCategory()
		{
			TreeNode rootNode = this.catView.Nodes[0];
			if(rootNode.Nodes.Count > 0)
				this.catView.SelectedNode = rootNode.Nodes[rootNode.Nodes.Count-1];
		}
		private void SelectACategory()
		{
			if(this.catView.SelectedNode == null
				|| this.catView.SelectedNode.Tag is BarManager)
			{
				TreeNode selectedRootNode = this.catView.SelectedNode;

				if(selectedRootNode == null)
				{
					int i = 0;
					while(this.catView.SelectedNode == null && i < this.catView.Nodes.Count)
					{
						selectedRootNode = this.catView.Nodes[i];
						if(selectedRootNode.Nodes.Count > 0)
							this.catView.SelectedNode = selectedRootNode.Nodes[0];
						i++;
					}
				}
			}
		}
		private void SelectCategoryIndex(int catIndex)
		{
			TreeNode rootNode = this.catView.Nodes[0];
			if( rootNode.Nodes.Count > catIndex && catIndex >= 0 )
				this.catView.SelectedNode = rootNode.Nodes[catIndex];
		}

		private void AlwaysFullMenus_Clicked(object sender, EventArgs e)
		{
			this.customizingBarManager.UsePartialMenus = !this.alwaysFullMenus.Checked;
			this.RefreshOptionsTab();
		}
		private void ExpandAfterDelay_Clicked(object sender, EventArgs e)
		{
			if(this.customizingBarManager is MainFrameBarManager)
				((MainFrameBarManager)this.customizingBarManager).ExpandPartialMenusAfterDelay = this.expandAfterDelay.Checked;
		}
		
		private void LargeIcons_Clicked(object sender, EventArgs e)
		{
			if(this.customizingBarManager is MainFrameBarManager)
				((MainFrameBarManager)this.customizingBarManager).LargeIcons = this.largeIcons.Checked;
		}

		private void RefreshOptionsTab()
		{
			if(this.customizingBarManager is MainFrameBarManager)
				this.alwaysFullMenus.Checked = !this.customizingBarManager.UsePartialMenus;
			else
				this.alwaysFullMenus.Enabled = false;

			if(this.customizingBarManager is MainFrameBarManager
				&& !this.alwaysFullMenus.Checked)
			{
				this.expandAfterDelay.Enabled = true;
				this.expandAfterDelay.Checked = ((MainFrameBarManager)this.customizingBarManager).ExpandPartialMenusAfterDelay;
			}
			else
				this.expandAfterDelay.Enabled = false;

			if(this.customizingBarManager is MainFrameBarManager)
			{
				this.largeIcons.Enabled = true;
				this.largeIcons.Checked = ((MainFrameBarManager)this.customizingBarManager).LargeIcons;
			}
			else
				this.largeIcons.Enabled = false;

			if (largeIcons.Enabled)
			{
				// Do atleast one of the managers have a LargeImageList?
				bool largeImageListFound = this.customizingBarManager.LargeImageList != null || this.customizingBarManager.LargeImageListAdv != null;
				if (!largeImageListFound && this.customizingBarManager is MainFrameBarManager)
				{
					ArrayList childManagers = ((MainFrameBarManager)this.customizingBarManager).ChildManagers;
					foreach (BarManager manager in childManagers)
					{
						if (manager.LargeImageList != null || manager.LargeImageListAdv != null)
						{
							largeImageListFound = true;
							break;
						}
					}
				}
				largeIcons.Enabled = largeImageListFound;
			}
		}

		private void RefreshCategoryList()
		{
			if(this.customizingBarManager.DesignMode)
			{
				int curSelection = -1;
				BarManager dummy = null;
				this.GetSelectedCategoryInTree(ref dummy, ref curSelection);

				this.catView.Nodes.Clear();

				TreeNode rootNode = this.AddCategoryNodeFrom(this.customizingBarManager);

				if(curSelection != -1 && curSelection < rootNode.Nodes.Count)
					this.catView.SelectedNode = rootNode.Nodes[curSelection];
			}
			else
			{
				this.catView.Nodes.Clear();

				this.AddCategoryNodeFrom(this.customizingBarManager);
				ArrayList managers = ((MainFrameBarManager)this.customizingBarManager).ChildManagers;
				foreach(BarManager manager in managers)
					this.AddCategoryNodeFrom(manager);
			}
		}
		private TreeNode AddCategoryNodeFrom(BarManager manager)
		{
			if(!manager.DesignMode && !manager.ShowItemsInCustomizationDialog)
				return null;

			// Insert only this manager's category list
			// Parent node
			TreeNode rootNode = new TreeNode(manager.FormName);
			rootNode.Tag = manager;
			if(manager.DesignMode)
			{
				rootNode.Text = rootNode.Text + " (Right Click to Add Categories)";
				rootNode.Checked = manager.ShowItemsInCustomizationDialog;
			}

			int i = -1;
			foreach(String catName in manager.Categories)
			{	
				i++;
				TreeNode treeNode = new TreeNode(catName);
				if(this.customizingBarManager.DesignMode)
				{
					if(manager.CategoriesToIgnoreInCustDialog.IndexOf(i) == -1)
						treeNode.Checked = true;
					else
						treeNode.Checked = false;
					rootNode.Nodes.Add(treeNode);
				}
				else
				{
					if(manager.CategoriesToIgnoreInCustDialog.IndexOf(i) == -1)
					{
						rootNode.Nodes.Add(treeNode);
						treeNode.Tag = i;	// Need to know the corresponding index, since we are not adding all the categories.
					}
				}
			}

			this.catView.Nodes.Add(rootNode);
			
			return rootNode;
		}
		private void RefreshBarItemsList()
		{
			this.menuGridBase.BeginUpdate();
			this.menuGridBase.Model.BeginInit();

			this.menuGridBase.HidePopup(PopupCloseType.Canceled);

			if(this.menuGridBase.ParentItem != null)
				this.menuGridBase.ParentItem.Items.CollectionChanged
					-= new CollectionChangeEventHandler(this.BarItemsCollectionChanged);

			int selectedCat = -1;
			BarManager manager = null;
			this.GetSelectedCategoryInTree(ref manager, ref selectedCat);

			StandAloneParentBarItem parentItem = new StandAloneParentBarItem();
			parentItem.Manager = manager;

            if(manager.Categories.Count > 0)
			{
				// Makes sure a category is selected
				this.SelectACategory();

				this.GetSelectedCategoryInTree(ref manager, ref selectedCat);

				if(selectedCat > -1)
				{
					foreach(BarItem item in manager.Items)
					{
						if(item.CategoryIndex == selectedCat)				
						{
							if(manager.DesignMode || item.Customizable)
								parentItem.Items.Add(item);
						}
					}
				}
			}
			parentItem.Items.CollectionChanged
				+= new CollectionChangeEventHandler(this.BarItemsCollectionChanged);
			this.menuGridBase.Model.EndInit();
			this.menuGridBase.EndUpdate(false);
			this.menuGridBase.Show(parentItem, new Point(0, 0), null, false);

			this.menuGridBase.Height = this.menuGridBase.GetVScrollPixelHeight();
			this.MenuGridSizeRefresh( this.menuGridBase );
			
			this.menuGridBase.Refresh();
		}
		private void AddCategory(string catName)
		{
			if(catName == String.Empty)
				return;

			// Check for duplicate entry
			foreach(String curCatName in this.customizingBarManager.Categories)
			{
				if(curCatName == catName)
				{
					MessageBox.Show(SR.GetString(SR.DuplicateNameWarning, catName));
					return;
				}
			}
			this.customizingBarManager.Categories.Add(catName);
			this.RaiseBarManagerChanged((MemberDescriptor)TypeDescriptor.GetProperties((object)this.customizingBarManager)[(string)"Categories"]);
			RefreshCategoryList();
			this.SelectLastCategory();
		}
		private void RaiseBarManagerChanged(MemberDescriptor memberDescriptor)
		{
			if (designerHost != null)
			{
				IComponentChangeService iComponentChangeService;
				iComponentChangeService = (IComponentChangeService)this.designerHost.GetService(typeof(IComponentChangeService));
				// Raise the RaiseComponentChanged events
				if(iComponentChangeService != null)
				{
					try{iComponentChangeService.OnComponentChanged(this.customizingBarManager, memberDescriptor,null,null);}
					finally{}
				}
			}
		}
		private void SelectComponentInDesigner(object component)
		{
			if(designerHost != null)
			{
				ISelectionService selService = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
				if(selService != null)
					selService.SetSelectedComponents(new Object[1] { component }, SelectionTypes.Replace);
			}
		}
		private void SetupSelectionListenerHandlers(bool listen)
		{
			if(this.designerHost != null)
			{
				ISelectionService selService = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
				if(selService != null)
				{
					if(listen)
						selService.SelectionChanged += new EventHandler(this.DesigntimeSelectionChanged);
					else
						selService.SelectionChanged -= new EventHandler(this.DesigntimeSelectionChanged);
				}
			}
		}

		private void DesigntimeSelectionChanged(object sender, EventArgs e)
		{
			if(designerHost != null)
			{
				ISelectionService selService = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
				if(selService != null)
				{
					Bar bar = selService.PrimarySelection as Bar;
					if(bar == null)
						this.toolbarList.SelectedIndex = -1;
					else
					{
						if(!this.ParentForm.Visible)
							this.customizingBarManager.Customize(this.designerHost);					
						// Ensure the right tab is selected
						if(this.tabControl.SelectedIndex != 0)
							this.tabControl.SelectedIndex = 0;

						this.toolbarList.SelectedIndex = this.barsList.IndexOf(bar);
					}
					
					BarItem item = selService.PrimarySelection as BarItem;
					if(item == null)
						this.menuGridBase.SelectedIndex = -1;
					else if(this.customizingBarManager.Items.Contains(item))
					{
						if(!this.ParentForm.Visible)
							this.customizingBarManager.Customize(this.designerHost);

						// Ensure the right tab is selected
						if(this.tabControl.SelectedIndex != 1)
							this.tabControl.SelectedIndex = 1;

						// Ensure the right category is selected
						this.SelectCategoryIndex(item.CategoryIndex);

						// Select the right item in the menugrid.
						this.menuGridBase.SelectedItem = item;
					}
				}
			}
		}

		private void MoveCategory(bool up)
		{
			int selectedIndex = -1;
			BarManager manager = null;
			this.GetSelectedCategoryInTree(ref manager, ref selectedIndex);
			if(selectedIndex != -1)
			{
				if((up && selectedIndex == 0)
					|| (!up && selectedIndex == manager.Categories.Count - 1))
					return;

				this.menuGridBase.BeginUpdate();

				object entry = manager.Categories[selectedIndex];
				manager.Categories.RemoveAt(selectedIndex);

				int newIndex = -1;
				if(up)
                    newIndex = selectedIndex - 1;
				else
                    newIndex = selectedIndex + 1;

				manager.Categories.Insert(newIndex, entry);
				this.AdjustCategoryIDs(selectedIndex, newIndex);
				this.RefreshCategoryList();
				this.SelectCategoryIndex(newIndex);
				this.menuGridBase.EndUpdate(true);
			}
		}
		// Assuming can only move 1 step at a time.
		private void AdjustCategoryIDs(int moveFrom, int moveTo)
		{
			// First pass; move the items to 2 different brand new categories
			int tempMoveFrom = this.customizingBarManager.Categories.Count;
			int tempMoveTo = this.customizingBarManager.Categories.Count + 1;
			this.customizingBarManager.Categories.Add("Dummy Cat 1");
			this.customizingBarManager.Categories.Add("Dummy Cat 2");
			foreach(BarItem item in this.customizingBarManager.Items)
			{	
				if(item.CategoryIndex == moveFrom)
					item.CategoryIndex = tempMoveTo;
				else if(item.CategoryIndex == moveTo)
					item.CategoryIndex = tempMoveFrom;
			}
			// Second pass, move them to the desired category
			foreach(BarItem item in this.customizingBarManager.Items)
			{	
				if(item.CategoryIndex == tempMoveFrom)
					item.CategoryIndex = moveFrom;
				else if(item.CategoryIndex == tempMoveTo)
					item.CategoryIndex = moveTo;
			}
			// Remove the last 2 dummy categories
			this.customizingBarManager.Categories.RemoveAt(tempMoveFrom);
			// tempMoveFrom again due to the above remove.
			this.customizingBarManager.Categories.RemoveAt(tempMoveFrom);
		}
		private void DeleteSelectedCategory()
		{
			int catIndex = -1;
			BarManager manager = null;
			this.GetSelectedCategoryInTree(ref manager, ref catIndex);
			if(catIndex != -1)
			{
				if(this.menuGridBase.ParentItem.Items.Count > 0)
				{
					MessageBox.Show("Please delete all the Command Items in this category before deleting this category.");
					return;
				}
				foreach(BarItem item in this.customizingBarManager.Items)
				{
					if(item.CategoryIndex > catIndex)
						item.CategoryIndex--;
				}
				manager.Categories.RemoveAt(catIndex);
				this.RaiseBarManagerChanged((MemberDescriptor)TypeDescriptor.GetProperties((object)manager)[(string)"Categories"]);
				this.RefreshCategoryList();
			}
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			this.custDlg.Close();
		}

		private void cmdModify_Click(object sender, System.EventArgs e)
		{
			this.barItemModifyMenu.Show(this.cmdModify, new Point(0, this.cmdModify.Height));
		}
		private void OnCatAdd(object sender, EventArgs e)
		{
			GenericNewDlg newDialog = new GenericNewDlg();
			newDialog.Text = "Category Name Entry Dialog";
			newDialog.NameLabel = "New Category Name:";
			if(newDialog.ShowDialog() == DialogResult.OK)
				this.AddCategory(newDialog.NewName);
		}
		private void CatModifyMenu_Popup(object sender, EventArgs e)
		{
			int selCatIndex = -1;
			BarManager manager = null;
			this.GetSelectedCategoryInTree(ref manager, ref selCatIndex);

			bool selectedItemAvailable = selCatIndex == -1 ? false : true;
			foreach(BarItem item in this.catModifyMenu.ParentBarItem.Items)
			{
				if(item.Text != SR.GetString(SR.Add))
				{
					if(selectedItemAvailable)
						item.Enabled = true;
					else
						item.Enabled = false;
				}
			}
		}
		private void BarItemModifyMenu_Popup(object sender, EventArgs e)
		{
			bool selectedItemAvailable = this.menuGridBase.SelectedIndex == -1 ? false : true;
			foreach(BarItem item in this.catModifyMenu.ParentBarItem.Items)
			{
				if(item.Text != SR.GetString(SR.Add))
				{
					if(selectedItemAvailable)
						item.Enabled = true;
					else
						item.Enabled = false;
				}
			}
		}
		private void BarItemModifyMenu(object sender, EventArgs e)
		{

		}
		private void OnCatRename(object sender, EventArgs e)
		{
			GenericNewDlg newDialog = new GenericNewDlg();
			newDialog.Text = "Category Rename Entry Dialog";
			newDialog.NameLabel = "New Category Name:";
			newDialog.Validating += new CancelEventHandler(this.NewDialog_Validating);
			if(newDialog.ShowDialog() == DialogResult.OK)
			{
				BarManager manager = null;
				int selCatIndex = -1;
				this.GetSelectedCategoryInTree(ref manager, ref selCatIndex);
				manager.Categories[selCatIndex] = newDialog.NewName;
				this.RaiseBarManagerChanged((MemberDescriptor)TypeDescriptor.GetProperties((object)this.customizingBarManager)[(string)"Categories"]);
				this.RefreshCategoryList();
			}
		}
		private void NewDialog_Validating(object sender, CancelEventArgs e)
		{
			GenericNewDlg dlg = (GenericNewDlg)sender;
			string newCatName = dlg.NewName;
			foreach(String curCatName in this.customizingBarManager.Categories)
			{
				if(curCatName == newCatName)
				{
					MessageBox.Show(SR.GetString(SR.DuplicateNameWarning, newCatName));
					e.Cancel = true;
					break;
				}
			}
		}
		private void OnCatDelete(object sender, EventArgs e)
		{
			this.DeleteSelectedCategory();
		}
		private void OnCatMoveUp(object sender, System.EventArgs e)
		{
			this.MoveCategory(true);
		}
		private void OnCatMoveDown(object sender, System.EventArgs e)
		{
			this.MoveCategory(false);
		}

		private void OnItemAdd(object sender, EventArgs e)
		{
			if(this.customizingBarManager.Categories.Count == 0)
			{
				MessageBox.Show("There should be atleast 1 Category in the BarManager before you can add a BarItem. Add a category by right-mouse-clicking in the Categories view.");
				return;
			}
			NewBarItemDialog newDialog = new NewBarItemDialog();
			if(this.customizingBarManager is MainFrameBarManager)
			{
				newDialog.typeBox.Items.Add( new TypeBoxItem( "MdiListBarItem", ItemType.MdiListBarItem ) );
				newDialog.typeBox.Items.Add( new TypeBoxItem( "ToolbarListBarItem", ItemType.ToolbarListBarItem ) );
			}

			newDialog.typeBox.Items.Add( new TypeBoxItem( "TextBoxBarItem", ItemType.TextBoxBarItem ) );

			if(newDialog.ShowDialog() == DialogResult.OK)
				this.AddBarItem(newDialog.NewName, newDialog.Type);
		}
		
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void RemoveReferencesToItem(Bar bar)
		{
			this.customizingBarManager.Bars.Remove(bar);
			this.RefreshToolbarList();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void RemoveReferencesToItem(BarItem item)
		{
			int curSelectedIndex = this.menuGridBase.SelectedIndex;
			if(curSelectedIndex != -1)
			{
				// Check if the selected item is the item to remove
				if(this.menuGridBase.ParentItem.Items[curSelectedIndex] != item)
					curSelectedIndex = -1;
			}

			this.customizingBarManager.RemoveReferencesToBarItem(item);
			
			if(this.Visible)
			{
				this.RefreshBarItemsList();
				if(curSelectedIndex != -1)
				{
					// Update the selection
					if(curSelectedIndex >= this.menuGridBase.ParentItem.Items.Count)
						curSelectedIndex = this.menuGridBase.ParentItem.Items.Count - 1;

					if(curSelectedIndex >= 0)
					{
						this.menuGridBase.SelectedIndex = curSelectedIndex;
						this.customizingBarManager.CustomizingItem = this.menuGridBase.SelectedItem;
					}
				}
			}
		}

		private void OnItemDelete(object sender, EventArgs e)
		{
			int curSelectedIndex = this.menuGridBase.SelectedIndex;
			if(curSelectedIndex != -1)
			{
				BarItem selectedItem = this.menuGridBase.ParentItem.Items[curSelectedIndex];
				
				if(this.designerHost != null)
					this.designerHost.DestroyComponent(selectedItem);
			}
		}

		private void OnDeleteItems(object sender, EventArgs e)
		{
			if(MessageBox.Show("Are you sure you want to delete all Command Items in this category?", "CustomizableCommandBars", MessageBoxButtons.YesNo)
				== DialogResult.Yes)
			{
				// Remove items with the current selected category index
				int catIndex = -1;
				BarManager manager = null;
				this.GetSelectedCategoryInTree(ref manager, ref catIndex);
				for(int i = manager.Items.Count - 1; i >= 0; i--)
				{
					BarItem item = manager.Items[i];
					if(item.CategoryIndex == catIndex)
					{
						manager.Items.RemoveAt(i);

						if(this.designerHost != null)
							this.designerHost.DestroyComponent(item);
					}
				}
				this.RefreshBarItemsList();
			}
		}

		private void AddBarItem(string newName, int type)
		{
			BarItem newBarItem = null;
			
			if (this.designerHost != null)
			{
				int catIndex = -1;
				BarManager manager = null;
				this.GetSelectedCategoryInTree(ref manager, ref catIndex);
				// Also call CreateComponent and parent the tabpage to the tabcontrol
				switch(type)
				{
					case 0:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(BarItem));
						break;
					case 1:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(ParentBarItem));
						break;
					case 2:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(DropDownBarItem));
						break;
					case 3:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(ComboBoxBarItem));
						break;
					case 4:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(ListBarItem));
						break;
					case 5:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(StaticBarItem));
						break;
					case 6:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(MdiListBarItem));
						break;
					case 7:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(ToolbarListBarItem));
						break;
					case 8:
						newBarItem = (BarItem)this.designerHost.CreateComponent(typeof(TextBoxBarItem));
						break;
				}

				newBarItem.CategoryIndex = catIndex;
				newBarItem.Text = newName;
				this.customizingBarManager.Items.Add((BarItem)newBarItem);
				
				this.RefreshBarItemsList();
				this.menuGridBase.SelectedIndex = this.menuGridBase.ParentItem.Items.Count - 1;
				if(this.menuGridBase.SelectedIndex > -1)
					this.customizingBarManager.CustomizingItem = this.menuGridBase.SelectedItem;
			}
		}
		private void OnCommandsPageActivated()
		{
			int catIndex = -1;
			BarManager manager = null;
			this.GetSelectedCategoryInTree(ref manager, ref catIndex);

			if(catIndex == -1)
				// Set a default selected index in the category list
				this.SelectACategory();

			if(!this.DesignMode)
				this.menuGridBase.TopRowIndex = 1;

			this.catView.Scrollable = true;
		}
		private void MenuGrid_MouseUp(object sender, MouseEventArgs e)
		{
			if(this.designerHost != null && e.Button == MouseButtons.Right)
				this.barItemModifyMenu.Show(this.menuGridBase, new Point(e.X, e.Y));
		}
		private void CatView_MouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = this.catView.GetNodeAt(e.X, e.Y);
			if (node != null)
				this.catView.SelectedNode = node;
		}
		private void CatView_MouseUp(object sender, MouseEventArgs e)
		{
			if(this.designerHost != null && e.Button == MouseButtons.Right)
				this.catModifyMenu.Show(this.catView, new Point(e.X, e.Y));
		}
		private void CatView_AfterCheck(object sender, TreeViewEventArgs args)
		{
			BarManager manager = null;
			int catIndex = -1;
			this.GetSelectedCategoryInTree(ref manager, ref catIndex);
			if(catIndex != -1)
			{
				// Clicked on a category
				if(args.Node.Checked)
					manager.CategoriesToIgnoreInCustDialog.Remove(catIndex);
				else
					if(manager.CategoriesToIgnoreInCustDialog.IndexOf(catIndex) == -1)
					manager.CategoriesToIgnoreInCustDialog.Add(catIndex);
			}
			else
			{
				// Clicked on the manager root.
				manager.ShowItemsInCustomizationDialog = args.Node.Checked;
			}
			this.RaiseBarManagerChanged((MemberDescriptor)TypeDescriptor.GetProperties((object)this.customizingBarManager)[(string)"CategoriesToIgnoreInCustDialog"]);
		}
		#endregion COMMANDS_PAGE
		private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch(this.tabControl.SelectedIndex)
			{
				case 1: /* Commands page*/
					this.OnCommandsPageActivated();
					break;
				case 0: /* Toolbars page*/
					this.OnToolbarsPageActivated();
					break;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual internal void OnDialogActivated()
		{
			this.customizingBarManager.Items.ItemPropertyChanged
				+= new SyncfusionPropertyChangedEventHandler
				(this.ItemPropertyChanged);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual internal void OnDialogDeactivated()
		{
			this.customizingBarManager.Items.ItemPropertyChanged
				-= new SyncfusionPropertyChangedEventHandler
				(this.ItemPropertyChanged);

			Form activeForm = Form.ActiveForm;
			if(Form.ActiveForm != this.customizingBarManager.Form
				&& !(Form.ActiveForm is MenuGridHost) 
				&& custDlg != Form.ActiveForm)
				this.customizingBarManager.HidePopups();
		}
		private void BarItemsCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			BarItem selectedItem = this.menuGridBase.SelectedItem;

			BarItems catMenuItems = this.menuGridBase.ParentItem.Items;
			for(int i = catMenuItems.Count - 1;
				i >= 0; i--)
			{
				int curIndex = this.customizingBarManager.Items.IndexOf(catMenuItems[i]);
				this.customizingBarManager.Items.Move(curIndex, i, 1);
			}

			this.RaiseBarManagerChanged((MemberDescriptor)TypeDescriptor.GetProperties((object)this.customizingBarManager)[(string)"Items"]);

			this.RefreshBarItemsList();
			
			this.menuGridBase.SelectedItem = selectedItem;
		}
		private void CustomizingItemChanged(object sender, EventArgs e)
		{
			if(this.customizingBarManager.DesignMode)
			{
				if(this.menuGridBase.SelectedItem != this.customizingBarManager.CustomizingItem)
					this.menuGridBase.SelectedItem = null;
			}
		}

		private void resetPartialMenusBtn_Click(object sender, System.EventArgs e)
		{
			if(this.customizingBarManager.MainFrameBarManager == null)
				return;

            if (MessageBox.Show(SR.GetString(SR.RecentlyUsedItemsResetConfirm, this.customizingBarManager.MainFrameBarManager),
                SR.GetString(SR.RecentlyUsedItemsResetConfirmTitle,this.customizingBarManager.MainFrameBarManager),
				MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				this.customizingBarManager.MainFrameBarManager.ResetRecentlyUsedItemsList();
                MessageBox.Show(SR.GetString(SR.NotifyRecentlyUsedItemsReset, this.customizingBarManager.MainFrameBarManager));
			}
		}

		private void resetCustomizationButton_Click(object sender, System.EventArgs e)
		{
			if(this.customizingBarManager.MainFrameBarManager == null)
				return;

            if (MessageBox.Show(SR.GetString(SR.CustomizationResetConfirm, this.customizingBarManager.MainFrameBarManager),
                SR.GetString(SR.CustomizationResetConfirmTitle, this.customizingBarManager.MainFrameBarManager),
				MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				//this.customizingBarManager.MainFrameBarManager.AutoPersistCustomization = false;
				this.customizingBarManager.MainFrameBarManager.ResetCustomization = true;
                MessageBox.Show(SR.GetString(SR.NotifyCustomizationReset, this.customizingBarManager.MainFrameBarManager), SR.GetString(SR.NotifyCustomizationResetTitle, this.customizingBarManager.MainFrameBarManager), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		/// <summary>
		/// Refresh items layout in menu grid.
		/// </summary>
		/// <param name="menuGrid"></param>
		private void MenuGridSizeRefresh( MenuGridControlCustomizable menuGrid )
		{
			if ( menuGrid != null )
			{
				int nWidth = menuGrid.Width;
				int nIconColumnWidth = menuGrid.IconColumnWidth;
				int nArrowColumnWidth = menuGrid.ArrowColumnWidth;
				int nShortcutColumnWidth = menuGrid.ShortcutColumnWidth;
				
				int NewTextColumnWidth = nWidth - nIconColumnWidth - nArrowColumnWidth - nShortcutColumnWidth;

				if( NewTextColumnWidth < c_nTEXT_WITH_ICON_WIDTH )
				{
					menuGrid.TextColumnWidth = c_nTEXT_COLUMN_WIDTH;
				}
				else
				{
					menuGrid.TextColumnWidth = NewTextColumnWidth;
				}

				menuGrid.ResizeColumns();
			}
		}

		private void gridHostPanel_SizeChanged(object sender, EventArgs e)
		{
			Panel panel = sender as Panel;
			
			if ( panel.Controls.Count > 0 )
			{
				ControlCollection controls = panel.Controls;
				Control ctrl = controls[0];
				this.MenuGridSizeRefresh( ( MenuGridControlCustomizable )ctrl );
			}
		}

		private void OnMenuGridDragDrop( object sender, DragEventArgs e )
		{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( customizingBarManager != null )
			{
				customizingBarManager.CustomDrag = false;
			}
#endif
		}

		private void gridHostPanel_MouseUp( object sender, MouseEventArgs e )
		{
			if( this.designerHost != null && e.Button == MouseButtons.Right )
				this.barItemModifyMenu.Show( this.menuGridBase, new Point( e.X, e.Y ) );
		}

		private void customizingBarManager_PropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			BarManager bm = this.customizingBarManager;

			if( "Style" == e.PropertyName || ( bm.Style == VisualStyle.Office2007 && "Office2007Theme" == e.PropertyName ) )
			{
				this.menuGridBase.Refresh();
			}
		}
	}
}
