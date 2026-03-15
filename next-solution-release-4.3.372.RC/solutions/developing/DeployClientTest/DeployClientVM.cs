using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DeployClientTest
{
    public class DeployClientVM : INotifyPropertyChanged
    {
        bool isInError;
        bool isConnected;
        DeployClientProfile activeProfile;
        int totalFilesUploadProgress;
        int singleFileUploadProgress;
        string defaultStatusDisplayString = "{0}%";
        string uploadStatusDisplayString;
        string uploadingFile;

        public event PropertyChangedEventHandler PropertyChanged;
        public DeployProfiles DeployProfiles { get; set; }
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
                        UploadStatusDisplayString = defaultStatusDisplayString;
                        SingleFileUploadProgress = TotalFilesUploadProgress = 0;
                    }
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }
        public bool IsInError {
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

        public DeployClientVM()
        {
            DeployProfiles = new DeployProfiles(new DeployClientProfile("TestProfile", "https://localhost:44319", "admin@progea.com", "Ciccio@bello1"));
            DeployProfiles.Profiles.CollectionChanged += (o, e) =>
            {
                OnPropertyChanged("DeployProfiles");
            };
        }

        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

    [DataContract(Name = "DeployProfiles")]
    public class DeployProfiles /*: INotifyPropertyChanged*/
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public ObservableCollection<DeployClientProfile> Profiles { get; set; }

        public DeployProfiles()
        {
            Profiles = new ObservableCollection<DeployClientProfile>()
            {
                new DeployClientProfile("TestProfile", "https://localhost:44319", "admin@progea.com", "Ciccio@bello1")
            };
            //Profiles.CollectionChanged += (o, e) =>
            //{
            //    OnPropertyChanged("Profiles");
            //};
        }

        public DeployProfiles(DeployClientProfile defaultProfile)
        {
            Profiles = new ObservableCollection<DeployClientProfile>() { defaultProfile };
            //Profiles.CollectionChanged += (o, e) =>
            //{
            //    OnPropertyChanged("Profiles");
            //};
        }

        public DeployClientProfile FindProfileByName(string name)
        {
            return (from p in Profiles where p.Name == name select p).FirstOrDefault();
        }

        //void OnPropertyChanged(string propName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //}
    }
}
