#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    internal class SelectedBarEditor :
        ObjectSelectorEditor
    {
        #region Construction

        public SelectedBarEditor() :
            base(true)
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Fills a hierarchical collection of labeled items, with each item represented by a <see cref="T:System.Windows.Forms.TreeNode"/>.
        /// </summary>
        /// <param name="selector">A hierarchical collection of labeled items.</param>
        /// <param name="context">The context information for a component.</param>
        /// <param name="provider">The <see cref="M:System.IServiceProvider.GetService(System.Type)"/> method of this interface that obtains the object that provides the service.</param>
        protected override void FillTreeWithData(Selector selector, System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider)
        {
            base.FillTreeWithData(selector, context, provider);

            NavigationView nv = context.Instance as NavigationView;

            if (nv != null)
            {
                selector.BeginUpdate();

                SetupSelector(selector, nv);
                FillTreeNodes(nv.Bars, selector.Nodes);

                selector.EndUpdate();
            }
        }

        /// <summary>
        /// Gets a value indicating whether drop-down editors should be resizable by the user.
        /// </summary>
        /// <value></value>
        /// <returns>true if drop-down editors are resizable; otherwise, false. </returns>
        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Edits the value of the specified object using the editor style indicated by <see cref="Overload:System.ComponentModel.Design.ObjectSelectorEditor.GetEditStyle"/>.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, the method should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            FieldInfo fieldInfo = typeof(ObjectSelectorEditor).GetField("selector", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(this, new SelectorExt(this));
            }

            return base.EditValue(context, provider, value);
        }

        #endregion

        #region Implementation

        private static void SetupSelector(Selector selector, NavigationView nv)
        {
        }

        private static void FillTreeNodes(BarCollection bars, TreeNodeCollection nodes)
        {
            foreach (Bar bar in bars)
            {
                SelectorNode node = new SelectorNode(bar.Text, bar);

                // node.ImageIndex = bar.ImageIndex;
                // node.SelectedImageIndex = bar.ImageIndex;
                FillTreeNodes(bar.Bars, node.Nodes);

                nodes.Add(node);
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// Extended <see cref="Selector"/>.
        /// </summary>
       public class SelectorExt : Selector
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SelectorExt"/> class.
            /// </summary>
            /// <param name="osEditor">The os editor.</param>
            public SelectorExt(ObjectSelectorEditor osEditor) :
                base(osEditor)
            {
            }

            /// <summary>
            /// Occurs when the mouse pointer is over the control and a mouse button is clicked.
            /// </summary>
            /// <param name="e">Provides data for the <see cref="E:System.Windows.Forms.Control.MouseUp"/>, <see cref="E:System.Windows.Forms.Control.MouseDown"/>, and <see cref="E:System.Windows.Forms.Control.MouseMove"/> events.</param>
            protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// BarCollection Editor
    /// </summary>
    internal partial class BarCollectionEditor :
        UITypeEditor
    {
        #region Fields

        private IWindowsFormsEditorService _editorService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCollectionEditor"/> class.
        /// </summary>
        public BarCollectionEditor() :
            base()
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (_editorService != null)
                {
                    IContainBars container = context.Instance as IContainBars;

                    if (container != null)
                    {
                        NavigationView navigationView = context.Instance as NavigationView;

                        if (navigationView != null)
                        {
                            BarCollectionEditorForm form = new BarCollectionEditorForm(provider, navigationView);

                            _editorService.ShowDialog(form);

                            if (form.IsDirty)
                            {
                                ISite site = navigationView.Site;

                                if (site != null)
                                {
                                    IComponentChangeService svc = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                                    if (svc != null)
                                    {
                                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(navigationView);
                                        PropertyDescriptor propBars = props.Find("Bars", false);

                                        svc.OnComponentChanging(navigationView, propBars);
                                        svc.OnComponentChanged(navigationView, propBars, navigationView.Bars, navigationView.Bars);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        #endregion
    }
}
