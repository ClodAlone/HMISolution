#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Silverlight.Chart.Olap
{
    public class OlapScrollpanel : Panel
    {
        private OlapLabelPanel m_olapLabelPanel = null;

        public OlapScrollpanel()
        {
            this.Loaded += new RoutedEventHandler(OlapScrollpanel_Loaded);
        }

        void OlapScrollpanel_Loaded(object sender, RoutedEventArgs e)
        {
          
            
        }

        internal OlapLabelPanel LabelPanel
        {
            get
            {
                return m_olapLabelPanel;
            }
            set
            {
                m_olapLabelPanel = value;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            Thickness axisThickness = this.ParentArea.AxesThickness;
            foreach (UIElement uielement in this.Children)
            {
                if (uielement is Grid || uielement is Border || uielement is OlapScrollpanel)
                {
                    uielement.Arrange(new Rect(axisThickness.Left, finalSize.Height - axisThickness.Bottom, uielement.DesiredSize.Width, uielement.DesiredSize.Height));
                }
            }
            return finalSize;

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement ele in this.Children)
            {
                ele.Measure(availableSize);
            }
            this.m_olapLabelPanel = new OlapLabelPanel();
            this.LabelPanel = this.m_olapLabelPanel;
            this.Children.Add(this.m_olapLabelPanel);
            if (this.ParentArea != null && this.ParentArea.CurrentEngine != null)
                this.m_olapLabelPanel.LabelSource = this.ParentArea.CurrentEngine;


            foreach (UIElement ele in this.Children)
            {
                ele.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        internal OlapArea ParentArea
        {
            get
            {
                if (m_olapArea == null)
                {
                    return this.GetParentArea();
                }
                return m_olapArea;
            }
        }
        internal OlapArea GetParentArea()
        {
            DependencyObject element = this;
            while (!(element is OlapArea))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                OlapArea area = element as OlapArea;
                if (area != null)
                {
                    area.PrimaryAxis.IsAutoSetRange = false;
                }
                return element as OlapArea;
            }
            return null;
        }

        public OlapArea m_olapArea { get; set; }
    }
}
