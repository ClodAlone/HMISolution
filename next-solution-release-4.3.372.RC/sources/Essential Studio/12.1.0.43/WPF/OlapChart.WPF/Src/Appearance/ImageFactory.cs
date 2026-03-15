#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Representing ImageFactory
    /// </summary>
   internal class ImageFactory
   {
       #region Members
       public static ChartImages chartimages = new ChartImages();
        public static int count = 0;
        public static Images images = new Images();
       #endregion

       #region Properties
        /// <summary>
        /// Gets the chart images.
        /// </summary>
        /// <value>The chart images.</value>
        public static ChartImages ChartImages
        {
            get
            {
                return chartimages;
            }
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <value>The images.</value>
        public static Images Images
        {
            get
            {
                return images;
            }
        }
        #endregion
   }
   /// <summary>
   /// Representing Images for different color palettes
   /// </summary>
    public class Images : ObservableCollection<ImageData>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Images"/> class.
        /// </summary>
        public Images()
        {
            this.Add(new ImageData(@"Images\Default.png", "Default"));
            this.Add(new ImageData(@"Images\Default-Alpha.png", "DefaultAlpha"));
            this.Add(new ImageData(@"Images\Analog.png", "Analog"));
            this.Add(new ImageData(@"Images\Colorful.png", "Colorful"));
            this.Add(new ImageData(@"Images\EarthTone.png", "EarthTone"));
            this.Add(new ImageData(@"Images\GaryScale.png", "Grayscale"));
            this.Add(new ImageData(@"Images\Nature.png", "Nature"));
            this.Add(new ImageData(@"Images\Pastel.png", "Pastel"));
            this.Add(new ImageData(@"Images\Triad.png", "Triad"));
            this.Add(new ImageData(@"Images\WarmCold.png", "WarmCold"));
            this.Add(new ImageData(@"Images\Custom.png", "Custom"));

            //string path = Application.Current.StartupUri.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Representing Images for different ChartType
    /// </summary>
    public class ChartImages : ObservableCollection<ImageData>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartImages"/> class.
        /// </summary>
        public ChartImages()
        {
            this.Add(new ImageData(@"Images\Column.png", "Column"));
            this.Add(new ImageData(@"Images\StackingColumn.png", "StackingColumn"));
            this.Add(new ImageData(@"Images\StackingColumn.png", "StackingColumn100"));
            this.Add(new ImageData(@"Images\Bar.png", "Bar"));
            this.Add(new ImageData(@"Images\StackingBar.png", "StackingBar"));
            this.Add(new ImageData(@"Images\Area.png", "Area"));
            this.Add(new ImageData(@"Images\StackingArea.png", "StackingArea"));
            this.Add(new ImageData(@"Images\SplineArea.png", "SplineArea"));
            this.Add(new ImageData(@"Images\StepArea.png", "StepArea"));
            this.Add(new ImageData(@"Images\Line.png", "Line"));
            this.Add(new ImageData(@"Images\Spline.png", "Spline"));
            this.Add(new ImageData(@"Images\RotatedSpline.png", "RotatedSpline"));
            this.Add(new ImageData(@"Images\StepLine.png", "StepLine"));
            this.Add(new ImageData(@"Images\Scatter.png", "Scatter"));
        }
        #endregion
    }
    /// <summary>
    /// Representing ImageData
    /// </summary>
    public class ImageData : INotifyPropertyChanged
    {
        #region Member
        private string m_imageName = "";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageData"/> class.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="text">The text.</param>
        public ImageData(string imageName, string text)
        {
            ImageFactory.count++;
            this.ImageName = imageName;
            this.Text = text;
        }
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>The name of the image.</value>
        public string ImageName
        {
            get
            {
                return m_imageName;
            }
            set
            {
                m_imageName = value;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get;
            set;
        }
        #endregion
        
    }
}
