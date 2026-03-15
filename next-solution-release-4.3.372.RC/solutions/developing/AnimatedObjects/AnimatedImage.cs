using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using ScreenSettings;
using StringManager.ComponentService;
using Utilities;
using UFInterfaces;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Threading;
using AnimatedObjects;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AnimatedObjects
{
    public class AnimatedImage : AnimatedObject, IDisposable
    {
        #region DP

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new AnimatedObjects.Controls.ImageSmartControl(this);
            }
        }

        #endregion

        #region ctor
        static AnimatedImage()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedImage), new FrameworkPropertyMetadata(typeof(AnimatedImage)));
        }
        public AnimatedImage()
        {

#if WINDOWS_UWP
            DefaultStyleKey = typeof(AnimatedImage1);
#endif
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bLDisposed)
                {
                    bLoaded = true;
                }

                if (!bLDisposed && !DesignerProperties.GetIsInDesignMode(this) && templateApplied)
                    RefreshAnimation();
            };
        }
        #endregion

        #region  Methods
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.templateApplied = true;
            RefreshAnimation();
        }
        void RefreshAnimation()
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && monitoredItemViewModel != null)
            {
                if (textBinding == null)
                    UpdateMonitoredValue(monitoredItemViewModel);
                else if (Value != null)
                    base.UpdateAnimation(Value);
            }
        }
        protected override internal void UpdateBackImage()
        {
            base.UpdateBackImage();
            if (BackImage == null)
                BackImage = (TryFindResource("Diamond") as BitmapImage).UriSource;
            if (backImage != null)
                backImage.Stretch = this.Stretch;
        }
        #endregion

        #region IDIsposable
        bool bLDisposed;
        public override void Dispose()
        {
            if (bLDisposed)
                return;
            bLDisposed = true;

            base.Dispose();    
        }
        #endregion
    }
}
