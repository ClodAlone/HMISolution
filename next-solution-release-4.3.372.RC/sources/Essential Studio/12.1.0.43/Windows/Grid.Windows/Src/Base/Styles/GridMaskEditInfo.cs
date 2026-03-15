//-------------------------------------------------------------------------------------------------
// <copyright file="GridMaskEditInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using System.Diagnostics;
using System.Globalization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for masked edit properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridMaskEditInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridMaskEditInfo defaultMaskEditInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridMaskEditInfo(identity, store as GridMaskEditInfoStore);
            }

            return new GridMaskEditInfo(identity);
        }

        // Constructors

        /// <summary>
        /// Initializes a new empty <see cref="GridMaskEditInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridMaskEditInfo()
            : base(new GridMaskEditInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridMaskEditInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridMaskEditInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridMaskEditInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridMaskEditInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridMaskEditInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridMaskEditInfo"/>.</param>
        /// <param name="store">A <see cref="GridMaskEditInfoStore"/> that holds data for this <see cref="GridMaskEditInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridMaskEditInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridMaskEditInfo(StyleInfoSubObjectIdentity identity, GridMaskEditInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates an exact copy of the current object.</summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">An identifier for the current object.</param>
        /// <returns>A copy of the current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridMaskEditInfo(newOwner.CreateSubObjectIdentity(sip), (GridMaskEditInfoStore)Store.Clone());
        }

        // Default

        /// <summary>
        /// Gets a default <see cref="GridMaskEditInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings are: <para/>
        /// <list type="table">
        /// <listheader><term>Property</term><description>Value</description></listheader>
        /// <item><term><see cref="AllowPrompt"/></term><description>False</description></item>
        /// <item><term><see cref="ClipMode"/></term><description>ClipModes.IncludeInternals</description></item>
        /// <item><term><see cref="DateSeparator"/></term><description>'/'</description></item>
        /// <item><term><see cref="DateTimeFormatInfoObject"/></term><description>Culture.DateTimeFormat</description></item>
        /// <item><term><see cref="DecimalSeparator"/></term><description>'.'</description></item>
        /// <item><term><see cref="Mask"/></term><description>String.Empty</description></item>
        /// <item><term><see cref="MaxValue"/></term><description>int.MaxValue</description></item>
        /// <item><term><see cref="MinValue"/></term><description>0</description></item>
        /// <item><term><see cref="NumberFormatInfoObject"/></term><description>Culture.NumberFormatInfo</description></item>
        /// <item><term><see cref="PaddingCharacter"/></term><description>' '</description></item>
        /// <item><term><see cref="PassivePromptCharacter"/></term><description>' '</description></item>
        /// <item><term><see cref="SpecialCultureValue"/></term><description>SpecialCultureValues.None</description></item>
        /// <item><term><see cref="ThousandSeparator"/></term><description>','</description></item>
        /// <item><term><see cref="TimeSeparator"/></term><description>':'</description></item>
        /// <item><term><see cref="UsageMode"/></term><description>MaskedUsageMode.Normal</description></item>
        /// <item><term><see cref="UseLocaleDefault"/></term><description>false</description></item>
        /// <item><term><see cref="UseUserOverride"/></term><description>true</description></item>
        /// </list>
        /// </remarks>
        public static GridMaskEditInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultMaskEditInfo == null)
                {
                    defaultMaskEditInfo = new GridMaskEditInfo();

                    // Retrive default values from an empty masked edit box.
                    //                    MaskedEditBox eb = new MaskedEditBox();
                    //                    defaultMaskEditInfo.AllowPrompt = eb.AllowPrompt;
                    //                    defaultMaskEditInfo.ClipMode = eb.ClipMode;
                    //                    defaultMaskEditInfo.DateSeparator = eb.DateSeparator;
                    //                    defaultMaskEditInfo.DateTimeFormatInfoObject = eb.DateTimeFormatInfoObject;
                    //                    defaultMaskEditInfo.DecimalSeparator = eb.DecimalSeparator;
                    //                    defaultMaskEditInfo.Mask = eb.Mask;
                    //                    defaultMaskEditInfo.MaxValue = eb.MaxValue;
                    //                    defaultMaskEditInfo.MinValue = eb.MinValue;
                    //                    defaultMaskEditInfo.NumberFormatInfoObject = eb.NumberFormatInfoObject;
                    //                    defaultMaskEditInfo.PaddingCharacter = eb.PaddingCharacter;
                    //                    defaultMaskEditInfo.PassivePromptCharacter = eb.PassivePromptCharacter;
                    //                    defaultMaskEditInfo.PromptCharacter = eb.PromptCharacter;
                    //                    defaultMaskEditInfo.SpecialCultureValue = eb.SpecialCultureValue;
                    //                    defaultMaskEditInfo.ThousandSeparator = eb.ThousandSeparator;
                    //                    defaultMaskEditInfo.TimeSeparator = eb.TimeSeparator;
                    //                    defaultMaskEditInfo.UsageMode = eb.UsageMode;
                    //                    defaultMaskEditInfo.UseLocaleDefault = eb.UseLocaleDefault;
                    //                    defaultMaskEditInfo.UseUserOverride = eb.UseUserOverride;
                    //
                    defaultMaskEditInfo.AllowPrompt = false;
                    defaultMaskEditInfo.ClipMode = ClipModes.IncludeLiterals;
                    defaultMaskEditInfo.DateSeparator = '/';
                    defaultMaskEditInfo.DecimalSeparator = '.';
                    defaultMaskEditInfo.Mask = string.Empty;
                    defaultMaskEditInfo.MaxValue = decimal.MaxValue;
                    defaultMaskEditInfo.MinValue = 0;
                    defaultMaskEditInfo.PaddingCharacter = ' ';
                    defaultMaskEditInfo.PassivePromptCharacter = ' ';
                    defaultMaskEditInfo.PromptCharacter = ' ';
                    defaultMaskEditInfo.SpecialCultureValue = SpecialCultureValues.CurrentCulture;
                    defaultMaskEditInfo.ThousandSeparator = ',';
                    defaultMaskEditInfo.TimeSeparator = ':';
                    defaultMaskEditInfo.UsageMode = MaskedUsageMode.Normal;
                    defaultMaskEditInfo.UseLocaleDefault = false;
                    defaultMaskEditInfo.UseUserOverride = true;
                    defaultMaskEditInfo.DateTimeFormatInfoObject = CultureInfo.CurrentCulture.DateTimeFormat;
                    defaultMaskEditInfo.NumberFormatInfoObject = CultureInfo.CurrentCulture.NumberFormat;
                }

                return defaultMaskEditInfo;
            }
        }

        /// <summary>
        /// Returns <see cref="GridMaskEditInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridMaskEditInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties.
        #region AllowPrompt
        /// <summary>
        /// Gets or sets a value indicating whether the prompt character can be allowed to be entered as an input character.
        /// </summary>
        [Description("specifies if the prompt character can be allowed to be entered as an input character."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool AllowPrompt
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridMaskEditInfoStore.AllowPromptProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.AllowPromptProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="AllowPrompt"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowPrompt()
        {
            ResetValue(GridMaskEditInfoStore.AllowPromptProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowPrompt()
        {
            return HasValue(GridMaskEditInfoStore.AllowPromptProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="AllowPrompt"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowPrompt
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.AllowPromptProperty);
            }
        }

        #endregion
        #region ClipMode
        /// <summary>
        /// Gets or sets the format of the text that will be returned by the MaskedEditBox control. The nature of the formatting is set the the <see cref="ClipModes"/> type.
        /// </summary>
        [Description("Specifies the format of the text that will be returned by the MaskedEditBox control."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public ClipModes ClipMode
        {
            [DebuggerStepThrough()]
            get
            {
                return (ClipModes)GetValue(GridMaskEditInfoStore.ClipModeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.ClipModeProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ClipMode"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetClipMode()
        {
            ResetValue(GridMaskEditInfoStore.ClipModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeClipMode()
        {
            return HasValue(GridMaskEditInfoStore.ClipModeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ClipMode"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasClipMode
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.ClipModeProperty);
            }
        }

        #endregion
        #region DateSeparator
        /// <summary>
        /// Gets or sets the character to use when a date separator position is specified.
        /// </summary>
        [Description("Gets or sets the character to use when a date separator position is specified."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char DateSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.DateSeparatorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.DateSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DateSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDateSeparator()
        {
            ResetValue(GridMaskEditInfoStore.DateSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDateSeparator()
        {
            return HasValue(GridMaskEditInfoStore.DateSeparatorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="DateSeparator"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDateSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.DateSeparatorProperty);
            }
        }

        #endregion
        #region DateTimeFormatInfoObject
        /// <summary>
        /// Gets or sets the <see cref="System.Globalization.DateTimeFormatInfo"/> provides the necessary globalization information for the properties that rely on the datetime settings.
        /// </summary>
        [Description("Provides the necessary globalization information for the properties that rely on the datetime settings.")]
        [Category("StyleCategoryAppearance")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTimeFormatInfo DateTimeFormatInfoObject
        {
            [DebuggerStepThrough()]
            get
            {
                return (DateTimeFormatInfo)GetValue(GridMaskEditInfoStore.DateTimeFormatInfoObjectProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.DateTimeFormatInfoObjectProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DateTimeFormatInfoObject"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDateTimeFormatInfoObject()
        {
            ResetValue(GridMaskEditInfoStore.DateTimeFormatInfoObjectProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDateTimeFormatInfoObject()
        {
            return HasValue(GridMaskEditInfoStore.DateTimeFormatInfoObjectProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="DateTimeFormatInfoObject"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDateTimeFormatInfoObject
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.DateTimeFormatInfoObjectProperty);
            }
        }

        #endregion
        #region DecimalSeparator
        /// <summary>
        /// Gets or sets the character to use when a decimal separator position is specified.
        /// </summary>
        [Description("Gets or sets the character to use when a decimal separator position is specified."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char DecimalSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.DecimalSeparatorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.DecimalSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DecimalSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDecimalSeparator()
        {
            ResetValue(GridMaskEditInfoStore.DecimalSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDecimalSeparator()
        {
            return HasValue(GridMaskEditInfoStore.DecimalSeparatorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DecimalSeparator"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDecimalSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.DecimalSeparatorProperty);
            }
        }
        #endregion
        #region Mask
        /// <summary>
        /// Gets or sets the mask string for the MaskedEditBox control.
        /// </summary>
        [Description("Gets or sets the property to define mask string for the MaskedEditBox control."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public string Mask
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditInfoStore.MaskProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.MaskProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Mask"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMask()
        {
            ResetValue(GridMaskEditInfoStore.MaskProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMask()
        {
            return HasValue(GridMaskEditInfoStore.MaskProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Mask"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMask
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.MaskProperty);
            }
        }

        #endregion
        #region MaxValue
        /// <summary>
        /// Gets or sets the Maximum Value that can be set through the MaskedEditBox.
        /// </summary>
        [Description("Specifies the maximum value that can be set through the MaskedEditBox."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public decimal MaxValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (decimal)GetValue(GridMaskEditInfoStore.MaxValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MaxValue"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaxValue()
        {
            ResetValue(GridMaskEditInfoStore.MaxValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaxValue()
        {
            return HasValue(GridMaskEditInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MaxValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaxValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.MaxValueProperty);
            }
        }
        #endregion
        #region MinValue
        /// <summary>
        /// Gets or sets the Minimum Value that can be set through the MaskedEditBox.
        /// </summary>
        [Description("Specifies the minimum value that can be set through the MaskedEditBox."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public decimal MinValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (decimal)GetValue(GridMaskEditInfoStore.MinValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.MinValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MinValue"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMinValue()
        {
            ResetValue(GridMaskEditInfoStore.MinValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMinValue()
        {
            return HasValue(GridMaskEditInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MinValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMinValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.MinValueProperty);
            }
        }

        #endregion
        #region NumberFormatInfoObject
        /// <summary>
        /// Gets or sets the <see cref="System.Globalization.NumberFormatInfo"/> provides the necessary globalization information for the properties that rely on these settings.
        /// </summary>
        [Description("provides the necessary globalization information for the properties that rely on these settings.")]
        [Category("StyleCategoryAppearance")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NumberFormatInfo NumberFormatInfoObject
        {
            [DebuggerStepThrough()]
            get
            {
                return (NumberFormatInfo)GetValue(GridMaskEditInfoStore.NumberFormatInfoObjectProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.NumberFormatInfoObjectProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="NumberFormatInfoObject"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNumberFormatInfoObject()
        {
            ResetValue(GridMaskEditInfoStore.NumberFormatInfoObjectProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNumberFormatInfoObject()
        {
            return HasValue(GridMaskEditInfoStore.NumberFormatInfoObjectProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="NumberFormatInfoObject"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNumberFormatInfoObject
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.NumberFormatInfoObjectProperty);
            }
        }

        #endregion
        #region PaddingCharacter
        /// <summary>
        /// Gets or sets the character that will be used instead of mask characters when the mask position has not been filled.
        /// </summary>
        [Description("Specifies the character that will be used instead of mask characters when the mask position has not been filled."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char PaddingCharacter
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.PaddingCharacterProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.PaddingCharacterProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="PaddingCharacter"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPaddingCharacter()
        {
            ResetValue(GridMaskEditInfoStore.PaddingCharacterProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePaddingCharacter()
        {
            return HasValue(GridMaskEditInfoStore.PaddingCharacterProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PaddingCharacter"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPaddingCharacter
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.PaddingCharacterProperty);
            }
        }

        #endregion
        #region PassivePromptCharacter
        /// <summary>
        /// Gets or sets the character that will be used instead of mask characters when the mask position has not been filled (when the control does not have the focus).
        /// </summary>
        [Description("Specifies the character that will be used instead of mask characters when the mask position has not been filled (when the control does not have the focus)."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char PassivePromptCharacter
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.PassivePromptCharacterProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.PassivePromptCharacterProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="PassivePromptCharacter"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPassivePromptCharacter()
        {
            ResetValue(GridMaskEditInfoStore.PassivePromptCharacterProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePassivePromptCharacter()
        {
            return HasValue(GridMaskEditInfoStore.PassivePromptCharacterProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PassivePromptCharacter"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPassivePromptCharacter
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.PassivePromptCharacterProperty);
            }
        }

        #endregion
        #region PromptCharacter
        /// <summary>
        /// Gets or sets the character that will be used instead of mask characters when the mask position has not been filled.
        /// </summary>
        [Description("Specifies the character that will be used instead of mask characters when the mask position has not been filled."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char PromptCharacter
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.PromptCharacterProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.PromptCharacterProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="PromptCharacter"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPromptCharacter()
        {
            ResetValue(GridMaskEditInfoStore.PromptCharacterProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePromptCharacter()
        {
            return HasValue(GridMaskEditInfoStore.PromptCharacterProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PromptCharacter"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPromptCharacter
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.PromptCharacterProperty);
            }
        }

        #endregion
        #region SpecialCultureValue
        /// <summary>
        /// Gets or sets the mode for the cultures.
        /// </summary>
        [Description("Specifies the mode for the cultures."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public SpecialCultureValues SpecialCultureValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (SpecialCultureValues)GetValue(GridMaskEditInfoStore.SpecialCultureValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.SpecialCultureValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="SpecialCultureValue"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSpecialCultureValue()
        {
            ResetValue(GridMaskEditInfoStore.SpecialCultureValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSpecialCultureValue()
        {
            return HasValue(GridMaskEditInfoStore.SpecialCultureValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="SpecialCultureValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSpecialCultureValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.SpecialCultureValueProperty);
            }
        }

        #endregion
        #region ThousandSeparator
        /// <summary>
        /// Gets or sets the character to use when a thousands separator position is specified.
        /// </summary>
        [Description("Specifies the character to use when a thousands separator position is specified."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char ThousandSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.ThousandSeparatorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.ThousandSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ThousandSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetThousandSeparator()
        {
            ResetValue(GridMaskEditInfoStore.ThousandSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeThousandSeparator()
        {
            return HasValue(GridMaskEditInfoStore.ThousandSeparatorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ThousandSeparator"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasThousandSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.ThousandSeparatorProperty);
            }
        }

        #endregion
        #region TimeSeparator
        /// <summary>
        /// Gets or sets the character to use when a time separator position is specified.
        /// </summary>
        [Description("Specifies the  character to use when a time separator position is specified."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public char TimeSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditInfoStore.TimeSeparatorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.TimeSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="TimeSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTimeSeparator()
        {
            ResetValue(GridMaskEditInfoStore.TimeSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTimeSeparator()
        {
            return HasValue(GridMaskEditInfoStore.TimeSeparatorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="TimeSeparator"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTimeSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.TimeSeparatorProperty);
            }
        }

        #endregion
        #region UsageMode
        /// <summary>
        /// Gets or sets the usage mode for the MaskedEditBox.
        /// </summary>
        [Description("Specifies the usage mode for the MaskedEditBox."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public MaskedUsageMode UsageMode
        {
            [DebuggerStepThrough()]
            get
            {
                return (MaskedUsageMode)GetValue(GridMaskEditInfoStore.UsageModeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.UsageModeProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="UsageMode"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUsageMode()
        {
            ResetValue(GridMaskEditInfoStore.UsageModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUsageMode()
        {
            return HasValue(GridMaskEditInfoStore.UsageModeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="UsageMode"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUsageMode
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.UsageModeProperty);
            }
        }

        #endregion
        #region UseLocaleDefault
        /// <summary>
        /// Gets or sets a value indicating whether the individual globalization property changes are to be ignored. If set to True, the individual values will be ignored and the locale default will be used.
        /// </summary>
        [Description("Specify if the individual globalization property changes are to be ignored."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool UseLocaleDefault
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridMaskEditInfoStore.UseLocaleDefaultProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.UseLocaleDefaultProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="UseLocaleDefault"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUseLocaleDefault()
        {
            ResetValue(GridMaskEditInfoStore.UseLocaleDefaultProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUseLocaleDefault()
        {
            return HasValue(GridMaskEditInfoStore.UseLocaleDefaultProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="UseLocaleDefault"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUseLocaleDefault
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.UseLocaleDefaultProperty);
            }
        }

        #endregion
        #region UseUserOverride
        /// <summary>
        /// Gets or sets a value indicating whether to UseUserOverride parameter for CultureInfo.
        /// </summary>
        [Description("Specifies the UseUserOverride parameter for CultureInfo."),
        Browsable(true),
        Category("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool UseUserOverride
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridMaskEditInfoStore.UseUserOverrideProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditInfoStore.UseUserOverrideProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="UseUserOverride"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUseUserOverride()
        {
            ResetValue(GridMaskEditInfoStore.UseUserOverrideProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUseUserOverride()
        {
            return HasValue(GridMaskEditInfoStore.UseUserOverrideProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="UseUserOverride"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUseUserOverride
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditInfoStore.UseUserOverrideProperty);
            }
        }
        #endregion
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridMaskEditInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridMaskEditInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridMaskEditInfoStore), typeof(GridMaskEditInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.AllowPrompt"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty AllowPromptProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowPrompt");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.ClipMode"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty ClipModeProperty = sd.CreateStyleInfoProperty(typeof(ClipModes), "ClipMode");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.DateSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty DateSeparatorProperty = sd.CreateStyleInfoProperty(typeof(char), "DateSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.DateTimeFormatInfoObject"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty DateTimeFormatInfoObjectProperty = sd.CreateStyleInfoProperty(typeof(DateTimeFormatInfo), "DateTimeFormatInfoObject");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.DecimalSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty DecimalSeparatorProperty = sd.CreateStyleInfoProperty(typeof(char), "DecimalSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.Mask"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MaskProperty = sd.CreateStyleInfoProperty(typeof(string), "Mask");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.MaxValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(decimal), "MaxValue");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.MinValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(decimal), "MinValue");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.NumberFormatInfoObject"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NumberFormatInfoObjectProperty = sd.CreateStyleInfoProperty(typeof(NumberFormatInfo), "NumberFormatInfoObject");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.PaddingCharacter"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty PaddingCharacterProperty = sd.CreateStyleInfoProperty(typeof(char), "PaddingCharacter");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.PassivePromptCharacter"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty PassivePromptCharacterProperty = sd.CreateStyleInfoProperty(typeof(char), "PassivePromptCharacter");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.PromptCharacter"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty PromptCharacterProperty = sd.CreateStyleInfoProperty(typeof(char), "PromptCharacter");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.SpecialCultureValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty SpecialCultureValueProperty = sd.CreateStyleInfoProperty(typeof(SpecialCultureValues), "SpecialCultureValue");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.ThousandSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty ThousandSeparatorProperty = sd.CreateStyleInfoProperty(typeof(char), "ThousandSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.TimeSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty TimeSeparatorProperty = sd.CreateStyleInfoProperty(typeof(char), "TimeSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.UsageMode"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UsageModeProperty = sd.CreateStyleInfoProperty(typeof(MaskedUsageMode), "UsageMode");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.UseLocaleDefault"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UseLocaleDefaultProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseLocaleDefault");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.UseUserOverride"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UseUserOverrideProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseUserOverride");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a new <see cref="GridMaskEditInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridMaskEditInfoStore"/>.
        /// </summary>
        public GridMaskEditInfoStore()
        {
        }

        static GridMaskEditInfoStore()
        {
            NumberFormatInfoObjectProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
            DateTimeFormatInfoObjectProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
        }

        /// <summary>
        /// Initializes a new <see cref="GridMaskEditInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridMaskEditInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        //// Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        //// Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates a copy of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridMaskEditInfoStore();
            CopyTo(target);
            return target;
        }
    }
}
