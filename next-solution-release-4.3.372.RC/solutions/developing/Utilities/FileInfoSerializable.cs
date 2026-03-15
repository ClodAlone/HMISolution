using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class FileInfoSerializable
    {
        #region public Props
        public string FullName
        {
            get;
        }
        public long Length 
        {
            get;
        }
        #endregion
        #region ctor
        public FileInfoSerializable(string fullName, long length)
        {
            FullName = fullName;
            Length = length;
        }
        #endregion
    }
}
