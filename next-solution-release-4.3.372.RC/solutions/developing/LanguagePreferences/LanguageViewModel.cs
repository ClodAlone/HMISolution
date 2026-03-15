using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utilities;
using ViewModelLib;

namespace LanguagePreferences
{
    internal class LanguageViewModel : INotifyPropertyChanged
    {
        #region Declarations
        int callingProcessId;
        string currentSkin = "Blend";
        #endregion

        #region Constructors
        public LanguageViewModel()
        {
            Initialize();
        }

        public LanguageViewModel(int callingProcessId, string currentSkin)
        {
            this.callingProcessId = callingProcessId;
            this.currentSkin = currentSkin;
            Initialize();
        }

        void Initialize()
        {
            selectedUI = LocalizationHelper.ReadCurrentLanguage();
        }
        #endregion

        #region Public Events
        public event EventHandler CallingProcessExited;
        #endregion

        #region Public Properties
        public string CurrentUI
        {
            get
            {
                return LocalizationHelper.ReadCurrentLanguage();
            }
        }

        public string CurrentSkin
        {
            get
            {
                return currentSkin;
            }
        }

        string selectedUI;
        public string SelectedUI
        {
            get
            {
                return selectedUI;
            }
            set
            {
                if (selectedUI == value)
                    return;
                selectedUI = value;

                OnPropertyChanged("SelectedUI");
            }
        }

        string[] availableLanguages;
        public string[] AvailableLanguages
        {
            get
            {
                if (availableLanguages == null)
                {
                    var languages = Properties.Settings.Default.Languages;
                    var list = languages.Split('|').ToList();
                    if (Properties.Settings.Default.AllowMatchWindowsOption)
                        list.Insert(0, String.Empty);
                    availableLanguages = list.ToArray();
                }

                return availableLanguages;
            }
        }
        #endregion

        #region Public Methods
        public void SetNewLanguage(string culture)
        {
            LocalizationHelper.ChangeCurrentLanguage(culture);

            availableLanguages = null;
            OnPropertyChanged("AvailableLanguages");
            OnPropertyChanged("CurrentUI");

            CheckProcessRunningAndShowRestartMessage();
        }
        #endregion

        #region Commands
        RelayCommand setAsDefault;
        public ICommand SetAsDefault
        {
            get
            {
                if (setAsDefault == null)
                {
                    setAsDefault = new RelayCommand(
                        param => SetNewLanguage(SelectedUI),
                        param => CurrentUI != SelectedUI
                        );
                }
                return setAsDefault;
            }
        }

        RelayCommand closeApplication;
        public ICommand CloseApplication
        {
            get
            {
                if (closeApplication == null)
                {
                    closeApplication = new RelayCommand(
                        param => Application.Current.Shutdown()
                        );
                }
                return closeApplication;
            }
        }
        #endregion

        #region Private Methods
        void CheckProcessRunningAndShowRestartMessage()
        {
            if (callingProcessId > 0)
            {
                try
                {
                    var process = Process.GetProcessById(callingProcessId);
                    if (process != null)
                    {
                        if (System.Windows.Forms.MessageBox.Show(Properties.Resources.ApplicationRestartMessage, 
                            Properties.Resources.Title, 
                            System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Application.Current.MainWindow.Hide();
                            var filepath = process.MainModule.FileName;
                            process.EnableRaisingEvents = true;
                            process.Exited += (o, e) =>
                            {
                                var newprocess = Process.Start(filepath);
                                callingProcessId = newprocess.Id;
                                CallingProcessExited?.Invoke(this, EventArgs.Empty);
                            };
                            process.CloseMainWindow();
                        }
                    }
                }
                catch { }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
