using DevExpress.Core;
using System.Collections.ObjectModel;

namespace NextAR.ViewModel
{
    /// <summary>
    /// A View Model for the OverviewPage.
    /// </summary>
    public class OverViewPageViewModel : BindableBase
    {
        public OverViewPageViewModel()
        {
            FirstName = "NExT";
            LastName = "Augmented Reality";
        }
        ObservableCollection<PhoneNumber> phoneNumbersCore;
        public ObservableCollection<PhoneNumber> PhoneNumbers
        {
            get
            {
                return phoneNumbersCore;
            }
            set
            {
                SetProperty(ref phoneNumbersCore, value, (s1, s2) => { OnPropertyChanged("PhoneNumbers"); });
            }
        }
        ObservableCollection<Email> emailsCore;
        public ObservableCollection<Email> Emails
        {
            get
            {
                return emailsCore;
            }
            set
            {
                SetProperty(ref emailsCore, value, (s1, s2) => { OnPropertyChanged("Emails"); });
            }
        }
        string photo = "ms-appx:///Assets/Logo.png";
        public string Photo { get { return photo; } }
        string bio = "This Augmented Reality Application developed by Progea srl lets you play with the real world and helps you navigate through QRCodes too.";
        public string Bio { get { return bio; } }
        string firstNameCore;
        public string FirstName
        {
            get { return firstNameCore; }
            set
            {
                SetProperty(ref firstNameCore, value, (s1, s2) =>
                {
                    OnPropertyChanged("DetailPageHeader");
                    OnPropertyChanged("FullName");
                });
            }
        }
        string lastNameCore;
        public string LastName
        {
            get { return lastNameCore; }
            set
            {
                SetProperty(ref lastNameCore, value, (s1, s2) =>
                {
                    OnPropertyChanged("DetailPageHeader");
                    OnPropertyChanged("FullName");
                });
            }
        }
        public string DetailPageHeader
        {
            get
            {
                return string.Format("{0} {1} Details", FirstName, LastName);
            }
        }
        public string FullName
        {
            get { return string.Join(", ", LastName, FirstName); }
        }
        public string OfficePhone { get; set; }
        public string HomePhone { get; set; }
    }
    public class Contact
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Contact(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public sealed class PhoneNumber : BindableBase
    {
        public bool IsRequired { get; internal set; }
        string typeCore;
        public string Type
        {
            get { return typeCore; }
            set { SetProperty(ref typeCore, value); }
        }
        string valueCore;
        public string Value
        {
            get { return valueCore; }
            set { SetProperty(ref valueCore, value); }
        }
    }
    public sealed class Email : BindableBase
    {
        public bool IsRequired { get; internal set; }
        string typeCore;
        public string Type
        {
            get { return typeCore; }
            set { SetProperty(ref typeCore, value); }
        }
        string valueCore;
        public string Value
        {
            get { return valueCore; }
            set { SetProperty(ref valueCore, value); }
        }
    }
}
