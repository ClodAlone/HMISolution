using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Text;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Gauges
{
    public sealed partial class CircularGauge : UserControl
    {
        public CircularGauge()
        {
            this.InitializeComponent();

            UpdateGuageControl();
        }

        DevExpress.UI.Xaml.Gauges.CircularGauge circularGaugeObject;
        private void UpdateGuageControl()
        {
            try
            {
                if (circularGaugeObject == null)
                {
                    circularGaugeObject = new DevExpress.UI.Xaml.Gauges.CircularGauge()
                    {
                        Width = 300,
                        Height = 300,
                        EnableAnimation = true,
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    if (!container.Children.Contains(circularGaugeObject))
                        container.Children.Add(circularGaugeObject);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
