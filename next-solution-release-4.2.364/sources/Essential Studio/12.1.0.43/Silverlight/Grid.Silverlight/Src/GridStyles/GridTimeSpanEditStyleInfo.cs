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
    using Syncfusion.Windows.Tools.Controls;

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for TimeSpanEdit control properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridTimeSpanEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridTimeSpanEditStyleInfo defaultTimeSpanEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridTimeSpanEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridTimeSpanEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridTimeSpanEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridTimeSpanEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridTimeSpanEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridTimeSpanEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridTimeSpanEditStyleInfoStore"/> that holds data for this <see cref="GridTimeSpanEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridTimeSpanEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridTimeSpanEditStyleInfo(StyleInfoSubObjectIdentity identity, GridTimeSpanEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridTimeSpanEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridTimeSpanEditStyleInfo()
            : base(new GridTimeSpanEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridTimeSpanEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridTimeSpanEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultTimeSpanEditInfo == null)
                {
                    defaultTimeSpanEditInfo = new GridTimeSpanEditStyleInfo();
                    defaultTimeSpanEditInfo.AllowNull = true;
                    defaultTimeSpanEditInfo.Format = "d.h:m:s";
                    defaultTimeSpanEditInfo.IncrementOnScrolling  = true;
                    defaultTimeSpanEditInfo.MaxValue = TimeSpan.MaxValue;
                    defaultTimeSpanEditInfo.MinValue = TimeSpan.MinValue;
                    defaultTimeSpanEditInfo.NullString  = String.Empty;
                    defaultTimeSpanEditInfo.ShowArrowButtons  = true;                    
                }

                return defaultTimeSpanEditInfo;
            }
        }

        public bool AllowNull
        {
            get
            {
                return (bool)this.GetValue(GridTimeSpanEditStyleInfoStore.AllowNullProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.AllowNullProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.AllowNull"/>.
        /// </summary>
        public void ResetAllowNull()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.AllowNullProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.AllowNull"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeAllowNull()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.AllowNullProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.AllowNull"/> is initialized for current object.
        /// </summary>
        public bool HasAllowNull
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.AllowNullProperty);
            }
        }

        /// <summary>
        ///  Gets or sets the Format for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.Empty.
        /// </value>
        /// <seealso cref="string"/>
        public string Format
        {
            get
            {
                return (string)this.GetValue(GridTimeSpanEditStyleInfoStore.FormatProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.FormatProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.Format"/>.
        /// </summary>
        public void ResetFormat()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.FormatProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.Format"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeFormat()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.FormatProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.Format"/> is initialized for current object.
        /// </summary>
        public bool HasFormat
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.FormatProperty);
            }
        }

        /// <summary>
        ///  Gets or sets the IncrementOnScrolling for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IncrementOnScrolling
        {
            get
            {
                return (bool)this.GetValue(GridTimeSpanEditStyleInfoStore.IncrementOnScrollingProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.IncrementOnScrollingProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.IncrementOnScrolling"/>.
        /// </summary>
        public void ResetIncrementOnScrolling()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.IncrementOnScrollingProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.IncrementOnScrolling"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeIncrementOnScrolling()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.IncrementOnScrollingProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.IncrementOnScrolling"/> is initialized for current object.
        /// </summary>
        public bool HasIncrementOnScrolling
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.IncrementOnScrollingProperty);
            }
        }

        /// <summary>
        ///  Gets or sets the Format for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.Empty.
        /// </value>
        /// <seealso cref="string"/>
        public string NullString
        {
            get
            {
                return (string)this.GetValue(GridTimeSpanEditStyleInfoStore.NullStringProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.NullStringProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.NullString"/>.
        /// </summary>
        public void ResetNullString()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.NullStringProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.NullString"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeNullString()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.NullStringProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.NullString"/> is initialized for current object.
        /// </summary>
        public bool HasNullString
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.NullStringProperty);
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
        public TimeSpan MinValue
        {
            get
            {
                return (TimeSpan)this.GetValue(GridTimeSpanEditStyleInfoStore.MinValueProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.MinValueProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.MinValue"/>.
        /// </summary>
        public void ResetMinValue()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.MinValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.MinValue"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeMinValue()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.MinValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.MinValue"/> is initialized for current object.
        /// </summary>
        public bool HasMinValue
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.MinValueProperty);
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
        public TimeSpan MaxValue
        {
            get
            {
                return (TimeSpan)this.GetValue(GridTimeSpanEditStyleInfoStore.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.MaxValueProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.MaxValue"/>.
        /// </summary>
        public void ResetMaxValue()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.MaxValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.MaxValue"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldSerializeMaxValue()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.MaxValueProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.MaxValue"/> is initialized for current object.
        /// </summary>
        public bool HasMaxValue
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.MaxValueProperty);
            }
        }

        /// <summary>
        ///  Gets or sets the IncrementOnScrolling for the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ShowArrowButtons
        {
            get
            {
                return (bool)this.GetValue(GridTimeSpanEditStyleInfoStore.ShowArrowButtonsProperty);
            }

            set
            {
                this.SetValue(GridTimeSpanEditStyleInfoStore.ShowArrowButtonsProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridTimeSpanEditStyleInfo.ShowArrowButtons"/>.
        /// </summary>
        public void ResetShowArrowButtons()
        {
            this.ResetValue(GridTimeSpanEditStyleInfoStore.ShowArrowButtonsProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.ShowArrowButtons"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable.</returns>
        public bool ShouldShowArrowButtons()
        {
            return this.HasValue(GridTimeSpanEditStyleInfoStore.ShowArrowButtonsProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridTimeSpanEditStyleInfo.ShowArrowButtons"/> is initialized for current object.
        /// </summary>
        public bool HasShowArrowButtons
        {
            get
            {
                return this.HasValue(GridTimeSpanEditStyleInfoStore.ShowArrowButtonsProperty);
            }
        }
       

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridTimeSpanEditStyleInfo(identity, store as GridTimeSpanEditStyleInfoStore);
            }

            return new GridTimeSpanEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridTimeSpanEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridTimeSpanEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridTimeSpanEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    public class GridTimeSpanEditStyleInfoStore : StyleInfoStore
    {
        static GridTimeSpanEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridTimeSpanEditStyleInfoStore), typeof(GridTimeSpanEditStyleInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.AllowNull"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty AllowNullProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowNull");

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.Format"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty FormatProperty = sd.CreateStyleInfoProperty(typeof(string), "Format");

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.IncrementOnScrolling"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IncrementOnScrollingProperty = sd.CreateStyleInfoProperty(typeof(bool), "IncrementOnScrolling");

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.MaxValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxValueProperty = sd.CreateStyleInfoProperty(typeof(TimeSpan), "MaxValue");

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.MinValue"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinValueProperty = sd.CreateStyleInfoProperty(typeof(TimeSpan), "MinValue");

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.NullString"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty NullStringProperty = sd.CreateStyleInfoProperty(typeof(string), "NullString");

        /// <summary>
        /// Provides information about the <see cref="GridTimeSpanEditStyleInfo.ShowArrowButtons"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ShowArrowButtonsProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowArrowButtons");


        /// <summary>
        /// Initializes a new instance of the <see cref="GridTimeSpanEditStyleInfoStore"/> class.
        /// </summary>
        public GridTimeSpanEditStyleInfoStore()
        {
        }

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
            StyleInfoStore target = new GridTimeSpanEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}