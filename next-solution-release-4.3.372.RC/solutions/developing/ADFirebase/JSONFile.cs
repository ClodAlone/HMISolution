using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADFirebase
{
    public class JSONFile: XPObject
    {
        #region Constructors

        public JSONFile(Session session)
            :base (session)
        {

        }
        protected JSONFile()
        {
            
        }

        #endregion  

        private byte[] _FileBody = null;
        public byte[] FileBody
        {
            get
            {
                return _FileBody;
            }
            set
            {
                SetPropertyValue("FileBody", ref _FileBody, value);
            }
        }
    }

}
