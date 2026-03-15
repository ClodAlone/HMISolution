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
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
     /// <summary>
     ///ChartColorModel contains a number of predefined color palette and have custom brushes collection to populate a custom palette.
     /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartColorModel
    {
        #region fields

        private List<Brush> metroBrushes;

        private List<Brush> currentBrushes;

        private ChartColorPalette palette;

        private List<Brush> customBrushes;

        #endregion

        #region properties

        internal ChartColorPalette Palette 
        {
            get
            {
                return palette;
            }
            set
            {
                if (value != palette)
                {
                    palette = value;
                    ApplyPalette(palette);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the custom brushes to be used to paint the interiors of each segment in series.
        /// </summary>
        public List<Brush> CustomBrushes
        {
            get
            {
                return customBrushes;
            }
            set
            {
                customBrushes = value;
                if (Palette == ChartColorPalette.Custom)
                {
                    ApplyPalette(Palette);
                }
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartColorModel
        /// </summary>
        public ChartColorModel()
        {
            CustomBrushes = new List<Brush>();
            this.ApplyPalette(ChartColorPalette.Metro);
        }

        /// <summary>
        /// Called when instance created for ChartColorModel with single arguments
        /// </summary>
        /// <param name="palette"></param>
        public ChartColorModel(ChartColorPalette palette)
        {
            if (Palette == palette)
                this.ApplyPalette(palette);
            Palette = palette;
        }

        #endregion

        #region methods

        internal void ApplyPalette(ChartColorPalette palette)
        {
            switch (palette)
            {
                case ChartColorPalette.Metro:
                    currentBrushes = GetMetroBrushes();
                    break;
                case ChartColorPalette.Custom:
                    currentBrushes = CustomBrushes;
                    break;
            }
        }

        /// <summary>
        /// Returns the collection of brushes for specified pallete
        /// </summary>
        /// <param name="palette">ChartColorPalette</param>
        /// <returns>List of brushes</returns>
        [ClassReference(IsReviewed = false)]
        public List<Brush> GetBrushes(ChartColorPalette palette)
        {
            switch (palette)
            {
                case ChartColorPalette.Metro:
                    return GetMetroBrushes();
            }

            return null;
        }

        /// <summary>
        /// Returns the brushes used for metro palette.
        /// </summary>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public List<Brush> GetMetroBrushes()
        {
            if (metroBrushes == null)
            {
                metroBrushes = new List<Brush>()
                {
                    new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0x50, 0x00)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x99, 0x33)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0xC1, 0x39)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0x00, 0x73)),               
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0x96, 0x09)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0x71, 0xB8)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xA2, 0x00, 0xFF)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0x14, 0x00)),
                    new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xAB, 0xA9))
                };
            }

            return metroBrushes;
        }

        /// <summary>
        /// Returns the brush at the specified index for current palette
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public Brush GetBrush(int colorIndex)
        {
            if (this.currentBrushes != null && currentBrushes.Count > 0)
                return this.currentBrushes[colorIndex % currentBrushes.Count()];
            return null;
        }

        #endregion
    }
}
