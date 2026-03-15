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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools.Design
{
   public class TabSplitterContainerDesigner : ParentControlDesigner
    {
        #region Properties

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (m_verbs == null)
                {
                    m_verbs = new DesignerVerbCollection();

                    m_verbs.Add(new DesignerVerb("Add primary page", new EventHandler(AddPrimaryPageHandler)));
                    m_verbs.Add(new DesignerVerb("Add secondary page", new EventHandler(AddSecondaryPageHandler)));
                }
                return m_verbs;
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        public override DesignerActionListCollection ActionLists
        {
            get { return m_actionLists; }
        }
#endif
        #endregion

        #region Overrides

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            TabSplitterContainer container = this.Control as TabSplitterContainer;
            if (container != null)
            {
                container.SplitterPositionChanging += new CancelEventHandler(OnSplitterPositionChanging);
                container.OrientationChanging += new CancelEventHandler(OnSplitterOrientationChanging);
                container.CollapsedChanging += new CancelEventHandler(OnSplitterCollapsedChanging);
                container.SwappedChanging += new CancelEventHandler(OnSplitterSwappedChanging);
            }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            m_actionLists = new DesignerActionListCollection(new DesignerActionList[] { new TabSplitterContainerActionList(this.Component) });
#endif
        }

        protected override void Dispose(bool disposing)
        {
            TabSplitterContainer container = this.Control as TabSplitterContainer;
            if (container != null)
            {
                container.SplitterPositionChanging -= new CancelEventHandler(OnSplitterPositionChanging);
                container.OrientationChanging -= new CancelEventHandler(OnSplitterOrientationChanging);
                container.CollapsedChanging -= new CancelEventHandler(OnSplitterCollapsedChanging);
                container.SwappedChanging -= new CancelEventHandler(OnSplitterSwappedChanging);
            }

            base.Dispose(disposing);
        }
 
        protected override bool GetHitTest(Point point)
        {
            TabSplitterContainer container = this.Control as TabSplitterContainer;
            if (container != null)
            {
                System.Windows.Forms.Control splitter = container.Splitter;

                if (splitter.ClientRectangle.Contains(splitter.PointToClient(point)))
                {
                    return true;
                }
            }
            return base.GetHitTest(point);
        }

        public override bool CanParent(Control control)
        {
            return false;
        }

        protected override void OnDragEnter(DragEventArgs de)
        {
            de.Effect = DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            de.Effect = DragDropEffects.None;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_LBUTTONDOWN:
                    OnWmLButtonDown(ref m);
                    break;
            }
            base.WndProc(ref m);
        }

        #endregion

        #region Event handlers

       public void OnSplitterPositionChanging(object sender, CancelEventArgs e)
        {
            PropertyDescriptor pd = TypeDescriptor.GetProperties(sender)["SplitterPosition"];
            if (pd != null)
            {
                TabSplitterContainer.PositionEventArgs ea = e as TabSplitterContainer.PositionEventArgs;
                if (ea != null)
                {
                    try
                    {
                        pd.SetValue(sender, ea.Position);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            e.Cancel = true;
        }

       public void OnSplitterOrientationChanging(object sender, CancelEventArgs e)
        {
            PropertyDescriptor pd = TypeDescriptor.GetProperties(sender)["Orientation"];
            if (pd != null)
            {
                TabSplitterContainer.OrientationEventArgs ea = e as TabSplitterContainer.OrientationEventArgs;
                if (ea != null)
                {
                    try
                    {
                        pd.SetValue(sender, ea.Orientation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            e.Cancel = true;
        }

       public void OnSplitterCollapsedChanging(object sender, CancelEventArgs e)
        {
            PropertyDescriptor pd = TypeDescriptor.GetProperties(sender)["Collapsed"];
            if (pd != null)
            {
                TabSplitterContainer.CollapsedEventArgs ea = e as TabSplitterContainer.CollapsedEventArgs;
                if (ea != null)
                {
                    try
                    {
                        pd.SetValue(sender, ea.Collapsed);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            e.Cancel = true;
        }

       public void OnSplitterSwappedChanging(object sender, CancelEventArgs e)
        {
            PropertyDescriptor pd = TypeDescriptor.GetProperties(sender)["Swapped"];
            if (pd != null)
            {
                TabSplitterContainer.SwappedEventArgs ea = e as TabSplitterContainer.SwappedEventArgs;
                if (ea != null)
                {
                    try
                    {
                        pd.SetValue(sender, ea.Swapped);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            e.Cancel = true;
        }

       public void AddPrimaryPageHandler(object sender, EventArgs e)
        {
            TabSplitterContainer tabSpliter = this.Component as TabSplitterContainer;
            if (tabSpliter != null)
            {
                IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (designerHost != null)
                {
                    TabSplitterPage page = designerHost.CreateComponent(typeof(TabSplitterPage)) as TabSplitterPage;
                    if (page != null)
                    {
                        page.Text = page.Name;
                        tabSpliter.PrimaryPages.Add(page);
                    }
                }
            }
        }
 
       public void AddSecondaryPageHandler(object sender, EventArgs e)
        {
            TabSplitterContainer tabSpliter = this.Component as TabSplitterContainer;
            if (tabSpliter != null)
            {
                IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (designerHost != null)
                {
                    TabSplitterPage page = designerHost.CreateComponent(typeof(TabSplitterPage)) as TabSplitterPage;
                    if (page != null)
                    {
                        page.Text = page.Name;
                        tabSpliter.SecondaryPages.Add(page);
                    }
                }
            }
        }
        #endregion

        #region Implementation

        private void OnWmLButtonDown(ref Message m)
        {
            ISelectionService selSvc = this.GetService(typeof(ISelectionService)) as ISelectionService;
            if (selSvc != null)
            {
                if (selSvc.SelectionCount != 1 || selSvc.PrimarySelection != this.Component)
                {
                    selSvc.SetSelectedComponents(new object[] { this.Component });
                }
            }
        }
        #endregion

        #region Fields

       private DesignerVerbCollection m_verbs;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

       private DesignerActionListCollection m_actionLists;
#endif
        #endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        #region *** TabSplitterContainerActionList

       private class TabSplitterContainerActionList : DesignerActionList
        {
            #region Constructor
  
            public TabSplitterContainerActionList(IComponent component)
                : base(component)
            {
            }
            #endregion

            #region Methods
    
            public void AddPrimaryPage()
            {
                TabSplitterContainer tabSpliter = this.Component as TabSplitterContainer;
                if (tabSpliter != null)
                {
                    IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (designerHost != null)
                    {
                        TabSplitterPage page = designerHost.CreateComponent(typeof(TabSplitterPage)) as TabSplitterPage;
                        if (page != null)
                        {
                            page.Text = page.Name;
                            tabSpliter.PrimaryPages.Add(page);
                        }
                    }
                }
            }
   
            public void AddSecondaryPage()
            {
                TabSplitterContainer tabSpliter = this.Component as TabSplitterContainer;
                if (tabSpliter != null)
                {
                    IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (designerHost != null)
                    {
                        TabSplitterPage page = designerHost.CreateComponent(typeof(TabSplitterPage)) as TabSplitterPage;
                        if (page != null)
                        {
                            page.Text = page.Name;
                            tabSpliter.SecondaryPages.Add(page);
                        }
                    }
                }
            }
            #endregion

            #region Overrides
  
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();

                items.Add(new DesignerActionMethodItem(this, "AddPrimaryPage", "Add primary page", "Data", "Adds primary page"));
                items.Add(new DesignerActionMethodItem(this, "AddSecondaryPage", "Add secondary page", "Data", "Adds secondary page"));

                return items;
            }
            #endregion
        }
        #endregion
#endif
    }
}
