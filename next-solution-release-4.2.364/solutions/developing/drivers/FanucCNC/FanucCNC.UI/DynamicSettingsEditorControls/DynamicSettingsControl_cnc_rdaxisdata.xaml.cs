using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_rdaxisdata : UserControl, IDisposable
    {
        FanucCNCDynTag_cnc_rdaxisdata functionSettings;
        bool bLoaded = false;
        public DynamicSettingsControl_cnc_rdaxisdata()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;

                functionSettings = DataContext as FanucCNCDynTag_cnc_rdaxisdata;

                FillListTypeData(functionSettings.Class);

                cmbClass.ItemsSource = functionSettings.GetClassList();

                cmbClass.SelectedValue = functionSettings.Class;

                cmbTypeData.SelectedValue = functionSettings.TypeData;
            };
        }

        private void FillListTypeData(short v)
        {
            var lst = functionSettings.GetTypeDataList(v);
            cmbTypeData.ItemsSource = lst;
            if (lst.Count > 0)
                cmbTypeData.SelectedIndex = 0;
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion

        private void cmbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null)
                return;

            short v = (short)((ComboBox)sender).SelectedValue;

            var lst = functionSettings.GetTypeDataList(v);
            if (lst.Count > 0)
            {
                FillListTypeData(lst[0].Code);

                cmbTypeData.ItemsSource = lst;

                cmbTypeData.SelectedIndex = 0;
            }
        }
    }
}
