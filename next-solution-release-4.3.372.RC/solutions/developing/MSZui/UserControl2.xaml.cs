using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.WPF;

namespace MSZui
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        private string serial = string.Empty;
        private string netstate = string.Empty;
        private string date = string.Empty;
        IUIMsgBoxAlertService UIInterface;

        public bool mSZMSZViewCheckSuspended
        {
            get
            {
                return MSZ.MSZView.IsSuspended();
            }
        }

        public bool mSZMSZViewForciblySuspended
        {
            get
            {
                return MSZ.MSZView.IsForciblySuspended();
            }
        }

        bool isChecked;
        bool isLocal;
        bool bLoaded;

        public UserControl2(IUIMsgBoxAlertService uIInterface, bool bHideSuspend = false)
        {
            InitializeComponent();

            //MSZ.MSZView.Init();
            if(bHideSuspend)
                txtSuspend.Visibility = btnSuspend.Visibility = Visibility.Collapsed;

            UIInterface = uIInterface;
            txtLabelLicence.Text = "...";
            btnGet.Content = "...";
            btnGet1.Content = "...";

            var bckg = ApplicationPropertiesHelper.GetProperty("CurrentSkinBackColor") as Brush;
            var foreg = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                layout.Background = bckg;
                var labels = layout.GetVisualChildrenOfType<Label>();
                var textblocks = layout.GetVisualChildrenOfType<TextBlock>();
                foreach (var l in labels)
                    l.Foreground = foreg;
                foreach (var t in textblocks)
                    t.Foreground = foreg;
            };

            //txtRemoved.Text = GetRemoved();

            using (new WaitCursor())
            {
                isChecked = MSZ.MSZView.CheckState(true);
            }


            if (isChecked)
            {
                if (mSZMSZViewForciblySuspended)
                    txtLabelLicence.Text = Properties.Resources.DongleForciblySuspendedInfo;
                else if (mSZMSZViewCheckSuspended)
                    txtLabelLicence.Text = Properties.Resources.DongleSuspendedInfo;
                else if (MSZ.MSZView.IsRemoved)
                    txtLabelLicence.Text = Properties.Resources.DongleRemoved;
                else
                    txtLabelLicence.Text = Properties.Resources.licenseNotFound;

                btnSuspend.IsEnabled = false;
                btnRemove.IsEnabled = false;

                btnGet1.Content = Properties.Resources.ActivateLicence;
                btnGet.Content = Properties.Resources.GetLicence;
                //finishButton.IsEnabled = true;
            }
            else
            {
                var isTemporaryLicense = MSZ.MSZView.GetExpiringDate() == DateTime.MinValue ? false : true;

                if (MSZ.MSZView.CheckLocal() && !isTemporaryLicense)
                    btnRemove.IsEnabled = true;
                else
                    btnRemove.IsEnabled = false;

                SetBusy(true);
                var task1 = Task.Factory.StartNew(delegate
                {
                    var expiringDate = MSZ.MSZView.GetExpiringDate();
                    date = expiringDate != DateTime.MinValue ? expiringDate.ToShortDateString() : Properties.Resources.NoDate;
                    serial = MSZ.MSZView.GetSerial();
                    netstate = WPFUtilities.CryptString.CryptString.DecryptString(MSZ.MSZView.GetNetState());
                    return;
                });
                var task2 = task1.ContinueWith(ret =>
                {
                    if (!string.IsNullOrEmpty(serial) && !serial.Equals("0"))
                        txtSerial.Content = String.Format("{0} : {1}", Properties.Resources.SerialNumber, serial);
                    if (!string.IsNullOrEmpty(netstate))
                    {
                        txtNetState.Content = String.Format("{0} : {1}", Properties.Resources.NetState, netstate);
                        txtLabelLicence.Text = Properties.Resources.NetLicenseFound;
                    }
                    else
                        txtLabelLicence.Text = Properties.Resources.licenseFound;

                    txtLabelExpiringDate.Content = String.Format("{0} : {1}", Properties.Resources.ExipingDate, date);
                    btnGet1.Content = Properties.Resources.ActivateLicence;
                    btnGet.Content = Properties.Resources.GetNewLicence;

                    SetBusy(false);
                    //finishButton.IsEnabled = true;
                }, TaskScheduler.FromCurrentSynchronizationContext()); 
            }
            txtBox.Text = MSZ.MSZUtils.GetPrevious();
        }

        //private string GetRemoved()
        //{
        //    var list = MSZ.MSZView.GetCodes();
        //    if (list.Count == 0)
        //        return string.Format(Properties.Resources.NoteDlg,Properties.Resources.RemovedDongleKeysEmpty);
        //    else
        //    {
        //        StringBuilder _ret = new StringBuilder(Properties.Resources.RemovedDongleKeys);
        //        _ret.Append(Environment.NewLine);
        //        int i = 0;
        //        list.ForEach(x =>
        //        {
        //            i++;
        //            _ret.Append(i);
        //            _ret.Append(") " + x);
        //            _ret.Append(Environment.NewLine);
        //        });

        //        return string.Format(Properties.Resources.NoteDlg, _ret.ToString());
        //    }
        //}

        private void SelectAll_onFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void SelectAll_onMouseDown(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Focus();
            e.Handled = true;
        }

        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            string postData = string.Format("{0}?SKGTreq=12&SKGTx3={1}",Properties.Resources.WebUrl,txtBox.Text);

            //frameView.Visibility = System.Windows.Visibility.Visible;
            //frameView.Source = new Uri(postData);

            //return;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(postData);
            proc.StartInfo = startInfo;
            proc.Start();
        }
        private static string DongleFilePath
        {
            get
            {
                return String.Format("{0}\\{1}",
                          ApplicationPropertiesHelper.GetProperty("CommonFolder"),
                          WPFUtilities.CryptString.CryptString.DecryptString("gbw/7vbvHvqkqAOktxu/QclnJSFoeDQhHfMc7yKrctw="));
            }
        }
        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Licence document (*.mlztrack)|*.mlztrack";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    try
                    {
                        System.IO.File.Copy(dlg.FileName, DongleFilePath, true);
                        using (new WaitCursor())
                        {
                            isChecked = MSZ.MSZView.CheckState(true);
                        }
                        if (isChecked)
                        {
                            UIInterface.ShowInformation(string.Format(Properties.Resources.ActivationError, Properties.Resources.DongleNotRecognized));
                        }
                        else
                        {
                            UIInterface.ShowInformation(Properties.Resources.ActivationSucceded);
                            var Owner = this.FindParent<Window>();
                            Owner.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(Properties.Resources.ActivationError, ex.Message), Properties.Resources.ActivateCaption);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("{0}{1}", "Missing MsgBoxAlert Component : ", ex.Message));
            }
        }
        #region Busy Content
        internal void SetBusy(bool bSet)
        {
            busyControl.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            busyContent.Visibility = busyControl.Visibility;
            busyControl.Refresh();
            busyContent.Refresh();
        }
        #endregion
        

        private void finishButton_Click(object sender, RoutedEventArgs e)
        {
            this.FindParent<Window>().DialogResult = true;
        }

        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            if (mSZMSZViewCheckSuspended && MSZ.MSZView.CheckState())
            {
                if(mSZMSZViewForciblySuspended)
                    UIInterface.ShowInformation(Properties.Resources.DongleForciblySuspended);
                else
                    UIInterface.ShowInformation(Properties.Resources.DongleSuspended);
            }
            else
            {
                if (MSZ.MSZView.CheckState())
                    UIInterface.ShowInformation(Properties.Resources.DongleAbsent);
                else
                    if (UIInterface.ShowOkCancel(Properties.Resources.DongleSuspendMessage, CustomDialogIcons.Question) == CustomDialogResults.OK)
                    {
                        using (new WaitCursor())
                        {
                            MSZ.MSZView.Suspend();
                            isChecked = MSZ.MSZView.CheckState(true);
                        }

                        if (mSZMSZViewCheckSuspended && isChecked)
                        {
                            if (mSZMSZViewForciblySuspended)
                                UIInterface.ShowInformation(Properties.Resources.DongleForciblySuspended);
                            else
                                UIInterface.ShowInformation(Properties.Resources.DongleSuspended);
                            var Owner = this.FindParent<Window>();
                            Owner.Close();
                        }
                        else
                            UIInterface.ShowWarning(Properties.Resources.DongleNotSuspended);

                    }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (MSZ.MSZView.CheckLocal())
            {
                var returnDialog = UIInterface.ShowYesNo(
                    Properties.Resources.DongleDialogRemoveMessage,
                    CustomDialogIcons.Warning, 
                    Properties.Resources.DongleDialogRemoveTitle);
                if (returnDialog == CustomDialogResults.Yes)
                {
                    string code;
                    using (new WaitCursor())
                    {
                        MSZ.MSZView.Remove(out code);
                        isLocal = MSZ.MSZView.CheckLocal(true);
                    }

                    if (!string.IsNullOrEmpty(code) && !isLocal)
                    {
                        var currentWindowParent = this.FindParent<Window>();
                        var licenseRemovalConfirmationUserControl = new LicenseRemovalConfirmationUserControl(code);

                        GeneralDialogContent licenseRemovalConfirmationWindow = 
                            new GeneralDialogContent(licenseRemovalConfirmationUserControl, GeneralDialogButtons.OkButton)
                        {
                            Title = Properties.Resources.LicenseRemovalConfirmation,
                            Owner = currentWindowParent
                        };

                        licenseRemovalConfirmationWindow.ShowDialog();
                        currentWindowParent.Close();
                    }
                    else
                        UIInterface.ShowWarning(Properties.Resources.DongleNotDisabled);
                }
            }
            else
            {
                UIInterface.ShowInformation(Properties.Resources.LocalDongleAbsent);
            }
        }
    }
    public class MyCommands
    {
        public static readonly ICommand CloseCommand = new ButtonCommand(o => ((Window)o).Close());
    }

    public class ButtonCommand : ICommand
    {

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public ButtonCommand(Action<object> execute)
            : this(execute, null)
        {

        }

        public ButtonCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
