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
using Microsoft.Windows.Design.Model;
using System.Windows;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
//using Syncfusion.Windows.Shared;
using System.Globalization;

namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    public class ColumnStyleModel : INotifyPropertyChanged
    {
        private ModelItem columnStyle;
        private ModelItem DateTimeEdit;
        private ModelItem CurrencyEdit;
        private ModelItem UpDownEdit;
        private ModelItem NumberFormat;



        public CellTypes CellType
        {
            get
            {
                CellTypes CellValue = ConvertToCellTypes(columnStyle.Properties["CellType"].ComputedValue as string);
                return CellValue;
            }

            set
            {
                this.columnStyle.Properties["CellType"].SetValue(value.ToString());
            }
        }

        private CellTypes ConvertToCellTypes(string value)
        {
            switch (value)
            {
                case "Static":
                    return CellTypes.Static;
                case "TextBlock":
                    return CellTypes.TextBlock;
                case "CheckBox":
                    return CellTypes.CheckBox;
                case "Button":
                    return CellTypes.Button;

            }
            return CellTypes.TextBox;
        }

        public string DisplayMember
        {
            get
            {
                return (string)columnStyle.Properties["DisplayMember"].ComputedValue;
            }
            set
            {
                columnStyle.Properties["DisplayMember"].SetValue(value);
                RaisePropertyChanged("DisplayMember");
            }
        }

        public GridDropDownStyle DropDownStyle
        {
            get
            {
                return (GridDropDownStyle)columnStyle.Properties["DropDownStyle"].ComputedValue;
            }
            set
            {
                columnStyle.Properties["DropDownStyle"].SetValue(value);
                RaisePropertyChanged("DropDownStyle");
            }
        }

        public String Format
        {
            get
            {
                return (String)this.columnStyle.Properties["Format"].ComputedValue;
            }
            set
            {
                this.columnStyle.Properties["Format"].SetValue(value);
                RaisePropertyChanged("Format");
            }
        }

        public double MinValue
        {
            get
            {
                if (this.UpDownEdit == null)
                    return 0;
                return (double)this.UpDownEdit.Properties["MinValue"].ComputedValue;
            }
            set
            {
                this.UpDownEdit.Properties["MinValue"].SetValue(value);
                RaisePropertyChanged("Minalue");
            }
        }

        public double MaxValue
        {
            get
            {
                if (this.UpDownEdit == null)
                    return 100;
                return (double)this.UpDownEdit.Properties["MaxValue"].ComputedValue;
            }
            set
            {
                this.UpDownEdit.Properties["MaxValue"].SetValue(value);
                RaisePropertyChanged("MaxValue");
            }
        }

        public decimal MaxValueCurrency
        {
            get
            {
                if (this.CurrencyEdit == null)
                    return 100;
                return (decimal)this.CurrencyEdit.Properties["MaxValue"].ComputedValue;
            }
            set
            {
                this.CurrencyEdit.Properties["MaxValue"].SetValue(value);
                RaisePropertyChanged("MaxValueCurrency");
            }
        }

        public decimal MinValueCurrency
        {
            get
            {
                if (this.CurrencyEdit == null)
                    return 0;
                return (decimal)this.CurrencyEdit.Properties["MinValue"].ComputedValue;
            }
            set
            {
                this.CurrencyEdit.Properties["MinValue"].SetValue(value);
                RaisePropertyChanged("MinValueCurrency");
            }
        }

        public string Type { get; set; }

        private string[] dateTimeFormats = new string[] 
        {
          "(none)",
          "d",
          "D",
          "f",
          "dddd, dd MMMM yyyy",
          "t",
          "s"
        };

        private string[] textFormats = new string[] 
        {
          "(none)",
          "0.00",
          "C",
          "0.00;(0.00)",
          "###0.##%",
          "#0.#E+00",
          "10:##,##0.#"
        };

        public string[] ColumnFormatSource
        {
            get
            {
                if (Type == "Int16" || Type == "Int32" || Type == "Int64" || Type == "Double" || Type == "Decimal")
                {
                    return textFormats;
                }
                else if (Type == "DateTime")
                {
                    return dateTimeFormats;
                }

                ColumnFormatEnabled = false;

                return null;
            }
        }

        private bool _columnFormatEnabled = true;

        public bool ColumnFormatEnabled
        {
            get
            {
                return _columnFormatEnabled;
            }
            set
            {
                _columnFormatEnabled = value;
                RaisePropertyChanged("ColumnFormatEnabled");
            }
        }

        public ColumnStyleModel(ModelItem columnStyle, CellTypes SpecialCellType, string type)
        {
            this.columnStyle = columnStyle;
            this.CellType = SpecialCellType;
            this.Type = type;
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
}
