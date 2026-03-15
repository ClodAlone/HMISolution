#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Collection modify commands.
    /// </summary>
    internal class CollectionModifyCommand
        : ICommand
    {
        #region Class members
        private CollectionExChangeType m_changeType;
        private CollectionEx m_collection;
        private ICollection m_elements;
        private int m_nIndex;
        #endregion

        #region Class initialize/finalize methods
        public CollectionModifyCommand(CollectionExChangeType changeType, CollectionEx collection, ICollection elements, int nIndex)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (elements == null)
                throw new ArgumentNullException("elements");

            m_collection = collection;
            m_elements = elements;
            m_changeType = changeType;
            m_nIndex = nIndex;
        }
        #endregion

        #region ICommand Members
        public string Description
        {
            get { return m_changeType.ToString() + " nodes"; }
        }
        public bool CanUndo
        {
            get { return true; }
        }
        public bool Undo()
        {
            bool success = true;
            ICollection elements = Reverse(m_elements);
            switch (m_changeType)
            {
                case CollectionExChangeType.Set:
                    Set();
                    break;
                case CollectionExChangeType.Insert:
                    IList list = m_collection;

                    if (m_elements.Count == 1)
                    {
                        foreach (object obj in elements)
                        {
                            m_nIndex = ((IList)m_collection).IndexOf(obj);
                            list.Remove(obj);
                            break;
                        }
                    }
                    else if (elements.Count == m_collection.Count)
                    {
                        list.Clear();
                    }
                    else
                    {
                        foreach (object obj in elements)
                        {
                            list.Remove(obj);
                        }
                    }
                    break;
                case CollectionExChangeType.Clear:
                    m_collection.AddRange(elements);
                    m_collection.Members.Reverse();
                    break;
                case CollectionExChangeType.Remove:
                    IList listRemovingFrom = m_collection;

                    foreach (object objCur in elements)
                    {
                        if (listRemovingFrom.Count < m_nIndex)
                        {
                            listRemovingFrom.Add(objCur);
                        }
                        else
                        {
                            if (m_nIndex != -1)
                                listRemovingFrom.Insert(m_nIndex, objCur);
                        }
                        break;
                    }
                    break;
            }
            if (commands.Count > 0)
            {
                ICommand cmd;

                // foreach( ICommand cmd in cmds )
                for (int nCounter = commands.Count - 1; nCounter >= 0; nCounter--)
                {
                    cmd = (ICommand)this.commands[nCounter];
                    success = cmd.Undo();

                    if (!success)
                    {
                        break;
                    }
                }
            }

            return success;
        }

        public bool CanMerge(ICommand cmd)
        {
            if (cmd != null && cmd != this)
                return true;
            else
                return false;
        }

        public void Merge(ICommand cmd)
        {
            commands.Add(cmd);
        }

        /// <summary>
        /// List of commands to execute.
        /// </summary>
        protected ArrayList commands = new ArrayList();
        #endregion

        #region IVerb Members
        public bool Do(object target)
        {
            bool success = true;
            switch (m_changeType)
            {
                case CollectionExChangeType.Set:
                    Set();
                    break;
                case CollectionExChangeType.Insert:
                    if (m_nIndex != -1 && m_elements.Count == 1)
                    {
                        foreach (object obj in m_elements)
                        {
                            ((IList)m_collection).Insert(m_nIndex, obj);
                            break;
                        }
                    }
                    else
                    {
                        m_collection.AddRange(m_elements);
                    }
                    break;
                case CollectionExChangeType.Clear:
                    m_collection.Clear();
                    break;
                case CollectionExChangeType.Remove:
                    IList listRemovingFrom = m_collection;

                    // we can remove only one element from collection at a time
                    foreach (object objCur in m_elements)
                    {
                        m_nIndex = listRemovingFrom.IndexOf(objCur);
                        if (m_nIndex != -1)
                        {
                            listRemovingFrom.Remove(objCur);
                        }
                        break;
                    }
                    break;
            }
            if (commands.Count > 0)
            {
                foreach (ICommand cmd in this.commands)
                {
                    success = cmd.Do(target);

                    if (!success)
                    {
                        break;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Class helper methods
        private void Set()
        {
            int nCounter = 0;
            object objTemp;
            ArrayList arrList = new ArrayList();
            IList list = m_collection;

            foreach (object objCur in m_elements)
            {
                //// get current value
                objTemp = list[m_nIndex + nCounter];
                //// add value to collection which will be used while Redo
                arrList.Add(objTemp);
                //// set new value
                list[m_nIndex + nCounter] = objCur;
                //// update counter
                nCounter++;
            }

            // assign new changing collection
            m_elements = arrList;
        }

        /// <summary>
        /// Reverse the sequence of the elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns>The collection.</returns>
        private ICollection Reverse(ICollection elements)
        {
            ArrayList listToReturn = new ArrayList(elements.Count);

            foreach (object element in elements)
            {
                listToReturn.Insert(0, element);
            }

            return listToReturn;
        }
        #endregion        
    }
}
