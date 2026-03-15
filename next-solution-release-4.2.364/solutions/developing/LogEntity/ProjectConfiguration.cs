using System;
using DevExpress.Xpo;

namespace LogEntity
{

    public class ProjectConfiguration : XPObject
    {
        public ProjectConfiguration()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ProjectConfiguration(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        #region Properties
        private string _Project;
        public string Project
        {
            get
            {
                return _Project;
            }
            set
            {
                SetPropertyValue("Project", ref _Project, value);
            }
        }

        private Guid _ProjectId;
        public Guid ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                SetPropertyValue("ProjectId", ref _ProjectId, value);
            }
        }

        private string _ConnectionString;
        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                SetPropertyValue("ConnectionString", ref _ConnectionString, value);
            }
        }
        #endregion
    }

}