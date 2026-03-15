// <copyright file="FormatedText3D.cs" company="Syncfusion">
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
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents FormattedText3D
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class FormattedText3D
    {
        #region Members
        /// <summary>
        /// Initializes m_formattedText
        /// </summary>
        private FormattedText m_formattedText = null;

        /// <summary>
        /// Initializes m_drawingVisual
        /// </summary>
        private DrawingVisual m_drawingVisual = null;

        /// <summary>
        /// Initializes m_drawingBrush
        /// </summary>
        private DrawingBrush m_drawingBrush = null;

        /// <summary>
        /// Initializes m_model
        /// </summary>
        private Model3D m_model = null;

        /// <summary>
        /// Initializes m_rect
        /// </summary>
        private Rect3D m_rect = new Rect3D();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the FormattedText
        /// </summary>
        public FormattedText FormattedText
        {
            get
            {
                return this.m_formattedText;
            }

            set
            {
                if (this.m_formattedText != value)
                {
                    this.m_formattedText = value;
                    this.UpdateBrush();
                }
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
        /// Initializes a new instance of the FormattedText3D class
        /// </summary>
        public FormattedText3D()
        {
            this.Init();
        }
        #endregion

        #region Implementation        

        /// <summary>
        /// The OnRender method
        /// </summary>
        /// <param name="drawingContext">The drawingContext</param>
        protected void OnRender(DrawingContext drawingContext)
        {
            string testString = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor";

            // Create the initial formatted text string.
            FormattedText formattedText = new FormattedText(
                testString,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                32,
                Brushes.Black);

            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            formattedText.MaxTextWidth = 300;
            formattedText.MaxTextHeight = 240;

            // Use a larger font size beginning at the first (zero-based) character and continuing for 5 characters.
            // The font size is calculated in terms of points -- not as device-independent pixels.
            formattedText.SetFontSize(36 * (96.0 / 72.0), 0, 5);

            // Use a Bold font weight beginning at the 6th character and continuing for 11 characters.
            formattedText.SetFontWeight(FontWeights.Bold, 6, 11);

            // Use a linear gradient brush beginning at the 6th character and continuing for 11 characters.
            formattedText.SetForegroundBrush(
                                    new LinearGradientBrush(Colors.Orange, Colors.Teal, 90.0), 6, 11);

            // Use an Italic font style beginning at the 28th character and continuing for 28 characters.
            formattedText.SetFontStyle(FontStyles.Italic, 28, 28);

            // Draw the formatted text string to the DrawingContext of the control.
            drawingContext.DrawText(formattedText, new Point(0, 0));
        }

        /// <summary>
        /// The Init method
        /// </summary>
        private void Init()
        {
            this.m_drawingVisual = new DrawingVisual();
            this.m_drawingBrush = new DrawingBrush(this.m_drawingVisual.Drawing);

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

            MaterialGroup material = new MaterialGroup();

            material.Children.Add(new DiffuseMaterial(this.m_drawingBrush));
            material.Children.Add(new EmissiveMaterial(this.m_drawingBrush));
            material.Children.Add(new SpecularMaterial(this.m_drawingBrush, 0));

            this.m_model = new GeometryModel3D(mg, material);
        }

        /// <summary>
        /// The UpdateBrush method
        /// </summary>
        private void UpdateBrush()
        {
            DrawingContext dc = this.m_drawingVisual.RenderOpen();

            dc.DrawRectangle(Brushes.Yellow, null, new Rect(0, 0, 0.5, 0.5));
            dc.DrawLine(new Pen(Brushes.YellowGreen, 1), new Point(0, 0), new Point(50, 40));
            dc.DrawLine(new Pen(Brushes.Red, 1), new Point(60, 0), new Point(0, 40));
            ////dc.DrawText(this.m_formattedText, new Point(0, 0));
            this.OnRender(dc);
            dc.Close();

            this.m_drawingBrush.Drawing = this.m_drawingVisual.Drawing;
        }

        /// <summary>
        /// The Update method
        /// </summary>
        /// <seealso cref="FormattedText3D"/>
        private void Update()
        {
            Transform3DGroup tg = new Transform3DGroup();

            ////      tg.Children.Add(new ScaleTransform3D(this.m_rect.SizeX, this.m_rect.SizeY, m_rect.SizeZ));
            tg.Children.Add(new TranslateTransform3D(this.m_rect.X, this.m_rect.Y, this.m_rect.Z));

            this.m_model.Transform = tg;
        }
        #endregion
    }
}
