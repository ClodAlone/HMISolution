//-------------------------------------------------------------------------------------------------
// <copyright file="GridNumericUpDownCellInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridNumericUpDownCellInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridNumericUpDownCellInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridNumericUpDownCellInfoStore), typeof(GridNumericUpDownCellInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridNumericUpDownCellInfo.Minimum"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MinimumProperty = sd.CreateStyleInfoProperty(typeof(int), "Minimum");

        /// <summary>
        /// Provides information about the <see cref="GridNumericUpDownCellInfo.Maximum"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty MaximumProperty = sd.CreateStyleInfoProperty(typeof(int), "Maximum");

        /// <summary>
        /// Provides information about the <see cref="GridNumericUpDownCellInfo.Step"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty StepProperty = sd.CreateStyleInfoProperty(typeof(int), "Step");

        /// <summary>
        /// Provides information about the <see cref="GridNumericUpDownCellInfo.StartValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty StartValueProperty = sd.CreateStyleInfoProperty(typeof(int), "StartValue");

        /// <summary>
        /// Provides information about the <see cref="GridNumericUpDownCellInfo.WrapValue"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty WrapValueProperty = sd.CreateStyleInfoProperty(typeof(bool), "WrapValue");

        internal static string[] sortOrder = sd.CreatePropertyGridSortOrder(new string[]
            {
                "Minimum",
                "Maximum",
                "Step",
                "WrapValue",
                "StartValue"
            });

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="GridNumericUpDownCellInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridNumericUpDownCellInfoStore"/>.
        /// </summary>
        public GridNumericUpDownCellInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridNumericUpDownCellInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridNumericUpDownCellInfoStore(SerializationInfo info, StreamingContext context)
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
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for customization of NumericUpDown cells.
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes numeric up / down information for cells:
    /// <code lang="C#">
    ///             model.TableStyle.NumericUpDown = new GridNumericUpDownCellInfo(0, 25, 1, 1, true);
    ///             model[rowIndex, 1].Text = "NumericUpDown";
    ///             // Wrapping, Range 0-1000
    ///             model[rowIndex, 3].CellType = "NumericUpDown";
    ///             model[rowIndex, 3].NumericUpDown = new GridNumericUpDownCellInfo(0, 1000, 0, 1, true);
    ///             // Disabled
    ///             model[rowIndex, 4].CellType = "NumericUpDown";
    ///             model[rowIndex, 4].Enabled = false;
    ///             model[rowIndex, 4].Text = "5";
    ///             // No wrapping, Range 1-20
    ///             model[rowIndex, 5].CellType = "NumericUpDown";
    ///             model[rowIndex, 5].NumericUpDown = new GridNumericUpDownCellInfo(1, 20, 1, 1, false);
    ///             rowIndex++;
    /// </code>
    /// </example>
    public class GridNumericUpDownCellInfo : GridStyleInfoSubObject
    {
        // Static Fields
        private static GridNumericUpDownCellInfo defaultGridNumericUpDownCellInfo;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridNumericUpDownCellInfo(identity, store as GridNumericUpDownCellInfoStore);
            }

            return new GridNumericUpDownCellInfo(identity);
        }

        /// <overload>
        /// Initializes a new GridNumericUpDownCellInfo object.
        /// </overload>
        /// <summary>
        /// Initializes a new GridNumericUpDownCellInfo object with numeric up / down information.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <param name="start">Start value. This is the first value when you press up or down in an empty cell.</param>
        /// <param name="num">The step to increase or decrease when clicking up or down buttons.</param>
        /// <param name="wrap">True if value should be starting over when value reaches maximum or minimum.</param>
        public GridNumericUpDownCellInfo(int min, int max, int start, int num, bool wrap)
            : base(new GridNumericUpDownCellInfoStore())
        {
            WrapValue = wrap;
            Maximum = max;
            Minimum = min;
            StartValue = start;
            Step = num;
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridNumericUpDownCellInfo"/> object.
        /// </summary>
        public GridNumericUpDownCellInfo()
            : base(new GridNumericUpDownCellInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridNumericUpDownCellInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridNumericUpDownCellInfo"/>.
        /// </param>
        public GridNumericUpDownCellInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridNumericUpDownCellInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridNumericUpDownCellInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridNumericUpDownCellInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridNumericUpDownCellInfoStore"/> that holds data for this <see cref="GridNumericUpDownCellInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridNumericUpDownCellInfoStore"/> object.
        /// </param>
        public GridNumericUpDownCellInfo(StyleInfoSubObjectIdentity identity, GridNumericUpDownCellInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates an exact copy of the current object.</summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">An identifier for this object.</param>
        /// <returns>Copied object.</returns>
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridNumericUpDownCellInfo(newOwner.CreateSubObjectIdentity(sip), (GridNumericUpDownCellInfoStore)Store.Clone());
        }

        /// <summary>
        /// Gets a default <see cref="GridNumericUpDownCellInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the default numeric up / down info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// Default settings are: <para/>
        /// <list type="table">
        /// <listheader><term>Property</term><description>Value</description></listheader>
        /// <item><term><see cref="Minimum"/></term><description>0</description></item>
        /// <item><term><see cref="Maximum"/></term><description>int.MaxValue</description></item>
        /// <item><term><see cref="WrapValue"/></term><description>false</description></item>
        /// <item><term><see cref="Step"/></term><description>1</description></item>
        /// <item><term><see cref="StartValue"/></term><description>0</description></item>
        /// </list>
        /// </remarks>
        public static GridNumericUpDownCellInfo Default
        {
            get
            {
                if (defaultGridNumericUpDownCellInfo == null)
                {
                    defaultGridNumericUpDownCellInfo = new GridNumericUpDownCellInfo();
                    defaultGridNumericUpDownCellInfo.Minimum = 0;
                    defaultGridNumericUpDownCellInfo.Maximum = int.MaxValue;
                    defaultGridNumericUpDownCellInfo.WrapValue = false;
                    defaultGridNumericUpDownCellInfo.Step = 1;
                    defaultGridNumericUpDownCellInfo.StartValue = 0;
                }

                return defaultGridNumericUpDownCellInfo;
            }
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        #region WrapValue
        /// <summary>
        /// Gets or sets a value indicating whether to wrap. True if value should be starting over when value reaches maximum or minimum.
        /// </summary>
        [Browsable(true),
        Description("true if value should be starting over when value reaches maximum or minimum."),
        SRCategory("StyleCategoryAppearance")]
        public bool WrapValue
        {
            get
            {
                return (bool)GetValue(GridNumericUpDownCellInfoStore.WrapValueProperty);
            }

            set
            {
                SetValue(GridNumericUpDownCellInfoStore.WrapValueProperty, (object)value);
            }
        }

        /// <summary>
        /// Resets the <see cref="WrapValue"/> property.
        /// </summary>
        public void ResetWrapValue()
        {
            ResetValue(GridNumericUpDownCellInfoStore.WrapValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeWrapValue()
        {
            return HasValue(GridNumericUpDownCellInfoStore.WrapValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="WrapValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasWrapValue
        {
            get
            {
                return HasValue(GridNumericUpDownCellInfoStore.WrapValueProperty);
            }
        }

        #endregion
        #region Minimum
        /// <summary>
        /// Gets or sets minimum value.
        /// </summary>
        [Browsable(true),
        Description("Minimum value"),
        SRCategory("StyleCategoryAppearance")]
        public int Minimum
        {
            get
            {
                return (int)GetValue(GridNumericUpDownCellInfoStore.MinimumProperty);
            }

            set
            {
                SetValue(GridNumericUpDownCellInfoStore.MinimumProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Minimum"/> property.
        /// </summary>
        public void ResetMinimum()
        {
            ResetValue(GridNumericUpDownCellInfoStore.MinimumProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMinimum()
        {
            return HasValue(GridNumericUpDownCellInfoStore.MinimumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Minimum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMinimum
        {
            get
            {
                return HasValue(GridNumericUpDownCellInfoStore.MinimumProperty);
            }
        }

        #endregion
        #region Maximum
        /// <summary>
        /// Gets or sets maximum value.
        /// </summary>
        [Browsable(true),
        Description("Maximum value."),
        SRCategory("StyleCategoryAppearance")]
        public int Maximum
        {
            get
            {
                return (int)GetValue(GridNumericUpDownCellInfoStore.MaximumProperty);
            }

            set
            {
                SetValue(GridNumericUpDownCellInfoStore.MaximumProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Maximum"/> property.
        /// </summary>
        public void ResetMaximum()
        {
            ResetValue(GridNumericUpDownCellInfoStore.MaximumProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaximum()
        {
            return HasValue(GridNumericUpDownCellInfoStore.MaximumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Maximum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaximum
        {
            get
            {
                return HasValue(GridNumericUpDownCellInfoStore.MaximumProperty);
            }
        }

        #endregion
        #region Step
        /// <summary>
        /// Gets or sets the step to increase or decrease when clicking up or down buttons.
        /// </summary>
        [Browsable(true),
        Description("The step to increase or decrease when clicking up or down buttons."),
        SRCategory("StyleCategoryAppearance")]
        public int Step
        {
            get
            {
                return (int)GetValue(GridNumericUpDownCellInfoStore.StepProperty);
            }

            set
            {
                SetValue(GridNumericUpDownCellInfoStore.StepProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Step"/> property.
        /// </summary>
        public void ResetStep()
        {
            ResetValue(GridNumericUpDownCellInfoStore.StepProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStep()
        {
            return HasValue(GridNumericUpDownCellInfoStore.StepProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Step"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStep
        {
            get
            {
                return HasValue(GridNumericUpDownCellInfoStore.StepProperty);
            }
        }

        #endregion
        #region StartValue
        /// <summary>
        /// Gets or sets start value. This is the first value when you press up or down in an empty cell.
        /// </summary>
        [Browsable(true),
        Description("Start value. This is the first value when you press up or down in an empty cell."),
        SRCategory("StyleCategoryAppearance")]
        public int StartValue
        {
            get
            {
                return (int)GetValue(GridNumericUpDownCellInfoStore.StartValueProperty);
            }

            set
            {
                SetValue(GridNumericUpDownCellInfoStore.StartValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="StartValue"/> property.
        /// </summary>
        public void ResetStartValue()
        {
            ResetValue(GridNumericUpDownCellInfoStore.StartValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStartValue()
        {
            return HasValue(GridNumericUpDownCellInfoStore.StartValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="StartValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStartValue
        {
            get
            {
                return HasValue(GridNumericUpDownCellInfoStore.StartValueProperty);
            }
        }
        #endregion
    }
}

