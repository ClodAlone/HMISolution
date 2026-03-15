using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace MSZUtils
{

    [DeferredDeletion(false)]
    [OptimisticLocking(false)]
    public class tbLicenze : XPCustomObject
    {
        #region Constructors
        public tbLicenze(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        private int _IdLicenza;
        [Key(true)]
        public int IdLicenza
        {
            get
            {
                return _IdLicenza;
            }
            set
            {
                SetPropertyValue("IdLicenza", ref _IdLicenza, value);
            }
        }

        private int _AnagrafeID;
        public int AnagrafeID
        {
            get
            {
                return _AnagrafeID;
            }
            set
            {
                SetPropertyValue("AnagrafeID", ref _AnagrafeID, value);
            }
        }

        private string _CodiceCliente;
        [Size(10)]
        public string CodiceCliente
        {
            get
            {
                return _CodiceCliente;
            }
            set
            {
                SetPropertyValue("CodiceCliente", ref _CodiceCliente, value);
            }
        }

        private int _NumeroLicenza;
        public int NumeroLicenza
        {
            get
            {
                return _NumeroLicenza;
            }
            set
            {
                SetPropertyValue("NumeroLicenza", ref _NumeroLicenza, value);
            }
        }

        private string _RemovedCode;
        public string RemovedCode
        {
            get
            {
                return _RemovedCode;
            }
            set
            {
                SetPropertyValue("RemovedCode", ref _RemovedCode, value);
            }
        }

        private string _SiteCode;
        [Size(50)]
        public string SiteCode
        {
            get
            {
                return _SiteCode;
            }
            set
            {
                SetPropertyValue("SiteCode", ref _SiteCode, value);
            }
        }

        private string _SoftKey;
        [Size(SizeAttribute.Unlimited)]
        public string SoftKey
        {
            get
            {
                return _SoftKey;
            }
            set
            {
                SetPropertyValue("SoftKey", ref _SoftKey, value);
            }
        }

        private string _UserNameGenerazioneSoftKey;
        [Size(250)]
        public string UserNameGenerazioneSoftKey
        {
            get
            {
                return _UserNameGenerazioneSoftKey;
            }
            set
            {
                SetPropertyValue("UserNameGenerazioneSoftKey", ref _UserNameGenerazioneSoftKey, value);
            }
        }

        private DateTime _DataGenerazioneSoftKey;
        public DateTime DataGenerazioneSoftKey
        {
            get
            {
                return _DataGenerazioneSoftKey;
            }
            set
            {
                SetPropertyValue("DataGenerazioneSoftKey", ref _DataGenerazioneSoftKey, value);
            }
        }
        private string _Ordine;
        [Size(50)]
        public string Ordine
        {
            get
            {
                return _Ordine;
            }
            set
            {
                SetPropertyValue("Ordine", ref _Ordine, value);
            }
        }


        private string _Fattura;
        [Size(50)]
        public string Fattura
        {
            get
            {
                return _Fattura;
            }
            set
            {
                SetPropertyValue("Fattura", ref _Fattura, value);
            }
        }

        private string _Note;
        [Size(500)]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                SetPropertyValue("Note", ref _Note, value);
            }
        }
        private int _IdProdotto;
        public int IdProdotto
        {
            get
            {
                return _IdProdotto;
            }
            set
            {
                SetPropertyValue("IdProdotto", ref _IdProdotto, value);
            }
        }

        private int _IdTipoLicenza;
        public int IdTipoLicenza
        {
            get
            {
                return _IdTipoLicenza;
            }
            set
            {
                SetPropertyValue("IdTipoLicenza", ref _IdTipoLicenza, value);
            }
        }

        private DateTime _DataOraRichiestaPulizia;
        public DateTime DataOraRichiestaPulizia
        {
            get
            {
                return _DataOraRichiestaPulizia;
            }
            set
            {
                SetPropertyValue("DataOraRichiestaPulizia", ref _DataOraRichiestaPulizia, value);
            }
        }

        private byte[] _FileKey;
        public byte[] FileKey
        {
            get
            {
                return _FileKey;
            }
            set
            {
                SetPropertyValue("FileKey", ref _FileKey, value);
            }
        }
        #endregion
    }
}
