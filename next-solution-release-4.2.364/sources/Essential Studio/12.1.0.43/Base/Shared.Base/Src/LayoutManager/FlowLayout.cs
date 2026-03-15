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
using System.Threading;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.ComponentModel;
using Syncfusion.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the alignment of layout components in the direction of flow.
	/// </summary>
	public enum FlowAlignment 
	{ 
		/// <summary>
		/// Center the components within the parent's width (if vertical layout) or 
		/// height (if horizontal layout).
		/// </summary>
		Center, 
		/// <summary>
		/// Dock the components to the left border (if vertical layout) or 
		/// top border (if horizontal layout).
		/// </summary>
		Near, 
		/// <summary>
		/// Dock the components to the right border (if vertical layout) or 
		/// bottom border (if horizontal layout).
		/// </summary>
		Far,
		/// <summary>
		/// Refer to the child's <see cref="FlowLayoutConstraints"/> to determine the alignment and layout.
		/// </summary>
		ChildConstraints
	};

	/// <summary>
	/// Specifies whether the children should be laid out horizontally or vertically.
	/// </summary>
	public enum FlowLayoutMode 
	{
		/// <summary>
		/// Children will be laid out horizontally, left to right.
		/// </summary>
		Horizontal, 
		/// <summary>
		/// Children will be laid out vertically, top to bottom.
		/// </summary>
		Vertical
	};

	/// <summary>
	/// Specifies how child components will be positioned inside a container managed
	/// by the <see cref="FlowLayout"/> manager.
	/// </summary>
	/// <remarks>
	/// <para>Flow Layout constraints are a set of properties that determine how a 
	/// child component will be horizontally and vertically aligned when laid out in 
	/// rows or columns.</para>
	/// </remarks>
	[
	TypeConverter(typeof(FlowLayoutConstraintsConverter)),
	Serializable()
	]
	public class FlowLayoutConstraints : ICloneable
	{
		private bool active = true;
		private HorzFlowAlign halign = HorzFlowAlign.Left;
		private bool newLine = false;
		private VertFlowAlign valign = VertFlowAlign.Center;
		internal bool isEmpty = false;
		private bool propRowHeight = false;
		private bool propColWidth = false;

		public static readonly FlowLayoutConstraints Empty;
		static FlowLayoutConstraints()
		{
			Empty = Default();
			Empty.IsEmpty = true;
		}

		/// <summary>
		/// Returns a default FlowLayoutConstraints object (that is not empty).
		/// </summary>
		/// <returns>The default FlowLayoutConstraints object.</returns>
		public static FlowLayoutConstraints Default(){return new FlowLayoutConstraints();}

		/// <summary>
		/// Overloaded. Creates a new instance of the FlowLayoutConstraints class and sets its defaults.
		/// </summary>
		public FlowLayoutConstraints()
		{
		}

		/// <summary>
		/// Creates a new instance of the FlowLayoutConstraints class 
		/// with the specified values.
		/// </summary>
		/// <param name="active">True indicates this child should participate in layout; False otherwise.</param>
		/// <param name="halign">The <see cref="HorzFlowAlign"/> mode in which child should be laid out (when in horizontal flow mode).</param>
		/// <param name="valign">The <see cref="VertFlowAlign"/> mode in which child should be laid out (when in vertical flow mode).</param>
		/// <param name="newline">True indicates a line break when this child is encountered
		/// while laying out.</param>
		/// <param name="proportionalColWidth">True indicates that the width of the column the corresponding 
		/// child control occupies should be proportional to the laid out column's preferred width (when laid out vertically).</param>
		/// /// <param name="proportionalRowHeight">True indicates that the height of the row the corresponding 
		/// child control occupies should be proportional to the laid out row's preferred height (when laid out horizontally).</param>
		public FlowLayoutConstraints(bool active, HorzFlowAlign halign, VertFlowAlign valign,
			bool newline, bool proportionalColWidth, bool proportionalRowHeight)
		{
			this.active = active;
			this.halign = halign;
			this.valign = valign;
			this.newLine = newline;
			this.propColWidth = proportionalColWidth;
			this.propRowHeight = proportionalRowHeight;
		}

		/// <summary>
		/// Indicates whether the child should participate in layout.
		/// </summary>
		/// <value>True to indicate this child should participate in layout; False otherwise. Default is True.</value>
		[DefaultValue(true),
		Description("Specifies whether the child should participate in layout.")]
		public bool Active{get{return this.active;}set{this.active = value;}}
		/// <summary>
		/// Specifies the mode in which the child should be laid out within a row.
		/// </summary>
		/// <value>A <see cref="HorzFlowAlign"/> value. Default is HorzFlowAlign.Justify.</value>
		[DefaultValue(HorzFlowAlign.Left),
		Description("Specifies the mode in which the child should be laid out within a row.")]
		public HorzFlowAlign HAlign{get{return this.halign;}set{this.halign = value;}}
		/// <summary>
		/// Specifies the mode in which the child should be laid out within a column.
		/// </summary>
		/// <value>A <see cref="VertFlowAlign"/> value. Default is VertFlowAlign.Center.</value>
		[DefaultValue(VertFlowAlign.Center),
		Description("Specifies the mode in which the child should be laid out within a column.")]
		public VertFlowAlign VAlign{get{return this.valign;}set{this.valign = value;}}
		/// <summary>
		/// Indicates whether this child should always be moved to the beginning of a new line when laid out.
		/// </summary>
		/// <value>True to move to a new line; False otherwise. Default is False.</value>
		[DefaultValue(false),
		Description("Specifies whether this child should always be moved to the beginning of a new line.")]
		public bool NewLine{get{return this.newLine;}set{this.newLine = value;}}
		/// <summary>
		/// Indicates whether the effective height of the row the corresponding child control occupies should be 
		/// proportional to the laid out rows' preferred heights, for horizontal layout mode.
		/// </summary>
		/// <value>True for proportional height; False otherwise. Default is False.</value>
		/// <remarks>
		/// This property is in effect only when the layout mode is horizontal. When this property is on, the row
		/// this control occupies will be deemed to take proportional height. Then the remaining vertical space
        /// available will be split proportionally between such rows wanting proportional-height based on their preferred height.
		/// </remarks>
		[DefaultValue(false),
		Description("Specifies if proportional row heights should be used in horizontal layout.")
		]
		public bool ProportionalRowHeight{get{return this.propRowHeight;}set{this.propRowHeight = value;}}
		/// <summary>
		/// Indicates whether the effective width of the column the corresponding child control occupies should be 
		/// proportional to the laid out columns' preferred widths, for vertical layout mode.
		/// </summary>
		/// <value>True for proportional width; False otherwise. Default is False.</value>
		/// <remarks>
		/// This property is in effect only when the layout mode is vertical. When this property is on, the column
		/// this control occupies will be deemed to take proportional width. Then the remaining horizontal space
        /// available will be split proportionally between such columns wanting proportional-width based on their preferred width.
		/// </remarks>
		[DefaultValue(false),
		Description("Specifies if proportional col widths should be used in vertical layout.")]
		public bool ProportionalColWidth{get{return this.propColWidth;}set{this.propColWidth = value;}}

		/// <summary>
		/// Gets / sets the FlowLayoutConstraints structure with its properties left uninitialized.
		/// </summary>
		[DefaultValue(false),
		Description("Specifies if there are valid values in this constraint instance.")]
		public bool IsEmpty{get{return this.isEmpty;}set{this.isEmpty=value;}}

		/// <summary>
		/// Creates an exact copy of this FlowLayoutConstraints object.
		/// </summary>
		/// <returns>The cloned object.</returns>
		public object Clone () 
		{
			FlowLayoutConstraints c = (FlowLayoutConstraints)this.MemberwiseClone();
			return c;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			bool eq = false;

			if ((object)this == o)
				return true;
		                    
			if ( o is FlowLayoutConstraints )
			{
				FlowLayoutConstraints flc = (FlowLayoutConstraints)o;
				if(this.Active != flc.Active)
					return false;
				else if(this.HAlign != flc.HAlign)
					return false;
				else if(this.NewLine != flc.NewLine)
					return false;
				else if(this.ProportionalColWidth != flc.ProportionalColWidth)
					return false;
				else if(this.ProportionalRowHeight != flc.ProportionalRowHeight)
					return false;
				else if(this.VAlign != flc.VAlign)
					return false;
				else
					return true;
			}
			return eq;
		}

		/// <summary cref="Equals">
		///		The basic == operator.
		/// </summary>
		/// <param name="lhs">The left-hand side of the operator.</param>
		/// <param name="rhs">The right-hand side of the operator.</param>
		/// <returns>
		///		Boolean value.
		///	</returns>
		public static bool operator==( FlowLayoutConstraints lhs, FlowLayoutConstraints rhs ) 
		{                            
			if((object)lhs == null && (object)rhs == null)
				return true;
			if ((object) lhs == null || (object) rhs == null)
				return false;
			return lhs.Equals(rhs);
		}

		/// <summary cref="Equals">
		///		The basic != operator.
		/// </summary>
		/// <param name="lhs">The left-hand side of the operator.</param>
		/// <param name="rhs">The right-hand side of the operator.</param>
		/// <returns>
		///		bool
		///	</returns>
		public static bool operator!=( FlowLayoutConstraints lhs, FlowLayoutConstraints rhs ) 
		{          
			if((object)lhs == null && (object)rhs == null)
				return false;

			if ((object) lhs == null || (object) rhs == null)
				return true;
			return !lhs.Equals(rhs);
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class FlowLayoutConstraintsConverter : 
		ByteStreamTypeConverter
	{
		public override /*TypeConverter*/ object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			FlowLayoutConstraints flc = new FlowLayoutConstraints();
			flc.Active = (bool)propertyValues["Active"];
			flc.HAlign = (HorzFlowAlign)propertyValues["HAlign"];			
			flc.VAlign = (VertFlowAlign)propertyValues["VAlign"];			
			flc.NewLine = (bool)propertyValues["NewLine"];
			flc.ProportionalColWidth = (bool)propertyValues["ProportionalColWidth"];
			flc.ProportionalRowHeight = (bool)propertyValues["ProportionalRowHeight"];
			return flc;
		} // end of method CreateInstance
 
		public override void OnBeforeDeserialize()
		{
			AppStateSerializer.SetBindingInfo("Syncfusion.Shared.Base", typeof(FlowLayoutConstraints).Assembly);

			// For backward compatibility.
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Shared.Base", typeof(FlowLayoutConstraints).FullName, typeof(FlowLayoutConstraints).Assembly);
		}

		public override void OnAfterDeserialize()
		{
			// Reset
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Shared.Base", typeof(FlowLayoutConstraints).FullName, null);
		}

		public override /*TypeConverter*/ bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		} // end of method GetCreateInstanceSupported

		public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		} // end of method CanConvertTo
        
		public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is FlowLayoutConstraints))
			{
				FlowLayoutConstraints flcInst = (FlowLayoutConstraints)value;

				System.Type[] args;
				args = new System.Type[6];

				args[0] = typeof(bool);
				args[1] = typeof(HorzFlowAlign);
				args[2] = typeof(VertFlowAlign);
				args[3] = typeof(bool);
				args[4] = typeof(bool);
				args[5] = typeof(bool);

				System.Reflection.ConstructorInfo constructorInfo;
				constructorInfo = typeof(FlowLayoutConstraints).GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = new System.Object[6];
					argValues[0] = flcInst.Active;
					argValues[1] = flcInst.HAlign;
					argValues[2] = flcInst.VAlign;
					argValues[3] = flcInst.NewLine;
					argValues[4] = flcInst.ProportionalColWidth;
					argValues[5] = flcInst.ProportionalRowHeight;

					return new InstanceDescriptor(constructorInfo,argValues);
				}
			}
			else if (destinationType == typeof(string) && value is FlowLayoutConstraints)
			{
				FlowLayoutConstraints flcInst = (FlowLayoutConstraints)value;

				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(flcInst);

				string constraints = String.Empty;

				PropertyDescriptor prop = properties.Find("Active", false);
				if(prop.ShouldSerializeValue(flcInst))
					constraints += "Active " + flcInst.Active.ToString() + "; ";

				prop = properties.Find("HAlign", false);
				if(prop.ShouldSerializeValue(flcInst))
					constraints += "HAlign " + flcInst.HAlign.ToString() + "; ";

				prop = properties.Find("VAlign", false);
				if(prop.ShouldSerializeValue(flcInst))
					constraints += "VAlign " + flcInst.VAlign.ToString() + "; ";

				prop = properties.Find("NewLine", false);
				if(prop.ShouldSerializeValue(flcInst))
					constraints += "NewLine " + flcInst.NewLine.ToString() + "; ";

				prop = properties.Find("ProportionalColWidth", false);
				if(prop.ShouldSerializeValue(flcInst))
					constraints += "ProportionalColWidth " + flcInst.ProportionalColWidth.ToString() + "; ";

				prop = properties.Find("ProportionalRowHeight", false);
				if(prop.ShouldSerializeValue(flcInst))
					constraints += "ProportionalRowHeight " + flcInst.ProportionalRowHeight.ToString() + "; ";

				return constraints;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		} // end of method ConvertTo        
	}
	/// <summary>
	/// Specifies the alignment of child components within a row when horizontally laid out.
	/// </summary>
	public enum HorzFlowAlign
	{
		/// <summary>
		/// The child component is left aligned within the row.
		/// </summary>
		Left,
		/// <summary>
		/// The child component is right aligned within the row.
		/// </summary>
		Right,
		/// <summary>
		/// The child component is centered within the row.
		/// </summary>
		Center,
		/// <summary>
		/// The child component will be expanded (or shrunk up to the MinimumSize) to fill any available extra width.
		/// </summary>
		/// <remarks>Justified components are positioned after the centered ones.</remarks>
		Justify
	}
	/// <summary>
	/// Specifies the alignment of child components within a column when vertically laid out.
	/// </summary>
	public enum VertFlowAlign
	{
		/// <summary>
		/// The child component is top aligned within the column.
		/// </summary>
		Top,
		/// <summary>
		/// The child component is bottom aligned within the column.
		/// </summary>
		Bottom,
		/// <summary>
		/// The child component is center aligned within the column.
		/// </summary>
		Center,
		/// <summary>
		/// The child component will be expanded to fill any available extra height.
		/// </summary>
		/// <remarks>Justified components are positioned after the centered ones.</remarks>
		Justify,
	}

	/// <summary>
	/// Represents the layout manager that does a left to right or top to bottom
	/// layout.
	/// </summary>
	/// <remarks>
	/// <para>Arranges components horizontally (left to right) or vertically (top to bottom)
	/// (As specified in the <see cref="LayoutMode"/> property). When there is no more space
	/// in a line, it moves the components to the next line.</para>
	/// <para>By default, each line is centered. You can change this justification using
	/// the <see cref="Alignment"/> property.</para>
	/// <para>You can also set <see cref="FlowLayoutConstraints"/> on each component for more control
	/// over the component's alignment and spacing within a row / column.</para>
	/// <para>You can control the component spacing, in pixels, through the <see cref="HGap"/> and <see cref="VGap"/> properties.</para>
	/// <para>When the FlowLayout's <see cref="ContainerControl"/> changes, it automatically assigns default FlowLayoutConstraints to the
	/// children, for convenience sake, so that you don't
	/// have to call <see cref="SetConstraints"/> for each child component. </para>
	/// <para>Take a look at the LayoutManager class documentation for more information on
	/// LayoutManagers in general.</para>
	/// </remarks>
	/// <example>
	/// The following example shows you how to initialize a FlowLayout manager with a container control:
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\cs\FlowLayoutForm.cs" name="Initializing FlowLayout" lang="C#"><code lang="C#">
	///             // Binding a control to the FlowLayout manager programmatically:
	///             this.flowLayout1 = new FlowLayout();
	/// 
	///             // Set the container control; all the child controls of this container control are
	///             // automatically registered as children with the manager:
	///             this.flowLayout1.ContainerControl = this.panel1;
	/// 
	///             // Set some properties on the flowLayout manager:
	///             this.flowLayout1.HGap = 20;
	///             this.flowLayout1.Alignment = FlowAlignment.Near;
	/// 
	///				// You can prevent one or more child controls from being laid out, like this (the first argument for FlowLayoutConstraints should be False).
	///				// This will have the same effect as calling RemoveLayoutComponent:
	///				this.flowLayout1.SetConstraints(this.label10, new FlowLayoutConstraints(false, HorzFlowAlign.Left, VertFlowAlign.Center, false, false, false));
	/// 
	///             // You can prevent automatic layout during the layout event.
	///             // If you decide to do so, make sure to call flowLayout.LayoutContainer manually:
	///             // this.flowLayout1.AutoLayout = false;</code></coderef>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\VB\FlowLayoutForm.vb" name="Initializing FlowLayout" lang="VB"><code lang="VB">
	///            ' Binding a control to the FlowLayout manager programmatically:
	///            Me.flowLayout1 = New FlowLayout
	///            ' Set the target control; all the child controls of this target control are
	///            ' automatically registered as children with the manager:
	///            Me.flowLayout1.ContainerControl = Me.panel1
	///            ' Set some properties on the flowLayout manager:
	///            Me.flowLayout1.HGap = 20
	///            Me.flowLayout1.Alignment = FlowAlignment.Near
	///            ' You can ignore one or more child controls from being laid out, like this (the first argument for FlowLayoutConstraints should be False).
	///            ' This will have the same effect as calling RemoveLayoutComponent:
	///            Me.flowLayout1.SetConstraints(Me.label10, New FlowLayoutConstraints(False, HorzFlowAlign.Center, VertFlowAlign.Center, False, False, False))
	///            ' You can prevent automatic layout during the layout event.
	///            ' If you decide to do so, make sure to call flowLayout.LayoutContainer manually:
	///            ' this.flowLayout1.AutoLayout = false;</code></coderef>
	/// Also take a look at the project in Tools/Samples/Quick Start/LayoutManagers for an example.
	/// </example>
	[
	ProvideProperty("Constraints", typeof(Control)),
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.FlowLayout.bmp"),
	ToolboxItemFilter("System.Windows.Forms"),
	Description("Represents the layout manager that does a left to right or top to bottom layout.")
	]
	public class FlowLayout : LayoutManager
	{
		private const int DefaultGap = 5;
		private bool reverseRows = false;
		
		private FlowAlignment alignment;
		private int hGap;
		private int vGap;
		private bool autoHeight = false;
		private FlowLayoutMode layoutMode;
		private Hashtable controlsMap;
		protected Hashtable noFillSizes;

		private bool layoutForDeterminingPreferredSize = false;
		// This will contain the preferred width or height based on the layout mode (adjusted for margins).
		private Size lastKnownPreferredSize = Size.Empty;

		/// <summary>
		/// Gets / sets the alignment of layout components in the direction of flow.
		/// </summary>
		/// <value>A FlowAlignment value specifying the justification.
		/// Default is FlowAlignment.Center.</value>
		/// <remarks>
		/// FlowAlignment.Near will be either left justified or top justified based on
		/// whether the layout mode is vertical or horizontal. And similarly,
		/// FlowAlignment.Far will be either right justified or top justified.
		/// FlowAlignment.ChildConstraints will make the manager refer to the child's constraints.
		/// </remarks> 
		[DefaultValue(FlowAlignment.Center),
		Localizable(true),
		Description("Specifies the alignment of layout components in the direction of flow."),
		Category("Behavior")
		]
		public FlowAlignment Alignment
		{
			get{return this.alignment;}
			set
			{
				if(this.alignment != value)
				{
					this.alignment = value;

					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		
		/// <summary>
		/// Gets / sets the layout mode.
		/// </summary>
		/// <value>The current FlowLayoutMode. Default is FlowLayoutMode.Horizontal.</value>
		[DefaultValue(FlowLayoutMode.Horizontal),
		Localizable(true),
		Description("Specifies the layout mode."),
		Category("Behavior")
		]
		public FlowLayoutMode LayoutMode
		{
			get{return this.layoutMode;}
			set
			{
				if(this.layoutMode != value)
				{
					this.layoutMode = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();
				}
			}
		}

		/// <summary>
		/// Indicates whether to lay out rows in the opposite direction (right to left or bottom to top).
		/// </summary>
		/// <value>False for regular layout; True for reverse layout. Default is False.</value>
		[
		Description("Lays out rows in opposite direction (right to left or bottom to top)."),
		DefaultValue(false),
			Category("Behavior"),
		Localizable(true)]
		public bool ReverseRows
		{
			get{return this.reverseRows;}
			set
			{
				if(this.reverseRows != value)
				{
					this.reverseRows = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}

		/// <summary>
		/// Gets / sets the horizontal spacing between the components.
		/// </summary>
		/// <value>The horizontal space in pixels.</value>
		[Category("Appearance"),
		DefaultValue(DefaultGap),
		Localizable(true),
		Description("Specifies the horizontal spacing between the layout border and the components.")
		]
		public int HGap
		{
			get
			{
				return this.hGap;
			}
			set
			{
				if( this.hGap != value )
				{
					int oldGap = this.hGap;

					this.hGap = value;

					if( this.ContainerControl != null )
					{
						this.ContainerControl.PerformLayout();
					}

					this.MakeDirty();

					OnGapChanged( this.HGapChanged, new ValueChangedEventArgs( oldGap, value ) );
				}
			}
		}

		/// <summary>
		/// Indicates whether the container control should automatically grow in height when
		/// there is not enough space when in horizontal alignment mode.
		/// </summary>
		/// <value>True to automatically increase the height; False otherwise.</value>
		/// <remarks>
		/// This applies only in horizontal alignment mode.
		/// </remarks>
		[
		Description("Specifies if the container's height should be enforced to the minimum when in horizontal alignment mode."),
		Category("Behavior"),
		DefaultValue(false),
		]
		public bool AutoHeight
		{
			get{return this.autoHeight;}
			set
			{
				if(this.autoHeight != value)
				{
					this.autoHeight = value;
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
		Localizable(true),
		Description("Specifies the vertical spacing between the layout border and the components.")
		]
		public int VGap
		{
			get
			{
				return this.vGap;
			}
			set
			{
				if( this.vGap != value )
				{
					int oldGap = this.vGap;

					this.vGap = value;

					if( this.ContainerControl != null )
					{
						this.ContainerControl.PerformLayout();
					}

					this.MakeDirty();

					OnGapChanged( this.VGapChanged, new ValueChangedEventArgs( oldGap, value ) );
				}
			}
		}

		/// <summary>
		/// Overloaded. Creates a new instance of the FlowLayout component and sets its defaults.
		/// </summary>
		public FlowLayout()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(FlowLayout));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.layoutMode = FlowLayoutMode.Horizontal;
			this.alignment = FlowAlignment.Center;
			this.hGap = DefaultGap;
			this.vGap = DefaultGap;
			controlsMap = new Hashtable();
			noFillSizes = new Hashtable();
		}


		/// <summary>
		/// Creates a new instance of the FlowLayout class and adds itself to the specified container.
		/// </summary>
		/// <param name="container">The logical ContainerControl parent into which to add itself.</param>
		/// <remarks><para>This constructor is used by the design-time to add a component to the form's
		/// IContainer field so that it gets Disposed when the form gets Disposed.</para>
		/// <para>Note that this is not the same as the layout manager's container control.</para></remarks>
		public FlowLayout(IContainer container)
			: this()
		{
			if(container != null)
				container.Add(this);
		}
		/// <summary>
		/// Creates a new instance of the FlowLayout component and sets its <see cref="ContainerControl"/>.
		/// </summary>
		public FlowLayout(Control container)
			:this()
		{
			ContainerControl = container;
		}
		/// <summary>
		/// Creates a new instance of the FlowLayout component and sets its <see cref="ContainerControl"/>,
		/// layout mode and alignment.
		/// </summary>
		public FlowLayout(Control container, FlowLayoutMode layoutMode, FlowAlignment align)
			:this(container)
		{
			this.layoutMode = layoutMode;
			this.alignment = align;
		}
		/// <summary>
		/// Creates a new instance of the FlowLayout component and sets its <see cref="ContainerControl"/>,
		/// layout mode, alignment, horizontal gap and vertical gap.
		/// </summary>
		public FlowLayout(Control container, FlowLayoutMode layoutMode, FlowAlignment align, int hGap, int vGap)
			:this(container, layoutMode, align)
		{
			this.hGap = hGap;
			this.vGap = vGap;
		}

		/// <override/>
		protected override Size GetStaticPreferredSize(Control control)
		{
			Size prefSize = Size.Empty;
			
			bool prefSizeAvailable = preferredSizes.Contains(control);

			// If no preferred size is available but if there is an AutoLabel associated with this control,
			// then set a preferred size.
			if(!prefSizeAvailable && AutoLabel.AutoLabelMap.Contains(control))
			{
				Size sz = base.GetStaticPreferredSize(control);
				AutoLabel al = AutoLabel.AutoLabelMap[control] as AutoLabel;
				if(al.Position == AutoLabelPosition.Top)
					sz.Height += al.Height;
				else
					sz.Width += al.Width;

				this.SetPreferredSize(control, sz);

				prefSizeAvailable = true;
			}

			if(!prefSizeAvailable)
			{
				if(this.noFillSizes[control] != null)
					prefSize = (Size)this.noFillSizes[control];
				else
					prefSize = control.Size;
			}
			else
				prefSize = (Size)preferredSizes[control];

			// Ignore preferred height if height is not constrained.
			if(this.Alignment == FlowAlignment.ChildConstraints)
			{
				FlowLayoutConstraints c = this.GetConstraintsRef(control);
				if(c.VAlign != VertFlowAlign.Justify)
					prefSize.Height = control.Size.Height;
				if(c.HAlign != HorzFlowAlign.Justify)
					prefSize.Width = control.Size.Width;

//				if(this.LayoutMode == FlowLayoutMode.Vertical)
//				{
//					FlowLayoutConstraints c = this.GetConstraintsRef(control);
//					if(c.VAlign != VertFlowAlign.Justify)
//						prefSize.Height = control.Size.Height;
//				}
//				else if(this.LayoutMode == FlowLayoutMode.Horizontal)
//				{
//					FlowLayoutConstraints c = this.GetConstraintsRef(control);
//					if(c.HAlign != HorzFlowAlign.Justify)
//						prefSize.Width = control.Size.Width;
//				}
			}
			return prefSize;
		}

		/// <override/>
		protected override Size GetStaticMinimumSize(Control control)
		{
			Size minSize = Size.Empty;

			bool minSizeAvailable = minimumSizes.Contains(control);

			// If no preferred size is available but if there is an AutoLabel associated with this control,
			// then set a preferred size.
			if(!minSizeAvailable && AutoLabel.AutoLabelMap.Contains(control))
			{
				Size sz = base.GetStaticMinimumSize(control);
				AutoLabel al = AutoLabel.AutoLabelMap[control] as AutoLabel;
				if(al.Position == AutoLabelPosition.Top)
					sz.Height += al.Height;
				else
					sz.Width += al.Width;

				this.SetMinimumSize(control, sz);

				minSizeAvailable = true;
			}

			if(!minSizeAvailable)
			{
				if(this.noFillSizes[control] != null)
					minSize = (Size)this.noFillSizes[control];
				else
					minSize = control.Size;
			}
			else
				minSize = (Size)minimumSizes[control];

			// Ignore preferred height if height is not constrained.
			if(this.Alignment == FlowAlignment.ChildConstraints)
			{
				if(this.LayoutMode == FlowLayoutMode.Vertical)
				{
					FlowLayoutConstraints c = this.GetConstraintsRef(control);
					if(c.VAlign != VertFlowAlign.Justify)
						minSize.Height = control.Size.Height;
				}
				else if(this.LayoutMode == FlowLayoutMode.Horizontal)
				{
					FlowLayoutConstraints c = this.GetConstraintsRef(control);
					if(c.HAlign != HorzFlowAlign.Justify)
						minSize.Width = control.Size.Width;
				}
			}
			return minSize;
		}

		/// <summary>
		/// Specifies the constraints associated with the specified control.
		/// </summary>
		/// <param name="control">The control for which to set the constraints.</param>
		/// <param name="value">The constraints of the control. NULL to remove the control
		/// from the layout list.</param>
		/// <remarks>
		/// Passing a NULL value will actually remove the component from the layout list.
		/// </remarks>
		public void SetConstraints(Control control, FlowLayoutConstraints value)
		{
			if(value == null)
			{
				controlsMap.Remove(control);
				base.RemoveLayoutComponent(control);
			}
			else
			{
				this.UpdateNoFillSizes(control, value);
				controlsMap[control] = value.Clone();
				base.AddLayoutComponent(control, value);
				if(this.ContainerControl != null)
					this.ContainerControl.PerformLayout();
			}
		}

		/// <summary>
		/// Adds or removes the specified control from the layout list.
		/// </summary>
		/// <param name="control">The control to be added or removed.</param>
		/// <param name="value">True means the control will be added; False will remove it.</param>
		/// <remarks>
		/// This method will be removed in a future version. Instead, use the <see cref="SetConstraints"/> method passing in a
		/// <see cref="FlowLayoutConstraints"/> instance with its <see cref="FlowLayoutConstraints.Active"/>
		/// property set to the appropriate value.
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Use the SetConstraints method instead. Please check the class reference for more information.")]
		public void SetParticipateInLayout(Control control, bool value)
		{
			FlowLayoutConstraints flc = this.GetConstraintsRef(control);
			flc.Active = value;
		}
		/// <summary>
		/// Indicates whether the component is in the layout list.
		/// </summary>
		/// <param name="control">The control whose participation needs to be verified.</param>
		/// <returns>True if it is in the layout list; False otherwise.</returns>
		/// <remarks>
		/// This method will be removed in a future version. Use the <see cref="GetConstraintsRef"/> method to get hold of the
		/// <see cref="FlowLayoutConstraints"/> associated with this control and then check its <see cref="FlowLayoutConstraints.Active"/>
		/// property instead.
		/// </remarks>
		[Category("Appearance"),
		Localizable(true),
		Obsolete("This method will be removed in a future version. Use the GetConstraintsRef method instead. Please check the class reference for more information.")
		]
		public bool GetParticipateInLayout(Control control)
		{
			FlowLayoutConstraints flc = this.GetConstraintsRef(control);
			return flc.Active;
		}
		/// <summary>
		/// Returns a reference to the constraints associated with the specified control.
		/// </summary>
		/// <param name="control">The control with constraints to retrieve.</param>
		/// <returns>A reference to the actual constraints object.</returns>
		/// <remarks>This is the actual object where the manager stores the constraints for 
		/// the control. Hence, making changes to the returned object will affect the 
		/// layout logic.</remarks>
		public FlowLayoutConstraints GetConstraintsRef(Control control) 
		{
			FlowLayoutConstraints constraints = (FlowLayoutConstraints)controlsMap[control];
			if (constraints == null) 
			{
				SetConstraints(control, FlowLayoutConstraints.Default());
				constraints = (FlowLayoutConstraints)controlsMap[control];
			}
			return constraints;
		}

		/// <summary>
		/// Returns the constraints associated with the specified control.
		/// </summary>
		/// <param name="control">The control with constraints to retrieve.</param>
		/// <returns>A clone of the stored constraints object.</returns>
		/// <remarks>
		/// The returned value is a clone which can be used independently by itself.
		/// The changes made to the returned instance will not have any effect on the stored
		/// constraints. Use <see cref="GetConstraintsRef"/> to get hold of the actual constraints object
		/// that is used by the manager.
		/// </remarks>
		[
		Category("Layout Manager"),
		Localizable(true)
		]
		public FlowLayoutConstraints GetConstraints(Control control) 
		{
			return (FlowLayoutConstraints)GetConstraintsRef(control).Clone();
		}

		protected bool ShouldSerializeConstraints(Control control)
		{
			if(this.GetConstraintsRef(control) == FlowLayoutConstraints.Default())
				return false;
			else
				return true;
		}

		protected void ResetConstraints(Control control)
		{
			this.SetConstraints(control, FlowLayoutConstraints.Default());
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.SetPreferredSize"/>.
		/// </summary>
		public override void SetPreferredSize(Control control, Size value)
		{
			base.SetPreferredSize(control, value);
			if(this.DesignMode)
			{
				FlowLayoutConstraints constraints = this.GetConstraintsRef(control);
				if(constraints != null && constraints.HAlign == HorzFlowAlign.Justify)
				{
					// The layout is being deserialized from code.
					// Let us set the noFillSizes, assuming the preferred size was just the
					// previous noFillSize.
					this.noFillSizes[control] = value;
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetLayoutInfo"/>.
		/// </summary>
		protected override void ResetLayoutInfo()
		{
			base.ResetLayoutInfo();
			this.noFillSizes.Clear();
			return;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetPreferredSize"/>.
		/// </summary>
		/// <param name="control"></param>
		public override void ResetPreferredSize(Control control)
		{
			if(this.noFillSizes[control] != null)
				MessageBox.Show("Cannot reset preferred size when the control's HAlign or VAlign is set to Justify.");
			else
				this.preferredSizes.Remove(control);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShouldSerializePreferredSize(Control control)
		{
			return (base.ShouldSerializePreferredSize(control) 
				|| this.noFillSizes[control] != null);
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.ResetMinimumSize"/>.
		/// </summary>
		/// <param name="control"></param>
		public override void ResetMinimumSize(Control control)
		{
			if(this.noFillSizes[control] != null)
				MessageBox.Show("Cannot reset minimum size when the control has a FillType other than FilleMode.None set.");
			else
				this.minimumSizes.Remove(control);
		}

		[EditorBrowsable(EditorBrowsableState.Never), Syncfusion.Documentation.DocumentationExclude()]
		public override bool ShouldSerializeMinimumSize(Control control)
		{
			return (base.ShouldSerializeMinimumSize(control) 
				|| this.noFillSizes[control] != null);
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.OnContainerControlChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnContainerControlChanged(EventArgs e)
		{
			// Make sure to call base class first.
			base.OnContainerControlChanged(e);

			if(this.ContainerControl != null /*&& !this.LoadingDocument*/)
			{
				// Make all the child controls participate in layout management by default.
				foreach(Control control in this.ContainerControl.Controls)
				{
					if(!this.controlsMap.Contains(control))
						this.SetConstraints(control, FlowLayoutConstraints.Default());
				}	
			}
		}

		/// <override/>
		protected override void OnControlAdded(object sender, ControlEventArgs e)
		{
			// Don't have to call LayoutContainer, since this is ususally followed by the Layout event.
			Control control = e.Control;
			if(!this.controlsMap.Contains(control))
				this.SetConstraints(control, FlowLayoutConstraints.Default());
		
			base.OnControlAdded(sender, e);
		}

		/// <summary>
		/// Removes a child component from the layout list.
		/// </summary>
		/// <param name="childControl">The control to be removed.</param>
		/// <remarks>
		/// <para>
		/// You can also pass a LayoutItemBase derived class as the first argument because
		/// it has an implicit type-conversion operator that will provide its corresponding
		/// control object (a place-holder control that allows the LayoutItemBase to participate
		/// in the layout framework seemlessly). In VB, use the LayoutItemBase.ToControl method.
		/// </para>
		/// </remarks>
		/// <override/>
		public override void RemoveLayoutComponent( Control childControl )
		{
			// To remove any constraints set.
			// This method will also call the base.RemoveLayoutComponent.
			SetConstraints( childControl, null );
		}

		private void UpdateNoFillSizes(Control control, FlowLayoutConstraints newConstraints)
		{
			if(this.DesignMode)
			{
				FlowLayoutConstraints oldConstraints = (FlowLayoutConstraints)controlsMap[control];

				// Determine whether adding Justify or removing Justify.
				bool addingJustify = false;
				bool removingJustify = false;
				if(newConstraints != null && (newConstraints.HAlign == HorzFlowAlign.Justify || newConstraints.ProportionalColWidth)
					&& (oldConstraints == null || (oldConstraints.HAlign != HorzFlowAlign.Justify && !oldConstraints.ProportionalColWidth)))
					addingJustify = true;
				if(!addingJustify && (newConstraints != null && (newConstraints.VAlign == VertFlowAlign.Justify || newConstraints.ProportionalRowHeight)
					&& (oldConstraints == null || (oldConstraints.VAlign != VertFlowAlign.Justify && !oldConstraints.ProportionalRowHeight))))
					addingJustify = true;

				if((newConstraints == null || (newConstraints.HAlign != HorzFlowAlign.Justify && !newConstraints.ProportionalColWidth))
					&& oldConstraints != null && (oldConstraints.HAlign == HorzFlowAlign.Justify || oldConstraints.ProportionalColWidth) && this.noFillSizes[control] != null)
					removingJustify = true;
				if(!removingJustify && (newConstraints == null || (newConstraints.VAlign != VertFlowAlign.Justify && !newConstraints.ProportionalRowHeight))
					&& oldConstraints != null && (oldConstraints.VAlign == VertFlowAlign.Justify || oldConstraints.ProportionalRowHeight) && this.noFillSizes[control] != null)
					removingJustify = true;

				// If going from noJustify to Justify store the current sizes.
				if(addingJustify)
				{
					this.noFillSizes[control] = control.Size;
				}
					// If going from Justify to noJustify reset the size of the control.
				else if(removingJustify)
				{
					control.SuspendLayout();
					this.ContainerControl.SuspendLayout();

					if(this.preferredSizes[control] == this.noFillSizes[control])
						this.preferredSizes[control] = null;

					if(this.minimumSizes[control] == this.noFillSizes[control])
						this.minimumSizes[control] = null;

					control.Size = (Size)this.noFillSizes[control];

					this.ContainerControl.ResumeLayout(false);
					control.ResumeLayout(false);
					this.noFillSizes[control] = null;
				}
			}
		}


		// Assuming preferred layout is one single line with children arranged
		// sequentially.
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.PreferredLayoutSize"/>.
		/// </summary>
		/// <returns>Returns the size with the preferred height (when laying out Horizontal)
		/// or size with the preferred width (when laying out Vertical).</returns>
		public override Size PreferredLayoutSize()
		{
			this.layoutForDeterminingPreferredSize = true;
			this.LayoutContainer();
			this.layoutForDeterminingPreferredSize = false;

			return this.lastKnownPreferredSize;

			/*Size size = new Size(0, 0);
			IList controls = GetControls();
			int nmembers = controls.Count;

			for (int i = 0 ; i < nmembers ; i++) 
			{
				Control m = controls[i] as Control;
				if (IsVisible(m)) 
				{
					Size s = GetPreferredSize(m);
					if(this.LayoutMode == FlowLayoutMode.Horizontal)
					{
						size.Height = Math.Max(size.Height, s.Height);
						if (i > 0) 
							size.Width += this.hGap;
						size.Width += s.Width;
					}
					else
					{
						size.Width = Math.Max(size.Width, s.Width);
						if(i > 0)
							size.Height += this.vGap;
						size.Height += s.Height;
					}
				}
			}
			size.Width += (this.HorzNearMargin + this.HorzFarMargin);
			size.Height += (this.TopMargin + this.BottomMargin);
			return size;*/
		}
		// Assuming preferred layout is one single line with children arranged
		// sequentially.
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.MinimumLayoutSize"/>.
		/// </summary>
		public override Size MinimumLayoutSize()
		{
			Size size = new Size(0, 0);
			IList controls = GetControls();
			int nmembers = controls.Count;

			for (int i = 0 ; i < nmembers ; i++) 
			{
				Control m = controls[i] as Control;
				if (this.IsVisible(m)) 
				{
					Size s = GetMinimumSize(m);
					if(this.LayoutMode == FlowLayoutMode.Horizontal)
					{
						size.Height = Math.Max(size.Height, s.Height);
						if (i > 0) 
							size.Width += this.hGap;
						size.Width += s.Width;
					}
					else
					{
						size.Width = Math.Max(size.Width, s.Width);
						if (i > 0) 
							size.Height += this.vGap;
						size.Height += s.Height;
					}
				}
			}
			size.Width += (this.HorzNearMargin + this.HorzFarMargin);
			size.Height += (this.TopMargin + this.BottomMargin);
			return size;
		}

		// Centers the elements in the specified row, if there is any slack.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void MoveComponentsHorizontal(int x, int y, int maxWidth, int deltaWidth, int height, int rowStart, int rowEnd,
			Hashtable prefSizes, Hashtable deltaWidths) 
		{
			Monitor.Enter(this);
			int initialX = x;
			Rectangle containerBounds = this.GetBounds();
			IList controls = GetControls();
			switch (this.alignment) 
			{
				case FlowAlignment.ChildConstraints:
					break;
				case FlowAlignment.Near:
					break;
				case FlowAlignment.Center:
					x += deltaWidth / 2;
					break;
				case FlowAlignment.Far:
					x += deltaWidth;
					break;
			}
			ArrayList leftAlignedControls = new ArrayList();
			ArrayList centerAlignedControls = new ArrayList();
			ArrayList rightAlignedControls = new ArrayList();
			ArrayList justifyAlignedControls = new ArrayList();
			bool justifiedCenterAlignedControls = false;
			bool justifiedRightAlignedControls = false;

			int centeredControlsWidth = 0;
			int rightAlignedControlsWidth = 0;

			HorzFlowAlign latestFlowAlign = HorzFlowAlign.Left;

			for (int i = rowStart ; i < rowEnd ; i++) 
			{
				Control m = controls[i] as Control;

				// Don't layout AutoLabels.
				if(m is AutoLabel && !this.ShouldLayoutAutoLabel(m as AutoLabel))
					continue;

				FlowLayoutConstraints flc = this.GetConstraintsRef(m);

				if (this.IsVisible(m) && flc.Active) 
				{
					Size prefSize = (Size)prefSizes[m];
					ControlBounds cb = this.GetChildControlBounds(m, prefSize);
					AutoLabel al = null;
					AutoLabelAndControl comb = null;

					if(this.alignment != FlowAlignment.ChildConstraints)
					{
						al = this.GetAutoLabel(m);
						if(al != null)
							comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
						else
							comb = new AutoLabelAndControl(cb);

						comb.BeginUpdate();
						comb.Location = new Point(containerBounds.Left + x , containerBounds.Top + y + (height - comb.Height) / 2);
						comb.Width = prefSize.Width;
						comb.Height = prefSize.Height;
						comb.EndUpdate();
						x += this.hGap + comb.Width;
					}
					else
					{
						HorzFlowAlign curAlign = flc.HAlign;
						if(curAlign == HorzFlowAlign.Justify)
						{
							justifyAlignedControls.Add(m);
							curAlign = latestFlowAlign;
							if(latestFlowAlign == HorzFlowAlign.Center)
								justifiedCenterAlignedControls = true;
							else if(latestFlowAlign == HorzFlowAlign.Right)
								justifiedRightAlignedControls = true;
						}

						switch(curAlign)
						{
							case HorzFlowAlign.Center: centerAlignedControls.Add(m);
								al = this.GetAutoLabel(m);
								if(al != null)
									comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
								else
									comb = new AutoLabelAndControl(cb);

								centeredControlsWidth += comb.Width; 
								latestFlowAlign = HorzFlowAlign.Center; break;
							//case HorzFlowAlign.Justify: justifyAlignedControls.Add(m);break;
							case HorzFlowAlign.Left: leftAlignedControls.Add(m);
								latestFlowAlign = HorzFlowAlign.Left; break;
							case HorzFlowAlign.Right: rightAlignedControls.Add(m);
								al = this.GetAutoLabel(m);
								if(al != null)
									comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
								else
									comb = new AutoLabelAndControl(cb);

								rightAlignedControlsWidth += comb.Width;
								latestFlowAlign = HorzFlowAlign.Right; 
								break;
							default: break;
						}
					}
				}
			}

			centeredControlsWidth += (centerAlignedControls.Count * this.hGap);
			rightAlignedControlsWidth += (rightAlignedControls.Count * this.hGap);
			if(this.alignment == FlowAlignment.ChildConstraints)
			{
				int justifiedControlsCount = justifyAlignedControls.Count;

				// Left aligned controls.
				this.AlignComponentsHorizontal(containerBounds, ref x, y, deltaWidth, height, leftAlignedControls, justifyAlignedControls, justifiedControlsCount,
					prefSizes, deltaWidths);

				// Center aligned controls.
				if(centerAlignedControls.Count > 0)
				{
					if(!justifiedCenterAlignedControls)
					{
						int prefCenterX = initialX + (containerBounds.Width - centeredControlsWidth)/2;
						if(prefCenterX > x)
						{
							deltaWidth -=  (prefCenterX - x);
							x = prefCenterX;
						}
					}
					this.AlignComponentsHorizontal(containerBounds, ref x, y, deltaWidth, height, centerAlignedControls, justifyAlignedControls, 
						justifiedControlsCount, prefSizes, deltaWidths);
				}

				// Justified control.
				//this.AlignComponentsHorizontal(containerBounds, ref x, y, deltaWidth, height, justifyAlignedControls, true, justifiedControlsCount, prefSizes, deltaWidths);

				// Right aligned controls.
				if(!justifiedRightAlignedControls)
					x = initialX + maxWidth - rightAlignedControlsWidth + this.hGap;
				this.AlignComponentsHorizontal(containerBounds, ref x, y, deltaWidth, height, rightAlignedControls, justifyAlignedControls, 
					justifiedControlsCount, prefSizes, deltaWidths);
			}

			Monitor.Exit(this);
		}

		private void AlignComponentsHorizontal(Rectangle containerBounds, ref int x, int y, int deltaWidth, int height, ArrayList list,
			ArrayList justifiedControlsList, int justifiedControlsCount, Hashtable prefSizes, Hashtable deltaWidths)
		{
			int totalDeltaWidths = 0;
			if(deltaWidth < 0)
			{
				foreach(Control c in list)
				{
					// If the control is justified.
					if(justifiedControlsList.Contains(c))
						totalDeltaWidths += (int)deltaWidths[c];
				}
			}
			foreach(Control c in list)
			{
				FlowLayoutConstraints flc = this.GetConstraintsRef(c);
				bool justify = flc.HAlign == HorzFlowAlign.Justify;
				Size prefSize = (Size)prefSizes[c];
				ControlBounds cb = this.GetChildControlBounds(c, prefSize);
				AutoLabel al = this.GetAutoLabel(c);
				AutoLabelAndControl comb = null;
				if(al != null)
					comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
				else
					comb = new AutoLabelAndControl(cb);
				
				int top = containerBounds.Top + y;

				if(height > comb.Height)
				{
					switch(flc.VAlign)
					{
						case VertFlowAlign.Center:
						{
							top = top + (height - comb.Height)/2;
						}
							break;
						case VertFlowAlign.Bottom:
						{
							top = top + height - comb.Height;
						}
							break;
					}
				}

				comb.BeginUpdate();
				comb.Location = new Point(containerBounds.Left + x , 
					top);

				// Changing Width & Height and then Location. This reduces flicker in RTL.
				if(this.alignment == FlowAlignment.ChildConstraints
					&& justify)
				{
					if(deltaWidth >= 0)
						comb.Width += (deltaWidth / justifiedControlsCount);
					else
					{
						if((int)deltaWidths[c] != 0)
							comb.Width += ((deltaWidth * (int)deltaWidths[c])/totalDeltaWidths);
					}
				}
				if(flc.VAlign == VertFlowAlign.Justify)
				{
					comb.Height = height;
				}
				comb.EndUpdate();

				x += this.hGap + comb.Width;
			}
		}

		// Centers the elements in the specified column, if there is any slack.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void MoveComponentsVertical(int x, int y, int width, int deltaHeight, int maxHeight, int rowStart, int rowEnd,
			Hashtable prefSizes, Hashtable deltaHeights) 
		{
			Monitor.Enter(this);
			int initialY = y;
			Rectangle containerBounds = this.GetBounds();
			IList controls = GetControls();
			switch (this.alignment) 
			{
				case FlowAlignment.ChildConstraints:
					break;
				case FlowAlignment.Near:
					break;
				case FlowAlignment.Center:
					y += deltaHeight / 2;
					break;
				case FlowAlignment.Far:
					y += deltaHeight;
					break;
			}
			ArrayList topAlignedControls = new ArrayList();
			ArrayList centerAlignedControls = new ArrayList();
			ArrayList bottomAlignedControls = new ArrayList();
			ArrayList justifyAlignedControls = new ArrayList();
			bool justifiedCenterAlignedControls = false;
			bool justifiedRightAlignedControls = false;

			int centeredControlsHeight = 0;
			int bottomAlignedControlsHeight = 0;

			VertFlowAlign latestFlowAlign = VertFlowAlign.Top;

			for (int i = rowStart ; i < rowEnd ; i++) 
			{
				Control control = controls[i] as Control;

				// Don't layout AutoLabels.
				if(control is AutoLabel && !this.ShouldLayoutAutoLabel(control as AutoLabel))
					continue;

				FlowLayoutConstraints flc = this.GetConstraintsRef(control);

				if (this.IsVisible(control) && flc.Active) 
				{
					Size prefSize = (Size)prefSizes[control];
					ControlBounds cb = this.GetChildControlBounds(control, prefSize);
					AutoLabel al = null;
					AutoLabelAndControl comb = null;

					if(this.alignment != FlowAlignment.ChildConstraints)
					{
						al = this.GetAutoLabel(control);
						if(al != null)
							comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
						else
							comb = new AutoLabelAndControl(cb);

						comb.BeginUpdate();
						comb.Location = new Point(containerBounds.Left + x + (width - comb.Width) / 2, containerBounds.Top + y);
						comb.Width = prefSize.Width;
						comb.Height = prefSize.Height;
						comb.EndUpdate();
						y += this.vGap + comb.Height;
					}
					else
					{
						VertFlowAlign curAlign = flc.VAlign;
						if(curAlign == VertFlowAlign.Justify)
						{
							justifyAlignedControls.Add(control);
							curAlign = latestFlowAlign;
							if(latestFlowAlign == VertFlowAlign.Center)
								justifiedCenterAlignedControls = true;
							else if(latestFlowAlign == VertFlowAlign.Bottom)
								justifiedRightAlignedControls = true;
						}

						switch(curAlign)
						{
							case VertFlowAlign.Center: centerAlignedControls.Add(control);
								al = this.GetAutoLabel(control);
								if(al != null)
									comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
								else
									comb = new AutoLabelAndControl(cb);
								centeredControlsHeight += comb.Height; 
								latestFlowAlign = VertFlowAlign.Center;break;
							//case VertFlowAlign.Justify: justifyAlignedControls.Add(control);break;
							case VertFlowAlign.Top: topAlignedControls.Add(control);
								latestFlowAlign = VertFlowAlign.Top;break;
							case VertFlowAlign.Bottom: bottomAlignedControls.Add(control);
								al = this.GetAutoLabel(control);
								if(al != null)
									comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
								else
									comb = new AutoLabelAndControl(cb);
								bottomAlignedControlsHeight += comb.Height;
								latestFlowAlign = VertFlowAlign.Bottom;
								break;
							default: break;
						}
					}
				}
			}
			centeredControlsHeight += (centerAlignedControls.Count * this.vGap);
			bottomAlignedControlsHeight += (bottomAlignedControls.Count * this.vGap);
			if(this.alignment == FlowAlignment.ChildConstraints)
			{
				int justifiedControlsCount = justifyAlignedControls.Count;

				// Top aligned controls.
				this.AlignComponentsVertical(containerBounds, x, ref y, deltaHeight, width, topAlignedControls, justifyAlignedControls, justifiedControlsCount, 
					prefSizes, deltaHeights);

				// Center aligned controls.
				if(centerAlignedControls.Count > 0)
				{
					if(!justifiedCenterAlignedControls)
					{
						int prefCenterY = initialY + (containerBounds.Height - centeredControlsHeight)/2;
						if(prefCenterY > y)
						{
							deltaHeight -=  (prefCenterY - y);
							y = prefCenterY;
						}
					}
					this.AlignComponentsVertical(containerBounds, x, ref y, deltaHeight, width, centerAlignedControls, justifyAlignedControls, justifiedControlsCount, prefSizes, deltaHeights);
				}

				// Justified control.
				//this.AlignComponentsVertical(containerBounds, x, ref y, deltaHeight, width, justifyAlignedControls, true, justifiedControlsCount, prefSizes, deltaHeights);

				// Bottom aligned controls.
				if(!justifiedRightAlignedControls)
					y = initialY + maxHeight - bottomAlignedControlsHeight + this.vGap;

				this.AlignComponentsVertical(containerBounds, x, ref y, deltaHeight, width, bottomAlignedControls, justifyAlignedControls, justifiedControlsCount, prefSizes, deltaHeights);
			}
			Monitor.Exit(this);
		}

		private void AlignComponentsVertical(Rectangle containerBounds, int x, ref int y, int deltaHeight, int width, ArrayList list,
			ArrayList justifiedControlsList, int justifiedControlsCount, Hashtable prefSizes, Hashtable deltaHeights)
		{
			int totalDeltaHeights = 0;
			if(deltaHeight < 0)
			{
				foreach(Control c in list)
				{
					// If the control is justified.
					if(justifiedControlsList.Contains(c))
						totalDeltaHeights += (int)deltaHeights[c];
				}
			}
			foreach(Control c in list)
			{
				FlowLayoutConstraints flc = this.GetConstraintsRef(c);
				bool justify = flc.VAlign == VertFlowAlign.Justify;
				Size prefSize = (Size)prefSizes[c];
				ControlBounds cb = this.GetChildControlBounds(c, prefSize);
				AutoLabel al = this.GetAutoLabel(c);
				AutoLabelAndControl comb = null;
				if(al != null)
					comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
				else
					comb = new AutoLabelAndControl(cb);
				
				//int top = containerBounds.Top + y;
				int left = containerBounds.Left + x;

				if(width > comb.Width)
				{
					switch(flc.HAlign)
					{
						case HorzFlowAlign.Center:
						{
							left = left + (width - comb.Width)/2;
						}
							break;
						case HorzFlowAlign.Right:
						{
							left = left + width - comb.Width;
						}
							break;
					}
				}

				comb.BeginUpdate();
				comb.Location = new Point(left, 
					containerBounds.Top + y);

				if(this.alignment == FlowAlignment.ChildConstraints
					&& justify)
				{
					if(deltaHeight >= 0)
						comb.Height += (deltaHeight / justifiedControlsCount);
					else
					{
						if((int)deltaHeights[c] != 0)
							comb.Height += ((deltaHeight * (int)deltaHeights[c])/totalDeltaHeights);
					}
				}
				if(flc.HAlign == HorzFlowAlign.Justify)
				{
					comb.Width = width;
				}
				comb.EndUpdate();

				y += this.vGap + comb.Height;
			}
		}
		private int recurseCount = 0;

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void LayoutContainerHorizontal()
		{
			Monitor.Enter(this);
			recurseCount++;
			if(recurseCount > 2)
			{
				recurseCount--;
				return;
			}

			Rectangle bounds = this.GetBounds();
			IList controls = GetControls();
			
			int maxWidth = bounds.Width;
			int nmembers = controls.Count;

			int x = 0, y = 0, shrinkableWidth = 0;
			int rowh = 0, start = 0;

			ArrayList rowHeights = new ArrayList();
			ArrayList rowWidths = new ArrayList();
			ArrayList rowBeginners = new ArrayList();
			ArrayList rowProportionalHeightRequired = new ArrayList();
			bool needPropHeightForLatestRow = false;
			bool atleastOneRowWithPropHeightRequirement = false;
			Hashtable htPrefSizes = new Hashtable();
			Hashtable htDeltaWidths = new Hashtable();

			for (int i = 0 ; i < nmembers ; i++) 
			{
				Control m = controls[i]  as Control;
				// Don't layout AutoLabels.
				if(m is AutoLabel && !this.ShouldLayoutAutoLabel(m as AutoLabel))
					continue;

				FlowLayoutConstraints flc = this.GetConstraintsRef(m);

				if (this.IsVisible(m) && flc.Active) 
				{
					Size d = this.GetPreferredSize(m);
					Size minSize = this.GetMinimumSize(m);
					htPrefSizes[m] = d;
					htDeltaWidths[m] = d.Width - minSize.Width;
					//m.Size = d;
					ControlBounds cb = this.GetChildControlBounds(m, d);
					ControlBounds cb2 = this.GetChildControlBounds(m, minSize);
					
					AutoLabel al = this.GetAutoLabel(m);
					AutoLabelAndControl comb = null;
					AutoLabelAndControl comb2 = null;

					if(al != null)
					{
						comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
					}
					else
						comb = new AutoLabelAndControl(cb);

					if(al != null)
						comb2 = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb2);
					else
						comb2 = new AutoLabelAndControl(cb2);

					d = new Size(comb.Width, comb.Height);
					minSize = new Size(comb2.Width, comb2.Height);

					if(this.alignment == FlowAlignment.ChildConstraints
						&& flc.HAlign == HorzFlowAlign.Justify)
						shrinkableWidth += (d.Width - minSize.Width);

					if ((x == 0) || 
						((x + d.Width - shrinkableWidth) <= (maxWidth - hGap) 
							&& !flc.NewLine)) 
					{
						if (x > 0) 
						{
							x += this.hGap;
						}
						x += d.Width;
						
						rowh = Math.Max(rowh, d.Height);
						if(flc.ProportionalRowHeight)
							needPropHeightForLatestRow = true;
					} 
					else 
					{
						//this.MoveComponentsHorizontal(hGap, y, maxWidth, maxWidth - x, rowh, start, i);
						rowHeights.Add(rowh);
						rowWidths.Add(x);
						rowBeginners.Add(start);
						rowProportionalHeightRequired.Add(needPropHeightForLatestRow);
						atleastOneRowWithPropHeightRequirement |= needPropHeightForLatestRow;
						needPropHeightForLatestRow = false;

						x = d.Width;
						shrinkableWidth = 0;
						y += vGap + rowh;
						rowh = d.Height;
						start = i;
						if(flc.ProportionalRowHeight)
							needPropHeightForLatestRow = true;
					}
				}
			}
			//this.MoveComponentsHorizontal(/*insets.Near +*/ hGap, y, maxWidth, maxWidth - x, rowh, start, nmembers);
			rowHeights.Add(rowh);
			rowWidths.Add(x);
			rowBeginners.Add(start);
			rowProportionalHeightRequired.Add(needPropHeightForLatestRow);
			atleastOneRowWithPropHeightRequirement |= needPropHeightForLatestRow;
			
			if(!this.layoutForDeterminingPreferredSize)
			{
				if(atleastOneRowWithPropHeightRequirement)
				{
					// Determine used height.
					int usedHeight = this.GetUsedHeight(rowHeights);

					int deltaHeight = bounds.Height - usedHeight;

					// Distribute deltaHeight among rows requiring proportional height.
					int totalHeightOfRowsRequiringPropHeight = 0;
					for(int i = 0; i < rowProportionalHeightRequired.Count; i++)
					{
						if((bool)rowProportionalHeightRequired[i] == true)
							totalHeightOfRowsRequiringPropHeight += (int)rowHeights[i];
					}
					for(int i = 0; i < rowProportionalHeightRequired.Count; i++)
					{
						if((bool)rowProportionalHeightRequired[i] == true)
						{
							int h = (int)rowHeights[i];
							rowHeights[i] = h + (h*deltaHeight)/totalHeightOfRowsRequiringPropHeight;
						}
					}
				}

				y = 0;
				if(this.ReverseRows)
				{
					y = bounds.Height;
				}

				for(int i = 0; i < rowHeights.Count; i++)
				{
					int rh = (int) rowHeights[i];
					int rw = (int) rowWidths[i];
					int rowEnd = -1;

					if((i+1) == rowHeights.Count)
						rowEnd = nmembers;
					else
						rowEnd = (int)rowBeginners[i+1];

					int top = 0;
					if(ReverseRows)
						top = y - rh;
					else
						top = y;

                    if( maxWidth > 0 )
					{
                        this.MoveComponentsHorizontal( 0, top, maxWidth, maxWidth - rw, rh, (int)rowBeginners[i], rowEnd, htPrefSizes, htDeltaWidths );
                    }

					if(ReverseRows)
						y -= (vGap + rh);
					else
						y += vGap + rh;
				}

				if(this.autoHeight && this.CustomLayoutBounds == Rectangle.Empty)
				{
					int usedHeight = this.GetUsedHeight(rowHeights);
				
					if(bounds.Height != usedHeight)
					{
						int newHeight = this.AdjustHeightForMargins(usedHeight);
						this.ContainerControl.SuspendLayout();
						this.ContainerControl.Height = newHeight;
						this.ContainerControl.ResumeLayout(false);
						this.LayoutContainerHorizontal();

					}
				}
			}
			else
			{
				int usedHeight = this.GetUsedHeight(rowHeights);
				this.lastKnownPreferredSize = new Size(this.AdjustWidthForMargins(bounds.Width), this.AdjustHeightForMargins(usedHeight));
			}

			this.recurseCount--;
			Monitor.Exit(this);
		}

		private int GetUsedHeight(ArrayList rowHeights)
		{
			int usedHeight = 0;
			foreach(int h in rowHeights)
			{
				usedHeight += h;
			}
			usedHeight += ((vGap * (rowHeights.Count - 1)));
			return usedHeight;
		}
		private int GetUsedWidth(ArrayList rowWidths)
		{
			int usedWidth = 0;
			foreach(int w in rowWidths)
			{
				usedWidth += w;
			}
			usedWidth += ((hGap * (rowWidths.Count - 1)));
			return usedWidth;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void LayoutContainerVertical()
		{
			Monitor.Enter(this);
			recurseCount++;
			if(recurseCount > 2)
			{
				recurseCount--;
				return;
			}

			Rectangle bounds = this.GetBounds();
			IList controls = GetControls();
			
			int maxHeight = bounds.Height;
			int nmembers = controls.Count;

			int x = 0, y = 0, shrinkableHeight = 0;
			int colw = 0, start = 0;

			ArrayList colWidths = new ArrayList();
			ArrayList colHeights = new ArrayList();
			ArrayList colBeginners = new ArrayList();
			ArrayList colProportionalWidthRequired = new ArrayList();
			bool needPropWidthForLatestCol = false;
			bool atleastOneColWithPropWidthRequirement = false;
			Hashtable htPrefSizes = new Hashtable();
			Hashtable htDeltaHeights = new Hashtable();

			for (int i = 0 ; i < nmembers ; i++) 
			{
				Control m = controls[i]  as Control;

				// Don't layout AutoLabels.
				if(m is AutoLabel && !this.ShouldLayoutAutoLabel(m as AutoLabel))
					continue;

				FlowLayoutConstraints flc = this.GetConstraintsRef(m);

				if (this.IsVisible(m) && flc.Active) 
				{
					Size d = this.GetPreferredSize(m);
					Size minSize = this.GetMinimumSize(m);
					htPrefSizes[m] = d;
					htDeltaHeights[m] = d.Height - minSize.Height;
					//m.Size = d;
	
					ControlBounds cb = this.GetChildControlBounds(m, d);
					ControlBounds cb2 = this.GetChildControlBounds(m, minSize);

					AutoLabel al = this.GetAutoLabel(m);
					AutoLabelAndControl comb = null;
					AutoLabelAndControl comb2 = null;

					if(al != null)
						comb = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb);
					else
						comb = new AutoLabelAndControl(cb);

					if(al != null)
						comb2 = new AutoLabelAndControl(this.GetChildControlBounds(al, al.Size), al.Position, al.DX, al.DY, cb2);
					else
						comb2 = new AutoLabelAndControl(cb2);
					
					d = new Size(comb.Width, comb.Height);
					minSize = new Size(comb2.Width, comb2.Height);

					if(this.alignment == FlowAlignment.ChildConstraints
						&& flc.VAlign == VertFlowAlign.Justify)
						shrinkableHeight += (d.Height - minSize.Height);

					if ((y == 0) || 
						((y + d.Height - shrinkableHeight) <= (maxHeight - vGap) 
							&& !flc.NewLine)) 
					{
						if (y > 0) 
						{
							y += this.vGap;
						}
						y += d.Height;
						colw = Math.Max(colw, d.Width);
						if(flc.ProportionalColWidth)
							needPropWidthForLatestCol = true;
					} 
					else 
					{
						//this.MoveComponentsVertical(x, 0, colw, maxHeight - y, maxHeight, start, i);
						colWidths.Add(colw);
						colHeights.Add(y);
						colBeginners.Add(start);
						colProportionalWidthRequired.Add(needPropWidthForLatestCol);
						atleastOneColWithPropWidthRequirement |= needPropWidthForLatestCol;
						needPropWidthForLatestCol = false;

						y = d.Height;
						shrinkableHeight = 0;
						x += hGap + colw;
						colw = d.Width;
						start = i;

						if(flc.ProportionalColWidth)
							needPropWidthForLatestCol = true;
					}
				}
			}
			//this.MoveComponentsVertical(x, 0, colw, maxHeight - y, maxHeight, start, nmembers);
			colWidths.Add(colw);
			colHeights.Add(y);
			colBeginners.Add(start);
			colProportionalWidthRequired.Add(needPropWidthForLatestCol);
			atleastOneColWithPropWidthRequirement |= needPropWidthForLatestCol;
			
			if(!this.layoutForDeterminingPreferredSize)
			{
				if(atleastOneColWithPropWidthRequirement)
				{
					// Determine used width.
					int usedWidth = this.GetUsedWidth(colWidths);

					int extraWidth = bounds.Width - usedWidth;

					// Distribute extraWidth among cols requiring proportional width.
					int totalWidthOfColsRequiringPropWidth = 0;
					for(int i = 0; i < colProportionalWidthRequired.Count; i++)
					{
						if((bool)colProportionalWidthRequired[i] == true)
							totalWidthOfColsRequiringPropWidth += (int)colWidths[i];
					}
					for(int i = 0; i < colProportionalWidthRequired.Count; i++)
					{
						if((bool)colProportionalWidthRequired[i] == true)
						{
							int w = (int)colWidths[i];
							colWidths[i] = w + (w*extraWidth)/totalWidthOfColsRequiringPropWidth;
						}
					}
				}

				x = 0;
				if(this.ReverseRows)
				{
					x = bounds.Width;
				}
				for(int i = 0; i < colWidths.Count; i++)
				{
					int cw = (int) colWidths[i];
					int ch = (int) colHeights[i];
					int colEnd = -1;
					if((i+1) == colWidths.Count)
						colEnd = nmembers;
					else
						colEnd = (int)colBeginners[i+1];

					int left = 0;
					if(ReverseRows)
						left = x - cw;
					else
						left = x;

					this.MoveComponentsVertical(left, 0, cw, maxHeight - ch, maxHeight, (int)colBeginners[i], colEnd,
						htPrefSizes, htDeltaHeights);

					if(ReverseRows)
						x -= (hGap + cw);
					else
						x += hGap + cw;
				}
			}
			else
			{
				int usedWidth = this.GetUsedWidth(colWidths);
				this.lastKnownPreferredSize = new Size(this.AdjustWidthForMargins(usedWidth), 
					this.AdjustHeightForMargins(bounds.Height));
			}

			this.recurseCount--;
			Monitor.Exit(this);
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.LayoutManager.LayoutContainer"/>.
		/// </summary>
		public override void LayoutContainer()
		{
			if( !this.IsInit() )
				return;

			if( !this.initializing )
			{
				Size sz = this.ContainerControl.Size;

				this.ContainerControl.SuspendLayout();

				if( this.layoutMode == FlowLayoutMode.Vertical )
					LayoutContainerVertical();
				else
					LayoutContainerHorizontal();

				this.ContainerControl.ResumeLayout( false );

				if( this.ContainerControl.Size != sz && this.ContainerControl.Parent != null && this.ContainerControl.Parent.IsHandleCreated )
				{
					this.ContainerControl.Parent.BeginInvoke( new MethodInvoker( this.ContainerControl.Parent.PerformLayout ) );
				}
			}
		}

		#region Events

		/// <summary>
		/// Occurs when <see cref="FlowLayout.HGap"/> property is changed.
		/// </summary>
        [Description("Occurs when HGap property  is changed.")]
		public event ValueChangedEventHandler HGapChanged;

		/// <summary>
		/// Occurs when <see cref="FlowLayout.VGap"/> property is changed.
		/// </summary>
        [Description ("Occurs when VGap property  is changed.")]
		public event ValueChangedEventHandler VGapChanged;

		protected void OnGapChanged( ValueChangedEventHandler gapChangedHandler, ValueChangedEventArgs args )
		{
			if( gapChangedHandler != null )
			{
				gapChangedHandler( this, args );
			}
		}

		#endregion
	}

	internal class AutoLabelAndControl
	{
		private ControlBounds al; 
		private ControlBounds c;
		private AutoLabelPosition alPosition;
		int x, y, width, height, alDX, alDY;
		bool updating = false;

		internal AutoLabelAndControl(ControlBounds c)
		{
			this.al = null;
			this.c = c;
			this.Init();
		}

		internal AutoLabelAndControl(ControlBounds al, AutoLabelPosition position, int dx, int dy, ControlBounds c)
		{
			this.al = al;
			this.c = c;
			this.alPosition = position;
			this.alDX = dx;
			this.alDY = dy;
			this.Init();
		}

		public void BeginUpdate()
		{
			if(updating)
				throw new Exception("Multiple BeginUpdates not allowed on AutoLabelAndControl.");
			this.c.BeginUpdate();
			updating = true;
		}
		public void EndUpdate()
		{
			if(!updating)
				throw new Exception("EndUpdate called without calling BeginUpdate in AutoLabelAndControl.");

			updating = false;
			this.c.EndUpdate();
			// This is necessary since the AutoLabel bounds would have changed when the control's bounds
			// changed.
			if(al != null)
				this.al.ReinitLocation();
			this.Init();
		}

		private void Init()
		{
			if(al != null)
			{
				if(this.alPosition == AutoLabelPosition.Side)
				{
					x = al.Location.X;
					if(c.Location.Y < al.Location.Y)
						y = c.Location.Y;
					else
						y = al.Location.Y;

					width = c.Width + (-this.alDX);
					height = Math.Max(al.Height, c.Height);
				}
				else if(this.alPosition == AutoLabelPosition.Top)
				{
					y = al.Location.Y;
					if(c.Location.X < al.Location.X)
						x = c.Location.X;
					else
						x = al.Location.X;

					height = c.Height + (-this.alDY);
					width = Math.Max(al.Width, c.Width);
				}
				else
				{
					// height
					if(c.Location.Y < al.Location.Y)
					{
						y = c.Location.Y;
						if(al.Bounds.Bottom > c.Bounds.Bottom)
							height = al.Bounds.Bottom - c.Location.Y;
						else
							height = c.Height;
					}
					else
					{
						y = al.Location.Y;
						if(c.Bounds.Bottom > al.Bounds.Bottom)
							height = c.Bounds.Bottom - al.Location.Y;
						else
							height = al.Height;
					}

					// width
					if(c.Location.X < al.Location.X)
					{
						x = c.Location.X;
						if(al.Bounds.Right > c.Bounds.Right)
							width = al.Bounds.Right - c.Location.X;
						else
							width = c.Width;
					}
					else
					{
						x = al.Location.X;
						if(c.Bounds.Right > al.Bounds.Right)
							width = c.Bounds.Right - al.Location.X;
						else
							width = al.Width;
					}

				}
			}
			else
			{
				x = c.Location.X;
				y = c.Location.Y;
				width = c.Width;
				height = c.Height;
			}
		}
		public Point Location
		{
			get
			{
				return new Point(x, y);
			}
			set
			{
				if(this.Location != value)
				{
					int dx = x - value.X;
					x -= dx;

					int dy = y - value.Y;
					y -= dy;
					
					//if(this.al != null)
					//	this.al.Location = new Point(al.Location.X - dx, al.Location.Y - dy);
					this.c.Location = new Point(c.Location.X - dx, c.Location.Y - dy);
				}
			}
		}
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				// Don't check non-equality here since, we need to set c.Height which will
				// actualize the height on the control.
				//if(this.Height != value)
				{
					int dy = value - this.Height;
					this.c.Height += dy;
					if(!this.updating)
						this.Init();
				}
			}
		}
		public int Width
		{
			get{return this.width;}
			set
			{
				// Don't check in-equality here since, we need to set c.Width which will
				// actualize the width on the control.
				//if(this.Width != value)
				{
					int dx = value - this.Width;
					this.c.Width += dx;
					if(!this.updating)
						this.Init();
				}
			}
		}
	}
}
