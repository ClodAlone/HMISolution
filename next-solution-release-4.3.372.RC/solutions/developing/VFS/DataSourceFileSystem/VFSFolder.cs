using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;

namespace VFS
{
    public class VFSFolder : XPObject
    {
        public VFSFolder(Session session)
            : base(session)
        { }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private DateTime _LastWriteTime;
        public DateTime LastWriteTime
        {
            get
            {
                return _LastWriteTime;
            }
            set
            {
                SetPropertyValue("LastWriteTime", ref _LastWriteTime, value);
            }
        }

        [Association("VFSFolder-Folders"), Aggregated]
        public XPCollection<VFSFolder> Folders
        {
            get
            {
                return GetCollection<VFSFolder>("Folders");
            }
        }

        private VFSFolder _VFSFolderAss;
        [Association("VFSFolder-Folders")]
        public VFSFolder VFSFolderAss
        {
            get
            {
                return _VFSFolderAss;
            }
            set
            {
                SetPropertyValue("VFSFolderAss", ref _VFSFolderAss, value);
            }
        }

        [Association("VFSFolder-Files"), Aggregated]
        public XPCollection<VFSFile> Files
        {
            get
            {
                return GetCollection<VFSFile>("Files");
            }
        }
    }
}
