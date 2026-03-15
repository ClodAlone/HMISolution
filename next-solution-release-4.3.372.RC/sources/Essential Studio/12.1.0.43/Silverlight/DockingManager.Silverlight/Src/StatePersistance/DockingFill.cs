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
using System.Collections.Generic;
using System.Linq;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Docking Manager class.
    /// </summary>
    public partial class DockingManager
    {
        /// <summary>
        /// Dockings the fill.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="ds">The ds.</param>
        /// <param name="dockPosition">The dock position.</param>
        protected internal void DockingFill(UIElement parent, DockState ds, Dock dockPosition)
        {
            double actualHeight = mouseHoveredWindow.ActualHeight;
            double actualWidth = mouseHoveredWindow.ActualWidth;
            if (_tarGetWindow.DockState == DockState.Float && base.Children.Contains(_tarGetWindow) && _tarGetWindow._Caption != string.Empty && mouseHoveredWindow.DockState == DockState.Dock)
            {
                if (_tarGetWindow.DockManager.Parent is DockingManager)
                {
                    if (_tarGetWindow.DockManager.gridDocking.GetOrderofGroup(_tarGetWindow) > 0)
                    {
                        UpdateMoveToTargetName(_tarGetWindow, false);
                    }
                }
            }
            if (_tarGetWindow.DockManager != null)
            {
                if (_tarGetWindow.DockManager.Parent is WindowContainer)
                {
                    UpdateMoveToTargetName(_tarGetWindow, true);
                }
            }
            else if (_tarGetWindow.OldValueDockManager != null)
            {
                if (_tarGetWindow.OldValueDockManager.Parent is WindowContainer)
                {
                    UpdateMoveToTargetName(_tarGetWindow, true);
                }
            }
            _tarGetWindow.UpdateZindex();
            ApplyDefaultBackground(_tarGetWindow);
            ApplyDefaultBackground(mouseHoveredWindow);
            if (mouseHoveredWindow.Parent.GetType() == typeof(Grid))
            {
                Canvas.SetZIndex(mouseHoveredWindow, 1);
                mouseHoveredWindow.IsremovedFromParent = false;
                if (_tarGetWindow.DockManager != null)
                {
                    if (_tarGetWindow.DockManager.Parent is WindowContainer)
                    {
                        if (((WindowContainer)_tarGetWindow.DockManager.Parent)._window.WindowCollection.Contains(_tarGetWindow))
                        {
                            ((WindowContainer)_tarGetWindow.DockManager.Parent)._window.WindowCollection.Remove(_tarGetWindow);
                        }
                    }
                    else if (_tarGetWindow.OldValueDockManager != null)
                    {
                        if (_tarGetWindow.OldValueDockManager.Parent is WindowContainer)
                        {
                            if (((WindowContainer)_tarGetWindow.OldValueDockManager.Parent)._window.WindowCollection.Contains(_tarGetWindow))
                            {
                                ((WindowContainer)_tarGetWindow.OldValueDockManager.Parent)._window.WindowCollection.Remove(_tarGetWindow);
                            }
                            if (_tarGetWindow.CustomTabControl != null)
                            {
                                foreach (CustomTabItem cstabitem in _tarGetWindow.CustomTabControl.Items)
                                {
                                    if (cstabitem.OwnWindow != _tarGetWindow)
                                    {
                                        if (((WindowContainer)_tarGetWindow.OldValueDockManager.Parent)._window.WindowCollection.Contains(cstabitem.OwnWindow))
                                        {
                                            ((WindowContainer)_tarGetWindow.OldValueDockManager.Parent)._window.WindowCollection.Remove(cstabitem.OwnWindow);
                                            cstabitem.OwnWindow.OldValueDockManager = null;
                                            
                                        }

                                    }
                                }
                            }
                            _tarGetWindow.OldValueDockManager = null;
                        }
                    }
                }
                
                if (parent.GetType() == typeof(WindowContainer))
                {
                    if (!((WindowContainer)parent)._window.WindowCollection.Contains(mouseHoveredWindow) && _tarGetWindow._Caption != string.Empty)
                    {
                        ((WindowContainer)parent)._window.WindowCollection.Add(mouseHoveredWindow);
                    }

                    if (!((WindowContainer)parent)._window.WindowCollection.Contains(_tarGetWindow) && _tarGetWindow._Caption != string.Empty)
                    {
                        ((WindowContainer)parent)._window.WindowCollection.Add(_tarGetWindow);
                    }

                    parent = (UIElement)((WindowContainer)parent).Children[0];
                    if (_tarGetWindow.DockManager != null)
                    {
                        //(_tarGetWindow.DockManager.Children[0] as DockingGrid).Remove(_tarGetWindow);
                        _tarGetWindow.Width = double.NaN;
                        _tarGetWindow.Height = double.NaN;
                    }

                    if (_tarGetWindow._Caption == string.Empty)
                    {
                        #region OldContent
                        //    ((_tarGetWindow.WindowContainer.Children[0] as DockManager).Children[0] as DockingGrid).gridDocking.Height = double.NaN;
                    //    _tarGetWindow.Width = double.NaN;
                    //    _tarGetWindow.Height = double.NaN;
                    //    if (_tarGetWindow.WindowCollection.Count > 0)
                    //    {
                    //        (_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearRowHeight((_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    //        (_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).ClearColumnWidth((_tarGetWindow.WindowCollection[0].DockManager.Children[0] as DockingGrid).gridDocking);
                    //    }
                    //    _tarGetWindow.WindowCollection[0].StoredMoveToWindow = mouseHoveredWindow;
                    //    _tarGetWindow.WindowCollection[0].MovetToDockPosition = dockPosition;
                    //    if (dockPosition == Dock.Left || dockPosition == Dock.Right)
                    //    {
                    //        RefreshPaneWidth();
                    //    }
                    //    else if (dockPosition == Dock.Top || dockPosition == Dock.Bottom)
                    //    {
                    //        RefreshPaneHeight();
                    //    }                        
                    //    WindowContainerFill(_tarGetWindow);
                    //    for (int i = 0; i < _tarGetWindow.WindowCollection.Count; i++)
                    //    {
                    //        if (ds == DockState.Float || ds == DockState.Hidden)
                    //        {
                    //            RemoveDock(_tarGetWindow.WindowCollection[i]);
                    //        }
                    //        else
                    //        {
                    //            ShowDockbutton(_tarGetWindow.WindowCollection[i]);
                    //        }
                    //        _tarGetWindow.WindowCollection[i].Width = double.NaN;
                    //        _tarGetWindow.WindowCollection[i].Height = double.NaN;
                    //        if (mouseHoveredWindow._Caption == string.Empty)
                    //        {
                    //            _tarGetWindow.WindowCollection[i].DockPosition = dockPosition;
                    //        }
                    //        else
                    //        {
                    //            _tarGetWindow.WindowCollection[i].DockPosition = mouseHoveredWindow.DockPosition;
                    //        }

                    //        ApplyDefaultBackground(_tarGetWindow.WindowCollection[i]);
                    //        if (_tarGetWindow.WindowCollection[i]._Caption == string.Empty)
                    //        {
                    //            if (mouseHoveredWindow._Caption == string.Empty)
                    //            {
                    //                UpdateWidthandHeight(_tarGetWindow.WindowCollection[i], dockPosition, ds);
                    //            }
                    //            else
                    //            {
                    //                UpdateWidthandHeight(_tarGetWindow.WindowCollection[i], mouseHoveredWindow.DockPosition, ds);
                    //            }
                    //        }
                    //    }

                    //    _tarGetWindow.captionBar.Visibility = Visibility.Collapsed;
                    //    _tarGetWindow.ContentGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    //    (_tarGetWindow.WindowContainer.Parent as Grid).Height = double.NaN;
                    //    (_tarGetWindow.WindowContainer.Parent as Grid).Width = double.NaN;
                    //    _tarGetWindow.WindowContainer.Height = double.NaN;
                    //    _tarGetWindow.WindowContainer.Width = double.NaN;
                        //    (_tarGetWindow.WindowContainer.Children[0] as DockManager).Margin = new Thickness(0, 0, 0, 0);
                        #endregion
                        int count = 0;
                        for (int i = 0; i < _tarGetWindow.WindowCollection.Count; i++)
                        {
                            if (_tarGetWindow.DuplicateWindowCollection.Count == 0)
                            {
                                DockManager parentDockManager = _tarGetWindow.WindowCollection[i].DockingManager.GetParentDockManager()._dockManager;//(this.DockManager.Parent as WindowContainer).IsstateTransInvoke = true;
                                DockManager windowContainer = _tarGetWindow.WindowCollection[i].DockManager;
                                if (_tarGetWindow.WindowContainer.IsstateTransInvoke)
                                {
                                    for (int j = 0; j < _tarGetWindow.WindowCollection.Count; j++)
                                    {
                                        if (_tarGetWindow.WindowCollection[i].DockState == DockState.Dock)
                                        {
                                            //you have to implement temp collection and update it
                                        }
                                    }
                                }
                                if (_tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count >= 1 && _tarGetWindow.WindowCollection[i].Visibility == Visibility.Visible && _tarGetWindow.WindowCollection[i].DockState == DockState.Float && !base.Children.Contains(_tarGetWindow.WindowCollection[i]))
                                {                                    
                                    if (count == 0)
                                    {
                                        UpDatePaneWidthForWindowContainerCollection(_tarGetWindow.WindowCollection[i], mouseHoveredWindow, dockPosition, actualWidth, actualHeight);//_tarGetWindow.WindowCollection[i].DockPosition);
                                        if (_tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count == 1)
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                                        }
                                        else
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                                        }
                                        //_tarGetWindow.WindowCollection[i].DockState = DockState.Hidden;
                                        //_tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);
                                        
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        windowContainer.gridDocking.ArrangeLayout();
                                        if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                                        {
                                            //windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
                                            if (mouseHoveredWindow.DockState == DockState.Float)
                                            {
                                                _tarGetWindow.WindowCollection[i].DockState = DockState.Float;
                                                UpdateTargetNameForMoveToFloatWindow(_tarGetWindow.WindowCollection[i], mouseHoveredWindow, dockPosition);
                                            }
                                        }
                                        else
                                        {
                                            if (mouseHoveredWindow.DockState == DockState.Dock)
                                            {
                                                if (mouseHoveredWindow.CustomTabControl != null)
                                                {
                                                    if (mouseHoveredWindow.CustomTabControl.Items.Count > 0)
                                                    {
                                                        UpdateTarGetNameForAlldockTabWindow(_tarGetWindow.WindowCollection[i],mouseHoveredWindow, dockPosition);
                                                    }
                                                }
                                                _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                                                UpdateTargetNameForMoveToDockWindow(_tarGetWindow.WindowCollection[i], mouseHoveredWindow, dockPosition);
                                            }
                                        }
                                        _tarGetWindow.WindowCollection[i].OldValueDockManager = parentDockManager;
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        _tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);
                                        _tarGetWindow.WindowCollection[i].MoveTo(mouseHoveredWindow, dockPosition);
                                        _tarGetWindow.WindowCollection[i].MovetToDockPosition = dockPosition;
                                        _tarGetWindow.WindowCollection[i].StoredMoveToWindow = mouseHoveredWindow;
                                        _tarGetWindow.WindowCollection[i].Visibility = Visibility.Visible;
                                        if (_tarGetWindow.WindowCollection[i].DockManager.Parent is WindowContainer)
                                        {
                                            if (!((WindowContainer)_tarGetWindow.WindowCollection[i].DockManager.Parent)._window.WindowCollection.Contains(_tarGetWindow.WindowCollection[i]))
                                            {
                                                ((WindowContainer)_tarGetWindow.WindowCollection[i].DockManager.Parent)._window.WindowCollection.Add(_tarGetWindow.WindowCollection[i]);
                                            }
                                        }
                                        
                                        count++;
                                    }
                                    else
                                    {
                                        int index = i - 1;
                                        UpDatePaneWidthHeightWindowContainerCollection(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                        if (_tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count == 1)
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                                        }
                                        else
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                                        }

                                        if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow != null)
                                        {
                                            if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.DockManager.Parent is WindowContainer)
                                            {
                                                //windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
                                                if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.DockState == DockState.Float)
                                                {
                                                    _tarGetWindow.WindowCollection[i].DockState = DockState.Float;
                                                    UpdateTargetNameForMoveToFloatWindow(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                                }
                                            }
                                            else
                                            {
                                                if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.DockState == DockState.Dock)
                                                {
                                                    if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.CustomTabControl != null)
                                                    {
                                                        if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.CustomTabControl.Items.Count > 0)
                                                        {
                                                            UpdateTarGetNameForAlldockTabWindow(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                                        }
                                                    }
                                                    _tarGetWindow.DockState = DockState.Dock;
                                                    UpdateTargetNameForMoveToDockWindow(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                                }
                                            }
                                        }
                                        //_tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        windowContainer.gridDocking.ArrangeLayout();
                                        _tarGetWindow.WindowCollection[i].OldValueDockManager = parentDockManager;
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        _tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);

                                        _tarGetWindow.WindowCollection[i].MoveTo(_tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                        _tarGetWindow.WindowCollection[i].MovetToDockPosition = _tarGetWindow.WindowCollection[i].MovetToDockPosition;
                                        _tarGetWindow.WindowCollection[i].StoredMoveToWindow = _tarGetWindow.WindowCollection[i].StoredMoveToWindow;
                                        _tarGetWindow.WindowCollection[i].Visibility = Visibility.Visible;
                                        if (_tarGetWindow.WindowCollection[i].DockManager.Parent is WindowContainer)
                                        {
                                            if (!((WindowContainer)_tarGetWindow.WindowCollection[i].DockManager.Parent)._window.WindowCollection.Contains(_tarGetWindow.WindowCollection[i]))
                                            {
                                                ((WindowContainer)_tarGetWindow.WindowCollection[i].DockManager.Parent)._window.WindowCollection.Add(_tarGetWindow.WindowCollection[i]);
                                            }
                                        }

                                    }
                                    _tarGetWindow.WindowCollection[i].Visibility = Visibility.Visible;
                                }
                                else if (!base.Children.Contains(_tarGetWindow.WindowCollection[i]))
                                {
                                    _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                    _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                                    //_tarGetWindow.WindowCollection[i].DockState = DockState.Hidden;
                                    //(_tarGetWindow.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                    //_tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                                    _tarGetWindow.WindowCollection[i].OldValueDockManager = parentDockManager;
                                    _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                    if (_tarGetWindow.WindowCollection[i].DockManager.Parent is WindowContainer)
                                    {
                                        if (!((WindowContainer)_tarGetWindow.WindowCollection[i].DockManager.Parent)._window.WindowCollection.Contains(_tarGetWindow.WindowCollection[i]))
                                        {
                                            ((WindowContainer)_tarGetWindow.WindowCollection[i].DockManager.Parent)._window.WindowCollection.Add(_tarGetWindow.WindowCollection[i]);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                GetDuplicateWindowCollection(_tarGetWindow, dockPosition, actualWidth, actualHeight);

                            }
                            if (!base.Children.Contains(_tarGetWindow.WindowCollection[i]))
                            {
                                if (ds == DockState.Float || ds == DockState.Hidden)
                                {
                                    RemoveDock(_tarGetWindow.WindowCollection[i]);
                                }
                                else
                                {
                                    ShowDockbutton(_tarGetWindow.WindowCollection[i]);
                                }
                            }
                            _tarGetWindow.WindowCollection[i].Width = double.NaN;
                            _tarGetWindow.WindowCollection[i].Height = double.NaN;
                            if (mouseHoveredWindow._Caption == string.Empty)
                            {
                                _tarGetWindow.WindowCollection[i].DockPosition = dockPosition;
                            }
                            else
                            {
                                _tarGetWindow.WindowCollection[i].DockPosition = mouseHoveredWindow.DockPosition;
                            }
                           
                        }

                        _tarGetWindow.Visibility = Visibility.Collapsed;
                    }
                    else if (_tarGetWindow._Caption != string.Empty)
                    {

                        DockingSingleWindowFill(dockPosition, actualWidth, actualHeight);
                    }
                }
                else
                {
                    if (_tarGetWindow._Caption != string.Empty)
                    {
                        DockingSingleWindowFill(dockPosition, actualWidth, actualHeight);
                    }
                    else
                    {
                        //_tarGetWindow = _tarGetWindow.WindowCollection[0];
                        int count = 0;
                        for (int i = 0; i < _tarGetWindow.WindowCollection.Count; i++)
                        {
                            if (mouseHoveredWindow.DockManager.Parent is DockingManager)
                            {
                                UpdateMoveToTargetNameForGroupWindow(_tarGetWindow.WindowCollection[i], false, _tarGetWindow);
                            }
                        }
                        for (int i = 0; i < _tarGetWindow.WindowCollection.Count; i++)
                        {
                            if (_tarGetWindow.DuplicateWindowCollection.Count == 0)
                            {
                                DockManager windowContainer = _tarGetWindow.WindowCollection[i].DockManager;
                                Dock sds = DockingManager.GetSideInDockedMode(_tarGetWindow.WindowCollection[i].WindowChildElement);
                                if (_tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count >= 1 && _tarGetWindow.WindowCollection[i].Visibility == Visibility.Visible && !base.Children.Contains(_tarGetWindow.WindowCollection[i]))
                                {
                                    if (count == 0)
                                    {
                                        UpDatePaneWidthForWindowContainerCollection(_tarGetWindow.WindowCollection[i], mouseHoveredWindow, dockPosition, actualWidth, actualHeight);//_tarGetWindow.WindowCollection[i].DockPosition);
                                        if (_tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count == 1)
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                                        }
                                        else
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                                        }
                                        //_tarGetWindow.WindowCollection[i].DockState = DockState.Hidden;
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        (windowContainer.Children[0] as DockingGrid).ArrangeLayout();
                                        _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                                        _tarGetWindow.WindowCollection[i].OldValueDockManager = windowContainer;
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        _tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);
                                        
                                        if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                                        {
                                            //windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
                                            if (mouseHoveredWindow.DockState == DockState.Float)
                                            {
                                                _tarGetWindow.WindowCollection[i].DockState = DockState.Float;
                                                UpdateTargetNameForMoveToFloatWindow(_tarGetWindow.WindowCollection[i], mouseHoveredWindow, dockPosition);
                                            }
                                        }
                                        else
                                        {
                                            if (mouseHoveredWindow.DockState == DockState.Dock)
                                            {
                                                if (mouseHoveredWindow.CustomTabControl != null)
                                                {
                                                    if (mouseHoveredWindow.CustomTabControl.Items.Count > 0)
                                                    {
                                                        UpdateTarGetNameForAlldockTabWindow(_tarGetWindow.WindowCollection[i],mouseHoveredWindow, dockPosition);
                                                    }
                                                }
                                                _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                                                UpdateTargetNameForMoveToDockWindow(_tarGetWindow.WindowCollection[i], mouseHoveredWindow, dockPosition);
                                                if (_tarGetWindow.WindowCollection[i].CustomTabControl != null)
                                                {
                                                    foreach (CustomTabItem csTabItem in _tarGetWindow.WindowCollection[i].CustomTabControl.Items)
                                                    {
                                                        if (csTabItem.OwnWindow != _tarGetWindow.WindowCollection[i])
                                                        {
                                                            csTabItem.OwnWindow.DockState = DockState.Dock;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        _tarGetWindow.WindowCollection[i].MoveTo(mouseHoveredWindow, dockPosition);
                                        count++;
                                    }
                                    else
                                    {
                                        int index = i - 1;
                                        UpDatePaneWidthHeightWindowContainerCollection(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                        if (_tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count == 1)
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                                        }
                                        else
                                        {
                                            _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                            _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                                        }
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        (windowContainer.Children[0] as DockingGrid).ArrangeLayout();
                                        _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                                        _tarGetWindow.WindowCollection[i].OldValueDockManager = windowContainer;
                                        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                                        _tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);


                                        if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow != null)
                                        {
                                            if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.DockManager.Parent is WindowContainer)
                                            {
                                                //windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
                                                if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.DockState == DockState.Float)
                                                {
                                                    _tarGetWindow.WindowCollection[i].DockState = DockState.Float;
                                                    UpdateTargetNameForMoveToFloatWindow(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                                }
                                            }
                                            else
                                            {
                                                if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.DockState == DockState.Dock)
                                                {
                                                    if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.CustomTabControl != null)
                                                    {
                                                        if (_tarGetWindow.WindowCollection[i].StoredMoveToWindow.CustomTabControl.Items.Count > 0)
                                                        {
                                                            UpdateTarGetNameForAlldockTabWindow(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                                        }
                                                    }
                                                    _tarGetWindow.DockState = DockState.Dock;
                                                    UpdateTargetNameForMoveToDockWindow(_tarGetWindow.WindowCollection[i], _tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
                                                }
                                            }
                                        }


                                        _tarGetWindow.WindowCollection[i].MoveTo(_tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);

                                        
                                    }
                                }
                                else if (!base.Children.Contains(_tarGetWindow.WindowCollection[i]))
                                {
                                    string targetName = DockingManager.GetTargetNameInFloatingMode(_tarGetWindow.WindowCollection[i].WindowChildElement);
                                    SetboolValueWithTargetName(_tarGetWindow.WindowCollection[i], targetName, DockState.Dock);
                                    DockingManager.SetTargetNameInDockedMode(_tarGetWindow.WindowCollection[i].WindowChildElement, targetName);
                                    SetboolValueWithSideInMode(_tarGetWindow.WindowCollection[i], Dock.Tabbed, DockState.Dock);
                                    DockingManager.SetSideInDockedMode(_tarGetWindow.WindowCollection[i].WindowChildElement, Dock.Tabbed);
                                    _tarGetWindow.WindowCollection[i].PreviousStateMain = StateMaintanance.TabWithContainer;
                                    _tarGetWindow.WindowCollection[i].CurrentStateMain = StateMaintanance.TabWithDock;
                                    //_tarGetWindow.WindowCollection[i].DockState = DockState.Hidden;
                                    //(_tarGetWindow.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                                    //_tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                                    _tarGetWindow.WindowCollection[i].OldValueDockManager = _tarGetWindow.WindowCollection[i].DockManager;
                                    _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;

                                }
                            }
                            else
                            {
                                GetDuplicateWindowCollection(_tarGetWindow, dockPosition, actualWidth, actualHeight);
                               
                            }
                            if (!base.Children.Contains(_tarGetWindow.WindowCollection[i]))
                            {
                                if (ds == DockState.Float || ds == DockState.Hidden)
                                {
                                    RemoveDock(_tarGetWindow.WindowCollection[i]);
                                }
                                else
                                {
                                    ShowDockbutton(_tarGetWindow.WindowCollection[i]);
                                }
                            }
                            _tarGetWindow.WindowCollection[i].Width = double.NaN;
                            _tarGetWindow.WindowCollection[i].Height = double.NaN;
                            if (mouseHoveredWindow._Caption == string.Empty)
                            {
                                _tarGetWindow.WindowCollection[i].DockPosition = dockPosition;
                            }
                            else
                            {
                                _tarGetWindow.WindowCollection[i].DockPosition = mouseHoveredWindow.DockPosition;
                            }
                        }

                        _tarGetWindow.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                Canvas.SetZIndex(mouseHoveredWindow, 1);
                mouseHoveredWindow.IsremovedFromParent = false;
                _tarGetWindow.DockState = DockState.Float;
                mouseHoveredWindow.DockState = DockState.Float;

                
                UpdateWindowContainer(dockPosition);
               
                
            }
            if (_tarGetWindow._Caption != string.Empty)
            {
                ActiveWindow = _tarGetWindow;
            }
        }

        /// <summary>
        /// Gets the duplicate window collection.
        /// </summary>
        /// <param name="_windows">The _windows.</param>
        /// <param name="dockPosition">The dock position.</param>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        protected internal void GetDuplicateWindowCollection(Window _windows,Dock dockPosition, double actualWidth, double actualHeight)
        {
            if (_windows.DuplicateWindowCollection.Count >= 1)
            {
                DockManager dm = _windows.DuplicateWindowCollection[0].DockManager;
                IEnumerable<Window> windowquery = _windows.WindowCollection.Where(tempwindow => ((Window)tempwindow).Visibility == Visibility.Visible && ((Window)tempwindow).DockState == DockState.Dock && ((Window)tempwindow).DockManager == dm && ((Window)tempwindow).Parent != null);
                Dictionary<int, Window> windowCollectionwithGroup = new Dictionary<int, Window>();
                for (int i = 0; i < windowquery.Count(); i++)
                {
                    int order = windowquery.ElementAt(i).DockManager.gridDocking.GetOrderofGroup(windowquery.ElementAt(i));
                    windowCollectionwithGroup.Add(order, windowquery.ElementAt(i));
                    windowCollectionwithGroup.OrderBy(d => d.Key);
                }
                List<Window> collection = windowCollectionwithGroup.Values.ToList();
                for (int i = 0; i < collection.Count; i++)
                {

                    if (i == 0)
                    {
                        UpDatePaneWidthForWindowContainerCollection(collection[i], mouseHoveredWindow, dockPosition, actualWidth, actualHeight);//_tarGetWindow.WindowCollection[i].DockPosition);
                        collection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                        collection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                        collection[i].DockState = DockState.Hidden;
                        (collection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        collection[i].DockState = DockState.Dock;
                        collection[i].OldValueDockManager = collection[i].DockManager;
                        collection[i].DockManager = mouseHoveredWindow.DockManager;
                        collection[i].DockManager.gridDocking.Remove(collection[i]);
                        collection[i].MoveTo(mouseHoveredWindow, dockPosition);
                    }
                    else
                    {
                        int index = i - 1;
                        UpDatePaneWidthHeightWindowContainerCollection(collection[i], collection[i].StoredMoveToWindow, collection[i].MovetToDockPosition);
                        collection[i].PreviousStateMain = StateMaintanance.WindowContainerToDock;
                        collection[i].CurrentStateMain = StateMaintanance.DockToWindowContainer;
                        collection[i].DockState = DockState.Hidden;
                        (collection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
                        collection[i].DockState = DockState.Dock;
                        collection[i].OldValueDockManager = collection[i].DockManager;
                        collection[i].DockManager = mouseHoveredWindow.DockManager;
                        collection[i].DockManager.gridDocking.Remove(collection[i]);
                        collection[i].MoveTo(collection[i].StoredMoveToWindow, collection[i].MovetToDockPosition);
                    }
                }
            
                
            }
        }

        /// <summary>
        /// Ups the date pane width height window container collection.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="mouseHover">The mouse hover.</param>
        /// <param name="dockPosition">The dock position.</param>
        protected internal void UpDatePaneWidthHeightWindowContainerCollection(Window target, Window mouseHover, Dock dockPosition)
        {
            if (mouseHover != null && target != null)
            {
                if (dockPosition == Dock.Top || dockPosition == Dock.Bottom)
                {
                    if (target.PaneHeight != 0.0)
                    {
                        target.PaneHeight = ((mouseHover.PaneHeight / 2.0) > target.PaneHeight) ? target.PaneHeight : mouseHover.PaneHeight / 2.0;
                        mouseHover.PaneHeight = mouseHover.PaneHeight - target.PaneHeight;
                        target.PaneWidth = mouseHover.PaneWidth;
                    }
                }
                else if (dockPosition == Dock.Left || dockPosition == Dock.Right)
                {
                    if (target.PaneWidth != 0.0)
                    {
                        target.PaneWidth = ((mouseHover.PaneWidth / 2.0) > target.PaneWidth) ? target.PaneWidth : mouseHover.PaneWidth / 2.0;
                        mouseHover.PaneWidth = mouseHover.PaneWidth - target.PaneWidth;
                        target.PaneHeight = mouseHover.PaneHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Ups the date pane width for window container collection.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="mouseHover">The mouse hover.</param>
        /// <param name="dockPosition">The dock position.</param>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        protected internal void UpDatePaneWidthForWindowContainerCollection(Window target, Window mouseHover, Dock dockPosition, double actualWidth, double actualHeight)
        {
            target.DesiredHeightInFloatMode = target.PaneHeight;
            target.DesiredWidthInFloatMode = target.PaneWidth;
            Window temp = target;
            if (target.DockManager.Parent is WindowContainer)
            {
                target = (target.DockManager.Parent as WindowContainer)._window;
            }
            if (dockPosition == Dock.Top || dockPosition == Dock.Bottom)
            {
                target.PaneWidth = actualWidth;
                target.PaneHeight = ((mouseHover.ActualHeight / 2.0) > target.ActualHeight) ? target.ActualHeight : mouseHover.ActualHeight / 2.0;
                
            }
            else if (dockPosition == Dock.Left || dockPosition == Dock.Right)
            {
                target.PaneWidth = ((mouseHover.ActualWidth / 2.0) > target.ActualWidth) ? target.ActualWidth : mouseHover.ActualWidth / 2.0;
                target.PaneHeight = actualHeight;
                
            }
            if (temp != null)
            {
                temp.PaneHeight = target.PaneHeight;
                temp.PaneWidth = target.PaneWidth;
            }

        }

        /// <summary>
        /// Refreshes the height of the pane.
        /// </summary>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        protected internal void RefreshPaneHeight(double actualWidth, double actualHeight)
        {
            if (_tarGetWindow.ActualHeight != 0.0)
            {
                _tarGetWindow.WindowCollection[0].PaneHeight = ((actualHeight / 2.0) > _tarGetWindow.ActualHeight) ? _tarGetWindow.ActualHeight : actualHeight / 2.0;
                mouseHoveredWindow.PaneHeight = actualHeight - _tarGetWindow.WindowCollection[0].PaneHeight;
            }
            else
            {
                if (_tabbedPopup != null)
                {
                    if (_tabbedPopup.IsOpen == true)
                    {
                        Rectangle rect = (Rectangle)_tabbedPopup.Child;
                        _tarGetWindow.WindowCollection[0].PaneHeight = ((actualHeight / 2.0) > rect.ActualHeight) ? rect.ActualHeight : actualHeight / 2.0;
                        mouseHoveredWindow.PaneHeight = actualHeight - _tarGetWindow.WindowCollection[0].PaneHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the width of the pane.
        /// </summary>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        protected internal void RefreshPaneWidth(double actualWidth, double actualHeight)
        {
            if (_tarGetWindow.ActualWidth != 0.0)
            {
                _tarGetWindow.WindowCollection[0].PaneWidth = ((actualWidth / 2.0) > _tarGetWindow.ActualWidth) ? _tarGetWindow.ActualWidth : actualWidth / 2.0;
                mouseHoveredWindow.PaneWidth = actualWidth - _tarGetWindow.WindowCollection[0].PaneWidth;
            }
            else
            {
                if (_tabbedPopup != null)
                {
                    if (_tabbedPopup.IsOpen == true)
                    {
                        Rectangle rect = (Rectangle)_tabbedPopup.Child;
                        _tarGetWindow.WindowCollection[0].PaneWidth = ((actualWidth / 2.0) > rect.ActualWidth) ? rect.ActualWidth : actualWidth / 2.0;
                        mouseHoveredWindow.PaneWidth = actualWidth - _tarGetWindow.WindowCollection[0].PaneWidth;
                    }
                }
            }
        }

        /// <summary>
        /// Windows the container fill.
        /// </summary>
        /// <param name="w">The w.</param>
        protected internal void WindowContainerFill(Window w)
        {
            Window windowcontainer = null;
            if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
            {
                windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
            }
           
            for (int i = 0; i < w.WindowCollection.Count; i++)
            {
                if (w.WindowCollection[i].DockManager.Parent is WindowContainer)
                {
                    w.WindowCollection[i].DockManager.gridDocking.Remove(w.WindowCollection[i]);
                }
                    w.WindowCollection[i].OldValueDockManager = w.WindowCollection[i].DockManager;
                    w.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
                    if (i >= 1 && w.WindowCollection[i].CustomTabControl != null)
                    {
                        if (w.WindowCollection[i].CustomTabControl.Items.Count >= 1)
                        {
                            if (w.WindowCollection[i].MovetToDockPosition == Dock.Bottom || w.WindowCollection[i].MovetToDockPosition == Dock.Top)
                            {
                                if (w.WindowCollection[i].PaneHeight > w.WindowCollection[i].StoredMoveToWindow.PaneHeight)
                                {
                                    w.WindowCollection[i].PaneHeight = w.WindowCollection[i].StoredMoveToWindow.PaneHeight / 2.0;
                                }
                            }
                            else if (w.WindowCollection[i].MovetToDockPosition == Dock.Left || w.WindowCollection[i].MovetToDockPosition == Dock.Right)
                            {
                                if (w.WindowCollection[i].PaneWidth > w.WindowCollection[i].StoredMoveToWindow.PaneWidth)
                                {
                                    w.WindowCollection[i].PaneWidth = w.WindowCollection[i].StoredMoveToWindow.PaneWidth / 2.0;
                                }
                            }
                        }
                    }
                    if ( w.WindowCollection[i].CustomTabControl != null)
                    {
                        if (w.WindowCollection[i].CustomTabControl.Items.Count >= 1)
                        {
                            w.WindowCollection[i].MoveTo(w.WindowCollection[i].StoredMoveToWindow, w.WindowCollection[i].MovetToDockPosition);
                        }
                    }

                    if (windowcontainer != null)
                    {
                        if (!windowcontainer.WindowCollection.Contains(w.WindowCollection[i]))
                        {
                            windowcontainer.WindowCollection.Add(w.WindowCollection[i]);                           
                        }
                    }
            }
            w.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Dockings the single window fill.
        /// </summary>
        /// <param name="dockPosition">The dock position.</param>
        /// <param name="actualWidth">The actual width.</param>
        /// <param name="actualHeight">The actual height.</param>
        protected internal void DockingSingleWindowFill(Dock dockPosition, double actualWidth, double actualHeight)
        {
            //_tarGetWindow.TargetNameCollection.Clear();
            //if (_tarGetWindow.TargetNameCollection.Contains(_tarGetWindow._Caption))
            //{
            //    _tarGetWindow.TargetNameCollection.Remove(_tarGetWindow._Caption);
            //}
            Window windowcontainer = null;
            if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
            {
                windowcontainer = (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window;
                if (mouseHoveredWindow.DockState == DockState.Float)
                {
                    _tarGetWindow.DockState = DockState.Float;
                    UpdateTargetNameForMoveToFloatWindow(_tarGetWindow, mouseHoveredWindow, dockPosition);
                }
            }
            else
            {
                if (mouseHoveredWindow.CustomTabControl != null)
                    {
                        if (mouseHoveredWindow.CustomTabControl.Items.Count > 0)
                        {
                            UpdateTarGetNameForAlldockTabWindow(_tarGetWindow, mouseHoveredWindow, dockPosition);
                        }
                        _tarGetWindow.DockState = DockState.Dock;
                        UpdateTargetNameForMoveToDockWindow(_tarGetWindow, mouseHoveredWindow, dockPosition);
                    }
                    else
                    {
                        UpdateTarGetNameForAlldockTabWindow(_tarGetWindow, mouseHoveredWindow, dockPosition);
                        _tarGetWindow.DockState = DockState.Dock;
                        UpdateTargetNameForMoveToDockWindow(_tarGetWindow, mouseHoveredWindow, dockPosition);
                    }
            }
            if (_tarGetWindow._Caption != string.Empty)
            {
                _tarGetWindow.Width = double.NaN;
                _tarGetWindow.Height = double.NaN;                 
                    if (dockPosition == Dock.Top || dockPosition == Dock.Bottom)
                    {
                        if (_tarGetWindow.ActualHeight != 0.0)
                        {
                            _tarGetWindow.PaneHeight = ((actualHeight / 2.0) > _tarGetWindow.ActualHeight) ? _tarGetWindow.ActualHeight : actualHeight / 2.0;
                            mouseHoveredWindow.PaneHeight = actualHeight - _tarGetWindow.PaneHeight;
                            _tarGetWindow.PaneWidth = mouseHoveredWindow.PaneWidth;
                        }
                        else
                        {
                            if (_tabbedPopup != null)
                            {
                                if (_tabbedPopup.IsOpen == true)
                                {
                                    Rectangle rect = (Rectangle)_tabbedPopup.Child;
                                    _tarGetWindow.PaneHeight = ((actualHeight / 2.0) > rect.ActualHeight) ? rect.ActualHeight : actualHeight / 2.0;
                                    mouseHoveredWindow.PaneHeight = actualHeight - _tarGetWindow.PaneHeight;
                                    _tarGetWindow.PaneWidth = mouseHoveredWindow.PaneWidth;
                                }
                            }
                        }
                    }
                    else if (dockPosition == Dock.Left || dockPosition == Dock.Right)
                    {
                        if (_tarGetWindow.ActualWidth != 0.0)
                        {
                            _tarGetWindow.PaneWidth = ((actualWidth / 2.0) > _tarGetWindow.ActualWidth) ? _tarGetWindow.ActualWidth : actualWidth / 2.0;
                            mouseHoveredWindow.PaneWidth = actualWidth - _tarGetWindow.PaneWidth;
                            _tarGetWindow.PaneHeight = mouseHoveredWindow.PaneHeight;
                        }
                        else
                        {
                            if (_tabbedPopup != null)
                            {
                                if (_tabbedPopup.IsOpen == true)
                                {
                                    Rectangle rect = (Rectangle)_tabbedPopup.Child;
                                    _tarGetWindow.PaneWidth = ((actualWidth / 2.0) > rect.ActualWidth) ? rect.ActualWidth : actualWidth / 2.0;
                                    mouseHoveredWindow.PaneWidth = actualWidth - _tarGetWindow.PaneWidth;
                                    _tarGetWindow.PaneHeight = mouseHoveredWindow.PaneHeight;
                                }
                            }
                        }
                    }
                 

                //_tarGetWindow.PaneWidth = mouseHoveredWindow.PaneWidth;
                //_tarGetWindow.DockState = DockState.Float;
                _tarGetWindow.ApplyDockStyle();
                _tarGetWindow.MovetToDockPosition = dockPosition;
                _tarGetWindow.StoredMoveToWindow = mouseHoveredWindow;
                mouseHoveredWindow.ApplyDockStyle();
                if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                {
                    DockManager swap = _tarGetWindow.DockManager;
                    _tarGetWindow.DockManager = mouseHoveredWindow.DockManager;
                    _tarGetWindow.OldValueDockManager = swap;
                    if (_tarGetWindow.CustomTabControl.Items.Count > 1)
                    {
                        foreach (CustomTabItem csTabItem in _tarGetWindow.CustomTabControl.Items)
                        {                            
                            if(csTabItem.OwnWindow!= _tarGetWindow)
                            {
                                csTabItem.OwnWindow.DockState = DockState.Float;
                                csTabItem.OwnWindow.Visibility = Visibility.Collapsed;
                                csTabItem.OwnWindow.DockManager = mouseHoveredWindow.DockManager;
                                csTabItem.OwnWindow.OldValueDockManager = swap;
                                if (!(mouseHoveredWindow.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(csTabItem.OwnWindow) && csTabItem.OwnWindow._Caption != string.Empty)
                                {
                                    (mouseHoveredWindow.DockManager.Parent as WindowContainer)._window.WindowCollection.Add(csTabItem.OwnWindow);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _tarGetWindow.DockManager = mouseHoveredWindow.DockManager;
                }
                //_tarGetWindow.PaneHeight = (actualHeight / 2.0 > _tarGetWindow.PaneHeight) ? _tarGetWindow.PaneHeight : (actualHeight / 2.0) - 20;                                        
                _tarGetWindow.MoveTo(mouseHoveredWindow, dockPosition);
                if (mouseHoveredWindow._Caption == string.Empty)
                {
                    _tarGetWindow.DockPosition = dockPosition;
                }              
            }
            if (windowcontainer != null)
            {
                if (windowcontainer.Visibility == Visibility.Visible)
                {
                    RemoveDock(_tarGetWindow);
                }
                else
                {
                    ShowDockbutton(_tarGetWindow);
                }
            }
            else
            {
                    ShowDockbutton(_tarGetWindow);
            }



            //if (_tarGetWindow.DuplicateWindowCollection.Contains(_tarGetWindow.WindowCollection[i]))
            //{
            //    if (!duplicate)
            //    {
            //        _tarGetWindow.WindowCollection[i].DockState = DockState.Hidden;
            //        (_tarGetWindow.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
            //        _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
            //        _tarGetWindow.WindowCollection[i].OldValueDockManager = _tarGetWindow.WindowCollection[i].DockManager;
            //        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
            //        _tarGetWindow.WindowCollection[i].MoveTo(mouseHoveredWindow, dockPosition);
            //        duplicate = true;
            //    }
            //    else
            //    {
            //        int index = i - 1;
            //        _tarGetWindow.WindowCollection[i].DockState = DockState.Hidden;
            //        (_tarGetWindow.WindowCollection[i].DockManager.Children[0] as DockingGrid).ArrangeLayout();
            //        _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
            //        _tarGetWindow.WindowCollection[i].OldValueDockManager = _tarGetWindow.WindowCollection[i].DockManager;
            //        _tarGetWindow.WindowCollection[i].DockManager = mouseHoveredWindow.DockManager;
            //        _tarGetWindow.WindowCollection[i].MoveTo(_tarGetWindow.WindowCollection[i].StoredMoveToWindow, _tarGetWindow.WindowCollection[i].MovetToDockPosition);
            //    }
            //}


        }

        /// <summary>
        /// Updates the tar get name for center drag.
        /// </summary>
        /// <param name="mouseHoverWindow">The mouse hover window.</param>
        /// <param name="targetWindow">The target window.</param>
        protected internal void UpdateTarGetNameForCenterDrag(Window mouseHoverWindow,Window targetWindow)
        {
            List<Window> windowcoll = new List<Window>();
            List<Window> windowCollection = new List<Window>(WindowCollection.Values);
            IEnumerable<Window> query1 = windowCollection.Where(tempwindow => DockingManager.GetTargetNameInFloatingMode(((Window)tempwindow).WindowChildElement) == targetWindow._Caption && DockingManager.GetSideInFloatMode(((Window)tempwindow).WindowChildElement) == Dock.Tabbed);
            foreach (CustomTabItem csTabItem in targetWindow.CustomTabControl.Items)
            {
                Window _w = csTabItem.OwnWindow;
                SetboolValueWithTargetName(_w, mouseHoverWindow._Caption, DockState.Dock);
                DockingManager.SetTargetNameInDockedMode(_w.WindowChildElement, mouseHoverWindow._Caption);
                SetboolValueWithSideInMode(_w, Dock.Tabbed, DockState.Dock);
                DockingManager.SetSideInDockedMode(_w.WindowChildElement, Dock.Tabbed);
            }

            SetboolValueWithTargetName(targetWindow, mouseHoverWindow._Caption, DockState.Dock);
            DockingManager.SetTargetNameInDockedMode(targetWindow.WindowChildElement, mouseHoverWindow._Caption);
            //w.InternalllyRaisedDockStateChanged = true;
            SetboolValueWithSideInMode(targetWindow, Dock.Tabbed, DockState.Dock);
            DockingManager.SetSideInDockedMode(targetWindow.WindowChildElement, Dock.Tabbed);
 
        }

        /// <summary>
        /// Hostings the element by center drag provider.
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <param name="_pos">The _pos.</param>
        protected internal void HostingElementByCenterDragProvider(DockState ds, Point _pos)
        {
            if (_tarGetWindow.DockState == DockState.Float && base.Children.Contains(_tarGetWindow) && _tarGetWindow._Caption != string.Empty && mouseHoveredWindow.DockState == DockState.Dock)
            {
                if (_tarGetWindow.DockManager.Parent is DockingManager)
                {
                    if (_tarGetWindow.DockManager.gridDocking.GetOrderofGroup(_tarGetWindow) > 0)
                    {
                        UpdateMoveToTargetName(_tarGetWindow, false);
                    }
                }
            }
            if (_tarGetWindow.DockManager != null)
            {
                if (_tarGetWindow.DockManager.Parent is WindowContainer)
                {
                    UpdateMoveToTargetName(_tarGetWindow, true);
                }
            }
            else if (_tarGetWindow.OldValueDockManager != null)
            {
                if (_tarGetWindow.OldValueDockManager.Parent is WindowContainer)
                {
                    UpdateMoveToTargetName(_tarGetWindow, true);
                }
            }
            if (mouseHoveredWindow.Caption != "Document" && mouseHoveredWindow._Caption != string.Empty && _tarGetWindow._Caption != string.Empty)
            {
                List<string> targetNameColection = _tarGetWindow.TargetNameCollection;
                List<CustomTabItem> tabCollection = new List<CustomTabItem>();
                bool isMorethanOneWindow = false;
                if (_tarGetWindow.CustomTabControl.Items.Count > 1)
                {
                    isMorethanOneWindow = true;
                    for (int i = 0; i < _tarGetWindow.CustomTabControl.Items.Count; i++)
                    {
                        CustomTabItem cstabItem = _tarGetWindow.CustomTabControl.Items[i] as CustomTabItem;
                        if (!tabCollection.Contains(cstabItem))
                        {
                            tabCollection.Add(cstabItem);
                        }
                    }
                }
                _tarGetWindow.DockState = mouseHoveredWindow.DockState;
                if (ds == DockState.Float || ds == DockState.Hidden)
                {
                    RemoveDock(_tarGetWindow);
                    RemoveDock(mouseHoveredWindow);
                }
                else
                {
                    ShowDockbutton(_tarGetWindow);
                    ShowDockbutton(mouseHoveredWindow);
                }
                if (_tabbedPopup != null)
                {
                    if (_tabbedPopup.IsOpen == true)
                    {
                        Rectangle rect = (Rectangle)_tabbedPopup.Child;
                        _tarGetWindow.FloatHeight = rect.Height;
                        _tarGetWindow.FloatWidth = rect.Width;
                        _tarGetWindow.LeftPosition = _pos.X;
                        _tarGetWindow.TopPosition = _pos.Y;
                    }
                }
                _tarGetWindow.UpdateZindex();
                if (_tarGetWindow.DockManager == mouseHoveredWindow.DockManager)
                {
                    if (_tarGetWindow.DockManager.Parent is WindowContainer)
                    {
                        if ((_tarGetWindow.DockManager.Parent as WindowContainer)._window.WindowCollection.Contains(_tarGetWindow))
                        {
                            (_tarGetWindow.DockManager.Parent as WindowContainer)._window.WindowCollection.Remove(_tarGetWindow);
                        }
                    }
                    if (mouseHoveredWindow.DockManager.Parent is DockingManager && !base.Children.Contains(mouseHoveredWindow) && mouseHoveredWindow.DockState == DockState.Dock)
                    {
                        (_tarGetWindow.DockManager.Children[0] as DockingGrid).Remove(_tarGetWindow);
                    }
                }
                HostWindowAsTab(mouseHoveredWindow, _tarGetWindow);
                if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                {
                    UpdateTargetNameForFloatWindow(_tarGetWindow, mouseHoveredWindow);
                }
                else
                {
                    if (mouseHoveredWindow.DockState == DockState.Float && base.Children.Contains(mouseHoveredWindow) && mouseHoveredWindow.Visibility == Visibility.Visible)
                    {
                        UpdateTargetNameForFloatWindow(_tarGetWindow, mouseHoveredWindow);
                    }
                    else
                    {
                       // UpdateTargetName(_tarGetWindow, mouseHoveredWindow);
                        UpdateTarGetNameForCenterDrag(mouseHoveredWindow, _tarGetWindow);
                    }
                }
                _tarGetWindow.PaneWidth = mouseHoveredWindow.PaneWidth;
                _tarGetWindow.PaneHeight = mouseHoveredWindow.PaneHeight;
                _tarGetWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
                mouseHoveredWindow.CustomTabControl.TabPanelBackground = TabPanelBackground;
                Canvas.SetZIndex(_tarGetWindow, 1);
                if (mouseHoveredWindow.CustomTabControl.primitiveTabPanel != null)
                {
                    mouseHoveredWindow.CustomTabControl.primitiveTabPanel.Background = TabPanelBackground;
                }

                if (_tarGetWindow.CustomTabControl.primitiveTabPanel != null)
                {
                    _tarGetWindow.CustomTabControl.primitiveTabPanel.Background = TabPanelBackground;
                }
                _tarGetWindow.StoredMoveToWindow = mouseHoveredWindow.StoredMoveToWindow;
                _tarGetWindow.MovetToDockPosition = mouseHoveredWindow.MovetToDockPosition;
                if (_tarGetWindow.DockManager != mouseHoveredWindow.DockManager)
                {
                    DockManager oldValue = _tarGetWindow.DockManager;
                    _tarGetWindow.DockManager = mouseHoveredWindow.DockManager;
                    _tarGetWindow.OldValueDockManager = oldValue;
                }
                if (base.Children.Contains(mouseHoveredWindow) && mouseHoveredWindow.Visibility == Visibility.Visible)
                {
                    mouseHoveredWindow.ApplyBorderForFloatWindow();
                }
                else if (mouseHoveredWindow.DockState == DockState.Dock)
                {
                    mouseHoveredWindow.ApplyDockStyle();
                }
                
                if (mouseHoveredWindow.CustomTabControl.SelectedItem != null)
                {
                    mouseHoveredWindow.Caption = ((CustomTabItem)mouseHoveredWindow.CustomTabControl.SelectedItem).Header.ToString();
                }

                if (mouseHoveredWindow.DockManager != null)
                {
                    if (mouseHoveredWindow.DockManager.Parent is WindowContainer)
                    {
                        StateMaintanance st = _tarGetWindow.CurrentStateMain;
                        if (_tarGetWindow.CurrentStateMain == StateMaintanance.TabWithDock || _tarGetWindow.CurrentStateMain == StateMaintanance.Dock)
                        {
                            _tarGetWindow.PreviousStateMain = st;
                        }
                        //if (_tarGetWindow.CurrentStateMain != StateMaintanance.Float)//_tarGetWindow.CurrentStateMain != StateMaintanance.Float || 
                        //{
                        //    _tarGetWindow.PreviousStateMain = st;

                        //}
                        _tarGetWindow.CurrentStateMain = StateMaintanance.TabWithContainer;
                        mouseHoveredWindow.CurrentStateMain = StateMaintanance.TabWithContainer;
                        _tarGetWindow.TargetNameCollection = targetNameColection;
                        //mouseHoveredWindow.TargetNameCollection = _tarGetWindow.TargetNameCollection;
                    }
                    else if (base.Children.Contains(mouseHoveredWindow) && _tarGetWindow.DockState == DockState.Float)
                    {
                        StateMaintanance st = _tarGetWindow.CurrentStateMain;
                        //if (_tarGetWindow.CurrentStateMain != StateMaintanance.Float && _tarGetWindow.DockState == DockState.Float)//_tarGetWindow.CurrentStateMain != StateMaintanance.Float || 
                        //{
                        //    _tarGetWindow.PreviousStateMain = st;
                        //} 

                        if (_tarGetWindow.CurrentStateMain == StateMaintanance.TabWithDock || _tarGetWindow.CurrentStateMain == StateMaintanance.Dock)
                        {
                            _tarGetWindow.PreviousStateMain = st;
                        }
                        _tarGetWindow.CurrentStateMain = StateMaintanance.TabWithFloat;
                        mouseHoveredWindow.CurrentStateMain = StateMaintanance.TabWithFloat;

                        _tarGetWindow.TargetNameCollection = targetNameColection;
                        //mouseHoveredWindow.TargetNameCollection = targetNameColection;
                    }
                    else //if (_tarGetWindow.DockState == DockState.Dock)
                    {
                        //StateMaintanance st = _tarGetWindow.CurrentStateMain;
                        if (!isMorethanOneWindow)
                        {
                            _tarGetWindow.CurrentStateMain = StateMaintanance.TabWithDock;
                            _tarGetWindow.PreviousStateMain = StateMaintanance.Float;
                            if (mouseHoveredWindow._Caption != string.Empty)
                            {
                                mouseHoveredWindow.CurrentStateMain = StateMaintanance.TabWithDock;
                            }
                            mouseHoveredWindow.TargetNameCollection.Add(_tarGetWindow._Caption);
                            _tarGetWindow.TargetNameCollection = mouseHoveredWindow.TargetNameCollection;
                            _tarGetWindow.DockingManager.SetboolValueWithTargetName(_tarGetWindow, mouseHoveredWindow._Caption, DockState.Dock);
                            DockingManager.SetTargetNameInDockedMode(_tarGetWindow.WindowChildElement, mouseHoveredWindow._Caption);
                        }
                        else if (isMorethanOneWindow)
                        {
                            foreach (CustomTabItem cstab in tabCollection)
                            {
                                if (mouseHoveredWindow._Caption != string.Empty)
                                {
                                    mouseHoveredWindow.CurrentStateMain = StateMaintanance.TabWithDock;
                                }
                                if (cstab.OwnWindow != null)
                                {
                                    cstab.OwnWindow.CurrentStateMain = StateMaintanance.TabWithDock;
                                    cstab.OwnWindow.PreviousStateMain = StateMaintanance.TabWithFloat;
                                }
                                //mouseHoveredWindow.TargetNameCollection.Add(_tarGetWindow._Caption);
                                //_tarGetWindow.TargetNameCollection = mouseHoveredWindow.TargetNameCollection;
                                cstab.OwnWindow.DockingManager.SetboolValueWithTargetName(cstab.OwnWindow, mouseHoveredWindow._Caption, DockState.Dock);
                                DockingManager.SetTargetNameInDockedMode(cstab.OwnWindow.WindowChildElement, mouseHoveredWindow._Caption);
                            }
                        }
                        _tarGetWindow.DockManager.gridDocking.Remove(_tarGetWindow);
                        _tarGetWindow.DockingManager.SetboolValueWithTargetName(_tarGetWindow, mouseHoveredWindow._Caption, DockState.Dock);
                        DockingManager.SetTargetNameInDockedMode(_tarGetWindow.WindowChildElement, mouseHoveredWindow._Caption);
                        _tarGetWindow.DockingManager.SetboolValueWithSideInMode(_tarGetWindow, Dock.Tabbed, DockState.Dock);
                        DockingManager.SetSideInDockedMode(_tarGetWindow.WindowChildElement, Dock.Tabbed);
                    }
                }
                _tarGetWindow.DockManager = mouseHoveredWindow.DockManager;
                _tarGetWindow.OldValueDockManager = mouseHoveredWindow.OldValueDockManager;
            }
            else if (mouseHoveredWindow.Caption != "Document" && mouseHoveredWindow._Caption != string.Empty && _tarGetWindow._Caption == string.Empty)
            {
                DockingGrid dockingGrid = this.GetParentDockManager();
                DockManager dm = null;
                if (dockingGrid != null)
                {
                    dm = dockingGrid._dockManager;
                }
                _tarGetWindow.UpdateZindex();
                for (int i = 0; i < _tarGetWindow.WindowCollection.Count; i++)
                {
                    if (mouseHoveredWindow.CustomTabControl != null)
                    {
                        if (_tarGetWindow.WindowCollection[i]._Caption == string.Empty)
                        {
                            AddCustomTabItem(_tarGetWindow.WindowCollection[i], mouseHoveredWindow);
                            if (ds == DockState.Float)
                            {
                                UpdateTargetNameForFloatWindow(_tarGetWindow.WindowCollection[i], mouseHoveredWindow);
                            }
                            else
                            {
                                //UpdateTargetName(_tarGetWindow.WindowCollection[i], mouseHoveredWindow);
                                UpdateTarGetNameForCenterDrag(mouseHoveredWindow, _tarGetWindow.WindowCollection[i]);
                            }
                        }
                        if (_tarGetWindow.WindowCollection[i].CustomTabControl != null)
                        {
                            for (int m = _tarGetWindow.WindowCollection[i].CustomTabControl.Items.Count - 1; m >= 0; m--)
                            {
                                CustomTabItem cs = (CustomTabItem)_tarGetWindow.WindowCollection[i].CustomTabControl.Items[m];
                                _tarGetWindow.WindowCollection[i].CustomTabControl.Items.Remove(cs);
                                mouseHoveredWindow.CustomTabControl.Items.Add(cs);
                                if (ds == DockState.Float)
                                {
                                    cs.OwnWindow.OldValueDockManager = null;
                                    if (cs.OwnWindow != _tarGetWindow.WindowCollection[i] && dm != null)
                                    {
                                        cs.OwnWindow.DockManager = dm;
                                    }
                                    cs.OwnWindow.DockState = DockState.Float;
                                }
                                else
                                {
                                    cs.OwnWindow.DockState = DockState.Dock;
                                }
                            }
                            if (ds == DockState.Float)
                            {
                                UpdateTargetNameForFloatWindow(_tarGetWindow.WindowCollection[i], mouseHoveredWindow);
                                //_tarGetWindow.WindowCollection[i].DockManager
                                _tarGetWindow.WindowCollection[i].OldValueDockManager = null;
                            }
                            else
                            {
                                DockManager swap = _tarGetWindow.WindowCollection[i].DockManager;
                                _tarGetWindow.WindowCollection[i].OldValueDockManager = swap;
                                //UpdateTargetName(_tarGetWindow.WindowCollection[i], mouseHoveredWindow);
                                UpdateTarGetNameForCenterDrag(mouseHoveredWindow, _tarGetWindow.WindowCollection[i]);
                                _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                            }
                        }

                        if (ds == DockState.Float && dm != null)
                        {
                            _tarGetWindow.WindowCollection[i].DockManager.gridDocking.Remove(_tarGetWindow.WindowCollection[i]);
                            _tarGetWindow.WindowCollection[i].DockManager = dm;
                            _tarGetWindow.WindowCollection[i].DockState = DockState.Float;
                        }
                        else
                        {
                            _tarGetWindow.WindowCollection[i].DockManager = dm;
                            _tarGetWindow.WindowCollection[i].DockState = DockState.Dock;
                        }
                        if (i == _tarGetWindow.WindowCollection.Count - 1)
                        {
                            _tarGetWindow.Visibility = Visibility.Collapsed;
                        }
                    }                    
                }
                if (ds != DockState.Float)
                {
                    _tarGetWindow.WindowContainer.DockManager.gridDocking.ArrangeLayout();
                }
                if (mouseHoveredWindow.CustomTabControl.Items.Count > 1)
                {
                    if (mouseHoveredWindow.CustomTabControl.primitiveTabPanel != null)
                    {
                        mouseHoveredWindow.CustomTabControl.primitiveTabPanel.Visibility = Visibility.Visible;
                        mouseHoveredWindow.CustomTabControl.TabPanelBorder.Visibility = Visibility.Visible;
                    }
                    if (mouseHoveredWindow.CustomTabControl.SelectedItem != null)
                    {
                        mouseHoveredWindow.Caption = ((CustomTabItem)mouseHoveredWindow.CustomTabControl.SelectedItem).Header.ToString();
                    }
                }
                HideTabPanel(mouseHoveredWindow);
            }

            if (mouseHoveredWindow != null && _tarGetWindow != null)
            {
                if (mouseHoveredWindow.CustomTabControl.SelectedItem != null)
                {
                    CustomTabItem cstabItem = mouseHoveredWindow.CustomTabControl.SelectedItem as CustomTabItem;
                    mouseHoveredWindow.NoHeaderVisibility(DockingManager.GetNoHeader(cstabItem.OwnWindow.WindowChildElement), DockingManager.GetHeaderHeight(cstabItem.OwnWindow.WindowChildElement));
                }
            }
            if (_tarGetWindow._Caption != string.Empty)
            {
                ActiveWindow = _tarGetWindow;
                Window newWindow = null;
                if (ActiveWindow == _tarGetWindow)
                {
                    CustomTabControl cstab = null;
                    if ((FrameworkElement)ActiveWindow.WindowChildElement != null && ((FrameworkElement)ActiveWindow.WindowChildElement).Parent != null && ((FrameworkElement)ActiveWindow.WindowChildElement).Parent.GetType() == typeof(CustomTabItem))
                    {
                        cstab = ((CustomTabItem)((FrameworkElement)ActiveWindow.WindowChildElement).Parent).Parent as CustomTabControl;

                        if (cstab != null && cstab.Items.Count > 1)
                        {
                            newWindow = GetWindow(cstab);
                        }
                        else
                        {
                            newWindow = ActiveWindow;
                        }
                    }
                    ActiveWindow = newWindow;
                    if (newWindow != null && cstab != null)
                    {
                        newWindow.ActiveForeground = ActiveForeground;
                        if (newWindow.maximizeButton != null)
                            VisualStateManager.GoToState(newWindow.maximizeButton, "Active", false);
                        if (newWindow.closeButton != null)
                            VisualStateManager.GoToState(newWindow.closeButton, "Active", false);
                        if (newWindow.dockToggle != null)
                            VisualStateManager.GoToState(newWindow.dockToggle, "Active", false);
                        if (newWindow.optionsButton != null)
                            VisualStateManager.GoToState(newWindow.optionsButton, "Active", false);

                        if (newWindow.DockState == DockState.Float || newWindow.DockState == DockState.Hidden)
                        {
                            newWindow.HeaderBackgroud = FloatWindowActiveHeaderBackground;                             
                        }
                        else
                        {
                            newWindow.HeaderBackgroud = ActiveWindowColor;
                        }
                    }
                }
                if (ActiveWindow != null && newWindow != ActiveWindow)
                {
                    newWindow.CaptionForeGround = CaptionForeGround;
                    if (newWindow.maximizeButton != null)
                        VisualStateManager.GoToState(newWindow.maximizeButton, "InActive", false);
                    if (newWindow.closeButton != null)
                        VisualStateManager.GoToState(newWindow.closeButton, "InActive", false);
                    if (newWindow.dockToggle != null)
                        VisualStateManager.GoToState(newWindow.dockToggle, "InActive", false);
                    if (newWindow.optionsButton != null)
                        VisualStateManager.GoToState(newWindow.optionsButton, "InActive", false);
                    if (ActiveWindow.DockState == DockState.Float || ActiveWindow.DockState == DockState.Hidden)
                    {
                        ActiveWindow.HeaderBackgroud = FloatWindowHeaderBackground; 
                    }
                    else
                    {
                        ActiveWindow.HeaderBackgroud = HeaderBackground; 
                    }
                }
            }
        }
    }
}
