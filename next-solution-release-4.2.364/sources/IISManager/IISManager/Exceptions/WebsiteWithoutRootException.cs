using System;
using System.Collections.Generic;
using System.Text;

namespace IISManager.Exceptions
{
    public class WebsiteWithoutRootException :Exception
    {
        private string _websiteName = "";

        public WebsiteWithoutRootException(string Website)
        {
            this._websiteName = Website;
        }

        public override string Message
        {
            get
            {
                string msg = "Website has no root. Website:" + _websiteName;
                return msg;
            }
        }
    }
}
