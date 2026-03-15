#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// Provides all properties to configure a scrollbar.
    /// </summary>
    public class ScrollInfo : IScrollBar, INotifyPropertyChanged
    {
        double value = 0;
        double minimum = 0;
        double maximum = 100;
        double largeChange = 10;
        double smallChange = 1;
        double proposedLargeChange = 10;
        bool enabled = true;

        /// <summary>
        /// Gets or sets a number that represents the current position of the scroll box on the scroll bar control.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the value of the property when the scroll box is moved a large distance.
        /// </summary>
        public double LargeChange
        {
            get
            {
                return this.largeChange;
            }
            set
            {
                this.proposedLargeChange = value;
                if (value < 0)
                    value = Maximum - Minimum;

                if (this.largeChange != value)
                {
                    this.largeChange = value;
                    OnPropertyChanged("LargeChange");
                }
            }
        }

        /// <summary>
        /// Gets or sets the upper limit of values of the scrollable range.
        /// </summary>
        public double Maximum
        {
            get
            {
                return this.maximum;
            }
            set
            {
                if (this.maximum != value)
                {
                    if (value >= 0)
                        this.maximum = value;
                    else
                        throw new Exception("Maximum value should be a non-negative.");
                    OnPropertyChanged("Maximum");
                }
            }
        }

        /// <summary>
        /// Gets or sets the lower limit of values of the scrollable range.
        /// </summary>
        public double Minimum
        {
            get
            {
                return this.minimum;
            }
            set
            {
                if (this.minimum != value)
                {
                    if (value >= 0)
                        this.minimum = value;
                    else
                        throw new Exception("Minimum value should be a non-negative.");
                    OnPropertyChanged("Minimum");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value to be added to or subtracted from the value of the property when the scroll box is moved a small distance.
        /// </summary>
        public double SmallChange
        {
            get
            {
                return this.smallChange;
            }
            set
            {
                if (this.smallChange != value)
                {
                    this.smallChange = value;
                    OnPropertyChanged("SmallChange");
                }
            }
        }

        /// <summary>
        /// Gets or sets a numeric value that represents the current position of the scroll box on the scroll bar control.
        /// </summary>
        public double Value
        {
            get
            {
                if (this.proposedLargeChange < 0)
                {
                    return this.minimum;
                }

                return Math.Max(this.minimum, Math.Min(this.maximum - this.largeChange + 1, this.value));
            }
            set
            {
                if (this.value != value)
                {
                    ValueChangingEventArgs e = new ValueChangingEventArgs(value, this.value);
                    if (ValueChanging != null)
                    {
                        ValueChanging(this, e);
                    }

                    if (!e.Cancel)
                    {
                        double offset = e.NewValue;

                        if (offset < this.Minimum || this.LargeChange > this.Maximum - this.Minimum)
                        {
                            offset = this.Minimum;
                        }
                        else
                        {
                            if (offset + this.LargeChange > this.Maximum)
                            {
                                offset = Math.Max(this.Minimum, this.Maximum - this.LargeChange + 1);
                            }
                        }

                        this.value = offset;
                        OnPropertyChanged("Value");
                        if (ValueChanged != null)
                        {
                            ValueChanged(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Called when a property is changed and raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///  <para>Occurs when the <see cref="Value" /> property has changed.</para>
        /// </summary>
        [
        Category(@"Action"),
        Description(@"Occurs when the value of the control changes.")
        ]
        public event EventHandler ValueChanged;

        /// <summary>
        ///  <para>Occurs before the <see cref="Value" /> property is changed.</para>
        /// </summary>
        [
        Category(@"Action"),
        Description(@"Occurs before the value of the control changes.")
        ]
        public event ValueChangingEventHandler ValueChanging;

        /// <summary>
        /// Copies current settings to another object.
        /// </summary>
        /// <param name="sb">another object.</param>
        public void CopyTo(ScrollInfo sb)
        {
            sb.value = value;
            sb.minimum = minimum;
            sb.maximum = maximum;
            sb.largeChange = largeChange;
            sb.smallChange = smallChange;
            sb.enabled = enabled;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public ScrollInfo Clone()
        {
            ScrollInfo sb = new ScrollInfo();
            CopyTo(sb);
            return sb;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ScrollInfo"/> is equal to the current <see cref="ScrollInfo"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ScrollInfo"/> to compare with the current <see cref="ScrollInfo"/>.</param>
        /// <returns>
        /// true if the specified <see cref="ScrollInfo"/> is equal to the current <see cref="ScrollInfo"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            ScrollInfo sb = (ScrollInfo)obj;
            if (sb == null && this == null)
                return true;
            else if (this == null || sb == null)
                return false;

            return sb.value == value &&
                sb.minimum == minimum &&
                sb.maximum == maximum &&
                sb.largeChange == largeChange &&
                sb.smallChange == smallChange &&
                sb.enabled == enabled;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="ScrollInfo"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format(
                "ScrollInfo ( Value = {0}, Minimum = {1}, Maximum = {2}, LargeChange = {3}, Enabled = {4} )",
                value, minimum, maximum, largeChange, enabled);

        }
    }


    /// <summary>
    /// Handles events with <see cref="ValueChangingEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ValueChangingEventArgs"/> that contains the event data.</param>
    public delegate void ValueChangingEventHandler(object sender, ValueChangingEventArgs e);

    /// <summary>
    /// Holds <see cref="NewValue"/> and <see cref="OldValue"/> properties.
    /// </summary>
    public sealed class ValueChangingEventArgs : SyncfusionCancelEventArgs
    {
        double newValue;
        double oldValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangingEventArgs"/> class.
        /// </summary>
        /// <param name="newValue">NewValue.</param>
        /// <param name="oldValue">OldValue.</param>
        public ValueChangingEventArgs(double newValue, double oldValue)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        /// <summary>
        /// Gets newValue.
        /// </summary>
        /// <value>NewValue.</value>
        public double NewValue
        {
            get
            {
                return newValue;
            }
        }

        /// <summary>
        /// Gets oldValue.
        /// </summary>
        /// <value>OldValue.</value>
        public double OldValue
        {
            get
            {
                return oldValue;
            }
        }
    }

}
