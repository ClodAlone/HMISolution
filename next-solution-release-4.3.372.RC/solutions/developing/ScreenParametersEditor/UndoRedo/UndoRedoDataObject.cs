using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenParameterSettings;
//using MenuSettings.MenuModel;

namespace ScreenParametersEditor.UndoRedo
{
    internal class UndoRedoDataObject
    {
        #region Declarations

        readonly UndoRedoAction undoRedoAction;

        ParameterItemList parameterItemList;
        Byte[] layoutMemento;

        #endregion

        #region Constructors

        public UndoRedoDataObject(UndoRedoAction action)
        {
            undoRedoAction = action;
        }

        public UndoRedoDataObject(UndoRedoDataObject source, UndoRedoAction newaction)
        {
            parameterItemList = source.parameterItemList;
            layoutMemento = source.layoutMemento;
            undoRedoAction = newaction;
        }

        #endregion

        #region Public Methods

        public void AddDataObject(ParameterItem datavalue)
        {
            if (datavalue == null)
                return;

            if (parameterItemList == null)
                parameterItemList = new ParameterItemList();

            parameterItemList.Add(datavalue.Clone() as ParameterItem);
        }

        public void AddDataObject(ParameterItemList valuelist)
        {
            if (valuelist == null || valuelist.Count == 0)
                return;

            if (parameterItemList == null)
                parameterItemList = new ParameterItemList();

            valuelist.ForEach(datavalue => { parameterItemList.Add(datavalue.Clone() as ParameterItem); });
        }

        public void StoreLayoutMemento(Byte[] layoutItems)
        {
            if (layoutItems == null || layoutItems.Length == 0)
                return;

            if (!CompareByteLayout(layoutMemento, layoutItems))
            {
                layoutMemento = layoutItems;
            }
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

        public ParameterItemList ParameterItemList
        {
            get
            {
                return parameterItemList;
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
                    return (parameterItemList != null && parameterItemList.Count > 0);
                }
            }
        }

        #endregion

    }
}
