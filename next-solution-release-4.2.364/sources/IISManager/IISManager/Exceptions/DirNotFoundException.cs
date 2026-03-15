using System;
using System.Collections.Generic;
using System.Text;

namespace IISManager.Exceptions
{
    /// <summary>
    /// Directory not found or illegal.
    /// </summary>
    public class DirNotFoundException : Exception
    {
        private string _dir = "";

        public DirNotFoundException(string dir)
        {
            this._dir = dir;
        }

        public override string Message
        {
            get
            {
                return "Directory not found:" + _dir;
            }
        }
    }
}
