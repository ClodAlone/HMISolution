//-------------------------------------------------------------------------------------------------
// <copyright file="ButtonEditInfo.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using Syncfusion.Styles;
    using Syncfusion.Windows.Forms.Grid;

    #region CustomProperties_with expandable objects

    /// <summary>
    /// Provides custom properties for button edit cell. All properties
    /// support the style inheritance mechanism. You can change these propeties
    /// in the property grid.
    /// </summary>   
    public class ButtonEditStyleProperties : GridStyleInfoCustomProperties
    {
        // static initialization of property descriptors
        static Type t = typeof(ButtonEditStyleProperties);

        readonly static StyleInfoProperty ButtonEditInfoProperty = CreateStyleInfoProperty(t, "ButtonEditInfo");

        // default settings for all properties this object holds
        static ButtonEditStyleProperties defaultObject;

        // initialize default settings for all properties in static ctor
        static ButtonEditStyleProperties()
        {
            // all properties must be initialized for the Default property
            defaultObject = new ButtonEditStyleProperties(GridStyleInfo.Default);
            defaultObject.ButtonEditInfo = ButtonEditInfo.Default;
        }

        /// <summary>
        /// Gets access to default values for this type
        /// </summary>
        public static ButtonEditStyleProperties Default
        {
            get
            {
                return defaultObject;
            }
        }

        /// <summary>
        /// Force static ctor being called at least once
        /// </summary>
        public static void Initialize()
        {
        }

        /// <summary>
        /// Explicit cast from GridStyleInfo to this custom propety object
        /// </summary>
        /// <param name="style">Specifies button edit cell's style information.</param>
        /// <returns>A new custom properties object.</returns>
        public static explicit operator ButtonEditStyleProperties(GridStyleInfo style)
        {
            return new ButtonEditStyleProperties(style);
        }

        /// <summary>
        /// Initializes a ButtonEditStyleProperties object with a style object that holds all data
        /// </summary>
        /// <param name="style">Specifies style information for the button edit cell.</param>
        public ButtonEditStyleProperties(GridStyleInfo style)
            : base(style)
        {
        }

        /// <summary>
        /// Initializes a ButtonEditStyleProperties object with an empty style object. Design
        /// time environment will use this ctor and later copy the values to a style object
        /// by calling style.CustomProperties.Add(gridExcelTipStyleProperties1)
        /// </summary>
        public ButtonEditStyleProperties()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets whether button should be shown at the left of textbox
        /// </summary>
        [Description("Button Edit properties"),
        Browsable(true),
        Category("Custom")]
        public ButtonEditInfo ButtonEditInfo
        {
            get
            {
                return (ButtonEditInfo)style.GetValue(ButtonEditInfoProperty);
            }

            set
            {
                style.SetValue(ButtonEditInfoProperty, value);
            }
        }
    }

    /// <summary>
    /// Specifies the image for the ButtonEdit CellType
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// Represents none
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents Browse
        /// </summary>
        Browse = 1,

        /// <summary>
        /// Represents Check
        /// </summary>
        Check = 2,

        /// <summary>
        /// Represents Down
        /// </summary>
        Down = 3,

        /// <summary>
        /// Represents Left
        /// </summary>
        Left = 4,

        /// <summary>
        /// Represents Left end
        /// </summary>
        Leftend = 5,

        /// <summary>
        /// Represents Redo
        /// </summary>
        Redo = 6,

        /// <summary>
        /// Represents Right
        /// </summary>
        Right = 7,

        /// <summary>
        /// Represents Right end 
        /// </summary>
        Rightend = 8,

        /// <summary>
        /// Represents Undo
        /// </summary>
        Undo = 9,

        /// <summary>
        /// Represents Up
        /// </summary>
        Up = 10,

        /// <summary>
        /// Represents Image
        /// </summary>
        Image = 11
    }
    #endregion

    /// <summary>
    /// Implements the data store for the <see cref="ButtonEditInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class ButtonEditInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(ButtonEditInfoStore), typeof(ButtonEditInfo), true);

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.IsLeft"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditIsLeftProperty = sd.CreateStyleInfoProperty(typeof(bool), "ButtonEditIsLeft");

        /// <summary>
        /// Provides information about the <see cref="StyleInfoProperty"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditTypeProperty = sd.CreateStyleInfoProperty(typeof(int), "ButtonEditType");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.Width"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditWidthProperty = sd.CreateStyleInfoProperty(typeof(int), "ButtonEditWidth");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.Text"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditTextProperty = sd.CreateStyleInfoProperty(typeof(string), "ButtonEditText");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.TextColor"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditTextColorProperty = sd.CreateStyleInfoProperty(typeof(System.Drawing.Color), "ButtonEditTextColor");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.BackColor"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditBackColorProperty = sd.CreateStyleInfoProperty(typeof(System.Drawing.Color), "ButtonEditBackColor");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.Enabled"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "ButtonEditEnabled");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.Image"/> property
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditImageProperty = sd.CreateStyleInfoProperty(typeof(Image), "ButtonEditImage");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.ForceBackColor"></see>
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditForceBackColorProperty = sd.CreateStyleInfoProperty(typeof(bool), "ButtonEditForceBackColor");

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.HorizontalAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditTextHorizontalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(GridHorizontalAlignment), "HorizontalAlignment", 3, true);

        /// <summary>
        /// Provides information about the <see cref="ButtonEditInfo.VerticalAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ButtonEditTextVerticalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(GridVerticalAlignment), "VerticalAlignment", 3, true);

        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="ButtonEditInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="ButtonEditInfoStore"/>.
        /// </summary>
        public ButtonEditInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ButtonEditInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private ButtonEditInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>Returns a copy of the current object.</summary>
        /// Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        /// calling new directly is more efficient. Otherwise this override is obsolete.
        /// <override/>       
        /// <returns>A copy of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new ButtonEditInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Provides a <see cref="GridStyleInfoSubObject"/> object for button edit properties in a cell.
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    [TypeConverter(typeof(StyleInfoBaseConverter))]
    public class ButtonEditInfo : GridStyleInfoSubObject
    {
        /// <summary> 
        /// Static Fields
        /// </summary>
        private static ButtonEditInfo defaultButtonEditInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new ButtonEditInfo(identity, store as ButtonEditInfoStore);
            }

            return new ButtonEditInfo(identity);
        }

        /// <summary>
        /// Releases the all resources used by the Component.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// Constructors
        /// <summary>
        /// Initializes a <see cref="ButtonEditInfo"/>
        /// </summary>
        /// <overload>
        /// Initializes a <see cref="ButtonEditInfo"/>
        /// </overload>
        [DebuggerStepThrough()]
        public ButtonEditInfo()
            : base(new ButtonEditInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="ButtonEditInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="ButtonEditInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ButtonEditInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ButtonEditInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="ButtonEditInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridFontInfo"/></param>.
        /// <param name="store">A <see cref="ButtonEditInfoStore"/> that holds data for this <see cref="GridFontInfo"/>.
        /// All changes in this style object will saved in the <see cref="ButtonEditInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public ButtonEditInfo(StyleInfoSubObjectIdentity identity, ButtonEditInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="ButtonEditInfo"/>  object and initializes its IsLeft, ButtonEditType, Width, Text, TextColor, Enabled and
        /// BackColor properties
        /// </summary>
        /// <param name="font">Cell font.</param>
        [DebuggerStepThrough()]
        public ButtonEditInfo(Font font)
            : base(new ButtonEditInfoStore())
        {
            this.IsLeft = false;
            this.ButtonEditType = Syncfusion.GridHelperClasses.ButtonType.Browse;
            this.Width = 20;
            this.Text = null;
            this.TextColor = Color.Black;
            this.Enabled = true;
            this.BackColor = Control.DefaultBackColor;
            Image = null;
            this.HorizontalAlignment = GridHorizontalAlignment.Center;

            this.VerticalAlignment = GridVerticalAlignment.Middle;
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Gets a copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">The identifier for this object.</param>
        /// <returns>A copy of current object.</returns>
        /// <remarks></remarks>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new ButtonEditInfo(newOwner.CreateSubObjectIdentity(sip), (ButtonEditInfoStore)Store.Clone());
        }

        /// <summary>
        /// Gets a default <see cref="ButtonEditInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// <para/>
        /// Default settings are:<para/>
        ///      <list type="table">
        ///         <listheader><term>Property</term><description>Value</description></listheader>
        ///         <item><term><see cref="IsLeft"/></term><description>False</description></item>
        ///         <item><term><see cref="ButtonEditType"/></term><description>ButtonEditType.Browse</description></item>
        ///         <item><term><see cref="Width"/></term><description>20</description></item>
        ///         <item><term><see cref="Text"/></term><description>""</description></item>
        ///         <item><term><see cref="TextColor"/></term><description>Color.Black</description></item>
        ///         <item><term><see cref="BackColor"/></term><description>Control.DefaultBackColor</description></item>
        ///         <item><term><see cref="Enabled"/></term><description>True</description></item>
        ///         <item><term><see cref="Image"/></term><description>null</description></item>
        ///      </list>
        /// </remarks>
        public static ButtonEditInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultButtonEditInfo == null)
                {
                    defaultButtonEditInfo = new ButtonEditInfo();
                    defaultButtonEditInfo.IsLeft = false;
                    defaultButtonEditInfo.ButtonEditType = Syncfusion.GridHelperClasses.ButtonType.Browse;
                    defaultButtonEditInfo.Width = 20;
                    defaultButtonEditInfo.Text = null;
                    defaultButtonEditInfo.TextColor = Color.Black;
                    defaultButtonEditInfo.Enabled = true;
                    defaultButtonEditInfo.BackColor = Control.DefaultBackColor;
                    defaultButtonEditInfo.Image = null;

                    // If the themes are enabled, setting this forces to draw the button color
                    defaultButtonEditInfo.ForceBackColor = false;

                    defaultButtonEditInfo.HorizontalAlignment = GridHorizontalAlignment.Center;
                    defaultButtonEditInfo.VerticalAlignment = GridVerticalAlignment.Middle;
                }

                return defaultButtonEditInfo;
            }
        }

        /// <summary>
        /// Overrides the method OnStyleChanged for your derived class
        /// </summary>
        /// <param name="sip">The style info property</param>
        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            base.OnStyleChanged(sip);
        }

        /// <summary>
        /// Gets the default style object by override this method for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        //// Properties
        #region IsLeft

        /// <summary>
        /// Gets or sets a value indicating whether the button should be positioned in the left side of the cell
        /// </summary>
        [Description("specifies if the button should be positioned to the left."),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public bool IsLeft
        {
            get
            {
                return (bool)GetValue(ButtonEditInfoStore.ButtonEditIsLeftProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditIsLeftProperty, value);
            }
        }

        #endregion

        #region ButtonEditType

        /// <summary>
        /// Gets or sets the type of the Image to be drawn in the button
        /// </summary>
        [Description("Specifies the type of the Image to be drawn in the button."),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public ButtonType ButtonEditType
        {
            get
            {
                return (ButtonType)GetValue(ButtonEditInfoStore.ButtonEditTypeProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditTypeProperty, value);
                if (this.ButtonEditType != ButtonType.Image)
                {
                    Image = null;
                }
            }
        }

        #endregion

        #region Width

        /// <summary>
        /// Gets or sets the width of the button
        /// </summary>
        [Description("Specifies the width of the button"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public int Width
        {
            get
            {
                return (int)GetValue(ButtonEditInfoStore.ButtonEditWidthProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditWidthProperty, value);
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Gets or sets the text to be shown in the button
        /// </summary>
        [Description("Specifies the text to be shown in the button"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public string Text
        {
            get
            {
                return (string)GetValue(ButtonEditInfoStore.ButtonEditTextProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditTextProperty, value);
            }
        }

        #endregion

        #region TextColor

        /// <summary>
        /// Gets or sets the text color
        /// </summary>
        [Description("Specifies the text color"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(ButtonEditInfoStore.ButtonEditTextColorProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditTextColorProperty, value);
            }
        }

        #endregion

        #region Enabled

        /// <summary>
        /// Gets or sets a value indicating whether the button should be enabled
        /// </summary>
        [Description("Specifies if the button should be enabled"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public bool Enabled
        {
            get
            {
                return (bool)GetValue(ButtonEditInfoStore.ButtonEditEnabledProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditEnabledProperty, value);
            }
        }

        #endregion

        #region BackColor

        /// <summary>
        /// Gets or sets the BackColor of the button
        /// </summary>
        [Description("Specifies the BackColor of the button"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public Color BackColor
        {
            get
            {
                return (Color)GetValue(ButtonEditInfoStore.ButtonEditBackColorProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditBackColorProperty, value);
            }
        }

        #endregion

        #region Image

        /// <summary>
        /// Gets or sets the Image to be displayed in the button
        /// </summary>
        [Description("Specifies the image to be displayed in the button"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public Image Image
        {
            get
            {
                return (Image)GetValue(ButtonEditInfoStore.ButtonEditImageProperty);
            }

            set
            {
                if (this.ButtonEditType == ButtonType.Image)
                {
                    SetValue(ButtonEditInfoStore.ButtonEditImageProperty, value);
                }
            }
        }

        #endregion

        #region ForceBackColor

        /// <summary>
        /// Gets or sets a value indicating whether the button should be colored with the <see cref="ButtonEditInfo.BackColor"></see>
        /// if themes are enabled
        /// </summary>
        [Description("specifies whether the button should be colored neglecting the themes"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public bool ForceBackColor
        {
            get
            {
                return (bool)GetValue(ButtonEditInfoStore.ButtonEditForceBackColorProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditForceBackColorProperty, value);
            }
        }

        #endregion

        #region VerticalAlignment

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        [Description("Specifies vertical alignment of text in the Button."),
        Browsable(true),
        Category("Custom")]
        [NotifyParentProperty(true)]
        public GridVerticalAlignment VerticalAlignment
        {
            get
            {
                return (GridVerticalAlignment)GetShortValue(ButtonEditInfoStore.ButtonEditTextVerticalAlignmentProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditTextVerticalAlignmentProperty, (short)value);
            }
        }

        #endregion

        #region HorizontalAlignment

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        [Description("Specifies horizontal alignment of text in the Button."),
        Browsable(true),
        Category("Custom")]
        [NotifyParentProperty(true)]
        public GridHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (GridHorizontalAlignment)GetShortValue(ButtonEditInfoStore.ButtonEditTextHorizontalAlignmentProperty);
            }

            set
            {
                SetValue(ButtonEditInfoStore.ButtonEditTextHorizontalAlignmentProperty, (short)value);
            }
        }

        #endregion
    }
}
