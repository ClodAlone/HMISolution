using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;

namespace UFWebClient.Views
{
    public partial class QuotaManager : ChildWindow
    {
        public QuotaManager()
        {
            InitializeComponent();
 
            SetStorageData();
        }
 
        private void SetStorageData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                SpacedUsed.Text     = "Current Spaced Used = "+(isf.Quota - isf.AvailableFreeSpace).ToString() +" bytes";
                SpaceAvaiable.Text  = "Current Space Available=" + isf.AvailableFreeSpace.ToString() + " bytes";
                CurrentQuota.Text   = "Current Quota=" + isf.Quota.ToString() + " bytes";
            }
        }
        
        private void IncreaseStorage(long spaceRequest)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                long newSpace = isf.Quota + spaceRequest; 
                try
                {
                    if (true == isf.IncreaseQuotaTo(newSpace))
                    {
                        Results.Text = "Quota successfully increased.";
                    }
                    else
                    {
                        Results.Text = "Quota increase was unsuccessfull.";
                    }
                }
                catch (Exception e)
                {
                    Results.Text = "An error occured: "+e.Message;
                }
                SetStorageData();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long spaceRequest = Convert.ToInt64(SpaceRequest.Text);
                IncreaseStorage(spaceRequest);
            }
            catch(Exception ex)
            { // User put bad data in text box }
                ErrorWindow.CreateNew(ex);
            }
        }
    }
}
