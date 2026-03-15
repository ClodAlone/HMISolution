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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for LegendEditor
    /// </summary>
    public class LegendEditor : Control
    {
        private ChartLegend chartlegend;
        //private Visibility iconVisibility;
        //private Visibility checkboxVisibility;

        /// <summary>
        /// called when instance created for LegendEditor
        /// </summary>
        /// <param name="legend"></param>
        public LegendEditor(ChartLegend legend)
        {
            DefaultStyleKey = typeof(LegendEditor);
            this.chartlegend = legend;
            this.chartlegend.iconVisibility = this.chartlegend.IconVisibility;
            this.chartlegend.checkboxVisibility = this.chartlegend.CheckboxVisibility;
            this.Loaded += new RoutedEventHandler(LegendEditor_Loaded);
        }

        void LegendEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.chartlegend.iconVisibility == Visibility.Visible)
                IconCombo.SelectedIndex = 0;
            else
                IconCombo.SelectedIndex = 1;
            if (this.chartlegend.checkboxVisibility == Visibility.Visible)
                CheckBoxCombo.SelectedIndex = 0;
            else
                CheckBoxCombo.SelectedIndex = 1;
        }
        
        private ComboBox CheckBoxCombo;
        private ComboBox IconCombo;
        private Button CancelButton;
        private Button OkButton;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            OkButton = this.GetTemplateChild("Okbutton") as Button;
            OkButton.Click += new RoutedEventHandler(OkButton_Click);
            CancelButton = this.GetTemplateChild("Cancelbutton") as Button;
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
            IconCombo = this.GetTemplateChild("IconCombo") as ComboBox;
            IconCombo.SelectionChanged += new SelectionChangedEventHandler(IconCombo_SelectionChanged);
            CheckBoxCombo = this.GetTemplateChild("CheckBoxCombo") as ComboBox;
            CheckBoxCombo.SelectionChanged += new SelectionChangedEventHandler(CheckBoxCombo_SelectionChanged);
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //chartlegend.CheckboxVisibility = this.chartlegend.checkboxVisibility;
            //chartlegend.IconVisibility = this.chartlegend.iconVisibility;
            this.chartlegend.LegendEditorChildWindow.Close();
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBox c = CheckBoxCombo;
            switch (c.SelectedIndex)
            {
                case 0:
                    chartlegend.checkboxVisibility = Visibility.Visible;
                    break;
                case 1:
                    chartlegend.checkboxVisibility = Visibility.Collapsed;
                    break;
            }
            c = IconCombo;
            switch (c.SelectedIndex)
            {
                case 0:
                    chartlegend.iconVisibility = Visibility.Visible;
                    break;
                case 1:
                    chartlegend.iconVisibility = Visibility.Collapsed;
                    break;
            }
            this.chartlegend.LegendEditorChildWindow.Close();
        }

        void CheckBoxCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            switch (c.SelectedIndex)
            {
                case 0:
                    chartlegend.CheckboxVisibility = Visibility.Visible;
                    break;
                case 1:
                    chartlegend.CheckboxVisibility = Visibility.Collapsed;
                    break;
            }
        }

        void IconCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = sender as ComboBox;
            switch (c.SelectedIndex)
            {
                case 0:
                    chartlegend.IconVisibility = Visibility.Visible;
                    break;
                case 1:
                    chartlegend.IconVisibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
