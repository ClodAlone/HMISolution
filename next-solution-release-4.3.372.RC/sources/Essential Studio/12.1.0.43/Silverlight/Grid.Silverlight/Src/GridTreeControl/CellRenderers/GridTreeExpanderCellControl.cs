#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GridTreeExpanderCellControl : Control
    {
        /// <summary>
        /// Gets and Privately Sets the ExpanderGlyphType of CustomPlusPath of the GridTreeControl to GridTreeExpanderCellControl.
        /// </summary>
        public Path CustomPlusPath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets and Privately Sets the ExpanderGlyphType of CustomMinusPath of the GridTreeControl to GridTreeExpanderCellControl.
        /// </summary>
        public Path CustomMinusPath
        {
            get;
            internal set;
        }

        private WriteableBitmap plusOrMinusBitmap;

        public WriteableBitmap PlusOrMinusBitmap
        {
            get { return plusOrMinusBitmap; }
            set { plusOrMinusBitmap = value; }
        }
        private GridControlBase gridControl;

        /// <summary>
        /// DependencyProperty for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridTreeExpanderCellControl),
            new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)this.GetValue(GridTreeExpanderCellControl.TextProperty);
            }

            set
            {
                this.SetValue(GridTreeExpanderCellControl.TextProperty, value);
            }
        }

        private bool isTextPropertyChangedBeforeInitialization = false;
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridTreeExpanderCellControl expanderCell = d as GridTreeExpanderCellControl;
            if (expanderCell.isTemplateApplied)
            {
                expanderCell.TextBlockPart.Text = (string)args.NewValue;
            }
            else
            {
                expanderCell.isTextPropertyChangedBeforeInitialization = true;
            }

        }

        public bool IsInSuspend
        {
            get;
            internal set;
        }

        public bool IsContentInitialized
        {
            get;
            internal set;
        }

        public GridTreeExpanderCellControl()
        {
            this.DefaultStyleKey = typeof(GridTreeExpanderCellControl);
        }

        /// <summary>
        /// To preserve the node image size.
        /// </summary>
        private Size ImageSize = Size.Empty;

        private bool isTemplateApplied = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.TextBlockPart = this.GetTemplateChild("PART_TextBlock") as TextBlock;
            this.Glyph = this.GetTemplateChild("PART_GLYPH") as Image;
            //the nodeimage binds to the templates child.
            this.NodeImage = this.GetTemplateChild("PART_NODEIMAGE") as Image;
            this.SetNodeImages();
            this.Glyph.MouseLeftButtonDown += new MouseButtonEventHandler(Glyph_MouseLeftButtonDown);                     
            this.SetProperties();
            this.isTemplateApplied = true; 
        }

        //here the NodeImage of the ExpanderCell is getting from GridTreeRequestNodeImageEventArgs event and set to GridTreeExpanderCellControl NodeImage.
        void SetNodeImages()
        {
            if (GridControl != null && (GridControl as GridTreeControlImpl).SupportNodeImages)
            {                
                GridTreeRequestNodeImageEventArgs args = new GridTreeRequestNodeImageEventArgs(this, GridControl as GridTreeControlImpl);
                (GridControl as GridTreeControlImpl).OnRequestNodeImage(args);
                this.NodeImage.Source = args.NodeImage;

                if (args.NodeImage != null)
                {
                    /// Event hooked to get the exact height and width of the image.
                    args.NodeImage.DownloadProgress += OnNodeImageDownloadProgress;
                }
            } 
        }

        /// <summary>
        /// Called when [node image download progress].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Media.Imaging.DownloadProgressEventArgs"/> instance containing the event data.</param>
        void OnNodeImageDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            BitmapImage src = (sender as BitmapImage);

            if (src == null || this.gridControl == null)
                return;

            /// Image size is preserved to reuse it in measure override
            this.ImageSize = new Size(src.PixelWidth, src.PixelHeight);

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            /// arranging the control based on desired size
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));

            /// Invalidating the visual to reflect the change in visual
            this.gridControl.InvalidateVisual();
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!ImageSize.IsEmpty)
            {
                /// New Image size is applied
                this.NodeImage.Height = ImageSize.Height;
                this.NodeImage.Width = ImageSize.Width;
            }

            return base.MeasureOverride(availableSize);
        }

        void GridTreeExpanderCellControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Node != null)
            {
                this.SetResource(this.Node.Expanded);
                this.Glyph.Visibility = this.Node.HasChildNodes == true ? Visibility.Visible : Visibility.Collapsed;
                this.Glyph.Source = this.plusOrMinusBitmap;
            }
        }

        void Glyph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetResource(this.Node.Expanded);
            this.Glyph.Source = this.plusOrMinusBitmap;
            e.Handled = true;
        }

        /// <summary>
        /// Gets and Sets the ExpanderGlyphType of the GridTreeControl to GridTreeExpanderCellControl.
        /// </summary>
        public GridTreeExpandGlyph ExpandGlyphType
        {            
            get;
            set;
        }

        private void SetResource(bool open)
        {
            switch(ExpandGlyphType)
            {
                    //here only sets the ExpanderGlyphType paths according to its type.
                case GridTreeExpandGlyph.PlusMinus:                    
                    if (!open)
                    {                        
                        string plusPathData = "M0.5,0.5L149.5,0.5L149.5,149.5L0.5,149.5zM25.5,72.5L124.5,72.5L124.5,77.5L25.5,77.5zM72.5,25.5L77.5,25.5L77.5,124.5L72.5,124.5z";
                        this.DrawPlusMinusExpander(plusPathData);                                               
                    }
                    else
                    {
                        string minusPathData = "M0.5,0.5L149.5,0.5L149.5,149.5L0.5,149.5zM25.5,72.5L124.5,72.5L124.5,77.5L25.5,77.5z";
                        this.DrawPlusMinusExpander(minusPathData);                                                
                    }                    
                    break;
                case GridTreeExpandGlyph.Triangle:
                    if (!open)
                    {        
                        string plusPathData = "M-91.702,-146.448L68.0101,-31.5536L-91.702,99.2988z"; 
                        this.DrawPlusMinusExpander(plusPathData);  
                    }                 
                    else         
                    {         
                        string minusPathData = "M105.264,-136.319L106.268,65.2559L-91.702,65.2559z"; 
                        this.DrawPlusMinusExpander(minusPathData); 
                    }  
                    break;  
                case GridTreeExpandGlyph.Custom:
                    Path customPath;
                    if (!open)     
                    {
                        customPath = this.CustomPlusPath;
                        if (customPath.Fill == null)
                        {
                            customPath.Fill = this.PlusMinusButtonBackground;
                        }
                        if (customPath.Stroke == null)
                        {
                            customPath.Stroke = this.PlusMinusButtonForeground;
                        }
                        if (double.IsNaN(customPath.Width))
                        {
                            customPath.Width = 20;
                        }
                        if (double.IsNaN(customPath.Height))
                        {
                            customPath.Height = 20;
                        }
                        if (customPath.StrokeThickness == 1.0)
                        {
                            customPath.StrokeThickness = .5;
                        }
                        if (customPath.Stretch == Stretch.None)
                        {
                            customPath.Stretch = Stretch.Fill;
                        }
                    }        
                    else           
                    {
                        customPath = this.CustomMinusPath;
                        if (customPath.Fill == null)
                        {
                            customPath.Fill = this.PlusMinusButtonBackground;
                        }
                        if (customPath.Stroke == null)
                        {
                            customPath.Stroke = this.PlusMinusButtonForeground;
                        }
                        if (double.IsNaN(customPath.Width))
                        {
                            customPath.Width = 20;
                        }
                        if (double.IsNaN(customPath.Height))
                        {
                            customPath.Height = 20;
                        }
                        if (customPath.StrokeThickness == 1.0)
                        {
                            customPath.StrokeThickness = 0.5;
                        }
                        if (customPath.Stretch == Stretch.None)
                        {
                            customPath.Stretch = Stretch.Fill;
                        }
                    }
                    this.plusOrMinusBitmap = new WriteableBitmap(25, 25);
                    this.plusOrMinusBitmap.Render(customPath, null);
                    this.plusOrMinusBitmap.Invalidate();          
                    break;      
            }
        }

        //Creates the image Path of the ExpanderGlyphType
        private Path GeneratePath(string data)
        {
            string pathEnvelope = ("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"{0}\"/>");
            return System.Windows.Markup.XamlReader.Load(String.Format(pathEnvelope, data)) as Path;
        }

        //Draws the ExpanderGlyphType image as bitmapimage
        private void DrawPlusMinusExpander(string expanderPathData)
        {           
            Path expanderPath = GeneratePath(expanderPathData);
            expanderPath.Fill = this.PlusMinusButtonBackground; 
            expanderPath.Stroke = this.PlusMinusButtonForeground;
            expanderPath.Width = 20;
            expanderPath.Height = 20;
            expanderPath.StrokeThickness = .5; 
            expanderPath.Stretch = Stretch.Fill;
            this.plusOrMinusBitmap = new WriteableBitmap(25, 25);
            this.plusOrMinusBitmap.Render(expanderPath, null);
            this.plusOrMinusBitmap.Invalidate();
        }





        public void SetProperties()
        {
            if (this.isTextPropertyChangedBeforeInitialization)
            {
                this.TextBlockPart.Text = this.Text;
            }

            if (this.Node != null)
            {
                this.SetResource(this.Node.Expanded);
                this.Glyph.Visibility = this.Node.HasChildNodes == true ? Visibility.Visible : Visibility.Collapsed;
                this.Glyph.Source = this.plusOrMinusBitmap;
            }
        }

        public GridControlBase GridControl
        {
            get
            {
                return gridControl;
            }
            set { gridControl = value; }
        }


    

        public Brush PlusMinusButtonBackground
        {
            get;
            set;
        }

        public Brush PlusMinusButtonForeground
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and private sets the NodeImage of UIElement associated with the Expander Cell Control.
        /// </summary>
        public Image NodeImage
        {   
            get; 
            private set;
        }


        public Image Glyph
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the TextBlock UIElement associated with the Expander Cell Control.
        /// </summary>
        public TextBlock TextBlockPart
        {
            get;
            private set;
        }

        public GridTreeNode Node
        {
            get;
            set;
        }
    }
}
