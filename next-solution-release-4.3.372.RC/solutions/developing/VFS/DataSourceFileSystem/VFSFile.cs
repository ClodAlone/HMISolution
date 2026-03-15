using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;

namespace VFS
{
    public class VFSFile : XPObject
    {
        public VFSFile(Session session)
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

        private VFSFolder _VFSFolderAss;
        [Association("VFSFolder-Files")]
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

        [Delayed(true)]
        public byte[] Data
        {
            get { return GetDelayedPropertyValue<byte[]>("Data"); }
            set { SetDelayedPropertyValue<byte[]>("Data", value); }
        }
    }
}
