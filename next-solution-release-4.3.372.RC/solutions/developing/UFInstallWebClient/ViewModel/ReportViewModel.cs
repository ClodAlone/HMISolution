using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInstallWebClient.ViewModel
{
    public class ReportViewModel: ViewModelBase
    {
        public string[] Users
        {
            get { return _users; }            
        }
        private string[] _users;

        public string[] Roles
        {
            get { return _roles; }            
        }
        private string[] _roles;

        public string[] Reports
        {
            get { return _reports; }
        }
        private string[] _reports;

        public string Report
        {
            get
            {
                return _report;
            }
            set
            {
                _report = value;
                RaisePropertyChanged(nameof(Report));
            }
        }
        private string _report;

        public string SelectedUsers
        {
            get
            {
                return _selectedUsers;
            }
            set
            {
                _selectedUsers = value;
                RaisePropertyChanged(nameof(SelectedUsers));
            }
        }
        private string _selectedUsers;

        public string SelectedRoles
        {
            get
            {
                return _selectedRoles;
            }
            set
            {
                _selectedRoles = value;
                RaisePropertyChanged(nameof(SelectedRoles));
            }
        }
        private string _selectedRoles;
        
        public ReportViewModel(string[] users, string[] roles, string[] reports)
        {
            _users = users;
            _roles = roles;
            _reports = reports;
        }
    }
}
