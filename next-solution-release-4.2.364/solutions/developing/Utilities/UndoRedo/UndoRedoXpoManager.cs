using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Xpo.UndoRedo
{
    public interface IUndoRedoXpo
    {
        String PathIdentifier { get; }
        String UniqueIdentifier { get; }
        String ParentIdentifier { get; }
        String OwnerIdentifier { get; }
    }

    #region UndoRedoXpoManager Class

    public class UndoRedoXpoManager
    {
        #region Declarations

        readonly Session sourceSession;
        readonly Session targetSession;

        //readonly Object lockObject = new Object();
        readonly short MaxActionsToStore;
        readonly Stack<UndoRedoXpoDataCollection> UndoStack;
        readonly Stack<UndoRedoAction> UndoAction;
        readonly Stack<UndoRedoXpoDataCollection> RedoStack;
        readonly Stack<UndoRedoAction> RedoAction;

        readonly List<XPObject> UndoObjectsToDelete;
        readonly List<XPObject> RedoObjectsToDelete;

        readonly Dictionary<String, XPObject> targets;

        #endregion

        #region Constructors

        public UndoRedoXpoManager(Session source, Session target) :
            this(source, target, numactions: 0)
        { }

        public UndoRedoXpoManager(Session source, Session target, short numactions)
        {
            sourceSession = source;
            targetSession = target;
            MaxActionsToStore = numactions;
            UndoStack = new Stack<UndoRedoXpoDataCollection>(numactions);
            UndoAction = new Stack<UndoRedoAction>(numactions);
            RedoStack = new Stack<UndoRedoXpoDataCollection>(numactions);
            RedoAction = new Stack<UndoRedoAction>(numactions);

            UndoObjectsToDelete = new List<XPObject>();
            RedoObjectsToDelete = new List<XPObject>();

            targets = new Dictionary<String, XPObject>();
        }

        #endregion

        #region Members

        public void AddUndoAction(UndoRedoXpoDataCollection sources, UndoRedoAction action)
        {
            //lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(UndoStack, UndoAction, MaxActionsToStore);

                var listdata = new UndoRedoXpoDataCollection();
                var listids = new List<String>();
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceSession, targetSession, copyaggregated: action != UndoRedoAction.Replaced, copyassociation: action != UndoRedoAction.Replaced);
                foreach (var entry in sources)
                {
                    if (!String.IsNullOrEmpty(entry.MergedIdentifier))
                    {
                        if (action != UndoRedoAction.Removed)
                            targets[entry.MergedIdentifier] = entry.Source;
                        else
                            targets.Remove(entry.MergedIdentifier);
                    }

                    if (action != UndoRedoAction.None)
                    {
                        var cloned = cloneHelper.Clone(entry.Source, false);
                        listdata.Add(new UndoRedoXpoData(cloned, entry.Source, entry));
                    }
                    if (action == UndoRedoAction.Added && !String.IsNullOrEmpty(entry.PathIdentifier))
                        listids.Add(entry.PathIdentifier);
                }

                if (listdata.Count > 0)
                {
                    UndoStack.Push(listdata);
                    UndoAction.Push(action);
                }

                if (listids.Count > 0)
                    RemoveActionsByIdentifiers(RedoStack, RedoAction, listids);
            }
        }

        public void AddRedoAction(UndoRedoXpoDataCollection sources, UndoRedoAction action)
        {
            //lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(RedoStack, RedoAction, MaxActionsToStore);

                var listdata = new UndoRedoXpoDataCollection();
                var listids = new List<String>();
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceSession, targetSession, copyaggregated: action != UndoRedoAction.Replaced, copyassociation: action != UndoRedoAction.Replaced);
                foreach (var entry in sources)
                {
                    if (!String.IsNullOrEmpty(entry.MergedIdentifier))
                    {
                        if (action != UndoRedoAction.Added)
                            targets[entry.MergedIdentifier] = entry.Source;
                        else
                            targets.Remove(entry.MergedIdentifier);
                    }

                    if (action != UndoRedoAction.None)
                    {
                        var cloned = cloneHelper.Clone(entry.Source, false);
                        listdata.Add(new UndoRedoXpoData(cloned, entry.Source, entry));
                    }
                    if (action == UndoRedoAction.Added && !String.IsNullOrEmpty(entry.PathIdentifier))
                        listids.Add(entry.PathIdentifier);
                }

                if (listdata.Count > 0)
                {
                    RedoStack.Push(listdata);
                    RedoAction.Push(action);
                }

                if (listids.Count > 0)
                    RemoveActionsByIdentifiers(UndoStack, UndoAction, listids);
            }
        }

        public void AddUndoAction(UndoRedoXpoData source, UndoRedoAction action)
        {
            var list = new UndoRedoXpoDataCollection();
            list.Add(source);
            AddUndoAction(list, action);
        }

        public void AddRedoAction(UndoRedoXpoData source, UndoRedoAction action)
        {
            var list = new UndoRedoXpoDataCollection();
            list.Add(source);
            AddRedoAction(list, action);
        }

        public UndoRedoXpoDataCollection Undo(out UndoRedoAction action)
        {
            var sources = new UndoRedoXpoDataCollection();
            //lock (lockObject)
            {
                action = UndoRedoAction.None;

                if (UndoStack.Count != UndoAction.Count)
                    throw new InvalidOperationException("Invalid memory stack for performing Undo command");

                if (UndoStack.Count > 0)
                {
                    action = UndoAction.Pop();
                    sources = UndoStack.Pop();
                }
            }

            var ret = new UndoRedoXpoDataCollection();
            var stack = new UndoRedoXpoDataCollection();
            var toDelete = new List<XPObject>();
            if (sources.Count > 0 && action != UndoRedoAction.None)
            {
                var syncronize = action != UndoRedoAction.Removed;
                var copyaggregated = !syncronize && action != UndoRedoAction.Changed && action != UndoRedoAction.Replaced;
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(targetSession, sourceSession, copyaggregated: copyaggregated);
                var stackHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceSession, targetSession, copyaggregated: action != UndoRedoAction.Replaced, copyassociation: action != UndoRedoAction.Replaced);
                foreach (var entry in sources)
                {
                    XPObject target = null;
                    if (syncronize)
                    {
                        if (!String.IsNullOrEmpty(entry.MergedIdentifier) &&
                            targets.ContainsKey(entry.MergedIdentifier))
                        {
                            target = targets[entry.MergedIdentifier];
                            if (action == UndoRedoAction.Added)
                                targets.Remove(entry.MergedIdentifier);
                        }

                        //var filterOperator = DevExpress.Data.Filtering.CriteriaOperator.Parse(
                        //    "UniqueIdentifier=? AND ParentIdentifier=? AND OwnerIdentifier=?",
                        //    entry.UniqueIdentifier, entry.ParentIdentifier, entry.OwnerIdentifier);
                        //target = sourceSession.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, entry.Source.GetType(), filterOperator) as XPObject;
                        //if (target == null && entry.Original is IUndoRedoXpo)
                        //{
                        //    original = entry.Original as IUndoRedoXpo;
                        //    var filterOperator = DevExpress.Data.Filtering.CriteriaOperator.Parse(
                        //    "UniqueIdentifier=? AND ParentIdentifier=? AND OwnerIdentifier=?",
                        //    original.UniqueIdentifier, original.ParentIdentifier, original.OwnerIdentifier);
                        //    target = sourceSession.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, entry.Original.GetType(), filterOperator) as XPObject;
                        //}

                        if (target == null || target.IsDeleted)
                            continue;
                        stack.Add(new UndoRedoXpoData(stackHelper.Clone(target, false), target, entry));
                    }
                    var cloned = cloneHelper.Clone(entry.Source, syncronize, target);
                    ret.Add(new UndoRedoXpoData(cloned, entry.Source, entry));
                    if (!syncronize)
                    {
                        stack.Add(new UndoRedoXpoData(stackHelper.Clone(cloned, false), cloned, entry));
                        if (!String.IsNullOrEmpty(entry.MergedIdentifier))
                            targets[entry.MergedIdentifier] = cloned;
                    }
                    toDelete.Add(entry.Source);
                }
            }

            if (stack.Count > 0)
            {
                //lock (lockObject)
                {
                    RedoAction.Push(action);
                    RedoStack.Push(stack);
                    UndoObjectsToDelete.AddRange(toDelete);
                }
            }

            return ret;
        }

        public UndoRedoXpoDataCollection Redo(out UndoRedoAction action)
        {
            var sources = new UndoRedoXpoDataCollection();
            //lock (lockObject)
            {
                action = UndoRedoAction.None;

                if (RedoStack.Count != RedoAction.Count)
                    throw new InvalidOperationException("Invalid memory stack for performing Redo command");

                if (RedoStack.Count > 0)
                {
                    action = RedoAction.Pop();
                    sources = RedoStack.Pop();
                }
            }

            var ret = new UndoRedoXpoDataCollection();
            var stack = new UndoRedoXpoDataCollection();
            var toDelete = new List<XPObject>();
            if (sources.Count > 0 && action != UndoRedoAction.None)
            {
                var syncronize = action != UndoRedoAction.Added;
                var copyaggregated = !syncronize && action != UndoRedoAction.Changed && action != UndoRedoAction.Replaced;
                var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(targetSession, sourceSession, copyaggregated: copyaggregated);
                var stackHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceSession, targetSession, copyaggregated: action != UndoRedoAction.Replaced, copyassociation: action != UndoRedoAction.Replaced);
                foreach (var entry in sources)
                {
                    XPObject target = null;
                    if (syncronize)
                    {
                        if (!String.IsNullOrEmpty(entry.MergedIdentifier) &&
                            targets.ContainsKey(entry.MergedIdentifier))
                        {
                            target = targets[entry.MergedIdentifier];
                            if (action == UndoRedoAction.Removed)
                                targets.Remove(entry.MergedIdentifier);
                        }

                        //var filterOperator = DevExpress.Data.Filtering.CriteriaOperator.Parse(
                        //    "UniqueIdentifier=? AND ParentIdentifier=? AND OwnerIdentifier=?", 
                        //    entry.UniqueIdentifier, entry.ParentIdentifier, entry.OwnerIdentifier);
                        //target = sourceSession.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, entry.Source.GetType(), filterOperator) as XPObject;
                        //if (target == null && entry.Original is IUndoRedoXpo)
                        //{
                        //    var original = entry.Original as IUndoRedoXpo;
                        //    filterOperator = DevExpress.Data.Filtering.CriteriaOperator.Parse(
                        //    "UniqueIdentifier=? AND ParentIdentifier=? AND OwnerIdentifier=?",
                        //    original.UniqueIdentifier, original.ParentIdentifier, original.OwnerIdentifier);
                        //    target = sourceSession.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, entry.Original.GetType(), filterOperator) as XPObject;
                        //}

                        if (target == null || target.IsDeleted)
                            continue;
                        stack.Add(new UndoRedoXpoData(stackHelper.Clone(target, false), target, entry));
                    }

                    var cloned = cloneHelper.Clone(entry.Source, syncronize, target);
                    ret.Add(new UndoRedoXpoData(cloned, entry.Source, entry));
                    if (!syncronize)
                    {
                        stack.Add(new UndoRedoXpoData(stackHelper.Clone(cloned, false), cloned, entry));
                        if (!String.IsNullOrEmpty(entry.MergedIdentifier))
                            targets[entry.MergedIdentifier] = cloned;
                    }
                    toDelete.Add(entry.Source);
                }
            }

            if (stack.Count > 0)
            {
                //lock (lockObject)
                {
                    UndoAction.Push(action);
                    UndoStack.Push(stack);
                    RedoObjectsToDelete.AddRange(toDelete);
                }
            }

            return ret;
        }

        public bool CanUndo()
        {
            //lock (lockObject)
            {
                return UndoStack.Count > 0;
            }
        }

        public bool CanRedo()
        {
            //lock (lockObject)
            {
                return RedoStack.Count > 0;
            }
        }

        public UndoRedoXpoDataCollection PeekNextUndo()
        {
            //lock (lockObject)
            {
                if (UndoStack.Count > 0)
                    return UndoStack.Peek();
                return null;
            }
        }

        public UndoRedoXpoDataCollection PeekNextRedo()
        {
            //lock (lockObject)
            {
                if (RedoStack.Count > 0)
                    return RedoStack.Peek();
                return null;
            }
        }

        public void PurgeUndoActions(bool clean = false)
        {
            if (clean)
            {
                //lock (lockObject)
                {
                    foreach (var sources in UndoStack)
                    {
                        foreach (var entry in sources)
                            entry.Source.Delete();
                    }

                    UndoStack.Clear();
                    UndoAction.Clear();
                    targets.Clear();
                }
            }
            else if (UndoObjectsToDelete.Count > 0)
            {
                //lock (lockObject)
                {
                    foreach (var obj in UndoObjectsToDelete)
                        obj.Delete();
                    UndoObjectsToDelete.Clear();
                }
            }

            targetSession.CommitTransaction();
            targetSession.PurgeDeletedObjects();
        }

        public void PurgeRedoActions(bool clean = false)
        {
            if (clean)
            {
                //lock (lockObject)
                {
                    foreach (var sources in RedoStack)
                    {
                        foreach (var entry in sources)
                            entry.Source.Delete();
                    }

                    RedoStack.Clear();
                    RedoAction.Clear();
                    targets.Clear();
                }
            }
            else if (RedoObjectsToDelete.Count > 0)
            {
                //lock (lockObject)
                {
                    foreach (var obj in RedoObjectsToDelete)
                        obj.Delete();
                    RedoObjectsToDelete.Clear();
                }
            }

            targetSession.CommitTransaction();
            targetSession.PurgeDeletedObjects();
        }

        #endregion

        #region Privates Members

        void CheckMaxActionsToStore(Stack<UndoRedoXpoDataCollection> listsources, Stack<UndoRedoAction> listactions, short numactions)
        {
            if (listactions.Count >= MaxActionsToStore)
            {
                var sources = new Stack<UndoRedoXpoDataCollection>(numactions);
                // fill queue with the stack list
                while (listsources.Count > 1)
                    sources.Push(listsources.Pop());
                // remove the latest object
                foreach (var entry in listsources.Pop())
                    entry.Source.Delete();
                // fill stack with the queue list - 1
                while (sources.Count > 0)
                    listsources.Push(sources.Pop());

                var actions = new Stack<UndoRedoAction>(numactions);
                // fill queue with the stack list
                while (listactions.Count > 1)
                    actions.Push(listactions.Pop());
                // remove the latest object
                listactions.Pop();
                // fill stack with the queue list - 1
                while (actions.Count > 0)
                    listactions.Push(actions.Pop());

                sources.Clear();
                actions.Clear();
            }
        }

        void RemoveActionsByIdentifiers(Stack<UndoRedoXpoDataCollection> listsources, Stack<UndoRedoAction> listactions, List<String> listids)
        {
            var sources = new Stack<UndoRedoXpoDataCollection>(listsources.Count);
            var actions = new Stack<UndoRedoAction>(listactions.Count);
            while (listsources.Count > 0)
            {
                var objs = listsources.Pop();
                var acts = listactions.Pop();
                for (int ii = 0; ii < objs.Count; ii++)
                {
                    if (String.IsNullOrEmpty(objs[ii].PathIdentifier))
                        continue;

                    if (listids.Contains(objs[ii].PathIdentifier))
                    {
                        if (!String.IsNullOrEmpty(objs[ii].MergedIdentifier))
                            targets.Remove(objs[ii].MergedIdentifier);
                        objs.RemoveAt(ii);
                        ii--;
                    }
                }

                if (objs.Count > 0)
                {
                    sources.Push(objs);
                    actions.Push(acts);
                }
            }

            while (sources.Count > 0)
                listsources.Push(sources.Pop());
            while (actions.Count > 0)
                listactions.Push(actions.Pop());

            sources.Clear();
            actions.Clear();
        }

        #endregion

    }

    #endregion
}
