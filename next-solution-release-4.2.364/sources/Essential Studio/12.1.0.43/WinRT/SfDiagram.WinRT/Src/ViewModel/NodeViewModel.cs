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
using System.Runtime.Serialization;
using System.Text;
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public partial class NodeViewModel :
        GroupableViewModel,
        INode
    {
        //protected virtual void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;

        public NodeViewModel()
        {
        }

        public NodeViewModel(Rect bounds, object content)
        {
            _mOffsetX = bounds.X;
            _mOffsetY = bounds.Y;
            _mUnitWidth = bounds.Width;
            _mUnitHeight = bounds.Height;
            _mContent = content;
        }

    }
}
