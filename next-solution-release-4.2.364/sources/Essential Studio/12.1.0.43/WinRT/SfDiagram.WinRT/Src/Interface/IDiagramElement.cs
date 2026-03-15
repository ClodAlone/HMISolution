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
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IDiagramElement: IID, INotifyPropertyChanged
    {
        object Key { get; set; }
    }

    public interface IInternalDiagramElement : IID, INotifyPropertyChanged
    {
        object InternalID { get; set; }
    }
}
