#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    #region Enum
    public enum SparkLineType
    {
        Line,
        Column,
        WinLoss
    }
    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(SparkLine), "ToolboxIcons.sparklinecontrol.png")]
    [Description("Displays a windows form SpatkLine chart")]
    
    public class SparkLine : Control,ISparkLine 
    {
      
       #region Member
        
       private System.ComponentModel.IContainer components = null;
     
       object m_Source = null;
       private double[] m_negativeItem;
       public SparkLineType m_sparkLineType=SparkLineType.Line;

       private BindingContext m_bindingContext;

       private double m_startPoint = 0;
       private double m_endPoint = 0;

       private double m_highPoint = 0;
       private double m_lowPoint = 0; 
              
      private LineRender  m_linerenderes=new LineRender();
      private WinLossRender m_winlossrenderer = new WinLossRender();
      private ColumnRender m_columnrenderes = new ColumnRender();
      private SparkLineSource m_sparklineSource = new SparkLineSource();
      
       private BrushInfo m_backInterior = new BrushInfo(Color.White);

       public List<object> List=new List<object>();
           
       private Line m_lineStyle = new Line();
       private Column  m_columnStyle = new Column();
       private Markers m_markers = new Markers();

#endregion
         
       #region Properties
       /// <summary>
       /// Gets or sets the data source of the sparkline control.
       /// </summary>
       public object Source
       {
           get { return m_Source; }
           set
           {
               if (value != null)
               {
                   m_Source = value;
                   if (m_bindingContext == null)
                   {
                       m_bindingContext = new BindingContext();
                   }
                   CurrencyManager manager = m_bindingContext[m_Source] as CurrencyManager;
                   manager.ListChanged += new ListChangedEventHandler(OnListChanged);
                   this.Refresh();
                 }      
           }
       }
       public int ControlWidth
       {
           get
           {
               return this.Width;
           }

       }
       public int ControlHeight
       {
           get
           {
               return this.Height;
           }
       }
      
     public double StartPoint
       {
           get
           {
               return m_startPoint;
           }
           set
           {
               if (m_startPoint != value)
               {
                   m_startPoint = value;

               }
           }
       }
     public double EndPoint
       {
           get
           {
               return m_endPoint;
           }
           set
           {
               if (m_endPoint != value)
               {
                   m_endPoint = value;

               }
           }
       }
     public double LowPoint
       {
           get
           {
               return m_lowPoint;
           }
           set
           {
               if (m_lowPoint != value)
               {
                   m_lowPoint = value;
                  

               }
           }
       }
     public double HighPoint
       {
           get
           {
               return m_highPoint;
           }
           set
           {
               if (m_highPoint != value)
               {
                   m_highPoint = value;

               }
           }
       }
     public double[] NegativeItem
     {
         get
         {
             return m_negativeItem;
         }
         set
         {
             if (m_negativeItem != value)
             {
                 m_negativeItem = value;

             }
         }
     }
       public BrushInfo BackInterior
       {
           get
           {
               return m_backInterior;
           }
           set
           {
               if (m_backInterior != value)
               {
                   m_backInterior = value;

               }
           }
       }
       /// <summary>
       /// Gets or sets the series type of the sparkline control.
       /// </summary>
       public SparkLineType Type
       {
           get
           {
               return m_sparkLineType;
           }
           set
           {
               if (m_sparkLineType != value)
               {
                   m_sparkLineType = value;
                   this.Refresh();

               }
           }
       }
       public Line LineStyle
       {
           get
           {
               return m_lineStyle;
           }
       }

       public Column ColumnStyle
       {
           get
           {
               return m_columnStyle;
           }
       }
       
       public Markers Markers
       {
           get
           {
               return m_markers;
           }
       }
       #endregion

       #region Constructor

       public SparkLine()
       {
           try
           {
               AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
               new Syncfusion.Core.Licensing.LicensedComponent(typeof(SparkLine));
           }
           finally
           {
               AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
           }
           InitializeComponent();
       }
       #endregion

       #region Public Method
       public double GetHighPoint()
       {
           return HighPoint;
       }
       public double GetLowPoint()
       {
           return LowPoint;
       }
       public double GetStartPoint()
       {
           return StartPoint;
       }
       public double GetEndPoint()
       {
           return EndPoint;
       }
       public double[] GetNegativePoint()
       {
           if(NegativeItem !=null)
               return NegativeItem;
           return null;
       }
       #endregion

       #region Event risers
       /// <summary>
       /// Event that is raised when the source list changed.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
       public void OnListChanged(object sender, ListChangedEventArgs e)
       {
           if (e.ListChangedType == ListChangedType.Reset)
           {
               this.ResetPoints();
           }
       }
       #endregion

       #region Implementation
       /// <summary>
       /// Overrides the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
       /// </summary>
       /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
       protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            Draw(e);
           
        }

       /// <summary>
       /// Renderer the SparklLine control
        /// </summary>
        private void  Draw(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            BrushPaint.FillRectangle(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), BackInterior);
            if (SparkLineType.Column == this.Type)
                m_columnrenderes.DrawSparkColumn(e.Graphics, this);
            else if (SparkLineType.WinLoss == this.Type)
                m_winlossrenderer.DrawSparkWinLoss(e.Graphics, this);
            else
                m_linerenderes.LineSparkLine(e.Graphics, this);
        }  
       public void ResetPoints()
       {
        m_sparklineSource.GetSourceList(this.Source,this);
         this.Refresh();
        }

            /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
          
        }

        #endregion
       #endregion
}
}
