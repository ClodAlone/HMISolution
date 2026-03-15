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
#if WINRT_USING
using System.Threading.Tasks; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal interface IView : IDiagramElement
    {
        void SetBusinessObject(object item);
    }

    internal interface INodeView : IView
    {
    }
    internal interface IConnectorView : IView
    {
    }
    internal interface IGroupView : INodeView
    {
    }

    internal interface ISelectorView : IGroupView
    {
    }

    //internal interface INodeGroupView : IView
    //{
    //}

    //internal interface IView : IView, IDiagramElement
    //{
    //}

    //internal interface INodeGroupView : INodeGroupView, IView
    //{
    //}

    //internal interface INodeView : INodeView, INodeGroupView
    //{
    //}
    //internal interface IConnectorView : IConnectorView, IView
    //{
    //}
    //internal interface IGroupView : IGroupView, INodeGroupView
    //{
    //}
    //internal interface ISelectorView : ISelectorView, IGroupView
    //{
    //}
}
