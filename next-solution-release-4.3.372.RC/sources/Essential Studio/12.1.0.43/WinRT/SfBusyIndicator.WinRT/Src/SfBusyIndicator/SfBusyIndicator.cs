#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
namespace Syncfusion.WP.Controls.Notification
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
namespace Syncfusion.Tools.Controls.Notification
#elif WPF
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;
using System.Windows.Media.Animation;
using System.Collections;
namespace Syncfusion.Windows.Controls.Notification
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
{
    /// <summary>
    /// Represents a control that allows the user to define when another program is in progress
    /// </summary>
    /// <remarks>
    /// <para>The busy indicator control is an animation technique, used 
    /// to enable the user to know that another process is in progress.
    /// </para>
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Notification.AnimationTypes"/>
    [ClassReference(IsReviewed = false)]
    public class SfBusyIndicator : ContentControl,IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.BusyIndicator"/> class.
        /// </summary>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Notification">Syncfusion.UI.Xaml.Controls.Notification
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfBusyIndicator()
        {
#if WPF
            if (EnvironmentTestNotification.IsSecurityGranted)
            {
                EnvironmentTestNotification.StartValidateLicense(typeof(SfBusyIndicator));
            }
#endif
            DefaultStyleKey = typeof (SfBusyIndicator);
            this.Loaded += SfBusyIndicator_Loaded;
            this.Unloaded += SfBusyIndicator_Unloaded;
        }

        #region Variables

        Grid LayoutRoot;
        Storyboard storyBoard;         

        #endregion

        void SfBusyIndicator_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= SfBusyIndicator_Loaded;          
            this.Unloaded -= SfBusyIndicator_Unloaded;
            this.IsEnabledChanged -= SfBusyIndicator_IsEnabledChanged;
        }

        void SfBusyIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVisualState();
            this.IsEnabledChanged += SfBusyIndicator_IsEnabledChanged;
        }

        void SfBusyIndicator_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();          
        }        

        private void UpdateVisualState()
        {           
#if !WINRT
            var visualStateGroup = LayoutRoot == null ? null : (VisualStateManager.GetVisualStateGroups(LayoutRoot).OfType<VisualStateGroup>().FirstOrDefault());
            var visualState = visualStateGroup == null ? null : visualStateGroup.States.OfType<VisualState>().Where(s => s.Name == this.AnimationType.ToString()).FirstOrDefault();
            storyBoard = visualState == null ? null : visualState.Storyboard;

            if (!this.IsEnabled)
            {
                if (LayoutRoot != null)
                {
                    if (storyBoard != null)
                    {
                        VisualStateManager.GoToState(this, "Disabled", true);
#if WPF
                        storyBoard.Begin(LayoutRoot,true);
                        storyBoard.Pause(LayoutRoot);
#elif SILVERLIGHT || WINDOWS_PHONE
                        storyBoard.Begin();
                        storyBoard.Pause();
#endif

                    }

                }
            }
            else
            {
                if (storyBoard != null)
                {
                    VisualStateManager.GoToState(this, "Normal", true);
#if WPF
                    storyBoard.Begin(LayoutRoot);
#elif SILVERLIGHT || WINDOWS_PHONE
                    storyBoard.Begin();                   
#endif
                }
            }
#else
 var visualStateGroup = LayoutRoot == null ? null : VisualStateManager.GetVisualStateGroups(LayoutRoot).FirstOrDefault();
            var visualState = visualStateGroup == null ? null : visualStateGroup.States.Where(s => s.Name == this.AnimationType.ToString()).FirstOrDefault();
            storyBoard = visualState == null ? null : visualState.Storyboard;

            if (!this.IsEnabled)
            {
                if (LayoutRoot != null)
                {                  
                    if (storyBoard != null)
                    {
                        VisualStateManager.GoToState(this, "Disabled", true);
                        storyBoard.Begin();
                        storyBoard.Pause();                       
                    }

                }
            }
            else
            {
                if (storyBoard != null)
                {
                    VisualStateManager.GoToState(this, "Normal", true);                    
                    storyBoard.Begin();                    
                }
            }
#endif
           
        }       
      

        /// <summary>
        /// Gets or sets the AnimationType of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/>
        /// </summary>
        /// <value>
        /// The default value is <see cref="P:Syncfusion.UI.Xaml.Controls.Notification.AnimationTypes.Flower">AnimationTypes.Flower</see>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator.OnAnimationTypeChanged"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/>
        [ClassReference(IsReviewed = false)]
        public AnimationTypes AnimationType
        {
            get { return (AnimationTypes)GetValue(AnimationTypeProperty); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for AnimationType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register("AnimationType", typeof(AnimationTypes), typeof(SfBusyIndicator), new PropertyMetadata(AnimationTypes.Flower,new PropertyChangedCallback(OnAnimationTypeChanged)));


        /// <summary>
        /// Gets or sets the ViewboxHeight of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> 
        /// </summary>
        /// <value>
        /// The default value is 0.0.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double ViewboxHeight
        {
            get { return (double)GetValue(ViewboxHeightProperty); }
            set { SetValue(ViewboxHeightProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for ViewboxHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ViewboxHeightProperty =
            DependencyProperty.Register("ViewboxHeight", typeof(double), typeof(SfBusyIndicator), new PropertyMetadata(0.0));



        /// <summary>
        /// Gets or sets the ViewboxWidth of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> 
        /// </summary>
        /// <value>
        /// The default value is 0.0.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double ViewboxWidth
        {
            get { return (double)GetValue(ViewboxWidthProperty); }
            set { SetValue(ViewboxWidthProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for ViewboxWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ViewboxWidthProperty =
            DependencyProperty.Register("ViewboxWidth", typeof(double), typeof(SfBusyIndicator), new PropertyMetadata(0.0));



        /// <summary>
        /// Gets or sets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(SfBusyIndicator), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the Header of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/>
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(SfBusyIndicator), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the HeaderTemplate of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/>
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SfBusyIndicator), new PropertyMetadata(null));

        

        private static void OnAnimationTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as SfBusyIndicator;
            if(control != null)
            {
                control.OnAnimationTypeChanged(e);
            }
        }

        /// <summary>
        /// Sets the State of Animation type of the<see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
        protected virtual void OnAnimationTypeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null && args.OldValue != null && args.OldValue != args.NewValue)
            {
#if !WINRT
               AnimationTypes oldAnimation=(AnimationTypes)(args.OldValue);
               var visualStateGroup = LayoutRoot == null ? null : (VisualStateManager.GetVisualStateGroups(LayoutRoot).OfType<VisualStateGroup>().FirstOrDefault());
               var visualState = visualStateGroup == null ? null : visualStateGroup.States.OfType<VisualState>().Where(s => s.Name == oldAnimation.ToString()).FirstOrDefault();
               storyBoard = visualState == null ? null : visualState.Storyboard;
               if (storyBoard != null)
                   storyBoard.Stop();
#else
                AnimationTypes oldAnimation=(AnimationTypes)(args.OldValue);
                var visualStateGroup = LayoutRoot == null ? null : VisualStateManager.GetVisualStateGroups(LayoutRoot).FirstOrDefault();
                var visualState = visualStateGroup == null ? null : visualStateGroup.States.Where(s => s.Name == oldAnimation.ToString()).FirstOrDefault();
                storyBoard = visualState == null ? null : visualState.Storyboard;
                if (storyBoard != null)
                    storyBoard.Stop();
#endif
            }
            VisualStateManager.GoToState(this, AnimationType.ToString(), true);
        }
        /// <summary>
        /// Sets the State of Animation type of the<see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control while applying template.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is changed; otherwise, <c>false</c>.
        /// </value>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            LayoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            VisualStateManager.GoToState(this, AnimationType.ToString(), true);
            base.OnApplyTemplate();
#if SILVERLIGHT
            UpdateVisualState();
#endif
        }
        /// <summary>
        /// Disposes the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Notification.SfBusyIndicator"/> control while unloading.
        /// </summary>        
        public void Dispose()
        {
            if(storyBoard!=null)
            storyBoard.Stop();
        }

    }
}
