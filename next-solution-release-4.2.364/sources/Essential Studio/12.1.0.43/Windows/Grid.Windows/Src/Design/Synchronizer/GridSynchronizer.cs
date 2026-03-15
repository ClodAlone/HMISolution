//-------------------------------------------------------------------------------------------------
// <copyright file="GridSynchronizer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Syncfusion.ComponentModel;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    /// <summary>
    /// Synchronizes the properties/data between two GridControl objects
    /// </summary>
    internal class GridSynchronizer : Disposable
    {
        #region Members
        Syncfusion.Windows.Forms.Grid.GridControl orig;
        Syncfusion.Windows.Forms.Grid.GridControl dup;
        PropertyInfoCollection initialCollection = new PropertyInfoCollection();
        private bool bInitialized = false;
        private string[] SpecialProps = new string[] { "FrozenRowCount", "FrozenColCount" };
        private IEnumerator specPropEnum;
        private static bool bSaveGroupStyles = true;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes <see cref="GridSynchronizer"/> object for use.
        /// </summary>
        internal GridSynchronizer()
        {
            GridSyncPropertiesStore.BackgroundImageProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
            GridStyleInfoStore.BackgroundImageProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
            specPropEnum = SpecialProps.GetEnumerator();
            GridDesignerMain.syncProps = new GridSyncProperties();
        }

        /// <summary>
        ///     Creates the GridSynchronizer object, intializing the duplicate grid with the properties/data from the original grid.
        /// </summary>
        /// <param name="original" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///         The GridControl to retrieve initial values from.
        ///     </para>
        /// </param>
        /// <param name="duplicate" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///         The GridControl that will recieve the initial values.
        ///     </para>
        /// </param>
        public GridSynchronizer(Syncfusion.Windows.Forms.Grid.GridControl original, Syncfusion.Windows.Forms.Grid.GridControl duplicate)
            : this()
        {
            GridDesignerMain.syncProps.PropertyChanged += new GridSyncProperties.PropertyChangedEventHandler(syncProps_Changed);
            orig = original;
            dup = duplicate;
            InitializeGridSync();
        }
        #endregion

        GridFrame mainWindow;

        internal GridFrame MainWindow
        {
            get { return mainWindow; }
            set { mainWindow = value; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GridDesignerMain.syncProps.PropertyChanged -= new GridSyncProperties.PropertyChangedEventHandler(syncProps_Changed);
                GridDesignerMain.syncProps.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Synchronization Methods
        /// <summary>
        ///     Applies the properties from the duplicate GridControl to the original GridControl
        /// </summary>    
        internal void SyncOriginal()
        {
            GridDesignerMain.syncProps.Cells.InitializeFrom(dup);
            SetGridProperties(orig, false, true);
        }

        /// <summary>
        ///     Applies the store GridSyncProperties to the supplied GridControl
        /// </summary>
        /// <param name="grid" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///         The GridControl to apply the properties to.
        ///     </para>
        /// </param>
        /// <param name="setImageID" type="bool"> To set imageid. 
        /// </param>       
        /// <param name="includeCellData" type ="bool"> to include Cell Data 
        /// </param>
        private void SetGridProperties(GridControl grid, bool setImageID, bool includeCellData)
        {
            grid.BeginInit();
            ResetGrid(grid);
            bool bIsSpecial = false;

            PropertyInfo[] pi = GridDesignerMain.syncProps.GetType().GetProperties();
            foreach (PropertyInfo p in pi)
            {
                try
                {
                    ////we'll do the Cells last, to ensure that the rows/columns are previously set before assigning values
                    if (p.Name.ToLower() == "cells")
                    {
                        continue;
                    }

                    ////general Font property of the grid control is based on the TableStyle, so we'll set this after we set the Cells.
                    if (p.Name.ToLower() == "font")
                    {
                        continue;
                    }
                    ////we will ignore the BackgroundImageId property
                    if (p.Name.ToLower() == "backgroundimageid")
                    {
                        if (!setImageID)
                        {
                            continue;
                        }
                    }

                    bIsSpecial = false;
                    specPropEnum.Reset();
                    while (specPropEnum.MoveNext())
                    {
                        if (specPropEnum.Current.ToString() == p.Name)
                        {
                            bIsSpecial = true;
                            break;
                        }
                    }

                    if (bIsSpecial)
                    {
                        ConfigureSpecialProperty(grid, p);
                    }
                    else
                    {
                        this.SyncProperty(grid, p.Name);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
            ////now do the Cells
            if (includeCellData)
            {
                ConfigureSpecialProperty(grid, GridDesignerMain.syncProps.GetType().GetProperty("Cells"));
                ConfigureSpecialProperty(grid, GridDesignerMain.syncProps.GetType().GetProperty("Font"));
            }

            grid.Initialize();
            grid.EndInit();
            grid.Refresh();
        }

        internal void InitializeGridSync(GridSyncProperties syncProperties, GridControl grid)
        {
            this.bInitialized = false;
            GridDesignerMain.syncProps = syncProperties;
            GridDesignerMain.syncProps.PropertyChanged += new GridSyncProperties.PropertyChangedEventHandler(syncProps_Changed);
            SetGridProperties(grid, true, true);

            this.bInitialized = true;
        }

        /// <summary> Initializes the duplicate GridControl with the supplied GridSyncProperties object.
        /// </summary>
        internal void InitializeGridSync(GridSyncProperties syncProperties)
        {
            InitializeGridSync(syncProperties, dup);
        }

        /// <summary>
        ///     Initializes the duplicate GridControl with the supplied PropertyInfoCollection.
        /// </summary>
        internal void InitializeGridSync(PropertyInfoCollection propertyCollection)
        {
            this.bInitialized = false;
            PropertyInfo pi = null;
            PropertyInfoCollection.PropertyInfoEnumerator ienum = propertyCollection.GetEnumerator();
            while (ienum.MoveNext())
            {
                object val = ienum.Current.GetValue(null, new object[0]);
                pi = GridDesignerMain.syncProps.GetType().GetProperty(ienum.Current.Name);
                if (pi != null)
                {
                    pi.SetValue(GridDesignerMain.syncProps, val, new object[0]);
                    try
                    {
                        SyncProperty(dup, ienum.Current.Name, val);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }

            dup.Initialize();
            this.bInitialized = true;
        }

        /// <summary>
        ///     Initializes the duplicate GridControl with the values from the original GridControl
        /// </summary>
        private void InitializeGridSync()
        {
            this.bInitialized = false;
            ////setup data sync
            GridCellsMemento wrapper = new GridCellsMemento();
            ////fill the wrapper with the original data
            wrapper.InitializeFrom(orig);
            ////get the properties
            InitializeGridSyncProperties(orig);

            ////now set the duplicate grid
            SetGridProperties(dup, true, false);
            wrapper.ApplyTo(dup);

            this.bInitialized = true;
        }

        /// <summary>
        ///     Fills the GridSyncProperties with the values from the supplied grid.
        /// </summary>
        internal void InitializeGridSyncProperties(GridControl grid)
        {
            MethodInfo setInfo;
            MethodInfo getInfo;
            object val;
            if (GridDesignerMain.syncProps == null)
            {
                GridDesignerMain.syncProps = new GridSyncProperties();
            }

            PropertyInfo[] syncProperties = GridDesignerMain.syncProps.GetType().GetProperties();
            try
            {
                // make sure that the values in the calling grid are not changed in InitializeGridSyncProperties.
                // Otherwise when the loop below changes values in GridSyncProperties then its OnChanged method 
                // is called which again cause the original grid to get the value from GridSyncProperties.
                // This would slow down this method a lot and is just a duplication of efforts. You could
                // see this when loading from template.
                GridDesignerMain.syncProps.shouldRaiseChanged = false;

                foreach (PropertyInfo property in syncProperties)
                {
                    if (property.Name.ToLower() != "modified")
                    {
                        PropertyInfo pi = grid.GetType().GetProperty(property.Name);
                        if (pi != null)
                        {
                            val = null;
                            getInfo = pi.GetGetMethod();
                            if (getInfo != null)
                            {
                                ////here's the value
                                val = getInfo.Invoke(grid, new object[0]);
                                if (val != null)
                                {
                                    Trace.WriteLine(property.Name + " = " + val.ToString());
                                    if (val.GetType() == typeof(Syncfusion.Windows.Forms.Grid.GridProperties))
                                    {
                                        GridDesignerMain.syncProps.Properties.CopyPropertiesFrom((Syncfusion.Windows.Forms.Grid.GridProperties)val);
                                        continue;
                                    }
                                }

                                ////set the value in the syncProps object
                                setInfo = property.GetSetMethod();
                                if (setInfo != null)
                                {
                                    setInfo.Invoke(GridDesignerMain.syncProps, new object[] { val });
                                }
                            }
                        }
                    }
                }

                GridDesignerMain.syncProps.FrozenRowCount = grid.Model.Rows.FrozenCount;
                GridDesignerMain.syncProps.FrozenColCount = grid.Model.Cols.FrozenCount;
            }
            finally
            {
                GridDesignerMain.syncProps.shouldRaiseChanged = true;
            }
        }        

        /// <summary>
        ///     Configures a complex property that is not an actual property of the grid. For example, the GridCellsMemento object
        ///     which contains values relating to various properties of the GridControl.
        /// </summary>
        /// <param name="grid" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///         The GridControl to apply the property to.
        ///     </para>
        /// </param>
        /// <param name="pi" type="System.Reflection.PropertyInfo">
        ///     <para>
        ///            The PropertyInfo object that contains a complex property.    
        ///     </para>
        /// </param>
        private void ConfigureSpecialProperty(Syncfusion.Windows.Forms.Grid.GridControl grid, PropertyInfo pi)
        {
            object result = null;
            MethodInfo mi = pi.GetGetMethod();
            if (mi != null)
            {
                result = mi.Invoke(GridDesignerMain.syncProps, new object[0]);
            }

            if (pi.Name == "Cells")
            {
                if (result != null)
                {
                    GridCellsMemento wrapper = result as GridCellsMemento;
                    wrapper.ApplyTo(grid);
                }
            }
            else if (pi.Name == "Font")
            {
                ////we'll grab this from the grid's tablestyle
                if (grid.TableStyle.Font != null)
                {
                    GridDesignerMain.syncProps.Store.SetValue(GridDesignerMain.syncProps.Store.FindStyleInfoProperty("Font"), grid.TableStyle.Font.GdipFont);
                }
            }
            else if (pi.Name == "FrozenRowCount")
            {
                grid.Model.Rows.FrozenCount = GridDesignerMain.syncProps.FrozenRowCount;
            }
            else if (pi.Name == "FrozenColCount")
            {
                grid.Model.Cols.FrozenCount = GridDesignerMain.syncProps.FrozenColCount;
            }
        }

        private void SyncProperty(Syncfusion.Windows.Forms.Grid.GridControl gridDest, string propertyName, object val)
        {
            MethodInfo setInfo;

            PropertyInfo pi = gridDest.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                setInfo = pi.GetSetMethod();
                if (setInfo != null)
                {
                    if (val != null && gridDest == this.orig)
                    {
                        if (val.GetType() == typeof(Syncfusion.Windows.Forms.Grid.GridProperties))
                        {
                            if (gridDest.Properties != null)
                            {
                                gridDest.Properties.CopyPropertiesFrom(((Syncfusion.Windows.Forms.Grid.GridProperties)val));
                                gridDest.Properties.modified = true;
                            }

                            return;
                        }
                    }

                    setInfo.Invoke(gridDest, new object[] { val });
                }
            }
        }

        private void SyncProperty(Syncfusion.Windows.Forms.Grid.GridControl gridDest, string propertyName)
        {
            if (propertyName == "Store")
            {
                return;
            }

            MethodInfo getInfo;
            object val;

            try
            {
                PropertyInfo pi = GridDesignerMain.syncProps.GetType().GetProperty(propertyName);
                if (pi != null)
                {
                    getInfo = pi.GetGetMethod();
                    if (getInfo != null)
                    {
                        StyleInfoProperty sip = GridDesignerMain.syncProps.Store.FindStyleInfoProperty(propertyName);
                        if (sip != null)
                        {
                            val = GridDesignerMain.syncProps.GetValue(sip);
                            if (val == null)
                            {
                                val = getInfo.Invoke(GridDesignerMain.syncProps, new object[0]);
                            }

                            SyncProperty(gridDest, propertyName, val);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(propertyName);
                Trace.WriteLine(ex.ToString());
            }
        }

        private void ResetGrid(GridControlBase grid)
        {
            if (typeof(GridControl).IsInstanceOfType(grid))
            {
                ((GridControl)grid).ResetAlphaBlendSelectionColor();
                ((GridControl)grid).ResetBackgroundImageID();
                ((GridControl)grid).ResetBanneredRanges();
                ((GridControl)grid).ResetBaseStylesMap();
                ((GridControl)grid).ResetColHiddenEntries();
                ((GridControl)grid).ResetColWidthEntries();
                ((GridControl)grid).ResetCoveredRanges();
                ((GridControl)grid).ResetCurrentCellInfo();
                ((GridControl)grid).ResetRangeStyles();
                ((GridControl)grid).ResetRowHeightEntries();
                ((GridControl)grid).ResetRowHiddenEntries();
                ((GridControl)grid).ResetTableStyle();
                ((GridControl)grid).ResetText();
                ((GridControl)grid).ResetVolatileData();
            }
        }

        private void syncProps_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.OnGridSynchronizerPropertiesChanged(sender, e);
            }

            if (bInitialized)
            {
                if (GridDesignerMain.syncProps.Modified)
                {
                    try
                    {
                        this.SyncProperty(dup, e.PropertyName);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }
        #endregion

        #region Properties
        public static bool SaveGroupStyle
        {
            get
            {
                return bSaveGroupStyles;
            }

            set
            {
                bSaveGroupStyles = value;
            }
        }

        #endregion
    }
}
