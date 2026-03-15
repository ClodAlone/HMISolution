using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    [DataContract(Name = "ExtDataValue")]
    public class ExtDataValue : DataValue
    {
        Guid token;
        [DataMember]
        public Guid Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }

        public ExtDataValue(DataValue d, Guid t)
            : base(d)
        {
            token = t;
        }
    }
}
