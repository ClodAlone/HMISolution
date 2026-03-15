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
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.ComponentModel;
using System.Diagnostics;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Text;
using System.Reflection;
using Syncfusion.Windows.Forms.Design;
#endif
namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the relative position of an <see cref="AutoLabel"/> control to that of the 
	/// control it labels.
	/// </summary>
	public enum AutoLabelPosition
	{
		/// <summary>
		/// The relative AutoLabel position can be set manually.
		/// </summary>
		Custom,
		/// <summary>
		/// Left has been replaced with side, to take into account RightToLeft configs. Please use side instead.
		/// </summary>
		[Obsolete("Left will be replaced with side, to take into account RightToLeft configs. Please use side instead.")]
		Left,
		/// <summary>
		/// The AutoLabel is always positioned to the top of the labeled control.
		/// </summary>
		Top,
		/// <summary>
		/// The AutoLabel is always positioned to the left (or right if the parent control is RTL enabled) of the labeled control.
		/// Replaces Left.
		/// </summary>
		Side,
	}
	/// <summary>
	/// A <see cref="Label"/> derived class that lets you label any control with it.
	/// </summary>
	/// <remarks>
	/// <para>Once a control is labeled by an instance of AutoLabel (through the <see cref="LabeledControl"/> property, 
	/// the label gets moved around as the labeled control moves around 
	/// automatically, preserving the relative positions. The relative positions can 
	/// also be configured to be left, top or custom through the <see cref="Position"/> property.</para>
	/// <para>
	/// Note that the <see cref="FlowLayout"/> manager will treat the label and its control as a 
	/// pair, always laying them out together as if they were one single control.
	/// </para>
	/// </remarks>
	[
	Designer(
		typeof(Syncfusion.Windows.Forms.Tools.AutoLabelDesigner),
		typeof(System.ComponentModel.Design.IDesigner)),
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.AutoLabel.bmp")
	]
	public class AutoLabel : Label
	{
		#region CONSTANS
		/// <summary>
		/// Value for PreferredHeight adding when borders are present.
		/// </summary>
		private const int DEF_WITH_BORDERS = 6;
		/// <summary>
		/// Value for PreferredHeight adding when borders are absent.
		/// </summary>
		private const int DEF_WITHOUT_BORDERS = 3;
		#endregion

		#region FIELDS
		private Control labeledControl = null;
		private AutoLabelPosition position = AutoLabelPosition.Side;
		private int gap = 4;
		private int dx = 0;
		private int dy = 0;
		private bool settingPos = false;
		#endregion FIELDS

		#region STATIC_FIELDS
		private static Hashtable htAutoLabelsByControl = new Hashtable();

		#endregion STATIC_FIELDS

		#region CONSTRUCTORS
		public AutoLabel()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(AutoLabel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			this.AutoSize = true;
		}
		#endregion CONSTRUCTORS


        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

		#region STATIC_PROPERTIES
		internal static Hashtable AutoLabelMap
		{
			get{return htAutoLabelsByControl;}
		}
		#endregion STATIC_PROPERTIES

		#region EVENTS
		/// <summary>
		/// Fired when the LabeledControl, Gap and Position properties of this class changes.
		/// </summary>
        [Description("Event raised when the Gap and Position properties of this control changes.")]
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;
		#endregion EVENTS

		#region VIRTUALS

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="e">
		/// An <see cref="EventArgs"/> object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnPropertyChanged method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Note to Inheritors: When overriding OnPropertyChanged in a derived 
		/// class, be sure to call the base class's OnPropertyChanged method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnPropertyChanged(SyncfusionPropertyChangedEventArgs e)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this, e);
		}

        /// <override/>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            this.UpdatePosition();
        }

		/// <override/>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			this.UpdatePosition();
		}

		/// <override/>
		protected override void OnParentRightToLeftChanged(EventArgs e)
		{
			base.OnParentRightToLeftChanged(e);
			this.UpdatePosition();
			if(this.Parent != null && this.Parent.RightToLeft != this.RightToLeft)
				Trace.WriteLine("AutoLable: " + this.Name + " has a RightToLeft value different from its parent's. This will cause problems while using a FlowLayout on the parent control.");
		}

		/// <override/>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			this.UpdatePosition();
			if(this.Parent != null && this.Parent.RightToLeft != this.RightToLeft)
				Trace.WriteLine("AutoLabel: " + this.Name + " has a RightToLeft value different from its parent's. This will cause problems while using a FlowLayout on the parent control.");
		}

		/// <summary>
		/// Updates the position of the AutoLabel when the parameters that affect
		/// the relative positions have changed (like the LabeledControl's position, size, etc.).
		/// </summary>
		protected internal virtual void UpdatePosition()
		{
			if(this.LabeledControl == null || this.LabeledControl.Parent != this.Parent
				|| this.settingPos)
				return;

			this.settingPos = true;
			if(this.Position == AutoLabelPosition.Custom)
			{
				int x = this.RightToLeft == RightToLeft.Yes ? 
						this.LabeledControl.Right - DX - this.Width
					:	this.LabeledControl.Location.X + DX;

				int y = this.LabeledControl.Location.Y + DY;
				this.Location = new Point(x, y);
			}
			else if(this.Position == AutoLabelPosition.Side || this.Position == AutoLabelPosition.Left)
			{
				int y = this.LabeledControl.Location.Y + (this.LabeledControl.Height - this.Height)/2;
				int x = (this.RightToLeft == RightToLeft.Yes) ?
					// RTL
					this.LabeledControl.Bounds.Right + this.gap
					:
					// LTR
					this.LabeledControl.Location.X - this.gap - this.Width;
				
				
				this.Location = new Point(x, y);
			}
			else
			{
				int x = this.LabeledControl.Location.X;
				int y = this.LabeledControl.Location.Y - this.gap - this.Height;
				this.Location = new Point(x, y);
			}
			this.settingPos = false;
		}

		/// <summary>
		/// Called when a new control is getting labeled (when set through the <see cref="LabeledControl"/> property.
		/// </summary>
		/// <param name="labeledControl">The control that is being labeled.</param>
		protected virtual void OnAttachLabeledControl(Control labeledControl)
		{
			if(labeledControl != null)
			{
				AutoLabel.AutoLabelMap[labeledControl] = this;
				labeledControl.LocationChanged += new EventHandler(this.LabeledControl_LocationChanged);
				labeledControl.SizeChanged += new EventHandler(this.LabeledControl_SizeChanged);
			}
		}

		/// <summary>
		/// Called when an exisiting label is getting unlabeled.
		/// </summary>
		/// <param name="labeledControl">The control that is being unlabeled.</param>
		protected virtual void OnDetachLabeledControl(Control labeledControl)
		{
			if(labeledControl != null)
			{
				AutoLabel.AutoLabelMap.Remove(labeledControl);
				labeledControl.LocationChanged -= new EventHandler(this.LabeledControl_LocationChanged);
				labeledControl.SizeChanged -= new EventHandler(this.LabeledControl_SizeChanged);
			}
		}
		private void LabeledControl_SizeChanged(object sender, EventArgs e)
		{
			if(this.RightToLeft == RightToLeft.Yes)
			{
				this.UpdatePosition();
			}
		}

		private void LabeledControl_LocationChanged(object sender, EventArgs e)
		{
			this.UpdatePosition();
		}
		#endregion VIRTUALS

		#region OVERRIDES
		private bool ShouldAllowBoundsChange()
		{
			if(this.Position == AutoLabelPosition.Custom)
				return true;
			else
				return false;
		}
		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				this.LabeledControl = null;
			}
			base.Dispose(disposing);
		}
		#endregion OVERRIDES

		#region PROPERTIES

		/// <summary>
        /// Gets or sets a value indicating whether the control is automatically resized
        ///     to display its entire contents.
		/// </summary>
		[DefaultValue(true)]
		public override bool AutoSize 
		{
			get{return base.AutoSize;}
			set{base.AutoSize = value;}
		}

		/// <summary>
		/// Gets or sets the control that is being labeled.
		/// </summary>
		/// <value>A control instance.</value>
		[
		Description("Specifies the control that is being labeled."),
		DefaultValue(null),
		Category("Layout Information")
		]
		public Control LabeledControl
		{
			get{return this.labeledControl;}
			set
			{
				if(this.labeledControl != value)
				{
					if(value is AutoLabel)
					{
						if(this.DesignMode)
							MessageBox.Show("Cannot Label an AutoLabel.", "AutoLabel error");
						return;
					}
					if(value != null && AutoLabel.AutoLabelMap.Contains(value))
					{
						if(this.DesignMode)
							MessageBox.Show("Cannot set LabeledControl: " + value.Text + ", since it's already associated with an AutoLabel", "AutoLabel error");
						return;
					}

					this.OnDetachLabeledControl(this.labeledControl);
					Control oldValue = this.labeledControl;

					this.labeledControl = value;
					
					this.OnAttachLabeledControl(value);

					this.UpdatePosition();
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "LabeledControl", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Gets / sets the relative position of the control and the AutoLabel.
		/// </summary>
		[
		Description("Specifies the relative position of the control and the AutoLabel."),
		DefaultValue(AutoLabelPosition.Side),
		Category("Layout Information")
		]
		public AutoLabelPosition Position
		{
			get{return this.position;}
			set
			{
				if(this.position != value)
				{
					AutoLabelPosition oldValue = this.position;

					this.position = value;
					
					this.UpdatePosition();
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Position", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Returns the preferred height of the control.
		/// </summary>
		[Browsable(false), SRCategory("CatLayout"), SRDescription("LabelPreferredHeightDescr"), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int PreferredHeight
		{
			get
			{
				Size textSize = MesureText( this.Text, this.Font );

				if ( this.BorderStyle != BorderStyle.None )
				{
					return ( textSize.Height + DEF_WITH_BORDERS );
				}

				return ( textSize.Height + DEF_WITHOUT_BORDERS );
			}
		}

		/// <summary>
		/// Gets or sets the horizontal and vertical gap to use when computing the relative position.
		/// </summary>
		[
		Description("Specifies the horizontal and vertical gap to use when computing the relative position."),
		DefaultValue(4),
		Category("Layout Information")
		]
		public int Gap
		{
			get{return this.gap;}
			set
			{
				if(this.gap != value)
				{
					int oldValue = this.gap;

					this.gap = value;
					
					this.UpdatePosition();
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Gap", oldValue, value));
				}
			}
		}

		/// <summary>
		/// Gets or sets the effective horizontal distance between the left of the AutoLabel and its labeled control.
		/// </summary>
		/// <remarks>When RightToLeft == Yes DX is the distance between the right of the labeled control
		/// and the right of the AutoLabel.</remarks>
		[
		Description("The effective horizontal distance between the left of the AutoLabel and its labeled control."),
		Category("Layout Information"),
        DefaultValue(0)
		]
		public int DX
		{
			get
			{
				if(this.LabeledControl == null)
					return 0;

				if(this.Position == AutoLabelPosition.Custom)
					return dx;
				else
				{
					if(this.RightToLeft == RightToLeft.Yes)
						return this.LabeledControl.Right - this.Right;
					else
						return this.Bounds.X - this.LabeledControl.Bounds.X;
				}	
			}
			set
			{
				if(this.dx != value)
				{
					int oldValue = dx;
					this.dx = value;

					this.UpdatePosition();
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "DX", oldValue, value));
				}
			}
		}
		/// <summary>
		/// Gets or sets the effective vertical distance between the top of the AutoLabel and its labeled control.
		/// </summary>
		[
		Description("The effective vertical distance between the top of the AutoLabel and its labeled control."),
        Category("Layout Information"), DefaultValue(0)
		]
		public int DY
		{
			get
			{
				if(this.LabeledControl == null)
					return 0;

				if(this.Position == AutoLabelPosition.Custom)
					return dy;
				else
					return this.Bounds.Y - this.LabeledControl.Bounds.Y;
			}
			set
			{
				if(this.dy != value)
				{
					int oldValue = dy;
					this.dy = value;

					this.UpdatePosition();
					this.OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "DY", oldValue, value));

				}
			}
		}
		#endregion PROPERTIES

		#region PRIVATE METHODS
		/// <summary>
		/// Calculates the size of the label's text.
		/// </summary>
		/// <param name="text">Text for measuring.</param>
		/// <param name="font">Current font.</param>
		/// <returns>Size of the text.</returns>
		private Size MesureText( string text, Font font )
		{
			if( text == null )
				throw new ArgumentNullException( "text" );

			if( font == null )
				throw new ArgumentNullException( "font" );

			SizeF size;
			using( Graphics g = this.CreateGraphics() )
			{
				StringFormat format = StringFormat.GenericDefault;
				format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

				size = g.MeasureString( text, font, int.MaxValue, format );
			}

			return size.ToSize();
		}
		#endregion
	}

	public class AutoLabelDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		public AutoLabelDesigner ( )
			: base()
		{

		}

		public override void Initialize (IComponent component)
		{
			base.Initialize(component);
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		System.ComponentModel.Design.DesignerActionListCollection actionLists;

		public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
		{
			get
			{
				if (null == actionLists)
				{
					actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
					actionLists.Add(
						new AutoLabelActionList(this.Component));
				}
				return actionLists;
			}
		}

#endif
	}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    public class AutoLabelActionList : SyncActionListBase<AutoLabel>
    {
		public AutoLabelActionList (IComponent component)
            : base(component)
        {
        }

        protected override void InitializeActionList()
        {
            this.AddDesignerActionHeaderItem("Essential Tools - Auto Label");

			//Design category.
			this.AddDesignerActionHeaderItem("Design");
            this.AddDesignerActionPropertyItem("Name", "Name", "Design", "Indicates the Name used for the control.");
			this.AddDesignerActionPropertyItem("Text", "Text", "Design", "Indicates the text to be displayed in the control.");

            //Appearance category.
			this.AddDesignerActionHeaderItem("Layout");
			this.AddDesignerActionPropertyItem("AutoSize", "Auto Size", "Layout", "Enabled automatic resizing.");

			this.AddDesignerActionHeaderItem("Layout Information");
			this.AddDesignerActionPropertyItem("DX", "DX", "Layout Information", "Indicates the horizontal distance between the left of AutoLabel and its labeled control.");
			this.AddDesignerActionPropertyItem("DY", "DY", "Layout Information", "Indicates the vertical distance between the left of AutoLabel and its labeled control.");
			this.AddDesignerActionPropertyItem("Gap", "Gap", "Layout Information", "Indicates the horizontal and vertical gap to use when computing relative position.");
			this.AddDesignerActionPropertyItem("LabeledControl", "Labeled Control", "Layout Information", "Specifies the control that is being labeled.");
			this.AddDesignerActionPropertyItem("Position", "Position", "Layout Information", "Specifies the relative position of the Labeled control and AutoLabel.");

        }
        public string Name
        {

            get
            {
				string name = string.Empty;
                if (this.Control != null)
                {
                    AutoLabel control = this.Control as AutoLabel;
                    name = control.Name;
                }
                return name;
            }
            set
            {
                SetValue("Name", value);
            }
        }

		public string Text
		{

			get
			{
				string text = string.Empty;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					text = control.Text;
				}
				return text;
			}
			set
			{
				SetValue("Text", value);
			}
		}

		public bool AutoSize
		{
			get
			{
				bool autoSize = true;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					autoSize = control.AutoSize;
				}
				return autoSize;
			}
			set
			{
				SetValue("AutoSize", value);
			}
		}

		public int DX
		{
			get
			{
				int dx = 0;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					dx = control.DX;
				}
				return dx;
			}
			set
			{
				SetValue("DX", value);
			}
		}

		public int DY
		{
			get
			{
				int dy = 0;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					dy = control.DY;
				}
				return dy;
			}
			set
			{
				SetValue("DY", value);
			}
		}

		public int Gap
		{
			get
			{
				int gap = 4;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					gap = control.Gap;
				}
				return gap;
			}
			set
			{
				SetValue("Gap", value);
			}
		}

		public Control LabeledControl
		{
			get
			{
				Control labeledControl = null;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					labeledControl = control.LabeledControl;
				}
				return labeledControl;
			}
			set
			{
				SetValue("LabeledControl", value);
			}
		}

		public AutoLabelPosition Position
		{
			get
			{
				AutoLabelPosition position = AutoLabelPosition.Side;
				if (this.Control != null)
				{
					AutoLabel control = this.Control as AutoLabel;
					position = control.Position;
				}
				return position;
			}
			set
			{
				SetValue("Position", value);
			}
		}
	}
#endif


}