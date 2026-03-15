using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace S7TCP.UI
{
    public class ImportTag : IImportTag, INotifyPropertyChanged
    {
        #region IImportTag Members

        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<IImportTag> ImportTags { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public int parentId { get; set; }

        public string szDescription { get; set; }
        public Opc.Ua.NodeId nType { get; set; }
        public string szType { get; set; }
        public uint nSize { get; set; }
        public Opc.Ua.NodeId nElemType { get; set; }
        public string szPreName { get; set; }

        #endregion

        public ImportTag() { }
        public ImportTag(int id, int parent = -1) 
        {
            Id = id;
            parentId = parent;
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        

        
        
    }
}
