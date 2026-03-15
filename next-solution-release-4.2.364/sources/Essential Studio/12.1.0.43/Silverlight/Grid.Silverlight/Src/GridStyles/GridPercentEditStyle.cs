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
    // using Syncfusion.Windows.Shared;
    //  using Syncfusion.Windows.Styles;

    public class GridPercentEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields 
        private static GridPercentEditStyleInfo defaultPercentEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridPercentEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridPercentEditStyleInfo"/>.
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
        /// All changes in this style object will be saved in the <see cref="GridPercentEditStyleInfoStore"/> object.</param>
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
                    defaultPercentEditInfo.MaxValue = double.MaxValue;
                    defaultPercentEditInfo.MinValue = double.MinValue;
                    defaultPercentEditInfo.UseNullOption = false;
                }

                return defaultPercentEditInfo;
            }
        }

        #region UseNullOption
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

        public void ResetUseNullOption()
        {
            this.ResetValue(GridPercentEditStyleInfoStore.UseNullOptionProperty);
        }

        public bool ShouldSerializeUseNullOption()
        {
            return this.HasValue(GridPercentEditStyleInfoStore.UseNullOptionProperty);
        }

        public bool HasUseNullOption
        {
            get
            {
                return this.HasValue(GridPercentEditStyleInfoStore.UseNullOptionProperty);
            }
        }

        #endregion

        public Double MaxValue
        {
            get
            {
                return (Double)this.GetValue(GridPercentEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridPercentEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        public string PercentageSymbol
        {
            get
            {
                return (string)GetValue(GridPercentEditStyleInfoStore.PercentageSymbolProperty);
            }

            set
            {
                SetValue(GridPercentEditStyleInfoStore.PercentageSymbolProperty, value);
            }
        }

        public void ResetMaxValue()
        {
            this.ResetValue(GridPercentEditStyleInfoStore.MaxValueProperty);
        }

        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridPercentEditStyleInfoStore.MaxValueProperty);
        }

        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridPercentEditStyleInfoStore.MaxValueProperty);
            }
        }

        public Double MinValue
        {
            get
            {
                return (Double)this.GetValue(GridPercentEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridPercentEditStyleInfoStore.MinValueProperty, value);
            }
        }

        public void ResetMinValue()
        {
            this.ResetValue(GridPercentEditStyleInfoStore.MinValueProperty);
        }

        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridPercentEditStyleInfoStore.MinValueProperty);
        }

        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridPercentEditStyleInfoStore.MinValueProperty);
            }
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridPercentEditStyleInfo(identity, store as GridPercentEditStyleInfoStore);
            }

            return new GridPercentEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridPercentEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridPercentEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridPercentEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    //[Serializable]
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
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(Double), "MaxValue");
        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");
        /// <summary>
        /// Provides information about the <see cref="GridPercentEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(Double), "MinValue");
        public static readonly StyleInfoProperty PercentageSymbolProperty = sd.CreateStyleInfoProperty(typeof(string), "PercentageSymbol");

        /// <overload>
        /// Initializes a new <see cref="GridPercentEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridPercentEditStyleInfoStore"/>.
        /// </summary>
        public GridPercentEditStyleInfoStore()
        {
        }

        //#if !SyncfusionFramework4_0
        //        /// <summary>
        //        /// Initializes a new <see cref="GridPercentEditStyleInfoStore"/> from a serialization stream.
        //        /// </summary>
        //        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        //        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        //        [System.Security.SecurityCritical()]
        //        protected GridPercentEditStyleInfoStore(SerializationInfo info, StreamingContext context)
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
            StyleInfoStore target = new GridPercentEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}