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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace Syncfusion.WinRT.Controls
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class PanningInfo
    {
        private double deltaPerHorizontalOffet;
        public double DeltaPerHorizontalOffet
        {
            get
            {
                return this.deltaPerHorizontalOffet;
            }
            set
            {
                this.deltaPerHorizontalOffet = value;
            }
        }

        private double deltaPerVerticalOffset;
        public double DeltaPerVerticalOffset
        {
            get
            {
                return this.deltaPerVerticalOffset;
            }
            set
            {
                this.deltaPerVerticalOffset = value;
            }
        }

        private int inertiaBoundaryBeginTimestamp;
        public int InertiaBoundaryBeginTimestamp
        {
            get
            {
                return this.inertiaBoundaryBeginTimestamp;
            }
            set
            {
                this.inertiaBoundaryBeginTimestamp = value;
            }
        }

        private bool inHorizontalFeedback;
        public bool InHorizontalFeedback
        {
            get
            {
                return this.inHorizontalFeedback;
            }
            set
            {
                this.inHorizontalFeedback = value;
            }
        }

        private bool inVerticalFeedback;
        public bool InVerticalFeedback
        {
            get
            {
                return this.inVerticalFeedback;
            }
            set
            {
                this.inVerticalFeedback = value;
            }
        }

        private bool isPanning;
        public bool IsPanning
        {
            get
            {
                return this.isPanning;
            }
            set
            {
                this.isPanning = value;
            }
        }

        private double originalHorizontalOffset;
        public double OriginalHorizontalOffset
        {
            get
            {
                return this.originalHorizontalOffset;
            }
            set
            {
                this.originalHorizontalOffset = value;
            }
        }

        private double originalVerticalOffset;
        public double OriginalVerticalOffset
        {
            get
            {
                return this.originalVerticalOffset;
            }
            set
            {
                this.originalVerticalOffset = value;
            }
        }

        private PanningMode panningMode;
        public PanningMode PanningMode
        {
            get
            {
                return this.panningMode;
            }
            set
            {
                this.panningMode = value;
            }
        }

        private Point unusedTranslation;
        public Point UnusedTranslation
        {
            get
            {
                return this.unusedTranslation;
            }
            set
            {
                this.unusedTranslation = value;
            }
        }
    }

    public enum PanningMode
    {
        None,
        HorizontalOnly,
        VerticalOnly,
        Both,
        HorizontalFirst,
        VerticalFirst
    }
}