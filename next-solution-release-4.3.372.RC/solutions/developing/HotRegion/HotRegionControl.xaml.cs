using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Utilities;
using WPFUtilities;

namespace HotRegion
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class HotRegionControl : Button
    {
        #region Declarations
        bool bLoaded;
        PropertyChangeNotifier notifierOpacity;
        #endregion

        #region Constructors
        public HotRegionControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    Opacity = 0.3;
                }
                else
                {
                    Opacity = 0.0;
                    this.MouseEnter += HotRegionControl_MouseEnter;
                    this.MouseLeave += HotRegionControl_MouseLeave;

                    if (notifierOpacity == null)
                    {
                        notifierOpacity = new PropertyChangeNotifier(this, OpacityProperty);
                        notifierOpacity.ValueChanged += notifierOpacity_ValueChanged;
                    }
                }
            };

            Unloaded += (o, e) =>
            {
                if (!bLoaded)
                    return;
                bLoaded = false;

                if (notifierOpacity != null)
                {
                    notifierOpacity.ValueChanged -= notifierOpacity_ValueChanged;
                    notifierOpacity.Dispose();
                    notifierOpacity = null;
                }

                this.MouseEnter -= HotRegionControl_MouseEnter;
                this.MouseLeave -= HotRegionControl_MouseLeave;
            };
        }

        private void notifierOpacity_ValueChanged(object sender, EventArgs e)
        {
            Opacity = 0.0;
        }
        #endregion

        #region Methods
        void HotRegionControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        void HotRegionControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        #endregion
        
        //---------------------------------------------------------------------------------------
        //http://stackoverflow.com/questions/28441538/touching-a-wpf-button-does-sometimes-not-invoke-the-click-handler-under-windows
        //---------------------------------------------------------------------------------------
        private void control_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            Mouse.Synchronize();
        }
    }
}
