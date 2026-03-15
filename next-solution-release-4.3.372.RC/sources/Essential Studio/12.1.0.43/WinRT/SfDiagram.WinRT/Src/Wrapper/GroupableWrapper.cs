#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Reflection;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.ComponentModel;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections;
#else
using System.Windows.Controls;
using VirtualKey = System.Windows.Input.Key; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal abstract partial class GroupableWrapper : 
        DiagramElementWrapper,
        IInternalGroupable, IUndoable
    {
        public object Info
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (Source is IGroupable)
                {
                    (Source as IGroupable).Info = value;
                }
            }
        }

        public object InternalID { get; set; }

        public abstract IView View { get; set; }
        GroupableState CurrentState = new GroupableState();
        RectCorners? _mCorners = null;
        public RectCorners? Corners
        {
            get
            {
                return _mCorners;
            }
            set
            {
                //if(_mCorners != value)
                {
                    _mCorners = value;
                }
            }
        }

        //public IGroupable KnownSource
        //{
        //    get
        //    {
        //        return _mKnownSource;
        //    }
        //    private set
        //    {
        //        if (_mKnownSource != null)
        //        {
        //            _mKnownSource.PropertyChanged -= KnownSource_PropertyChanged;
        //            _mKnownSource = null;
        //        }
        //        _mKnownSource = value;
        //        if (_mKnownSource is UIElement)
        //        {
        //            CanVirtualize = false;
        //        }
        //        else
        //        {
        //            CanVirtualize = true;
        //        }
        //        _misKnownType = true;
        //        SourceChanged();
        //        _mKnownSource.PropertyChanged += KnownSource_PropertyChanged;
        //    }
        //}
        
        protected override void SharedDataInitialized()
        {
            
        }

        public override void Dispose()
        {
            IsSelected = false;
            //Source = default(IGroupable);
        }
        
        //private readonly BoundsChangedEvent _mBoundsChanged;
        private readonly SelectedEvent<IInternalGroupable> _mSelected;
        private readonly UnSelectedEvent<IInternalGroupable> _mUnSelected;

        protected GroupableWrapper(SharedData shared) : base(shared)
        {
            //if ((this is ISelector))
            //{
            //    State = UndoableState.Disabled;
            //}
            //_mBoundsChanged = SharedData.EventAggregator.GetEvent<BoundsChangedEvent>();
            _mSelected = SharedData.Selected;
            _mUnSelected = SharedData.UnSelected;
        }

        public void Init(object source)
        {
            Source = source;
            if (!(source is IGroup) && ZIndex==0)
            {
                ZIndex = SharedData.AutoIncrement;
                SharedData.AutoIncrement++;
            }
           
           //PrivateData = this;
            Graph = SharedData.Graph;
            if (IsSelected)
            {
                OnIsSelectedChanged();
            }
        }

        public IGraph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                OnGraphChanged();
            }
        }

        protected virtual void OnGraphChanged()
        {
            if (Annotations != null)
            {
                OnAnnotationsChanged();
            }
        }

        protected override void SourceChanged()
        {
            Info = this;
        }

        //public void Init(IInternalGroupable source)
        //{
        //    KnownSource = source;
        //    Graph = SharedData.Graph;
        //    if (IsSelected)
        //    {
        //        OnPropertyChanged(GroupableConstants.IsSelected);
        //    }
        //}        

        protected void SetBounds(Rect rect)
        {
            Rect old = Bounds;
            if (old != rect)
            {
                IInternalConnector con = this as IInternalConnector;
                if (con != null && con.CanBridge() && !(con as ConnectorWrapper).CanRoute())
                {
                    con.UpdateParentBridging(old);
                    con.UpdateParentBridging(rect);
                }
            }
            //_mBounds = rect;
            Bounds = rect;
            //if (old != rect)
            //{
            //    //OnPropertyChanged("Bounds");
            //    if (CanVirtualize ||
            //        VirtualizationState == VirtualizationState.Virtualized ||
            //        VirtualizationState == VirtualizationState.Virtualizing)
            //    {
            //        if (_mBoundsChanged != null &&
            //            _mBoundsChanged.HasSubscripitons &&
            //            !_mBoundsChanged.IsSuspended)
            //        {
            //            ChangeArgs<Rect> args = new ChangeArgs<Rect>(this, old, _mBounds);
            //            _mBoundsChanged.Publish(args);
            //        }
            //    }
            //}
        }

        public bool UserInteracting { get; set; }

        public virtual void Tap(bool fireEvent)
        {
            if (CanSelect())
            {
                UserInteracting = true;
                Select();
                UserInteracting = false;
            }
        }

        public virtual void DoubleTap(bool fireEvent)
        {
            if (InternalAnnotations != null && InternalAnnotations.Any())
            {
                IAnnotation annotation = InternalAnnotations.FirstOrDefault().Source as IAnnotation;
                if (annotation != null)
                {
                    annotation.Mode = ContentEditorMode.Edit;
                }
            }
        }

        public void PointerDragStarted()
        {
            if (CanSelect())
            {
                UserInteracting = true;
                if (!IsSelected && (KnownParentGroup == null || !KnownParentGroup.IsParentSelected))
                {
                    Select();
                }
                UserInteracting = false;
            }
        }

        public void RubberbandSelect()
        {
            if (CanSelect())
            {
                Select();
            }
        }

        protected virtual void Select()
        {
            if (this is ISelector)
            {
                return;
            }
            if (KnownParentGroup == null)
            {
                IsSelected = !IsSelected;
            }
            else if (KnownParentGroup.IsDescendentSelected)
            {
                IsSelected = !IsSelected;
            }
            else if (KnownParentGroup.IsSelected)
            {
                KnownParentGroup.IsSelected = false;
                IsSelected = true;
            }
            else
            {
                IsSelected = false;
                if (UserInteracting)
                {
                    KnownParentGroup.Tap(false);
                }
                else
                    KnownParentGroup.IsSelected = true;
            }
        }

        protected virtual void OnIsSelectedChanged()
        {
            bool clear = false;
            if (UserInteracting)
            {
                clear = !CanMultipleSelect();
            }
            SelectionArgs<IInternalGroupable> args =
                new SelectionArgs<IInternalGroupable>(
                    this, clear);
            if (IsSelected)
            {
                if (KnownParentGroup != null)
                {
                    KnownParentGroup.DescendentSelected();
                }
                _mSelected.Publish(args);
            }
            else
            {
                if (KnownParentGroup != null)
                {
                    KnownParentGroup.CheckDescendentSelected();
                }
                _mUnSelected.Publish(args);
            }
        }

        protected virtual void OnBoundsChanged()
        {
            if (SharedData != null)
            {
                if (!(this is ISelector))
                {
                    SharedData.SpatialSearch.UpdateQuad(this);
                }
            }
            if (BoundsChanged != null)
            {
                BoundsChanged.Invoke(this);
            }
        }

        protected virtual void OnParentGroupChanged()
        {
            if (ParentGroup != null)
            {
                if (KnownParentGroup == null ||
                    KnownParentGroup.Source != ParentGroup)
                {
                    KnownParentGroup = SharedData.Graph.GetGroupWrapper(ParentGroup, false);
                }
                IsGrouped = true;
            }
            else
            {
                KnownParentGroup = null;
                IsGrouped = false;
            }
        }

        protected virtual void OnAnnotationsChanged()
        {
            if (Annotations != null)
            {
                DestructAnnotate();
                InternalAnnotations =
                    new ObservableElements
                        <IAnnotation, AnnotationEditorWrapper>
                        (Annotations,
                         ElementType.Annotation,
                         SourceType.Node,
                         SharedData.EventAggregator,
                         SharedData.Graph.GetAnnotationWrapper);
                ConstrctAnnotate();
            }
            else
            {
                DestructAnnotate();
                InternalAnnotations = null;
            }
        }

        private void ConstrctAnnotate()
        {
            if (InternalAnnotations != null)
            {
                foreach (var annotate in InternalAnnotations)
                {
                    AddAnnotate(annotate.View);
                }
                InternalAnnotations.Added += InternalAnnotations_Added;
                InternalAnnotations.Deleted += InternalAnnotations_Deleted;
            }
        }

        private void DestructAnnotate()
        {
            if (InternalAnnotations != null)
            {
                foreach (var annotate in InternalAnnotations)
                {
                    DeleteAnnotate(annotate.View);
                }
                InternalAnnotations.Added -= InternalAnnotations_Added;
                InternalAnnotations.Deleted -= InternalAnnotations_Deleted;
            }
        }

        void InternalAnnotations_Added(CollectionArgs<AnnotationEditorWrapper> obj)
        {
            AddAnnotate(obj.Element.View);
        }

        void InternalAnnotations_Deleted(CollectionArgs<AnnotationEditorWrapper> obj)
        {
            DeleteAnnotate(obj.Element.View);
        }

        private void AddAnnotate(UIElement view)
        {
            Panel host = null;
            if (View != null)
            {
                if ((View as Node) != null)
                {
                    host = (View as Node).AnnotationHost;
                }
                else if ((View as Connector) != null)
                {
                    host = (View as Connector).AnnotationHost;
                }
            }
            if (host != null)
            {
                host.Children.Add(view);
            }
        }

        private void DeleteAnnotate(UIElement view)
        {
            Panel host = null;
            if (View != null)
            {
                if ((View as Node) != null)
                {
                    host = (View as Node).AnnotationHost;
                }
                else if ((View as Connector) != null)
                {
                    host = (View as Connector).AnnotationHost;
                }
            }
            if (host != null)
            {
                host.Children.Remove(view);
            }
        }

        protected override void OnPropertyChanged(string name)
        {
            //if (SharedData.UndoRedoController != null && CanLogData())
            //{
            //    LogData(GetData());
            //}
            if (!SharedData._unitchanging)
            {
                switch (name)
                {
                    case GroupableConstants.IsSelected:
                        OnIsSelectedChanged();
                        break;
                    case GroupableConstants.Bounds:
                        OnBoundsChanged();
                        break;
                    case GroupableConstants.ParentGroup:
                        OnParentGroupChanged();
                        break;
                    case GroupableConstants.Annotations:
                        OnAnnotationsChanged();
                        break;
                    case GroupableConstants.ZIndex:
                        OnZindexChanged();
                        break;
                }
            }
        }

        private void OnZindexChanged()
        {
            CurrentState.Zindex = ZIndex;
        }

        protected virtual bool CanSelect()
        {
            DiagramPreviewEventArgs e = new DiagramPreviewEventArgs(this.View);
            SharedData.Graph.OnItemSelectingEvent(e);
            if (!e.Cancel)
            {
                if ((SharedData.Graph.Tool.Contains(Tool.SingleSelect) ||
                     SharedData.Graph.Tool.Contains(Tool.MultipleSelect)) &&
                    SharedData.Graph.MultipleSelectionMode != MultipleSelectionMode.None)
                {
                    return true;
                }
            }
            return false;
        }
        
        protected virtual bool CanMultipleSelect()
        {
            if (SharedData.Graph.Tool.Contains(Tool.MultipleSelect) &&
                SharedData.Graph.MultipleSelectionMode != MultipleSelectionMode.None)
            {
                if (SharedData.Graph.MultipleSelectionMode.Contains(MultipleSelectionMode.HoldKeyAndTap) &&
                    CodeSharingUtilities.IsControlKeyPressed())
                {
                    return true;
                }
                else if (SharedData.Graph.MultipleSelectionMode.Contains(MultipleSelectionMode.JustTap))
                {
                    return true;
                }
            }
            return false;
        }

        //private void InvokePropertyChanged(string name)
        //{
        //}
        
        public void SetVirtualizationState(VirtualizationState value)
        {
            VirtualizationState = value;
        }

        public event SimpleEventHandler BoundsChanged;

        private IInternalGroup _mKnownParentGroup;

        public IInternalGroup KnownParentGroup
        {
            get { return _mKnownParentGroup; }
            set
            {
                if (_mKnownParentGroup != value)
                {
                    if (_mKnownParentGroup != null)
                    {
                        _mKnownParentGroup.AddRemoveItem(this, false);
                    }
                    _mKnownParentGroup = value;
                    if (_mKnownParentGroup != null)
                    {
                        if (_mKnownParentGroup.Source != ParentGroup)
                        {
                            ParentGroup = _mKnownParentGroup.Source;
                        }
                        _mKnownParentGroup.AddRemoveItem(this, true);
                    }
                    else
                    {
                        ParentGroup = null;
                    }
                }
            }
        }
        
        public UndoableState State { get; set; }

        public virtual bool CanUndo(object data)
        {
            if (State == UndoableState.Idle)
            {
                return true;
            }
            return false;
        }

        public virtual bool CanRedo(object data)
        {
            if (State == UndoableState.Idle)
            {
                return true;
            }
            return false;
        }

        private object compositeID = null;
        private IGraph _graph;

        public virtual bool CanLogData()
        {
            if (State == UndoableState.Idle)
            {
                if (SharedData.UndoRedoController.CanLogData)
                {
                    //if (SharedData.UndoRedoController.State == UndoableState.Idle)
                    //{
                    //    return true;
                    //}
                    //else 
                    if(SharedData.UndoRedoController.State == UndoableState.CompositeLogging &&
                         SharedData.UndoRedoController.CompositeID != compositeID)
                    {
                        compositeID = SharedData.UndoRedoController.CompositeID;
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool LogData(object data)
        {
            //if (CanLogData())
            {
                return SharedData.UndoRedoController.LogData(this, data);
            }
            //return false;
        }

        public virtual object GetData()
        {
            return CurrentState;
        }

        public virtual object Undo(object data)
        {
            return data;
        }

        public virtual object Redo(object data)
        {
            return data;
        }

        public Quad Quad { get; set; }
        public UIElement SelectionPreview { get; set; }

    }
}
