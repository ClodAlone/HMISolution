using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UFUAEditor.Controls
{
    public class TagsImportProgressViewModel : INotifyPropertyChanged
    {
        #region Properties

        private readonly int _totalNumberOfPrototypesToImport;
        private readonly int _totalNumberOfTagsToImport;
        private readonly int _totalNumberOfItemsToImport;
       
        private int prototypesCounter;
        public int PrototypesCounter {
            get
            {
                return this.prototypesCounter;
            }
            set
            {
                this.prototypesCounter = value;
                OnPropertyChanged(nameof(PrototypesCounter));
            }
        }
        
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

        private float tagsImportOverallProgress;
        public float TagsImportOverallProgress 
        {
            get
            {
                return this.tagsImportOverallProgress;
            }
            set
            {
                this.tagsImportOverallProgress= value;
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
        public TagsImportProgressViewModel(int totalNumberOfPrototypesToImport, int totalNumberOfTagsToImport)
        {
            _totalNumberOfPrototypesToImport = totalNumberOfPrototypesToImport;
            _totalNumberOfTagsToImport = totalNumberOfTagsToImport;
            _totalNumberOfItemsToImport = totalNumberOfPrototypesToImport + totalNumberOfTagsToImport;
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

        public event EventHandler NotifyToDisableAbortButton;
        protected virtual void OnDisableAbortButton()
        {
            NotifyToDisableAbortButton?.Invoke(this, EventArgs.Empty);
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

        public void DisableAbortButton()
        {
            OnDisableAbortButton();
        }

        public void SetPrototypesCounterValue(int value)
        {
            PrototypesCounter = value;
            PrototypesImportOverallProgress = PrototypesCounter * 100 / _totalNumberOfPrototypesToImport;
            ImportOverallProgress = SumPrototypesAndTagsCounters(PrototypesCounter, TagsCounter) * 100 / (float)_totalNumberOfItemsToImport;
        }

        public void SetTagsCounterValue(int value)
        {
            TagsCounter = value;
            TagsImportOverallProgress = (TagsCounter * 100 / (float)_totalNumberOfTagsToImport);
            ImportOverallProgress = SumPrototypesAndTagsCounters(PrototypesCounter, TagsCounter) * 100 / (float)_totalNumberOfItemsToImport;
        }

        private int SumPrototypesAndTagsCounters(int prototypesCounter, int tagsCounter)
        {
            return prototypesCounter + tagsCounter;
        }
        #endregion
    }
}
