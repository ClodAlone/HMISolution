//-------------------------------------------------------------------------------------------------
// <copyright file="GridCurrencyEditInfo.cs" company="syncfusion">
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
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for currency textbox properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridCurrencyEditInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridCurrencyEditInfo defaultCurrencyEditInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridCurrencyEditInfo(identity, store as GridCurrencyEditInfoStore);
            }

            return new GridCurrencyEditInfo(identity);
        }

        // Constructors

        /// <summary>
        /// Initializes a new empty <see cref="GridCurrencyEditInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridCurrencyEditInfo()
            : base(new GridCurrencyEditInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCurrencyEditInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridCurrencyEditInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridCurrencyEditInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCurrencyEditInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditInfo"/>.</param>
        /// <param name="store">A <see cref="GridCurrencyEditInfoStore"/> that holds data for this <see cref="GridCurrencyEditInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridCurrencyEditInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridCurrencyEditInfo(StyleInfoSubObjectIdentity identity, GridCurrencyEditInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Makes an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">The identifier for this object.</param>
        /// <returns>Copy of current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridCurrencyEditInfo(newOwner.CreateSubObjectIdentity(sip), (GridCurrencyEditInfoStore)Store.Clone());
        }

        // Default

        /// <summary>
        /// Gets a default <see cref="GridCurrencyEditInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings for US culture are: <para/>
        /// <list type="table">
        /// <listheader><term>Property</term><description>Value</description></listheader>
        /// <item><term><see cref="UseCultureInfo"/></term><description>false</description></item>
        /// <item><term><see cref="CurrencyDecimalDigits"/></term><description>2</description></item>
        /// <item><term><see cref="NegativeSign"/></term><description>"-"</description></item>
        /// <item><term><see cref="CurrencyDecimalSeparator"/></term><description>"."</description></item>
        /// <item><term><see cref="CurrencyGroupSeparator"/></term><description>","</description></item>
        /// <item><term><see cref="CurrencyGroupSizes"/></term><description></description></item>
        /// <item><term><see cref="CurrencyNegativePattern"/></term><description>0</description></item>
        /// <item><term><see cref="CurrencyPositivePattern"/></term><description>0</description></item>
        /// <item><term><see cref="CurrencySymbol"/></term><description>"$"</description></item>
        /// <item><term><see cref="CurrencyNumberDigits"/></term><description>27</description></item>
        /// <item><term><see cref="NegativeColor"/></term><description>Red</description></item>
        /// <item><term><see cref="PositiveColor"/></term><description>Black</description></item>
        /// <item><term><see cref="ClipMode"/></term><description>ExcludeFormatting</description></item>
        /// <item><term><see cref="NullString"/></term><description>"0"</description></item>
        /// </list>
        /// </remarks>
        public static GridCurrencyEditInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultCurrencyEditInfo == null)
                {
                    defaultCurrencyEditInfo = new GridCurrencyEditInfo();

                    //// retrive default values from an empty masked edit box.
                    ////                    CurrencyTextBox eb = new CurrencyTextBox();
                    ////                    ////defaultCurrencyEditInfo.SpecialCultureValue = eb.SpecialCultureValue;
                    ////                    defaultCurrencyEditInfo.NumberFormatInfoObject = eb.NumberFormatInfoObject;
                    ////
                    ////                    //// setting NumberFormatInfoObject will force setting these properties:
                    ////                    //// -->
                    ////                    ////defaultCurrencyEditInfo.CurrencyDecimalDigits = eb.CurrencyDecimalDigits;
                    ////                    ////defaultCurrencyEditInfo.NegativeSign = eb.NegativeSign;
                    ////                    ////defaultCurrencyEditInfo.CurrencyDecimalSeparator = eb.CurrencyDecimalSeparator;
                    ////                    ////defaultCurrencyEditInfo.CurrencyGroupSeparator = eb.CurrencyGroupSeparator;
                    ////                    ////defaultCurrencyEditInfo.CurrencyGroupSizes = eb.CurrencyGroupSizes;
                    ////                    ////defaultCurrencyEditInfo.CurrencyNegativePattern = eb.CurrencyNegativePattern;
                    ////                    ////defaultCurrencyEditInfo.CurrencyPositivePattern = eb.CurrencyPositivePattern;
                    ////                    ////defaultCurrencyEditInfo.CurrencySymbol = eb.CurrencySymbol;
                    ////NumberDecimalSeparator
                    ////                    //// <-- 
                    ////
                    ////                    defaultCurrencyEditInfo.CurrencyNumberDigits = eb.CurrencyNumberDigits;
                    ////                    defaultCurrencyEditInfo.UseCultureInfo = false; //// eb.UseCultureInfo;
                    ////                    defaultCurrencyEditInfo.PositiveColor = eb.PositiveColor;
                    ////                    defaultCurrencyEditInfo.NegativeColor = eb.NegativeColor;
                    ////                    defaultCurrencyEditInfo.ClipMode = CurrencyClipModes.ExcludeFormatting;
                    ////                    defaultCurrencyEditInfo.NullString = eb.NullString;

                    defaultCurrencyEditInfo.NumberFormatInfoObject = CultureInfo.CurrentCulture.NumberFormat;
                    defaultCurrencyEditInfo.CurrencyNumberDigits = 27;
                    defaultCurrencyEditInfo.UseCultureInfo = false;
                    defaultCurrencyEditInfo.NegativeColor = Color.Red;
                    defaultCurrencyEditInfo.PositiveColor = SystemColors.ControlText;
                    defaultCurrencyEditInfo.ClipMode = CurrencyClipModes.ExcludeFormatting;
                    defaultCurrencyEditInfo.NullString = "0";
                    defaultCurrencyEditInfo.NullValue = DBNull.Value;
                }

                return defaultCurrencyEditInfo;
            }
        }

        /// <summary>
        /// Returns <see cref="GridCurrencyEditInfo.Default"/>
        /// </summary>
        /// <returns>A <see cref="GridCurrencyEditInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        //        #region SpecialCultureValue
        //        /// <summary>
        //        /// The mode for the cultures
        //        /// </summary>
        //        [
        //        Browsable(true),
        //        Category("Appearance"),
        //        RefreshProperties(RefreshProperties.Repaint)
        //        ]
        //        public SpecialCultureValues SpecialCultureValue
        //        {
        //            [DebuggerStepThrough()] 
        //            get 
        //            {
        //                return (SpecialCultureValues) GetValue(GridCurrencyEditInfoStore.SpecialCultureValueProperty);
        //            }
        //            [DebuggerStepThrough()] 
        //            set 
        //            {
        //                SetValue(GridCurrencyEditInfoStore.SpecialCultureValueProperty, value);
        //            }
        //        }
        //        /// <summary>
        //        /// Resets the <see cref="SpecialCultureValue"/> property.
        //        /// </summary>
        //        [DebuggerStepThrough()] public void ResetSpecialCultureValue()
        //        {
        //            ResetValue(GridCurrencyEditInfoStore.SpecialCultureValueProperty);
        //        }
        //        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        //        private bool ShouldSerializeSpecialCultureValue()
        //        {
        //            return HasValue(GridCurrencyEditInfoStore.SpecialCultureValueProperty);
        //        }
        //        /// <summary>
        //        /// Checks if <see cref="SpecialCultureValue"/> property is initialized.
        //        /// </summary>
        //        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //        public bool HasSpecialCultureValue
        //        {
        //            [DebuggerStepThrough()] 
        //            get
        //            {
        //                return HasValue(GridCurrencyEditInfoStore.SpecialCultureValueProperty);
        //            }
        //        }
        //        #endregion
        #region NumberFormatInfoObject
        /// <summary>
        /// Gets or sets the NumberFormaInfo object that will be used for formatting the
        /// currency string.
        /// </summary>
        /// <remarks>
        /// This property will affect or reflect the settings of the following properties
        /// in this object:
        /// <list type="table">
        /// <listheader><term>Property</term><description>Default Value</description></listheader>
        /// <item><term><see cref="CurrencyDecimalDigits"/></term><description>2</description></item>
        /// <item><term><see cref="NegativeSign"/></term><description>"-"</description></item>
        /// <item><term><see cref="CurrencyDecimalSeparator"/></term><description>"."</description></item>
        /// <item><term><see cref="CurrencyGroupSeparator"/></term><description>","</description></item>
        /// <item><term><see cref="CurrencyGroupSizes"/></term><description></description></item>
        /// <item><term><see cref="CurrencyNegativePattern"/></term><description>0</description></item>
        /// <item><term><see cref="CurrencyPositivePattern"/></term><description>0</description></item>
        /// <item><term><see cref="CurrencySymbol"/></term><description>"$"</description></item>
        /// </list>
        /// </remarks>
        [Browsable(false),
        Category("Appearance"),
        Description("The NumberFormaInfo object that will be used for formatting the currency string."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        NotifyParentProperty(true)]
        public NumberFormatInfo NumberFormatInfoObject
        {
            [DebuggerStepThrough()]
            get
            {
                GridStyleInfo style = ((StyleInfoSubObjectIdentity)this.Identity).Owner as GridStyleInfo;
                CultureInfo ci = (style != null) ? style.GetCulture(true) : CultureInfo.CurrentCulture;
                if (this.UseCultureInfo)
                {
                    return ci.NumberFormat;
                }
                else
                {
                    NumberFormatInfo nfi = (NumberFormatInfo)ci.NumberFormat.Clone(); ////new NumberFormatInfo();
                    nfi.CurrencyDecimalDigits = this.CurrencyDecimalDigits;
                    nfi.NegativeSign = this.NegativeSign;
                    nfi.CurrencyDecimalSeparator = this.CurrencyDecimalSeparator;
                    nfi.CurrencyGroupSeparator = this.CurrencyGroupSeparator;
                    nfi.CurrencyGroupSizes = this.CurrencyGroupSizes;
                    nfi.CurrencyNegativePattern = this.CurrencyNegativePattern;
                    nfi.CurrencyPositivePattern = this.CurrencyPositivePattern;
                    nfi.CurrencySymbol = this.CurrencySymbol;
                    nfi.NumberDecimalDigits = this.CurrencyDecimalDigits;
                    nfi.NumberDecimalSeparator = this.CurrencyDecimalSeparator;
                    nfi.NumberGroupSeparator = this.CurrencyGroupSeparator;
                    nfi.NumberGroupSizes = this.CurrencyGroupSizes;
                    ////                    nfi.NumberNegativePattern = this.CurrencyNegativePattern;
                    ////                    nfi.PercentDecimalDigits = this.CurrencyDecimalDigits;
                    ////                    nfi.PercentDecimalSeparator = this.CurrencyDecimalSeparator;
                    ////                    nfi.PercentGroupSeparator = this.CurrencyGroupSeparator;
                    ////                    nfi.PercentGroupSizes = this.CurrencyGroupSizes;
                    ////                    nfi.PercentNegativePattern = this.CurrencyNegativePattern;
                    ////                    nfi.PercentPositivePattern = this.CurrencyPositivePattern;
                    return nfi;
                }
            }

            [DebuggerStepThrough()]
            set
            {
                this.CurrencyDecimalDigits = value.CurrencyDecimalDigits;
                this.NegativeSign = value.NegativeSign;
                this.CurrencyDecimalSeparator = value.CurrencyDecimalSeparator;
                this.CurrencyGroupSeparator = value.CurrencyGroupSeparator;
                this.CurrencyGroupSizes = value.CurrencyGroupSizes;
                this.CurrencyNegativePattern = value.CurrencyNegativePattern;
                this.CurrencyPositivePattern = value.CurrencyPositivePattern;
                this.CurrencySymbol = value.CurrencySymbol;
            }
        }
        #endregion
        #region CurrencyDecimalDigits
        /// <summary>
        /// Gets or sets the maximum number of digits for the decimal portion of the currency.
        /// </summary>
        /// <remarks>
        /// The US dollar requires 2 decimal points to accommodate the smallest 
        /// denomination and this property will have the value 2 in this case. 
        /// </remarks>
        [Description("Gets or sets the maximum number of digits for the decimal portion of the currency."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public int CurrencyDecimalDigits
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridCurrencyEditInfoStore.CurrencyDecimalDigitsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyDecimalDigitsProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyDecimalDigits"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyDecimalDigits()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyDecimalDigitsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyDecimalDigits()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyDecimalDigitsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyDecimalDigits"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyDecimalDigits
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyDecimalDigitsProperty);
            }
        }

        #endregion
        #region NegativeSign
        /// <summary>
        /// Gets or sets the sign that is to be used to indicate a negative value.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Specifies the sign that is to be used to indicate a negative value."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public string NegativeSign
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCurrencyEditInfoStore.NegativeSignProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.NegativeSignProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="NegativeSign"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNegativeSign()
        {
            ResetValue(GridCurrencyEditInfoStore.NegativeSignProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNegativeSign()
        {
            return HasValue(GridCurrencyEditInfoStore.NegativeSignProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="NegativeSign"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNegativeSign
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.NegativeSignProperty);
            }
        }

        #endregion
        #region CurrencyDecimalSeparator
        /// <summary>
        /// Gets or sets the decimal separator character that will be used for the display.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Gets or sets the decimal separator character that will be used for the display."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public string CurrencyDecimalSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCurrencyEditInfoStore.CurrencyDecimalSeparatorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyDecimalSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyDecimalSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyDecimalSeparator()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyDecimalSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyDecimalSeparator()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyDecimalSeparatorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyDecimalSeparator"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyDecimalSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyDecimalSeparatorProperty);
            }
        }

        #endregion
        #region CurrencyGroupSeparator
        /// <summary>
        /// Gets or sets this property specifies the separator to be used for grouping digits.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Gets or sets the separator to be used for grouping digits."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public string CurrencyGroupSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCurrencyEditInfoStore.CurrencyGroupSeparatorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyGroupSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyGroupSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyGroupSeparator()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyGroupSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyGroupSeparator()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyGroupSeparatorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyGroupSeparator"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyGroupSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyGroupSeparatorProperty);
            }
        }

        #endregion
        #region CurrencyGroupSizes
        /// <summary>
        /// Gets or sets this property specifies the grouping of CurrencyDigits in the CurrencyTextBox.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Gets or sets the grouping of CurrencyDigits in the CurrencyTextBox."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public int[] CurrencyGroupSizes
        {
            [DebuggerStepThrough()]
            get
            {
                return (int[])GetValue(GridCurrencyEditInfoStore.CurrencyGroupSizesProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyGroupSizesProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyGroupSizes"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyGroupSizes()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyGroupSizesProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyGroupSizes()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyGroupSizesProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyGroupSizes"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyGroupSizes
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyGroupSizesProperty);
            }
        }

        #endregion
        #region CurrencyNegativePattern
        /// <summary>
        /// Gets or sets this property specifies the pattern to use when the value is negative.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Gets or sets the pattern to use when the value is negative."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public int CurrencyNegativePattern
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridCurrencyEditInfoStore.CurrencyNegativePatternProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyNegativePatternProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyNegativePattern"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyNegativePattern()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyNegativePatternProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyNegativePattern()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyNegativePatternProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyNegativePattern"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyNegativePattern
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyNegativePatternProperty);
            }
        }

        #endregion
        #region CurrencyPositivePattern
        /// <summary>
        /// Gets or sets this property specifies the pattern to use when the value is positive.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Gets or Sets the pattern to use when the value is positive."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public int CurrencyPositivePattern
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridCurrencyEditInfoStore.CurrencyPositivePatternProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyPositivePatternProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyPositivePattern"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyPositivePattern()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyPositivePatternProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyPositivePattern()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyPositivePatternProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyPositivePattern"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyPositivePattern
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyPositivePatternProperty);
            }
        }

        #endregion
        #region CurrencySymbol
        /// <summary>
        /// Gets or sets this property specifies the currency symbol to be used in the CurrencyTextBox.
        /// </summary>
        /// <remarks>
        /// This value is initially set from the <see cref="System.Globalization.NumberFormatInfo"/> 
        /// and can be changed based on your requirements or based on the locale.
        /// </remarks>
        [Description("Gets or sets the currency symbol to be used in the CurrencyTextBox."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public string CurrencySymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCurrencyEditInfoStore.CurrencySymbolProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencySymbolProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencySymbol"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencySymbol()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencySymbolProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencySymbol()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencySymbolProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencySymbol"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencySymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencySymbolProperty);
            }
        }

        #endregion
        #region CurrencyNumberDigits
        /// <summary>
        /// Gets or sets the number of digits for the number part. This is not part of the globalization structure.
        /// </summary>
        /// <remarks>
        /// This value is initially set based on the maximum value of the
        /// Currency data type.
        /// </remarks>
        [Description("Gets/Sets the number of digits for the number part.This value is initially set based on the maximum value of the Currency data type."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        NotifyParentProperty(true)]
        public int CurrencyNumberDigits
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridCurrencyEditInfoStore.CurrencyNumberDigitsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.CurrencyNumberDigitsProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencyNumberDigits"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencyNumberDigits()
        {
            ResetValue(GridCurrencyEditInfoStore.CurrencyNumberDigitsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyNumberDigits()
        {
            return HasValue(GridCurrencyEditInfoStore.CurrencyNumberDigitsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CurrencyNumberDigits"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyNumberDigits
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.CurrencyNumberDigitsProperty);
            }
        }

        #endregion
        #region UseCultureInfo
        /// <summary>
        /// Gets or sets a value indicating whether NumberFormat should be based on Grid's Style or individual properties of this object
        /// true if <see cref="NumberFormatInfo"/> should be based on <see cref="GridStyleInfo.CultureInfo"/>;
        /// false if <see cref="NumberFormatInfo"/> should be based on individual properties of this object.
        /// </summary>
        [Description("Specifies whether the NumberFormat should be based on Grid's Style or individual properties of this object."),
        Browsable(true),
        Category("Culture"),
        NotifyParentProperty(true)]
        public bool UseCultureInfo
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridCurrencyEditInfoStore.UseCultureInfoProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.UseCultureInfoProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="UseCultureInfo"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUseCultureInfo()
        {
            ResetValue(GridCurrencyEditInfoStore.UseCultureInfoProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUseCultureInfo()
        {
            return HasValue(GridCurrencyEditInfoStore.UseCultureInfoProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="UseCultureInfo"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUseCultureInfo
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.UseCultureInfoProperty);
            }
        }

        #endregion
        #region NegativeColor
        /// <summary>
        /// Gets or sets this property specifies the forecolor when the current value is negative.
        /// </summary>
        /// <remarks>
        /// You can customize the look and provide feedback to the user by defining
        /// a different color for the negative numbers.
        /// </remarks>
        [Description("Gets/Sets the forecolor when the current value is negative."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        NotifyParentProperty(true)]
        public Color NegativeColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridCurrencyEditInfoStore.NegativeColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.NegativeColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="NegativeColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNegativeColor()
        {
            ResetValue(GridCurrencyEditInfoStore.NegativeColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNegativeColor()
        {
            return HasValue(GridCurrencyEditInfoStore.NegativeColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="NegativeColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNegativeColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.NegativeColorProperty);
            }
        }

        #endregion
        #region PositiveColor
        /// <summary>
        /// Gets or sets this property specifies the forecolor when the current value is positive.
        /// </summary>
        /// <remarks>
        /// You can customize the look and provide feedback to the user by defining
        /// a different color for the positive numbers.
        /// </remarks>
        [Description("Gets/Sets the forecolor when the current value is positive."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        NotifyParentProperty(true)]
        public Color PositiveColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridCurrencyEditInfoStore.PositiveColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.PositiveColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="PositiveColor"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPositiveColor()
        {
            ResetValue(GridCurrencyEditInfoStore.PositiveColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePositiveColor()
        {
            return HasValue(GridCurrencyEditInfoStore.PositiveColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PositiveColor"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPositiveColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.PositiveColorProperty);
            }
        }

        #endregion
        #region ClipMode
        /// <summary>
        /// Gets or sets whether to include or exclude the literal characters in the input mask when doing a  or copy command.
        /// </summary>
        /// <remarks>
        /// This property is used when copying to the clipboard and also the
        /// <see cref="Control.Text"/> property.
        /// <para>
        /// When databinding the Text property it is advisable to have the ClipMode
        /// set to <see cref="CurrencyClipModes.ExcludeFormatting"/> in cases where
        /// the data source does not accept the formatted text.
        /// </para>
        /// </remarks>
        [Description("Determines whether to include or exclude the literal characters in the input mask when doing a  or copy command."),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        NotifyParentProperty(true)]
            ////        DefaultValue(CurrencyClipModes.ExcludeFormatting)        
        public CurrencyClipModes ClipMode
        {
            [DebuggerStepThrough()]
            get
            {
                return (CurrencyClipModes)GetValue(GridCurrencyEditInfoStore.ClipModeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.ClipModeProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ClipMode"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetClipMode()
        {
            ResetValue(GridCurrencyEditInfoStore.ClipModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeClipMode()
        {
            return HasValue(GridCurrencyEditInfoStore.ClipModeProperty);
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
                return HasValue(GridCurrencyEditInfoStore.ClipModeProperty);
            }
        }

        #endregion
        #region NullString
        /// <summary>
        /// Gets or sets the null string to be displayed
        /// </summary>
        [Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Specify the string to be displayed when the DecimalValue is 0."),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public string NullString
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCurrencyEditInfoStore.NullStringProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.NullStringProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="NullString"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNullString()
        {
            ResetValue(GridCurrencyEditInfoStore.NullStringProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNullString()
        {
            return HasValue(GridCurrencyEditInfoStore.NullStringProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="NullString"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNullString
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.NullStringProperty);
            }
        }
        #endregion
        #region NullValue
        /// <summary>
        /// Gets or sets the value to be saved when the modified display text matches the null string.
        /// </summary>
        [Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Specify the value to be saved when the modified display text matches the null string."),
        RefreshProperties(RefreshProperties.Repaint),
        NotifyParentProperty(true)]
        public object NullValue
        {
            [DebuggerStepThrough()]
            get
            {
                return GetValue(GridCurrencyEditInfoStore.NullValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCurrencyEditInfoStore.NullValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="NullValue"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNullValue()
        {
            ResetValue(GridCurrencyEditInfoStore.NullValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNullValue()
        {
            return HasValue(GridCurrencyEditInfoStore.NullValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="NullValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNullValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCurrencyEditInfoStore.NullValueProperty);
            }
        }
        #endregion
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridCurrencyEditInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridCurrencyEditInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridCurrencyEditInfoStore), typeof(GridCurrencyEditInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyDecimalDigits"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyDecimalDigitsProperty = sd.CreateStyleInfoProperty(typeof(int), "CurrencyDecimalDigits");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.NegativeSign"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NegativeSignProperty = sd.CreateStyleInfoProperty(typeof(string), "NegativeSign");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyDecimalSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyDecimalSeparatorProperty = sd.CreateStyleInfoProperty(typeof(string), "CurrencyDecimalSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyGroupSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyGroupSeparatorProperty = sd.CreateStyleInfoProperty(typeof(string), "CurrencyGroupSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyGroupSizes"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyGroupSizesProperty = sd.CreateStyleInfoProperty(typeof(int[]), "CurrencyGroupSizes");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyNegativePattern"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyNegativePatternProperty = sd.CreateStyleInfoProperty(typeof(int), "CurrencyNegativePattern");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyPositivePattern"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyPositivePatternProperty = sd.CreateStyleInfoProperty(typeof(int), "CurrencyPositivePattern");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencySymbol"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencySymbolProperty = sd.CreateStyleInfoProperty(typeof(string), "CurrencySymbol");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.CurrencyNumberDigits"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencyNumberDigitsProperty = sd.CreateStyleInfoProperty(typeof(int), "CurrencyNumberDigits");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.UseCultureInfo"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UseCultureInfoProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseCultureInfo");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.NegativeColor"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NegativeColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "NegativeColor");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.PositiveColor"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty PositiveColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "PositiveColor");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.ClipMode"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty ClipModeProperty = sd.CreateStyleInfoProperty(typeof(CurrencyClipModes), "ClipMode");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.NullString"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NullStringProperty = sd.CreateStyleInfoProperty(typeof(string), "NullString");

        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditInfo.NullValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NullValueProperty = sd.CreateStyleInfoProperty(typeof(object), "NullValue");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a new <see cref="GridCurrencyEditInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridCurrencyEditInfoStore"/>
        /// </summary>
        public GridCurrencyEditInfoStore()
        {
        }

        static GridCurrencyEditInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCurrencyEditInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridCurrencyEditInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>Copied object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridCurrencyEditInfoStore();
            CopyTo(target);
            return target;
        }
    }
}
