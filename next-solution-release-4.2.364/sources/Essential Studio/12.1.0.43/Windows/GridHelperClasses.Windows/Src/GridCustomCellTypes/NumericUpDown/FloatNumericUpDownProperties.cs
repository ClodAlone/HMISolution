//-------------------------------------------------------------------------------------------------
// <copyright file="FloatNumericUpDownProperties.cs" company="Syncfusion">
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
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.IO;
    using System.Drawing.Imaging;
    using System.ComponentModel;

    using Syncfusion.Diagnostics;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Styles;

    // #region CustomProperties_with expandable objects

    /// <summary>
    /// Defines the properties for FloatNumericUpDown cell.
    /// </summary>
    public class FloatNumericUpDownStyleProperties : GridStyleInfoCustomProperties
    {
        // static initialization of property descriptors
        static Type t = typeof(FloatNumericUpDownStyleProperties);

        readonly static StyleInfoProperty NumericUpDownInfoProperty = CreateStyleInfoProperty(t, "FloatNumericUpDownProperties");

        // default settings for all properties this object holds
        static FloatNumericUpDownStyleProperties defaultObject;

        // initialize default settings for all properties in static ctor
        static FloatNumericUpDownStyleProperties()
        {
            // all properties must be initialized for the Default property
            defaultObject = new FloatNumericUpDownStyleProperties(GridStyleInfo.Default);
            defaultObject.FloatNumericUpDownProperties = FloatNumericUpDownProperties.Default;
        }

        /// <summary>
        /// Gets access to default values for this type
        /// </summary>
        public static FloatNumericUpDownStyleProperties Default
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
        /// Returns the properties of specifed FloatNumericUpDown cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>The properties for FloatNumericUpDown cell.</returns>
        public static explicit operator FloatNumericUpDownStyleProperties(GridStyleInfo style)
        {
            return new FloatNumericUpDownStyleProperties(style);
        }

        /// <summary>
        /// Constructor for FloatNumericUpDownStyleProperties.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        public FloatNumericUpDownStyleProperties(GridStyleInfo style)
            : base(style)
        {
        }

        /// <summary>
        /// Constructor for FloatNumericUpDownStyleProperties.
        /// </summary>
        public FloatNumericUpDownStyleProperties()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the properties of FloatNumericUpDown control.
        /// </summary>
        [Description("Numeric Up Down properties"),
        Browsable(true),
        Category("Custom")]
        public FloatNumericUpDownProperties FloatNumericUpDownProperties
        {
            get
            {
                return (FloatNumericUpDownProperties)style.GetValue(NumericUpDownInfoProperty);
            }

            set
            {
                style.SetValue(NumericUpDownInfoProperty, value);
            }
        }
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    [Serializable,
    StaticDataField("sd")]
    public class NumericUpDownStyleStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(NumericUpDownStyleStore), typeof(FloatNumericUpDownProperties), true);
        /// <summary>
        /// Default value for NumericMinimum.
        /// </summary>
        public readonly static StyleInfoProperty NumericMinimumProperty = sd.CreateStyleInfoProperty(typeof(double), "NumericMinimum");
        /// <summary>
        /// Default value for NumericMaximum.
        /// </summary>
        public readonly static StyleInfoProperty NumericMaximumProperty = sd.CreateStyleInfoProperty(typeof(double), "NumericMaximum");
        /// <summary>
        /// Default value for NumericStep.
        /// </summary>
        public readonly static StyleInfoProperty NumericStepProperty = sd.CreateStyleInfoProperty(typeof(double), "NumericStep");
        /// <summary>
        /// Default value for NumericStartValue.
        /// </summary>
        public readonly static StyleInfoProperty NumericStartValueProperty = sd.CreateStyleInfoProperty(typeof(double), "NumericStartValue");
        /// <summary>
        /// Default value for NumericWrapValue.
        /// </summary>
        public readonly static StyleInfoProperty NumericWrapValueProperty = sd.CreateStyleInfoProperty(typeof(double), "NumericWrapValue");
        /// <summary>
        /// Default value for NumericDecimalPlaces.
        /// </summary>
        public readonly static StyleInfoProperty NumericDecimalPlacesProperty = sd.CreateStyleInfoProperty(typeof(int), "NumericDecimalPlaces");
        /// <summary>
        /// Default value for NumericOrientation.
        /// </summary>
        public readonly static StyleInfoProperty NumericOrientationProperty = sd.CreateStyleInfoProperty(typeof(int), "NumericOrientation");
        /// <summary>
        /// Default value for NumericThousandsSeperator.
        /// </summary>
        public readonly static StyleInfoProperty NumericThousandsSeparatorProperty = sd.CreateStyleInfoProperty(typeof(bool), "NumericThousandsSeparator");
        /// <summary>
        /// Default value for NumericInterceptArrowkeys.
        /// </summary>
        public readonly static StyleInfoProperty NumericInterceptArrowkeysProperty = sd.CreateStyleInfoProperty(typeof(bool), "NumericInterceptArrowkeys");

        /// <summary>
        /// Gets the static data which must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }

        /// <summary>
        /// Constructor for NumericUpDownStyleStore.
        /// </summary>
        public NumericUpDownStyleStore()
        {
        }

        private NumericUpDownStyleStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Syncfusion.Styles.StyleInfoStore"/> with same data as the current object.
        /// </returns>
        public override object Clone()
        {
            StyleInfoStore target = new NumericUpDownStyleStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Defines the properties for FloatNumericUpDown cell control.
    /// </summary>
    [TypeConverter(typeof(StyleInfoBaseConverter))]
    public class FloatNumericUpDownProperties : GridStyleInfoSubObject
    {
        //// Static Fields
        private static FloatNumericUpDownProperties defaultNumericUpDownProperties;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new FloatNumericUpDownProperties(identity, store as NumericUpDownStyleStore);
            }

            return new FloatNumericUpDownProperties(identity);
        }

        /// <summary>
        /// Releases all the resources used by the component.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Constructor for FloatNumericUpDownProperties.
        /// </summary>
        [DebuggerStepThrough()]
        public FloatNumericUpDownProperties()
            : base(new NumericUpDownStyleStore())
        {
        }

        /// <summary>
        /// Constructor for FloatNumericUpDownProperties.
        /// </summary>
        /// <param name="identity">Style info identity.</param>
        [DebuggerStepThrough()]
        public FloatNumericUpDownProperties(StyleInfoSubObjectIdentity identity)
            : base(identity, new NumericUpDownStyleStore())
        {
        }

        /// <summary>
        /// Cosntructor for FloatNumericUpDownProperties.
        /// </summary>
        /// <param name="identity">Style info identity.</param>
        /// <param name="store">Style info store.</param>
        [DebuggerStepThrough()]
        public FloatNumericUpDownProperties(StyleInfoSubObjectIdentity identity, NumericUpDownStyleStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Cosntructor for FloatNumericUpDownProperties.
        /// </summary>
        /// <param name="font">The Font value.</param>
        [DebuggerStepThrough()]
        public FloatNumericUpDownProperties(Font font)
            : base(new NumericUpDownStyleStore())
        {
        }

        /// <summary>
        /// Gets an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object. </param>
        /// <param name="sip">The identifier for this object. </param>
        /// <returns>The FloatNumericUpDownProperties</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new FloatNumericUpDownProperties(newOwner.CreateSubObjectIdentity(sip), (NumericUpDownStyleStore)Store.Clone());
        }

        /// <summary>
        /// Gets the default value of the properties.
        /// </summary>
        /// <value>The default NumericUpDownProperties.</value>
        public static FloatNumericUpDownProperties Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultNumericUpDownProperties == null)
                {
                    defaultNumericUpDownProperties = new FloatNumericUpDownProperties();
                    defaultNumericUpDownProperties.Maximum = double.MaxValue;
                    defaultNumericUpDownProperties.Minimum = double.MinValue;
                    defaultNumericUpDownProperties.StartValue = 0;
                    defaultNumericUpDownProperties.Step = 1;
                    defaultNumericUpDownProperties.WrapValue = false;

                    //// New
                    defaultNumericUpDownProperties.DecimalPlaces = 0;
                    defaultNumericUpDownProperties.Orientation = OrientationType.Vertical;
                    defaultNumericUpDownProperties.ThousandsSeparator = false;
                    defaultNumericUpDownProperties.InterceptArrowkeys = false;
                }

                return defaultNumericUpDownProperties;
            }
        }

        /// <summary>
        ///  Notifies the associated identity object that a specific property was changed 
        ///  and raises a Syncfusion.Styles.StyleInfoBase.Changed event.
        /// </summary>
        /// <param name="sip">Identifies the property to look for</param>
        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            base.OnStyleChanged(sip);
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
        /// Gets or sets a value indicating whether if the value should be starting over when value reaches maximum or minimum when true.
        /// </summary>
        [Browsable(true),
        Description("true if value should be starting over when value reaches maximum or minimum.")]
        public bool WrapValue
        {
            get
            {
                return (bool)GetValue(NumericUpDownStyleStore.NumericWrapValueProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericWrapValueProperty, (object)value);
            }
        }

        /// <summary>
        /// Resets the <see cref="WrapValue"/> property.
        /// </summary>
        public void ResetWrapValue()
        {
            ResetValue(NumericUpDownStyleStore.NumericWrapValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether to serialize wrap value.
        /// </summary>
        /// <returns>return boolean value</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeWrapValue()
        {
            return HasValue(NumericUpDownStyleStore.NumericWrapValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="WrapValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasWrapValue
        {
            get
            {
                return HasValue(NumericUpDownStyleStore.NumericWrapValueProperty);
            }
        }

        #endregion

        #region Minimum

        /// <summary>
        /// Gets or sets Minimum value.
        /// </summary>
        [Browsable(true),
        Description("Minimum value")]
        public double Minimum
        {
            get
            {
                return (double)GetValue(NumericUpDownStyleStore.NumericMinimumProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericMinimumProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Minimum"/> property.
        /// </summary>
        public void ResetMinimum()
        {
            ResetValue(NumericUpDownStyleStore.NumericMaximumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether to serialize minimum.
        /// </summary>
        /// <returns>returns boolean value</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMinimum()
        {
            return HasValue(NumericUpDownStyleStore.NumericMinimumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether if <see cref="Minimum"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMinimum
        {
            get
            {
                return HasValue(NumericUpDownStyleStore.NumericMinimumProperty);
            }
        }

        #endregion

        #region Maximum

        /// <summary>
        /// Gets or sets the Maximum value.
        /// </summary>
        [Browsable(true),
        Description("Maximum value.")]
        public double Maximum
        {
            get
            {
                return (double)GetValue(NumericUpDownStyleStore.NumericMaximumProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericMaximumProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Maximum"/> property.
        /// </summary>
        public void ResetMaximum()
        {
            ResetValue(NumericUpDownStyleStore.NumericMaximumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether to serialize maximum.
        /// </summary>
        /// <returns>returns boolean value</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaximum()
        {
            return HasValue(NumericUpDownStyleStore.NumericMaximumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Maximum"/> property is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has maximum; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaximum
        {
            get
            {
                return HasValue(NumericUpDownStyleStore.NumericMaximumProperty);
            }
        }

        #endregion

        #region Step

        /// <summary>
        /// Gets or sets the step to increase or decrease when clicking up or down buttons.
        /// </summary>
        [Browsable(true),
        Description("The step to increase or decrease when clicking up or down buttons.")]
        public double Step
        {
            get
            {
                return (double)GetValue(NumericUpDownStyleStore.NumericStepProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericStepProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Step"/> property.
        /// </summary>
        public void ResetStep()
        {
            ResetValue(NumericUpDownStyleStore.NumericStepProperty);
        }

        /// <summary>
        /// Gets a value indicating whether to serialize step.
        /// </summary>
        /// <returns>return boolean value</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStep()
        {
            return HasValue(NumericUpDownStyleStore.NumericStepProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Step"/> property is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance has step; otherwise, <c>false</c>.</value>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStep
        {
            get
            {
                return HasValue(NumericUpDownStyleStore.NumericStepProperty);
            }
        }

        #endregion

        #region StartValue

        /// <summary>
        /// Gets or sets the start value. This is the first value when you press up or down in an empty cell.
        /// </summary>
        [Browsable(true),
        Description("Start value. This is the first value when you press up or down in an empty cell.")]
        public double StartValue
        {
            get
            {
                return (double)GetValue(NumericUpDownStyleStore.NumericStartValueProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericStartValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="StartValue"/> property.
        /// </summary>
        public void ResetStartValue()
        {
            ResetValue(NumericUpDownStyleStore.NumericStartValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether to serialize start value.
        /// </summary>
        /// <returns>returns a boolean value</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStartValue()
        {
            return HasValue(NumericUpDownStyleStore.NumericStartValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="StartValue"/> property is initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStartValue
        {
            get
            {
                return HasValue(NumericUpDownStyleStore.NumericStartValueProperty);
            }
        }

        #endregion

        /// <summary>
        /// Specifies the OrientationType
        /// </summary>
        public enum OrientationType
        {
            /// <summary>
            /// Represents vertical orientation
            /// </summary>
            Vertical = 0,

            /// <summary>
            /// Represents horizontal orientation
            /// </summary>
            Horizontal = 1,
        }

        // Properties
        #region Orientation

        /// <summary>
        /// Gets or sets a value indicating the orientation for the control.
        /// </summary>
        public OrientationType Orientation
        {
            get
            {
                return (OrientationType)GetValue(NumericUpDownStyleStore.NumericOrientationProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericOrientationProperty, value);
            }
        }

        #endregion

        #region  DecimalPlaces

        /// <summary>
        /// Gets or sets the number of digits after the decimal point.
        /// </summary>
        public int DecimalPlaces
        {
            get
            {
                return (int)GetValue(NumericUpDownStyleStore.NumericDecimalPlacesProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericDecimalPlacesProperty, value);
            }
        }

        #endregion

        #region Thousands Separator

        /// <summary>
        /// Gets or sets a value indicating whether the width of the button
        /// </summary>
        [Description("Specifies the width of the button"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public bool ThousandsSeparator
        {
            get
            {
                return (bool)GetValue(NumericUpDownStyleStore.NumericThousandsSeparatorProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericThousandsSeparatorProperty, value);
            }
        }

        #endregion

        #region Intercept Arrowkeys
        /// <summary>
        /// Gets or sets a value indicating whether the Up and Down keys Performs the same function as up and down arrow keys
        /// </summary>
       
        [Description("Up and down keys Performs the same function as up and down arrow keys"),
        Browsable(true),
        Category("Custom"),
        NotifyParentProperty(true)]
        public bool InterceptArrowkeys
        {
            get
            {
                return (bool)GetValue(NumericUpDownStyleStore.NumericInterceptArrowkeysProperty);
            }

            set
            {
                SetValue(NumericUpDownStyleStore.NumericInterceptArrowkeysProperty, value);
            }
        }
        #endregion
    }
}
