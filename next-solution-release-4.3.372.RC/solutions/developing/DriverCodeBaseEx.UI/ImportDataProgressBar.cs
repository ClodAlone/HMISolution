using System;
using DriverCodeBaseEx.UI.Controls;
using System.Threading;
using Utilities;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DriverCodeBaseEx.UI
{    
    /// <summary>   Import data. </summary>
    public class ImportDataProgressBar : IDisposable
    {
        #region class ImportDataProgressViewModel
        private class ImportDataProgressViewModel : INotifyPropertyChanged
        {
            #region Properties
                        
            private readonly int _totalNumberOfTagsToImport;
            private readonly int _totalNumberOfItemsToImport;            

            private int tagsCounter;
            public int TagsCounter
            {
                get
                {
                    return this.tagsCounter;
                }
                set
                {
                    this.tagsCounter = value;
                    OnPropertyChanged(nameof(TagsCounter));
                }
            }

            private string tagName;
            public string TagName
            {
                get
                {
                    return this.tagName;
                }
                set
                {
                    this.tagName = value;
                    OnPropertyChanged(nameof(tagName));
                }
            }

            private float prototypesImportOverallProgress;
            public float PrototypesImportOverallProgress
            {
                get
                {
                    return this.prototypesImportOverallProgress;
                }
                set
                {
                    this.prototypesImportOverallProgress = value;
                    OnPropertyChanged(nameof(PrototypesImportOverallProgress));
                }
            }

            private bool prototypesImportOverallProgressIsIndeterminate;
            public bool PrototypesImportOverallProgressIsIndeterminate
            {
                get
                {
                    return this.prototypesImportOverallProgressIsIndeterminate;
                }
                set
                {
                    this.prototypesImportOverallProgressIsIndeterminate = value;
                    OnPropertyChanged(nameof(PrototypesImportOverallProgressIsIndeterminate));
                }
            }

            private float tagsImportOverallProgress;
            public float TagsImportOverallProgress
            {
                get
                {
                    return this.tagsImportOverallProgress;
                }
                set
                {
                    this.tagsImportOverallProgress = value;
                    OnPropertyChanged(nameof(TagsImportOverallProgress));
                }
            }

            private float importOverallProgress;
            public float ImportOverallProgress
            {
                get
                {
                    return this.importOverallProgress;
                }
                set
                {
                    this.importOverallProgress = value;
                    OnPropertyChanged(nameof(ImportOverallProgress));
                }
            }

            bool isAborting;
            public bool IsAborting
            {
                get
                {
                    return isAborting;
                }
                set
                {
                    if (isAborting == value)
                        return;
                    isAborting = value;
                    OnPropertyChanged(nameof(IsAborting));
                }
            }
            #endregion

            #region Constructors
            public ImportDataProgressViewModel(int totalNumberOfTagsToImport)
            {
                _totalNumberOfTagsToImport = totalNumberOfTagsToImport;
                _totalNumberOfItemsToImport = totalNumberOfTagsToImport;
            }
            #endregion

            #region Events
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string name = null)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }

            public event EventHandler NotifyTagsImportProgressWindowToHide;
            protected virtual void OnNotifyTagsImportProgressWindowToHide()
            {
                NotifyTagsImportProgressWindowToHide?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler NotifyTagsImportProgressWindowToAppear;
            protected virtual void OnNotifyTagsImportProgressWindowToAppear()
            {
                NotifyTagsImportProgressWindowToAppear?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler NotifyTagsImportProgressWindowToClose;
            protected virtual void OnNotifyTagsImportProgressWindowToClose()
            {
                NotifyTagsImportProgressWindowToClose?.Invoke(this, EventArgs.Empty);
            }

            #endregion

            #region Methods
            public void HideProgressBarImport()
            {
                OnNotifyTagsImportProgressWindowToHide();
            }

            public void ShowProgressBarImport()
            {
                OnNotifyTagsImportProgressWindowToAppear();
            }

            public void CloseProgressBarImport()
            {
                OnNotifyTagsImportProgressWindowToClose();
            }

            public void ImportNewTag(string tagName)
            {
                TagsCounter++;
                TagName = tagName;
                TagsImportOverallProgress = (TagsCounter * 100 / (float)_totalNumberOfTagsToImport);
                ImportOverallProgress = (TagsCounter) * 100 / (float)_totalNumberOfItemsToImport;
            }

            public void UpdateImportingTag(string tagName)
            {
                TagName = tagName;
            }
            #endregion
        }
        #endregion

        #region CTOR
        public ImportDataProgressBar()
        {            
        }
        #endregion

        #region Import Tags/Prototype Progress Bar

        // import progress bar management
        ImportDataProgressViewModel tagsImportProgressViewModel;
        CancellationTokenSource tokenSource = null;
        CancellationToken cancellationToken;

        public void StartImport(int totalNumberOfTagsToImport)
        {
            var tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;

            var waitEvent = new ManualResetEvent(false);

            tagsImportProgressViewModel = new ImportDataProgressViewModel(totalNumberOfTagsToImport);

            var threadImportProgressDialog = new Thread(() =>
            {
                var captions = GeneralDialogContent.GetDefaultButtonCaptions();
                captions[GeneralDialogButtons.CancelButton] = DriverCodeBaseEx.UI.Properties.Resources.TagsImportProgressCloseButtonLabel;

                var tagsImportProgressUserControl = new TagsImportProgress() { DataContext = tagsImportProgressViewModel };

                GeneralDialogContent tagsImportDialog = new GeneralDialogContent(
                    tagsImportProgressUserControl,
                    GeneralDialogButtons.CancelButton,
                    captions)
                {
                    DialogKeepContent = true,
                    Title = DriverCodeBaseEx.UI.Properties.Resources.TagsImportProgressTitle,
                };

                tagsImportDialog.SetIsTopmost(true);

                bool bIsHidden = false;
                bool bIsClosing = false;
                var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

                tagsImportProgressViewModel.NotifyTagsImportProgressWindowToClose += (o, e) =>
                {
                    dispatcher.InvokeIfRequired(() =>
                    {
                        bIsClosing = true;
                        tokenSource.Cancel();
                        tagsImportDialog.Close();
                        dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    });
                };

                tagsImportDialog.Closing += (o, e) =>
                {
                    e.Cancel = !bIsClosing;
                    if (!bIsHidden && !tokenSource.IsCancellationRequested)
                    {
                        tokenSource.Cancel();
                        tagsImportProgressViewModel.IsAborting = true;
                        tagsImportDialog.DisableCancelButton();
                    }
                };

                tagsImportProgressViewModel.NotifyTagsImportProgressWindowToHide += (o, e) =>
                {
                    dispatcher.InvokeIfRequired(() =>
                    {
                        bIsHidden = true;
                        tagsImportDialog.Hide();
                    });
                };

                tagsImportProgressViewModel.NotifyTagsImportProgressWindowToAppear += (o, e) =>
                {
                    dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                    {
                        bIsHidden = false;
                        tagsImportDialog.ShowDialog();
                    });
                };

                waitEvent.Set();
                tagsImportDialog.ShowDialog();

                System.Windows.Threading.Dispatcher.Run();
            });

            threadImportProgressDialog.SetApartmentState(ApartmentState.STA);
            threadImportProgressDialog.IsBackground = true;
            threadImportProgressDialog.Start();
            waitEvent.WaitOne();
            waitEvent.Dispose();
        }

        public void StopImport()
        {
            tagsImportProgressViewModel?.CloseProgressBarImport();            
            tokenSource?.Dispose();
        }

        public void ThrowIfCancellationRequested()
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        public void ImportNewTag(string tagName)
        {
            tagsImportProgressViewModel?.ImportNewTag(tagName);
        }

        public void UpdateImportingTag(string tagName)
        {
            tagsImportProgressViewModel?.UpdateImportingTag(tagName);
        }
        #endregion

        #region Dispose
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            StopImport();

            tagsImportProgressViewModel = null;
        }
        #endregion

    };
}
