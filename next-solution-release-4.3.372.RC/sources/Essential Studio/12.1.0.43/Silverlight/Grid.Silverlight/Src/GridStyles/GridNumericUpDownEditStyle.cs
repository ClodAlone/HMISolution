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
                }

                return defaultUpDownEditInfo;
            }
        }

        //public Brush FocusedBackground
        //{
        //    get
        //    {
        //        return (Brush)this.GetValue(GridNumericUpDownEditStyleInfoStore.FocusedBackgroundProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridNumericUpDownEditStyleInfoStore.FocusedBackgroundProperty, value);
        //    }
        //}

        //public void ResetFocusedBackground()
        //{
        //    this.ResetValue(GridNumericUpDownEditStyleInfoStore.FocusedBackgroundProperty);
        //}

        //public bool ShouldSerializeFocusedBackground()
        //{
        //    return this.HasValue(GridNumericUpDownEditStyleInfoStore.FocusedBackgroundProperty);
        //}

        //public bool HasFocusedBackground
        //{
        //    get
        //    {
        //        return this.HasValue(GridNumericUpDownEditStyleInfoStore.FocusedBackgroundProperty);
        //    }
        //}

        //public Brush FocusedForeground
        //{
        //    get
        //    {
        //        return (Brush)this.GetValue(GridNumericUpDownEditStyleInfoStore.FocusedForegroundProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridNumericUpDownEditStyleInfoStore.FocusedForegroundProperty, value);
        //    }
        //}

        //public void ResetFocusedForeground()
        //{
        //    this.ResetValue(GridNumericUpDownEditStyleInfoStore.FocusedForegroundProperty);
        //}

        //public bool ShouldSerializeFocusedForeground()
        //{
        //    return this.HasValue(GridNumericUpDownEditStyleInfoStore.FocusedForegroundProperty);
        //}

        //public bool HasFocusedForeground
        //{
        //    get
        //    {
        //        return this.HasValue(GridNumericUpDownEditStyleInfoStore.FocusedForegroundProperty);
        //    }
        //}

        //public Brush NegativeForeground
        //{
        //    get
        //    {
        //        return (Brush)this.GetValue(GridNumericUpDownEditStyleInfoStore.NegativeForegroundProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridNumericUpDownEditStyleInfoStore.NegativeForegroundProperty, value);
        //    }
        //}

        //public void ResetNegativeForeground()
        //{
        //    this.ResetValue(GridNumericUpDownEditStyleInfoStore.NegativeForegroundProperty);
        //}

        //public bool ShouldSerializeNegativeForeground()
        //{
        //    return this.HasValue(GridNumericUpDownEditStyleInfoStore.NegativeForegroundProperty);
        //}

        //public bool HasNegativeForeground
        //{
        //    get
        //    {
        //        return this.HasValue(GridNumericUpDownEditStyleInfoStore.NegativeForegroundProperty);
        //    }
        //}

        public double MinValue
        {
            get
            {
                if (this.HasMinValue)
                {
                    return (double)this.GetValue(GridUpDownEditStyleInfoStore.MinValueProperty);
                }
                return double.MinValue;
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.MinValueProperty, value);
            }
        }

        public void ResetMinValue()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.MinValueProperty);
        }

        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.MinValueProperty);
        }

        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.MinValueProperty);
            }
        }

        public double MaxValue
        {
            get
            {
                if (this.HasMaxValue)
                {
                    return (double)this.GetValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
                }

                return double.MaxValue;
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        public void ResetMaxValue()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
        }

        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
            }
        }

        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.MaxValueProperty);
        }

        public double Step
        {
            get
            {
                if (this.HasStep)
                {
                    return (double)this.GetValue(GridUpDownEditStyleInfoStore.StepProperty);
                }

                return double.MinValue;
            }

            set
            {
                this.SetValue(GridUpDownEditStyleInfoStore.StepProperty, value);
            }
        }

        public void ResetStep()
        {
            this.ResetValue(GridUpDownEditStyleInfoStore.StepProperty);
        }

        public bool ShouldSerializeStep()
        {
            return this.HasValue(GridUpDownEditStyleInfoStore.StepProperty);
        }

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
   // [Serializable]
    [StaticDataField("sd")]
    public class GridUpDownEditStyleInfoStore : StyleInfoStore
    {
        static GridUpDownEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridUpDownEditStyleInfoStore), typeof(GridUpDownEditStyleInfo), true);

        ///// <summary>
        ///// Provides information about the <see cref="GridUpDownEditStyleInfo.FocusedBackground"/> property. 
        ///// </summary>
        //public static readonly StyleInfoProperty FocusedBackgroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "FocusedBackground");

        ///// <summary>
        ///// Provides information about the <see cref="GridUpDownEditStyleInfo.FocusedForeground"/> property. 
        ///// </summary>
        //public static readonly StyleInfoProperty FocusedForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "FocusedForeground");

        ///// <summary>
        ///// Provides information about the <see cref="GridUpDownEditStyleInfo.NegativeForeground"/> property. 
        ///// </summary>
        //public static readonly StyleInfoProperty NegativeForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "NegativeForeground");

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

//#if !SyncfusionFramework4_0
//        /// <summary>
//        /// Initializes a new <see cref="GridUpDownEditStyleInfoStore"/> from a serialization stream.
//        /// </summary>
//        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
//        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
//       // [System.Security.SecurityCritical()]
//        protected GridNumericUpDownEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
            StyleInfoStore target = new GridUpDownEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}