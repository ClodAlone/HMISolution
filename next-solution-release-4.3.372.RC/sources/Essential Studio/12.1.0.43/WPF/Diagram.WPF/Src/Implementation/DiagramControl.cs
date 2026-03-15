// <copyright file="DiagramControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Xml.Linq;
using System.Reflection;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Diagram control.
    /// </summary>
    /// <remarks>
    /// <para>The Diagram control is the base class which contains the view and the model. 
    /// It receives user input and translates it into actions and commands on the model and view.  
    /// It also implements symbol palette and scrolling, and enables horizontal and vertical scrollbars when the size of the view exceeds the size of the window. 
    /// </para>
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in XAML.
    /// <code language="XAML">
    /// &lt;Window x:Class="RulersAndUnits.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
    ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
    ///  Icon="Images/App.ico" &gt;
    ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
    ///                                IsSymbolPaletteEnabled="True" 
    ///                                Background="WhiteSmoke"&gt;
    ///              lt;syncfusion:DiagramControl.Model&gt;
    ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
    ///                  &lt;/syncfusion:DiagramModel&gt;
    ///             &lt;/syncfusion:DiagramControl.Model&gt;
    ///           &lt;syncfusion:DiagramControl.View&gt;
    ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
    ///                                        Background="LightGray"  
    ///                                        Bounds="0,0,12,12"  
    ///                                        ShowHorizontalGridLine="False" 
    ///                                        ShowVerticalGridLine="False"
    ///                                        Name="diagramView"  &gt;
    ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
    ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
    ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
    ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
    ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
    ///       &lt;/syncfusion:DiagramView&gt;
    ///    &lt;/syncfusion:DiagramControl.View&gt;
    /// &lt;/syncfusion:DiagramControl&gt;
    /// &lt;/Window&gt;
    /// </code>
    /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in C#.
    /// <code language="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using System.ComponentModel;
    /// using Syncfusion.Core;
    /// using Syncfusion.Windows.Diagram;
    /// namespace WpfApplication1
    /// {
    /// public partial class Window1 : Window
    /// {
    ///    public DiagramControl Control;
    ///    public DiagramModel Model;
    ///    public DiagramView View;
    ///    public Window1 ()
    ///    {
    ///       InitializeComponent ();
    ///       Control = new DiagramControl ();
    ///       Model = new DiagramModel ();
    ///       View = new DiagramView ();
    ///       Control.View = View;
    ///       Control.Model = Model;
    ///       HorizontalRuler hruler = new HorizontalRuler();
    ///       View.HorizontalRuler = hruler;
    ///       View.ShowHorizontalGridLine = false;
    ///       View.ShowVerticalGridLine = false;
    ///       VerticalRuler vruler = new VerticalRuler();
    ///       View.VerticalRuler = vruler;
    ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
    ///       View.IsPageEditable = true;
    ///    }
    ///    }
    ///    }
    /// </code>
    /// </example>
    /// <seealso cref="DiagramView"/>
    /// <seealso cref="DiagramModel"/>
    ///     
    /// 
    
    public partial class DiagramControl : ContentControl, INotifyPropertyChanged
    {
        #region Class Variables

        /// <summary>
        /// Used to check delay layout.
        /// </summary>
        internal bool m_delayLayout = false;

        /// <summary>
        /// Used to check if Diagram Control is unloaded.
        /// </summary>
        private bool isunloaded = false;

        /// <summary>
        /// Used to check if executed once.
        /// </summary>
        private bool exe = false;

        /*
        /// <summary>
        /// Used to store DiagramControl instance.
        /// </summary>
        ////private static DiagramControl dc;
        */

        /// <summary>
        /// Used to check if page is loaded.
        /// </summary>
        private static bool m_ispageloaded = false;

        /// <summary>
        /// Used to store SymbolPalette visibility changed value.
        /// </summary>
        private bool m_ispalettechanged = false;

        /// <summary>
        /// Used to check if page is saved or not.
        /// </summary>
        private bool ispagesaved = false;

        /// <summary>
        /// Used to check if Nodes.Clear() is called.
        /// </summary>
        private bool nodecleared = false;

        /// <summary>
        /// Used to check if node is removed or not.
        /// </summary>
        private bool noderemoved = false;

        /// <summary>
        /// Used to check if connector is removed or not.
        /// </summary>
        private bool connremoved = false;

        /// <summary>
        /// Checks if any node object is available for deletion.
        /// </summary>
        private bool isnodepresent = true;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="DiagramControl"/> class.
        /// </summary>
        static DiagramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramControl), new FrameworkPropertyMetadata(typeof(DiagramControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramControl"/> class.
        /// </summary>
        public DiagramControl()
        {            
            //EnvironmentTestDiagramWPF.ValidateLicense(typeof(DiagramControl));
            if (EnvironmentTestDiagramWPF.IsSecurityGranted)
            {
                EnvironmentTestDiagramWPF.StartValidateLicense(typeof(Syncfusion.Windows.Diagram.DiagramControl));
            }
            this.Loaded += new RoutedEventHandler(ParseItemsSource);
            this.Loaded += new RoutedEventHandler(DiagramControl_Loaded);
            this.Unloaded += new RoutedEventHandler(DiagramControl_Unloaded);
            this.LocalizationPath = "Resources";
            IsLoadingFromFile = false;
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
        }
        
        void ParseItemsSource(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (this.Model != null)
                {
                    this.Model.PrepareItems();
                    this.Loaded -= ParseItemsSource;
                }
            }

            if (Model != null && Model.parser != null && Model.parser.ChildItems != null)
            {
                foreach (ChildDataParser child in Model.parser.ChildItems)
                {
                    if (child.node != null)
                    {
                        Model.RootNodes.Add(child.node);
                    }
                }
            }

            if (Model != null)
            {
                if (Model.Nodes != null)
                {
                    if (Model.Nodes.Count == 0)
                    {
                        Model.LayoutRoot = null;
                    }

                    if (Model.LayoutRoot != null)
                    {
                        if (Model.Nodes.Contains(Model.LayoutRoot))
                        {
                            if (Model.RootNodes.Count > 0 && Model.LayoutRoot.Edges.Count == 0)
                            {
                                foreach (Node node in Model.RootNodes)
                                {
                                    if (node != Model.LayoutRoot)
                                    {
                                        LineConnector ortho;
                                        if (this.View != null)
                                        {
                                            ortho = new LineConnector(View);
                                        }
                                        else
                                        {
                                            ortho = new LineConnector(null);
                                        }
                                        ortho.HeadNode = Model.LayoutRoot;
                                        ortho.TailNode = node;
                                        ortho.Content = node.Content;
                                        Model.Connections.Add(ortho);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ////throw new InvalidOperationException(string.Format("Layout Root node should be in Model.Nodes collection"));
                        }
                    }
                    else
                    {
                        if (Model.RootNodes.Count > 0)
                        {
                            Model.LayoutRoot = Model.RootNodes[0] as IShape;
                        }
                    }
                }//
            }//

            //        DrawNodes();
            //        Model.InternalNodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(InternalNodes_CollectionChanged);
            //    }

            //    if (Model.Connections != null)
            //    {
            //        DrawConnectios();
            //        Model.Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Connections_CollectionChanged);
            //    }
            //}

            if (this.View != null && this.View.Page != null)
            {
                //this.View.Page.UpdateLayout();
            }
        }

        /// <summary>
        /// Calls DiagramControl_Loaded method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void DiagramControl_Loaded(object sender, RoutedEventArgs e)
        {
            int n = 0;
            int j;
            if (View != null)
            {
                foreach (UIElement element in View.Page.Children.OfType<ICommon>())
                {
                    if ((element.ReadLocalValue(Panel.ZIndexProperty) == DependencyProperty.UnsetValue))
                    {
                        Panel.SetZIndex(element, n++);
                        j = Panel.GetZIndex(element);
                    }
                }
            }
            CollectionExt.Cleared = false;
            //if (this.SymbolPalette == null)
            //{
            //    ResourceDictionary rs = new ResourceDictionary();
            //    rs.Source = new Uri("/Syncfusion.Diagram.WPF;component/Themes/SymbolPalette.xaml", UriKind.RelativeOrAbsolute);
            //    this.SymbolPalette = rs["defaultSymbolPalette"] as SymbolPalette;
            //}
            InvalidateMeasure();
            if (m_delayLayout)
            {
                if (View.PxBounds.Equals(new Thickness(0, 0, 0, 0))/*View.m_autoBounds*/ && (View.Scrollviewer == null || View.Scrollviewer.ActualWidth == 0.0))
                {
                    m_delayLayout = true;
                }
                else
                {
                    if (Model.LayoutType == LayoutType.DirectedTreeLayout)
                    {
                        DirectedTreeLayout tree = new DirectedTreeLayout(this.Model, this.View);
                        tree.RefreshLayout();
                        exe = true;
                    }
                    else if (Model.LayoutType == LayoutType.HierarchicalTreeLayout)
                    {
                        HierarchicalTreeLayout tree = new HierarchicalTreeLayout(this.Model, this.View);
#if SILVERLIGHT
                        tree.PrepareActivity(tree);
                        tree.StartNodeArrangement();
#endif
                        tree.RefreshLayout();
                        exe = true;
                    }
                    else if (Model.LayoutType == LayoutType.TableLayout)
                    {
                        TableLayout tree = new TableLayout(this.Model, this.View);
                        tree.RefreshLayout();
                        exe = true;
                    }
                    else if (Model.LayoutType == LayoutType.RadialTreeLayout)
                    {
                        RadialTreeLayout tree = new RadialTreeLayout(this.Model, this.View);
                        tree.RefreshLayout();
                        exe = true;
                    }
                    else if (Model.LayoutType == LayoutType.BowtieLayout)
                    {
                        BowtieLayout tree = new BowtieLayout(this.Model, this.View);
                        tree.RefreshLayout();
                        exe = true;
                    }
                    m_delayLayout = false;
                }
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the DiagramControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DiagramControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.IsUnloaded = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }

        #endregion

        #region Implementation

        #region SymbolPalette Save and Load

        #region SymbolPalette SaveandLoad

        public void SaveSymbolPalette()
        {
            SaveFileDialog dialog = new SaveFileDialog();           
            dialog.Title = "Save SymbolPalette";
            dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName != string.Empty)
                {
                    string extension = new FileInfo(dialog.FileName).Extension.ToLower(CultureInfo.InvariantCulture);
                    if (extension.Equals(".xaml"))
                    {
                        this.SaveSymbolPalette(dialog.FileName);
                    }
                }
            }
        }

        internal void SaveSymbolPalette(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(filename, settings);
            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;

            CollectionExt symbolGroups = new CollectionExt();
            //System.Windows.Markup.XamlWriter.Save(this.SymbolPalette.SymbolGroups, writer);
            foreach (SymbolPaletteGroup group in this.SymbolPalette.SymbolGroups)
            {
                symbolGroups.Add(group);
                //System.Windows.Markup.XamlWriter.Save(group as SymbolPaletteGroup, writer);
            }
            System.Windows.Markup.XamlWriter.Save(symbolGroups, writer);
            writer.Close();
        }
        public void LoadSymbolPalette()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load SymbolPalette";
            dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                LoadSymbolPalette(dialog.OpenFile());
            }
        }
        public void LoadSymbolPalette(Stream stream)
        {
            CollectionExt SymbolGroup = (CollectionExt)XamlReader.Load(stream);
            this.SymbolPalette.SymbolGroups.Clear();
            foreach (SymbolPaletteGroup group in SymbolGroup)
            {
                this.SymbolPalette.SymbolGroups.Add(group);
            }
            
        }

        #endregion

        #region SymbolPaletteGroup Save and Load

        public void SaveSymbolPaletteGroup(SymbolPaletteGroup SymbolGroup)
        {

            if (SymbolGroup!=null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "Save SymbolPaletteGroup";
                dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    if (dialog.FileName != string.Empty)
                    {
                        string extension = new FileInfo(dialog.FileName).Extension.ToLower(CultureInfo.InvariantCulture);
                        if (extension.Equals(".xaml"))
                        {
                            this.SaveSymbolPaletteGroup(dialog.FileName, SymbolGroup);
                        }
                    }
                }
            }
        }

        internal void SaveSymbolPaletteGroup(string filename, SymbolPaletteGroup SymbolPaletteGroup)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(filename, settings);
            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            System.Windows.Markup.XamlWriter.Save(SymbolPaletteGroup, writer);
            writer.Close();
        }

        public void LoadSymbolPaletteGroup()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load SymbolPaletteGroup";
            dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                LoadSymbolGroup(dialog.OpenFile());
            }
        }

        public void LoadSymbolPaletteGroup(string filename)
        {
            XmlReader xmlreader = XmlReader.Create(filename);
            StreamReader reader = new StreamReader(filename);
            string str = reader.ReadToEnd();
            SymbolPaletteGroup group = (SymbolPaletteGroup)XamlReader.Load(xmlreader);
            this.SymbolPalette.SymbolGroups.Add(group);
        }

        public void AppendSymbolPaletteGroup(SymbolPaletteGroup ReplaceGroup)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load XAML";
            dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                this.SymbolPalette.SymbolGroups.Remove(ReplaceGroup);
                LoadSymbolGroup(dialog.OpenFile());
            }
        }

        internal void LoadSymbolGroup(Stream stream)
        {
                SymbolPaletteGroup group = (SymbolPaletteGroup)XamlReader.Load(stream);
                this.SymbolPalette.SymbolGroups.Add(group);
        }

        #endregion

        #region SymbolPaletteItem SaveAndLoad

        public void SaveSymbolPaletteItem(SymbolPaletteItem symbolPaletteItem)
        {

            if (symbolPaletteItem!=null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "Save SymbolPaletteItem";
                dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    if (dialog.FileName != string.Empty)
                    {
                        string extension = new FileInfo(dialog.FileName).Extension.ToLower(CultureInfo.InvariantCulture);
                        if (extension.Equals(".xaml"))
                        {
                            this.SaveSymbolPaletteItem(dialog.FileName,symbolPaletteItem);
                        }
                    }
                }
            }
        }

        internal void SaveSymbolPaletteItem(string filename, SymbolPaletteItem SymbolPaletteItem)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(filename, settings);
            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            System.Windows.Markup.XamlWriter.Save(SymbolPaletteItem, writer);
            writer.Close();
        }
        public void LoadSymbolPaletteItem(SymbolPaletteGroup SymbolPaletteGroup)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load SymbolPaletteItem";
            dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                LoadSymbolItem(dialog.OpenFile(),SymbolPaletteGroup);
            }
        }
        internal void LoadSymbolItem(Stream stream,SymbolPaletteGroup symbolgroup)
        {
            try
            {
                SymbolPaletteItem item = (SymbolPaletteItem)XamlReader.Load(stream);
                symbolgroup.Items.Add(item);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        #endregion

        #endregion

        #region Importing SymbolFrom Visio and WF

        //public void ImportVisioXMLStenchil()
        //{
        //    OpenFileDialog dialog = new OpenFileDialog();
        //    dialog.Title = "Load XAML";
        //    dialog.Filter = "EDP Files (*.edp)|*.xaml|All Files (*.*)|*.*";
        //    if (dialog.ShowDialog() == true)
        //    {
        //        ImportVisio(dialog.OpenFile());
        //    }
        //}

        //internal void ImportVisio(Stream stream)
        //{
        //    bool abcd = true;
        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.Load(stream);
        //    //BinaryFormatter soap = new BinaryFormatter();
        //    //object xml= (object)soap.Deserialize(stream);
        //    //xmldoc.LoadXml(xml.ToString());
        //    ProcessXmlDocument(xmldoc);

        //    #region oldone

        //    //XmlNodeList xml = xmldoc.GetElementsByTagName("Shape");
        //    //XmlNodeList xmlnewlist;
        //    //foreach (XmlElement node in xml)
        //    //{
        //    //    xmlnewlist = node.GetElementsByTagName("XForm");
                
        //    //}
        //    //foreach (XmlElement node in xml)
        //    //{
        //    //    if (abcd)
        //    //    {

        //    //    }
        //    //    else
        //    //    {
        //    //        abcd = true;
        //    //    }
        //    //    foreach (XmlElement nod in node)
        //    //    {
        //    //        foreach (XmlElement no in nod)
        //    //        {

        //    //        }
        //    //    }
        //    //}
        //    #endregion
        //}

        //private void ProcessXmlDocument(XmlDocument doc)
        //{
        //    if (doc.HasChildNodes)
        //    {
        //        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        //        {
        //            switch (node.LocalName.ToLower())
        //            {
        //                //case "documentproperties":
        //                //    ProcessDocProps(node);
        //                //    break;
        //                case "pages":
        //                    ProcessStencils(node);
        //                    break;
        //                //case "stylesheets":
        //                //    ProcessStyleSheets(node);
        //                //    break;
        //                //case "colors":
        //                //    ProcessColors(node);
        //                //    break;
        //                //case "facenames":
        //                //    ProcessFaceNames(node);
        //                //    break;
        //            }
        //        }
        //    }
        //}
        //private void ProcessStencils(XmlNode nodeMasters)
        //{
        //    if (nodeMasters.HasChildNodes)
        //    {
        //        //CreateSymbolPalette();
        //        foreach (XmlNode nodeMaster in nodeMasters)
        //        {
        //            ProcessCurrStencil(nodeMaster);
        //        }

        //        //m_hashVisioShapes.Clear();
        //    }
        //}
        //private void ProcessCurrStencil(XmlNode nodeMaster)
        //{
        //    if (nodeMaster.HasChildNodes)
        //    {
        //        XmlElement masterElement = (XmlElement)nodeMaster;

        //        string strSymbolName = masterElement.GetAttribute("Name");
        //        string strSymbolNameU = masterElement.GetAttribute("NameU");

        //        if (strSymbolName == string.Empty)
        //            strSymbolName = strSymbolNameU;

        //        int nID = int.Parse(masterElement.GetAttribute("ID"));
        //        foreach (XmlNode curNode in nodeMaster.ChildNodes)
        //        {
        //            switch (curNode.LocalName.ToLower())
        //            {
        //                case "shapes":
        //                    ProcessCurrShape(curNode);
        //                    //m_symbolTemp = new Group();
        //                    //m_bMainShape = true;
        //                    //NodeCollection mainShapeChildren = new NodeCollection();
        //                    //ProcessShapes(curNode, m_symbolTemp, mainShapeChildren);
        //                    //int start;
        //                    //m_symbolTemp.AppendChildren(mainShapeChildren, out start);
        //                    //m_symbolTemp.Name = strSymbolName;

        //                    //// update EditStyle
        //                    //EditStyle stlEdit = new EditStyle();
        //                    XmlNode nodeShape = null;
        //                    // find shape node
        //                    foreach (XmlElement n in curNode.ChildNodes)
        //                    {
        //                        if (n.LocalName.ToLower() == "shape")
        //                        {
        //                            nodeShape = n;
        //                            break;
        //                        }
        //                    }

        //                    if (nodeShape.HasChildNodes)
        //                    {
        //                        foreach (XmlElement n in nodeShape.ChildNodes)
        //                        {
        //                            if (n.LocalName.ToLower() == "protection")
        //                            {
        //                                //ProcessProtection(n, stlEdit);
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private void ProcessCurrShape(XmlNode shape)
        //{
        //    if (shape.HasChildNodes)
        //    {
        //        foreach (XmlNode curNode in shape.ParentNode.ChildNodes)
        //        {
        //            switch (curNode.LocalName.ToLower())
        //            {
        //                case "shapes":
        //                    break;
        //                case "xform":
        //                    break;
        //            }
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// Sets the namescope.
        /// </summary>
        /// <param name="obj">The object to set the scope to.</param>
        internal void SetScope(DependencyObject obj)
        {
            DependencyObject ele = this;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!ns.GetType().FullName.Contains("TemplateNameScope"))
                    {
                        NameScope.SetNameScope(obj, ns);
                    }
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }
        
        /// <summary>
        /// Gets the isolated file contents .
        /// </summary>
        /// <param name="style">The style of the node or line connector.</param>
        /// <returns>The contents stored in the isolated file.</returns>
        private string GetIsolatedFileContents(Style style)
        {
            string tempfile = "tempfile.txt";
            string s = string.Empty;
            IsolatedStorageFile isoFile =
            IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                //// write data to the stream
                IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(tempfile, FileMode.OpenOrCreate, FileAccess.Write, isoFile);
                try
                {
                    XmlWriter writer = XmlWriter.Create(isoStream);
                    try
                    {
                        System.Windows.Markup.XamlWriter.Save(style, writer);
                    }
                    finally
                    {
                        writer.Close();
                    }
                }
                finally
                {
                    isoStream.Close();
                }

                //// read data back from the stream
                isoStream = new IsolatedStorageFileStream(tempfile, FileMode.Open, FileAccess.Read, isoFile);
                try
                {
                    StreamReader reader = new StreamReader(isoStream);
                    try
                    {
                        s = reader.ReadLine();
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    isoStream.Close();
                }
            }
            finally
            {
                isoFile.Close();
            }

            return s;
        }

        /// <summary>
        /// Saves the page as bitmap or xaml.Opens the save dialog box to select the  format.
        /// </summary>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       Control.Save();
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void Save()
        {
            SaveViewstateProperties();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save XAML";
            dialog.Filter = "XAML File (*.xaml)|*.xaml|Bitmap Files (*.bmp)|*.bmp|JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|TIF Files (*.tif)|*.tif|GIF Files (*.gif)|*.gif|WDP File (*.wdp)|*.wdp";
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName != string.Empty)
                {
                    string extension = new FileInfo(dialog.FileName).Extension.ToLower(CultureInfo.InvariantCulture);
                    if (extension.Equals(".xaml"))
                    {
                        this.Save(dialog.FileName);
                    }
                    else
                    {
                        View.Save(dialog.FileName);
                    }
                }
            }
        }

        private void SaveViewstateProperties()
        {
            (this.View.Page as DiagramPage).Viewstate.CurrentZoom = this.View.CurrentZoom;
            (this.View.Page as DiagramPage).Viewstate.ZoomFactor = this.View.ZoomFactor;
            (this.View.Page as DiagramPage).Viewstate.BoundaryConstraintsArea = this.View.BoundaryConstraintsArea;
            (this.View.Page as DiagramPage).Viewstate.BoundaryConstraintsEnabled = this.View.BoundaryConstraintsEnabled;
            (this.View.Page as DiagramPage).Viewstate.SizeToContent = this.View.SizeToContent;
            (this.View.Page as DiagramPage).Viewstate.PageBackground = this.View.PageBackground;
            (this.View.Page as DiagramPage).Viewstate.OffPageBackground = this.View.OffPageBackground;
            (this.View.Page as DiagramPage).Viewstate.PageMargin = this.View.PageMargin;
        }

        /// <summary>
        /// Saves the page into the specified file. Takes filename as a parameter.
        /// </summary>
        /// <param name="filename">Name of the XAML file to be loaded.</param>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       Control.Save("Hello.xaml");
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void Save(string filename)
        {
            SaveViewstateProperties();
            foreach (UIElement element in (View.Page as DiagramPage).AllChildren)
            {
                if (element is Group && !(element is Layer))
                {
                    foreach (INodeGroup child in (element as Group).NodeChildren)
                    {
                        (element as Group).GroupChildrenRef.Add(child.ReferenceNo);
                    }
                }
                else if (element is Layer)
                {
                    (element as Layer).NodesRef = new CollectionExt();
                    (element as Layer).LinesRef = new CollectionExt();

                    foreach (Node n in (element as Layer).Nodes)
                    {
                        if (((element as Layer).Parent as DiagramPage).Children.Contains(n))
                        {
                            (element as Layer).NodesRef.Add(n.ReferenceNo);
                        }
                    }
                    foreach (LineConnector l in (element as Layer).Lines)
                    {
                        if (((element as Layer).Parent as DiagramPage).Children.Contains(l))
                        {
                            (element as Layer).LinesRef.Add(l.ReferenceNo);
                        }
                    }
                }
                else if (element is Node)
                {
                    if ((element as Node).Content != null && (element as Node).Content.GetType() != typeof(string) && (element as Node).Content is UIElement && ((element as Node).Content as UIElement).IsHitTestVisible)
                    {
                        (element as Node).ContentHitTestVisible = true;
                    }
                    findCT(element as Node);
                }
                else if (element is LineConnector)
                {
                    (element as LineConnector).SetSerializationData();
                }
            }
            Style style = FindResource(typeof(Node)) as Style;
            XmlWriter writer1 = XmlWriter.Create(filename);
            System.Windows.Markup.XamlWriter.Save(style, writer1);
            writer1.Close();
            string filecontents = File.ReadAllText(filename);
            if (!filecontents.Contains("PART_Rotator") && !filecontents.Contains("PART_RotateOrigin") && !filecontents.Contains("PART_DragProvider") && filecontents.Contains("PART_Node"))
            {
                (View.Page as DiagramPage).StyleRef = style;
            }

            Style style2 = FindResource(typeof(LineConnector)) as Style;
            XmlWriter writer2 = XmlWriter.Create(filename);
            System.Windows.Markup.XamlWriter.Save(style2, writer2);
            writer2.Close();
            string filecontents2 = File.ReadAllText(filename);
            if (!filecontents2.Contains("PART_ConnectionPath") && !filecontents2.Contains("PART_SinkAnchorPath") && !filecontents2.Contains("PART_ConnectorLabelEditor")&& filecontents.Contains("PART_LineConnector"))
            {
                (View.Page as DiagramPage).LineStyleRef = style2;
            }

            (View.Page as DiagramPage).OrientationRef = Model.Orientation;
            (View.Page as DiagramPage).HorizontalSpacingref = Model.HorizontalSpacing;
            (View.Page as DiagramPage).VerticalSpacingref = Model.VerticalSpacing;
            (View.Page as DiagramPage).SubTreeSpacingref = Model.SpaceBetweenSubTrees;
            (View.Page as DiagramPage).LayoutTyperef = Model.LayoutType;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(filename, settings);
            XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            IspageSaved = true;
#if SyncfusionFramework3_5
            Style tempStyle = (this.View.Page as DiagramPage).Style;
            (this.View.Page as DiagramPage).Style = null;
#endif
            System.Windows.Markup.XamlWriter.Save((this.View.Page as DiagramPage), writer);
#if SyncfusionFramework3_5
            (this.View.Page as DiagramPage).Style = tempStyle;
#endif
            writer.Close();
            IspageSaved = false;
            View.IsPageSaved = true;
        }

        private void findCT(Node node)
        {
            if (node.Content == null)
            {
                return;
            }
            if (node.Content is String)
            {
                return;
            }
            if (node.Content is Panel)
            {
                Panel p = (Panel)node.Content;
                UIElementCollection x = (UIElementCollection)p.Children;
                replaceCT(x);
            }
            if (node.Content is ButtonBase)
            {
                (node.Content as ButtonBase).CommandTarget = null;
            }
        }

        private void replaceCT(UIElementCollection uIElement)
        {
            foreach (UIElement e in uIElement)
            {
                if (e is ButtonBase)
                {
                    (e as ButtonBase).CommandTarget = null;
                }
                else if (e is Panel)
                {
                    replaceCT((e as Panel).Children);
                }
            }
        }
        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///   System.IO.MemoryStream stream = new System.IO.MemoryStream();
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       Control.Save(stream as System.IO.Stream);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void Save(Stream stream)
        {
            SaveViewstateProperties();
            foreach (UIElement element in View.Page.Children.OfType<ICommon>())
            {
                if (element is Group && !(element is Layer))
                {
                    foreach (INodeGroup child in (element as Group).NodeChildren)
                    {
                        (element as Group).GroupChildrenRef.Add(child.ReferenceNo);
                    }
                }
                else if (element is Layer)
                {
                    (element as Layer).NodesRef = new CollectionExt();
                    (element as Layer).LinesRef = new CollectionExt();

                    foreach (Node n in (element as Layer).Nodes)
                    {
                        if (((element as Layer).Parent as DiagramPage).Children.Contains(n))
                        {
                            (element as Layer).NodesRef.Add(n.ReferenceNo);
                        }
                    }
                    foreach (LineConnector l in (element as Layer).Lines)
                    {
                        if (((element as Layer).Parent as DiagramPage).Children.Contains(l))
                        {
                            (element as Layer).LinesRef.Add(l.ReferenceNo);
                        }
                    }
                }
                else if (element is Node)
                {
                    if ((element as Node).Content.GetType() != typeof(string) && (element as Node).Content is UIElement && ((element as Node).Content as UIElement).IsHitTestVisible)
                    {
                        (element as Node).ContentHitTestVisible = true;
                    }
                    findCT(element as Node);
                }
                else if (element is LineConnector)
                {
                    (element as LineConnector).SetSerializationData();
                }
            }

            Style style = FindResource(typeof(Node)) as Style;

            string filecontents = GetIsolatedFileContents(style);
            if (!filecontents.Contains("PART_Rotator") && !filecontents.Contains("PART_RotateOrigin") && !filecontents.Contains("PART_DragProvider"))
            {
                (View.Page as DiagramPage).StyleRef = style;
            }

            Style style2 = FindResource(typeof(LineConnector)) as Style;

            string filecontents2 = GetIsolatedFileContents(style2);
            if (!filecontents2.Contains("PART_ConnectionPath") && !filecontents2.Contains("PART_SinkAnchorPath") && !filecontents2.Contains("PART_ConnectorLabelEditor"))
            {
                (View.Page as DiagramPage).LineStyleRef = style2;
            }

            (View.Page as DiagramPage).OrientationRef = Model.Orientation;
            (View.Page as DiagramPage).HorizontalSpacingref = Model.HorizontalSpacing;
            (View.Page as DiagramPage).VerticalSpacingref = Model.VerticalSpacing;
            (View.Page as DiagramPage).SubTreeSpacingref = Model.SpaceBetweenSubTrees;
            (View.Page as DiagramPage).LayoutTyperef = Model.LayoutType;
            IspageSaved = true;
#if SyncfusionFramework3_5
            Style tempStyle = (this.View.Page as DiagramPage).Style;
            (this.View.Page as DiagramPage).Style = null;
#endif
            System.Windows.Markup.XamlWriter.Save((this.View.Page as DiagramPage), stream);
#if SyncfusionFramework3_5
            (this.View.Page as DiagramPage).Style = tempStyle;
#endif
            IspageSaved = false;
            View.IsPageSaved = true;

        }

        internal static void LoadIntermediatePoints(LineConnector ln)
        {
            if (ln != null)
            {
                if (ln.VirtualConnectorPathGeometry == null)
                {
                    ln.IntermediatePoints = LineConnector.getLinePts(ln.ConnectorPathGeometry.Figures[0]);
                    ln.PxStartPointPosition = ln.IntermediatePoints[0];
                    ln.IntermediatePoints.RemoveAt(0);
                    ln.PxEndPointPosition = ln.IntermediatePoints[ln.IntermediatePoints.Count - 1];
                    ln.IntermediatePoints.RemoveAt(ln.IntermediatePoints.Count - 1);
                }
                else
                {
                    ln.IntermediatePoints = LineConnector.getLinePts(ln.VirtualConnectorPathGeometry.Figures[0]);
                    ln.PxStartPointPosition = ln.IntermediatePoints[0];
                    ln.IntermediatePoints.RemoveAt(0);
                    ln.PxEndPointPosition = ln.IntermediatePoints[ln.IntermediatePoints.Count - 1];
                    ln.IntermediatePoints.RemoveAt(ln.IntermediatePoints.Count - 1);
                }
            }
        }

        void PerformLoadOperations(DiagramPage page)
        {
            List<DiagramViewGrid> dvgs = page.Children.OfType<DiagramViewGrid>().ToList<DiagramViewGrid>();
            while (dvgs.Count > 1)
            {
                page.Children.Remove(dvgs[0]);
                dvgs.RemoveAt(0);
            }
            View.Page = page;
            Model.Orientation = (View.Page as DiagramPage).OrientationRef;
            Model.HorizontalSpacing = (View.Page as DiagramPage).HorizontalSpacingref;
            Model.VerticalSpacing = (View.Page as DiagramPage).VerticalSpacingref;
            Model.SpaceBetweenSubTrees = (View.Page as DiagramPage).SubTreeSpacingref;
            Model.LayoutType = (View.Page as DiagramPage).LayoutTyperef;
            for (int i = 0; i < page.Children.Count; i++)
            {
                UIElement element = page.Children[i];
                if (element is DiagramViewGrid)
                {
                    continue;
                }
                if (element is LineConnector)
                {
                    LineConnector ln = element as LineConnector;
                    ln.MeasurementUnit = (View.Page as DiagramPage).MeasurementUnits;
                    ln.RetrieveSerializationData();
                    ln.LabelAngle = 0;
                    //if (ln.ConnectorType == ConnectorType.Straight || ln.ConnectorType == ConnectorType.Orthogonal)
                    //{
                    //    try
                    //    {
                    //        DiagramControl.LoadIntermediatePoints(ln);
                    //    }
                    //    catch { }
                    //}
                }
                if (element is Layer)
                {
                    if (page != null)
                    {
                        (element as Node).Page = page;
                    }
                    Model.Layers.Add(element as Layer);
                    foreach (int no in (element as Layer).NodesRef)
                    {
                        foreach (UIElement child in page.Children.OfType<ICommon>())
                        {
                            if (child is DiagramViewGrid)
                            {
                                continue;
                            }
                            if ((child as INodeGroup).ReferenceNo == no)
                            {
                                (element as Layer).Nodes.Add(child as Node);
                                break;
                            }
                        }
                    }
                    foreach (int no in (element as Layer).LinesRef)
                    {
                        foreach (UIElement child in page.Children.OfType<ICommon>())
                        {
                            if (child is DiagramViewGrid)
                            {
                                continue;
                            }
                            if ((child as INodeGroup).ReferenceNo == no)
                            {
                                (element as Layer).Lines.Add(child as LineConnector);
                                break;
                            }
                        }
                    }
                }
                if ((element is Group) && !(element is Layer))
                {
                    foreach (int no in (element as Group).GroupChildrenRef)
                    {
                        foreach (UIElement child in page.Children.OfType<ICommon>())
                        {
                            if (child is DiagramViewGrid)
                            {
                                continue;
                            }
                            if ((child as INodeGroup).IsGrouped && (child as INodeGroup).ReferenceNo == no)
                            {
                                if (!(element as Group).NodeChildren.Contains(child as INodeGroup))
                                {
                                    (element as Group).AddChild(child as INodeGroup);
                                }

                                if (!(child as INodeGroup).Groups.Contains(element as Group))
                                {
                                    (child as INodeGroup).Groups.Add(element as Group);
                                }

                                break;
                            }
                        }
                    }
                }
                if (element is IShape && !(element is Layer))
                {
                    if (!Model.Nodes.Contains(element))
                    {
                        if ((element as Node).ContentHitTestVisible && ((element as Node).Content is UIElement))
                        {
                            ((element as Node).Content as UIElement).IsHitTestVisible = true;
                        }

                        (element as Node).MeasurementUnits = page.MeasurementUnits;
                        (element as Node).PxWidth = (element as Node).Width;
                        (element as Node).PxHeight = (element as Node).Height;
                        if (page.StyleRef != null)
                        {
                            (element as Node).Style = page.StyleRef;
                        }
                        if (page != null)
                        {
                            (element as Node).Page = page;
                        }
                        Model.Nodes.Add(element);

                        foreach (ConnectionPort port in (element as Node).Ports)
                        {
                            if (port.CenterPortReferenceNo == 0)
                            {
                                port.Name = "PART_Sync_CenterPort";
                            }

                            port.Node = element as Node;
                        }
                    }
                }

                else
                    if (!Model.Connections.Contains(element))
                    {
                        foreach (UIElement node in page.Children.OfType<ICommon>())
                        {
                            if (node is DiagramViewGrid)
                            {
                                continue;
                            }
                            if (node is Node && !(element is Layer))
                            {
                                if (page.LineStyleRef != null)
                                {
                                    (element as LineConnector).Style = page.LineStyleRef;
                                }

                                if ((element as LineConnector).HeadNodeReferenceNo == (node as Node).ReferenceNo)
                                {
                                    (element as LineConnector).HeadNode = node as Node;
                                }
                                else if ((element as LineConnector).TailNodeReferenceNo == (node as Node).ReferenceNo)
                                {
                                    (element as LineConnector).TailNode = node as Node;
                                }

                                foreach (ConnectionPort port in (node as Node).Ports)
                                {
                                    if (((element as LineConnector).HeadPortReferenceNo == port.PortReferenceNo) && ((element as LineConnector).HeadNode == node as Node))
                                    {
                                        (element as LineConnector).ConnectionHeadPort = port;
                                    }
                                    else
                                        if (((element as LineConnector).TailPortReferenceNo == port.PortReferenceNo) && ((element as LineConnector).TailNode == node as Node))
                                        {
                                            (element as LineConnector).ConnectionTailPort = port;
                                        }
                                }
                            }
                        }
                        if (!(element is Layer))
                            Model.Connections.Add(element);
                    }
            }

            if (View.ViewGrid != null)
            {
                View.ViewGrid.RenderTransform = new TranslateTransform(0, 0);
            }

            View.ViewGridOrigin = new Point(0, 0);
            CollectionExt.Cleared = false;

            foreach (Node node in Model.Nodes)
            {
                (View.Page as DiagramPage).AllChildren.Add(node);
                node.IsInternallyLoaded = true;
            }
            this.IsLoadingFromFile = false;
            foreach (LineConnector lc in Model.Connections)
            {
                lc.IsInternallyLoaded = true;
                (View.Page as DiagramPage).AllChildren.Add(lc);
                lc.InvalidateConnectorPathGeometry();
                lc.Loaded += new RoutedEventHandler(Line_Loaded);
            }
            if (View.ScrollGrid != null)
            {
                View.VerifyVirtualization();
            }
            View.IsPageSaved = true;
        }
        /// <summary>
        /// Loads the page. Opens a Load XAML dialog box to select the XAML file to be loaded.
        /// </summary>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       Control.Load();
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void Load()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load XAML";
            dialog.Filter = "XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                Load(dialog.OpenFile());
            }
        }

        /// <summary>
        /// Loads the page. Takes the filename as a parameter.
        /// </summary>
        /// <param name="filename">Name of the XAML file to be loaded.</param>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       Control.Save("Hello.xaml");
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void Load(string filename)
        {
            this.IsLoadingFromFile = true;
            View.UndoStack.Clear();
            View.RedoStack.Clear();
            DiagramControl.IsPageLoaded = true;
            View.Page.Children.Clear();
            //// string str;
            Model.Nodes.Clear();
            Model.Connections.Clear();
            Model.Nodes.Clear();
            Model.Layers.Clear();

            XmlReader xmlreader = XmlReader.Create(filename);
            StreamReader reader = new StreamReader(filename);
            string str = reader.ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlreader);
            XmlNodeList list = doc.GetElementsByTagName("Node");
            foreach (XmlNode node in list)
            {
                if (node.Attributes["Position"]!=null)
                {
                    node.Attributes.RemoveNamedItem("Position");
                }
            }
            MemoryStream outStream = new MemoryStream();
            doc.Save(outStream);
            outStream.Position = 0;
            DiagramPage page;
            if (str.Contains("http://schemas.syncfusion.com/wpf"))
            {
                //page = (DiagramPage)XamlReader.Load(xmlreader);
                page = (DiagramPage)XamlReader.Load(outStream);
            }
            else
            {
                str = str.Replace("clr-namespace:Syncfusion.Windows.Diagram;assembly=Syncfusion.Diagram.Silverlight", "http://schemas.syncfusion.com/wpf");
                str = str.Replace("http://schemas.microsoft.com/client/2007", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                str = LoadSLXaml(str);
                str = str.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

                byte[] byteArray = Encoding.ASCII.GetBytes(str);
                MemoryStream stream = new MemoryStream(byteArray);
                page = (DiagramPage)XamlReader.Load(stream);
            }
            
            PerformLoadOperations(page);           
        }

        private string LoadSLXaml(String str)
        {
            XDocument document = XDocument.Load(new StringReader(str.ToString()));
            Assembly ass = Assembly.Load(Assembly.GetExecutingAssembly().FullName);
            int d1 = document.Descendants().Count();
            List<XElement> ele = new List<XElement>();
            foreach (XElement element in document.Descendants())
            {
                if (element.Name.LocalName.Contains("NodePathFill") || element.Name.LocalName.Contains("PathStyle") || element.Name.LocalName.Contains("NodePathStroke"))
                {
                    ele.Add(element);

                }

                List<XAttribute> attr = new List<XAttribute>();
                foreach (XAttribute xattr in element.Attributes())
                {
                    if (xattr.Name.LocalName == "TextWidth" || xattr.Name.LocalName == "LabelAngle" || xattr.Name.LocalName == "LabelHeight" || xattr.Name.LocalName == "CenterPosition" || xattr.Name.LocalName == "TabNavigation" || xattr.Name.LocalName == "UseLayoutRounding" || xattr.Name.LocalName == "PortReferenceCount" || xattr.Name.LocalName == "NodePathStrokeThickness")
                    {
                        attr.Add(xattr);

                    }

                }
                foreach (XAttribute xa in attr)
                {
                    if (xa != null)
                        element.Attributes(xa.Name).Remove();
                }

            }

            IEnumerable<XElement> source = document.Descendants().Where<XElement>(delegate(XElement c)
            {
                if (ele.Contains(c))
                {
                    return true;
                }
                return false;
            });
            while (source.Any<XElement>())
            {
                Extensions.Remove<XElement>(source);
            }
            StringWriter strwri = new StringWriter();
            document.Save(strwri);
            str = strwri.ToString();
            return str;
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    System.IO.MemoryStream stream = new System.IO.MemoryStream();
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       Control.Save("Hello.xaml");
        ///       stream.Position = 0;
        ///        Control.Load(stream as System.IO.Stream);
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        public void Load(Stream stream)
        {
            if (stream as FileStream != null)
            {
                string s = (stream as FileStream).Name;
                try
                {
                    if (s.Contains(".xaml"))
                    {
                        LoadXaml(stream);            
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception XamlException)
                {
                    throw XamlException;
                }
            }
            else
            {
                LoadXaml(stream);
            }
        }

        internal void LoadXaml(Stream stream)
        {
            DiagramControl.IsPageLoaded = true;
            View.Page.Children.Clear();
            Model.Nodes.Clear();
            Model.Nodes.Clear();
            Model.Connections.Clear();
            XmlDocument dco = new XmlDocument();
            dco.Load(stream);
            XmlNodeList list = dco.GetElementsByTagName("Node");
            foreach (XmlNode node in list)
            {
                node.Attributes.RemoveNamedItem("Position");
                if (node.Attributes["PortVisibility"] != null)
                {
                    var a = node.Attributes["PortVisibility"].Value;
                    if (a == "Collapsed")
                    {
                        node.Attributes["PortVisibility"].Value = "MouseOverNode";
                    }
                    else if (a == "Visible")
                    {
                        node.Attributes["PortVisibility"].Value = "AlwaysVisible";
                    }
                    else if (a == "Hidden")
                    {
                        node.Attributes["PortVisibility"].Value = "AlwaysHidden";
                    }
                }
            }
            MemoryStream outputstream = new MemoryStream();
            dco.Save(outputstream);
            outputstream.Position = 0;
            DiagramPage page = (DiagramPage)XamlReader.Load(outputstream);
            PerformLoadOperations(page);
            View.IsPageSaved = true;
        }
        void Line_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as LineConnector).Loaded -= new RoutedEventHandler(Line_Loaded);
            View.IsPageSaved = true;
        }

        internal ResourceWrapper m_ResourceWrapper;

        public string LocalizationPath
        {
            get;
            set;
        }

        /// <summary>
        /// Overrides the OnApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (View != null)
            {
                View.ClearUndoRedoStack();
            }
            m_ResourceWrapper = new ResourceWrapper(LocalizationPath);


            CollectionExt.Cleared = false;
            if (View != null)
            {
                View.Page.LayoutUpdated += new EventHandler(DiagramView_LayoutUpdated);
            }

            if (Model != null)
            {
                if (Model.Nodes != null)
                {
                    DrawNodes();
                   // Model.Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(InternalNodes_CollectionChanged);
                }

                if (Model.Connections != null)
                {
                    DrawConnectios();
                  //  Model.Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Connections_CollectionChanged);
                }
            }

            if (!exe)
            {
                if (Model != null && View != null)
                {
                    if (View.PxBounds.Equals(new Thickness(0, 0, 0, 0)) && (View.Scrollviewer == null || View.Scrollviewer.ActualWidth == 0.0))
                    {
                        m_delayLayout = true;
                    }
                    m_delayLayout = true;
                }
            }

            if (Model != null)
            {
                foreach (Node node in this.Model.Nodes)
                {
                    SetScope(node);
                    foreach (ConnectionPort port in node.Ports)
                    {
                        SetScope(port);
                    }
                }

                foreach (LineConnector line in this.Model.Connections)
                {
                    SetScope(line);
                }
            }
        }

        private void ConstructPalette()
        {
            this.SymbolPalette = new SymbolPalette();
            this.SymbolPalette.Loaded += new RoutedEventHandler(SymbolPalette_Loaded);
        }

        void SymbolPalette_Loaded(object sender, RoutedEventArgs e)
        {
            LocalizePalette();
        }

        private void LocalizePalette()
        {
            //SymbolPaletteGroup
            if (this.SymbolPalette.SPGConnectors.ReadLocalValue(SymbolPaletteGroup.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGConnectors.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Connectors;
            }
            if (this.SymbolPalette.SPGElectrical.ReadLocalValue(SymbolPaletteGroup.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGElectrical.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_ElectricalShapes;
            }
            if (this.SymbolPalette.SPGCustom.ReadLocalValue(SymbolPaletteGroup.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGCustom.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_CustomShapes;
            }
            if (this.SymbolPalette.SPGFlowchart.ReadLocalValue(SymbolPaletteGroup.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGFlowchart.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Flowchart;
            }
            if (this.SymbolPalette.SPGShapes.ReadLocalValue(SymbolPaletteGroup.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPGShapes.HeaderName = m_ResourceWrapper.SymbolPaletteGroup_Shapes;
            }

            //SymbolPaletteFilter
            if (this.SymbolPalette.SPFCustom.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFCustom.Label = m_ResourceWrapper.SymbolPaletteFilter_CustomShapes;
            }
            if (this.SymbolPalette.SPFConnectors.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFConnectors.Label = m_ResourceWrapper.SymbolPaletteFilter_Connectors;
            }
            if (this.SymbolPalette.SPFAll.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFAll.Label = m_ResourceWrapper.SymbolPaletteFilter_All;
            }
            if (this.SymbolPalette.SPFElectrical.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFElectrical.Label = m_ResourceWrapper.SymbolPaletteFilter_ElectricalShapes;
            }
            if (this.SymbolPalette.SPFFlowchart.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFFlowchart.Label = m_ResourceWrapper.SymbolPaletteFilter_Flowchart;
            }
            if (this.SymbolPalette.SPFShapes.ReadLocalValue(SymbolPaletteFilter.LabelProperty) == DependencyProperty.UnsetValue)
            {
                this.SymbolPalette.SPFShapes.Label = m_ResourceWrapper.SymbolPaletteFilter_Shapes;
            }
        }

        /// <summary>
        /// Calls InternalNodes_CollectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public void InternalNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (View != null)
            {
                if (!(View.undo || View.redo))
                {
                    View.RedoStack.Clear();
                }

                if (CollectionExt.Cleared && !DiagramControl.IsPageLoaded)
                {
                    if (Model.Connections.Count != 0)
                    {
                        foreach (LineConnector line in Model.Connections)
                        {
                            if (line.HeadNode != null || line.TailNode != null)
                            {
                                ConnectionDeleteRoutedEventArgs newEventArgs3 = new ConnectionDeleteRoutedEventArgs();
                                newEventArgs3.RoutedEvent = DiagramView.ConnectorDeletingEvent;
                                View.RaiseEvent(newEventArgs3);
                            }

                            break;
                        }
                    }

                    View.SelectionList.Clear();
                    CollectionExt connections = new CollectionExt();
                    foreach (ICommon obj in View.Page.Children.OfType<ICommon>())
                    {
                        connections.Add(obj);
                    }

                    foreach (ICommon shape in connections)
                    {
                        if (shape is LineConnector)
                        {
                            if ((shape as LineConnector).HeadNode != null || (shape as LineConnector).TailNode != null)
                            {
                                nodecleared = true;
                                Model.Connections.Remove((shape as LineConnector));
                                nodecleared = false;
                                connremoved = true;
                            }
                        }

                        if (shape is Group && (shape as Group).AllowDelete)
                        {
                            for (int k = 0; k <= (shape as Group).NodeChildren.Count - 1; k++)
                            {
                                if ((shape as Group).NodeChildren[k] is Node)
                                {
                                    if (((shape as Group).NodeChildren[k] as Node).AllowDelete)
                                    {
                                        NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs();
                                        newEventArgs.RoutedEvent = DiagramView.NodeDeletingEvent;
                                        View.RaiseEvent(newEventArgs);

                                    }

                                    isnodepresent = false;
                                    View.Page.Children.Remove(((shape as Group).NodeChildren[k] as Node));
                                    noderemoved = true;
                                    if (!((shape as Group).NodeChildren[k] as Node).AllowDelete)
                                    {
                                        isnodepresent = true;
                                        noderemoved = false;
                                        View.Page.Children.Add(((shape as Group).NodeChildren[k]) as Node);
                                        if (!Model.Nodes.Contains(((shape as Group).NodeChildren[k]) as Node))
                                        {
                                            Model.Nodes.Add(((shape as Group).NodeChildren[k]) as Node);
                                        }

                                    }
                                    ((shape as Group).NodeChildren[k] as Node).IsGrouped = false;
                                }
                            }
                            View.Page.Children.Remove(shape as Group);
                        }
                        else if (shape is Node && !(shape as Node).IsGrouped && !(shape is Group))
                        {
                            if (isnodepresent && (shape as Node).AllowDelete)
                            {
                                NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs();
                                newEventArgs.RoutedEvent = DiagramView.NodeDeletingEvent;
                                View.RaiseEvent(newEventArgs);
                            }

                            isnodepresent = false;
                            View.Page.Children.Remove(shape as Node);
                            //if (!Model.Nodes.Contains(shape as Node))
                            {
                                if ((View.Page as DiagramPage).AllChildren.Contains(shape as Node))
                                {
                                    (View.Page as DiagramPage).AllChildren.Remove(shape as Node);
                                }
                            }
                            noderemoved = true;
                            if (!(shape as Node).AllowDelete)
                            {
                                noderemoved = false;
                                isnodepresent = true;

                                if (!Model.Nodes.Contains(shape as Node))
                                {
                                   Model.Nodes.Add(shape as Node);
                                }
                            }
                        }
                    }

                    CollectionExt.Cleared = false;

                    View.ViewGridOrigin = new Point(0, 0);
                    if (noderemoved)
                    {
                        NodeDeleteRoutedEventArgs newEventArgs2 = new NodeDeleteRoutedEventArgs();
                        newEventArgs2.RoutedEvent = DiagramView.NodeDeletedEvent;
                        View.RaiseEvent(newEventArgs2);
                    }

                    if (connremoved)
                    {
                        ConnectionDeleteRoutedEventArgs newEventArgs3 = new ConnectionDeleteRoutedEventArgs();
                        newEventArgs3.RoutedEvent = DiagramView.ConnectorDeletedEvent;
                        View.RaiseEvent(newEventArgs3);
                    }

                    connremoved = false;
                    noderemoved = false;
                }

                try
                {
                    if (e.NewItems != null)
                    {
                        foreach (IShape inode in e.NewItems)
                        {
                            if (!View.Undone && View.UndoRedoEnabled && !View.IsLayout && !(inode is Layer))
                            {
                                View.UndoStack.Push(inode);
                                View.UndoStack.Push("Added");
                            }

                            (inode as Node).Page = View.Page;
                            if (!View.Page.Children.Contains(inode as Node))
                            {
                                
                                (View.Page as DiagramPage).AllChildren.Add(inode as Node);

                                if (View.EnableVirtualization)
                                {
                                    if (View.ScrollGrid != null)
                                    {
                                        View.ScrollGrid.VirtualzingNode(inode as Node);
                                    }
                                }
                                else
                                {
                                    (View.Page as DiagramPage).AddingChildren(inode as Node);
                                }
                                if ((inode as Node).ReadLocalValue(Panel.ZIndexProperty) == DependencyProperty.UnsetValue)
                                    if ((inode as Node).ReadLocalValue(Panel.ZIndexProperty) == DependencyProperty.UnsetValue)
                                        if ((inode as Node).m_NodeDrawing == true)
                                        {
                                            Panel.SetZIndex(inode as Node, View.Page.Children.Count - 1);
                                        }
                                        else
                                        {
                                            Panel.SetZIndex(inode as Node, View.Page.Children.Count);
                                        }
                                SetScope(inode as Node);
                            }
                        }
                    }
                }
                catch
                {
                }

                if (e.OldItems != null)
                {
                    foreach (IShape inode in e.OldItems)
                    {

                        if (!View.Isnodedeleted && (inode as Node).AllowDelete)
                        {
                            NodeDeleteRoutedEventArgs newEventArgs = new NodeDeleteRoutedEventArgs(inode as Node);
                            newEventArgs.RoutedEvent = DiagramView.NodeDeletingEvent;
                            View.RaiseEvent(newEventArgs);
                        }

                        View.Isnodedeleted = true;

                        if ((inode as Node).Content == null)
                        {
                            View.DupDeleted = true;
                        }

                        if ((inode as Node).Content != null)
                        {
                            if ((inode as Node).Content.ToString() != "VirtualNode")
                            {
                                View.DupDeleted = true;
                            }
                        }
                        CollectionExt head = new CollectionExt();
                        CollectionExt tail = new CollectionExt();
                        List<IEdge> edges = (inode as Node).Edges.OfType<IEdge>().ToList();
                        foreach (IEdge line in edges)
                        {
                            if ((inode as Node).DeletingMode == DeletingMode.DeleteDependentEdges)
                            {
                                ConnectionDeleteRoutedEventArgs newEventArgs = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                                newEventArgs.RoutedEvent = DiagramView.ConnectorDeletingEvent;
                                this.View.RaiseEvent(newEventArgs);
                                if (newEventArgs.Cancel == false)
                                {
                                    View.Page.Children.Remove(line as LineConnector);
                                    Model.Connections.Remove(line as LineConnector);
                                    ConnectionDeleteRoutedEventArgs newEventArgs1 = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                                    newEventArgs1.RoutedEvent = DiagramView.ConnectorDeletedEvent;
                                    this.View.RaiseEvent(newEventArgs1);
                                }
                                else
                                {
                                    if ((inode as Node).InEdges.Contains(line))
                                    {
                                        tail.Add(line as LineConnector);
                                    }
                                    if ((inode as Node).OutEdges.Contains(line))
                                    {
                                        head.Add(line as LineConnector);
                                    }
                                }
                            }
                        }

                        foreach (LineConnector l in head)
                        {
                            (l as LineConnector).PxStartPointPosition = (l as LineConnector).m_TempStart;
                            (l as LineConnector).HeadNode = null;
                        }

                        foreach (LineConnector l in tail)
                        {
                            (l as LineConnector).PxEndPointPosition = (l as LineConnector).m_TempEnd;
                            (l as LineConnector).TailNode = null;
                        }

                        for (int i = 0; i < View.InternalEdges.Count; i++)
                        {
                            LineConnector lineconn = View.InternalEdges[i] as LineConnector;
                            if (lineconn != null && !this.Model.Connections.Contains(lineconn))
                            {
                                View.InternalEdges.RemoveAt(i);
                                i--;
                            }
                        }

                        if (!View.Redone)
                        {
                            View.DeleteCount++;
                        }

                        if (!View.Undone && View.UndoRedoEnabled && !View.IsLayout && !(inode is Layer))
                        {
                            View.UndoStack.Push(inode);
                            View.UndoStack.Push(Panel.GetZIndex(inode as Control));
                            View.UndoStack.Push("Deleted");
                        }
                        (View.Page as DiagramPage).AllChildren.Remove(inode as Node);
                        (View.Page as DiagramPage).RemovingChildren(inode as Node);

                        if (Model.Nodes.Contains(inode as Node))
                        {
                            Model.Nodes.Remove(inode as Node);
                        }
                        if ((inode as Node).Content != null && (inode as Node).Content.ToString() != "VirtualNode" && (inode as Node).AllowDelete)
                        {
                            NodeDeleteRoutedEventArgs newEventArgs2 = new NodeDeleteRoutedEventArgs(inode as Node);
                            newEventArgs2.RoutedEvent = DiagramView.NodeDeletedEvent;
                            View.RaiseEvent(newEventArgs2);
                        }                        
                        View.Isnodedeleted = false;
                    }
                }
            }
            View.IsPageSaved = false;
        }

        /// <summary>
        /// Calls DiagramView_LayoutUpdated method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void DiagramView_LayoutUpdated(object sender, EventArgs e)
        {
            if (Model != null)
            {
                if (Model.Connections != null)
                {
                    DrawConnectios();
                }
            }

            View.Page.LayoutUpdated -= new EventHandler(DiagramView_LayoutUpdated);
        }

        /// <summary>
        /// Calls Connections_CollectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="sender"> object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Connections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(View.undo || View.redo))
            {
                View.RedoStack.Clear();
            }

            bool islinepresent = false;
            if (CollectionExt.Cleared && !DiagramControl.IsPageLoaded && !nodecleared)
            {
                var conn = View.Page.Children.OfType<LineConnector>();
                if (conn.Count() != 0)
                {
                    islinepresent = true;
                    ConnectionDeleteRoutedEventArgs newEventArgs = new ConnectionDeleteRoutedEventArgs();
                    newEventArgs.RoutedEvent = DiagramView.ConnectorDeletingEvent;
                    View.RaiseEvent(newEventArgs);
                }

                for (int i = 0; i < View.Page.Children.Count; i++)
                {
                    UIElement ele = View.Page.Children[i];
                    if (ele is ICommon)
                    {
                        View.Page.Children.Remove(ele);
                        i--;
                    }
                }
                View.SelectionList.Clear();
                foreach (Node n in Model.Nodes)
                {
                    View.Page.Children.Add(n);
                }

                if (islinepresent)
                {
                    ConnectionDeleteRoutedEventArgs newEventArgs2 = new ConnectionDeleteRoutedEventArgs();
                    newEventArgs2.RoutedEvent = DiagramView.ConnectorDeletedEvent;
                    View.RaiseEvent(newEventArgs2);
                }

                if (Model.LayoutType == LayoutType.DirectedTreeLayout || Model.LayoutType == LayoutType.HierarchicalTreeLayout || Model.LayoutType == LayoutType.BowtieLayout)
                {
                    foreach (Node node in Model.Nodes)
                    {
                        //(node as IShape).Edges.Clear();
                        node.PxOffsetX = 0;
                        node.PxOffsetY = 0;
                    }
                }

                CollectionExt.Cleared = false;
            }

            try
            {
                if (e.NewItems != null)
                {
                    foreach (IEdge iconn in e.NewItems)
                    {
                        if (!View.Undone && View.UndoRedoEnabled && !View.IsLayout)
                        {
                            View.UndoStack.Push(iconn as LineConnector);
                            View.UndoStack.Push("Added");
                        }
                        if (!View.Page.Children.Contains(iconn as Control))
                        {
                            (View.Page as DiagramPage).AllChildren.Add(iconn as Control);
                            if (View.EnableVirtualization)
                            {
                                if (View.ScrollGrid != null)
                                {
                                    View.ScrollGrid.VirtualizingLine(iconn as LineConnector);
                                }
                            }
                            else
                            {
                                (View.Page as DiagramPage).AddingChildren(iconn as LineConnector);
                            }
                            if ((iconn as Control).ReadLocalValue(Panel.ZIndexProperty) == DependencyProperty.UnsetValue)
                            Panel.SetZIndex(iconn as Control, View.Page.Children.Count);
                            SetScope(iconn as Control);
                        }
                    }
                }
            }
            catch
            {
            }

            if (e.OldItems != null)
            {
                foreach (IEdge line in e.OldItems)
                {
                    if (!View.Islinedeleted && !nodecleared)
                    {
                        if (((line.HeadNode as Node) != null && (line.HeadNode as Node).Content != null && (line.HeadNode as Node).Content.ToString() != "VirtualNode") || (line.HeadNode as Node) == null || (line.TailNode as Node) == null)
                        {
                            ConnectionDeleteRoutedEventArgs newEventArgs = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                            newEventArgs.RoutedEvent = DiagramView.ConnectorDeletingEvent;
                            View.RaiseEvent(newEventArgs);
                        }
                    }

                    if (line.TailNode != null)
                    {
                        if (View.Isnodedeleted)
                        {
                            if (!View.InternalEdges.Contains(line as LineConnector))
                            {
                                View.InternalEdges.Add(line as LineConnector);
                            }
                        }
                    }

                    if (line.HeadNode != null)
                    {
                        if (View.Isnodedeleted)
                        {
                            if (!View.InternalEdges.Contains(line as LineConnector))
                            {
                                View.InternalEdges.Add(line as LineConnector);
                            }
                        }
                    }

                    if (!View.Undone && View.UndoRedoEnabled && !View.IsLayout)
                    {
                        View.UndoStack.Push(line as LineConnector);
                        View.UndoStack.Push(Panel.GetZIndex(line as Control));
                        View.UndoStack.Push("Deleted");
                    }

                    if (!View.Redone)
                    {
                        View.DeleteCount++;
                    }
                    (View.Page as DiagramPage).AllChildren.Remove(line as LineConnector);
                    (View.Page as DiagramPage).RemovingChildren(line as LineConnector);

                    if (!nodecleared)
                    {
                        if (((line.HeadNode as Node) != null && (line.HeadNode as Node).Content != null && (line.HeadNode as Node).Content.ToString() != "VirtualNode") || (line.HeadNode as Node) == null || (line.TailNode as Node) == null)
                        {
                            ConnectionDeleteRoutedEventArgs newEventArgs2 = new ConnectionDeleteRoutedEventArgs(line as LineConnector);
                            newEventArgs2.RoutedEvent = DiagramView.ConnectorDeletedEvent;
                            View.RaiseEvent(newEventArgs2);
                        }
                    }
                }
            }
            View.IsPageSaved = false;
            DiagramCommandManager.UpdateZOrder(View);
        }

        /// <summary>
        /// Draws the nodes on the DiagramPage.
        /// </summary>
        private void DrawNodes()
        {
            if (View != null && View.Page != null)
            {
                foreach (Node item in Model.Nodes)
                {
                    (item as Node).Page = View.Page;
                }

                var temp = Model.Nodes.OfType<UIElement>();
                var intersect = (View.Page as DiagramPage).AllChildren.Intersect(Model.Nodes.OfType<UIElement>());
                (View.Page as DiagramPage).AllChildren.AddRange(temp.Except(intersect));
            }
        }        

        /// <summary>
        /// Draws the connections on the DiagramPage.
        /// </summary>
        private void DrawConnectios()
        {
            if (!CollectionExt.Cleared)
            {
                foreach (IEdge connection in Model.Connections)
                {
                    if (connection.HeadNode != null && connection.TailNode != null)
                    {
                        (connection as LineConnector).HeadNodeReferenceNo = ((connection as LineConnector).HeadNode as Node).ReferenceNo;
                        (connection as LineConnector).TailNodeReferenceNo = ((connection as LineConnector).TailNode as Node).ReferenceNo;
                    }
                    if ((connection as ConnectorBase).ReadLocalValue(Panel.ZIndexProperty) == DependencyProperty.UnsetValue)
                    Panel.SetZIndex(connection as ConnectorBase, View.Page.Children.Count);
                }
                var temp = Model.Connections.OfType<UIElement>();
                var intersect = (View.Page as DiagramPage).AllChildren.Intersect(Model.Connections.OfType<UIElement>());
                (View.Page as DiagramPage).AllChildren.AddRange(temp.Except(intersect));
            }
        }

        /// <summary>
        /// Searches for the specified node based on its name and returns the one with the same name.
        /// </summary>
        /// <param name="name">Unique identifier .</param>
        /// <returns>Matched Node.</returns>
        public Node FindByName(string name)
        {
            foreach (Node node in Model.Nodes)
            {
                if (node.Name.Equals(name))
                {
                    return node;
                }
            }

            return null;
        }

        #endregion

        #region Class Properties

        /// <summary>
        /// Gets or sets the Model property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DiagramModel"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///              lt;syncfusion:DiagramControl.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Background="LightGray"  
        ///                                        Bounds="0,0,12,12"  
        ///                                        ShowHorizontalGridLine="False" 
        ///                                        ShowVerticalGridLine="False"
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramControl"/> in C#.
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="DiagramModel"/>
        //#if SyncfusionFramework4_0
        [Browsable(false)]
        //#endif
        public DiagramModel Model
        {
            get
            {
                return (DiagramModel)GetValue(ModelProperty);
            }

            set
            {
                SetValue(ModelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>Type: <see cref="DiagramView"/></value>
        /// <example>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in XAML.
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow"
        /// xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        /// Icon="Images/App.ico" &gt;
        /// &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl"
        /// IsSymbolPaletteEnabled="True"
        /// Background="WhiteSmoke"&gt;
        /// lt;syncfusion:DiagramControl.Model&gt;
        /// &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        /// &lt;/syncfusion:DiagramModel&gt;
        /// &lt;/syncfusion:DiagramControl.Model&gt;
        /// &lt;syncfusion:DiagramControl.View&gt;
        /// &lt;syncfusion:DiagramView  IsPageEditable="True"
        /// Background="LightGray"
        /// Bounds="0,0,12,12"
        /// ShowHorizontalGridLine="False"
        /// ShowVerticalGridLine="False"
        /// Name="diagramView"  &gt;
        /// &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        /// &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        /// &lt;syncfusion:DiagramView.VerticalRuler&gt;
        /// &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        /// &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        /// &lt;/syncfusion:DiagramView&gt;
        /// &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <para/>The following example shows how to create a <see cref="DiagramView"/> in C#.
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// HorizontalRuler hruler = new HorizontalRuler();
        /// View.HorizontalRuler = hruler;
        /// View.ShowHorizontalGridLine = false;
        /// View.ShowVerticalGridLine = false;
        /// VerticalRuler vruler = new VerticalRuler();
        /// View.VerticalRuler = vruler;
        /// View.Bounds = new Thickness (0, 0, 1000, 1000);
        /// View.IsPageEditable = true;
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="DiagramView"/>
        [Browsable(false)]
        public DiagramView View
        {
            get
            {
                return (DiagramView)GetValue(ViewProperty);
            }

            set
            {
                SetValue(ViewProperty, value);
            }
        }

        /// <summary>
        /// Gets the SymbolPalette property.
        /// </summary>
        /// <value>
        /// Type: <see cref="SymbolPalette"/>
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///              lt;syncfusion:DiagramControl.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       SymbolPaletteFilter sfilter = new SymbolPaletteFilter();
        ///       sfilter.Label = "Custom";
        ///        Control.SymbolPalette.SymbolFilters.Add(sfilter);
        ///       SymbolPaletteGroup s = new SymbolPaletteGroup();
        ///       s.Label = "Custom";
        ///       SymbolPalette.SetFilterIndexes(s, new Int32Collection(new int[] { 0, 5}));
        ///       Control.SymbolPalette.SymbolGroups.Add(s);
        ///       SymbolPaletteItem ss = new SymbolPaletteItem();
        ///       Path path = this.Resources["CustomShape"] as Path;
        ///       ss.Content = path;
        ///       s.Items.Add(ss);
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="SymbolPalette"/>
        /// <seealso cref="SymbolPaletteGroup"/>
        /// <seealso cref="SymbolPaletteItem"/>
        /// <seealso cref="SymbolPaletteFilter"/>
        public SymbolPalette SymbolPalette
        {
            get
            {
                return (SymbolPalette)GetValue(SymbolPaletteProperty);
            }

            internal set
            {
                SetValue(SymbolPaletteProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is symbol palette enabled.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True, if SymbolPalette is enabled, false otherwise.
        /// </value>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///              lt;syncfusion:DiagramControl.Model&gt;
        ///                  &lt;syncfusion:DiagramModel LayoutType="None"  x:Name="diagramModel" &gt;
        ///                  &lt;/syncfusion:DiagramModel&gt;
        ///             &lt;/syncfusion:DiagramControl.Model&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///             &lt;syncfusion:HorizontalRuler Name="horizontalRuler" /&gt;
        ///          &lt;/syncfusion:DiagramView.HorizontalRuler&gt;
        ///           &lt;syncfusion:DiagramView.VerticalRuler&gt;
        ///               &lt;syncfusion:VerticalRuler    Name="verticalRuler" /&gt;
        ///           &lt;/syncfusion:DiagramView.VerticalRuler &gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Text;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// using System.Windows.Documents;
        /// using System.Windows.Input;
        /// using System.Windows.Media;
        /// using System.Windows.Media.Imaging;
        /// using System.Windows.Navigation;
        /// using System.Windows.Shapes;
        /// using System.ComponentModel;
        /// using Syncfusion.Core;
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///       InitializeComponent ();
        ///       Control = new DiagramControl ();
        ///       Model = new DiagramModel ();
        ///       View = new DiagramView ();
        ///       Control.View = View;
        ///       Control.Model = Model;
        ///       SymbolPaletteFilter sfilter = new SymbolPaletteFilter();
        ///       sfilter.Label = "Custom";
        ///        Control.SymbolPalette.SymbolFilters.Add(sfilter);
        ///       SymbolPaletteGroup s = new SymbolPaletteGroup();
        ///       s.Label = "Custom";
        ///       SymbolPalette.SetFilterIndexes(s, new Int32Collection(new int[] { 0, 5}));
        ///       Control.SymbolPalette.SymbolGroups.Add(s);
        ///       SymbolPaletteItem ss = new SymbolPaletteItem();
        ///       Path path = this.Resources["CustomShape"] as Path;
        ///       ss.Content = path;
        ///       s.Items.Add(ss);
        ///       HorizontalRuler hruler = new HorizontalRuler();
        ///       View.HorizontalRuler = hruler;
        ///       View.ShowHorizontalGridLine = false;
        ///       View.ShowVerticalGridLine = false;
        ///       VerticalRuler vruler = new VerticalRuler();
        ///       View.VerticalRuler = vruler;
        ///       View.Bounds = new Thickness (0, 0, 1000, 1000);
        ///       View.IsPageEditable = true;
        ///    }
        ///    }
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="SymbolPalette"/>
        /// <seealso cref="SymbolPaletteGroup"/>
        /// <seealso cref="SymbolPaletteItem"/>
        /// <seealso cref="SymbolPaletteFilter"/>
        public bool IsSymbolPaletteEnabled
        {
            get
            {
                return (bool)GetValue(IsSymbolPaletteEnabledProperty);
            }

            set
            {
                SetValue(IsSymbolPaletteEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is page loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page loaded; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsPageLoaded
        {
            get { return m_ispageloaded; }
            set { m_ispageloaded = value; }
        }

        internal bool IsLoadingFromFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Diagram Control is unloaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Diagram Control is unloaded; otherwise, <c>false</c>.
        /// </value>
        internal bool IsUnloaded
        {
            get
            {
                return isunloaded;
            }

            set
            {
                isunloaded = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether page is saved.
        /// </summary>
        /// <value><c>true</c> if page is saved; otherwise, <c>false</c>.</value>
        internal bool IspageSaved
        {
            get
            {
                return ispagesaved;
            }

            set
            {
                ispagesaved = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Symbol Palette
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is symbol palette visibility changed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSymbolPaletteVisibilityChanged
        {
            get { return m_ispalettechanged; }
            set { m_ispalettechanged = value; }
        }
        #endregion

        #region Dependency Properties

        public static BowtieSubTreePlacement GetBowtieSubTreePlacement(DependencyObject obj)
        {
            return (BowtieSubTreePlacement)obj.GetValue(BowtieSubTreePlacementProperty);
        }

        public static void SetBowtieSubTreePlacement(DependencyObject obj, BowtieSubTreePlacement value)
        {
            obj.SetValue(BowtieSubTreePlacementProperty, value);
        }

        // Using a DependencyProperty as the backing store for BowtieSubTreePlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BowtieSubTreePlacementProperty =
            DependencyProperty.RegisterAttached("BowtieSubTreePlacement", typeof(BowtieSubTreePlacement), typeof(DiagramControl), new PropertyMetadata(BowtieSubTreePlacement.Right));

        /// <summary>
        /// Identifies the Model .  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(DiagramModel), typeof(DiagramControl), new PropertyMetadata(null, new PropertyChangedCallback(OnModelChanged)));

        /// <summary>
        /// Identifies the View.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(DiagramView), typeof(DiagramControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnViewChanged)));

        /// <summary>
        /// Identifies whether the SymbolPalette is enabled or not. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSymbolPaletteEnabledProperty = DependencyProperty.Register("IsSymbolPaletteEnabled", typeof(bool), typeof(DiagramControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnShowPalleteChanged)));

        /// <summary>
        /// Identifies  the SymbolPalette . This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteProperty = DependencyProperty.Register("SymbolPalette", typeof(SymbolPalette), typeof(DiagramControl), new UIPropertyMetadata(null));

        #endregion

        #region Events

        /// <summary>
        /// Calls OnModelChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl d_Control = d as DiagramControl;
            if (e.OldValue != e.NewValue)
            {
                if (e.OldValue != null && e.OldValue is DiagramModel)
                {
                    (e.OldValue as DiagramModel).Nodes.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.InternalNodes_CollectionChanged);
                    (e.OldValue as DiagramModel).Connections.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.Connections_CollectionChanged);
                }
                if (e.NewValue != null && e.NewValue is DiagramModel)
                {
                    (e.NewValue as DiagramModel).dc = d_Control;
                    //if ((e.NewValue as DiagramModel).dc.IsLoaded)
                    {
                        (e.NewValue as DiagramModel).Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.InternalNodes_CollectionChanged);
                        (e.NewValue as DiagramModel).Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.Connections_CollectionChanged);
                    }
                    if (DesignerProperties.GetIsInDesignMode(d_Control))
                    {
                        if (d_Control.Model != null)
                        {
                            //d_Control.SetScope(d_Control.Model);
                        }
                    }
                }
                if (e.OldValue != null)
                {
                    (e.OldValue as DiagramModel).Connections.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.Connections_CollectionChanged2);
                }
                if (e.NewValue != null)
                {
                    (e.NewValue as DiagramModel).Connections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(d_Control.Connections_CollectionChanged2);
                }
            }
        }

        private void Connections_CollectionChanged2(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LineConnector lc in e.NewItems.OfType<LineConnector>())
                {
                    lc.Add();
                    lc.setMode(Model);
                }
            }

            if (e.OldItems != null)
            {
                foreach (LineConnector lc in e.OldItems.OfType<LineConnector>())
                {
                    lc.DeleteConnector();
                    lc.setMode(null);
                }
            }
        }

        /// <summary>
        /// Calls OnViewChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DiagramControl d_Control = d as DiagramControl;
                d_Control.SetScope(d_Control);
                DiagramView view = d_Control.View;
                d_Control.SetScope(view);
                d_Control.SetScope(view.Page as DiagramPage);
                if (!DesignerProperties.GetIsInDesignMode(d_Control))
                {
                    if (d_Control.Model != null)
                    {
                        d_Control.SetScope(d_Control.Model);
                    }
                }
            }

            //if (e.OldValue != null)
            //{
            //    DiagramControl d_Control = d as DiagramControl;
            //    if (d_Control != null && d_Control.Model != null)
            //    {
            //        d_Control.Model.Nodes.Clear();
            //        d_Control.Model.Connections.Clear();
            //    }
            //}
        }

        /// <summary>
        /// Calls OnShowPalleteChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowPalleteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramControl d_control = d as DiagramControl;
            if (d_control.IsSymbolPaletteEnabled)
            {
                if (d_control.SymbolPalette == null)
                {
                    d_control.ConstructPalette();
                }
            }
            if (d_control.View != null)
            {
                d_control.IsSymbolPaletteVisibilityChanged = true;
            }
        }

        #endregion

        #region Class Override
        
        /// <summary>
        /// Provides class handling for the KeyUp routed event that occurs when the any key on the keyboard
        /// is released.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.Escape)
            {
                if (this.SymbolPalette != null)
                {
                    SymbolPaletteItem symbol = this.SymbolPalette.SelectedItem as SymbolPaletteItem;

                    this.SymbolPalette.SelectedItem = null;
                    DiagramPage dp = (View.Page as DiagramPage);
                    if (View.tempPolyLine != null)
                    {
                        dp.Children.Remove(View.tempPolyLine);
                        View.tempPolyLine = null;
                    }
                }
            }
        }

        double FindWidth()
        {
            double width = 0;
            if (Model != null)
            {
                var offxx = from Node nod in Model.Nodes select nod.OffsetX;
                double d = offxx.ToList<double>().Min();
                var offx = from Node nod in Model.Nodes select nod.OffsetX + nod._Width;
                width = offx.ToList<double>().Max();
                if (d < 0)
                    return width - d;
            }
            return width;
        }

        double FindHeight()
        {
            double height = 0;
            if (Model != null)
            {
                var offyy = from Node nod in Model.Nodes select nod.OffsetY;
                double d = offyy.ToList<double>().Min();
                var offy = from Node nod in Model.Nodes select nod.OffsetY + nod._Height;
                height = offy.ToList<double>().Max();
                if(d<0)
                return height - d;
            }
            return height;
        }

        /// <summary>
        /// Measures the size of the <see cref="DiagramControl"/> and returns the available size.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>Size of the diagram control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                {
                    availableSize.Width = 600;
                }

                if (double.IsPositiveInfinity(availableSize.Height))
                {
                    availableSize.Height = 600;
                }
            }

            //if (Model != null && (double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height)))
            //{
            //    if (this.Model.LayoutType == LayoutType.HierarchicalTreeLayout)
            //    {
            //        HierarchicalTreeLayout layout = new HierarchicalTreeLayout(this.Model, this.View);
            //        layout.RefreshLayout();
            //    }
            //    else if (this.Model.LayoutType == LayoutType.DirectedTreeLayout)
            //    {
            //        DirectedTreeLayout layout = new DirectedTreeLayout(this.Model, View);
            //        layout.RefreshLayout();
            //    }
            //    else if (this.Model.LayoutType == Diagram.LayoutType.RadialTreeLayout)
            //    {
            //        RadialTreeLayout layout = new RadialTreeLayout(this.Model, View);
            //        layout.RefreshLayout();
            //    }
            //    else if (this.Model.LayoutType == Diagram.LayoutType.TableLayout)
            //    {
            //        TableLayout layout = new TableLayout(this.Model, View);
            //        layout.RefreshLayout();
            //    }
            //    else if (this.Model.LayoutType == LayoutType.BowtieLayout)
            //    {
            //        BowtieLayout layout = new BowtieLayout(this.Model, View);
            //        layout.RefreshLayout();
            //    }
            //}
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                availableSize.Width = FindWidth();
            }

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                availableSize.Height = FindHeight();
            }

            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Calls property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the appropriate property changes.
        /// </summary>
        /// <param name="name">The property name</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}

