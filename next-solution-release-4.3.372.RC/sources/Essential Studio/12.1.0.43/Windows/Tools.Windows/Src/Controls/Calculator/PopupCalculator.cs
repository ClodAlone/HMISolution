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
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Win32;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The PopupCalculator is a class derived from <see cref="System.Windows.Forms.Control"/>
    /// that embeds a <see cref="CalculatorControl"/> to display it in Popup Mode.
    /// <para>
    /// The PopupCalculator class includes a <see cref="PopupControlContainer"/> that it
    /// uses for displaying the drop down window.
    /// </para>
    /// <para>
    /// The alignment of the Calculator Control with the parent control can be set through the
    /// <see cref="PopupCalculatorAlignment"/> property.
    /// </para>
    /// <para>
    /// The embedded <see cref="CalculatorControl"/> itself can be accessed through the
    /// <see cref="Calculator"/> property.
    /// </para>
    /// <para>
    /// This class also raises the <see cref="BeforeCalculatorPopupDisplay"/> event.
    /// <see cref="PopupCalculatorAlignment"/> property.
    /// </para>
    /// </summary>
    [
    ToolboxItem(false)
    ]
    public class PopupCalculator :
        Control,
        ISupportOffice2007Theme
    {
        /// <summary>
        /// The popup control container for the calculator.
        /// </summary>
        private PopupCalculatorContainer calcPopupContainer;

        /// <summary>
        /// The Calculator Control.
        /// </summary>
        private CalculatorControl calcControl;

        /// <summary>
        /// The PopupCalculator will be closed after this action.
        /// </summary>
        private CalcActions closeAction;

        /// <summary>
        /// The alignment of the Calculator Control with respect to the
        /// parent control.
        /// </summary>
        private CalculatorPopupAlignment calculatorAlignment;

        /// <summary>
        /// Event raised before the calculator popup is displayed.
        /// </summary>
        /// <remarks>
        /// This event is a <see cref="CancelEventHandler"/> type
        /// event. You can cancel the PopupCalculator from being displayed
        /// by handling this event.
        /// </remarks>
        public event CancelEventHandler BeforeCalculatorPopupDisplay;

        /// <summary>
        /// Event raised by a popup calculator when closing after the
        /// '=' button was clicked.
        /// </summary>
        /// <remarks>
        /// By default the CalculatorControl will raise this event when
        /// the '=' equal button is clicked. This event can be canceled 
        /// by a handler and the PopupCalculator will not be closed.
        /// </remarks>
        public event PopupCalculatorClosingEventHandler Closing;

        /// <summary>
        /// Indicates whether the embedded calculator has been created.
        /// </summary>
        private bool calcInitialized = false;

        /// <summary>
        /// The FlatStyle for the embedded calculator.
        /// </summary>
        private FlatStyle initFlatStyle;

        /// <summary>
        /// The Size for the embedded calculator. 
        /// </summary>
        private Size initSize = new System.Drawing.Size(158, 200);

        /// <summary>
        /// The BorderStyle for the embedded calculator.
        /// </summary>
        private Border3DStyle initBorderStyle;

        /// <summary>
        /// The Value for the embedded calculator.
        /// </summary>
        private CalculatorValue initValue;

        /// <summary>
        /// The LayoutType for the embedded calculator.
        /// </summary>
        private CalculatorLayoutTypes initLayoutType;

        private bool initUseVisualStyle;
        private ButtonAppearance initAppearance;
        private Office2007Theme initOffice2007Theme;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupCalculator"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the internal controls used for displaying
        /// the Calculator Control.
        /// <para>
        /// The initial value of the calculator needs to be set through the 
        /// <see cref="CalculatorControl.Value"/> property.
        /// </para>
        /// </remarks>
        public PopupCalculator()
        {
            this.calcPopupContainer = new PopupCalculatorContainer(this);
            this.initValue = new CalculatorValue();
            this.calcPopupContainer.Location = new System.Drawing.Point(0, 48);
            this.calcPopupContainer.Name = "popupControlContainer1";
            this.calcPopupContainer.Size = new System.Drawing.Size(158, 202);
            this.calcPopupContainer.TabIndex = 0;
            this.calcPopupContainer.BeforePopup += new System.ComponentModel.CancelEventHandler(this.HandleCalcPopupContainerBeforePopup);
            this.calcPopupContainer.CloseUp += new PopupClosedEventHandler(this.HandlePopupControlClosing);
            CTRLSIZE = this.calcPopupContainer.Size;
        }
        bool isScaling = false;

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
            foreach (Control ctrl in this.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");//,
                fi.SetValue(ctrl, this.EnableTouchMode, null);
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        } 
        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this.calcPopupContainer != null)
                {
                    if (this.calcPopupContainer.IsShowing())
                        this.calcPopupContainer.HidePopup(PopupCloseType.Done);
                    this.calcPopupContainer.Controls.Clear();
                    this.calcPopupContainer.ParentControl = null;
                    this.calcPopupContainer.BeforePopup -= new System.ComponentModel.CancelEventHandler(this.HandleCalcPopupContainerBeforePopup);
                    this.calcPopupContainer.CloseUp -= new PopupClosedEventHandler(this.HandlePopupControlClosing);
                    this.calcPopupContainer.Dispose();
                    this.calcPopupContainer = null;
                }

                if (this.calcControl != null)
                {
                    this.calcControl.ValueCalculated -= new CalculatorValueCalculatedEventHandler(this.HandleCalculatorValueCalculated);
                    this.calcControl.Dispose();
                    this.calcControl = null;
                }

                initValue = null;
            }

            base.Dispose(disposing);
        }

        protected virtual void InitializeCalculatorControl()
        {
            this.calcControl = new CalculatorControl();

            this.calcPopupContainer.SuspendLayout();

            this.calcPopupContainer.Controls.Add(this.calcControl);
            this.calcControl.ValueCalculated += new CalculatorValueCalculatedEventHandler(this.HandleCalculatorValueCalculated);
            this.calcControl.Dock = DockStyle.Fill;
            this.calcControl.AutoSize = true;
            this.calcControl.BorderStyle = Border3DStyle.Raised;
            this.calcControl.TabIndex = 0;
            this.calcControl.Dock = DockStyle.Fill;

            this.calcControl.BorderStyle = this.initBorderStyle;
            this.calcControl.FlatStyle = this.initFlatStyle;
            this.calcControl.LayoutType = this.initLayoutType;
            this.calcControl.Size = this.initSize;
            this.calcControl.Value = this.initValue;

            this.calcControl.UseVisualStyle = this.initUseVisualStyle;
            this.calcControl.ButtonStyle = this.initAppearance;
            this.calcControl.Office2007Theme = this.initOffice2007Theme;
            this.calcControl.MetroColor = this.metroColor;

            if (this.calcControl.UseVisualStyle && this.calcControl.ButtonStyle != ButtonAppearance.Classic)
                this.calcControl.BorderStyle = Border3DStyle.Flat;

            this.calcPopupContainer.ResumeLayout(false);
            this.calcInitialized = true;
        }

        /// <summary>
        /// Sets the flat style for the PopupCalculator.
        /// </summary>
        public FlatStyle FlatStyle
        {
            set
            {
                this.initFlatStyle = value;
                if (this.calcControl != null)
                    this.calcControl.FlatStyle = value;
            }
        }

        /// <summary>
        /// Sets the border style for the PopupCalculator.
        /// </summary>
        public Border3DStyle BorderStyle
        {
            set
            {
                this.initBorderStyle = value;
                if (this.calcControl != null)
                    this.calcControl.BorderStyle = value;
            }
        }

        /// <summary>
        /// Sets the size of the PopupCalculator.
        /// </summary>
        public new Size Size
        {
            set
            {
                this.initSize = value;

                if (this.calcPopupContainer != null)
                {
                    this.calcPopupContainer.Size = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value for the PopupCalculator.
        /// </summary>
        public CalculatorValue Value
        {
            get
            {
                if (this.calcControl != null)
                    return this.calcControl.Value;
                else
                    return this.initValue;
            }

            set
            {
                this.initValue = value;
                if (this.calcControl != null)
                    this.calcControl.Value = value;
            }
        }

        /// <summary>
        /// Sets the Layout type for the PopupCalculator.
        /// </summary>
        public CalculatorLayoutTypes LayoutType
        {
            set
            {
                this.initLayoutType = value;
                if (this.calcControl != null)
                    this.calcControl.LayoutType = value;
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public void HandleEnterKey()
        {
            KeyEventArgs e = new KeyEventArgs(Keys.Enter);
            this.calcControl.HandleChildKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="Closing"/> event. This event is
        /// raised by the calculator control before the popup calculator closes
        /// when the '=' button is clicked. This event will not be raised when
        /// the popup calculator control closes when it loses focus.
        /// </summary>
        /// <param name="closeType">Calculator close type.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void RaiseClosingEvent(CalculatorCloseType closeType)
        {
            CalculatorClosingEventArgs args = new CalculatorClosingEventArgs(closeType, this.Calculator.Value);
            this.OnClosing(args);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public void HandlePopupControlClosing(object sender, PopupClosedEventArgs args)
        {
            PopupCloseType closeType = args.PopupCloseType;
            CalculatorCloseType calcCloseType = new CalculatorCloseType();
            if (closeType == PopupCloseType.Canceled)
                calcCloseType = CalculatorCloseType.Canceled;
            else if (closeType == PopupCloseType.Deactivated)
                calcCloseType = CalculatorCloseType.Deactivated;
            else
                calcCloseType = CalculatorCloseType.Done;

            this.RaiseClosingEvent(calcCloseType);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public void HandleCalculatorValueCalculated(object sender, CalculatorValueCalculatedEventArgs args)
        {
            CalcActions lastAction = args.LastAction;
            if (lastAction == this.closeAction)
                this.CloseCalculator();
        }

        /// <summary>
        /// Gets or sets the action that will trigger closing the popup calculator.
        /// </summary>
        [
        Category("Behavior")
        ]
        public CalcActions CloseAction
        {
            get
            {
                return this.closeAction;
            }

            set
            {
                this.closeAction = value;
            }
        }

        /// <summary>
        /// Invokes the PopupCalculatorClosing event.
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
        protected virtual void OnClosing(CalculatorClosingEventArgs args)
        {
            this.CloseCalculator();

            if (this.Closing != null)
                this.Closing(this, args);
        }

        /// <summary>
        /// Gets or sets the property is the same as the <see cref="ParentControl"/> property
        /// of the PopupControlContainer.
        /// </summary>
        public Control ParentControl
        {
            get
            {
                return this.calcPopupContainer.ParentControl;
            }

            set
            {
                this.calcPopupContainer.ParentControl = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="BeforeCalculatorPopupDisplay"/> event.
        /// </summary>
        /// <returns>True if the Calculator display has been canceled by a event handler; false otherwise.</returns>
        /// <remarks>
        /// This event can be handled if you want to make some changes to the Calculator Control
        /// before it is displayed.
        /// </remarks>
        protected bool RaiseBeforeCalculatorPopupDisplayEvent()
        {
            bool bCancel = false;
            CancelEventArgs arg = new CancelEventArgs(bCancel);
            return this.OnBeforeCalculatorPopupDisplay(arg);
        }

        /// <summary>
        /// Gets or sets the relative alignment of the Popup with the
        /// parent control.
        /// </summary>
        /// <remarks>
        /// Refer to the <see cref="CalculatorPopupAlignment"/> enumeration
        /// for the list of values that this can take.
        /// </remarks>
        public CalculatorPopupAlignment PopupCalculatorAlignment
        {
            get
            {
                return this.calculatorAlignment;
            }

            set
            {
                this.calculatorAlignment = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="CalculatorControl"/> that this PopupCalculator class embeds. 
        /// </summary>
        /// <remarks>
        /// This is a read only property.
        /// </remarks>
        public CalculatorControl Calculator
        {
            get
            {
                if (this.calcControl == null)
                    InitializeCalculatorControl();
                return this.calcControl;
            }
        }

        /// <summary>
        /// Invokes the BeforeCalculatorPopupDisplay event.
        /// </summary>
        /// <param name="args">A BeforeCalculatorPopupDisplayEventArgs that contains the event data.</param>
        /// <returns> Returns Bool property.</returns>
        /// <remarks>
        /// The OnBeforeCalculatorPopupDisplay method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.
        /// <note type="note">Inheritors:  When overriding OnBeforeCalculatorPopupDisplay in a derived
        /// class, be sure to call the base class's OnBeforeCalculatorPopupDisplay method so that
        /// registered delegates receive the event.</note> 
        /// </remarks>
        protected virtual bool OnBeforeCalculatorPopupDisplay(CancelEventArgs args)
        {
            if (this.BeforeCalculatorPopupDisplay != null)
                this.BeforeCalculatorPopupDisplay(this, args);

            return args.Cancel;
        }

        /// <summary>
        /// Closes the popup calculator if it is displayed.
        /// </summary>
        /// <remarks>
        /// This method is invoked when the <see cref="CalculatorControl"/> raises the
        /// <see cref="PopupCalculator.Closing"/> event.
        /// </remarks>
        public void CloseCalculator()
        {
            if (this.calcPopupContainer.IsShowing())
                this.calcPopupContainer.HidePopup(PopupCloseType.Done);

            if (this.calcControl != null)
            {
                this.initBorderStyle = this.calcControl.BorderStyle;
                this.initFlatStyle = this.calcControl.FlatStyle;
                this.initLayoutType = this.calcControl.LayoutType;
                this.initSize = this.calcControl.Size;
                this.Value = this.calcControl.Value;
                this.calcControl.ValueCalculated -= new CalculatorValueCalculatedEventHandler(this.HandleCalculatorValueCalculated);

                this.calcInitialized = false;
                this.calcControl.Dispose();
                this.calcControl = null;
            }
        }

        /// <summary>
        /// Handles the popup control container's BeforePopup event.
        /// </summary>
        /// <param name="sender">The popup control container.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// This handles the <see cref="PopupControlContainer"/> 's <see cref="PopupControlContainer.BeforePopup"/>
        /// event and sets the BorderStyle and BackColor for the PopupControl conatiner.
        /// </remarks>
        private void HandleCalcPopupContainerBeforePopup(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.calcPopupContainer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.calcPopupContainer.BackColor = this.BackColor;
        }

        /// <summary>
        /// Overloaded. Displays the calculator.
        /// </summary>
        /// <remarks>
        /// This is the method used to display the Popup Calculator itself. This
        /// method takes into account the CalculatorPopupAlignment and displays the
        /// Popup Calculator at the right position.
        /// </remarks>
        public void DisplayCalculator()
        {
            this.DisplayCalculator(Point.Empty);
        }

        /// <summary>
        /// Displays the calculator.
        /// </summary>
        /// <param name="location">The location to display the popup calculator.</param>
        /// <remarks>
        /// This method displays the CalculatorControl and raises the <see cref="BeforeCalculatorPopupDisplay"/>
        /// event. This event allows the display to be canceled by a handler.
        /// </remarks>
        public void DisplayCalculator(Point location)
        {
            if (this.RaiseBeforeCalculatorPopupDisplayEvent() == false)
            {
                if (this.calcInitialized == false)
                    InitializeCalculatorControl();

                this.calcPopupContainer.Size = this.Calculator.Size;
                this.calcPopupContainer.ShowPopup(location);
            }
        }

        /// <summary>
        /// Sets a value indicating whether to use visual styles.
        /// </summary>
        public bool UseVisualStyle
        {
            set
            {
                this.initUseVisualStyle = value;
                if (this.calcControl != null)
                {
                    this.calcControl.UseVisualStyle = value;
                }
            }
        }
		/// <summary>
		/// Gets the metrocolor.
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#16A5DC");
		/// <summary>
		/// Gets or sets the metrocolor.
		/// </summary>
        public Color MetroColor
        {
            set
            {
                    metroColor = value;
            }
        }
        /// <summary>
        /// Sets the button style for the PopupCalculator.
        /// </summary>
        public ButtonAppearance ButtonStyle
        {
            set
            {
                this.initAppearance = value;
                if (this.calcControl != null)
                {
                    this.calcControl.ButtonStyle = value;
                }
            }
        }

        #region ISupportOffice2007Theme Members

        /// <summary>
        /// Gets or sets <see cref="Office2007Theme"/> to use.
        /// </summary>
        [Description("Specifies Office2007Theme to use.")]
        public Office2007Theme Office2007ColorTheme
        {
            get
            {
                return initOffice2007Theme;
            }
            set
            {
                initOffice2007Theme = value;
            }
        }

        /// <summary>
        /// Enables rendering with <see cref="Office2007Theme"/>.
        /// </summary>
        public void EnableOffice2007Style()
        {
            this.initAppearance = ButtonAppearance.Office2007;
        }

        /// <summary>
        /// Gets a value indicating whether rendering with <see cref="Office2007Theme"/> is enabled.
        /// </summary>
        public bool Office2007StyleEnabled
        {
            get
            {
                return this.initAppearance == ButtonAppearance.Office2007;
            }
        }

        #endregion
    }

    /// <summary>
    /// Derived class for the PopupCalculator.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class PopupCalculatorContainer : PopupControlContainer
    {
        private PopupCalculator calculator;
        
        public PopupCalculatorContainer(PopupCalculator calculator)
        {
            this.calculator = calculator;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.calculator = null;
            }
            base.Dispose(disposing);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public override Point GetPreferredLocation(PopupRelativeAlignment prevAlignment, out PopupRelativeAlignment newAlignment)
        {
            if (this.PopupParent != null)
            {
                Point loc = this.PopupParent.GetLocationForPopupAlignment(prevAlignment, out newAlignment);
                if (loc != Point.Empty)
                    return loc;
            }

            if (this.DiscreetLocation != Point.Empty)
            {
                newAlignment = PopupRelativeAlignment.Default;
                return this.DiscreetLocation;
            }

            Point location = Point.Empty;

            Control ctrlParent = this.ParentControl;
            if (ctrlParent != null)
            {
                Rectangle parentBounds = ctrlParent.Bounds;

                location = PopupUtils.ComputeDefaultPopupAlignment(prevAlignment, out newAlignment, this.calculator.PopupCalculatorAlignment == CalculatorPopupAlignment.Left ? PopupRelativeAlignment.BottomLeft : PopupRelativeAlignment.BottomRight, PopupRelativeAlignment.BottomLeft, parentBounds);
                if (RightToLeft.Yes == ctrlParent.RightToLeft)
                {
                    int nLeftOffset = parentBounds.Left - location.X;
                    location.X = parentBounds.Right - this.Width - nLeftOffset;
                }

                Control ctrlParentHost = ctrlParent.Parent;
                if (ctrlParentHost != null)
                {
                    location = ctrlParentHost.PointToScreen(location);
                }
            }
            else
            {
                newAlignment = PopupRelativeAlignment.Default;
                location = new Point(1, 1);
            }

            return location;
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            Keys keyCode = key & Keys.KeyCode;
            bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;

            switch (keyCode)
            {
                case Keys.F2:
                case Keys.Enter:
                case Keys.Tab:
                    this.calculator.HandleEnterKey();
                    break;
            }
            return base.ProcessDialogKey(key);
        }
    }

    /// <summary>
    /// Specifies the way in which a popup calculator was closed.
    /// </summary>
    /// <remarks>
    /// This information is usually provided in a 
    /// PopupControlContainer's <see cref="Syncfusion.Windows.Forms.PopupControlContainer.CloseUp"/> event.
    /// You can use it to determine, in some cases, whether or not
    /// to use the updated data in a popup.
    /// </remarks>
    public enum CalculatorCloseType
    {
        /// <summary>
        /// The user wants the changes made in the popup to be applied.
        /// </summary>
        Done,

        /// <summary>
        /// The user canceled the popup and expects the changes, if any to be ignored.
        /// </summary>
        Canceled,

        /// <summary>
        /// The popup was deactivated due to the user clicking in some
        /// other window, a different application getting focus, etc.
        /// </summary>
        Deactivated
    }

    /// <summary>
    /// Delegate for the <see cref="PopupCalculator.Closing"/> event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="args">Calculator closing event Args</param>
    /// <remarks>
    /// See the <see cref="PopupCalculator.Closing"/> event for more information.
    /// </remarks>
    public delegate void PopupCalculatorClosingEventHandler(object sender, CalculatorClosingEventArgs args);

    /// <summary>
    /// The event data for <see cref="PopupCalculator.Closing"/> event.
    /// </summary>
    /// <remarks>
    /// This event is raised by the <see cref="PopupCalculator"/> class
    /// when in Popup mode. The event is raised when the Popup is being closed
    /// after the Calculator was displayed.
    /// </remarks>
    public class CalculatorClosingEventArgs
    {
        /// <summary>
        /// Specifies whether the equal to(=) button was clicked.
        /// </summary>
        private CalculatorCloseType closeType;

        /// <summary>
        /// The final value of the calculator.
        /// </summary>
        private CalculatorValue finalValue;

        /// <summary>
        /// Initializes a new instance of the CalculatorClosingEventArgs class.
        /// </summary>
        /// <param name="closeType">Specifies if the '=' button was clicked.</param>
        /// <param name="finalValue">The final value of the calculator.</param>
        /// <remarks>
        /// This constructor for the <see cref="CalculatorClosingEventArgs"/> class
        /// takes the two required pieces of information for this class as parameters.
        /// The first parameter specifies if the Equal to button was clicked. What this means
        /// is that the user chose to click the Equal button and complete the calculation.
        /// The <see cref="CalculatorValue"/> object contains the final calculated value
        /// of the Calculator Control.
        /// </remarks>
        public CalculatorClosingEventArgs(CalculatorCloseType closeType, CalculatorValue finalValue)
        {
            this.finalValue = finalValue;
            this.closeType = closeType;
        }

        /// <summary>
        /// Gets whether the equal button was clicked.
        /// </summary>
        /// <remarks>
        /// The behavior of the CalculatorControl in PopupMode is to close when the
        /// Equal to button is clicked. This property specifies if the Equal to button 
        /// was clicked by the user.
        /// </remarks>
        public CalculatorCloseType CloseType
        {
            get
            {
                return this.closeType;
            }
        }

        /// <summary>
        /// Gets the final value of the calculator.
        /// </summary>
        /// <remarks>
        /// The final value of the <see cref="CalculatorControl"/> is the value that
        /// the <see cref="CalculatorControl.Engine"/> object had when the Popup Calculator Control
        /// was closed.
        /// </remarks>
        public CalculatorValue FinalValue
        {
            get
            {
                return this.finalValue;
            }
        }
    }

    /// <summary>
    /// This enumeration lists the possible alignments for the CalculatorPopup when displayed 
    /// by the <see cref="CurrencyEdit"/> control.
    /// </summary>
    public enum CalculatorPopupAlignment
    {
        /// <summary>
        /// Align to the left of the control.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Align to the right of the control.
        /// </summary>
        Right
    }
}
