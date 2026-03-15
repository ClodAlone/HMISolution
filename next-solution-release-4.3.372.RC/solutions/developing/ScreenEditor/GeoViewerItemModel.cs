using DevExpress.Mvvm;
using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModelLib;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.Core;
using System.Windows;

namespace ScreenManager
{
    public class GeoViewerItemModel : BindableBase
    {
        #region Declarations
        readonly Uri controller;
        readonly GeoViewer view;
        #endregion

        public GeoViewerItemModel(Uri c, GeoViewer v)
        {
            controller = c;
            view = v;
        }

        #region Properties
        bool isHistorizingLat;
        public bool IsHistorizingLat
        {
            get { return isHistorizingLat; }
            set 
            { 
                if (SetProperty(ref isHistorizingLat, value, "IsHistorizingLat"))
                    RaisePropertyChanged("IsHistorizing");
            }
        }

        bool isHistorizingLon;
        public bool IsHistorizingLon
        {
            get { return isHistorizingLon; }
            set
            {
                if (SetProperty(ref isHistorizingLon, value, "IsHistorizingLon"))
                    RaisePropertyChanged("IsHistorizing");
            }
        }

        public bool IsHistorizing
        {
            get { return IsHistorizingLon && isHistorizingLat; }
        }

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                SetProperty(ref isBusy, value, "IsBusy");
            }
        }

        DateTime startDate = DateTime.Now - TimeSpan.FromDays(1);
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                SetProperty(ref startDate, value, "StartDate");
            }
        }

        DateTime endDate = DateTime.Now + TimeSpan.FromDays(1);
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                SetProperty(ref endDate, value, "EndDate");
            }
        }
        #endregion

        #region Commands
        RelayCommand _showPathCommand;
        public ICommand ShowPathCommand
        {
            get
            {
                if (_showPathCommand == null)
                {
                    _showPathCommand = new RelayCommand(
                        param => 
                        { 
                            bool bRet = view.ShowPath(controller);
                            if (!bRet)
                            {
                                WinUIMessageBox.Show(
                                                Window.GetWindow(view),
                                                Properties.Resources.GeoLocationHistoryNotSupported,
                                                Properties.Resources.GeoLocationHistory,
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Information,
                                                MessageBoxResult.None, MessageBoxOptions.None,
                                                FloatingMode.Window
                                                );
                            }
                        },
                        param => !IsBusy
                        );
                }
                return _showPathCommand;
            }
        }

        RelayCommand _clearPathCommand;
        public ICommand ClearPathCommand
        {
            get
            {
                if (_clearPathCommand == null)
                {
                    _clearPathCommand = new RelayCommand(
                        param => 
                        { 
                            view.RemovePathLine(controller);
                        },
                        param => !IsBusy
                        );
                }
                return _clearPathCommand;
            }
        }
        #endregion
    }
}
