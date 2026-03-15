#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Displays HtmlContent inside Silverlight
    /// </summary>
    [TemplateVisualState(Name = "ValidUrl", GroupName = "UrlStates")]
    [TemplateVisualState(Name = "InvalidUrl", GroupName = "UrlStates")]

    public class HtmlHost : Control
    {
        #region Constructor
        /// <summary>
        /// Constructor Initializer
        /// </summary>
        public HtmlHost()
        {
            base.DefaultStyleKey = typeof(HtmlHost);

            this.Loaded += new RoutedEventHandler(HtmlHost_Loaded);
            this.MinHeight = 100;
            this.MinWidth = 100;
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                this.htmldocument = HtmlPage.Document;

            Binding visibilityBinding = new Binding("Visibility");
            visibilityBinding.Source = this;
            BindingOperations.SetBinding(this, HtmlHost.VisibilityInternalProperty, visibilityBinding);
        }

        /// <summary>
        /// Initializes the <see cref="HtmlHost"/> class.
        /// </summary>
        static HtmlHost()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }
        }
       
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            layoutRoot = this.GetTemplateChild("MainGrid") as Grid;
        }

        /// <summary>
        /// Loaded Method
        /// </summary>
        /// <param name="sender">Info about the event triggered</param>
        /// <param name="e">Routed Event Arguments</param>
        void HtmlHost_Loaded(object sender, RoutedEventArgs e)
        {
            this.LayoutUpdated += new EventHandler(HtmlHost_LayoutUpdated);
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                if (this.layoutRoot != null)
                    this.layoutRoot.Children.Clear();
                if (HtmlPage.BrowserInformation.UserAgent.Contains("MSIE"))
                {
                    htmldocument.Body.SetStyleAttribute("overflow", "hidden");
                }
            }
            else
                this.layoutRoot.Children.Add(new TextBlock { Text = "Syncfusion HtmlHost Control" });
        }

        /// <summary>
        /// Method is called whenever layout gets updated.
        /// </summary>
        /// <param name="sender">Arguments About Sender</param>
        /// <param name="e">Change Happened After The Event</param>
        void HtmlHost_LayoutUpdated(object sender, EventArgs e)
        {
            if(this.VisibilityInternal != System.Windows.Visibility.Collapsed)
                this.Position();

            if (iFrame != null)
            {
                Panel panel = GetPanelParent(this);
                if (panel != null)
                {
                    double elewidth = this.Width;
                    double eleheight = this.Height;
                    if (double.IsNaN(this.Width) && panel.ActualWidth < this.MinWidth)
                    {
                        elewidth = panel.ActualWidth;
                    }

                    if (double.IsNaN(this.Height) && panel.ActualHeight < this.MinHeight)
                    {
                        eleheight = panel.ActualHeight;
                    }

                    if ((panel.ActualWidth - 1) < elewidth && !((panel.ActualHeight - 1) < eleheight))
                    {
                        iFrame.SetStyleAttribute("clip", "rect(auto," + (panel.ActualWidth - 1) + "px, auto,auto)");
                    }
                    else if ((panel.ActualHeight - 1) < eleheight && !((panel.ActualWidth - 1) < elewidth))
                    {
                        iFrame.SetStyleAttribute("clip", "rect(auto,auto," + (panel.ActualHeight - 1) + "px,auto)");
                    }
                    else if ((panel.ActualHeight - 1) < eleheight && (panel.ActualWidth - 1) < elewidth)
                    {
                        iFrame.SetStyleAttribute("clip", "rect(auto," + (panel.ActualWidth - 1) + "px," + (panel.ActualHeight - 1) + "px,auto)");
                    }
                    else
                    {
                        iFrame.SetStyleAttribute("clip", "rect(auto,auto,auto,auto)");
                    }
                }
                htmldocument.Body.AppendChild(iFrame);
            }
        }

        #endregion

        #region DependencyPropeties
        /// <summary>
        /// urlSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty UrlSourceProperty = DependencyProperty.Register("UrlSource", typeof(string), typeof(HtmlHost), new PropertyMetadata(new PropertyChangedCallback(OnUrlSourceChanged)));

        /// <summary>
        /// HtmlSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty HtmlSourceProperty = DependencyProperty.Register("HtmlSource", typeof(string), typeof(HtmlHost), new PropertyMetadata(new PropertyChangedCallback(OnHtmlSourceChanged)));

        /// <summary>
        /// FrameOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrameOpacityProperty = DependencyProperty.Register("FrameOpacity", typeof(double), typeof(HtmlHost), new PropertyMetadata(new PropertyChangedCallback(OnFrameOpacityChanged)));

        #endregion

        internal Visibility VisibilityInternal
        {
            get { return (Visibility)GetValue(VisibilityInternalProperty); }
            set { SetValue(VisibilityInternalProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisibilityInternalProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(HtmlHost), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnHTMLHostVisibilityChanged)));

        #region Event Declarations

        /// <summary>
        /// Event that is raised when <see cref="UrlSource"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback UrlSourceChanged;

        /// <summary>
        /// Event that is raised when <see cref="HtmlSource"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HtmlSourceChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameOpacity"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameOpacityChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets HtmlSource Property
        /// </summary>
        public string HtmlSource
        {
            get
            {
                return (string)GetValue(HtmlSourceProperty);
            }

            set
            {
                SetValue(HtmlSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets UrlSource Property
        /// </summary>
        public string UrlSource
        {
            get
            {
                return (string)GetValue(UrlSourceProperty);
            }

            set
            {
                SetValue(UrlSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Opacity Property
        /// </summary>
        public double FrameOpacity
        {
            get
            {
                return (double)GetValue(FrameOpacityProperty);
            }

            set
            {
                SetValue(FrameOpacityProperty, value);
            }
        }

        #endregion

        #region Variable Declarations
        /// <summary>
        /// Internal Variables
        /// </summary>
        private HtmlElement element;
        private HtmlElement iFrame;
        private HtmlDocument htmldocument;

        /// <summary>
        /// Root Grid variable
        /// </summary>
        Grid layoutRoot;

        #endregion

        #region Events

        /// <summary>
        /// Updates property value cache and raises <see cref="UrlSourceChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnUrlSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, "ValidUrl", false);
            this.NavigateUrl();
            if (this.UrlSourceChanged != null)
            {
                this.UrlSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="HtmlSourceChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnHtmlSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, "ValidUrl", false);
            this.NavigateHtml();
            if (this.HtmlSourceChanged != null)
            {
                this.HtmlSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Event Fires When Opacity Property Changes
        /// </summary>
        /// <param name="e">Dependency EventArgs</param>
        protected virtual void OnFrameOpacityChanged(DependencyPropertyChangedEventArgs e)
        {
            this.FrameOpacityfunction(this.FrameOpacity);
            if (this.FrameOpacityChanged != null)
            {
                this.FrameOpacityChanged(this, e);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the opacity of the Control
        /// </summary>
        /// <param name="opacity">Value of opacity</param>
        private void FrameOpacityfunction(double opacity)
        {
            if (iFrame != null)
            {
                iFrame.SetStyleAttribute("filter", "alpha(opacity=" + (opacity * 100) + ")");
                iFrame.SetStyleAttribute("opacity", opacity.ToString());
            }
        }

        /// <summary>
        /// Position the frame with respect to the position of the control
        /// </summary>
        private void Position()
        {
            if (iFrame != null && Application.Current.RootVisual != null)
            {
                try
                {
                    GeneralTransform gt = this.TransformToVisual(Application.Current.RootVisual);
                    Point offset = gt.Transform(new Point(0, 0));
                    iFrame.SetStyleAttribute("position", "absolute");
                    iFrame.SetStyleAttribute("left", (offset.X + this.BorderThickness.Left).ToString() + "px");
                    iFrame.SetStyleAttribute("top", (offset.Y + this.BorderThickness.Top).ToString() + "px");
                    if (HtmlPage.BrowserInformation.UserAgent.Contains("MSIE"))
                    {
                        iFrame.SetStyleAttribute("width", (this.ActualWidth - (this.BorderThickness.Left + this.BorderThickness.Right)).ToString() + "px");
                        iFrame.SetStyleAttribute("height", (this.ActualHeight - (this.BorderThickness.Top + this.BorderThickness.Bottom)).ToString() + "px");
                    }
                    else
                    {
                        iFrame.SetStyleAttribute("width", (this.ActualWidth - (this.BorderThickness.Left + this.BorderThickness.Right) - 4).ToString() + "px");
                        iFrame.SetStyleAttribute("height", (this.ActualHeight - (this.BorderThickness.Top + this.BorderThickness.Bottom) - 4).ToString() + "px");
                    }

                    iFrame.SetStyleAttribute("visibility", "visible");
                }
                catch
                {
                    if (element != null && iFrame != null)
                    {
                        if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                        {
                            htmldocument.Body.RemoveChild(element);
                            htmldocument.Body.RemoveChild(iFrame);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given Htmlsource is displayed in the frame
        /// </summary>
        private void NavigateHtml()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                if (element != null && iFrame != null)
                {
                    htmldocument.Body.RemoveChild(element);
                    htmldocument.Body.RemoveChild(iFrame);
                }

                element = htmldocument.CreateElement("div");
                iFrame = htmldocument.CreateElement("span");
                iFrame.SetProperty("innerHTML", this.HtmlSource);
                iFrame.SetStyleAttribute("overflow", "scroll");
                iFrame.SetStyleAttribute("allowtransparency", "true");
                element.AppendChild(iFrame);
                htmldocument.Body.AppendChild(element);
                this.Position();
                double opacity = this.FrameOpacity;
                this.FrameOpacity = 0;
                this.FrameOpacity = opacity;
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Given Urlsource is displayed in the frame
        /// </summary>
        private void NavigateUrl()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                if (element != null && iFrame != null)
                {
                    htmldocument.Body.RemoveChild(element);
                    htmldocument.Body.RemoveChild(iFrame);
                }

                element = htmldocument.CreateElement("div");
                UriBuilder mAddress = new UriBuilder();
                try
                {
                    mAddress = new UriBuilder(this.UrlSource);
                    iFrame = htmldocument.CreateElement("IFRAME");
                    iFrame.SetAttribute("src", mAddress.Uri.AbsoluteUri);
                    element.AppendChild(iFrame);
                    htmldocument.Body.AppendChild(element);
                    this.Position();
                    iFrame.SetStyleAttribute("allowtransparency", "true");
                    double opacity = this.FrameOpacity;
                    this.FrameOpacity = 0;
                    this.FrameOpacity = opacity;
                    this.InvalidateMeasure();
                }
                catch
                {
                    VisualStateManager.GoToState(this, "InvalidUrl", false);
                }
            }
        }

        /// <summary>
        /// Gets the Panel for the current object
        /// </summary>
        /// <param name="obj">Name of the Dependency Object</param>
        /// <returns>Returns the corresponding ParentPanel</returns>
        private Panel GetPanelParent(DependencyObject obj)
        {
            while (obj != null)
            {
                DependencyObject parentElement = VisualTreeHelper.GetParent(obj);
                if (parentElement is Panel)
                {
                    return parentElement as Panel;
                }

                obj = parentElement;
            }

            return null;
        }
        #endregion

        #region Static Events

        /// <summary>
        /// Calls OnUrlSourceChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">HtmlHost object, the change occures on.</param>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        private static void OnUrlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlHost instance = (HtmlHost)d;
            instance.OnUrlSourceChanged(e);
        }

        /// <summary>
        /// Calls OnHtmlSourceChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">HtmlHost object, the change occures on.</param>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        private static void OnHtmlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlHost instance = (HtmlHost)d;
            instance.OnHtmlSourceChanged(e);
        }

        /// <summary>
        /// Static Event For Opacity Property
        /// </summary>
        /// <param name="d">Name of the Dependency Object</param>
        /// <param name="e">Event Args</param>
        private static void OnFrameOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlHost instance = (HtmlHost)d;
            instance.OnFrameOpacityChanged(e);
        }

        private static void OnHTMLHostVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlHost instance = (HtmlHost)d;
            instance.OnHTMLHostVisibilityChanged(e);
        }

        private void OnHTMLHostVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (iFrame != null)
            {
                    if(this.VisibilityInternal == System.Windows.Visibility.Visible)
                        iFrame.SetStyleAttribute("visibility", "visible");
                    else
                        iFrame.SetStyleAttribute("visibility", "hidden");
            }
        }

        #endregion
    }
}
