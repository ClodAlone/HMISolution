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
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    #region CURRENCYEDIT
    /// <summary>
    /// CurrencyEdit class encapsulates a <see cref="CurrencyTextBox"/> control and
    /// adds the ability to drop down a <see cref="PopupCalculator"/>.
    /// </summary>
    /// <remarks>
    /// The CurrencyEdit class derives from <see cref="ButtonEdit"/> and embeds
    /// a CurrencyTextBox class.
    /// <para>
    /// The embedded CurrencyTextBox class is exposed through the <see cref="CurrencyEdit.TextBox"/>
    /// property.
    /// </para>
    /// <para>
    /// The CurrencyEdit has full support for the Windows Forms designer and you
    /// can just drag and drop it onto the form and set the properties.
    /// </para>
    /// <para>
    /// The calculator button's visibility is controlled by the <see cref="ShowCalculator"/>
    /// property.
    /// </para>
    /// </remarks>
    /// <example>
    /// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\currencydemo\CS\MainForm.cs" name="Currency InitializeComponent - Currency Edit" lang="CS">
    /// <code lang="C#">
    /// // CurrencyEdit control
    /// this.currencyEdit1 = new Syncfusion.Windows.Forms.Tools.CurrencyEdit();
    /// //
    /// // currencyEdit1
    /// //
    /// this.currencyEdit1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
    /// this.currencyEdit1.Location = new System.Drawing.Point(16, 80);
    /// this.currencyEdit1.Name = "currencyEdit1";
    /// this.currencyEdit1.SelectionLength = 0;
    /// this.currencyEdit1.SelectionStart = 5;
    /// this.currencyEdit1.ShowCalculator = true;
    /// this.currencyEdit1.ShowTextBox = true;
    /// this.currencyEdit1.Size = new System.Drawing.Size(304, 22);
    /// this.currencyEdit1.TabIndex = 1;
    /// this.currencyEdit1.Text = "$1.00";
    /// this.currencyEdit1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
    /// // Add the CurrencyEdit control to the form
    /// this.Controls.Add(this.currencyEdit1);
    /// </code></coderef>
    /// <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\currencydemo\VB\MainForm.vb" name="Currency InitializeComponent - Currency Edit" lang="VB"><code lang="VB">
    /// ' CurrencyEdit control
    /// Me.currencyEdit1 = New Syncfusion.Windows.Forms.Tools.CurrencyEdit
    /// '
    /// ' currencyEdit1
    /// '
    /// Me.currencyEdit1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
    /// Me.currencyEdit1.Location = New System.Drawing.Point(16, 80)
    /// Me.currencyEdit1.Name = "currencyEdit1"
    /// Me.currencyEdit1.SelectionLength = 0
    /// Me.currencyEdit1.SelectionStart = 5
    /// Me.currencyEdit1.ShowCalculator = true
    /// Me.currencyEdit1.ShowTextBox = true
    /// Me.currencyEdit1.Size = New System.Drawing.Size(304, 22)
    /// Me.currencyEdit1.TabIndex = 1
    /// Me.currencyEdit1.Text = "$1.00"
    /// Me.currencyEdit1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
    /// ' Add the CurrencyEdit control to the form
    /// Me.Controls.Add(Me.currencyEdit1)
    /// </code>
    /// </coderef></example>
    [
    ToolboxItem(true),
    DefaultProperty(@"TextBox"),
    Designer(typeof(Syncfusion.Windows.Forms.Tools.Design.CurrencyEditDesigner), typeof(System.ComponentModel.Design.IDesigner)),
    ToolboxBitmap(typeof(CurrencyEdit), "ToolboxIcons.CurrencyEdit.bmp"),
    Description("Represents a Currency TextBox control and adds the ability to drop down a PopupCalculator.")
    ]
    public class CurrencyEdit : ButtonEdit, IVisualStyle
    {
        #region FIELDS

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// The calculator button.
        /// </summary>
        private ButtonEditChildButton btnCalculator;

        /// <summary>
        /// The CalculatorControl popup.
        /// </summary>
        private PopupCalculator calcPopup;

        /// <summary>
        /// Indicates whether the calculator button is to be displayed.
        /// </summary>
        private bool showCalculator;

        /// <summary>
        /// Closes the calculator when this action occurs.
        /// </summary>
        private CalcActions calcCloseAction;

        /// <summary>
        /// Indicates whether the current value of the CalculatorControl
        /// should be transferred to the CurrencyTextBox.
        /// </summary>
        private bool transferFromCalculator = true;

        /// <summary>
        /// Indicates whether the current value of the CurrencyTextBox should be
        /// transferred to the CalculatorControl.
        /// </summary>
        private bool transferToCalculator = true;

        /// <summary>
        /// The LayoutType for the drop down calculator.
        /// </summary>
        private CalculatorLayoutTypes calculatorLayoutType;

        /// <summary>
        /// To prevent serialization of the child buttons.
        /// </summary>
        private ArrayList childButtons;

        /// <summary>
        /// Indicates whether calculator must be shown and got focus.
        /// </summary>
        private bool beforeCalcShow;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        #endregion

        #region INITIALIZATION

        /// <summary>
        /// Initializes a new instance of the CurrencyEdit class.
        /// </summary>
        /// <remarks>
        /// The CurrencyEdit class also creates the controls that it hosts such
        /// as the <see cref="CurrencyTextBox"/> control and the <see cref="PopupCalculator"/>
        /// control.
        /// </remarks>
        public CurrencyEdit()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(CurrencyEdit));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            InitializeComponent();

            this.showCalculator = true;
            this.calculatorLayoutType = CalculatorLayoutTypes.Financial;
            this.btnCalculator = new ButtonEditChildButton();
            this.calcPopup = new PopupCalculator();

            this.calcPopup.PopupCalculatorAlignment = CalculatorPopupAlignment.Right;
            this.calcPopup.ParentControl = this;

            this.SuspendLayout();

            if (this.Buttons.Contains(btnCalculator) == false)
            {
                this.Buttons.Add(btnCalculator);
            }
            this.btnCalculator.ButtonAlign = ButtonAlignment.Right;
            this.btnCalculator.ButtonType = ButtonTypes.Calculator;
            this.btnCalculator.Image = GetImage("Calculator.bmp");
            this.calcPopup.Size = new System.Drawing.Size(158, 200);
            this.calcPopup.BorderStyle = Border3DStyle.Raised;
            this.calcPopup.Value.SetValue(0);
            CTRLSIZE = this.Size;

            this.calcCloseAction = CalcActions.CalcOperatorEquals;

            this.btnCalculator.Click += new EventHandler(this.HandleCalculatorBtnClick);
            this.CalcPopup.BeforeCalculatorPopupDisplay += new CancelEventHandler(this.HandleBeforeCalculatorDisplayEvent);
            this.CalcPopup.Closing += new PopupCalculatorClosingEventHandler(this.HandleCalculatorPopupClosingEvent);

            this.RightToLeftChanged += new EventHandler(CurrencyEdit_RightToLeftChanged);
            this.ResumeLayout();
        }

        /// <summary>
        /// Initializes the layout for the <see cref="CurrencyEdit"/> control.
        /// Overrides InitializeLayout in <see cref="ButtonEdit"/>.
        /// </summary>
        public void InitializeLayout()
        {
            if (this.btnCalculator != null)
            {
                if (this.showCalculator == false && this.Buttons.Contains(this.btnCalculator) == true)
                {
                    this.Buttons.Remove(this.btnCalculator);
                }
                else if (this.showCalculator == true && this.Buttons.Contains(this.btnCalculator) == false)
                {
                    SuspendLayout();

                    this.Buttons.Add(this.btnCalculator);

                    Layout();

                    ResumeLayout(false);
                }
            }
        }

        protected override void OnEndInit()
        {
            this.btnCalculator.UseVisualStyle = this.UseVisualStyle;
            this.btnCalculator.SetAppearance(this.ButtonStyle);
            this.CalcPopup.UseVisualStyle = this.UseVisualStyle;
            this.CalcPopup.ButtonStyle = this.ButtonStyle;
            base.OnEndInit();
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

        #endregion

        #region CALCULATOR

        /// <summary>
        /// Event raised before the calculator popup is displayed.
        /// </summary>
        /// <remarks>
        /// This event is a <see cref="CancelEventHandler"/> type
        /// event. You can cancel the PopupCalculator from being displayed
        /// by handling this event.
        /// </remarks>
        [Description("Event raised before the calculator popup is displayed.")]
        public event CancelEventHandler CalculatorShowing;

        /// <summary>
        /// Event raised by a popup calculator when closing after the
        /// specified button was clicked.
        /// </summary>
        /// <remarks>
        /// By default the CalculatorControl will raise this event when
        /// the specified button is clicked. This event can be canceled
        /// by a handler and the PopupCalculator will not be closed.
        /// </remarks>
        [Description("Event raised by a popup calculator when closing after the specified button was clicked.")]
        public event PopupCalculatorClosingEventHandler CalculatorClosing;

        /// <summary>
        /// Gets the Calculator Button.
        /// </summary>
        /// <remarks>
        /// The Calculator Button can be used to change the appearance of the
        /// button.
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Appearance")
        ]
        public ButtonEditChildButton CalculatorButton
        {
            get
            {
                return this.btnCalculator;
            }
        }

        /// <summary>
        /// Gets or sets the action that will close the popup calculator.
        /// </summary>
        /// <remarks>
        /// This is set to <see cref="CalcActions.CalcOperatorEquals"/> by default.
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("The popup calculation will close when this action is encountered."),
        DefaultValue(typeof(CalcActions), "CalcOperatorEquals")
        ]
        public CalcActions CloseAction
        {
            get
            {
                return this.calcCloseAction;
            }

            set
            {
                this.calcCloseAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout type for the Calculator Control.
        /// </summary>
        /// <remarks>
        /// The Calculator Control supports different layouts for
        /// Financial and Windows Standard type calculators.
        /// <para>
        /// The different layouts change the layout of the buttons
        /// and their appearance.
        /// </para>
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(CalculatorLayoutTypes.Financial),
        Category("Appearance"),
        Description("Gets or sets the layout type for the Calculator Control.")
        ]
        public CalculatorLayoutTypes CalculatorLayoutType
        {
            get
            {
                return this.calculatorLayoutType;
            }

            set
            {
                this.calculatorLayoutType = value;
                if (this.calculatorLayoutType == CalculatorLayoutTypes.Financial)
                {
                    this.calcPopup.Size = new System.Drawing.Size(158, 200);
                }
                else
                {
                    this.calcPopup.Size = new System.Drawing.Size(200, 300);
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the Popup Calculator
        /// with respect to the ButtonEdit control.
        /// </summary>
        /// <remarks>
        /// Please refer to the <see cref="CalculatorPopupAlignment"/> type
        /// for the types of alignments supported.
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance"),
        DefaultValue(typeof(CalculatorPopupAlignment), "Right"),
        Description("This property specifies the alignment of the Popup Calculator with respect to the ButtonEdit control.")
        ]
        public CalculatorPopupAlignment PopupCalculatorAlignment
        {
            get
            {
                return this.calcPopup.PopupCalculatorAlignment;
            }

            set
            {
                this.calcPopup.PopupCalculatorAlignment = value;
            }
        }

        /// <summary>
        /// Draws the  background of control depending on ReadOnly property .
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> within which to draw.</param>
        /// <remarks>
        ///  Draws the background with ReadOnlyBackColor when
        /// control is set to ReadOnly.Else,with default BackColor. 
        /// </remarks>
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            using(Brush brush = new SolidBrush(
                (null != this.TextBox && this.TextBox.ReadOnly) ? this.TextBox.ReadOnlyBackColor : this.BackColor))

            g.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Gets or sets the alignment of the Popup Calculator
        /// with respect to the ButtonEdit control.
        /// </summary>
        /// <remarks>
        /// Please refer to the <see cref="CalculatorPopupAlignment"/> type
        /// for the types of alignments supported.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public PopupCalculator CalcPopup
        {
            get
            {
                return this.calcPopup;
            }

            set
            {
                this.calcPopup = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the calculator button is to be displayed.
        /// If the Calculator is not to be used, you can use the
        /// <see cref="CurrencyTextBox"/> class that is used internally
        /// by the <see cref="CurrencyEdit"/> class.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DefaultValue(true),
        Description("Indicates whether the calculator button is to be displayed.")
        ]
        public bool ShowCalculator
        {
            get
            {
                return this.showCalculator;
            }

            set
            {
                if (this.showCalculator != value)
                {
                    this.showCalculator = value;
                    this.InitializeLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to transfer the values from the edit control to the calculator.
        /// </summary>
        /// <remarks>
        /// Indicates whether the current currency value of the CurrencyTextBox
        /// is to be transferred to the CalculatorControl when the Calculator
        /// is dropped down.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(true),
        Category("Behavior"),
        Description("Indicates whether to transfer the values from the edit control to the calculator.")
        ]
        public bool TransferToCalculator
        {
            get
            {
                return this.transferToCalculator;
            }

            set
            {
                this.transferToCalculator = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to transfer the calculated value to the edit control.
        /// </summary>
        /// <remarks>
        /// Indicates whether the calculated value is to be transferred to
        /// the CurrencyTextBox when the popup calculator is closed.
        /// </remarks>
        [
        Browsable(true),
        DefaultValue(true),
        Category("Behavior"),
        Description("Indicates whether to transfer the calculated value to the edit control.")
        ]
        public bool TransferFromCalculator
        {
            get
            {
                return this.transferFromCalculator;
            }

            set
            {
                this.transferFromCalculator = value;
            }
        }

        /// <summary>
        /// This method is the handler for the calculator button's click event.
        /// </summary>
        /// <param name="sender">The calculator event.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// Displays the PopupCalculator when the calculator button is clicked.
        /// </remarks>
        private void HandleCalculatorBtnClick(object sender, System.EventArgs e)
        {
            DisplayCalculator();
        }

        private void DisplayCalculator()
        {
            if (this.DesignMode == false)
            {
                if (this.CalcPopup.Calculator.Culture != this.TextBox.Culture)
                    this.CalcPopup.Calculator.Culture = this.TextBox.Culture;
                this.CalcPopup.CloseAction = this.CloseAction;
                this.calcPopup.DisplayCalculator();
                beforeCalcShow = true;
            }
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            if (beforeCalcShow && this.CalcPopup.Calculator != null)
            {
                this.CalcPopup.Calculator.Focus();
                beforeCalcShow = false;
            }
        }

        private void TextBox_ReadOnlyChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void HandleBeforeCalculatorDisplayEvent(object sender, CancelEventArgs args)
        {
            // Set the specific values we want to set
            this.CalcPopup.LayoutType = this.CalculatorLayoutType;
            if (this.TransferToCalculator == true)
            {
                decimal decimalValue = this.TextBox.DecimalValue;
                double dValue = Convert.ToDouble(decimalValue);

                CalculatorValue calcValue = new CalculatorValue();
                calcValue.NumberFormatInfoObject = this.CalcPopup.Calculator.NumberFormatInfoObject;
                calcValue.SetValue(dValue);
                this.CalcPopup.Value = calcValue;
            }

            // Raise the event
            args.Cancel = this.RaiseCalculatorShowingEvent();
        }

        /// <summary>
        /// Handles the PopupCalculator's CalculatorClosing event.
        /// </summary>
        /// <param name="sender">The popup calculator.</param>
        /// <param name="args">The event data.</param>
        private void HandleCalculatorPopupClosingEvent(object sender, CalculatorClosingEventArgs args)
        {
            if (this.TransferFromCalculator && args.CloseType == CalculatorCloseType.Done)
            {
                double finalValue = args.FinalValue.ToDouble();
                decimal dValue = Convert.ToDecimal(finalValue);
                this.TextBox.DecimalValue = dValue;
            }

            this.RaiseCalculatorClosingEvent(args.CloseType);
        }

        /// <summary>
        /// This method raises the <see cref="CalculatorShowing"/> event.
        /// </summary>
        /// <returns>True if the Calculator display has been cancelled by a event handler; false otherwise.</returns>
        /// <remarks>
        /// This event can be handled if you want to make some changes to the Calculator Control
        /// before it is displayed.
        /// </remarks>
        protected bool RaiseCalculatorShowingEvent()
        {
            bool bCancel = false;
            CancelEventArgs arg = new CancelEventArgs(bCancel);
            return this.OnCalculatorShowing(arg);
        }

        /// <summary>
        /// Raises the <see cref="CalculatorClosing"/> event. This event is
        /// raised by the calculator control before the popup calculator closes
        /// when the '=' button is clicked. This event will not be raised when
        /// the popup calculator control closes when it loses focus.
        /// </summary>
        /// <param name="closeType">Calculator close type</param>
        protected void RaiseCalculatorClosingEvent(CalculatorCloseType closeType)
        {
            CalculatorClosingEventArgs args = new CalculatorClosingEventArgs(closeType, this.CalcPopup.Value);
            this.OnCalculatorClosing(args);
        }

        /// <summary>
        /// Invokes the CalculatorClosing event.
        /// </summary>
        /// <param name="args">A CancelEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnPopupCalculatorClosing method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnStyleChanged in a derived
        /// class, be sure to call the base class's OnStyleChanged method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        protected virtual void OnCalculatorClosing(CalculatorClosingEventArgs args)
        {
            if (this.CalculatorClosing != null)
            {
                this.CalculatorClosing(this, args);
            }
        }

        /// <summary>
        /// Invokes the BeforeCalculatorPopupDisplay event.
        /// </summary>
        /// <param name="args">A BeforeCalculatorPopupDisplayEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnBeforeCalculatorPopupDisplay method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnBeforeCalculatorPopupDisplay in a derived
        /// class, be sure to call the base class's OnBeforeCalculatorPopupDisplay method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        /// <returns>True if hte Calculator is showing.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool OnCalculatorShowing(CancelEventArgs args)
        {
            if (this.CalculatorShowing != null)
                this.CalculatorShowing(this, args);

            return args.Cancel;
        }

        /// <summary>
        /// Handles the KeyDown event of the embedded TextBox and drops down the
        /// Calculator Control.
        /// </summary>
        /// <param name="sender">The TextBox.</param>
        /// <param name="e">The event args.</param>
        protected void HandleTextBoxKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.Alt == true && e.KeyCode == Keys.Down)
            {
                this.DisplayCalculator();
            }
        }

        #endregion

        #region BUTTONEDIT
        /// <summary>
        /// Gets the collection of Buttons that make up this ButtonEdit control.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        MergableProperty(false),
        Category(@"Behavior"),
        Localizable(true),
        Description(@"The collection of Buttons that make up this ButtonEdit control."),
        Browsable(false)
        ]
        public new ButtonEditChildButtonCollection Buttons
        {
            get
            {
                return base.Buttons;
            }
        }

        /// <summary>
        /// Indicates whether the TextBox is to be displayed.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance")
        ]
        public override bool ShowTextBox
        {
            get
            {
                bool val = true;
                if (this.TextBox != null)
                {
                    val = this.TextBox.Visible;
                }

                return val;
            }
        }

        /// <summary>
        /// Sets the CurrencyTextBox as the TextBox for this control.
        /// </summary>
        /// <returns>A CurrencyTextBox object.</returns>
        /// <remarks>
        /// This override of the CreateTextBox method allows us to change the
        /// default TextBox object that will be created with a <see cref="CurrencyTextBox"/>
        /// object.
        /// </remarks>
        protected override TextBoxExt CreateTextBox()
        {
            TextBoxExt txtBox = new CurrencyTextBox() as TextBoxExt;
            txtBox.BorderStyle = BorderStyle.None;
            return txtBox;
        }

        /// <summary>
        /// Gets the CurrencyTextBox control that defines the Currency behavior for this control.
        /// </summary>
        /// <remarks>
        /// See the <see cref="CurrencyTextBox"/> control for more information.
        /// </remarks>
        [
        Browsable(true),
        Description("The CurrencyTextBox control that defines the Currency behavior for this control."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public new CurrencyTextBox TextBox
        {
            get
            {
                return (CurrencyTextBox)base.TextBox;
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            ResetChildButtons();

            InitializeLayout();

            base.OnLayout(levent);
        }

        protected internal override void AttachTextBox()
        {
            base.AttachTextBox();

            this.TextBox.ValidationError += new ValidationErrorEventHandler(this.HandleTextBoxValidationErrorEvent);
            this.TextBox.KeyDown += new KeyEventHandler(this.HandleTextBoxKeyDownEvent);
            this.TextBox.DecimalValueChanged += new EventHandler(this.HandleTextBoxDecimalValueChanged);
            this.TextBox.GotFocus += new EventHandler(TextBox_GotFocus);
            this.TextBox.ReadOnlyChanged += new EventHandler(TextBox_ReadOnlyChanged);
        }

        protected internal override void DetachTextBox()
        {
            if (this.TextBox != null)
            {
                this.TextBox.ValidationError -= new ValidationErrorEventHandler(this.HandleTextBoxValidationErrorEvent);
                this.TextBox.KeyDown -= new KeyEventHandler(this.HandleTextBoxKeyDownEvent);
                this.TextBox.DecimalValueChanged -= new EventHandler(this.HandleTextBoxDecimalValueChanged);
                this.TextBox.GotFocus -= new EventHandler(TextBox_GotFocus);
                this.TextBox.ReadOnlyChanged -= new EventHandler(TextBox_ReadOnlyChanged);

                base.DetachTextBox();
            }
        }

        private void CurrencyEdit_RightToLeftChanged(object sender, EventArgs e)
        {
            this.TextBox.RightToLeft = RightToLeft;
        }

        #endregion

        #region DATA

        /// <summary>
        /// Gets or sets the decimal value of the control. This will be formatted and
        /// displayed.
        /// </summary>
        [
        Browsable(true),
        DefaultValue(0),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Data"),
        Description("The decimal value of the currency control."),
        RefreshProperties(RefreshProperties.Repaint),
        ]
        public decimal DecimalValue
        {
            get
            {
                decimal val = 0;
                if (this.TextBox != null)
                {
                    val = this.TextBox.DecimalValue;
                }

                return val;
            }

            set
            {
                if (this.TextBox != null)
                {
                    this.TextBox.DecimalValue = value;
                }
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
            this.CalcPopup.EnableTouchMode = _touchMode;
            this.CalcPopup.ApplyScaleToControl(scaleFactor);
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
        /// <summary>
        /// Occurs when the <see cref="DecimalValue"/> property is changed.
        /// </summary>
        [
        Category(@"Property Changed"),
        Description(@"Occurs when the DecimalValue property is changed.")
        ]
        public event EventHandler DecimalValueChanged;

        /// <summary>
        /// Raises the <see cref="CurrencyTextBox.DecimalValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnDecimalValueChanged(EventArgs e)
        {
            if (DecimalValueChanged != null)
                DecimalValueChanged(this, e);
        }

        private void HandleTextBoxDecimalValueChanged(object sender, EventArgs e)
        {
            this.OnDecimalValueChanged(e);
        }

        /// <summary>
        /// Overrides the Text property.
        /// </summary>
        /// <remarks>
        /// The Text property is not persisted.
        /// </remarks>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

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
                if (style != null)
                   this.UseVisualStyle = true;
                else
                    this.UseVisualStyle = false;
                if (UseVisualStyle)
                {
                    switch (value)
                    {
                        case "Office2007Black":
                            ButtonStyle = ButtonAppearance.Office2007;
                            this.btnCalculator.Office2007ColorScheme = Office2007Theme.Black;
                            break;
                        case "Office2007Blue":
                            ButtonStyle = ButtonAppearance.Office2007;
                            this.btnCalculator.Office2007ColorScheme = Office2007Theme.Blue;
                            break;
                        case "Managed":
                            ButtonStyle = ButtonAppearance.Office2007;
                            this.btnCalculator.Office2007ColorScheme = Office2007Theme.Managed;
                            break;
                        case "Office2007Silver":
                            ButtonStyle = ButtonAppearance.Office2007;
                            this.btnCalculator.Office2007ColorScheme = Office2007Theme.Silver;
                            break;
                        case "Metro":
                            ButtonStyle = ButtonAppearance.Metro;
                            break;
                        case "Office2000":
                            ButtonStyle = ButtonAppearance.Office2000;
                            break;
                        case "OfficeXP":
                            ButtonStyle = ButtonAppearance.OfficeXP;
                            break;
                        case "Office2003":
                            ButtonStyle = ButtonAppearance.Office2003;
                            break;
                        case "Classic":
                            ButtonStyle = ButtonAppearance.Classic;
                            break;
                        case "None":
                            ButtonStyle = ButtonAppearance.None;
                            break;
                    }                  
                }
            }
        }
        #endregion

        #region INTERNAL OPERATIONS

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.RightToLeftChanged -= new EventHandler(CurrencyEdit_RightToLeftChanged);

                if (components != null)
                    components.Dispose();

                if (this.btnCalculator != null)
                {
                    this.btnCalculator.Click -= new EventHandler(this.HandleCalculatorBtnClick);
                    this.btnCalculator.Dispose();
                    this.btnCalculator = null;
                }

                if (calcPopup != null)
                {
                    this.CalcPopup.BeforeCalculatorPopupDisplay -= new CancelEventHandler(this.HandleBeforeCalculatorDisplayEvent);
                    this.CalcPopup.Closing -= new PopupCalculatorClosingEventHandler(this.HandleCalculatorPopupClosingEvent);
                    calcPopup.ParentControl = null;
                    calcPopup.Dispose();
                    calcPopup = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region VALIDATION
        /// <summary>
        /// Raised when an unacceptable character is encountered as input.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This event can be handled and you can do your processing based
        /// on the information provided. The ValidationErrorEventArgs object
        /// will provide the invalid text that was input and also the position
        /// within that text where the error occurred.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///             Console.WriteLine("ValidationError in currencyTextBox1 InvalidText" + e.InvalidText);
        ///             Console.WriteLine("ValidationError in currencyTextBox1 StartPosition" + e.StartPosition );</code>
        ///             <coderef file="c:\syncfusion\essential suite\tools\samples\quick start\currencydemo\VB\MainForm.vb" name="Currency ValidationError" lang="VB"><code lang="VB">
        ///             Console.WriteLine(("ValidationError in currencyTextBox1 InvalidText" + e.InvalidText))
        ///             Console.WriteLine(("ValidationError in currencyTextBox1 StartPosition" + e.StartPosition))</code></coderef>
        /// </example>
        [Description("Raised when an unacceptable character is encountered as input.")]
        public event ValidationErrorEventHandler ValidationError;

        /// <summary>
        /// Invokes the ValidationError event.
        /// </summary>
        /// <param name="args">A ValidationErrorEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnValidationError method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnValidationError in a derived
        /// class, be sure to call the base class's OnValidationError method so that
        /// registered delegates receive the event.</note>
        /// </remarks>
        protected virtual void OnValidationError(ValidationErrorArgs args)
        {
            if (this.ValidationError != null)
                this.ValidationError(this, args);
        }

        /// <summary>
        /// HandleTextBox ValidationErrorEvent
        /// </summary>
        /// <param name="sender"> Sender object</param>
        /// <param name="args">ValidationErrorArgs that contains the event data.</param>
        private void HandleTextBoxValidationErrorEvent(object sender, ValidationErrorArgs args)
        {
            this.OnValidationError(args);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Validating"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="System.ComponentModel.CancelEventArgs"></see> that contains the event data.</param>
        /// <remarks > Overridden</remarks>
        protected override void OnValidating(CancelEventArgs e)
        {
            if (Text.Length == 0)
            {
                this.DecimalValue = 0m;
            }
            else
            {
                try
                {
                    this.DecimalValue = this.DecimalValue;
                }
                catch
                {
                }
            }
            base.OnValidating(e);
        }

        #endregion

        #region DESIGNER SERIALIZATION
        /// <summary>
        /// Stores the child buttons removed by the designer in a separate arraylist to prevent serialization
        /// </summary>
        /// <param name="childButtons">Arraylist containing the child buttons</param>
        [Browsable(false),
        Syncfusion.Documentation.DocumentationExclude()]
        public void ChildButtonsRemovedByDesigner(ArrayList childButtons)
        {
            this.childButtons = childButtons;
            this.Invalidate(false);
        }

        private void ResetChildButtons()
        {
            if (childButtons != null)
            {
                if (childButtons.Count > 0)
                {
                    foreach (Control childButton in childButtons)
                    {
                        this.Buttons.Add((ButtonEditChildButton)childButton);
                    }
                }
                childButtons.Clear();
            }
        }

        #endregion
    }

    #endregion

    #region CURRENCYEDITCOLLECTIONSERIALIZATIONPROVIDER
    [Syncfusion.Documentation.DocumentationExclude()]
    public class CurrencyEditCollectionSerializationProvider : IDesignerSerializationProvider
    {
        private Control.ControlCollection collectionToWatch;
        private ICurrencyEditDesigner notifyDesigner;
        public CurrencyEditCollectionSerializationProvider(Control.ControlCollection collectionToWatch, ICurrencyEditDesigner notifyDesigner)
        {
            this.collectionToWatch = collectionToWatch;
            this.notifyDesigner = notifyDesigner;
        }
        public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)        
        {
            if (objectType != null &&
                (objectType.IsSubclassOf(typeof(Control.ControlCollection))
                || objectType == typeof(Control.ControlCollection))
                )
                notifyDesigner.RemoveUnserializableChildControls();
            return null;
        }
    }
    #endregion

    #region CURRENCYEDITBUTTONSCOLLECTIONSERIALIZATIONPROVIDER
    [Syncfusion.Documentation.DocumentationExclude()]
    public class CurrencyEditButtonsCollectionSerializationProvider : IDesignerSerializationProvider
    {
        private ButtonEdit.ButtonEditChildButtonCollection collectionToWatch;
        private ICurrencyEditDesigner notifyDesigner;
        public CurrencyEditButtonsCollectionSerializationProvider(ButtonEdit.ButtonEditChildButtonCollection collectionToWatch, ICurrencyEditDesigner notifyDesigner)
        {
            this.collectionToWatch = collectionToWatch;
            this.notifyDesigner = notifyDesigner;
        }
        public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            return null;
        }
    }
    #endregion
}
