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

#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Data;

#endif

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    public class ScrollableElement : NotifyPropertyChangeHelper, IScrollableElement
    {
        #region Private Members

        private FrameworkElement element;
        private int index;
        private bool isEnsured;
        private bool isElipsisElement;
        private ElipsisPosition elipsisPosition;

        #endregion

        #region Ctor

        public ScrollableElement()
        {

        }

        #endregion

        #region Public Members

        public bool IsElipsisElement
        {
            get { return isElipsisElement; }
            set
            {
                isElipsisElement = value;
                OnPropertyChanged("IsElipsisElement");
            }
        }

        public ElipsisPosition ElipsisPosition
        {
            get { return elipsisPosition; }
            set
            {
                elipsisPosition = value;
                OnPropertyChanged("ElipsisPosition");
            }
        }

        #endregion

        #region IElement Members

        public FrameworkElement Element
        {
            get { return element; }
            set { element = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public bool IsEnsured
        {
            get { return isEnsured; }
            set { isEnsured = value; }
        }

        bool IScrollableElement.IsElipsisElement
        {
            get { return IsElipsisElement; }
        }

        #endregion

        #region Internal Methods

        internal void InitializeElement(Style numericButtonStyle,string elipsisContent)
        {
            NumericButton button = new NumericButton();

            if (!this.IsElipsisElement)
            {
                button.DataContext = index + 1;
                button.PageIndex = index + 1;
                button.Content = button.PageIndex;
            }
            else
            {
                button.Content = elipsisContent;
            }
            if (numericButtonStyle != null)
                button.Style = numericButtonStyle;
            this.element = button;
            this.SetBinding();
        }

        internal void UpdateElement()
        {
            var numButton = this.Element as NumericButton;
            numButton.DataContext = index + 1;
            numButton.PageIndex = index + 1;
            numButton.Content = numButton.PageIndex;
            numButton.IsCurrentPage = false;
        }

        #endregion

        #region Private Methods

        private void SetBinding()
        {
            var bind = new Binding();
            bind.Path = new PropertyPath("IsElipsisElement");
            bind.Source = this;
            bind.Mode = BindingMode.TwoWay;
            this.Element.SetBinding(NumericButton.IsElipsisElementProperty, bind);

            bind = new Binding();
            bind.Path = new PropertyPath("ElipsisPosition");
            bind.Source = this;
            bind.Mode = BindingMode.TwoWay;
            this.Element.SetBinding(NumericButton.ElipsisPositionProperty, bind);
        }

        #endregion
    }
}