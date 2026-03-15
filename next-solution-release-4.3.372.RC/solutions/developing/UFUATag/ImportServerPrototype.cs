using System;
using System.Collections.Generic;

namespace UFUAModel
{
    public class ImportServerPrototype
    {

        public ImportServerPrototype()
        {

        }

        #region Properties
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
        private Guid _NodeId;
        public Guid NodeId
        {
            get { return _NodeId; }
            set
            {
                _NodeId = value;
            }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
            }
        }
        private List<ImportTag> _Elements;
        public List<ImportTag> Elements
        {
            get { return _Elements; }
            set
            {
                _Elements = value;
            }
        }

        #endregion
    }
}
