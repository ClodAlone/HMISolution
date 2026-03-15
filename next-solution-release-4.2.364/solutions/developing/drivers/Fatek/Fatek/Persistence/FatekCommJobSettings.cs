using DriverCodeBase;
using DevExpress.Xpo;

namespace Fatek
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FatekCommJobSettings : CommJobSettings
    {
        #region Constructors

        public FatekCommJobSettings(Session session, FatekCommJob job)
            : base(session, job)
        {
            _StartAddress = job.StartAddress;
            //_StringLength = job.StringLength;
        }

        public FatekCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected FatekCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StartAddress = string.Empty;
            //_SwapDWords = false;
            //_StringLength = 32;
        }

        #region Properties        
        private string _StartAddress;
        public string StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                SetPropertyValue("StartAddress", ref _StartAddress, value);
            }
        }        
        //private bool _SwapDWords;
        //public bool SwapDWords
        //{
        //    get { return _SwapDWords; }
        //    set
        //    {
        //        SetPropertyValue("SwapDWords", ref _SwapDWords, value);
        //    }
        //}
        //private uint _StringLength;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   String Length of memory area Property. </summary>
        /////
        ///// <value> The String Length. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public uint StringLength
        //{
        //    get { return _StringLength; }
        //    set { SetPropertyValue("String Length", ref _StringLength, value); }
        //}
        #endregion

        #region IDataErrorInfo Members
        #endregion
    
    }
}
