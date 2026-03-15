#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The ButtonEditChildButton class is used by the <see cref="ButtonEdit"/>
    /// class as its child buttons. This class is derived from the <see cref="System.Windows.Forms.Button"/>
    /// class. You will not need to create this control directly as it will be created through
    /// the ButtonEdit class.
    /// </summary>
    /// <remarks><para>
    /// The Buttons embedded in a <see cref="ButtonEdit"/> control need to abide by some constraints
    /// imposed by the layout and the settings of the ButtonEdit control. A customized Button control
    /// is needed for this purpose so that the relationship between the embedded child buttons and the
    /// parent ButtonEdit control can be established.
    /// </para><para>
    /// Customizing the Button class to provide features suitable for use inside the ButtonEdit
    /// control also provides ease of use for users. The ButtonEditChildButton implements additional
    /// properties such as <see cref="ButtonType"/> (that allow you to choose different types of
    /// commonly used button icons).
    /// </para><para>
    /// The ButtonEditChildButton also supports a listener model that notifies a listener when its
    /// size or alignment changes. The listener implements the <see cref="IButtonEditParent"/> interface
    /// to listen to these notifications.
    /// </para></remarks>
    [
    ToolboxItem(false),
    DesignTimeVisible(true),
    Designer(typeof(ButtonEditChildButtonDesigner), typeof(IDesigner))
    ]
    public class ButtonEditChildButton : ButtonAdv
    {
        #region Class constants
        /// <summary>Pre-defined names of button image resources.</summary>
        private static readonly string[] s_names = new string[]
            {
                string.Empty, "Calculator.bmp", "Currency.bmp", "Down.bmp", "ComboXPDown.bmp", "Up.bmp", "Left.bmp", "Right.bmp", "Redo.bmp", "Undo.bmp", "Check.bmp", "Browse.bmp", "Leftend.bmp", "Rightend.bmp"
            };

        /// <summary>Default height of child button.</summary>
        private const int DefaultHeight = 17;

        /// <summary>Default widht of the button.</summary>
        private const int DefaultWidth = 18;
        #endregion

        #region Class members

        /// <summary>
        /// Button alignment of the edit control.
        /// </summary>
        private ButtonAlignment buttonAlign = ButtonAlignment.Right;

        /// <summary>
        /// The ButtonEditParent that needs to be notified of changes.
        /// </summary>
        private IButtonEditParent buttonEditParent;

        /// <summary>
        /// The preferred width of the button.
        /// </summary>
        private int preferredWidth;
        private bool isButtonTypeChanged = false;

        /// <summary>
        /// To save the text of the button.
        /// </summary>
        internal string ChildButtonText;

        /// <summary>
        /// Indicates that button was clicked.
        /// </summary>
        private bool m_bIsbuttonClicked = false;
        #endregion

        #region Class properties

        /// <summary>
        /// Gets or sets the <see cref="ButtonEdit"/> control that is the parent and also the listener.
        /// </summary>
        /// <remarks>
        /// The <see cref="ButtonEdit"/> control implements the <see cref="IButtonEditParent"/>
        /// interface to act as a listener for change notifications from ButtonEditChildButton controls.
        /// </remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public IButtonEditParent ButtonEditParent
        {
            get
            {
                return this.buttonEditParent;
            }

            set
            {
                this.buttonEditParent = value;
            }
        }

        /// <summary>
        /// Gets or sets the preferred width; the width set by the use, 
        /// treated as the width of the ButtonEditChildButton control.
        /// </summary>
        /// <remarks>
        /// The <see cref="Size"/> property and the PreferredWidth property are maintained in sync
        /// and the Size property will not allow the width property to be changed. Changes to the 
        /// width of the button have to be set through the PreferredWidth property.
        /// </remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The preferred width is the desired width of the Button.")
        ]
        public int PreferredWidth
        {
            get
            {
                return this.preferredWidth;
            }

            set
            {
                this.preferredWidth = value;

                if (this.ButtonEditParent != null)
                {
                    this.ButtonEditParent.ChildButtonSizeChanged(this, new Size(this.preferredWidth, DefaultHeight));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Size property. 
        /// </summary>
        /// <remarks>
        /// The Size property displays the size of the ButtonEditChildButton. The height of the button
        /// is fixed by the height of the <see cref="ButtonEdit"/> control and the width is specified
        /// through the <see cref="PreferredWidth"/> property.
        /// </remarks>
        [
        Browsable(false),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Overrides the Size property. Set the Width through the PreferredWidth property.")
        ]
        public new Size Size
        {
            get
            {
                return base.Size;
            }

            set
            {
                if (value.Width > this.preferredWidth)
                {
                    value.Width = this.preferredWidth;
                }
                if (this.ButtonEditParent != null)
                {
                    this.ButtonEditParent.ChildButtonSizeChanged(this, value);
                }
                base.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets the Location property. Cannot be changed.
        /// </summary>
        [
        Browsable(false),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Overrides the Location property. Cannot the changed.")
        ]
        public new Point Location
        {
            get
            {
                return base.Location;
            }

            set
            {
                base.Location = value;
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="ButtonTypes"/> for this ButtonEditChildButton.
        /// </summary>
        /// <remarks>
        /// Each of the ButtonTypes are associated with a commonly used button icon
        /// such as Up, down, undo, redo, etc. Choosing one of these types will set the
        /// <see cref="Image"/> property to one of the pre configured images.
        /// <para>
        /// This dependence between the ButtonType and the Image is not maintained strictly
        /// and is meant to be a help to developers. You can specify any image you want for 
        /// the button through the <see cref="Image"/> property.
        /// </para></remarks>
        [
        Browsable(true),
        Category("Appearance - Styles"),
        Editor(typeof(ButtonEditTypeEditor), typeof(UITypeEditor)),
        TypeConverter(typeof(ButtonEditTypeEditor)),
        Description("Specifies the ButtonType for this ButtonEditChildButton"),
        DefaultValue(ButtonTypes.Normal)
        ]
        public override ButtonTypes ButtonType
        {
            get
            {
                return base.ButtonType;
            }
            set
            {
                base.ButtonType = value;

                if (this.ButtonType == ButtonTypes.Calculator && this.Width < 24)
                {
                    this.PreferredWidth = 24;
                    if (this.ButtonEditParent != null)
                        this.ButtonEditParent.ChildButtonSizeChanged(this, this.Size);
                }

                isButtonTypeChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the image based on the button style for the control.
        /// </summary>
        public new Image Image
        {
            get
            {
                if (isButtonTypeChanged)
                {
                    isButtonTypeChanged = false;
                    return GetButtonImage(this.ButtonType);
                }
                else
                {
                    return base.Image;
                }
            }
            set
            {
                if (base.Image != value)
                {
                    base.Image = value;
                }
            }
        }

        /// <summary>
        /// Gets the appearance of the button control
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ButtonAppearance Appearance
        {
            get
            {
                return base.Appearance;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the button with respect to the edit control.
        /// </summary>
        /// <remarks>
        /// The possible values for the ButtonAlign property are the values of the
        /// <see cref="ButtonAlignment"/> enumeration. Using the values of the enumeration
        /// the value can be set to be at the right or the left of the edit control.
        /// <para>
        /// The default value for this property is <see cref="ButtonAlignment.Right"/></para></remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The alignment of the button with respect to the edit control."),
        DefaultValue(ButtonAlignment.Right)
        ]
        public ButtonAlignment ButtonAlign
        {
            get
            {
                return this.buttonAlign;
            }

            set
            {
                this.buttonAlign = value;
                if (this.ButtonEditParent != null)
                {
                    this.ButtonEditParent.ChildButtonAlignmentChanged(this, this.ButtonAlign);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the TAB key to give focus to the control.
        /// </summary>
        [
        Browsable(true),
        Category("Behavior"),
        Description("Indicates whether the user can use the TAB key to give focus to the control."),
        DefaultValue(false)
        ]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }

            set
            {
                base.TabStop = value;
            }
        }

        /// <summary>
        /// Indicates whether to use visual styles.
        /// </summary>
        /// <remarks>Override required for hiding property from design time for user.</remarks>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool UseVisualStyle
        {
            get
            {
                return base.UseVisualStyle;
            }
            set
            {
                base.UseVisualStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the display of the control when users move the mouse over the control and click.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance - Styles"),
        Description("Determines the display of the control when users move the mouse over the control and click.")
        ]
        public new FlatStyle FlatStyle
        {
            get
            {
                return base.FlatStyle;
            }
            set
            {
                if (this.UseVisualStyle && value == FlatStyle.System)
                {
                    throw new ArgumentException("FlatStyle cannot be System when UseVisualStyle is true");
                }

                base.FlatStyle = value;
            }
        }
        protected internal bool LastLeftButton
        {
            get
            {
                return GetIsLastLeftButton();
            }
            set
            {
                SetIsLastLeftButton(value);
            }
        }
        protected internal bool FirstRightButton
        {
            get
            {
                return GetIsFirstRightButton();
            }
            set
            {
                SetIsFirstRightButton(value);
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonEditChildButton"/> class.
        /// </summary>
        /// <remarks>
        /// The ButtonEditChildButton is created and assigned the constraints that need to be
        /// in place for it to be embedded in a <see cref="ButtonEdit"/> control. 
        /// <para>
        /// The <see cref="ButtonEditChildButton.ButtonType"/> is initially set to <see cref="ButtonTypes.Normal"/>.
        /// </para></remarks>
        public ButtonEditChildButton()
            : base(true)
        {
            this.preferredWidth = DefaultWidth;

            this.TabStop = false;
            this.Text = string.Empty;
            this.Size = new Size(DefaultWidth, DefaultHeight);
            this.ButtonType = ButtonTypes.Normal;
            this.ImageAlign = ContentAlignment.MiddleCenter;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Sets the appearance of the button control.
        /// </summary>
        /// <param name="appearance">The appearance to be set.</param>
        public void SetAppearance(ButtonAppearance appearance)
        {
            base.Appearance = appearance;
        }
        /// <summary>
        /// Sets the appearance of the button control.
        /// </summary>
        /// <param name="appearance">The appearance to be set.</param>
        public void SetAppearance(ButtonAppearance appearance,Color metroColor)
        {
            base.BackColor = metroColor;
            base.Appearance = appearance;         
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns the image for the specified button type. 
        /// </summary>
        /// <remarks>
        /// This dependence between the ButtonType and the Image is not maintained strictly
        /// and is meant to be a help to developers. You can specify any image you want for 
        /// the button through the <see cref="Image"/> property.
        /// </remarks>
        /// <returns>returns image</returns>
        /// <param name="buttonType">Button Types</param>
        protected virtual Image GetButtonImage(ButtonTypes buttonType)
        {
            if (this.ButtonType != ButtonTypes.Normal)
            {
                this.Text = string.Empty;
            }
            else
            {
                this.Image = null;
                this.Text = this.ChildButtonText;
            }

            int index = (int)buttonType;

            if (index > 0)
            {
                this.Image = ButtonEdit.GetImage(s_names[index]);
                return this.Image;
            }

            return null;
        }

        /// <summary>Overrides the OnClick method. Set focus to parent control 
        /// when needed.</summary>
        /// <param name="e">The click event data.</param>
        protected override void OnClick(EventArgs e)
        {
            m_bIsbuttonClicked = true;

            base.OnClick(e);

            if (this.Parent != null && !this.Parent.Focused)
            {
                ((ButtonEdit)this.Parent).TextBox.Focus();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_bIsbuttonClicked = false;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.FlatStyle == FlatStyle.System && !m_bIsbuttonClicked)
            {
                this.OnClick(EventArgs.Empty);
            }
            base.OnMouseUp(e);
        }
        #endregion
    }
}