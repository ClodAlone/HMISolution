using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace NextLeanLicense
{

    [DeferredDeletion(false)]
    [OptimisticLocking(false)]
    public class TbOptimistic : XPCustomObject
    {
        #region Constructors
        public TbOptimistic(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        private int _Id;
        [Key(true)]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                SetPropertyValue("Id", ref _Id, value);
            }
        }

        private Byte[] _ConstPerf;
        [Size(255)]
        public Byte[] ConstPerf
        {
            get
            {
                return _ConstPerf;
            }
            set
            {
                SetPropertyValue("ConstPerf", ref _ConstPerf, value);
            }
        }
        private int _ProgNum;
        public int ProgNum
        {
            get
            {
                return _ProgNum;
            }
            set
            {
                SetPropertyValue("ProgNum", ref _ProgNum, value);
            }
        }

        #endregion
    }
}
