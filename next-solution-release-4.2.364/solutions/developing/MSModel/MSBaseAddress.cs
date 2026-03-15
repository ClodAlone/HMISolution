using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;

namespace MSModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("MSBaseAddressEx")]
    public class MSBaseAddress : UFUAModel.AddressBase
    {
        public MSBaseAddress(Session session)
            : base(session)
        { }
    }
}
