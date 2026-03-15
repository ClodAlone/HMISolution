using DevExpress.Xpo;
using System;

namespace XpoHelpers
{
    public interface IXpoDocument
    {
        IDataLayer GetDataLayer();
    }
}
