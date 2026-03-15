//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellValidateValueInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridCellValidateValueInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridCellValidateValueInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridCellValidateValueInfoStore), typeof(GridCellValidateValueInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridCellValidateValueInfo.NumberRequired"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty NumberRequiredProperty = sd.CreateStyleInfoProperty(typeof(bool), "NumberRequired");

        /// <summary>
        /// Provides information about the <see cref="GridCellValidateValueInfo.Minimum"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MinimumProperty = sd.CreateStyleInfoProperty(typeof(double), "Minimum");

        /// <summary>
        /// Provides information about the <see cref="GridCellValidateValueInfo.Maximum"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MaximumProperty = sd.CreateStyleInfoProperty(typeof(double), "Maximum");

        /// <summary>
        /// Provides information about the <see cref="GridCellValidateValueInfo.ErrorMessage"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty ErrorMessageProperty = sd.CreateStyleInfoProperty(typeof(string), "ErrorMessage");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a new <see cref="GridCellValidateValueInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridCellValidateValueInfoStore"/>
        /// </summary>
        public GridCellValidateValueInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellValidateValueInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridCellValidateValueInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>A copy of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridCellValidateValueInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for validation of text entry in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes validation information for cells:
    /// <code lang="C#">
    ///             model[rowIndex, 1].Text = "Positive numbers";
    ///             RowStyles[rowIndex].CustomStyleProperties.Add(new GridValidateNumberStyleProperty(true, 1, float.NaN, "Please enter a number greater than 0!"));
    ///             model.RowStyles[rowIndex].ValidateValue.NumberRequired = true;
    ///             model.RowStyles[rowIndex].ValidateValue.Minimum = 0;
    ///             model.RowStyles[rowIndex].ValidateValue.Maximum = float.NaN;
    ///             model.RowStyles[rowIndex].ValidateValue.ErrorMessage = "Please enter a number greater than 0!";
    ///             rowIndex++;
    ///             model[rowIndex, 1].Text = "Validation (1-100 valid range)";
    ///             model.RowStyles[rowIndex].ValidateValue = new GridCellValidateValueInfo(true, 1, 100, "Please enter a number between 1 and 100!");
    /// </code>
    /// </example>
    public class GridCellValidateValueInfo : GridStyleInfoSubObject
    {
        // Static Fields
        private static GridCellValidateValueInfo defaultValidateInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridCellValidateValueInfo(identity, store as GridCellValidateValueInfoStore);
            }

            return new GridCellValidateValueInfo(identity);
        }

        // Constructors

        /// <overload>
        /// Initializes a new <see cref="GridCellValidateValueInfo"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new GridCellValidateValueInfo object with validation criteria.
        /// </summary>
        /// <param name="numberRequired">true if only number allows; false if any characters</param>
        /// <param name="minimum">The minimum value allowed for the cell.</param>
        /// <param name="maximum">The maximum value allowed for the cell.</param>
        /// <param name="errorMessage">A error message to be displayed if entered text does not meet criteria.</param>
        [DebuggerStepThrough()]
        public GridCellValidateValueInfo(bool numberRequired, double minimum, double maximum, string errorMessage)
            : base(new GridCellValidateValueInfoStore())
        {
            NumberRequired = numberRequired;
            Minimum = minimum;
            Maximum = maximum;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridCellValidateValueInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridCellValidateValueInfo()
            : base(new GridCellValidateValueInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCellValidateValueInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCellValidateValueInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridCellValidateValueInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridCellValidateValueInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCellValidateValueInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCellValidateValueInfo"/>.</param>
        /// <param name="store">A <see cref="GridCellValidateValueInfoStore"/> that holds data for this <see cref="GridCellValidateValueInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridCellValidateValueInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridCellValidateValueInfo(StyleInfoSubObjectIdentity identity, GridCellValidateValueInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Makes an exact copy of the current object.</summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">Identifier for this object.</param>
        /// <returns>Copy of current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridCellValidateValueInfo(newOwner.CreateSubObjectIdentity(sip), (GridCellValidateValueInfoStore)Store.Clone());
        }

        // Default

        /// <summary>
        /// Gets a default <see cref="GridCellValidateValueInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings are: <para/>
        /// <list type="table">
        /// <listheader><term>Property</term><description>Value</description></listheader>
        /// <item><term><see cref="NumberRequired"/></term><description>false</description></item>
        /// <item><term><see cref="Minimum"/></term><description>double.MinValue</description></item>
        /// <item><term><see cref="Maximum"/></term><description>double.MaxValue</description></item>
        /// <item><term><see cref="ErrorMessage"/></term><description>"Value is out of range"</description></item>
        /// </list>
        /// </remarks>
        public static GridCellValidateValueInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultValidateInfo == null)
                {
                    defaultValidateInfo = new GridCellValidateValueInfo();
                    defaultValidateInfo.NumberRequired = false;
                    defaultValidateInfo.Minimum = double.MinValue / 10.0; // have to divide by 10 so that 
                    // Parse routine does not throw exception when you call 
                    // double.Parse(defaultValidateInfo.Minimum.ToString) 
                    defaultValidateInfo.Maximum = double.MaxValue / 10.0;
                    defaultValidateInfo.ErrorMessage = "Value is out of range";
                }

                return defaultValidateInfo;
            }
        }

        /// <summary>
        /// Returns <see cref="GridCellValidateValueInfo.Default"/>
        /// </summary>
        /// <returns>A <see cref="GridCellValidateValueInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        #region NumberRequired
        /// <summary>
        /// Gets or sets a value indicating whether numeric entry is allowed. True if only numeric entry is allowed; false if any characters are allowed.
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("true if only number allows; false if any characters are allowed."),
        NotifyParentProperty(true)]
        public bool NumberRequired
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridCellValidateValueInfoStore.NumberRequiredProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCellValidateValueInfoStore.NumberRequiredProperty, (object)value);
            }
        }

        /// <summary>
        /// Resets the <see cref="NumberRequired"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNumberRequired()
        {
            ResetValue(GridCellValidateValueInfoStore.NumberRequiredProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNumberRequired()
        {
            return HasValue(GridCellValidateValueInfoStore.NumberRequiredProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="NumberRequired"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNumberRequired
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCellValidateValueInfoStore.NumberRequiredProperty);
            }
        }
        #endregion

        #region Minimum
        /// <summary>
        /// Gets or sets the minimum value allowed for the cell.
        /// </summary>
        [Description("The minimum value allowed for the cell"),
        Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public double Minimum
        {
            [DebuggerStepThrough()]
            get
            {
                return (double)GetValue(GridCellValidateValueInfoStore.MinimumProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCellValidateValueInfoStore.MinimumProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Minimum"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMinimum()
        {
            ResetValue(GridCellValidateValueInfoStore.MinimumProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMinimum()
        {
            return HasValue(GridCellValidateValueInfoStore.MinimumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Minimum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMinimum
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCellValidateValueInfoStore.MinimumProperty);
            }
        }

        #endregion
        #region Maximum
        /// <summary>
        /// Gets or sets the maximum value allowed for the cell.
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The maximum value allowed for the cell."),
        NotifyParentProperty(true)]
        public double Maximum
        {
            [DebuggerStepThrough()]
            get
            {
                return (double)GetValue(GridCellValidateValueInfoStore.MaximumProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCellValidateValueInfoStore.MaximumProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Maximum"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaximum()
        {
            ResetValue(GridCellValidateValueInfoStore.MaximumProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaximum()
        {
            return HasValue(GridCellValidateValueInfoStore.MaximumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Maximum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaximum
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCellValidateValueInfoStore.MaximumProperty);
            }
        }

        #endregion
        #region ErrorMessage
        /// <summary>
        /// Gets or sets the mimium value allowed for the cell.
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("A error message to be displayed if entered text does not meet criteria."),
        NotifyParentProperty(true)]
        public string ErrorMessage
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCellValidateValueInfoStore.ErrorMessageProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCellValidateValueInfoStore.ErrorMessageProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="ErrorMessage"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetErrorMessage()
        {
            ResetValue(GridCellValidateValueInfoStore.ErrorMessageProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeErrorMessage()
        {
            return HasValue(GridCellValidateValueInfoStore.ErrorMessageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="ErrorMessage"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasErrorMessage
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCellValidateValueInfoStore.ErrorMessageProperty);
            }
        }
        #endregion
    }
}

