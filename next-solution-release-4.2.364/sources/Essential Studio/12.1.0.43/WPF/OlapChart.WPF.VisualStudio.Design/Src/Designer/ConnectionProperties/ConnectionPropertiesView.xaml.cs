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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Model;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// Used for Creating or editing connection properties.
    /// </summary>
    public partial class ConnectionPropertiesView 
        : UserControl
    {
        #region Members

        /// <summary>
        /// Local instance of the connection properties model.
        /// </summary>
        public ConnectionPropertiesModel _connectionPropertiesModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPropertiesView"/> class.
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        /// <param name="wizardWindow">The wizard window.</param>
        public ConnectionPropertiesView(ModelItem selectedItem, WizardWindow wizardWindow)
        {
            InitializeComponent();

            //// Initializing the model.
            this._connectionPropertiesModel = new ConnectionPropertiesModel(selectedItem, wizardWindow);

            //// Initializing the data context.
            this.DataContext = _connectionPropertiesModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Commits the changes.
        /// </summary>
        public void CommitChanges()
        {
            if (this._connectionPropertiesModel != null)
            {
                this._connectionPropertiesModel.CommitChanges();
            }
        }

        /// <summary>
        /// Gets the connection string from the connection properties model.
        /// </summary>
        /// <returns>Returns the connection string.</returns>
        public string GetConnectionString()
        {
            if (this._connectionPropertiesModel != null)
            {
                return this._connectionPropertiesModel.GetConnectionString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <returns>Returns true if connection string is valid. Else, returns false.</returns>
        public bool TestConnection()
        {
            return this._connectionPropertiesModel.TestConnectionString();
        }

        #endregion
    }
}
