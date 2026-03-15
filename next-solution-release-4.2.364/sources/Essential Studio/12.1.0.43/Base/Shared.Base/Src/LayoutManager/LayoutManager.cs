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
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools
{
	[
	ToolboxItem(false),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class LayoutItemPlaceHolderControl : Control, IProvideLayoutInformation
	{
		private LayoutItemBase layoutItem;
		public LayoutItemPlaceHolderControl(LayoutItemBase layoutItem)
		{
			this.layoutItem = layoutItem;
		}
		public LayoutItemBase LayoutComponent
		{
			get{return this.layoutItem;}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[Obsolete("Use LayoutPreferredSize instead. There was a naming conflict with Whidbeys Control.PreferredSize property.")]
		public new Size PreferredSize
#else
		public Size PreferredSize 
#endif
		{
			get
			{
				return this.layoutItem.PreferredSize;
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[Obsolete("Use LayoutMinimumSize instead. There was a naming conflict with Whidbeys Control.PreferredSize property.")]
		public new Size MinimumSize
#else
		public Size MinimumSize 
#endif
		{
			get
			{
				return this.layoutItem.MinimumSize;
			}
		}

		public Size LayoutPreferredSize
		{
			get
			{
				return this.layoutItem.PreferredSize;
			}
		}

		public Size LayoutMinimumSize
		{
			get
			{
				return this.layoutItem.MinimumSize;
			}
		}

		bool visible;
		public virtual new bool Visible
		{
			get{return visible;}
			set{base.Visible = value;}
		}
		// Intercept base Control's "Control Visibility updating" logic.
		/// <override/>
		protected override void SetVisibleCore(bool value)
		{
			if(this.visible != value)
				this.visible = value;

			base.OnLocationChanged(EventArgs.Empty);
		}
		protected internal LayoutManager LayoutManager
		{
			set{layoutItem.LayoutManager = value;}
		}
	}

	/// <summary>
	/// Represents a non-control based layout component.
	/// </summary>
	/// <remarks>
	/// <para>Derive your non-control based components from this class if you want them to
	/// participate in the layout management.</para>
	/// <para>You can add such components to the manager using the same methods as the control
	/// derived classes. You can pass a LayoutItemBase derived class to any method that
	/// expects a control type argument since the LayoutItemBase has an implicit type-conversion operator
	/// that can convert itself to a control. In VB, use the <see cref="ToControl"/> method to convert this instance to a control.</para>
	/// <para>In your derived class, you can find out the size set by the layout manager
	/// through the <see cref="Bounds"/> property and the visibility through the Visible property (listening
	/// for the <see cref="BoundsChanged"/> event should also help). You
	/// should also provide the preferred size and minimum size of your component through
	/// the <see cref="PreferredSize"/> and <see cref="MinimumSize"/> overrides.</para>
	/// </remarks>
	/// <example>
	/// This first example shows a sample LayoutItemBase derived class:
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\cs\NonControlBasedComponents.cs" name="Sample LayoutItemBase Derived Class" lang="C#"><code lang="C#">
	///	public class MyRectangle : LayoutItemBase
	///	{
	///		public static Size PrefSize = new Size(0, 0);
	///		protected Control parent;
	///		protected Color color;
	///		protected string text;
	///		public MyRectangle(Control parent, Color color, string text)
	///		{
	///			this.parent = parent;
	///			this.color = color;
	///			this.text = text;
	///		}
	///		public void OnPaint( PaintEventArgs e)
	///		{
	///			e.Graphics.FillRectangle(new SolidBrush(color), this.Bounds);
	///			StringFormat sf = new StringFormat();
	///			sf.Alignment = StringAlignment.Center;
	///			sf.LineAlignment = StringAlignment.Center;
	///			RectangleF r = new RectangleF(Bounds.Left, Bounds.Top,
	///				Bounds.Width, Bounds.Height);
	///			e.Graphics.DrawString(text, Control.DefaultFont, SystemBrushes.ControlText, r, sf);
	///		}
	///		// This override is a good place to repaint.
	///		// Or you can listen to BoundsChanged event in LayoutItemBase.
	///		protected override void OnBoundsChanged()
	///		{
	///			parent.Invalidate(new Rectangle(0, 0, this.parent.Width, this.parent.Height));
	///		}
	///
	///		public override System.Drawing.Size MinimumSize
	///		{
	///			get	{	return MyRectangle.PrefSize;	}
	///		}
	///
	///		public override System.Drawing.Size PreferredSize
	///		{
	///			get	
	///			{
	///				return MyRectangle.PrefSize;
	///			}
	///		}
	///	}</code></coderef>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\vb\MyRectangle.vb" name="Sample LayoutItemBase Derived Class" lang="VB"><code lang="VB">
	///    Public Class MyRectangle
	///        Inherits LayoutItemBase
	///        Protected WithEvents parent As Control
	///        Protected color As color
	///        Protected [text] As String
	///        Public Shared PrefSize As Size
	///        'Fields
	///        'Constructors
	///        'Events
	///        'Methods
	///        Shared Sub New()
	///            'Warning: Implementation not found
	///        End Sub
	///        Public Sub New(ByVal parent As Control, ByVal color As color, ByVal [text] As String)
	///            MyBase.New()
	///            Me.parent = parent
	///            Me.color = color
	///            Me.text = [text]
	///
	///        End Sub
	///        Public Overrides ReadOnly Property MinimumSize() As Size
	///            Get
	///
	///                Return MyRectangle.PrefSize
	///
	///            End Get
	///        End Property
	///        Public Overrides ReadOnly Property PreferredSize() As Size
	///            Get
	///
	///                Return MyRectangle.PrefSize
	///
	///            End Get
	///        End Property
	///        Protected Overloads Overrides Sub OnBoundsChanged()
	///
	///            parent.Invalidate(New Rectangle(0, 0, Me.parent.Width, Me.parent.Height))
	///
	///        End Sub
	///        Public Sub OnPaint(ByVal e As PaintEventArgs)
	///
	///            e.Graphics.FillRectangle(New SolidBrush(color), Me.Bounds)
	///            Dim sf As StringFormat
	///            sf = New StringFormat()
	///            sf.Alignment = StringAlignment.Center
	///            sf.LineAlignment = StringAlignment.Center
	///            Dim r As RectangleF
	///            r = New RectangleF(Me.Bounds.Left, Me.Bounds.Top, Me.Bounds.Width, Me.Bounds.Height)
	///            e.Graphics.DrawString([text], Control.DefaultFont, SystemBrushes.ControlText, r, sf)
	///
	///        End Sub
	///    End Class</code></coderef>
    /// <para>The above class can then participate in layout as follows. The example assumes
	/// that there is a GridBagLayout manager that is already bound to a container.</para>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\cs\NonControlBasedComponents.cs" name="Initializing LayoutItemBase derived class" lang="C#"><code lang="C#">
	///		private void Form1_Load(object sender, System.EventArgs e)
	///		{
	///			this.SuspendLayout();
	///			// Current layout manager (Update every time you change the manager)
	///			
	///			// Layout Component 1:
	///			this.myRect1 = new MyRectangle(this.gridBagLayout1.ContainerControl, Color.FromArgb(133, 191, 117), "Paint Area 1");
	///			this.myRect1.Bounds = new Rectangle(10, 10, 80, 20);
	///			this.myRect1.Visible = true;
	///
	///			// Layout Component 2:
	///			this.myRect2 = new MyRectangle(this.gridBagLayout1.ContainerControl, Color.FromArgb(222, 100, 19), "Paint Area 2");
	///			this.myRect2.Bounds = new Rectangle(10, 40, 80, 20);
	///			this.myRect2.Visible = true;
	///
	///			// Layout Component 3:
	///			this.myRect3 = new MyRectangle(this.gridBagLayout1.ContainerControl, Color.FromArgb(196, 214, 233), "Paint Area 3");
	///			this.myRect3.Bounds = new Rectangle(10, 70, 80, 20);
	///			this.myRect3.Visible = true;
	///
	///
	///			// Sample GridBagConstraints:
	///			GridBagConstraints gbc1 = new GridBagConstraints();
	///			GridBagConstraints gbc2 = new GridBagConstraints();
	///			GridBagConstraints gbc3 = new GridBagConstraints();
	///
	///			gbc1.Fill = FillType.Both;
	///			gbc1.WeightX = 0.2;
	///			gbc1.WeightY = 0.5;
	///			gbc1.GridPosX = 0;
	///			gbc1.GridPosY = 0;
	///
	///			gbc2.Fill = FillType.Both;
	///			gbc2.WeightX = 0.2;
	///			gbc2.WeightY = 0.5;
	///			gbc2.GridPosX = 1;
	///			gbc2.GridPosY = 0;
	///
	///			gbc3.Fill = FillType.Both;
	///			gbc3.WeightX = 0.4;
	///			gbc3.WeightY = 0.5;
	///			gbc3.GridPosX = 0;
	///			gbc3.GridPosY = 1;
	///			gbc3.CellSpanX = 2;
	///
	///			// Add all the components that are to participate in Layout Management.
	///			
	///			// For GridBagLayouts pass gbcs for GridBagLayouts:
	///			this.gridBagLayout1.SetConstraints(this.myRect1.ToControl(), gbc1);	
	///			this.gridBagLayout1.SetConstraints(this.myRect2.ToControl(), gbc2);
	///			this.gridBagLayout1.SetConstraints(this.myRect3.ToControl(), gbc3);
	///
	///			this.ResumeLayout(true);
	///		}</code></coderef>
	/// <coderef file="\Tools\Samples\Quick Start\LayoutManagers\vb\NonControlBasedComponents.vb" name="Initializing LayoutItemBase derived class" lang="VB"><code lang="VB">
	///        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
	///
	///            Me.SuspendLayout()
	///            ' Current layout manager (Update every time you change the manager)
	///            ' Layout Component 1:
	///            Me.myRect1 = New MyRectangle(Me.gridBagLayout1.ContainerControl, Color.FromArgb(133, 191, 117), "Paint Area 1")
	///            Me.myRect1.Bounds = New Rectangle(10, 10, 80, 20)
	///            Me.myRect1.Visible = True
	///            ' Layout Component 2:
	///            Me.myRect2 = New MyRectangle(Me.gridBagLayout1.ContainerControl, Color.FromArgb(222, 100, 19), "Paint Area 2")
	///            Me.myRect2.Bounds = New Rectangle(10, 40, 80, 20)
	///            Me.myRect2.Visible = True
	///            ' Layout Component 3:
	///            Me.myRect3 = New MyRectangle(Me.gridBagLayout1.ContainerControl, Color.FromArgb(196, 214, 233), "Paint Area 3")
	///            Me.myRect3.Bounds = New Rectangle(10, 70, 80, 20)
	///            Me.myRect3.Visible = True
	///            ' Sample GridBagConstraints:
	///            Dim gbc1 As GridBagConstraints
	///            gbc1 = New GridBagConstraints()
	///            Dim gbc2 As GridBagConstraints
	///            gbc2 = New GridBagConstraints()
	///            Dim gbc3 As GridBagConstraints
	///            gbc3 = New GridBagConstraints()
	///            gbc1.Fill = FillType.Both
	///            gbc1.WeightX = 0.2
	///            gbc1.WeightY = 0.5
	///            gbc1.GridPosX = 0
	///            gbc1.GridPosY = 0
	///            gbc2.Fill = FillType.Both
	///            gbc2.WeightX = 0.2
	///            gbc2.WeightY = 0.5
	///            gbc2.GridPosX = 1
	///            gbc2.GridPosY = 0
	///            gbc3.Fill = FillType.Both
	///            gbc3.WeightX = 0.4
	///            gbc3.WeightY = 0.5
	///            gbc3.GridPosX = 0
	///            gbc3.GridPosY = 1
	///            gbc3.CellSpanX = 2
	///            ' Add all the components that are to participate in Layout Management.
	///            ' For GridBagLayouts pass gbcs for GridBagLayouts:
	///            Me.gridBagLayout1.SetConstraints(Me.myRect1.ToControl, gbc1)
	///            Me.gridBagLayout1.SetConstraints(Me.myRect2.ToControl, gbc2)
	///            Me.gridBagLayout1.SetConstraints(Me.myRect3.ToControl, gbc3)
	///            Me.ResumeLayout(True)
	///
	///        End Sub</code></coderef>
	/// </example>
	public abstract class LayoutItemBase : IProvideLayoutInformation
	{
		internal LayoutItemPlaceHolderControl placeHolderControl;
		private LayoutManager _layoutManager;
		protected bool boundsChangingFromOutsideFramework = false;
		/// <summary>
		/// Creates an instance of the LayoutItemBase.
		/// </summary>
		protected LayoutItemBase()
		{
			this.placeHolderControl = new LayoutItemPlaceHolderControl(this);
			this.placeHolderControl.SizeChanged += new EventHandler(this.Control_BoundsChanged);
			this.placeHolderControl.LocationChanged += new EventHandler(this.Control_BoundsChanged);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal LayoutManager LayoutManager
		{
			set
			{
				this._layoutManager = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public void Control_BoundsChanged(object sender, EventArgs e)
		{
			OnBoundsChanged();
			if(boundsChangingFromOutsideFramework && this._layoutManager != null
				&& this._layoutManager.ContainerControl != null)
				this._layoutManager.ContainerControl.PerformLayout();
			boundsChangingFromOutsideFramework = false;
		}
		protected virtual void OnBoundsChanged()
		{
			if(this.BoundsChanged != null)
				this.BoundsChanged(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Called when the Bounds property changes.
		/// </summary>
		public event EventHandler BoundsChanged;

		/// <summary>
		/// Returns the preferred size of the component.
		/// </summary>
		public abstract Size PreferredSize { get; }
		/// <summary>
		/// Returns the minimum size of the component.
		/// </summary>
		public abstract Size MinimumSize { get; }

		/// <summary>
		/// Indicates whether the component should be drawn visible.
		/// </summary>
		/// <value>True for visible; False for hidden.</value>
		public bool Visible
		{
			get{ return this.placeHolderControl.Visible;}
			set { this.placeHolderControl.Visible =value;}
		}
		/// <summary>
		/// Gets / sets the bounds of the component in the corresponding layout manager's
		/// ContainerControl's client co-ordinates.
		/// </summary>
		/// <value>The rectangle within the parent control, in client co-ordinates.</value>
		public Rectangle Bounds
		{
			get{ return this.placeHolderControl.Bounds;}
			set 
			{
				if(this.placeHolderControl.Bounds != value)
				{
					this.placeHolderControl.Bounds  = value;
					boundsChangingFromOutsideFramework = true;
				}
			}
		}
		/// <summary>
		/// Returns the place holder control corresponding to the LayoutItemBase that lets
		/// the LayoutItemBase participate in the LayoutManager framework.
		/// </summary>
		/// <param name="lm">The LayoutItemBase object.</param>
		/// <returns>The corresponding place holder control.</returns>
		public static implicit operator Control(LayoutItemBase lm) 
		{
			return lm.placeHolderControl;
		}
		/// <summary>
		/// Returns the place holder control corresponding to this LayoutItemBase that lets
		/// the LayoutItemBase participate in the LayoutManager framework.
		/// </summary>
		/// <returns>The corresponding place holder control.</returns>
		public Control ToControl()
		{
			return this.placeHolderControl;
		}
	}

	/// <summary>
	/// Defines a mechanism through which dynamic size information can be provided.
	/// </summary>
	/// <remarks>
	/// When a layout component implements this interface, the layout manager will obtain
	/// the size information through this interface whenever layout is performed. This allows
	/// you to provide dynamic layout information.
	/// </remarks>
	public interface IProvideLayoutInformation
	{
		/// <summary>
		/// Returns the preferred size of the component.
		/// </summary>
		Size PreferredSize { get; }
		/// <summary>
		/// Returns the minimum size of the component.
		/// </summary>
		Size MinimumSize { get; }
	}

	/// <summary>
	/// Specifies the type of size information requested.
	/// </summary>
	public enum LayoutInformationType 
	{
		/// <summary>
		/// The preferred size of the component.
		/// </summary>
		PreferredSize,
		/// <summary>
		/// The minimum size of the component.
		/// </summary>
		MinimumSize
	}
	
	/// <summary>
	/// Represents the method that will handle the <see cref="LayoutManager.ProvideLayoutInformation"/> event of
	/// the LayoutManager.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="ProvideLayoutInformationEventArgs"/> that contains the event data.</param>
	public delegate void ProvideLayoutInformationEventHandler(object 
	sender, ProvideLayoutInformationEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="LayoutManager.ProvideLayoutInformation"/> event.
	/// </summary>
	public class ProvideLayoutInformationEventArgs : EventArgs 
	{
		private Control control;
		private LayoutInformationType requestedInfoType;
		private Size size;
		private bool handled = false;

		/// <summary>
		/// Creates a new instance of the ProvideLayoutInformationEventArgs class.
		/// </summary>
		/// <param name="control">The control for which the layout information is requested.</param>
		/// <param name="requested">The type of information requested.</param>
		public ProvideLayoutInformationEventArgs(Control control,
			LayoutInformationType requested)
		{
			this.control = control;
			this.requestedInfoType = requested;
			this.size = Size.Empty;
		}

		/// <summary>
		/// Returns the type of information requested.
		/// </summary>
		/// <value>A <see cref="LayoutInformationType"/> enum.</value>
		public LayoutInformationType Requested 
		{ 
			get
			{
				return this.requestedInfoType;
			} 
		}
		/// <summary>
		/// Returns the control for which the layout information is requested.
		/// </summary>
		/// <value>A control instance.</value>
		public Control Control 
		{ 
			get
			{
				return this.control;
			}
		}
		/// <summary>
		/// Gets / sets the size to be returned.
		/// </summary>
		/// <value>A size value.</value>
		public Size Size 
		{ 
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}
		/// <summary>
		/// Indicates whether this event was handled and a value provided.
		/// </summary>
		/// <value>True to indicate a value was provided; False otherwise.</value>
		public bool Handled 
		{
			get
			{
				return this.handled;
			} 
			set
			{
				this.handled = value;
			}
		}
	}

	/// <summary>
	/// Defines the base class for Layout Managers.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="FlowLayout"/>, <see cref="CardLayout"/>, <see cref="GridLayout"/>, and <see cref="GridBagLayout"/> classes derive from this
	/// base class.</para>
	/// <para>Use one of the above classes to include layout management support in your forms / controls.</para>
	/// <para>The layout manager can be configured to operate in different modes.</para> <para>By default, <see cref="AutoLayout"/> mode
	/// layout will be automatically triggered when the <see cref="ContainerControl"/> fires a <see cref="System.Windows.Forms.Control.Layout"/> event. If not in
	/// this mode, then you can call the <see cref="LayoutContainer"/> method to trigger a layout.</para>
	/// <para>By default, the ContainerControl's ClientRectangle will be used as the bounds for the
	/// layout. But, if the <see cref="CustomLayoutBounds"/> property is set to a value other than Rectangle.Empty
	/// then that rectangle area will be used as the layout bounds.</para>
	/// <para>You can also lay out non-control based components as long as they derive from
	/// the <see cref="LayoutItemBase"/> class. The LayoutItemBase derived object can be used in any 
	/// method call that expects a control instance because the LayoutItemBase has an implicit type-conversion operator
	/// that can convert itself to a control (use the <see cref="LayoutItemBase.ToControl"/> method in VB).</para>
	/// <para>You can also specify / provide preferred and minimum sizes for the child components.
	/// The default layout logic uses the component's preferred size to lay them out.
	/// The LayoutManager also has the <see cref="MinimumLayoutSize"/> and <see cref="PreferredLayoutSize"/> methods
	/// that will let you query for the corresponding sizes.</para>
	/// <para>There are different ways in which you can provide the preferred and minimum sizes
	/// for a component. The manager will first look for the <see cref="IProvideLayoutInformation"/> interface
	/// in your child component, which if found, will be used to obtain the sizes. Second, the
	/// manager will throw a <see cref="ProvideLayoutInformation"/> event for a specific child component, which if
	/// handled will then be used to obtain the sizes, if provided. Third, the sizes provided
	/// using the <see cref="SetPreferredSize"/> / <see cref="SetMinimumSize"/> method will be used to obtain the sizes. However
	/// if SetPreferredSize / SetMinimumSize was never called, the framework will call them
	/// with the current size of the component as the preferred / minimum size.</para>
	/// <para>Take a look at the <see cref="LayoutItemBase"/> class documentation for sample code on how to
	/// create non-control based classes that can participate in Layout management.</para>
	/// </remarks>
	[
	Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.LMDesigner),
		typeof(System.ComponentModel.Design.IDesigner)),
	ProvideProperty("PreferredSize", typeof(Control)),
	ProvideProperty("MinimumSize", typeof(Control))
	]
	public abstract class LayoutManager : Component, System.ComponentModel.IExtenderProvider, ILayoutManager,
		ISupportInitialize
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool initializing = false;
		private bool autoLayout = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable preferredSizes;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable minimumSizes;
		private Control container;
		private ArrayList controlList;
		private bool useControlCollectionPosition;
		private IDesignerHost designerHost = null;
		private IDesigner componentDesigner = null;
		private Rectangle customLayoutBounds = Rectangle.Empty;
		private int horzNearMargin, topMargin, horzFarMargin, bottomMargin;

		/// <summary>
		/// This event is triggered when the ContainerControl property is changed.
		/// </summary>
		[Description("Occurs when the ContainerControl property is changed.")]
		public event EventHandler ContainerControlChanged; 
		/// <summary>
		/// This event is triggered to obtain preferred size information for a child control
		/// during layout.
		/// </summary>
		[Description("This event is triggered to obtain preferred size information for a child control.")]
		public event ProvideLayoutInformationEventHandler ProvideLayoutInformation;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool LoadingDocument
		{
			get
			{
				if(this.designerHost != null)
					return this.designerHost.Loading;
				else
					return false;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool DesignerInTransaction
		{
			get
			{
				if(this.designerHost != null)
					return this.designerHost.InTransaction;
				else
					return false;
			}
		}

        /// <summary>
        /// Gets or sets the designer host.
        /// </summary>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public IDesignerHost DesignerHost
		{
			get
			{
				return this.designerHost;
			}
			set
			{
				this.designerHost = value;
			}
		}

        /// <summary>
        /// Gets or sets the component designer.
        /// </summary>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public IDesigner ComponentDesigner
		{
			get{return this.componentDesigner;}
			set
			{
				this.componentDesigner = value;
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void MakeDirty()
		{
			if(this.componentDesigner != null && this.componentDesigner is IAllowMakeDirty)
			{
				IAllowMakeDirty amd = this.componentDesigner as IAllowMakeDirty;
				amd.SetDirty();
			}
		}

		/// <summary>
		/// Gets or sets the container control that this manager will lay out.
		/// </summary>
		/// <value>A control object.</value>
		/// <remarks>
		/// Changing this property will raise the ContainerControlChanged event.
		/// </remarks>
		[
		Category("Behavior"),
		DefaultValue(null),
		Description("Specifies the container control that this manager will lay out.")
		]
		public Control ContainerControl
		{
			get{return this.container;}
			set
			{
				if(this.container != value)
				{
					if(this.container != null)
						ResetLayoutInfo();

					this.container = value;

					if(this.container != null)
					{
						this.container.Layout += new LayoutEventHandler(OnLayoutEvent);
						this.container.ControlAdded += new ControlEventHandler(OnControlAdded);
						this.container.ControlRemoved += new ControlEventHandler(OnControlRemoved);
						this.container.RightToLeftChanged += new EventHandler(OnRTLChanged);
					}
					this.OnContainerControlChanged(EventArgs.Empty);					
				}
			}
		}
		/// <summary>
		/// Raises the ContainerControlChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnContainerControlChanged method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. </para>
		/// <para>Note to Inheritors: When overriding OnContainerControlChanged in a derived 
		/// class, be sure to call the base class's OnContainerControlChanged method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnContainerControlChanged(EventArgs e)
		{
			if(this.ContainerControl == null)
				this.controlList.Clear();
			// Throw an event
			if(this.ContainerControlChanged != null)
				this.ContainerControlChanged(this, EventArgs.Empty);
		}

	
		/// <summary>
		/// Gets or sets the custom layout bounds, if any, to be used for layout calculation
		/// instead of the container control's ClientRectangle.
		/// </summary>
		/// <value>A Rectangle specifying the custom bounds. Default is Rectangle.Empty.</value>
		/// <remarks>
		/// <para>If this value is Rectangle.Empty, then the manager will use the container control's
		/// ClientRectangle. If other than Rectangle.Empty, then that value will be used.</para>
		/// <para>When using CustomLayoutBounds, you might have to disable <see cref="AutoLayout"/> in
		/// most cases and instead manually reset CustomLayoutBounds and call <see cref="LayoutContainer"/> in
		/// the container control's Layout event handler.</para>
		/// </remarks>
		[
		Localizable(true),
		Description("Specifies the custom layout bounds, if any, to be used for layout calculation instead of the container control's ClientRectangle."),
		SRCategory(SR.CategoryBehavior)
		]
		public Rectangle CustomLayoutBounds
		{
			get{return customLayoutBounds;}
			set
			{
				if(customLayoutBounds != value)
				{
					customLayoutBounds = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		/// <summary>
		/// Sets the CustomLayoutBounds property to Rectangle.Empty.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)] 
		protected virtual void ResetCustomLayoutBounds()
		{
			this.CustomLayoutBounds = Rectangle.Empty;
		}
		/// <summary>
		/// Indicates whether the CustomLayoutBounds property is a value other than Rectangle.Empty.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)] 
		protected virtual bool ShouldSerializeCustomLayoutBounds()
		{
			if(this.CustomLayoutBounds == Rectangle.Empty)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Indicates whether the container control's <see cref="System.Windows.Forms.Control.ControlCollection"/>
		/// should be used as the order for laying out the child controls.
		/// </summary>
		/// <value>True to use the ControlCollection order; False to use the order in which
		/// the child components were added to the manager.
		/// Default value is true.</value>
		/// <remarks>
		/// <para>This property matters only when the corresponding layout manager relies on the 
		/// order of children in the child list in its layout logic. The FlowLayout, CardLayout
		/// and the GridLayout managers rely on the order, while the GridBagLayout managers do not.</para>
		/// <para>Note that if you have both control-based and <see cref="LayoutItemBase"/>-based child components
		/// participating in the layout and this property is True, then the LayoutItemBase based
		/// child components will always be at the bottom of the list when layout is performed.
		/// In this case, if you want more control on the child order, set this property to False
		/// and use the LayoutControls list to modify the exisiting order.</para>
		/// </remarks>
		[
		DefaultValue(true),
		Browsable(false),
		Description("Specifies whether the container control's child ControlCollection (gotten from the controls property) should be used as the order for laying out the child controls.")
		]
		public bool UseControlCollectionPosition
		{
			get{return this.useControlCollectionPosition;}
			set
			{
				this.useControlCollectionPosition = value;
			}
		}

		/// <summary>
		/// Returns the list of child components participating in layout.
		/// </summary>
		/// <value>An ArrayList containing the child components.</value>
		/// <remarks>
		/// <para>There are very specific cases when you have to access this list.</para>
		/// <para>You should access this list to modify the position of the children in the child
		/// components list only when <see cref="UseControlCollectionPosition"/> property is False and
		/// you are laying out both control-based and LayoutItemBase-based components in 
		/// the manager. Take a look at the UseControlCollectionPosition property documentation
		/// for information on this issue.</para>
		/// <para>However, you should only use this property to change the position of child
		/// components, but never to effectively add or remove components (use the 
		/// methods provided by the respective managers).</para>
		/// </remarks>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Description("Specifies the list of child components participating in layout.")
		]
		public ArrayList LayoutControls
		{
			get{return this.controlList;}
		}

 		/// <summary>
		/// Indicates whether the manager should lay out automatically on Layout event.
		/// </summary>
		/// <value>True indicates auto layout; False otherwise. Default is True.</value>
		/// <remarks>
		/// If True, the manager will listen to the ContainerControl control's Layout event and perform layout
		/// automatically. If False, you should call the manager's <see cref="LayoutManager.LayoutContainer"/> method to trigger
		/// layout.
		/// </remarks>
		[
		DefaultValue(true),
		Description("Specifies whether the manger should layout automatically on Layout event."),
		SRCategory(SR.CategoryBehavior)
		]
		public bool AutoLayout
		{
			get{return autoLayout;}
			set
			{
				if(this.autoLayout != value)
				{
					autoLayout = value;

					this.MakeDirty();
				}
			}
		}

		/// <summary>
		/// This method has been replaced by HorzNearMargin. Please use that instead.
		/// </summary>
		[Obsolete("This method has been replaced by HorzNearMargin. Please use that instead."),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftMargin
		{
			get{return this.HorzNearMargin;}
			set{this.HorzNearMargin = value;}
		}

		/// <summary>
		/// Gets or sets the left margin between the client rectangle and the layout rectangle.
		/// </summary>
		/// <value>An integer value in pixels. Default is 4 pixels.</value>
		[DefaultValue(0),
		Category("Appearance"),
		Description("Indicates the left margin between the client rectangle and the layout rectangle.")]
		public int HorzNearMargin
		{
			get{return this.horzNearMargin;}
			set
			{
				if(this.horzNearMargin != value)
				{
					this.horzNearMargin = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		
		/// <summary>
		/// This method has been replaced by HorzFarMargin. Please use that instead.
		/// </summary>
		[Obsolete("This method has been replaced by HorzFarMargin. Please use that instead."),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RightMargin
		{
			get{return this.HorzFarMargin;}
			set{this.HorzFarMargin = value;}
		}

		/// <summary>
		/// Gets or sets the right margin between the client rectangle and the layout rectangle.
		/// </summary>
		/// <value>An integer value in pixels. Default is 4 pixels.</value>
		[DefaultValue(0),
		Category("Appearance"),
		Description("Indicates the right margin between the client rectangle and the layout rectangle.")]
		public int HorzFarMargin
		{
			get{return this.horzFarMargin;}
			set
			{
				if(this.horzFarMargin != value)
				{
					this.horzFarMargin = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the top margin between the client rectangle and the layout rectangle.
		/// </summary>
		/// <value>An integer value in pixels. Default is 4 pixels.</value>
		[DefaultValue(0),
		Category("Appearance"),
		Description("Indicates the top margin between the client rectangle and the layout rectangle.")]
		public int TopMargin
		{
			get{return this.topMargin;}
			set
			{
				if(this.topMargin != value)
				{
					this.topMargin = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the bottom margin between the client rectangle and the layout rectangle.
		/// </summary>
		/// <value>An integer value in pixels. Default is 4 pixels.</value>
		[DefaultValue(0),
		Category("Appearance"),
	    Description("Indicates the bottom margin between the client rectangle and the layout rectangle.")]
		public int BottomMargin
		{
			get{return this.bottomMargin;}
			set
			{
				if(this.bottomMargin != value)
				{
					this.bottomMargin = value;
					if(this.ContainerControl != null)
						this.ContainerControl.PerformLayout();

					this.MakeDirty();
				}
			}
		}

		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                ArrayList alTemp = new ArrayList();
                foreach( Control ctrl in this.controlList )
                {
                    alTemp.Add(ctrl);
                }
                foreach( Control ctrl in alTemp )
                {
                    this.RemoveLayoutComponent(ctrl);
                }
                alTemp.Clear();

				this.ResetLayoutInfo();
				this.container = null;
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Constructor to be called by derived classes.
		/// </summary>
		protected LayoutManager()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(LayoutManager));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			BeginInit();
			preferredSizes = new Hashtable();
			minimumSizes = new Hashtable();
			controlList = new ArrayList();
			useControlCollectionPosition = true;
			this.horzNearMargin = this.horzFarMargin = this.bottomMargin = this.topMargin = 0;
			EndInit();
		}
		
		/// <summary>
		/// Starts designer initialization.
		/// </summary>
		public virtual void BeginInit()
		{
			initializing = true;
		}

		/// <summary>
		/// Ends designer initialization.
		/// </summary>
		public virtual void EndInit()
		{
			
			if(this.ContainerControl != null 
				&& this.DesignMode
				&& this.ContainerControl is UserControl)
			{
				// The UserControl doesn't fire a Layout 
				// during design-time after setting a new size, so forcing a layout.
				this.ContainerControl.PerformLayout();
			}
			initializing = false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		bool IExtenderProvider.CanExtend(object target) 
		{
			return this.CanExtend(target);
		}	
	
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool CanExtend(object target)
		{
			if (target is Control && ((Control)target).Parent != null && 
				((Control)target).Parent == ContainerControl && !(target is AutoLabel))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Retrieves the preferred size associated with the specified control.
		/// </summary>
		/// <param name="control">The control for which to retrieve the preferred size.</param>
		/// <returns>The preferred size for the specified control.</returns>
		/// <remarks>
		/// Take a look at the LayoutManager class documentation for information on the
		/// different ways in which the manager obtains and you can specify the preferred and minimum size information
		/// for a child component.
		/// </remarks>
		[Category("Layout Manager"),
		Localizable(true)]
		public virtual Size GetPreferredSize(Control control)
		{
			// If dynamic size available, go ahead and use it.
			Size prefSize = Size.Empty;
			if(this.GetDynamicSize(control, LayoutInformationType.PreferredSize, ref prefSize))
				return prefSize;

			// Else use the static size provided
			return this.GetStaticPreferredSize(control);
		}

		/// <summary>
		/// Returns the preferred size provided with a call to SetPreferredSize.
		/// </summary>
		/// <param name="control">The control whose preferred size is to be known.</param>
		/// <returns>The size, if any, provided or the current control size.</returns>
		/// <remarks>
		/// <para>Unlike GetPreferredSize, this does not throw an event or look for IProvideLayoutInformation
		/// in the child controls.</para>
		/// <para>Calling this will in turn call SetPreferredSize with the current control size, if there is no size available.</para>
		/// </remarks>
		protected virtual Size GetStaticPreferredSize(Control control)
		{
			if(preferredSizes[control] == null)
				SetPreferredSize(control, control.Size);

			return (Size)preferredSizes[control];
		}

		/// <summary>
		/// Associates a preferred size with the specified control.
		/// </summary>
		/// <param name="control">The control to associate the preferred size with.</param>
		/// <param name="value">The preferred size of the control.</param>
		/// <remarks>
		/// Take a look at the LayoutManager class documentation for information on the
		/// different ways in which the manager obtains and you can specify the preferred and minimum size information
		/// for a child component.
		/// </remarks>
		public virtual void SetPreferredSize(Control control, Size value)
		{
			if(preferredSizes[control] == null
				|| !((Size)preferredSizes[control]).Equals(value))
			{
				preferredSizes[control] = value;
				if(this.ContainerControl != null)
					this.ContainerControl.PerformLayout();
			}
		}
		
		/// <summary>
		/// Makes the current sizes of the child controls their minimum and preferred sizes.
		/// </summary>
		protected void ForcePreferredAndMinimumsSize()
		{
			if(this.ContainerControl != null)
				this.ContainerControl.SuspendLayout();
			// Force preferred and minimum size setting.
			foreach(Control control in this.GetControls())
			{
				if(!(control is AutoLabel))
				{
					if(!this.ShouldSerializePreferredSize(control))
						this.SetPreferredSize(control, control.Size);
					if(!this.ShouldSerializeMinimumSize(control))
						this.SetMinimumSize(control, control.Size);
				}
			}
			if(this.ContainerControl != null)
				this.ContainerControl.ResumeLayout();
		}

		/// <summary>
		/// Indicates whether the PreferredSize property is a value other than Rectangle.Empty.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializePreferredSize(Control control)
		{
			return this.preferredSizes[control] != null;
		}


		/// <summary>
		/// Removes any custom preferred size set for the specified control.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)] 
		public virtual void ResetPreferredSize(Control control)
		{
			this.preferredSizes[control] = null;
		}

		/// <summary>
		/// Retrieves the minimum size associated with the specified control.
		/// </summary>
		/// <param name="control">The control for which to retrieve the minimum size.</param>
		/// <returns>The minimum size for the specified control.</returns>
		/// <remarks>
		/// Take a look at the LayoutManager class documentation for information on the
		/// different ways in which the manager obtains and you can specify the preferred and minimum size information
		/// for a child component.
		/// </remarks>
		[Category("Layout Manager"),
		Localizable(true)]
		public virtual Size GetMinimumSize(Control control)
		{
			// If dynamic size available, go ahead and use it.
			Size minSize = Size.Empty;
			if(this.GetDynamicSize(control, LayoutInformationType.MinimumSize, ref minSize))
				return minSize;

			return this.GetStaticMinimumSize(control);
		}
		/// <summary>
		/// Returns the minimum size provided with a call to SetMinimumSize.
		/// </summary>
		/// <param name="control">The control whose minimum size is to be known.</param>
		/// <returns>The size, if any provided, or the current control size.</returns>
		/// <remarks>
		/// <para>Unlike GetMinimumSize, this does not throw an event or look for IProvideLayoutInformation
		/// in the child controls.</para>
		/// <para>Calling this will in turn call SetMinimumSize with the current control size, if there is no size available.</para>
		/// </remarks>
		protected virtual Size GetStaticMinimumSize(Control control)
		{
			if(minimumSizes[control] == null)
				SetMinimumSize(control, control.Size);
			//				return control.Size;

			return (Size)minimumSizes[control];
		}

		/// <summary>
		/// Associates a minimum size with the specified control.
		/// </summary>
		/// <param name="control">The control to associate the minimum size with.</param>
		/// <param name="value">The minimum size of the control.</param>
		/// <remarks>
		/// Take a look at the LayoutManager class documentation for information on the
		/// different ways in which the manager obtains, and you can specify, the preferred and minimum size information
		/// for a child component.
		/// </remarks>
		public virtual void SetMinimumSize(Control control, Size value)
		{
			if(minimumSizes[control] == null
				|| !((Size)minimumSizes[control]).Equals(value))
			{
				minimumSizes[control] = value;	// Am I holding a reference to control?
				if(this.ContainerControl != null)
					this.ContainerControl.PerformLayout();
			}
		}
		/// <summary>
		/// Indicates whether the MinimumSize property is a value other than Rectangle.Empty.
		/// </summary>
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public virtual bool ShouldSerializeMinimumSize(Control control)
		{
			return minimumSizes[control] != null;
		}
		/// <summary>
		/// Removes any custom minimum size set for the specified control.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)] 
		public virtual void ResetMinimumSize(Control control)
		{
			minimumSizes[control] = null;
		}
		
		/// <summary>
		/// Returns the dynamic preferred or minimum size of a child component.
		/// </summary>
		/// <param name="control">The child control.</param>
		/// <param name="type">The type of size required, preferred or minimum.</param>
		/// <param name="size">A reference value through which the size should be returned to the caller.</param>
		/// <returns>True to indicate a dynamic size was found and that the size argument has a valid value. False otherwise.</returns>
		/// <remarks>
		/// <para>This function will first check if the child control has an <see cref="IProvideLayoutInformation"/>
		/// interface and if so returns the size provided by that interface.
		/// If not, it throws a <see cref="ProvideLayoutInformation"/> to obtain the dynamic size. The handlers,
		/// if any, for that event may provide the dynamic size which will be returned.</para>
		/// <para>If none of the above cases succeeds then False will be returned.</para>
		/// </remarks>
		protected virtual bool GetDynamicSize(Control control, LayoutInformationType type, ref Size size)
		{
			// First check if the control implements IProvideLayoutInformation.
			if(control is IProvideLayoutInformation)
			{
				if(type == LayoutInformationType.MinimumSize)
					size = ((IProvideLayoutInformation)control).MinimumSize;
				else if(type == LayoutInformationType.PreferredSize)
					size = ((IProvideLayoutInformation)control).PreferredSize;

				return true;
			}

			// Else throw an event and see if anyone cares to provide size information.
			ProvideLayoutInformationEventArgs args = 
				new ProvideLayoutInformationEventArgs(control, type);
			this.OnProvideLayoutInformation(args);
			if(args.Handled)
				size = args.Size;

			return args.Handled;
		}

		/// <summary>
		/// Raises the <see cref="ProvideLayoutInformation"/> event.
		/// </summary>
		/// <param name="args">A <see cref="ProvideLayoutInformationEventArgs"/> that contains the event data.</param>
		/// <remarks>
		/// <para>The OnProvideLayoutInformation method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. </para>
		/// <para>Note to Inheritors: When overriding OnProvideLayoutInformation in a derived 
		/// class, be sure to call the base class's OnProvideLayoutInformation method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnProvideLayoutInformation(ProvideLayoutInformationEventArgs args)
		{
			if(this.ProvideLayoutInformation != null)
			{
				this.ProvideLayoutInformation(this, args);
			}
		}

		void OnLayoutEvent(object sender, LayoutEventArgs e)
		{
			if(this.AutoLayout)
				LayoutContainer();
		}
		void OnRTLChanged(object sender, EventArgs e)
		{
			if(this.AutoLayout)
			{
				this.ContainerControl.SuspendLayout();
				// First let the AutoLabels update their positions, so that their DXs will be updated properly.
				foreach(Control c in this.ContainerControl.Controls)
				{
					if(c is AutoLabel)
					{
						AutoLabel al = c as AutoLabel;
						al.UpdatePosition();
					}
				}
				this.ContainerControl.ResumeLayout(false);

				LayoutContainer();
			}
		}
		
		/// <summary>
		/// The handler for the container's ControlAdded event.
		/// </summary>
		/// <param name="sender">The container into which a control was added.</param>
 		/// <param name="e">An ControlEventArgs that contains the event data.</param>
		/// <remarks>
		/// This is an easy way for the derived classes to know when a child gets added to the ContainerControl.
		/// </remarks>
		protected virtual void OnControlAdded(object sender, ControlEventArgs e)
		{
			this.OnDockStyleChanged(e.Control, EventArgs.Empty);
		}

		/// <summary>
		/// The handler for the container's ControlRemoved event.
		/// </summary>
		/// <param name="sender">The container into which a control was added.</param>
		/// <param name="e">An ControlEventArgs that contains the event data.</param>
		/// <remarks>
		/// This is an easy way for the derived classes to know when a child gets removed from the ContainerControl.
		/// </remarks>
		protected virtual void OnControlRemoved(object sender, ControlEventArgs e)
		{
			// Don't have to call LayoutContainer, since this is ususally followed by the Layout event
			this.RemoveLayoutComponent(e.Control);
		}
		
		/// <summary>
		/// Adds a child component to the layout list with the specified constraints.
		/// </summary>
		/// <param name="childControl">The control to add to the list.</param>
		/// <param name="constraints">The associated constraints.</param>
		/// <remarks>
		/// <para>The type of constraints to be passed varies based on the layout manager.
		/// The <see cref="CardLayout"/> for example expects a string type while the <see cref="GridBagLayout"/> expects
		/// a <see cref="GridBagConstraints"/> type. The <see cref="FlowLayout"/> and the <see cref="GridLayout"/> do not expect anything
		/// which means you can specify NULL. Take a look at the individual layout manager's documentation 
		/// for the type of constraints expected. The individual managers also provide custom type safe methods
		/// to let you specify the appropriate constraints.</para>
		/// <para>
		/// You can also pass a <see cref="LayoutItemBase"/> derived class as the first argument because
		/// it has an implicit type-conversion operator that will provide its corresponding
		/// control object (a place-holder control that allows the LayoutItemBase to seamlessly participate
		/// in the layout framework). In VB, use the <see cref="LayoutItemBase.ToControl"/> method.
		/// </para>
		/// </remarks>
		public virtual void AddLayoutComponent(Control childControl, object constraints)
		{
			if(childControl is LayoutItemPlaceHolderControl)
			{
				LayoutItemPlaceHolderControl placeHolderControl 
					= (LayoutItemPlaceHolderControl)childControl;
				placeHolderControl.LayoutManager = this;
			}

			if(this.controlList.IndexOf(childControl) == -1)
			{
				childControl.DockChanged += new EventHandler(this.OnDockStyleChanged);
				this.OnDockStyleChanged(childControl, EventArgs.Empty);
				this.controlList.Add(childControl);
				if(this.ContainerControl != null)
					this.ContainerControl.PerformLayout();
			}
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
		public virtual void RemoveLayoutComponent(Control childControl)
		{
			if(this.controlList.Contains(childControl))
			{
				if(childControl is LayoutItemPlaceHolderControl)
				{
					LayoutItemPlaceHolderControl placeHolderControl 
						= (LayoutItemPlaceHolderControl)childControl;
					placeHolderControl.LayoutManager = null;
				}

				this.controlList.Remove(childControl);

				childControl.DockChanged -= new EventHandler(this.OnDockStyleChanged);

				if(this.ContainerControl != null)
					this.ContainerControl.PerformLayout();
			}
		}
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnDockStyleChanged(object sender, EventArgs e)
		{
			Control child = sender as Control;
			if(child != null && !(child is MdiClient) && child.Dock != DockStyle.None &&
				this.controlList.IndexOf(child) != -1)
			{
				child.Dock = DockStyle.None;
				if(this.DesignMode)
					MessageBox.Show("Controls managed by the LayoutManager cannot have a DockStyle. Resetting Control's DockStyle to DockStyle.None", "LayoutManager Warning");
			}
		}
		/// <summary>
		/// Removes any references to the container control and handlers for events in that
		/// control. Will also remove references to the child control.
		/// </summary>
		/// <remarks>
		/// Will be called when the user sets the ContainerControl to NULL and from Dispose.
		/// Make sure to call the base class to perform normal operations when you override
		/// this method.
		/// </remarks>
		protected virtual void ResetLayoutInfo()
		{
			if(this.container != null)
			{
				this.container.Layout -= new LayoutEventHandler(OnLayoutEvent);
				this.container.ControlAdded -= new ControlEventHandler(OnControlAdded);
				this.container.ControlRemoved -= new ControlEventHandler(OnControlRemoved);
				this.container.RightToLeftChanged -= new EventHandler(OnRTLChanged);
			}
			this.controlList.Clear();
		}
		
		/// <summary>
		/// Returns the minimum size for the ContainerControl.
		/// </summary>
		/// <returns>A size value representing the minimum size required.</returns>
		/// <remarks>
		/// This value is calculated based on the minimum size requirement for the child controls.
		/// </remarks>
		public abstract Size MinimumLayoutSize();
		
		/// <summary>
		/// Returns the preferred size for the ContainerControl.
		/// </summary>
		/// <returns>A size value representing the preferred size.</returns>
		/// <remarks>
		/// This value is calculated based on the preferred size requirement for the child controls.
		/// </remarks>
		public abstract Size PreferredLayoutSize();

		/// <summary>
		/// Triggers a layout of the child components.
		/// </summary>
		/// <remarks>
		/// Use this when you want to manually trigger a layout. This will automatically 
		/// be called by the framework when <see cref="AutoLayout"/> is True and a Layout event occurs on the <see cref="ContainerControl"/>.
		/// </remarks>
		public abstract void LayoutContainer();

		/// <summary>
		/// Returns the child components that participate in the layout.
		/// </summary>
		/// <returns>
		/// The child control list.
		/// </returns>
		/// <remarks>
		/// The order of child controls in the returned list will take into account the 
		/// <see cref="UseControlCollectionPosition"/> property value.
		/// </remarks>
		public virtual IList GetControls()
		{
			if(!this.UseControlCollectionPosition)
			{
				return (ArrayList)this.controlList.Clone();
			}
			else
			{
				// Return a list that reflects the position of controls in the controls list.
				ArrayList childControls = new ArrayList();
				if(this.ContainerControl != null)
				{
					foreach(Control control in this.ContainerControl.Controls)
					{
						if(this.controlList.IndexOf(control) > -1 && !(control is MdiClient))
							childControls.Add(control);
					}
					if(childControls.Count != this.controlList.Count)
					{
						// There are controls not parented by ContainerControl.
						// Add them to this list.
						foreach(Control control in this.controlList)
						{
							if(!this.ContainerControl.Contains(control))
								childControls.Add(control);
						}
					}
				}
				return childControls;
			}
		}
		/// <summary>
		/// Returns the layout bounds within which to perform layout.
		/// </summary>
		/// <returns>The Rectangle specifying the layout bounds.</returns>
		/// <remarks>
		/// This takes into account the <see cref="CustomLayoutBounds"/> value, if not empty.
		/// If empty, the bounds are calculated based on the Container Control's ClientRectangle 
		/// and the margins specified.
		/// </remarks>
		protected virtual Rectangle GetBounds()
		{
			Rectangle bounds = Rectangle.Empty;
			if(!this.customLayoutBounds.IsEmpty)
				bounds = this.customLayoutBounds;
			else
			{
				bounds = this.ContainerControl.ClientRectangle;

				// keep in mind, that container control maybe scrollable control,
				// and top-left position maybe not (0,0), but scrolled 
				// ( for example, -10, -200 ).
				if( this.ContainerControl is ScrollableControl )
				{
					bounds.Offset( ( this.ContainerControl as ScrollableControl ).AutoScrollPosition );
                    
                   
				}
			}
			// It's assumed in "new RTLMatrix" call that horzNearMargin and horzFarMargins are the
			// only parameters that will affect the GetBounds call.

			// Left Margin
			bounds.X += this.horzNearMargin;
			bounds.Width -= this.horzNearMargin;
            bounds.Width -= this.horzFarMargin;
			// Right Margin
            //if (this.ContainerControl.RightToLeft ==   = RightToLeft.Yes)
            //{
            //    bounds.Width -= this.horzFarMargin + 17;
            //}
			
			// Top Margin
			bounds.Y += this.topMargin;
			bounds.Height -= this.topMargin;
			// Bottom Margin
			bounds.Height -= this.bottomMargin;
            
			return bounds;
		}
		[Documentation.DocumentationExclude()]
		protected int AdjustHeightForMargins(int height)
		{
			// Margins
			height += this.topMargin;
			height += this.bottomMargin;
			return height;
		}
		[Documentation.DocumentationExclude()]
		protected int AdjustWidthForMargins(int width)
		{
			// Margins
			width += this.horzNearMargin;
			width += this.horzFarMargin;
			return width;
		}

		/// <summary>
		/// Indicates the Visible state of the child control or LayoutItemBase.
		/// </summary>
		/// <param name="control">The control whose visibility is to be determined.</param>
		/// <returns>The visibility state.</returns>
		/// <remarks>
		/// Use this instead of checking the Visible property of the control directly, because
		/// if this control is a place-holder control for a LayoutItemBase, the Visibility
		/// state will be stored elsewhere.
		/// </remarks>
		protected bool IsVisible(Control control)
		{
			if(control is LayoutItemPlaceHolderControl)
				return ((LayoutItemPlaceHolderControl)control).Visible;
			else
				return control.Visible;
		}
		/// <summary>
		/// Indicates whether the layout manager is in a state where it can start laying out
		/// components.
		/// </summary>
		/// <returns>True indicates its ready for layout; False otherwise.</returns>
		/// <remarks>
		/// This will return True if it has a valid ContainerControl and at least one child component
		/// to be laid out.
		/// If you override this method, make sure to call the base class.
		/// </remarks>
		protected virtual bool IsInit()
		{
			if(this.ContainerControl != null)
			{
				if(this.controlList.Count > 0
					|| this.ContainerControl.Controls.Count > 0)
					return true;
			}

			return false;
		}

		#region RTL_RELATED
		internal ControlBounds GetChildControlBounds(Control c)
		{
			return this.GetChildControlBounds(c, c.Size);
		}
		internal ControlBounds GetChildControlBounds(Control c, Size prefSize)
		{
			if(this.ContainerControl.RightToLeft == RightToLeft.No)
				return new ControlBounds(new Rectangle(c.Location, prefSize), c);
			else
			{
                if (this.ContainerControl is ScrollableControl)
                {
                    return new ControlBounds(new Rectangle(c.Location, prefSize), c);
                }
                else
                {    
                Rectangle containerBounds = this.GetBounds();
				ControlBounds cb = new ControlBounds(c.Bounds, c, new RTLMatrix(containerBounds.Left, this.HorzFarMargin, containerBounds.Width));
				// Adjust the bounds for prefSize only after the transform, not before the transform.
				Rectangle adjustedBounds = new Rectangle(new Point(cb.Bounds.X, cb.Bounds.Y), prefSize);
				cb.SetOnlyInternalBounds(adjustedBounds);
				return cb;
               }


			}
		}
		#endregion RTL_RELATED

		#region AUTOLABELS
		[Syncfusion.Documentation.DocumentationExclude()]
		protected AutoLabel GetAutoLabel(Control c)
		{
			if(AutoLabel.AutoLabelMap.Contains(c))
			{
				AutoLabel autoLabel = AutoLabel.AutoLabelMap[c] as AutoLabel;
				if(autoLabel.Parent == c.Parent)
					return autoLabel;
			}
			return null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual bool ShouldLayoutAutoLabel(AutoLabel autoLabel)
		{
			if(autoLabel.LabeledControl == null)
				return true;

			if(autoLabel.LabeledControl.Parent != autoLabel.Parent)
				return true;
			
			return false;
		}
		#endregion AUTOLABELS
	}

	
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ILayoutManager
	{
		void AddLayoutComponent(Control childControl, object constraints);
		void RemoveLayoutComponent(Control childControl);
		
		Size PreferredLayoutSize();
		Size MinimumLayoutSize();

		Size GetPreferredSize(Control childControl);
		void SetPreferredSize(Control control, Size value);
		Size GetMinimumSize(Control control);
		void SetMinimumSize(Control control, Size value);

		void LayoutContainer();

		Control ContainerControl
		{
			get;
			set;
		}
		Rectangle CustomLayoutBounds{get;set;}
		bool AutoLayout{get;set;}
	}

	internal class ControlBounds 
	{
		Rectangle bounds;
		Control c;
		RTLMatrix rtlMatrix;
		bool needRTLTransform;
		bool updating = false;

		public ControlBounds(Rectangle bounds, Control c)
			: this(bounds, c, null)
		{
		}

		public ControlBounds(Rectangle bounds, Control c, RTLMatrix rtlMatrix)
		{
			this.bounds = bounds;
			this.c = c;
			this.rtlMatrix = rtlMatrix;
			this.needRTLTransform = this.rtlMatrix != null;
			if(this.NeedRTLTransform)
			{
				this.bounds = this.RTLTransformRect(this.bounds);
			}
		}

		// This will usually be called for AutoLabels.
		internal void ReinitLocation()
		{
			if(this.needRTLTransform)
			{
				Rectangle tBounds = this.RTLTransformRect(this.c.Bounds);
				tBounds.Size = this.bounds.Size;
				this.bounds = tBounds;
			}
			else
				this.bounds.Location = this.c.Location;
		}

		internal void SetOnlyInternalBounds(Rectangle newBounds)
		{
			this.bounds = newBounds;
		}

		public override string ToString()
		{
			return this.bounds.ToString();
		}

		private bool NeedRTLTransform
		{
			get{return this.needRTLTransform;}
		}

		private Rectangle RTLTransformRect(Rectangle rect)
		{
			// Transform just the top-left point.
			Point tl = rtlMatrix.TransformPoint(new Point(rect.Left, rect.Top));
			tl.X -= rect.Width;

			return new Rectangle(tl, rect.Size);
		}

		public void BeginUpdate()
		{
			if(updating)
				throw new Exception("Multiple BeginUpdates not allowed on ControlBounds.");
			updating = true;
		}
		public void EndUpdate()
		{
			if(!updating)
				throw new Exception("EndUpdate called without calling BeginUpdate in ControlBounds.");

			updating = false;
			this.UpdateControlBounds();
		}

		private void UpdateControlBounds()
		{
			if(!this.NeedRTLTransform)
				this.c.Bounds = this.bounds;
			else
			{
				this.c.Bounds = this.RTLTransformRect(this.bounds);
			}
		}

		public Point Location
		{
			get{return new Point(bounds.X, bounds.Y);}
			set
			{
				this.bounds.X = value.X;
				this.bounds.Y = value.Y;
				if(!updating)
				{
					if(!this.NeedRTLTransform)
						this.c.Location = value;
					else
					{
						// Transform it to real co-ords before setting the control's location.
						Point tPoint = this.rtlMatrix.TransformPoint(value);
						tPoint.X -= this.Width;
						this.c.Location = tPoint;
					}
				}
			}
		}

		public Rectangle Bounds
		{
			get{return this.bounds;}
			set
			{
				this.bounds = value;
				if(!updating)
				{
					this.UpdateControlBounds();
				}
			}
		}

		public int Width
		{
			get{return this.bounds.Width;}
			set
			{
				this.bounds.Width = value;
				if(!updating)
				{
					if(!this.NeedRTLTransform)
						this.c.Width = value;
					else
					{
						Rectangle ltrRect = this.bounds;
						ltrRect.Width = value;
						Rectangle rtlRect = this.RTLTransformRect(ltrRect);
						this.c.Location = new Point(rtlRect.X, this.c.Location.Y);
						this.c.Width = rtlRect.Width;
					}
				}
			}
		}

		public int Height
		{
			get{return this.bounds.Height;}
			set
			{
				this.bounds.Height = value;
				if(!updating)
                    this.c.Height = value;
			}
		}
	}

	/// <summary>
	/// Helps transform points from LTR to RTL co-ordinates and vice versa.
	/// </summary>
	internal class RTLMatrix
	{
		Matrix mirrorMatrix;
		int leftMargin, rightMargin;

		public RTLMatrix(int leftMargin, int rightMargin, int containerWidth)
		{
			this.leftMargin = leftMargin;
			this.rightMargin = rightMargin;
			mirrorMatrix = new Matrix(-1f, 0f, 0f, 1f, (float)containerWidth - this.leftMargin, 0f);
		}

		public Point TransformPoint(Point pt)
		{
			pt.X -= leftMargin;
			Point[] tPoints =
				{
					pt
				};

			mirrorMatrix.TransformPoints(tPoints);

			tPoints[0].X = tPoints[0].X + this.rightMargin;

			return tPoints[0];
		}
	}
}
