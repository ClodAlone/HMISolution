using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;

namespace IISManager.Exceptions
{
    public class VirtualDirAlreadyExistException : Exception
    {
        private string _dir = "";

        public VirtualDirAlreadyExistException(DirectoryEntry entry, string vDir)
        {
            this._dir = entry.Path;
            if (this._dir[this._dir.Length - 1] != '/')
            {
                this._dir = this._dir + "/";
            }

            this._dir = this._dir + vDir;
        }

        public override string Message
        {
            get
            {
                string msg = "Virtual Dir already exist:" + this._dir;
                return msg;
            }
        }
    }
}
