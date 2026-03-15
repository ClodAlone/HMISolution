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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Syncfusion.Windows.Tools.Controls;
using System.Reflection;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Shared;
using System.Drawing;
using System.Globalization;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.RDL.Internal;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal enum MatchType
    {
        NoMatch,
        ExactMatch,
        ClosestMatch
    };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class ExpressionDialog : ChromelessWindow
    {
        int count = 0;
        string tip = null;
        string nexttip = "=";
        ColorEdit color = new ColorEdit();
        string colorpal = null;
        string systemColor = null;
        string consttext = null;
        //string output = null;
        bool check = false;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ExpressionDialog), new UIPropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }  


        #region DeclareCollection

        Collections.Built built_in = new Collections.Built();
        Collections.Arith arith_item = new Collections.Arith();
        Collections.Compare Compare = new Collections.Compare();
        Collections.Concate Concate = new Collections.Concate();
        Collections.Logic Logic = new Collections.Logic();
        Collections.Bitshift Bitshift = new Collections.Bitshift();
        Collections.Text textCollection = new Collections.Text();
        Collections.DateandTime DateandTime = new Collections.DateandTime();
        Collections.Maths Maths = new Collections.Maths();
        Collections.Inspection Inspection = new Collections.Inspection();
        Collections.Programflow Programflow = new Collections.Programflow();
        Collections.Aggregate Aggregate = new Collections.Aggregate();
        Collections.Financial Financial = new Collections.Financial();
        Collections.Conversion Conversion = new Collections.Conversion();
        Collections.Miscellaneous Miscellaneous = new Collections.Miscellaneous();

        #endregion

        #region Description

        Descriptions.Built desc_builtin = new Descriptions.Built();
        Descriptions.Arith desc_arithmetic = new Descriptions.Arith();
        Descriptions.Compare desc_comapre = new Descriptions.Compare();
        Descriptions.Concate desc_concate = new Descriptions.Concate();
        Descriptions.Logic desc_logic = new Descriptions.Logic();
        Descriptions.Bitshift desc_bitshift = new Descriptions.Bitshift();
        Descriptions.Text desc_text = new Descriptions.Text();
        Descriptions.DateandTime desc_date = new Descriptions.DateandTime();
        Descriptions.Maths desc_math = new Descriptions.Maths();
        Descriptions.Inspection desc_inspection = new Descriptions.Inspection();
        Descriptions.Programflow desc_programflow = new Descriptions.Programflow();
        Descriptions.Aggregate desc_aggregate = new Descriptions.Aggregate();
        Descriptions.Financial desc_financial = new Descriptions.Financial();
        Descriptions.Conversion desc_conversion = new Descriptions.Conversion();
        Descriptions.Miscellaneous desc_miscellaneous = new Descriptions.Miscellaneous();

        #endregion

        #region Examples

        Built ex_built = new Built();
        Arith ex_arithmetic = new Arith();
        Compare ex_compare = new Compare();
        Concate ex_concate = new Concate();
        Logic ex_logic = new Logic();
        Bitshift ex_bitshift = new Bitshift();
        Text ex_text = new Text();
        DateandTime ex_date = new DateandTime();
        Maths ex_math = new Maths();
        Inspection ex_inspection = new Inspection();
        Programflow ex_programflow = new Programflow();
        Aggregate ex_aggregate = new Aggregate();
        Financial ex_financial = new Financial();
        Conversion ex_conversion = new Conversion();
        Miscellaneous ex_miscellaneous = new Miscellaneous();

        #endregion

        public ExpressionDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            noConsts.Visibility = Visibility.Hidden;

            if (ReportDesignView.CurrentPanel != null)
            {
                if (!ReportDesignView.CurrentPanel.ShowHelp)
                {
                    this.help.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            this.Loaded += new RoutedEventHandler(ExpressionDialog_Loaded);
            this.help.Click += new RoutedEventHandler(help_Click);
        }

        public ExpressionDialog(ValueType valuetype,string name)
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ExpressionDialog_Loaded);
            this.DataContext = this;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            noConsts.Visibility = Visibility.Hidden;
            PropertyValid(valuetype,name);

            if (ReportDesignView.CurrentPanel != null)
            {
                if (!ReportDesignView.CurrentPanel.ShowHelp)
                {
                    this.help.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            this.help.Click += new RoutedEventHandler(help_Click);
        }

        void help_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/ReportExpression");
        }

        void ExpressionDialog_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }
        private void SelectConst(object sender, RoutedEventArgs e)
        {
            count = 1;
            parameters.Visibility = System.Windows.Visibility.Hidden;
            list.ItemsSource = null;
            list.Items.Clear();
            //consts.Items.Add(null);
            if (check == true)
            {
                noConsts.Visibility = Visibility.Visible;
                noConsts.Text = "No constants are available for this property.";
            }
            consts.Visibility = Visibility.Visible;
            values.Visibility = Visibility.Visible;
            color.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            color.IsAlphaVisible = true;

        }

        public void WireEvent()
        {
            color.ColorChanged += new PropertyChangedCallback(ColorPickerPalette_ColorChanged);
        }      

        static MatchType FindColour(System.Drawing.Color colour, out string name)
        {
            MatchType result = MatchType.NoMatch;

            int least_difference = 0;

            name = "";

            foreach (PropertyInfo system_colour in typeof(System.Drawing.Color).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                System.Drawing.Color system_colour_value = (System.Drawing.Color)system_colour.GetValue(null, null);

                if (system_colour_value == colour)
                {
                    name = system_colour.Name;
                    result = MatchType.ExactMatch;
                    break;
                }

                int
                  a = colour.A - system_colour_value.A,
                  r = colour.R - system_colour_value.R,
                  g = colour.G - system_colour_value.G,
                  b = colour.B - system_colour_value.B,
                  difference = a * a + r * r + g * g + b * b;

                if (result == MatchType.NoMatch || difference < least_difference)
                {
                    result = MatchType.ClosestMatch;
                    name = system_colour.Name;
                    least_difference = difference;
                }
            }

            return result;
        }

        private void built(object sender, RoutedEventArgs e)
        {
            try
            {
                count = 2;
                list.ItemsSource = null;
                list.Items.Clear();
                parameters.Items.Clear();
                noConsts.Visibility = Visibility.Hidden;
                consts.Visibility = Visibility.Hidden;
                values.Visibility = Visibility.Hidden;
                parameters.Visibility = Visibility.Hidden;
                desc.Text = "The date and time that the report began to run.";
                exam.Text = built_in[0].ItemPty;
                list.ItemsSource = built_in;
                list.DisplayMemberPath = "ItemPty";
            }
            catch
            {

            }
        }

        private void param(object sender, RoutedEventArgs e)
        {
            try
            {
                count = 3;
                noConsts.Visibility = Visibility.Hidden;
                consts.Visibility = Visibility.Hidden;
                values.Visibility = Visibility.Hidden;
                parameters.Visibility = Visibility.Visible;
                parameters.SelectionChanged += new SelectionChangedEventHandler(parameters_SelectionChanged);
                //    parameters.Items.Add("Report has no parameters.");
                parameters.Items.Clear();
                list.ItemsSource = null;
                list.Items.Clear();
                if (ReportDesignView.CurrentPanel.ReportParameters != null)
                {
                    foreach (RDL.DOM.ReportParameter param in ReportDesignView.CurrentPanel.ReportParameters)
                    {
                        parameters.Items.Add(param.Name);
                    }

                }
            }
            catch { }
        }

        private void field(object sender, RoutedEventArgs e)
        {
            count = 4;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Visible;
            list.ItemsSource = null;
            list.Items.Clear();
            parameters.Items.Clear();

            parameters.Items.Add("Report item not linked to a dataset.");
        }

        private void dataset(object sender, RoutedEventArgs e)
        {
            try
            {
                count = 5;
                noConsts.Visibility = Visibility.Hidden;
                consts.Visibility = Visibility.Hidden;
                values.Visibility = Visibility.Hidden;
                parameters.Visibility = Visibility.Visible;
                //parameters.Items.Add("Report has no datasets.");
                parameters.Items.Clear();
                list.ItemsSource = null;
                list.Items.Clear();
                if (ReportDesignView.CurrentPanel.DataSets != null)
                {
                    foreach (RDL.DOM.DataSet dataset in ReportDesignView.CurrentPanel.DataSets)
                    {
                        list.Items.Add(dataset.Name);
                    }
                }
            }
            catch
            {

            }
        }

        private void variable(object sender, RoutedEventArgs e)
        {
            count = 20;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Visible;
            list.ItemsSource = null;
            list.Items.Clear();
            parameters.Items.Clear();
            if (ReportDesignView.CurrentPanel.Report.Variables != null)
            {
                foreach (var variable in ReportDesignView.CurrentPanel.Report.Variables)
                {
                    parameters.Items.Add(variable.Name);
                }
            }
        }

        private void arith(object sender, RoutedEventArgs e)
        {
            count = 6;
            parameters.Items.Clear();
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_arithmetic[0].ItemPty;
            exam.Text = ex_arithmetic[0].ItemPty;
            list.ItemsSource = arith_item;
            list.DisplayMemberPath = "ItemPty";
        }

        private void compare(object sender, RoutedEventArgs e)
        {
            count = 7;
            parameters.Items.Clear();

            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_comapre[0].ItemPty;
            exam.Text = ex_compare[0].ItemPty;
            list.ItemsSource = Compare;
            list.DisplayMemberPath = "ItemPty";
        }

        private void concate(object sender, RoutedEventArgs e)
        {
            count = 8;
            parameters.Items.Clear();

            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_concate[0].ItemPty;
            exam.Text = ex_concate[0].ItemPty;
            list.ItemsSource = Concate;
            list.DisplayMemberPath = "ItemPty";
        }

        private void logical(object sender, RoutedEventArgs e)
        {
            count = 9;
            parameters.Items.Clear();

            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_logic[0].ItemPty;
            exam.Text = ex_logic[0].ItemPty;
            list.ItemsSource = Logic;
            list.DisplayMemberPath = "ItemPty";
        }

        private void bitshift(object sender, RoutedEventArgs e)
        {
            count = 10;
            parameters.Items.Clear();

            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_bitshift[0].ItemPty;
            exam.Text = ex_bitshift[0].ItemPty;
            list.ItemsSource = Bitshift;
            list.DisplayMemberPath = "ItemPty";
        }

        private void text(object sender, RoutedEventArgs e)
        {
            count = 11;

            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_text[0].ItemPty;
            exam.Text = ex_text[0].ItemPty;
            list.ItemsSource = textCollection;
            list.DisplayMemberPath = "ItemPty";
        }

        private void dateandtime(object sender, RoutedEventArgs e)
        {
            count = 12;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_date[0].ItemPty;
            exam.Text = ex_date[0].ItemPty;
            list.ItemsSource = DateandTime;
            list.DisplayMemberPath = "ItemPty";
        }

        private void math(object sender, RoutedEventArgs e)
        {
            count = 13;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_math[0].ItemPty;
            exam.Text = ex_math[0].ItemPty;
            list.ItemsSource = Maths;
            list.DisplayMemberPath = "ItemPty";
        }

        private void inspection(object sender, RoutedEventArgs e)
        {
            count = 14;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_inspection[0].ItemPty;
            exam.Text = ex_inspection[0].ItemPty;
            list.ItemsSource = Inspection;
            list.DisplayMemberPath = "ItemPty";
        }

        private void programflow(object sender, RoutedEventArgs e)
        {
            count = 15;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_programflow[0].ItemPty;
            exam.Text = ex_programflow[0].ItemPty;
            list.ItemsSource = Programflow;
            list.DisplayMemberPath = "ItemPty";
        }

        private void aggregate(object sender, RoutedEventArgs e)
        {
            count = 16;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_aggregate[0].ItemPty;
            exam.Text = ex_aggregate[0].ItemPty;
            list.ItemsSource = Aggregate;
            list.DisplayMemberPath = "ItemPty";
        }

        private void financial(object sender, RoutedEventArgs e)
        {
            count = 17;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_financial[0].ItemPty;
            exam.Text = ex_financial[0].ItemPty;
            list.ItemsSource = Financial;
            list.DisplayMemberPath = "ItemPty";
        }

        private void conversion(object sender, RoutedEventArgs e)
        {
            count = 18;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_conversion[0].ItemPty;
            exam.Text = ex_conversion[0].ItemPty;
            list.ItemsSource = Conversion;
            list.DisplayMemberPath = "ItemPty";
        }

        private void miscellaneous(object sender, RoutedEventArgs e)
        {
            count = 19;
            noConsts.Visibility = Visibility.Hidden;
            consts.Visibility = Visibility.Hidden;
            values.Visibility = Visibility.Hidden;
            parameters.Visibility = Visibility.Hidden;
            desc.Text = desc_miscellaneous[0].ItemPty;
            exam.Text = ex_miscellaneous[0].ItemPty;
            list.ItemsSource = Miscellaneous;
            list.DisplayMemberPath = "ItemPty";
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (expressionTextBox.Text.StartsWith("="))
            {
                nexttip = expressionTextBox.Text;
            }
            else
            {
                nexttip = "=";
            }

            if (count == 5)
            {
                string content = (sender as ListBoxItem).Content as String;
                var Dataset = (from dataset in ReportDesignView.CurrentPanel.DataSets where dataset.Name == content select dataset);
                RDL.DOM.DataSet datset = Dataset.FirstOrDefault();
                if (datset != null)
                {
                    parameters.Items.Clear();
                    foreach (RDL.DOM.Field field in datset.Fields)
                    {
                        if (field.TypeName.ToLower().Contains("int") || field.TypeName.ToLower().Contains("double"))
                        {
                            this.parameters.Items.Add("Sum(" + field.Name + ")");
                        }

                        else
                        {
                            this.parameters.Items.Add("First(" + field.Name + ")");
                        }
                    }

                    return;
                }

                else if(content!=null)
                {
                    if (content.StartsWith("First("))
                    {
                        string field = content.Substring(6, content.IndexOf(')') - 6);
                        tip = TextBoxControl.FieldConverter(field, Dialogs.PlaceHolderType.First, list.SelectedItem.ToString());
                    }

                    else 
                    {
                        string field = content.Substring(4, content.IndexOf(')') - 4);
                        tip = TextBoxControl.FieldConverter(field, Dialogs.PlaceHolderType.Sum, list.SelectedItem.ToString());
                    }
                    tip= tip.Remove(0, 1);
                }

            }
            if (count == 20)
            {
                string content = (sender as ListBoxItem).Content as String;
                tip = "Variables!" + content + ".Value";
            }
            if (expressionTextBox.Text == "" || expressionTextBox.Text == "=")
            {
                nexttip = "=";
            }

            if (count > 10 && count != 20)
            {
                expressionTextBox.Text = nexttip + tip + "(";
            }
            else
            {
                expressionTextBox.Text = nexttip + tip;
            }
        }

        private void Values_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string selectitem = null;
            consttext = expressionTextBox.Text;
            if (expressionTextBox.Text.StartsWith("="))
            {
                selectitem = consts.SelectedItem.ToString();
                expressionTextBox.Text = consttext + "\"" + selectitem + "\"";
            }
            else
            {
                expressionTextBox.Text = consts.SelectedItem.ToString();
            }

            consttext = expressionTextBox.Text;
        }


        void ColorPickerPalette_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string value = null;
            colorpal = expressionTextBox.Text;
            color.Visibility = Visibility.Visible;
            color.Width = 200;
            System.Windows.Media.Brush selcolor = new ReportingBrushConverter().ConvertFromInvariantString(e.NewValue.ToString()); 
            System.Drawing.ColorConverter convert1 = new System.Drawing.ColorConverter();
            System.Drawing.Color c = (System.Drawing.Color)convert1.ConvertFromInvariantString(selcolor.ToString());
            
            MatchType match_type = FindColour(c, out value);
            systemColor = value;

            if (!expressionTextBox.Text.StartsWith("="))
            {
                expressionTextBox.Text = systemColor;
            }
            else
            {
                expressionTextBox.Text = colorpal + "\"" + systemColor + "\"";
            }

            colorpal = expressionTextBox.Text;
        }

        private void changevalue(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (count == 2)
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_builtin[i].ItemPty;
                            exam.Text = ex_built[i].ItemPty;
                            tip = ex_built[i].ItemPty;
                            break;
                        }
                    }
                }


                if (count == 6)
                {
                    for (int i = 0; i <= 6; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_arithmetic[i].ItemPty;
                            exam.Text = ex_arithmetic[i].ItemPty;
                            tip = arith_item[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 7)
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_comapre[i].ItemPty;
                            exam.Text = ex_compare[i].ItemPty;
                            tip = Compare[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 8)
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_concate[i].ItemPty;
                            exam.Text = ex_concate[i].ItemPty;
                            tip = Concate[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 9)
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_logic[i].ItemPty;
                            exam.Text = ex_logic[i].ItemPty;
                            tip = Logic[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 10)
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_bitshift[i].ItemPty;
                            exam.Text = ex_bitshift[i].ItemPty;
                            tip = Bitshift[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 11)
                {
                    for (int i = 0; i <= 31; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_text[i].ItemPty;
                            exam.Text = ex_text[i].ItemPty;
                            tip = textCollection[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 12)
                {
                    for (int i = 0; i <= 23; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_date[i].ItemPty;
                            exam.Text = ex_date[i].ItemPty;
                            tip = DateandTime[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 13)
                {
                    for (int i = 0; i <= 25; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_math[i].ItemPty;
                            exam.Text = ex_math[i].ItemPty;
                            tip = Maths[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 14)
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_inspection[i].ItemPty;
                            exam.Text = ex_inspection[i].ItemPty;
                            tip = Inspection[i].ItemPty;
                            break;
                        }
                    }

                }
                if (count == 15)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_programflow[i].ItemPty;
                            exam.Text = ex_programflow[i].ItemPty;
                            tip = Programflow[i].ItemPty;
                            break;
                        }
                    }

                }
                if (count == 16)
                {
                    for (int i = 0; i <= 14; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_aggregate[i].ItemPty;
                            exam.Text = ex_aggregate[i].ItemPty;
                            tip = Aggregate[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 17)
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_financial[i].ItemPty;
                            exam.Text = ex_financial[i].ItemPty;
                            tip = Financial[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 18)
                {
                    for (int i = 0; i <= 17; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_conversion[i].ItemPty;
                            exam.Text = ex_conversion[i].ItemPty;
                            tip = Conversion[i].ItemPty;
                            break;
                        }
                    }

                }

                if (count == 19)
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        if (list.SelectedItem == list.Items[i] && list.SelectedItem != null)
                        {
                            desc.Text = desc_miscellaneous[i].ItemPty;
                            exam.Text = ex_miscellaneous[i].ItemPty;
                            tip = Miscellaneous[i].ItemPty;
                            break;
                        }
                    }

                }
            }
            catch
            {

            }

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.SetValue(TextProperty, this.expressionTextBox.Text);
            this.Close();
        }       

        public void PropertyValid(ValueType valuetype,string name)
        {
            Title.Text = name;

            if (valuetype==ValueType.FillStyle)
            {
                consts.Items.Remove(color);
                consts.Items.Add(color);
                WireEvent();
            }
            else if (valuetype == ValueType.HorizontalAlignment)
            {
                EnumToListBox(typeof(HorizontalAlignment), consts);
            }
            else if (valuetype == ValueType.VerticalAlignment)
            {
                EnumToListBox(typeof(VerticalAlignment), consts);
            }
            else if (valuetype == ValueType.FontStyle)
            {
                EnumToListBox(typeof(FontStyle), consts);
            }
            else if (valuetype == ValueType.FontWeight)
            {
                EnumToListBox(typeof(FontWeight), consts);
            }
            else if (valuetype == ValueType.FontEffects)
            {
                EnumToListBox(typeof(FontEffects), consts);
            }
            else if (valuetype==ValueType.Sizing)
            {
                EnumToListBox(typeof(Sizing), consts);
            }
            else if (valuetype == ValueType.BorderStyle)
            {
                EnumToListBox(typeof(LineStyle), consts);
            }
            else if (valuetype==ValueType.ChartType)
            {
                EnumToListBox(typeof(ChartType), consts);
            }
            else if (valuetype==ValueType.FillStyle)
            {
                EnumToListBox(typeof(FillStyle), consts);
            }
            else if (valuetype==ValueType.GradientStyle)
            {
                EnumToListBox(typeof(GradientStyle), consts);
            }
            else if (valuetype==ValueType.Position)
            {
                EnumToListBox(typeof(Position), consts);
            }
            else if (valuetype==ValueType.Layout)
            {
                EnumToListBox(typeof(Layout), consts);
            }
            else if (valuetype==ValueType.TickStyle)
            {
                EnumToListBox(typeof(TickStyle), consts);
            }
            else if (valuetype==ValueType.TitleAlignment)
            {
                EnumToListBox(typeof(TitleAlignment), consts);
            }
            else if (valuetype==ValueType.CircularType)
            {
                EnumToListBox(typeof(Type), consts);
            }
            else if (valuetype==ValueType.PageBreak)
            {
                EnumToListBox(typeof(PageBreak), consts);
            }
            else if (valuetype==ValueType.BorderStyle||valuetype==ValueType.LineStyle)
            {
                EnumToListBox(typeof(Style), consts);
            }
            else if (valuetype==ValueType.TickPlacement)
            {
                EnumToListBox(typeof(Placement), consts);
            }
            else if (valuetype==ValueType.PointerType)
            {
                EnumToListBox(typeof(PointerType), consts);
            }
            else if (valuetype==ValueType.NeedleType)
            {
                EnumToListBox(typeof(NeedleType), consts);
            }
            else if (valuetype==ValueType.TickShape)
            {
                EnumToListBox(typeof(MajorTickShape), consts);
            }
            else if (valuetype.ToString() == "Shading")
            {
                EnumToListBox(typeof(Shading), consts);
            }
            else if (valuetype==ValueType.Hidden)
            {
                EnumToListBox(typeof(Hidden), consts);
            }
            else if (valuetype==ValueType.FontFamily)
            {
                consts.ItemsSource = Fonts.SystemFontFamilies;
            }
            else if (valuetype == ValueType.KeepTogether)
            {
                EnumToListBox(typeof(Hidden), consts);
            }
            else if (valuetype == ValueType.AdornmentType)
            {
                EnumToListBox(typeof(AdornmentType), consts);
            }
            else
            {
                check = true;
                values.Visibility = Visibility.Visible;
                noConsts.Visibility = Visibility.Visible;
                noConsts.Text = "No constants are available for this property.";
            }
        }
        static public void EnumToListBox(System.Type EnumType, System.Windows.Controls.ListBox TheListBox)
        {
            Array Values = System.Enum.GetValues(EnumType);
            foreach (var Value in Values)
            {
                TheListBox.Items.Remove(Value);
                TheListBox.Items.Add(Value);

            }

        }

        private void parameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as ListBox).SelectedItem != null)
                {
                    string selecteditem = (sender as ListBox).SelectedItem.ToString();
                    if (count == 3)
                    {
                        tip = "Parameters!" + selecteditem + ".Value";
                    }
                }
            }
            catch
            {

            }
        }

        private void ChromelessWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if ((e.Source as TreeViewItem).Header.Equals("Operators") || (e.Source as TreeViewItem).Header.Equals("Common Functions"))
            {
                noConsts.Visibility = System.Windows.Visibility.Visible;
                noConsts.Text = "";
                list.ItemsSource = null;
                list.Items.Clear();
            }
        }
    }

    internal static class IconHelper
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                   int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
                   IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |
                  SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

    }


}
