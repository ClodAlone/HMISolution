// <copyright file="ChartPropertyMetadata.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Represents ChartPropertyMetadataOptions
    /// </summary>
    /// <exclude/>
    public enum ChartPropertyMetadataOptions
    {
        /// <summary>
        /// No options are specified.
        /// </summary>
        None,

        /// <summary>
        /// The update is affected by value changes to this dependency property. 
        /// </summary>
        AffectsUpdate,

        /// <summary>
        /// The redraw is affected by value changes to this dependency property. 
        /// </summary>
        AffectsRedraw
    }

    /// <summary>
    /// Represents ChartPropertyMetadata
    /// </summary>
    /// <exclude/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPropertyMetadata : PropertyMetadata
    {
        #region Members
        /// <summary>
        /// Initializes m_options
        /// </summary>
        private ChartPropertyMetadataOptions m_options = ChartPropertyMetadataOptions.None;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public ChartPropertyMetadataOptions Options
        {
            get { return m_options; }
            set { m_options = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        public ChartPropertyMetadata()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public ChartPropertyMetadata(object defaultValue)
            : base(defaultValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="propertyChangedCallback">The property changed callback.</param>
        public ChartPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : base(propertyChangedCallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="propertyChangedCallback">The property changed callback.</param>
        public ChartPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value of the dependency property, usually provided as a value of some specific type.</param>
        /// <param name="propertyChangedCallback">Reference to a handler implementation that is to be called by the property system whenever the effective value of the property changes.</param>
        /// <param name="coerceValueCallback">Reference to a handler implementation that is to be called whenever the property system calls <see cref="M:System.Windows.DependencyObject.CoerceValue(System.Windows.DependencyProperty)"></see> against this property.</param>
        /// <exception cref="T:System.ArgumentException">defaultValue cannot be set to the value <see cref="F:System.Windows.DependencyProperty.UnsetValue"></see>; see Remarks.</exception>
        public ChartPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="options">The options.</param>
        public ChartPropertyMetadata(object defaultValue, ChartPropertyMetadataOptions options)
            : base(defaultValue)
        {
            m_options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="propertyChangedCallback">The property changed callback.</param>
        /// <param name="options">The options.</param>
        public ChartPropertyMetadata(PropertyChangedCallback propertyChangedCallback, ChartPropertyMetadataOptions options)
            : base(propertyChangedCallback)
        {
            m_options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="propertyChangedCallback">The property changed callback.</param>
        /// <param name="options">The options.</param>
        public ChartPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, ChartPropertyMetadataOptions options)
            : base(defaultValue, propertyChangedCallback)
        {
            m_options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="propertyChangedCallback">The property changed callback.</param>
        /// <param name="coerceValueCallback">The coerce value callback.</param>
        /// <param name="options">The options.</param>
        public ChartPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback, ChartPropertyMetadataOptions options)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            m_options = options;
        }
        #endregion
    }
}
