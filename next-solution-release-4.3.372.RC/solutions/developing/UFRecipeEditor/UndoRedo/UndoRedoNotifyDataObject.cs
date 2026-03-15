using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFRecipeSettings.UFRecipeModel;

namespace UFRecipeEditor.UndoRedo
{
    internal class UndoRedoNotifyDataObject : UndoRedoDataObject, IDisposable
    {
        #region Declarations

        readonly List<INotifyPropertyChanged> notifiers;
        bool isAnyPropertyChanged;

        #endregion

        #region Constructors

        public UndoRedoNotifyDataObject(UndoRedoAction action) :
            base(action)
        {
            notifiers = new List<INotifyPropertyChanged>();
        }

        #endregion

        #region Public Methods
        public override void AddDataObject(UFRecipeEntity recipe)
        {
            base.AddDataObject(recipe);
            if (recipe is INotifyPropertyChanged)
            {
                (recipe as INotifyPropertyChanged).PropertyChanged += UndoRedoDataObject_PropertyChanged;
                notifiers.Add(recipe as INotifyPropertyChanged);
            }
        }

        public override void AddDataObject(UFGroupEntity group)
        {
            base.AddDataObject(group);
            if (group is INotifyPropertyChanged)
            {
                (group as INotifyPropertyChanged).PropertyChanged += UndoRedoDataObject_PropertyChanged;
                notifiers.Add(group as INotifyPropertyChanged);
            }
        }


        public override void AddDataObject(UFDataValueEntity datavalue)
        {
            base.AddDataObject(datavalue);
            if (datavalue is INotifyPropertyChanged)
            {
                (datavalue as INotifyPropertyChanged).PropertyChanged += UndoRedoDataObject_PropertyChanged;
                notifiers.Add(datavalue as INotifyPropertyChanged);
            }
        }

        #endregion

        #region Private Methods

        void UndoRedoDataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            isAnyPropertyChanged = true;
        }

        #endregion

        #region Properties

        public bool IsAnyPropertyChanged
        {
            get
            {
                return isAnyPropertyChanged;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (notifiers != null)
                notifiers.ForEach(item => item.PropertyChanged -= UndoRedoDataObject_PropertyChanged);
            isAnyPropertyChanged = false;
        }

        #endregion


    }
}
