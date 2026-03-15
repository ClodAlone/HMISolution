using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Utilities;

namespace UFProjectManager.Controls
{
    public class DeployClientVM : INotifyPropertyChanged
    {
        bool isInError;
        bool isConnected;
        bool isUploading;
        bool canDownload;
        bool canScanProjects;
        bool isDownloading;
        bool isWebHMIRunning;
        bool isDataServerRunning;
        bool isRecipesServerRunning;
        bool isSchedulerServerRunning;
        bool isLogicsServerRunning;
        bool isADServerRunning;
        bool isBrowserRunning;
        bool isWebHMIStarting;
        bool isDataServerStarting;
        bool isRecipesServerStarting;
        bool isSchedulerServerStarting;
        bool isLogicsServerStarting;
        bool isADServerStarting;
        bool isBrowserStarting;
        bool isDataServerInstalled;
        bool isWebHMIServerInstalled;
        bool isRecipesServerInstalled;
        bool isLogicsServerInstalled;
        bool isADServerInstalled;
        bool isSchedulerServerInstalled;
        bool isDataServerNeeded;
        bool isSchedulerServerNeeded;
        bool isRecipesServerNeeded;
        bool isLogicsServerNeeded;
        bool isADServerNeeded;
        DeployClientProfile activeProfile;
        int totalFilesUploadProgress;
        int totalFilesDownloadProgress;
        int singleFileUploadProgress;
        string defaultStatusDisplayString = "{0}%";
        string uploadStatusDisplayString;
        string downloadStatusDisplayString;
        string uploadingFile;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Public Props
        public DeployProfiles DeployProfiles { get; set; } = new DeployProfiles();
        public DeployClientProfile ActiveProfile
        {
            get
            {
                return activeProfile;
            }
            set
            {
                if (value != activeProfile)
                {
                    activeProfile = value;
                    OnPropertyChanged("ActiveProfile");
                }
            }
        }
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                if (value != isConnected)
                {
                    if (value)
                    {
                        ApplicationPropertiesHelper.SetProperty("LastConnectedDeployProfile", activeProfile.Name);
                        UploadStatusDisplayString = defaultStatusDisplayString;
                        SingleFileUploadProgress = TotalFilesUploadProgress = TotalFilesDownloadProgress = 0;
                    }
                    isConnected = value;
                    IsWebHMIServerInstalled = false;
                    CanDownload = false;
                    OnPropertyChanged("IsConnected");
                    OnPropertyChanged("CanEditProfiles");
                }
            }
        }
        public bool IsUploading
        {
            get
            {
                return isUploading;
            }
            set
            {
                if (value != isUploading)
                {
                    isUploading = value;
                    OnPropertyChanged("IsUploading");
                }
            }
        }
        public bool CanDownload
        {
            get
            {
                return canDownload;
            }
            set
            {
                if (value != canDownload)
                {
                    canDownload = value;
                    OnPropertyChanged("CanDownload");
                }
            }
        }
        public bool CanScanProjects
        {
            get
            {
                return canScanProjects;
            }
            set
            {
                if (value != canScanProjects)
                {
                    canScanProjects = value;
                    OnPropertyChanged("CanScanProjects");
                }
            }
        }
        public bool IsDownloading
        {
            get
            {
                return isDownloading;
            }
            set
            {
                if (value != isDownloading)
                {
                    isDownloading = value;
                    OnPropertyChanged("IsDownloading");
                }
            }
        }
        public bool IsInError
        {
            get
            {
                return isInError;
            }
            set
            {
                if (value != isInError)
                {
                    isInError = value;
                    OnPropertyChanged("IsInError");
                    OnPropertyChanged("CanEditProfiles");
                }
            }
        }
        public bool CanEditProfiles
        {
            get
            {
                return !IsInError && !IsConnected;
            }
        }
        public bool IsAllServersStarted
        {
            get
            {
                return
                    (IsDataServerRunning || (!IsDataServerNeeded || !IsDataServerInstalled)) &&
                    (IsLogicsServerRunning || (!IsLogicsServerNeeded || !IsLogicsServerInstalled)) &&
                    (IsRecipesServerRunning || (!IsRecipesServerNeeded || !IsRecipesServerInstalled)) &&
                    (IsSchedulerServerRunning || (!IsSchedulerServerNeeded || !IsSchedulerServerInstalled)) &&
                    (IsADServerRunning || (!IsADServerNeeded || !IsADServerInstalled)) &&
                    (IsWebHMIRunning || !IsWebHMIServerInstalled);
            }
        }
        public bool IsWebHMIRunning
        {
            get
            {
                return isWebHMIRunning;
            }
            set
            {
                if (value != isWebHMIRunning)
                {
                    IsWebHMIStarting = false;
                    isWebHMIRunning = value;
                    OnPropertyChanged("IsWebHMIRunning");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsDataServerRunning
        {
            get
            {
                return isDataServerRunning;
            }
            set
            {
                if (value != isDataServerRunning)
                {
                    IsDataServerStarting = false;
                    isDataServerRunning = value;
                    OnPropertyChanged("IsDataServerRunning");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsRecipesServerRunning
        {
            get
            {
                return isRecipesServerRunning;
            }
            set
            {
                if (value != isRecipesServerRunning)
                {
                    IsRecipesServerStarting = false;
                    isRecipesServerRunning = value;
                    OnPropertyChanged("IsRecipesServerRunning");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsSchedulerServerRunning
        {
            get
            {
                return isSchedulerServerRunning;
            }
            set
            {
                if (value != isSchedulerServerRunning)
                {
                    IsSchedulerServerStarting = false;
                    isSchedulerServerRunning = value;
                    OnPropertyChanged("IsSchedulerServerRunning");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsLogicsServerRunning
        {
            get
            {
                return isLogicsServerRunning;
            }
            set
            {
                if (value != isLogicsServerRunning)
                {
                    IsLogicsServerStarting = false;
                    isLogicsServerRunning = value;
                    OnPropertyChanged("IsLogicsServerRunning");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsADServerRunning
        {
            get
            {
                return isADServerRunning;
            }
            set
            {
                if (value != isADServerRunning)
                {
                    IsADServerStarting = false;
                    isADServerRunning = value;
                    OnPropertyChanged("IsADServerRunning");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsBrowserRunning
        {
            get
            {
                return isBrowserRunning;
            }
            set
            {
                if (value != isBrowserRunning)
                {
                    IsBrowserStarting = false;
                    isBrowserRunning = value;
                    OnPropertyChanged("IsBrowserRunning");
                }
            }
        }
        public bool IsWebHMIStarting
        {
            get
            {
                return isWebHMIStarting;
            }
            set
            {
                if (value != isWebHMIStarting)
                {
                    isWebHMIStarting = value;
                    OnPropertyChanged("IsWebHMIStarting");
                }
            }
        }
        public bool IsDataServerStarting
        {
            get
            {
                return isDataServerStarting;
            }
            set
            {
                if (value != isDataServerStarting)
                {
                    isDataServerStarting = value;
                    OnPropertyChanged("IsDataServerStarting");
                }
            }
        }
        public bool IsRecipesServerStarting
        {
            get
            {
                return isRecipesServerStarting;
            }
            set
            {
                if (value != isRecipesServerStarting)
                {
                    isRecipesServerStarting = value;
                    OnPropertyChanged("IsRecipesServerStarting");
                }
            }
        }
        public bool IsSchedulerServerStarting
        {
            get
            {
                return isSchedulerServerStarting;
            }
            set
            {
                if (value != isSchedulerServerStarting)
                {
                    isSchedulerServerStarting = value;
                    OnPropertyChanged("IsSchedulerServerStarting");
                }
            }
        }
        public bool IsLogicsServerStarting
        {
            get
            {
                return isLogicsServerStarting;
            }
            set
            {
                if (value != isLogicsServerStarting)
                {
                    isLogicsServerStarting = value;
                    OnPropertyChanged("IsLogicsServerStarting");
                }
            }
        }
        public bool IsADServerStarting
        {
            get
            {
                return isADServerStarting;
            }
            set
            {
                if (value != isADServerStarting)
                {
                    isADServerStarting = value;
                    OnPropertyChanged("IsADServerStarting");
                }
            }
        }
        public bool IsBrowserStarting
        {
            get
            {
                return isBrowserStarting;
            }
            set
            {
                if (value != isBrowserStarting)
                {
                    isBrowserStarting = value;
                    OnPropertyChanged("IsBrowserStarting");
                }
            }
        }
        public bool IsDataServerInstalled
        {
            get
            {
                return isDataServerInstalled;
            }
            set
            {
                if (value != isDataServerInstalled)
                {
                    isDataServerInstalled = value;
                    OnPropertyChanged("IsDataServerInstalled");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsWebHMIServerInstalled
        {
            get
            {
                return isWebHMIServerInstalled;
            }
            set
            {
                if (value != isWebHMIServerInstalled)
                {
                    isWebHMIServerInstalled = value;
                    OnPropertyChanged("IsWebHMIServerInstalled");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsRecipesServerInstalled 
        {
            get
            {
                return isRecipesServerInstalled;
            }
            set
            {
                if (value != isRecipesServerInstalled)
                {
                    isRecipesServerInstalled = value;
                    OnPropertyChanged("IsRecipesServerInstalled");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsLogicsServerInstalled 
        {
            get
            {
                return isLogicsServerInstalled;
            }
            set
            {
                if (value != isLogicsServerInstalled)
                {
                    isLogicsServerInstalled = value;
                    OnPropertyChanged("IsLogicsServerInstalled");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsSchedulerServerInstalled 
        {
            get
            {
                return isSchedulerServerInstalled;
            }
            set
            {
                if (value != isSchedulerServerInstalled)
                {
                    isSchedulerServerInstalled = value;
                    OnPropertyChanged("IsSchedulerServerInstalled");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsADServerInstalled
        {
            get
            {
                return isADServerInstalled;
            }
            set
            {
                if (value != isADServerInstalled)
                {
                    isADServerInstalled = value;
                    OnPropertyChanged("IsADServerInstalled");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsDataServerNeeded
        {
            get
            {
                return isDataServerNeeded;
            }
            set
            {
                if (value != isDataServerNeeded)
                {
                    isDataServerNeeded = value;
                    OnPropertyChanged("IsDataServerNeeded");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsSchedulerServerNeeded
        {
            get
            {
                return isSchedulerServerNeeded;
            }
            set
            {
                if (value != isSchedulerServerNeeded)
                {
                    isSchedulerServerNeeded = value;
                    OnPropertyChanged("IsSchedulerServerNeeded");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsRecipesServerNeeded
        {
            get
            {
                return isRecipesServerNeeded;
            }
            set
            {
                if (value != isRecipesServerNeeded)
                {
                    isRecipesServerNeeded = value;
                    OnPropertyChanged("IsRecipesServerNeeded");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsLogicsServerNeeded
        {
            get
            {
                return isLogicsServerNeeded;
            }
            set
            {
                if (value != isLogicsServerNeeded)
                {
                    isLogicsServerNeeded = value;
                    OnPropertyChanged("IsLogicsServerNeeded");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public bool IsADServerNeeded
        {
            get
            {
                return isADServerNeeded;
            }
            set
            {
                if (value != isADServerNeeded)
                {
                    isADServerNeeded = value;
                    OnPropertyChanged("IsADServerNeeded");
                    OnPropertyChanged("IsAllServersStarted");
                }
            }
        }
        public int SingleFileUploadProgress
        {
            get
            {
                return singleFileUploadProgress;
            }
            set
            {
                if (value != singleFileUploadProgress)
                {
                    singleFileUploadProgress = value;
                    OnPropertyChanged("SingleFileUploadProgress");
                }
            }
        }
        public int TotalFilesUploadProgress
        {
            get
            {
                return totalFilesUploadProgress;
            }
            set
            {
                if (value != totalFilesUploadProgress)
                {
                    totalFilesUploadProgress = value;
                    OnPropertyChanged("TotalFilesUploadProgress");
                }
            }
        }
        public int TotalFilesDownloadProgress
        {
            get
            {
                return totalFilesDownloadProgress;
            }
            set
            {
                if (value != totalFilesDownloadProgress)
                {
                    totalFilesDownloadProgress = value;
                    OnPropertyChanged("TotalFilesDownloadProgress");
                }
            }
        }
        public string UploadStatusDisplayString
        {
            get
            {
                return String.IsNullOrEmpty(uploadStatusDisplayString) ? defaultStatusDisplayString : uploadStatusDisplayString;
            }
            set
            {
                if (value != uploadStatusDisplayString)
                {
                    uploadStatusDisplayString = value;
                    OnPropertyChanged("UploadStatusDisplayString");
                }
            }
        }
        public string DownloadStatusDisplayString
        {
            get
            {
                return String.IsNullOrEmpty(downloadStatusDisplayString) ? defaultStatusDisplayString : downloadStatusDisplayString;
            }
            set
            {
                if (value != downloadStatusDisplayString)
                {
                    downloadStatusDisplayString = value;
                    OnPropertyChanged("DownloadStatusDisplayString");
                }
            }
        }
        public string UploadingFileLabelText
        {
            get
            {
                return String.IsNullOrEmpty(uploadingFile) ? Properties.Resources.FileUploadingDefaultLabel : String.Format(Properties.Resources.UploadingFile, uploadingFile);
            }
            set
            {
                if (uploadingFile != value)
                {
                    uploadingFile = value;
                    OnPropertyChanged("UploadingFileLabelText");
                }
            }
        }
        #endregion

        #region Ctor
        public DeployClientVM()
        {
            DeployProfiles.Profiles.CollectionChanged += (o, e) =>
            {
                OnPropertyChanged("DeployProfiles");
            };
        }
        #endregion

        #region Methods
        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }

    [DataContract(Name = "DeployProfiles")]
    public class DeployProfiles : IDisposable
    {
        ObservableCollection<DeployClientProfile> profiles;
        [DataMember]
        public ObservableCollection<DeployClientProfile> Profiles { 
            get 
            {
                return profiles;
            }
            set
            {
                if (profiles != value)
                {
                    if (profiles != null)
                    {
                        foreach (var profile in profiles)
                        {
                            profile.PropertyChanged -= OnProfilePropertyChanged;
                        }
                        profiles.CollectionChanged -= OnProfilesCollectionChanged;
                    }
                    profiles = value;
                    if (value != null)
                    {
                        foreach (var profile in profiles)
                        {
                            profile.PropertyChanged += OnProfilePropertyChanged;
                        }
                        profiles.CollectionChanged += OnProfilesCollectionChanged;
                    }
                }
            } 
        }
        public bool bNeedsSave = false;
        bool bSorting = false;
        bool bDisposed;

        public DeployProfiles()
        {
            Profiles = new ObservableCollection<DeployClientProfile>();
        }

        public DeployProfiles(DeployClientProfile defaultProfile)
        {
            Profiles = new ObservableCollection<DeployClientProfile>() { defaultProfile };
        }

        void OnProfilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!bSorting)
                bNeedsSave = true;

            if (e.NewItems != null)
                foreach (INotifyPropertyChanged added in e.NewItems)
                {
                    added.PropertyChanged += OnProfilePropertyChanged;
                }

            if (e.OldItems != null)
                foreach (INotifyPropertyChanged removed in e.OldItems)
                {
                    removed.PropertyChanged -= OnProfilePropertyChanged;
                }
        }

        void OnProfilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            bNeedsSave = true;
        }

        public DeployClientProfile FindProfileByName(string name)
        {
            return (from p in Profiles where p.Name == name select p).FirstOrDefault();
        }

        public void Sort()
        {
            bSorting = true;
            List<DeployClientProfile> sorted = Profiles.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                Profiles.Move(Profiles.IndexOf(sorted[i]), i);
            bSorting = false;
        }

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (profiles != null)
            {
                profiles.CollectionChanged -= OnProfilesCollectionChanged;
                foreach (var profile in profiles)
                {
                    profile.PropertyChanged -= OnProfilePropertyChanged;
                }
            }
        }
    }
}
