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
using System.Globalization;

#if !WinRT

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Controls.Grid
#else

using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.ComponentModel;
using Windows.UI.Core;
using Windows.System;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellBoundWrapper : DependencyObject
    {
        public static readonly DependencyProperty CellBoundValueProperty = DependencyProperty.Register(
            "CellBoundValue",
            typeof(object),
            typeof(GridCellBoundWrapper),
#if!WinRT
            new PropertyMetadata(OnValueChanged));
#else
 new PropertyMetadata(null, OnValueChanged));
#endif

        /// <summary>
        /// Gets or sets the CellBoundValue. This is a DependencyProperty exposed for the
        /// underlying object data source. The object can be binded to an external WPF
        /// control's DependencyProperty in Two-way mode. The changes reflected in the
        /// object would be passed on to the underlying object accordingly.
        /// <para></para>
        /// <para></para>
        /// <code>    &lt;syncfusion:GridDataVisibleColumn MappingName=&quot;LastName&quot; HeaderText=&quot;LastName&quot;
        /// Width=&quot;90&quot; &gt;
        ///                         &lt;syncfusion:GridDataVisibleColumn.CellTemplate&gt;
        ///                             &lt;DataTemplate&gt;
        ///                                 &lt;TextBox Text=&quot;{Binding
        /// Path=CellBoundValue, Mode=TwoWay}&quot;
        ///                                        Foreground=&quot;Black&quot;
        /// syncfusion:VisualContainer.WantsMouseInput=&quot;True&quot;/&gt;
        ///                             &lt;/DataTemplate&gt;
        ///                         &lt;/syncfusion:GridDataVisibleColumn.CellTemplate&gt;
        ///                     &lt;/syncfusion:GridDataVisibleColumn&gt;</code>
        /// </summary>
        public object CellBoundValue
        {
            get
            {
                return this.GetValue(GridCellBoundWrapper.CellBoundValueProperty);
            }
            set
            {
                this.SetValue(GridCellBoundWrapper.CellBoundValueProperty, value);
            }
        }

        public GridStyleInfo Style
        {
            get;
            internal set;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var boundObject = d as GridCellBoundWrapper;
            if (boundObject != null)
            {
                if (args.NewValue != args.OldValue)
                {
                    boundObject.RaiseOnValueChanged(args.NewValue);
                }
            }
        }

        private void RaiseOnValueChanged(object value)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new GridDataValueEventArgs<object>(value));
            }
        }

        public event EventHandler<GridDataValueEventArgs<object>> ValueChanged;
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellDataTemplateCellModel : GridCellModel<GridCellDataTemplateRenderer>
    {
        public GridCellDataTemplateCellModel()
        {
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellDataTemplateRenderer : GridVirtualizingCellRenderer<GridCell>
    {
        public GridCellDataTemplateRenderer()
        {
            this.SupportsRenderOptimization = false;
            this.AllowRecycle = true;
            this.IsControlTextShown = false;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override GridCell CreateUIElement(Cells.ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            var uiElement = base.CreateUIElement(aca, style) as GridCell;
            uiElement.RowColumnIndex = aca.CellRowColumnIndex;
            uiElement.GridControl = style.GridControl;

            bool found = false;
            DataTemplate dt = null;
            if (style.CellItemTemplateKey != null && style.CellItemTemplateKey != string.Empty)
            {
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);

                if (Application.Current != null && dt == null)
#if WinRT
                    if (Application.Current.Resources.ContainsKey(style.CellItemTemplateKey))
#else
                    if (Application.Current.Resources.Contains(style.CellItemTemplateKey))
#endif
                        dt = Application.Current.Resources[style.CellItemTemplateKey] as DataTemplate;

                found = dt != null;
            }
            else if (style.CellItemTemplate != null)
            {
                dt = style.CellItemTemplate;
                found = true;
            }

            if (found)
            {
                this.OnUnwireUIElement(uiElement);
                uiElement.ContentTemplate = dt;
                GridCellBoundWrapper wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
                if (wrapperInstance == null)
                {
                    wrapperInstance = this.CreateWrapperInstance();
                }
                wrapperInstance.CellBoundValue = style.CellValue;
                wrapperInstance.Style = style;
                this.ProvideWrapperInstance(wrapperInstance, style);
                uiElement.DataContext = wrapperInstance;
                GridControlBase.SetDelayLoad(uiElement, false);
                this.OnWireUIElement(uiElement);
            }
            return uiElement;
        }

        protected override UIElement CreateRendererElement(Cells.ArrangeCellArgs aca, GridRenderStyleInfo cellInfo)
        {
            return base.CreateRendererElement(aca, cellInfo);
        }

        public override void OnInitializeContent(GridCell uiElement, GridRenderStyleInfo style)
        {
            uiElement.RowColumnIndex = style.CellRowColumnIndex;
            uiElement.GridControl = style.GridControl;
            bool found = false;
            DataTemplate dt = null;

            if (CurrentCell.IsEditing && CurrentCell.CellRowColumnIndex == style.CellRowColumnIndex)
            {
                if (style.CellEditTemplateKey != null && style.CellEditTemplateKey != string.Empty)
                {
                    dt = (DataTemplate)style.GridControl.TryFindResource(style.CellEditTemplateKey);

                    if (Application.Current != null && dt == null)
#if WinRT
                        if (Application.Current.Resources.ContainsKey(style.CellEditTemplateKey))
#else
                        if (Application.Current.Resources.Contains(style.CellEditTemplateKey))
#endif
                            dt = Application.Current.Resources[style.CellEditTemplateKey] as DataTemplate;
                    found = dt != null;
                }
                else if (style.CellEditTemplate != null)
                {
                    dt = style.CellEditTemplate;
                    found = true;
                }
            }

            if (!found)
            {
                if (style.CellItemTemplateKey != null && style.CellItemTemplateKey != string.Empty)
                {
                    dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);

                    if (Application.Current != null && dt == null)
#if WinRT
                        if (Application.Current.Resources.ContainsKey(style.CellItemTemplateKey))
#else
                        if (Application.Current.Resources.Contains(style.CellItemTemplateKey))
#endif
                            dt = Application.Current.Resources[style.CellItemTemplateKey] as DataTemplate;

                    found = dt != null;
                }
                else if (style.CellItemTemplate != null)
                {
                    dt = style.CellItemTemplate;
                    found = true;
                }
            }

            if (found)
            {
                this.OnUnwireUIElement(uiElement);

                uiElement.ContentTemplate = dt;
                GridCellBoundWrapper wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
                if (wrapperInstance == null)
                {
                    wrapperInstance = this.CreateWrapperInstance();
                }
                //wrapperInstance.CellBoundValue = null;
                //uiElement.ContentTemplate = dt;
                wrapperInstance.CellBoundValue = style.CellValue;
                wrapperInstance.Style = style;
                this.ProvideWrapperInstance(wrapperInstance, style);
                uiElement.DataContext = wrapperInstance;
                //uiElement.SetBinding(GridCell.DataContextProperty, new System.Windows.Data.Binding());
                //uiElement.InvalidateArrange();
                GridControlBase.SetDelayLoad(uiElement, false);

                this.OnWireUIElement(uiElement);
            }

        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !CurrentCell.IsEditing
                && GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                CurrentCell.BeginEdit(true);
            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        protected virtual GridCellBoundWrapper CreateWrapperInstance()
        {
            return new GridCellBoundWrapper();
        }

        protected virtual void ProvideWrapperInstance(GridCellBoundWrapper wrapperInstance, GridRenderStyleInfo style)
        {
        }

        protected override void OnWireUIElement(GridCell uiElement)
        {
            base.OnWireUIElement(uiElement);
            GridCellBoundWrapper wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
            if (wrapperInstance != null)
            {
                wrapperInstance.ValueChanged += new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
            }
            uiElement.HookEvents();
        }

        //private bool ProcessTabKey()
        //{
        //    var control = ((Control)this.GridControl.FocusedElement);
        //    if (control != null && this.CurrentCellUIElement != null)
        //    {
        //        var allChildren = CurrentCellUIElement.FindElementsOfType<Control>();
        //        var nextControl = allChildren.FirstOrDefault(c => c != null && c.TabIndex == control.TabIndex + 1);
        //        if (nextControl != default(Control))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        // private bool isEnteryKeyPressed = false;
#if WinRT
        public override bool ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            bool isControlKey=false;
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control);
            if (ctrl == CoreVirtualKeyStates.Down)
                isControlKey = true;

            if (isControlKey)
                return true;

            switch (e.Key)
            {
                case VirtualKey.Enter:
                    //this.isEnteryKeyPressed = true;
                    //this.CurrentCell.MoveDown();
                    return false;
                case VirtualKey.Right:
                case VirtualKey.Left:
                case VirtualKey.Down:
                case VirtualKey.Up:
                    {
                        CurrentCell.EndEdit();
                        return !CurrentCell.IsEditing;
                    }

                case VirtualKey.End:
                    return true;
                case VirtualKey.Home:
                    {
                        CurrentCell.BeginEdit(true);
                        return false;
                    }

                case VirtualKey.Delete:
                    {
                        if (this.CurrentStyle != null)
                        {
                            var renderer = this.CurrentCell.Renderer;
                            if (renderer != null)
                            {
                                CurrentCell.BeginEdit(true);
                                SetControlText("");
                                e.Handled = true;
                            }
                        }

                        return false;
                    }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#else
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isControlKey = false;
            isControlKey = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
             if (isControlKey)
                return true;
             switch (e.Key)
             {
                 case Key.Enter:
                     // this.isEnteryKeyPressed = true;
                     this.CurrentCell.MoveDown();
                     return false;
                 case Key.Tab:
                 //return this.ProcessTabKey();
                 case Key.Right:
                 case Key.Left:
                 case Key.Down:
                 case Key.Up:
                     {
                         CurrentCell.EndEdit();
                         return !CurrentCell.IsEditing;
                     }

                 case Key.End:
                     return true;
                 case Key.Home:
                     {
                         CurrentCell.BeginEdit(true);
                         return false;
                     }

                 case Key.Delete:
                     {
                         if (this.CurrentStyle != null)
                         {
                             var renderer = this.CurrentCell.Renderer;
                             if (renderer != null)
                             {
                                 GridDataStyleInfo sif = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                                 if (sif != null && sif.CellIdentity.TableCellType == GridDataTableCellType.RecordCell)
                                 {
                                     CurrentCell.BeginEdit(true);
                                     SetControlText("");
                                     e.Handled = true;
                                 }
                                 else
                                 {
                                     return false;
                                 }
                             }
                         }

                         return false;
                     }
             }     
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif
        protected override void OnUnwireUIElement(GridCell uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            GridCellBoundWrapper wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
            if (wrapperInstance != null)
            {
                wrapperInstance.ValueChanged -= new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
            }
            uiElement.UnHookEvents();
            uiElement.DataContext = null;
        }

        internal override void UnRegisterUIElement(GridCell uiElement)
        {
            if (uiElement != null)
                uiElement.DataContext = null;
        }

        void wrapperInstance_ValueChanged(object sender, GridDataValueEventArgs<object> e)
        {
            if (!this.IsInArrange && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlValue(e.Value))
                {
                    RefreshContent();
                }
            }
        }

        protected override object GetControlValueFromEditorCore(GridCell uiElement)
        {
            var wrapperInstance = uiElement.DataContext as GridCellBoundWrapper;
            if (wrapperInstance != null)
            {
                return wrapperInstance.CellBoundValue;
            }

            return base.GetControlValueFromEditorCore(uiElement);
        }

        protected override string GetControlTextFromEditorCore(GridCell uiElement)
        {
            return CurrentStyle.CellValue.ToString();
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
    public class GridDataValueEventArgs<T> : SyncfusionHandledEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataValueEventArgs&lt;T&gt;"/> class.
        /// </summary>
        public GridDataValueEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataValueEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public GridDataValueEventArgs(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get;
            internal set;
        }
    }
#endif
}
