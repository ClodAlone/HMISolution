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
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Double Range Class.
    /// </summary>
    [TypeConverter (typeof(DoubleRangeConverter))]
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
        /// Represents the Empty Static variable.
        /// </summary>
        public static DoubleRange Empty = new DoubleRange(0, 0);
        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public  bool IsEmpty { get;set; }
        #endregion

        #region Internal Variables
        internal double end;
        internal double start;
        #endregion

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
        { }
        #endregion

        #region PublicMethods
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
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    
}
