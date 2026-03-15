// <copyright file="TextBlock3D.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents TextBlock3D
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class TextBlock3D
    {
        #region Members
        /// <summary>
        /// Initializes m_textBlock
        /// </summary>
        private TextBlock m_textBlock = null;

        /// <summary>
        /// Initializes m_visualBrush
        /// </summary>
        private VisualBrush m_visualBrush = null;

        /// <summary>
        /// Initializes m_model
        /// </summary>
        private Model3D m_model = null;

        /// <summary>
        /// Initializes m_rect
        /// </summary>
        private Rect3D m_rect = new Rect3D(0, 0, 0, 1, 1, 0);

        /// <summary>
        /// Initializes m_image
        /// </summary>
        private BitmapImage m_image = new BitmapImage();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the TextBlock
        /// </summary>
        public TextBlock TextBlock
        {
            get
            {
                return this.m_textBlock;
            }
        }

        /// <summary>
        /// Gets the Model
        /// </summary>
        public Model3D Model
        {
            get
            {
                return this.m_model;
            }
        }

        /// <summary>
        /// Gets or sets the Rect
        /// </summary>
        public Rect3D Rect
        {
            get
            {
                return this.m_rect;
            }

            set
            {
                this.m_rect = value;
                this.Update();
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the TextBlock3D class
        /// </summary>
        public TextBlock3D()
        {
            this.Init();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// The Init method
        /// </summary>
        private void Init()
        {
            this.m_textBlock = new TextBlock();
            this.m_visualBrush = new VisualBrush();
            this.m_visualBrush.Visual = this.m_textBlock;

            MeshGeometry3D mg = new MeshGeometry3D();

            mg.Positions.Add(new Point3D(0, 0, 0));
            mg.Positions.Add(new Point3D(1, 0, 0));
            mg.Positions.Add(new Point3D(1, 1, 0));
            mg.Positions.Add(new Point3D(0, 1, 0));

            mg.Normals.Add(new Vector3D(0, 0, -1));
            mg.Normals.Add(new Vector3D(0, 0, -1));
            mg.Normals.Add(new Vector3D(0, 0, -1));
            mg.Normals.Add(new Vector3D(0, 0, -1));

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));
            mg.TextureCoordinates.Add(new Point(0, 0));

            ImageBrush img = new ImageBrush(this.m_image);

            MaterialGroup material = new MaterialGroup();

            material.Children.Add(new DiffuseMaterial(img));
            material.Children.Add(new EmissiveMaterial(img));
            material.Children.Add(new SpecularMaterial(img, 0));

            GeometryModel3D gmodel = new GeometryModel3D(mg, material);

            ////      gmodel.BackMaterial = new DiffuseMaterial(this.m_visualBrush);

            this.m_model = gmodel;
        }

        /// <summary>
        /// The Update method
        /// </summary>
        /// <seealso cref="TextBlock3D"/>
        private void Update()
        {
            Transform3DGroup tg = new Transform3DGroup();
            ImageDrawing imgDrawing = new ImageDrawing(this.m_image, new Rect(0, 0, this.m_textBlock.ActualWidth, this.m_textBlock.ActualHeight));

            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();

            dc.DrawRectangle(this.m_visualBrush, null, imgDrawing.Rect);
            dc.Close();

            ////      tg.Children.Add(new ScaleTransform3D(this.m_rect.SizeX, this.m_rect.SizeY, this.m_rect.SizeZ));
            tg.Children.Add(new TranslateTransform3D(this.m_rect.X, this.m_rect.Y, this.m_rect.Z));

            this.m_model.Transform = tg;
        }
        #endregion
    }
}