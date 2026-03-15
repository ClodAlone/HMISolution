#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
////
#endregion

#region file using directives
using System;
using System.Collections;
using System.Xml;

using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection of classes which control script codes.
    /// </summary>
    public class ScriptManagerExCollection
    : CollectionBase, IDisposable
    {
        #region Class members

        /// <summary>
        /// Control object.
        /// </summary>
        private HTMLUIControl m_control;

        /// <summary>
        /// Indicates whether object is disposed.
        /// </summary>
        private bool m_bDisposed;
        #endregion

        #region Class properties

        /// <summary>
        /// Gets the element in the specified index.
        /// </summary>
        /// <param name="index">An index value</param>
        public ScriptManagerEx this[int index]
        {
            get
            {
                if (index < 0 || index > this.Count)
                    throw new ArgumentOutOfRangeException("index", index, "Value can not be less than 0 and greater than this.Count.");

                return this.List[index] as ScriptManagerEx;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the ScriptManagerExCollection class
        /// </summary>
        /// <param name="control">HTMLUI control instance</param>
        public ScriptManagerExCollection(HTMLUIControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            m_control = control;
        }
        #endregion

        #region Class Public Methods

        /// <summary>
        /// Adds the specified object to the collection.
        /// </summary>
        /// <param name="obj">Object for adding into the collection.</param>
        public void Add(ScriptManagerEx obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            this.List.Add(obj);

            ItemAdded(obj);
        }

        /// <summary>
        /// Indicates whether the collection contains the specified object.
        /// </summary>
        /// <param name="obj">Object for checking in the collection.</param>
        /// <returns>A boolean value</returns>
        public bool Contains(ScriptManagerEx obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return this.List.Contains(obj);
        }

        /// <summary>
        /// Removes the specified object from the collection.
        /// </summary>
        /// <param name="obj">Object for removing from the collection.</param>
        public void Remove(ScriptManagerEx obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            InnerList.Remove(obj);

            HTMLScript script = obj.Script as HTMLScript;

            if (script != null && script.Owner != null)
            {
                if (script.Owner.Parent != null)
                {
                    script.Owner.Parent.Children.Remove(script.Owner);
                }
            }

            obj.Dispose();
        }

        /// <summary>
        /// Compiles all objects in the collection.
        /// </summary>
        public void Compile()
        {
            ScriptManagerEx obj = null;

            for (int i = 0, len = this.Count; i < len; i++)
            {
                obj = this[i];

                obj.Compile();
            }
        }

        /// <summary>
        /// Runs all scripts.
        /// </summary>
        public void Run()
        {
            ScriptManagerEx obj = null;

            for (int i = 0, len = this.Count; i < len; i++)
            {
                obj = this[i];

                obj.RunScript();
            }
        }

        /// <summary>
        /// Stops all scripts.
        /// </summary>
        public void Stop()
        {
            ScriptManagerEx obj = null;

            for (int i = 0, len = this.Count; i < len; i++)
            {
                obj = this[i];
                obj.ResetScriptEngine();
            }
        }

        /// <summary>
        /// Clears the collection of scripts.
        /// </summary>
        public new void Clear()
        {
            ScriptManagerEx obj;
            HTMLScript script;

            for (int i = 0, len = List.Count; i < len; i++)
            {
                obj = List[i] as ScriptManagerEx;
                script = obj.Script as HTMLScript;

                if (obj != null && script != null && script.Owner != null)
                {
                    if (script.Owner.Parent != null)
                    {
                        script.Owner.Parent.Children.Remove(script.Owner);
                    }

                    obj.Dispose();
                }
            }

            base.Clear();
        }
        #endregion

        #region Class helper methods

        /// <summary>
        /// Creates a new object and adds it to the collection.
        /// </summary>
        /// <returns>Newly created object.</returns>
        internal ScriptManagerEx CreateItem()
        {
            HTMLScriptSite scrSite = new HTMLScriptSite(m_control);
            HTMLScript script = new HTMLScript();

            ScriptManagerEx obj = new ScriptManagerEx(scrSite, script);

            Add(obj);

            return obj;
        }

        /// <summary>
        /// Runs just after adding item to the collection.
        /// </summary>
        /// <param name="obj">Script manager object of the script.</param>
        private void ItemAdded(ScriptManagerEx obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            HTMLScript script = obj.Script as HTMLScript;

            if (script != null && script.Owner == null && obj.IsEmbeded)
            {
                XmlElement element = m_control.ThreadDocument.Document.CreateElement(TagName.Script);

                element.SetAttribute(AttributeName.Language, obj.LanguageStr);

                BaseElement bodyElm = m_control.ThreadDocument.RenderRoot as BaseElement;

                BaseElement elm = m_control.ConvertDocument(element, bodyElm, null);

                elm.InnerHTML = obj.Script.SourceText;

                (obj.Script as HTMLScript).Owner = elm;

                bodyElm.Storage.AppendChild(element);
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                DisposeScripts();

                Clear();

                m_bDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Disposes scripts.
        /// </summary>
        private void DisposeScripts()
        {
            if (m_bDisposed) return;

            ScriptManagerEx obj = null;

            for (int i = 0, len = List.Count; i < len; i++)
            {
                obj = List[i] as ScriptManagerEx;

                if (obj != null)
                {
                    obj.Dispose();
                }
            }
        }
        #endregion
    }
}
