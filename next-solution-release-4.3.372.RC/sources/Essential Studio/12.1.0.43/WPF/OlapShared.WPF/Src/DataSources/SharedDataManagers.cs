#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.Olap.Manager;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// SharedDataManagers holds collection of data manager inoformation.
    /// </summary>
    public sealed class SharedDataManagers : IDisposable
    {
        #region Members

        private static SharedDataManagers _sharedDataManagers = new SharedDataManagers(); 
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedDataManagers"/> class.
        /// </summary>
        private SharedDataManagers()
        {
            this.DataManagers = new List<DataManager>();
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SharedDataManagers Instance
        {
            get
            {
                return _sharedDataManagers;
            }
        }

        /// <summary>
        /// Gets or sets the data managers.
        /// </summary>
        /// <value>The data managers.</value>
        public List<DataManager> DataManagers { get; set; } 

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.DataManagers != null)
            {
                for (int i = 0; i < this.DataManagers.Count; i++)
                {
                    if (this.DataManagers[i].OlapDataManager != null)
                    {
                        if (this.DataManagers[i].OlapDataManager.DataProvider != null)
                        {
                            this.DataManagers[i].OlapDataManager.DataProvider.CloseConnection();
                        }
                        this.DataManagers[i].OlapDataManager.Dispose();
                    }
                    this.DataManagers[i] = null;
                }
            }
            this.DataManagers = null;
        }

        #endregion
    }

    /// <summary>
    /// DataManager class holds OLAP data manager information.
    /// </summary>
    public class DataManager
    {
        /// <summary>
        /// Gets or sets the olap data manager.
        /// </summary>
        /// <value>The olap data manager.</value>
        public OlapDataManager OlapDataManager { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}
