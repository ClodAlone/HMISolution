#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls
{

    public class ScrollableContentViewerAutomationPeer : FrameworkElementAutomationPeer, IScrollProvider
    {
        public ScrollableContentViewerAutomationPeer(ScrollableContentViewer owner) : base((FrameworkElement) owner)
        {
        }

        private static double AutomationGetScrollPercent(double extent, double viewport, double actualOffset)
        {
            if (!AutomationIsScrollable(extent, viewport))
            {
                return -1.0;
            }
            return ((actualOffset * 100.0) / (extent - viewport));
        }

        private static double AutomationGetViewSize(double extent, double viewport)
        {
            if (DoubleUtil.IsZero(extent))
            {
                return 100.0;
            }
            return Math.Min((double) 100.0, (double) ((viewport * 100.0) / extent));
        }

        private static bool AutomationIsScrollable(double extent, double viewport)
        {
            return DoubleUtil.GreaterThan(extent, viewport);
        }

        //internal override bool ChildIsAcceptable(UIElement child)
        //{
        //    if (child == null)
        //    {
        //        return false;
        //    }
        //    ScrollContentViewer owner = (ScrollContentViewer) base.Owner;
        //    return (((child != owner.ElementHorizontalScrollBar) && (child != owner.ElementVerticalScrollBar)) || (child.Visibility == Visibility.Visible));
        //}

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Pane;
        }

        protected override string GetClassNameCore()
        {
            return "ScrollContentViewer";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Scroll)
            {
                return this;
            }
            return null;
        }

        //protected override bool IsControlElementCore()
        //{
        //    return !XcpImports.UIElement_HasTemplatedParent(base.Owner);
        //}

        internal void RaiseAutomationEvents(double extentX, double extentY, double viewportX, double viewportY, double offsetX, double offsetY)
        {
            IScrollProvider provider = this;
            if (AutomationIsScrollable(extentX, viewportX) != provider.HorizontallyScrollable)
            {
                base.RaisePropertyChangedEvent(ScrollPatternIdentifiers.HorizontallyScrollableProperty, AutomationIsScrollable(extentX, viewportX), provider.HorizontallyScrollable);
            }
            if (AutomationIsScrollable(extentY, viewportY) != provider.VerticallyScrollable)
            {
                base.RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticallyScrollableProperty, AutomationIsScrollable(extentY, viewportY), provider.VerticallyScrollable);
            }
            if (AutomationGetViewSize(extentX, viewportX) != provider.HorizontalViewSize)
            {
                base.RaisePropertyChangedEvent(ScrollPatternIdentifiers.HorizontalViewSizeProperty, AutomationGetViewSize(extentX, viewportX), provider.HorizontalViewSize);
            }
            if (AutomationGetViewSize(extentY, viewportY) != provider.VerticalViewSize)
            {
                base.RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalViewSizeProperty, AutomationGetViewSize(extentY, viewportY), provider.VerticalViewSize);
            }
            if (AutomationGetScrollPercent(extentX, viewportX, offsetX) != provider.HorizontalScrollPercent)
            {
                base.RaisePropertyChangedEvent(ScrollPatternIdentifiers.HorizontalScrollPercentProperty, AutomationGetScrollPercent(extentX, viewportX, offsetX), provider.HorizontalScrollPercent);
            }
            if (AutomationGetScrollPercent(extentY, viewportY, offsetY) != provider.VerticalScrollPercent)
            {
                base.RaisePropertyChangedEvent(ScrollPatternIdentifiers.VerticalScrollPercentProperty, AutomationGetScrollPercent(extentY, viewportY, offsetY), provider.VerticalScrollPercent);
            }
        }

        void IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            bool flag = horizontalAmount != ScrollAmount.NoAmount;
            bool flag2 = verticalAmount != ScrollAmount.NoAmount;
            ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
            if ((flag && !this.HorizontallyScrollable) || (flag2 && !this.VerticallyScrollable))
            {
                throw new InvalidOperationException("UIA_OperationCannotBePerformed");
            }
            switch (horizontalAmount)
            {
                case ScrollAmount.LargeDecrement:
                    owner.PageLeft();
                    break;

                case ScrollAmount.SmallDecrement:
                    owner.LineLeft();
                    break;

                case ScrollAmount.NoAmount:
                    break;

                case ScrollAmount.LargeIncrement:
                    owner.PageRight();
                    break;

                case ScrollAmount.SmallIncrement:
                    owner.LineRight();
                    break;

                default:
                    throw new InvalidOperationException(("UIA_OperationCannotBePerformed"));
            }
            switch (verticalAmount)
            {
                case ScrollAmount.LargeDecrement:
                    owner.PageUp();
                    return;

                case ScrollAmount.SmallDecrement:
                    owner.LineUp();
                    return;

                case ScrollAmount.NoAmount:
                    return;

                case ScrollAmount.LargeIncrement:
                    owner.PageDown();
                    return;

                case ScrollAmount.SmallIncrement:
                    owner.LineDown();
                    return;
            }
            throw new InvalidOperationException(("UIA_OperationCannotBePerformed"));
        }

        void IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
            bool flag = horizontalPercent != -1.0;
            bool flag2 = verticalPercent != -1.0;
            ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
            if ((flag && !this.HorizontallyScrollable) || (flag2 && !this.VerticallyScrollable))
            {
                throw new InvalidOperationException(("UIA_OperationCannotBePerformed"));
            }
            if ((flag && (horizontalPercent < 0.0)) || (horizontalPercent > 100.0))
            {
                throw new ArgumentOutOfRangeException("horizontalPercent");//, horizontalPercent.ToString(CultureInfo.InvariantCulture), "0", "100" );
            }
            if ((flag2 && (verticalPercent < 0.0)) || (verticalPercent > 100.0))
            {
                throw new ArgumentOutOfRangeException("verticalPercent");//, verticalPercent.ToString(CultureInfo.InvariantCulture), "0", "100" );
            }
            if (flag)
            {
                owner.ScrollToHorizontalOffset(((owner.ExtentWidth - owner.ViewportWidth) * horizontalPercent) * 0.01);
            }
            if (flag2)
            {
                owner.ScrollToVerticalOffset(((owner.ExtentHeight - owner.ViewportHeight) * verticalPercent) * 0.01);
            }
        }

        private bool HorizontallyScrollable
        {
            get
            {
                ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
                return ((owner.ScrollInfo != null) && DoubleUtil.GreaterThan(owner.ExtentWidth, owner.ViewportWidth));
            }
        }

        bool IScrollProvider.HorizontallyScrollable
        {
            get
            {
                return this.HorizontallyScrollable;
            }
        }

        double IScrollProvider.HorizontalScrollPercent
        {
            get
            {
                if (!this.HorizontallyScrollable)
                {
                    return -1.0;
                }
                ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
                return ((owner.HorizontalOffset * 100.0) / (owner.ExtentWidth - owner.ViewportWidth));
            }
        }

        double IScrollProvider.HorizontalViewSize
        {
            get
            {
                ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
                if ((owner.ScrollInfo != null) && !DoubleUtil.IsZero(owner.ExtentWidth))
                {
                    return Math.Min((double) 100.0, (double) ((owner.ViewportWidth * 100.0) / owner.ExtentWidth));
                }
                return 100.0;
            }
        }

        bool IScrollProvider.VerticallyScrollable
        {
            get
            {
                return this.VerticallyScrollable;
            }
        }

        double IScrollProvider.VerticalScrollPercent
        {
            get
            {
                if (!this.VerticallyScrollable)
                {
                    return -1.0;
                }
                ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
                return ((owner.VerticalOffset * 100.0) / (owner.ExtentHeight - owner.ViewportHeight));
            }
        }

        double IScrollProvider.VerticalViewSize
        {
            get
            {
                ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
                if ((owner.ScrollInfo != null) && !DoubleUtil.IsZero(owner.ExtentHeight))
                {
                    return Math.Min((double) 100.0, (double) ((owner.ViewportHeight * 100.0) / owner.ExtentHeight));
                }
                return 100.0;
            }
        }

        private bool VerticallyScrollable
        {
            get
            {
                ScrollableContentViewer owner = (ScrollableContentViewer) base.Owner;
                return ((owner.ScrollInfo != null) && DoubleUtil.GreaterThan(owner.ExtentHeight, owner.ViewportHeight));
            }
        }
    }
}
