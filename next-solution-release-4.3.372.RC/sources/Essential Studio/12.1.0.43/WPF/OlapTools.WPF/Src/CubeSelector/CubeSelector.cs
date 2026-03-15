#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Olap
{
    using System.Windows;
    using System.Windows.Controls;
    using Syncfusion.Olap.Data;
    using Syncfusion.Olap.Manager;
    using System.Collections.Generic;
    using System;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// CubeSelector lists the available type in the Selected Analysis srevice database
    /// </summary>
   // [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   //Type = typeof(CubeSelector), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Office2007Black,
   // Type = typeof(CubeSelector), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Office2007Silver,
   // Type = typeof(CubeSelector), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Office2003,
   // Type = typeof(CubeSelector), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   // [SkinType(SkinVisualStyle = Skin.Blend,
   // Type = typeof(CubeSelector), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")] 
   // [SkinType(SkinVisualStyle = Skin.Default,
   // Type = typeof(CubeSelector), XamlResource = "/Syncfusion.OlapTools.WPF;component/Themes/Generic.xaml")]
   
    public class CubeSelector : ComboBox
    {
        #region Dependency Property Implementation

        /// <summary>
        /// OlapDataManager Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(IOlapDataManager), typeof(CubeSelector), new UIPropertyMetadata(CubeSelector.OlapDataManagerChanged));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeSelector"/> class.
        /// </summary>
        public CubeSelector()
        {
            this.SelectionChanged += new SelectionChangedEventHandler(this.CubeSelector_SelectionChanged);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the cube.
        /// </summary>
        /// <value>The name of the cube.</value>
        public string CubeName
        {
            get
            {
                 if (( !string.IsNullOrEmpty(this.SelectionBoxItem.ToString()) && this.SelectedItem != null) || (this.OlapDataManager.CurrentCubeName == null && SelectedItem != null))
                 {
                     return SelectedItem.ToString();
                 }
                 else if (this.OlapDataManager != null && this.OlapDataManager.CurrentCubeName != null)
                 {
                     foreach (string cubename in this.ItemsSource)
                     {
                         if (cubename == this.OlapDataManager.CurrentCubeName.ToString())
                             return this.OlapDataManager.CurrentCubeName.ToString();
                     }
                 }
                
                return string.Empty;
            }
        }

        
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public IOlapDataManager OlapDataManager
        {
            get { return (IOlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="cubes">The cubes.</param>
        private void BindData(CubeInfoCollection cubes)
        {
            List<string> names = new List<string>();
            foreach(CubeInfo cube in cubes)
            {
                names.Add(cube.Caption);
            }
            this.ItemsSource = names;
            this.Text = "{Binding}";
            if (cubes.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the CubeSelector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void CubeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.OlapDataManager != null && this.OlapDataManager.DataProvider!=null)
                {
                    foreach (CubeInfo item in this.OlapDataManager.DataProvider.GetCubes)
                    {
                        if (item.Caption.Equals(this.CubeName.ToString()))
                        {
                            this.OlapDataManager.CurrentCubeName = item.Name;
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cube Selector");
            }
        }

        #endregion

        #region Public static Methods

        private void WireEvents()
        {
            if (this.OlapDataManager != null)
            {
                this.OlapDataManager.ReportChanged += new ReportChangedEventHandler(OlapDataManager_ReportChanged);
            }
        }

        void OlapDataManager_ReportChanged(object sender, ReportChangedEventArgs e)
        {
            this.SelectedItem =  e.NewReport.CurrentCubeName;
        }

        /// <summary>
        /// Cubes the model changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OlapDataManagerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is CubeSelector)
            {
                CubeSelector cubeSelector = (CubeSelector)dependencyObject;
                cubeSelector.BindData(cubeSelector.OlapDataManager.DataProvider.GetCubes);
                cubeSelector.WireEvents();
            }
        }

        #endregion
    }
}
