// <copyright file="DocumentContainer_VistaFlipCase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Events
        /// <summary>
        /// Occurs when [opacity factor of vista flip changed].
        /// </summary>
        public event PropertyChangedCallback OpacityFactorOfVistaFlipChanged;
        
        /// <summary>
        /// Occurs when [first flip item opacity changed].
        /// </summary>
        public event PropertyChangedCallback FirstFlipItemOpacityChanged;
        
        /// <summary>
        /// Event that is raised when VistaFlipItemsHeightFactor property is changed.
        /// </summary>
        public event PropertyChangedCallback VistaFlipItemsHeightFactorChanged;
        
        /// <summary>
        /// Event that is raised when VistaFlipItemsWidthFactor property is changed.
        /// </summary>
        public event PropertyChangedCallback VistaFlipItemsWidthFactorChanged;
        
        /// <summary>
        /// Event that is raised when FactoryOfViewVistaFlip property is changed.
        /// </summary>
        public event PropertyChangedCallback FactoryOfViewVistaFlipChanged;
        
        /// <summary>
        /// Event that is raised when VistaFlipAnimationDuration property is changed.
        /// </summary>
        public event PropertyChangedCallback VistaFlipAnimationDurationChanged;
        
        /// <summary>
        /// Event that is raised when KeepLimitedVistaItemsStack property is changed.
        /// </summary>
        public event PropertyChangedCallback KeepLimitedVistaItemsStackChanged;
        #endregion

        #region Preoprties
        /// <summary>
        /// Gets or sets the opacity factor of vista flip.
        /// </summary>
        /// <value>The opacity factor of vista flip.</value>
        public double OpacityFactorOfVistaFlip
        {
            get
            {
                return (double)GetValue(OpacityFactorOfVistaFlipProperty);
            }

            set
            {
                SetValue(OpacityFactorOfVistaFlipProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the first flip item opacity.
        /// </summary>
        /// <value>The first flip item opacity.</value>
        public double FirstFlipItemOpacity
        {
            get
            {
                return (double)GetValue(FirstFlipItemOpacityProperty);
            }

            set
            {
                SetValue(FirstFlipItemOpacityProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the FactoryOfViewVistaFlip dependency property.
        /// </summary>
        public double FactoryOfViewVistaFlip
        {
            get
            {
                return (double)GetValue(FactoryOfViewVistaFlipProperty);
            }

            set
            {
                SetValue(FactoryOfViewVistaFlipProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the VistaFlipAnimationDuration dependency property.
        /// </summary>
        public Duration VistaFlipAnimationDuration
        {
            get
            {
                return (Duration)GetValue(VistaFlipAnimationDurationProperty);
            }

            set
            {
                SetValue(VistaFlipAnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [keep limited vista items stack].
        /// </summary>
        /// <value>
        /// <c>true</c> if [keep limited vista items stack]; otherwise, <c>false</c>.
        /// </value>
        public bool KeepLimitedVistaItemsStack
        {
            get
            {
                return (bool)GetValue(KeepLimitedVistaItemsStackProperty);
            }

            set
            {
                SetValue(KeepLimitedVistaItemsStackProperty, value);
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Raises the <see cref="E:OpacityFactorOfVistaFlipChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnOpacityFactorOfVistaFlipChanged(DependencyPropertyChangedEventArgs args)
        {
            if (null != OpacityFactorOfVistaFlipChanged)
            {
                OpacityFactorOfVistaFlipChanged(this, args);
            }
        }
        
        /// <summary>
        /// Raises the <see cref="E:FirstFlipItemOpacityChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFirstFlipItemOpacityChanged(DependencyPropertyChangedEventArgs args)
        {
            if (null != FirstFlipItemOpacityChanged)
            {
                FirstFlipItemOpacityChanged(this, args);
            }
        }
        
        /// <summary>
        /// Raises VistaFlipItemsHeightFactorChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnVistaFlipItemsHeightFactorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != VistaFlipItemsHeightFactorChanged)
            {
                VistaFlipItemsHeightFactorChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises VistaFlipItemsWidthFactorChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnVistaFlipItemsWidthFactorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != VistaFlipItemsWidthFactorChanged)
            {
                VistaFlipItemsWidthFactorChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises FactoryOfViewVistaFlipChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFactoryOfViewVistaFlipChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != FactoryOfViewVistaFlipChanged)
            {
                FactoryOfViewVistaFlipChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises VistaFlipAnimationDurationChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnVistaFlipAnimationDurationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != VistaFlipAnimationDurationChanged)
            {
                VistaFlipAnimationDurationChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises KeepLimitedVistaItemsStackChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnKeepLimitedVistaItemsStackChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != KeepLimitedVistaItemsStackChanged)
            {
                KeepLimitedVistaItemsStackChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [opacity factor of vista flip changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOpacityFactorOfVistaFlipChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DocumentContainer owner = (DocumentContainer)d;
            owner.OnOpacityFactorOfVistaFlipChanged(args);
        }
        
        /// <summary>
        /// Called when [validate opacity factor].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns> bool value </returns>
        private static bool OnValidateOpacityFactor(object value)
        {
            return (double)value > 0;
        }
        
        /// <summary>
        /// Called when [first flip item opacity changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFirstFlipItemOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DocumentContainer owner = (DocumentContainer)d;
            owner.OnFirstFlipItemOpacityChanged(args);
        }
        
        /// <summary>
        /// Calls OnVistaFlipItemsHeightFactorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVistaFlipItemsHeightFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnVistaFlipItemsHeightFactorChanged(e);
        }
        
        /// <summary>
        /// Calls OnVistaFlipItemsWidthFactorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVistaFlipItemsWidthFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnVistaFlipItemsWidthFactorChanged(e);
        }
        
        /// <summary>
        /// Calls OnFactoryOfViewVistaFlipChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFactoryOfViewVistaFlipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnFactoryOfViewVistaFlipChanged(e);
        }
        
        /// <summary>
        /// Called when [factory of view vista flip validate value callback].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns> bool value type </returns>
        private static bool OnFactoryOfViewVistaFlipValidateValueCallback(object value)
        {
            double factory = (double)value;
            return 0.25 <= factory && 0.95 >= factory;
        }
        
        /// <summary>
        /// Calls OnVistaFlipAnimationDurationChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnVistaFlipAnimationDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnVistaFlipAnimationDurationChanged(e);
        }
        
        /// <summary>
        /// Calls OnKeepLimitedVistaItemsStackChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKeepLimitedVistaItemsStackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnKeepLimitedVistaItemsStackChanged(e);
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Presents factor of opacity for VistaFlip items.
        /// </summary>
        public static readonly DependencyProperty OpacityFactorOfVistaFlipProperty = DependencyProperty.Register("OpacityFactorOfVistaFlip", typeof(double), typeof(DocumentContainer), new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnOpacityFactorOfVistaFlipChanged)), new ValidateValueCallback(OnValidateOpacityFactor));
        
        /// <summary>
        /// Presents opacity of first flip item in VistaFlip.
        /// </summary>
        public static readonly DependencyProperty FirstFlipItemOpacityProperty = DependencyProperty.Register("FirstFlipItemOpacity", typeof(double), typeof(DocumentContainer), new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnFirstFlipItemOpacityChanged)));
        
        /// <summary>
        /// Presents factory of view in Vista Flip.
        /// </summary>
        public static readonly DependencyProperty FactoryOfViewVistaFlipProperty = DependencyProperty.Register("FactoryOfViewVistaFlip", typeof(double), typeof(DocumentContainer), new FrameworkPropertyMetadata(0.75, new PropertyChangedCallback(OnFactoryOfViewVistaFlipChanged)), new ValidateValueCallback(OnFactoryOfViewVistaFlipValidateValueCallback));
        
        /// <summary>
        /// Presents VistaFlip animation duration.
        /// </summary>
        public static readonly DependencyProperty VistaFlipAnimationDurationProperty = DependencyProperty.Register("VistaFlipAnimationDuration", typeof(Duration), typeof(DocumentContainer), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromSeconds(0.3)), new PropertyChangedCallback(OnVistaFlipAnimationDurationChanged)));
        
        /// <summary>
        /// Presents property that indicates whether needs to show all Vista Flip items.
        /// </summary>
        public static readonly DependencyProperty KeepLimitedVistaItemsStackProperty = DependencyProperty.Register("KeepLimitedVistaItemsStack", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnKeepLimitedVistaItemsStackChanged)));
        #endregion

        #region IVistaFlipOwner Members
        /// <summary>
        /// Gets a value indicating whether this instance is items in full screen.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is items in full screen; otherwise, <c>false</c>.
        /// </value>
        public bool IsItemsInFullScreen
        {
            get
            {
                return DocumentContainerMode.TDI == Mode || IsInMDIMaximizedState;
            }
        }
        #endregion
    }
}