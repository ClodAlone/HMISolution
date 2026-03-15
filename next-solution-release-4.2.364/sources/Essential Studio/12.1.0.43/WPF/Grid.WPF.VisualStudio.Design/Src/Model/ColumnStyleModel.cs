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
using Syncfusion.Windows.Shared;
using System.Globalization;

namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    public class ColumnStyleModel : DependencyObject, INotifyPropertyChanged
    {
        private ModelItem columnStyle;
        private ModelItem DateTimeEdit;
        private ModelItem CurrencyEdit;
        private ModelItem UpDownEdit;

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
                OnCellTypeChanged(value.ToString());
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
                case "MaskEdit":
                    return CellTypes.MaskEdit;
                case "PercentEdit":
                    return CellTypes.PercentEdit;
                case "DoubleEdit":
                    return CellTypes.DoubleEdit;
                case "IntegerEdit":
                    return CellTypes.IntegerEdit;
                case "UpDownEdit":
                    return CellTypes.UpDownEdit;
                case "ComboBox":
                    return CellTypes.ComboBox;
                case "DropDownList":
                    return CellTypes.DropDownList;
                case "CurrencyEdit":
                    return CellTypes.CurrencyEdit;
                case "DateTimeEdit":
                    return CellTypes.DateTimeEdit;

            }
            return CellTypes.TextBox;
        }

        private void OnCellTypeChanged(string value)
        {
            this.columnStyle.Properties["CellType"].SetValue(value);

            columnStyle.Properties["NumberFormat"].ClearValue();

            if (this.DateTimeEdit != null)
            {
                columnStyle.Properties["DateTimeEdit"].ClearValue();
            }

            if (this.CurrencyEdit != null)
            {
                columnStyle.Properties["CurrencyEdit"].ClearValue();
            }

            if (this.UpDownEdit != null)
            {
                columnStyle.Properties["UpDownEdit"].ClearValue();
            }

            switch (value)
            {

                case "CurrencyEdit":
                    this.CurrencyEdit = columnStyle.Properties["CurrencyEdit"].SetValue(new GridCurrencyEditStyleInfo());
                    break;

                case "DateTimeEdit":
                    this.DateTimeEdit = columnStyle.Properties["DateTimeEdit"].SetValue(new GridDateTimeEditStyleInfo());
                    break;

                case "UpDownEdit":
                    this.UpDownEdit = columnStyle.Properties["UpDownEdit"].SetValue(new GridUpDownEditStyleInfo());
                    break;
            }
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

        public string Type { get; set; }

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

        public bool IsWatchEnabled
        {
            get
            {
                if (DateTimeEdit == null)
                    return new GridDateTimeEditStyleInfo().IsWatchEnabled;

                return (bool)DateTimeEdit.Properties["IsWatchEnabled"].ComputedValue;
            }
            set
            {
                DateTimeEdit.Properties["IsWatchEnabled"].SetValue(value);
                RaisePropertyChanged("IsWatchEnabled");
            }
        }

        public bool IsVisibleRepeatButton
        {
            get
            {
                if (DateTimeEdit == null)
                    return new GridDateTimeEditStyleInfo().IsVisibleRepeatButton;
                return (bool)DateTimeEdit.Properties["IsVisibleRepeatButton"].ComputedValue;
            }
            set
            {
                DateTimeEdit.Properties["IsVisibleRepeatButton"].SetValue(value);
                RaisePropertyChanged("IsVisibleRepeatButton");
            }
        }

        public DateTimePattern DateTimePattern
        {
            get
            {
                if (DateTimeEdit == null)
                    return new GridDateTimeEditStyleInfo().DateTimePattern;
                return (DateTimePattern)DateTimeEdit.Properties["DateTimePattern"].ComputedValue;
            }
            set
            {
                DateTimeEdit.Properties["DateTimePattern"].SetValue(value);
                RaisePropertyChanged("DateTimePattern");
            }
        }

        public string NoneDateText
        {
            get
            {
                if (DateTimeEdit == null)
                    return new GridDateTimeEditStyleInfo().NoneDateText;
                return (string)DateTimeEdit.Properties["NoneDateText"].ComputedValue;
            }
            set
            {
                DateTimeEdit.Properties["NoneDateText"].SetValue(value);
                RaisePropertyChanged("NoneDateText");
            }
        }

        public Brush FocusedBackground
        {
            get
            {
                if (this.UpDownEdit == null)
                    return new GridUpDownEditStyleInfo().FocusedBackground;
                return (Brush)this.UpDownEdit.Properties["FocusedBackground"].ComputedValue;
            }
            set
            {
                DateTimeEdit.Properties["FocusedBackground"].SetValue(value);
                RaisePropertyChanged("FocusedBackground");
            }
        }

        public Brush FocusedForeground
        {
            get
            {
                if (this.UpDownEdit == null)
                    return new GridUpDownEditStyleInfo().FocusedForeground;
                return (Brush)this.UpDownEdit.Properties["FocusedForeground"].ComputedValue;
            }
            set
            {
                this.UpDownEdit.Properties["FocusedForeground"].SetValue(value);
                RaisePropertyChanged("FocusedForeground");
            }
        }

        public Brush FocusedBorderBrush
        {
            get
            {
                if (this.UpDownEdit == null)
                    return new GridUpDownEditStyleInfo().FocusedBorderBrush;
                return (Brush)this.UpDownEdit.Properties["FocusedBorderBrush"].ComputedValue;
            }
            set
            {
                this.UpDownEdit.Properties["FocusedBorderBrush"].SetValue(value);
                RaisePropertyChanged("FocusedBorderBrush");
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


        public PercentEditMode PercentEditMode
        {
            get
            {
                return (PercentEditMode)this.columnStyle.Properties["PercentEditMode"].ComputedValue;
            }
            set
            {
                this.columnStyle.Properties["PercentEditMode"].SetValue(value);
                RaisePropertyChanged("PercentEditMode");
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

        public bool HasMinValueCurrency
        {
            get
            {
                if (this.CurrencyEdit == null)
                    return new GridCurrencyEditStyleInfo().HasMinValue;
                return (bool)this.CurrencyEdit.Properties["HasMinValue"].ComputedValue;
            }
            set
            {
                this.CurrencyEdit.Properties["HasMinValue"].SetValue(value);
                RaisePropertyChanged("HasMinValueCurrency");
            }
        }

        public bool HasMaxValueCurrency
        {
            get
            {
                if (this.CurrencyEdit == null)
                    return new GridCurrencyEditStyleInfo().HasMaxValue;
                return (bool)this.CurrencyEdit.Properties["HasMaxValue"].ComputedValue;
            }
            set
            {
                this.CurrencyEdit.Properties["HasMaxValue"].SetValue(value);
                RaisePropertyChanged("HasMaxValueCurrency");
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
