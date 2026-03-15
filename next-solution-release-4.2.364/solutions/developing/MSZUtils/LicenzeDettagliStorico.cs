using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;


namespace MSZUtils
{

    [DeferredDeletion(false)]
    [OptimisticLocking(false)]
    public class tbLicenzeDettagliStorico : XPCustomObject
    {
        #region Constructors
        public tbLicenzeDettagliStorico(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        private int _IdLicenza;
        //[Association("FK_tbLicenzeDettagliStorico_tbLicenzeStorico", typeof(int))]
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

        [Persistent("IdOpzione"), Key(false), MemberDesignTimeVisibility(false)]
        private int _IdOpzione;
        [PersistentAlias("_IdOpzione")]
        //[Association("FK_tbLicenzeDettagliStorico_tbOpzioni", typeof(int))]
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

        private string _Valore;
        [Size(50)]
        public string Valore
        {
            get
            {
                return _Valore;
            }
            set
            {
                SetPropertyValue("Valore", ref _Valore, value);
            }
        }
        #endregion
    }
}
