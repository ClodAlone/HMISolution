//-------------------------------------------------------------------------------------------------
// <copyright file="GridCheckBoxCellInfo.cs" company="syncfusion">
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
    /// Implements the data store for the <see cref="GridCheckBoxCellInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridCheckBoxCellInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridCheckBoxCellInfoStore), typeof(GridCheckBoxCellInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridCheckBoxCellInfo.CheckedValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty CheckedValueProperty = sd.CreateStyleInfoProperty(typeof(string), "CheckedValue");

        /// <summary>
        /// Provides information about the <see cref="GridCheckBoxCellInfo.UncheckedValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UncheckedValueProperty = sd.CreateStyleInfoProperty(typeof(string), "UncheckedValue");

        /// <summary>
        /// Provides information about the <see cref="GridCheckBoxCellInfo.IndetermValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty IndetermValueProperty = sd.CreateStyleInfoProperty(typeof(string), "IndetermValue");

        /// <summary>
        /// Provides information about the <see cref="GridCheckBoxCellInfo.FlatLook"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty FlatLookProperty = sd.CreateStyleInfoProperty(typeof(bool), "FlatLook");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a new <see cref="GridCheckBoxCellInfo"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridCheckBoxCellInfo"/>
        /// </summary>
        public GridCheckBoxCellInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCheckBoxCellInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridCheckBoxCellInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates an exact copy of the current object.</summary>
        /// <returns>Copy of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridCheckBoxCellInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object with options to customize
    /// checkbox cell type behavior in a cell. <para/>
    /// Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes checkbox behavior for cells:
    /// <code lang="C#">
    ///             model.TableStyle.CheckBoxOptions = new GridCheckBoxCellInfo("True", "False", string.Empty, false);
    ///             model[rowIndex, 1].CheckBoxOptions.FlatLook = true;
    /// </code>
    /// </example>
    public class GridCheckBoxCellInfo : GridStyleInfoSubObject
    {
        // Static Fields
        private static GridCheckBoxCellInfo defaultGridCheckBoxCellInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridCheckBoxCellInfo(identity, store as GridCheckBoxCellInfoStore);
            }

            return new GridCheckBoxCellInfo(identity);
        }

        /// <overload>
        /// Initializes a new <see cref="GridCheckBoxCellInfo"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new GridCellValidateValueInfo object with checkbox options.
        /// </summary>
        /// <param name="checkedValue">The text value that represents checked state.</param>
        /// <param name="uncheckedValue">The text value that represents unchecked state.</param>
        /// <param name="indetermValue">The text value that represents indeterminated state.</param>
        /// <param name="flatLook">true if you want to draw flat checkbox; false otherwise.</param>
        [DebuggerStepThrough()]
        public GridCheckBoxCellInfo(string checkedValue, string uncheckedValue, string indetermValue, bool flatLook)
            : base(new GridCheckBoxCellInfoStore())
        {
            CheckedValue = checkedValue;
            UncheckedValue = uncheckedValue;
            IndetermValue = indetermValue;
            FlatLook = flatLook;
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridCheckBoxCellInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridCheckBoxCellInfo()
            : base(new GridCheckBoxCellInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCheckBoxCellInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCheckBoxCellInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridCheckBoxCellInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridCheckBoxCellInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCheckBoxCellInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCheckBoxCellInfo"/>.</param>
        /// <param name="store">A <see cref="GridCheckBoxCellInfoStore"/> that holds data for this <see cref="GridCheckBoxCellInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridCheckBoxCellInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public GridCheckBoxCellInfo(StyleInfoSubObjectIdentity identity, GridCheckBoxCellInfoStore store)
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
        /// <param name="sip">the identifier for this object.</param>
        /// <returns>A copy of current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridCheckBoxCellInfo(newOwner.CreateSubObjectIdentity(sip), (GridCheckBoxCellInfoStore)Store.Clone());
        }

        // Default

        /// <summary>
        /// Gets a default <see cref="GridCheckBoxCellInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the default border info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings are: <para/>
        /// <list type="table">
        /// <listheader><term>Property</term><description>Value</description></listheader>
        /// <item><term><see cref="CheckedValue"/></term><description>"1"</description></item>
        /// <item><term><see cref="UncheckedValue"/></term><description>"0"</description></item>
        /// <item><term><see cref="IndetermValue"/></term><description>""</description></item>
        /// <item><term><see cref="FlatLook"/></term><description>true</description></item>
        /// </list>
        /// </remarks>
        public static GridCheckBoxCellInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultGridCheckBoxCellInfo == null)
                {
                    defaultGridCheckBoxCellInfo = new GridCheckBoxCellInfo();
                    defaultGridCheckBoxCellInfo.CheckedValue = "1";
                    defaultGridCheckBoxCellInfo.UncheckedValue = "0";
                    defaultGridCheckBoxCellInfo.IndetermValue = string.Empty;
                    defaultGridCheckBoxCellInfo.FlatLook = true;
                }

                return defaultGridCheckBoxCellInfo;
            }
        }

        /// <summary>
        /// Returns <see cref="GridCheckBoxCellInfo.Default"/>
        /// </summary>
        /// <returns>A <see cref="GridCheckBoxCellInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        #region FlatLook
        /// <summary>
        /// Gets or sets a value indicating whether checkbox shall be drawn with flat-look
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("Specifies if checkbox shall be drawn with flat-look"),
        NotifyParentProperty(true)]
        public bool FlatLook
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(GridCheckBoxCellInfoStore.FlatLookProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCheckBoxCellInfoStore.FlatLookProperty, (object)value);
            }
        }

        /// <summary>
        /// Resets the <see cref="FlatLook"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFlatLook()
        {
            ResetValue(GridCheckBoxCellInfoStore.FlatLookProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFlatLook()
        {
            return HasValue(GridCheckBoxCellInfoStore.FlatLookProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="FlatLook"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFlatLook
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCheckBoxCellInfoStore.FlatLookProperty);
            }
        }

        #endregion
        #region CheckedValue
        /// <summary>
        /// Gets or sets the text value that represents checked state.
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The text value that represents checked state."),
        NotifyParentProperty(true)]
        public string CheckedValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCheckBoxCellInfoStore.CheckedValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCheckBoxCellInfoStore.CheckedValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="CheckedValue"/> property
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCheckedValue()
        {
            ResetValue(GridCheckBoxCellInfoStore.CheckedValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCheckedValue()
        {
            return HasValue(GridCheckBoxCellInfoStore.CheckedValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="CheckedValue"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCheckedValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCheckBoxCellInfoStore.CheckedValueProperty);
            }
        }

        #endregion
        #region UncheckedValue

        /// <summary>
        /// Gets or sets the text value that represents unchecked state.
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The text value that represents unchecked state."),
        NotifyParentProperty(true)]
        public string UncheckedValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCheckBoxCellInfoStore.UncheckedValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCheckBoxCellInfoStore.UncheckedValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="UncheckedValue"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUncheckedValue()
        {
            ResetValue(GridCheckBoxCellInfoStore.UncheckedValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUncheckedValue()
        {
            return HasValue(GridCheckBoxCellInfoStore.UncheckedValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="UncheckedValue"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUncheckedValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCheckBoxCellInfoStore.UncheckedValueProperty);
            }
        }

        #endregion
        #region IndetermValue

        /// <summary>
        /// Gets or sets the text value that represents indeterminated state.
        /// </summary>
        [Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        Description("The text value that represents indeterminated state."),
        NotifyParentProperty(true)]
        public string IndetermValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridCheckBoxCellInfoStore.IndetermValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridCheckBoxCellInfoStore.IndetermValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="IndetermValue"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetIndetermValue()
        {
            ResetValue(GridCheckBoxCellInfoStore.IndetermValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeIndetermValue()
        {
            return HasValue(GridCheckBoxCellInfoStore.IndetermValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="IndetermValue"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasIndetermValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridCheckBoxCellInfoStore.IndetermValueProperty);
            }
        }
        #endregion
    }
}

