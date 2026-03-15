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

namespace Syncfusion.UI.Xaml.Grid
{

    [ClassReference(IsReviewed = false)]
    public class FilterElement : INotifyPropertyChanged
    {
        private string _DisplayText;
        internal Func<object, string> FormatedString;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string DisplayText
        {
            get
            {
                if (this.FormatedString != null)
                    return this.FormatedString(this.ActualValue);
                return _DisplayText;
            }
            set
            {
                _DisplayText = value;
            }
        }

        private object _actualValue;

        /// <summary>
        /// Gets or sets the actual value.
        /// </summary>
        /// <value>The actual value.</value>
        public object ActualValue
        {
            get
            {
                return _actualValue;
            }
            set
            {
                _actualValue = value;
            }
        }

        private bool _isSelected;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    /// <summary>
    /// To order the FilterElement collection as ascending
    /// </summary>
    [ClassReference(IsReviewed = false)]
    class FilterElementAscendingOrder : IComparer<FilterElement>
    {
        public int Compare(FilterElement x, FilterElement y)
        {
#if WinRT
            if (x.ActualValue == null || y.ActualValue == null || x.ActualValue is Nullable || y.ActualValue is Nullable)
#else
            if (x.ActualValue == null || y.ActualValue == null || x.ActualValue is DBNull || y.ActualValue is DBNull)
#endif
            {
#if WinRT
                if ((x.ActualValue == null && y.ActualValue == null) ||
                    (x.ActualValue is Nullable && y.ActualValue is Nullable))
#else
                if ((x.ActualValue == null && y.ActualValue == null) ||
                    (x.ActualValue is DBNull && y.ActualValue is DBNull))
#endif
                    return 0;

#if WinRT
                if (x.ActualValue == null || x.ActualValue is Nullable)
#else
                if (x.ActualValue == null || x.ActualValue is DBNull)
#endif
                    return -1;
                return 1;
            }

            if (x.ActualValue is string)
            {
                return String.Compare(x.ActualValue.ToString(), y.ActualValue.ToString());
            }
            if (x.ActualValue is double)
            {
                if ((double)x.ActualValue < (double)y.ActualValue)
                    return -1;
                return (double)x.ActualValue > (double)y.ActualValue ? 1 : 0;
            }
            if (x.ActualValue is decimal)
            {
                if ((decimal)x.ActualValue < (decimal)y.ActualValue)
                    return -1;
                return (decimal)x.ActualValue > (decimal)y.ActualValue ? 1 : 0;
            }
            if (x.ActualValue is DateTime)
            {
                if ((DateTime)x.ActualValue < (DateTime)y.ActualValue)
                    return -1;
                return (DateTime)x.ActualValue > (DateTime)y.ActualValue ? 1 : 0;
            }
            if (x.ActualValue.GetHashCode() < y.ActualValue.GetHashCode())
                return -1;
            return x.ActualValue.GetHashCode() > y.ActualValue.GetHashCode() ? 1 : 0;
        }
    }

}
