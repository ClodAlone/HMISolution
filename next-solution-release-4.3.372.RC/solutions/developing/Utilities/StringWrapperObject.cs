using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class StringWrapperObject : Observable
    {
        #region Constructors
        public StringWrapperObject() : 
            this(String.Empty)
        { }

        public StringWrapperObject(String value)
        {
            this.value = value;
        }
        #endregion

        #region Properties

        String value;
        public String Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value == value)
                    return;

                this.value = value;
                OnPropertyChanged("Value");
            }
        }
        #endregion
    }
}
