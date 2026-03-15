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
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
    public class FilterElement : INotifyPropertyChanged, ICloneable
    {
        private string _name;
        internal Func<object, string> FormatedString;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                if (this.FormatedString != null)
                    return this.FormatedString(this.ActualValue);
                return _name;
            }
            set
            {
                _name = value;
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


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    /// <summary>
    /// To order the FilterElement collection as ascending
    /// </summary>
    class FilterElementAscendingOrder : IComparer<FilterElement>
    {
        public int Compare(FilterElement x, FilterElement y)
        {
            if ((x.ActualValue == null|| y.ActualValue==null) || (x.ActualValue == DBNull.Value || y.ActualValue == DBNull.Value))
            {
                // below code is to bring the (Blanks) to top of the list
                if ((x.ActualValue == null && y.ActualValue == null) || (x.ActualValue == DBNull.Value && y.ActualValue == DBNull.Value))
                    return 0;
                else if (x.ActualValue == null || x.ActualValue == DBNull.Value)
                {
                    return string.Compare(x.Name.ToString(), y.ActualValue.ToString());
                }
                else if (y.ActualValue == null || y.ActualValue == DBNull.Value)
                {
                    return string.Compare(x.ActualValue.ToString(), y.Name.ToString());
                }
                else
                    return 0;
            }
            else if (x.ActualValue.GetType() == typeof(String))
            {
                return string.Compare(x.ActualValue.ToString(), y.ActualValue.ToString());
            }
            else if (x.ActualValue.GetType() == typeof(double))
            {
                if ((double)x.ActualValue < (double)y.ActualValue) return -1;
                else if ((double)x.ActualValue > (double)y.ActualValue) return 1;
                else return 0;
            }
            else if (x.ActualValue.GetType() == typeof(decimal))
            {
                if ((decimal)x.ActualValue < (decimal)y.ActualValue) return -1;
                else if ((decimal)x.ActualValue > (decimal)y.ActualValue) return 1;
                else return 0;
            }
            else if (x.ActualValue.GetType() == typeof(DateTime))
            {
                if ((DateTime)x.ActualValue < (DateTime)y.ActualValue) return -1;
                else if ((DateTime)x.ActualValue > (DateTime)y.ActualValue) return 1;
                else return 0;
            }
            else
            {
                if (x.ActualValue.GetHashCode() < y.ActualValue.GetHashCode()) return -1;
                else if (x.ActualValue.GetHashCode() > y.ActualValue.GetHashCode()) return 1;
                else return 0;
            }
        }
    }   
}
