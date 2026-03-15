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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Threading;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents the layout manager that lays out the child components as a
	/// grid consisting of rows and columns.
	/// </summary>
	/// <remarks>
	/// <para>The layout manager divides the layout space into rows and columns based on the
	/// Rows and Columns properties and assigns each similar sized cell to a child component.</para>
	/// <para>You can control the component spacing, in pixels, through the HGap and VGap properties.</para>
	/// <para>You can remove or add child controls through the SetParticipateInLayout
	/// method. When the GridLayout's ContainerControl changes, it automatically includes all of its
	/// children in the layout component list, for convenience sake, so that you don't
	/// have to call SetParticipateInLayout for each child component. </para>
	/// <para>The <b>PreferredSize</b> and <b>MinimimumSize</b> settings for the children are ignored by 
	/// the GridLayout during layout, however the <see cref="PreferredLayoutSize"/> and <see cref="MinimumLayoutSize"/>
	/// methods do refer to the above settings to determine the sizes.</para>
	/// <para>Take a look at the LayoutManager class documentation for more information on
	/// LayoutManagers in general.</para>
	/// </remarks>
	/// <example>
	/// The following example shows you how to initialize a GridLayout manager with a container control:
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\cs\GridLayoutForm.cs" name="Initializing GridLayout" lang="C#"><code lang="C#">
	///             // Binding a Control to the GridLayout manager programmatically:
	///             this.gridLayout1 = new Syncfusion.Windows.Forms.Tools.GridLayout();
	/// 
	///             // Set the container control; all the child controls of this container control are
	///             // automatically registered as children with the manager:
	///             this.gridLayout1.ContainerControl = this.innerPanel;
	///             // Set some properties on the flowLayout manager:
	///             this.gridLayout1.Columns = 4;
	///             this.gridLayout1.Rows = 5;
	///             this.gridLayout1.HGap = 4;
	///             this.gridLayout1.VGap = 4;
	/// 
	///             // You can ignore one or more child Control from being laid out, like this.
	///             // This will have the same effect as calling RemoveLayoutComponent:
	///             //this.gridLayout1.SetParticipateInLayout(this.button1, false);
	/// 
	///             // You can prevent automatic layout during the layout event:
	///             // If you decide to do so, make sure to call gridLayout1.LayoutContainer manually:
	///             // this.gridLayout1.AutoLayout = false;</code></coderef>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\VB\GridLayoutForm.vb" name="Initializing GridLayout" lang="VB"><code lang="VB">
	///            ' Binding a Control to the GridLayout manager programmatically:
	///            Me.gridLayout1 = New Syncfusion.Windows.Forms.Tools.GridLayout
	///            ' Set the target control; all the child controls of this target control are
	///            ' automatically registered as children with the manager:
	///            Me.gridLayout1.ContainerControl = Me.innerPanel
	///            ' Set some properties on the flowLayout manager:
	///            Me.gridLayout1.Columns = 4
	///            Me.gridLayout1.Rows = 5
	///            Me.gridLayout1.HGap = 4
	///            Me.gridLayout1.VGap = 4
	///            ' You can ignore one or more child Control from being laid out, like this.
	///            ' This will have the same effect as calling RemoveLayoutComponent:
	///            'this.gridLayout1.SetParticipateInLayout(this.button1, false);
	///            ' You can prevent automatic layout during the layout event.
	///            ' If you decide to do so, make sure to call gridLayout1.LayoutContainer manually:
	///            ' this.gridLayout1.AutoLayout = false;</code></coderef>
	/// Also take a look at the project in Tools/Samples/Quick Start/LayoutManagers for an example.
	/// </example>
	[
	ProvideProperty("ParticipateInLayout", typeof(Control)),
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.GridLayout.bmp"),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Represents the layout manager that lays out the child components as a grid consisting of rows and columns.")
	]
	public class GridLayout : LayoutManager
	{
		private const int DefaultGap = 0;
		private int rows, columns, hGap, vGap;
		
		/// <summary>
		/// Overloaded. Creates an instance of the GridLayout class and sets its defaults.
		/// </summary>
		public GridLayout()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridLayout));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.rows = 1;
			this.columns = 0;
			this.hGap = 0;
			this.vGap = 0;
		}
		/// <summary>
		/// Creates a new instance of the GridLayout class and adds itself to the specified container.
		/// </summary>
		/// <param name="container">The logical ContainerControl parent into which to add itself.</param>
		/// <remarks><para>This constructor is used by the design-time to add a component to the form's
		/// IContainer field so that it gets Disposed when the form gets Disposed.</para>
		/// <para>Note that this is not the same as the layout manager's ContainerControl.</para></remarks>
		public GridLayout(IContainer container)
			: this()
		{
			if(container != null)
				container.Add(this);
		}
		/// <summary>
		/// Creates an instance of the GridLayout class and sets its ContainerControl.
		/// </summary>
		public GridLayout(Control container)
			:this()
		{
			ContainerControl = container;
		}
		/// <summary>
		/// Creates an instance of the GridLayout class and sets its ContainerControl,
		/// rows and columns.
		/// </summary>
		public GridLayout(Control container, int rows, int columns)
			:this(container)
		{
			this.rows = rows;
			this.columns = columns;
		}
		/// <summary>
		/// Creates an instance of the GridLayout class and sets its ContainerControl,
		/// rows, columns, horizontal gap and vertical gap.
		/// </summary>
		public GridLayout(Control container, int rows, int columns, int hGap, int vGap)
			:this(container, rows, columns)
		{
			this.hGap = hGap;
			this.vGap = vGap;
		}

		/// <summary>
		/// Gets / sets the number of rows in the grid.
		/// </summary>
		/// <value>The number of rows. Default is 1.</value>
		/// <remarks>At least one Row or Column property should be greater than zero.
		/// If they are both set to zero, then the Rows property will be changed to 1.
		/// <para>If you try to set a negative value, this will instead be set to zero.</para></remarks>
		[Category("Appearance"),
		Description("Specifies the number of rows in the grid.")]
		public int Rows
		{
			get
			{
				if(this.rows == 0 && this.columns == 0)
					this.rows = 1;
				return this.rows;
			}
			set
			{
				if(this.rows != value)
				{
					if(value < 0)
						value = 0;
					this.rows = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}

		/// <summary>
		/// Gets / sets the number of columns in the grid.
		/// </summary>
		/// <value>The number of columns. Default is 1.</value>
		/// <remarks><para>At least one Row or Column property should be greater than zero.
		/// If they are both set to zero, then the Row property will be changed to 1.</para>
		/// <para>If you try to set a negative value, this will instead be set to zero.</para></remarks>
		[Category("Appearance"),
		Description("Specifies the number of columns in the grid.")
		]
		public int Columns
		{
			get{return this.columns;}
			set
			{
				if(this.columns != value)
				{
					if(value < 0)
						value = 0;
					this.columns = value;
					int rows = this.Rows; // To update Rows, if necessary.
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		/// <summary>
		/// Gets / sets the horizontal spacing between the layout border and the components. 
		/// </summary>
		/// <value>The horizontal space in pixels.</value>
		[Category("Appearance"),
		DefaultValue(DefaultGap),
		Description("Specifies the horizontal spacing between the layout border and the components.")
		]
		public int HGap
		{
			get{return this.hGap;}
			set
			{
				if(this.hGap != value)
				{
					this.hGap = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}

		/// <summary>
		/// Gets / sets the vertical spacing between the layout border and the components. 
		/// </summary>
		/// <value>The vertical space in pixels.</value>
		[Category("Appearance"),
		DefaultValue(DefaultGap),
		Description("Specifies the vertical spacing between the layout border and the components.")
		]
		public int VGap
		{
			get{return this.vGap;}
			set
			{
				if(this.vGap != value)
				{
					this.vGap = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		/// <summary>
		/// Indicates whether the component is in the layout list.
		/// </summary>
		/// <param name="control">The control whose participation needs to be verified.</param>
		/// <returns>True if it is in the layout list; False otherwise.</returns>
		[Category("Layout Manager"),
		Localizable(true)]
		public bool GetParticipateInLayout(Control control)
		{
			if(this.LayoutControls.IndexOf(control) != -1)
				return true;
			else
				return false;
		}
		/// <summary>
		/// Adds or removes the specified control from the layout list.
		/// </summary>
		/// <param name="control">The control to be added or removed.</param>
		/// <param name="value">True means the control will be added; False will remove it.</param>
		public void SetParticipateInLayout(Control control, bool value)
		{
			if(value)
				AddLayoutComponent(control, null);
			else
				RemoveLayoutComponent(control);
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.OnControlAdded"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnControlAdded(object sender, ControlEventArgs e)
		{
			// Don't have to call LayoutContainer, since this is ususally followed by the Layout event
			if( !this.LoadingDocument && !this.initializing )
			{
			this.SetParticipateInLayout(e.Control, true);
			}

			base.OnControlAdded(sender, e);
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.OnContainerControlChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnContainerControlChanged(EventArgs e)
		{
			// Make sure to call base class first.
			base.OnContainerControlChanged(e);

			if(this.ContainerControl != null && !this.LoadingDocument)
			{
				// Make all the child controls participate in layout management by default.
				foreach(Control control in this.ContainerControl.Controls)
					this.SetParticipateInLayout(control, true);
			}
		}
		// These are not exposed because they don't get used.
		/// <override/>
		[Browsable(false)]
		public override Size GetPreferredSize(Control control)
		{
			return base.GetPreferredSize(control);
		}
		/// <override/>
		[Browsable(false)]
		public override Size GetMinimumSize(Control control)
		{
			return base.GetMinimumSize(control);
		}

		// Not used by this class.
		/// <override/>
		protected override void ResetLayoutInfo()
		{
			base.ResetLayoutInfo();
			// Important: this has to be 1.
			this.rows = 1;
			this.columns = 0;
			// Also remove the child control cache.
			return;
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.PreferredLayoutSize"/>.
		/// </summary>
		/// <returns>Returns the preferred layout size.</returns>
		public override Size PreferredLayoutSize()
		{
			IList controls = this.GetControls();
			int nMembers = controls.Count;
			int nRows = Rows;
			int nCols = Columns;

			if (nRows > 0) 
			{
				nCols = (nMembers + nRows - 1) / nRows;
			} 
			else 
			{
				nRows = (nMembers + nCols - 1) / nCols;
			}
			int w = 0;
			int h = 0;
			for (int i = 0 ; i < nMembers ; i++) 
			{
				Control control = controls[i] as Control;
				Size sz = GetPreferredSize(control);
				if (w < sz.Width) 
				{
					w = sz.Width;
				}
				if (h < sz.Height) 
				{
					h = sz.Height;
				}
			}
			return new Size(this.AdjustWidthForMargins(nCols*w + (nCols-1)*HGap), 
				this.AdjustHeightForMargins(nRows*h + (nRows-1)*VGap));
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.MinimumLayoutSize"/>.
		/// </summary>
		public override Size MinimumLayoutSize()
		{
			IList controls = this.GetControls();
			int nMembers = controls.Count;
			int nRows = Rows;
			int nCols = Columns;

			if (nRows > 0) 
			{
				nCols = (nMembers + nRows - 1) / nRows;
			} 
			else 
			{
				nRows = (nMembers + nCols - 1) / nCols;
			}
			int w = 0;
			int h = 0;
			for (int i = 0 ; i < nMembers ; i++) 
			{
				Control control = controls[i]  as Control;
				Size sz = GetMinimumSize(control);
				if (w < sz.Width) 
				{
					w = sz.Width;
				}
				if (h < sz.Height) 
				{
					h = sz.Height;
				}
			}
			return new Size(nCols*w + (nCols-1)*HGap + this.HorzNearMargin + this.HorzFarMargin, 
				nRows*h + (nRows-1)*VGap + this.TopMargin + this.BottomMargin);
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.LayoutContainer"/>.
		/// </summary>
		public override void LayoutContainer()
		{
			if(!this.IsInit())
				return;
			Monitor.Enter(this);

			this.ContainerControl.SuspendLayout();

			Rectangle containerBounds = this.GetBounds();
			
			IList controls = this.GetControls();
			int nmembers = controls.Count;
			int nRows = this.Rows;
			int nCols = this.Columns;

			if (nmembers == 0) 
				return;
			if (nRows > 0) 
				nCols = (nmembers + nRows - 1) / nRows;
			else 
				nRows = (nmembers + nCols - 1) / nCols;

			int w = containerBounds.Width;
			int h = containerBounds.Height;
			
			w = (w - (nCols - 1) * this.hGap);
			int modw = w % nCols;
			w = w / nCols;
			
			h = (h - (nRows - 1) * this.vGap);
			int modh = h % nRows;
			h = h / nRows;

			int boundsLeft = containerBounds.Left + (int)Math.Ceiling((float)modw/(float)2);
			int boundsTop = containerBounds.Top + (int)Math.Ceiling((float)modh/(float)2);

			for (int c = 0, x = 0 ; c < nCols ; c++, x += w + this.hGap) 
			{
				for (int r = 0, y = 0 ; r < nRows ; r++, y += h + this.vGap) 
				{
					int i = r * nCols + c;
					if (i < nmembers) 
					{
						//((Control)controls[i]).Bounds = new Rectangle(boundsLeft + x , 
						//	boundsTop + y, w, h);
						// Apply this on the ControlBounds instead of the contro itself to take care of RTL configs.
						ControlBounds cb = this.GetChildControlBounds((Control)controls[i]);
						cb.Bounds = new Rectangle(boundsLeft + x , 
							boundsTop + y, w, h);
					}
				}
			}

//			if(this.ContainerControl is ScrollableControl)
//			{
//				ScrollableControl sc = this.ContainerControl as ScrollableControl;
//				MethodInfo mInfo = typeof(ScrollableControl).GetMethod("AdjustFormScrollbars", 
//					BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
//				if(mInfo != null)
//				{
//					mInfo.Invoke(sc, new object[]{sc.AutoScroll});
//				}
//			}
			this.ContainerControl.ResumeLayout(false);
			Monitor.Exit(this);
		}
	}
}
