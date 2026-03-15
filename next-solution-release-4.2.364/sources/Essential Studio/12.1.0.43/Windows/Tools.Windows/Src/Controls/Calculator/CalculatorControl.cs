#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Calculator control encapsulates the functionality of a calculator with
    /// the ability to perform arithmetic calculations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The calculator control can be used in two different layouts. See
    /// <see cref="CalculatorLayoutTypes"/> for more information.
    /// </para>
    /// <para>
    /// The default property of the Calculator Control class is the
    /// <see cref="CalculatorControl.Value"/> property. This property is of
    /// type <see cref="CalculatorValue"/>.
    /// </para>
    /// <para>
    /// The Calculator Control uses a <see cref="CalculatorEngine"/> object to perform the
    /// calculations and maintain the state of the calculations. The CalculatorControl implements
    /// the <see cref="ICalculatorEngineParent"/> interface to receive notifications
    /// from the Calculator Engine.
    /// </para>
    /// <para>
    /// The calculator buttons are of type <see cref="CalculatorButton"/> and the buttons
    /// maintain their own information about the action that is to be performed when clicked.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code lang="C#">
    /// //InitializeComponent
    /// // Create the Calculator Control
    /// this.calculatorControl1 = new CalculatorControl();
    /// this.textBox1 = new TextBox();
    /// // Set the value of the calculator control
    /// this.calculatorControl1.DoubleValue = 0;
    /// // Set the border style for the control
    ///  this.calculatorControl1.BorderStyle = Border3DStyle.Raised;
    /// // The flat style for the buttons
    /// this.calculatorControl1.FlatStyle = FlatStyle.Standard;
    /// // Set the size of the calculator
    /// this.calculatorControl1.Size = new System.Drawing.Size(288, 232);
    /// // Add a event handler for the ValueCalculated event of the child button
    /// this.calculatorControl1.ValueCalculated += new Syncfusion.Windows.Forms.Tools.CalculatorControl.ValueCalculatedEventHandler(this.CodeGen_calculatorControl1_ValueCalculated);
    /// // Add the CalculatorControl control to the form
    /// this.Controls.Add(this.calculatorControl1);</code>
    /// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\calculatordemo\VB\MainForm.vb" name="CalcuatorControl InitializeComponent" lang="VB"><code lang="VB">
    /// 'InitializeComponent
    /// ' Create the Calculator Control
    /// Me.calculatorControl1 = New CalculatorControl()
    /// Me.textBox1 = New TextBox()
    /// ' Set the value of the calculator control
    /// Me.calculatorControl1.DoubleValue = 0
    /// ' Set the border style for the control
    /// Me.calculatorControl1.BorderStyle = Border3DStyle.Raised
    /// ' The flat style for the buttons
    /// Me.calculatorControl1.FlatStyle = FlatStyle.Standard
    /// ' Set the size of the calculator
    /// Me.calculatorControl1.Size = New System.Drawing.Size(288, 232)
    /// ' Add a event handler for the ValueCalculated event of the child button
    /// AddHandler Me.calculatorControl1.ValueCalculated, New Syncfusion.Windows.Forms.Tools.CalculatorControl.ValueCalculatedEventHandler(AddressOf CodeGen_calculatorControl1_ValueCalculated)
    /// ' Add the CalculatorControl control to the form
    /// Me.Controls.Add(Me.calculatorControl1)</code></coderef>
    /// </example>
    [
    Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.CalculatorDesigner), typeof(System.ComponentModel.Design.IDesigner)),
    DefaultProperty(@"DoubleValue"),
    ToolboxItem(true),
    ToolboxBitmap(typeof(CalculatorControl), "ToolboxIcons.CalculatorControl.bmp"),
    Description("Provides the functionality of a calculator with the ability to perform arithmetic calculations")
    ]
    public class CalculatorControl : System.Windows.Forms.ContainerControl, ICalculatorEngineParent, ICalculatorButtonParent,IVisualStyle 
    {
        #region Constants
        private static readonly Insets c_WindowsStandardInsets = new Insets(5, 5, 5, 5);
        private static readonly Insets c_FinancialInsets = new Insets(4, 4, 4, 4);
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        #endregion

        /// <summary>
        /// The total number of buttons.
        /// </summary>
       private static int totalButtonCount;

        /// <summary>
        /// The border 3D style.
        /// </summary>
        private Border3DStyle border3dStyleValue;

        /// <summary>
        /// For tooltip.
        /// </summary>
        private ToolTip toolTipHelp;

        /// <summary>
        /// The last action received.
        /// </summary>
        private CalcActions lastAction;

        /// <summary>
        /// The display box for the CalculatorControl.
        /// </summary>
        /// <remarks>
        /// This TextBox will display the output from the calculations.
        /// </remarks>
        protected TextBox textCalculatorBox;

        /// <summary>
        /// The array of buttons for the CalculatorControl.
        /// </summary>
        /// <remarks>
        /// Each of these buttons will have a different function.
        /// </remarks>
        private CalculatorButton[] calcButtons;

        /// <summary>
        /// The label to display the memory state.
        /// </summary>
        private CustomLabel memoryStateDisplay;

        /// <summary>
        /// The layout type.
        /// </summary>
        private CalculatorLayoutTypes calcLayoutType;

        /// <summary>
        /// Indicates whether the display text box should be displayed.
        /// </summary>
        private bool showDisplayArea;

        /// <summary>
        /// Hashtable to mnemonic keys.
        /// </summary>
        private Hashtable mnemonicKeys;

        /// <summary>
        /// The calculator 'engine'.
        /// </summary>
        private ICalculatorEngine calcEngineObject;

        /// <summary>
        /// The GridBagLayout object for the Windows Standard Layout.
        /// </summary>
        private GridBagLayout calcWinStandardLayout;

        /// <summary>
        /// Indicates whether the Calculator buttons are ThemesEnabled.
        /// </summary>
        private bool themesEnabled = true;

        /// <summary>
        /// The GridBagLayout for Financial Layout.
        /// </summary>
        private GridBagLayout calcFinancialLayout;

        /// <summary>
        /// Value when the control is resized based on the layout.
        /// </summary>
        protected bool autoSizeValue;

        /// <summary>
        /// The currently selected culture.
        /// </summary>
        private CultureInfo selectedCulture = new CultureInfo(CultureInfo.CurrentCulture.LCID);

        /// <summary>
        /// Specifies the special culture value that needs to be applied.
        /// </summary>
        private SpecialCultureValues specialCultureValue = SpecialCultureValues.None;

        /// <summary>
        /// Array of cultures that require RightToLeft by default.
        /// </summary>
        private ArrayList rightToLeftCultures = new ArrayList();

        /// <summary>
        /// The UseUserOverride value to be passed in when creating CultureInfo objects.
        /// </summary>
        private bool useUserOverride = true;

        /// <summary>
        /// The NumberFormatInfo object that will specify the
        /// localized attributes for displaying the current
        /// value
        /// </summary>
        private NumberFormatInfo numberFormatInfoObject = null;

        /// <summary>
        /// Indicates whether the previous action is to be repeated when the Assignment (=) action is clicked.
        /// </summary>
        private bool repeatAssignAction = true;

        /// <summary>
        /// Colorschemes for Office2007 visual style.
        /// </summary>
        protected Office2007Theme m_office2007Theme = Office2007Theme.Blue;

        /// <summary>
        /// Colorschemes for Office2010 visual style.
        /// </summary>
        protected Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        /// <summary>
        /// Occurs when the <see cref="CalculatorControl.Office2007Theme"/> property is changed.
        /// </summary>
        [Description("Occurs when the Office2007Theme property is changed.")]
        public event EventHandler Office2007ThemeChanged;
        /// <summary>
        /// Occurs when the <see cref="CalculatorControl.Office2010Theme"/> property is changed.
        /// </summary>
        [Description("Occurs when the Office2010Theme property is changed.")]
        public event EventHandler Office2010ThemeChanged;
        /// <summary>
        /// Raised when the <see cref="Value"/> of the calculator control changes.
        /// </summary>
        /// <remarks>
        /// Handle this event if you want to do some processing when the
        /// Value changes.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// this.calculatorControl1.ValueCalculated += new Syncfusion.Windows.Forms.Tools.CalculatorControl.ValueCalculatedEventHandler(this.CodeGen_calculatorControl1_ValueCalculated);
        /// calculatorControl1_ValueCalculated
        /// if(arg.ErrorCondition == false)
        /// this.textBox1.Text = arg.Value.ToString();
        /// else
        /// this.textBox1.Text = arg.Message;</code>
        /// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\calculatordemo\VB\MainForm.vb" name="CalcuatorControl ValueCalculated event" lang="VB"><code lang="VB">
        /// AddHandler Me.calculatorControl1.ValueCalculated, New Syncfusion.Windows.Forms.Tools.CalculatorControl.ValueCalculatedEventHandler(AddressOf CodeGen_calculatorControl1_ValueCalculated)
        /// 'calculatorControl1_ValueCalculated
        /// If (arg.ErrorCondition = False) Then
        /// Me.textBox1.Text = arg.Value.ToString
        /// Else
        /// Me.textBox1.Text = arg.Message
        /// End If</code></coderef>
        /// </example>
        [Description("Raised when the Value property changes.")]
        public event CalculatorValueCalculatedEventHandler ValueCalculated;

        /// <summary>
        /// Raised when the <see cref="LayoutType"/> changes.
        /// </summary>
        /// <remarks>
        /// Other classes that need to be aware of the current layout of the
        /// Calculator Control or just need to know that the layout has changed
        /// so that they can reinitialize themselves, can handle this event.
        /// </remarks>
        [Description("Raised when LayoutType property changes.")]
        public event CalculatorLayoutTypeChangedEventHandler LayoutTypeChanged;

        /// <summary>
        /// Raised when the FlatStyle changes for the Calculator Control.
        /// </summary>
        /// <remarks>
        /// The Calculator Control maintains the same flatstyle for all its child
        /// buttons. This event is handled by all the buttons and they set their
        /// flatstyle property appropriately.
        /// </remarks>
        [Description("Raised when the FlatStyle property changes.")]
        public event CalculatorStyleChangedEventHandler FlatStyleChanged;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// The flat style for the calculator buttons.
        /// </summary>
        private FlatStyle flatStyle;

        /// <summary>
        /// The NumberFormatInfo that defines the formatting.
        /// </summary>
        private ButtonAppearance appearance = ButtonAppearance.Classic;
        private bool useVisualStyle = false;
        private BrushInfo m_bgBrush = BrushInfo.Empty;

        /// <summary>
        /// Vertical spacing between buttons.
        /// </summary>
        private int m_iVerticalSpacing = 10;

        /// <summary>
        /// Horizontal spacing between buttons.
        /// </summary>
        private int m_iHorizontalSpacing = 10;

        /// <summary>
        /// Use vertical and horizontal spacing between buttons.
        /// </summary>
        private bool m_bUseVerticalAndHorizontalSpacing = false;

        /// <summary>
        /// Initializes static members of the CalculatorControl class.
        /// </summary>
        static CalculatorControl()
        {
            CalculatorControl.totalButtonCount = 27;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorControl"/> class.
        /// <para>
        /// The constructor will initialize the <see cref="CalculatorEngine"/>
        /// and the display textbox with the initial values.</para>
        /// </summary>
        public CalculatorControl()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(CalculatorControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.rightToLeftCultures.Add("ar");
            this.rightToLeftCultures.Add("fa");
            this.rightToLeftCultures.Add("he");
            this.rightToLeftCultures.Add("ur");
            this.rightToLeftCultures.Add("syr");
            this.rightToLeftCultures.Add("div");

            // Calculation engine initialization
            this.InitializeCalculatorEngine();

            // Display related initialization
            this.InitializeCalculatorDisplay();
            this.lastAction = CalcActions.CalcOperatorNone;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            CTRLSIZE = this.Size;

            // optimize painting( reduce flicker )
            base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        #region CULTURE

        /// <summary>
        /// Gets or sets the culture that is to be used for formatting the currency display.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Editor(typeof(Syncfusion.Windows.Forms.Tools.Design.CalculatorCultureEditor), typeof(System.Drawing.Design.UITypeEditor)),
        Description("Gets or sets the culture that is to be used for formatting the currency display.")
        ]
        public CultureInfo Culture
        {
            get
            {
                return this.selectedCulture;
            }

            set
            {
                if (this.SpecialCultureValue == SpecialCultureValues.None)
                    this.selectedCulture = value;
                else if (this.SpecialCultureValue == SpecialCultureValues.CurrentCulture)
                    this.selectedCulture = CultureInfo.CurrentCulture;
                else if (this.SpecialCultureValue == SpecialCultureValues.UICulture)
                    this.selectedCulture = CultureInfo.CurrentUICulture;
                else if (this.SpecialCultureValue == SpecialCultureValues.InstalledCulture)
                    this.selectedCulture = CultureInfo.InstalledUICulture;

                // if(this.Initializing == false)
                // {
                // Change the NumberFormat also
                this.NumberFormatInfoObject = this.Culture.NumberFormat;
                this.ApplyRightToLeft();

                // }
                this.RaiseLayoutTypeChangedEvent();
                this.CalculatorEngine.NumberFormatInfoObject = this.NumberFormatInfoObject;
                this.SetDisplayString();
            }
        }

        /// <summary>
        /// Returns a copy of the current NumberFormatInfo.
        /// </summary>
        /// <returns>Returns Numberformat info</returns>
        protected NumberFormatInfo GetCopyOfCurrentNumberFormatInfo()
        {
            NumberFormatInfo info = new NumberFormatInfo();
            if (this.NumberFormatInfoObject != null)
            {
                info.CurrencyDecimalDigits = this.NumberFormatInfoObject.CurrencyDecimalDigits;

                info.CurrencyDecimalSeparator = this.NumberFormatInfoObject.CurrencyDecimalSeparator;
                info.NumberDecimalSeparator = this.NumberFormatInfoObject.NumberDecimalSeparator;
                info.PercentDecimalSeparator = this.NumberFormatInfoObject.PercentDecimalSeparator;

                info.CurrencyGroupSeparator = this.NumberFormatInfoObject.CurrencyGroupSeparator;
                info.NumberGroupSeparator = this.NumberFormatInfoObject.NumberGroupSeparator;
                info.PercentGroupSeparator = this.NumberFormatInfoObject.PercentGroupSeparator;

                info.CurrencyGroupSizes = this.NumberFormatInfoObject.CurrencyGroupSizes;
                info.CurrencyNegativePattern = this.NumberFormatInfoObject.CurrencyNegativePattern;
                info.CurrencyPositivePattern = this.NumberFormatInfoObject.CurrencyPositivePattern;
                info.CurrencySymbol = this.NumberFormatInfoObject.CurrencySymbol;
            }

            return info;
        }

        /// <summary>
        /// Gets or sets the NumberFormatInfo object that will be used for formatting the
        /// number value.
        /// </summary>
        /// <remarks>
        /// This property will not be exposed to the developer. This is only meant
        /// to be an acccessor for use within the control. The developer will be able
        /// to access the properties of the NumberFormatInfo through the individual
        /// properties exposed.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        Category("Appearance"),
        Description("The NumberFormaInfo object that will be used for formatting the number value."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public NumberFormatInfo NumberFormatInfoObject
        {
            get
            {
                if (this.numberFormatInfoObject == null)
                {
                    this.numberFormatInfoObject = new NumberFormatInfo();

                    this.numberFormatInfoObject.CurrencyDecimalDigits = this.Culture.NumberFormat.CurrencyDecimalDigits;

                    this.numberFormatInfoObject.CurrencyDecimalSeparator = this.Culture.NumberFormat.CurrencyDecimalSeparator;
                    this.numberFormatInfoObject.NumberDecimalSeparator = this.Culture.NumberFormat.CurrencyDecimalSeparator;
                    this.numberFormatInfoObject.PercentDecimalSeparator = this.Culture.NumberFormat.CurrencyDecimalSeparator;

                    this.numberFormatInfoObject.CurrencyGroupSeparator = this.Culture.NumberFormat.CurrencyGroupSeparator;
                    this.numberFormatInfoObject.NumberGroupSeparator = this.Culture.NumberFormat.CurrencyGroupSeparator;
                    this.numberFormatInfoObject.PercentGroupSeparator = this.Culture.NumberFormat.CurrencyGroupSeparator;

                    this.numberFormatInfoObject.CurrencyGroupSizes = this.Culture.NumberFormat.CurrencyGroupSizes;
                    this.numberFormatInfoObject.CurrencyNegativePattern = this.Culture.NumberFormat.CurrencyNegativePattern;
                    this.numberFormatInfoObject.CurrencyPositivePattern = this.Culture.NumberFormat.CurrencyPositivePattern;
                    this.numberFormatInfoObject.CurrencySymbol = this.Culture.NumberFormat.CurrencySymbol;

                    this.numberFormatInfoObject.NumberNegativePattern = this.Culture.NumberFormat.NumberNegativePattern;
                }
                return this.numberFormatInfoObject;
            }
            set
            {
                NumberFormatInfo prevFormat = GetCopyOfCurrentNumberFormatInfo();

                if (this.numberFormatInfoObject == null)
                    this.numberFormatInfoObject = new NumberFormatInfo();

                NumberFormatInfo localObject = new NumberFormatInfo();

                if (value != null)
                    localObject = value;
                else
                    localObject = this.Culture.NumberFormat;

                this.numberFormatInfoObject.CurrencyDecimalDigits = localObject.CurrencyDecimalDigits;

                this.numberFormatInfoObject.CurrencyDecimalSeparator = localObject.CurrencyDecimalSeparator;
                this.numberFormatInfoObject.NumberDecimalSeparator = localObject.CurrencyDecimalSeparator;
                this.numberFormatInfoObject.PercentDecimalSeparator = localObject.CurrencyDecimalSeparator;

                this.numberFormatInfoObject.CurrencyGroupSeparator = localObject.CurrencyGroupSeparator;
                this.numberFormatInfoObject.NumberGroupSeparator = localObject.CurrencyGroupSeparator;
                this.numberFormatInfoObject.PercentGroupSeparator = localObject.CurrencyGroupSeparator;

                this.numberFormatInfoObject.CurrencyGroupSizes = localObject.CurrencyGroupSizes;
                this.numberFormatInfoObject.CurrencyNegativePattern = localObject.CurrencyNegativePattern;
                this.numberFormatInfoObject.CurrencyPositivePattern = localObject.CurrencyPositivePattern;
                this.numberFormatInfoObject.CurrencySymbol = localObject.CurrencySymbol;

                this.FormatChanged(prevFormat);
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void FormatChanged(NumberFormatInfo previousFormat)
        {
        }

        /// <summary>
        /// Gets or sets the mode for the cultures.
        /// </summary>
        [
        Browsable(false),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public SpecialCultureValues SpecialCultureValue
        {
            get
            {
                return this.specialCultureValue;
            }

            set
            {
                this.specialCultureValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the NumberFormatInfo used for formatting will use the UseUserOverride parameter for CultureInfo.
        /// </summary>
        /// <remarks>
        /// The NumberTextBoxBase control has several properties that expose Culture specific
        /// information. These properties use a <see cref="NumberFormatInfo"/> object for
        /// handling the culture specific information. This property is used in the creation
        /// of the NumberFormatInfo object. <seealso cref="CultureInfo.UseUserOverride"/>
        /// </remarks>
        [
        Browsable(true),
        Category("Culture"),
        Description("Specifies if the NumberFormatInfo used for formatting will use the User Overrides for the culture."),
        DefaultValue(true)
        ]
        public bool UseUserOverride
        {
            get
            {
                return this.useUserOverride;
            }

            set
            {
                this.useUserOverride = value;
            }
        }

        /// <summary>
        /// Applies RightToLeft based on the current culture.
        /// </summary>
        protected void ApplyRightToLeft()
        {
            if (this.rightToLeftCultures.Contains(this.Culture.Parent.Name))
                this.RightToLeft = RightToLeft.Yes;
            else
                this.RightToLeft = RightToLeft.No;
        }

        #endregion

        /// <summary>
        /// Initalizes the CalculatorEngine that will perform the calculations.
        /// </summary>
        /// <remarks>
        /// The CalculatorEngine is initialized and the CalculatorControl sets the
        /// interface for communicating with the CalculatorEngine.
        /// </remarks>
        protected virtual void InitializeCalculatorEngine()
        {
            this.calcEngineObject = (ICalculatorEngine)new Engine(this);
        }

        /// <summary>
        /// Initializes the calculator display.
        /// </summary>
        /// <remarks>
        /// The display TextBox and the memory value display label are created and initialized.
        /// <para>
        /// The calculator buttons are created and added to the CalculatorControl.
        /// </para>
        /// </remarks>
        protected virtual void InitializeCalculatorDisplay()
        {
            this.mnemonicKeys = new Hashtable();
            this.showDisplayArea = true;
            this.toolTipHelp = new ToolTip();
            this.flatStyle = FlatStyle.Flat;
            this.border3dStyleValue = Border3DStyle.Adjust;

            this.calcButtons = new CalculatorButton[CalculatorControl.totalButtonCount];

            // We create the first button so that it receives focus
            CalculatorButton focusBtn = new CalculatorButton(this);
            focusBtn.TabStop = true;
            focusBtn.Name = "CalcFocus";
            focusBtn.Size = new Size(0, 0);
            this.Controls.Add(focusBtn);

            for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
            {
                CalculatorButton btnCreator = new CalculatorButton(this);
                this.calcButtons[i] = btnCreator;
                this.calcButtons[i].Parent = this;
            }

            // Create the text box for the display
            this.CreateCalculatorDisplayBox();
            if (this.textCalculatorBox != null)
            {
                textCalculatorBox.TextAlign = HorizontalAlignment.Right;
                this.SetTextBoxColor();
                this.textCalculatorBox.TabStop = false;
            }

            // Create the memory state display 
            this.memoryStateDisplay = new CustomLabel();
            if (this.ButtonStyle != ButtonAppearance.Metro)
            this.memoryStateDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.memoryStateDisplay.BackColor = Color.Transparent;

            this.InitializeLayoutAndAddControls();
            this.SizeChanged += new System.EventHandler(this.HandleBoundsChanged);
            this.SetDisplayString();
        }

        /*/// <summary>
        /// The NumberFormatInfo that defines the formatting.
        /// </summary>
        public NumberFormatInfo NumberFormat
        {
            get
            {
                return this.numberFormat;
            }

            set
            {
                this.numberFormat = value;
            }
        }*/

        /// <summary>
        /// Modifies the properties of the TextBox used for displaying the calculated value.
        /// </summary>
        /// <remarks>
        /// The TextBox is  enabled and the forecolor and backcolor
        /// properties are changed.
        /// </remarks>
        protected virtual void SetTextBoxColor()
        {
            textCalculatorBox.Enabled = true;

            // The color of the TextBox is set to black by 
            // handling the OnPaint handler
            this.ForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Overrides the <see cref="System.Windows.Forms.Control.OnPaint"/> method.
        /// </summary>
        /// <param name="pe">The Paint event data.</param>
        /// <remarks>
        /// This override is for drawing a border around the Calculator Control
        /// by invoking the <see cref="DrawBorder"/> method.
        /// </remarks>
        protected override void OnPaint(PaintEventArgs pe)
        {
            this.DrawBorder(pe.Graphics);

            BrushInfo brush = this.BackgroundColor;

            if (this.UseVisualStyle && this.ButtonStyle == ButtonAppearance.Office2007 && this.Office2007Theme == Office2007Theme.Managed)
            {
                Color backColor = new Color();

                brush = GetOffice2007BackColor(this.Office2007Theme, ref backColor);
            }
            else if (this.UseVisualStyle && this.ButtonStyle == ButtonAppearance.Office2010 && this.Office2010Theme == Office2010Theme.Managed)
            {
                Color backColor = new Color();

                brush = GetOffice2010BackColor(this.Office2010Theme, ref backColor);
            }

            if (!brush.Equals(BrushInfo.Empty))
            {
                BrushPaint.FillRectangle(pe.Graphics, this.ClientRectangle, brush);
            }

            base.OnPaint(pe);
        }

        /// <summary>
        /// Draws a border around the Calculator Control.
        /// </summary>
        /// <param name="g">the Graphics object to draw on.</param>
        /// <remarks>
        /// The type of the border drawn is based on the <see cref="Border3DStyle"/>
        /// property.
        /// </remarks>
        protected virtual void DrawBorder(Graphics g)
        {
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            ControlPaint.DrawBorder3D(g, rect, this.BorderStyle, Border3DSide.Bottom | Border3DSide.Top | Border3DSide.Right | Border3DSide.Left);
        }

        /// <summary>
        /// Overrides the <see cref="System.Windows.Forms.Control.OnBackColorChanged"/> method.
        /// </summary>
        /// <param name="arg">The event data.</param>
        /// <remarks>
        /// This method is overriden in order to set the color of the 
        /// child buttons to be the same as the backcolor of the Calculator Control.
        /// </remarks>
        protected override void OnBackColorChanged(EventArgs arg)
        {
            base.OnBackColorChanged(arg);
            if (this.ButtonStyle != ButtonAppearance.Metro)
            {
                for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
                {
                    this.calcButtons[i].BackColor = this.BackColor;
                }
            }
        }

        /// <summary>
        /// Processes the dialog key.
        /// </summary>
        /// <param name="keyData">One of the Keys values that represents the key to process.</param>
        /// <returns>Returns bool value</returns>
        /// <remarks >This is overriden in order to capture and process enterkey input </remarks>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                int enteredKey = (int)keyData;
                if (this.mnemonicKeys.ContainsKey(enteredKey) == true)
                {
                    int index = (int)this.mnemonicKeys[enteredKey];
                    this.SimulateButtonClick(index);
                    return true;
               }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Gets or sets the Border3DStyle for the CalculatorControl's border.
        /// </summary>
        /// <remarks>
        /// This value can be any of the values of the type <see cref="Border3DStyle"/>.
        /// Setting the value to <see cref="Border3DStyle.Adjust"/> displays no border.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies the Border3DStyle for the CalculatorControl's border."),
        DefaultValue(typeof(Border3DStyle), "Adjust")
        ]
        public Border3DStyle BorderStyle
        {
            get
            {
                return this.border3dStyleValue;
            }

            set
            {
                this.border3dStyleValue = value;
                this.Invalidate();
            }
        }
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
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
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
                    this.UseVerticalAndHorizontalSpacing = value;
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
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i].Name == "CalcFocus")
                    this.Controls[i].Size = new Size(0, 0);
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is to be resized based on the layout.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies if the control is to be resized based on the layout.")
        ]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public bool AutoSize
#else
        public new bool AutoSize
#endif
        {
            get
            {
                return this.autoSizeValue;
            }

            set
            {
                if (value != this.autoSizeValue)
                {
                    this.autoSizeValue = value;
                    if (value == true)
                        SetLayoutSize();
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical spacing between buttons.
        /// </summary>
        [
        Browsable(true),
        Category("Layout"),
        Description("Gets or sets vertical spacing between buttons."),
        DefaultValue(10)
        ]
        public int VerticalSpacing
        {
            get
            {
                return m_iVerticalSpacing;
            }
            set
            {
                if (value != m_iVerticalSpacing)
                {
                    m_iVerticalSpacing = value;

                    if (this.LayoutType == CalculatorLayoutTypes.WindowsStandard)
                    {
                        this.InitializeWindowsStandardLayout();
                    }
                    else
                    {
                        this.InitializeFinancialLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal spacing between buttons.
        /// </summary>
        [
        Browsable(true),
        Category("Layout"),
        Description("Gets or sets horizontal spacing between buttons."),
        DefaultValue(10)
        ]
        public int HorizontalSpacing
        {
            get
            {
                return m_iHorizontalSpacing;
            }
            set
            {
                if (value != m_iHorizontalSpacing)
                {
                    m_iHorizontalSpacing = value;

                    if (this.LayoutType == CalculatorLayoutTypes.WindowsStandard)
                    {
                        this.InitializeWindowsStandardLayout();
                    }
                    else
                    {
                        this.InitializeFinancialLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether use vertical and horizontal spacing between buttons.
        /// </summary>
        [
        Browsable(true),
        Category("Layout"),
        Description("Gets or sets use vertical and horizontal spacing between buttons."),
        DefaultValue(false)
        ]
        public bool UseVerticalAndHorizontalSpacing
        {
            get
            {
                return m_bUseVerticalAndHorizontalSpacing;
            }
            set
            {
                if (value != m_bUseVerticalAndHorizontalSpacing)
                {
                    m_bUseVerticalAndHorizontalSpacing = value;

                    if (this.LayoutType == CalculatorLayoutTypes.WindowsStandard)
                    {
                        this.InitializeWindowsStandardLayout();
                    }
                    else
                    {
                        this.InitializeFinancialLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Creates the display textbox.
        /// </summary>
        /// <remarks>
        /// This method creates the TextBox that will be used by the
        /// Calculator Control to display the value of the calculations.
        /// </remarks>
        protected virtual void CreateCalculatorDisplayBox()
        {
            this.textCalculatorBox = new CalculatorTextBox(this);
        }

        /// <summary> 
        /// Cleans up any resources being used. 
        /// </summary>
        /// <param name="disposing">Bool value for disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SizeChanged -= new System.EventHandler(this.HandleBoundsChanged);

                if (calcWinStandardLayout != null)
                {
                    calcWinStandardLayout.Dispose();
                    calcWinStandardLayout = null;
                }
                if (calcFinancialLayout != null)
                {
                    calcFinancialLayout.Dispose();
                    calcFinancialLayout = null;
                }
                if (this.textCalculatorBox != null)
                {
                    this.Controls.Remove(this.textCalculatorBox);
                    this.textCalculatorBox.Dispose();
                    this.textCalculatorBox = null;
                }
                if (this.toolTipHelp != null)
                {
                    this.toolTipHelp.Dispose();
                    this.toolTipHelp = null;
                }
                if (calcButtons != null)
                {
                    foreach (CalculatorButton cb in calcButtons)
                    {
                        this.Controls.Remove(cb);
                        cb.Dispose();
                    }
                    calcButtons = null;
                }

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles the KeyDown event of the child controls.
        /// </summary>
        /// <param name="e">The KeyEventArgs for the event data.</param>
        public void HandleChildKeyDown(KeyEventArgs e)
        {
            // KeyData is combination of KeyCode and Modifier
            // keys
            int enteredKey = (int)e.KeyData;
            if (this.mnemonicKeys.ContainsKey(enteredKey) == true)
            {
                int index = (int)this.mnemonicKeys[enteredKey];
                if (index >= 0)
                {
                    this.SimulateButtonClick(index);
                }
          }
        }

        private void SimulateButtonClick(int buttonIndex)
        {
            CalculatorButton button = this.calcButtons[buttonIndex];
            if (button != null)
            {
                button.Select();
                button.PerformClick();
            }
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        /// <summary>
        /// Gets or sets the flat style for the <see cref="CalculatorButton"/> objects.
        /// </summary>
        /// <remarks>
        /// This property raises the StyleChanged event so that the 
        /// buttons can set themselves to this new FlatStyle.
        /// </remarks>
        [
        Browsable(true),
        Description("The flat style for the CalculatorButton objects."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        DefaultValue(typeof(FlatStyle), "Flat")
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                return this.flatStyle;
            }

            set
            {
                if (value != this.flatStyle)
                {
                    this.flatStyle = value;
                    this.OnUseVisualStyleChanged();
                    this.RaiseFlatStyleChangedEvent();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether themes are enabled for the Calculator Control.
        /// </summary>
        /// <remarks>
        /// This property raises the StyleChanged event so that the 
        /// buttons can set themselves to this new FlatStyle.
        /// </remarks>
        [
        Browsable(true),
        Description("Indicates whether themes are enabled for the Calculator control."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        DefaultValue(true)
        ]
        public bool ThemesEnabled
        {
            get
            {
                return this.themesEnabled;
            }

            set
            {
                this.themesEnabled = value;
                this.RaiseThemesChangedEvent();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the assignment action (=) will repeat the previous action.
        /// </summary>
        /// <remarks>
        /// This property raises the StyleChanged event so that the 
        /// buttons can set themselves to this new FlatStyle.
        /// </remarks>
        [
        Browsable(true),
        Description("Specifies if the assignment action will repeat the previous action."),
        Category("Behavior"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        DefaultValue(true)
        ]
        public bool RepeatAssignAction
        {
            get
            {
                return this.repeatAssignAction;
            }

            set
            {
                this.repeatAssignAction = value;
            }
        }

        /// <summary>
        /// Gets a accessor for the Calculator Engine.
        /// </summary>
        private ICalculatorEngine CalculatorEngine
        {
            get
            {
                return this.calcEngineObject;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Value"/> of the Calculator Control as a double value.
        /// </summary>
        /// <remarks>
        /// This property does not maintain its own value and converts the Calculator Value object to double.
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Gets or sets the value of CalculatorControl as a double value.")
        ]
        public double DoubleValue
        {
            get
            {
                return this.Value.ToDouble();
            }
            set
            {
                this.Value.SetValue(value);
                this.SetDisplayString();
            }
        }

        /// <summary>
        /// Gets or sets the current value of the Calculator Control.
        /// </summary>
        /// <remarks>
        /// The Value property is a shadow of the Calculator Engine's Value property. 
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public CalculatorValue Value
        {
            get
            {
                if (this.CalculatorEngine != null)
                    return this.CalculatorEngine.GetValue();
                else
                    return CalculatorValue.Empty;
            }

            set
            {
                if (this.CalculatorEngine != null)
                    this.CalculatorEngine.SetValue(value);
                this.SetDisplayString();
            }
        }

        /// <summary>
        /// Gets or sets the text alignment. Based on the <see cref="HorizontalAlignment"/>,
        /// the Text in the display textbox will be aligned left or right based on
        /// this value.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DefaultValue(HorizontalAlignment.Right),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Gets or sets the text alignment.")
        ]
        public HorizontalAlignment DisplayTextAlign
        {
            get
            {
                return this.textCalculatorBox.TextAlign;
            }

            set
            {
                this.textCalculatorBox.TextAlign = value;
            }
        }

        /// <summary>
        /// Resets the calculator and initializes the internal calculator engine.
        /// </summary>
        public void ResetCalculator()
        {
            this.calcEngineObject.Reset();
            this.memoryStateDisplay.Text = String.Empty;
        }

        /// <summary>
        /// Gets or sets the current layout type for the Calculator Control. This is of type
        /// <see cref="CalculatorLayoutTypes"/>.
        /// </summary>
        [
        Browsable(true),
        DefaultValue(CalculatorLayoutTypes.WindowsStandard),
        Category("Appearance"),
        Description("Gets or sets the current layout type.")
        ]
        public CalculatorLayoutTypes LayoutType
        {
            get
            {
                return this.calcLayoutType;
            }

            set
            {
                if (this.calcLayoutType != value)
                {
                    this.calcLayoutType = value;
                    this.RaiseLayoutTypeChangedEvent();
                }
            }
        }

        /// <summary>
        /// Raises the LayoutTypeChangedEvent.
        /// </summary>
        protected void RaiseLayoutTypeChangedEvent()
        {
            this.SuspendLayout();
            if (this.AutoSize)
                SetLayoutSize();
            if (calcLayoutType == CalculatorLayoutTypes.Financial)
            {
                this.calcFinancialLayout.AutoLayout = true;
                this.calcWinStandardLayout.AutoLayout = false;
                this.InitializeFinancialLayout();
                this.calcFinancialLayout.LayoutContainer();
            }
            else
            {
                this.calcFinancialLayout.AutoLayout = false;
                this.calcWinStandardLayout.AutoLayout = true;
                this.InitializeWindowsStandardLayout();
                this.calcWinStandardLayout.LayoutContainer();
            }
            this.ResetCalculator();
            this.ResumeLayout();

            CalculatorLayoutTypeChangedEventArgs arg = new CalculatorLayoutTypeChangedEventArgs(this.LayoutType);
            this.OnLayoutTypeChange(arg);
        }

        private void SetLayoutSize()
        {
            if (this.LayoutType == CalculatorLayoutTypes.Financial)
                this.Size = new Size(200, 184);
            else
                this.Size = new Size(288, 232);
        }

        /// <summary>
        /// Invokes the LayoutTypeChange event.
        /// </summary>
        /// <param name="args">A LayoutTypeChangedEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnLayoutTypeChange method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnLayoutTypeChange in a derived
        /// class, be sure to call the base class's OnLayoutTypeChange method so that
        /// registered delegates receive the event.</note> 
        /// </remarks>
        protected virtual void OnLayoutTypeChange(CalculatorLayoutTypeChangedEventArgs args)
        {
            if (this.LayoutTypeChanged != null)
                this.LayoutTypeChanged(this, args);
        }

        /// <summary>
        /// Raises the FlatStyleChangedEvent.
        /// </summary>
        protected void RaiseFlatStyleChangedEvent()
        {
            this.SuspendLayout();

            for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
            {
                this.calcButtons[i].Appearance = this.ButtonStyle;
                this.calcButtons[i].ButtonStyle.FlatStyle = this.FlatStyle;
                this.calcButtons[i].UseVisualStyle = this.UseVisualStyle;
            }
            this.ResumeLayout();
            CalculatorStyleChangedEventArgs arg = new CalculatorStyleChangedEventArgs(this.FlatStyle);
        }

        /// <summary>
        /// Raises the FlatStyleChangedEvent.
        /// </summary>
        protected void RaiseThemesChangedEvent()
        {
            this.SuspendLayout();
            for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
            {
                this.calcButtons[i].ThemesEnabled = this.ThemesEnabled;
            }
            this.ResumeLayout();

            // CalculatorStyleChangedEventArgs arg = new CalculatorStyleChangedEventArgs(this.FlatStyle);
        }

        /// <summary>
        /// Raises the <see cref="CalculatorControl.Office2007Theme"/> event.
        /// </summary>
        protected virtual void OnOffice2007ThemeChanged()
        {
            RaiseOffice2007ThemeChanged();

            for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
            {
                this.calcButtons[i].Office2007ColorScheme = m_office2007Theme;
            }
            if (this.UseVisualStyle && this.ButtonStyle == ButtonAppearance.Office2007)
            {
                this.ChangeBackgroundColor(m_office2007Theme);
            }
        }
        /// <summary>
        /// Raises the <see cref="CalculatorControl.Office2007Theme"/> event.
        /// </summary>
        protected virtual void OnOffice2010ThemeChanged()
        {
            RaiseOffice2010ThemeChanged();

            for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
            {
                this.calcButtons[i].Office2010ColorScheme = m_office2010Theme;
            }
            if (this.UseVisualStyle && this.ButtonStyle == ButtonAppearance.Office2010)
            {
                this.ChangeBackgroundColor(m_office2010Theme);
            }
        }
        private void ChangeBackgroundColor(Office2010Theme theme)
        {
            Color backColor = new Color();

            this.BackgroundColor = GetOffice2010BackColor(theme, ref backColor);
            this.BackColor = backColor;
        }
        private void ChangeBackgroundColor(Office2007Theme theme)
        {
            Color backColor = new Color();

            this.BackgroundColor = GetOffice2007BackColor(theme, ref backColor);
            this.BackColor = backColor;
        }
        private BrushInfo GetOffice2010BackColor(Office2010Theme theme, ref Color backColor)
        {
            Office2010Colors colorTable = Office2010Colors.GetColorTable(theme);
            Color[] colors = new Color[2] { colorTable.ButtonDefaultTopColor, colorTable.ButtonDefaultBottomColor };

            backColor = colorTable.ButtonDefaultBottomColor;

            return new BrushInfo(GradientStyle.Vertical, colors);
        }
        private BrushInfo GetOffice2007BackColor(Office2007Theme theme, ref Color backColor)
        {
            Office2007Colors colorTable = Office2007Colors.GetColorTable(theme);
            Color[] colors = new Color[2] { colorTable.ButtonDefaultTopColor, colorTable.ButtonDefaultBottomColor };

            backColor = colorTable.ButtonDefaultBottomColor;

            return new BrushInfo(GradientStyle.Vertical, colors); 
        }

        /// <summary>
        /// Raises the <see cref="CalculatorControl.Office2007Theme"/> event.
        /// </summary>
        private void RaiseOffice2007ThemeChanged()
        {
            if (this.Office2007ThemeChanged != null)
            {
                this.Office2007ThemeChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises the <see cref="CalculatorControl.Office2007Theme"/> event.
        /// </summary>
        private void RaiseOffice2010ThemeChanged()
        {
            if (this.Office2010ThemeChanged != null)
            {
                this.Office2010ThemeChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Invokes the FlatStyleChange event.
        /// </summary>
        /// <param name="args">A StyleChangedEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnFlatStyleChange method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnFlatStyleChange in a derived
        /// class, be sure to call the base class's OnFlatStyleChange method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnFlatStyleChange(CalculatorStyleChangedEventArgs args)
        {
            if (this.FlatStyleChanged != null)
                this.FlatStyleChanged(this, args);
        }
        /// <summary>
        ///Font changed event
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (!EnableTouchMode && this.DesignMode)
                CTRLSIZE = this.Size;
            base.OnSizeChanged(e);
        }
        /// <summary>
        /// Gets or sets a value indicating whether the calculator control is to display the display textbox. 
        /// The textbox can be hidden and the application using the Calculator Control 
        /// can handle the <see cref="ValueCalculated"/> event to display the value
        /// in their own display area / textbox.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DefaultValue(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Indicates whether the calculator control is to display the display textbox.")
        ]
        public bool ShowDisplayArea
        {
            get
            {
                return this.showDisplayArea;
            }
            set
            {
                this.showDisplayArea = value;

                this.SuspendLayout();
                if (calcLayoutType == CalculatorLayoutTypes.Financial)
                    this.InitializeFinancialLayout();
                else
                    this.InitializeWindowsStandardLayout();
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Returns the current layout manager for the calculator.
        /// </summary>
        /// <returns>The current layout manager for the calculator.</returns>
        public GridBagLayout GetLayoutManager()
        {
            return this.GetLayoutManager(this.LayoutType);
        }

        /// <summary>
        /// Returns the layout manager for this layout type.
        /// </summary>
        /// <param name="layoutType">The layout type.</param>
        /// <returns>The layout manager.</returns>
        public GridBagLayout GetLayoutManager(CalculatorLayoutTypes layoutType)
        {
            if (layoutType == CalculatorLayoutTypes.Financial)
                return this.calcFinancialLayout;
            else
                return this.calcWinStandardLayout;
        }

        /// <summary>
        /// Handles the <see cref="LayoutItemBase.BoundsChanged"/> event. This initiates a 
        /// refresh of the layout.
        /// </summary>
        /// <param name="sender">The calculator control.</param>
        /// <param name="e">The event data.</param>
        protected void HandleBoundsChanged(object sender, EventArgs e)
        {
            GridBagLayout layoutManager = this.GetLayoutManager();

            layoutManager.AutoLayout = true;
            layoutManager.LayoutContainer();
        }

        /// <summary>
        /// Sets the characteristics for a button with the current layout. This
        /// helper method is invoked when a Layout is initialized for each 
        /// calculator button.
        /// </summary>
        /// <param name="index">The index of the button.</param>
        /// <param name="btnStyle">The Button style.</param>
        /// <param name="action">The Button action.</param>
        /// <param name="title">The caption of the button.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetButton(int index, CalculatorButtonStyle btnStyle, CalcActions action, string title)
        {
            this.calcButtons[index].Initialize(btnStyle, action, title);
            this.calcButtons[index].Visible = true;
        }

        private void CheckCalcActionValidity(CalcActions caCalcButton)
        {
            if (!Enum.IsDefined(typeof(CalcActions), caCalcButton))
            {
                throw new ArgumentOutOfRangeException("caCalcButton");
            }
        }

        /// <summary>
        /// Returns the button caption color.
        /// </summary>
        /// <param name="caCalcButton">Calculator button.</param>
        /// <returns> Returns Button Color</returns>
        public Color GetButtonColor(CalcActions caCalcButton)
        {
            CheckCalcActionValidity(caCalcButton);

            int index = (int)caCalcButton;

            return this.calcButtons[index].ForeColor;
        }

        /// <summary>
        /// Returns the button caption font.
        /// </summary>
        /// <param name="caCalcButton">Calculator button.</param>
        /// <returns> Returns button Font.</returns>
        public Font GetButtonFont(CalcActions caCalcButton)
        {
            CheckCalcActionValidity(caCalcButton);

            int index = (int)caCalcButton;

            return this.calcButtons[index].Font;
        }

        /// <summary>
        /// Sets the button caption color.
        /// </summary>
        /// <param name="caCalcButton">Calculator button.</param>
        /// <param name="color">Color to set.</param>
        public void SetButtonColor(CalcActions caCalcButton, Color color)
        {
            CheckCalcActionValidity(caCalcButton);
            int index = GetCalcButtonIndexFromAction(caCalcButton);

            if (index != -1)
            {
                this.calcButtons[index].ButtonStyle.ForeColor = color;
                this.calcButtons[index].ForeColor = color;
            }
        }

        /// <summary>
        /// Returns the button index from the calculator button action.
        /// </summary>
        /// <param name="caCalcButton">Button action.</param>
        /// <returns>Button index; -1 if invalid code is passed.</returns>
        private int GetCalcButtonIndexFromAction(CalcActions caCalcButton)
        {
            int index = -1;

            for (int i = 0, len = calcButtons.Length; i < len; ++i)
            {
                if (calcButtons[i].Action == caCalcButton)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Sets the button caption font.
        /// </summary>
        /// <param name="caCalcButton">Calculator button.</param>
        /// <param name="font">Font to set.</param>
        public void SetButtonFont(CalcActions caCalcButton, Font font)
        {
            CheckCalcActionValidity(caCalcButton);
            int index = GetCalcButtonIndexFromAction(caCalcButton);

            if (index != -1)
            {
                this.calcButtons[index].ButtonStyle.Font = font;
                this.calcButtons[index].Font = font;
            }
        }

        /// <summary>
        /// Adds a shortcut key for the button.
        /// </summary>
        /// <param name="index">The index of the button.</param>
        /// <param name="keyData">The KeyData that the button will be invoked through.</param>
        protected void AddShortcutKey(int index, int keyData)
        {
            if (this.mnemonicKeys.ContainsKey(keyData) == false)
                this.mnemonicKeys.Add(keyData, index);
        }

        /// <summary>
        /// Sets the layout constraints for a child control.
        /// </summary>
        /// <param name="layoutType">The layout type.</param>
        /// <param name="control">The target control.</param>
        /// <param name="gbc">The grid bag constraints object for this control.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="prefSize">The preferred size.</param>
        /// <remarks>
        /// This helper method is invoked when a Layout is initialized for each 
        /// non button control.
        /// </remarks>
        private void SetLayoutConstraintsForControl(CalculatorLayoutTypes layoutType, Control control, GridBagConstraints gbc, Size minSize, Size prefSize)
        {
            GridBagLayout layoutManager = GetLayoutManager(layoutType);

            layoutManager.SetConstraints(control, gbc);
            layoutManager.SetMinimumSize(control, minSize);
            layoutManager.SetPreferredSize(control, prefSize);
        }

        private void SetAccessibilitySettingsForControl(Control control, AccessibleRole role, string name, string description, string defaultAction)
        {
            control.AccessibleRole = role;
            control.AccessibleName = name;
            control.AccessibleDescription = description;
            control.AccessibleDefaultActionDescription = defaultAction;
        }

        /// <summary>
        /// Initialized the layout managers and the controls. This method initializes
        /// all the controls that needs to be initialized for a layout.
        /// </summary>
        public void InitializeLayoutAndAddControls()
        {
            // CalculatorControl
            this.Size = new System.Drawing.Size(288, 232);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Accessibility settings
            this.AccessibleDescription = "Calculator control";
            this.AccessibleName = "Calculator Control";
            this.AccessibleRole = AccessibleRole.Default;

            this.calcFinancialLayout = new GridBagLayout();
            this.calcWinStandardLayout = new GridBagLayout();

            this.calcFinancialLayout.AutoLayout = false;
            this.calcFinancialLayout.ContainerControl = this;

            this.calcWinStandardLayout.AutoLayout = false;
            this.calcWinStandardLayout.ContainerControl = this;

            this.Controls.Add(this.textCalculatorBox);
            this.Controls.Add(this.memoryStateDisplay);

            for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
                this.Controls.Add(this.calcButtons[i]);

            this.LayoutType = CalculatorLayoutTypes.WindowsStandard;
        }

        /// <summary>
        /// Initializes the <see cref="CalculatorLayoutTypes.Financial"/> layout for the Calculator Control.
        /// </summary>
        /// <remarks>
        /// The Financial Layout resembles the layout of the calculator popularized
        /// by the Quicken(r) products. Most of the functionality is geared towards
        /// simple banking arithmetic.
        /// </remarks>
        protected void InitializeFinancialLayout()
        {
            this.mnemonicKeys.Clear();

            this.calcFinancialLayout.ContainerControl = this;
            Font defaultFont = Syncfusion.Drawing.FontUtil.CreateFont(this.Font, FontStyle.Regular);
            GridBagConstraints gbc = new GridBagConstraints();
            gbc.Fill = FillType.Horizontal;
            gbc.Insets = c_FinancialInsets;
            CalculatorButtonStyle blueTextBtn;
            CalculatorButtonStyle redTextBtn;
            if (ButtonStyle == ButtonAppearance.Metro)
            {
                blueTextBtn = new CalculatorButtonStyle(ColorTranslator.FromHtml("#2E3192"), defaultFont, this.flatStyle);
                redTextBtn = new CalculatorButtonStyle(Color.White, defaultFont, this.flatStyle);
            }
            else
            {
                blueTextBtn = new CalculatorButtonStyle(Color.Blue, defaultFont, this.flatStyle);
                redTextBtn = new CalculatorButtonStyle(Color.Red, defaultFont, this.flatStyle);
            }
            Size textSize = new Size(104, 20);
            Size normalSize = new Size(26, 20);
            Size largeSize = new Size(52, 20);
            
            // ROW 1 CalculatorDisplay
            gbc.GridPosX = 0;
            gbc.GridPosY = 0;
            gbc.CellSpanX = 4;
            gbc.CellSpanY = 1;
            gbc.WeightX = 1;
            gbc.WeightY = .2;

            if (this.ShowDisplayArea == true)
            {
                this.textCalculatorBox.Visible = true;
                this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.textCalculatorBox, gbc, textSize, textSize);
                this.SetAccessibilitySettingsForControl(this.textCalculatorBox, AccessibleRole.StaticText, "Calculator Display", "Displays results for the calculator", string.Empty);
                gbc.GridPosY++;
            }
            else
                this.textCalculatorBox.Visible = false;

            gbc.Insets = this.FormatingGridBag(ref gbc);

            // ROW 2
            gbc.GridPosX = 0;
            gbc.CellSpanX = 1;
            gbc.CellSpanY = 1;
            this.SetButton(0, blueTextBtn, CalcActions.CalcSpecialClear, "C");
            this.AddShortcutKey(0, (int)Keys.Escape/*27*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[0], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[0], AccessibleRole.PushButton, "CButton", "Clears the calculator control", "Press");

            gbc.GridPosX = 1;
            this.SetButton(1, blueTextBtn, CalcActions.CalcSpecialClearEntry, "CE");
            this.AddShortcutKey(1, (int)Keys.Delete /*16*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[1], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[1], AccessibleRole.PushButton, "CEButton", "Clear the calculator's entry", "Press");

            gbc.GridPosX = 2;
            this.SetButton(2, blueTextBtn, CalcActions.CalcSpecialBackspace, "<-");
            this.AddShortcutKey(2, (int)Keys.Back/*27*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[2], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[2], AccessibleRole.PushButton, "BackSpaceButton", "Deletes text in the Calculator display", "Press");

            gbc.GridPosX = 3;
            this.SetButton(3, blueTextBtn, CalcActions.CalcOperatorPercent, "%");
            this.AddShortcutKey(3, (int)(Keys.D5 | Keys.Shift)/*37*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[3], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[3], AccessibleRole.PushButton, "PercentButton", "Computes the percentage", "Press");

            gbc.GridPosX = 0;
            gbc.GridPosY = 2;
            this.SetButton(4, redTextBtn, CalcActions.CalcDigit7, "7");
            this.AddShortcutKey(4, (int)Keys.D7 /*55*/);
            this.AddShortcutKey(4, (int)Keys.NumPad7);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[4], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[4], AccessibleRole.PushButton, "SevenButton", "Inputs the numeral 7", "Press");

            gbc.GridPosX = 1;
            this.SetButton(5, redTextBtn, CalcActions.CalcDigit8, "8");
            this.AddShortcutKey(5, (int)Keys.D8 /*56*/);
            this.AddShortcutKey(5, (int)Keys.NumPad8);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[5], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[5], AccessibleRole.PushButton, "EightButton", "Inputs the numeral 9", "Press");

            gbc.GridPosX = 2;
            this.SetButton(6, redTextBtn, CalcActions.CalcDigit9, "9");
            this.AddShortcutKey(6, (int)Keys.D9 /*57*/);
            this.AddShortcutKey(6, (int)Keys.NumPad9);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[6], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[6], AccessibleRole.PushButton, "NineButton", "Inputs the numeral 9", "Press");

            gbc.GridPosX = 3;
            this.SetButton(7, blueTextBtn, CalcActions.CalcOperatorDivide, "/");
            this.AddShortcutKey(7, (int)Keys.Divide  /*47*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[7], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[7], AccessibleRole.PushButton, "DivideButton", "Divides two numbers", "Press");
            gbc.GridPosY++;

            // ROW 3
            gbc.GridPosX = 0;
            this.SetButton(8, redTextBtn, CalcActions.CalcDigit4, "4");
            this.AddShortcutKey(8, (int)Keys.D4 /*52*/);
            this.AddShortcutKey(8, (int)Keys.NumPad4);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[8], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[8], AccessibleRole.PushButton, "FourButton", "Inputs the numeral 4", "Press");

            gbc.GridPosX = 1;
            this.AddShortcutKey(9, (int)Keys.D5 /*53*/);
            this.AddShortcutKey(9, (int)Keys.NumPad5);
            this.SetButton(9, redTextBtn, CalcActions.CalcDigit5, "5");
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[9], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[9], AccessibleRole.PushButton, "FiveButton", "Inputs the numeral 5", "Press");

            gbc.GridPosX = 2;
            this.AddShortcutKey(10, (int)Keys.D6 /*54*/);
            this.AddShortcutKey(10, (int)Keys.NumPad6);
            this.SetButton(10, redTextBtn, CalcActions.CalcDigit6, "6");
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[10], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[10], AccessibleRole.PushButton, "SixButton", "Inputs the numeral 6", "Press");

            gbc.GridPosX = 3;
            this.SetButton(11, blueTextBtn, CalcActions.CalcOperatorMultiply, "X");
            this.AddShortcutKey(11, (int)(Keys.D8 | Keys.Shift)/*42*/);
            this.AddShortcutKey(11, (int)Keys.Multiply);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[11], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[11], AccessibleRole.PushButton, "MultiplyButton", "Multiplies two buttons", "Press");
            gbc.GridPosY++;

            // ROW 4
            gbc.GridPosX = 0;
            this.SetButton(12, redTextBtn, CalcActions.CalcDigit1, "1");
            this.AddShortcutKey(12, (int)Keys.D1 /*49*/);
            this.AddShortcutKey(12, (int)Keys.NumPad1);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[12], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[12], AccessibleRole.PushButton, "OneButton", "Inputs the numeral 1", "Press");

            gbc.GridPosX = 1;
            this.SetButton(13, redTextBtn, CalcActions.CalcDigit2, "2");
            this.AddShortcutKey(13, (int)Keys.D2 /*50*/);
            this.AddShortcutKey(13, (int)Keys.NumPad2);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[13], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[13], AccessibleRole.PushButton, "TwoButton", "Inputs the numeral 2", "Press");

            gbc.GridPosX = 2;
            this.SetButton(14, redTextBtn, CalcActions.CalcDigit3, "3");
            this.AddShortcutKey(14, (int)Keys.D3 /*51*/);
            this.AddShortcutKey(14, (int)Keys.NumPad3);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[14], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[14], AccessibleRole.PushButton, "ThreeButton", "Inputs the numeral 3", "Press");

            gbc.GridPosX = 3;
            this.SetButton(15, blueTextBtn, CalcActions.CalcOperatorMinus, "-");
            this.AddShortcutKey(15, (int)Keys.Subtract /*45*/);
            this.AddShortcutKey(15, (int)Keys.OemMinus /*45*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[15], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[15], AccessibleRole.PushButton, "SubtractButton", "Subtracts one number from another", "Press");
            gbc.GridPosY++;

            // ROW 5
            gbc.GridPosX = 0;
            gbc.CellSpanX = 2;
            this.SetButton(16, redTextBtn, CalcActions.CalcDigit0, "0");
            this.AddShortcutKey(16, (int)Keys.D0 /*48*/);
            this.AddShortcutKey(16, (int)Keys.NumPad0);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[16], gbc, largeSize, largeSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[16], AccessibleRole.PushButton, "ZeroButton", "Inputs the numeral 0", "Press");

            gbc.GridPosX = 2;
            gbc.CellSpanX = 1;
            this.SetButton(17, redTextBtn, CalcActions.CalcSpecialDecimal, this.NumberFormatInfoObject.CurrencyDecimalSeparator);
            this.AddShortcutKey(17, (int)Keys.Decimal  /*46*/);
            this.AddShortcutKey(17, (int)Keys.OemPeriod);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[17], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[17], AccessibleRole.PushButton, "DecimalButton", "Decimal Input", "Press");

            gbc.GridPosX = 3;
            this.SetButton(18, blueTextBtn, CalcActions.CalcOperatorPlus, "+");
            this.AddShortcutKey(18, (int)Keys.Oemplus /*61*/);
            this.AddShortcutKey(18, (int)Keys.Add  /*61*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[18], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[18], AccessibleRole.PushButton, "AddButton", "Adds two numbers", "Press");
            gbc.GridPosY++;

            // ROW 6
            gbc.GridPosX = 0;
            gbc.CellSpanX = 4;
            this.SetButton(19, redTextBtn, CalcActions.CalcOperatorEquals, "=");
            this.AddShortcutKey(19, (int)Keys.Enter  /*13*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.Financial, this.calcButtons[19], gbc, textSize, textSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[19], AccessibleRole.PushButton, "EnterButton", "Initiates a calculation", "Press");

            // Hide the controls we do not use
            this.memoryStateDisplay.Visible = false;
            this.calcButtons[20].Visible = false;
            this.calcButtons[21].Visible = false;
            this.calcButtons[22].Visible = false;
            this.calcButtons[23].Visible = false;
            this.calcButtons[24].Visible = false;
            this.calcButtons[25].Visible = false;
            this.calcButtons[26].Visible = false;

            this.Invalidate();
        }

        /// <summary>
        /// Initializes the <see cref="CalculatorLayoutTypes.WindowsStandard"/> layout for the Calculator Control.
        /// </summary>
        /// <remarks>
        /// The Windows Standard Layout resembles the layout of the calculator provides
        /// with the windows operating system. Most of the functionality is the same as
        /// the operation of the Windows operating system calculator.
        /// </remarks>
        protected void InitializeWindowsStandardLayout()
        {
            Font defaultFont = Syncfusion.Drawing.FontUtil.CreateFont(this.Font, FontStyle.Regular);
            GridBagConstraints gbc = new GridBagConstraints();
            gbc.Fill = FillType.Horizontal;

            Insets normalInsets = c_WindowsStandardInsets;
            gbc.Insets = normalInsets;
            CalculatorButtonStyle blueTextBtn;
            CalculatorButtonStyle redTextBtn;
            this.mnemonicKeys.Clear();
            if (ButtonStyle == ButtonAppearance.Metro)
            {
                blueTextBtn = new CalculatorButtonStyle(Color.White, defaultFont, this.flatStyle);
                redTextBtn = new CalculatorButtonStyle(ColorTranslator.FromHtml("#2E3192"), defaultFont, this.flatStyle);
            }
            else
            {
                blueTextBtn = new CalculatorButtonStyle(Color.Blue, defaultFont, this.flatStyle);
                redTextBtn = new CalculatorButtonStyle(Color.Red, defaultFont, this.flatStyle);
            }

            Size textSize = new Size(240, 22);
            Size normalSize = new Size(34, 27);
            Size largeSize = new Size(62, 27);

            // ROW 1 CalculatorDisplay[6 cells]
            gbc.GridPosX = 0;
            gbc.GridPosY = 0;
            gbc.CellSpanX = 6;
            gbc.CellSpanY = 1;
            gbc.WeightX = 1;
            gbc.WeightY = .2;

            if (this.ShowDisplayArea == true)
            {
                this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.textCalculatorBox, gbc, textSize, textSize);
                this.SetAccessibilitySettingsForControl(this.textCalculatorBox, AccessibleRole.StaticText, "CalculatorDisplay", "Displays the Calculator results", string.Empty);
                gbc.GridPosY++;
                this.textCalculatorBox.Visible = true;
                this.toolTipHelp.SetToolTip(this.textCalculatorBox, "The calculator display area");
            }
            else
                this.textCalculatorBox.Visible = false;

            normalInsets = this.FormatingGridBag(ref gbc);
            gbc.Insets = normalInsets;

            // ROW 2 |memory status|Backspace[2 cells]|CE|C[2 cells]
            gbc.GridPosX = 0;
            gbc.CellSpanX = 1;
            gbc.CellSpanY = 1;
            gbc.WeightX = .2;
            gbc.WeightY = .2;
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.memoryStateDisplay, gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.memoryStateDisplay, AccessibleRole.StaticText, "MemoryState", "Shows if there is a value in calculator memory.", string.Empty);
            this.memoryStateDisplay.Visible = true;

            gbc.GridPosX = 1;
            gbc.CellSpanX = 2;
            gbc.CellSpanY = 1;
            gbc.WeightX = .30;
            gbc.WeightY = .2;
            this.SetButton(0, redTextBtn, CalcActions.CalcSpecialBackspace, "Backspace");
            this.AddShortcutKey(0, (int)Keys.Back  /*8*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[0], gbc, largeSize, largeSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[0], AccessibleRole.PushButton, "BackSpaceButton", "Backspace key to delete text.", "Press");

            gbc.GridPosX = 3;
            gbc.CellSpanX = 1;
            gbc.CellSpanY = 1;
            gbc.WeightX = .20;
            gbc.WeightY = .2;
            this.SetButton(1, redTextBtn, CalcActions.CalcSpecialClearEntry, "CE");
            this.AddShortcutKey(1, (int)Keys.Delete /*16*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[1], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[1], AccessibleRole.PushButton, "CEButton", "Clears the content of the Calculator display.", "Press");

            gbc.GridPosX = 4;
            gbc.CellSpanX = 2;
            gbc.CellSpanY = 1;
            gbc.WeightX = .30;
            gbc.WeightY = .2;
            this.SetButton(2, redTextBtn, CalcActions.CalcSpecialClear, "C");
            this.AddShortcutKey(2, (int)Keys.Escape /*27*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[2], gbc, largeSize, largeSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[2], AccessibleRole.PushButton, "CButton", "Clears the content of the Calculator display.", "Press");
            gbc.GridPosY++;

            // ROW 3 MC|7|8|9|/|sqrt
            gbc.GridPosX = 0;
            gbc.CellSpanX = 1;
            gbc.CellSpanY = 1;
            gbc.WeightY = .2;
            gbc.WeightX = .2;

            this.SetButton(3, redTextBtn, CalcActions.CalcOperatorMemoryClear, "MC");
            this.AddShortcutKey(3, (int)(Keys.D7 | Keys.Control) /*0*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[3], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[3], AccessibleRole.PushButton, "MCButton", "Clears the memory of the Calculator.", "Press");

            gbc.GridPosX = 1;
            gbc.WeightX = .16;
            gbc.Insets = normalInsets;
            this.SetButton(4, blueTextBtn, CalcActions.CalcDigit7, "7");
            this.AddShortcutKey(4, (int)Keys.D7 /*55*/);
            this.AddShortcutKey(4, (int)Keys.NumPad7);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[4], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[4], AccessibleRole.PushButton, "SevenButton", "Enters Numeral 7.", "Press");

            gbc.GridPosX = 2;
            this.SetButton(5, blueTextBtn, CalcActions.CalcDigit8, "8");
            this.AddShortcutKey(5, (int)Keys.D8 /*56*/);
            this.AddShortcutKey(5, (int)Keys.NumPad8);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[5], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[5], AccessibleRole.PushButton, "EightButton", "Enters Numeral 8.", "Press");

            gbc.GridPosX = 3;
            this.SetButton(6, blueTextBtn, CalcActions.CalcDigit9, "9");
            this.AddShortcutKey(6, (int)Keys.D9 /*57*/);
            this.AddShortcutKey(6, (int)Keys.NumPad9);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[6], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[6], AccessibleRole.PushButton, "NineButton", "Enters Numeral 9.", "Press");

            gbc.GridPosX = 4;
            this.SetButton(7, redTextBtn, CalcActions.CalcOperatorDivide, "/");
            this.AddShortcutKey(7, (int)Keys.Divide /*47*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[7], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[7], AccessibleRole.PushButton, "DivideButton", "Divides 2 numbers", "Press");

            gbc.GridPosX = 5;
            this.SetButton(8, blueTextBtn, CalcActions.CalcOperatorSqrt, "sqrt");
            this.AddShortcutKey(8, (int)(Keys.D2 | Keys.Shift) /*64*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[8], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[8], AccessibleRole.PushButton, "SquareRootButton", "Computes square root of a number", "Press");
            gbc.GridPosY++;

            // ROW 4 MR|4|5|6|*|%
            gbc.WeightX = .20;
            gbc.GridPosX = 0;
            this.SetButton(9, redTextBtn, CalcActions.CalcOperatorMemoryRecall, "MR");
            this.AddShortcutKey(9, (int)(Keys.R | Keys.Control)/*0*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[9], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[9], AccessibleRole.PushButton, "MRButton", "Computes square root of a number", "Press");

            gbc.GridPosX = 1;
            gbc.WeightX = .16;
            gbc.Insets = normalInsets;
            this.SetButton(10, blueTextBtn, CalcActions.CalcDigit4, "4");
            this.AddShortcutKey(10, (int)Keys.D4 /*52*/);
            this.AddShortcutKey(10, (int)Keys.NumPad4);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[10], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[10], AccessibleRole.PushButton, "FourButton", "Computes square root of a number", "Press");

            gbc.GridPosX = 2;
            this.SetButton(11, blueTextBtn, CalcActions.CalcDigit5, "5");
            this.AddShortcutKey(11, (int)Keys.D5 /*53*/);
            this.AddShortcutKey(11, (int)Keys.NumPad5);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[11], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[11], AccessibleRole.PushButton, "FiveButton", "Computes square root of a number", "Press");

            gbc.GridPosX = 3;
            this.SetButton(12, blueTextBtn, CalcActions.CalcDigit6, "6");
            this.AddShortcutKey(12, (int)Keys.D6 /*54*/);
            this.AddShortcutKey(12, (int)Keys.NumPad6);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[12], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[12], AccessibleRole.PushButton, "SixButton", "Computes square root of a number", "Press");

            gbc.GridPosX = 4;
            this.SetButton(13, redTextBtn, CalcActions.CalcOperatorMultiply, "*");
            this.AddShortcutKey(13, (int)(Keys.D8 | Keys.Shift) /*42*/);
            this.AddShortcutKey(13, (int)Keys.Multiply  /*61*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[13], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[13], AccessibleRole.PushButton, "MultiplyButton", "Computes square root of a number", "Press");

            gbc.GridPosX = 5;
            this.SetButton(14, blueTextBtn, CalcActions.CalcOperatorPercent, "%");
            this.AddShortcutKey(14, (int)(Keys.D5 | Keys.Shift) /*37*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[14], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[14], AccessibleRole.PushButton, "PercentButton", "Computes square root of a number", "Press");
            gbc.GridPosY++;

            // ROW 5 MS|1|2|3|-|1/x
            gbc.GridPosX = 0;
            gbc.WeightX = .20;
            this.SetButton(15, redTextBtn, CalcActions.CalcOperatorMemoryStore, "MS");
            this.AddShortcutKey(15, (int)(Keys.M | Keys.Control) /*0*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[15], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[15], AccessibleRole.PushButton, "MSButton", "Computes square root of a number", "Press");

            gbc.GridPosX = 1;
            gbc.WeightX = .16;

            this.SetButton(16, blueTextBtn, CalcActions.CalcDigit1, "1");
            this.AddShortcutKey(16, (int)Keys.D1 /*49*/);
            this.AddShortcutKey(16, (int)Keys.NumPad1);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[16], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[16], AccessibleRole.PushButton, "OneButton", "Inputs the numeral 1", "Press");

            gbc.GridPosX = 2;
            this.SetButton(17, blueTextBtn, CalcActions.CalcDigit2, "2");
            this.AddShortcutKey(17, (int)Keys.D2 /*50*/);
            this.AddShortcutKey(17, (int)Keys.NumPad2);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[17], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[17], AccessibleRole.PushButton, "TwoButton", "Inputs the numeral 2", "Press");

            gbc.GridPosX = 3;
            this.SetButton(18, blueTextBtn, CalcActions.CalcDigit3, "3");
            this.AddShortcutKey(18, (int)Keys.D3 /*51*/);
            this.AddShortcutKey(18, (int)Keys.NumPad3);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[18], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[18], AccessibleRole.PushButton, "ThreeButton", "Inputs the numeral 3", "Press");

            gbc.GridPosX = 4;
            this.SetButton(19, redTextBtn, CalcActions.CalcOperatorMinus, "-");
            this.AddShortcutKey(19, (int)Keys.OemMinus /*45*/);
            this.AddShortcutKey(19, (int)Keys.Subtract /*45*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[19], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[19], AccessibleRole.PushButton, "MinusButton", "Computes the difference between two numbers", "Press");

            gbc.GridPosX = 5;
            this.SetButton(20, blueTextBtn, CalcActions.CalcOperatorReciprocal, "1/x");
            this.AddShortcutKey(20, (int)Keys.R /*114*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[20], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[20], AccessibleRole.PushButton, "ReciprocalButton", "Computes the reciprocal of a number", "Press");
            gbc.GridPosY++;

            // ROW 6 M+|0|+/-|.|+|=
            gbc.GridPosX = 0;
            gbc.WeightX = .20;
            this.SetButton(21, redTextBtn, CalcActions.CalcOperatorMemoryPlus, "M+");
            this.AddShortcutKey(21, (int)(Keys.P | Keys.Control) /*0*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[21], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[21], AccessibleRole.PushButton, "M+Button", "Add the current display value to memory", "Press");

            gbc.GridPosX = 1;
            gbc.WeightX = .16;
            this.SetButton(22, blueTextBtn, CalcActions.CalcDigit0, "0");
            this.AddShortcutKey(22, (int)Keys.D0 /*48*/);
            this.AddShortcutKey(22, (int)Keys.NumPad0);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[22], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[22], AccessibleRole.PushButton, "ZeroButton", "Inputs the numeral 0", "Press");

            gbc.GridPosX = 2;
            this.SetButton(23, blueTextBtn, CalcActions.CalcOperatorSign, "+/-");
            this.AddShortcutKey(23, (int)Keys.F9 /*0*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[23], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[23], AccessibleRole.PushButton, "SignButton", "Toggles the sign of the display", "Press");

            gbc.GridPosX = 3;
            this.SetButton(24, blueTextBtn, CalcActions.CalcSpecialDecimal, this.NumberFormatInfoObject.CurrencyDecimalSeparator);
            this.AddShortcutKey(24, (int)Keys.Decimal /*46*/);
            this.AddShortcutKey(24, (int)Keys.OemPeriod);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[24], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[24], AccessibleRole.PushButton, "DecimalButton", "Inputs the decimal character.", "Press");

            gbc.GridPosX = 4;
            this.SetButton(25, redTextBtn, CalcActions.CalcOperatorPlus, "+");
            this.AddShortcutKey(25, (int)Keys.Oemplus /*61*/);
            this.AddShortcutKey(25, (int)Keys.Add /*61*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[25], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[25], AccessibleRole.PushButton, "AddButton", "Computes sum of two numbers", "Press");

            gbc.GridPosX = 5;
            this.SetButton(26, redTextBtn, CalcActions.CalcOperatorEquals, "=");
            this.AddShortcutKey(26, (int)Keys.Enter /*13*/);
            this.SetLayoutConstraintsForControl(CalculatorLayoutTypes.WindowsStandard, this.calcButtons[26], gbc, normalSize, normalSize);
            this.SetAccessibilitySettingsForControl(this.calcButtons[26], AccessibleRole.PushButton, "EnterButton", "Initiates a calculation", "Press");
        }

        /// <summary>
        /// Sets the FillType in GridBagConstraints and returns the Insets for use in GridBagConstraints.
        /// </summary>
        /// <param name="gbc">The GridBagConstraints.</param>
        /// <returns>The insets for use.</returns>
        private Insets FormatingGridBag(ref GridBagConstraints gbc)
        {
            Insets normalInsets;

            if (!this.UseVerticalAndHorizontalSpacing)
            {
                gbc.Fill = FillType.Horizontal;
                if (this.LayoutType == CalculatorLayoutTypes.WindowsStandard)
                {
                    normalInsets = c_WindowsStandardInsets;
                }
                else
                {
                    normalInsets = c_FinancialInsets;
                }
            }
            else
            {
                gbc.Fill = FillType.Both;

                int left = this.HorizontalSpacing / 2;
                int top = this.VerticalSpacing / 2;
                int right = left + this.VerticalSpacing % 2;
                int bottom = top + this.HorizontalSpacing % 2;

                normalInsets = new Insets(left, top, right, bottom);
            }
            return normalInsets;
        }

        /// <summary>
        /// Raises the <see cref="ValueCalculated"/> event.
        /// </summary>
        /// <param name="arg">The event data for the ValueCalculated event.</param>
        /// <remarks>
        /// This event is raised when there is a change in the <see cref="Value"/>
        /// property of the CalculatorControl. The handler can output the value to
        /// its own display area. This helps customize the Calculator control usage
        /// and the default display area of the CalculatorControl can be hidden
        /// and the display can be another suitable display area.
        /// </remarks>
        protected virtual void OnValueCalculated(CalculatorValueCalculatedEventArgs arg)
        {
            if (ValueCalculated != null)
                ValueCalculated(this, arg);
        }

        /// <summary>
        /// Transfers the current value to the display.
        /// </summary>
        /// <remarks>
        /// This method displays output only if the <see cref="ShowDisplayArea"/>
        /// property is set to true.
        /// </remarks>
        public void SetDisplayString()
        {
            if (this.ShowDisplayArea == true && this.textCalculatorBox != null)
            {
                this.textCalculatorBox.Text = this.Value.ToString();
            }
        }

        /// <summary>
        /// Refreshes the memory label based on the new memory value.
        /// </summary>
        /// <param name="memoryValue">The memory value.</param>
        /// <remarks>
        /// The memory label is set to M if the memory value is > 0
        /// and empty otherwise.
        /// </remarks>
        public void RefreshMemoryLabel(double memoryValue)
        {
            if (memoryValue > 0)
                this.memoryStateDisplay.Text = "M";
            else
                this.memoryStateDisplay.Text = string.Empty;
        }

        /// <summary>
        /// The Calculator Buttons will use this method to call back 
        /// a click action and pass in their <see cref="CalcActions"/> type
        /// action property.
        /// </summary>
        /// <param name="action">The action that is to be performed.</param>
        public void ButtonAction(CalcActions action)
        {
            this.lastAction = action;
            this.calcEngineObject.HandleAction(action);
        }

        /// <summary>
        /// The calculator engine will invoke this method for its parent
        /// to be informed of a change in its value.
        /// </summary>
        /// <param name="internalValue">The internal string value of the engine.</param>
        /// <param name="errorCondition">The error condition.</param>
        /// <param name="feedbackMessage">The feedback message.</param>
        /// <param name="memoryValue">The memory value.</param>
        public void EngineValueChanged(CalculatorValue internalValue, bool errorCondition, string feedbackMessage, double memoryValue)
        {
            this.SetDisplayString();
            this.RefreshMemoryLabel(memoryValue);
            CalculatorValueCalculatedEventArgs args = new CalculatorValueCalculatedEventArgs(internalValue, errorCondition, feedbackMessage, this.lastAction);
            this.OnValueCalculated(args);
        }

        /// <summary>
        /// Gets or sets the background color, gradient and other styles can be set through 
        /// this property.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Lets you set the background color, gradient, etc."),
        Category("Appearance")
        ]
        public BrushInfo BackgroundColor
        {
            get
            {
                return m_bgBrush;
            }
            set
            {
                if (value != m_bgBrush)
                {
                    m_bgBrush = value;
                    this.Invalidate();
                }
            }
        }
        private Color m_metrocolor = System.Drawing.SystemColors.Control;
        public Color MetroColor
        {
            get
            {
                return m_metrocolor;
            }
            set
            {
                if (value != m_metrocolor)
                {
                    m_metrocolor = value;
                    for (int i = 0; i < CalculatorControl.totalButtonCount; i++)
                    {
                        this.calcButtons[i].BackColor = value ;
                        this.memoryStateDisplay.BorderColor = value;
                    }
                  
                    
                }
            }
        }
        #region VISUALSTYLE
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

                if (value == "Office2007Blue")
                {
                    ButtonStyle = ButtonAppearance.Office2007;
                    useVisualStyle = true;
                    Office2007Theme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    ButtonStyle = ButtonAppearance.Office2007;
                    useVisualStyle = true;
                    Office2007Theme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    ButtonStyle = ButtonAppearance.Office2007;
                    useVisualStyle = true;
                    Office2007Theme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    ButtonStyle = ButtonAppearance.Office2010;
                    useVisualStyle = true;
                    Office2010Theme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    ButtonStyle = ButtonAppearance.Office2010;
                    useVisualStyle = true;
                    Office2010Theme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    ButtonStyle = ButtonAppearance.Office2010;
                    useVisualStyle = true;
                    Office2010Theme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    ButtonStyle = ButtonAppearance.Office2007;
                    useVisualStyle = true;
                    Office2007Theme = Office2007Theme.Managed;
                }
                else if (value == "Classic")
                    ButtonStyle = ButtonAppearance.Classic;
                else if (value == "Office2000")
                    ButtonStyle = ButtonAppearance.Office2000;
                else if (value == "Office2003")
                    ButtonStyle = ButtonAppearance.Office2003;
                else if (value == "OfficeXP")
                    ButtonStyle = ButtonAppearance.OfficeXP;
                else if (value == "WindowsXP")
                    ButtonStyle = ButtonAppearance.WindowsXP;
                else if (value == "None")
                    ButtonStyle = ButtonAppearance.None;
            }
        }
        /// <summary>
        /// Gets or sets colorschemes for Office2007 visual style.
        /// </summary>
        [
        Description("Colorschemes for Office2007 visual style."),
        Category("Appearance"),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007Theme
        {
            get
            {
                return m_office2007Theme;
            }
            set
            {
                if (value != m_office2007Theme)
                {
                    m_office2007Theme = value;
                    OnOffice2007ThemeChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets colorschemes for Office2010 visual style.
        /// </summary>
        [
        Description("Colorschemes for Office2010 visual style."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get
            {
                return m_office2010Theme;
            }
            set
            {
                if (value != m_office2010Theme)
                {
                    m_office2010Theme = value;
                    OnOffice2010ThemeChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to use visual style.
        /// </summary>
        [
        Description("Indicates whether to use visual style."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public bool UseVisualStyle
        {
            get
            {
                return this.useVisualStyle && this.FlatStyle != FlatStyle.System;
            }

            set
            {
                if (this.useVisualStyle != value && this.FlatStyle != FlatStyle.System)
                {
                    this.useVisualStyle = value;
                    this.OnUseVisualStyleChanged();
                    this.RaiseFlatStyleChangedEvent();
                }
            }
        }

        private void OnUseVisualStyleChanged()
        {
            if (this.ButtonStyle == ButtonAppearance.Office2007 || this.ButtonStyle == ButtonAppearance.Office2010)
            {
                if (this.UseVisualStyle)
                {
                    if (this.ButtonStyle == ButtonAppearance.Office2007)
                        this.ChangeBackgroundColor(this.Office2007Theme);
                    else
                        this.ChangeBackgroundColor(this.Office2010Theme);
                }
                else
                {
                    this.ResetBackgroundColor();
                    this.ResetBackColor();
                }
            }
        }

        /// <summary>
        /// Gets or sets the button style for the Calculator Control.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        DefaultValue(ButtonAppearance.Classic),
        Description("The button style for the Calculator Control."),
        Category("Appearance")
        ]
        public ButtonAppearance ButtonStyle
        {
            get
            {
                return appearance;
            }
            set
            {
                if (appearance != value)
                {
                    appearance = value;
                    InitializeFinancialLayout();
                    InitializeWindowsStandardLayout();
                    if (appearance == ButtonAppearance.Metro)
                        this.UseVisualStyle = true;
                    this.OnButtonStyleChanged();
                    this.RaiseFlatStyleChangedEvent();
                }
            }
        }

        private void OnButtonStyleChanged()
        {
            if (this.UseVisualStyle)
            {
                if (this.ButtonStyle == ButtonAppearance.Office2007)
                {
                    this.ChangeBackgroundColor(this.Office2007Theme);
                }
                else if (this.ButtonStyle == ButtonAppearance.Office2010)
                {
                    this.ChangeBackgroundColor(this.Office2010Theme);
                }
                else
                {
                    if (this.ButtonStyle == ButtonAppearance.Metro)
                    {
                        this.BorderStyle = Border3DStyle.Flat;
                        this.memoryStateDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
                        this.memoryStateDisplay.BorderColor = MetroColor;
                        textCalculatorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    }
                    else
                    {
                        this.memoryStateDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D ;

                        textCalculatorBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                    }
                    this.ResetBackgroundColor();
                    this.ResetBackColor();
                }
            }
        }
        #endregion

        #region Class codedom serialization
        protected void ResetBackgroundColor()
        {
            m_bgBrush = BrushInfo.Empty;
            this.Invalidate();
        }

        protected bool ShouldSerializeBackgroundColor()
        {
            return !m_bgBrush.Equals(BrushInfo.Empty);
        }
        #endregion

        /// <summary>
        /// The Calculator Engine class is used by the <see cref="CalculatorControl"/> to
        /// perform the internal arithmetic calculations. The Calculator Control itself does not
        /// retain any information about the calculations performed. 
        /// </summary>
        /// <remarks>
        /// The design of the Calculator Control uses the Calculator Engine class for all its
        /// calculations. The Calculator Control only takes user input and passes the results
        /// of the calculation back to the display.
        /// <para>
        /// Give the functionality that this class is required to perform, it provides 
        /// methods and properties that will perform calculations and also inform the 
        /// Calculator Control about the changes.
        /// </para>
        /// <para>
        /// You will not need to use this class separately.
        /// </para>
        /// </remarks>
        internal class Engine : ICalculatorEngine
        {
            /// <summary>
            /// The internal value of the Calculator Engine.
            /// </summary>
            private CalculatorValue calculatorValue;

            /// <summary>
            /// The memory value of the Calculator Engine.
            /// </summary>
            private double memoryNumber;

            /// <summary>
            /// Indicates whether data is appended to the double part.
            /// </summary>
            private bool decimalMode = false;

            /// <summary>
            /// Replaces the text in the display string for the next input.
            /// </summary>
            private bool replaceMode;

            /// <summary>
            /// The current error condition.
            /// </summary>
            private bool errorCondition;

            /// <summary>
            /// The error message in case the Calculator Engine is in error state.
            /// </summary>
            private string message;

            /// <summary>
            /// Indicates whether the equal button was clicked.
            /// </summary>
            private bool equalClicked;

            /// <summary>
            /// Indicates whether the percent operation was performed last.
            /// </summary>
            private bool percentLastClicked = false;

            /// <summary>
            /// The internal stack object used for the calculations.
            /// </summary>
            private ArrayList stackObject;

            /// <summary>
            /// The stack position.
            /// </summary>
            private int stackTopValue;

            /// <summary>
            /// Indicates whether a digit was entered lastly.
            /// </summary>
            private bool digitEntered;

            /// <summary>
            /// The parent object.
            /// </summary>
            private ICalculatorEngineParent calculatorEngineParentObject;

            /// <summary>
            /// Globalization information.
            /// </summary>
            private NumberFormatInfo numberFormatInfoObject;

            /// <summary>
            /// Indicates whether the Reciprocal or Sqrt operator was selected.
            /// </summary>
            private bool singleOperator;

            /// <summary>
            /// Initializes a new instance of the Engine class.
            /// </summary>
            /// <param name="parent">The calculator engine parent.</param>
            /// <remarks>
            /// This constructor invokes the default constructor and also
            /// assigns the parameter that is passed in to be the parent for this
            /// CalculatorEngine. The interface <see cref="ICalculatorEngineParent"/>
            /// is defined for the interaction between the Calculator Engine and the
            /// Parent.
            /// </remarks>
            public Engine(ICalculatorEngineParent parent)
            {
                this.calculatorValue = new CalculatorValue();
                this.InitializeCalculatorStack();
                this.numberFormatInfoObject = new NumberFormatInfo();
                this.calculatorEngineParentObject = null;
                this.replaceMode = false;
                this.singleOperator = false;

                this.calculatorEngineParentObject = parent;
            }

            /// <summary>
            /// The CalculatorEngine class uses an internal stack to
            /// perform the calculations. This method initializes the
            /// stack.
            /// </summary>
            private void InitializeCalculatorStack()
            {
                this.stackTopValue = 0;
                this.stackObject = new ArrayList();
                this.stackObject.Add(new object());
                this.stackObject.Add(new object());
                this.stackObject.Add(new object());
                this.digitEntered = true;
                this.equalClicked = false;
            }

            /// <summary>
            /// Returns the default double character recognized by system functions.
            /// </summary>
            /// <returns> Returns String value.</returns>
            private string GetFormatDecimal()
            {
                return this.NumberFormatInfoObject.CurrencyDecimalSeparator;

                // NumberFormatInfo info = new NumberFormatInfo();
                // string strDecimal = info.CurrencyDecimalSeparator;
                // return strDecimal;
            }

            /// <summary>
            /// Gets or sets the top of the stack.
            /// </summary>
            private int StackTop
            {
                get
                {
                    return this.stackTopValue;
                }

                set
                {
                    this.stackTopValue = value;
                }
            }

            /// <summary>
            /// Helper function that performs the arithmetic calculation.
            /// </summary>
            /// <param name="op1">Operand 1.</param>
            /// <param name="oper">The operator.</param>
            /// <param name="op2">Operand 2.</param>
            /// <returns>The calculated value.</returns>
            private double DoCalculation(double op1, CalcActions oper, double op2)
            {
                double calcValue = 0;

                try
                {
                    switch (oper)
                    {
                        case CalcActions.CalcOperatorPlus:
                            return op1 + op2;

                        case CalcActions.CalcOperatorMinus:
                            return op1 - op2;

                        case CalcActions.CalcOperatorMultiply:
                            return op1 * op2;

                        case CalcActions.CalcOperatorDivide:
                            return op1 / op2;

                        case CalcActions.CalcOperatorPercent:
                            return op1 * op2 / 100;

                        default:
                            return 0;
                    }
                }
                catch
                {
                    this.errorCondition = true;
                    this.message = "Calculation failed";
                }

                return calcValue;
            }

            /// <summary>
            /// Pushes the element - could be an operand or the operator-
            /// into the stack.
            /// </summary>
            /// <param name="element">The object to be pushed into the stack.</param>
            private void Push(object element)
            {
                this.StackTop++;
                this.stackObject[this.StackTop] = element;
            }

            /// <summary>
            /// Pops the top element from the stack.
            /// </summary>
            /// <returns> Returns Stack object</returns>
            private object Pop()
            {
                return this.stackObject[this.StackTop--];
            }

            /// <summary>
            /// Indicates whether the stack is empty.
            /// </summary>
            /// <returns>True if the stack is empty; false otherwise.</returns>
            private bool StackIsEmpty()
            {
                if (this.StackTop == 0)
                    return true;
                else
                    return false;
            }

            /// <summary>
            /// Clears the calculation stack.
            /// </summary>
            private void ClearStack()
            {
                this.StackTop = 0;
            }

            /// <summary>
            /// Gets or sets the NumberFormatInfo class that defines the globalization
            /// values for the CalculatorEngine.
            /// </summary>
            /// <remarks>
            /// This is a read only property.
            /// <para>
            /// The values for the NumberFormatInfo object can be set through the
            /// individual properties such as <see cref="DecimalSeparator"/>
            /// </para>
            /// </remarks>
            public NumberFormatInfo NumberFormatInfoObject
            {
                get
                {
                    if (this.numberFormatInfoObject != null)
                        return this.numberFormatInfoObject;
                    else
                    {
                        this.numberFormatInfoObject = new NumberFormatInfo();
                        this.numberFormatInfoObject.CurrencyDecimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                        return this.numberFormatInfoObject;
                    }
                }

                set
                {
                    this.numberFormatInfoObject = value;
                    if (this.Value != null)
                        this.Value.NumberFormatInfoObject = value;
                }
            }

            /// <summary>
            /// Gets or sets the Decimal separator that is in effect in the globalization settings.
            /// </summary>
            /// <remarks>
            /// By default this value is taken from the globalization values provided
            /// by the NumberFormatInfo object. However, this default value can be changed
            /// by setting this property.
            /// </remarks>
            public string DecimalSeparator
            {
                get
                {
                    string decimalSeparator = String.Empty;

                    if (this.numberFormatInfoObject != null)
                        decimalSeparator = this.NumberFormatInfoObject.CurrencyDecimalSeparator;
                    else
                        decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;

                    return decimalSeparator;
                }

                set
                {
                    this.NumberFormatInfoObject.CurrencyDecimalSeparator = value;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the current error condition of the Calculator Engine.
            /// </summary>
            /// <remarks>
            /// This is a read only property that returns the current error condition
            /// of the Calculator Engine.
            /// </remarks>
            public bool ErrorCondition
            {
                get
                {
                    return this.errorCondition;
                }
            }

            /// <summary>
            /// Gets a value indicating whether current error message that accompanies an error condition.
            /// </summary>
            /// <remarks>
            /// This property is valid only if the <see cref="ErrorCondition"/> 
            /// property is set to true.
            /// </remarks>
            public string Message
            {
                get
                {
                    return this.message;
                }
            }

            /// <summary>
            /// Gets or sets the Calculator Value object that specifies the current value of
            /// the Calculator Engine.
            /// </summary>
            public CalculatorValue Value
            {
                get
                {
                    if (this.calculatorValue != null)
                        return this.calculatorValue;
                    else
                        return CalculatorValue.Empty;
                }

                set
                {
                    this.calculatorValue = value;
                }
            }

            /// <summary>
            /// Handler for the Clear Entry action. This action clears the last entry or error.
            /// </summary>
            /// <returns>True if the action completed; false otherwise.</returns>
            /// <remarks>
            /// The interaction between the Calculator Control buttons and the
            /// Calculator Engine is defined by the <see cref="CalculatorButton.ButtonAction"/>
            /// event raised. This event is handled by the <see cref="HandleAction"/> method.
            /// <para>
            /// The HandleAction method then invokes the HandleClearEntry method if the "Clear
            /// Entry" button was clicked.
            /// </para>
            /// </remarks>
            public bool HandleClearEntry()
            {
                if (this.errorCondition == true)
                    HandleClear();
                else
                {
                    this.SetValue(CalculatorValue.Empty);
                    this.replaceMode = true;
                }
                return true;
            }

            /// <summary>
            /// Handles the button action based on the <see cref="CalcActions"/> object that was passed in.
            /// </summary>
            /// <param name="action">The calculator action.</param>
            /// <returns>True if the action was handled successfully; false otherwise.</returns>
            /// <remarks>
            /// The interaction between the Calculator Control buttons and the
            /// Calculator Engine is defined by the <see cref="CalculatorButton.ButtonAction"/>
            /// event raised. This event is handled by the <see cref="HandleAction"/> method.
            /// <para>
            /// The HandleAction method then invokes the appropriate method based on the
            /// button that was clicked.
            /// </para>
            /// </remarks>
            public bool HandleAction(CalcActions action)
            {
                bool returnValue = true;
                double calculatedValue = 0;
                double valueNumber = 0;

                try
                {
                    if (this.errorCondition == true)
                    {
                        if (action != CalcActions.CalcSpecialClear && action != CalcActions.CalcSpecialClearEntry)
                            return false;
                    }

                    valueNumber = this.Value.ToDouble();
                    if (valueNumber == Double.NaN ||
                        valueNumber == Double.PositiveInfinity ||
                        valueNumber == Double.NegativeInfinity)
                    {
                        if (action != CalcActions.CalcSpecialClear && action != CalcActions.CalcSpecialClearEntry)
                        {
                            this.errorCondition = true;
                            this.message = "Invalid input";
                            return false;
                        }
                    }

                    switch (action)
                    {
                        case CalcActions.CalcDigit0:
                        case CalcActions.CalcDigit1:
                        case CalcActions.CalcDigit2:
                        case CalcActions.CalcDigit3:
                        case CalcActions.CalcDigit4:
                        case CalcActions.CalcDigit5:
                        case CalcActions.CalcDigit6:
                        case CalcActions.CalcDigit7:
                        case CalcActions.CalcDigit8:
                        case CalcActions.CalcDigit9:
                            this.HandleDigit(action);
                            this.digitEntered = true;
                            break;

                        case CalcActions.CalcSpecialDecimal:
                            this.HandleDecimalPoint();
                            break;

                        case CalcActions.CalcSpecialBackspace:
                            this.HandleBackspace();
                            break;

                        case CalcActions.CalcSpecialClear:
                            this.HandleClear();
                            break;

                        case CalcActions.CalcSpecialClearEntry:
                            this.HandleClearEntry();
                            break;

                        case CalcActions.CalcOperatorNone:
                            break;

                        case CalcActions.CalcOperatorEquals:
                            if (this.StackIsEmpty() == false)
                            {
                                double operand1, operand2;
                                CalcActions oper;
                                bool continueAction = true;

                                if (this.equalClicked == false)
                                {
                                    operand2 = valueNumber;
                                    oper = (CalcActions)this.Pop();
                                    operand1 = (double)this.Pop();

                                    if (this.calculatorEngineParentObject.RepeatAssignAction == false && percentLastClicked)
                                    {
                                        continueAction = false;
                                    }
                                }
                                else
                                {
                                    operand1 = valueNumber;
                                    oper = (CalcActions)this.Pop();
                                    operand2 = (double)this.Pop();

                                    if (this.calculatorEngineParentObject.RepeatAssignAction == false)
                                    {
                                        continueAction = false;
                                    }
                                }

                                if (continueAction)
                                {
                                    calculatedValue = this.DoCalculation(operand1, oper, operand2);
                                    this.ClearStack();
                                    this.Push(operand2);
                                    this.Push(oper);
                                    this.digitEntered = true;
                                    this.Value.SetValue(calculatedValue);
                                }
                            }
                            this.equalClicked = true;
                            this.replaceMode = true;
                            break;

                        case CalcActions.CalcOperatorPercent:
                            if (this.StackIsEmpty() == false)
                            {
                                double operand2 = valueNumber; // double
                                CalcActions oper = (CalcActions)this.Pop();
                                double operand1 = (double)this.Pop(); // double

                                calculatedValue = this.DoCalculation(operand1, action, operand2);
                                this.Value.SetValue(calculatedValue);
                                this.Push(operand1);
                                this.Push(oper);

                                percentLastClicked = true;
                            }
                            this.replaceMode = true;
                            break;

                        case CalcActions.CalcOperatorReciprocal:
                            calculatedValue = 1 / valueNumber;
                            this.Value.SetValue(calculatedValue);
                            this.replaceMode = true;
                            this.digitEntered = false;
                            this.singleOperator = true;
                            break;

                        case CalcActions.CalcOperatorSign:
                            calculatedValue = -1 * valueNumber;
                            this.Value.SetValue(calculatedValue);
                            break;

                        case CalcActions.CalcOperatorSqrt:
                            calculatedValue = Math.Sqrt(valueNumber);
                            this.Value.SetValue(calculatedValue);
                            this.digitEntered = false;
                            this.replaceMode = true;
                            this.singleOperator = true;
                            break;

                        case CalcActions.CalcOperatorPlus:
                        case CalcActions.CalcOperatorMinus:
                        case CalcActions.CalcOperatorDivide:
                        case CalcActions.CalcOperatorMultiply:
                            if (this.digitEntered == true || this.singleOperator == true)
                            {
                                if (this.StackIsEmpty() == true || this.equalClicked == true)
                                {
                                    this.ClearStack();
                                    this.Push(valueNumber);
                                    this.Push(action);
                                }
                                else
                                {
                                    double operand2 = valueNumber; // double
                                    CalcActions oper = (CalcActions)this.Pop();
                                    double operand1 = (double)this.Pop(); // double

                                    calculatedValue = this.DoCalculation(operand1, oper, operand2);

                                    this.Push(calculatedValue);
                                    this.Push(action);
                                    this.Value.SetValue(calculatedValue);
                                }

                                this.singleOperator = false;
                            }
                            else
                            {
                                if (this.StackIsEmpty() == false)
                                {
                                    this.Pop();
                                    this.Push(action);
                                }
                            }
                            this.replaceMode = true;
                            this.decimalMode = false;
                            this.digitEntered = false;
                            break;

                        case CalcActions.CalcOperatorMemoryClear:
                            this.memoryNumber = 0;
                            break;

                        case CalcActions.CalcOperatorMemoryPlus:
                            this.memoryNumber += valueNumber;
                            this.replaceMode = true;
                            break;

                        case CalcActions.CalcOperatorMemoryRecall:
                            this.Value.SetValue(this.memoryNumber);
                            this.digitEntered = true;
                            this.replaceMode = true;
                            break;

                        case CalcActions.CalcOperatorMemoryStore:
                            this.memoryNumber = valueNumber;
                            this.replaceMode = true;
                            break;

                        default:
                            break;
                    }

                    if (action != CalcActions.CalcOperatorEquals && action != CalcActions.CalcSpecialBackspace)
                    {
                        this.equalClicked = false;

                        if (action != CalcActions.CalcOperatorPercent)
                            percentLastClicked = false;
                    }

                    if (Double.IsInfinity(calculatedValue))
                    {
                        this.errorCondition = true;
                        this.message = "Infinity";
                    }
                    else if (Double.IsNaN(calculatedValue))
                    {
                        this.errorCondition = true;
                        this.message = "Not a number";
                    }

                    this.RaiseCalculatorValueChangedEvent();
                }
                catch (ArgumentException ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    returnValue = false;
                }
                catch (Exception ex)
                {
                    returnValue = false;
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(typeof(CalculatorControl.Engine), ex))
                        throw ex;
                }

                return returnValue;
            }

            /// <summary>
            /// Raises the <see cref="EngineValueChanged"/> event.
            /// </summary>
            /// <remarks>
            /// The EngineValueChanged event is raised when there is a change in the
            /// value or the error condition.
            /// <para>
            /// See <see cref="EngineValueChanged"/> for more information.
            /// </para>
            /// </remarks>
            protected void RaiseCalculatorValueChangedEvent()
            {
                if (this.calculatorEngineParentObject != null)
                    this.calculatorEngineParentObject.EngineValueChanged(this.Value, this.ErrorCondition, this.Message, this.memoryNumber);
            }

            /// <summary>
            /// A number has been entered.
            /// </summary>
            /// <param name="action">The value of the number.</param>
            /// <returns>^True if there is no error. </returns>
            /// <remarks>
            /// The new digit is added to the appropriate operand and held in memory
            /// for calculations. This is subject to the max length restrictions.
            /// </remarks>
            public bool HandleDigit(CalcActions action)
            {
                double valueNumber = 0;
                string formatDecimal = this.GetFormatDecimal();
                string stringForCalculations = this.Value.ToString();
                int decimalPositionSeek;

                try
                {
                    valueNumber = Double.Parse(stringForCalculations, this.NumberFormatInfoObject);
                }
                catch (Exception e)
                {
                    this.errorCondition = true;
                    this.message = "Invalid number Format";
                    throw e;
                }

                if (this.errorCondition == true)
                    return false;
                if (this.equalClicked == true)
                    this.ClearStack();
                string digit = Convert.ToString((int)action);
                char c = Convert.ToChar(digit);

                if (this.decimalMode == true)
                {
                    if (this.replaceMode == true)
                    {
                        stringForCalculations = Convert.ToString(formatDecimal) + Convert.ToString(c);
                    }
                    else
                    {
                        decimalPositionSeek = stringForCalculations.IndexOf(formatDecimal);
                        if (decimalPositionSeek == -1)
                            stringForCalculations += formatDecimal;

                        stringForCalculations += Convert.ToString(c);
                    }
                }
                else
                {
                    if (this.replaceMode == true)
                    {
                        stringForCalculations = Convert.ToString(c);
                    }
                    else
                    {
                        // We ignore the case where leading zeros are entered
                        if ((valueNumber == 0 && action == CalcActions.CalcDigit0) == false)
                        {
                            if (valueNumber == 0)
                            {
                                valueNumber += Convert.ToDouble(Convert.ToString(c));
                                stringForCalculations = Convert.ToString(valueNumber);
                            }
                            else
                            {
                                // This character is allowed, so add it to the display string
                                decimalPositionSeek = stringForCalculations.IndexOf(formatDecimal);
                                if (decimalPositionSeek == -1)
                                    stringForCalculations += Convert.ToString(c);
                                else
                                {
                                    string integerPart = stringForCalculations.Substring(0, decimalPositionSeek);
                                    string decimalPart = stringForCalculations.Substring(decimalPositionSeek + 1, stringForCalculations.Length - decimalPositionSeek - 1);
                                    integerPart += Convert.ToString(c);
                                    stringForCalculations = integerPart + formatDecimal + decimalPart;
                                }
                            }
                        }
                    }
                }

                this.Value.SetValue(stringForCalculations);
                this.replaceMode = false;
                return true;
            }

            /// <summary>
            /// This method is invoked when the decimal point button has been clicked.
            /// </summary>
            /// <returns>True if the action was handled without error; false otherwise.</returns>
            /// <remarks>
            /// All digits entered after this is added to the decimal portion of the
            /// internal value string.
            /// </remarks>
            public bool HandleDecimalPoint()
            {
                if (this.errorCondition == true)
                    return false;

                this.decimalMode = true;
                return true;
            }

            /// <summary>
            /// This method is invoked when the backspace key is pressed.
            /// </summary>
            /// <returns>True if the action was handled without error; false otherwise.</returns>
            /// <remarks>
            /// Deletes one character from the end of the displayed current value.
            /// This key is ignored if the value in the display is a calculated value.
            /// </remarks>
            public bool HandleBackspace()
            {
                string formatDecimal = this.GetFormatDecimal();

                // We just return if any of these conditions are true. 
                // We don't erase any calculated output.
                if (this.errorCondition == true || this.digitEntered == false || this.equalClicked == true)
                    return false;

                this.replaceMode = false;

                // string stringForCalculations = this.SwitchDecimalSeparator(this.Value.ToString());
                string stringForCalculations = this.Value.ToString();

                // Remove the right most character
                int len = this.Value.ToString().Length;
                int moveBy = 1;

                char lastChar = this.Value.ToString()[this.Value.ToString().Length - 1];
                if (lastChar == Convert.ToChar(this.GetFormatDecimal()))
                    moveBy += 1;

                this.Value.SetValue(this.Value.ToString().Remove(len - Math.Min(moveBy, len), 1));
                if (this.Value.ToString() == String.Empty ||
                    this.Value.ToString() == Convert.ToString(this.GetFormatDecimal()))
                {
                    this.Value.SetValue(0);
                    this.decimalMode = false;
                }

                return true;
            }

            /// <summary>
            /// This method is invoked when the Clear Button is pressed.
            /// </summary>
            /// <returns>True if the action was handled without error; false otherwise.</returns>
            /// <remarks>
            /// This action resets the Calculator Engine and the display is also initialized.
            /// </remarks>
            public bool HandleClear()
            {
                this.Reset();
                this.replaceMode = false;
                this.decimalMode = false;
                return true;
            }

            /// <summary>
            /// This takes the Calculator Engine object back to the initialized state.
            /// </summary>
            /// <remarks>
            /// All data held in memory for calculations are dropped.
            /// </remarks>
            public void Reset()
            {
                this.Value = CalculatorValue.Empty;
                this.Value.NumberFormatInfoObject = this.NumberFormatInfoObject;
                this.errorCondition = false;
                this.message = String.Empty;
                this.decimalMode = false;
                this.ClearStack();
            }

            public CalculatorValue GetValue()
            {
                return this.Value;
            }

            public void SetValue(CalculatorValue value)
            {
                this.Value = value;
            }
        }
    }
    internal class CustomLabel : Label
    {
        Color m_BorderColor= System .Drawing .SystemColors .Control  ;
        
        internal  Color BorderColor
        {
            get
            {
                return m_BorderColor;
            }
            set
            {
                if (value != m_BorderColor)
                {
                    m_BorderColor = value;
                   
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if ((this.Parent as CalculatorControl ).ButtonStyle == ButtonAppearance .Metro )
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, BorderColor, ButtonBorderStyle.Solid);
        }

    }
    /// <summary>
    /// This interface can be implemented by classes that want to act
    /// as the parent for this calculator engine.
    /// </summary>
    /// <remarks>
    /// The Calculator Control implements this interface to get notifications
    /// from the Calculator Engine when there is a change in the state of
    /// the Calculator Engine.
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface ICalculatorButtonParent
    {
        /// <summary>
        /// The Calculator Buttons will use this method to call back 
        /// a click action and pass in their <see cref="CalcActions"/> type
        /// action property.
        /// </summary>
        /// <param name="action">The action that is to be performed.</param>
        void ButtonAction(CalcActions action);

        /// <summary>
        /// This handler will be invoked by the buttons each time a 
        /// button has the focus and the KeyDown event is raised. This will
        /// be used by the parent Container Control to process keyboard shortcuts.
        /// </summary>
        /// <param name="e">The KeyEventArgs defining the KeyDown event.</param>
        void HandleChildKeyDown(KeyEventArgs e);
    }

    /// <summary>
    /// This interface can be implemented by classes that want to act
    /// as the parent for this calculator engine.
    /// </summary>
    /// <remarks>
    /// The Calculator Control implements this interface to get notifications
    /// from the Calculator Engine when there is a change in the state of
    /// the Calculator Engine.
    /// </remarks>
    public interface ICalculatorEngineParent
    {
        /// <summary>
        /// The calculator engine will invoke this method for its parent
        /// to be informed of a change in its value.
        /// </summary>
        /// <param name="internalValue">The internal string value of the engine.</param>
        /// <param name="errorCondition">The error condition.</param>
        /// <param name="feedbackMessage">The feedback message.</param>
        /// <param name="memoryValue">The memory value.</param>
        void EngineValueChanged(CalculatorValue internalValue, bool errorCondition, string feedbackMessage, double memoryValue);

        /// <summary>
        /// Gets a value indicating whether the engine needs to repeat the assign (=) action.
        /// </summary>
        bool RepeatAssignAction
        {
            get;
        }
    }

    /// <summary>
    /// This interface is implemented by the Calculator Engine class to 
    /// provide a common interface for interacting with the Calculator Control.
    /// </summary>
    /// <remarks>
    /// The Calculator Control can work with any class that implements this
    /// interface.
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    public interface ICalculatorEngine
    {
        /// <summary>
        /// This method allows the Calculator Control to tell the
        /// Calculator Engine to perform an action. 
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        /// <returns>Return bool value</returns>
        bool HandleAction(CalcActions action);

        /// <summary>
        /// The Value of the Calculator Engine at this point.
        /// </summary>
        /// <returns> Returns Calculator value</returns>
        CalculatorValue GetValue();

        /// <summary>
        /// Sets the value of the Calculator Engine.
        /// </summary>
        /// <param name="value">The value.</param>
        void SetValue(CalculatorValue value);

        /// <summary>
        /// Resets the Calculator Engine.
        /// </summary>
        /// <remarks>
        /// This method is for resetting the Calculator Engine. This is useful when
        /// switching layouts for example.
        /// </remarks>
        void Reset();

        /// <summary>
        /// Gets or sets the NumberFormatInfo object.
        /// </summary>
        NumberFormatInfo NumberFormatInfoObject
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Defines the look and feel of a <see cref="CalculatorButton"/>.
    /// </summary>
    /// <remarks>
    /// The CalcultorBtnStyle class is closely tied to the display attributes
    /// of the <see cref="CalculatorButton"/> class.
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class CalculatorButtonStyle
    {
        /// <summary>
        /// The font to be used for the button caption.
        /// </summary>
        private Font font;

        /// <summary>
        /// The forecolor for the button.
        /// </summary>
        private Color foreColor;

        /// <summary>
        /// The flat style to be applied to the button
        /// </summary>
        private FlatStyle flatStyle;

        /// <summary>
        /// The StyleChanged event.
        /// </summary>
        public event EventHandler StyleChanged;

        /// <summary>
        /// Initializes a new instance of the CalculatorButtonStyle class.
        /// </summary>
        public CalculatorButtonStyle()
        {
            this.font = Syncfusion.Drawing.FontUtil.CreateFont(Control.DefaultFont.Name, 8);
            this.foreColor = Control.DefaultForeColor;
        }

        /// <summary>
        /// Initializes a new instance of the CalculatorButtonStyle class.
        /// </summary>
        /// <param name="foreColor">The forecolor to be used for the button.</param>
        /// <param name="font">The font to be used for the button.</param>
        /// <param name="flatStyle">The FlatStyle to be used for the button.</param>
        public CalculatorButtonStyle(Color foreColor, Font font, FlatStyle flatStyle)
        {
            this.ForeColor = foreColor;
            this.Font = font;
            this.FlatStyle = flatStyle;
        }

        /// <summary>
        /// Initializes a new instance of the CalculatorButtonStyle class.
        /// </summary>
        /// <param name="btnStyle">Another instance of the <see cref="CalculatorButtonStyle"/> class.</param>
        public CalculatorButtonStyle(CalculatorButtonStyle btnStyle)
        {
            this.font = btnStyle.Font;
            this.foreColor = btnStyle.ForeColor;
            this.flatStyle = btnStyle.FlatStyle;
        }

        /// <summary>
        /// Gets or sets the font to be used for displaying the Button title.
        /// </summary>
        public Font Font
        {
            get
            {
                return this.font;
            }

            set
            {
                this.font = value;
            }
        }

        /// <summary>
        /// Gets or sets the color for the button title.
        /// </summary>
        public Color ForeColor
        {
            get
            {
                return this.foreColor;
            }

            set
            {
                this.foreColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the flat style to be applied to the button.
        /// </summary>
        public FlatStyle FlatStyle
        {
            get
            {
                return this.flatStyle;
            }

            set
            {
                this.flatStyle = value;
                this.RaiseStyleChangedEvent();
            }
        }

        /// <summary>
        /// Raises the <see cref="StyleChanged"/> event.
        /// </summary>
        protected void RaiseStyleChangedEvent()
        {
            this.OnStyleChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the StyleChanged event.
        /// </summary>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnStyleChanged method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class. 
        /// <note type="note">Inheritors:  When overriding OnStyleChanged in a derived
        /// class, be sure to call the base class's OnStyleChanged method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        protected virtual void OnStyleChanged(EventArgs args)
        {
            if (this.StyleChanged != null)
                this.StyleChanged(this, args);
        }
    }

    /// <summary>
    /// Enumerates all the actions that could be assigned to a 
    /// calculator button including the digits and all arithmetic 
    /// operators.
    /// </summary>
    [Serializable()]
    public enum CalcActions
    {
        /// <summary>
        /// The digit 0.
        /// </summary>
        CalcDigit0 = 0,

        /// <summary>
        /// The digit 1.
        /// </summary>
        CalcDigit1 = 1,

        /// <summary>
        /// The digit 2.
        /// </summary>
        CalcDigit2 = 2,

        /// <summary>
        /// The digit 3.
        /// </summary>
        CalcDigit3 = 3,

        /// <summary>
        /// The digit 4.
        /// </summary>
        CalcDigit4 = 4,

        /// <summary>
        /// The digit 5.
        /// </summary>
        CalcDigit5 = 5,

        /// <summary>
        /// The digit 6.
        /// </summary>
        CalcDigit6 = 6,

        /// <summary>
        /// The digit 7.
        /// </summary>
        CalcDigit7 = 7,

        /// <summary>
        /// The digit 8.
        /// </summary>
        CalcDigit8 = 8,

        /// <summary>
        /// The digit 9.
        /// </summary>
        CalcDigit9 = 9,

        /// <summary>
        /// Dummy operator.
        /// </summary>
        CalcOperatorNone = 10,

        /// <summary>
        /// The * multiplication operator.
        /// </summary>
        CalcOperatorMultiply = 11,

        /// <summary>
        /// The + addition operator.
        /// </summary>
        CalcOperatorPlus = 12,

        /// <summary>
        /// The - subtraction operator.
        /// </summary>
        CalcOperatorMinus = 13,

        /// <summary>
        /// The / division operator.
        /// </summary>
        CalcOperatorDivide = 14,

        /// <summary>
        /// The % percent operator.
        /// </summary>
        CalcOperatorPercent = 15,

        /// <summary>
        /// The = equal to operator.
        /// </summary>
        CalcOperatorEquals = 16,

        /// <summary>
        /// The MC memory clear operator.
        /// </summary>
        CalcOperatorMemoryClear = 17,

        /// <summary>
        /// The MR memory recall operator.
        /// </summary>
        CalcOperatorMemoryRecall = 18,

        /// <summary>
        /// The MS memory store operator.
        /// </summary>
        CalcOperatorMemoryStore = 19,

        /// <summary>
        /// The M+ memory plus operator.
        /// </summary>
        CalcOperatorMemoryPlus = 20,

        /// <summary>
        /// The reciprocal operator.
        /// </summary>
        CalcOperatorReciprocal = 21,

        /// <summary>
        /// The sqrt operator.
        /// </summary>
        CalcOperatorSqrt = 22,

        /// <summary>
        /// The +/- sign operator.
        /// </summary>
        CalcOperatorSign = 23,

        /// <summary>
        /// The C Clear operator.
        /// </summary>
        CalcSpecialClear = 24,

        /// <summary>
        /// The CE Clear Entry operator.
        /// </summary>
        CalcSpecialClearEntry = 25,

        /// <summary>
        /// The . Decimal operator.
        /// </summary>
        CalcSpecialDecimal = 26,

        /// <summary>
        /// The backspace operator.
        /// </summary>
        CalcSpecialBackspace = 27
    }

    [ToolboxItem(false)]
    internal class XPButton : ButtonAdv, IThemedControl
    {
        protected bool isPushed = false;
        protected bool isFocused = false;
        protected bool isHover = false;

        protected ThemedXPButtonDrawing themedDrawing = null;

        protected bool themesEnabled = true;

        public event EventHandler ThemeChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="XPButton"/> class.
        /// </summary>
        public XPButton()
        {
            if (XPThemes.IsThemedOS)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
                this.themedDrawing = new ThemedXPButtonDrawing(this, ThemedControls.BUTTON);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x031A/*WM_THEMECHANGED*/)
            {
                if (this.themedDrawing != null)
                {
                    this.Invalidate();
                }
            }

            base.WndProc(ref m);
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
        protected virtual void OnThemeChanged(EventArgs e)
        {
            if (this.ThemeChanged != null)
            {
                this.ThemeChanged(this, e);
            }
        }

        /// <summary>
        /// Overrides the OnPaint method.
        /// </summary>
        /// <param name="e">Paint event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.UseVisualStyle)
                base.OnPaint(e);
            else
                this.OnPaint3D(e);
        }

        /// <summary>
        /// Gets or sets a value indicating whether themes are enables for the XP button.
        /// </summary>
        [
        Category("Appearance"),
        DefaultValue(true)
        ]
        public bool ThemesEnabled
        {
            get 
            {
                return this.themesEnabled; 
            }
            set
            {
                this.themesEnabled = value;
                this.OnThemeChanged(EventArgs.Empty);
                this.Invalidate();
            }
        }

        /// <summary>
        /// Raises the paint event depending on UseVisualStyle.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPaint3D(PaintEventArgs e)
        {
            int n0;
            System.Drawing.Size size1;
            size1 = this.ClientSize;
            n0 = size1.Height / 2;

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                this.themedDrawing.DrawXPButton(e.Graphics, this.ClientRectangle);
            }
            else
            {
                base.OnPaint(e);
            }
            return;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public bool Hover
        {
            get
            {
                return isHover;
            }
            set
            {
                if (this.Enabled && value != this.Hover)
                {
                    this.isHover = value;
                    this.Invalidate();
                    OnStateChanged();
                }
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public bool Pushed
        {
            get
            {
                return isPushed;
            }
            set
            {
                if (this.Enabled && value != this.Pushed)
                {
                    this.isPushed = value;
                    this.Invalidate();
                    OnStateChanged();
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual void OnStateChanged()
        {
        }

        /// <summary>
        /// Overrides the OnMouseDown method.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Pushed = true;
            }
        }

        /// <summary>
        /// Overrides the OnMouseUp method.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Pushed = false;
            }
        }

        /// <summary>
        /// Overrides the OnMouseEnter method.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Hover = true;
        }

        /// <summary>
        /// Overrides the OnMouseLeave method.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Hover = false;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool  proeprty Disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.themedDrawing != null)
                {
                    this.themedDrawing.Dispose();
                    this.themedDrawing = null;
                }
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Class derived from <see cref="System.Windows.Forms.Button"/>
    /// and customized to hold information specific to calculator
    /// functions.
    /// </summary>
    /// <remarks>
    /// The Calculator Buttons are not created directly by users and the <see cref="CalculatorControl"/>
    /// creates and uses these buttons.
    /// <para>
    /// The style for the Calculator Button objects are kept in <see cref="CalculatorButtonStyle"/> objects.
    /// </para>
    /// </remarks>
    internal class CalculatorButton : XPButton
    {
        /// <summary>
        /// The <see cref="CalculatorButtonStyle"/> object that specifies
        /// the style characteristics of the button.
        /// </summary>
        private CalculatorButtonStyle btnStyleObject;

        /// <summary>
        /// The button title.
        /// </summary>
        private string title;

        /// <summary>
        /// The calculator action associated with this button.
        /// </summary>
        private CalcActions calcAction;

        /// <summary>
        /// The calculator button parent.
        /// </summary>
        private ICalculatorButtonParent buttonParent;

        /// <summary>
        /// Raised when a Calculator Button with an associated CalcAction is pressed.
        /// </summary>
        /// <remarks>
        /// Each Calculator Button has a CalcAction associated with it.
        /// The interaction between the <see cref="CalculatorControl"/> buttons and the
        /// <see cref="CalculatorControl.Engine"/> is defined by the ButtonAction event.
        /// </remarks>
        public event CalculatorButtonEventHandler ButtonAction;

        /// <summary>
        /// Initializes a new instance of the CalculatorButton class.
        /// </summary>
        /// <param name="buttonParent">Button parent</param>
        /// <remarks>
        /// This constructor initializes the style object and the title for the 
        /// Button with empty objects. They can be set through the <see cref="CalculatorButtonStyle"/>
        /// property.
        /// </remarks>
        public CalculatorButton(ICalculatorButtonParent buttonParent)
            : base()
        {
            this.btnStyleObject = null;
            this.title = String.Empty;
            this.buttonParent = buttonParent;
            this.TabStop = false;
        }

        /// <summary>
        /// Initializes the CalculatorButton object.
        /// </summary>
        /// <param name="btnStyle">The button style object.</param>
        /// <param name="action">The action to be performed by this button.</param>
        /// <param name="title">The caption text.</param>
        /// <returns>True if the button is initialized successfully; false otherwise.</returns>
        /// <remarks>
        /// The Calculator Button also changes its style when the CalculatorButtonStyle object raises
        /// the <see cref="CalculatorButtonStyle.StyleChanged"/> event.
        /// </remarks>
        public bool Initialize(CalculatorButtonStyle btnStyle, CalcActions action, string title)
        {
            this.btnStyleObject = btnStyle;
            this.ApplyStyles();
            this.calcAction = action;
            this.title = title;
            this.Text = this.title;
            this.ButtonStyle.StyleChanged += new EventHandler(this.HandleStyleChanged);
            return true;
        }

        /// <summary>
        /// Internal helper function to apply the button styles to the button.
        /// </summary>
        private void ApplyStyles()
        {
            this.FlatStyle = this.ButtonStyle.FlatStyle;
            this.Font = this.ButtonStyle.Font;
            this.ForeColor = this.ButtonStyle.ForeColor;
        }

        /// <summary>
        /// Overrides the base class' OnClick method and raises the
        /// CalculatorButton event.
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.buttonParent != null)
                this.buttonParent.ButtonAction(this.calcAction);

            this.RaiseCalculatorButtonEvent(this.calcAction);
        }

        /// <summary>
        /// Indicates the parent that a KeyDown event has occurred.
        /// </summary>
        /// <param name="e">The KeyEventArgs object defining the KeyDown event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Tell the parent
            if (this.buttonParent != null)
                this.buttonParent.HandleChildKeyDown(e);
        }

        /// <summary>
        /// Raises the CalculatorButton event.
        /// </summary>
        /// <param name="action">The action performed by this calculator.</param>
        private void RaiseCalculatorButtonEvent(CalcActions action)
        {
            CalculatorButtonEventArgs args = new CalculatorButtonEventArgs(action);
            this.OnCalculatorButton(args);
        }

        /// <summary>
        /// Invokes the CalculatorButton event.
        /// </summary>
        /// <param name="arg">An CalculatorButtonEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnCalculatorButton method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnCalculatorButton in a derived
        /// class, be sure to call the base class's OnCalculatorButton method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        protected virtual void OnCalculatorButton(CalculatorButtonEventArgs arg)
        {
            if (this.ButtonAction != null)
                this.ButtonAction(this, arg);
        }

        /// <summary>
        /// Gets or sets the action performed by this Calculator Button.
        /// </summary>
        [
        Browsable(false)
        ]
        public CalcActions Action
        {
            get
            {
                return this.calcAction;
            }

            set
            {
                this.calcAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalculatorButtonStyle"/> object that specifies
        /// the look and feel of the button.
        /// </summary>
        /// <remarks>
        /// The Calculator Button dynamically updates its style when there is a
        /// change to the style object by handling the <see cref="CalculatorButtonStyle.StyleChanged"/>
        /// event of the <see cref="CalculatorButtonStyle"/> object.
        /// </remarks>
        public CalculatorButtonStyle ButtonStyle
        {
            get
            {
                return this.btnStyleObject;
            }

            set
            {
                this.btnStyleObject = value;
                this.ApplyStyles();
            }
        }

        /// <summary>
        /// Handles the StyleChanged event of the <see cref="CalculatorButtonStyle"/>
        /// object associated with this button.
        /// </summary>
        /// <param name="sender">The CalculatorButtonStyle object.</param>
        /// <param name="args">The event data.</param>
        private void HandleStyleChanged(object sender, EventArgs args)
        {
            CalculatorButtonStyle style = sender as CalculatorButtonStyle;
            this.ButtonStyle = style;
            this.Invalidate();
        }
    }

    /// <summary>
    /// The delegate for the <see cref="CalculatorButtonEventHandler"/>.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="arg">Calculator Button EventArgs</param>
    public delegate void CalculatorButtonEventHandler(object sender, CalculatorButtonEventArgs arg);

    /// <summary>
    /// Delegate for the <see cref="CalculatorButtonStyle.StyleChanged"/> event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="args">Calculator StyleChanged EventArgs</param>
    /// <remarks>
    /// The event handler for the <see cref="CalculatorButtonStyle.StyleChanged"/> event should bear this 
    /// signature.
    /// </remarks>
    public delegate void CalculatorStyleChangedEventHandler(object sender, CalculatorStyleChangedEventArgs args);

    /// <summary>
    /// Event data for the <see cref="CalculatorButton.ButtonAction"/> event.
    /// </summary>
    public class CalculatorButtonEventArgs
    {
        /// <summary>
        /// The <see cref="CalcActions"/> associated with the button 
        /// raising this event.
        /// </summary>
        private CalcActions action;

        /// <summary>
        /// Initializes a new instance of the CalculatorButtonEventArgs class.
        /// </summary>
        /// <param name="action">The <see cref="CalcActions"/> associated with 
        /// the button.</param>
        public CalculatorButtonEventArgs(CalcActions action)
        {
            this.action = action;
        }

        /// <summary>
        /// Gets or sets the <see cref="CalcActions"/> associated with the button.
        /// </summary>
        public CalcActions Action
        {
            get
            {
                return this.action;
            }

            set
            {
                this.action = value;
            }
        }
    }

    /// <summary>
    /// The Calculator Control uses this class to accept new values and
    /// also reports its internal value using this class.
    /// </summary>
    /// <remarks>
    /// This class encapsulates the value of the Calculator Control in the
    /// form that it maintains internally and also provides the value in the
    /// format required to the requesting class.
    /// <para>
    /// The CalculatorValue class provides methods to get the value of the
    /// Calculator Control as a string or as a double value.
    /// </para>
    /// <para>
    /// The receiving method or class can get the value in the format they require
    /// it in and format it themselves for display.
    /// </para>
    /// <para>
    /// This helps the Calculator Control to work with different kinds of classes that
    /// require the value to be formatted differently.
    /// </para>
    /// </remarks>
    [Serializable()]
    public class CalculatorValue : object
    {
        /// <summary>
        /// Gets a value for the CalculatorValue class.
        /// </summary>
        /// <remarks>
        /// This can be accessed as CalculatorValue.Empty.
        /// </remarks>
        public static CalculatorValue Empty
        {
            get
            {
                return new CalculatorValue();
            }
        }

        /// <summary>
        /// The internal string value of the Calculator Control.
        /// </summary>
        private string internalValue;
        private NumberFormatInfo numberFormatInfoObject;

        /// <summary>
        /// Gets or sets <see cref="NumberFormatInfo"/> provides the necessary globalization information for the properties that rely on these settings. 
        /// </summary>
        public NumberFormatInfo NumberFormatInfoObject
        {
            get
            {
                if (this.numberFormatInfoObject == null)
                {
                    this.numberFormatInfoObject = new NumberFormatInfo();
                }

                return this.numberFormatInfoObject;
            }

            set
            {
                if (this.numberFormatInfoObject == null)
                    this.numberFormatInfoObject = new NumberFormatInfo();

                NumberFormatInfo localObject = new NumberFormatInfo();

                if (value != null)
                {
                    this.numberFormatInfoObject.CurrencyDecimalDigits = value.CurrencyDecimalDigits;

                    this.numberFormatInfoObject.CurrencyDecimalSeparator = value.CurrencyDecimalSeparator;
                    this.numberFormatInfoObject.NumberDecimalSeparator = value.CurrencyDecimalSeparator;
                    this.numberFormatInfoObject.PercentDecimalSeparator = value.CurrencyDecimalSeparator;

                    this.numberFormatInfoObject.CurrencyGroupSeparator = value.CurrencyGroupSeparator;
                    this.numberFormatInfoObject.NumberGroupSeparator = value.CurrencyGroupSeparator;
                    this.numberFormatInfoObject.PercentGroupSeparator = value.CurrencyGroupSeparator;

                    this.numberFormatInfoObject.CurrencyGroupSizes = value.CurrencyGroupSizes;
                    this.numberFormatInfoObject.CurrencyNegativePattern = value.CurrencyNegativePattern;
                    this.numberFormatInfoObject.CurrencyPositivePattern = value.CurrencyPositivePattern;
                    this.numberFormatInfoObject.CurrencySymbol = value.CurrencySymbol;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the CalculatorValue class.
        /// </summary>
        /// <remarks>
        /// The CalculatorValue is initially set to zero. This can be changed by 
        /// invoking the <see cref="CalculatorValue.SetValue(double)"/> method.
        /// </remarks>
        public CalculatorValue()
        {
            this.internalValue = "0";
        }

        /// <summary>
        /// Initializes a new instance of the CalculatorValue class.
        /// </summary>
        /// <param name="val">The initial value that is to be set.</param>
        public CalculatorValue(string val)
        {
            this.internalValue = val;
        }

        /// <summary>
        /// Initializes a new instance of the CalculatorValue class.
        /// </summary>
        /// <param name="val">The initial value that is to be set.</param>
        public CalculatorValue(double val)
        {
            try
            {
                this.internalValue = String.Format(this.NumberFormatInfoObject, "{0:G}", val);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Returns the current value of the class object as a double type.
        /// </summary>
        /// <returns> returns double value</returns>
        /// <remarks>
        /// Invoke this method to get the value of the CalculatorValue object as
        /// a double.
        /// </remarks>
        public double ToDouble()
        {
            double val = 0;
            try
            {
                val = Double.Parse(this.internalValue, NumberFormatInfoObject);
            }
            catch
            {
            }
            return val;
        }

        /// <summary>
        /// Returns the current value of the class object as a decimal type.
        /// </summary>
        /// <returns> Returns decimal value</returns>
        /// <remarks>
        /// Invoke this method to get the value of the CalculatorValue object as
        /// a decimal.
        /// </remarks>
        public decimal ToDecimal()
        {
            decimal val = 0m;
            try
            {
                val = Decimal.Parse(this.internalValue, NumberFormatInfoObject);
            }
            catch
            {
            }
            return val;
        }

        /// <summary>
        /// Returns the current value of the class object as a string type.
        /// </summary>
        /// <returns>Returns string value</returns>
        /// <remarks>
        /// Invoke this method to get the value of the CalculatorValue object as
        /// a string.
        /// </remarks>
        public override string ToString()
        {
            return this.internalValue;
        }

        /// <summary>
        /// Sets the current value of the class object through a string.
        /// </summary>
        /// <param name="val">The value that is to be set.</param>
        /// <remarks>
        /// Invoke this method to set the value of the CalculatorValue object using
        /// a string.
        /// </remarks>
        public void SetValue(string val)
        {
            this.internalValue = val;
        }

        /// <summary>
        /// Sets the current value of the class object through a double value.
        /// </summary>
        /// <param name="val">The value that is to be set.</param>
        /// <remarks>
        /// Invoke this method to set the value of the CalculatorValue object using
        /// a double.
        /// </remarks>
        public void SetValue(double val)
        {
            try
            {
                this.internalValue = String.Format(this.NumberFormatInfoObject, "{0:G}", val);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// The event data for <see cref="CalculatorControl.LayoutTypeChanged"/> event.
    /// </summary>
    /// <remarks>
    /// This class contains the data needed for handling a change in the
    /// layout type of the Calculator Control. The Calculator Control supports
    /// two different layouts as enumerated by the <see cref="CalculatorLayoutTypes"/>
    /// enumeration.
    /// <para>
    /// When the <see cref="CalculatorControl.LayoutType"/> property is set to 
    /// a particular layout, the internal implementation of the <see cref="CalculatorControl"/>
    /// needs to know about the change and other classes that interact with the <see cref="CalculatorControl"/>
    /// will also needs to know so that they can adjust themselves to the new layout.
    /// </para>
    /// </remarks>
    public class CalculatorLayoutTypeChangedEventArgs
    {
        /// <summary>
        /// The new layout.
        /// </summary>
        private CalculatorLayoutTypes layoutType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorLayoutTypeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="layoutType">A value of type <see cref="CalculatorLayoutTypes"/>.</param>
        /// <remarks>
        /// The new layout that has been set on the <see cref="CalculatorControl"/>
        /// is the only variable that is needed for this data type. This can also be accessed
        /// through the <see cref="CalculatorLayoutTypeChangedEventArgs.LayoutType"/> property.
        /// </remarks>
        public CalculatorLayoutTypeChangedEventArgs(CalculatorLayoutTypes layoutType)
        {
            this.layoutType = layoutType;
        }

        /// <summary>
        /// Gets or sets the new layout type that has been applied to the <see cref="CalculatorControl"/>.
        /// </summary>
        /// <remarks>
        /// The CalculatorControl supports
        /// two different layouts as enumerated by the <see cref="CalculatorLayoutTypes"/>
        /// enumeration.
        /// </remarks>
        public CalculatorLayoutTypes LayoutType
        {
            get
            {
                return this.layoutType;
            }

            set
            {
                this.layoutType = value;
            }
        }
    }

    /// <summary>
    /// The Calculator Control supports the following layouts.
    /// </summary>
    [Serializable()]
    public enum CalculatorLayoutTypes
    {
        /// <summary>
        /// This layout is modeled after the Windows Standard calculator.
        /// </summary>
        WindowsStandard = 1,

        /// <summary>
        /// This layout is modeled after the standard financial layout.
        /// </summary>
        Financial
    }

    /// <summary>
    /// Provides data for a <see cref="CalculatorButtonStyle.StyleChanged"/> event.
    /// </summary>
    /// <remarks>
    /// The <see cref="CalculatorControl"/> applies the FlatStyle property to
    /// all the Calculator Buttons. This event argument specifies the FlatStyle
    /// currently applied to the CalculatorControl so that the buttons handling 
    /// the <see cref="CalculatorButtonStyle.StyleChanged"/> event will be able to apply the right
    /// FlatStyle.
    /// </remarks>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class CalculatorStyleChangedEventArgs
    {
        /// <summary>
        /// The changed flat style.
        /// </summary>
        private FlatStyle flatStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorStyleChangedEventArgs"/> class.
        /// </summary>
        /// <param name="flatStyle">The changed flat style</param>
        /// <remarks>
        /// The FlatStyle property is passed in as a parameter.
        /// </remarks>
        public CalculatorStyleChangedEventArgs(FlatStyle flatStyle)
        {
            this.flatStyle = flatStyle;
        }

        /// <summary>
        /// Gets or sets the changed Flat style.
        /// </summary>
        public FlatStyle FlatStyle
        {
            get
            {
                return this.flatStyle;
            }

            set
            {
                this.flatStyle = value;
            }
        }
    }

    /// <summary>
    /// Delegate for the LayoutTypeChanged event.
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="arg">Calculator Layout TypeChanged EventArgs</param>
    /// <remarks>
    /// Refer to the <see cref="CalculatorControl.LayoutTypeChanged"/> event for more information.
    /// </remarks>
    public delegate void CalculatorLayoutTypeChangedEventHandler(object sender, CalculatorLayoutTypeChangedEventArgs arg);

    /// <summary>
    /// The event data for CalculatorControl.ValueCalculated event.
    /// </summary>
    /// <remarks>
    /// This event is raised whenever there is a change in the internal value of the
    /// <see cref="CalculatorControl"/>. This event data class contains the necessary 
    /// pieces of information for the handlers to get the new value of the Calculator Control.
    /// <para>
    /// This class also has members that provide information about the current error state
    /// of the Calculator Control.
    /// </para>
    /// <para>
    /// The Memory value of the calculator can also be got from this class.
    /// </para>
    /// </remarks>
    public class CalculatorValueCalculatedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The internal value.
        /// </summary>
        private CalculatorValue internalValue;

        /// <summary>
        /// The error condition.
        /// </summary>
        private bool errorCondition;

        /// <summary>
        /// The feedback message.
        /// </summary>
        private string message;

        /// <summary>
        /// The memory value.
        /// </summary>
        private double memoryValue;

        /// <summary>
        /// The last action performed.
        /// </summary>
        private CalcActions lastAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorValueCalculatedEventArgs"/> class.
        /// </summary>
        /// <param name="val">The Value of the CalculatorControl.</param>
        /// <param name="errorCondition">The error condition.</param>
        /// <param name="message">The feedback messsage.</param>
        /// <param name="lastAction">The last action that was performed.</param>
        /// <remarks>
        /// This constructor sets all the required values for the <see cref="CalculatorValueCalculatedEventArgs"/>.
        /// </remarks>
        public CalculatorValueCalculatedEventArgs(CalculatorValue val, bool errorCondition, string message, CalcActions lastAction)
        {
            this.internalValue = val;
            this.errorCondition = errorCondition;
            this.message = message;
            this.lastAction = lastAction;
        }

        /// <summary>
        /// Gets or sets the <see cref="CalculatorValue"/> object that contains the value of the Calculator Control.
        /// </summary>
        /// <remarks>
        /// This value has to be read in conjunction with the error condition of the Calculator
        /// Control and the error message. The memory value can also be accessed through this class.
        /// </remarks>
        public CalculatorValue Value
        {
            get
            {
                return this.internalValue;
            }

            set
            {
                this.internalValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the last action that was performed.
        /// </summary>
        /// <remarks>
        /// This value has to be read in conjunction with the error condition of the Calculator
        /// Control and the error message. The memory value can also be accessed through this class.
        /// </remarks>
        public CalcActions LastAction
        {
            get
            {
                return this.lastAction;
            }

            set
            {
                this.lastAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the memory value of the Calculator Control.
        /// </summary>
        /// <remarks>
        /// This value is to be used along with the <see cref="Value"/> property of 
        /// the CalculatorControl as well as the error condition if any.
        /// </remarks>
        public double MemoryValue
        {
            get
            {
                return this.memoryValue;
            }

            set
            {
                this.memoryValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom error message when in error mode.
        /// </summary>
        /// <remarks>
        /// This message has to be used in conjunction with the <see cref="ErrorCondition"/>
        /// value set by the Calculator Control.
        /// </remarks>
        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.message = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error condition of the Calculator Control if any.
        /// </summary>
        /// <remarks>
        /// This message has to be used in conjunction with the <see cref="Message"/>
        /// value set by the CalculatorControl.
        /// </remarks>
        public bool ErrorCondition
        {
            get
            {
                return this.errorCondition;
            }

            set
            {
                this.errorCondition = value;
            }
        }
    }

    /// <summary>
    /// The delegate for the <see cref="CalculatorControl.ValueCalculated"/> event.
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="arg">Calculator ValueCalculated EventArgs</param>
    /// <remarks>
    /// Refer to the <see cref="CalculatorControl.ValueCalculated"/> event for more information.
    /// </remarks>
    public delegate void CalculatorValueCalculatedEventHandler(object sender, CalculatorValueCalculatedEventArgs arg);

    /// <summary>
    /// XP Button drawing class.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ThemedXPButtonDrawing : ThemedControlDrawing
    {
        private Control control;
        public ThemedXPButtonDrawing(Control control, string classList) :
            base(classList, control)
        {
            this.control = control;
        }

        /// <summary>
        /// Draws the XP Button.
        /// </summary>
        /// <param name="g">The graphics object.</param>
        /// <param name="rect">The rectangle.</param>
        public void DrawXPButton(Graphics g, Rectangle rect)
        {
            // Get part id
            int part = ThemeParts.BP_PUSHBUTTON;
            int state = 0;

            XPButton btnControl = this.control as XPButton;

            state = ThemeStates.PBS_NORMAL; // Normal
            if (btnControl.Hover == true) state = ThemeStates.PBS_HOT; // Hot
            if (btnControl.Pushed == true) state = ThemeStates.PBS_PRESSED; // Pressed
            if (btnControl.Enabled == false) state = ThemeStates.PBS_DISABLED; // Disabled

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using (Brush backBrush = new SolidBrush(btnControl.BackColor))
            {
                g.FillRectangle(backBrush, rect);
            }
            this.DrawThemeBackground(g, part, state, rect);
            using (Brush brush = new SolidBrush(btnControl.ForeColor))
            {
                g.DrawString(btnControl.Text, btnControl.Font, brush, rect, sf);
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
        /// </summary>
        /// <param name="disposing"> Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.control = null;
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// TextBox derived class that draws the display text for the <see cref="CalculatorControl"/>. 
    /// </summary>
    [
    ToolboxItem(false),
    DesignTimeVisible(false)
    ]
    [Syncfusion.Documentation.DocumentationExclude()]
    public class CalculatorTextBox : TextBox
    {
        private ICalculatorButtonParent calcParent;

        /// <summary>
        /// Initializes a new instance of the CalculatorTextBox class.
        /// </summary>
        /// <param name="calcParent">The calculator parent.</param>
        public CalculatorTextBox(ICalculatorButtonParent calcParent)
            : base()
        {
            // This call is required by the Windows.Forms Form Designer. 
            this.calcParent = calcParent;
            this.TabStop = true;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.CreateControl"></see> event.
        /// </summary>
        /// <remarks > Overridden. Input focus is disabled </remarks>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.SetStyle(ControlStyles.Selectable, false);
        }

        /// <summary>
        /// Indicates the parent that a KeyDown event has occurred.
        /// </summary>
        /// <param name="e">The KeyEventArgs object defining the KeyDown event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Tell the parent
            if (this.calcParent != null)
                this.calcParent.HandleChildKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.GotFocus"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            // base.OnGotFocus(e);
            this.Parent.Focus();
        }

        /// <summary>
        /// Overrides the WndProc to deactivate mouse click.
        /// </summary>
        /// <param name="m">The Message.</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_MOUSEACTIVATE /*0x0021*/
                || m.Msg == NativeMethods.WM_LBUTTONDOWN/*0x0201*/
                || m.Msg == NativeMethods.WM_RBUTTONDOWN/*0x0204*/
                || m.Msg == NativeMethods.WM_MBUTTONDOWN/*0x0207*/)
            {
                m.Result = (IntPtr)3;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
