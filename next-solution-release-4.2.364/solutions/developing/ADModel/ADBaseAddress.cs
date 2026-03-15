using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;

namespace ADModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("ADBaseAddressEx")]
    public class ADBaseAddress : UFUAModel.AddressBase
    {
        public ADBaseAddress(Session session)
            : base(session)
        { }
    }
}
