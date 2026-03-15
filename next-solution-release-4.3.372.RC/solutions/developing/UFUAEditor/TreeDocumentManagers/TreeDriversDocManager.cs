using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using UFUAEditor.ComponentService;
using System.Windows.Controls;
using UFUAEditor.Document;
using Utilities;
using System.ComponentModel;
using WPFUtilities;

namespace UFUAEditor.TreeDocumentManagers
{
    class TreeDriversDocManager : TreeChangedDocument
    {
        #region Declarations
        UFUAModel.UFUACommunicationDriver CommDriver;
        readonly IDocumentManager Parent;
        #endregion

        #region Constructors
        internal TreeDriversDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc) 
            : base (c, doc)
        { }

        TreeDriversDocManager(UFUAEditorManagerComponent c, UFUAServerDocument doc,
            UFUAModel.UFUACommunicationDriver commdriver, IDocumentManager parent) 
            : base(c, doc)
        {
            CommDriver = commdriver;
            Parent = parent;
        }
        #endregion

        #region Overrides
        protected override void FillChilds(bool bRefresh, bool bReload)
        {
            if (bReload)
            {
                if (CommDriver != null)
                {
                    var configuration = Document.GetConfiguration();
                    CommDriver = (from c in configuration.ComunicationDrivers
                                  where c.Name == CommDriver.Name
                                  select c).FirstOrDefault();
                }
            }

            if (list == null || bRefresh)
            {
                if (list == null)
                    list = new ObservableUpdateCollection<IDocumentManager>(bCheckDisposableElement: true);
                else
                    list.Clear();

                if (CommDriver == null)
                {
                    foreach (var name in Document.GetConfiguration().ComunicationDrivers)
                    {
                        list.Add(new TreeDocumentManagers.TreeDriversDocManager(editorManagerComponent,
                                                    Document, name, this));
                    }
                }
            }
        }

        protected override bool IsRootTreeDocManager
        {
            get
            {
                return CommDriver == null;
            }
        }

        internal override bool OnChangedDocument(Object sender, ChangedType type, object changedObject)
        {
            var driver = changedObject as UFUAModel.UFUACommunicationDriver;

            switch (type)
            {
                case ChangedType.changed:
                    if (driver != null)
                    {
                        if (driver == CommDriver)
                        {
                            OnPropertyChanged();
                            return true;
                        }
                        else
                        {
                            if (list != null)
                            {
                                foreach (var item in list.OfType<TreeDocumentManagers.TreeDataLoggerSettingsDocManager>())
                                {
                                    if (item.OnChangedDocument(sender, type, changedObject))
                                        return true;
                                }
                            }
                        }
                    }
                    break;

                case ChangedType.added:
                    if (driver != null)
                    {
                        if (list != null)
                            list.Add(new TreeDocumentManagers.TreeDriversDocManager(editorManagerComponent,
                                                        Document, driver, this));
                        return true;
                    }
                    break;

                case ChangedType.removed:
                    if (driver != null)
                    {
                        if (list != null)
                        {
                            var found = (from c in list.OfType<TreeDriversDocManager>() where c.CommDriver == driver select c).ToList();
                            found.ForEach(item => list.Remove(item));
                        }
                        return true;
                    }
                    break;
            }

            return false;
        }

        public override void Edit(Uri uri, IDocument parent)
        {
            editorManagerComponent.Edit(uri, parent);
            editorManagerComponent.GetViewFromUri(uri).tabDrivers.IsSelected = true;

            if (!Document.IsDisposed && CommDriver != null)
            {
                UFUAServerDocument.EditDriverSettings(CommDriver, Document);
            }
        }

        public override string TypeLabel
        {
            get 
            {
                if (CommDriver != null)
                    return CommDriver.FriendlyName;
                else
                    return Properties.Resources.Drivers; 
            }
        }

        public override BitmapImage TypeIcon
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASDriversSmall"); 
            }
        }

        public override BitmapImage TypeIconLarge
        {
            get 
            {
                return UFUAEditorManagerComponent.GetBitmapImage("UFUASDriver"); 
            }
        }

        public override string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.DriversTypeScheme;
            }
        }

        public override bool IsResourceExpandable(IDocument document = null)
        {
            if (IsRootTreeDocManager)
                CreateDocumentIfDisposed();
            if (Document == null || Document.IsDisposed)
                return IsRootTreeDocManager;

            if (CommDriver != null)
                return false;
            else
            {
                var list = Document.GetConfiguration().ComunicationDrivers;
                return list.Count > 0;
            }
        }

        public override Object BrowsableContent
        {
            get
            {
                if (Document == null || Document.IsDisposed)
                    return null;

                if (CommDriver != null)
                    return CommDriver;

                return null;
            }
        }
        #endregion
    }
}
