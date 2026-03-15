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

namespace Syncfusion.Windows.Controls.Grid
{
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
    using Syncfusion.Windows;
    using Syncfusion.Windows.Controls.Grid;

    public class GridDataCellBoundWrapper : DependencyObject
    {
        public static readonly DependencyProperty CellBoundValueProperty = DependencyProperty.Register(
            "CellBoundValue",
            typeof(object),
            typeof(GridDataCellBoundWrapper),
            new PropertyMetadata(OnValueChanged));

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
                return this.GetValue(GridDataCellBoundWrapper.CellBoundValueProperty);
            }
            set
            {
                this.SetValue(GridDataCellBoundWrapper.CellBoundValueProperty, value);
            }
        }

        public GridStyleInfo Style
        {
            get;
            internal set;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var boundObject = d as GridDataCellBoundWrapper;
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

        public object Record
        {
            get;
            internal set;
        }
    }

    public class GridDataDataBoundTemplateCellBoundModel : GridCellModel<GridDataDataBoundTemplateCellRenderer>
    {
        public GridDataDataBoundTemplateCellBoundModel()
        {
        }
    }

    public class GridDataDataBoundTemplateCellRenderer : GridVirtualizingCellRenderer<Border>
    {
        public GridDataDataBoundTemplateCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = false;
            this.AllowKeepAliveOnlyCurrentCell = true;
        }

        protected override UIElement CreateRendererElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            bool found = false;
            DataTemplate dt = null;
            if (style.CellItemTemplateKey != null && style.CellItemTemplateKey != string.Empty)
            {
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellItemTemplateKey);

                if (Application.Current != null && dt == null)
                    if (Application.Current.Resources.Contains(style.CellItemTemplateKey))
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
                var frameWorkelement = dt.LoadContent() as FrameworkElement;
                // frameWorkelement.Measure(new Size(double.MaxValue, double.MaxValue));
                GridDataCellBoundWrapper wrapperInstance = frameWorkelement.DataContext as GridDataCellBoundWrapper;
                if (wrapperInstance == null)
                {
                    wrapperInstance = this.CreateWrapperInstance();
                }
                wrapperInstance.CellBoundValue = style.CellValue;
                wrapperInstance.Style = style;
                var styleInfo = style.ModelStyle as GridDataStyleInfo;
                wrapperInstance.Record = styleInfo.CellIdentity.RecordEntry;
                frameWorkelement.DataContext = wrapperInstance;
                GridControlBase.SetDelayLoad(frameWorkelement, true);
                return frameWorkelement;
            }
            return null;
        }

        protected override void OnInitializeRendererElement(UIElement rendererElement, GridRenderStyleInfo style)
        {
            // we simply take the recycled item and reset the DataContext here
            // this would improve the recycling logic
            var frameworkEl = rendererElement as FrameworkElement;
            if (frameworkEl != null)
            {
                GridDataCellBoundWrapper wrapperInstance = frameworkEl.DataContext as GridDataCellBoundWrapper;
                wrapperInstance.CellBoundValue = style.CellValue;
                wrapperInstance.Style = style;
                var styleInfo = style.ModelStyle as GridDataStyleInfo;
                wrapperInstance.Record = styleInfo.CellIdentity.RecordEntry;
            }
        }

        public override void OnInitializeContent(Border uiElement, GridRenderStyleInfo style)
        {
            bool found = false;
            DataTemplate dt = null;
            if (style.CellEditTemplateKey != null && style.CellEditTemplateKey != string.Empty)
            {
                dt = (DataTemplate)style.GridControl.TryFindResource(style.CellEditTemplateKey);

                if (Application.Current != null && dt == null)
                    if (Application.Current.Resources.Contains(style.CellItemTemplateKey))
                        dt = Application.Current.Resources[style.CellItemTemplateKey] as DataTemplate;

                found = dt != null;
            }
            else if (style.CellEditTemplate != null)
            {
                dt = style.CellEditTemplate;
                found = true;
            }

            if (found)
            {
                //uiElement.ContentTemplate = dt;
                this.OnUnwireUIElement(uiElement);
                var frameWorkelement = dt.LoadContent() as FrameworkElement;
                frameWorkelement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                uiElement.Child = frameWorkelement;
                GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
                if (wrapperInstance == null)
                {
                    wrapperInstance = this.CreateWrapperInstance();
                }
                wrapperInstance.CellBoundValue = style.CellValue;
                wrapperInstance.Style = style;
                this.ProvideWrapperInstance(wrapperInstance, style);
                uiElement.DataContext = wrapperInstance;
                GridControlBase.SetDelayLoad(uiElement, true);
                this.OnWireUIElement(uiElement);
            }

        }

        protected virtual GridDataCellBoundWrapper CreateWrapperInstance()
        {
            return new GridDataCellBoundWrapper();
        }

        protected virtual void ProvideWrapperInstance(GridDataCellBoundWrapper wrapperInstance, GridRenderStyleInfo style)
        {
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var styleInfo = style.ModelStyle as GridDataStyleInfo;
                wrapperInstance.Record = styleInfo.CellIdentity.RecordEntry;
                //var recordIdx = styleInfo.CellIdentity.Record;
                //if (recordIdx > -1 && recordIdx < this.TableModel.SourceListCount)
                //{
                //    var record = this.TableModel.View.Records[recordIdx];
                //    wrapperInstance.Record = record;
                //}
            }
        }

        protected override void OnWireUIElement(Border uiElement)
        {
            base.OnWireUIElement(uiElement);
            GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
            if (wrapperInstance != null)
            {
                wrapperInstance.ValueChanged += new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
            }
        }

        protected override void OnUnwireUIElement(Border uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
            if (wrapperInstance != null)
            {
                wrapperInstance.ValueChanged += new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
            }
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

        protected override object GetControlValueFromEditorCore(Border uiElement)
        {
            var wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
            if (wrapperInstance != null)
            {
                return wrapperInstance.CellBoundValue;
            }

            return base.GetControlValueFromEditorCore(uiElement);
        }

        protected override string GetControlTextFromEditorCore(Border uiElement)
        {
            return CurrentStyle.CellValue.ToString();
        }
    }
}
