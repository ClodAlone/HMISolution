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

namespace Syncfusion.Silverlight.DockingManager
{
    class OverlayDockablePane : Window
    {
        public readonly Window ReferencedPane;
        public readonly Window ReferencedContent;

        public OverlayDockablePane(DockManager dockManager, Window content, Dock initialDock)
           // : base(dockManager, initialDock)
        {
           
            ReferencedPane = content.ContainerPane as DockablePane;
            ReferencedContent = content;
            Add(ReferencedContent);
            Show(ReferencedContent);
            ReferencedContent.SetContainerPane(ReferencedPane);

            _state = PaneState.AutoHide;
        }

        public override void Show()
        {
            ChangeState(PaneState.Docked);
        }

        public override void Close()
        {
            ChangeState(PaneState.Hidden);
        }

        public override void Close(DockableContent content)
        {
            ChangeState(PaneState.Hidden);
        }
    }
}
