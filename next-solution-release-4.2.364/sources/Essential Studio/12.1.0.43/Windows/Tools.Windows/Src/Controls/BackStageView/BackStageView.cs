#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;
using System.Collections;
using Syncfusion.Windows.Forms.Tools.Design;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Collections;
using System.Collections.Generic;
using Syncfusion.Runtime.InteropServices;
namespace Syncfusion.Windows.Forms
{
	#region BackStageView
	
	[ToolboxItem(true)]
	[Designer(typeof(BackStageViewDesigner))]
	[ToolboxBitmap(typeof(RibbonControlAdv), "ToolboxIcons.BackStageView.bmp"),]
	[Description("Provides an easy way to create office 2010 syle.back stage control.")]
	public class BackStageView : Component
	{
		#region Fields

		private BackStage _backStage = null;
		private Form _hostForm = null;
		private Control _hostControl;

		#endregion

		#region Ctor
		public BackStageView(IContainer container)
			: base()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(BackStageView));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			if (container != null)
				container.Add(this);
            visibleTabCollections = new RibbonControlAdvHeader.RibbonItemsCollection();
            this.BeforeBackStageOpening += delegate { };
		}
		#endregion

		#region Properties
		public Form HostForm
		{
			get { return _hostForm; }
			set
			{
				if (_hostForm != null)
					this._hostForm.SizeChanged -= new EventHandler(OnParentLayoutChanged);
			 
				_hostForm = value;

				if (_hostForm != null)
				{
					this._hostForm.SizeChanged += new EventHandler(OnParentLayoutChanged);
					this._hostForm.Load += new EventHandler(OnHostFormLoad);
				}
			}
		}

		void OnHostFormLoad(object sender, EventArgs e)
		{
			if (this._hostForm != null)
				this._hostForm.Load -= new EventHandler(OnHostFormLoad);

			this.HideBackStage();
		}

		public Control HostControl
		{
			get { return _hostControl; }
			set { _hostControl = value; }
		}

		public BackStage BackStage
		{
			get 
			{ 
				return _backStage; 
			}
			set 
			{ 
				_backStage = value;

				if(_backStage!=null)
					_backStage.Visible = false;
			}
		}
		
		public bool IsVisible
		{
			get { return this.BackStage != null && this.HostForm.Contains(this.BackStage) && this.BackStage.Visible; }
			set
			{
				if (value)
					this.ShowBackStage();
				else
					this.HideBackStage();
			}
		}

		public bool ShouldSerializeIsVisible()
		{
			return false;
		}

		#endregion
            BackStageButton DefaultButton = new BackStageButton();
            #region Events BackStageOpen
             public event EventHandler  BeforeBackStageOpening;
            #endregion
            #region Implementations
            public void ShowBackStage()
		{
			if(this.BackStage==null)
				return;
            BeforeBackStageOpening(this, new EventArgs());
            if (this.BackStage.BackStageStyle == RibbonStyle.Office2013)
            {
            }
            else
            {
                if (DefaultButton != null)
                this.BackStage.Controls.Remove(DefaultButton);
            }

			if (this.HostForm != null && !this.HostForm.Contains(this.BackStage))
			{
				this.HostForm.Controls.Add(this.BackStage);
			}
            if (this.DesignMode && this.HostForm != null)
            {
                this.BackStage.Dock = DockStyle.None;
                this._backStage.Bounds = GetBackStageBounds();
            }
            else
            {
                this.BackStage.Dock = DockStyle.None;
                this._backStage.Bounds = GetBackStageBounds();
            }
            if (!this.DesignMode && this.BackStage.TabPages.Count > 0)
            {
                this.BackStage.SelectedTab = this.BackStage.TabPages[0];
            }
            this.BackStage.SuspendLayout();
            foreach (Control ctrl in this.BackStage.Parent.Controls)
            {
                if (ctrl is RibbonControlAdv)
                {
                    RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                    (Ribbon.Header as RibbonControlAdvHeader).MinimizeButton.Enabled = false;
                    if (Ribbon.RibbonStyle == Tools.RibbonStyle.Office2010 && Ribbon.BackStageView != null)
                    {
                        this.BackStage.Visible = true;
                        foreach (ToolStripItem quickitem in Ribbon.HeaderInternal.QuickItems)
                        {
                            if (quickitem is ToolStripSplitButton)
                            {
                                if (quickitem.Enabled)
                                {
                                    if (!(splitCollecion.Contains(quickitem as ToolStripSplitButton)))
                                    {
                                        splitCollecion.Add(quickitem as ToolStripSplitButton);
                                    }
                                }
                            }
                            if (quickitem is Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable)
                            {
                                if (quickitem.Enabled)
                                {
                                    if (!(quickReflectableEnableItems.Contains(quickitem as Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable)))
                                    {
                                        quickReflectableEnableItems.Add(quickitem as Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable);
                                    }
                                }
                            }

                        }
                        foreach (ToolStripItem quickItem in Ribbon.HeaderInternal.QuickItems)
                        {
                            if (!(Ribbon.HeaderInternal.OverflowsItems.Contains(quickItem)))
                                quickItem.Enabled = false;
                        }

                        Ribbon.HeaderInternal.QuickAccessButton.Enabled = false;
                        Ribbon.HeaderInternal.QuickOverflowButton.Enabled = false;
                    }
                    if (Ribbon.RibbonStyle == RibbonStyle.Office2013)
                    {
                        if (!Ribbon.HeaderInternal.mouseclick)
                        {
                            this.BackStage.BackStageStyle = Ribbon.RibbonStyle;
                            if (Ribbon.RightToLeft != RightToLeft.Yes)
                                BackStage.Location = new Point(1, 51);
                            else
                                BackStage.Location = new Point(-1, 51);
                            Ribbon.HeaderInternal.mouseclick = true;
                        }
                        selectedtab = Ribbon.SelectedTab;
                        foreach (ToolStripTabItem tab in Ribbon.Header.MainItems)
                        {
                            if (tab.Visible)
                                VisibleTabCollections.Add(tab);
                        }
                        NativeMethods.LockWindowUpdate(Ribbon.Handle);
                        if (Ribbon.MenuButtonVisible)
                        {
                            handleMenuButtonVisibility = true;
                        }
                        else
                        {
                            handleMenuButtonVisibility = false;
                        }
                        Ribbon.MenuButtonVisible = false;
                        foreach (ToolStripTabItem tab in Ribbon.Header.MainItems)
                        {
                            tab.Visible = false;
                        }
                        if (Ribbon != null && Ribbon.RibbonStyle == Tools.RibbonStyle.Office2013)
                        {
                            Ribbon.QuickPanelVisible = false;
                        }
                        this.BackStage.Visible = true;
                        NativeMethods.LockWindowUpdate(IntPtr.Zero);
                        Ribbon.HeaderInternal.imageButton1.Visible = true;
                    }
                    if (Ribbon.RightToLeft == RightToLeft.Yes)
                        this.BackStage.Alignment = TabAlignment.Right;
                    else
                        this.BackStage.Alignment = TabAlignment.Left;
                    Ribbon.HeaderInternal.UpdateSystemButtons();
                    break;
                }
            }
            this.BackStage.ResumeLayout(false);
			this.BackStage.BringToFront();
            this.BackStage.SuspendLayout();
            for (int i = 0; i < this.BackStage.TabPages.Count; i++)
                this.BackStage.SelectedTab = this.BackStage.TabPages[i];
            this.BackStage.SelectedTab = this.BackStage.TabPages[0];
            this.BackStage.ResumeLayout(false);
        }

        static List<ToolStripSplitButton> splitCollecion = new List<ToolStripSplitButton>();
        static List<Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable> quickReflectableEnableItems = new List<Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable>();
        /// <summary>
        /// Collection of Visible ToolStripItem instances with data about items in main panel.
        /// </summary>
        private Syncfusion.Windows.Forms.Tools.RibbonControlAdvHeader.RibbonItemsCollection visibleTabCollections;
        /// <summary>
        /// List of objects storing info about visible tab items.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        internal ObservableList<ToolStripItem> VisibleTabCollections
        {
            get
            {
                return visibleTabCollections;
            }
        }
        void DefaultButton_Click(object sender, EventArgs e)
        {
            this.HideBackStage();
        }

		void OnParentLayoutChanged(object sender, EventArgs e)
		{
			if(this.BackStage!=null && this.IsVisible)
				this.BackStage.Bounds = GetBackStageBounds();
		}

		protected virtual Rectangle GetBackStageBounds()
		{
			Size sz = this.HostForm.ClientRectangle.Size;
			Point pt = Point.Empty;

			if(this.ProvideBackStageBounds!=null)
			{
				ProvideBoundsEventArgs e = new ProvideBoundsEventArgs(Point.Empty,sz);
				this.ProvideBackStageBounds(this, e);
				sz = e.Size;
				pt = e.Location;
			}

			return new Rectangle(pt, sz);
		}
        /// <summary>
        /// For checking the backstage opened or not.
        /// </summary>
        private bool BackStageOpened = false;
        private bool HandleQATVisibility = true;
        public void HideBackStage()
        {

            if (this.BackStage == null)
                return;
            if (!BackStageOpened)
            {
                foreach (Control ctrl in this.BackStage.Parent.Controls)
                {
                    if (ctrl is RibbonControlAdv)
                    {
                        if ((ctrl as RibbonControlAdv).QuickPanelVisible)
                        {
                            HandleQATVisibility = true;
                        }
                        else
                        {
                            HandleQATVisibility = false;
                        }
                        break;
                    }
                }
                BackStageOpened = true;
            }
            if (!this.DesignMode && this.BackStage.Parent!=null)
            {
                foreach (Control ctrl in this.BackStage.Parent.Controls)
                    if (ctrl is RibbonControlAdv)
                    {
                        RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                        NativeMethods.LockWindowUpdate(Ribbon.Handle);
                        if (Ribbon.RibbonStyle == RibbonStyle.Office2013 && !Ribbon.HeaderInternal.AutoHide)
                        {
                            foreach (ToolStripItem item in (Ribbon.Header as RibbonControlAdvHeader).QuickItems)
                            {
                                item.Visible = true;
                            }
                            foreach (ToolStripTabGroup obt in Ribbon.TabGroups)
                            {
                                obt.Visible = true;
                            }
                            Ribbon.MenuButtonVisible = handleMenuButtonVisibility;
                            Ribbon.ShowQuickItemsDropDownButton = true;
                        }
                        if (Ribbon.RibbonStyle == RibbonStyle.Office2010)
                        {
                            foreach (ToolStripItem mainQuickItem in Ribbon.HeaderInternal.QuickItems)
                            {
                                if (mainQuickItem is ToolStripSplitButton)
                                {
                                    if (splitCollecion.Contains(mainQuickItem as ToolStripSplitButton))
                                    {
                                        mainQuickItem.Enabled = true;
                                    }
                                }
                                else if (mainQuickItem is Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable)
                                {
                                    if (quickReflectableEnableItems.Contains(mainQuickItem as Syncfusion.Windows.Forms.Tools.QuickToolstripReflectable))
                                    {
                                        mainQuickItem.Enabled = true;
                                    }

                                }
                            }
                            Ribbon.HeaderInternal.QuickAccessButton.Enabled = true;
                            Ribbon.HeaderInternal.QuickOverflowButton.Enabled = true;
                        }
                        Ribbon.QuickPanelVisible = HandleQATVisibility;
                        if (this.BackStage.Visible)
                        {
                            this.BackStage.Visible = false;
                            if ((Ribbon.Header as RibbonControlAdvHeader).Minimizeimageindex == 1)
                            {
                                Ribbon.MinimizePanel = false;
                                (Ribbon.Header as RibbonControlAdvHeader).Minimizeimage = true;
                            }
                            else
                            {
                                if ((Ribbon.Header as RibbonControlAdvHeader).Minimizeimageindex == 0)
                                {
                                    Ribbon.MinimizePanel = true;
                                    (Ribbon.Header as RibbonControlAdvHeader).Minimizeimage = true;
                                }
                            }
                            (Ribbon.Header as RibbonControlAdvHeader).MinimizeButton.Enabled = true;
                        }
                        Ribbon.Refresh();
                        NativeMethods.LockWindowUpdate(IntPtr.Zero);
                    }
            }
            if (this.BackStage.Parent != null)
            {
                foreach (Control ctrl in this.BackStage.Parent.Controls)
                    if (ctrl != null && ctrl is RibbonControlAdv)
                    {
                        RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                        Ribbon.HeaderInternal.imageButton1.Visible = false;
                        if (Ribbon.RibbonStyle == RibbonStyle.Office2013 && !Ribbon.HeaderInternal.AutoHide && VisibleTabCollections!= null)
                        {
                            foreach (ToolStripTabItem tab in VisibleTabCollections)
                            {
                                tab.Visible = true;
                            }
                            if (this.DesignMode && !Ribbon.HeaderInternal.MenuButtonVisible)
                                Ribbon.HeaderInternal.MenuButtonVisible = handleMenuButtonVisibility;
                        }
                        if (!Ribbon.HeaderInternal.AutoHide)
                            Ribbon.QuickPanelVisible = HandleQATVisibility;
                        if (Ribbon.RibbonStyle == RibbonStyle.Office2013 && Ribbon.HeaderInternal.AutoHide && !this.DesignMode)
                        {
                            Ribbon.QuickPanelVisible = false;
                        }
                        break;
                    }
                this.BackStage.Parent.Focus();
            }
            this.BackStage.Visible = false;
        }
        internal bool handleMenuButtonVisibility = true;
        private ToolStripTabItem selectedtab = new ToolStripTabItem();
        internal ToolStripTabItem SeletedTabItem
        {
            get
            {
                return selectedtab;
            }
            set
            {
                selectedtab = value;
            }
        }
		#endregion

		#region Override

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if(this._hostForm!=null)
					this._hostForm.SizeChanged -= new EventHandler(OnParentLayoutChanged);

				if (this._backStage != null)
					this._backStage.Dispose();
			}

			base.Dispose(disposing);
		}

		#endregion

		#region event

		public event ProvideBoundsEventHandler ProvideBackStageBounds;

		#endregion
	}

	#endregion

	#region BackStage
	[ToolboxItem(false)]
	[Designer(typeof(BackStageDesigner))]
	public class BackStage : TabControlAdv
	{
        /// <summary>
        /// Supports to assign SuperAccelerator 
        /// </summary>
        private SuperAccelerator m_superAccelerator;

		#region Ctor

		public BackStage()
			: base()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(BackStage));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			base.Alignment = TabAlignment.Left;
			base.RotateTextWhenVertical = true;
			base.ItemSize = new Size(130,30);
			base.TabStyle = typeof(BackStageRenderer);
			base.BorderColor = Color.Green;
			base.BorderStyle = BorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint| ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);
            this.VisibleChanged += BackStage_VisibleChanged;
		}

        /// <summary>
        /// Assign focus once backstage is visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackStage_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
                this.Focus();
        }

        /// <summary>
        /// Gets or Sets super accelerator for backstage elements
        /// </summary>
        [DefaultValue((object)null), TypeConverter(typeof(ReferenceConverter))]
        public SuperAccelerator SuperAccelerator
        {
            get
            {
                return this.m_superAccelerator;
            }
            set
            {
                this.m_superAccelerator = value;
            }
        }


        private RibbonStyle backStageStyle = RibbonStyle.Office2010;
        internal RibbonStyle BackStageStyle
        {
            get { return backStageStyle; }
            set
            {
                if (backStageStyle != value)
                {
                    backStageStyle = value;
                    setReneder();
                    this.RendererChanged(null);
                    UpdateRenderer();
                    
                }
            }

        }
        public override Size ItemSize
        {
            get
            {
                
                return base.ItemSize;
            }
            set
            {
                using (Graphics g = this.CreateGraphics())
                {
                    if (BackStageStyle == RibbonStyle.Office2010)
                    {
                        if (g.DpiX > 120)
                            base.ItemSize = new Size(180, 36);
                        else if (g.DpiX > 96)
                            base.ItemSize = new Size(162, 36);
                        else
                            base.ItemSize = new Size(130, 30);
                    }
                }
            }
        }
        internal void setReneder()
        {
            if (this.BackStageStyle== RibbonStyle.Office2013)
            {
                base.TabStyle = typeof(BackStage2013Renderer);
                BackStage2013Renderer.RegisterTabType();
            }
            else
            {
                base.TabStyle = typeof(BackStageRenderer);
                BackStageRenderer.RegisterTabType();
            }
        }


		static BackStage()
		{
			BackStageRenderer.RegisterTabType();
		}

		#endregion

		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),Browsable(false)]
		public new bool RotateTextWhenVertical
		{
			get
			{
				return base.RotateTextWhenVertical;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),Browsable(false)]
		public new TabAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
            set
            {
                base.Alignment = value;
            }
		}

		private ToolStripEx.ColorScheme _officeColorScheme = ToolStripEx.ColorScheme.Blue;

		internal Color mColor = ColorTranslator.FromHtml("#255BB2");

        internal Color MenuColor
        {
            get { return mColor; }
            set
            {
                mColor = value;
                this.Invalidate();
                foreach (TabPageAdv tab in this.TabPages)
                {
                    tab.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets whether default highlight color should be used 
        /// </summary>
        private bool useDefaultHighlightColor = true;
        /// <summary>
        /// Gets or Sets whether default highlight color should be used 
        /// </summary>
        internal bool UseDefaultHighlightColor
        {
            get { return useDefaultHighlightColor; }
            set
            {
                useDefaultHighlightColor = value;
                this.Invalidate();
                foreach (TabPageAdv tab in this.TabPages)
                {
                    tab.Invalidate();
                }
            }
        }
		public ToolStripEx.ColorScheme OfficeColorScheme
		{
			get { return _officeColorScheme; }
			set 
			{
				if (this._officeColorScheme != value)
				{
					_officeColorScheme = value; 

					UpdateRenderer();
				}
			}
		}
        protected override void OnLocationChanged(EventArgs e)
        {
            this.SuspendLayout();
            using (Graphics g = this.CreateGraphics())
            {
                if (this.BackStageStyle == RibbonStyle.Office2013)
                {
                    if ((this.Parent as RibbonForm).WindowState == FormWindowState.Normal)
                        base.Location = new Point(1, 51);
                    else
                        base.Location = new Point(6, 53);
                }
                else
                {
                    Point backStageLocation = Point.Empty;
                    if (g.DpiX > 120)
                    {

                        if (this.Parent != null)
                        {
                            foreach (Control ctrl in this.Parent.Controls)
                                if (ctrl is RibbonControlAdv)
                                {
                                    RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                                    backStageLocation = new Point(11, (Ribbon.Header as RibbonControlAdvHeader).MenuButton.Bounds.Bottom);
                                    break;
                                }
                        }
                        base.Size = new Size(base.Size.Width - 6, base.Size.Height - 4);
                    }
                    else if (g.DpiX > 96)
                    {
                        if (this.Parent != null)
                        {
                            foreach (Control ctrl in this.Parent.Controls)
                                if (ctrl is RibbonControlAdv)
                                {
                                    RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                                    backStageLocation = new Point(8, (Ribbon.Header as RibbonControlAdvHeader).MenuButton.Bounds.Bottom);
                                    
                                            base.Size = new Size(base.Size.Width - 2, base.Size.Height - 4);
                                            break;
                                }
                        }

                    }
                    else
                    {
                        if (this.Parent != null)
                        {
                            foreach (Control ctrl in this.Parent.Controls)
                                if (ctrl is RibbonControlAdv)
                                {
                                    RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                                    backStageLocation = new Point(1, (Ribbon.Header as RibbonControlAdvHeader).MenuButton.Bounds.Bottom);
                                    if (this.Parent is RibbonForm)
                                        if (Ribbon.RibbonStyle == RibbonStyle.Office2010 && !(this.Parent as RibbonForm).CompositionEnabled)
                                        {
                                            base.Size = new Size(base.Size.Width + 2, base.Size.Height);
                                        }
                                    break;
                                }
                        }
                    }
                    base.Location = backStageLocation;
                }
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            base.OnLocationChanged(e);     
        }
		private void  UpdateRenderer()
		{
			Office2010ColorScheme colorScheme = GetOffice2010ColorScheme(this.OfficeColorScheme);

            if (this.BackStageStyle == RibbonStyle.Office2013)
            {
                foreach (BackStage2013Renderer renderer in this.Renderer.Renderers)
                {
                    renderer.ColorScheme = colorScheme;
                }
            }
            else if (this.Renderer.Renderers.Count > 0)
            {
                if (this.Renderer.Renderers[0] is BackStageRenderer)
                {
                    foreach (BackStageRenderer renderer in this.Renderer.Renderers)
                    {
                        renderer.ColorScheme = colorScheme;
                    }
                }
            }
		}

		public static Office2010ColorScheme GetOffice2010ColorScheme(ToolStripEx.ColorScheme colorScheme)
		{
			Office2010ColorScheme office2010colorScheme = Office2010ColorScheme.Blue;

			switch (colorScheme)
			{
				case ToolStripEx.ColorScheme.Blue:
					office2010colorScheme = Office2010ColorScheme.Blue;
					break;
				case ToolStripEx.ColorScheme.Black:
					office2010colorScheme = Office2010ColorScheme.Black;
					break;
				case ToolStripEx.ColorScheme.Silver:
					office2010colorScheme = Office2010ColorScheme.Silver;
					break;
			}

			return office2010colorScheme;
		}
		private Point mousePoint = new Point (0,0);
		internal Point MousePoint
		{
			get { return mousePoint; }
			set{mousePoint = value ;}
		}

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Parent != null)
            {
                foreach (Control ctrl in this.Parent.Controls)
                    if (ctrl is RibbonControlAdv)
                    {
                        RibbonControlAdv Ribbon = (ctrl as RibbonControlAdv);
                        if (!this.Visible && !IsDispose)
                        {
                            if ((Ribbon.Header as RibbonControlAdvHeader).Minimizeimageindex == 1 || this.DesignMode)
                                Ribbon.MinimizePanel = false;
                            Ribbon.BackStageView.HideBackStage();
                        }
                        break;
                    }
            }            base.OnVisibleChanged(e);
        }
		#endregion

		#region Overrides
		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.MousePoint = e.Location;
			base.OnMouseMove(e);
			this.Invalidate();
		}

        /// <summary>
        /// To hide accelerators on keypress
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.SuperAccelerator != null && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Tab) || (e.KeyCode == Keys.Enter)))
                this.SuperAccelerator.ClearAccelerator();
            
            base.OnKeyUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
           
            base.OnPaint(e);
            if (this.BackStageStyle == RibbonStyle.Office2010)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, 5);
                LinearGradientBrush LinearGradientBrush = new LinearGradientBrush(rect, ControlPaint.Light(this.MenuColor), Color.Transparent, LinearGradientMode.Vertical);
                if (this.Alignment == TabAlignment.Right)
                {
                    Point[] points = new Point[] { new Point(rect.Width, rect.Y), new Point(rect.X, rect.Y), new Point(rect.Width, 2) };
                    Pen pen = new Pen(ControlPaint.Light(this.MenuColor));
                    e.Graphics.DrawLine(pen, new Point(rect.Width, rect.Y), new Point(rect.Width - 150, rect.Y));
                    pen.Dispose();
                    e.Graphics.FillPolygon(LinearGradientBrush, points);
                    LinearGradientBrush.Dispose();
                }
                else
                {
                    Point[] points = new Point[] { new Point(rect.X, rect.Y), new Point(rect.Width, rect.Y), new Point(rect.X, 2) };
                    Pen pen = new Pen(ControlPaint.Light(this.MenuColor));
                    e.Graphics.DrawLine(pen, new Point(rect.X, rect.Y), new Point(150, rect.Y));
                    pen.Dispose();
                    e.Graphics.FillPolygon(LinearGradientBrush, points);
                    LinearGradientBrush.Dispose();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.MousePoint = new Point(0, 0);
            base.OnMouseLeave(e);
            this.Invalidate();
        }
		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);

			Size tabSize = this.ComputeTabSize();

			base.ComputeTabPanelBoundsInternal();

			RectangleF tabPanelBounds = base.GetTabPanelBounds();

			int tabsHeight = 2 + 2 * this.TabGap;
			if (this.TabCount <= 0)
				tabPanelBounds.Width = base.ItemSize.Width;
			foreach (ITabRenderer renderer in this.Renderer.Renderers)
			{
				tabsHeight += this.TabGap + (int)TabUtils.ApplyTransform(this.CreateGraphics(), this.Alignment, renderer.Bounds, false).Height;
			}
			if (this.BackStageStyle == RibbonStyle.Office2013)
            {
                int width =0;
                int height = 0;
                if (this.Parent is Form)
                {
                    width = (this.Parent as Form).Bounds.Width;
                    height = (this.Parent as Form).Bounds.Height;
                }
                base.Size = new Size(width - 3, height - 52);
            }
            int width1 = 0;
            if (BackStageStyle == RibbonStyle.Office2013)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is BackStageButton)
                    {
                        if ((ctrl as BackStageButton).AutoSize)
                        {
                            if (width1 < TextRenderer.MeasureText(ctrl.Text, ctrl.Font).Width)
                            {
                                if (ctrl.BackgroundImage != null)
                                {
                                    width1 = TextRenderer.MeasureText(ctrl.Text, ctrl.Font).Width + 40 + ctrl.BackgroundImage.Size.Width;
                                }
                                else
                                    width1 = TextRenderer.MeasureText(ctrl.Text, ctrl.Font).Width + 40;
                            }
                        }
                    }
                    if (ctrl is BackStageTab)
                    {
                        if ((ctrl as BackStageTab).AutoSize)
                        {
                            if (width1 < TextRenderer.MeasureText(ctrl.Text, ctrl.Font).Width)
                                width1 = TextRenderer.MeasureText(ctrl.Text, ctrl.Font).Width + 40;
                        }
                    }
                }
                if (width1 < 138)
                    width1 = 138;
                base.ItemSize = new Size(width1, 40);
                this.Refresh();
            }
            if(this.Parent != null)
            foreach (Control ctrl in this.Parent.Controls)
            {
                if (ctrl is RibbonControlAdv)
                {
                    if (ctrl.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                    {
                        base.Alignment = TabAlignment.Right;
                    }
                    (ctrl as RibbonControlAdv).Refresh();
                }
            }
            UpdateRenderer();
			/*int y = tabsHeight;
			int x = 0;

			foreach (Control control in this.Controls)
			{
				if (control is BackStageButton)
				{
					x = (int)(tabPanelBounds.Width - control.Width) / 2;

					control.Location = new Point(x, y);

					y += control.Height + this.TabGap;
				}
			}*/
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                if (this.TabPages != null)
                {
                    for (int i = 0; i < this.TabPages.Count; i++)
                    {
                        if (this.TabPages[i] is BackStageTab)
                        {
                            this.TabPages[i].Dispose();
                        }
                    }
                    IsDispose = true;
                    this.TabPages.Clear();
                }
			}
            this.VisibleChanged -= BackStage_VisibleChanged;
			base.Dispose(disposing);
		}
        private bool IsDispose = false;
		#endregion

		#region Implementations
		public Size ComputeTabSize()
		{
			int height = 0, width = 0;

			if (this.TabPages != null)
			{
				height = this.ItemSize.Height * base.TabPages.Count;

				width = this.ItemSize.Width;
			}

			return new Size(width, height);
		}

		internal new RectangleF GetTabPanelBounds()
		{
			if (this.TabCount <= 0)
			{
				RectangleF bgPanelBounds = base.GetTabPanelBounds();
				bgPanelBounds.Width = base.ItemSize.Width;
				return bgPanelBounds;
			}
			else 
			return base.GetTabPanelBounds();
		}

		#endregion
	}
	#endregion

	#region BackStageTab

	public class BackStageTab : TabPageAdv
	{
		public BackStageTab()
			: base()
		{
			this.BackColor = Color.White;
			this.BorderStyle = BorderStyle.None;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint| ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		}

        internal BackStage BackStage
        {
            get
            {
                return this.Parent as BackStage;
            }
        }

        /// <summary>
        /// Gets or sets accelerator key for BackstageTab
        /// </summary>
        private string m_Accelerator = string.Empty;
        public string Accelerator
        {
            get
            {
                return m_Accelerator;
            }
            set
            {
                if (m_Accelerator != value)
                {
                    m_Accelerator = value;
                    if (this.BackStage.SuperAccelerator != null)
                    {
                        this.BackStage.SuperAccelerator.SetAccelerator(this, Accelerator);
                    }
                }
            }
        }

		public BackStageTab(string label)
			: base(label)
		{
			this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint| ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		}
        internal Point mPosition = Point.Empty;
        public Point Position
        {
            get
            {
              return mPosition;
            }
            set
            {
                mPosition = value;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Parent is BackStage && (this.Parent as BackStage).BackStageStyle == RibbonStyle.Office2010)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, 5);
                LinearGradientBrush LinearGradientBrush = new LinearGradientBrush(rect, ControlPaint.Light((this.Parent as BackStage).MenuColor), Color.Transparent, LinearGradientMode.Horizontal);
                if ((this.Parent as BackStage).Alignment == TabAlignment.Right)
                {
                    Point[] points = new Point[] { new Point(rect.Width, rect.Y), new Point(rect.X-130, rect.Y), new Point(rect.Width, 2) };
                    Pen pen = new Pen(LinearGradientBrush);
                    e.Graphics.DrawLine(pen, new Point(rect.Width -130, rect.Y), new Point(rect.X, rect.Y));
                    pen.Dispose();
                    LinearGradientBrush = new LinearGradientBrush(rect, ControlPaint.Light((this.Parent as BackStage).MenuColor), Color.Transparent, LinearGradientMode.Vertical);
                    e.Graphics.FillPolygon(LinearGradientBrush, points);
                    LinearGradientBrush.Dispose();
                }
                else
                {
                    Point[] points = new Point[] { new Point(rect.X - 130, rect.Y), new Point(rect.Width, rect.Y), new Point(rect.X - 130, 2) };
                    Pen pen = new Pen(LinearGradientBrush);
                    e.Graphics.DrawLine(pen, new Point(rect.X - 130, rect.Y), new Point(rect.Width, rect.Y));
                    pen.Dispose();
                    LinearGradientBrush = new LinearGradientBrush(rect, ControlPaint.Light((this.Parent as BackStage).MenuColor), Color.Transparent, LinearGradientMode.Vertical);
                    e.Graphics.FillPolygon(LinearGradientBrush, points);
                    LinearGradientBrush.Dispose();
                }
            }
        }

	}

	#endregion

    #region DummyTabPage

    public class DummyTabPage : TabPageAdv
    {
        public DummyTabPage()
            : base()
        {
            this.Name = "DummyTabPage";
            this.Text = "DummyTabPage";
            this.Hide();
        }
    }

    #endregion

    #region BackStageTabRenderer

    public class BackStageTabRenderer : SingleLineTabPanelRenderer
    {
        public BackStageTabRenderer(ITabControl parent)
            : base(parent)
        {

        }

        public override void ComputeTabPositions(Graphics g)
        {
            if (/*tabPositionsKnown ||*/ tabRenderers.Count <= 0)
            {
                DummyTabPage tab = new DummyTabPage();
                (parent as BackStage).Controls.Add(tab);
                //return;
            }

            ITabPanelData itpdTabPanelData = this.TabPanelData;
            bool bShrinkToFit = false;

            int hiddenTabs = this.GetHiddenTabsCount();

            ITabDefaultProperties defaultProperties = TabRendererFactory.GetRegisteredExtender(itpdTabPanelData.TabStyle);
            SizeF overlappedSize = defaultProperties.GetOverlapSize(itpdTabPanelData.TabSize);


            // Compute the beginning left and bottom and height
            float fLeftRaw = 0;
            fLeftRaw = tdbounds.X + (fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX);

            float offset = fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX;

            float fTabOffsetLeft = (float)Math.Round((double)fLeftRaw);
            float fBottom = tdbounds.Bottom - 1;
            float fHeight = 0;//overlappedSize.Height;

            fHeight += TabPanelData.TabSize.Width;

            float y = 10;
            int ind = 0;
            int tabsHeight = 30;
            // iterate the renderers and update their bounds
            foreach (ITabRenderer renderer in tabRenderers)
            {
                if (this.ShouldDrawVisible(renderer.TabData))
                {
                    // Compute Preferred Size or use fixed size
                    SizeF tabSize = new SizeF(0, 0);
                    if (this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.Normal ||
                        this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.FillToRight ||
                        this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.ShrinkToFit)
                        tabSize = renderer.GetPreferredSize(g);
                    else if (this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.Fixed)
                        tabSize = this.TabPanelData.TabSize;

                    float fTabGapWidth = TabPanelData.TabGap;
                    if (bShrinkToFit)
                    {
                        int nActualTabCount = TabPanelData.TabsData.Count - hiddenTabs;
                        float fCleanAvgTabWidth = (this.tdbounds.Width - this.fpadX) / nActualTabCount;

                        float fFraction = fCleanAvgTabWidth / 10.0F;
                        if (fFraction < fTabGapWidth)
                        {
                            fTabGapWidth = fFraction;
                        }
                        float fAvgTabWidth = ((this.tdbounds.Width - offset - this.fpadX - (nActualTabCount - 1) * fTabGapWidth) / nActualTabCount);
                        tabSize.Width = fAvgTabWidth;
                    }

                    renderer.Visible = true;
                    RectangleF tabPanelBounds = (parent as BackStage).GetTabPanelBounds();

                    Control.ControlCollection controls = (parent as BackStage).Controls;

                    for (int i = ind; i < controls.Count; i++, ind++)
                    {
                        if (controls[i] is BackStageButton)
                        {
                            float x = (int)(tabPanelBounds.Width - controls[i].Width) / 2;
                            if ((parent as BackStage).Alignment == TabAlignment.Right)
                                controls[i].Location = new Point((int)(parent as BackStage).Width - (parent as BackStage).ItemSize.Width + 2, (int)y);
                            else
                                controls[i].Location = new Point((int)x, (int)y);

                            y += controls[i].Height + fTabGapWidth;
                        }
                      
                        if (controls[i] is BackStageSeparator)
                        {
                            controls[i].Visible = false;
                        }

                        if (controls[i] is BackStageTab)
                        {
                            if (controls[i].Text == renderer.TabData.Text)
                            {
                                RectangleF rectBounds = new RectangleF(y + 1, fBottom - fHeight + 1, tabSize.Width, fHeight);
                                renderer.Bounds = rectBounds;                          
                                float size =  g.MeasureString(controls[i].Text, controls[i].Font).Height;                                     
                                y += tabsHeight + fTabGapWidth +size-5;
                                Point pnt = new Point((int)rectBounds.X, (int)y);
                                (controls[i] as BackStageTab).Position = pnt;
                                                
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    renderer.TabAlignment = this.TabPanelData.Alignment;

                    fTabOffsetLeft += (tabSize.Width + fTabGapWidth);
                }
                else
                {
                    renderer.Visible = false;
                    renderer.Bounds = RectangleF.Empty;
                }
            }
        }
    }
    public class BackStage2013TabRenderer : SingleLineTabPanelRenderer
    {
        public BackStage2013TabRenderer(ITabControl parent)
            : base(parent)
        {

        }

        public override void ComputeTabPositions(Graphics g)
        {
            if (/*tabPositionsKnown ||*/ tabRenderers.Count <= 0)
                return;

            ITabPanelData itpdTabPanelData = this.TabPanelData;
            bool bShrinkToFit = false;

            int hiddenTabs = this.GetHiddenTabsCount();

            ITabDefaultProperties defaultProperties = TabRendererFactory.GetRegisteredExtender(itpdTabPanelData.TabStyle);
            SizeF overlappedSize = defaultProperties.GetOverlapSize(itpdTabPanelData.TabSize);


            // Compute the beginning left and bottom and height
            float fLeftRaw = 0;
            fLeftRaw = tdbounds.X + (fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX);

            float offset = fpadX + (float)Math.Ceiling(overlappedSize.Width / 2) - m_fScrollOffsetX;

            float fTabOffsetLeft = (float)Math.Round((double)fLeftRaw);
            float fBottom = tdbounds.Bottom - 1;
            float fHeight = 0;//overlappedSize.Height;

            fHeight += TabPanelData.TabSize.Width;

            float y = 16;
            int ind = 0;
            int tabsHeight = 30;
            // iterate the renderers and update their bounds
            foreach (ITabRenderer renderer in tabRenderers)
            {
                if (this.ShouldDrawVisible(renderer.TabData))
                {
                    // Compute Preferred Size or use fixed size
                    SizeF tabSize = new SizeF(0, 0);
                    if (this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.Normal ||
                        this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.FillToRight ||
                        this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.ShrinkToFit)
                        tabSize = renderer.GetPreferredSize(g);
                    else if (this.TabPanelData.SizeMode == Syncfusion.Windows.Forms.Tools.TabSizeMode.Fixed)
                        tabSize = this.TabPanelData.TabSize;

                    float fTabGapWidth = TabPanelData.TabGap;
                    if (bShrinkToFit)
                    {
                        int nActualTabCount = TabPanelData.TabsData.Count - hiddenTabs;
                        float fCleanAvgTabWidth = (this.tdbounds.Width - this.fpadX) / nActualTabCount;

                        float fFraction = fCleanAvgTabWidth / 10.0F;
                        if (fFraction < fTabGapWidth)
                        {
                            fTabGapWidth = fFraction;
                        }
                        float fAvgTabWidth = ((this.tdbounds.Width - offset - this.fpadX - (nActualTabCount - 1) * fTabGapWidth) / nActualTabCount);
                        tabSize.Width = fAvgTabWidth;
                    }

                    renderer.Visible = true;
                    RectangleF tabPanelBounds = (parent as BackStage).GetTabPanelBounds();

                    Control.ControlCollection controls = (parent as BackStage).Controls;

                    for (int i = ind; i < controls.Count; i++, ind++)
                    {
                        if (controls[i] is BackStageButton)
                        {
                            float x = (int)(tabPanelBounds.Width - controls[i].Width) / 2;
                            if ((parent as BackStage).Alignment == TabAlignment.Right)
                                controls[i].Location = new Point((int)(parent as BackStage).Width - (parent as BackStage).ItemSize.Width +2, (int)y);
                            else
                                controls[i].Location = new Point((int)0, (int)y);
                            y += controls[i].Height + fTabGapWidth;
                        }
                        if (controls[i] is BackStageSeparator)
                        {
                            controls[i].Visible = true;
                            float x = (int)(tabPanelBounds.Width - controls[i].Width) / 2;
                            y += controls[i].Height + 5;
                            if ((parent as BackStage).Alignment == TabAlignment.Right)
                                controls[i].Location = new Point((int)(parent as BackStage).Width - (parent as BackStage).ItemSize.Width + 2 + (((parent as BackStage).ItemSize.Width - controls[i].Width)/2), (int)y);
                            else
                                controls[i].Location = new Point((int)x, (int)y);
                            foreach (Control ctrl in (parent as BackStage).Parent.Controls)
                            {
                                if (ctrl is RibbonControlAdv)
                                {
                                    controls[i].BackColor = ControlPaint.LightLight((ctrl as RibbonControlAdv).MenuColor);
                                    break;
                                }
                            }
                            y += controls[i].Height + 5;
                        }
                        if (controls[i] is BackStageTab)
                        {
                            if (controls[i].Text == renderer.TabData.Text)
                            {
                                RectangleF rectBounds = new RectangleF(y + 1, fBottom - fHeight + 1, tabSize.Width, fHeight);
                                renderer.Bounds = rectBounds;
                                y += tabsHeight + fTabGapWidth+7 ;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    renderer.TabAlignment = this.TabPanelData.Alignment;

                    fTabOffsetLeft += (tabSize.Width + fTabGapWidth);
                }
                else
                {
                    renderer.Visible = false;
                    renderer.Bounds = RectangleF.Empty;
                }
            }
        }
    }
    
    #endregion

    #region BackStageButton
    [ToolboxItem(false),
    Designer(typeof(BackStageButtonDesigner), typeof(IDesigner))]
	public class BackStageButton : ButtonAdv
	{
		private BackStageRenderer renderer = null;
        private BackStage2013Renderer BackStage2013renderer = null;
        private ButtonRenderer m_renderer;
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
                if (this.BackStage!=null && this.BackStage.BackStageStyle == RibbonStyle.Office2013)
                    this.Size = new Size(this.BackStage .ItemSize .Width -1, 35);
                else
                    this.Size = new Size(110, 25);
			}
		}

        /// <summary>
        /// Gets or sets accelerator key for Backstagebutton
        /// </summary>
        private string m_Accelerator = string.Empty;
        public string Accelerator 
        {
            get 
            {
               return m_Accelerator;
            }
            set
            {
                if (m_Accelerator != value)
                {
                    m_Accelerator = value;

                    if (this.BackStage.SuperAccelerator != null)
                    {
                        this.BackStage.SuperAccelerator.SetAccelerator(this, Accelerator);
                    }
                }
            }
        }
        

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (this.BackStage2013Renderer != null &&this.BackStage.Parent!= null && this.BackStage.BackStageStyle == RibbonStyle.Office2013 && !this.DesignMode)
            {
                if (this.BackStage.Parent is RibbonForm)
                {
                    foreach (Control ctrl in (this.BackStage.Parent as RibbonForm).Controls)
                    {
                        if (ctrl is RibbonControlAdv)
                        {
                            if ((ctrl as RibbonControlAdv).RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                            {
                                this.Appearance = ButtonAppearance.Metro;
                                this.UseVisualStyle = true;
                                this.BorderStyleAdv = ButtonAdvBorderStyle.None;
                                this.UseVisualStyleBackColor = true;
                                this.ForeColor = Color.White;
                                BackColor = this.BackStage.MenuColor;
                                this.IsBackStageButton = true;
                            }
                            break;
                        }
                    }
                }
            }
            else if (this.Renderer != null && this.BackStage.BackStageStyle == RibbonStyle.Office2010 && !this.DesignMode && this.BackStage.Parent!= null)
            {
                if (this.BackStage.Parent is RibbonForm)
                {
                    foreach (Control ctrl in (this.BackStage.Parent as RibbonForm).Controls)
                    {
                        if (ctrl is RibbonControlAdv)
                        {
                            if ((ctrl as RibbonControlAdv).RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                            {
                                this.Appearance = ButtonAppearance.Office2010;
                                this.UseVisualStyle = true;
                                this.BorderStyleAdv = ButtonAdvBorderStyle.None;
                                this.UseVisualStyleBackColor = true;
                                this.ForeColor = Color.Black;
                                BackColor = ControlPaint.LightLight(Color.FromArgb(25, this.BackStage.MenuColor));
                                this.IsBackStageButton = true;
                            }
                        }
                        break;
                    }
                }
            }
        }
        public BackStage2013Renderer BackStage2013Renderer
        {
            get
            {
                    if (this.BackStage != null && this.BackStage.BackStageStyle ==RibbonStyle.Office2013)
                    {
                        this.BackStage2013renderer = new BackStage2013Renderer(this.BackStage, this.BackStage.Renderer);
                    }

                return BackStage2013renderer;
            }
        }
		public BackStageRenderer Renderer
		{
			get 
			{
				if (this.renderer == null)
				{
                    if (this.BackStage != null && this.BackStage.BackStageStyle == RibbonStyle.Office2010)
					{
						this.renderer = new BackStageRenderer(this.BackStage, this.BackStage.Renderer);
					}
				}

				return renderer; 
			}
		}

		public BackStageButton()
			: base()
		{
			this.SetStyle(ControlStyles.Opaque, true);
			this.BackColor = Color.Transparent;
            if (this.Renderer != null && this.BackStage.BackStageStyle == RibbonStyle.Office2010)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    if (g.DpiX > 120)
                    {
                        this.Width = 166;
                        this.Height = 35;
                    }
                    else if (g.DpiX > 96)
                    {
                        this.Width = 146;
                        this.Height = 30;
                    }
                    else
                    {
                        this.Width = 110;
                        this.Height = 25;
                    }
                }
            }
		}

        internal BackStage BackStage
        {
            get
            {
                return this.Parent as BackStage;
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.BackStage !=null && this.BackStage.BackStageStyle == RibbonStyle.Office2010)
            {
                using (Graphics g = CreateGraphics())
                {
                    if (g.DpiX > 120 && this.Size != new Size(166, 35))
                    {
                        this.Size = new Size(110, 25);
                    }
                    else if (g.DpiX > 96 && this.Size != new Size(146, 30))
                    {
                        this.Size = new Size(110, 25);
                    }
                    else if(g.DpiX == 96 && this.Size != new Size(110, 25))
                    {
                        this.Size = new Size(110, 25);
                    }
                }
            }
        }
		protected override void OnPaint(PaintEventArgs e)
		{
            if (this.Renderer != null && this.BackStage.BackStageStyle == RibbonStyle.Office2010)
            {
                this.Renderer.ColorScheme = BackStage.GetOffice2010ColorScheme(this.BackStage.OfficeColorScheme);
                if (this.BackStage.Alignment == TabAlignment.Right)
                    base.OnPaint(e);
                else
                    this.Renderer.DrawButtonBackground(e, this);
            }
            else if (this.BackStage2013Renderer != null && this.BackStage.BackStageStyle == RibbonStyle.Office2013)
            {
                this.Width = this.BackStage.ItemSize.Width - 1;
                this.Height = 38;
                this.BackStage2013Renderer.DrawButtonBackground(e, this);
            }
            if (this.BackStage.Alignment == TabAlignment.Right && this.BackStage.BackStageStyle == RibbonStyle.Office2013)
            {
                if (this.State == ButtonAdvState.MouseOver)
                {
                    if ((this.Parent as BackStage).MenuColor != ColorTranslator.FromHtml("#0072C6"))
                    {
                        BackColor = ControlPaint.Light((this.Parent as BackStage).MenuColor);
                    }
                    else
                    {
                        BackColor = ColorTranslator.FromHtml("#0067B0");
                    }
                }
                else
                {
                    this.BackStage2013Renderer.DrawButtonBackground(e, this);
                    BackColor = this.BackStage.MenuColor;
                }
                if (this.Parent != null && this.Parent is BackStage && (this.Parent as BackStage).RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                {
                    this.IsBackStageButton = true;
                }
                base.OnPaint(e);
                if (this.Parent != null && this.Parent is BackStage && (this.Parent as BackStage).RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                {
                    this.IsBackStageButton = false;
                    this.Renderer.DrawButtonBackground(e, this);
                }

            }
            else if (this.BackStage.Alignment == TabAlignment.Right && this.BackStage.BackStageStyle == RibbonStyle.Office2010)
            {
                if (this.State == ButtonAdvState.MouseOver)
                {
                    BackColor =  this.BackStage.MenuColor;
                }
                else
                {
                    BackColor = ControlPaint.LightLight(this.BackStage.MenuColor);
                }
                base.OnPaint(e);
            }
            Rectangle rect = new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height);
            if (this.BackStage.BackStageStyle != RibbonStyle.Office2013)
            {
                if (this.Focused && this.KeepFocusRectangle)
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect);
            }
		}
 
	}
    #endregion

    #region BackStageSeparator

    [ToolboxItem(false),
 Designer(typeof(BackStageButtonDesigner), typeof(IDesigner))]
    public class BackStageSeparator : Control
    {
        public BackStageSeparator()
        {
            this.AutoSize = false;
            this.Width = 100;
            this.Height = 2;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Width = 100;
            this.Height = 1;
        }
    }

    #endregion

    #region BackStageButtonDesigner

    public class BackStageButtonDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.None;
            }
        }
    }

    #endregion

    #region BackStageViewDesigner
    public class BackStageViewDesigner : ComponentDesigner
	{
		#region Fields
		private DesignerVerbCollection verbs = null;
		BackStageView backStageView = null;
		IDesignerHost iDesignerHost = null;
		ISelectionService iSelectionService = null;

		public ISelectionService SelectionService
		{
			get 
			{ 
				return iSelectionService; 
			}
		}
		#endregion

		#region Properties

		public BackStageView BackStageView
		{
			get
			{
				if (this.backStageView == null)
					this.backStageView = this.Component as BackStageView;

				return backStageView;
			}
		}

		public IDesignerHost DesignerHost
		{
			get
			{
				if (this.iDesignerHost == null)
					this.iDesignerHost = this.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

				return iDesignerHost;
			}
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.verbs = new DesignerVerbCollection();

					this.verbs.Add(new DesignerVerb("Show BackStage", this.OnShowBackStage));
					this.verbs.Add(new DesignerVerb("Hide BackStage", this.OnHideBackStage));
				}

				return this.verbs;
			}
		}

		#endregion

		#region Override

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			this.backStageView = component as BackStageView;

			this.iDesignerHost = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));

			this.iSelectionService = (ISelectionService)component.Site.GetService(typeof(ISelectionService));

			if (backStageView != null)
			{
				IDesignerHost host = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

				if (host != null)
				{
					this.BackStageView.HostForm = host.RootComponent as Form;
				}
			}
            this.OnHideBackStage(this, new EventArgs());
		}

		#endregion

		#region Implementations

		private void OnShowBackStage(object sender, EventArgs e)
		{
			if (this.BackStageView.BackStage == null)
			{
				BackStage backStage = (BackStage)this.DesignerHost.CreateComponent(typeof(BackStage));

				this.BackStageView.BackStage = backStage;

				this.BackStageView.HostForm.Controls.Add(backStage);

				DummyTabPage bsTab23 = new DummyTabPage();

				backStage.TabPages.Add(bsTab23);
                foreach (Control ctrl in this.BackStageView.BackStage.Parent.Controls)
                {
                    if (ctrl is RibbonControlAdv)
                    {
                        switch ((ctrl as RibbonControlAdv).RibbonStyle)
                        {
                            case RibbonStyle.Office2013:
                                this.BackStageView.BackStage.BackStageStyle = RibbonStyle.Office2013;
                                break;
                            default:
                                this.BackStageView.BackStage.BackStageStyle = RibbonStyle.Office2010;
                                break;
                        }
                        break;
                    }
                }
			}

			this.BackStageView.ShowBackStage();

			this.iSelectionService.SetSelectedComponents(new Component[] { this.BackStageView.BackStage});
		}

		private void OnHideBackStage(object sender, EventArgs e)
		{
			this.BackStageView.HideBackStage();
		}

		#endregion
	}
	#endregion

	#region BackStageDesigner

	public class BackStageDesigner : ParentControlDesigner
	{
		#region Fileds

		private DesignerVerbCollection _verbs = null;
		private BackStage _backStage = null;
		private IDesignerHost _iDesignerHost = null;
		private ISelectionService _iSelectionService = null;

		#endregion

		#region Properties

		public ISelectionService SelectionService
		{
			get 
			{
				if (this._iSelectionService == null)
					this._iSelectionService = (ISelectionService)this.Component.Site.GetService(typeof(ISelectionService));

				return _iSelectionService; 
			}
		}

		public BackStage BackStage
		{
			get 
			{
				if (this._backStage == null)
					this._backStage = this.Component as BackStage;
 
				return _backStage; 
			}
		}

		public override SelectionRules SelectionRules
		{
			get
			{
				return SelectionRules.None;
			}
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this._verbs == null)
				{
                    this._verbs = new DesignerVerbCollection(new DesignerVerb[]
					                  {
					                      new DesignerVerb("Add BackStage Tab", this.OnAddBackStageTab),
					                      new DesignerVerb("Add BackStage Button", this.OnAddBackStageButton),
                                          new DesignerVerb("Add BackStage Separator", this.OnAddBackSeparator)
					                  });
				}

				return this._verbs;
			}
		}

		public IDesignerHost DesignerHost
		{
			get
			{
				if (this._iDesignerHost == null)
					this._iDesignerHost = (IDesignerHost)this.Component.Site.GetService(typeof(IDesignerHost));

				return _iDesignerHost;
			}
		}

		#endregion

		#region Ctor

		public BackStageDesigner()
			: base()
		{ }

		#endregion

		#region Overrides

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			this._backStage = this.Component as BackStage;

			this.SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			this.BackStage.SelectedIndexChanged += new EventHandler(OnBackStageSelectedTabChanged);
		}

		protected override /*ControlDesigner*/ bool GetHitTest(Point point)
		{
			if (this.BackStage.TabPages.Count > 1)
			{
				point = this.BackStage.PointToClient(point);
				return ((this.BackStage.Renderer.HitTestTabs(point, false) != -1) || this.BackStage.GetHitTestScroll(point));
			}
			else
				return false;
		}

		#region Property Filters

		protected override void PreFilterProperties(IDictionary properties)
		{
			if (properties.Contains("Alignment"))
				properties.Remove("Alignment");

			if (properties.Contains("ImageAlignmentR"))
				properties.Remove("ImageAlignmentR");

			if (properties.Contains("KeepSelectedTabInFrontRow"))
				properties.Remove("KeepSelectedTabInFrontRow");

			if (properties.Contains("LabelEdit"))
				properties.Remove("LabelEdit");

			if (properties.Contains("LevelTextAndImage"))
				properties.Remove("LevelTextAndImage");

			if (properties.Contains("Multiline"))
				properties.Remove("Multiline");

			if (properties.Contains("MultilineText"))
				properties.Remove("MultilineText");

			if (properties.Contains("PersistTabState"))
				properties.Remove("PersistTabState");

			if (properties.Contains("RotateTabsWhenRTL"))
				properties.Remove("RotateTabsWhenRTL");

			if (properties.Contains("RotateTextWhenVertical"))
				properties.Remove("RotateTextWhenVertical");

			if (properties.Contains("SelectedIndex"))
				properties.Remove("SelectedIndex");

			if (properties.Contains("ShowTabCloseButton"))
				properties.Remove("ShowTabCloseButton");

			if (properties.Contains("TabStyle"))
				properties.Remove("TabStyle");

			if (properties.Contains("TabPrimitivesHost"))
				properties.Remove("TabPrimitivesHost");

			if (properties.Contains("UserMoveTabs"))
				properties.Remove("UserMoveTabs");

			if (properties.Contains("VerticalAlignment"))
				properties.Remove("VerticalAlignment");
		}

		protected override void PostFilterProperties(IDictionary properties)
		{
			if (properties.Contains("Alignment"))
				properties.Remove("Alignment");

			if (properties.Contains("ImageAlignmentR"))
				properties.Remove("ImageAlignmentR");

			if (properties.Contains("KeepSelectedTabInFrontRow"))
				properties.Remove("KeepSelectedTabInFrontRow");

			if (properties.Contains("LabelEdit"))
				properties.Remove("LabelEdit");

			if (properties.Contains("LevelTextAndImage"))
				properties.Remove("LevelTextAndImage");

			if (properties.Contains("Multiline"))
				properties.Remove("Multiline");

			if (properties.Contains("MultilineText"))
				properties.Remove("MultilineText");

			if (properties.Contains("PersistTabState"))
				properties.Remove("PersistTabState");

			if (properties.Contains("RotateTabsWhenRTL"))
				properties.Remove("RotateTabsWhenRTL");

			if (properties.Contains("RotateTextWhenVertical"))
				properties.Remove("RotateTextWhenVertical");

			if (properties.Contains("SelectedIndex"))
				properties.Remove("SelectedIndex");

			if (properties.Contains("ShowTabCloseButton"))
				properties.Remove("ShowTabCloseButton");

			if (properties.Contains("TabStyle"))
				properties.Remove("TabStyle");

			if (properties.Contains("TabPrimitivesHost"))
				properties.Remove("TabPrimitivesHost");

			if (properties.Contains("UserMoveTabs"))
				properties.Remove("UserMoveTabs");

			if (properties.Contains("VerticalAlignment"))
				properties.Remove("VerticalAlignment");

		}

		#endregion

		#endregion

		#region Implementation

		void OnBackStageSelectedTabChanged(object sender, EventArgs e)
		{
			ICollection componentsSelected = this.SelectionService.GetSelectedComponents();

			foreach (Component component in componentsSelected)
			{
				BackStageTab bsTab = (BackStageTab)TabControlAdvDesigner.GetTabPageOfComponent(component);
				if (bsTab != null && bsTab.Parent == this.BackStage && this.BackStage.SelectedTab != bsTab)
				{
					this.BackStage.SelectedTab = bsTab;

					this.SelectionService.SetSelectedComponents(new Component[] { bsTab });
				}
			}

			/*for (int i = 0; i < this.BackStage.TabPages.Count; i++)
			{
				if (this.BackStage.TabPages[i] is BackStageTab)
				{
					this.BackStage.TabPages[i].SendToBack();
				}
			}*/
		}

		private void OnAddBackStageTab(object sender, EventArgs e)
		{
			BackStageTab bsTab = this.DesignerHost.CreateComponent(typeof(BackStageTab)) as BackStageTab;

			if (bsTab != null)
			{
				PropertyDescriptor pDesc = TypeDescriptor.GetProperties((object)bsTab)[(string)@"Name"];

				pDesc = TypeDescriptor.GetProperties((object)bsTab)[(string)@"Name"];
				if (pDesc != null)
				{
					if (pDesc.PropertyType == typeof(System.String))
					{
						string name = (string)(System.String)pDesc.GetValue((object)bsTab);
						bsTab.Text = name;
					}
				}

				this.BackStage.TabPages.Add(bsTab);

                this.BackStage.SelectedTab = this.BackStage.TabPages[this.BackStage.TabPages.Count - 1];

				this.BackStage.PerformLayout();
			}
		}
        private void OnAddBackSeparator(object sender, EventArgs e)
        {

            BackStageSeparator bsSep = this.DesignerHost.CreateComponent(typeof(BackStageSeparator)) as BackStageSeparator;

			if (bsSep != null)
			{
                PropertyDescriptor pDesc = TypeDescriptor.GetProperties((object)bsSep)[(string)@"Name"];

                pDesc = TypeDescriptor.GetProperties((object)bsSep)[(string)@"Name"];
				if (pDesc != null)
				{
					if (pDesc.PropertyType == typeof(System.String))
					{
                        string name = (string)(System.String)pDesc.GetValue((object)bsSep);
                        bsSep.Text = name;
					}
				}

                this.BackStage.Controls.Add(bsSep);
			}

        }
		private void OnAddBackStageButton(object sender, EventArgs e)
		{
			BackStageButton bsBtn = this.DesignerHost.CreateComponent(typeof(BackStageButton)) as BackStageButton;

			if (bsBtn != null)
			{
				PropertyDescriptor pDesc = TypeDescriptor.GetProperties((object)bsBtn)[(string)@"Name"];

				pDesc = TypeDescriptor.GetProperties((object)bsBtn)[(string)@"Name"];
				if (pDesc != null)
				{
					if (pDesc.PropertyType == typeof(System.String))
					{
						string name = (string)(System.String)pDesc.GetValue((object)bsBtn);
						bsBtn.Text = name;
					}
				}

				this.BackStage.Controls.Add(bsBtn);
			}
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			ICollection componentsSelected = this.SelectionService.GetSelectedComponents();

			foreach (Component component in componentsSelected)
			{
				BackStageTab bsTab = (BackStageTab) TabControlAdvDesigner.GetTabPageOfComponent(component);
				if (bsTab != null && bsTab.Parent == this.BackStage)
				{
					this.BackStage.SelectedTab = bsTab;
				}
			}
		}

		#endregion
	}

	#endregion

	#region BackStageRenderer

	public class BackStageRenderer : TabRenderer2D
	{
		#region constants
		private const string DEF_RENDERER_NAME = "BackStage";
		private const int DEF_CORNER_RADIUS = 2;
		private const int DEF_OVERLAP_HEIGHT = 3;
		protected internal const int DEF_SELECTION_LINE_WIDTH = 5;
		const int DEF_IMG_SIZE = 15;
		const int IMG_PADDING = 3;
		#endregion

		#region fields

		private static BackStageRendererProperty _tabPanelProperty;
		private readonly Rectangle _lastCloseButtonBounds = Rectangle.Empty;
		Hashtable _hsColorTable = new Hashtable();

		#endregion

		#region Ctor

		public BackStageRenderer(ITabControl tabControl, ITabPanelRenderer tabPanelRenderer)
			: base(tabControl, tabPanelRenderer)
		{
			this.ColorScheme = Office2010ColorScheme.Blue;
		}

		static BackStageRenderer()
		{
			_tabPanelProperty = new BackStageRendererProperty();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(BackStageRenderer), _tabPanelProperty);
		}
		#endregion

		#region properties

		private Office2010ColorScheme _colorScheme;
		public Office2010ColorScheme ColorScheme
		{
			get { return _colorScheme; }
			set { _colorScheme = value; }
		}

		public Office2010ColorTable ColorTable
		{
			get
			{
				Office2010ColorTable colorTable = _hsColorTable[this.ColorScheme] as Office2010ColorTable;

				if (colorTable == null)
					_hsColorTable[this.ColorScheme] = colorTable = new Office2010ColorTable(this.ColorScheme);

				return colorTable;
			}
		}

		public static new string TabStyleName
		{
			get
			{
				return DEF_RENDERER_NAME;
			}
		}

		public static new BackStageRendererProperty TabPanelPropertyExtender
		{
			get
			{
				return _tabPanelProperty;
			}
		}

		public new Rectangle CloseButtonBounds
		{
			get
			{
				return _lastCloseButtonBounds;
			}
		}
		#endregion

		#region Implementation

		public static new void RegisterTabType()
		{
			_tabPanelProperty = new BackStageRendererProperty();
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(BackStageRenderer), _tabPanelProperty);
		}

		#endregion

		#region Overriedes

        protected override void DrawText(Graphics g, RectangleF rectText, string text, StringFormat format, DrawTabEventArgs e)
        {
            if ((this.parent as BackStage).Alignment == System.Windows.Forms.TabAlignment.Right)
            {
                rectText.X = rectText.X - 50;
            }
            base.DrawText(g, rectText, text, format, e);
        }
		protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
		{
			if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
			{
				Graphics g = drawItemInfo.Graphics;
				RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
				base.ApplyTransform(g);
				Color mColor = (this.parent as BackStage).MenuColor;
				if (this.IsSelectedState(drawItemInfo.State))
				{
					RectangleF upperRect = new RectangleF(bounds.Location, new SizeF(bounds.Width, 1 + (bounds.Height / 2)));
					RectangleF lowerRect = new RectangleF(upperRect.Left, upperRect.Bottom, bounds.Width, 1 + (bounds.Height / 2));
					RectangleF leftRect = new RectangleF(bounds.Location, new SizeF(bounds.Width/4, bounds.Height));
					RectangleF rightRect = new RectangleF(bounds.Left + bounds.Width / 2,bounds.Location.X , bounds.Width/2, 1 + bounds.Height);

					Color gradientBegin = ControlPaint.LightLight (mColor); ;
					Color gradientEnd = mColor ;
					Rectangle marginRect = new Rectangle(0, (int)bounds.Height - 7, 1000, 30);
					using (LinearGradientBrush brush = new LinearGradientBrush(marginRect, Color.Transparent,ColorTable.BackStagePageAdornerBackgroundGradient, LinearGradientMode.Vertical))
					{
						g.FillRectangle(brush, marginRect);
					}
					using (LinearGradientBrush brush = new LinearGradientBrush(upperRect, gradientEnd, gradientBegin, LinearGradientMode.Vertical))
					{
						g.FillRectangle(brush, upperRect);
					}
					using (LinearGradientBrush brush = new LinearGradientBrush(lowerRect,gradientBegin, gradientEnd,  LinearGradientMode.Vertical  ))
					{
						g.FillRectangle(brush, lowerRect);
					}

					gradientBegin = Color.Transparent;// ColorTable.BackStagePageAdornerBackgroundGradient;
					// gradientEnd = Color.Transparent;

					using (LinearGradientBrush brush = new LinearGradientBrush(leftRect, gradientEnd, gradientBegin, LinearGradientMode.Horizontal))
					{
						g.FillRectangle(brush, leftRect);
					}

					Pen pen = new Pen( ((gradientEnd)));

					g.DrawRectangle(pen, bounds.X,bounds.Y,bounds.Width ,bounds.Height );
					pen.Dispose();

					Pen margin = new Pen(ColorTable.BackStagePageAdornerBackgroundGradient);
					g.DrawLine(margin, new Point(0, (int)bounds.Y), new Point(1000, (int)bounds.Y));
					margin = new Pen(Color.FromArgb(50, ColorTable.BackStagePageAdornerBackgroundGradient));

					g.DrawLine(margin, new Point(0, (int)bounds.Height - 2), new Point(1000, (int)bounds.Height - 2));

					margin.Dispose();
					using (GraphicsPath path = new GraphicsPath())
					{
						path.AddRectangle(bounds);

						const int sz = 15;
						float offset = (bounds.Width - sz) / 2;

						RectangleF rcTri = new RectangleF(bounds.Left + offset, bounds.Bottom - sz, sz, sz);

						using (Brush brush = new SolidBrush(ColorTable.BackStagePageTriangleClip))
						{
							DrawTriangle(g, rcTri, brush);
						}
					}

				}
				else if (this.IsHotLightState(drawItemInfo.State))
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(25, ColorTable.BackStageHotTrackColor)))
					{
						g.FillRectangle(brush, bounds);
					}

					using (Pen pen = new Pen(ColorTable.BackStageHotTrackColor))
					{
						g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
						g.DrawLine(pen, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
					}
				}
				else
				{
					if (drawItemInfo.Bounds.Contains((this.parent as BackStage).MousePoint))
					{
						SolidBrush brush = new SolidBrush(Color.FromArgb(25, mColor));
						g.FillRectangle(brush, bounds);
						brush.Dispose();

						Pen pen = new Pen(mColor);
						g.DrawLine (pen, new PointF (bounds.X ,bounds.Y) ,new PointF (bounds.X  ,bounds.Y + bounds.Height  ));
						g.DrawLine(pen, new PointF(bounds.X + bounds.Width, bounds.Y), new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height));
						pen.Dispose();
					}
				}

				g.ResetTransform();
			}
		}

		public void DrawButtonBackground(PaintEventArgs e, BackStageButton button)
		{
			RectangleF bgPanelBounds = ((BackStage)this.TabControl).GetTabPanelBounds();
			bgPanelBounds.Location = new PointF(-button.Left, -button.Top);
            if (bgPanelBounds.Height <= 0)
            {
                bgPanelBounds.Height = 1;
            }
			Rectangle rcBounds = new Rectangle(Point.Empty, button.Size);

			Color gradientBegin = ColorTable.BackStagePanelGradientBegin;
			Color gradientEnd = ColorTable.BackStagePanelGradientEnd;

			using (Brush brush = new LinearGradientBrush(bgPanelBounds, gradientBegin, gradientEnd, LinearGradientMode.Vertical))
			{
				e.Graphics.FillRectangle(brush, bgPanelBounds);
			}
			Color mColor = (this.parent as BackStage).MenuColor ;
			if ( button.Enabled && (button.State & ButtonAdvState.MouseOver) > 0)
			{
				using (Brush brush = new SolidBrush(Color.FromArgb(25, mColor)))
				{
					e.Graphics.FillRectangle(brush, rcBounds);
				}

				using (Pen pen = new Pen(mColor ))
				{
					Point[] polygon = RendererUtils.GetRoundedPolygon(rcBounds, 2);

					e.Graphics.DrawPolygon(pen, polygon);
				}
			}

			Rectangle rcImage = Rectangle.Empty;

			if (button.Image != null)
			{
				rcImage= new Rectangle(IMG_PADDING, (rcBounds.Height - DEF_IMG_SIZE) / 2, DEF_IMG_SIZE, DEF_IMG_SIZE);
				if(button.Enabled)
					e.Graphics.DrawImage(button.Image, rcImage );
				else
					ControlPaint.DrawImageDisabled(e.Graphics, button.Image, rcImage.Left , rcImage.Top,Color.Gray);
			}

			Rectangle rcText = new Rectangle(
				rcImage.Right + IMG_PADDING,
				(rcBounds.Height - TextRenderer.MeasureText(button.Text,button.Font,Size.Empty,TextFormatFlags.EndEllipsis).Height)/2,
				rcBounds.Width - rcImage.Right - IMG_PADDING,
				rcBounds.Height);
			Color foreColor = button.ForeColor;
			if (!button.Enabled)
				foreColor = Color.Gray;
			if ((this.parent as BackStage).OfficeColorScheme == ToolStripEx.ColorScheme.Black)
				foreColor = Color.White;
            if (this.TabControl != null && this.TabControl is BackStage)
            {
                if ((this.TabControl as BackStage).RightToLeft == RightToLeft.Yes)
                {
                    rcText.X = ((this.parent as BackStage).ItemSize.Width - TextRenderer.MeasureText(button.Text, button.Font).Width) - 15;
                }
            }
            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, rcText, foreColor, Color.Transparent,
                                 TextFormatFlags.EndEllipsis);
		}

		public static void DrawTriangle(Graphics g, RectangleF bounds, Brush brush)
		{
			PointF[] points = new PointF[]
				{
					new PointF( bounds.X + bounds.Width / 2, bounds.Y +5),
					new PointF( bounds.X - 2, bounds.Bottom ),
					new PointF( bounds.Right +2, bounds.Bottom  )
				};

			using (GraphicsPath gp = new GraphicsPath())
			{
				gp.AddLines(points);
				gp.CloseAllFigures();

				using (Region rgn = new Region(gp))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.FillPolygon(brush, points);
					//g.FillRegion(brush, rgn);
				}
			}
		}

		protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
		{
			
		}

		protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
		{
		}
		#endregion

	}
    public class BackStage2013Renderer : TabRenderer2D
    {
        #region constants
        private const string DEF_RENDERER_NAME = "BackStage";
        private const int DEF_CORNER_RADIUS = 2;
        private const int DEF_OVERLAP_HEIGHT = 3;
        protected internal const int DEF_SELECTION_LINE_WIDTH = 5;
        const int DEF_IMG_SIZE = 15;
        const int IMG_PADDING = 3;
        #endregion

        #region fields

        private static BackStageRendererProperty _tabPanelProperty;
        private readonly Rectangle _lastCloseButtonBounds = Rectangle.Empty;
        Hashtable _hsColorTable = new Hashtable();

        #endregion

        #region Ctor

        public BackStage2013Renderer(ITabControl tabControl, ITabPanelRenderer tabPanelRenderer)
            : base(tabControl, tabPanelRenderer)
        {
            this.ColorScheme = Office2010ColorScheme.Blue;
        }

        static BackStage2013Renderer()
        {
            _tabPanelProperty = new BackStageRendererProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(BackStage2013Renderer), _tabPanelProperty);
        }
        #endregion

        #region properties

        private Office2010ColorScheme _colorScheme;
        public Office2010ColorScheme ColorScheme
        {
            get { return _colorScheme; }
            set { _colorScheme = value; }
        }

        public Office2010ColorTable ColorTable
        {
            get
            {
                Office2010ColorTable colorTable = _hsColorTable[this.ColorScheme] as Office2010ColorTable;

                if (colorTable == null)
                    _hsColorTable[this.ColorScheme] = colorTable = new Office2010ColorTable(this.ColorScheme);

                return colorTable;
            }
        }

        public static new string TabStyleName
        {
            get
            {
                return DEF_RENDERER_NAME;
            }
        }

        public static new BackStageRendererProperty TabPanelPropertyExtender
        {
            get
            {
                return _tabPanelProperty;
            }
        }

        public new Rectangle CloseButtonBounds
        {
            get
            {
                return _lastCloseButtonBounds;
            }
        }
        #endregion

        #region Implementation

        public static new void RegisterTabType()
        {
            _tabPanelProperty = new BackStageRendererProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(BackStage2013Renderer), _tabPanelProperty);
        }

        #endregion

        #region Overriedes
        /// <summary>
        /// BackStageTab Selection color
        /// </summary>
        Color SelectedColor = Color.FromArgb(200, ColorTranslator.FromHtml("#2A8DD4"));
        /// <summary>
        /// BackStagerTab Highlight Color
        /// </summary>
        Color HighlightColor = Color.FromArgb(200, ColorTranslator.FromHtml("#0067B0"));
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            if ((this.parent as BackStage).UseDefaultHighlightColor)
            {
                SelectedColor = ColorTranslator.FromHtml("#2a8dd4");
                HighlightColor = ColorTranslator.FromHtml("#0067b0");
            }
            else if ((this.parent as BackStage).MenuColor != ColorTranslator.FromHtml("#0072C6"))
            {
                SelectedColor = ControlPaint.LightLight((this.parent as BackStage).MenuColor);
                HighlightColor = ControlPaint.Light((this.parent as BackStage).MenuColor);
            }
            else
            {
                SelectedColor = Color.FromArgb(200, ColorTranslator.FromHtml("#2A8DD4"));
                HighlightColor = Color.FromArgb(200, ColorTranslator.FromHtml("#0067B0"));
            }
            if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
            {
                Graphics g = drawItemInfo.Graphics;
                RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
                base.ApplyTransform(g);
                Color mColor = Color.Blue;
                if (this.IsSelectedState(drawItemInfo.State))
                {
                    RectangleF upperRect = new RectangleF(bounds.Location, new SizeF(bounds.Width, 1 + (bounds.Height / 2)));
                    RectangleF lowerRect = new RectangleF(upperRect.Left, upperRect.Bottom, bounds.Width, 1 + (bounds.Height / 2));
                    RectangleF leftRect = new RectangleF(bounds.Location, new SizeF(bounds.Width / 4, bounds.Height));
                    RectangleF rightRect = new RectangleF(bounds.Left + bounds.Width / 2, bounds.Location.X, bounds.Width / 2, 1 + bounds.Height);

                    Color gradientBegin = ControlPaint.LightLight(mColor); ;
                    Color gradientEnd = mColor;

                    using (SolidBrush brush = new SolidBrush(SelectedColor))
                    {
                        g.FillRectangle(brush, upperRect);
                    }
                    using (SolidBrush brush = new SolidBrush(SelectedColor))
                    {
                        g.FillRectangle(brush, lowerRect);
                    }

                    gradientBegin = Color.Transparent;
                    if (drawItemInfo.Bounds.Contains((this.parent as BackStage).MousePoint))
                    {
                        SolidBrush brush = new SolidBrush(HighlightColor);
                        g.FillRectangle(brush, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height + 5));
                        brush.Dispose();
                    }
                    else
                    {
                        using (SolidBrush brush = new SolidBrush(SelectedColor))
                        {
                            g.FillRectangle(brush, leftRect);
                        }
                    }

                }
                else if (this.IsHotLightState(drawItemInfo.State))
                {
                    using (Brush brush = new SolidBrush(Color.Yellow))
                    {
                        g.FillRectangle(brush, bounds);
                    }

                    using (Pen pen = new Pen(Color.Yellow))
                    {
                        g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                        g.DrawLine(pen, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                    }
                }
                else
                {
                    if (drawItemInfo.Bounds.Contains((this.parent as BackStage).MousePoint))
                    {
                        SolidBrush brush = new SolidBrush(HighlightColor);
                        g.FillRectangle(brush, new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height + 5));
                        brush.Dispose();
                    }
                }

                g.ResetTransform();
            }
        }

        public void DrawButtonBackground(PaintEventArgs e, BackStageButton button)
        {
            if ((this.parent as BackStage).UseDefaultHighlightColor)
            {
                SelectedColor = ColorTranslator.FromHtml("#0067b0");
                HighlightColor = ColorTranslator.FromHtml("#0067b0");
            }
            else if ((this.parent as BackStage).MenuColor != ColorTranslator.FromHtml("#0072C6"))
            {
                SelectedColor = ControlPaint.LightLight((this.parent as BackStage).MenuColor);
                HighlightColor = ControlPaint.Light((this.parent as BackStage).MenuColor);
            }
            RectangleF bgPanelBounds = ((BackStage)this.TabControl).GetTabPanelBounds();

            Rectangle rcBounds = new Rectangle(Point.Empty, button.Size);

            Color gradientBegin = ColorTable.BackStagePanelGradientBegin;
            Color gradientEnd = ColorTable.BackStagePanelGradientEnd;
            using (Brush brush = new SolidBrush((this.parent as BackStage).MenuColor))
            {
                e.Graphics.FillRectangle(brush, bgPanelBounds);
            }
            Color mColor = Color.Blue;
            if (button.Name == "DefaultBackStageButton")
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Pen pen = new Pen(Color.White, 2.5f);
                Rectangle rect = new Rectangle ( 22, 0, button.Height - 4, button.Height - 4);
                e.Graphics.DrawEllipse(pen, rect);
                e.Graphics.DrawLine(pen, new Point(32, rect.Height / 2), new Point(47, rect.Height / 2));
                e.Graphics.DrawLine(pen, new Point(31, rect.Height / 2), new Point(38, rect.Height / 2 -6));
                e.Graphics.DrawLine(pen, new PointF(31, rect.Height / 2), new PointF(38, rect.Height / 2 +5.5f));
                pen = new Pen(Color.White, 1.5f);
                e.Graphics.DrawLine(pen, new Point(31, rect.Height / 2), new Point(47, rect.Height / 2));
                pen.Dispose();
            }
            Color cr,cl;
            if ((this.parent as BackStage).UseDefaultHighlightColor)
                cr = cl = ColorTranslator.FromHtml("#0067b0");
            else
            {
                cr = Color.FromArgb(150, (this.parent as BackStage).MenuColor);
                cl = HighlightColor;
            }
                
            if (button.Enabled && (button.State & ButtonAdvState.MouseOver) > 0)
            {
                if (button.Name == "DefaultBackStageButton")
                {
                    SolidBrush br = new SolidBrush(cr);
                    e.Graphics.FillRectangle(br, new Rectangle(20, -1, button.Height , button.Height ));
                    br.Dispose();
                }
                else
                {
                    using (Brush brush = new SolidBrush(cl))
                    {
                        e.Graphics.FillRectangle(brush, rcBounds);
                    }
                }
            }

            Rectangle rcImage = Rectangle.Empty;

            if (button.Image != null)
            {
                rcImage = new Rectangle(IMG_PADDING, (rcBounds.Height - DEF_IMG_SIZE) / 2, DEF_IMG_SIZE, DEF_IMG_SIZE);
                if (button.Enabled)
                    e.Graphics.DrawImage(button.Image, rcImage);
                else
                    ControlPaint.DrawImageDisabled(e.Graphics, button.Image, rcImage.Left, rcImage.Top, Color.Gray);
            }

            Rectangle rcText = new Rectangle(
                rcImage.Right + IMG_PADDING,
                (rcBounds.Height - TextRenderer.MeasureText(button.Text, button.Font, Size.Empty, TextFormatFlags.EndEllipsis).Height) / 2,
                rcBounds.Width - rcImage.Right - IMG_PADDING,
                rcBounds.Height);
            Color foreColor = Color.White;
            if (!button.Enabled)
                foreColor = Color.Gray;
            if ((this.parent as BackStage).OfficeColorScheme == ToolStripEx.ColorScheme.Black)
                foreColor = Color.White;
            rcText = new Rectangle(rcText.X + 20, rcText.Y, rcText.Width, rcText.Height);
            if ((this.parent as BackStage).Alignment == System.Windows.Forms.TabAlignment.Right)
            {
                rcText.X = ((this.parent as BackStage).ItemSize.Width - TextRenderer.MeasureText (button.Text , button.Font).Width) /2  ;
                TextRenderer.DrawText(e.Graphics, button.Text, button.Font, rcText, foreColor, Color.Transparent,
                                 TextFormatFlags.EndEllipsis);
            }
            else
            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, rcText, foreColor, Color.Transparent,
                                  TextFormatFlags.EndEllipsis);
        }


        public static void DrawTriangle(Graphics g, RectangleF bounds, Brush brush)
        {
            PointF[] points = new PointF[]
				{
					new PointF( bounds.X + bounds.Width / 2, bounds.Y +5),
					new PointF( bounds.X - 2, bounds.Bottom ),
					new PointF( bounds.Right +2, bounds.Bottom  )
				};

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(points);
                gp.CloseAllFigures();

                using (Region rgn = new Region(gp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPolygon(brush, points);
                    //g.FillRegion(brush, rgn);
                }
            }
        }
        protected override void DrawText(Graphics g, RectangleF rectText, string text, StringFormat format, DrawTabEventArgs e)
        {
            if (this.TabAlignment == System.Windows.Forms.TabAlignment.Right)
            {
                //rectText.X = rectText.X - 3 * (this.parent as BackStage).ItemSize.Width / 5;
                rectText.X = rectText.X - 60;
            }
            base.DrawText(g, rectText, text, format, e);
        }
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {

        }

        protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            Rectangle rect = new Rectangle((int)focusRect.X, (int)focusRect.Y, (int)focusRect.Width, (int)focusRect.Height);            
        }
        #endregion

    }
	public class BackStageRendererProperty : TabPanelProperty2D
	{
		Hashtable hsColorTable = new Hashtable();
		public Office2010ColorTable ColorTable
		{
			get
			{
				Office2010ColorTable colorTable = hsColorTable[this.ColorScheme] as Office2010ColorTable;
				
				if (colorTable == null)
					hsColorTable[this.ColorScheme] = colorTable = new Office2010ColorTable(BackStage.GetOffice2010ColorScheme(this.ColorScheme));

				return colorTable;
			}
		}

		private ToolStripEx.ColorScheme _colorScheme = ToolStripEx.ColorScheme.Blue;
		public ToolStripEx.ColorScheme ColorScheme
		{
			get { return _colorScheme; }
			set { _colorScheme = value; }
		}

		public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
		{
			this.ColorScheme = (tabControl as BackStage).OfficeColorScheme;

			Color gradientBegin = ColorTable.BackStagePanelGradientBegin;
			Color gradientEnd = ColorTable.BackStagePanelGradientEnd;
            if ((tabControl as BackStage).BackStageStyle == RibbonStyle.Office2013)
            {
                using (Brush brush = new LinearGradientBrush(bounds, (tabControl as BackStage).MenuColor, (tabControl as BackStage).MenuColor, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
            else
            {
                using (Brush brush = new LinearGradientBrush(bounds, gradientBegin, gradientEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
		}

		public override SizeF GetOverlapSize(SizeF tabSize)
		{
			return Size.Empty;
		}
	}

	#endregion

	#region Delegates and EventArgs

	public delegate void ProvideBoundsEventHandler(object sender, ProvideBoundsEventArgs e);

	public class ProvideBoundsEventArgs
	{
		private Size _size = Size.Empty;
		private Point _location = Point.Empty;

		public ProvideBoundsEventArgs(Point location, Size size)
		{
			Size = size;
			Location = location;
		}

		public ProvideBoundsEventArgs(int top, int left, int width, int height)
		{
			Location = new Point(top, left);
			Size = new Size(width, height);
		}

		public Size Size
		{
			get { return _size; }
			set { _size = value; }
		}

		public int Width 
		{
			get { return _size.Width; }
			set { _size.Width = value; }
		}

		public int Height
		{
			get { return _size.Height; }
			set { _size.Height = value; }
		}

		public Point Location
		{
			get { return _location; }
			set { _location = value; }
		}

		public Rectangle Bounds
		{
			get { return new Rectangle(this.Location, this.Size); }
			set 
			{
				this.Size = value.Size;
				this.Location = value.Location;
			}
		}
	}

	#endregion
}