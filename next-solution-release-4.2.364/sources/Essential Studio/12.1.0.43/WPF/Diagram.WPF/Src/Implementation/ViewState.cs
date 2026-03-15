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
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Diagram
{
    public class ViewState
    {
        private double currentzoom=1d;
        public double CurrentZoom
        {
            get { return currentzoom; }
            set { currentzoom = value; }
        }

        private double zoomfactor=0.2d;
        public double ZoomFactor
        {
            get { return zoomfactor; }
            set { zoomfactor = value; }
        }

        private Rect boundaryConsrainsArea = new Rect(0, 0, 0, 0);
        public Rect BoundaryConstraintsArea
        {
            get { return boundaryConsrainsArea; }
            set { boundaryConsrainsArea = value; }
        }

        private bool sizetocontetnt = true;
        public bool SizeToContent
        {
            get { return sizetocontetnt; }
            set { sizetocontetnt = value; }
        }

        private bool boundaryConstraintsEnabled = false;
        public bool BoundaryConstraintsEnabled
        {
            get { return boundaryConstraintsEnabled; }
            set { boundaryConstraintsEnabled = value; }
        }

        private Brush pageBackground;
        public Brush PageBackground
        {
            get { return pageBackground; }
            set { pageBackground = value; }
        }

        private Brush offPagebackground;
        public Brush OffPageBackground
        {
            get { return offPagebackground; }
            set { offPagebackground = value; }
        }

        private Thickness pageMargin;
        public Thickness PageMargin
        {
            get { return pageMargin; }
            set { pageMargin = value; }
        }
    }
}
