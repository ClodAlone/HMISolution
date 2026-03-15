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
	/// Represents the layout manager that lays out the children along the borders and
	/// at the center, very similar to the Windows Form's control docking behavior.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This layout manager will dock up to five controls along the four borders and the
	/// center. This is very similar to the control docking behavior exhibited by the
	/// DockStyle setting of a control.
	/// </para>
	/// <para>The <see cref="GetPosition"/> and <see cref="SetPosition "/> methods
	/// let you specify a <see cref="BorderPosition"/> for a child control and also
	/// act as an extended property during design-time for the child controls.</para>
	/// <para>Here are some of the differences between Windows Forms style docking and the
	/// BorderLayout.</para>
	/// <list type="bullet">
	/// <item>
	/// <term>When using <b>BorderLayout</b>, there can be only a single control that 
	/// can be docked to a border.</term>
	/// </item>
	/// <item>
	/// <term>When using <b>BorderLayout</b> and the <see cref="LayoutManager.CustomLayoutBounds"/> setting, 
	/// the bounds for layout can be customized to be something different from the control's client
	/// rectangle.</term>
	/// </item>
	/// <item>
	/// <term>Just like our other <see cref="LayoutManager"/>s, you can layout non-control
	/// based items when using the <b>BorderLayout</b>.</term>
	/// </item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <para>Here is some sample code that tells you how to initialize a CardLayout manager.</para>
	/// <coderef file="\Tools\Samples\Layout Manager Package\LayoutManagers\cs\BorderLayout.cs" name="Initializing BorderLayout" lang="C#"><code lang="C#">
	///			// Binding a control to the CardLayout manager programmatically.
	/// 		this.borderLayout1 = new BorderLayout();
	/// 		this.borderLayout1.ContainerControl = this;
	/// 		
	/// 		// Set the border-position of the button.
	/// 		this.borderLayout1.SetPosition(this.btnNorth, BorderPosition.North);
	/// 		this.borderLayout1.SetPosition(this.btnSouth, BorderPosition.South);
	/// 		this.borderLayout1.SetPosition(this.btnCenter, BorderPosition.Center);
	/// 		this.borderLayout1.SetPosition(this.btnEast, BorderPosition.East);
	/// 		this.borderLayout1.SetPosition(this.btnWest, BorderPosition.West);</code></coderef>
	/// <coderef file="\Tools\Samples\Layout Manager Package\LayoutManagers\VB\BorderLayout.vb" name="Initializing BorderLayout" lang="VB"><code lang="VB">
	///            ' Binding a Control to the CardLayout manager programmatically.
	///            Me.borderLayout1 = New BorderLayout()
	///            
	///            Me.borderLayout1.ContainerControl = Me
	///            
	///            ' Set the border-position of the button.
	///            Me.borderLayout1.SetPosition(Me.btnNorth, BorderPosition.North)
	///            Me.borderLayout1.SetPosition(Me.btnSouth, BorderPosition.South)
	///            Me.borderLayout1.SetPosition(Me.btnCenter, BorderPosition.Center)
	///            Me.borderLayout1.SetPosition(Me.btnEast, BorderPosition.East)
	///            Me.borderLayout1.SetPosition(Me.btnWest, BorderPosition.West)</code></coderef>
	/// <para>Also, take a look at the project in Tools/Samples/Layout Manager Package/LayoutManagers for an example.</para>
	/// </example>
	[
	ProvideProperty("Position", typeof(Control)),
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.BorderLayout.bmp"),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Represents the layout manager that lays out the children along the borders and at the center, very similar to the Windows Form's control docking behavior.")
	]
	public class BorderLayout : LayoutManager
	{	
		private int vGap;
		private int hGap;

		/// <summary>
		/// Hashtable to maintain constraint to control mapping.
		/// </summary>
		private Hashtable positionVsControls = new Hashtable();

		
		/// <summary>
		/// Overloaded. Creates an instance of the BorderLayout class and sets its defaults.
		/// </summary>
		public BorderLayout()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(BorderLayout));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
		}
		
		public BorderLayout(IContainer container)
			: this()
		{
			if(container != null)
				container.Add(this);
		}

		/// <summary>
		/// Creates an instance of the GridLayout class and sets its ContainerControl.
		/// </summary>
		public BorderLayout(Control container)
			:this()
		{
			ContainerControl = container;
		}

		/// <summary>
		/// Creates an instance of the GridLayout class and sets its ContainerControl.
		/// </summary>
		public BorderLayout(Control container, int vGap, int hGap)
			:this()
		{
			ContainerControl = container;
			this.vGap = vGap;
			this.hGap = hGap;
		}
			
		/// <summary>
		/// Gets or sets the vertical spacing between the layout border and the components. 
		/// </summary>
		/// <value>The vertical space in pixels.</value>
		[
		Category( "Appearance" ),
		Description( "Gets or sets the vertical spacing between the layout border and the components." )
		]
		public int VGap
		{
			get
			{
				return this.vGap;
			}
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
        /// Gets or sets the horizontal spacing between the layout border and the components.
		/// </summary>
		/// <value>The horizontal space in pixels.</value>
		[
		Category( "Appearance" ),
        Description("Gets or sets the horizontal spacing between the layout border and the components.")
		]
		public int HGap
		{
			get
			{
				return this.hGap;
			}
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
		/// Sets the <see cref="BorderPosition"/> for a child component.
		/// </summary>
		/// <param name="control">The child component whose position is to be set.</param>
		/// <param name="position">The <see cref="BorderPosition"/>.</param>
		public void SetPosition(Control control, BorderPosition position)
		{
			if(position == BorderPosition.None)
				this.RemoveLayoutComponent(control);
			else
				AddLayoutComponent(control, position);
		}
		private bool ShouldSerializePosition(Control control)
		{
			BorderPosition curPosition = this.GetPositionFromControl(control);
			return (curPosition == BorderPosition.None) ?	false : true;
		}
		private void ResetPosition(Control control)
		{
			this.SetPosition(control, BorderPosition.None);
		}

		/// <summary>
		/// Removes a child component from the layout list.
		/// <seealso cref="LayoutManager.RemoveLayoutComponent"/>
		/// </summary>
		public override void RemoveLayoutComponent(Control childControl)
		{
			BorderPosition curPosition = this.GetPositionFromControl(childControl);
			if(curPosition != BorderPosition.None)
				this.positionVsControls.Remove(curPosition);

			if(this.preferredSizes[childControl] != null)
			{
				Size prefSize = (Size)this.preferredSizes[childControl];
				childControl.Size = prefSize;
				this.preferredSizes.Remove(childControl);
			}
			base.RemoveLayoutComponent(childControl);
		}

		/// <summary>
		/// Adds a child component to the layout list with the specified constraints.
		/// <seealso cref="LayoutManager.AddLayoutComponent"/>
		/// </summary>
		public override void AddLayoutComponent(Control childControl, object constraints)
		{
			if(constraints == null || typeof(BorderPosition) != constraints.GetType())
				throw new ArgumentException("Constraints should be of the BorderPosition when calling AddLayoutComponent", "constraints");

			BorderPosition newPosition = (BorderPosition)constraints;

			BorderPosition curPosition = (BorderPosition)this.GetPositionFromControl(childControl);

			if(curPosition != newPosition)
			{
				if(this.ContainerControl != null)
					this.ContainerControl.SuspendLayout();

				// Check if an entry for this control already exists.
				RemoveLayoutComponent(childControl);

				if(newPosition != BorderPosition.None)
					this.positionVsControls[newPosition] = childControl;

				base.AddLayoutComponent(childControl, constraints);

				if(this.ContainerControl != null)
				{
					this.ContainerControl.ResumeLayout(false);
					this.ContainerControl.PerformLayout(childControl, "Bounds");
				}
			}
			return;
		}

		/// <summary>
		/// Returns the <see cref="BorderPosition"/> of a child component.
		/// </summary>
		/// <param name="control">The child component whose position is to be retrieved.</param>
		/// <returns>The <see cref="BorderPosition"/>.</returns>
		[Category("Layout Manager")]
		public BorderPosition GetPosition(Control control)
		{
			return this.GetPositionFromControl(control);
		}

		private BorderPosition GetPositionFromControl(Control control)
		{
			object controlKeyValue = null;
			foreach(object key in this.positionVsControls.Keys)
			{
				if(this.positionVsControls[key] == control)
				{
					controlKeyValue = key;
					break;
				}
			}
			
			return (controlKeyValue == null) ? BorderPosition.None : (BorderPosition)controlKeyValue;
		}

		private Control GetControlFromPosition(BorderPosition direction)
		{
			object o = this.positionVsControls[direction];
			return (o == null) ? null : o as Control;
		}

		/// <summary>
		/// Retrieves the preferred size associated with the specified control.
		/// <seealso cref="LayoutManager.GetPreferredSize"/>
		/// </summary>		
		[Browsable(false)]
		public override Size GetPreferredSize(Control control)
		{
			return base.GetPreferredSize(control);
		}

		// PreferredSize should be partly (width or height).
		/// <override/>
		protected override Size GetStaticPreferredSize(Control control)
		{
			Size prefSize = base.GetStaticPreferredSize(control);
			BorderPosition position = this.GetPositionFromControl(control);
			Size newPrefSize = prefSize;
			if(position != BorderPosition.None)
			{
				switch(position)
				{
					case BorderPosition.Center : 
						break;
					case BorderPosition.East:
					case BorderPosition.West:
						newPrefSize.Width = control.Size.Width;
						break;
					case BorderPosition.North:
					case BorderPosition.South:
						newPrefSize.Height = control.Size.Height;
						break;
				}
				if(newPrefSize != prefSize)
				{
					this.SetPreferredSize(control, newPrefSize);
				}
			}
			return newPrefSize;
		}

		/// <summary>
		/// Retrieves the minimum size associated with the specified control.
		/// <seealso cref="LayoutManager.GetMinimumSize"/>
		/// </summary>
		[Browsable(false)]
		public override Size GetMinimumSize(Control control)
		{
			return base.GetMinimumSize(control);
		}

		/// <summary>
		/// Returns the preferred size for the ContainerControl.
		/// <seealso cref="LayoutManager.PreferredLayoutSize"/>
		/// </summary>
		public override Size PreferredLayoutSize()
		{
			Size prefSizeNorth = Size.Empty;
			Size prefSizeSouth = Size.Empty;
			Size prefSizeWest = Size.Empty;
			Size prefSizeEast = Size.Empty;
			Size prefSizeCenter = Size.Empty;

			this.LookupPreferredSizes(ref prefSizeNorth, ref prefSizeSouth, ref prefSizeWest, 
				ref prefSizeEast, ref prefSizeCenter);

			int preferredWidth = this.CalcMax(prefSizeNorth.Width, prefSizeSouth.Width, 
				(prefSizeWest.Width + prefSizeEast.Width + prefSizeCenter.Width)
				);
			
			int preferredHeight = prefSizeNorth.Height + prefSizeSouth.Height +
				this.CalcMax(prefSizeWest.Height, prefSizeCenter.Height, prefSizeEast.Height);
				
			return new Size(preferredWidth, preferredHeight);
		}

		// Only deals with positive values.
		private int CalcMax(params int[] values)
		{
			int max = 0;
			foreach(int i in values)
				if( max < i) 
					max = i;
			return max;
		}

		private void LookupPreferredSizes(ref Size prefSizeNorth, ref Size prefSizeSouth, ref Size prefSizeWest, 
			ref Size prefSizeEast, ref Size prefSizeCenter)
		{
			Control c = null;
			
			if( (c = this.GetControlFromPosition(BorderPosition.North)) != null)
				prefSizeNorth = this.GetPreferredSize(c);

			if( (c = this.GetControlFromPosition(BorderPosition.South)) != null)
				prefSizeSouth = this.GetPreferredSize(c);

			if( (c = this.GetControlFromPosition(BorderPosition.West)) != null)
				prefSizeWest = this.GetPreferredSize(c);

			if( (c = this.GetControlFromPosition(BorderPosition.East)) != null)
				prefSizeEast = this.GetPreferredSize(c);

			if( (c = this.GetControlFromPosition(BorderPosition.Center)) != null)
				prefSizeCenter = this.GetPreferredSize(c);		
		}

		/// <summary>
		/// Returns the minimum size for the ContainerControl.
		/// <seealso cref="LayoutManager.MinimumLayoutSize"/>
		/// </summary>
		public override Size MinimumLayoutSize()
		{
			return this.PreferredLayoutSize();
		}

		/// <summary>
		/// Triggers a layout of the child components.
		/// <seealso cref="LayoutManager.LayoutContainer"/>
		/// </summary>
		public override void LayoutContainer()
		{
			if(!this.IsInit())
				return;
			Monitor.Enter(this);

			this.ContainerControl.SuspendLayout();

			Rectangle containerBounds = this.GetBounds();

			// Look up preferred sizes.
			Size prefSizeNorth = Size.Empty; Size prefSizeSouth = Size.Empty;
			Size prefSizeWest = Size.Empty; Size prefSizeEast = Size.Empty;
			Size prefSizeCenter = Size.Empty;

			this.LookupPreferredSizes(ref prefSizeNorth, ref prefSizeSouth, ref prefSizeWest, 
				ref prefSizeEast, ref prefSizeCenter);

			Size sizeNorth = Size.Empty; Size sizeSouth = Size.Empty;
			Size sizeWest = Size.Empty; Size sizeEast = Size.Empty;
			Size sizeCenter = Size.Empty;
			
			if(prefSizeNorth.IsEmpty == false)
			{
				sizeNorth.Height = prefSizeNorth.Height;
				sizeNorth.Width = containerBounds.Width;
			}

			if(prefSizeSouth.IsEmpty == false)
			{
				sizeSouth.Height = prefSizeSouth.Height;
				sizeSouth.Width = containerBounds.Width;
			}
			
			int remainingHeight = Math.Max(0, containerBounds.Height - (sizeNorth.Height + sizeSouth.Height));

			if(prefSizeEast.IsEmpty == false)
			{
				sizeEast.Width = prefSizeEast.Width;
				sizeEast.Height = remainingHeight;
			}

			if(prefSizeWest.IsEmpty == false)
			{
				sizeWest.Width = prefSizeWest.Width;
				sizeWest.Height = remainingHeight;
			}

			int remainingWidth = Math.Max(0, containerBounds.Width - (sizeEast.Width + sizeWest.Width));

			// Center is always calculated as a dummy value even if it does not exist
			//if(prefSizeCenter.IsEmpty == False).
			sizeCenter.Width = remainingWidth;
			sizeCenter.Height = remainingHeight;

			// Now factor in the gaps.
			// vgap
			int middleHeight = this.CalcMax(sizeWest.Height, sizeCenter.Height, sizeEast.Height);
			int vGapTotal = 2 * vGap;
			
			int middleCorrection = middleHeight >= vGapTotal ? vGapTotal : middleHeight;
			
			if(sizeWest.Height != 0)
				sizeWest.Height -= middleCorrection;
				
			if(sizeEast.Height != 0)
				sizeEast.Height -= middleCorrection;
			
			if(sizeCenter.Height != 0)
				sizeCenter.Height -= middleCorrection;
						
			middleHeight = this.CalcMax(sizeWest.Height, sizeCenter.Height, sizeEast.Height);

			// hgap
			int hGapTotal = 2 * hGap;
			int widthCorrection = sizeCenter.Width >= hGapTotal ? hGapTotal : sizeCenter.Width;
			
			if(sizeCenter.Width != 0)
				sizeCenter.Width -= widthCorrection;
			
			int x0 = containerBounds.X; int x1 = x0 + sizeWest.Width + widthCorrection/2; int x2 = x1 + sizeCenter.Width + widthCorrection/2;
			int y0 = containerBounds.Y; int y1 = y0 + sizeNorth.Height + middleCorrection/2; int y2 = y1 + middleHeight + middleCorrection/2;

			Rectangle boundsNorth = new Rectangle(new Point(x0, y0), sizeNorth);
			Rectangle boundsSouth = new Rectangle(new Point(x0, y2), sizeSouth);
			Rectangle boundsWest = new Rectangle(new Point(x0, y1), sizeWest);
			Rectangle boundsEast = new Rectangle(new Point(x2, y1), sizeEast);
			Rectangle boundsCenter = new Rectangle(new Point(x1, y1), sizeCenter);
			
			Control c = null;
			if( (c = this.GetControlFromPosition(BorderPosition.North)) != null)
				c.Bounds = boundsNorth;
			if( (c = this.GetControlFromPosition(BorderPosition.South)) != null)
				c.Bounds = boundsSouth;
			if( (c = this.GetControlFromPosition(BorderPosition.West)) != null)
				c.Bounds = boundsWest;
			if( (c = this.GetControlFromPosition(BorderPosition.East)) != null)
				c.Bounds = boundsEast;
			if( (c = this.GetControlFromPosition(BorderPosition.Center)) != null)
				c.Bounds = boundsCenter;


			this.ContainerControl.ResumeLayout(false);
			Monitor.Exit(this);
		}
	}
	/// <summary>
	/// Specifies the position and the manner in which the control will be laid out
	/// by the <see cref="BorderLayout"/>.
	/// </summary>
	public enum BorderPosition
	{
		/// <summary>
		/// The control is not laid out.
		/// </summary>
		None,
		/// <summary>
		/// The control's top edge is docked to the top of its containing control.
		/// </summary>
		North,
		/// <summary>
		/// The control's bottom edge is docked to the bottom of its containing control.
		/// </summary>
		South,
		/// <summary>
		/// The control's left edge is docked to the left edge of its containing control.
		/// </summary>
		East,
		/// <summary>
		/// The control's right edge is docked to the right edge of its containing control.
		/// </summary>
		West,
		/// <summary>
		/// The control is resized to fit the area between the controls laid out in the other borders.
		/// </summary>
		Center,
	}
}
