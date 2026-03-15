#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Syncfusion.Windows.Forms.Tools.Design
{
   public class TrackBarItemDesigner : IDesigner
    {
        #region Nested classes
       public class TrackBarItemSelectionService : IServiceProvider
        {
            #region Constants
           private const BindingFlags BF_NONPUBLIC = BindingFlags.Instance | BindingFlags.NonPublic;
            #endregion

            #region Constructors
          
            protected TrackBarItemSelectionService(IServiceContainer services)
            {
                m_parentProvider = services;

                ISelectionService selSvc = m_parentProvider.GetService(typeof(ISelectionService)) as ISelectionService;

                if (selSvc != null)
                {
                    selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
                }
            }
            #endregion

            #region IServiceProvider implementation
        
            public virtual object GetService(Type serviceType)
            {
                return m_parentProvider.GetService(serviceType);
            }
            #endregion

            #region Static methods
   
            public static TrackBarItemSelectionService Get(ISite site)
            {
                TrackBarItemSelectionService service = null;

                if (site != null)
                {
                    IServiceContainer services = site.GetService(typeof(IServiceContainer)) as IServiceContainer;

                    if (services != null)
                    {
                        service = services.GetService(typeof(TrackBarItemSelectionService)) as TrackBarItemSelectionService;

                        if (service == null)
                        {
                            service = new TrackBarItemSelectionService(services);

                            services.AddService(typeof(TrackBarItemSelectionService), service);
                        }
                    }
                }

                return service;
            }
            #endregion

            #region Properties

            public Hashtable Designers
            {
                get
                {
                    if (m_designers == null)
                    {
                        IDesignerHost host = m_parentProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                        if (host != null)
                        {
                            FieldInfo fiDesigners = host.GetType().GetField("_designers", BF_NONPUBLIC);
                            if (fiDesigners != null)
                            {
                                m_designers = fiDesigners.GetValue(host) as Hashtable;
                            }
                        }
                    }
                    return m_designers;
                }
            }
            #endregion

            #region Event handlers

           public void OnSelectionChanged(object sender, EventArgs e)
            {
                BehaviorService behaviorService = m_parentProvider.GetService(typeof(BehaviorService)) as BehaviorService;

                if (behaviorService != null)
                {
                    // Get host for finding TrackBarItem to invalidate.
                    IDesignerHost designerHost = m_parentProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;

                    if (designerHost != null)
                    {
                        IContainer container = designerHost.Container;

                        if (container != null)
                        {
                            ComponentCollection collection = container.Components;

                            foreach (Component comp in container.Components)
                            {
                                if (comp is TrackBarItem)
                                {
                                    TrackBarItem item = comp as TrackBarItem;
                                    Point p = behaviorService.MapAdornerWindowPoint(item.Control.Handle, Point.Empty);
                                    behaviorService.Invalidate(new Rectangle(p, item.Control.Size));
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Fields
            private IServiceProvider m_parentProvider = null;

          private Hashtable m_designers = null;
            #endregion
        }
        #endregion

        #region IDesigner implementation

        #region Properties
  
        IComponent IDesigner.Component
        {
            get
            {
                return m_component;
            }
        }
  
        DesignerVerbCollection IDesigner.Verbs
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Methods
   
        void IDesigner.Initialize(IComponent component)
        {
            m_component = component;

            if (component != null)
            {
                TrackBarItemSelectionService selSvc = TrackBarItemSelectionService.Get(component.Site);

                if (selSvc != null)
                {
                    if (selSvc.Designers != null)
                    {
                        Type type = Type.GetType("System.Windows.Forms.Design.ToolStripItemDesigner, System.Design");

                        if (type != null)
                        {
                            ComponentDesigner designer = Activator.CreateInstance(type) as ComponentDesigner;

                            if (designer != null)
                            {
                                selSvc.Designers[component] = designer;
                                designer.Initialize(component);
                            }
                        }
                    }
                }
            }
        }
        void IDesigner.DoDefaultAction()
        {
        }
        #endregion

        #endregion

        #region IDisposable implementation
 
        void IDisposable.Dispose()
        {
            m_component = null;
        }
        #endregion

        #region Fields
     
        protected IComponent m_component = null;
        #endregion
    }
}
#endif
