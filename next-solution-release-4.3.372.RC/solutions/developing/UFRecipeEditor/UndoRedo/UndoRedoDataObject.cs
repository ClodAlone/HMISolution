using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFRecipeSettings.UFRecipeModel;

namespace UFRecipeEditor.UndoRedo
{
    internal class UndoRedoDataObject
    {
        #region Declarations

        readonly UndoRedoAction undoRedoAction;

        UFRecipeEntity recipeEntity;
        UFGroupsList ufgroupsList;
        UFDataValuesList ufdataValuesList;
        Byte[] layoutMemento;

        #endregion

        #region Constructors

        public UndoRedoDataObject(UndoRedoAction action)
        {
            undoRedoAction = action;
        }

        public UndoRedoDataObject(UndoRedoDataObject source, UndoRedoAction newaction)
        {
            recipeEntity = source.recipeEntity;
            ufgroupsList = source.ufgroupsList;
            ufdataValuesList = source.ufdataValuesList;
            layoutMemento = source.layoutMemento;
            undoRedoAction = newaction;
        }

        #endregion

        #region Public Methods
        public virtual void AddDataObject(UFRecipeEntity recipe)
        {
            if (recipe == null || undoRedoAction != UndoRedoAction.Changed)
                return;

            recipeEntity = recipe.CreateSnapshot();
        }

        public virtual void AddDataObject(UFGroupEntity group)
        {
            if (group == null)
                return;

            if (ufgroupsList == null)
                ufgroupsList = new UFGroupsList();

            UFGroupEntity cloneGroup = group.CreateSnapshot();
            ufgroupsList.Add(cloneGroup);
        }

        public void AddDataObject(UFGroupsList grouplist)
        {
            if (grouplist == null || grouplist.Count == 0)
                return;

            if (ufgroupsList == null)
                ufgroupsList = new UFGroupsList();

            grouplist.ForEach(group => { AddDataObject(group); });
        }

        public virtual void AddDataObject(UFDataValueEntity datavalue)
        {
            if (datavalue == null)
                return;

            if (ufdataValuesList == null)
                ufdataValuesList = new UFDataValuesList();

            UFDataValueEntity cloneDatavalue = datavalue.CreateSnapshot();
            ufdataValuesList.Add(cloneDatavalue);
        }

        public void AddDataObject(UFDataValuesList valuelist)
        {
            if (valuelist == null || valuelist.Count == 0)
                return;

            if (ufdataValuesList == null)
                ufdataValuesList = new UFDataValuesList();

            valuelist.ForEach(datavalue => { AddDataObject(datavalue); });
        }

        public bool StoreLayoutMemento(Byte[] layoutItems)
        {
            if (layoutItems == null || layoutItems.Length == 0 ||
                CompareByteLayout(layoutMemento, layoutItems))
                return false;

            layoutMemento = layoutItems;
            return true;
        }

        #endregion

        #region Private Methods

        bool CompareByteLayout(byte[] value1, byte[] value2)
        {
            if (value1 == null || value2 == null)
            {
                if (value1 != value2)
                {
                    return false;
                }

                return true;
            }

            if (value1.Length != value2.Length)
            {
                return false;
            }

            for (int ii = 0; ii < value1.Length; ii++)
            {
                if (value1[ii] != value2[ii])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Properties

        public UndoRedoAction UndoRedoAction
        {
            get
            {
                return undoRedoAction;
            }
        }

        public UFRecipeEntity RecipeEntity
        {
            get
            {
                return recipeEntity;
            }
        }

        public UFGroupsList UFGroupsList
        {
            get
            {
                return ufgroupsList;
            }
        }

        public UFDataValuesList UFDataValuesList
        {
            get
            {
                return ufdataValuesList;
            }
        }

        public Byte[] LayoutMemento
        {
            get
            {
                return layoutMemento;
            }
        }

        public bool IsAnyActionAvailable
        {
            get
            {
                if (undoRedoAction == UndoRedo.UndoRedoAction.LayoutState)
                {
                    return layoutMemento != null && layoutMemento.Length > 0;
                }
                else
                {
                    return (recipeEntity != null ||
                        (ufgroupsList != null && ufgroupsList.Count > 0) ||
                        (ufdataValuesList != null && ufdataValuesList.Count > 0));
                }
            }
        }

        #endregion

    }
}
