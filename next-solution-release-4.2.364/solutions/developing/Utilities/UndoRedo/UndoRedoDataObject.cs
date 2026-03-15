using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utilities.UndoRedo
{
    public class UndoRedoDataObject<T>
    {
        #region Declarations

        readonly UndoRedoAction undoRedoAction;
        readonly List<T> dataObjects = new List<T>();

        #endregion

        #region Constructors

        public UndoRedoDataObject(UndoRedoAction action)
        {
            undoRedoAction = action;
        }

        public UndoRedoDataObject(UndoRedoDataObject<T> source, UndoRedoAction newaction)
        {
            dataObjects = source.dataObjects;
            undoRedoAction = newaction;
        }

        #endregion

        #region Public Methods

        public void AddDataObject(T dataObject)
        {
            if (dataObject == null)
                return;

            this.dataObjects.Add(dataObject);
        }

        public void AddDataObject(IList<T> dataObjects)
        {
            if (dataObjects == null || dataObjects.Count == 0)
                return;

            this.dataObjects.AddRange(dataObjects);
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

        public IList<T> DataObjects
        {
            get
            {
                return dataObjects;
            }
        }

        public bool IsAnyActionAvailable
        {
            get
            {
                return dataObjects.Count > 0;
            }
        }

        #endregion

    }
}
