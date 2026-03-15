using System;
using System.Collections;
using System.Collections.Generic;

namespace DocumentManager.ComponentService
{
    public enum ChangedType
    {
        added,
        removed,
        changed
    }

    public class ChangedDocumentEvent : EventArgs
    {
        readonly object source;
        readonly ChangedType changedType;
        readonly ICollection changedObjects;
        public ChangedDocumentEvent(Object s, ChangedType type, ICollection changed)
        {
            source = s;
            changedType = type;
            changedObjects = changed;
        }

        public object Source { get { return source; } }
        public ChangedType ChangedType { get { return changedType; } }
        public ICollection ChangedObjects { get { return changedObjects; } }
    }
}
