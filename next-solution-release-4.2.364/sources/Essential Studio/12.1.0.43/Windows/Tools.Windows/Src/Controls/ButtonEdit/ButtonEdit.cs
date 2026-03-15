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
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Globalization;

using Syncfusion.Core.Licensing;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Design;
using System.Diagnostics;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Used for processing properties in PropertyGrid of TextBox which belongs to ButtonEdit control.
	/// </summary>
	public abstract class CustomPropertiesTypeConverter: ReferenceConverter
	{
		#region Class members

		protected static readonly Attribute[] EmptyAttributes = new Attribute[0];

		#endregion

		#region Class initialize

		protected CustomPropertiesTypeConverter()
			: base( typeof( TextBoxExt ) )
		{
		}

		#endregion

		#region Class methods

		protected abstract Attribute[] GetPropertyAttributes( TextBoxExt component, PropertyDescriptor property );

		public AttributeCollection GetExistingCollection( AttributeCollection existing, params Attribute[] newAttributes )
		{
			if( existing == null )
			{
				throw new ArgumentNullException( "existing" );
			}
			if( newAttributes == null )
			{
				newAttributes = new Attribute[0];
			}

			Attribute[] array = new Attribute[existing.Count + newAttributes.Length];
			int length = existing.Count;
			existing.CopyTo( array, 0 );

			for( int i = 0; i < newAttributes.Length; i++ )
			{
				if( newAttributes[i] == null )
				{
					throw new ArgumentNullException( "newAttributes" );
				}
				bool flag = false;
				for( int j = 0; j < existing.Count; j++ )
				{
					if( array[j].TypeId.Equals( newAttributes[i].TypeId ) )
					{
						flag = true;
						array[j] = newAttributes[i];
						break;
					}
				}
				if( !flag )
				{
					array[length++] = newAttributes[i];
				}
			}
			Attribute[] destinationArray = null;
			if( length < array.Length )
			{
				destinationArray = new Attribute[length];
				Array.Copy( array, 0, destinationArray, 0, length );
			}
			else
			{
				destinationArray = array;
			}

			return new AttributeCollection( destinationArray );
		}

		#endregion

		#region Class overrides

		public override bool GetPropertiesSupported( ITypeDescriptorContext context )
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] filter )
		{
			if( value != null )
			{
				TextBoxExt component = value as TextBoxExt;
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( value, true );
				ArrayList descriptors = new ArrayList( properties.Count );
				foreach( PropertyDescriptor property in properties )
				{
					Attribute[] propertyAttributes = GetPropertyAttributes( component, property );
					AttributeCollection attributeCollection = GetExistingCollection( property.Attributes, propertyAttributes );

					if( attributeCollection.Contains( filter ) )
					{
						Attribute[] attrs = new Attribute[attributeCollection.Count];
						attributeCollection.CopyTo( attrs, 0 );
						descriptors.Add( new CustomPropertyDescriptor( property, attrs ) );
					}
				}

				PropertyDescriptor[] pdArr = new PropertyDescriptor[descriptors.Count];
				descriptors.CopyTo( pdArr );
				return new PropertyDescriptorCollection( pdArr );
			}

			return TypeDescriptor.GetProperties( typeof( TextBoxExt ), filter, true );
		}

		#endregion

		#region Class internal declaration

		/// <summary>
		/// Necessary for settings of the attributes of the property.
		/// </summary>
		private sealed class CustomPropertyDescriptor: SimplePropertyDescriptor
		{
			#region Class members

			private readonly PropertyDescriptor inner;

			#endregion

			#region Class properties

			private PropertyDescriptor InnerPropertyDescriptor
			{
				[DebuggerStepThrough]
				get
				{
					return inner;
				}
			}

			#endregion

			#region Class initialize

			public CustomPropertyDescriptor( PropertyDescriptor inner, Attribute[] attributes )
				: base( inner != null ? inner.ComponentType : null,
						inner != null ? inner.Name : null,
						inner != null ? inner.PropertyType : null,
						attributes )
			{
				if( inner == null )
				{
					throw new ArgumentNullException( "inner" );
				}
				this.inner = inner;
			}

			#endregion

			#region Class overrides

			public override object GetValue( object component )
			{
				return InnerPropertyDescriptor.GetValue( component );
			}

			public override void SetValue( object component, object value )
			{
				InnerPropertyDescriptor.SetValue( component, value );
			}

			#endregion
		}

		#endregion
	}


	/// <summary>
	/// Used for processing of TextBox property which belongs to ButtonEdit control.
	/// </summary>
	class TextBoxExtConverter: CustomPropertiesTypeConverter
	{
		#region Class properties
		/// <summary></summary>
		private const string PROPERTY_DOCK = "Dock";
		/// <summary></summary>
		private const string PROPERTY_BORDER_STYLE = "BorderStyle";
		#endregion

		#region Class overrides

		protected override Attribute[] GetPropertyAttributes( TextBoxExt component, PropertyDescriptor property )
		{
			if( property.Name == PROPERTY_DOCK || property.Name == PROPERTY_BORDER_STYLE )
			{
				return new Attribute[] { BrowsableAttribute.No };
			}

			return EmptyAttributes; // this field is defined in the base class.
		}

		#endregion
	}


	/// <summary>
	/// The ButtonEdit class provides an easy way to create controls
	/// with an edit control (<see cref="TextBox"/> and <see cref="ComboBox"/>) and any number of associated buttons.
	/// </summary>
	/// <remarks><para>
	/// The buttons can be set to be aligned to either side of the edit control.
	/// </para><para>
	/// The buttons are derived from the <see cref="Button"/> class and are implemented
	/// in the <see cref="ButtonEditChildButton"/> class. This class provides a customized
	/// version of Button that can work with the ButtonEdit class.
	/// </para><para>
	/// The ButtonEdit class implements the <see cref="IButtonEditParent"/> interface
	/// that enables it to act as a listener with the ButtonEditChildButton class. The
	/// ButtonEdit class listens for change notifications from the child ButtonEditChildButton
	/// controls and adjusts its layout accordingly.
	/// </para><para>
	/// The buttons can be added to the ButtonEdit control through the designer. The buttons
	/// will be automatically laid out. The ButtonEdit control uses <see cref="GridBagLayout"/>
	/// to layout the buttons.
	/// </para><para>
	/// The ButtonEdit class can be easily derived from to replace the standard
	/// edit control with a specialized TextBox derived class.
	/// </para><para>
	/// The ButtonEdit class supports the Windows Forms styles and can be used as a regular TextBox control.
	/// </para></remarks>
	/// <example><code lang="C#">
	///             // InitializeComponent sample
	///             this.buttonEdit1 = new Syncfusion.Windows.Forms.Tools.ButtonEdit();
	///             this.buttonEditChildButton1 = new Syncfusion.Windows.Forms.Tools.ButtonEditChildButton();
	///             this.buttonEdit1.SuspendLayout();
	///             this.SuspendLayout();
	///             this.buttonEdit1.Buttons.Add(this.buttonEditChildButton1);
	///             this.buttonEdit1.Controls.AddRange(new System.Windows.Forms.Control[] {
	///                                                                                       this.buttonEditChildButton1});
	///             this.buttonEdit1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
	///             this.buttonEdit1.Location = new System.Drawing.Point(8, 16);
	///             this.buttonEdit1.Name = "buttonEdit1";
	///             this.buttonEdit1.SelectionLength = 0;
	///             this.buttonEdit1.SelectionStart = 0;
	///             this.buttonEdit1.ShowTextBox = true;
	///             this.buttonEdit1.Size = new System.Drawing.Size(368, 22);
	///             this.buttonEdit1.TabIndex = 0;
	///             this.buttonEdit1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
	///             //
	///             // buttonEditChildButton1
	///             //
	///             this.buttonEditChildButton1.ButtonAlign = Syncfusion.Windows.Forms.Tools.ButtonAlignment.Right;
	///             this.buttonEditChildButton1.ButtonEditParent = this.buttonEdit1;
	///             this.buttonEditChildButton1.ButtonType = Syncfusion.Windows.Forms.Tools.ButtonTypes.Browse;
	///             this.buttonEditChildButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
	///             this.buttonEditChildButton1.Location = new System.Drawing.Point(352, 0);
	///             this.buttonEditChildButton1.Name = "buttonEditChildButton1";
	///             this.buttonEditChildButton1.PreferredWidth = 16;
	///             this.buttonEditChildButton1.Size = new System.Drawing.Size(16, 22);
	///             this.buttonEditChildButton1.TabIndex = 1;
	///             this.buttonEditChildButton1.Click += new System.EventHandler(this.buttonEditChildButton1_Click);
	///             this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
	///             this.ClientSize = new System.Drawing.Size(400, 273);
	///             this.Controls.AddRange(new System.Windows.Forms.Control[] {      this.buttonEdit1});
	///             this.Text = "Syncfusion ButtonEdit Demo";
	///             this.buttonEdit1.ResumeLayout(false);
	///             this.ResumeLayout(false);</code><coderef file="c:\syncfusion\essential suite\tools\samples\quick start\buttoneditdemo\VB\MainForm.vb" name="ButtonEdit InitializeComponent" lang="VB"><code lang="VB">
	///            ' InitializeComponent sample
	///            Me.buttonEdit1 = New Syncfusion.Windows.Forms.Tools.ButtonEdit()
	///            Me.buttonEditChildButton1 = New Syncfusion.Windows.Forms.Tools.ButtonEditChildButton()
	///            Me.buttonEdit1.SuspendLayout()
	///            Me.SuspendLayout()
	///            Me.buttonEdit1.Buttons.Add(Me.buttonEditChildButton1)
	///            Me.buttonEdit1.Controls.AddRange(New System.Windows.Forms.Control() {Me.buttonEditChildButton1})
	///            Me.buttonEdit1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
	///            Me.buttonEdit1.Location = New System.Drawing.Point(8, 16)
	///            Me.buttonEdit1.Name = "buttonEdit1"
	///            Me.buttonEdit1.SelectionLength = 0
	///            Me.buttonEdit1.SelectionStart = 0
	///            Me.buttonEdit1.ShowTextBox = True
	///            Me.buttonEdit1.Size = New System.Drawing.Size(368, 22)
	///            Me.buttonEdit1.TabIndex = 0
	///            Me.buttonEdit1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
	///            '
	///            ' buttonEditChildButton1
	///            '
	///            Me.buttonEditChildButton1.ButtonAlign = Syncfusion.Windows.Forms.Tools.ButtonAlignment.Right
	///            Me.buttonEditChildButton1.ButtonEditParent = Me.buttonEdit1
	///            Me.buttonEditChildButton1.ButtonType = Syncfusion.Windows.Forms.Tools.ButtonTypes.Browse
	///            Me.buttonEditChildButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
	///            Me.buttonEditChildButton1.Location = New System.Drawing.Point(352, 0)
	///            Me.buttonEditChildButton1.Name = "buttonEditChildButton1"
	///            Me.buttonEditChildButton1.PreferredWidth = 16
	///            Me.buttonEditChildButton1.Size = New System.Drawing.Size(16, 22)
	///            Me.buttonEditChildButton1.TabIndex = 1
	///            AddHandler Me.buttonEditChildButton1.Click, New System.EventHandler(AddressOf buttonEditChildButton1_Click)
	///            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
	///            Me.ClientSize = New System.Drawing.Size(400, 273)
	///            Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.buttonEdit1})
	///            Me.Text = "Syncfusion ButtonEdit Demo"
	///            Me.buttonEdit1.ResumeLayout(False)
	///            Me.ResumeLayout(False)</code></coderef></example>
	[
	ToolboxBitmap( typeof( ButtonEdit ), "ToolboxIcons.ButtonEdit.bmp" ),
	Designer( typeof( ButtonEditDesigner ), typeof( IDesigner ) ),
		//DesignerSerializer( typeof( ButtonEditSerializer ), typeof( CodeDomSerializer ) ),
	DefaultProperty( @"Text" ),
	Description( "Provides an easy way to create controls with a textbox and any number of associated buttons." )
	]
	public class ButtonEdit:
		ContainerControl,
		IEditControlsEmbed,
		IButtonEditParent,
		IPopupParent,
		IThemedControl,
		ISupportInitialize,
		INonClientPaintingSupport,
        IVisualStyle 
	{
		#region Class constants
		/// <summary>
		/// The width of the dropdown button.
		/// </summary>
		protected readonly int DropDownButtonWidth = 18;
		/// <summary></summary>
		private const int c_nDOUBLE_BORDER = 2;
		/// <summary></summary>
		private const int c_nTEXT_OFFSET = 3;
		/// <summary></summary>
		private const int c_nADJUST_XPOS = 1;
		/// <summary></summary>
		private const int c_nADJUST_HEIGHT = 4;
		#endregion

		#region Class members
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		/// <summary>
		/// The flat style to be applied across all buttons.
		/// </summary>
		private FlatStyle flatStyleValue = FlatStyle.Standard;
		/// <summary>
		/// Collection of ButtonEditChildButtons.
		/// </summary>
		private ButtonEditChildButtonCollection buttonsCollection;
		/// <summary>
		/// The host form.
		/// </summary>
		protected internal Form hostFormObject = null;
		/// <summary>
		/// The textbox.
		/// </summary>
		private TextBoxExt buttonTextBox;
		/// <summary>
		/// The resource manager.
		/// </summary>
		internal ResourceManager rm;
		/// <summary></summary>
		private IEditControlsEmbedListener listener;
		/// <summary>
		/// The IContainerControl Parent
		/// </summary>
		private IContainerControl parentContainer = null;
		/// <summary></summary>
		private int dropDownButtonHeight = 17;
		/// <summary></summary>
		private bool preventHeightChange = true;
		/// <summary></summary>
		internal int editPortionHeight = 0;
		/// <summary></summary>
		private static Color DefaultButtonEditBackColor = SystemColors.Window;
		/// <summary></summary>
		internal bool disposing = false;
		/// <summary></summary>
		private Color flatBorderColor = SystemColors.ControlDark;
		/// <summary></summary>
		private Border3DSide borderSides = Border3DSide.All;
		/// <summary></summary>
		private Border3DStyle border3DStyle = Border3DStyle.Sunken;
		/// <summary></summary>
		private ControlDrawing cd;
		/// <summary></summary>
		private ThemedEditDrawing themedDrawing = null;
		/// <summary></summary>
		private ButtonAppearance appearance = ButtonAppearance.WindowsXP;
		//private bool isActive = false;
		/// <summary></summary>
		private int borderFactor = 0;
		/// <summary></summary>
		private int spacingFactor = 0;
		/// <summary></summary>
		private bool useVisualStyle = false;
		/// <summary></summary>
		private int textBoxWidth = 0;
		/// <summary></summary>
		private ButtonAdvState state = ButtonAdvState.Default;
		/// <summary>
		/// The color scheme that the renderer will render. 
		/// </summary>
		private WindowsXPColorScheme colorScheme = WindowsXPColorScheme.DefaultBlueCombo;
		/// <summary></summary>
		private ArrayList childControls;
		/// <summary></summary>
		private bool m_bHandleFocusChanged = false;
		/// <summary></summary>
		private EditNativeWindow m_nativeWindow;
		/// <summary></summary>
		private bool droppedDown = false;
		/// <summary></summary>
		private IntPtr cachedRgn = IntPtr.Zero;
		/// <summary></summary>
		private bool isTextBoxVisible = true;
		/// <summary></summary>
		private int textBoxHeight;
		/// <summary></summary>
		private bool m_showTextBox = true;
        /// <summary>MetroColor</summary>
        private Color m_metroColor = Color.FromArgb(22,165,220);

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);




        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the theme color of the ButtonAdv
        /// </summary>
        [
        Browsable(true),
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the pressed background color of the control.")
        ]
        public Color MetroColor
        {
            get { return m_metroColor; }
            set
            {
                m_metroColor = value;
                ApplyStyle();
            }
        }
	
		/// <summary>
		/// Gets or sets the backcolor of the ButtonEdit control.
		/// </summary>
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}

			set
			{
				if( null != this.TextBox )
				{
					base.BackColor = value;
					buttonTextBox.BackColor = value;
					if( this.BackColor == ButtonEdit.DefaultButtonEditBackColor )
					{
						buttonTextBox.ResetBackColor();
					}
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Specifies whether the ComboBoxBase control modifies the case of characters as they are typed.
		/// </summary>
		/// <value><para>One of the <see cref="System.Windows.Forms.CharacterCasing"/> enumeration values that specifies whether the ComboBoxBase control modifies the case of characters. The default is CharacterCasing.Normal.</para></value>
		[
		  Category( "Behavior" ), DefaultValue( CharacterCasing.Normal ),
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		  Description( "Specifies whether the ComboBoxBase control modifies the case of characters as they are typed." )
		]
		public virtual CharacterCasing CharacterCasing
		{
			get
			{
				CharacterCasing c = CharacterCasing.Normal;
				if( buttonTextBox != null )
				{
					c = buttonTextBox.CharacterCasing;
				}
				return c;
			}
			set
			{
				if( null != buttonTextBox && buttonTextBox.CharacterCasing != value )
				{
					buttonTextBox.CharacterCasing = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignment of text in this control.
		/// </summary>
		/// <value>
		/// One of the <see cref="System.Windows.Forms.HorizontalAlignment"/> enumeration 
		/// values that specifies how text is aligned in the control. 
		/// The default is <b>HorizontalAlignment.Left</b>.
		/// </value>
		/// <remarks>
		/// You can use this property to align the text within a ComboBoxBase 
		/// to match the layout of text on your form. For example, if your controls 
		/// are all located on the right side of the form, you can set the TextAlign 
		/// property to <b>HorizontalAlignment.Right</b>, and the text will be aligned 
		/// along the right side of the control instead of the default left alignment.
		/// </remarks>
		[
		  Category( "Appearance" ), DefaultValue( HorizontalAlignment.Left ),
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		  Description( "Specifies how the text is aligned in this control." )
		]
		public virtual HorizontalAlignment TextAlign
		{
			get
			{
				HorizontalAlignment align = HorizontalAlignment.Left;
				if( buttonTextBox != null )
				{
					align = buttonTextBox.TextAlign;
				}
				return align;
			}
			set
			{
				if( null != buttonTextBox && buttonTextBox.TextAlign != value )
				{
					buttonTextBox.TextAlign = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or Sets the Forecolor based on layout of the text.
		/// </summary>
		/// <override/>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Color ForeColor
		{
			get
			{
				Color c = Color.Empty;
				if( buttonTextBox != null )
				{
					c = buttonTextBox.ForeColor;
				}
				return c;

			}
			set
			{
				if( null != buttonTextBox && buttonTextBox.ForeColor != value )
				{
					buttonTextBox.ForeColor = value;
					base.ForeColor = buttonTextBox.ForeColor;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Use this property to bind the TextBox.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
	   Description( "Use this property to bind the TextBox." )]
		public ControlBindingsCollection TextBoxBindings
		{
			get
			{
				ControlBindingsCollection c = null;
				if( buttonTextBox != null )
					c = buttonTextBox.DataBindings;
				return c;
			}

			set
			{
				if( buttonTextBox != null )
				{
					ControlBindingsCollection bindings;
					bindings = (ControlBindingsCollection)value;

					buttonTextBox.DataBindings.Clear();
					foreach( Binding b in bindings )
					{
						buttonTextBox.DataBindings.Add( b );
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the ParentContainer control that implements IContainerControl.
		/// </summary>
		/// <remarks>
		/// Set this property to a Form or UserControl if its not the same
		/// as the Parent of the control.
		/// </remarks>
		[
		DefaultValue( null ),
		Description( "Gets or sets the ParentContainer control that implements IContainerControl." )
		]
		public IContainerControl ParentContainer
		{
			get
			{
				return this.parentContainer;
			}

			set
			{
				this.parentContainer = value;
			}
		}

		/// <summary>
		/// Gets or sets the collection of Buttons that makes up this ButtonEdit control.
		/// </summary>
		/// <remarks>
		/// The Buttons property is a collection of type <see cref="ButtonEditChildButtonCollection"/>
		/// that includes all the child buttons that are embedded as child controls in the
		/// ButtonEdit control. 
		/// <para>
		/// You can add and remove buttons and edit their properties through the designer.
		/// </para></remarks>
		[
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		  MergableProperty( false ),
		  Category( @"Behavior" ),
		  Localizable( true ),
		  Description( @"The collection of Buttons that make up this ButtonEdit control." )
		]
		public ButtonEditChildButtonCollection Buttons
		{
			get
			{
				return this.buttonsCollection;
			}
		}

		/// <summary>
		/// Gets or sets the background image
		/// </summary>
		[Browsable( false ),
	   Description( "Gets or sets the background image" )]

		public override Image BackgroundImage
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the drop-down portion is displayed or not.
		/// </summary>
		private bool DroppedDown
		{
			get
			{
				return this.droppedDown;
			}

			set
			{
				this.droppedDown = value;
			}
		}

		/// <summary>
		/// Indicates whether to use visual styles.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		DefaultValue( false ),
		Description( "Indicates whether to use visual styles." )
		]
		public bool UseVisualStyle
		{
			get
			{
				return useVisualStyle;
			}

			set
			{
				if( useVisualStyle != value )
				{
					useVisualStyle = value;
					SetBaseStyle();
					ApplyStyle();
				}
			}
		}
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                if(style != null )
                    useVisualStyle = true;
                else
                    useVisualStyle = false;
                switch (value)
                {
                    case "Office2000":
                        ButtonStyle = ButtonAppearance.Office2000;
                        break;
                    case "Classic":
                        ButtonStyle = ButtonAppearance.Classic;
                        break;
                    case "Office2003":
                        ButtonStyle = ButtonAppearance.Office2003;
                        break;
                    case "OfficeXP":
                        ButtonStyle = ButtonAppearance.OfficeXP;
                        break;
                    case "None":
                        ButtonStyle = ButtonAppearance.None;
                        break;
                    case "Office2007":
                    case "Office2007Blue":
                    case "Office2007Black":
                    case "Office2007Silver":
                    case "Managed":
                        ButtonStyle = ButtonAppearance.Office2007;
                        break;
                    case "Metro":
                        ButtonStyle = ButtonAppearance.Metro;
                        this.ApplyStyle();
                        break;
                }
           }
        }

		/// <summary>
		/// Gets or sets the button style for the control.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance - Styles" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		DefaultValue( ButtonAppearance.WindowsXP ),
		AmbientValue( ButtonAppearance.None ),
		Description( "Gets or sets the button style for the control." )
		]
		public ButtonAppearance ButtonStyle
		{
			get
			{
				return appearance;
			}
			set
			{
				if( appearance != value )
				{
					appearance = value;

					ComputeBorderFactor();
					this.ApplyStyle();
               		this.InvalidateWindow();
				}
			}
		}
		/// <summary>
		/// Gets or sets the state of the ButtonEdit control.
		/// </summary>
		[
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		  Browsable( false )
		]
		public ButtonAdvState State
		{
			get
			{
				return this.state;
			}

			set
			{
				this.state = value;
			}
		}
		/// <override/>
		/// <summary></summary>
		protected override Size DefaultSize
		{
			get
			{
				if( this.IsHandleCreated )
				{
					this.Layout();
					return new Size( 121, this.Height );
				}
				else
				{
					return new Size( 121, 21 );
				}
			}
		}

		/// <summary>
		/// Gets / sets the height of the drop-down button.
		/// </summary>
		protected int DropDownButtonHeight
		{
			get
			{
				return this.dropDownButtonHeight;
			}
			set
			{
				this.dropDownButtonHeight = value;
			}
		}

		/// <summary>
		/// Gets / sets the height of the edit portion.
		/// </summary>
		protected int EditPortionHeight
		{
			get
			{
				return this.editPortionHeight;
			}
			set
			{
				this.editPortionHeight = value;
			}
		}

		/// <summary>
		/// Returns the embedded <see cref="Syncfusion.Windows.Forms.Tools.TextBoxExt"/> control.
		/// </summary>
		/// <remarks>
		/// The TextBoxExt control is the core control of the <see cref="ButtonEdit"/> control.
		/// This control takes up the width of the control minus the widths of the individual
		/// buttons. This TextBoxExt control can be accessed through this property.
		/// </remarks>
		[
		  Category( "Appearance" ),
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		  TypeConverter( typeof( TextBoxExtConverter ) ),
		  Browsable( true )
		]
		public TextBoxExt TextBox
		{
			get
			{
				if( buttonTextBox == null )
				{
					buttonTextBox = CreateTextBox();
					AttachTextBox();
					this.BackColor = DefaultButtonEditBackColor;
				}

				return this.buttonTextBox;
			}
			set
			{
                if (value == null)
                {
                    throw new ArgumentNullException("value"); // We cannot add an empty TextBox to button.
                }
                else if( value != buttonTextBox ) // We should not operate on TextBox, previously added to our control
				{
					if( value.Parent != null )
					{
                        if (value.Parent.Controls.Count > 0)
                        {
                            foreach (Control c in value.Parent.Controls)
                            {
                                if (c != value && c.GetType() == typeof(TextBoxExt))
                                {
                                    value.Parent.Controls.Remove(c);
                                    c.Dispose();
                                }
                            }
                        }
						//value.Parent.Controls.Remove( value );
					}

					DetachTextBox(); // Removing previously attached checkbox
					buttonTextBox = value; // and adding a new one
					AttachTextBox();
				}
			}
		}

		/// <summary>
		/// The <see cref="ButtonEdit.Text"/> property of the <see cref="ButtonEdit"/> class is the same
		/// as the <see cref="System.Windows.Forms.TextBox"/> property of the embedded
		/// <see cref="ButtonEdit.TextBox"/> control.
		/// <value>A TextBox object.</value></summary>
		/// <remarks>
		/// The TextBox control's properties can be changed through the property grid in the
		/// designer. The TextBox control can be hidden from view by setting the <see cref="System.Windows.Forms.Control.Visible"/>
		/// property to false. The Text property is overriden to keep the text in sync with the
		/// embedded TextBox control's Text property.
		/// </remarks>
		[
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		  Browsable( true ),
		  Category( "Data" ),
		  Description( "The Text property of the ButtonEdit control is the same as the Text property of the embedded TextBox" ),
		  DefaultValue( "" )
		]
		public override string Text
		{
			get
			{
				string text = "";
				if( buttonTextBox != null )
				{
					text = buttonTextBox.Text;
				}
				return text;
			}
			set
			{
				if( null != this.TextBox && this.TextBox.Text != value )
				{
					this.TextBox.Text = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the embedded TextBox is visible in the ButtonEdit.
		/// </summary>
		/// <remarks>
		/// If the TextBox control is kept invisible, it will be ignored during the
		/// layout process and the space will be divided among the child <see cref="ButtonEdit.Buttons"/>.
		/// </remarks>
		[
		Category( "Appearance" ),
		DefaultValue( true ),
		Description( "Indicates whether the embedded TextBox is visible in the ButtonEdit." )
		]
		public virtual bool ShowTextBox
		{
			get
			{
				bool b = true;
				if( buttonTextBox != null )
				{
					b = m_showTextBox;
				}
				return b;
			}

			set
			{
				if( null != buttonTextBox && m_showTextBox!= value )
				{
					buttonTextBox.Visible = value;
					m_showTextBox = value;
					Layout();
				}
			}
		}

		/// <summary>
		/// Gets or sets the SelectionStart property of the ButtonEdit control which is same as the <see cref="System.Windows.Forms.TextBoxBase.SelectionStart"/>
		/// property of the embedded <see cref="System.Windows.Forms.TextBox"/>.
		/// </summary>
		/// <remarks>
		/// The ButtonEdit control shadows some of the properties of the embedded TextBox control.
		/// The SelectionStart property is shadowed enabling access to the ButtonEdit in a manner
		/// similar to the TextBox control for accessing the Text content of the control.
		/// </remarks>
		[
		  Category( "Appearance" ),
		  Description( @"The SelectionStart property of the embedded TextBox control." )
		]
		public int SelectionStart
		{
			get
			{
				int start = 0;
				if( null != buttonTextBox )
				{
					start = buttonTextBox.SelectionStart;
				}
				return start;
			}

			set
			{
				if( null != buttonTextBox && buttonTextBox.SelectionStart != value )
				{
					buttonTextBox.SelectionStart = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the Selection Length of the embedded TextBox control.
		/// </summary>
		/// <remarks>
		/// The ButtonEdit control shadows some of the properties of the embedded TextBox control.
		/// The SelectionLength property is shadowed enabling access to the ButtonEdit in a manner
		/// similar to the TextBox control for accessing the Text content of the control.
		/// </remarks>
		[
		  Category( "Appearance" ),
		  Description( @"The SelectionLength property of the embedded TextBox control." )
		]
		public int SelectionLength
		{
			get
			{
				int length = 0;
				if( null != buttonTextBox )
				{
					length = buttonTextBox.SelectionLength;
				}
				return length;
			}

			set
			{
				if( null != buttonTextBox && buttonTextBox.SelectionLength != value )
				{
					buttonTextBox.SelectionLength = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the FlatStyle to be applied to the buttons in the <see cref="ButtonEdit"/> control.
		/// </summary>
		/// <remarks>
		/// The ButtonEdit control applies the same FlatStyle to all the child controls
		/// in the layout. 
		/// </remarks>
		[
		  Category( "Appearance" ),
		  Description( @"The FlatStyle to be applied to the buttons in the ButtonEdit control." ),
		  DefaultValue( FlatStyle.Standard )
		]
		public FlatStyle FlatStyle
		{
			get
			{
				return this.flatStyleValue;
			}

			set
			{
				if( this.flatStyleValue != value )
				{
					this.flatStyleValue = value;
					this.ApplyStyle();
					this.PerformLayout();
				}
			}
		}

		/// <summary>
		/// Indicates whether the Height property of the control can be changed.
		/// </summary>
		/// <value>True to prevent height change; false otherwise.</value>
		/// <remarks>
		/// Note that this property will be frequently set and reset within the control layout.
		/// You can use this temporarily to force a particular height on the control.
		/// </remarks>
		protected bool PreventHeightChange
		{
			get
			{
				return this.preventHeightChange;
			}
			set
			{
				this.preventHeightChange = value;
			}
		}

		/// <summary>
		/// Indicates whether the Layout method needs to be called to layout the combo
		/// elements.
		/// </summary>
		/// <remarks>
		/// Internal method. You will not have to call this property explicitly.
		/// </remarks>
		[
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		  Browsable( false ),
		  EditorBrowsable( EditorBrowsableState.Never ),
		  Obsolete( "Do not use this property. It is obsolete." )
		]
		public bool NeedLayout
		{
			get
			{
				return false;
			}
		}

		/// <summary></summary>
		bool IPopupParent.IsRightToLeft
		{
			get
			{
				return ( RightToLeft.Yes == this.RightToLeft );
			}
		}

		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
		private bool ThemesEnabled
		{
			get
			{
				return ( (IThemedControl)this ).ThemesEnabled;
			}
		}

		/// <summary></summary>
		bool IThemedControl.ThemesEnabled
		{
			get
			{
				return this.FlatStyle == FlatStyle.System;
			}
			set
			{
				if( this.ThemesEnabled != value )
				{
					if( value )
					{
						this.FlatStyle = FlatStyle.System;
					}
					else
					{
						this.FlatStyle = FlatStyle.Standard;
					}
					this.OnThemeChanged( EventArgs.Empty );
				}
			}
		}
		/// <summary>
		/// Gets or sets the 3D border style for the control.
		/// </summary>
		/// <remarks>
		/// This property is used only when BorderStyle is Fixed3D.
		/// </remarks>
		[
		  Description( "Indicates the style of the 3D border." ),
		  Category( "Appearance" ),
		  DefaultValue( Border3DStyle.Sunken )
		]
		public Border3DStyle Border3DStyle
		{
			get
			{
				return border3DStyle;
			}
			set
			{
				if( border3DStyle != value )
				{
					border3DStyle = value;
					this.OnBorder3DStyleChanged( EventArgs.Empty );
					this.InvalidateWindow();
				}
			}
		}
		/// <summary>
		/// Gets or sets the border sides for which you want the 3D border style applied.
		/// </summary>
		/// <remarks>
		/// This property is used only when BorderStyle is Fixed3D.
		/// </remarks>
		[
		  Description( "Indicates the border sides for which to use 3D borders in the control." ),
		  Category( "Appearance" ),
		  DefaultValue( Border3DSide.All )
		]
		public Border3DSide BorderSides
		{
			get
			{
				return borderSides;
			}
			set
			{
				if( borderSides != value )
				{
					borderSides = value;
					this.OnBorderSidesChanged( EventArgs.Empty );
					InvalidateWindow();
				}
			}
		}
		/// <summary>
		/// Gets or sets the color with which the Flat Border should be drawn.
		/// </summary>
		/// <value>
		/// A Color value. Default is SystemColors.ControlDark.
		/// </value>
		[
		  Category( "Appearance" ),
		  Description( "Specifies the Color with which the Flat Border should be drawn." ),
		  Browsable( true ),
		  DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
		]
		public virtual Color FlatBorderColor
		{
			get
			{
				if( this.flatBorderColor != Color.Empty )
				{
					return this.flatBorderColor;
				}
				else
				{
					return SystemColors.ControlDark;
				}
			}
			set
			{
				if( this.flatBorderColor != value )
				{
					this.flatBorderColor = value;
					this.InvalidateWindow();
				}
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Gets or sets the maximum size of the control.
		/// </summary>
		/// <value>
		/// A Size value. Default is (0,0).
		/// </value>
		[
		  Description( "Gets or sets the maximum size of the control." ),
		  Category( "Layout" )
		]
		public override Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
				if( base.MaximumSize != value )
				{
					if( value.Height != 0 || value.Width != 0 )
					{
						if( value.Height == 0 )
						{
							value.Height = this.Height;
						}
						if( value.Width == 0 )
						{
							value.Width = this.Width;
						}
					}

					base.MaximumSize = value;
					this.OnMaximumSizeChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum size of the control.
		/// </summary>
		/// <value>
		/// A Size value. Default is (0,0).
		/// </value>
		[
		  Description( "Gets or sets the minimum size of the control." ),
		  Category( "Layout" )
		]
		public override Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
				if( base.MinimumSize != value )
				{
					if( value.Height != 0 || value.Width != 0 )
					{
						if( value.Height == 0 )
						{
							value.Height = this.Height;
						}
						if( value.Width == 0 )
						{
							value.Width = this.Width;
						}
					}

					base.MinimumSize = value;
					this.OnMinimumSizeChanged( EventArgs.Empty );
				}
			}
		}
		/// <summary>
		/// Gets or sets the background image layout
		/// </summary>
		[Browsable( false ), Description( "Gets or sets the background image layout" ), DefaultValue(typeof(ImageLayout), "None")]
		public override System.Windows.Forms.ImageLayout BackgroundImageLayout
		{
			get
			{
				return System.Windows.Forms.ImageLayout.None;
			}
			set
			{ }
		}
#endif

		internal bool IsDesignMode
		{
			get
			{
				return this.DesignMode;
			}
		}

		#endregion

		#region Class events
		/// <summary>
		/// Raised when one of the child <see cref="ButtonEditChildButton"/> is clicked.
		/// </summary>
		/// <remarks>
		/// Handle this event if you want to handle the click event of any of the child button.
		/// You can also add any of the events exposed by the child buttons themselves as the
		/// <see cref="System.Windows.Forms.Button"/> class derived buttons that raise the Click
		/// event.
		/// </remarks>
		[
		Category( "Behavior" ),
		Description( "This event is raised when a child button is clicked." )
		]
		public event ButtonClickedEventHandler ButtonClicked;
		/// <summary>
		/// Fired when the ThemesEnabled property changes.
		/// </summary>
		[Description( "This event will be fired when the ThemesEnabled property changes." ),
	   Category( "Appearance" )]
		public event EventHandler ThemeChanged;
		/// <summary>
		/// Fired when BorderSides property changes.
		/// </summary>
		[
		Category( "Property Changed" ),
		Description( "Fired when BorderSides property changes." )
		]
		public event EventHandler BorderSidesChanged;
		/// <summary>
		/// Fired when Border3DStyle property changes.
		/// </summary>
		[
		Category( "Property Changed" ),
		Description( "Fired when Border3DStyle property changes." )
		]
		public event EventHandler Border3DStyleChanged;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// This event is raised if the MaximumSize property is changed.
		/// </summary>
		[
		Category( "Property Changed" ),
		Description( "This event is raised if the MaximumSize property is changed." )
		]
		public event EventHandler MaximumSizeChanged;
		/// <summary>
		/// This event is raised if the MinimumSize property is changed.
		/// </summary>
		[
		Category( "Property Changed" ),
		Description( "This event is raised if the MinimumSize property is changed." )
		]
		public event EventHandler MinimumSizeChanged;

		/// <summary>
		/// Raises the MaximumSizeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnMaximumSizeChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnMaximumSizeChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnMaximumSizeChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnMaximumSizeChanged( EventArgs e )
		{
			if( MaximumSizeChanged != null )
			{
				MaximumSizeChanged( this, e );
			}
		}

		/// <summary>
		/// Raises the MinimumSizeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnMinimumSizeChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnMinimumSizeChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnMinimumSizeChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnMinimumSizeChanged( EventArgs e )
		{
			if( MinimumSizeChanged != null )
			{
				MinimumSizeChanged( this, e );
			}
		}
#endif
		#endregion

		#region Class initialize/finilize methods
		/// <summary>
		/// Creates an object of type <see cref="ButtonEdit"/> and initializes it.
		/// </summary>
		/// <remarks>
		/// The ButtonEdit class can be created by dragging and dropping a ButtonEdit control
		/// from the control toolbox. The constructor initializes the embedded TextBox.
		/// The embedded child controls will have to be explicitly added to the ButtonEdit
		/// object (this will be done by the designer if using the control through the Windows
		/// Forms designer).
		/// <para>
		/// The <see cref="CreateTextBox"/> method provides an easy way to change the default
		/// embedded TextBox to a specialized TextBox derived class such as the <see cref="MaskedEditBox"/>
		/// or the <see cref="CurrencyTextBox"/>.
		/// </para></remarks>
		public ButtonEdit()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new LicensedComponent( typeof( ButtonEdit ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			SetColorScheme();

			this.rm = new ResourceManager( "Syncfusion.Windows.Forms.Tools.ButtonEditIcons", Assembly.GetExecutingAssembly() );

			this.SetStyle( ControlStyles.AllPaintingInWmPaint
			            	| ControlStyles.Selectable, true );

			this.cd = new ControlDrawing();

			// Init buttons
			this.buttonsCollection = new ButtonEditChildButtonCollection( this );

			if( XPThemes.IsThemedOS )
			{
				this.themedDrawing = new ThemedEditDrawing( this );
			}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			this.MaximumSizeChanged += new EventHandler( HandleButtonEditMaximumSizeChanged );
			this.MinimumSizeChanged += new EventHandler( HandleButtonEditMinimumSizeChanged );
#endif
			this.DockChanged += new EventHandler( HandleButtonEditDockChanged );
            CTRLSIZE = this.Size;
		}

		/// <summary>
		/// Begins the initialization.
		/// </summary>
		public void /*ISupportInitialize*/ BeginInit()
		{
			OnBeginInit();
		}

		/// <summary>
		/// Begins the initialization.
		/// </summary>
		protected virtual void OnBeginInit()
		{
		}

		/// <summary>
		/// Indicates the object that initialization is complete.
		/// </summary>
		public void /*ISupportInitialize*/ EndInit()
		{
			this.OnEndInit();
		}

		/// <summary>
		/// Indicates  that the initialization is complete.
		/// </summary>
		protected virtual void OnEndInit()
		{
			SetBaseStyle();
			if( this.UseVisualStyle )
			{
				this.ApplyStyle();
			}
			Layout();
		}

		/// <summary>
		/// Creates a <see cref="System.Windows.Forms.TextBox"/> object.
		/// </summary>
		/// <returns>The TextBoxExt object that is created.</returns>
		/// <remarks>
		/// This method can be overriden to create a <see cref="Syncfusion.Windows.Forms.Tools.TextBoxExt"/> derived
		/// class to be returned. This will result in the TextBox object in the <see cref="ButtonEdit"/>
		/// control to be replaced with a derived control.
		/// </remarks>
		protected virtual TextBoxExt CreateTextBox()
		{
			return new TextBoxExt();
		}

		/// <summary>
		/// Initializes the <see cref="System.Windows.Forms.TextBox"/> used in the 
		/// editable text portion.
		/// </summary>
		/// <remarks><para>
		/// This method is called once to initialize the <b>TextBox</b> used to draw the 
		/// editable portion of the ButtonEdit. Use the <see cref="TextBox"/> property to get a 
		/// reference to the <b>TextBox</b> from inside an override of this method.
		/// </para><para>
		/// Make sure to call the base class when you override this method for 
		/// default initialization.
		/// </para><seealso cref="CreateTextBox"/></remarks>
		protected virtual void InitTextBox()
		{
			if( this.TextBox != null )
			{
				this.Controls.Add( this.TextBox );
				this.TextBox.TabIndex = 0;
				base.BackColor = this.TextBox.BackColor;
				this.TextBox.Dock = DockStyle.None;
				this.TextBox.Visible = this.isTextBoxVisible;

				this.TextBox.Parent = this;
				this.TextBox.Click += new EventHandler( this.HandleChildClicked );

				this.TextBox.MouseEnter += new EventHandler( HandleChildMouseEnter );
				this.TextBox.MouseLeave += new EventHandler( HandleChildMouseLeave );

				this.TextBox.LostFocus += new EventHandler( HandleChildLostFocus );
				this.TextBox.GotFocus += new EventHandler( HandleChildGotFocus );

				this.TextBox.TextChanged += new EventHandler( HandleEditTextChanged );

				this.TextBox.DockChanged += new EventHandler( HandleEditDockChanged );
				this.TextBox.SizeChanged += new EventHandler( HandleEditSizeChanged );
				this.TextBox.MultilineChanged += new EventHandler( HandleEditMultilineChanged );
				this.TextBox.BackColorChanged += new EventHandler( HandleEditBackColorChanged );

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				this.TextBox.MaximumSizeChanged += new EventHandler( HandleEditMaximumSizeChanged );
				this.TextBox.MinimumSizeChanged += new EventHandler( HandleEditMinimumSizeChanged );
#endif
				this.textBoxHeight = this.TextBox.Height;
			}
		}

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( m_nativeWindow != null )
				{
					m_nativeWindow.ReleaseHandle();
				}

				if( components != null )
				{
					components.Dispose();
				}

				int count = this.Buttons.Count;
				for( int i = 0; i < count; i++ )
				{
					this.Buttons[0].Dispose();
				}

				this.rm.ReleaseAllResources();

				this.buttonTextBox = null;

				if( this.themedDrawing != null )
				{
					this.themedDrawing.Dispose();
					this.themedDrawing = null;
				}
                this.Font = null;
      		}
			base.Dispose( disposing );
		}
		#endregion INIT

		#region Class codedom serialization
		/// <summary></summary>
		/// <returns></returns>
		protected bool ShouldSerializeBackColor()
		{
			return ( null != buttonTextBox ) && !( this.BackColor == ButtonEdit.DefaultButtonEditBackColor );
		}

		/// <summary>
		/// Resets to the default ButtonEditBackColor.
		/// </summary>
		/// <override/>
		public override void ResetBackColor()
		{
			this.BackColor = ButtonEdit.DefaultButtonEditBackColor;
			Invalidate();
		}

		/// <summary></summary>
		/// <returns></returns>
		protected bool ShouldSerializeFlatBorderColor()
		{
			return ( this.flatBorderColor != SystemColors.ControlDark );
		}

		/// <summary>
		/// Resets the color for the flat border to default.
		/// </summary>
		public void ResetFlatBorderColor()
		{
			this.flatBorderColor = SystemColors.ControlDark;
			this.Invalidate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeSelectionStart()
		{
			return null != buttonTextBox && buttonTextBox.SelectionStart != 0;
		}

		/// <summary>
		/// Resets the <see cref="SelectionStart"/> property to its default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void ResetSelectionStart()
		{
			if( null != buttonTextBox )
			{
				buttonTextBox.SelectionStart = 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeSelectionLength()
		{
			return null != buttonTextBox && buttonTextBox.SelectionLength != 0;
		}

		/// <summary>
		/// Resets the <see cref="SelectionLength"/> property to its default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void ResetSelectionLength()
		{
			if( null != buttonTextBox )
			{
				buttonTextBox.SelectionLength = 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeShowTextBox()
		{
			return null != buttonTextBox && buttonTextBox.Visible != true;
		}

		/// <summary>
		/// Resets the <see cref="ShowTextBox"/> property to its default value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public void ResetShowTextBox()
		{
			if( null != buttonTextBox )
			{
				buttonTextBox.Visible = true;
			}
		}

		#endregion

		#region Class paint logic
		/// <summary>
		/// Draws the border and background of the control.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
		/// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
		/// <remarks><para>
		/// This method is used to draw the border around the text area (when called from 
		/// <see cref="DrawEditPortionBorderAndBackground"/> method) and around the listbox area (when in 
		/// ComboBoxStyle.Simple mode and called from the <see cref="DrawListPortion"/> method).
		/// </para><para>This method uses themes to draw if necessary or calls <see cref="DrawBackground"/> and 
		/// <see cref="DrawBorder"/> to draw the background and border.</para></remarks>
		protected virtual void DrawBorderAndBackground( Graphics g, Rectangle rect )
		{
			if( this.UseVisualStyle )
			{
				this.DrawBackground( g, rect );
				this.DrawBorder( g, rect );
			}
			else
			{
				if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
				{
					if (this.themedDrawing!=null)
					this.themedDrawing.DrawEditBoxBackground( g, rect, this.Enabled );
				}
				else
				{
					this.DrawBackground( g, rect );

					this.DrawBorder( g, rect );
				}
			}
		}

		/// <summary>
		/// Draws the unthemed border of this control.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
		/// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
		/// <remarks><para>Called by <see cref="DrawBorderAndBackground(System.Drawing.Graphics, System.Drawing.Rectangle)"/> to draw the border when
		/// not in themes mode.</para></remarks>
		protected virtual void DrawBorder( Graphics g, Rectangle rect )
		{
			if( !this.UseVisualStyle )
			{
				DrawBordersInternal( g, rect );
			}
			else
			{
				Color borderColor = Color.Black;
				switch( this.ButtonStyle )
				{
					case ButtonAppearance.Classic:
					DrawBordersInternal( g, rect, BorderStyle.Fixed3D );
					break;
					case ButtonAppearance.Office2000:
					DrawBordersInternal( g, rect, BorderStyle.Fixed3D, Border3DStyle.Flat );
					break;
					case ButtonAppearance.Office2003:
					if( this.State == ButtonAdvState.Default )
					{
						borderColor = ButtonOffice2003Colors.BorderColor;
					}
					else
					{
						borderColor = ButtonOffice2003Colors.FocusBorderColor;
					}
					DrawBordersInternal( g, rect, BorderStyle.FixedSingle, Border3DStyle.Flat, borderColor );
					break;
                    case ButtonAppearance.Office2007:
                    DrawBordersInternal(g, rect, BorderStyle.FixedSingle);
                    break;
					case ButtonAppearance.OfficeXP:
					DrawBordersInternal( g, rect, BorderStyle.FixedSingle, Border3DStyle.Flat, ButtonOfficeXPColors.BorderColor );
					break;
					case ButtonAppearance.WindowsXP:
					switch( this.colorScheme )
					{
						case WindowsXPColorScheme.SilverCombo:
						DrawBordersInternal( g, rect, BorderStyle.FixedSingle, Border3DStyle.Flat, WindowsXPColors.SilverBorderColor );
						break;

						case WindowsXPColorScheme.OliveGreenCombo:
						DrawBordersInternal( g, rect, BorderStyle.FixedSingle, Border3DStyle.Flat, WindowsXPColors.OliveGreenComboBorderColor );
						break;

						default:
						DrawBordersInternal( g, rect, BorderStyle.FixedSingle, Border3DStyle.Flat, WindowsXPColors.DefaultBlueComboBorderColor );
						break;
					}
					break;
                    case ButtonAppearance.Metro:
                    {                        
                        cd.DrawBorder(g, rect, BorderStyle.FixedSingle, Border3DStyle.Flat, ButtonBorderStyle.Solid, this.m_metroColor);
                        break;
                    }
				}
			}
		}

		/// <summary>
		/// Draws the borders of the buttonEdit.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		private void DrawBordersInternal( Graphics g, Rectangle rect )
		{
			BorderStyle borderStyle = BorderStyle.Fixed3D;
			if( this.FlatStyle == FlatStyle.Flat )
			{
				borderStyle = BorderStyle.FixedSingle;
			}

			DrawBordersInternal( g, rect, borderStyle );
		}

		/// <summary>
		/// Draws the borders of the buttonEdit with specified border style.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="borderStyle"></param>
		private void DrawBordersInternal( Graphics g, Rectangle rect, BorderStyle borderStyle )
		{
			if( this.BorderSides != Border3DSide.All )
			{
				if( this.BorderSides != Border3DSide.Middle )
				{
					cd.DrawBorder( g, rect, borderStyle, this.border3DStyle, ButtonBorderStyle.Solid, this.flatBorderColor, this.borderSides );
				}
			}
			else
			{
				cd.DrawBorder( g, rect, borderStyle, this.border3DStyle, ButtonBorderStyle.Solid, this.flatBorderColor );
			}
		}

		/// <summary>
		/// Draws the borders of the buttonEdit with specified border style and specified border 3D style.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="borderStyle"></param>
		private void DrawBordersInternal( Graphics g, Rectangle rect, BorderStyle borderStyle, Border3DStyle border3DStyle )
		{
			if( this.BorderSides != Border3DSide.All )
			{
				if( this.BorderSides != Border3DSide.Middle )
				{
					cd.DrawBorder( g, rect, borderStyle, border3DStyle, ButtonBorderStyle.Solid, this.flatBorderColor, this.borderSides );
				}
			}
			else
			{
				cd.DrawBorder( g, rect, borderStyle, border3DStyle, ButtonBorderStyle.Solid, this.flatBorderColor );
			}
		}

		/// <summary>
		/// Draws the borders of the buttonEdit with specified border style, specified border 3D style and specified color.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="borderStyle"></param>
		private void DrawBordersInternal( Graphics g, Rectangle rect, BorderStyle borderStyle, Border3DStyle border3DStyle, Color borderColor )
		{
			if( this.BorderSides != Border3DSide.All )
			{
				if( this.BorderSides != Border3DSide.Middle )
				{
					cd.DrawBorder( g, rect, borderStyle, border3DStyle, ButtonBorderStyle.Solid, borderColor, this.borderSides );
				}
			}
			else
			{
				cd.DrawBorder( g, rect, borderStyle, border3DStyle, ButtonBorderStyle.Solid, borderColor );
			}
		}

		/// <summary>
		/// Draws the unthemed background of this control.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
		/// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
		/// <remarks><para>Called by <see cref="DrawBorderAndBackground(System.Drawing.Graphics, System.Drawing.Rectangle)"/> to draw the background when
		/// not in themes mode.</para></remarks>
		protected virtual void DrawBackground( Graphics g, Rectangle rect )
		{
			if( ( !this.Enabled ) && this.BackColor == ButtonEdit.DefaultButtonEditBackColor )
			{
				using (Brush br = new SolidBrush(SystemColors.Control))
				{
					g.FillRectangle(br, rect);
				}
			}
			else
			{
				using (Brush br = new SolidBrush(this.BackColor))
				{
					g.FillRectangle(br, rect);
				}
			}
		}

		/// <summary></summary>
		/// <param name="e"></param>
		/// <param name="displayRect"></param>
		/// <param name="windowRectInScreen"></param>
		/// <returns></returns>
		IntPtr INonClientPaintingSupport.NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen )
		{
			Rectangle bounds = displayRect;

			// This is not good for the following reasons:
			// 1) When dragging a hidden tree into the visible desktop range, clipping
			// is not proper and as a result the BG is drawn over the whole control and stays there!
			// 2) Even in other scenarios, we can see the BG color being drawn first
			// followed by the tree. Causing a flicker effect.
			//
			// So, instead fill the bg only in the border area.
			// 
			// Without this some 3D styles (SunkenOuter) will leave a 1 pixel transparent area.
			// g.FillRectangle(new SolidBrush(this.BackColor),bounds);

			int w = 2; // 2 for either of the border style because we allocate 2 pixels for either border style

			// The borders as 4 rectangles
			Rectangle[ ] clipRects = new Rectangle[]
				{
					new Rectangle( bounds.Location, new Size( w, bounds.Height ) ),
					new Rectangle( bounds.Location, new Size( bounds.Width, w ) ),
					new Rectangle( bounds.Width - w, bounds.Y, w, bounds.Height ),
					new Rectangle( bounds.X, bounds.Height - w, bounds.Width, w )
				};

			// Fill the border-rectangles with the bg brush, since some of the 
			// 3d border types are only 1 pixel wide.
			for( int i = 0; i < 4; i++ )
			{
				using (Brush br = new SolidBrush(this.BackColor))
				{
					e.Graphics.FillRectangle(br, clipRects[i]);
				}
			}

				DrawBordersInternal(e.Graphics, bounds);

			// return a region excluding where you just drew.
			return NativeMethods.CreateRectRgn( windowRectInScreen.Left + 2, windowRectInScreen.Top + 2, windowRectInScreen.Right - 2, windowRectInScreen.Bottom - 2 );
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Helper function to get an image from within embedded resources.
		/// </summary>
		/// <param name="resourceName">The resource name to get from.</param>
		/// <returns>An image; null if the image is not available.</returns>
		/// <remarks>
		/// The <see cref="ButtonEditChildButton"/> class can take an image based on the
		/// <see cref="ButtonEditChildButton.ButtonType"/> property. This helper function
		/// loads the images based on the resource name.
		/// </remarks>
		public static Image GetImage( string resourceName )
		{
			const string prefix = "Syncfusion.Windows.Forms.Tools.Controls.ButtonEdit.resources.";
			string fullResourceName = prefix + resourceName;

			Type type = typeof( ButtonEditChildButton );
			Assembly assembly = type.Module.Assembly;
			Stream stream = assembly.GetManifestResourceStream( fullResourceName );

			if( stream != null )
			{
				Bitmap bmp = new Bitmap( stream );
				bmp.MakeTransparent( Color.White );
				return bmp;
			}

			return null;
		}

		/// <summary>
		/// This is the implementation of the <see cref="IButtonEditParent"/> interface
		/// for listening to changes to the <see cref="ButtonEditChildButton"/> child buttons.
		/// </summary>
		/// <param name="btn">The ButtonEditChildButton that has changed its size.</param>
		/// <param name="newSize">The new size of the button.</param>
		/// <remarks>
		/// This notification is sent by the <see cref="ButtonEditChildButton"/> when the 
		/// <see cref="ButtonEditChildButton.PreferredWidth"/> property value is changed.
		/// The <see cref="ButtonEdit"/> control implements this interface and receives
		/// the notification to change its layout in accordance with the new size of the
		/// ButtonEditChildButton.
		/// </remarks>
		public void ChildButtonSizeChanged( ButtonEditChildButton btn, Size newSize )
		{
			btn.Left -= btn.PreferredWidth - btn.Width;
			btn.Width = btn.PreferredWidth;

			Layout();
		}

		/// <summary>
		/// This is the implementation of the <see cref="IButtonEditParent"/> interface
		/// for listening to changes to the <see cref="ButtonEditChildButton"/> child buttons.
		/// </summary>
		/// <param name="btn">The ButtonEditChildButton that has changed its alignment.</param>
		/// <param name="newAlign">The new alignment of the button.</param>
		/// <remarks>
		/// This notification is sent by the <see cref="ButtonEditChildButton"/> when the 
		/// <see cref="ButtonEditChildButton.ButtonAlign"/> property value is changed.
		/// The <see cref="ButtonEdit"/> control implements this interface and receives
		/// the notification to change its layout in accordance with the new alignment of the
		/// ButtonEditChildButton. See the <see cref="ButtonAlignment"/> type for the values
		/// that the ButtonAlignment can support.
		/// </remarks>
		public void ChildButtonAlignmentChanged( ButtonEditChildButton btn, ButtonAlignment newAlign )
		{
			Layout();
		}
		/// <summary>
		/// Handles the Button Clicked event of the child buttons.
		/// </summary>
		/// <param name="sender">The child button.</param>
		/// <param name="valArgs">The event data.</param>
		/// <remarks>
		/// This handler raises the <see cref="ButtonClicked"/> event and passes the
		/// <see cref="ButtonEditChildButton"/> that was clicked as the sender of the
		/// event. 
		/// </remarks>
		public void HandleChildButtonClicked( Object sender, EventArgs valArgs )
		{
			// See the method HandleChildClicked for an explanation
			if( this.Parent != null && this.Parent is IContainerControl )
			{
				( (IContainerControl)this.Parent ).ActiveControl = this;
			}

			if( this.ParentContainer != null )
			{
				this.ParentContainer.ActiveControl = this;
			}

			this.RaiseButtonClicked( sender, valArgs );
		}

		/// <summary>
		/// Handles the Button text changed event of the child buttons.
		/// </summary>
		/// <param name="sender">The child button.</param>
		/// <param name="valArgs">The event data.</param>
		/// <remarks>
		/// </remarks>
		public void HandleChildButtonTextChanged( Object sender, EventArgs valArgs )
		{
			if( ( sender as ButtonEditChildButton ).ButtonType != ButtonTypes.Normal )
			{
				( sender as ButtonEditChildButton ).Text = "";
			}
			else
			{
				( sender as ButtonEditChildButton ).ChildButtonText = ( sender as ButtonEditChildButton ).Text;
			}
		}

		/// <summary>
		/// Returns the active edit control. This implements the <see cref="IEditControlsEmbed"/>
		/// interface.
		/// </summary>
		/// <returns>The edit control that has the current focus.</returns>
		/// <remarks>
		/// This interface is implemented so that the <see cref="AutoComplete"/> control
		/// can provide auto completion services for the embedded TextBox control.
		/// </remarks>
		public Control GetActiveEditControl( IEditControlsEmbedListener listener )
		{
			this.listener = listener;
			return (Control)this.TextBox;
		}
		/// <summary>
		/// Indicates that paint message should not be passed when child controls  are removed by designer.
		/// </summary>
		[Browsable( false ),
	   DocumentationExclude()]
		public void ChildControlsRemovedByDesigner( ArrayList childControls )
		{
			this.childControls = childControls;
			this.Invalidate( false );
		}
		/// <summary>
		/// Indicates whether a child button is visible or hidden within the <see cref="ButtonEdit"/> layout.
		/// </summary>
		/// <remarks>
		/// There might be instances when you need to hide a child button that is part of
		/// the <see cref="Buttons"/> collection. Calling this method with the button index
		/// and the right value for the visibility will set the appropriate visibility for the
		/// button and also adjust the layout so that the other child buttons and the TextBox
		/// are aligned properly.
		/// </remarks>
		public bool HideButton( int btnIndex, bool visible )
		{
			if( btnIndex < this.Buttons.Count )
			{
				this.Buttons[btnIndex].Visible = visible;
				Layout();
			}
			else
			{
				return false;
			}

			return true;
		}
		/// <summary>
		///Handles MouseEnter event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleChildButtonMouseEnter( Object sender, EventArgs e )
		{
			( sender as ButtonEditChildButton ).State = ButtonAdvState.MouseOver;
		}

		/// <summary>
		/// Handles MouseLeave event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleChildButtonMouseLeave( Object sender, EventArgs e )
		{
			( sender as ButtonEditChildButton ).State = ButtonAdvState.Default;
		}

		/// <summary>
		/// Handles the BackColorChanged event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleChildButtonBackColorChanged( Object sender, EventArgs e )
		{
			if( !( sender as ButtonEditChildButton ).ShouldSerializeBackColor() )
			{
				( sender as ButtonEditChildButton ).BackColor = SystemColors.ControlLight;
			}
		}

		/// <summary>
		///  Handles the MouseDown event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleChildButtonMouseDown( Object sender, MouseEventArgs e )
		{
			if( ( e.Button & MouseButtons.Left ) == MouseButtons.Left )
			{
				ButtonEditChildButton btn = sender as ButtonEditChildButton;
				btn.State = ButtonAdvState.Pressed;
			}
		}

		/// <summary>
		/// Handles the MouseUp event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void HandleChildButtonMouseUp( Object sender, MouseEventArgs e )
		{
			if( ( e.Button & MouseButtons.Left ) == MouseButtons.Left )
			{
				ButtonEditChildButton btn = sender as ButtonEditChildButton;
				btn.State = ButtonAdvState.MouseOver;
			}
		}

		/// <summary>
		/// Handles the MouseLeave event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleChildMouseLeave( Object sender, EventArgs e )
		{
			UpdateState();
		}

		/// <summary>
		/// Handles the MouseEnter event of the child buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleChildMouseEnter( Object sender, EventArgs e )
		{
			UpdateState();
		}

		/// <summary>
		/// Checks whether the given child is contained within the control.Returns false for ButtonEdit TextBox.
		/// </summary>
		public new bool Contains( Control ctl )
		{
			if( ctl == this.TextBox )
			{
				return false;
			}
			else
			{
				return base.Contains( ctl );
			}
		}
		/// <summary>
		/// Handles the LostFocus event event of the child buttons.
		/// </summary>
		public void HandleChildLostFocus( Object sender, EventArgs e )
		{
			UpdateState();
		}

		#endregion

		#region Class overrides
		/// <summary>
		/// Override this to handle the <see cref="ButtonClicked"/> event in a derived class.
		/// Call the base implementation first so that the delegates will be called.
		/// </summary>
		/// <param name="args">The ButtonClickEventArgs event data.</param>
		/// <remarks>
		/// This method raises the <see cref="ButtonClicked"/> event when any of the
		/// Child buttons are clicked.
		/// </remarks>
		protected virtual void OnButtonClicked( ButtonClickedEventArgs args )
		{
			if( ButtonClicked != null )
			{
				ButtonClicked( this, args );
			}
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			base.OnRightToLeftChanged( e );

			PerformLayout();
		}
		/// <summary>
		/// Raises the BorderSidesChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorderSidesChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBorderSidesChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorderSidesChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorderSidesChanged( EventArgs e )
		{
			if( BorderSidesChanged != null )
			{
				BorderSidesChanged( this, e );
			}
		}

		/// <summary>
		/// Raises the Border3DStyleChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>Raising an event invokes the event handler 
		/// through a delegate. For more information, see Raising 
		/// an Event. <para>The OnBorder3DStyleChanged method also 
		/// allows derived classes to handle the event without 
		/// attaching a delegate. This is the preferred technique 
		/// for handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnBorder3DStyleChanged 
		/// in a derived class, be sure to call the base class's 
		/// OnBorder3DStyleChanged method so that registered 
		/// delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBorder3DStyleChanged( EventArgs e )
		{
			if( Border3DStyleChanged != null )
			{
				Border3DStyleChanged( this, e );
			}
		}

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override void WndProc( ref Message m )
		{
			if( this.UseVisualStyle == false )
			{
				if( this.cachedRgn != IntPtr.Zero )
				{
					NativeMethods.DeleteObject( this.cachedRgn );
					this.cachedRgn = IntPtr.Zero;
				}

				if( m.Msg == NativeMethods.WM_NCPAINT )
				{
					if( this.FlatStyle != FlatStyle.System )
					{
						this.cachedRgn = DrawingUtils.NCPaintHelper( this, this, ref m );
					}
				}
			}

			base.WndProc( ref m );
		}

		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Notes to Inheritors:  When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged( EventArgs e )
		{
			if( this.ThemeChanged != null )
			{
				try
				{
					this.ThemeChanged( this, e );
				}
				catch
				{
				}
			}
		}

		/// <override/>
		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			// Height cannot be 0. We need to check this rather than in OnPaint, 
			// since OnPaint will not be called if set to 0!
			if( height <= 0 )
			{
				height = this.Height;
			}

			if( this.Dock == DockStyle.Fill ||
                 this.Dock == DockStyle.Left ||
                 this.Dock == DockStyle.Right )
			{
				if( TextBox != null )
				{
					if( !this.TextBox.Multiline )
					{
						height = this.TextBox.Height + 8;
					}
					else
					{
						preventHeightChange = false;
					}
				}
				else
				{
					height = 21;
				}
			}

			if( this.preventHeightChange )
			{
				specified &= ~BoundsSpecified.Height;

				Rectangle curBounds = this.Bounds;

				if( curBounds.X != x || curBounds.Y != y || curBounds.Width != width )
				{
					base.SetBoundsCore( x, y, width, height, specified );
				}
			}
			else
			{
				base.SetBoundsCore( x, y, width, height, specified );
			}
		}

		/// <override/>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			ComputeBorderFactor();
			Layout();
			base.OnLayout( levent );
		}

		/// <summary>
		/// Forces the laying out of combobox elements.
		/// </summary>
		/// <param name="g">The Graphics object using which to calculate element sizes and positions.</param>
		/// <remarks>
		/// Advanced method. You do not have to call this directly.
		/// </remarks>
		public new virtual void Layout()
		{
			int textAreaHeight = 0;
			this.DetermineHeightsBasedOnFont( null, ref textAreaHeight );
			this.UpdateTextBoxAndButtonBounds( textAreaHeight );
		}

		/// <summary>
		/// Updates the bounds of the drop-down button bounds.
		/// </summary>
		/// <remarks>
		/// Sets the bounds based on the <see cref="DropDownButtonWidth"/>.
		/// </remarks>
		protected virtual void UpdateTextBoxAndButtonBounds( int textAreaHeight )
		{
			this.SuspendLayout();

			bool bIsMirrored = GetIsMirrored();
			buttonTextBox.Visible = m_showTextBox;

			if( this.UseVisualStyle )
			{
				int leftButtonLeft = 0;
				int leftButtonTotalWidth = 0;

				int rightButtonTotalWidth = 0;

				int top = 0;

				ButtonEditChildButton lastButton = null;
				bool firstRightButton = true;

				int textBoxTop = 0;

				foreach( ButtonEditChildButton btn in this.Buttons )
				{
					if( btn.Visible == true )
					{
						if( ( btn.ButtonAlign == ButtonAlignment.Left ) != bIsMirrored )
						{
							btn.Bounds =
								new Rectangle( leftButtonLeft + leftButtonTotalWidth + c_nDOUBLE_BORDER, top + c_nDOUBLE_BORDER, btn.PreferredWidth, DropDownButtonHeight ); //this.Height - 2 * this.borderFactor );
							leftButtonTotalWidth += btn.PreferredWidth;
							lastButton = btn;
						}
						else
						{
							rightButtonTotalWidth += btn.PreferredWidth;
						}

						btn.SetIsLastLeftButton( false );
						btn.SetIsFirstRightButton( false );

						if( null != this.TextBox )
						{
							btn.ComboEditBackColor = this.TextBox.BackColor;
						}
					}
				}

				// Set IsLastLeftButton
				if( lastButton != null )
				{
					lastButton.SetIsLastLeftButton( true );
					lastButton = null;
				}

				if( null != this.TextBox )
				{
					textBoxWidth = this.Bounds.Width - 2 * this.borderFactor - 2 * this.spacingFactor - leftButtonTotalWidth - rightButtonTotalWidth;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					if( this.TextBox.MinimumSize.Width > textBoxWidth && textBoxWidth > 0 )
					{
						int dist = this.TextBox.MinimumSize.Width - textBoxWidth;
						Size size = new Size( this.Width + dist, this.Height );
						this.MaximumSize = size;
						this.Size = size;

						if( this.Width > this.MinimumSize.Width )
						{
							this.MinimumSize = this.Size;
						}
					}
#endif

					// Center the textBox based on its height.
					textBoxTop = ( this.editPortionHeight - textAreaHeight ) / 2;
					int textBoxLeft = leftButtonTotalWidth + this.borderFactor + this.spacingFactor;
					if( this.FlatStyle != FlatStyle.System )
					{
						textBoxLeft = leftButtonTotalWidth + c_nDOUBLE_BORDER;
					}
					this.TextBox.Bounds = new Rectangle( textBoxLeft, textBoxTop, textBoxWidth, textAreaHeight );
				}

				// Reset the rightButtonTotalWidth
				rightButtonTotalWidth = 0;

				foreach( ButtonEditChildButton btn in this.Buttons )
				{
					if( btn.Visible == true )
					{
						if( ( btn.ButtonAlign == ButtonAlignment.Right ) != bIsMirrored )
                        {
                            if (this.appearance == ButtonAppearance.Metro)
                                btn.Bounds =
                                    new Rectangle(leftButtonTotalWidth + rightButtonTotalWidth + textBoxWidth + c_nDOUBLE_BORDER + 1, top + c_nDOUBLE_BORDER, btn.PreferredWidth, DropDownButtonHeight);
                            else
							btn.Bounds =
								new Rectangle( leftButtonTotalWidth + rightButtonTotalWidth + textBoxWidth + c_nDOUBLE_BORDER, top + c_nDOUBLE_BORDER, btn.PreferredWidth, DropDownButtonHeight );
							rightButtonTotalWidth += btn.PreferredWidth;

							if( firstRightButton )
							{
								btn.SetIsFirstRightButton( true );
								firstRightButton = false;
							}
						}
					}
				}
			}
			else
			{
				int leftButtonLeft = 2; //this.Bounds.Width - ddWidth - 2
				int leftButtonTotalWidth = 0;

				int rightButtonLeft = 4; //this.Bounds.Width - ddWidth - 2
				int rightButtonTotalWidth = 0;


				int textBoxTop = 0;

				if( this.FlatStyle != FlatStyle.System )
				{
					leftButtonLeft -= 2;
				}
				else
				{
					leftButtonLeft -= 1;
				}

				foreach( ButtonEditChildButton btn in this.Buttons )
				{
					if( btn.Visible == true )
					{
						if( ( btn.ButtonAlign == ButtonAlignment.Left ) != bIsMirrored )
						{
							btn.Bounds = new Rectangle( leftButtonLeft + leftButtonTotalWidth + c_nDOUBLE_BORDER, c_nDOUBLE_BORDER, btn.PreferredWidth, DropDownButtonHeight );
							leftButtonTotalWidth += btn.PreferredWidth;
						}
						else
						{
							rightButtonTotalWidth += btn.PreferredWidth;
						}
					}
				}

				if( null != this.TextBox )
				{
					// Same logic used in UpdateDropDownButtonBounds
					textBoxWidth = this.Bounds.Width - 6 - leftButtonTotalWidth - rightButtonTotalWidth;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					if( this.TextBox.MinimumSize.Width > textBoxWidth && textBoxWidth > 0 )
					{
						int dist = this.TextBox.MinimumSize.Width - textBoxWidth;
						Size size = new Size( this.Width + dist, this.Height );
						this.MaximumSize = size;
						this.Size = size;

						if( this.Width > this.MinimumSize.Width )
						{
							this.MinimumSize = this.Size;
						}
					}
#endif
					// Center the textBox based on its height.
					textBoxTop = ( this.editPortionHeight - textAreaHeight ) / 2;
					int textBoxLeft = leftButtonTotalWidth + c_nTEXT_OFFSET;
					if( this.FlatStyle != FlatStyle.System )
					{
						textBoxLeft = leftButtonTotalWidth + c_nDOUBLE_BORDER + c_nADJUST_XPOS;
					}

					// Set the bounds in all modes   
					this.TextBox.Bounds = new Rectangle( textBoxLeft, textBoxTop, textBoxWidth, textAreaHeight );
				}

				// Reset the rightButtonTotalWidth
				rightButtonTotalWidth = 0;

				if( this.FlatStyle != FlatStyle.System )
				{
					rightButtonLeft -= 2;
				}
				else
				{
					rightButtonLeft -= 1;
				}

				foreach( ButtonEditChildButton btn in this.Buttons )
				{
					if( btn.Visible == true )
					{
						if( ( btn.ButtonAlign == ButtonAlignment.Right ) != bIsMirrored )
						{
							btn.Bounds = new Rectangle(
								rightButtonLeft + leftButtonTotalWidth + rightButtonTotalWidth + textBoxWidth + c_nDOUBLE_BORDER,
								c_nDOUBLE_BORDER, btn.PreferredWidth, DropDownButtonHeight );

							rightButtonTotalWidth += btn.PreferredWidth;
						}
					}
				}
			}

			this.ResumeLayout( false );

		}

		/// <override/>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( childControls != null && childControls.Count > 0 )
			{
				ResetChildControls();
			}
			else
			{
				this.DrawBorderAndBackground( e );
				//base.OnPaint(e);
			}
		}

		/// <summary>
		/// Called from the <b>Paint</b> event handler to draw the edit portion's border and background.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> from the Paint event.</param>
		/// <remarks>
		/// This method calls the <see cref="DrawBorderAndBackground(System.Drawing.Graphics, System.Drawing.Rectangle)"/> method with the appropriate
		/// dimenison to draw the border around the text portion.
		/// </remarks>
		protected virtual void DrawBorderAndBackground( PaintEventArgs e )
		{
			this.DrawBorderAndBackground( e.Graphics, new Rectangle( 0, 0, this.Width, this.Height ) );
		}

		/// <summary>
		/// Forces the laying out of combo elements within the next Paint Message handler.
		/// </summary>
		/// <param name="needLayout">True to force; false to prevent layout.</param>
		[Obsolete( "Use Layout method instead." )]
		protected internal virtual void SetNeedLayout( bool needLayout )
		{
		}

		/// <summary>
		/// Determines the heights of certain portions of this control.
		/// </summary>
		/// <param name="g">Not used, obsolete.</param>
		/// <param name="textAreaHeight">A reference variable through which to return the height for the text area.</param>
		/// <remarks>
		/// <para>
		/// Make sure to call the base class when you override this method.
		/// </para>
		/// <para>
		/// This method expects you to return a height for the text area through the reference variable,
		/// set the height of this control and the height of the buttons.
		/// </para>
		/// </remarks>
		protected virtual void DetermineHeightsBasedOnFont( Graphics g, ref int textAreaHeight )
		{
			this.SuspendLayout();

			if( null != this.TextBox )
			{
				// Use the TextBox to determine the preferred height.
				bool textBoxVisibility = TextBox.Visible;

				this.TextBox.Visible = true;
				this.TextBox.PerformLayout();
				Size textAreaSize = this.TextBox.Size;

				// Button Size.
				this.DropDownButtonHeight = textAreaSize.Height + c_nADJUST_HEIGHT;
				textAreaHeight = textAreaSize.Height;

				if( TextBox.BorderStyle != BorderStyle.None )
				{
					this.DropDownButtonHeight -= c_nADJUST_HEIGHT / 2;
				}

				// Determine Control height
				this.editPortionHeight = this.DropDownButtonHeight + c_nADJUST_HEIGHT;

				this.preventHeightChange = false;
				this.Height = this.editPortionHeight;
				this.preventHeightChange = true;

				this.TextBox.Visible = textBoxVisibility;
			}
			else
			{
				textAreaHeight = 13;
			}

			this.ResumeLayout( false );
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnEnter( EventArgs e )
		{
			base.OnEnter( e );
			this.Layout();

			//if(this.textBox.Visible)
			//	this.textBox.Focus();
			this.ActiveControl = this.TextBox;

			OnEditGotFocus( this, e );
		}

		// The next 2 methods are not necessary in Everette.
		/// <override/>
		protected override void OnGotFocus( EventArgs e )
		{
			if( !m_bHandleFocusChanged )
			{
				return;
			}

			base.OnGotFocus( e );
			Layout();
		}

		/// <override/>
		protected override void OnLostFocus( EventArgs e )
		{
			if( !m_bHandleFocusChanged )
			{
				return;
			}

			base.OnLostFocus( e );
			Layout();
		}

		/// <summary>
		/// Raises the OnMouseLeave event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnMouseLeave( EventArgs e )
		{
			UpdateState();
			base.OnMouseLeave( e );
		}
		/// <summary>
		/// Raises the OnMouseEnter event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnMouseEnter( EventArgs e )
		{
			UpdateState();
			base.OnMouseEnter( e );
		}
		/// <summary>
		/// Raises the OnMouseDown event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );
			UpdateState();
		}
		/// <summary>
		/// Refreshes ButtonEdit ChildButtons.
		/// </summary>
		///<override/>
		public override void Refresh()
		{
			foreach( ButtonEditChildButton btn in this.Buttons )
			{
				btn.Refresh();
			}

			base.Refresh();
		}

		#endregion

		#region Class utility methods
		/// <summary>
		/// Event handler that sets the Parent's ActiveControl when one
		/// of the child controls are clicked.
		/// </summary>
		/// <remarks>There is a problem with using the ButtonEdit control
		/// inside a UserControl that doesn't update the UserControl's Active
		/// Control property properly when the user clicks inside the ButtonEdit.
		/// This event handler sets the ActiveControl explicitly.</remarks>
		/// <param name="sender">The child TextBox.</param>
		/// <param name="e">The event data.</param>
		protected void HandleChildClicked( object sender, EventArgs e )
		{
			Control child = (Control)sender;

			if( this == child.Parent )
			{
				if( this.Parent != null && this.Parent is IContainerControl )
				{
					( (IContainerControl)this.Parent ).ActiveControl = this;
				}

				if( this.ParentContainer != null )
				{
					this.ParentContainer.ActiveControl = this;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the control is mirrored. 
		/// </summary>
		/// <returns></returns>
		protected bool GetIsMirrored()
		{
			return ( RightToLeft.Yes == base.RightToLeft );
		}

		#endregion

		#region Class helpful methods
		protected internal virtual void AttachTextBox()
		{
			InitTextBox();
			if( this.TextBox.IsHandleCreated )
			{
				// subscribe for got/lost focus events
				m_nativeWindow = new EditNativeWindow( this );
				m_nativeWindow.LostFocus += new EventHandler( OnEditLostFocus );
			}
			else
			{
				this.TextBox.HandleCreated += new EventHandler( this.TextBox_HandleCreated );
			}
		}

		protected internal virtual void DetachTextBox()
		{
			// unsubscribe for got/lost focus events
			if( m_nativeWindow != null )
			{
				m_nativeWindow.LostFocus -= new EventHandler( OnEditLostFocus );
				m_nativeWindow.StopListening();
			}

			if( this.buttonTextBox != null )
			{
				this.Controls.Remove( buttonTextBox );

				this.buttonTextBox.TabIndex = 0;
				this.isTextBoxVisible = this.buttonTextBox.Visible;

				buttonTextBox.Click -= new EventHandler( this.HandleChildClicked );

				buttonTextBox.MouseEnter -= new EventHandler( HandleChildMouseEnter );
				buttonTextBox.MouseLeave -= new EventHandler( HandleChildMouseLeave );

				buttonTextBox.LostFocus -= new EventHandler( HandleChildLostFocus );
				buttonTextBox.GotFocus -= new EventHandler( HandleChildGotFocus );

				buttonTextBox.TextChanged -= new EventHandler( this.HandleEditTextChanged );

				buttonTextBox.HandleCreated -= new EventHandler( this.TextBox_HandleCreated );
			}
		}

		/// <summary>
		/// Private helper function that applies the style to all the child buttons.
		/// </summary>
		private void ApplyStyle()
		{
			foreach( ButtonEditChildButton btn in this.Buttons )
			{
                if (useVisualStyle && !(btn.FlatStyle != System.Windows.Forms.FlatStyle.System))
				    btn.FlatStyle = this.FlatStyle;
                if (this.ButtonStyle == ButtonAppearance.Metro)
                {
                    ComputeBorderFactor();
                    Layout();
                    buttonTextBox.BorderStyle = BorderStyle.None;
                    btn.BorderStyleAdv = ButtonAdvBorderStyle.Flat;
                    btn.BackColor = m_metroColor;
                    btn.SetAppearance(this.ButtonStyle, m_metroColor);
                }
                else
                    btn.SetAppearance(this.ButtonStyle);

				btn.Invalidate();
			}
			this.Invalidate();
		}

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.Control.DockChanged"/> event of the embedded TextBox control.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The DockChanged event is handled. DockStyle value must be None.
		/// </remarks>
		private void HandleEditDockChanged( object sender, EventArgs e )
		{
			if( this.TextBox.Dock != DockStyle.None )
			{
				throw new ArgumentException( "DockStyle value must be None" );
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.TextBoxExt.MaximumSizeChanged"/> event of the embedded 
		/// TextBoxExt control.
		/// </summary>
		/// <param name="sender">The TextBoxExt control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The MaximumSizeChanged event is handled.
		/// </remarks>
		private void HandleEditMaximumSizeChanged( object sender, EventArgs e )
		{
			if( this.TextBox.MaximumSize.Width != 0 )
			{
				this.Layout();

				int dist = this.textBoxWidth - this.TextBox.MaximumSize.Width;
				Size size = new Size( this.Width - dist, this.MaximumSize.Height );
				this.MaximumSize = size;
			}


			if( this.TextBox.Multiline == true )
			{
				if( this.TextBox.MaximumSize.Height != 0 )
				{
					Size size = new Size( this.MaximumSize.Width, this.TextBox.MaximumSize.Height + 8 );
					this.MaximumSize = size;
				}
			}
			else
			{
				Size size = new Size( this.TextBox.MaximumSize.Width, this.TextBox.Height );
				this.TextBox.MaximumSize = size;
			}
		}

		/// <summary>
		/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.TextBoxExt.MinimumSizeChanged"/> event of the embedded 
		/// TextBoxExt control.
		/// </summary>
		/// <param name="sender">The TextBoxExt control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The MinimumSizeChanged event is handled.
		/// </remarks>
		private void HandleEditMinimumSizeChanged( object sender, EventArgs e )
		{
			if( this.TextBox.MinimumSize.Width != 0 )
			{
				this.Layout();

				int dist = this.TextBox.MinimumSize.Width - this.textBoxWidth;
				Size size = new Size( this.Width + dist, this.MaximumSize.Height );
				this.MinimumSize = size;
			}


			if( this.TextBox.Multiline == true )
			{
				if( this.TextBox.MinimumSize.Height != 0 )
				{
					Size size = new Size( this.MinimumSize.Width, this.TextBox.MinimumSize.Height + 8 );
					this.MinimumSize = size;
				}
			}
			else
			{
				Size size = new Size( this.TextBox.MinimumSize.Width, this.TextBox.Height );
				this.TextBox.MinimumSize = size;
			}
		}


		/// <summary>
		/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.ButtonEdit.MaximumSizeChanged"/> event of the 
		/// ButtonEdit control.
		/// </summary>
		/// <param name="sender">The ButtonEdit control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The MaximumSizeChanged event is handled.
		/// </remarks>
		private void HandleButtonEditMaximumSizeChanged( object sender, EventArgs e )
		{
			int btnEditMaxWidth = this.MaximumSize.Width;
			if( btnEditMaxWidth != 0 )
			{
				this.Layout();

				int buttonsWidth = 0;
				foreach( ButtonEditChildButton button in this.Buttons )
				{
					buttonsWidth += button.Width;
				}

				int txtMaxWidth = this.TextBox.MaximumSize.Width;
				if( btnEditMaxWidth - buttonsWidth - 6 > txtMaxWidth )
				{
					Size size = new Size( btnEditMaxWidth - buttonsWidth - 6, this.TextBox.MaximumSize.Height );
					this.TextBox.MaximumSize = size;
				}
			}

			if( this.TextBox.Multiline == true )
			{
				if( this.MaximumSize.Height != 0 )
				{
					Size size = new Size( this.TextBox.MaximumSize.Width, this.MaximumSize.Height - 8 );
					this.TextBox.MaximumSize = size;
				}
			}
		}


		/// <summary>
		/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.ButtonEdit.MinimumSizeChanged"/> event of the 
		/// ButtonEdit control.
		/// </summary>
		/// <param name="sender">The ButtonEdit control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The MinimumSizeChanged event is handled.
		/// </remarks>
		private void HandleButtonEditMinimumSizeChanged( object sender, EventArgs e )
		{
			int btnEditMinWidth = this.MinimumSize.Width;
			if( btnEditMinWidth != 0 )
			{
				this.Layout();

				int buttonsWidth = 0;
				foreach( ButtonEditChildButton button in this.Buttons )
				{
					buttonsWidth += button.Width;
				}

				int txtMinWidth = this.TextBox.MinimumSize.Width;
				if( btnEditMinWidth + buttonsWidth - 6 < txtMinWidth )
				{
					Size size = new Size( btnEditMinWidth + buttonsWidth - 6, this.TextBox.MaximumSize.Height );
					this.TextBox.MinimumSize = size;
				}
			}

			if( this.TextBox.Multiline == true )
			{
				if( this.MinimumSize.Height != 0 )
				{
					Size size = new Size( this.TextBox.MinimumSize.Width, this.MinimumSize.Height - 8 );
					this.TextBox.MinimumSize = size;
				}
			}
		}
#endif

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.Control.SizeChanged"/> event of the embedded TextBox control.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The SizeChanged event is handled. 
		/// </remarks>
		private void HandleEditSizeChanged( object sender, EventArgs e )
		{
			if( TextBox != null )
			{
				if( TextBox.Width != textBoxWidth )
				{
					TextBox.Width = textBoxWidth;
				}

				if( this.Dock == DockStyle.Fill ||
                     this.Dock == DockStyle.Left ||
                     this.Dock == DockStyle.Right )
				{
					TextBox.Height = this.Height - 8;
				}
				else
				{
					textBoxHeight = TextBox.Height;
				}
			}
		}

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.TextBoxBase.MultilineChanged"/> event of the embedded TextBox control.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The MultilineChanged event is handled. 
		/// </remarks>
		private void HandleEditMultilineChanged( object sender, EventArgs e )
		{
			if( this.TextBox.Multiline )
			{
				if( this.Parent != null )
				{
					this.preventHeightChange = false;
					this.Parent.PerformLayout( this, "Bounds" );
					this.preventHeightChange = true;
				}

				this.TextBox.Height = this.Height - 8;

				//base.SetStyle(ControlStyles.FixedHeight, false);
			}
			//else
			//{
			//    base.SetStyle(ControlStyles.FixedHeight, true);                
			//}

			//this.RecreateHandle();

		}

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.Control.BackColorChanged"/> event of the embedded TextBox control.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The BackColorChanged event is handled. 
		/// </remarks>
		private void HandleEditBackColorChanged( object sender, EventArgs e )
		{
			if( this.TextBox.BackColor == SystemColors.Window )
			{
				this.TextBox.BackColor = this.BackColor;
			}
		}

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.Control.HandleCreated"/> event of the embedded TextBox control.
		/// </summary>
		private void TextBox_HandleCreated( object sender, EventArgs e )
		{
			m_nativeWindow = new EditNativeWindow( this );
			m_nativeWindow.LostFocus += new EventHandler( OnEditLostFocus );
		}

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.Control.DockChanged"/> event of the ButtonEdit control.
		/// </summary>
		/// <param name="sender">The ButtonEdit control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The DockChanged event is handled.
		/// </remarks>
		private void HandleButtonEditDockChanged( object sender, EventArgs e )
		{
			if( this.TextBox.Multiline )
			{
				if( this.Dock == DockStyle.None )
				{
					this.TextBox.Height = this.textBoxHeight;
					this.Height = this.textBoxHeight + 8;
				}
				else
					if( this.Dock == DockStyle.Fill || 
                     this.Dock == DockStyle.Left ||
                     this.Dock == DockStyle.Right )
					{
						this.TextBox.Height = this.Height - 8;
					}
					else
					{
						this.TextBox.Height = this.textBoxHeight;
					}
			}
		}

		/// <summary>
		/// Raises the <see cref="ButtonClicked"/> event.
		/// </summary>
		/// <param name="sender">The child button.</param>
		/// <param name="valArgs">The event data.</param>
		/// <remarks>
		/// Calls the <see cref="OnButtonClicked"/> method.
		/// </remarks>
		private void RaiseButtonClicked( Object sender, EventArgs valArgs )
		{
			ButtonClickedEventArgs args = new ButtonClickedEventArgs( (ButtonEditChildButton)sender );
			this.OnButtonClicked( args );
		}

		private void InvalidateWindow()
		{
			int redrawFlags = NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_ALLCHILDREN;
			NativeMethodsHelper.RedrawWindow( this.Handle, redrawFlags );
		}

		/// <summary>
		/// 
		/// </summary>
		[DocumentationExclude()]
		Control IPopupItem.GetPopupParentControl()
		{
			return (Control)this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="testControl"></param>
		/// <param name="askParent"></param>
		/// <returns></returns>
		[DocumentationExclude()]
		bool IPopupItem.IsRelatedControl( Control testControl, bool askParent )
		{
			if( testControl == this.TextBox || testControl.Parent == this )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="child"></param>
		/// <param name="closeType"></param>
		[DocumentationExclude()]
		void IPopupParent.ChildClosing( IPopupChild child, PopupCloseType closeType )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		[DocumentationExclude()]
		Point[] IPopupParent.GetBorderOverlapCue( PopupRelativeAlignment alignment )
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		[DocumentationExclude()]
		Point IPopupParent.GetLocationForPopupAlignment( PopupRelativeAlignment alignment, out PopupRelativeAlignment newAlignment )
		{
			newAlignment = PopupRelativeAlignment.Default;
			return Point.Empty;
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditGotFocus( object sender, EventArgs e )
		{
			m_bHandleFocusChanged = true;
			OnGotFocus( e );
			m_bHandleFocusChanged = false;
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditLostFocus( object sender, EventArgs e )
		{
			m_bHandleFocusChanged = true;
			OnLostFocus( e );
			m_bHandleFocusChanged = false;
		}

		/// <summary>
		/// Inserts the <see cref="ButtonEditChildButton"/> into the controls collection.
		/// </summary>
		/// <param name="index">The index at which to insert.</param>
		/// <param name="button">The child button control that is to be added.</param>
		/// <remarks>
		/// This method is invoked by the <see cref="ButtonEdit.ButtonEditChildButtonCollection"/> class
		/// when new buttons are added to the ButtonEdit control.
		/// </remarks>
		private void Insert( int index, ButtonEditChildButton button )
		{
			if( this.Controls.Contains( button ) == false )
			{
				int buttonHeight = this.dropDownButtonHeight;

				this.Controls.Add( button );

				button.ButtonEditParent = this;
				button.Parent = this;
				button.Size = new Size( button.Width, buttonHeight );
				button.FlatStyle = this.FlatStyle;
				button.BackColor = SystemColors.ControlLight;
				if (this.appearance != ButtonAppearance.Metro)
                    button.SetAppearance(ButtonStyle);
                else
                {
                    button.BackColor = m_metroColor;
                    button.SetAppearance(ButtonStyle, m_metroColor);
                }
				// mouse state not required for child buttons
				ButtonEditChildButton buttonIndx = this.Buttons[index];

				buttonIndx.SuspendMouseState();
				buttonIndx.Click += new EventHandler( HandleChildButtonClicked );
				buttonIndx.TextChanged += new EventHandler( HandleChildButtonTextChanged );
				buttonIndx.MouseDown += new MouseEventHandler( HandleChildButtonMouseDown );
				buttonIndx.MouseUp += new MouseEventHandler( HandleChildButtonMouseUp );
				buttonIndx.MouseEnter += new EventHandler( HandleChildButtonMouseEnter );
				buttonIndx.MouseLeave += new EventHandler( HandleChildButtonMouseLeave );
				buttonIndx.MouseHover += new EventHandler( buttonIndx_MouseHover );
				buttonIndx.BackColorChanged += new EventHandler( HandleChildButtonBackColorChanged );
				buttonIndx.ChildButtonText = buttonIndx.Name;

				this.ControlRemoved += new ControlEventHandler( HandleChildButtonControlRemoved );

				button.BringToFront();
			}

			Layout();
		}
		/// <summary>
		/// Sets the Buttonstate
		/// </summary>
		/// <param name="sender">
		/// Button Control
		/// </param>
		/// <param name="e">
		/// EventArgs
		/// </param>
		public void buttonIndx_MouseHover( object sender, EventArgs e )
		{
			ButtonAdvState st = ( sender as ButtonAdv ).State;
		}

		/// <summary>
		/// Removes a ButtonEditChildButton from the controls collection.
		/// </summary>
		/// <param name="index">The index of the child control to be removed</param>
		/// <param name="button">The child control that is to be removed</param>
		/// <remarks>
		/// This method is invoked by the <see cref="ButtonEditChildButtonCollection"/> class
		/// when buttons are removed from the ButtonEdit control.
		/// </remarks>
		private void Remove( int index, ButtonEditChildButton button )
		{
			if( this.Controls.Contains( button ) )
			{
				this.Controls.Remove( button );
			}

			if( index >= 0 && index < this.Buttons.Count )
			{
				this.Buttons[index].Click -= new EventHandler( HandleChildButtonClicked );
			}

			Layout();
			this.Invalidate();
			return;
		}

		/// <summary>
		/// Handles the <see cref="System.Windows.Forms.Control.TextChanged"/> event of the embedded TextBox control.
		/// </summary>
		/// <param name="sender">The TextBox control that sends the event.</param>
		/// <param name="e">The event data.</param>
		/// <remarks>
		/// The TextChanged event is handled and the ButtonEdit control sets its own
		/// text to be the same as that of the embedded TextBox control's Text property.
		/// </remarks>
		private void HandleEditTextChanged( object sender, EventArgs e )
		{
			this.OnTextChanged( e );
		}

		/// <summary></summary>
		private void ResetChildControls()
		{
			if( childControls != null )
			{
				if( childControls.Count > 0 )
				{
					foreach( Control childControl in childControls )
					{
						this.Controls.Add( childControl );
					}
				}
				childControls.Clear();
			}
			Layout();
		}

		/// <summary>
		/// Sets the base style to use.
		/// </summary>
		private void SetBaseStyle()
		{
			int wndStyle = NativeMethods.GetWindowLong( this.Handle, NativeMethods.GWL_STYLE );
			int wndExStyle = NativeMethods.GetWindowLong( this.Handle, NativeMethods.GWL_EXSTYLE );

			wndStyle &= ~NativeMethods.WS_BORDER;
			wndExStyle &= ~NativeMethods.WS_EX_CLIENTEDGE;

			if( this.UseVisualStyle == false )
			{
				NativeMethods.SetWindowLong( this.Handle, NativeMethods.GWL_EXSTYLE, wndExStyle );
			}

			NativeMethods.SetWindowPos( Handle, IntPtr.Zero, 0, 0, 0, 0,
				NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE |
				NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER |
				NativeMethods.SWP_NOOWNERZORDER | NativeMethods.SWP_FRAMECHANGED );
		}

		/// <summary>
		/// Computes the border factor based on the button style.
		/// </summary>
		private void ComputeBorderFactor()
		{
			if( this.ButtonStyle == ButtonAppearance.Classic )
			{
				this.borderFactor = 2;
				this.spacingFactor = 1;
			}
			else if( this.ButtonStyle == ButtonAppearance.WindowsXP || this.ButtonStyle==ButtonAppearance.Metro)
			{
				this.borderFactor = 1;
				this.spacingFactor = 1;
			}
			else
			{
				this.borderFactor = 0;
				this.spacingFactor = 2;
			}
		}

		/// <summary>
		///This is called by the event system when a ControlRemoved event occurs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleChildButtonControlRemoved( object sender, ControlEventArgs e )
		{
			if( e.Control is ButtonEditChildButton && !DesignMode)
			{
				this.buttonsCollection.Remove( e.Control as ButtonEditChildButton );
			}
		}

		/// <summary>
		/// This is called by the event system when a GotFocus event occurs.
		/// </summary>
		private void HandleChildGotFocus( Object sender, EventArgs e )
		{
			this.UpdateState();
		}

		/// <summary></summary>
		private void UpdateState()
		{
			if( this.IsDisposed == false )
			{
				this.State = ButtonAdvState.Default;
				foreach( ButtonEditChildButton btn in this.Buttons )
				{
					btn.State = this.State;
					btn.Invalidate();
				}
				this.Invalidate();
			}
		}

		/// <summary>
		/// Sets the Color scheme for the button based on the current XP Scheme and
		/// the IsComboButton property.
		/// </summary>
		private void SetColorScheme()
		{
			if( XPThemes.IsSilverThemeOn )
			{
				this.colorScheme = WindowsXPColorScheme.SilverCombo;
			}
			else if( XPThemes.IsOliveGreenThemeOn )
			{
				this.colorScheme = WindowsXPColorScheme.OliveGreenCombo;
			}
			else
			{
				this.colorScheme = WindowsXPColorScheme.DefaultBlueCombo;
			}
		}

		internal void SetTextBoxNull()
		{
			buttonTextBox = null;
		}

		#endregion

		#region Class internal declarations
		/// <summary>
		/// Collection of <see cref="ButtonEditChildButton"/> objects.
		/// </summary>
		/// <remarks>
		/// The collection of <see cref="ButtonEditChildButton"/> controls that make up the
		/// <see cref="ButtonEdit.Buttons"/> property of the ButtonEdit class.
		/// <para>
		/// You will not need to use this class directly.
		/// </para>
		/// </remarks>
		public class ButtonEditChildButtonCollection: CollectionBase
		{
			/// <summary>
			/// The owner ButtonEdit object.
			/// </summary>
			private ButtonEdit owner;

			/// <summary>
			/// Creates an object of type <see cref="ButtonEditChildButtonCollection"/>.
			/// </summary>
			/// <param name="owner">The ButtonEdit object that owns this collection.</param>
			/// <remarks>
			/// The ButtonEdit class that owns this collection is passed in as a parameter
			/// and this collection class will use this reference to the owner to inform
			/// it to add a Button control or remove a Button control when an item is
			/// added or removed to the collection.
			/// </remarks>
			public ButtonEditChildButtonCollection( ButtonEdit owner )
			{
				this.owner = owner;
			}

			/// <summary>
			/// Gets / sets the Indexer property for <see cref="ButtonEditChildButtonCollection"/>
			/// </summary>
			/// <remarks>
			/// This allows the ButtonEditChildButtons to be accessed through the indexer.
			/// </remarks>
			[
			Browsable( true )
			]
			public ButtonEditChildButton this[int index]
			{
				get
				{
					return (ButtonEditChildButton)( this.List[index] );
				}

				set
				{
					this.List[index] = (Object)value;
				}
			}

			/// <summary>
			/// Adds a <see cref="ButtonEditChildButton"/> to the collection.
			/// </summary>
			/// <param name="button">The object to be added.</param>
			/// <returns>The index of the object in the collection.</returns>
			/// <remarks>
			/// Add an item to the internal List object results in the <see cref="OnInsertComplete"/>
			/// method being invoked.
			/// </remarks>
			public int Add( ButtonEditChildButton button )
			{
				int retval = -1;
				if( this.List.Contains( button ) == false )
				{
					retval = this.List.Add( button );
				}
				return retval;
			}

			/// <summary>
			/// Derived handler for the InsertComplete event.
			/// </summary>
			/// <param name="index">The index of the inserted item.</param>
			/// <param name="value">The object that was inserted.</param>
			/// <remarks>
			/// Be sure to call the base class implementation of this method
			/// if overriding.
			/// <para>
			/// The owner (the ButtonEdit) control is instructed to insert a new
			/// ButtonEditChildButton.
			/// </para>
			/// </remarks>
			protected override void OnInsertComplete( int index, object value )
			{
				base.OnInsertComplete( index, value );

				this.owner.Insert( index, this.owner.Buttons[index] );
				if( this.owner.DesignMode )
				{
					this.owner.Layout();
				}
                (value as ButtonAdv).BackColor = this.owner.MetroColor;
			}

			/// <summary>
			/// Removes a<see cref="ButtonEditChildButton"/> object from the collection.
			/// </summary>
			/// <param name="button">The ButtonEditChildButton that is to be removed from the collection.</param>
			/// <remarks>
			/// Removes the ButtonEditChildButton from the internal List object if the ButtonEditChildButton
			/// exists.
			/// </remarks>
			public void Remove( ButtonEditChildButton button )
			{
				if( this.owner.Controls.Contains( button ) )
				{
					this.owner.Remove( this.List.IndexOf( button ), button );
				}

				if( this.List.Contains( button ) )
				{
					this.List.Remove( button );
				}
			}

			/// <summary>
			/// Indicates whether an object exists in this collection.
			/// </summary>
			/// <param name="button">The object to check for.</param>
			/// <returns>True if the object exists in this collection; false otherwise.</returns>
			/// <remarks>
			/// This method is used for checking if an item exists in the collection before trying
			/// to delete or change that item.
			/// </remarks>
			public bool Contains( ButtonEditChildButton button )
			{
				return this.List.Contains( button );
			}

			/// <summary>
			/// Copies elements of this collection to another collection starting
			/// at an index.
			/// </summary>
			/// <param name="array">The array to be copied to.</param>
			/// <param name="index">The index to begin from.</param>
			/// <remarks>
			/// The internal List copies the child buttons to the new array passed in.
			/// </remarks>
			public void CopyTo( ButtonEditChildButton[] array, int index )
			{
				this.List.CopyTo( array, index );
			}

			/// <summary>
			/// Derived handler for the RemoveComplete event.
			/// </summary>
			/// <param name="index">The index of the inserted item.</param>
			/// <param name="value">The object that was inserted.</param>
			/// <remarks>
			/// The owner is informed by the collection to re-layout itself to accommodate the
			/// change.
			/// </remarks>
			protected override void OnRemoveComplete( int index, object value )
			{
				base.OnRemoveComplete( index, value );
				this.owner.Layout();
			}
		}
		#endregion

        #region For Touch

        bool isScaling = false;

        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        ///Gets or Sets the Touchmode
        /// </summary>
		[DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                    {
                        ApplyScaleToControl(1.5f);
                    }
                    else
                    {
                        ApplyScaleToControl(1.0f);
                    }
                }
            }
        }
        /// <summary></summary>
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applie the scaling for controls
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            foreach (Control ctrl in this.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");
                fi.SetValue(ctrl, this.EnableTouchMode, null);
            }
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (!EnableTouchMode && this.DesignMode)
                CTRLSIZE = this.Size;
            base.OnSizeChanged(e);
        }
        #endregion
	}

	/// <summary></summary>
	public class ButtonEditSerializer:
		CodeDomSerializer
	{
		#region Class overrides

		/// <summary></summary>
		/// <param name="manager"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object Serialize( IDesignerSerializationManager manager, object value )
		{
			if( ( manager == null ) || ( value == null ) )
			{
				throw new ArgumentNullException( ( manager == null ) ? "manager" : "value" );
			}
			CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer( typeof( Component ), typeof( CodeDomSerializer ) );
			if( serializer == null )
			{
				return null;
			}
			object obj2 = serializer.Serialize( manager, value );

			return obj2;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object Deserialize( IDesignerSerializationManager manager, object codeObject )
		{
			if( ( manager == null ) || ( codeObject == null ) )
			{
				throw new ArgumentNullException( ( manager == null ) ? "manager" : "codeObject" );
			}
			IContainer service = (IContainer)manager.GetService( typeof( IContainer ) );

			CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer( typeof( Component ), typeof( CodeDomSerializer ) );
			if( serializer == null )
			{
				return null;
			}

			object obj2 = serializer.Deserialize( manager, codeObject );

			return obj2;
		}

		#endregion
	}

	/// <summary>
	/// Used for processing focus changed event for ButtonEdit control.
	/// </summary>
	public class EditNativeWindow: NativeWindow
	{
		#region Class members
		/// <summary></summary>
		private bool m_bIsListening = false;
		/// <summary>
		/// ButtonEdit control that this native window is bound to.
		/// </summary>
		private ButtonEdit m_btnEdit;
		#endregion

		#region Class events
		/// <summary>
		///Specifies actions to occur when an object receives the focus
		/// </summary>
		public event EventHandler GotFocus;
		/// <summary>
		/// Occurs when the control loses focus
		/// </summary>
		public event EventHandler LostFocus;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="btnEdit"> ButtonEdit control to handle events for. </param>
		public EditNativeWindow( ButtonEdit btnEdit )
		{
			if( btnEdit == null )
			{
				throw new ArgumentNullException( "btnEdit" );
			}

			m_btnEdit = btnEdit;
			m_bIsListening = true;
			this.AssignHandle( m_btnEdit.TextBox.Handle );
		}
		#endregion

		#region Class utility methods
		/// <summary>
		/// 
		/// </summary>
		public void StopListening()
		{
			m_bIsListening = false;
		}

		/// <summary>
		/// Indicates whether to fire focus changed events or it is received from some child controls of ButtonEdit.
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		private bool IsFocusChanged( IntPtr handle )
		{
			if( m_btnEdit == null )
			{
				return false;
			}

			if( handle == m_btnEdit.Handle || handle == IntPtr.Zero )
			{
				return false;
			}

			for( int i = 0, len = m_btnEdit.Controls.Count; i < len; i++ )
			{
				if( m_btnEdit.Controls[i].Handle == handle && m_btnEdit.Controls[i] != m_btnEdit.TextBox )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Invokes the default window procedure associated with this window.
		/// </summary>
		/// <param name="m">A <see cref="System.Windows.Forms.Message"></see> that is associated with the current Windows message.</param>
		protected override void WndProc( ref Message m )
		{
			if( m_bIsListening )
			{
				// Process GotFocus event.
				if( m.Msg == NativeMethods.WM_SETFOCUS )
				{
					if( GotFocus != null && IsFocusChanged( m.WParam ) )
					{
						GotFocus( this, EventArgs.Empty );
					}
				}
				// Process LostFocus event.
				else if( m.Msg == NativeMethods.WM_KILLFOCUS )
				{
					if( LostFocus != null && IsFocusChanged( m.WParam ) )
					{
						LostFocus( this, EventArgs.Empty );
					}
				}
			}

			base.WndProc( ref m );
		}
		#endregion
	}

}