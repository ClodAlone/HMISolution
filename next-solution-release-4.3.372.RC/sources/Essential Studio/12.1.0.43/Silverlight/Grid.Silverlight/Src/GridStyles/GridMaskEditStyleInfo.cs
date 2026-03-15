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
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;
    using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Tools.Controls;

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for masked edit properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridMaskEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridMaskEditStyleInfo defaultMaskEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridMaskEditInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridMaskEditInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridMaskEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridMaskEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridMaskEditInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridMaskEditInfo"/>.</param>
        /// <param name="store">A <see cref="GridMaskEditInfoStore"/> that holds data for this <see cref="GridMaskEditInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridMaskEditInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridMaskEditStyleInfo(StyleInfoSubObjectIdentity identity, GridMaskEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridMaskEditInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridMaskEditStyleInfo()
            : base(new GridMaskEditStyleInfoStore())
        {
        }

        #region CurrencySymbol

        /// <summary>
        /// Use this property to define the CurrencySymbol string for the MaskedTextBox control.
        /// </summary>
        public string CurrencySymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditStyleInfoStore.CurrencySymbolProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.CurrencySymbolProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrencySymbol"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCurrencySymbol()
        {
            ResetValue(GridMaskEditStyleInfoStore.CurrencySymbolProperty);
        }

        /// <summary>
        /// Checks if the <see cref="CurrencySymbol"/> property is initialized.
        /// </summary>
        public bool HasCurrencySymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.CurrencySymbolProperty);
            }
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencySymbol()
        {
            return HasValue(GridMaskEditStyleInfoStore.CurrencySymbolProperty);
        }
        
        #endregion

        #region DateSeparator
        /// <summary>
        /// Use this property to define the date separator for the MaskedEditBox control.
        /// </summary>
        public string DateSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditStyleInfoStore.DateSeparatorProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.DateSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DateSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDateSeparator()
        {
            ResetValue(GridMaskEditStyleInfoStore.DateSeparatorProperty);
        }

        /// <summary>
        /// Checks if the <see cref="DateSeparator"/> property is initialized.
        /// </summary>
        public bool HasDateSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.DateSeparatorProperty);
            }
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDateSeparator()
        {
            return HasValue(GridMaskEditStyleInfoStore.DateSeparatorProperty);
        }
        
        #endregion

        #region DecimalSeparator
        /// <summary>
        /// Use this property to define the decimal separator for the MaskedEditBox control.
        /// </summary>
        public string DecimalSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditStyleInfoStore.DecimalSeparatorProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.DecimalSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Checks if the <see cref="DecimalSeparator"/> property is initialized.
        /// </summary>
        public bool HasDecimalSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.DecimalSeparatorProperty);
            }
        }

        /// <summary>
        /// Resets the <see cref="DecimalSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDecimalSeparator()
        {
            ResetValue(GridMaskEditStyleInfoStore.DecimalSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDecimalSeparator()
        {
            return HasValue(GridMaskEditStyleInfoStore.DecimalSeparatorProperty);
        }
        
        #endregion

        #region StringValidation

        /// <summary>
        /// Gets or sets the String validation.
        /// </summary>
        /// If you set this as StringValidation.OnKeyPress then control will handle the validation part.
        /// if it is set to StringValidation.OnLostFocus then you can handle the vaidation in the CurrentCellValidating event.
        /// <value>The min validation.</value>
        public StringValidation StringValidation
        {
            get
            {
                return (StringValidation)GetValue(GridMaskEditStyleInfoStore.StringValidationProperty);
            }
            set
            {
                SetValue(GridMaskEditStyleInfoStore.StringValidationProperty, value);
            }
        }

        public bool HasStringValidation
        {
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.StringValidationProperty);
            }
        }

        public void ResetStringValidation()
        {
            ResetValue(GridMaskEditStyleInfoStore.StringValidationProperty);
        }

        public bool ShouldSerializeStringValidation()
        {
            return HasValue(GridMaskEditStyleInfoStore.StringValidationProperty);
        }

        #endregion

        /// <summary>
        /// Returns a default <see cref="GridMaskEditInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings are: <para/>
        /// 	<list type="table">
        /// 		<listheader><term>Property</term><description>Value</description></listheader>
        /// 		<item><term><see cref="DateSeparator"/></term><description>'/'</description></item>
        /// 		<item><term><see cref="DecimalSeparator"/></term><description>'.'</description></item>
        /// 		<item><term><see cref="Mask"/></term><description>String.Empty</description></item>
        /// 		<item><term><see cref="TimeSeparator"/></term><description>':'</description></item>
        /// 	</list>
        /// </remarks>
        public static GridMaskEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultMaskEditInfo == null)
                {
                    defaultMaskEditInfo = new GridMaskEditStyleInfo();

                    defaultMaskEditInfo.DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    defaultMaskEditInfo.NumberGroupSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    defaultMaskEditInfo.CurrencySymbol = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol;
                    defaultMaskEditInfo.PromptChar = '_';
                    defaultMaskEditInfo.Mask = "";
                }

                return defaultMaskEditInfo;
            }
        }

        /// <summary>
        /// Checks if the <see cref="Mask"/> property is initialized.
        /// </summary>
        public bool HasMask
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.MaskProperty);
            }
        }

        /// <summary>
        /// Checks if the <see cref="NumberGroupSeparator"/> property is initialized.
        /// </summary>
        public bool HasNumberGroupSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.NumberGroupSeparatorProperty);
            }
        }

        /// <summary>
        /// Checks if the <see cref="PromptChar"/> property is initialized.
        /// </summary>
        public bool HasPromptChar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.PromptCharProperty);
            }
        }

        /// <summary>
        /// Checks if the <see cref="TimeSeparator"/> property is initialized.
        /// </summary>
        public bool HasTimeSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.TimeSeparatorProperty);
            }
        }

        /// <summary>
        /// Checks if the <see cref="MaxLength"/> property is initialized.
        /// </summary>
        public bool HasMaxLength
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.MaxLengthProperty);
            }
        }

        /// <summary>
        /// Checks if the <see cref="MinLength"/> property is initialized.
        /// </summary>
        public bool HasMinLength
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridMaskEditStyleInfoStore.MinLengthProperty);
            }
        }

        /// <summary>
        /// Use this property to define the mask string for the MaskedEditBox control.
        /// </summary>
        public string Mask
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditStyleInfoStore.MaskProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.MaskProperty, value);
            }
        }

        /// <summary>
        /// Use this property to define the number group separator for the MaskedEditBox control.
        /// </summary>
        public string NumberGroupSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditStyleInfoStore.NumberGroupSeparatorProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.NumberGroupSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Use this property to define the PromptChar string for the MaskedTextBox control.
        /// </summary>
        public char PromptChar
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridMaskEditStyleInfoStore.PromptCharProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.PromptCharProperty, value);
            }
        }

        /// <summary>
        /// Use this property to define the time separator for the MaskedEditBox control.
        /// </summary>
        public string TimeSeparator
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridMaskEditStyleInfoStore.TimeSeparatorProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.TimeSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Use this property to define the Maximum Length for the MaskedEditBox control.
        /// </summary>
        public int MaxLength
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridMaskEditStyleInfoStore.MaxLengthProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.MaxLengthProperty, value);
            }
        }

        /// <summary>
        /// Use this property to define the Minimum Length for the MaskedEditBox control.
        /// </summary>
        public int MinLength
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridMaskEditStyleInfoStore.MinLengthProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridMaskEditStyleInfoStore.MinLengthProperty, value);
            }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Makes an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">The identifier for this object.</param>
        /// <returns>The copy of the current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridMaskEditStyleInfo(newOwner.CreateSubObjectIdentity(sip), (GridMaskEditStyleInfoStore)Store.Clone());
        }

        /// <summary>
        /// Resets the <see cref="Mask"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMask()
        {
            ResetValue(GridMaskEditStyleInfoStore.MaskProperty);
        }

        /// <summary>
        /// Resets the <see cref="NumberGroupSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNumberGroupSeparator()
        {
            ResetValue(GridMaskEditStyleInfoStore.NumberGroupSeparatorProperty);
        }

        /// <summary>
        /// Resets the <see cref="PromptChar"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPromptChar()
        {
            ResetValue(GridMaskEditStyleInfoStore.PromptCharProperty);
        }

        /// <summary>
        /// Resets the <see cref="TimeSeparator"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTimeSeparator()
        {
            ResetValue(GridMaskEditStyleInfoStore.TimeSeparatorProperty);
        }

        /// <summary>
        /// Resets the <see cref="MaxLength"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaxLength()
        {
            ResetValue(GridMaskEditStyleInfoStore.MaxLengthProperty);
        }

        /// <summary>
        /// Resets the <see cref="MinLength"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMinLength()
        {
            ResetValue(GridMaskEditStyleInfoStore.MinLengthProperty);
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
                return new GridMaskEditStyleInfo(identity, store as GridMaskEditStyleInfoStore);
            return new GridMaskEditStyleInfo(identity);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMask()
        {
            return HasValue(GridMaskEditStyleInfoStore.MaskProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNumberGroupSeparator()
        {
            return HasValue(GridMaskEditStyleInfoStore.NumberGroupSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePromptChar()
        {
            return HasValue(GridMaskEditStyleInfoStore.PromptCharProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTimeSeparator()
        {
            return HasValue(GridMaskEditStyleInfoStore.TimeSeparatorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaxLength()
        {
            return HasValue(GridMaskEditStyleInfoStore.MaxLengthProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMinLength()
        {
            return HasValue(GridMaskEditStyleInfoStore.MinLengthProperty);
        }

        /// <summary>
        /// Returns <see cref="GridMaskEditInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridMaskEditInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
        // Static Fields


        // Constructors

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.
    }


    /// <summary>
    /// Implements the data store for the <see cref="GridMaskEditInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    public class GridMaskEditStyleInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridMaskEditStyleInfoStore), typeof(GridMaskEditStyleInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.DateSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CurrencySymbolProperty = sd.CreateStyleInfoProperty(typeof(string), "CurrencySymbol");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.DateSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty DateSeparatorProperty = sd.CreateStyleInfoProperty(typeof(string), "DateSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.DecimalSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty DecimalSeparatorProperty = sd.CreateStyleInfoProperty(typeof(string), "DecimalSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.Mask"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MaskProperty = sd.CreateStyleInfoProperty(typeof(string), "Mask");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.NumberGroupSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NumberGroupSeparatorProperty = sd.CreateStyleInfoProperty(typeof(string), "NumberGroupSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.PromptChar"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty PromptCharProperty = sd.CreateStyleInfoProperty(typeof(char), "PromptChar");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.TimeSeparator"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty TimeSeparatorProperty = sd.CreateStyleInfoProperty(typeof(string), "TimeSeparator");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.MaxLength"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MaxLengthProperty = sd.CreateStyleInfoProperty(typeof(int), "MaxLength");

        /// <summary>
        /// Provides information about the <see cref="GridMaskEditInfo.MinCharLength"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MinLengthProperty = sd.CreateStyleInfoProperty(typeof(int), "MinLength");

        public readonly static StyleInfoProperty StringValidationProperty = sd.CreateStyleInfoProperty(typeof(StringValidation), "StringValidation");

        static GridMaskEditStyleInfoStore()
        {
        }

        /// <overload>
        /// Initializes a new <see cref="GridMaskEditInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridMaskEditInfoStore"/>.
        /// </summary>
        public GridMaskEditStyleInfoStore()
        {
        }

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Returns a copy of the current object.</summary>
        /// <returns>A copy of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridMaskEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}