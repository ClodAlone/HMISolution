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
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.ComponentModel;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ImageGeneral.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ImageGeneral
        : UserControl
    {
        #region Constructor
        public ImageGeneral()
        {
            InitializeComponent();
            this.cmb_ImageSource.SelectedItem = ImageSourceType.Embedded;
            string appLocation = AppDomain.CurrentDomain.BaseDirectory.ToString();
            this.cmb_ImageSource.SelectionChanged += new SelectionChangedEventHandler(cmb_ImageSource_SelectionChanged);
            this.btn_Browse.Click += new RoutedEventHandler(btn_Browse_Click);
        }

        public ImageGeneral(ImageSourceType Type)
        {
            InitializeComponent();
            this.cmb_ImageSource.SelectedItem = Type;
            string appLocation = AppDomain.CurrentDomain.BaseDirectory.ToString();
            this.cmb_ImageSource.SelectionChanged += new SelectionChangedEventHandler(cmb_ImageSource_SelectionChanged);
            this.btn_Browse.Click += new RoutedEventHandler(btn_Browse_Click);
        }

        internal System.Drawing.Imaging.ImageFormat PopulateImageFormatFromString(string extension)
        {
            switch (extension.ToLower())
            {
                case "png":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case "bmp":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                case "gif":
                    return System.Drawing.Imaging.ImageFormat.Gif;
                case "emf":
                    return System.Drawing.Imaging.ImageFormat.Emf;
                default:
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
        }

        private void btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Images (*.jpg, *.jpeg , *.jpe, *.gif, *.png, *.bmp)|*.jpg;*.jpeg;*.jpe;*.gif;*.png;*.bmp|All Files|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!this.cmb_ExternalImageName.Items.Contains(openFileDialog.FileName))
                {
                    this.cmb_ExternalImageName.Items.Add(openFileDialog.FileName);
                    this.cmb_ExternalImageName.SelectedItem = openFileDialog.FileName;
                }
            }
        }

        void cmb_ImageSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0].ToString() == ImageSourceType.Embedded.ToString())
            {
                this.stk_ExternalImage.Visibility = Visibility.Hidden;
                this.stk_EmbeddedImage.Visibility = Visibility.Visible;
            }
            else if (e.AddedItems[0].ToString() == ImageSourceType.Database.ToString())
            {
                this.stk_ExternalImage.Visibility = Visibility.Hidden;
                this.stk_EmbeddedImage.Visibility = Visibility.Visible;
            }
            else
            {
                this.stk_ExternalImage.Visibility = Visibility.Visible;
                this.stk_EmbeddedImage.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Public Properties
        public new System.Windows.Controls.TextBox Name
        {
            get
            {
                return this.txt_GeneralName;
            }
            set
            {
                this.txt_GeneralName = value;
            }
        }

        public System.Drawing.Image ImageContent { get; set; }

        public System.Drawing.Imaging.ImageFormat ImageFormat { get; set; }

        public new ExpressionComboBox ToolTip
        {
            get
            {
                return this.txt_ToolTip;
            }
            set
            {
                this.txt_ToolTip = value;
            }
        }

        public System.Windows.Controls.ComboBox ImageSource
        {
            get
            {
                return this.cmb_ImageSource;
            }
            set
            {
                this.cmb_ImageSource = value;
            }
        }

        public ExpressionComboBox ImageName
        {
            get
            {
                return this.cmb_ImageName;
            }
            set
            {
                this.cmb_ImageName = value;
            }
        }

        public System.Windows.Controls.Button ImportImage
        {
            get
            {
                return this.btn_Import;
            }
            set
            {
                this.btn_Import = value;
            }
        }
        #endregion
    }
}
