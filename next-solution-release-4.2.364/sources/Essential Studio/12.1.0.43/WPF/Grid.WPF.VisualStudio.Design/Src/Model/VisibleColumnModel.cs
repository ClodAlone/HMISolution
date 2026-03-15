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
using System.ComponentModel;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Cells;
using System.Collections.Specialized;

namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    public class VisibleColumnModel : INotifyPropertyChanged
    {
        private ModelItem visibleColumn;

        public string MappingNameType
        {
            get;
            set;
        }

        public CellTypes SpecialCellType
        {
            get;
            set;
        }

        public bool AllowDrag
        {
            get
            {
                return (bool)visibleColumn.Properties["AllowDrag"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["AllowDrag"].SetValue(value);
                RaisePropertyChanged("AllowDrag");
            }
        }

        public bool AllowFilter
        {
            get
            {
                return (bool)visibleColumn.Properties["AllowFilter"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["AllowFilter"].SetValue(value);
                RaisePropertyChanged("AllowFilter");
            }
        }

        public bool AllowGroup
        {
            get
            {
                return (bool)visibleColumn.Properties["AllowGroup"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["AllowGroup"].SetValue(value);
                RaisePropertyChanged("AllowGroup");
            }
        }

        public bool AllowResize
        {
            get
            {
                return (bool)visibleColumn.Properties["AllowResize"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["AllowResize"].SetValue(value);
                RaisePropertyChanged("AllowResize");
            }
        }

        public bool AllowSort
        {
            get
            {
                return (bool)visibleColumn.Properties["AllowSort"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["AllowSort"].SetValue(value);
                RaisePropertyChanged("AllowSort");
            }
        }

        public bool AutoFit
        {
            get
            {
                return (bool)visibleColumn.Properties["AutoFit"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["AutoFit"].SetValue(value);
                RaisePropertyChanged("AutoFit");
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return (bool)visibleColumn.Properties["IsReadOnly"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["IsReadOnly"].SetValue(value);
                RaisePropertyChanged("IsReadOnly");
            }
        }


        public bool ShowColumnOptions
        {
            get
            {
                return (bool)visibleColumn.Properties["ShowColumnOptions"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["ShowColumnOptions"].SetValue(value);
                RaisePropertyChanged("ShowColumnOptions");
            }
        }

        public String Format
        {
            get
            {
                return (String)visibleColumn.Properties["Format"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["Format"].SetValue(value);
                RaisePropertyChanged("Format");
            }
        }

        public string MappingName
        {
            get
            {
                return (string)visibleColumn.Properties["MappingName"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["MappingName"].SetValue(value);
                RaisePropertyChanged("MappingName");
            }
        }

        public string HeaderText
        {
            get
            {
                return (string)visibleColumn.Properties["HeaderText"].ComputedValue;
            }
            set
            {
                visibleColumn.Properties["HeaderText"].SetValue(value);
                RaisePropertyChanged("HeaderText");
            }
        }

        public double ColumnWidth
        {
            get
            {
                var value = (GridDataControlLength)visibleColumn.Properties["Width"].ComputedValue;
                return value.Value;
            }
            set
            {
                var width = visibleColumn.Properties["Width"].ComputedValue as GridDataControlLength;
                if (width.IsNone)
                {
                    visibleColumn.Properties["Width"].SetValue(new GridDataControlLength(value));
                }
                RaisePropertyChanged("ColumnWidth");
            }
        }


        public ColumnStyleModel ColumnStyleModel
        {
            get;
            set;
        }
        public bool _generateStyle = false;
        public bool GenerateStyle
        {
            get
            {
                return _generateStyle;
            }
            set
            {
                _generateStyle = value;
                if (value == true)
                {
                    RaisePropertyChanged("GenerateStyle");

                    ModelItem colStyle = visibleColumn.Properties["ColumnStyle"].Value;

                    if (visibleColumn.Properties["ColumnStyle"].Value == null)
                    {
                        GridDataColumnStyle style = new GridDataColumnStyle();
                        colStyle = visibleColumn.Properties["ColumnStyle"].SetValue(style);
                    }

                    colStyle.Properties["Comment"].ClearValue();
                    colStyle.Properties["CommentTemplateKey"].ClearValue();
                    colStyle.Properties["ImageList"].ClearValue();
                    colStyle.Properties["MaskEdit"].ClearValue();
                    colStyle.Properties["ImageContentAlignment"].ClearValue();
                    colStyle.Properties["ImageContentStretch"].ClearValue();
                    colStyle.Properties["ImageHeight"].ClearValue();
                    colStyle.Properties["ImageWidth"].ClearValue();
                    colStyle.Properties["ImageMargins"].ClearValue();
                    colStyle.Properties["IsThemed"].ClearValue();
                    colStyle.Properties["Image"].ClearValue();
                    //colStyle.Properties["CellIdentity"].ClearValue();
                    colStyle.Properties["CommentAlignment"].ClearValue();
                    colStyle.Properties["ImageIndex"].ClearValue();
                    colStyle.Properties["CellTypeEnum"].ClearValue();
                    this.ColumnStyleModel = new ColumnStyleModel(colStyle, SpecialCellType, this.Type);
                }
                else
                {
                    visibleColumn.Properties["ColumnStyle"].ClearValue();
                }
            }
        }

        public string Type { get; set; }

        public VisibleColumnModel(ModelItem VisibleColumn, CellTypes CellType, string type)
        {
            this.visibleColumn = VisibleColumn;
            this.SpecialCellType = CellType;
            this.Type = type;
            this.GenerateStyle = true;
        }

        #region INotifyPropertyChanged Members

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public enum CellTypes
    {
        Static,
        TextBox,
        TextBlock,
        CheckBox,
        Button,
        MaskEdit,
        PercentEdit,
        DoubleEdit,
        IntegerEdit,
        UpDownEdit,
        ComboBox,
        DropDownList,
        CurrencyEdit,
        DateTimeEdit
    }
}
