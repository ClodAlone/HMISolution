using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Utilities.Xpo.UndoRedo
{
    public class UndoRedoXpoDataCollection : Collection<UndoRedoXpoData>
    {
        #region Methods
        void InvalidateColection()
        {
            sources = null;
            originals = null;
        }
        #endregion

        #region Public Properties
        List<XPObject> sources;
        public List<XPObject> Sources
        {
            get
            {
                if (sources == null)
                {
                    sources = new List<XPObject>();
                    foreach (var entry in this)
                        sources.Add(entry.Source);
                }
                return sources;
            }
        }

        List<XPObject> originals;
        public List<XPObject> Originals
        {
            get
            {
                if (originals == null)
                {
                    originals = new List<XPObject>();
                    foreach (var entry in this)
                        originals.Add(entry.Original);
                }
                return originals;
            }
        }
        #endregion

        #region Overrides
        protected override void ClearItems()
        {
            base.ClearItems();
            InvalidateColection();
        }

        protected override void InsertItem(int index, UndoRedoXpoData item)
        {
            base.InsertItem(index, item);
            InvalidateColection();
        }
        
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            InvalidateColection();
        }
        
        protected override void SetItem(int index, UndoRedoXpoData item)
        {
            base.SetItem(index, item);
            InvalidateColection();
        }
        #endregion
    }

    public class UndoRedoXpoData
    {
        #region Declarations

        readonly XPObject source;
        readonly XPObject original;
        readonly String ownerTypeName;

        readonly String pathIdentifier;
        readonly String uniqueIdentifier;
        readonly String parentIdentifier;
        readonly String ownerIdentifier;

        static char separator = '#';

        #endregion

        #region Constructors
        public UndoRedoXpoData(IXPSimpleObject source, object owner) :
            this(source as XPObject, owner)
        { }

        public UndoRedoXpoData(XPObject source, object owner) : 
            this (source, null, null)
        {
            if (source is IUndoRedoXpo)
            {
                var data = source as IUndoRedoXpo;
                ownerTypeName = owner.GetType().Name;
                pathIdentifier = data.PathIdentifier;
                uniqueIdentifier = data.UniqueIdentifier;
                parentIdentifier = data.ParentIdentifier;
                ownerIdentifier = data.OwnerIdentifier;
            }
        }

        internal UndoRedoXpoData(XPObject sourceObj, XPObject originalObj, UndoRedoXpoData parent)
        {
            source = sourceObj;
            original = originalObj;
            if (parent != null)
            {
                ownerTypeName = parent.OwnerTypeName;
                pathIdentifier = parent.ParentIdentifier;
                uniqueIdentifier = parent.UniqueIdentifier;
                parentIdentifier = parent.ParentIdentifier;
                ownerIdentifier = parent.OwnerIdentifier;
            }
        }

        #endregion

        #region Public Properties
        public XPObject Source
        {
            get
            {
                return source;
            }
        }

        public XPObject Original
        {
            get
            {
                return original;
            }
        }

        public String OwnerTypeName
        {
            get
            {
                return ownerTypeName;
            }
        }

        public String MergedIdentifier
        {
            get
            {
                if (String.IsNullOrEmpty(uniqueIdentifier))
                    return null;

                return String.Format("{1}{0}{2}{0}{3}", separator, uniqueIdentifier, parentIdentifier ?? String.Empty, ownerIdentifier ?? String.Empty);
            }
        }

        public String PathIdentifier
        {
            get
            {
                return pathIdentifier;
            }
        }

        public String UniqueIdentifier
        {
            get
            {
                return uniqueIdentifier;
            }
        }

        public String ParentIdentifier
        {
            get
            {
                return parentIdentifier;
            }
        }

        public String OwnerIdentifier
        {
            get
            {
                return ownerIdentifier;
            }
        }
        #endregion
    }
}
