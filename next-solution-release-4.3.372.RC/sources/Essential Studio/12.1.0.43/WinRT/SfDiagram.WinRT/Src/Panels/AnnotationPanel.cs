#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
#endif
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Panels
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public class AnnotationPanel : Panel
    {
        //internal Size _mAvailableSize;
        internal Size _mDesiredSize;
        internal Size _mActualSize;

        private bool _mMeasureing = false;
        //private bool _mArranging = false;

        private Node _mParentNode;

        internal void ForceInvalidateMeasure()
        {
            if (!_mMeasureing)
            {
                InvalidateMeasure();
            }
            this.Loaded += AnnotationPanel_Loaded;
        }

        void AnnotationPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _mParentNode = this.FindVisualParent<Node>();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _mMeasureing = true;
            _mActualSize = availableSize;
            _mDesiredSize = new Size(0, 0);
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                if (child is AnnotationEditor)
                {
                    _mDesiredSize = _mDesiredSize.Max((child as AnnotationEditor)._mDesiredSize);
                }
            }
            if (_mParentNode == null)
            {
                _mParentNode = this.FindVisualParent<Node>();
            }
            if (_mParentNode != null)
            {
                _mParentNode.ForceInvalidateMeasure();
            }
            _mMeasureing = false;
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _mActualSize = new Size(0, 0);
            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(new Point(0, 0), new Size(0, 0)));
                if (child is AnnotationEditor)
                {
                    _mActualSize = _mActualSize.Max((child as AnnotationEditor)._mActualSize);
                }
            }
            return new Size(0,0);
        }
    }
}
