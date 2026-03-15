#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

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

#if!WinRT
using System.Windows.Media;
using Syncfusion.Windows.Styles;
namespace Syncfusion.Windows.Controls.Grid
#else

using Syncfusion.WinRT.Styles;
using System;
using System.Diagnostics;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Grid

#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrencyEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridCurrencyEditStyleInfo defaultCurrencyEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridIntegerEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridCurrencyEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridCurrencyEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridIntegerEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridCurrencyEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridCurrencyEditStyleInfoStore"/> that holds data for this <see cref="GridIntegerEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridCurrencyEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridCurrencyEditStyleInfo(StyleInfoSubObjectIdentity identity, GridCurrencyEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridIntegerEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridCurrencyEditStyleInfo()
            : base(new GridCurrencyEditStyleInfoStore())
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
        public static GridCurrencyEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultCurrencyEditInfo == null)
                {
                    defaultCurrencyEditInfo = new GridCurrencyEditStyleInfo();
                    defaultCurrencyEditInfo.MaxValue = Decimal.MaxValue;
                    defaultCurrencyEditInfo.MinValue = Decimal.MinValue;
                    defaultCurrencyEditInfo.UseNullOption = false;
                }

                return defaultCurrencyEditInfo;
            }
        }

        #region CurrencySymbol

        public string CurrencySymbol
        {
            get
            {
                return (string)this.GetValue(GridCurrencyEditStyleInfoStore.CurrencySymbolProperty);
            }

            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.CurrencySymbolProperty, value);
            }
        }

        public void ResetCurrencySymbol()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.CurrencySymbolProperty);
        }

        private bool ShouldSerializeCurrencySymbol()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.CurrencySymbolProperty);
        }

        public bool HasCurrencySymbol
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.CurrencySymbolProperty);
            }
        }

        #endregion

        #region MaxValue

        public Decimal MaxValue
        {
            get
            {
                return (Decimal)this.GetValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.MaxValueProperty, value);
            }
        }

        public void ResetMaxValue()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
        }

        private bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
        }

        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.MaxValueProperty);
            }
        }

        #endregion

        #region MinValue

        public Decimal MinValue
        {
            get
            {
                return (Decimal)this.GetValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridCurrencyEditStyleInfoStore.MinValueProperty, value);
            }
        }

        public void ResetMinValue()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
        }

        private bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
        }

        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.MinValueProperty);
            }
        }

        #endregion


        #region UseNullOption

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

        public void ResetUseNullOption()
        {
            this.ResetValue(GridCurrencyEditStyleInfoStore.UseNullOptionProperty);
        }

        private bool ShouldSerializeUseNullOption()
        {
            return this.HasValue(GridCurrencyEditStyleInfoStore.UseNullOptionProperty);
        }

        public bool HasUseNullOption
        {
            get
            {
                return this.HasValue(GridCurrencyEditStyleInfoStore.UseNullOptionProperty);
            }
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCurrencyEditStyleInfoStore : StyleInfoStore
    {
        static GridCurrencyEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridCurrencyEditStyleInfoStore), typeof(GridCurrencyEditStyleInfo), true);
        /// <summary>
        /// Provides information about the <see cref="GridIntegerEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(Decimal), "MaxValue");

        /// <summary>
        /// Provides information about the <see cref="GridIntegerEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(Decimal), "MinValue");
        public static readonly StyleInfoProperty UseNullOptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "UseNullOption");

        public static readonly StyleInfoProperty CurrencySymbolProperty = sd.CreateStyleInfoProperty(typeof(string), "CurrencySymbol");

        /// <overload>
        /// Initializes a new <see cref="GridIntegerEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridIntegerEditStyleInfoStore"/>.
        /// </summary>
        public GridCurrencyEditStyleInfoStore()
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
            StyleInfoStore target = new GridCurrencyEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridImageCellStyleInfo : StyleInfoSubObjectBase
    {
        public static GridImageCellStyleInfo dafaultImageCellStyleInfo;
        [DebuggerStepThrough()]
        public GridImageCellStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridImageCellStyleInfoStore())
        {
        }

        [DebuggerStepThrough()]
        public GridImageCellStyleInfo(StyleInfoSubObjectIdentity identity, GridImageCellStyleInfoStore store)
            : base(identity, store)
        {
        }

        [DebuggerStepThrough()]
        public GridImageCellStyleInfo()
            : base(new GridImageCellStyleInfoStore())
        {
        }

        public static GridImageCellStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (dafaultImageCellStyleInfo == null)
                {
                    dafaultImageCellStyleInfo = new GridImageCellStyleInfo();
                    dafaultImageCellStyleInfo.Stretch = Stretch.Fill;
                }
                return dafaultImageCellStyleInfo;
            }
        }

        public Stretch Stretch
        {
            get { return (Stretch)this.GetValue(GridImageCellStyleInfoStore.StretchProperty); }
            set { this.SetValue(GridImageCellStyleInfoStore.StretchProperty, value); }
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridImageCellStyleInfo(identity, store as GridImageCellStyleInfoStore);
            }

            return new GridImageCellStyleInfo(identity);
        }

        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridImageCellStyleInfoStore : StyleInfoStore
    {
        static GridImageCellStyleInfoStore()
        {
        }

        public GridImageCellStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridImageCellStyleInfoStore), typeof(GridImageCellStyleInfo), true);

        public static readonly StyleInfoProperty StretchProperty = sd.CreateStyleInfoProperty(typeof(Stretch), "Stretch");

        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        public override object Clone()
        {
            StyleInfoStore target = new GridImageCellStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}