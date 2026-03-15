using System;
using System.Collections.Generic;
using System.Text;

namespace IISManager.Exceptions
{
    public class FileNotFoundException : Exception
    {
         private string _file = "";

        public FileNotFoundException(string file)
        {
            this._file = file;
        }

        public override string Message
        {
            get
            {
                string msg = "File not found:" + this._file;
                return msg;
            }
        }
    }
}
