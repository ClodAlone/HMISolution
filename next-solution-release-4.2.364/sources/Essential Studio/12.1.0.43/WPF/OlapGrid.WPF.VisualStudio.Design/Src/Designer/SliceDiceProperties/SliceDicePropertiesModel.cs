#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Olap.Manager;
using Syncfusion.Windows.Tools.Olap;
using System.ComponentModel;
using Microsoft.Windows.Design.Model;
using System.Windows;
using Syncfusion.Windows.Grid.Olap.Misc;

namespace Syncfusion.OlapGrid.WPF.VisualStudio.Design
{
    public class SliceDicePropertiesModel
        : INotifyPropertyChanged
    {
        #region Members

        private ModelItem selectedControl;

        #endregion

        #region Constructor

        public SliceDicePropertiesModel(ModelItem selectedControl, string conStr)
        {
            this.selectedControl = selectedControl;

            var designerSettings = selectedControl.Properties["DesignerSettings"].ComputedValue;

            if (designerSettings != null)
            {
                string connectionString = selectedControl.Properties["DesignerSettings"].Value.Properties["ConnectionString"].ComputedValue.ToString();

                if (!string.IsNullOrEmpty(connectionString))
                {
                    DesignerSettings desSetting = designerSettings as DesignerSettings;
                    this.MetaTreeOlapDataManager = new OlapDataManager(desSetting.ConnectionString);
                    this.MetaTreeOlapDataManager.SetCurrentReport(desSetting.GetOlapReport());
                    //this.MetaTreeOlapDataManager.cur(designerSettings as DesignerSettings).GetOlapDataManager();
                }
                else
                {
                    throw new ArgumentException("Connection String is empty in designer settings.");
                }
            }
            else
            {
                this.ConnectonString = conStr;

                //// This is being created newly. So, get the connection string information passed from the connection properties view.
                if (!string.IsNullOrEmpty(this.ConnectonString))
                {
                    this.MetaTreeOlapDataManager = new OlapDataManager(this.ConnectonString);
                }
                else
                {
                    MessageBox.Show("Connection string is empty");
                    //throw new ArgumentException("Connection string is empty.");
                }
            }
        }

        #endregion

        #region Public Properties

        public OlapDataManager MetaTreeOlapDataManager
        {
            get;
            set;
        }

        public string ConnectonString
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void CommitChanges()
        {
            if (this.MetaTreeOlapDataManager != null)
            {
                if (this.selectedControl.Properties["DesignerSettings"].ComputedValue != null)
                {
                    this.selectedControl.Properties["DesignerSettings"].Value.Properties["CurrentCubeName"].SetValue(this.MetaTreeOlapDataManager.CurrentCubeName);
                    this.selectedControl.Properties["DesignerSettings"].Value.Properties["CategoricalElements"].SetValue(this.MetaTreeOlapDataManager.CurrentReport.CategoricalElements);
                    //this.selectedControl.Properties["DesignerSettings"].Value.Properties["SeriesElements"].SetValue(this.MetaTreeOlapDataManager.CurrentReport.SeriesElements);
                    //this.selectedControl.Properties["DesignerSettings"].Value.Properties["SlicerElements"].SetValue(this.MetaTreeOlapDataManager.CurrentReport.SlicerElements);
                    MessageBox.Show("Slice and Dice model station commit completed.");
                }
                else
                {
                    MessageBox.Show("Commit unsuccessful. Designer settings was not properly saved.");
                }
            }
            else
            {
                MessageBox.Show("Could not commit the changes. MetaTree is null or badly formatted.");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
