using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CommandManager;
using OPCUAViewModel;
using UFInterfaces;
using UFInterfaces.Commandable;
using UFInterfaces.Constants;
using UFShortcutSettings.Documents;

namespace UFShortcutSettings.ShortcutModel
{
    [DataContract(Name = "ShortcutEntity", Namespace = Namespaces.UriProgea)]
    public class UFShortcutEntity : INotifyPropertyChanged, IDataErrorInfo, ICloneable, IEntityReference
    {
        #region Declarations

        internal UFShortcutDocument Document;

        #endregion

        #region Persistance

        [DataMember]
        Guid _NodeId;
        [DataMember]
        string _Description;
        [DataMember]
        List<UFKeyCommandEntity> _KeyCommands;


        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            // ensure a valid recipe association value and oid value
            int oid = 0;
            var values = KeyCommands.OrderBy(o => o.OID);
            foreach (var value in values)
            {
                value.UFShortcutAss = this;
                value.OID = oid++;
           }
        }

        #endregion

        #region Properties
        string _Name;
        [ReadOnly(true)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name == value)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                if (_NodeId == value)
                    return;

                _NodeId = value;
                OnPropertyChanged("NodeId");
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description == value)
                    return;

                _Description = value;
                OnPropertyChanged("Description");
            }
        }

        [Browsable(false)]
        public List<UFKeyCommandEntity> KeyCommands
        {
            get
            {
                if (_KeyCommands == null)
                    _KeyCommands = new List<UFKeyCommandEntity>();

                return _KeyCommands;
            }
        }

        #endregion

        

        #region IEntityReference Members

        [Browsable(false)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone() as UFShortcutEntity;
        }

        #endregion ICloneable Members

        #region IDataErrorInfo

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        [Browsable(false)]
        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {

                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        #region Methods
        protected String PerformValidation(String propertyName)
        {
            return null;
        }
        #endregion
    }
}
