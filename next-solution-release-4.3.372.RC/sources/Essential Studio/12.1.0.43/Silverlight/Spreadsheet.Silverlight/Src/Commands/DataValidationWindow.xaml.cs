#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class DataValidationWindow : WindowControl,INotifyPropertyChanged 
    {
        public DataValidationWindow()
        {
            _allowTypeList = new List<string>();
            _allowTypeList.Add(SpreadsheetResourceWrapper.AnyValue);
            _allowTypeList.Add(SpreadsheetResourceWrapper.WholeNumber);
            _allowTypeList.Add(SpreadsheetResourceWrapper.Decimal);
            _allowTypeList.Add(SpreadsheetResourceWrapper.List);
            _allowTypeList.Add(SpreadsheetResourceWrapper.DateAndTime);
            _allowTypeList.Add(SpreadsheetResourceWrapper.TextLength);

            _dataList = new List<string>();
            _dataList.Add(SpreadsheetResourceWrapper.Between_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.Notbetween_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.Equalto_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.NotEqualTo_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.GreaterThan_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.Lessthan_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.GreaterThanOrEqualTo_Lower);
            _dataList.Add(SpreadsheetResourceWrapper.LessThanOrEqualTo_Lower);

            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
            Loaded += new RoutedEventHandler(DataValidationWindow_Loaded);
        }

        private List<string> _allowTypeList;
        public List<string> AllowTypeList
        {
            get { return _allowTypeList; }
            set
            {
                _allowTypeList = value;
                OnPropertyChanged("AllowTypeList");
            }
        }

        private List<string> _dataList;
        public List<string> DataList
        {
            get { return _dataList; }
            set
            {
                _dataList = value;
                OnPropertyChanged("DataList");
            }
        }

        void DataValidationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
            IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (AssociatedSpreadsheet != null)
            {
                int row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                int col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                string cells = GridRangeInfo.GetAlphaLabel(col) + row;
                ValidationRange = worksheet.Range[cells];

                if (ValidationRange.HasDataValidation)
                {
                    if (ValidationRange.DataValidation.AllowType == ExcelDataType.User)
                    {
                        string list = string.Empty;
                        if (validationRange.DataValidation.ListOfValues != null)
                        {
                            foreach (var item in validationRange.DataValidation.ListOfValues)
                            {
                                if (!string.IsNullOrEmpty(item))
                                    list += item + ',';
                            }
                            firstValueTxtBox.Text = list;
                        }
                        else if (validationRange.DataValidation.DataRange != null)
                        {
                            firstValueTxtBox.Text = string.Format("={0}",validationRange.DataValidation.FirstFormula);
                        }
                    }
                    else
                        firstValueTxtBox.Text = ValidationRange.DataValidation.FirstFormula == null ? string.Empty : ValidationRange.DataValidation.FirstFormula;
                    secondValueTxtBox.Text = ValidationRange.DataValidation.SecondFormula == null ? string.Empty : ValidationRange.DataValidation.SecondFormula;
                    promptBoxTitleTxtBox.Text = ValidationRange.DataValidation.PromptBoxTitle;
                    promptBoxTextTxtBox.Text = ValidationRange.DataValidation.PromptBoxText;
                    errorBoxTitleTxtBox.Text = ValidationRange.DataValidation.ErrorBoxText;
                    errorBoxTextTxtBox.Text = ValidationRange.DataValidation.ErrorBoxText;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
            base.OnKeyDown(e);
        }
        

        private IRange validationRange;
        public IRange ValidationRange
        {
            get { return validationRange; }
            set
            {
                validationRange = value;
                OnPropertyChanged("ValidationRange");
            }
        }

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set
            {
                _associatedExcelEditor = value;
                OnPropertyChanged("AssociatedSpreadsheet");
            }
        }

        private void OnClearAllButtonClick(object sender, RoutedEventArgs e)
        {
            allowCombo.SelectedIndex = 0;
            dataCombo.SelectedIndex = -1;
            firstValueTxtBox.Text = string.Empty;
            secondValueTxtBox.Text = string.Empty;
            promptBoxTitleTxtBox.Text = string.Empty;
            promptBoxTextTxtBox.Text = string.Empty;
            errorBoxTitleTxtBox.Text = string.Empty;
            errorBoxTextTxtBox.Text = string.Empty;

            if (ValidationRange.HasDataValidation)
            {
                ValidationRange.DataValidation.AllowType = ExcelDataType.Any;
                validationRange.DataValidation.CompareOperator = ExcelDataValidationComparisonOperator.Between;
                //validationRange.DataValidation.FirstFormula = null;
                //validationRange.DataValidation.SecondFormula = string.Empty;
                validationRange.DataValidation.PromptBoxText = string.Empty;
                validationRange.DataValidation.PromptBoxTitle = string.Empty;
                validationRange.DataValidation.ErrorBoxText = string.Empty;
                validationRange.DataValidation.ErrorBoxTitle = string.Empty;
            }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            bool success = true;
            if (allowCombo.SelectedItem != null)
            {
                IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                    foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                    {
                        string cells = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        var validationRange = worksheet.Range[cells];
                        success = ApplyDataValidation(validationRange);
                        if (!success)
                        {
                            validationRange.DataValidation.AllowType = ExcelDataType.Any;
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                    }
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    int row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                    int col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                    string cells = GridRangeInfo.GetAlphaLabel(col) + row;
                    var validationRange = worksheet.Range[cells];
                    success = ApplyDataValidation(validationRange);
                    if (!success)
                    {
                        validationRange.DataValidation.AllowType = ExcelDataType.Any;
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
                AssociatedSpreadsheet.GridProperties.CurrentCell.Deactivate();
            }
            if (success)
                Close();
        }

        private bool ApplyDataValidation(IRange range)
        {
            IDataValidation validation = range.DataValidation;

            validation.PromptBoxTitle = promptBoxTitleTxtBox.Text;
            validation.PromptBoxText = promptBoxTextTxtBox.Text;
            validation.ShowPromptBox = true;
            validation.ErrorBoxTitle = errorBoxTitleTxtBox.Text;
            validation.ErrorBoxText = errorBoxTextTxtBox.Text;
            validation.ShowErrorBox = true;

            if (!string.IsNullOrEmpty(firstValueTxtBox.Text))
            {
                int firstintvalue = 0;
                double firstdoublevalue = 0;
                DateTime firstdatetimevalue = DateTime.Today;
                ExcelDataType AllowType = GetAllowType();
                if((AllowType== ExcelDataType.Integer || AllowType== ExcelDataType.TextLength) && !int.TryParse(firstValueTxtBox.Text,out firstintvalue))
                {
                    ShowErrorMessage();
                    validation.AllowType = ExcelDataType.Any;
                    return false;
                }
                else if (AllowType == ExcelDataType.Decimal && !double.TryParse(firstValueTxtBox.Text, out firstdoublevalue))
                {
                    ShowErrorMessage();
                    validation.AllowType = ExcelDataType.Any;
                    return false;
                }
                else if (AllowType == ExcelDataType.Date && !DateTime.TryParse(firstValueTxtBox.Text, out firstdatetimevalue))
                {
                    ShowErrorMessage();
                    validation.AllowType = ExcelDataType.Any;
                    return false;
                }
                validation.AllowType = GetAllowType();
                validation.CompareOperator = GetDataValidationComparisonOperator();
                if (validation.AllowType != ExcelDataType.User && (validation.CompareOperator == ExcelDataValidationComparisonOperator.Between || validation.CompareOperator == ExcelDataValidationComparisonOperator.NotBetween))
                {
                    if (string.IsNullOrEmpty(secondValueTxtBox.Text))
                    {
                        if (validation.AllowType == ExcelDataType.Integer || validation.AllowType == ExcelDataType.Decimal || validation.AllowType == ExcelDataType.TextLength)
                            MessageBox.Show(SpreadsheetResourceWrapper.MinimumMaximumErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                        if (validation.AllowType == ExcelDataType.Date || validation.AllowType == ExcelDataType.Time)
                            MessageBox.Show(SpreadsheetResourceWrapper.EndDateStartDateErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                        validation.AllowType = ExcelDataType.Any;
                        return false;
                    }
                    else
                    {
                        int secondintvalue = 0;
                        double seconddoublevalue = 0;
                        DateTime seconddatetimevalue = DateTime.Today;
                        if ((AllowType == ExcelDataType.Integer || AllowType == ExcelDataType.TextLength) && !int.TryParse(secondValueTxtBox.Text, out secondintvalue))
                        {
                            ShowErrorMessage();
                            validation.AllowType = ExcelDataType.Any;
                            return false;
                        }
                        else if (AllowType == ExcelDataType.Decimal && !double.TryParse(secondValueTxtBox.Text, out seconddoublevalue))
                        {
                            ShowErrorMessage();
                            validation.AllowType = ExcelDataType.Any;
                            return false;
                        }
                        else if (AllowType == ExcelDataType.Date && !DateTime.TryParse(secondValueTxtBox.Text, out seconddatetimevalue))
                        {
                            ShowErrorMessage();
                            validation.AllowType = ExcelDataType.Any;
                            return false;
                        }

                        if ((AllowType == ExcelDataType.Integer || AllowType == ExcelDataType.TextLength) && firstintvalue > secondintvalue)
                        {
                            ShowErrorMessage();
                            validation.AllowType = ExcelDataType.Any;
                            return false;
                        }
                        else if (AllowType == ExcelDataType.Decimal && firstdoublevalue > seconddoublevalue)
                        {
                            ShowErrorMessage();
                            validation.AllowType = ExcelDataType.Any;
                            return false;
                        }
                        else if (AllowType == ExcelDataType.Date && firstdatetimevalue > seconddatetimevalue)
                        {
                            ShowErrorMessage();
                            validation.AllowType = ExcelDataType.Any;
                            return false;
                        }
                    }
                }
                if (validation.AllowType == ExcelDataType.User)
                {
                    try
                    {
                        string firstFormula = firstValueTxtBox.Text;
                        if (firstFormula.Contains(','))
                        {
                            var list = firstFormula.Split(new char[] { ',' });
                            validation.ListOfValues = list;
                        }
                        else
                            validation.FirstFormula = firstFormula;
                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
                        return false;
                    }
                }
                string FirstFormula = string.IsNullOrEmpty(firstValueTxtBox.Text)
                                              ? string.Empty
                                              : firstValueTxtBox.Text;
                validation.FirstFormula = FirstFormula;
                string SecondFormula = string.IsNullOrEmpty(secondValueTxtBox.Text)
                                              ? string.Empty
                                              : secondValueTxtBox.Text;
                if (!string.IsNullOrEmpty(SecondFormula))
                    validation.SecondFormula = SecondFormula;
                return true;
            }
            else
            {
                ExcelDataType allowType = GetAllowType();
                ExcelDataValidationComparisonOperator ComparisonOperator = GetDataValidationComparisonOperator();
                if ((allowType == ExcelDataType.Integer || allowType == ExcelDataType.Decimal || allowType == ExcelDataType.TextLength) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Between || ComparisonOperator == ExcelDataValidationComparisonOperator.NotBetween))
                    MessageBox.Show(SpreadsheetResourceWrapper.MinimumMaximumErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if ((allowType == ExcelDataType.Integer || allowType == ExcelDataType.Decimal || allowType == ExcelDataType.TextLength) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Greater || ComparisonOperator == ExcelDataValidationComparisonOperator.GreaterOrEqual))
                    MessageBox.Show(SpreadsheetResourceWrapper.MinimumErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if ((allowType == ExcelDataType.Integer || allowType == ExcelDataType.Decimal || allowType == ExcelDataType.TextLength) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Less || ComparisonOperator == ExcelDataValidationComparisonOperator.LessOrEqual))
                    MessageBox.Show(SpreadsheetResourceWrapper.MaximumErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if ((allowType == ExcelDataType.Integer || allowType == ExcelDataType.Decimal || allowType == ExcelDataType.TextLength) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Equal || ComparisonOperator == ExcelDataValidationComparisonOperator.NotEqual))
                    MessageBox.Show(SpreadsheetResourceWrapper.ValueErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if ((allowType == ExcelDataType.Date || allowType == ExcelDataType.Time) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Between || ComparisonOperator == ExcelDataValidationComparisonOperator.NotBetween))
                    MessageBox.Show(SpreadsheetResourceWrapper.EndDateStartDateErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if ((allowType == ExcelDataType.Date || allowType == ExcelDataType.Time) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Greater || ComparisonOperator == ExcelDataValidationComparisonOperator.GreaterOrEqual))
                    MessageBox.Show(SpreadsheetResourceWrapper.StartDateErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if ((allowType == ExcelDataType.Date || allowType == ExcelDataType.Time) && (ComparisonOperator == ExcelDataValidationComparisonOperator.Less || ComparisonOperator == ExcelDataValidationComparisonOperator.LessOrEqual))
                    MessageBox.Show(SpreadsheetResourceWrapper.EndDateErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if (allowType == ExcelDataType.User)
                    MessageBox.Show(SpreadsheetResourceWrapper.SourceErrorText, SpreadsheetResourceWrapper.ErrorMessageTitle, MessageBoxButton.OK);
                if (allowType == ExcelDataType.Any)
                    return true;
                return false;
            }
        }

        private void ShowErrorMessage()
        {
            MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnteredInvalidData, SpreadsheetResourceWrapper.ValidationMessage_Error, MessageBoxButton.OK);
        }

        private ExcelDataType GetAllowType()
        {
            if (allowCombo.SelectedItem != null)
            {
                string selectedValue = allowCombo.SelectedItem.ToString();
                if (selectedValue == SpreadsheetResourceWrapper.AnyValue)
                    return ExcelDataType.Any;
                else if (selectedValue == SpreadsheetResourceWrapper.WholeNumber)
                    return ExcelDataType.Integer;
                else if (selectedValue == SpreadsheetResourceWrapper.Decimal)
                    return ExcelDataType.Decimal;
                else if (selectedValue == SpreadsheetResourceWrapper.DateAndTime)
                    return ExcelDataType.Date;
                else if (selectedValue == SpreadsheetResourceWrapper.List)
                    return ExcelDataType.User;
                else if (selectedValue == SpreadsheetResourceWrapper.TextLength)
                    return ExcelDataType.TextLength;
                else if (selectedValue == SpreadsheetResourceWrapper.Formula)
                    return ExcelDataType.Formula;
            }
            return ExcelDataType.Any;
        }

        private ExcelDataValidationComparisonOperator GetDataValidationComparisonOperator()
        {
            if (dataCombo.SelectedItem != null)
            {
                string selectedValue = dataCombo.SelectedItem.ToString();
                if (selectedValue == SpreadsheetResourceWrapper.Between_Lower)
                    return ExcelDataValidationComparisonOperator.Between;
                else if (selectedValue == SpreadsheetResourceWrapper.Notbetween_Lower)
                    return ExcelDataValidationComparisonOperator.NotBetween;
                else if (selectedValue == SpreadsheetResourceWrapper.GreaterThan_Lower)
                    return ExcelDataValidationComparisonOperator.Greater;
                else if (selectedValue == SpreadsheetResourceWrapper.GreaterThanOrEqualTo_Lower)
                    return ExcelDataValidationComparisonOperator.GreaterOrEqual;
                else if (selectedValue == SpreadsheetResourceWrapper.Lessthan_Lower)
                    return ExcelDataValidationComparisonOperator.Less;
                else if (selectedValue == SpreadsheetResourceWrapper.LessThanOrEqualTo_Lower)
                    return ExcelDataValidationComparisonOperator.LessOrEqual;
                else if (selectedValue == SpreadsheetResourceWrapper.Equalto_Lower)
                    return ExcelDataValidationComparisonOperator.Equal;
                else if (selectedValue == SpreadsheetResourceWrapper.NotEqualTo_Lower)
                    return ExcelDataValidationComparisonOperator.NotEqual;
            }
            return ExcelDataValidationComparisonOperator.Equal;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        

        #region INotifyPropertyChanged Members

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        private void allowComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allowCombo.SelectedItem != null)
            {
                string allowComboselectedValue = allowCombo.SelectedItem.ToString();
                 string dataComoselectedValue =string.Empty;
                 if (dataCombo.SelectedItem != null)
                     dataComoselectedValue = dataCombo.SelectedItem.ToString();
                SetVisibility(allowComboselectedValue, dataComoselectedValue);
            }
        }

        private void dataComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataCombo.SelectedItem != null)
            {
                string dataComoselectedValue = dataCombo.SelectedItem.ToString();
                string allowCombosselectedValue = string.Empty;
                if (allowCombo.SelectedItem != null)
                    allowCombosselectedValue = allowCombo.SelectedItem.ToString();
                SetVisibility(allowCombosselectedValue, dataComoselectedValue);
            }
        }

        private void SetVisibility(string allowCombosselectedValue, string dataComoselectedValue)
        {
            if (allowCombosselectedValue == SpreadsheetResourceWrapper.AnyValue)
            {
                dataCombo.IsEnabled = false;
                firstValueTxtBlock.Visibility = Visibility.Collapsed;
                firstValueTxtBox.Visibility = Visibility.Collapsed;
                secondValueTxtBlock.Visibility = Visibility.Collapsed;
                secondValueTxtBox.Visibility = Visibility.Collapsed;
            }
            else if (allowCombosselectedValue == SpreadsheetResourceWrapper.List)
            {
                 dataCombo.IsEnabled = false;
                firstValueTxtBlock.Visibility = Visibility.Visible;
                firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Source;
                firstValueTxtBox.Visibility = Visibility.Visible;
                secondValueTxtBlock.Visibility = Visibility.Collapsed;
                secondValueTxtBox.Visibility = Visibility.Collapsed;
            }
            else if (allowCombosselectedValue == SpreadsheetResourceWrapper.WholeNumber ||
                allowCombosselectedValue == SpreadsheetResourceWrapper.Decimal ||
                allowCombosselectedValue == SpreadsheetResourceWrapper.TextLength)
            {
                dataCombo.IsEnabled = true;
                if (dataComoselectedValue == SpreadsheetResourceWrapper.Between_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Minimum;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Text = SpreadsheetResourceWrapper.Maximum;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.Notbetween_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Minimum;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Text = SpreadsheetResourceWrapper.Maximum;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.GreaterThan_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Minimum;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.GreaterThanOrEqualTo_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Minimum;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.Lessthan_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Maximum;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.LessThanOrEqualTo_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Maximum;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.Equalto_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Value;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;

                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.NotEqualTo_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Value;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else if (allowCombosselectedValue == SpreadsheetResourceWrapper.DateAndTime)
            {
                dataCombo.IsEnabled = true;
                if (dataComoselectedValue == SpreadsheetResourceWrapper.Between_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.StartDate;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Text = SpreadsheetResourceWrapper.EndDate;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.Notbetween_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.StartDate;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Text = SpreadsheetResourceWrapper.EndDate;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.GreaterThan_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.StartDate;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.GreaterThanOrEqualTo_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.StartDate;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.Lessthan_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.EndDate;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.LessThanOrEqualTo_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.EndDate;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.Equalto_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Date;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;

                }
                else if (dataComoselectedValue == SpreadsheetResourceWrapper.NotEqualTo_Lower)
                {
                    firstValueTxtBlock.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBox.Visibility = System.Windows.Visibility.Visible;
                    firstValueTxtBlock.Text = SpreadsheetResourceWrapper.Date;
                    secondValueTxtBlock.Visibility = System.Windows.Visibility.Collapsed;
                    secondValueTxtBox.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }

    }

    public class StringToBoolConverter:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.ToString() != SpreadsheetResourceWrapper.AnyValue && value.ToString() != SpreadsheetResourceWrapper.List;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class StringToVisibilityforMinVale:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.ToString() != SpreadsheetResourceWrapper.AnyValue ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class StringToVisibiltyforMaxValue:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value.ToString() == SpreadsheetResourceWrapper.Between || value.ToString() == SpreadsheetResourceWrapper.NotBetween ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ExcelDataTypeToStringConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ExcelDataType)value == ExcelDataType.Any)
                return SpreadsheetResourceWrapper.AnyValue;
            else if ((ExcelDataType)value == ExcelDataType.Integer)
                return SpreadsheetResourceWrapper.WholeNumber;
            else if ((ExcelDataType)value == ExcelDataType.Decimal)
                return SpreadsheetResourceWrapper.Decimal;
            else if ((ExcelDataType)value == ExcelDataType.Date)
                return SpreadsheetResourceWrapper.DateAndTime;
            else if ((ExcelDataType)value == ExcelDataType.User)
                return SpreadsheetResourceWrapper.List;
            else if ((ExcelDataType)value == ExcelDataType.TextLength)
                return SpreadsheetResourceWrapper.TextLength;
            return SpreadsheetResourceWrapper.AnyValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ExcelDataValidationComparisonOperatorToStringConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.Between)
                return SpreadsheetResourceWrapper.Between_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.NotBetween)
                return SpreadsheetResourceWrapper.Notbetween_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.Greater)
                return SpreadsheetResourceWrapper.GreaterThan_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.GreaterOrEqual)
                return SpreadsheetResourceWrapper.GreaterThanOrEqualTo_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.Less)
                return SpreadsheetResourceWrapper.Lessthan_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.LessOrEqual)
                return SpreadsheetResourceWrapper.LessThanOrEqualTo_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.Equal)
                return SpreadsheetResourceWrapper.Equalto_Lower;
            else if ((ExcelDataValidationComparisonOperator)value == ExcelDataValidationComparisonOperator.NotEqual)
                return SpreadsheetResourceWrapper.NotEqualTo_Lower;
            return SpreadsheetResourceWrapper.Between_Lower;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
