using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using ViewModelLib;
using System.ComponentModel;
using System.Windows.Input;

namespace UFProjectManager
{
    class UriModel : Observable
    {
        readonly Uri uri;

        public event EventHandler deleteCommand;
        public event EventHandler openCommand;
        public event EventHandler renameCommand;
        void OnDeleteCommand(EventArgs ea)
        {
            var e = deleteCommand;
            if (e != null)
                e(this, ea);
        }
        void OnOpenCommand(EventArgs ea)
        {
            var e = openCommand;
            if (e != null)
                e(this, ea);
        }
        void OnRenameCommand(EventArgs ea)
        {
            var e = renameCommand;
            if (e != null)
                e(this, ea);
        }

        public UriModel(Uri u)
        {
            uri = u;
        }

        public Uri Uri
        {
            get
            {
                return uri;
            }
        }

        public String Title
        {
            get
            {
                //if (uri.IsAbsoluteUri)
                //    return System.IO.Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                //else
#if !WINDOWS_UWP && !NET_STANDARD
                var path = uri.GetPathString();
                if (XpoHelpers.XpoHelper.IsDataSource(path))
                    return XpoHelpers.XpoHelper.GetDataSourceTitle(path);
#endif
                return System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
            }
        }

        RelayCommand _deleteCommand;
        [Browsable(false)]
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => OnDeleteCommand(EventArgs.Empty)
                        );
                }
                return _deleteCommand;
            }
        }

        RelayCommand _openCommand;
        [Browsable(false)]
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(
                        param => OnOpenCommand(EventArgs.Empty)
                        );
                }
                return _openCommand;
            }
        }

        RelayCommand _renameCommand;
        [Browsable(false)]
        public ICommand RenameCommand
        {
            get
            {
                if (_renameCommand == null)
                {
                    _renameCommand = new RelayCommand(
                        param => OnRenameCommand(EventArgs.Empty)
                        );
                }
                return _renameCommand;
            }
        }
    }
}
