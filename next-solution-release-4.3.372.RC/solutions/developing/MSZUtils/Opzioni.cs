using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;


namespace MSZUtils
{

    [DeferredDeletion(false)]
    [OptimisticLocking(false)]
    public class tbOpzioni : XPCustomObject
    {
        #region Constructors
        public tbOpzioni(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        [Persistent("IdOpzione"), Key(false), MemberDesignTimeVisibility(false)]
        private int _IdOpzione;
        [PersistentAlias("_IdOpzione")]
        public int IdOpzione
        {
            get
            {
                return _IdOpzione;
            }
            set
            {
                SetPropertyValue("IdOpzione", ref _IdOpzione, value);
            }
        }

        private string _NomeOpzione;
        public string NomeOpzione
        {
            get
            {
                return _NomeOpzione;
            }
            set
            {
                SetPropertyValue("NomeOpzione", ref _NomeOpzione, value);
            }
        }

        private string _TipoOpzione;
        [Size(1)]
        public string TipoOpzione
        {
            get
            {
                return _TipoOpzione;
            }
            set
            {
                SetPropertyValue("TipoOpzione", ref _TipoOpzione, value);
            }
        }
        private bool _Abilitata;
        public bool Abilitata
        {
            get
            {
                return _Abilitata;
            }
            set
            {
                SetPropertyValue("Abilitata", ref _Abilitata, value);
            }
        }
        private string _ParametroMsz;
        [Size(50)]
        public string ParametroMsz
        {
            get
            {
                return _ParametroMsz;
            }
            set
            {
                SetPropertyValue("ParametroMsz", ref _ParametroMsz, value);
            }
        }
        private int _OrderNum;
        public int OrderNum
        {
            get
            {
                return _OrderNum;
            }
            set
            {
                SetPropertyValue("OrderNum", ref _OrderNum, value);
            }
        }
        private bool _Writable;
        public bool Writable
        {
            get
            {
                return _Writable;
            }
            set
            {
                SetPropertyValue("Writable", ref _Writable, value);
            }
        }
        #endregion
    }
}
