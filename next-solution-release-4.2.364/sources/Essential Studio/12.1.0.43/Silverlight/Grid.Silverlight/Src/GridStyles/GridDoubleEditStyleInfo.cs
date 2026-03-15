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
    using System.Windows;
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
    using Syncfusion.Windows.Tools.Controls;
    // using Syncfusion.Windows.Shared;
    // using Syncfusion.Windows.Styles;

    public class GridDoubleEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields 
        private static GridDoubleEditStyleInfo defaultDoubleEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridDoubleEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridDoubleEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridDoubleEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridDoubleEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridDoubleEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridDoubleEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridDoubleEditStyleInfoStore"/> that holds data for this <see cref="GridDoubleEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridDoubleEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridDoubleEditStyleInfo(StyleInfoSubObjectIdentity identity, GridDoubleEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridDoubleEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridDoubleEditStyleInfo()
            : base(new GridDoubleEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridDoubleEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridDoubleEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultDoubleEditInfo == null)
                {
                    defaultDoubleEditInfo = new GridDoubleEditStyleInfo();
                    defaultDoubleEditInfo.MaxValue = Double.MaxValue;
                    defaultDoubleEditInfo.MinValue = Double.MinValue;
                    defaultDoubleEditInfo.IsScrollingOnCircle = true;
                    defaultDoubleEditInfo.UseNullOption = false;
                }

                return defaultDoubleEditInfo;
            }
        }

        #region UseNullOption
        public bool UseNullOption
        {
            get
            {
                return (bool)this.GetValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
            }

            set
            {
                this.SetValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty, value);
            }
        }

        public void ResetUseNullOption()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
        }

        private bool ShouldSerializeUseNullOption()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
        }

        public bool HasUseNullOption
        {
            get
            {
                return this.HasValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
            }
        }

        #endregion

        public Double MaxValue
        {
            get
            {
                return (Double)this.GetValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridDoubleEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        public void ResetMaxValue()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
        }

        private bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
        }

        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
            }
        }

        public Double MinValue
        {
            get
            {
                return (Double)this.GetValue(GridDoubleEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridDoubleEditStyleInfoStore.MinValueProperty, value);
            }
        }

        public void ResetMinValue()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.MinValueProperty);
        }

        private bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.MinValueProperty);
        }

        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridDoubleEditStyleInfoStore.MinValueProperty);
            }
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridDoubleEditStyleInfo(identity, store as GridDoubleEditStyleInfoStore);
            }

            return new GridDoubleEditStyleInfo(identity);
        }

        #region MinValidation && MaxValidation

        /// <summary>
        /// Gets or sets the Min validation.
        /// </summary>
        /// If you set this as MinValidation.OnKeyPress then control will handle the validation part.
        /// if it is set to MinValidation.OnLostFocus then you can handle the vaidation in the CurrentCellValidating event.
        /// <value>The min validation.</value>
        public MinValidation MinValidation
        {
            get
            {
                return (MinValidation)GetValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
            }
            set
            {
                SetValue(GridDoubleEditStyleInfoStore.MinValidationProperty, value);
            }
        }

        public bool HasMinValidation
        {
            get
            {
                return HasValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
            }
        }

        public void ResetMinValidation()
        {
            ResetValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
        }

        public bool ShouldSerializeMinValidation()
        {
            return HasValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
        }

        /// <summary>
        /// Gets or sets the Max validation.
        /// </summary>
        /// If you set this as MaxValidation.OnKeyPress then control will handle the validation part.
        /// if it is set to MaxValidation.OnLostFocus then you can handle the vaidation in the CurrentCellValidating event.
        /// <value>The min validation.</value>
        public MaxValidation MaxValidation
        {
            get
            {
                return (MaxValidation)GetValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
            }
            set
            {
                SetValue(GridDoubleEditStyleInfoStore.MaxValidationProperty, value);
            }
        }

        public bool HasMaxValidation
        {
            get
            {
                return HasValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
            }
        }

        public void ResetMaxValidation()
        {
            ResetValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
        }

        public bool ShouldSerializeMaxValidation()
        {
            return HasValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
        }

        #endregion

        #region IsScrollingOnCircle
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scrolling on circle.
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get
            {
                return (bool)this.GetValue(GridDoubleEditStyleInfoStore.IsScrollingOnCircleProperty);
            }

            set
            {
                this.SetValue(GridDoubleEditStyleInfoStore.IsScrollingOnCircleProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsScrollingOnCircle property.
        /// </summary>
        public void ResetIsScrollingOnCircle()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsScrollingOnCircle()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is initialized.
        /// </summary>
        public bool HasIsScrollingOnCircle
        {
            get
            {
                return this.HasValue(GridDoubleEditStyleInfoStore.IsScrollingOnCircleProperty);
            }
        }

        #endregion

        /// <summary>
        /// Returns <see cref="GridDoubleEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridDoubleEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridDoubleEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    //[Serializable]
    [StaticDataField("sd")]
    public class GridDoubleEditStyleInfoStore : StyleInfoStore
    {
        static GridDoubleEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridDoubleEditStyleInfoStore), typeof(GridDoubleEditStyleInfo), true);
        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(Double), "MaxValue");
        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");
        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(Double), "MinValue");

        public static readonly StyleInfoProperty MinValidationProperty = sd.CreateStyleInfoProperty(typeof(MinValidation), "MinValidation");

        public static readonly StyleInfoProperty MaxValidationProperty = sd.CreateStyleInfoProperty(typeof(MaxValidation), "MaxValidation");

        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <overload>
        /// Initializes a new <see cref="GridDoubleEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridDoubleEditStyleInfoStore"/>.
        /// </summary>
        public GridDoubleEditStyleInfoStore()
        {
        }

        //#if !SyncfusionFramework4_0
        //        /// <summary>
        //        /// Initializes a new <see cref="GridDoubleEditStyleInfoStore"/> from a serialization stream.
        //        /// </summary>
        //        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        //        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        //        [System.Security.SecurityCritical()]
        //        protected GridDoubleEditStyleInfoStore(SerializationInfo info, StreamingContext context)
        //            : base(info, context)
        //        {
        //        }
        //#endif
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new GridDoubleEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}