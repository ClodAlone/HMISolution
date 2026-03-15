using MSModel;
using MSSchedulerSettings.Converters;
using MSSchedulerSettings.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Mvvm.Native;
using Utilities;
using UIMsgBoxAlertService.ComponentService;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for WebListView.xaml
    /// </summary>
    public partial class WeeklyItemEditList : UserControl
    {
        #region Events
        public event EventHandler<WeeklyCalendarItem> Delete;
        protected void OnDelete(WeeklyCalendarItem item)
        {
            Delete?.Invoke(this, item);
        }
        public event EventHandler<WeeklyItem> Edit;
        protected void OnEdit(WeeklyItem item)
        {
            Edit?.Invoke(this, item);
        }
        public event EventHandler AddNew;
        protected void OnAddNew()
        {
            AddNew?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Ok;
        protected void OnOk()
        {
            Ok?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        IUIMsgBoxAlertService iUIMsgBoxAlertService;

        public WeeklyItemEditList(List<WeeklyCalendarItem> weeklyItems, DayOfWeekToTextConverter dayOfWeekToTextConverter, SchedulerEditorDocument Document, bool bdesign)
        {
            InitializeComponent();
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document, !bdesign);
            weeklyItems.OrderBy(w => w.StartDate).ForEach(w => itemList.Items.Add(new WeeklyItem() { Item = w, Name = w.GetTooltip(dayOfWeekToTextConverter, false) }));
            iUIMsgBoxAlertService = Document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        private void DlgButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button != null && button.Tag is WeeklyItem)
            {
                OnEdit(button.Tag as WeeklyItem);
            }
        }

        private void DlgButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            WeeklyItem weeklyItem = button?.Tag as WeeklyItem;
            if (weeklyItem != null)
            {
                if(itemList.Items.Contains(weeklyItem))
                    itemList.Items.Remove(weeklyItem);
                OnDelete(weeklyItem.Item);
            }
        }

        private void DlgButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            OnAddNew();
        }
        private void listViewLink_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WeeklyItem weeklyItem = (sender as ListViewItem)?.Content as WeeklyItem;
            if(weeklyItem != null)
                OnEdit(weeklyItem);
        }
        void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            OnOk();
        }

        private void DlgButtonRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            if (iUIMsgBoxAlertService != null && 
                iUIMsgBoxAlertService.ShowYesNo(Properties.Resources.RemoveAllListedWeeklyItems, CustomDialogIcons.Warning) == CustomDialogResults.No)
                return;

            foreach (WeeklyItem weeklyItem in itemList.Items)
            {
                OnDelete(weeklyItem.Item);
            }
            itemList.Items.Clear();
            OnOk();
        }
    }
    public class WeeklyItem
    {
        public WeeklyCalendarItem Item { get; set; }
        public string Name { get; set; }
        public bool IsNewAppointment { get; set; }
    }
}
