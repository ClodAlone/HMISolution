#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Syncfusion.Windows.Data;
    using System.Diagnostics;


    public class GridDataCellBoundWrapper : GridCellBoundWrapper
    {
        public object Record
        {
            get;
            internal set;
        }
    }


    public class GridDataDataBoundTemplateCellBoundModel : GridCellModel<GridDataDataTemplateCellRenderer>
    {
        public GridDataDataBoundTemplateCellBoundModel()
        {
        }
    }


    public class GridDataDataTemplateCellRenderer : GridCellDataTemplateRenderer
    {
        public GridDataDataTemplateCellRenderer()
            : base()
        {

        }

        protected override GridCellBoundWrapper CreateWrapperInstance()
        {
            return new GridDataCellBoundWrapper();
        }


        protected override void ProvideWrapperInstance(GridCellBoundWrapper wrapperInstance, GridRenderStyleInfo style)
        {
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var styleInfo = style.ModelStyle as GridDataStyleInfo;
                if (this.TableModel != null)
                {
                    ((GridDataCellBoundWrapper)wrapperInstance).Record = styleInfo.CellIdentity.RecordEntry;
                }
            }
        }

        protected GridDataTableModel TableModel
        {
            get
            {
                var tableModel = this.GridControl.Model as GridDataTableModel;
                return tableModel;
            }
        }

        protected override void OnUnwireUIElement(GridCell uiElement)
        {            
            //base.OnUnwireUIElement(uiElement);
            GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
            if (wrapperInstance != null)
            {
                wrapperInstance.ValueChanged -= new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
            }
            uiElement.UnHookEvents();
            //uiElement.DataContext = null;
        }

        protected override void OnEnteredEditMode()
        {
            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                var text = GetControlText(style);
                if (this.ControlValue != null)
                {
                    if (!(this.ControlValue.ToString().Equals(text)))
                        this.ControlValue = text;
                }
                else if (text != null)
                    this.ControlValue = text;
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

        protected override void OnWireUIElement(GridCell uiElement)
        {            
            //base.OnWireUIElement(uiElement);
            GridDataCellBoundWrapper wrapperInstance = uiElement.DataContext as GridDataCellBoundWrapper;
            if (wrapperInstance != null)
            {
                wrapperInstance.ValueChanged += new EventHandler<GridDataValueEventArgs<object>>(wrapperInstance_ValueChanged);
            }
            uiElement.HookEvents();

        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            base.RaiseGridCellClick(rowIndex, colIndex, e);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        private void UpdateDataTemplate()
        {            
            if(CurrentCellUIElement==null)
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
                GridControl.InvalidateVisual(true);
            }
        }
        protected override void OnDeactivated()
        {
            UpdateDataTemplate();            
            base.OnDeactivated();
        }

        protected override void OnEditingComplete()
        {
            UpdateDataTemplate();
            base.OnEditingComplete();

            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                var value = GetControlValue(style);
                if (value != null)
                {
                    if (!(this.ControlValue.Equals((object)value)))
                        this.ControlValue = value;
                }
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

        public void wrapperInstance_ValueChanged(object sender, GridDataValueEventArgs<object> e)
        {

            
            
            if (!this.IsInArrange && !this.CurrentCell.IsInEndEdit)
            {
                var wrapper = sender as GridDataCellBoundWrapper;
                Debug.WriteLine("Cellboundvalue " + wrapper.CellBoundValue);
                Debug.WriteLine("ControlValue" + e.Value.ToString());
                //Debug.WriteLine(this.CurrentStyle.CellValue);
                //this.CurrentStyle.CellValue = e.Value;
                if (!this.SetControlValue(e.Value))
                {
                    RefreshContent();
                }
                //Debug.WriteLine("After" + e.Value.ToString());
                //Debug.WriteLine(this.CurrentStyle.CellValue);
                //this.GridControl.Model.InvalidateCell(this.CellRowColumnIndex);
            }
        }
    }
}
