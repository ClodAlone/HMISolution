using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Administration;

namespace IIS7Manager.Exceptions
{
    public class VirtualDirAlreadyExistException : Exception
    {
        private string _dir = "";

        public VirtualDirAlreadyExistException(IISWebVirturalDir entry, string vDir)
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
