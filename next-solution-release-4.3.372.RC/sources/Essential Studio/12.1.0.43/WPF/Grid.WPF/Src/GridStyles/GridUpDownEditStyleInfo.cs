#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Styles;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for updown edit control properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridUpDownEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridUpDownEditStyleInfo defaultUpDownEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridUpDownEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridUpDownEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridUpDownEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridUpDownEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridUpDownEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridUpDownEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridUpDownEditStyleInfoStore"/> that holds data for this <see cref="GridUpDownEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridUpDownEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridUpDownEditStyleInfo(StyleInfoSubObjectIdentity identity, GridUpDownEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridUpDownEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridUpDownEditStyleInfo()
            : base(new GridUpDownEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridUpDownEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridUpDownEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultUpDownEditInfo == null)
                {
                    defaultUpDownEditInfo = new GridUpDownEditStyleInfo();
                    defaultUpDownEditInfo.AnimationSpeed = (double)UpDown.AnimationSpeedProperty.DefaultMetadata.DefaultValue;
                    defaultUpDownEditInfo.FocusedBackground = (Brush)UpDown.FocusedBackgroundProperty.DefaultMetadata.DefaultValue;
                    defaultUpDownEditInfo.FocusedBorderBrush = (Brush)UpDown.FocusedBorderBrushProperty.DefaultMetadata.DefaultValue;
                    defaultUpDownEditInfo.FocusedForeground = (Brush)UpDown.FocusedForegroundProperty.DefaultMetadata.DefaultValue;
                    defaultUpDownEditInfo.MaxValue = (double)UpDown.MaxValueProperty.DefaultMetadata.DefaultValue;
                    defaultUpDownEditInfo.MinValue = (double)UpDown.MinValueProperty.DefaultMetadata.DefaultValue;
                    defaultUpDownEditInfo.NegativeForeground = (Brush)UpDown.NegativeForegroundProperty.DefaultMetadata.DefaultValue;                    
                }

                return defaultUpDownEditInfo;
            }
        }
        /// <summary>
        /// Gets or sets the background when control is focused.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.White.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush FocusedBackground
        {
            get
            {
                return (Brush)this.GetValue(GridUpDownEditStyleInfoStore.FocusedBackgroundProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.FocusedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.FocusedBackground"/>.
        /// </summary>
        public void ResetFocusedBackground()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.FocusedBackgroundProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.FocusedBackground"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeFocusedBackground()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.FocusedBackgroundProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.FocusedBackground"/> is initialized for current object.
        /// </summary>
        public bool HasFocusedBackground
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.FocusedBackgroundProperty);
            }
        }
        /// <summary>
        /// Gets or sets the foreground when control is focused.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Black.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush FocusedForeground
        {
            get
            {
                return (Brush)this.GetValue(GridUpDownEditStyleInfoStore.FocusedForegroundProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.FocusedForegroundProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.FocusedForeground"/>.
        /// </summary>
        public void ResetFocusedForeground()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.FocusedForegroundProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.FocusedForeground"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeFocusedForeground()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.FocusedForegroundProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.FocusedForeground"/> is initialized for current object.
        /// </summary>
        public bool HasFocusedForeground
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.FocusedForegroundProperty);
            }
        }
        /// <summary>
        /// Gets or sets the borderBrush when control is focused. 
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Black.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush FocusedBorderBrush
        {
            get
            {
                return (Brush)this.GetValue(GridUpDownEditStyleInfoStore.FocusedBorderBrushProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.FocusedBorderBrushProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.FocusedBorderBrush"/>.
        /// </summary>
        public void ResetFocusedBorderBrush()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.FocusedBorderBrushProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.FocusedBorderBrush"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeFocusedBorderBrush()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.FocusedBorderBrushProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.FocusedBorderBrush"/> is initialized for current object.
        /// </summary>
        public bool HasFocusedBorderBrush
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.FocusedBorderBrushProperty);
            }
        }
        /// <summary>
        /// Gets or sets background of the control when it's value is negative.
        /// </summary>
        /// <value>Type: <see cref="Brush"/></value>
        public Brush NegativeForeground
        {
            get
            {
                return (Brush)this.GetValue(GridUpDownEditStyleInfoStore.NegativeForegroundProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.NegativeForegroundProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.NegativeForeground"/>.
        /// </summary>
        public void ResetNegativeForeground()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.NegativeForegroundProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.NegativeForeground"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeNegativeForeground()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.NegativeForegroundProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.NegativeForeground"/> is initialized for current object.
        /// </summary>
        public bool HasNegativeForeground
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.NegativeForegroundProperty);
            }
        }
        /// <summary>
        /// Gets or sets the animation speed of value change.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.1.
        /// </value>
        /// <seealso cref="double"/>
        public double AnimationSpeed
        {
            get
            {
                return (double)this.GetValue(GridUpDownEditStyleInfoStore.AnimationSpeedProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.AnimationSpeedProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.AnimationSpeed"/>.
        /// </summary>
        public void ResetAnimationSpeed()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.AnimationSpeedProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.AnimationSpeed"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeAnimationSpeed()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.AnimationSpeedProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.AnimationSpeed"/> is initialized for current object.
        /// </summary>
        public bool HasAnimationSpeed
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.AnimationSpeedProperty);
            }
        }
        /// <summary>
        ///  Gets or sets the minimum value for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is double.MinValue.
        /// </value>
        /// <seealso cref="double"/>
        public double MinValue
        {
            get
            {
                return (double)this.GetValue(GridUpDownEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.MinValueProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.MinValue"/>.
        /// </summary>
        public void ResetMinValue()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.MinValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.MinValue"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.MinValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.MinValue"/> is initialized for current object.
        /// </summary>
        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.MinValueProperty);
            }
        }
        /// <summary>
        /// Gets or sets the maximum value for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is double.MaxValue.
        /// </value>
        /// <seealso cref="double"/>
        public double MaxValue
        {
            get
            {
                return (double)this.GetValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.MaxValueProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.MaxValue"/>.
        /// </summary>
        public void ResetMaxValue()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.MaxValue"/> is initialized for current object.
        /// </summary>
        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
            }
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.MaxValue"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
        }
        /// <summary>
        /// Gets or sets the step to increment or decrement the value of the control
        /// when the up or down button is clicked.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 1.
        /// </value>
        /// <seealso cref="double"/>
        public double Step
        {
            get
            {
                return (double)this.GetValue(GridUpDownEditStyleInfoStore.StepProperty);
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.StepProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridUpDownEditStyleInfo.Step"/>.
        /// </summary>
        public void ResetStep()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.StepProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.Step"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeStep()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.StepProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridUpDownEditStyleInfo.Step"/> is initialized for current object.
        /// </summary>
        public bool HasStep
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.StepProperty);
            }
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridUpDownEditStyleInfo(identity, store as GridUpDownEditStyleInfoStore);
            }

            return new GridUpDownEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridUpDownEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridUpDownEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridUpDownEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable]
    [StaticDataField("sd")]
    public class GridUpDownEditStyleInfoStore : StyleInfoStore
    {
        static GridUpDownEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridUpDownEditStyleInfoStore), typeof(GridUpDownEditStyleInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.FocusedBackground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty FocusedBackgroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "FocusedBackground");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.FocusedForeground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty FocusedForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "FocusedForeground");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.FocusedBorderBrush"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty FocusedBorderBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "FocusedBorderBrush");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.NegativeForeground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty NegativeForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "NegativeForeground");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.AnimationSpeed"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty AnimationSpeedProperty = sd.CreateStyleInfoProperty(typeof(double), "AnimationSpeed");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(double), "MinValue");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(double), "MaxValue");

        /// <summary>
        /// Provides information about the <see cref="GridUpDownEditStyleInfo.Step"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty StepProperty = sd.CreateStyleInfoProperty(typeof(double), "Step");

        /// <overload>
        /// Initializes a new <see cref="GridUpDownEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridUpDownEditStyleInfoStore"/>.
        /// </summary>
        public GridUpDownEditStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridUpDownEditStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridUpDownEditStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Returns a copy of current object.</summary>
        /// <returns>A copy of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridUpDownEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}