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
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using Windows.System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
#else
using KeyRoutedEventArgs = System.Windows.Input.KeyEventArgs;
using VirtualKey = System.Windows.Input.Key; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class SfDiagramWrapper : IUndoable
    {
        public UndoableState State { get; set; }

        public bool CanUndo(object data)
        {
            if ((this as IUndoable).State == UndoableState.Idle)
            {
                return true;
            }
            return false;
        }

        public bool CanRedo(object data)
        {
            if ((this as IUndoable).State == UndoableState.Idle)
            {
                return true;
            }
            return false;
        }

        private object _mCompositeId = null;

        public bool CanLogData()
        {
            if ((this as IUndoable).State == UndoableState.Idle)
            {
                if (SharedData.UndoRedoController != null && SharedData.UndoRedoController.CanLogData)
                {
                    if (SharedData.UndoRedoController.State == UndoableState.Idle)
                    {
                        return true;
                    }
                    else
                        if (SharedData.UndoRedoController.State == UndoableState.CompositeLogging &&
                             SharedData.UndoRedoController.CompositeID != _mCompositeId)
                        {
                            _mCompositeId = SharedData.UndoRedoController.CompositeID;
                            return true;
                        }
                }
            }
            return false;
        }

        public bool LogData(object data)
        {
            //if ((this as IUndoable).CanLogData())
            //{
                return SharedData.UndoRedoController.LogData(this, data);
            //}
            //return false;
        }

        public object Undo(object data)
        {
            return DoJob(data);
        }

        private object DoJob(object data)
        {
            if (data is AddDelete)
            {
                AddDelete addDelete = (AddDelete)data;
                if (addDelete.IsAdd)
                {
                    switch (addDelete.ItemType)
                    {
                        case ElementType.Node:
                            SharedData.Graph.InternalNodes.Add(
                                addDelete.Item as IInternalNode, ItemSource.UnKnown);
                            break;
                        case ElementType.Connector:
                            SharedData.Graph.InternalConnectors.Add(
                                addDelete.Item as IInternalConnector, ItemSource.UnKnown);
                            break;
                        case ElementType.Group:
                            SharedData.Graph.InternalGroups.Add(
                                addDelete.Item as IInternalGroup, ItemSource.UnKnown);
                            break;
                    }
                    SharedData.SpatialSearch.UpdateQuad(addDelete.Item as IInternalGroupable);
                }
                else
                {
                    switch (addDelete.ItemType)
                    {
                        case ElementType.Node:
                            SharedData.Graph.InternalNodes.Remove(
                                addDelete.Item as IInternalNode);
                            break;
                        case ElementType.Connector:
                            SharedData.Graph.InternalConnectors.Remove(
                                addDelete.Item as IInternalConnector);
                            break;
                        case ElementType.Group:
                            SharedData.Graph.InternalGroups.Remove(
                                addDelete.Item as IInternalGroup);
                            break;
                    }
                }
                addDelete.IsAdd = !addDelete.IsAdd;
                return addDelete;
            }
            return data;
        }

        public object Redo(object data)
        {
            return DoJob(data);
        }

        //protected override void OnKeyDown(KeyRoutedEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    if (_mSharedData.UndoRedoController != null)
        //    {
        //        if (e.Key == VirtualKey.Z && CodeSharingUtilities.IsControlKeyPressed())
        //        {
        //            _mSharedData.UndoRedoController.Undo();
        //        }
        //        if (e.Key == VirtualKey.Y && CodeSharingUtilities.IsControlKeyPressed())
        //        {
        //            _mSharedData.UndoRedoController.Redo();
        //        }
        //    }
        //} 
    }
}
