#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// DoubleRange Class. A class with two values Start and End of type double. It is used for determining the range of RangeSlider Control
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [TypeConverter(typeof(DoubleRangeConverter))]
    public class DoubleRange : INotifyPropertyChanged
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
                NotifyPropertyChanged("Start");
            }
        }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public double End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
                NotifyPropertyChanged("End");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Member static Variable for the range which is not empty
        /// </summary>
        public static DoubleRange Empty = new DoubleRange(0, 0);

        #endregion Public Properties

        #region Internal Variables

        /// <summary>
        /// Member Variable for the end
        /// </summary>
        internal double end;

        /// <summary>
        /// Member Variable for the start
        /// </summary>
        internal double start;

        #endregion Internal Variables

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public DoubleRange(double start, double end)
        {
            this.Start = start;
            this.End = end;
            if (this != Empty)
                this.IsEmpty = false;
            else
                this.IsEmpty = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleRange"/> class.
        /// </summary>
        public DoubleRange()
        {
        }

        #endregion Initialization

        #region Public Methods

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Public Methods

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        //SU I78477
        //public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        //EU I78477

        #endregion INotifyPropertyChanged Members
    }
}