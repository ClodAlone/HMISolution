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
    using System.Windows;


    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for currency textbox properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridCurrencyEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridCurrencyEditStyleInfo defaultCurrencyEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridCurrencyEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridCurrencyEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridCurrencyEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCurrencyEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridCurrencyEditStyleInfoStore"/> that holds data for this <see cref="GridCurrencyEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridCurrencyEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridCurrencyEditStyleInfo(StyleInfoSubObjectIdentity identity, GridCurrencyEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridCurrencyEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridCurrencyEditStyleInfo()
            : base(new GridCurrencyEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridCurrencyEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridCurrencyEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultCurrencyEditInfo == null)
                {
                    defaultCurrencyEditInfo = new GridCurrencyEditStyleInfo();
                    defaultCurrencyEditInfo.MaxValue = decimal.MaxValue;
                    defaultCurrencyEditInfo.MinValue = decimal.MinValue;
                    defaultCurrencyEditInfo.IsScrollingOnCircle = true;
                    defaultCurrencyEditInfo.UseNullOption = false;
                    defaultCurrencyEditInfo.NullValue = null;
                }

                return defaultCurrencyEditInfo;
            }
        }

        #region IsScrollingOnCircle
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scrolling on circle.
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get
            {
                return (bool)this.GetValue(GridCurrencyEditStyleInfoStore.IsScrollingOnCircleProperty);
            }

            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.IsScrollingOnCircleProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsScrollingOnCircle property.
        /// </summary>
        public void ResetIsScrollingOnCircle()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsScrollingOnCircle()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is initialized.
        /// </summary>
        public bool HasIsScrollingOnCircle
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.IsScrollingOnCircleProperty);
            }
        }

        #endregion

        public bool UseNullOption
        {
            get
            {
                return (bool)this.GetValue(GridCurrencyEditStyleInfoStore.UseNullOptionProperty);
            }
            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.UseNullOptionProperty, value);
            }
        }

        public decimal? NullValue
        {
            get
            {
                return (decimal?)this.GetValue(GridCurrencyEditStyleInfoStore.NullValueProperty);
            }
            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.NullValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for currency cell.
        /// </summary>
        public decimal MaxValue
        {
            get
            {
                return (decimal)this.GetValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MaxValue"/> property.
        /// </summary>
        public void ResetMaxValue()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MaxValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MaxValue"/> property is initialized.
        /// </summary>
        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for currency cell.
        /// </summary>
        public decimal MinValue
        {
            get
            {
                return (decimal)this.GetValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.MinValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MinValue"/> property.
        /// </summary>
        public void ResetMinValue()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MinValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MinValue"/> property is initialized.
        /// </summary>
        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
            }
        }

        #region MinValidation

        /// <summary>
        /// Gets or sets the min validation.
        /// </summary>
        /// <value>The min validation.</value>
        public MinValidation MinValidation
        {
            get
            {
                return (MinValidation)GetValue(GridCurrencyEditStyleInfoStore.MinValidationProperty);
            }
            set
            {
                SetValue(GridCurrencyEditStyleInfoStore.MinValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has min validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has min validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinValidation
        {
            get
            {
                return HasValue(GridCurrencyEditStyleInfoStore.MinValidationProperty);
            }
        }

        /// <summary>
        /// Resets the Min validation.
        /// </summary>
        public void ResetMinValidation()
        {
            ResetValue(GridCurrencyEditStyleInfoStore.MinValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize Min validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMinValidation()
        {
            return HasValue(GridCurrencyEditStyleInfoStore.MinValidationProperty);
        }

        #endregion

        #region MaxValidation

        /// <summary>
        /// Gets or sets the max validation.
        /// </summary>
        /// <value>The max validation.</value>
        public MaxValidation MaxValidation
        {
            get
            {
                return (MaxValidation)GetValue(GridCurrencyEditStyleInfoStore.MaxValidationProperty);
            }
            set
            {
                SetValue(GridCurrencyEditStyleInfoStore.MaxValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has max validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has max validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaxValidation
        {
            get
            {
                return HasValue(GridCurrencyEditStyleInfoStore.MaxValidationProperty);
            }
        }

        /// <summary>
        /// Resets the max validation.
        /// </summary>
        public void ResetMaxValidation()
        {
            ResetValue(GridCurrencyEditStyleInfoStore.MaxValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize max validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMaxValidation()
        {
            return HasValue(GridCurrencyEditStyleInfoStore.MaxValidationProperty);
        }
        #endregion

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridCurrencyEditStyleInfo(identity, store as GridCurrencyEditStyleInfoStore);
            }

            return new GridCurrencyEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridCurrencyEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridCurrencyEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridCurrencyEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable]
    [StaticDataField("sd")]
    public class GridCurrencyEditStyleInfoStore : StyleInfoStore
    {
        static GridCurrencyEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridCurrencyEditStyleInfoStore), typeof(GridCurrencyEditStyleInfo), true);
        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(decimal), "MaxValue");

        /// <summary>
        /// 
        /// </summary>
        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <summary>
        /// 
        /// </summary>
        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");

        /// <summary>
        /// 
        /// </summary>
        public static readonly StyleInfoProperty NullValueProperty = sd.CreateStyleInfoProperty(typeof(decimal?), "NullValue");
        
        /// <summary>
        /// Provides information about the <see cref="GridCurrencyEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(decimal), "MinValue");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MinValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValidationProperty = sd.CreateStyleInfoProperty(typeof(MinValidation), "MinValidation");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MaxValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValidationProperty = sd.CreateStyleInfoProperty(typeof(MaxValidation), "MaxValidation");

        /// <overload>
        /// Initializes a new <see cref="GridCurrencyEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridCurrencyEditStyleInfoStore"/>.
        /// </summary>
        public GridCurrencyEditStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridCurrencyEditStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridCurrencyEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a duplicate of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridCurrencyEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    public class GridDoubleEditStyleInfoStore : StyleInfoStore
    {
        static GridDoubleEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridDoubleEditStyleInfoStore), typeof(GridDoubleEditStyleInfo), true);
        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.NullValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty NullValueProperty = sd.CreateStyleInfoProperty(typeof(double?), "NullValue");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.UseNullOption"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(double), "MaxValue");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(double), "MinValue");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.IsScrollingOnCircle"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MinValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValidationProperty = sd.CreateStyleInfoProperty(typeof(MinValidation), "MinValidation");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MaxValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValidationProperty = sd.CreateStyleInfoProperty(typeof(MaxValidation), "MaxValidation");


        /// <overload>
        /// Initializes a new <see cref="GridDoubleEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridDoubleEditStyleInfoStore"/>.
        /// </summary>
        public GridDoubleEditStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridDoubleEditStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridDoubleEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a duplicate of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridDoubleEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

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
                    defaultDoubleEditInfo.MaxValue = double.MaxValue;
                    defaultDoubleEditInfo.MinValue = double.MinValue;
                    defaultDoubleEditInfo.UseNullOption = false;
                }

                return defaultDoubleEditInfo;
            }
        }

        #region NullValue

        public double? NullValue
        {
            get { return (double?)this.GetValue(GridDoubleEditStyleInfoStore.NullValueProperty); }
            set { this.SetValue(GridDoubleEditStyleInfoStore.NullValueProperty, value); }
        }

        #endregion

        #region UseNullOption

        /// <summary>
        /// Specifies whether the UseNullOption property is initialized.
        /// </summary>
        public bool HasUseNullOption
        {
            get
            {
                return this.HasValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
            }
        }

        /// <summary>
        /// Resets the value of UseNullOption property.
        /// </summary>
        public void ResetUseNullOption()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
        }

        /// <summary>
        /// Specifies whether the UseNullOption property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeUseNullOption()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty);
        }
        /// <summary>
        /// Gets or sets a value indicating whether [use null option].
        /// </summary>
        /// <value><c>true</c> if [use null option]; otherwise, <c>false</c>.</value>
        public bool UseNullOption
        {
            get { return (bool)this.GetValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty); }
            set { this.SetValue(GridDoubleEditStyleInfoStore.UseNullOptionProperty, value); }
        }

        #endregion

        #region IsScrollingOnCircle
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scrolling on circle.
        /// </summary>
        public bool IsScrollingOnCircle
        {
            [DebuggerStepThrough()]
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
        /// Gets or sets the maximum value for currency cell.
        /// </summary>
        public double MaxValue
        {
            get
            {
                return (double)this.GetValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridDoubleEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MaxValue"/> property.
        /// </summary>
        public void ResetMaxValue()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MaxValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MaxValue"/> property is initialized.
        /// </summary>
        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridDoubleEditStyleInfoStore.MaxValueProperty);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for currency cell.
        /// </summary>
        public double MinValue
        {
            get
            {
                return (double)this.GetValue(GridDoubleEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridDoubleEditStyleInfoStore.MinValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MinValue"/> property.
        /// </summary>
        public void ResetMinValue()
        {
            this.ResetValue(GridDoubleEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MinValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridDoubleEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MinValue"/> property is initialized.
        /// </summary>
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

        #region MinValidation

        /// <summary>
        /// Gets or sets the min validation.
        /// </summary>
        /// <value>The min validation.</value>
        public MinValidation MinValidation
        {
            [DebuggerStepThrough()]
            get
            {
                return (MinValidation)GetValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
            }
            set
            {
                SetValue(GridDoubleEditStyleInfoStore.MinValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has min validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has min validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinValidation
        {
            get
            {
                return HasValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
            }
        }

        /// <summary>
        /// Resets the min validation.
        /// </summary>
        public void ResetMinValidation()
        {
            ResetValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize min validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMinValidation()
        {
            return HasValue(GridDoubleEditStyleInfoStore.MinValidationProperty);
        }

        #endregion

        #region MaxValidation

        /// <summary>
        /// Gets or sets the max validation.
        /// </summary>
        /// <value>The max validation.</value>
        public MaxValidation MaxValidation
        {
            [DebuggerStepThrough()]
            get
            {
                return (MaxValidation)GetValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
            }
            set
            {
                SetValue(GridDoubleEditStyleInfoStore.MaxValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has max validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has max validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaxValidation
        {
            get
            {
                return HasValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
            }
        }

        /// <summary>
        /// Resets the max validation.
        /// </summary>
        public void ResetMaxValidation()
        {
            ResetValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize max validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMaxValidation()
        {
            return HasValue(GridDoubleEditStyleInfoStore.MaxValidationProperty);
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

    public class GridIntegerEditStyleInfoStore : StyleInfoStore
    {
        static GridIntegerEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridIntegerEditStyleInfoStore), typeof(GridIntegerEditStyleInfo), true);
        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.NullValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty NullValueProperty = sd.CreateStyleInfoProperty(typeof(long?), "NullValue");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.UseNullOption"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");

        /// <summary>
        /// Provides information about the <see cref="GridIntegerEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(int), "MaxValue");

        /// <summary>
        ///  Provides information about the <see cref="GridIntegerEditStyleInfo.IsScrollingOnCircle"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <summary>
        ///  Provides information about the <see cref="GridIntegerEditStyleInfo.GroupSeperatorEnabled"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty GroupSeperatorEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "GroupSeperatorEnabled");

        /// <summary>
        /// Provides information about the <see cref="GridIntegerEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(int), "MinValue");

        /// <summary>
        ///  Provides information about the <see cref="GridIntegerEditStyleInfo.MinValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValidationProperty = sd.CreateStyleInfoProperty(typeof(MinValidation), "MinValidation");

        /// <summary>
        ///  Provides information about the <see cref="GridIntegerEditStyleInfo.MaxValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValidationProperty = sd.CreateStyleInfoProperty(typeof(MaxValidation), "MaxValidation");

        /// <overload>
        /// Initializes a new <see cref="GridIntegerEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridIntegerEditStyleInfoStore"/>.
        /// </summary>
        public GridIntegerEditStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridIntegerEditStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridIntegerEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a duplicate of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridIntegerEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    public class GridIntegerEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridIntegerEditStyleInfo defaultIntegerEditInfo;

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
                if (defaultIntegerEditInfo == null)
                {
                    defaultIntegerEditInfo = new GridIntegerEditStyleInfo();
                    defaultIntegerEditInfo.MaxValue = int.MaxValue;
                    defaultIntegerEditInfo.MinValue = int.MinValue;
                    defaultIntegerEditInfo.UseNullOption = false;
                    defaultIntegerEditInfo.NullValue = null;
                }

                return defaultIntegerEditInfo;
            }
        }

        #region IsScrollingOnCircle
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scrolling on circle.
        /// </summary>
        public bool IsScrollingOnCircle
        {
            [DebuggerStepThrough()]
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


        #region GroupSeperatorEnabled
        /// <summary>
        /// Gets or sets a value indicating whether Group Seperaator is Enabled.
        /// </summary>
        public bool GroupSeperatorEnabled
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)this.GetValue(GridIntegerEditStyleInfoStore.GroupSeperatorEnabledProperty);
            }

            set
            {
                this.SetValue(GridIntegerEditStyleInfoStore.GroupSeperatorEnabledProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of GroupSeperatorEnabled property.
        /// </summary>
        public void ResetGroupSeperatorEnabled()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.GroupSeperatorEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the GroupSeperatorEnabled property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeGroupSeperatorEnabled()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.GroupSeperatorEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the GroupSeperatorEnabled property is initialized.
        /// </summary>
        public bool HasGroupSeperatorEnabled
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.GroupSeperatorEnabledProperty);
            }
        }

        #endregion

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

        public long? NullValue
        {
            get
            {
                return (long?)this.GetValue(GridIntegerEditStyleInfoStore.NullValueProperty);
            }
            set
            {
                this.SetValue(GridIntegerEditStyleInfoStore.NullValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for currency cell.
        /// </summary>
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

        /// <summary>
        /// Resets the <see cref="MaxValue"/> property.
        /// </summary>
        public void ResetMaxValue()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MaxValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MaxValue"/> property is initialized.
        /// </summary>
        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.MaxValueProperty);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for currency cell.
        /// </summary>
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

        /// <summary>
        /// Resets the <see cref="MinValue"/> property.
        /// </summary>
        public void ResetMinValue()
        {
            this.ResetValue(GridIntegerEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MinValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridIntegerEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MinValue"/> property is initialized.
        /// </summary>
        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridIntegerEditStyleInfoStore.MinValueProperty);
            }
        }

        #region MinValidation

        /// <summary>
        /// Gets or sets the min validation.
        /// </summary>
        /// <value>The min validation.</value>
        public MinValidation MinValidation
        {
            [DebuggerStepThrough()]
            get
            {
                return (MinValidation)GetValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
            }
            set
            {
                SetValue(GridIntegerEditStyleInfoStore.MinValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has min validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has min validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinValidation
        {
            get
            {
                return HasValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
            }
        }

        /// <summary>
        /// Resets the Min validation.
        /// </summary>
        public void ResetMinValidation()
        {
            ResetValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize Min validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMinValidation()
        {
            return HasValue(GridIntegerEditStyleInfoStore.MinValidationProperty);
        }

        #endregion

        #region MaxValidation

        /// <summary>
        /// Gets or sets the max validation.
        /// </summary>
        /// <value>The max validation.</value>
        public MaxValidation MaxValidation
        {
            [DebuggerStepThrough()]
            get
            {
                return (MaxValidation)GetValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
            }
            set
            {
                SetValue(GridIntegerEditStyleInfoStore.MaxValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has max validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has max validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaxValidation
        {
            get
            {
                return HasValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
            }
        }

        /// <summary>
        /// Resets the max validation.
        /// </summary>
        public void ResetMaxValidation()
        {
            ResetValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize max validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMaxValidation()
        {
            return HasValue(GridIntegerEditStyleInfoStore.MaxValidationProperty);
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

    public class GridPercentEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridPercentEditStyleInfo defaultPercentEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridPercentEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridPercentEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridPercentEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridPercentEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridPercentEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridPercentEditStyleInfoStore"/> that holds data for this <see cref="GridPercentEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridCurrencyEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridPercentEditStyleInfo(StyleInfoSubObjectIdentity identity, GridPercentEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridPercentEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridPercentEditStyleInfo()
            : base(new GridPercentEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridPercentEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridPercentEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultPercentEditInfo == null)
                {
                    defaultPercentEditInfo = new GridPercentEditStyleInfo();
                    defaultPercentEditInfo.MaxValue = decimal.MaxValue;
                    defaultPercentEditInfo.MinValue = decimal.MinValue;
                    defaultPercentEditInfo.IsScrollingOnCircle = true;
                    defaultPercentEditInfo.UseNullOption = false;
                }

                return defaultPercentEditInfo;
            }
        }

        public bool UseNullOption
        {
            get
            {
                return (bool)this.GetValue(GridPercentEditStyleInfoStore.UseNullOptionProperty);
            }
            set
            {
                this.SetValue(GridPercentEditStyleInfoStore.UseNullOptionProperty, value);
            }
        }
        
        #region IsScrollingOnCircle
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scrolling on circle.
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get
            {
                return (bool)this.GetValue(GridPercentEditStyleInfoStore.IsScrollingOnCircleProperty);
            }

            set
            {
                this.SetValue(GridPercentEditStyleInfoStore.IsScrollingOnCircleProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsScrollingOnCircle property.
        /// </summary>
        public void ResetIsScrollingOnCircle()
        {
            this.ResetValue(GridPercentEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsScrollingOnCircle()
        {
            return this.HasValue(GridPercentEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is initialized.
        /// </summary>
        public bool HasIsScrollingOnCircle
        {
            get
            {
                return this.HasValue(GridPercentEditStyleInfoStore.IsScrollingOnCircleProperty);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the maximum value for currency cell.
        /// </summary>
        public decimal MaxValue
        {
            get
            {
                return (decimal)this.GetValue(GridPercentEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridPercentEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MaxValue"/> property.
        /// </summary>
        public void ResetMaxValue()
        {
            this.ResetValue(GridPercentEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MaxValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridPercentEditStyleInfoStore.MaxValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MaxValue"/> property is initialized.
        /// </summary>
        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridPercentEditStyleInfoStore.MaxValueProperty);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for currency cell.
        /// </summary>
        public decimal MinValue
        {
            get
            {
                return (decimal)this.GetValue(GridPercentEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridPercentEditStyleInfoStore.MinValueProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="MinValue"/> property.
        /// </summary>
        public void ResetMinValue()
        {
            this.ResetValue(GridPercentEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="MinValue"/> can be serialized.
        /// </summary>
        /// <returns>True if it can be serialized; False otherwise.</returns>
        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridPercentEditStyleInfoStore.MinValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MinValue"/> property is initialized.
        /// </summary>
        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridPercentEditStyleInfoStore.MinValueProperty);
            }
        }

        #region MinValidation

        /// <summary>
        /// Gets or sets the min validation.
        /// </summary>
        /// <value>The min validation.</value>
        public MinValidation MinValidation
        {
            get
            {
                return (MinValidation)GetValue(GridPercentEditStyleInfoStore.MinValidationProperty);
            }
            set
            {
                SetValue(GridPercentEditStyleInfoStore.MinValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has min validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has min validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinValidation
        {
            get
            {
                return HasValue(GridPercentEditStyleInfoStore.MinValidationProperty);
            }
        }

        /// <summary>
        /// Resets the Min validation.
        /// </summary>
        public void ResetMinValidation()
        {
            ResetValue(GridPercentEditStyleInfoStore.MinValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize Min validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMinValidation()
        {
            return HasValue(GridPercentEditStyleInfoStore.MinValidationProperty);
        }

        #endregion

        #region MaxValidation

        /// <summary>
        /// Gets or sets the max validation.
        /// </summary>
        /// <value>The max validation.</value>
        public MaxValidation MaxValidation
        {
            get
            {
                return (MaxValidation)GetValue(GridPercentEditStyleInfoStore.MaxValidationProperty);
            }
            set
            {
                SetValue(GridPercentEditStyleInfoStore.MaxValidationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has max validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has max validation; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaxValidation
        {
            get
            {
                return HasValue(GridPercentEditStyleInfoStore.MaxValidationProperty);
            }
        }

        /// <summary>
        /// Resets the max validation.
        /// </summary>
        public void ResetMaxValidation()
        {
            ResetValue(GridPercentEditStyleInfoStore.MaxValidationProperty);
        }

        /// <summary>
        /// Shoulds the serialize max validation.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMaxValidation()
        {
            return HasValue(GridPercentEditStyleInfoStore.MaxValidationProperty);
        }
        #endregion

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridPercentEditStyleInfo(identity, store as GridPercentEditStyleInfoStore);
            }

            return new GridPercentEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridCurrencyEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridCurrencyEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    [Serializable]
    [StaticDataField("sd")]
    public class GridPercentEditStyleInfoStore : StyleInfoStore
    {
        static GridPercentEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridPercentEditStyleInfoStore), typeof(GridPercentEditStyleInfo), true);
        /// <summary>
        /// Provides information about the <see cref="GridPercentEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(decimal), "MaxValue");

        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <summary>
        /// Provides information about the <see cref="GridPercentEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(decimal), "MinValue");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MinValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValidationProperty = sd.CreateStyleInfoProperty(typeof(MinValidation), "MinValidation");

        /// <summary>
        /// Provides information about the <see cref="GridDoubleEditStyleInfo.MaxValidation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValidationProperty = sd.CreateStyleInfoProperty(typeof(MaxValidation), "MaxValidation");


        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");

        /// <overload>
        /// Initializes a new <see cref="GridPercentEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridPercentEditStyleInfoStore"/>.
        /// </summary>
        public GridPercentEditStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        [System.Security.SecurityCritical()]
        protected GridPercentEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a duplicate of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridPercentEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    
    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for comment properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridCommentStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridCommentStyleInfo defaultCommentStyleInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridCommentStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCommentStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridCommentStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridCommentStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridCommentStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCommentStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridCommentStyleInfoStore"/> that holds data for this <see cref="GridCommentStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridCommentStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridCommentStyleInfo(StyleInfoSubObjectIdentity identity, GridCommentStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridCommentStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridCommentStyleInfo()
            : base(new GridCommentStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridCommentStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridCommentStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultCommentStyleInfo == null)
                {
                    defaultCommentStyleInfo = new GridCommentStyleInfo();
                    defaultCommentStyleInfo.BottomLeftCommentBrush = Brushes.Red;
                    defaultCommentStyleInfo.BottomRightCommentBrush = Brushes.Red;
                    defaultCommentStyleInfo.TopLeftCommentBrush = Brushes.Red;
                    defaultCommentStyleInfo.TopRightCommentBrush = Brushes.Red;
                }

                return defaultCommentStyleInfo;
            }
        }

        #region BottomLeftComment
        
        public string BottomLeftComment
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.BottomLeftCommentProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.BottomLeftCommentProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of BottomLeftComment property.
        /// </summary>
        public void ResetBottomLeftComment()
        {
            this.ResetValue(GridCommentStyleInfoStore.BottomLeftCommentProperty);
        }

        /// <summary>
        /// Specifies whether the BottomLeftComment property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeBottomLeftComment()
        {
            return this.HasValue(GridCommentStyleInfoStore.BottomLeftCommentProperty);
        }

        /// <summary>
        /// Specifies whether the BottomLeftComment property is initialized.
        /// </summary>
        public bool HasBottomLeftComment
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.BottomLeftCommentProperty);
            }
        }

        #endregion

        #region BottomRightComment
        public string BottomRightComment
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.BottomRightCommentProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.BottomRightCommentProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of BottomRightComment property.
        /// </summary>
        public void ResetBottomRightComment()
        {
            this.ResetValue(GridCommentStyleInfoStore.BottomRightCommentProperty);
        }

        /// <summary>
        /// Specifies whether the BottomRightComment property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeBottomRightComment()
        {
            return this.HasValue(GridCommentStyleInfoStore.BottomRightCommentProperty);
        }

        /// <summary>
        /// Specifies whether the BottomRightComment property is initialized.
        /// </summary>
        public bool HasBottomRightComment
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.BottomRightCommentProperty);
            }
        }
        #endregion

        #region TopLeftComment
        public string TopLeftComment
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.TopLeftCommentProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.TopLeftCommentProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of TopLeftComment property.
        /// </summary>
        public void ResetTopLeftComment()
        {
            this.ResetValue(GridCommentStyleInfoStore.TopLeftCommentProperty);
        }

        /// <summary>
        /// Specifies whether the TopLeftComment property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeTopLeftComment()
        {
            return this.HasValue(GridCommentStyleInfoStore.TopLeftCommentProperty);
        }

        /// <summary>
        /// Specifies whether the TopLeftComment property is initialized.
        /// </summary>
        public bool HasTopLeftComment
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.TopLeftCommentProperty);
            }
        }
        #endregion

        #region TopRightComment
        public string TopRightComment
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.TopRightCommentProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.TopRightCommentProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of TopRightComment property.
        /// </summary>
        public void ResetTopRightComment()
        {
            this.ResetValue(GridCommentStyleInfoStore.TopRightCommentProperty);
        }

        /// <summary>
        /// Specifies whether the TopRightComment property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeTopRightComment()
        {
            return this.HasValue(GridCommentStyleInfoStore.TopRightCommentProperty);
        }

        /// <summary>
        /// Specifies whether the TopRightComment property is initialized.
        /// </summary>
        public bool HasTopRightComment
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.TopRightCommentProperty);
            }
        }
        #endregion

        #region BottomLeftTemplateKey
        public string BottomLeftCommentTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.BottomLeftCommentTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.BottomLeftCommentTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of BottomLeftCommentTemplateKey property.
        /// </summary>
        public void ResetBottomLeftCommentTemplateKey()
        {
            this.ResetValue(GridCommentStyleInfoStore.BottomLeftCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the BottomLeftCommentTemplateKey property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeBottomLeftCommentTemplateKey()
        {
            return this.HasValue(GridCommentStyleInfoStore.BottomLeftCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the BottomLeftCommentTemplateKey property is initialized.
        /// </summary>
        public bool HasBottomLeftCommentTemplateKey
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.BottomLeftCommentTemplateKeyProperty);
            }
        }
        #endregion

        #region BottomRightTemplateKey
        public string BottomRightCommentTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.BottomRightCommentTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.BottomRightCommentTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of BottomRightCommentTemplateKey property.
        /// </summary>
        public void ResetBottomRightCommentTemplateKey()
        {
            this.ResetValue(GridCommentStyleInfoStore.BottomRightCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the BottomRightCommentTemplateKey property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeBottomRightCommentTemplateKey()
        {
            return this.HasValue(GridCommentStyleInfoStore.BottomRightCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the BottomRightCommentTemplateKey property is initialized.
        /// </summary>
        public bool HasBottomRightCommentTemplateKey
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.BottomRightCommentTemplateKeyProperty);
            }
        }
        #endregion

        #region TopLeftCommentTemplateKey
        public string TopLeftCommentTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.TopLeftCommentTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.TopLeftCommentTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of TopLeftCommentTemplateKey property.
        /// </summary>
        public void ResetTopLeftCommentTemplateKey()
        {
            this.ResetValue(GridCommentStyleInfoStore.TopLeftCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the TopLeftCommentTemplateKey property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeTopLeftCommentTemplateKey()
        {
            return this.HasValue(GridCommentStyleInfoStore.TopLeftCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the TopLeftCommentTemplateKey property is initialized.
        /// </summary>
        public bool HasTopLeftCommentTemplateKey
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.TopLeftCommentTemplateKeyProperty);
            }
        }
        #endregion

        #region TopRightCommentTemplateKey
        public string TopRightCommentTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridCommentStyleInfoStore.TopRightCommentTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.TopRightCommentTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of TopRightCommentTemplateKey property.
        /// </summary>
        public void ResetTopRightCommentTemplateKey()
        {
            this.ResetValue(GridCommentStyleInfoStore.TopRightCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the TopRightCommentTemplateKey property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeTopRightCommentTemplateKey()
        {
            return this.HasValue(GridCommentStyleInfoStore.TopRightCommentTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the TopRightCommentTemplateKey property is initialized.
        /// </summary>
        public bool HasTopRightCommentTemplateKey
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.TopRightCommentTemplateKeyProperty);
            }
        }
        #endregion

        #region TopLeftCommentBrush
        public Brush BottomLeftCommentBrush
        {
            get
            {
                return (Brush)this.GetValue(GridCommentStyleInfoStore.BottomLeftCommentBrushProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.BottomLeftCommentBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of BottomLeftCommentBrush property.
        /// </summary>
        public void ResetBottomLeftCommentBrush()
        {
            this.ResetValue(GridCommentStyleInfoStore.BottomLeftCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the BottomLeftCommentBrush property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeBottomLeftCommentBrush()
        {
            return this.HasValue(GridCommentStyleInfoStore.BottomLeftCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the BottomLeftCommentBrush property is initialized.
        /// </summary>
        public bool HasBottomLeftCommentBrush
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.BottomLeftCommentBrushProperty);
            }
        }
        #endregion

        #region BottomRightCommentBrush
        public Brush BottomRightCommentBrush
        {
            get
            {
                return (Brush)this.GetValue(GridCommentStyleInfoStore.BottomRightCommentBrushProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.BottomRightCommentBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of BottomRightCommentBrush property.
        /// </summary>
        public void ResetBottomRightCommentBrush()
        {
            this.ResetValue(GridCommentStyleInfoStore.BottomRightCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the BottomRightCommentBrush property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeBottomRightCommentBrush()
        {
            return this.HasValue(GridCommentStyleInfoStore.BottomRightCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the BottomRightCommentBrush property is initialized.
        /// </summary>
        public bool HasBottomRightCommentBrush
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.BottomRightCommentBrushProperty);
            }
        }
        #endregion

        #region TopLeftCommentBrush
        public Brush TopLeftCommentBrush
        {
            get
            {
                return (Brush)this.GetValue(GridCommentStyleInfoStore.TopLeftCommentBrushProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.TopLeftCommentBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of TopLeftCommentBrush property.
        /// </summary>
        public void ResetTopLeftCommentBrush()
        {
            this.ResetValue(GridCommentStyleInfoStore.TopLeftCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the TopLeftCommentBrush property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeTopLeftCommentBrush()
        {
            return this.HasValue(GridCommentStyleInfoStore.TopLeftCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the TopLeftCommentBrush property is initialized.
        /// </summary>
        public bool HasTopLeftCommentBrush
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.TopLeftCommentBrushProperty);
            }
        }
        #endregion

        #region TopRightCommentBrush
        public Brush TopRightCommentBrush
        {
            get
            {
                return (Brush)this.GetValue(GridCommentStyleInfoStore.TopRightCommentBrushProperty);
            }

            set
            {
                this.SetValue(GridCommentStyleInfoStore.TopRightCommentBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of TopRightCommentBrush property.
        /// </summary>
        public void ResetTopRightCommentBrush()
        {
            this.ResetValue(GridCommentStyleInfoStore.TopRightCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the TopRightCommentBrush property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeTopRightCommentBrush()
        {
            return this.HasValue(GridCommentStyleInfoStore.TopRightCommentBrushProperty);
        }

        /// <summary>
        /// Specifies whether the TopRightCommentBrush property is initialized.
        /// </summary>
        public bool HasTopRightCommentBrush
        {
            get
            {
                return this.HasValue(GridCommentStyleInfoStore.TopRightCommentBrushProperty);
            }
        }
        #endregion

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridCommentStyleInfo(identity, store as GridCommentStyleInfoStore);
            }

            return new GridCommentStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridCommentStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridCommentStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridCommentStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable]
    [StaticDataField("sd")]
    public class GridCommentStyleInfoStore : StyleInfoStore
    {
        static GridCommentStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridCommentStyleInfoStore), typeof(GridCommentStyleInfo), true);

        /// <summary>
        /// 
        /// </summary>
        public static readonly StyleInfoProperty BottomLeftCommentProperty = sd.CreateStyleInfoProperty(typeof(string), "BottomLeftComment");
        public static readonly StyleInfoProperty BottomRightCommentProperty = sd.CreateStyleInfoProperty(typeof(string), "BottomRightComment");
        public static readonly StyleInfoProperty TopLeftCommentProperty = sd.CreateStyleInfoProperty(typeof(string), "TopLeftComment");
        public static readonly StyleInfoProperty TopRightCommentProperty = sd.CreateStyleInfoProperty(typeof(string), "TopRightComment");

        public static readonly StyleInfoProperty BottomLeftCommentTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "BottomLeftCommentTemplateKey");
        public static readonly StyleInfoProperty BottomRightCommentTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "BottomRightCommentTemplateKey");
        public static readonly StyleInfoProperty TopLeftCommentTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "TopLeftCommentTemplateKey");
        public static readonly StyleInfoProperty TopRightCommentTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "TopRightCommentTemplateKey");

        public static readonly StyleInfoProperty BottomLeftCommentBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "BottomLeftCommentBrush");
        public static readonly StyleInfoProperty BottomRightCommentBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "BottomRightCommentBrush");
        public static readonly StyleInfoProperty TopLeftCommentBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "TopLeftCommentBrush");
        public static readonly StyleInfoProperty TopRightCommentBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "TopRightCommentBrush");

        /// <overload>
        /// Initializes a new <see cref="GridCommentStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridCommentStyleInfoStore"/>.
        /// </summary>
        public GridCommentStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridCommentStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridCommentStyleInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a duplicate of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridCommentStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    public class GridDropdownEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridDropdownEditStyleInfo defaultDropdownEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridDropdownEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridDropdownEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridDropdownEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridDropdownEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridDropdownEditStyleInfoStore"/> that holds data for this <see cref="GridDropdownEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridDropdownEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridDropdownEditStyleInfo(StyleInfoSubObjectIdentity identity, GridDropdownEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridDropdownEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridDropdownEditStyleInfo()
            : base(new GridDropdownEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridDropdownEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridDropdownEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultDropdownEditInfo == null)
                {
                    defaultDropdownEditInfo = new GridDropdownEditStyleInfo();
                    defaultDropdownEditInfo.ShowButton = true;
                }

                return defaultDropdownEditInfo;
            }
        }

        #region ShowButton
        /// <summary>
        /// Gets or sets a value indicating whether the Dropdown button should appear by default.
        /// </summary>
        public bool ShowButton
        {
            get
            {
                return (bool)this.GetValue(GridDropdownEditStyleInfoStore.ShowButtonProperty);
            }

            set
            {
                this.SetValue(GridDropdownEditStyleInfoStore.ShowButtonProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of ShowButton property.
        /// </summary>
        public void ResetShowButton()
        {
            this.ResetValue(GridDropdownEditStyleInfoStore.ShowButtonProperty);
        }

        /// <summary>
        /// Specifies whether the ShowButton property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeShowButton()
        {
            return this.HasValue(GridDropdownEditStyleInfoStore.ShowButtonProperty);
        }

        /// <summary>
        /// Specifies whether the ShowButton property is initialized.
        /// </summary>
        public bool HasShowButton
        {
            get
            {
                return this.HasValue(GridDropdownEditStyleInfoStore.ShowButtonProperty);
            }
        }

        #endregion

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridDropdownEditStyleInfo(identity, store as GridDropdownEditStyleInfoStore);
            }

            return new GridDropdownEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridDropdownEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridDropdownEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridDropdownEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable]
    [StaticDataField("sd")]
    public class GridDropdownEditStyleInfoStore : StyleInfoStore
    {
        static GridDropdownEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridDropdownEditStyleInfoStore), typeof(GridDropdownEditStyleInfo), true);

        /// <summary>
        /// 
        /// </summary>
        public static readonly StyleInfoProperty ShowButtonProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowButton");

        /// <overload>
        /// Initializes a new <see cref="GridDropdownEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridDropdownEditStyleInfoStore"/>.
        /// </summary>
        public GridDropdownEditStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridDropdownEditStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridDropdownEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
        /// <summary>Creates a duplicate of current object.</summary>
        /// <returns>A duplicate of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridDropdownEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}