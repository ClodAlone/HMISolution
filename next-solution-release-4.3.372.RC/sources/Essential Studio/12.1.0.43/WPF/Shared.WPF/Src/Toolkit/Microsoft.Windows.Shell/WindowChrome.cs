#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace Syncfusion.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Data;
    using Standard;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    public delegate void MarginChangedHandler(object o, MarginChangedEventArgs e);
    /// <summary>
    /// 
    /// </summary>
    public class MarginChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly Thickness Margin;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="margin"></param>
        public MarginChangedEventArgs(Thickness margin)
        {
            Margin = margin;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class WindowChrome : Freezable
    {
        /// <summary>
        /// 
        /// </summary>
        public event MarginChangedHandler MarginChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wChrome"></param>
        /// <param name="args"></param>
        public void OnMarginChanged(WindowChrome wChrome,MarginChangedEventArgs args)
        {
            if (MarginChanged != null)
            {
                MarginChanged(wChrome, args);
            }
        }

        private struct _SystemParameterBoundProperty
        {
            public string SystemParameterPropertyName { get; set; }
            public DependencyProperty DependencyProperty { get; set; }
        }

        // Named property available for fully extending the glass frame.
        /// <summary>
        /// 
        /// </summary>
        public static Thickness GlassFrameCompleteThickness { get { return new Thickness(-1); } }

        #region Attached Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty WindowChromeProperty = DependencyProperty.RegisterAttached(
            "WindowChrome",
            typeof(WindowChrome),
            typeof(WindowChrome),
            new PropertyMetadata(null, _OnChromeChanged));

        private static void _OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // The different design tools handle drawing outside their custom window objects differently.
            // Rather than try to support this concept in the design surface let the designer draw its own
            // chrome anyways.
            // There's certainly room for improvement here.
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            var window = (Window)d;
            var newChrome = (WindowChrome)e.NewValue;

            Assert.IsNotNull(window);

            // Update the ChromeWorker with this new object.

            // If there isn't currently a worker associated with the Window then assign a new one.
            // There can be a many:1 relationship of to Window to WindowChrome objects, but a 1:1 for a Window and a WindowChromeWorker.
            WindowChromeWorker chromeWorker = WindowChromeWorker.GetWindowChromeWorker(window);
            if (chromeWorker == null)
            {
                chromeWorker = new WindowChromeWorker();
                WindowChromeWorker.SetWindowChromeWorker(window, chromeWorker);
            }

            chromeWorker.SetWindowChrome(newChrome);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static WindowChrome GetWindowChrome(Window window)
        {
            Verify.IsNotNull(window, "window");
            return (WindowChrome)window.GetValue(WindowChromeProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="chrome"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetWindowChrome(Window window, WindowChrome chrome)
        {
            Verify.IsNotNull(window, "window");
            window.SetValue(WindowChromeProperty, chrome);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached(
            "IsHitTestVisibleInChrome",
            typeof(bool),
            typeof(WindowChrome),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputElement"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            return (bool)dobj.GetValue(IsHitTestVisibleInChromeProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputElement"></param>
        /// <param name="hitTestVisible"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            dobj.SetValue(IsHitTestVisibleInChromeProperty, hitTestVisible);
        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CaptionHeightProperty = DependencyProperty.Register(
            "CaptionHeight",
            typeof(double),
            typeof(WindowChrome),
            new PropertyMetadata(
                0d,
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()),
            value => (double)value >= 0d);

        /// <summary>The extent of the top of the window to treat as the caption.</summary>
        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty = DependencyProperty.Register(
            "ResizeBorderThickness",
            typeof(Thickness),
            typeof(WindowChrome),
            new PropertyMetadata(default(Thickness)),
            (value) => Utility.IsThicknessNonNegative((Thickness)value));
        /// <summary>
        /// 
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlassFrameThicknessProperty = DependencyProperty.Register(
            "GlassFrameThickness",
            typeof(Thickness),
            typeof(WindowChrome),
            new PropertyMetadata(
                default(Thickness),
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint(),
                (d, o) => _CoerceGlassFrameThickness((Thickness)o)));

        private static object _CoerceGlassFrameThickness(Thickness thickness)
        {
            // If it's explicitly set, but set to a thickness with at least one negative side then 
            // coerce the value to the stock GlassFrameCompleteThickness.
            if (!Utility.IsThicknessNonNegative(thickness))
            {
                return GlassFrameCompleteThickness;
            }

            return thickness;
        }
        /// <summary>
        /// 
        /// </summary>
        public Thickness GlassFrameThickness
        {
            get { return (Thickness)GetValue(GlassFrameThicknessProperty); }
            set { SetValue(GlassFrameThicknessProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(WindowChrome),
            new PropertyMetadata(
                default(CornerRadius),
                (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()),
            (value) => Utility.IsCornerRadiusValid((CornerRadius)value));
        /// <summary>
        /// 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public NonClientFrameEdges NonClientFrameEdges
        {
            get { return (NonClientFrameEdges)base.GetValue(NonClientFrameEdgesProperty); }
            set { SetValue(NonClientFrameEdgesProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NonClientFrameEdgesProperty = DependencyProperty.Register(
            "NonClientFrameEdges", 
            typeof(NonClientFrameEdges), 
            typeof(WindowChrome), 
            new PropertyMetadata(
                NonClientFrameEdges.None,
               (d,e)=>((WindowChrome) d)._OnPropertyChangedThatRequiresRepaint()),
              new ValidateValueCallback(WindowChrome._NonClientFrameEdgesAreValid));
        /// <summary>
        /// 
        /// </summary>
        public bool UseAeroCaptionButtons
        {
            get { return (bool)GetValue(UseAeroCaptionButtonsProperty); }
            set { SetValue(UseAeroCaptionButtonsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UseAeroCaptionButtonsProperty = DependencyProperty.Register(
            "UseAeroCaptionButtons", 
            typeof(bool), 
            typeof(WindowChrome), 
            new FrameworkPropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ResizeGripDirectionProperty = DependencyProperty.RegisterAttached(
            "ResizeGripDirection",
            typeof(ResizeGripDirection),
            typeof(WindowChrome), 
            new FrameworkPropertyMetadata(ResizeGripDirection.None, FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputElement"></param>
        /// <param name="direction"></param>
        public static void SetResizeGripDirection(IInputElement inputElement, ResizeGripDirection direction)
        {
            Verify.IsNotNull<IInputElement>(inputElement, "inputElement");
            DependencyObject dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            dobj.SetValue(ResizeGripDirectionProperty, direction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputElement"></param>
        /// <returns></returns>
        public static ResizeGripDirection GetResizeGripDirection(IInputElement inputElement)
        {
            Verify.IsNotNull<IInputElement>(inputElement, "inputElement");
            DependencyObject dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            return (ResizeGripDirection)dobj.GetValue(ResizeGripDirectionProperty);
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new WindowChrome();
        }

        private static readonly List<_SystemParameterBoundProperty> _BoundProperties = new List<_SystemParameterBoundProperty>
        {
            new _SystemParameterBoundProperty { DependencyProperty = CornerRadiusProperty, SystemParameterPropertyName = "WindowCornerRadius" },
            new _SystemParameterBoundProperty { DependencyProperty = CaptionHeightProperty, SystemParameterPropertyName = "WindowCaptionHeight" },
            new _SystemParameterBoundProperty { DependencyProperty = ResizeBorderThicknessProperty, SystemParameterPropertyName = "WindowResizeBorderThickness" },
            new _SystemParameterBoundProperty { DependencyProperty = GlassFrameThicknessProperty, SystemParameterPropertyName = "WindowNonClientFrameThickness" },
        };
        private static readonly NonClientFrameEdges NonClientFrameEdges_All = NonClientFrameEdges.Bottom | NonClientFrameEdges.Right | NonClientFrameEdges.Top | NonClientFrameEdges.Left;
        /// <summary>
        /// 
        /// </summary>
        public WindowChrome()
        {
            // Effective default values for some of these properties are set to be bindings
            // that set them to system defaults.
            // A more correct way to do this would be to Coerce the value iff the source of the DP was the default value.
            // Unfortunately with the current property system we can't detect whether the value being applied at the time
            // of the coersion is the default.
            foreach (var bp in _BoundProperties)
            {
                // This list must be declared after the DP's are assigned.
                Assert.IsNotNull(bp.DependencyProperty);
                BindingOperations.SetBinding(
                    this,
                    bp.DependencyProperty,
                    new Binding
                    {
                        Source = SystemParameters2.Current,
                        Path = new PropertyPath(bp.SystemParameterPropertyName),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    });
            }
        }

        private static bool _NonClientFrameEdgesAreValid(object value)
        {
            NonClientFrameEdges ncEdges = NonClientFrameEdges.None;
            try
            {
                ncEdges = (NonClientFrameEdges)value;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            if (ncEdges != NonClientFrameEdges.None)
            {
                if ((ncEdges | NonClientFrameEdges_All) != NonClientFrameEdges_All)
                {
                    return false;
                }
                if (ncEdges == NonClientFrameEdges_All)
                {
                    return false;
                }
            }
            return true;
        }

        private void _OnPropertyChangedThatRequiresRepaint()
        {
            var handler = PropertyChangedThatRequiresRepaint;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        internal event EventHandler PropertyChangedThatRequiresRepaint;
    }
}
