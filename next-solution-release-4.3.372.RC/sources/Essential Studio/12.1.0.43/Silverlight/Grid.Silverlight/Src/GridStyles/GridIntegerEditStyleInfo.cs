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

    public class GridIntegerEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridIntegerEditStyleInfo defaultIntEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridIntegerEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridIntegerEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridIntegerEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridIntegerEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridIntegerEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridIntegerEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridIntegerEditStyleInfoStore"/> that holds data for this <see cref="GridIntegerEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridIntegerEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridIntegerEditStyleInfo(StyleInfoSubObjectIdentity identity, GridIntegerEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridIntegerEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridIntegerEditStyleInfo()
            : base(new GridIntegerEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridIntegerEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridIntegerEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultIntEditInfo == null)
                {
                    defaultIntEditInfo = new GridIntegerEditStyleInfo();
                    defaultIntEditInfo.MaxValue = Int64.MaxValue;
                    defaultIntEditInfo.MinValue = Int64.MinValue;
                    defaultIntEditInfo.IsScrollingOnCircle = true;
                    defaultIntEditInfo.UseNullOption = false;
                }

                return defaultIntEditInfo;
            }
        }

        #region UseNullOption

        public bool UseNullOption
        {
            get
            {
                return (bool)this.GetValue(GridIntegerEditStyleInfoStore.UseNullOptionProperty);
            }

            set
            {
                this.SetValue(GridIntegerEditStyleInfoStore.UseNullOptionProperty, value);
            }
        }

        public void ResetUseNullOption()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.UseNullOptionProperty);
        }

        private bool ShouldSerializeUseNullOption()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.UseNullOptionProperty);
        }

        public bool HasUseNullOption
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.UseNullOptionProperty);
            }
        }
        #endregion

        public Int64 MaxValue
        {
            get
            {
                return (Int64)this.GetValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridIntegerEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        public void ResetMaxValue()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
        }

        private bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
        }

        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
            }
        }

        public Int64 MinValue
        {
            get
            {
                return (Int64)this.GetValue(GridIntegerEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridIntegerEditStyleInfoStore.MinValueProperty, value);
            }
        }

        public void ResetMinValue()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.MinValueProperty);
        }

        private bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.MinValueProperty);
        }

        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.MinValueProperty);
            }
        }

        #region MinValidation

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
                return (MinValidation)GetValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
            }
            set
            {
                SetValue(GridIntegerEditStyleInfoStore.MinValidationProperty, value);
            }
        }

        public bool HasMinValidation
        {
            get
            {
                return HasValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
            }
        }

        public void ResetMinValidation()
        {
            ResetValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
        }

        public bool ShouldSerializeMinValidation()
        {
            return HasValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
        }

        #endregion

        #region MaxValidation

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
                return (MaxValidation)GetValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
            }
            set
            {
                SetValue(GridIntegerEditStyleInfoStore.MaxValidationProperty, value);
            }
        }

        public bool HasMaxValidation
        {
            get
            {
                return HasValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
            }
        }

        public void ResetMaxValidation()
        {
            ResetValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
        }

        public bool ShouldSerializeMaxValidation()
        {
            return HasValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
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
                return (bool)this.GetValue(GridIntegerEditStyleInfoStore.IsScrollingOnCircleProperty);
            }

            set
            {
                this.SetValue(GridIntegerEditStyleInfoStore.IsScrollingOnCircleProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsScrollingOnCircle property.
        /// </summary>
        public void ResetIsScrollingOnCircle()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsScrollingOnCircle()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is initialized.
        /// </summary>
        public bool HasIsScrollingOnCircle
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.IsScrollingOnCircleProperty);
            }
        }

        #endregion

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridIntegerEditStyleInfo(identity, store as GridIntegerEditStyleInfoStore);
            }

            return new GridIntegerEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridIntegerEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridIntegerEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridIntegerEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
     //[Serializable]
    [StaticDataField("sd")]
    public class GridIntegerEditStyleInfoStore : StyleInfoStore
    {
        static GridIntegerEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridIntegerEditStyleInfoStore), typeof(GridIntegerEditStyleInfo), true);

        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");
        /// <summary>
        /// Provides information about the <see cref="GridIntegerEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(Int64), "MaxValue");

        /// <summary>
        /// Provides information about the <see cref="GridIntegerEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(Int64), "MinValue");

        public static readonly StyleInfoProperty MinValidationProperty = sd.CreateStyleInfoProperty(typeof(MinValidation), "MinValidation");

        public static readonly StyleInfoProperty MaxValidationProperty = sd.CreateStyleInfoProperty(typeof(MaxValidation), "MaxValidation");

        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <overload>
        /// Initializes a new <see cref="GridIntegerEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridIntegerEditStyleInfoStore"/>.
        /// </summary>
        public GridIntegerEditStyleInfoStore()
        {
        }

//#if !SyncfusionFramework4_0
//        /// <summary>
//        /// Initializes a new <see cref="GridIntEditStyleInfoStore"/> from a serialization stream.
//        /// </summary>
//        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
//        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
//        [System.Security.SecurityCritical()]
//        protected GridIntEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
            StyleInfoStore target = new GridIntegerEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}