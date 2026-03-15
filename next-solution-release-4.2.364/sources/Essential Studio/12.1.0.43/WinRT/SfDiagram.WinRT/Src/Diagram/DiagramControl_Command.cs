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
using System.Windows;
using System.Windows.Input;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using System.IO;
using Windows.UI;
using Windows.UI.Xaml.Input;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#else

using ManipulationDeltaRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System.ComponentModel;
using Syncfusion.UI.Xaml.Diagram.Controls;

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IDiagramParameterBase
    {
        SfDiagram Diagram { get; set; }
    }

    public interface IDiagramCommands : INotifyPropertyChanged
    {
        ICommand Undo { get; set; }
        void OnUndoCommand(object param);
        bool CanUndoExecute(object param);

        ICommand Zoom { get; set; }
        void OnZoomCommand(object param);
        bool CanZoomExecute(object param);

        ICommand Reset { get; set; }
        void OnResetCommand(object param);
        bool CanResetExecute(object param);

        ICommand SelectAll { get; set; }
        void OnSelectAllCommand(object param);
        bool CanSelectAllExecute(object param);

        ICommand Redo { get; set; }
        void OnRedoCommand(object param);
        bool CanRedoExecute(object param);

        /// <summary>
        /// Group Command
        /// </summary>
        ICommand Group { get; set; }
        void OnGroupCommand(object param);
        bool CanGroupExecute(object param);

        /// <summary>
        /// UnGroup Command
        /// </summary>
        ICommand UnGroup { get; set; }
        void OnUnGroupCommand(object param);
        bool CanUnGroupExecute(object param);

        /// <summary>
        /// SameSize Command
        /// </summary>
        ICommand SameSize { get; set; }
        void OnSameSizeCommand(object param);
        bool CanSameSizeExecute(object param);

        /// <summary>
        /// SameHeight Command
        /// </summary>
        ICommand SameHeight { get; set; }
        void OnSameHeightCommand(object param);
        bool CanSameHeightExecute(object param);

        /// <summary>
        /// SameWidth Command
        /// </summary>
        ICommand SameWidth { get; set; }
        void OnSameWidthCommand(object param);
        bool CanSameWidthExecute(object param);

        /// <summary>
        /// AlignBottom Command
        /// </summary>
        ICommand AlignBottom { get; set; }
        void OnAlignBottomCommand(object param);
        bool CanAlignBottomExecute(object param);

        /// <summary>
        /// AlignTop Command
        /// </summary>
        ICommand AlignTop { get; set; }
        void OnAlignTopCommand(object param);
        bool CanAlignTopExecute(object param);

        /// <summary>
        /// AlignLeft Command
        /// </summary>
        ICommand AlignLeft { get; set; }
        void OnAlignLeftCommand(object param);
        bool CanAlignLeftExecute(object param);

        /// <summary>
        /// AlignCenter Command
        /// </summary>
        ICommand AlignCenter { get; set; }
        void OnAlignCenterCommand(object param);
        bool CanAlignCenterExecute(object param);

        /// <summary>
        /// AlignRight Command
        /// </summary>
        ICommand AlignRight { get; set; }
        void OnAlignRightCommand(object param);
        bool CanAlignRightExecute(object param);

        /// <summary>
        /// AlignMiddle Command
        /// </summary>
        ICommand AlignMiddle { get; set; }
        void OnAlignMiddleCommand(object param);
        bool CanAlignMiddleExecute(object param);

        /// <summary>
        /// SpaceAcross Command
        /// </summary>
        ICommand SpaceAcross { get; set; }
        void OnSpaceAcrossCommand(object param);
        bool CanSpaceAcrossExecute(object param);

        /// <summary>
        /// SpaceDown Command
        /// </summary>
        ICommand SpaceDown { get; set; }
        void OnSpaceDownCommand(object param);
        bool CanSpaceDownExecute(object param);

        /// <summary>
        /// SendToBack Command
        /// </summary>
        ICommand SendToBack { get; set; }
        void OnSendToBackCommand(object param);
        bool CanSendToBackExecute(object param);

        /// <summary>
        /// SendBackward Command
        /// </summary>
        ICommand SendBackward { get; set; }
        void OnSendBackwardCommand(object param);
        bool CanSendBackwardExecute(object param);

        /// <summary>
        /// BringToFront Command
        /// </summary>
        ICommand BringToFront { get; set; }
        void OnBringToFrontCommand(object param);
        bool CanBringToFrontExecute(object param);

        /// <summary>
        /// BringForward Command
        /// </summary>
        ICommand BringForward { get; set; }
        void OnBringForwardCommand(object param);
        bool CanBringForwardExecute(object param);

        /// <summary>
        /// MoveDown Command
        /// </summary>
        ICommand MoveDown { get; set; }
        void OnMoveDownCommand(object param);
        bool CanMoveDownExecute(object param);

        /// <summary>
        /// MoveDown Command
        /// </summary>
        ICommand MoveUp { get; set; }
        void OnMoveUpCommand(object param);
        bool CanMoveUpExecute(object param);

        /// <summary>
        /// MoveDown Command
        /// </summary>
        ICommand MoveLeft { get; set; }
        void OnMoveLeftCommand(object param);
        bool CanMoveLeftExecute(object param);

        /// <summary>
        /// MoveDown Command
        /// </summary>
        ICommand MoveRight { get; set; }
        void OnMoveRightCommand(object param);
        bool CanMoveRightExecute(object param);

        /// <summary>
        /// Cut Command
        /// </summary>
        ICommand Cut { get; set; }
        void OnCutCommand(object param);
        bool CanCutExecute(object param);

        /// <summary>
        /// Copy Command
        /// </summary>
        ICommand Copy { get; set; }
        void OnCopyCommand(object param);
        bool CanCopyExecute(object param);

        /// <summary>
        /// Paste Command
        /// </summary>
        ICommand Paste { get; set; }
        void OnPasteCommand(object param);
        bool CanPasteExecute(object param);

        /// <summary>
        /// Delete Command
        /// </summary>
        ICommand Delete { get; set; }
        void OnDeleteCommand(object param);
        bool CanDeleteExecute(object param);

        /// <summary>
        /// Draw Command
        /// </summary>
        ICommand Draw { get; set; }
        void OnDrawCommand(object param);
        bool CanDrawExecute(object param);

        /// <summary>
        /// Flip Command
        /// </summary>
        ICommand Flip { get; set; }
        void OnFlipCommand(object param);
        bool CanFlipExecute(object param);

        /// <summary>
        /// FitToPage Command
        /// </summary>
        ICommand FitToPage { get; set; }
        void OnFitToPageCommand(object param);
        bool CanFitToPageExecute(object param);

        /// <summary>
        /// Duplicate Command
        /// </summary>
        ICommand Duplicate { get; set; }
        void OnDuplicateCommand(object param);
        bool CanDuplicateExecute(object param);
    }

    internal partial class DiagramCommands : DiagramElementViewModel, ISharedData, IDiagramCommands
    {
        private SharedData _mSharedData;

        private ObservableElements<object, IInternalNode> _mInternalNodes
        {
            get { return _mSharedData.Graph.InternalNodes; }
        }
        private ObservableElements<object, IInternalConnector> _mInternalConnectors
        {
            get { return _mSharedData.Graph.InternalConnectors; }
        }
        private ObservableElements<object, IInternalGroup> _mInternalGroups
        {
            get { return _mSharedData.Graph.InternalGroups; }
        }

        public void Init(SharedData shared)
        {
            _mSharedData = shared;
            InitCommands();
            _mSharedData.Selected.Subscribe(e => InvalidateCommand());
            _mSharedData.UnSelected.Subscribe(e => InvalidateCommand());
        }

        private void InvalidateCommand()
        {
            Undo.InvalidateCanExecute();
            Zoom.InvalidateCanExecute();
            Reset.InvalidateCanExecute();
            SelectAll.InvalidateCanExecute();
            Redo.InvalidateCanExecute();
            Group.InvalidateCanExecute();
            UnGroup.InvalidateCanExecute();
            SameSize.InvalidateCanExecute();
            SameHeight.InvalidateCanExecute();
            SameWidth.InvalidateCanExecute();
            AlignBottom.InvalidateCanExecute();
            AlignTop.InvalidateCanExecute();
            AlignLeft.InvalidateCanExecute();
            AlignCenter.InvalidateCanExecute();
            AlignRight.InvalidateCanExecute();
            AlignMiddle.InvalidateCanExecute();
            SpaceAcross.InvalidateCanExecute();
            SpaceDown.InvalidateCanExecute();
            SendToBack.InvalidateCanExecute();
            SendBackward.InvalidateCanExecute();
            BringToFront.InvalidateCanExecute();
            BringForward.InvalidateCanExecute();
            MoveDown.InvalidateCanExecute();
            MoveUp.InvalidateCanExecute();
            MoveLeft.InvalidateCanExecute();
            MoveRight.InvalidateCanExecute();
            Cut.InvalidateCanExecute();
            Copy.InvalidateCanExecute();
            Paste.InvalidateCanExecute();
            Delete.InvalidateCanExecute();
            Draw.InvalidateCanExecute();
            Flip.InvalidateCanExecute();
            Duplicate.InvalidateCanExecute();
        }

        private void InitCommands()
        {
            SelectAll = new Command(OnSelectAllCommand, CanSelectAllExecute);
            Reset = new Command(OnResetCommand, CanResetExecute);
            Zoom = new Command(OnZoomCommand, CanZoomExecute);
            Undo = new Command(OnUndoCommand, CanUndoExecute);
            Redo = new Command(OnRedoCommand, CanRedoExecute);
            MoveDown = new Command(OnMoveDownCommand, CanMoveDownExecute);
            MoveUp = new Command(OnMoveUpCommand, CanMoveUpExecute);
            MoveLeft = new Command(OnMoveLeftCommand, CanMoveLeftExecute);
            MoveRight = new Command(OnMoveRightCommand, CanMoveRightExecute);
            Group = new Command(OnGroupCommand, CanGroupExecute);
            UnGroup = new Command(OnUnGroupCommand, CanUnGroupExecute);
            SameSize = new Command(OnSameSizeCommand, CanSameSizeExecute);
            SameWidth = new Command(OnSameWidthCommand, CanSameWidthExecute);
            SameHeight = new Command(OnSameHeightCommand, CanSameHeightExecute);
            AlignBottom = new Command(OnAlignBottomCommand, CanAlignBottomExecute);
            AlignTop = new Command(OnAlignTopCommand, CanAlignTopExecute);
            AlignMiddle = new Command(OnAlignMiddleCommand, CanAlignMiddleExecute);
            AlignLeft = new Command(OnAlignLeftCommand, CanAlignLeftExecute);
            AlignRight = new Command(OnAlignRightCommand, CanAlignRightExecute);
            AlignCenter = new Command(OnAlignCenterCommand, CanAlignCenterExecute);
            SpaceAcross = new Command(OnSpaceAcrossCommand, CanSpaceAcrossExecute);
            SpaceDown = new Command(OnSpaceDownCommand, CanSpaceDownExecute);
            SendToBack = new Command(OnSendToBackCommand, CanSendToBackExecute);
            SendBackward = new Command(OnSendBackwardCommand, CanSendBackwardExecute);
            BringForward = new Command(OnBringForwardCommand, CanBringForwardExecute);
            BringToFront = new Command(OnBringToFrontCommand, CanBringToFrontExecute);
            Cut = new Command(OnCutCommand, CanCutExecute);
            Copy = new Command(OnCopyCommand, CanCopyExecute);
            Paste = new Command(OnPasteCommand, CanPasteExecute);
            Duplicate = new Command(OnDuplicateCommand, CanDuplicateExecute);
            Delete = new Command(OnDeleteCommand, CanDeleteExecute);

            Draw = new Command(OnDrawCommand, CanDrawExecute);
            Flip = new Command(OnFlipCommand, CanFlipExecute);
            FitToPage = new Command(OnFitToPageCommand, CanFitToPageExecute);
        }

        public void OnFitToPageCommand(object obj)
        {
            if (_mSharedData.ScrollViewer == null) { return; }
            BeginComposite();
            if (obj == null)
            {
                obj = new FitToPageParameter() { FitToPage = Diagram.FitToPage.FitToPage };
            }
            if (obj as IFitToPage != null)
            {
                double right = _mSharedData.SpatialSearch._pageRight != long.MinValue ? _mSharedData.SpatialSearch._pageRight : 0;
                double bottom = _mSharedData.SpatialSearch._pageBottom != long.MinValue ? _mSharedData.SpatialSearch._pageBottom : 0;
                double left = _mSharedData.SpatialSearch._pageLeft != long.MaxValue ? _mSharedData.SpatialSearch._pageLeft : 0;
                double top = _mSharedData.SpatialSearch._pageTop != long.MaxValue ? _mSharedData.SpatialSearch._pageTop : 0;
                IFitToPage FitToPageParameter = obj as IFitToPage;
                double pageWidth = _mSharedData.PageSettingsController.PageWidth;
                double pageHeight = _mSharedData.PageSettingsController.PageHeight;
                Thickness margin = FitToPageParameter.Margin;

                Rect pageBounds;

                if (pageWidth.IsValid() && pageWidth != 0)
                {
                    if (_mSharedData.PageSettingsController.MultiplePage)
                    {
                        left = Math.Floor(left / pageWidth) * pageWidth;
                        right = Math.Ceiling(right / pageWidth) * pageWidth;
                        if (right == 0)
                        {
                            right = pageWidth;
                        }
                    }
                    else
                    {
                        right = pageWidth;
                        left = 0;
                    }
                }

                if (pageHeight.IsValid() && pageHeight != 0)
                {
                    if (_mSharedData.PageSettingsController.MultiplePage)
                    {
                        top = Math.Floor(top / pageHeight) * pageHeight;
                        bottom = Math.Ceiling(bottom / pageHeight) * pageHeight;
                        if (bottom == 0)
                        {
                            bottom = pageHeight;
                        }
                    }
                    else
                    {
                        bottom = pageHeight;
                        top = 0;
                    }
                }
                if (right - left > 0 && bottom - top > 0)
                {
                    pageBounds = new Rect(left, top, right - left, bottom - top);

                    double ViewportWidth = _mSharedData.ScrollViewer.ViewportWidth;
                    double ViewportHeight = _mSharedData.ScrollViewer.ViewportHeight;
                    double HorizontalOffset = _mSharedData.ScrollViewer.HorizontalOffset;
                    double VerticalOffset = _mSharedData.ScrollViewer.VerticalOffset;

                    double x = (ViewportWidth - margin.Left - margin.Right) / pageBounds.Width;
                    double y = (ViewportHeight - margin.Bottom - margin.Top) / pageBounds.Height;
                    double currentZoom = _mSharedData.ScrollViewer.CurrentZoom;
                    if ((FitToPageParameter.FitToPage & Diagram.FitToPage.FitToPage) == Diagram.FitToPage.FitToPage)
                    {
                        currentZoom = Math.Min(x, y);
                    }
                    else if (FitToPageParameter.FitToPage.Contains(Diagram.FitToPage.FitToWidth))
                    {
                        currentZoom = x;
                    }
                    else if (FitToPageParameter.FitToPage.Contains(Diagram.FitToPage.FitToHeight))
                    {
                        currentZoom = y;
                    }

                    if ((y < x || !FitToPageParameter.FitToPage.Contains(Diagram.FitToPage.FitToWidth) || !FitToPageParameter.FitToPage.Contains(Diagram.FitToPage.FitToHeight)) &&
                        ViewportHeight <= pageBounds.Height * currentZoom)
                    {
                        VerticalOffset = pageBounds.Top * currentZoom - margin.Top;
                    }
                    else
                    {
                        double spaceAtTop = pageBounds.Top * currentZoom;
                        double spaceAtBottom = ViewportHeight - pageBounds.Bottom * currentZoom;
                        VerticalOffset = spaceAtTop - (spaceAtBottom + spaceAtTop) / 2;
                    }

                    if ((x < y || !FitToPageParameter.FitToPage.Contains(Diagram.FitToPage.FitToHeight) ||
                       !FitToPageParameter.FitToPage.Contains(Diagram.FitToPage.FitToWidth)) && pageBounds.Width * currentZoom >= ViewportWidth)
                    {
                        HorizontalOffset = pageBounds.Left * currentZoom - margin.Left;
                    }
                    else
                    {
                        double spaceAtLeft = pageBounds.Left * currentZoom;
                        double spaceAtRight = (ViewportWidth - pageBounds.Right * currentZoom);
                        HorizontalOffset = spaceAtLeft - (spaceAtRight + spaceAtLeft) / 2;
                    }
                    _mSharedData.ScrollViewer.PanTo(new Point(HorizontalOffset, VerticalOffset));
                    _mSharedData.ScrollViewer.CurrentZoom = currentZoom;
                }
            }
            EndComposite();
        }

        public void OnFlipCommand(object param)
        {
            BeginComposite();
            if (param == null)
            {
                param = new FlipParameter() { Flip = Diagram.Flip.Flip };
            }
            if (param != null)
            {
                IFlip flipParam = param as IFlip;
                if (flipParam != null)
                {
                    if (_mSharedData.Graph.InternalSelectedItems != null && (_mSharedData.Graph.InternalSelectedItems as SelectorWrapper) != null)
                    {
                        if ((flipParam.Flip & Diagram.Flip.Flip) == Diagram.Flip.Flip)
                        {
                            _mSharedData.Graph.InternalSelectedItems.RotateAngle = (_mSharedData.Graph.InternalSelectedItems.RotateAngle + 180) % 360;
                        }
                        else if (flipParam.Flip == Diagram.Flip.HorizontalFlip)
                        {
                            double flipped = -((_mSharedData.Graph.InternalSelectedItems.View as Selector).RotateAngle % 360);

                            if (flipped < 0)
                                flipped += 360;

                            foreach (IInternalNode element in (_mSharedData.Graph.InternalSelectedItems as SelectorWrapper).InternalNodes)
                            {
                                //element.Flip = Diagram.Flip.None;
                                element.Flip = element.Flip == Diagram.Flip.None ? Diagram.Flip.HorizontalFlip : Diagram.Flip.None;
                            }
                            (_mSharedData.Graph.InternalSelectedItems.View as Selector).RotateAngle = flipped;
                        }

                        else if (flipParam.Flip == Diagram.Flip.VerticalFlip)
                        {
                            double flipped = 180 - ((_mSharedData.Graph.InternalSelectedItems.View as Selector).RotateAngle % 360);
                            if (flipped < 0)
                                flipped += 360;
                            foreach (IInternalNode element in (_mSharedData.Graph.InternalSelectedItems as SelectorWrapper).InternalNodes)
                            {
                                //element.Flip = Diagram.Flip.None;
                                element.Flip = element.Flip == Diagram.Flip.None ? Diagram.Flip.VerticalFlip : Diagram.Flip.None;
                            }
                            (_mSharedData.Graph.InternalSelectedItems.View as Selector).RotateAngle = flipped;
                        }

                    }
                }
            }
            EndComposite();
        }

        public void OnDrawCommand(object param)
        {
            if (param is IDrawParameter)
            {
                _mSharedData.Graph.StartDraw(param as IDrawParameter);
            }
        }

        #region ClipBoard

        public void OnDeleteCommand(object sender)
        {
            DiagramPreviewEventArgs e = new DiagramPreviewEventArgs(_mSharedData.Graph.SelectedItems);
            _mSharedData.Graph.OnItemDeletingEvent(e);
            if (!e.Cancel)
            {
                DeleteObjects(_mSharedData.Graph.InternalSelectedItems);
            }
        }

        private void DeleteObjects(IInternalGroup internalSelector)
        {
            if (internalSelector.InternalNodes != null)
            {
                List<IInternalNode> selectedV = internalSelector.InternalNodes.ToList();
                foreach (var internalNode in selectedV)
                {
                    _mInternalNodes.Remove(internalNode);
                }

                if (internalSelector.InternalConnectors != null)
                {
                    List<IInternalConnector> selectedE = internalSelector.InternalConnectors.ToList();
                    foreach (var internalConnector in selectedE)
                    {
                        _mInternalConnectors.Remove(internalConnector);
                    }
                }

                if (internalSelector.InternalGroups != null)
                {
                    List<IInternalGroup> seletedG = internalSelector.InternalGroups.ToList();
                    foreach (var internalgroup in seletedG)
                    {
                        DeleteObjects(internalgroup);
                        _mInternalGroups.Remove(internalgroup);
                    }
                }
            }
        }

        public void OnCutCommand(object sender)
        {
            OnCopyCommand(null);
            OnDeleteCommand(null);
        }

        public void OnDuplicateCommand(object obj)
        {
            BeginComposite();
            OnCopyCommand(null);
            OnPasteCommand(obj);
            EndComposite();
            if (obj != null && obj is IDuplicateParameter)
            {
                IDuplicateParameter param = obj as IDuplicateParameter;
                if (param.DragClone && param.PointerArgs != null)
                {
                    istarted = false;
                    _initialLocation = param.PointerArgs.GetCurrentPoint(_mSharedData.Page).Position;
                    if (param.Thumb != null)
                    {
#if WINRT
                        param.Thumb.PointerMoved += Thumb_PointerMoved;
                        param.Thumb.PointerReleased += Thumb_PointerReleased;
#else
                        param.Thumb.MouseMove += Thumb_PointerMoved;
                        param.Thumb.MouseLeftButtonUp += Thumb_PointerReleased;
#endif
                    }
                }
            }
        }

        void Thumb_PointerReleased(object sender, MouseEventArgs e)
        {
#if WINRT
            (sender as DiagramThumb).PointerMoved -= Thumb_PointerMoved;
            (sender as DiagramThumb).PointerReleased -= Thumb_PointerReleased;
#else
            (sender as DiagramThumb).MouseMove-=Thumb_PointerMoved;
            (sender as DiagramThumb).MouseLeftButtonUp -= Thumb_PointerReleased;
#endif
        }

        void Thumb_PointerMoved(object sender, MouseEventArgs e)
        {
            if ((sender as DiagramThumb).CapturePointer(e))
            {
                if (!istarted)
                {
                    NodeDragStartingEvent dragstart = _mSharedData.EventAggregator.GetEvent<NodeDragStartingEvent>();
                    dragstart.Publish(e);
                    istarted = true;
                }
                Point current = e.GetCurrentPoint(_mSharedData.Page).Position;
                NodeDragEvent dragevent = _mSharedData.EventAggregator.GetEvent<NodeDragEvent>();
                dragevent.Publish(
                    new NodeDragDeltaEventArgs(sender,
                        new ManipulationDragDelta(current.X - _initialLocation.Value.X, current.Y - _initialLocation.Value.Y, 0, 0)));
            }
        }


        Point? _initialLocation;
        bool istarted = false;

        public void OnCopyCommand(object sender)
        {
            string copy = _mSharedData.Serializer.Copy();
#if WINRT
            var dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Move;
            dataPackage.SetText(copy);
            Clipboard.SetContent(dataPackage);
#endif
        }

#if WINRT
        public async void OnPasteCommand(object sender)
        {
            var dataPackageView = Clipboard.GetContent();
            string text = await dataPackageView.GetTextAsync();
            _mSharedData.Serializer.Paste(sender, text);
        }
#else
        public void OnPasteCommand(object sender)
        {
        }
#endif

        #endregion

        #region Undo Redo commands

        public void OnUndoCommand(object sender)
        {
            _mSharedData.UndoRedoController.Undo();
        }

        public void OnRedoCommand(object sender)
        {
            _mSharedData.UndoRedoController.Redo();
        }

        #endregion

        private void BeginComposite()
        {
            if (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Undoable))
                _mSharedData.UndoRedoController.BeginComposite(null);
        }

        private void EndComposite()
        {
            if (_mSharedData.Graph.Constraints.Contains(GraphConstraints.Undoable))
                _mSharedData.UndoRedoController.EndComposite();
        }

        #region ZoomPan

        public void OnZoomCommand(object sender)
        {
            if (sender != null)
            {
                if (sender is IZoomManipulationParameter)
                {
                    IZoomManipulationParameter param = sender as IZoomManipulationParameter;
                    if (param.ManipulationArgs != null)
                    {
#if TOUCH
                        if (param.ManipulationArgs.Delta.Scale < 1)
                        {
                            _mSharedData.ScrollViewer.ZoomIn(param);
                        }
                        else
                        {
                            _mSharedData.ScrollViewer.ZoomOut(param);
                        }
#endif
                    }
                }
                else if (sender is IZoomPointerParameter)
                {
                    IZoomPointerParameter param = sender as IZoomPointerParameter;
                    if (param.ZoomCommand == ZoomCommand.ZoomIn)
                    {
                        _mSharedData.ScrollViewer.ZoomIn(param);
                    }
                    else if (param.ZoomCommand == ZoomCommand.ZoomOut)
                    {
                        _mSharedData.ScrollViewer.ZoomOut(param);
                    }
                }
                else if (sender is IZoomPositionParameter)
                {
                    IZoomPositionParameter param = sender as IZoomPositionParameter;
                    if (param.ZoomCommand == ZoomCommand.ZoomIn)
                    {
                        _mSharedData.ScrollViewer.ZoomIn(param);
                    }
                    else if (param.ZoomCommand == ZoomCommand.ZoomOut)
                    {
                        _mSharedData.ScrollViewer.ZoomOut(param);
                    }
                }
            }
        }

        public void OnResetCommand(object sender)
        {
            if (sender != null && sender is IReset)
            {
                switch ((sender as IReset).Reset)
                {
                    case Diagram.Reset.ZoomPan:
                        _mSharedData.ScrollViewer.Reset();
                        break;
                    case Diagram.Reset.Pan:
                        _mSharedData.ScrollViewer.ResetPan();
                        break;
                    case Diagram.Reset.Zoom:
                        _mSharedData.ScrollViewer.ResetZoom();
                        break;
                }
            }
        }
        #endregion

        #region SelectAll Commands
        public void OnSelectAllCommand(object sender)
        {
            if (_mInternalNodes != null && _mInternalNodes.Count > 0)
            {
                foreach (var o in _mInternalNodes)
                {
                    o.IsSelected = true;
                }
            }
            if (_mInternalConnectors != null && _mInternalConnectors.Count > 0)
            {
                foreach (var o in _mInternalConnectors)
                {
                    o.IsSelected = true;
                }
            }
            if (_mInternalGroups != null && _mInternalGroups.Count > 0)
            {
                foreach (var o in _mInternalGroups)
                {
                    o.IsSelected = true;
                }
            }

        }
        #endregion

        #region Nudge commands

        /// <summary>
        /// Invoked when the MoveUp Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveUpCommand(object sender)
        {
            BeginComposite();
            double NudgeY = 1;
            CommonFunctionForNudge(NudgeY, "Up");

        }

        /// <summary>
        /// Invoked when the MoveDown Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveDownCommand(object sender)
        {
            BeginComposite();
            double NudgeY = 1;
            CommonFunctionForNudge(NudgeY, "Down");
        }

        /// <summary>
        /// Invoked when the MoveLeft Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveLeftCommand(object sender)
        {
            BeginComposite();
            double NudgeX = 1;
            CommonFunctionForNudge(NudgeX, "Left");
        }

        /// <summary>
        /// Invoked when the MoveRight Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        public void OnMoveRightCommand(object sender)
        {
            BeginComposite();
            double NudgeX = 1;
            CommonFunctionForNudge(NudgeX, "Right");
        }

        private void CommonFunctionForNudge(double value, string condition)
        {
            if (_mSharedData.Graph.InternalSelectedItems.InternalNodes != null)
            {
                foreach (var o in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                {
                    if (condition == "Up")
                    {
                        o.OffsetY -= value;
                    }
                    else if (condition == "Down")
                    {
                        o.OffsetY += value;
                    }
                    else if (condition == "Left")
                    {
                        o.OffsetX -= value;
                    }
                    else if (condition == "Right")
                    {
                        o.OffsetX += value;
                    }

                }
            }

            if (_mSharedData.Graph.InternalSelectedItems.InternalConnectors != null)
            {
                foreach (var o in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                {
                    if (condition == "Up")
                    {
                        UpdateConnector(o, value, "MY");
                    }
                    else if (condition == "Down")
                    {
                        UpdateConnector(o, value, "AY");
                    }
                    else if (condition == "Left")
                    {
                        UpdateConnector(o, value, "MX");
                    }
                    else if (condition == "Right")
                    {
                        UpdateConnector(o, value, "AX");
                    }
                }
            }
            if (_mSharedData.Graph.InternalSelectedItems.InternalGroups != null)
            {
                foreach (var o in _mSharedData.Graph.InternalSelectedItems.InternalGroups)
                {
                    if (condition == "Up")
                    {
                        o.OffsetY -= value;
                    }
                    else if (condition == "Down")
                    {
                        o.OffsetY += value;
                    }
                    else if (condition == "Left")
                    {
                        o.OffsetX -= value;
                    }
                    else if (condition == "Right")
                    {
                        o.OffsetX += value;
                    }
                    if (o.InternalNodes.ToArray().Any())
                    {
                        foreach (var node in o.InternalNodes)
                        {
                            if (condition == "Up")
                            {
                                node.OffsetY -= value;
                            }
                            else if (condition == "Down")
                            {
                                node.OffsetY += value;
                            }
                            else if (condition == "Left")
                            {
                                o.OffsetX -= value;
                            }
                            else if (condition == "Right")
                            {
                                o.OffsetX += value;
                            }
                        }
                    }

                    if (o.InternalConnectors != null && o.InternalConnectors.ToArray().Any())
                    {
                        foreach (var connector in o.InternalConnectors)
                        {
                            if (condition == "Up")
                            {
                                UpdateConnector(connector, value, "MY");
                            }
                            else if (condition == "Down")
                            {
                                UpdateConnector(connector, value, "AY");
                            }
                            else if (condition == "Left")
                            {
                                UpdateConnector(connector, value, "MX");
                            }
                            else if (condition == "Right")
                            {
                                UpdateConnector(connector, value, "AX");
                            }
                        }
                    }
                }
            }
            EndComposite();
        }

        private void UpdateConnector(IInternalConnector connector, double value, string val)
        {
            if (connector.KnownSourceNode == null && connector.KnownTargetNode == null)
            {
                if (val == "MY")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X, connector.SourcePoint.Y - value);
                    connector.TargetPoint = new Point(connector.TargetPoint.X, connector.TargetPoint.Y - value);
                }
                else if (val == "AY")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X, connector.SourcePoint.Y + value);
                    connector.TargetPoint = new Point(connector.TargetPoint.X, connector.TargetPoint.Y + value);
                }
                else if (val == "MX")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X - value, connector.SourcePoint.Y);
                    connector.TargetPoint = new Point(connector.TargetPoint.X - value, connector.TargetPoint.Y);
                }
                else if (val == "AX")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X + value, connector.SourcePoint.Y);
                    connector.TargetPoint = new Point(connector.TargetPoint.X + value, connector.TargetPoint.Y);
                }
            }
            else if (connector.KnownSourceNode != null && connector.KnownTargetNode == null)
            {
                if (val == "MY")
                {
                    connector.TargetPoint = new Point(connector.TargetPoint.X, connector.TargetPoint.Y - value);
                }
                else if (val == "AY")
                {
                    connector.TargetPoint = new Point(connector.TargetPoint.X, connector.TargetPoint.Y + value);
                }
                else if (val == "MX")
                {
                    connector.TargetPoint = new Point(connector.TargetPoint.X - value, connector.TargetPoint.Y);
                }
                else if (val == "AX")
                {
                    connector.TargetPoint = new Point(connector.TargetPoint.X + value, connector.TargetPoint.Y);
                }
            }
            else if (connector.KnownSourceNode == null && connector.KnownTargetNode != null)
            {
                if (val == "MY")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X, connector.SourcePoint.Y - value);
                }
                else if (val == "AY")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X, connector.SourcePoint.Y + value);
                }
                else if (val == "MX")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X - value, connector.SourcePoint.Y);
                }
                else if (val == "AX")
                {
                    connector.SourcePoint = new Point(connector.SourcePoint.X + value, connector.SourcePoint.Y);
                }
            }
        }

        #endregion

        #region Group Commands
        /// <summary>
        /// Invoked when the GroupNodes Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnGroupCommand(object sender)
        {
            if (_mSharedData.Graph.InternalGroups != null)
            {
                if (_mSharedData.Graph.InternalSelectedItems.InternalNodes.Count > 0 || _mSharedData.Graph.InternalSelectedItems.InternalConnectors.Count > 0
                    || _mSharedData.Graph.InternalSelectedItems.InternalGroups.Count > 0)
                {
                    object group = _mSharedData.Graph.GetNewItem(ElementType.Group, _mSharedData.Graph.InternalGroups.ItemType);

                    IInternalGroup internalgroup = _mSharedData.Graph.GetGroupWrapper(group, true);

                    internalgroup.InternalNodes = new ObservableElements<object, IInternalNode>
                                ((_mSharedData.Graph.InternalSelectedItems.Nodes as IEnumerable<object>).ToList(),
                                 ElementType.Node,
                                 SourceType.Group,
                                 _mSharedData.Graph.SharedData.EventAggregator,
                                 _mSharedData.Graph.SharedData.Graph.GetNodeWrapper);
                    internalgroup.InternalConnectors = new ObservableElements<object, IInternalConnector>
                                ((_mSharedData.Graph.InternalSelectedItems.Connectors as IEnumerable<object>).ToList(),
                                 ElementType.Connector,
                                SourceType.Group,
                                 _mSharedData.Graph.SharedData.EventAggregator,
                                 _mSharedData.Graph.SharedData.Graph.GetConnectorWrapper);

                    internalgroup.InternalGroups = new ObservableElements<object, IInternalGroup>
                                ((_mSharedData.Graph.InternalSelectedItems.Groups as IEnumerable<object>).ToList(),
                                 ElementType.Group,
                                SourceType.Group,
                                 _mSharedData.Graph.SharedData.EventAggregator,
                                 _mSharedData.Graph.SharedData.Graph.GetGroupWrapper);

                    _mSharedData.Graph.InternalGroups.Add(internalgroup, ItemSource.UnKnown);
                    foreach (var groups in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        //internalgroup.InternalNodes.Add(groups,ItemSource.UnKnown);
                        groups.KnownParentGroup = _mSharedData.Graph.GetGroupWrapper(group, false);
                    }
                    foreach (var groups in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        //internalgroup.InternalConnectors.Add(groups,ItemSource.UnKnown);
                        groups.KnownParentGroup = _mSharedData.Graph.GetGroupWrapper(group, false);
                    }
                    foreach (var groups in _mSharedData.Graph.InternalSelectedItems.InternalGroups)
                    {
                        groups.KnownParentGroup = _mSharedData.Graph.GetGroupWrapper(group, false);
                    }
                    _mSharedData.Graph.InternalSelectedItems.InternalNodes.Clear();
                    _mSharedData.Graph.InternalSelectedItems.InternalConnectors.Clear();
                    _mSharedData.Graph.InternalSelectedItems.InternalGroups.Clear();
                    internalgroup.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Invoked when the UnGroupNodes Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnUnGroupCommand(object sender)
        {
            if (_mSharedData.Graph.InternalSelectedItems.InternalGroups.Count > 0)
            {
                List<IInternalGroup> internalgroups = _mSharedData.Graph.InternalSelectedItems.InternalGroups.ToList();
                foreach (IInternalGroup group in internalgroups)
                {
                    while (group.InternalNodes.Count > 0)
                    {
                        group.InternalNodes.ElementAt(0).KnownParentGroup = group.KnownParentGroup;
                    }
                    while (group.InternalConnectors.Count > 0)
                    {
                        group.InternalConnectors.ElementAt(0).KnownParentGroup = group.KnownParentGroup; 
                    }
                    while (group.InternalGroups.Count > 0)
                    {
                        group.InternalGroups.ElementAt(0).KnownParentGroup = group.KnownParentGroup; 
                    }
                    _mSharedData.Graph.InternalGroups.Remove(group);
                }
                _mSharedData.Graph.InternalSelectedItems.InternalGroups.Clear();
            }
        }

        #endregion

        #region Size Commands

        /// <summary>
        /// Invoked when the SameSize Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSameSizeCommand(object sender)
        {
            BeginComposite();
            Size refferencesize = new Size();

            if (_mSharedData.Graph.InternalSelectedItems.InternalNodes != null)
            {
                refferencesize = new Size(_mSharedData.Graph.InternalSelectedItems.InternalNodes.First().UnitWidth, _mSharedData.Graph.InternalSelectedItems.InternalNodes.First().UnitHeight);
                foreach (var o in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                {
                    if (o != _mSharedData.Graph.InternalSelectedItems.InternalNodes.First())
                    {
                        o.UnitWidth = refferencesize.Width;
                        o.UnitHeight = refferencesize.Height;
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the SameWidth Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSameWidthCommand(object sender)
        {
            BeginComposite();
            double refferencesize;
            if (_mSharedData.Graph.InternalSelectedItems.InternalNodes != null)
            {
                refferencesize = _mSharedData.Graph.InternalSelectedItems.InternalNodes.First().UnitWidth;

                foreach (var o in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                {
                    if (o != _mSharedData.Graph.InternalSelectedItems.InternalNodes.First())
                    {
                        o.UnitWidth = refferencesize;
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the SameHeight Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSameHeightCommand(object sender)
        {
            BeginComposite();
            double refferencesize;

            if (_mSharedData.Graph.InternalSelectedItems.InternalNodes != null)
            {
                refferencesize = _mSharedData.Graph.InternalSelectedItems.InternalNodes.First().UnitHeight;

                foreach (var o in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                {
                    if (o != _mSharedData.Graph.InternalSelectedItems.InternalNodes.First())
                    {
                        o.UnitHeight = refferencesize;
                    }
                }
            }
            EndComposite();
        }

        #endregion

        #region Space Commands

        /// <summary>
        /// Invoked when the SpaceDown Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSpaceDownCommand(object sender)
        {
            BeginComposite();
            double firstrefervalue = 0;
            double lasrrefervalue = 0;

            Dictionary<IInternalGroupable, Point> SelectedItems = FindFirstItem("ByY");
            firstrefervalue = SelectedItems.OrderBy(y => y.Value.Y).ToList()[0].Value.Y;
            lasrrefervalue = SelectedItems.OrderBy(y => y.Value.Y).ToList()[SelectedItems.Count() - 1].Value.Y;
            double refervalue = (lasrrefervalue - firstrefervalue) / (SelectedItems.Keys.Count - 1);

            IInternalGroupable firstobj = SelectedItems.OrderBy(y => y.Value.Y).ToList()[0].Key;
            var coll = SelectedItems.Keys.OrderBy(x => SelectedItems[x].Y);
            foreach (IInternalGroupable item in coll)
            {
                if (item != firstobj)
                {
                    if (item is IInternalNode)
                    {
                        (item as IInternalNode).OffsetY = firstrefervalue + (coll.ToList().IndexOf(item) * refervalue);
                    }
                    else if (item is IInternalConnector)
                    {
                        if ((item as IInternalConnector).SourcePoint.Y < (item as IInternalConnector).TargetPoint.Y)
                        {
                            double oldy = (item as IInternalConnector).SourcePoint.Y;
                            (item as IInternalConnector).SourcePoint = new Point((item as IInternalConnector).SourcePoint.X, (firstrefervalue + coll.ToList().IndexOf(item) * refervalue));

                            double offset = (item as IInternalConnector).SourcePoint.Y - oldy;
                            (item as IInternalConnector).TargetPoint = new Point((item as IInternalConnector).TargetPoint.X, (item as IInternalConnector).TargetPoint.Y + offset);

                        }
                        else
                        {
                            double oldy = (item as IInternalConnector).TargetPoint.Y;
                            (item as IInternalConnector).TargetPoint = new Point((item as IInternalConnector).TargetPoint.X, (item as IInternalConnector).TargetPoint.Y + (firstrefervalue + coll.ToList().IndexOf(item) * refervalue));

                            double offset = (item as IInternalConnector).TargetPoint.Y - oldy;
                            (item as IInternalConnector).SourcePoint = new Point((item as IInternalConnector).SourcePoint.X, (item as IInternalConnector).SourcePoint.Y + offset);

                        }
                    }
                    //else if(item is IInternalGroup)
                    //{
                    //    (item as IInternalGroup).OffsetY = firstrefervalue + (SelectedItems.Keys.ToList().IndexOf(item) * refervalue);
                    //}
                }
            }
            EndComposite();
        }

        private Dictionary<IInternalGroupable, Point> FindFirstItem(string condition)
        {
            Dictionary<IInternalGroupable, Point> Collection = new Dictionary<IInternalGroupable, Point>();

            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                if (_mSharedData.Graph.InternalSelectedItems.InternalNodes != null)
                {
                    foreach (var item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        Collection.Add(item, new Point(item.OffsetX, item.OffsetY));
                    }
                }

                if (_mSharedData.Graph.InternalSelectedItems.InternalConnectors != null)
                {
                    foreach (var item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        if (condition == "ByY")
                        {
                            if (item.SourcePoint.Y < item.TargetPoint.Y)
                            {

                                Collection.Add(item, item.SourcePoint);
                            }
                            else
                            {
                                Collection.Add(item, item.TargetPoint);
                            }
                        }
                        else
                        {
                            if (item.SourcePoint.X < item.TargetPoint.X)
                            {

                                Collection.Add(item, item.SourcePoint);
                            }
                            else
                            {
                                Collection.Add(item, item.TargetPoint);
                            }
                        }
                    }
                }

                if (_mSharedData.Graph.InternalSelectedItems.InternalGroups != null)
                {
                    foreach (var item in _mSharedData.Graph.InternalSelectedItems.InternalGroups)
                    {
                        Collection.Add(item, new Point(item.OffsetX, item.OffsetY));
                    }
                }


                var val = Collection.Values.OrderBy(x => x.Y);
                Point value = val.ToList()[0];
            }
            return Collection;
        }

        /// <summary>
        /// Invoked when the SpaceAcross Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnSpaceAcrossCommand(object sender)
        {
            BeginComposite();
            double firstrefervalue = 0;
            double lasrrefervalue = 0;

            Dictionary<IInternalGroupable, Point> SelectedItems = FindFirstItem("ByX");
            firstrefervalue = SelectedItems.OrderBy(y => y.Value.Y).ToList()[0].Value.X;
            lasrrefervalue = SelectedItems.OrderBy(y => y.Value.Y).ToList()[SelectedItems.Count() - 1].Value.X;
            double refervalue = (lasrrefervalue - firstrefervalue) / (SelectedItems.Keys.Count - 1);
            IInternalGroupable firstobj = SelectedItems.OrderBy(y => y.Value.Y).ToList()[0].Key;
            var coll = SelectedItems.Keys.OrderBy(x => SelectedItems[x].Y);
            foreach (IInternalGroupable item in coll)
            {
                if (item != firstobj)
                {
                    if (item is IInternalNode)
                    {
                        (item as IInternalNode).OffsetX = firstrefervalue + (coll.ToList().IndexOf(item) * refervalue);
                    }
                    else if (item is IInternalConnector)
                    {
                        if ((item as IInternalConnector).SourcePoint.X < (item as IInternalConnector).TargetPoint.X)
                        {
                            double oldx = (item as IInternalConnector).SourcePoint.X;
                            (item as IInternalConnector).SourcePoint = new Point((firstrefervalue + coll.ToList().IndexOf(item) * refervalue), (item as IInternalConnector).SourcePoint.Y);

                            double offset = (item as IInternalConnector).SourcePoint.X - oldx;
                            (item as IInternalConnector).TargetPoint = new Point((item as IInternalConnector).TargetPoint.X + offset, (item as IInternalConnector).TargetPoint.Y);

                        }
                        else
                        {
                            double oldx = (item as IInternalConnector).TargetPoint.X;
                            (item as IInternalConnector).TargetPoint = new Point((firstrefervalue + coll.ToList().IndexOf(item) * refervalue), (item as IInternalConnector).TargetPoint.Y);

                            double offset = (item as IInternalConnector).TargetPoint.X - oldx;
                            (item as IInternalConnector).SourcePoint = new Point((item as IInternalConnector).SourcePoint.X + offset, (item as IInternalConnector).SourcePoint.Y);

                        }
                    }
                    //else if (item is IInternalGroup)
                    //{
                    //    (item as IInternalGroup).OffsetX = firstrefervalue + (SelectedItems.Keys.ToList().IndexOf(item) * refervalue);
                    //}
                }
            }
            EndComposite();
        }
        #endregion

        #region Alignment Commands

        /// <summary>
        /// Invoked when the AlignLeft Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignLeftCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                double leftvalue = 0;
                IInternalGroupable FirstSelectedItem = _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem;
                if (FirstSelectedItem != null)
                {
                    leftvalue = FirstSelectedItem.Bounds.Left;

                    foreach (IInternalNode item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        item.OffsetX += leftvalue - item.Bounds.Left;
                    }

                    foreach (IInternalConnector item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        //if (FirstSelectedItem is IInternalConnector && item != FirstSelectedItem)
                        {
                            double x = item.Bounds.Left - leftvalue;
                            item.SourcePoint = new Point(item.SourcePoint.X - x, item.SourcePoint.Y);
                            item.TargetPoint = new Point(item.TargetPoint.X - x, item.TargetPoint.Y);
                        }
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the AlignCenter Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignCenterCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                double leftvalue = 0;
                IInternalGroupable FirstSelectedItem = _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem;
                if (FirstSelectedItem != null)
                {
                    leftvalue = FirstSelectedItem.Center.X;
                    if (FirstSelectedItem is IInternalConnector)
                    {
                        leftvalue = FirstSelectedItem.Bounds.Left + FirstSelectedItem.Bounds.Width / 2;
                    }

                    foreach (IInternalNode item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        if (item != FirstSelectedItem)
                        {
                            item.OffsetX += leftvalue - item.Center.X;
                        }
                    }

                    foreach (IInternalConnector item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        //if (FirstSelectedItem is IInternalConnector && item != FirstSelectedItem)
                        {
                            double x = (item.Bounds.Left + item.Bounds.Right) / 2 - leftvalue;
                            item.SourcePoint = new Point(item.SourcePoint.X - x, item.SourcePoint.Y);
                            item.TargetPoint = new Point(item.TargetPoint.X - x, item.TargetPoint.Y);
                        }
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the AlignRight Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignRightCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                double leftvalue = 0;
                IInternalGroupable FirstSelectedItem = _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem;
                if (FirstSelectedItem != null)
                {
                    leftvalue = FirstSelectedItem.Bounds.Right;

                    foreach (IInternalNode item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        item.OffsetX += leftvalue - item.Bounds.Right;
                    }

                    foreach (IInternalConnector item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        //if (FirstSelectedItem is IInternalConnector && item != FirstSelectedItem)
                        {
                            double x = item.Bounds.Right - leftvalue;
                            item.SourcePoint = new Point(item.SourcePoint.X - x, item.SourcePoint.Y);
                            item.TargetPoint = new Point(item.TargetPoint.X - x, item.TargetPoint.Y);
                        }
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the AlignTop Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignTopCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                double leftvalue = 0;
                IInternalGroupable FirstSelectedItem = _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem;
                if (FirstSelectedItem != null)
                {
                    leftvalue = FirstSelectedItem.Bounds.Top;

                    foreach (IInternalNode item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        item.OffsetY += leftvalue - item.Bounds.Top;
                    }

                    foreach (IInternalConnector item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        //if (FirstSelectedItem is IInternalConnector && item != FirstSelectedItem)
                        {
                            double x = item.Bounds.Top - leftvalue;
                            item.SourcePoint = new Point(item.SourcePoint.X, item.SourcePoint.Y - x);
                            item.TargetPoint = new Point(item.TargetPoint.X, item.TargetPoint.Y - x);
                        }
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the AlignMiddle Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignMiddleCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                double leftvalue = 0;
                IInternalGroupable FirstSelectedItem = _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem;
                if (FirstSelectedItem != null)
                {
                    leftvalue = FirstSelectedItem.Center.Y;
                    if (FirstSelectedItem is IInternalConnector)
                    {
                        leftvalue = FirstSelectedItem.Bounds.Top + FirstSelectedItem.Bounds.Height / 2;
                    }
                    foreach (IInternalNode item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        item.OffsetY += leftvalue - item.Center.Y;
                    }

                    foreach (IInternalConnector item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        // if (FirstSelectedItem is IInternalConnector && item != FirstSelectedItem)
                        {
                            double x = (item.Bounds.Top + item.Bounds.Bottom) / 2 - leftvalue;
                            item.SourcePoint = new Point(item.SourcePoint.X, item.SourcePoint.Y - x);
                            item.TargetPoint = new Point(item.TargetPoint.X, item.TargetPoint.Y - x);
                        }
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the AlignBottom Command is executed.
        /// </summary>
        /// <param name="sender">The DiagramView.</param>        
        public void OnAlignBottomCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                double leftvalue = 0;
                IInternalGroupable FirstSelectedItem = _mSharedData.Graph.InternalSelectedItems.FirstSelectedItem;
                if (FirstSelectedItem != null)
                {
                    leftvalue = FirstSelectedItem.Bounds.Bottom;

                    foreach (IInternalNode item in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
                    {
                        item.OffsetY += leftvalue - item.Bounds.Bottom;
                    }

                    foreach (IInternalConnector item in _mSharedData.Graph.InternalSelectedItems.InternalConnectors)
                    {
                        // if (FirstSelectedItem is IInternalConnector && item != FirstSelectedItem)
                        {
                            double x = item.Bounds.Bottom - leftvalue;
                            item.SourcePoint = new Point(item.SourcePoint.X, item.SourcePoint.Y - x);
                            item.TargetPoint = new Point(item.TargetPoint.X, item.TargetPoint.Y - x);
                        }
                    }
                }
            }
            EndComposite();
        }

        #endregion

        #region Order commands

        /// <summary>
        /// Invoked when the BringToFront Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnBringToFrontCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                IInternalGroupable first =
                    _mSharedData.Graph.InternalSelectedItems.InternalNodes.FirstOrDefault() ??
                    _mSharedData.Graph.InternalSelectedItems.InternalConnectors.FirstOrDefault() ?? (IInternalGroupable)_mSharedData.Graph.InternalSelectedItems.InternalGroups.FirstOrDefault();

                if (first == null)
                    return;

                int pivotIndex = first.ZIndex;
                int maxIndes = -1;

                foreach (var node in _mSharedData.Graph.Page.Children.OfType<IGroupable>())
                {
                    if (node.ZIndex > pivotIndex)
                    {
                        node.ZIndex--;
                    }
                    if (maxIndes < node.ZIndex)
                    {
                        maxIndes = node.ZIndex;
                    }
                }
                first.ZIndex = maxIndes + 1;
                if (first is IInternalGroup)
                {
                    UpdateGroup(first, first.ZIndex);
                }
            }
            EndComposite();
        }

        private void UpdateGroup(IInternalGroupable first, int p)
        {
            BeginComposite();
            if ((first as IInternalGroup).InternalNodes != null)
            {
                foreach (IInternalNode node in (first as IInternalGroup).InternalNodes)
                {
                    node.ZIndex = p;
                }
            }
            if ((first as IInternalGroup).InternalConnectors != null)
            {
                foreach (IInternalConnector con in (first as IInternalGroup).InternalConnectors)
                {
                    con.ZIndex = p;
                }
            }

            if ((first as IInternalGroup).InternalGroups != null)
            {
                foreach (IInternalGroup g in (first as IInternalGroup).InternalGroups)
                {
                    g.ZIndex = p;
                    UpdateGroup(g, p);
                }
            }
            EndComposite();
        }

        private bool overlap(Rect r, Rect compare)
        {
            if (r.IsIntersect(compare))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Invoked when the SendToBack Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnSendToBackCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                IInternalGroupable First =
                     _mSharedData.Graph.InternalSelectedItems.InternalNodes.FirstOrDefault() ??
                     _mSharedData.Graph.InternalSelectedItems.InternalConnectors.FirstOrDefault() ?? (IInternalGroupable)_mSharedData.Graph.InternalSelectedItems.InternalGroups.FirstOrDefault();

                if (First == null)
                {
                    return;
                }

                int pivotIndex = First.ZIndex;
                int minIndes = int.MaxValue;

                foreach (var node in _mSharedData.Graph.Page.Children.OfType<IGroupable>())
                {
                    if (node.ZIndex < pivotIndex)
                    {
                        node.ZIndex--;
                    }
                    if (minIndes > node.ZIndex)
                    {
                        minIndes = node.ZIndex;
                    }
                }
                First.ZIndex = minIndes - 1;
                if (First is IInternalGroup)
                {
                    UpdateGroup(First, First.ZIndex);
                }
            }
            EndComposite();
            //List<UIElement> ordered = (from UIElement item in _mSharedData.Graph.Page.Children.OfType<IGroupable>()
            //                           orderby Canvas.GetZIndex(item as UIElement)
            //                           select item as UIElement).ToList();

            //List<int> list = new List<int>();


            //var First = _mSharedData.Graph.InternalSelectedItems.InternalNodes.First();

            //foreach (var ver in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
            //{
            //    if (ver != First)
            //    {
            //        bool val = overlap(First.OffsetX, First.OffsetY, First.Width, First.Height, ver.OffsetX, ver.OffsetY, ver.Width, ver.Height);

            //        if (val == true)
            //        { 
            //            list.Add(ver.ZIndex);
            //            int Zindex = ver.ZIndex;
            //            First.ZIndex = Zindex;
            //        }
            //    }
            //}

            //foreach (var front in _mSharedData.Graph.InternalSelectedItems.InternalNodes)
            //{
            //    if (front != First)
            //    {
            //        int Zindex = front.ZIndex;
            //        front.ZIndex = Zindex;
            //    }
            //}
        }

        /// <summary>
        /// Invoked when the MoveForward Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnBringForwardCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                IInternalGroupable selectednode;
                Dictionary<IInternalGroupable, int> indexvalue = CommonLogicforOrderCommands(out selectednode);
                List<int> greateritems = new List<int>();
                if (selectednode != null)
                {
                    int temp = selectednode.ZIndex;
                    if (indexvalue.Count > 0)
                    {
                        foreach (int val in indexvalue.Values)
                        {
                            if (val > temp)
                            {
                                greateritems.Add(val);
                            }
                        }
                        if (greateritems.Count != 0)
                        {
                            selectednode.ZIndex = greateritems.Min();
                            IInternalGroupable g = indexvalue.FirstOrDefault(x => x.Value == selectednode.ZIndex).Key;
                            g.ZIndex = temp;
                            if (selectednode is IInternalGroup)
                            {
                                UpdateGroup((selectednode as IInternalGroup), (selectednode as IInternalGroup).ZIndex);
                            }
                        }
                    }
                }
            }
            EndComposite();
        }

        /// <summary>
        /// Invoked when the SendBackward Command is executed.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>      
        public void OnSendBackwardCommand(object sender)
        {
            BeginComposite();
            if (_mSharedData.Graph.InternalSelectedItems != null)
            {
                IInternalGroupable selectednode;
                Dictionary<IInternalGroupable, int> indexvalue = CommonLogicforOrderCommands(out selectednode);
                List<int> lesseritems = new List<int>();
                if (selectednode != null)
                {
                    int temp = selectednode.ZIndex;
                    if (indexvalue.Count > 0)
                    {
                        foreach (int val in indexvalue.Values)
                        {
                            if (val < temp)
                            {
                                lesseritems.Add(val);
                            }
                        }
                        if (lesseritems.Count != 0)
                        {
                            selectednode.ZIndex = lesseritems.Max();
                            IInternalGroupable g = indexvalue.FirstOrDefault(x => x.Value == selectednode.ZIndex).Key;
                            g.ZIndex = temp;
                            if (selectednode is IInternalGroup)
                            {
                                UpdateGroup((selectednode as IInternalGroup), (selectednode as IInternalGroup).ZIndex);
                            }
                        }
                    }
                }
            }
            EndComposite();
        }

        private Dictionary<IInternalGroupable, int> CommonLogicforOrderCommands(out IInternalGroupable selectednode)
        {
            List<IInternalGroupable> ordered = new List<IInternalGroupable>();

            //foreach (var node in _mSharedData.Graph.InternalNodes)
            //{
            //    ordered.Add(node);
            //}
#if WINRT
            ordered.AddRange(_mSharedData.Graph.InternalNodes);
            ordered.AddRange(_mSharedData.Graph.InternalConnectors);
            ordered.AddRange(_mSharedData.Graph.InternalGroups);
#endif

            Dictionary<IInternalGroupable, int> zindex = new Dictionary<IInternalGroupable, int>();
            IInternalGroupable First = _mSharedData.Graph.InternalSelectedItems.InternalNodes.FirstOrDefault() ??
                _mSharedData.Graph.InternalSelectedItems.InternalConnectors.FirstOrDefault() ??
                (IInternalGroupable)_mSharedData.Graph.InternalSelectedItems.InternalGroups.FirstOrDefault();
            bool val = false;
            selectednode = First;
            ordered.Remove(selectednode);
            foreach (var item in ordered)
            {
                val = overlap(First.Bounds, item.Bounds);
                if (val)
                {
                    zindex.Add(item, item.ZIndex);
                }

            }
            return zindex;
        }

        #endregion

        public void Dispose()
        {
        }

        public bool CanUndoExecute(object param)
        {
            if (_mSharedData.UndoRedoController != null)
            {
                return _mSharedData.UndoRedoController.CanUndo();
            }
            return false;
        }

        public bool CanZoomExecute(object param)
        {
            return true;
        }

        public bool CanResetExecute(object param)
        {
            //if (_mSharedData.ScrollViewer != null &&
            //    _mSharedData.ScrollViewer.HorizontalOffset == 0 &&
            //   _mSharedData.ScrollViewer.VerticalOffset == 0 &&
            //   _mSharedData.ScrollViewer.CurrentZoom == 1)
            //{
            //    return false;
            //}
            return true;
        }

        public bool CanSelectAllExecute(object param)
        {
            //if ((_mSharedData.Graph.InternalNodes != null && _mSharedData.Graph.InternalNodes.Count > 0) ||
            //    (_mSharedData.Graph.InternalConnectors != null && _mSharedData.Graph.InternalConnectors.Count > 0) ||
            //    (_mSharedData.Graph.InternalGroups != null && _mSharedData.Graph.InternalGroups.Count > 0))
            //{
            //    return true;
            //}
            //return false;
            return true;
        }

        public bool CanRedoExecute(object param)
        {
            if (_mSharedData.UndoRedoController != null)
            {
                return _mSharedData.UndoRedoController.CanRedo();
            }
            return false;
        }

        public bool CanGroupExecute(object param)
        {
            return nodeSelectionGreaterThan(1);
        }

        private bool selectionGreaterThan(int cnt)
        {
            if (_mSharedData.Graph.InternalSelectedItems != null &&
               ((_mSharedData.Graph.InternalSelectedItems.InternalNodes != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalNodes.Count > cnt) ||
               (_mSharedData.Graph.InternalSelectedItems.InternalConnectors != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalConnectors.Count > cnt) ||
               (_mSharedData.Graph.InternalSelectedItems.InternalGroups != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalGroups.Count > cnt)))
            {
                return true;
            }
            return false;
        }

        private bool nodeSelectionGreaterThan(int cnt)
        {
            if (_mSharedData.Graph.InternalSelectedItems != null &&
               (_mSharedData.Graph.InternalSelectedItems.InternalNodes != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalNodes.Count > cnt))
            {
                return true;
            }
            return false;
        }

        private bool groupSelectionGreaterThan(int cnt)
        {
            if (_mSharedData.Graph.InternalSelectedItems != null &&
               (_mSharedData.Graph.InternalSelectedItems.InternalGroups != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalGroups.Count > cnt))
            {
                return true;
            }
            return false;
        }

        private bool nodeGroupSelectionGreaterThan(int cnt)
        {
            if (_mSharedData.Graph.InternalSelectedItems != null &&
               ((_mSharedData.Graph.InternalSelectedItems.InternalNodes != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalNodes.Count > cnt) ||
               (_mSharedData.Graph.InternalSelectedItems.InternalGroups != null &&
               _mSharedData.Graph.InternalSelectedItems.InternalGroups.Count > cnt)))
            {
                return true;
            }
            return false;
        }

        public bool CanUnGroupExecute(object param)
        {
            return groupSelectionGreaterThan(0);
        }

        public bool CanSameSizeExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanSameHeightExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanSameWidthExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanAlignBottomExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanAlignTopExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanAlignLeftExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanAlignCenterExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanAlignRightExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanAlignMiddleExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanSpaceAcrossExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanSpaceDownExecute(object param)
        {
            return nodeGroupSelectionGreaterThan(1);
        }

        public bool CanSendToBackExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanSendBackwardExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanBringToFrontExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanBringForwardExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanMoveDownExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanMoveUpExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanMoveLeftExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanMoveRightExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanCutExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanCopyExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanPasteExecute(object param)
        {
            return true;
        }

        public bool CanDeleteExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanDrawExecute(object param)
        {
            return true;
        }

        public bool CanFlipExecute(object param)
        {
            return selectionGreaterThan(0);
        }

        public bool CanFitToPageExecute(object param)
        {
            return true;
        }

        public bool CanDuplicateExecute(object param)
        {
            return selectionGreaterThan(0);
        }
    }

    public interface IZoomParameter
    {
    }

    /// <summary>
    /// Parameter for executing zoom manually
    /// </summary>
    public interface IZoomPositionParameter : IZoomParameter
    {
        /// <summary>
        /// Zoom to a particular scale value.
        /// </summary>
        double? ZoomTo { get; set; }

        /// <summary>
        /// Percentage of scale value for each ZoomIn or ZoomOut functionality
        /// </summary>
        double? ZoomFactor { get; set; }

        /// <summary>
        /// Point of foucus while zooming. Usually used to specify a particular point in the content.
        /// </summary>
        Point? FocusPoint { get; set; }

        ZoomCommand ZoomCommand { get; set; }
    }

    public interface IZoomPointerParameter : IZoomParameter
    {
        MouseButtonEventArgs PointerArgs { get; set; }

        ZoomCommand ZoomCommand { get; set; }
    }

    public interface IZoomManipulationParameter : IZoomParameter
    {
        ManipulationDeltaRoutedEventArgs ManipulationArgs { get; set; }
    }

    /// <summary>
    /// Manipulation Parameters for executing zoom manually
    /// </summary>
    public class ZoomManipulationParamenter : IZoomManipulationParameter
    {
        public ManipulationDeltaRoutedEventArgs ManipulationArgs { get; set; }
    }

    /// <summary>
    /// Pointer Parameters for executing zoom manually
    /// </summary>
    public class ZoomPointerParamenter : IZoomPointerParameter
    {
        /// <summary>
        /// Point of foucus while zooming. Usually used to specify a particular point in the content.
        /// Note: Focus point for zooming can also be defined using FocusPoint property.
        /// </summary>
        public MouseButtonEventArgs PointerArgs { get; set; }

        public ZoomCommand ZoomCommand { get; set; }
    }

    /// <summary>
    /// Position Parameters for executing zoom manually
    /// </summary>
    public class ZoomPositionParamenter : IZoomPositionParameter
    {
        /// <summary>
        /// Zoom to a particular scale value.
        /// </summary>
        public double? ZoomTo { get; set; }

        /// <summary>
        /// Percentage of scale value for each ZoomIn or ZoomOut functionality
        /// </summary>
        public double? ZoomFactor { get; set; }

        /// <summary>
        /// Point of foucus while zooming. Usually used to specify a particular point in the content.
        /// Note: Focus point for zooming can also be defined using Pointer property.
        /// </summary>
        public Point? FocusPoint { get; set; }

        public ZoomCommand ZoomCommand { get; set; }

    }

    public interface IReset
    {
        Reset Reset { get; set; }
    }

    /// <summary>
    /// Parameters to execute Reset Command
    /// </summary>
    public class ResetParameter : IReset
    {
        private Reset _mZoomPanReset;

        /// <summary>
        /// Gets or sets ZoomPanReset
        /// </summary>
        public Reset Reset
        {
            get { return _mZoomPanReset; }
            set { _mZoomPanReset = value; }
        }

    }

    public interface IFitToPage
    {
        FitToPage FitToPage { get; set; }

        Thickness Margin { get; set; }
    }

    /// <summary>
    /// Parameters to execute FitToPageCommand
    /// </summary>
    public class FitToPageParameter : IFitToPage
    {
        private FitToPage _fitToPage;

        /// <summary>
        /// Gets or sets FitToPage
        /// </summary>
        public FitToPage FitToPage
        {
            get { return _fitToPage; }
            set { _fitToPage = value; }
        }

        private Thickness _margin;

        /// <summary>
        /// Gets or sets the Margin
        /// </summary>
        public Thickness Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }

    }

    public interface IFlip
    {
        Flip Flip { get; set; }
    }

    /// <summary>
    /// Parameters to execute Flip Command
    /// </summary>
    public class FlipParameter : IFlip
    {
        private Flip _flip;

        /// <summary>
        /// Gets or sets Flip
        /// </summary>
        public Flip Flip
        {
            get { return _flip; }
            set { _flip = value; }
        }


    }

    public interface IDuplicateParameter
    {
        bool DragClone { get; set; }

        DiagramThumb Thumb { get; set; }

        MouseEventArgs PointerArgs { get; set; }
    }


    public class DupilicateParameter : IDuplicateParameter
    {
        private bool _mDragClone;

        public bool DragClone
        {
            get { return _mDragClone; }
            set { _mDragClone = value; }
        }

        private DiagramThumb _pressedThumb;

        public DiagramThumb Thumb
        {
            get { return _pressedThumb; }
            set { _pressedThumb = value; }
        }

        private MouseEventArgs _startingArgs;

        public MouseEventArgs PointerArgs
        {
            get { return _startingArgs; }
            set { _startingArgs = value; }
        }

    }
}
