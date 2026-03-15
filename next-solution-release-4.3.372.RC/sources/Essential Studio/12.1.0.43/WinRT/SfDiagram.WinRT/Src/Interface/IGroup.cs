#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IGroup : INode
    {
        object Nodes { get; set; }

        object Connectors { get; set; }

        object Groups { get; set; }
    }

    public interface IGroupInfo : 
        INodeInfo
    {
        bool IsDescendentSelected { get; }
        bool IsParentSelected { get; }
    }

    internal interface IInternalGroup :
        IGroup,
        IInternalNode,
        //IWrapper<TID, TNode, TConnector, TGroup, TGroup, IView>,
        IGroupInfo
    {
        void AddRemoveItem(IInternalGroupable item, bool add);

        ObservableElements<object, IInternalNode>
            InternalNodes { get; set; }

        ObservableElements<object, IInternalConnector> 
            InternalConnectors { get; set; }

        ObservableElements<object, IInternalGroup>
            InternalGroups { get; set; }

        void CheckDescendentSelected();
        void DescendentSelected();
    }

    internal interface IProtectedGroup : IInternalGroup
    {
        Size UpdateBounds();
    }
}
