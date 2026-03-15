using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xacc
{
    public delegate void RefObjectEventHandler(object sender, RefObjectEventArgs e);

    public class RefObjectEventArgs : EventArgs
    {
        private object _RefObject;
        private object _Value;

        public RefObjectEventArgs(object refobject, object value)
	    {
            this._RefObject = refobject;
	        this._Value = value;
	    }

        public object Value
        {
            get { return _Value; }
            set { this._Value = value; }
        }

        public object RefObject
        {
            get { return _RefObject; }
            set { this._RefObject = value; }
        }
    }
}
