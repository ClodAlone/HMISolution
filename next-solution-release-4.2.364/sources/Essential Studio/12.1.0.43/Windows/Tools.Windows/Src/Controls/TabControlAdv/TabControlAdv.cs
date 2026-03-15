#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Diagnostics;
using Syncfusion.Windows.Forms;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Design;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.ComponentModel.Design;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Renderers;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
    [ToolboxItem(false)]
    internal class LabelEdit : TextBox
    {
    }

    [Serializable]
    public class TabPageInfo
    {
        #region Class members

        private string m_strText = null;
        private int m_order = -1;
        private bool m_bIsSelected = false;

        #endregion

        #region Class properties

        public string Text
        {
            get
            {
                return m_strText;
            }
            set
            {
                if (value != m_strText)
                {
                    m_strText = value;
                }
            }
        }

        public int Order
        {
            get
            {
                return m_order;
            }
            set
            {
                if (value != m_order)
                {
                    m_order = value;
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return m_bIsSelected;
            }
            set
            {
                if (value != m_bIsSelected)
                {
                    m_bIsSelected = value;
                }
            }
        }

        #endregion

        #region Class initialize

        public TabPageInfo()
        { }
        public TabPageInfo(string text, int order, bool selected)
        {
            m_strText = text;
            m_order = order;
            m_bIsSelected = selected;
        }

        #endregion
    }

    [Serializable]
    public class TabControlInfo
    {
        #region Constants
        private const string DEF_ID = "TabControlInfo";
        #endregion

        #region members
        private Hashtable m_htPages = new Hashtable();
        #endregion

        #region Initialization
        private TabControlInfo()
        {
        }

        // Private constructor called during the deserialization process
        private TabControlInfo(SerializationInfo info, StreamingContext context)
        {
            Hashtable deserializedObj = info.GetValue(DEF_ID, typeof(Hashtable)) as Hashtable;

            if (deserializedObj != null)
            {
                m_htPages = deserializedObj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabControl"></param>
        public TabControlInfo(TabControlAdv tabControl)
        {
            if (tabControl == null)
                throw new ArgumentNullException("tabControl");

            if (tabControl != null && tabControl.TabPages != null && tabControl.TabPages.Count > 0)
            {
                ITabPanelData data = tabControl.TabPanelData;

                if (data != null)
                {
                    for (int i = 0, len = tabControl.TabPages.Count; i < len; i++)
                    {
                        TabPageAdv page = tabControl.TabPages[i];
                        int index = data.TabsData.IndexOf(page.TabData);

                        TabPageInfo pageInfo = new TabPageInfo(page.Text, index,
                            (index == data.SelectedIndex));

                        m_htPages[page.Name] = pageInfo;
                    }
                }
            }
        }

        #endregion

        #region Overrides
        // ISerializable implementation
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TabPagesInfo", m_htPages, typeof(Hashtable));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabName"></param>
        public TabPageInfo GetTabPageInfo(string tabName)
        {
            if (tabName == null)
                throw new ArgumentNullException("tabName");

            return (TabPageInfo)m_htPages[tabName];
        }
        #endregion
    }

    /// <summary>
    /// Manages a set of tab pages.
    /// </summary>
    /// <remarks>
    /// <para>A TabControlAdv contains tab pages, which are represented
    /// by <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv"/> objects that you add through the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabPages"/> property.</para>
    /// <para>It provides a set of pre-built tab types(<see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property) with different look
    /// and feel, allows you to align the tabs to either of the four sides
    /// of the Control (<see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.Alignment"/> property), can be used in a singleline or multiline mode(<see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.Multiline"/> property)
    /// and provides a broad set of properties which affects its appearance and behavior.</para>
    /// <para>
    /// It also provides you a simple event based mechanism (<see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.DrawItem"/> event) to customize
    /// the drawing of the tabs.
    /// </para>
    /// <para>
    /// To enable themes support in XP, turn on the <see cref="ThemesEnabled"/> property.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example uses the Visual Studio .NET Windows Forms Designer to
    /// create a TabControlAdv with three tab pages. Each tab page contains several controls.
    /// <code lang="C#">
    /// public class Form1 : System.Windows.Forms.Form
    /// {
    ///     private Syncfusion.Windows.Forms.Tools.TabControlAdv TabControlAdv1;
    ///     private System.Windows.Forms.Label tab2label1;
    ///     private System.Windows.Forms.Button tab3Button;
    ///     private System.Windows.Forms.MonthCalendar tab3monthCalendar1;
    ///     private System.Windows.Forms.DateTimePicker tab3dateTimePicker1;
    ///     private System.Windows.Forms.Label tab3label;
    ///     private System.Windows.Forms.Label tab2label2;
    ///     private System.Windows.Forms.TextBox tab2textBox1;
    ///     private System.Windows.Forms.ListBox tab1listBox1;
    ///     private System.Windows.Forms.ComboBox tab1comboBox1;
    /// 
    ///     private System.Windows.Forms.Label tab1label1;
    ///     private Syncfusion.Windows.Forms.Tools.TabPageAdv tab1;
    ///     private Syncfusion.Windows.Forms.Tools.TabPageAdv tab3;
    ///     private Syncfusion.Windows.Forms.Tools.TabPageAdv tab2;
    ///     private System.ComponentModel.IContainer components;
    /// 
    ///         public Form1()
    ///         {
    ///             //
    ///             // Required for Windows Form Designer support
    ///             //
    ///             InitializeComponent();
    ///         }
    ///         private void InitializeComponent()
    ///             {
    ///                 this.components = new System.ComponentModel.Container();
    ///                 this.tab2label1 = new System.Windows.Forms.Label();
    ///                 this.tab1 = new Syncfusion.Tools.Windows.Forms.Tab.TabPageAdv();
    ///                 this.tab1listBox1 = new System.Windows.Forms.ListBox();
    ///                 this.tab1comboBox1 = new System.Windows.Forms.ComboBox();
    ///                 this.tab1label1 = new System.Windows.Forms.Label();
    ///                 this.tab3 = new Syncfusion.Tools.Windows.Forms.Tab.TabPageAdv();
    ///                 this.tab3Button = new System.Windows.Forms.Button();
    ///                 this.tab3monthCalendar1 = new System.Windows.Forms.MonthCalendar();
    ///                 this.tab3dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
    ///                 this.tab3label = new System.Windows.Forms.Label();
    ///                 this.tab2 = new Syncfusion.Tools.Windows.Forms.Tab.TabPageAdv();
    ///                 this.tab2label2 = new System.Windows.Forms.Label();
    ///                 this.tab2textBox1 = new System.Windows.Forms.TextBox();
    ///                 this.TabControlAdv1 = new Syncfusion.Tools.Windows.Forms.Tab.TabControlAdv();
    ///                 this.tab1.SuspendLayout();
    ///                 this.tab3.SuspendLayout();
    ///                 this.tab2.SuspendLayout();
    ///                 this.TabControlAdv1.SuspendLayout();
    ///                 this.SuspendLayout();
    ///                 //
    ///                 // tab2label1
    ///                 //
    ///                 this.tab2label1.Dock = System.Windows.Forms.DockStyle.Top;
    ///                 this.tab2label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
    ///                 this.tab2label1.Name = "tab2label1";
    ///                 this.tab2label1.Size = new System.Drawing.Size(373, 48);
    ///                 this.tab2label1.TabIndex = 0;
    ///                 this.tab2label1.Text = "Tab2";
    ///                 this.tab2label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
    ///                 this.tab2label1.Paint += new System.Windows.Forms.PaintEventHandler(this.TabPageAdv1_Paint);
    ///                 //
    ///                 // tab1
    ///                 //
    ///                 this.tab1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
    ///                 this.tab1.Controls.AddRange(new System.Windows.Forms.Control[] {
    ///                                                                                 this.tab1listBox1,
    ///                                                                                 this.tab1comboBox1,
    ///                                                                                 this.tab1label1});
    ///                 this.tab1.Location = new System.Drawing.Point(1, 29);
    ///                 this.tab1.Name = "tab1";
    ///                 this.tab1.Size = new System.Drawing.Size(373, 257);
    ///                 this.tab1.TabIndex = 0;
    ///                 this.tab1.Text = "Tab 1";
    ///                 this.tab1.ToolTipText = "0asdfasdf";
    ///                 //
    ///                 // tab1listBox1
    ///                 //
    ///                 this.tab1listBox1.Items.AddRange(new object[] {
    ///                                                                 "Item 1",
    ///                                                                 "Item 2",
    ///                                                                 "Item 3"});
    ///                 this.tab1listBox1.Location = new System.Drawing.Point(8, 88);
    ///                 this.tab1listBox1.Name = "tab1listBox1";
    ///                 this.tab1listBox1.Size = new System.Drawing.Size(192, 147);
    ///                 this.tab1listBox1.TabIndex = 2;
    ///                 //
    ///                 // tab1comboBox1
    ///                 //
    ///                 this.tab1comboBox1.DropDownWidth = 192;
    ///                 this.tab1comboBox1.Location = new System.Drawing.Point(8, 56);
    ///                 this.tab1comboBox1.Name = "tab1comboBox1";
    ///                 this.tab1comboBox1.Size = new System.Drawing.Size(192, 21);
    ///                 this.tab1comboBox1.TabIndex = 1;
    ///                 this.tab1comboBox1.Text = "comboBox1";
    ///                 //
    ///                 // tab1label1
    ///                 //
    ///                 this.tab1label1.Dock = System.Windows.Forms.DockStyle.Top;
    ///                 this.tab1label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
    ///                 this.tab1label1.Name = "tab1label1";
    ///                 this.tab1label1.Size = new System.Drawing.Size(371, 48);
    ///                 this.tab1label1.TabIndex = 0;
    ///                 this.tab1label1.Text = "Tab Page 1";
    ///                 this.tab1label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
    ///                 //
    ///                 // tab3
    ///                 //
    ///                 this.tab3.Controls.AddRange(new System.Windows.Forms.Control[] {
    ///                                                                                 this.tab3Button,
    ///                                                                                 this.tab3monthCalendar1,
    ///                                                                                 this.tab3dateTimePicker1,
    ///                                                                                 this.tab3label});
    ///                 this.tab3.Location = new System.Drawing.Point(1, 29);
    ///                 this.tab3.Name = "tab3";
    ///                 this.tab3.Size = new System.Drawing.Size(373, 257);
    ///                 this.tab3.TabIndex = 1;
    ///                 this.tab3.Text = "Tab 3";
    ///                 this.tab3.ToolTipText = "2asdfasdf";
    ///                 this.tab3.Layout += new System.Windows.Forms.LayoutEventHandler(this.TabPageAdv2_Layout);
    ///                 //
    ///                 // tab3Button
    ///                 //
    ///                 this.tab3Button.Location = new System.Drawing.Point(264, 72);
    ///                 this.tab3Button.Name = "tab3Button";
    ///                 this.tab3Button.Size = new System.Drawing.Size(72, 24);
    ///                 this.tab3Button.TabIndex = 3;
    ///                 this.tab3Button.Text = "button1";
    ///                 this.tab3Button.Click += new System.EventHandler(this.button1_Click);
    ///                 //
    ///                 // tab3monthCalendar1
    ///                 //
    ///                 this.tab3monthCalendar1.Location = new System.Drawing.Point(16, 96);
    ///                 this.tab3monthCalendar1.Name = "tab3monthCalendar1";
    ///                 this.tab3monthCalendar1.TabIndex = 2;
    ///                 //
    ///                 // tab3dateTimePicker1
    ///                 //
    ///                 this.tab3dateTimePicker1.Location = new System.Drawing.Point(8, 64);
    ///                 this.tab3dateTimePicker1.Name = "tab3dateTimePicker1";
    ///                 this.tab3dateTimePicker1.TabIndex = 1;
    ///                 //
    ///                 // tab3label
    ///                 //
    ///                 this.tab3label.Dock = System.Windows.Forms.DockStyle.Top;
    ///                 this.tab3label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
    ///                 this.tab3label.Name = "tab3label";
    ///                 this.tab3label.Size = new System.Drawing.Size(373, 48);
    ///                 this.tab3label.TabIndex = 0;
    ///                 this.tab3label.Text = "Tab3";
    ///                 this.tab3label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
    ///                 //
    ///                 // tab2
    ///                 //
    ///                 this.tab2.Controls.AddRange(new System.Windows.Forms.Control[] {
    ///                                                                                 this.tab2label2,
    ///                                                                                 this.tab2textBox1,
    ///                                                                                 this.tab2label1});
    ///                 this.tab2.Location = new System.Drawing.Point(1, 29);
    ///                 this.tab2.Name = "tab2";
    ///                 this.tab2.Size = new System.Drawing.Size(373, 257);
    ///                 this.tab2.TabIndex = 2;
    ///                 this.tab2.Text = "Tab2";
    ///                 this.tab2.ToolTipText = "1asdfasdfasd";
    ///                 //
    ///                 // tab2label2
    ///                 //
    ///                 this.tab2label2.Location = new System.Drawing.Point(8, 96);
    ///                 this.tab2label2.Name = "tab2label2";
    ///                 this.tab2label2.Size = new System.Drawing.Size(112, 16);
    ///                 this.tab2label2.TabIndex = 2;
    ///                 this.tab2label2.Text = "Text Entry:";
    ///                 //
    ///                 // tab2textBox1
    ///                 //
    ///                 this.tab2textBox1.Location = new System.Drawing.Point(8, 120);
    ///                 this.tab2textBox1.Multiline = true;
    ///                 this.tab2textBox1.Name = "tab2textBox1";
    ///                 this.tab2textBox1.Size = new System.Drawing.Size(368, 80);
    ///                 this.tab2textBox1.TabIndex = 1;
    ///                 this.tab2textBox1.Text = "textBox1";
    ///                 //
    ///                 // TabControlAdv1
    ///                 //
    ///                 this.TabControlAdv1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
    ///                     | System.Windows.Forms.AnchorStyles.Left)
    ///                     | System.Windows.Forms.AnchorStyles.Right);
    ///                 this.TabControlAdv1.BackColor = System.Drawing.SystemColors.ActiveBorder;
    ///                 this.TabControlAdv1.Controls.AddRange(new System.Windows.Forms.Control[] {
    ///                                                                                             this.tab3,
    ///                                                                                             this.tab2,
    ///                                                                                             this.tab1});
    ///                 this.TabControlAdv1.Cursor = System.Windows.Forms.Cursors.Default;
    ///                 this.TabControlAdv1.HotTrack = true;
    ///                 this.TabControlAdv1.ImageList = this.imageList1;
    ///                 this.TabControlAdv1.ItemSize = new System.Drawing.Size(80, 30);
    ///                 this.TabControlAdv1.Location = new System.Drawing.Point(40, 16);
    ///                 this.TabControlAdv1.Name = "TabControlAdv1";
    ///                 this.TabControlAdv1.ShowToolTips = true;
    ///                 this.TabControlAdv1.Size = new System.Drawing.Size(376, 288);
    ///                 this.TabControlAdv1.TabGap = 20;
    ///                 this.TabControlAdv1.TabIndex = 4;
    ///                 this.TabControlAdv1.TabStyle = typeof(Syncfusion.Tools.Windows.Forms.Tab.TabRenderer2D);
    ///                 this.TabControlAdv1.TextAlignment = System.Drawing.StringAlignment.Near;
    ///                 this.TabControlAdv1.UserMoveTabs = true;
    ///                 this.TabControlAdv1.VSLikeScrollButton = true;
    ///                 //
    ///                 // Form1
    ///                 //
    ///                 this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
    ///                 this.ClientSize = new System.Drawing.Size(688, 309);
    ///                 this.Controls.AddRange(new System.Windows.Forms.Control[] {
    ///                                                                             this.TabControlAdv1});
    ///                 this.Name = "Form1";
    ///                 this.Text = "Form1";
    ///                 this.tab1.ResumeLayout(false);
    ///                 this.tab3.ResumeLayout(false);
    ///                 this.tab2.ResumeLayout(false);
    ///                 this.TabControlAdv1.ResumeLayout(false);
    ///                 this.ResumeLayout(false);
    /// 
    ///             }
    ///         }
    /// </code>
    /// <code lang="VB">
    /// Private Function Form1() As Public
    /// '
    /// ' Required for Windows Form Designer support
    /// '
    /// InitializeComponent()
    /// End Function
    /// 
    /// Private  Sub InitializeComponent()
    /// Me.components = New System.ComponentModel.Container()
    /// Me.tab2label1 = New System.Windows.Forms.Label()
    /// Me.tab1 = New Syncfusion.Tools.Windows.Forms.Tab.TabPageAdv()
    /// Me.tab1listBox1 = New System.Windows.Forms.ListBox()
    /// Me.tab1comboBox1 = New System.Windows.Forms.ComboBox()
    /// Me.tab1label1 = New System.Windows.Forms.Label()
    /// Me.tab3 = New Syncfusion.Tools.Windows.Forms.Tab.TabPageAdv()
    /// Me.tab3Button = New System.Windows.Forms.Button()
    /// Me.tab3monthCalendar1 = New System.Windows.Forms.MonthCalendar()
    /// Me.tab3dateTimePicker1 = New System.Windows.Forms.DateTimePicker()
    /// Me.tab3label = New System.Windows.Forms.Label()
    /// Me.tab2 = New Syncfusion.Tools.Windows.Forms.Tab.TabPageAdv()
    /// Me.tab2label2 = New System.Windows.Forms.Label()
    /// Me.tab2textBox1 = New System.Windows.Forms.TextBox()
    /// Me.TabControlAdv1 = New Syncfusion.Tools.Windows.Forms.Tab.TabControlAdv()
    /// Me.tab1.SuspendLayout()
    /// Me.tab3.SuspendLayout()
    /// Me.tab2.SuspendLayout()
    /// Me.TabControlAdv1.SuspendLayout()
    /// Me.SuspendLayout()
    /// 
    /// '
    /// ' tab2label1
    /// '
    /// Me.tab2label1.Dock = System.Windows.Forms.DockStyle.Top
    /// Me.tab2label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (CType((0), System.Byte)))
    /// Me.tab2label1.Name = "tab2label1"
    /// Me.tab2label1.Size = New System.Drawing.Size(373, 48)
    /// Me.tab2label1.TabIndex = 0
    /// Me.tab2label1.Text = "Tab2"
    /// Me.tab2label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
    /// Me.tab2label1.Paint += New System.Windows.Forms.PaintEventHandler(Me.TabPageAdv1_Paint)
    /// 
    /// '
    /// ' tab1
    /// '
    /// Me.tab1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
    /// Me.tab1.Controls.AddRange(New System.Windows.Forms.Control()
    /// {Me.tab1listBox1,Me.tab1comboBox1,Me.tab1label1})
    /// Me.tab1.Location = New System.Drawing.Point(1, 29)
    /// Me.tab1.Name = "tab1"
    /// Me.tab1.Size = New System.Drawing.Size(373, 257)
    /// Me.tab1.TabIndex = 0
    /// Me.tab1.Text = "Tab 1"
    /// Me.tab1.ToolTipText = "0asdfasdf"
    /// '
    /// ' tab1listBox1
    /// '
    /// Me.tab1listBox1.Items.AddRange(New System.Windows.Forms.Control()
    /// {Me.tab1listBox1,Me.tab1comboBox1,Me.tab1label1})
    /// Dim Object() As Me.tab1listBox1.Items.AddRange(New string()
    /// {
    /// "Item 1",
    /// "Item 2",
    /// "Item 3"
    /// })
    /// Me.tab1listBox1.Location = New System.Drawing.Point(8, 88)
    /// Me.tab1listBox1.Name = "tab1listBox1"
    /// Me.tab1listBox1.Size = New System.Drawing.Size(192, 147)
    /// Me.tab1listBox1.TabIndex = 2
    /// '
    /// ' tab1comboBox1
    /// '
    /// Me.tab1comboBox1.DropDownWidth = 192
    /// Me.tab1comboBox1.Location = New System.Drawing.Point(8, 56)
    /// Me.tab1comboBox1.Name = "tab1comboBox1"
    /// Me.tab1comboBox1.Size = New System.Drawing.Size(192, 21)
    /// Me.tab1comboBox1.TabIndex = 1
    /// Me.tab1comboBox1.Text = "comboBox1"
    /// '
    /// ' tab1label1
    /// '
    /// Me.tab1label1.Dock = System.Windows.Forms.DockStyle.Top
    /// Me.tab1label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (CType((0), System.Byte)))
    /// Me.tab1label1.Name = "tab1label1"
    /// Me.tab1label1.Size = New System.Drawing.Size(371, 48)
    /// Me.tab1label1.TabIndex = 0
    /// Me.tab1label1.Text = "Tab Page 1"
    /// Me.tab1label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
    /// 
    /// Me.tab3.Controls.AddRange(New System.Windows.Forms.Control()
    /// {Me.tab3Button,Me.tab3monthCalendar1,Me.tab3dateTimePicker1,Me.tab3label})
    /// Me.tab3.Location = New System.Drawing.Point(1, 29)
    /// Me.tab3.Name = "tab3"
    /// Me.tab3.Size = New System.Drawing.Size(373, 257)
    /// Me.tab3.TabIndex = 1
    /// Me.tab3.Text = "Tab 3"
    /// Me.tab3.ToolTipText = "2asdfasdf"
    /// Me.tab3.Lay+= New System.Windows.Forms.LayoutEventHandler(Me.TabPageAdv2_Layout)
    /// '
    /// ' tab3Button
    /// '
    /// Me.tab3Button.Location = New System.Drawing.Point(264, 72)
    /// Me.tab3Button.Name = "tab3Button"
    /// Me.tab3Button.Size = New System.Drawing.Size(72, 24)
    /// Me.tab3Button.TabIndex = 3
    /// Me.tab3Button.Text = "button1"
    /// Me.tab3Button.Click += New System.EventHandler(Me.button1_Click)
    /// '
    /// ' tab3monthCalendar1
    /// '
    /// Me.tab3monthCalendar1.Location = New System.Drawing.Point(16, 96)
    /// Me.tab3monthCalendar1.Name = "tab3monthCalendar1"
    /// Me.tab3monthCalendar1.TabIndex = 2
    /// '
    /// ' tab3dateTimePicker1
    /// '
    /// Me.tab3dateTimePicker1.Location = New System.Drawing.Point(8, 64)
    /// Me.tab3dateTimePicker1.Name = "tab3dateTimePicker1"
    /// Me.tab3dateTimePicker1.TabIndex = 1
    /// '
    /// ' tab3label
    /// '
    /// Me.tab3label.Dock = System.Windows.Forms.DockStyle.Top
    /// Me.tab3label.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (CType((0), System.Byte)))
    /// Me.tab3label.Name = "tab3label"
    /// Me.tab3label.Size = New System.Drawing.Size(373, 48)
    /// Me.tab3label.TabIndex = 0
    /// Me.tab3label.Text = "Tab3"
    /// Me.tab3label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
    /// '
    /// ' tab2
    /// '
    /// Me.tab2.Controls.AddRange(New System.Windows.Forms.Control()
    /// {Me.tab2label2,Me.tab2textBox1,Me.tab2label1})
    ///     Me.tab2.Location = New System.Drawing.Point(1, 29)
    /// Me.tab2.Name = "tab2"
    /// Me.tab2.Size = New System.Drawing.Size(373, 257)
    /// Me.tab2.TabIndex = 2
    /// Me.tab2.Text = "Tab2"
    /// Me.tab2.ToolTipText = "1asdfasdfasd"
    /// '
    /// ' tab2label2
    /// '
    /// Me.tab2label2.Location = New System.Drawing.Point(8, 96)
    /// Me.tab2label2.Name = "tab2label2"
    /// Me.tab2label2.Size = New System.Drawing.Size(112, 16)
    /// Me.tab2label2.TabIndex = 2
    /// Me.tab2label2.Text = "Text Entry:"
    /// '
    /// ' tab2textBox1
    /// '
    /// Me.tab2textBox1.Location = New System.Drawing.Point(8, 120)
    /// Me.tab2textBox1.Multiline = True
    /// Me.tab2textBox1.Name = "tab2textBox1"
    /// Me.tab2textBox1.Size = New System.Drawing.Size(368, 80)
    /// Me.tab2textBox1.TabIndex = 1
    /// Me.tab2textBox1.Text = "textBox1"
    /// '
    /// ' TabControlAdv1
    /// '
    /// Me.TabControlAdv1.Anchor = (((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
    /// Or System.Windows.Forms.AnchorStyles.Left)  _
    /// Or System.Windows.Forms.AnchorStyles.Right)
    /// Me.TabControlAdv1.Cursor = System.Windows.Forms.Cursors.Default
    /// Me.TabControlAdv1.HotTrack = True
    /// Me.TabControlAdv1.ImageList = Me.imageList1
    /// Me.TabControlAdv1.ItemSize = New System.Drawing.Size(80, 30)
    /// Me.TabControlAdv1.Location = New System.Drawing.Point(40, 16)
    /// Me.TabControlAdv1.Name = "TabControlAdv1"
    /// Me.TabControlAdv1.ShowToolTips = True
    /// Me.TabControlAdv1.Size = New System.Drawing.Size(376, 288)
    /// Me.TabControlAdv1.TabGap = 20
    /// Me.TabControlAdv1.TabIndex = 4
    /// Me.TabControlAdv1.TabStyle = Type.GetType(Syncfusion.Tools.Windows.Forms.Tab.TabRenderer2D)
    /// Me.TabControlAdv1.TextAlignment = System.Drawing.StringAlignment.Near
    /// Me.TabControlAdv1.UserMoveTabs = True
    /// Me.TabControlAdv1.VSLikeScrollButton = True
    /// '
    /// ' Form1
    /// '
    /// Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    /// Me.ClientSize = New System.Drawing.Size(688, 309)
    /// Me.Controls.AddRange(New System.Windows.Forms.Control()
    /// {Me.TabControlAdv1})
    ///     Me.Name = "Form1"
    /// Me.Text = "Form1"
    /// Me.tab1.ResumeLayout(False)
    /// Me.tab3.ResumeLayout(False)
    /// Me.tab2.ResumeLayout(False)
    /// Me.TabControlAdv1.ResumeLayout(False)
    /// Me.ResumeLayout(False)
    /// End Sub
    /// </code>
    /// </example>
    [
    Designer(
        typeof(Syncfusion.Windows.Forms.Tools.Design.TabControlAdvDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    DefaultProperty(@"TabPages"),
    DefaultEvent(@"SelectedIndexChanged"),
    DefaultChildType(typeof(TabPageAdv)),
    ToolboxBitmap(typeof(TabControlAdv), "ToolboxIcons.TabControlExt.bmp"),
    Description("Manages a set of tab pages.")
    ]
    public class TabControlAdv :
        Control,
        ITabControl,
        ISupportInitialize,
        ISupportOffice2007Theme,
        IVisualStyle 
    {
        #region Class constants

        private const string DEF_PERSIST_ID = "TabControlAdvInfo";

        private Color DEF_ONENOTESTYLE_COLOR = Color.FromArgb(158, 190, 245);

        private const int DEF_SCROLL_BUTTONS_GRADIENT_OFFSET = 5;
        private const int DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET = 1;

        private const int DEF_ADJUST_POS = 1;

        #endregion

        /// <summary>
        /// Loads the tab state from persistent storage medium.
        /// </summary>
        public virtual void LoadState()
        {
            LoadState(AppStateSerializer.GetSingleton());
        }

        /// <summary>
        /// Saves the tab state to Isolated Storage.
        /// </summary>
        public virtual void SaveState()
        {
            SaveState(AppStateSerializer.GetSingleton());
        }

        /// <summary>
        /// Reads a previously serialized tabState using the AppStateSerializer object.
        /// </summary>
        /// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
        /// <remarks>
        /// Reads the tabstate information from the specified persistent store and applies the new state.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// //Loading TabState from xml file(TabState.xml located in Application folder)
        /// AppStateSerializer serializer = new AppStateSerializer(SerializeMode.XMLFile, "TabState");
        /// this.tabControlAdv1.LoadState(serializer);
        /// </code>
        /// <code lang="VB">
        /// 'Loading TabState from xml file(TabState.xml located in Application folder)
        /// Dim serializer As New AppStateSerializer(SerializeMode.XMLFile, "TabState")
        /// Me.tabControlAdv1.LoadState(serializer)
        /// </code>
        /// </example>
        public virtual void LoadState(AppStateSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            if (this.TabPages.Count > 0)
            {
                TabControlInfo tabsInfo = serializer.DeserializeObject(DEF_PERSIST_ID) as TabControlInfo;

                if (tabsInfo != null)
                {
                    int selectedIndex = -1;

                    Hashtable htPages = new Hashtable();
                    m_tabPanelData.TabsData.Clear();

                    for (int i = 0, len = TabPages.Count; i < len; i++)
                    {
                        TabPageAdv page = TabPages[i];
                        TabPageInfo info = tabsInfo.GetTabPageInfo(this.TabPages[i].Name);

                        if (info != null)
                        {
                            page.Text = info.Text;

                            htPages[info.Order] = page;

                            if (info.IsSelected)
                            {
                                selectedIndex = info.Order;
                            }
                        }
                    }

                    for (int i = 0, len = this.TabPages.Count; i < len; i++)
                    {
                        TabPageAdv page = (TabPageAdv)htPages[i];
                        if (page == null)
                        {
                            page = this.TabPages[i];
                        }
                        m_tabPanelData.TabsData.Add(page.TabData);
                    }

                    this.SelectedIndex = selectedIndex;
                }

            }
        }

        /// <summary>
        /// Saves the current tab state information to the specified <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/>.
        /// </summary>
        /// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
        /// <remarks>
        /// Writes the tab state information like Active TabPage, TabOrder and New Text using Label Edit to the persistence medium.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// //Saving TabState to xml file(TabState.xml located in Application folder)
        /// AppStateSerializer serializer =new AppStateSerializer(SerializeMode.XMLFile, "TabState");
        /// this.tabControlAdv1.SaveState(serializer);
        /// serializer.PersistNow();
        /// </code>
        /// <code lang="VB">
        /// 'Saving TabState to xml file(TabState.xml located in Application folder)
        /// Dim serializer As New AppStateSerializer(SerializeMode.XMLFile, "TabState")
        /// Me.tabControlAdv1.SaveState(serializer)
        /// serializer.PersistNow()
        /// </code>
        /// </example>
        public virtual void SaveState(AppStateSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            if (this.TabPages.Count > 0)
            {
                serializer.SerializeObject(DEF_PERSIST_ID, new TabControlInfo(this));
            }
        }

        private bool m_bPersistTabState = false;

        /// <summary>
        /// Gets or Sets, should Tabs state automatically persisted or not.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or Sets, whether Tabs State (ActivePage, TabOrder, Text) should be automatically persisted or not.")]
        public bool PersistTabState
        {
            get
            {
                return m_bPersistTabState;
            }
            set
            {
                if (value != m_bPersistTabState)
                {
                    m_bPersistTabState = value;
                }
            }
        }

        /// <summary>
        /// Should rotate tabs when RightToLeft mode active.
        /// </summary>
        private bool m_bRotateTabsWhenRTL = true;

        /// <summary>
        /// Gets or sets should rotate tabs when RightToLeft mode is active.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets should rotate tabs are drawn in RightToLeft mode.")]
        public bool RotateTabsWhenRTL
        {
            get
            {
                return m_bRotateTabsWhenRTL;
            }
            set
            {
                if (m_bRotateTabsWhenRTL != value)
                {
                    Type vs2005StyleType = typeof(TabRendererWhidbey);
                    Type oneNoteStyleType = typeof(OneNoteStyleRenderer);
                    Type dockingWhidbeyTypeBeta = typeof(TabRendererDockingWhidbeyBeta);
                    Type dockingWhidbeyType = typeof(TabRendererDockingWhidbey);
                    Type VS2008Type = typeof(TabRendererVS2008);

                    if (this.TabStyle == oneNoteStyleType ||
                        this.TabStyle == vs2005StyleType ||
                        this.TabStyle == dockingWhidbeyTypeBeta ||
                        this.TabStyle == dockingWhidbeyType ||
                        this.TabStyle == VS2008Type || m_bIsInitializing)
                    {
                        m_bRotateTabsWhenRTL = value;

                        this.ComputeTabPanelBounds();
                        this.Invalidate();
                    }
                    else
                    {
                        if (this.DesignMode)
                        {
                            throw new ArgumentException("Only VS2005Style, OneNoteStyle and DockingWhidbey styles support RotateTabsWhenRTL property.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicate multiline text.
        /// </summary>
        private bool m_bMultilineText = false;

        /// <summary>
        /// Gets or sets indicate multiline text.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Gets or sets indicate multiline text.")]
        public bool MultilineText
        {
            get
            {
                return m_bMultilineText;
            }
            set
            {
                if (m_bMultilineText != value)
                {
                    m_bMultilineText = value;
                    this.ComputeTabPanelBounds();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                if (!(this is BackStage))
                {
                    if (value == "Office2007Blue")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2007);
                        Office2007ColorScheme = Office2007Theme.Blue;
                    }
                    else if (value == "Office2007Silver")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2007);
                        Office2007ColorScheme = Office2007Theme.Silver;
                    }
                    else if (value == "Office2007Black")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2007);
                        Office2007ColorScheme = Office2007Theme.Black;
                    }
                    else if (value == "Office2010Blue")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2010);
                        Office2010ColorTheme = Office2010Theme.Blue;
                    }
                    else if (value == "Office2010Silver")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2010);
                        Office2010ColorTheme = Office2010Theme.Silver;
                    }
                    else if (value == "Office2010Black")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2010);
                        Office2010ColorTheme = Office2010Theme.Black;
                    }
                    else if (value == "Metro")
                    {
                        TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererMetro);                        
                    }
                    else if (value == "Managed")
                    {
                        Office2007ColorScheme = Office2007Theme.Managed;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control interprets an ampersand character to be an access key prefix character.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets a value indicating whether the control interprets an ampersand character (&) to be an access key prefix character.")]
        public bool UseMnemonic
        {
            get
            {
                return m_useMnemonic;
            }

            set
            {
                if (m_useMnemonic != value)
                {
                    m_useMnemonic = value;
                    this.Invalidate();
                }
            }
        }

        private TabPrimitivesHost m_tabPrimitivesHost = null;

        /// <summary>
        /// Occurs before navigation button click.
        /// </summary>
        [
        Category("Action"),
        Description("Occurs before navigation button click.")
        ]
        public event TabPrimitiveClick TabPrimitiveClick;

        /// <summary>
        /// Raises the NavigationButtonClick event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabPrimitiveClick(TabPrimitiveClickEventArgs e)
        {
            if (this.TabPrimitiveClick != null)
            {
                this.TabPrimitiveClick(this, e);
            }
        }

        /// <summary>
        /// Gets the navigation control used to navigate through tabs.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets the navigation control used to navigate through tabs.")]
        public TabPrimitivesHost TabPrimitivesHost
        {
            get
            {
                return m_tabPrimitivesHost;
            }
        }

        /// <summary>
        /// Gets the value indicating whether the component is currently in design mode.
        /// </summary>
        internal bool IsDesignMode
        {
            get
            {
                return DesignMode;
            }
        }

        /// <summary>
        /// Contains a list of Control instances.
        /// </summary>
        /// <remarks>This collection makes sure that the TabControlAdv's
        /// Controls list will get populated with only TabPageAdv objects.</remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        public new class ControlCollection :
        Control.ControlCollection
        {

            // Fields
            private TabControlAdv tabControl;

            /// <summary>
            /// Creates an instance of the TabControlAdv.ControlCollection class.
            /// </summary>
            /// <param name="tabControl">The TabControlAdv object whose
            /// tab page collection this list will hold.</param>
            public ControlCollection(TabControlAdv tabControl)
                : base(tabControl)
            {
                this.tabControl = tabControl;
            }

            /// <summary>
            /// Removed the specified control to the collection..
            /// </summary>
            /// <param name="value">The tabpage to remove.</param>
            public override void Remove(Control value)
            {
                base.Remove(value);
                if (value is TabPageAdv)
                {
                    if (tabControl.TabPages.IndexOf(value) != -1)
                        tabControl.TabPages.Remove(value);
                }
            }

            /// <summary>
            /// Adds the specified control to the collection.
            /// </summary>
            /// <param name="value">The tabpage to add.</param>
            public override void Add(Control value)
            {
                if (!(value is TabPageAdv) && !(value is LabelEdit) &&
                    !(value is Syncfusion.Windows.Forms.ScrollButtons) && !(value is BackStageButton) && !(value is BackStageSeparator))
                    throw new ArgumentException("Invalid TabPageAdv Type:" + value.GetType().Name);

                if (value is TabPageAdv)
                {
                    // If not already in teh list, add it to the list
                    if (tabControl.TabPages.IndexOf(value) == -1)
                    {
                        tabControl.TabPages.Add((TabPageAdv)value);
                    }
                }

                base.Add(value);
            }

            /// <summary>
            /// Overridden. See <see cref="System.Windows.Forms.Control.ControlCollection.AddRange"/>
            /// </summary>
            /// <param name="controls">An array of controls.</param>
            public override void AddRange(Control[] controls)
            {
                base.AddRange(controls);
            }

        }

        #region CONSTANTS

        internal static readonly Point DEFAULT_PADDING = new Point(6, 3);
        private static readonly Size m_sizeScrollButtonsSize = new Size(30, 15);
        private const int c_defaultBorderWidth = 5;
        private const int c_cornerCut = 3;
        private const int c_CLOSE_BUTTON_PADDING = 5;
        #endregion CONSTANTS

        #region FIELDS
        private bool m_bLabelEdit = false;
        private ThemedTabDrawing m_themedDrawing = null;
        private TabPanelData m_tabPanelData;
        protected ITabPanelRenderer m_tabPanelRenderer;
        private TabPanelDefaultPropertiesReflected m_tabPanelDefaultProperties;
        private RectangleF m_internalTabPanelBounds;
        private TabPageAdvCollection m_tabPagesCollection;
        private bool m_needLayout = false;
        private Graphics m_currentGraphics = null;
        private TabPageAdv m_currentTabPage;
        private TabPageAdv m_previousTabPage;
        private bool m_bShowScroll = true;
        private bool m_bCachedShowScroll = true;
        private bool m_vsLikeScrollButton = false;
        private bool m_themesEnabled = false;
        private bool m_focusOnTabClick = true;
        private bool m_switchPagesForDialogKeys = true;
        private bool m_useMnemonic = true;

        protected ScrollButtons m_sbScrollButtons = null;
        private bool m_bMirrored = false;
        private bool dockJustChanged = false;
        /// <summary>
        /// The value that indicates whether close button should be visible for each tab.
        /// </summary>
        private bool m_bShowCloseButton = false;
        /// <summary>
        /// The value that indicates whether close button should be visible for tab only if mouse is over it.
        /// </summary>
        private bool m_bShowCloseButtonForActiveTabOnly = false;
        /// <summary>
        /// The value that indicates previous active tab index.
        /// </summary>
        private int m_iPrevActiveTabIndex = int.MinValue;

        // validation status variable
        ValidateStatus m_validateStatus = ValidateStatus.None;

        internal TabControlAdvWeakContainer m_tabControlAdvWeakContainer = null;

        /// <summary></summary>
        private Office2007Theme m_Office2007ColorScheme = Office2007Theme.Blue;
        private Office2010Theme m_Office2010ColorScheme = Office2010Theme.Blue;
        /// <summary></summary>
        private Office2010Colors m_office2010ColorTable = null;
        private Office2007Colors m_office2007ColorTable = null;
        /// <summary>
        /// Indicates width of the custom borders.
        /// </summary>
        private int m_borderWidth = c_defaultBorderWidth;
        /// <summary>
        /// Indicates visibility of the custom borders.
        /// </summary>
        private bool m_borderVisible = false;
        /// <summary>
        /// Indicates color of the custom borders.
        /// </summary>
        private Color m_borderColor = SystemColors.Control;
        #endregion FIELDS

        #region EVENTS
        /// <summary>
        /// Occurs when the tabs are drawn.
        /// </summary>
        /// <value>
        /// The event handler receives an argument of type DrawTabEventArgs containing data related to this event. 
        /// Take a look at the DrawTabEventArgs class reference for information on the
        /// data passed to this event handler.
        /// </value>
        /// <remarks>
        /// In this event handler, you can take over drawing of the whole tab or draw portions
        /// of the tab while delegating the rest to the default drawing logic.
        /// <para>
        /// A tab's default drawing logic is exposed in the DrawTabEventArgs args.
        /// The default drawing logic is classified as drawing the background, interiors and borders.
        /// You can call the corresponding DrawBackground, DrawInterior, DrawBorders methods
        /// in the DrawTabEventArgs class to use the default painting logic.
        /// The example below illustrates this logic.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example customizes tab drawing to create a Yahoo-Messenger like
        /// tab panel. It uses just the tab's default drawing logic to obtain this effect.
        /// <code lang="C#">
        /// // Make sure to set the "3D" tab style, turn on the HotTrack property and handle
        /// // the DrawItem event of the tab control.
        /// private void InitializeComponent()
        /// {
        ///         ....
        ///         this.TabControlAdv1.HotTrack = true;
        ///         this.TabControlAdv1.TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRenderer3D);
        ///         this.TabControlAdv1.DrawItem += new Syncfusion.Windows.Forms.Tools.DrawTabEventHandler(this.Tab_DrawItemYahooMessengerLike);
        ///         ....
        /// }
        /// private void Tab_DrawItemYahooMessengerLike(object sender, DrawTabEventArgs drawItemInfo)
        /// {
        ///     // Draw the default background and interior in all cases.
        ///     drawItemInfo.DrawBackground();
        ///     drawItemInfo.DrawInterior();
        /// 
        ///     // The border should be drawn only when the item is selected or highlighted.
        ///     if(((int)drawItemInfo.State &amp; ((int)DrawItemState.Selected | (int)DrawItemState.HotLight)) > 0)
        ///     {
        ///         // Draw the borders
        ///         drawItemInfo.DrawBorders();
        ///     }
        /// }
        /// </code>
        /// <code lang="VB">
        /// ' Make sure to set the "3D" tab style, turn on the HotTrack property and handle
        /// ' the DrawItem event of the tab control.
        /// Private Sub InitializeComponent()
        ///      Me.TabControlAdv1.HotTrack = True
        ///      Me.TabControlAdv1.TabStyle = GetType(Syncfusion.Windows.Forms.Tools.TabRenderer3D)
        ///         Me.TabControlAdv1.DrawItem += New Syncfusion.Windows.Forms.Tools.DrawTabEventHandler(Me.Tab_DrawItemYahooMessengerLike)
        ///     End Sub 'InitializeComponent
        /// 
        ///     Private Sub Tab_DrawItemYahooMessengerLike(sender As Object, drawItemInfo As DrawTabEventArgs)
        ///         ' Draw the default background and interior in all cases.
        ///         drawItemInfo.DrawBackground()
        ///         drawItemInfo.DrawInterior()
        /// 
        ///         ' The border should be drawn only when the item is selected or highlighted.
        ///         If(CInt(drawItemInfo.State) And(CInt(DrawItemState.Selected) Or CInt(DrawItemState.HotLight))) > 0 Then
        ///             ' Draw the borders
        ///             drawItemInfo.DrawBorders()
        ///         End If
        ///     End Sub 'Tab_DrawItemYahooMessengerLike
        /// </code>
        /// </example>
        [
        Category(@"Action"),
        Description(@"Occurs whenever a particular item/area needs to be painted.")
        ]
        public event DrawTabEventHandler DrawItem;

        /// <summary>
        /// Occurs when the SelectedIndex property is changed.
        /// </summary>
        [
        Description(@"Occurs whenever the 'SelectedIndex' property for this control changes."),
        Category(@"Action")
        ]
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Occurs before the SelectedIndex property gets changed to let you cancel the new selection.
        /// </summary>
        [
        Description(@"Occurs before the 'SelectedIndex' property for this control changes."),
        Category(@"Action")
        ]
        public event SelectedIndexChangingEventHandler SelectedIndexChanging;
        #endregion EVENTS

        #region INIT
        private bool m_bIsInitializing = false;
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool IsInitializing
        {
            get
            {
                return m_bIsInitializing;
            }
        }

        void System.ComponentModel.ISupportInitialize.BeginInit()
        {
            this.BeginInit();
            m_bIsInitializing = true;
        }

        void System.ComponentModel.ISupportInitialize.EndInit()
        {
            m_bIsInitializing = false;

            Type vs2005StyleType = typeof(TabRendererWhidbey);
            Type oneNoteStyleType = typeof(OneNoteStyleRenderer);
            Type dockingWhidbeyTypeBeta = typeof(TabRendererDockingWhidbeyBeta);
            Type dockingWhidbeyType = typeof(TabRendererDockingWhidbey);
            Type VS2008Type = typeof(TabRendererVS2008);
            Type TabRendererBlendDark = typeof(TabRendererBlendDark);
            Type TabRendererBlendLight = typeof(TabRendererBlendLight);

            if (this.TabStyle != oneNoteStyleType &&
                this.TabStyle != vs2005StyleType &&
                this.TabStyle != dockingWhidbeyTypeBeta &&
                this.TabStyle != dockingWhidbeyType &&
                this.TabStyle != VS2008Type && !this.RotateTabsWhenRTL)
            {
                throw new ArgumentException("Only vs2005Style, oneNoteStyle, dockingWhidbey and dockingWhidbeyBeta styles support disabled RotateTabsWhenRTL propety");
            }

            Color borderColor = this.GetRendererBorderColor();
            if (borderColor != Color.Empty)
            {
                m_borderColor = borderColor;
            }

            this.EndInit();
        }

        protected virtual void BeginInit()
        {
        }

        protected virtual void EndInit()
        {
            if (this.Parent != null)
                this.Parent.MouseDown += new MouseEventHandler(Parent_MouseDown);

            if (PersistTabState)
            {
                LoadState();
            }

            m_office2007ColorTable = Office2007Colors.GetColorTable(m_Office2007ColorScheme);
        }

        /// <summary>
        /// Indicates whether the Layout method needs to be called to layout the TabControlAdv
        /// elements.
        /// </summary>
        /// <remarks>
        /// Internal method. You will not have to call or override this method explicitly.
        /// </remarks>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        protected virtual bool NeedLayout
        {
            get
            {
                bool result = m_needLayout;
                if (this.m_tabPanelRenderer != null)
                    result = result | this.m_tabPanelRenderer.NeedLayout;
                return result;
            }
        }

        /// <summary>
        /// Returns the current <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/>
        /// used by the tab control to render the tab panel.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITabPanelRenderer Renderer
        {
            get { return this.m_tabPanelRenderer; }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal virtual TabPanelData TabPanelData
        {
            get { return this.m_tabPanelData; }
            set
            {
                if (this.m_tabPanelData != value)
                {
                    if (this.m_tabPanelData != null)
                    {
                        this.m_tabPanelData.PropertyChanged -= new TabPanelPropertyChangedEventHandler(this.TabPanel_PropertyChanged);
                        this.m_tabPanelData.SelectedIndexChanging -= new SelectedIndexChangingEventHandler(this.TabPanel_SelectedIndexChanging);
                    }

                    this.m_tabPanelData = value;

                    if (this.m_tabPanelData != null)
                    {
                        this.m_tabPanelData.PropertyChanged += new TabPanelPropertyChangedEventHandler(this.TabPanel_PropertyChanged);
                        this.m_tabPanelData.SelectedIndexChanging += new SelectedIndexChangingEventHandler(this.TabPanel_SelectedIndexChanging);
                    }
                }
            }
        }

        /// <summary>
        /// Forces the laying out of tab control elements within the next Paint Message handler.
        /// </summary>
        /// <param name="value">True to force; false to prevent layout.</param>
        protected internal void SetNeedLayout(bool value)
        {
            m_needLayout = value;
        }
        /// <summary>
        /// Returns the collection of tab pages in this tab control.
        /// </summary>
        /// <value>
        /// A <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdvCollection"/> that contains the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv"/>
        /// objects in this TabControlAdv.
        /// </value>
        [
        MergableProperty(false),
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description(@"The TabPages in the TabControl.")
        ]
        public virtual TabPageAdvCollection TabPages
        {
            get { return this.m_tabPagesCollection; }
        }

        /// <summary>
        /// Returns the number of tabs in the tab strip.
        /// </summary>
        /// <value>The number of tabs in the tab strip.</value>
        [
        Description("Gets the number of tabs in the tab strip."),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int TabCount
        {
            get { return this.m_tabPanelData.TabsData.Count; }
        }

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// default item size
        /// </summary>
        private static Size ITMSIZE = default(Size);
        /// <summary>
        /// 
        /// </summary>
        private static Size SCROLLSIZE = default(Size);


        /// <summary>
        /// Initializes a new instance of the TabControl class.
        /// </summary>
        /// <example>
        ///  The following example creates a TabControlAdv with one TabPageAdv object. 
        ///  The constructor instantiates tabControl1.
        ///  Use the Syncfusion.Windows.Forms.Tools namespace for this example.
        /// <code lang="C#">
        /// public Form1()
        /// {
        ///		this.tabPage1 = new TabPageAdv();
        ///		// Invokes the TabControlAdv() constructor to create the tabControl1 object.
        ///		this.tabControl1 = new TabControlAdv();
        ///		
        ///		this.tabControl1.Controls.Add(tabPage1);
        ///		this.Controls.Add(tabControl1);
        ///	}
        /// </code>
        /// <code lang="VB">
        /// Public Sub New()
        ///		Me.tabPage1 = New TabPageAdv()
        ///		' Invokes the TabControlAdv() constructor to create the tabControl1 object.
        ///		Me.tabControl1 = New TabControlAdv()
        ///		Me.tabControl1.Controls.Add(tabPage1)
        ///		Me.Controls.Add(tabControl1)
        /// End Sub 'New
        /// </code>
        /// </example>
        public TabControlAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TabControlAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            Init();
           
        }

        /// <summary>
        /// Handles the MouseDown event of the Parent of TabControlAdv.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void Parent_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.IsEditing)
            {
                //	m_prevSelectedIndex = int.MinValue;
                m_tabPageText = this.SelectedTab.Text;

                if (m_tabPageText != m_txtLabel.Text)
                    EndLabelEdit(true);
                else
                    EndLabelEdit(false);
            }
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static TabControlAdv()
        {
            // So that the renderers will be registered and available.
            TabRenderer2D.RegisterTabType();
            TabRenderer3D.RegisterTabType();
            TabRendererWorkbookMode.RegisterTabType();
            OneNoteStyleRenderer.RegisterTabType();
            TabRendererOffice2003.RegisterTabType();
            TabRendererWhidbey.RegisterTabType();
            TabRendererIE7.RegisterTabType();
            TabRendererOffice2007.RegisterTabType();
            TabRendererOffice2010.RegisterTabType();
            TabRendererDockingWhidbeyBeta.RegisterTabType();
            TabRendererDockingWhidbey.RegisterTabType();
            TabRendererVS2008.RegisterTabType();
            TabRendererBlendDark.RegisterTabType();
            TabRendererBlendLight.RegisterTabType();
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.WndProc"/>.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x031A/*WM_THEMECHANGED*/)
            {
                if (this.m_themedDrawing != null)
                {
                    this.SetNeedLayout(true);
                }

                WindowsXPThemeColors.UpdateColors();

                Color borderColor = this.GetRendererBorderColor();
                if (borderColor != Color.Empty)
                {
                    m_borderColor = borderColor;
                }
                this.UpdateScrollButtonsStyle();
            }

            base.WndProc(ref m);
        }


        /// <summary>
        /// Called by the constructor to initialize default properties of the tab control.
        /// </summary>
        /// <remarks>
        /// Advanced method. You do not have to call this directly.
        /// </remarks>
        protected virtual void Init()
        {
            m_tabPrimitivesHost = new TabPrimitivesHost(this);

            if (this.TabPanelData == null)
                this.TabPanelData = new TabPanelData(this);

            // Borders need to be redrawn on resize, so including this setting
            SetStyle(ControlStyles.ResizeRedraw, true);

            // When the control gets drawn the very first time, we want D-B to be turned off,
            // otherwise, the tabs get draw a little later than the tab pages which doesn't look good for aesthetic reasons.
            // We will however turn it on, in OnVisibleChanged
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, false);

            // This style was added to perform container validation
            // (Fixed defect 12096)
            SetStyle(ControlStyles.ContainerControl, true);

            if (this.m_tabPanelDefaultProperties == null)
                this.m_tabPanelDefaultProperties = new TabPanelDefaultPropertiesReflected();

            RendererChanged(null);

            this.m_tabPagesCollection = new TabPageAdvCollection(this.m_tabPanelData, this, this.m_tabPanelDefaultProperties);

            this.ControlAdded += new ControlEventHandler(TabControlAdv_ControlAdded);
            this.ControlRemoved += new ControlEventHandler(TabControlAdv_ControlRemoved);

            this.Size = new Size(200, 100);

            if (XPThemes.IsThemedOS)
                this.m_themedDrawing = new ThemedTabDrawing(this, ThemedControls.TAB);

            m_tabControlAdvWeakContainer = new TabControlAdvWeakContainer(this);
            // The renderer might use the Office2003 colors.
            Office2003Colors.MenuColorsChanged += new EventHandler(this.m_tabControlAdvWeakContainer.Office2003ColorsChangedWeakEventHandler);
            SetNeedLayout(true);

            m_sbScrollButtons = new ScrollButtons();
            m_sbScrollButtons.Visible = false;
            base.Controls.Add(m_sbScrollButtons);

            Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007Colors_ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010Colors_ManagedColorsApplied);
            SCROLLSIZE = new Size(30,15);
            CTRLSIZE = this.Size;
            ITMSIZE = new Size(79,22);
        }

        /// <summary>
        /// Removes all the tab pages and additional controls from this tab control.
        /// </summary>
        /// <remarks>All controls are removed through the Controls property.</remarks>
        public void RemoveAll()
        {
                this.m_tabPanelData.TabsData.Clear();
            
        }
        /// <summary>
        /// Brings the selected tab to view, if scrolled out of view.
        /// </summary>
        public void BringSelectedTabToView()
        {
            if (this.m_tabPanelRenderer != null)
                this.m_tabPanelRenderer.ValidateScrollOffset(true, true);
        }
        /// <summary>
        /// Validates that the currently selected tab is not disabled or invisible.
        /// </summary>
        /// <remarks>
        /// This method ensures that the currently selected tab is not disabled or invisible. If so,
        /// it would reset the SelectedIndex to a new tab page that is selectable. If no selectable
        /// tab pages are found, then this method will do nothing.
        /// </remarks>
        public void ValidateSelectedIndex()
        {
            if (this.SelectedTab != null && (!this.SelectedTab.TabEnabled || !this.SelectedTab.TabVisible))
            {
                int tabCount = this.TabCount;

                // Move to the next tab
                int selectedIndex = this.SelectedIndex;
                bool selectableTabAvailable = false;

                while (!selectableTabAvailable)
                {
                    selectedIndex = ((selectedIndex + 1) % tabCount);

                    if (selectedIndex == this.SelectedIndex)
                        break;

                    if (this.TabPanelData.IsTabSelectable(selectedIndex, true))
                        selectableTabAvailable = true;
                }

                if (selectableTabAvailable)
                    this.SelectedIndex = selectedIndex;
            }
        }
        #endregion INIT

        internal ValidateStatus ValidateStatus
        {
            get { return m_validateStatus; }
        }
        /// <summary>
        /// Returns the current <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelDefaultProperties"/>
        /// used by the tab control to render the tab panel.
        /// </summary>
        protected ITabPanelDefaultProperties CurDefaultTabPanelProperties
        {
            get { return this.m_tabPanelDefaultProperties; }
        }

        internal class TabPanelDefaultPropertiesReflected : ITabPanelDefaultProperties
        {
            ITabPanelDefaultProperties tabPanelDefaultProperties;
            internal ITabPanelDefaultProperties TabPanelDefaultProperties
            {
                set { this.tabPanelDefaultProperties = value; }
            }

            public System.Drawing.Color DefaultActiveTabColor()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultActiveTabColor();
                return Color.Empty;
            }

            public System.Drawing.Font DefaultActiveTabFont()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultActiveTabFont();
                return Control.DefaultFont;
            }

            public System.Drawing.Color DefaultInactiveTabColor()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultInactiveTabColor();
                return Color.Empty;
            }

            public System.Drawing.Color DefaultTabForeColor()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultTabForeColor();
                return SystemColors.WindowText;
            }

            public System.Drawing.Color DefaultFixedSingleBorderColor()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultFixedSingleBorderColor();
                return Color.Empty;
            }

            public System.Drawing.Font DefaultInactiveTabFont()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultInactiveTabFont();
                return Control.DefaultFont;
            }

            public System.Drawing.Color DefaultTabPanelBackgroundColor()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultTabPanelBackgroundColor();
                return Color.Empty;
            }

            public System.Drawing.Font DefaultTabPanelFont()
            {
                if (this.tabPanelDefaultProperties != null)
                    return this.tabPanelDefaultProperties.DefaultTabPanelFont();
                return Control.DefaultFont;
            }
        }

        internal Office2007Colors Office2007ColorTable
        {
            get
            {
                if (m_office2007ColorTable == null)
                {
                    m_office2007ColorTable = Office2007Colors.GetColorTable(this.Office2007ColorScheme);
                }
                return m_office2007ColorTable;
            }
        }
        internal Office2010Colors Office2010ColorTable
        {
            get
            {
                if (m_office2010ColorTable == null)
                {
                    m_office2010ColorTable = Office2010Colors.GetColorTable(this.Office2010ColorTheme);
                }
                return m_office2010ColorTable;
            }
        }
        /// <summary>
        /// The value that indicates close button BackColor.
        /// </summary>
        [
         Category("Appearance"),
         Description("Specifies whether close button BackColor.")
        ]
        private Color closeButtonBackColor = Color.White;

        /// <summary>
        /// Gets or sets the value whether close button BackColor.
        /// </summary>
        public Color CloseButtonBackColor
        {
            get
            {
                return closeButtonBackColor;
            }
            set
            {
                if (value != closeButtonBackColor)
                {
                    closeButtonBackColor = value;
                    OnShowCloseButtonChanged();
                }
            }
        }

        /// <summary>
        /// The value that indicates whether close button BackColor should be visible for each tab.
        /// </summary>
        [
         Category("Appearance"),
         DefaultValue(false),
         Description("Specifies whether close button BackColor should be visible for each tab.")
        ]
        private bool m_bShowCloseButtonBackColor = false;

        /// <summary>
        /// Gets or sets the value whether close button BackColor should be visible for each tab.
        /// </summary>
        public bool ShowCloseButtonHighLightBackColor
        {
            get
            {
                return m_bShowCloseButtonBackColor;
            }
            set
            {
                if (value != m_bShowCloseButtonBackColor)
                {
                    m_bShowCloseButtonBackColor = value;
                   OnShowCloseButtonChanged();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value whether close button should be visible for tab only if mouse is over it.
        /// This property will work only if <see cref="TabControlAdv.ShowCloseButton" /> property is set to true.
        /// </summary>
        [
         Category("Appearance"),
         DefaultValue(false),
         Description("Specifies whether close button should be visible for tab only if mouse is over it.")
        ]
        public bool ShowCloseButtonForActiveTabOnly
        {
            get
            {
                return m_bShowCloseButtonForActiveTabOnly;
            }
            set
            {
                if (value != m_bShowCloseButtonForActiveTabOnly)
                {
                    m_bShowCloseButtonForActiveTabOnly = value;

                    OnShowCloseButtonChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value, whether close button should be visible for each tab.
        /// </summary>
        [
         Category("Appearance"),
         DefaultValue(false),
         Description("Specifies whether close button should be visible for each tab.")
        ]
        public bool ShowTabCloseButton
        {
            get
            {
                return m_bShowCloseButton;
            }
            set
            {
                if (m_bShowCloseButton != value)
                {
                    m_bShowCloseButton = value;

                    OnShowCloseButtonChanged();
                }
            }
        }

        /// <summary>
        /// Called when ShowCloseButton property is changed.
        /// </summary>
        internal virtual void OnShowCloseButtonChanged()
        {
            if (this.Renderer != null)
            {
                ArrayList arrRenderers = this.Renderer.Renderers;

                if (arrRenderers != null && arrRenderers.Count > 0)
                {
                    TabRendererBase renderer = null;

                    for (int i = 0, len = arrRenderers.Count; i < len; i++)
                    {
                        renderer = arrRenderers[i] as TabRendererBase;

                        if (renderer != null)
                        {
							if (this.ShowTabCloseButton)
							{
                                if (m_tabPagesCollection.Count > i)
                                {
                                    if ((m_tabPagesCollection[i].Parent as MDITabPanel) != null)
                                    {
                                        bool formCloseButtonVisible = false;
                                        foreach (Form form in (this as MDITabPanel).m_MDIManager.childForms)
                                        {
                                            if (m_tabPagesCollection[i].Tag == form)
                                            {
                                                renderer.ShowCloseButton = m_tabPagesCollection[i].ShowCloseButton;
                                                formCloseButtonVisible = true;
                                            }
                                        }
                                        renderer.ShowCloseButton = formCloseButtonVisible;
                                    }
                                    if (this.ShowCloseButtonForActiveTabOnly)
                                    {
                                        if(this.SelectedIndex == i)
                                            renderer.ShowCloseButton = true;
                                        else
                                            renderer.ShowCloseButton = false;
                                    }
                                    else
                                    {
                                        renderer.ShowCloseButton = m_tabPagesCollection[i].ShowCloseButton;
                                        renderer.ShowCloseButtonBackColor = ShowCloseButtonHighLightBackColor;
                                        renderer.CloseButtonBackColor = CloseButtonBackColor;
                                    }
                                }
							}
							else
								renderer.ShowCloseButton = false;
                        }
                    }

                    ComputeTabPositions();
                    ComputeTabPanelBounds();
                    InvalidatePanel();
                }
            }
        }

        /// <summary>
        /// Indicates whether the close button should be visible accordingly to the
        /// <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ShowCloseButtonForActiveTabOnly"/> property.
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool ShouldDrawCloseButton(int pTabIndex)
        {
            bool bShouldDraw = m_bShowCloseButton;

            if (m_bShowCloseButtonForActiveTabOnly)
            {
                bShouldDraw = bShouldDraw && (this.SelectedIndex == pTabIndex);
            }

            return bShouldDraw;
        }
        /// <summary>
        /// Indicates whether the close button should be visible accordingly to the
        /// <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ShowCloseButtonForActiveTabOnly"/> property.
        /// </summary>
        protected internal bool ShouldDrawCloseButton(int pTabIndex,bool close)
        {
            bool bShouldDraw = close;

            if (m_bShowCloseButtonForActiveTabOnly)
            {
                bShouldDraw = bShouldDraw && (this.SelectedIndex == pTabIndex);
            }
            return bShouldDraw;
        }
        /// <summary>
        /// Sets the visibility of the close button.
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="bShowCloseButton">Value that indicates the visibility of the close button.</param>
        private void SetCloseButtonForTab(int tabIndex, bool bShowCloseButton)
        {
            if (this.Renderer != null && this.Renderer.Renderers != null &&
                tabIndex >= 0 && tabIndex < this.Renderer.Renderers.Count)
            {
                TabRendererBase tabPageRenderer = this.Renderer.Renderers[tabIndex] as TabRendererBase;

                if (tabPageRenderer != null)
                {
                    tabPageRenderer.ShowCloseButton = bShowCloseButton;
                }
            }
        }

        /// <summary>
        /// Updates the visibility of the close buttons on the tabs.
        /// </summary>
        /// <param name="mousePos">Position of the mouse.</param>
        private void UpdateCloseButtons(Point mousePos)
        {
            int activeTabIndex = this.HitTestTabs(mousePos);

            if (activeTabIndex != m_iPrevActiveTabIndex)
            {
                SetCloseButtonForTab(m_iPrevActiveTabIndex, false);
                SetCloseButtonForTab(activeTabIndex, true);

                m_iPrevActiveTabIndex = activeTabIndex;

                this.ComputeTabPositions();
                this.ComputeTabPanelBounds();
                InvalidatePanel();
            }
        }

        /// <summary>
        /// Updates the visibility of the close buttons on the tabs.
        /// </summary>
        /// <param name="newIndex">Tab index.</param>
        private void UpdateCloseButtons(int newIndex)
        {
            SetCloseButtonForTab(m_iPrevActiveTabIndex, false);
            SetCloseButtonForTab(newIndex, true);

            m_iPrevActiveTabIndex = newIndex;

            this.ComputeTabPositions();
            this.ComputeTabPanelBounds();
            InvalidatePanel();
        }

        /// <summary>
        /// Called when the tabStyle is changed.
        /// </summary>
        protected virtual void OnStyleChanged()
        {
            if (!m_bIsInitializing)
            {
                TabGap = (IsOffice2007Style || IsOffice2010Style) ? 10 : 0;

                if (ActiveTabFont == null)
                {
                    ActiveTabFont = m_tabPanelDefaultProperties.DefaultActiveTabFont();
                }

                if (IsIE7Style)
                {
                    TabPanelBackColor = Color.FromKnownColor(KnownColor.Control);
                    ActiveTabFont = FontUtil.CreateFont(ActiveTabFont, FontStyle.Regular);
                }
                else if (IsDockingWhidbeyStyle)
                {
                    TabPanelBackColor = StyleRendererPropertyDockingWhidbey.BackgroundColor;
                    ActiveTabFont = FontUtil.CreateFont(ActiveTabFont, FontStyle.Regular);
                }
                else if (this.TabStyle == typeof(TabRendererOffice2007))
                {
                    TabPanelBackColor = this.Office2007ColorTable.TabPanelBackColor;

                    ActiveTabFont = FontUtil.CreateFont(ActiveTabFont, FontStyle.Regular);
                }
                else if (this.TabStyle == typeof(TabRendererOffice2010))
                {
                    TabPanelBackColor = this.Office2010ColorTable.TabPanelBackColor;

                    ActiveTabFont = FontUtil.CreateFont(ActiveTabFont, FontStyle.Regular);
                }
                else if (IsDockingWhidbeyStyleBeta || IsWhidbeyStyle)
                {
                    TabPanelBackColor = Color.FromKnownColor(KnownColor.Control);
                    ActiveTabFont = FontUtil.CreateFont(ActiveTabFont, FontStyle.Bold);
                }
                else if (IsOneNoteStyle || IsOneNoteStyleFlatTabs || IsOffice2003Style)
                {
                    TabPanelBackColor = Office2003Colors.DockBarColorDark;
                    ActiveTabFont = FontUtil.CreateFont(ActiveTabFont, FontStyle.Bold);
                }
                else if (IsBlendDarkStyle)
                {
                    TabPanelBackColor = BlendDarkRendererProperty.BackgroundColor;

                }
                else if (IsBlendLightStyle)
                {
                    TabPanelBackColor = BlendLightRendererProperty.BackgroundColor;

                }
                else
                {
                    if (m_tabPanelData.TabStyle != TabRendererOffice2007.TabStyleName)
                    {
                        TabPanelBackColor = Color.Empty;
                    }

                    Padding = new Point(Padding.X, Padding.Y);
                    ActiveTabFont = m_tabPanelDefaultProperties.DefaultActiveTabFont();
                }

                UpdateScrollButtonsStyle();

                InvalidatePanel();

                Color borderColor = this.GetRendererBorderColor();
                if (borderColor != Color.Empty)
                {
                    m_borderColor = borderColor;
                }

                this.ComputeTabPositions();
            }
        }

        private bool m_bReserveTabSpace = false;
        private int m_reservedSpace = 0;

        /// <summary>
        /// Gets or sets the space to be reserved when no Tabs are present.
        /// </summary>
        [DefaultValue(0)]
        [Category("Appearance")]
        [Description("Indicates, reserve space for TabPage's or not, when there is no Tab pages. ")]
        public int ReservedSpace
        {
            get
            {
                return m_reservedSpace;
            }
            set
            {
                if (value != m_reservedSpace)
                {
                    if (value < 0 || value > this.Height)
                    {
                        throw new ArgumentOutOfRangeException("value");
                    }

                    m_reservedSpace = value;

                    ComputeTabPanelBounds();
                    InvalidatePanel();
                }
            }
        }

        /// <summary>
        /// Indicates whether space has been reserved for TabPage's when there are no Tab pages.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates, reserve space for TabPage's or not, when there is no Tab pages. ")]
        public bool ReserveTabSpace
        {
            get
            {
                return m_bReserveTabSpace;
            }
            set
            {
                if (value != m_bReserveTabSpace)
                {
                    m_bReserveTabSpace = value;
                    ComputeTabPanelBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Indicates whether the scroll buttons should be drawn with
        /// the Visual Studio MDI child tabs like flat look.
        /// </summary>
        /// <value>True for VS like scroll buttons; false otherwise. Default is false.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [DefaultValue(false),
        Category(@"Appearance"),
        Localizable(true),
        Description("Specifies whether the scroll buttons should be drawn in the Visual Studio MDI child tabs like flat look.")
        ]
        public bool VSLikeScrollButton
        {
            get { return this.m_vsLikeScrollButton; }
            set
            {
                if (this.m_vsLikeScrollButton != value)
                {
                    this.m_vsLikeScrollButton = value;
                    if (m_sbScrollButtons != null)
                        m_sbScrollButtons.VSLikeButton = this.m_vsLikeScrollButton;
                }
            }
        }
        /// <summary>
        /// Indicates whether to show or hide scroll buttons when there is not enough space for the tabs in single Line 
        /// mode.
        /// </summary>
        /// <value>True if scroll buttons are needed; false otherwise.
        /// Default value is true.</value>
        [
        Category(@"Appearance"),
        Description(@"Show or Hide Scroll Buttons when there is not enough space for the tabs in Single Line mode."),
        DefaultValue(true),
        Localizable(true),
        ]
        public virtual bool ShowScroll
        {
            get
            {
                return m_bShowScroll;
            }

            set
            {
                if (m_bShowScroll != value)
                {
                    m_bShowScroll = value;
                    m_bCachedShowScroll = value;

                    DestroyScrollButtons(false);

                    SetNeedLayout(true);
                    Invalidate(false);
                }
            }
        }

        /// <override/>
        protected override Size DefaultSize
        {
            get
            {
                return ((Size)(new Size(200, 100)));
            }
        }


        /// <summary>
        /// Gets a value indicating whether the tabStyle is docking whidbey style.
        /// </summary>
        protected bool IsDockingWhidbeyStyleBeta
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererDockingWhidbeyBeta));
            }
        }

        protected bool IsDockingWhidbeyStyle
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererDockingWhidbey));
            }
        }

        protected virtual bool IsOffice2007Style
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererOffice2007) && !this.ShouldDrawThemed);
            }
        }

        protected virtual bool IsOffice2010Style
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererOffice2010) && !this.ShouldDrawThemed);
            }
        }

        protected virtual bool IsMetroStyle
        {
            get
            {
                return this.TabStyle == typeof(TabRendererMetro) ;
            }
        }

        protected virtual bool IsVS2008
        {
            get
            {
                return this.TabStyle == typeof(TabRendererVS2008);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tabstyle is VS2010.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is V S2010; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool IsVS2010
        {
            get
            {
                return this.TabStyle == typeof(TabRendererVS2010);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the tabStyle is Whidbey style.
        /// </summary>
        protected virtual bool IsIE7Style
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererIE7) && !this.ShouldDrawThemed);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tabStyle is BlendDark style.
        /// </summary>
        protected virtual bool IsBlendDarkStyle
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererBlendDark) && !this.ShouldDrawThemed);
            }
        }
        /// <summary>
        /// Gets a value indicating whether the tabStyle is BlendLight style.
        /// </summary>
        protected virtual bool IsBlendLightStyle
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererBlendLight) && !this.ShouldDrawThemed);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tabStyle is Whidbey style.
        /// </summary>
        protected virtual bool IsWhidbeyStyle
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererWhidbey) && !this.ShouldDrawThemed);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tabStyle is Office2003 style.
        /// </summary>
        protected virtual bool IsOffice2003Style
        {
            get
            {
                return (this.TabStyle == typeof(TabRendererOffice2003) && !this.ShouldDrawThemed);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tabStyle is OneNote style.
        /// </summary>
        protected bool IsOneNoteStyle
        {
            get
            {
                return (this.TabStyle == typeof(OneNoteStyleRenderer));
            }
        }
        /// <summary>
        /// Gets a value indicating whether the tabStyle is OneNoteFlatTabs style.
        /// </summary>
        protected bool IsOneNoteStyleFlatTabs
        {
            get
            {
                return (this.TabStyle == typeof(OneNoteStyleFlatTabsRenderer));
            }
        }
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		internal bool IsMirrored
#else
        internal new bool IsMirrored
#endif
        {
            get
            {
                return m_bMirrored;
            }
            set
            {
                m_bMirrored = value;
            }
        }

        internal bool GetIsMirroredForVerticalAlignment()
        {
            bool bIsMirrored = IsMirrored;

            if (TabAlignment.Right == this.Alignment || TabAlignment.Left == this.Alignment)
            {
                if ((TabVerticalAlignment.Bottom == this.VerticalAlignment && !bIsMirrored) ||
                    (TabVerticalAlignment.Top == this.VerticalAlignment && bIsMirrored))
                {
                    bIsMirrored = !bIsMirrored;
                }
            }

            return bIsMirrored;
        }

        /// <summary>
        /// Gets a value indicating whether the tabStyle is TabRenderer2D style.
        /// </summary>
        protected bool Is2DStyle
        {
            get
            {
                return TabStyle == typeof(TabRenderer2D);
            }
        }

        /// <summary>
        /// Indicates whether host OS is Vista.
        /// </summary>
        internal bool IsVistaOS
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        #region TABPANEL_PROPERTIES_REFLECTED
        /// <summary>
        /// Indicates whether the tabs are painted as 2D, 3D(regular),
        /// WorkbookMode or other registered tab types.
        /// </summary>
        /// <value>A reference to a type that implements the ITabRenderer interface.</value>
        /// <remarks>
        /// There are 3 pre-built tab styles available, represented by the following classes
        /// in the Syncfusion.Windows.Forms.Tools namespace: TabRenderer2D (2D tabs),
        /// TabRenderer3D(3D tabs), TabRendererWorkbookMode (Workbook mode tabs).
        /// <para>This type-based TabStyle property allows you to implement custom tab types and
        /// plug them into the available TabStyles list of a TabControlAdv instance and specify them as the preferred TabStyle seemlessly.</para>
        /// </remarks>
        /// <example>
        /// <para>The following example creates a TabControlAdv with three TabPageAdv objects.
        /// This example sets the TabStyle property to 2D which displays the tabs of the
        /// tab pages in a flat/2D appearance.</para>
        /// <para>To define the dimensions of the tabs, set the ItemSize property equal to a
        /// Size structure. In this example, Size defines the tabs 90 pixels wide and
        /// 50 pixels high. You cannot change the width of the tabs unless the SizeMode
        /// property is set to Fixed.</para>
        /// <para>Use the System.Drawing and Syncfusion.Windows.Forms.Tools namspaces for this example.</para>
        /// <coderef file="\Tools\Samples\Tabs Package\XPTabs\CS\Form2_InitProgramatically.cs" name="Initailizing TabControlAdv properties" lang="C#"><code lang="C#">
        ///         private void InitMyTabs()
        ///         {
        ///             this.tabControl1 = new TabControlAdv();
        ///             this.tabPage1 = new TabPageAdv();
        ///             this.tabPage2 = new TabPageAdv();
        ///             this.tabPage3 = new TabPageAdv();
        /// 
        ///             // Positions tabs on the left side of tabControl1.
        /// //            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
        /// 
        ///             // Sets the tabs to appear in 2D mode.
        ///             tabControl1.TabStyle = typeof(TabRenderer2D);
        /// 
        ///             // Highlights TabPage.Text when the mouse passes over tabs.
        ///             this.tabControl1.HotTrack = true;
        /// 
        ///             // Set the relative alignment between the images and text in a tab
        ///             this.tabControl1.ImageAlignmentR = RelativeImageAlignment.BelowText;
        /// 
        ///             // Allows more than one row of tabs.
        ///             // this.tabControl1.Multiline = true;
        /// 
        ///             // Creates a cushion of 22 pixels around TabPage.Text strings.
        ///             this.tabControl1.Padding = new System.Drawing.Point(22, 22);
        /// 
        ///             // Makes the tab width definable.
        ///             this.tabControl1.SizeMode = Syncfusion.Windows.Forms.Tools.TabSizeMode.Fixed;
        /// 
        ///             // Sizes the tabs of tabControl1.
        ///             this.tabControl1.ItemSize = new Size(90, 64); // Make sure to take into account the padding values.
        /// 
        ///             // To rotate text when aligned vertically.
        ///             this.tabControl1.RotateTextWhenVertical = true;
        /// 
        ///             // Allows the user to move the tabs by simply dragging and dropping
        ///             this.tabControl1.UserMoveTabs = true;
        /// 
        ///             // Draws the scroll buttons Visual Studio MDI Tabs like.
        ///             this.tabControl1.VSLikeScrollButton = true;
        /// 
        ///             this.tabControl1.Controls.AddRange(new Control[] {
        ///                                                                  this.tabPage1,
        ///                                                                  this.tabPage2,
        ///                                                                  this.tabPage3});
        ///             this.tabControl1.Location = new Point(16, 24);
        ///             this.tabControl1.SelectedIndex = 0;
        ///             this.tabControl1.Size = new Size(248, 232);
        /// 
        ///             this.tabPage1.Text = "Tab1";
        ///             this.tabPage2.Text = "Tab2";
        ///             this.tabPage3.Text = "Tab3";
        /// 
        ///             this.Size = new Size(300,300);
        ///             this.Controls.AddRange(new Control[] {
        ///                                                      this.tabControl1});
        /// 
        ///             // Selects tabPage1 using SelectedIndex.
        ///             this.tabControl1.SelectedIndex = 1;
        /// 
        ///             // Shows ToolTipText when the mouse passes over tabs.
        ///             this.tabControl1.ShowToolTips = true;
        ///         }</code></coderef>
        /// <coderef file="\Tools\Samples\Tabs Package\XPTabs\VB\Form2_InitProgramatically.vb" name="Initailizing TabControlAdv properties" lang="VB"><code lang="VB">
        ///        Private Sub InitMyTabs()
        /// 
        ///            Me.tabControl1 = New TabControlAdv()
        ///            Me.tabPage1 = New TabPageAdv()
        ///            Me.tabPage2 = New TabPageAdv()
        ///            Me.tabPage3 = New TabPageAdv()
        ///            ' Positions tabs on the left side of tabControl1.
        ///            '            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
        ///            ' Sets the tabs to appear in 2D mode.
        ///            tabControl1.TabStyle = GetType(TabRenderer2D)
        ///            ' Highlights TabPage.Text when the mouse passes over tabs.
        ///            Me.tabControl1.HotTrack = True
        ///            ' Set the relative alignment between the images and text in a tab
        ///            Me.tabControl1.ImageAlignmentR = RelativeImageAlignment.BelowText
        ///            ' Allows more than one row of tabs.
        ///            ' this.tabControl1.Multiline = true;
        ///            ' Creates a cushion of 22 pixels around TabPage.Text strings.
        ///            Me.tabControl1.Padding = New System.Drawing.Point(22, 22)
        ///            ' Makes the tab width definable.
        ///            Me.tabControl1.SizeMode = Syncfusion.Windows.Forms.Tools.TabSizeMode.Fixed
        ///            ' Sizes the tabs of tabControl1.
        ///            Me.tabControl1.ItemSize = New Size(90, 64)
        ///            ' Make sure to take into account the padding values.
        ///            ' To rotate text when aligned vertically.
        ///            Me.tabControl1.RotateTextWhenVertical = True
        ///            ' Allows the user to move the tabs by simply dragging and dropping
        ///            Me.tabControl1.UserMoveTabs = True
        ///            ' Draws the scroll buttons Visual Studio MDI Tabs like.
        ///            Me.tabControl1.VSLikeScrollButton = True
        ///            Me.tabControl1.Controls.AddRange(New Control() {Me.tabPage1, Me.tabPage2, Me.tabPage3})
        ///            Me.tabControl1.Location = New Point(16, 24)
        ///            Me.tabControl1.SelectedIndex = 0
        ///            Me.tabControl1.Size = New Size(248, 232)
        ///            Me.tabPage1.Text = "Tab1"
        ///            Me.tabPage2.Text = "Tab2"
        ///            Me.tabPage3.Text = "Tab3"
        ///            Me.Size = New Size(300, 300)
        ///            Me.Controls.AddRange(New Control() {Me.tabControl1})
        ///            ' Selects tabPage1 using SelectedIndex.
        ///            Me.tabControl1.SelectedIndex = 1
        ///            ' Shows ToolTipText when the mouse passes over tabs.
        ///            Me.tabControl1.ShowToolTips = True
        /// 
        ///        End Sub</code></coderef>
        /// </example>
        [
        Description(@"Indicates whether the tabs are painted as 2D, 3D(regular), WorkbookMode or other registered tab types."),
        DefaultValue(typeof(TabRenderer3D)),
        Category(@"Appearance"),
        Editor(typeof(TabStyleEditor), typeof(System.Drawing.Design.UITypeEditor)),
        TypeConverter(typeof(TabStyleConverter)),
        ]
        public virtual Type TabStyle
        {
            get
            {
                return ReflectionHelper.GetTabTypeFromName(this.m_tabPanelData.TabStyle);
            }

            set
            {
                if (m_tabPanelData.TabStyle != ReflectionHelper.GetTabNameFromType(value))
                {
                    if (this.GetType() == typeof(TabControlAdv) && !IsValidRendererType(value))
                    {
                        throw new ArgumentException("Invalid Renderer used");
                    }

                    Type office2003StyleType = typeof(TabRendererOffice2003);
                    Type vs2005StyleType = typeof(TabRendererWhidbey);

                    Type oneNoteStyleType = typeof(OneNoteStyleRenderer);
                    Type dockingWhidbeyTypeBeta = typeof(TabRendererDockingWhidbeyBeta);
                    Type dockingWhidbeyType = typeof(TabRendererDockingWhidbey);
                    Type VS2008Type = typeof(TabRendererVS2008);
                    Type TabRendererBlendDark = typeof(TabRendererBlendDark);
                    Type TabRendererBlendLight = typeof(TabRendererBlendLight);

                    if (value != oneNoteStyleType &&
                    value != vs2005StyleType &&
                    value != dockingWhidbeyTypeBeta &&
                    value != dockingWhidbeyType &&
                    value != VS2008Type && !this.RotateTabsWhenRTL)
                    {
                        if (this.DesignMode)
                        {
                            throw new ArgumentException("Only vs2005Style, oneNoteStyle, dockingWhidbey and dockingWhidbeyType2 styles support disabled RotateTabsWhenRTL propety");
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (this.ThemesEnabled && (value == office2003StyleType || value == vs2005StyleType))
                    {
                        if (this.DesignMode)
                        {
                            throw new ArgumentException("With Office2003 or VS2005 styles Themes must be disabled", "TabStyle");
                        }
                        else
                        {
                            return;
                        }
                    }

                    // Get the tabstyle from TabStyleName static property of the type (not the factory).
                    this.m_tabPanelData.TabStyle = ReflectionHelper.GetTabNameFromType(value);

                    if (value == typeof(OneNoteStyleRenderer) || value == office2003StyleType)
                    {
                        this.TextAlignment = StringAlignment.Near;
                    }
                    else
                    {
                        this.TextAlignment = StringAlignment.Center;
                    }

                    OnStyleChanged();
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal static bool IsValidRendererType(Type rendererType)
        {
            bool bIsValid = true;

            if (rendererType == null)
            {
                bIsValid = false;
            }
            else
            {
                Type groupRendererInterface = rendererType.GetInterface(typeof(ITabGroupRenderer).FullName);

                if (groupRendererInterface != null)
                {
                    bIsValid = false;
                }
            }
            return bIsValid;
        }

        /// <summary>
        /// Gets or sets the border style for the tab control.
        /// </summary>
        [
        DefaultValue(BorderStyle.Fixed3D),
        Category(@"Appearance"),
        Description("Specifies the border style for the tab control.")
        ]
        public virtual BorderStyle BorderStyle
        {
            get
            {
                return this.m_tabPanelData.BorderStyle;
            }

            set
            {
                if (this.BorderStyle != value)
                {
                    this.m_tabPanelData.BorderStyle = value;
                    this.SetNeedLayout(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color when the BorderStyle is FixedSingle.
        /// </summary>
        [
        Category(@"Appearance"),
        Description("Specifies the border color when the BorderStyle is FixedSingle")
        ]
        public virtual Color FixedSingleBorderColor
        {
            get
            {
                Color color = this.m_tabPanelData.FixedSingleBorderColor;
                if (color == Color.Empty)
                    return this.m_tabPanelDefaultProperties.DefaultFixedSingleBorderColor();
                else
                    return color;
            }

            set
            {
                this.m_tabPanelData.FixedSingleBorderColor = value;
            }
        }

        /// <summary>
        /// Specifies a value indicating whether fixed single border color need to be serialized.
        /// </summary>
        /// <returns></returns>
        [Browsable(false),
        Documentation.DocumentationExclude()]
        public bool ShouldSerializeFixedSingleBorderColor()
        {
            return (this.FixedSingleBorderColor != this.m_tabPanelDefaultProperties.DefaultFixedSingleBorderColor());
        }

        /// <summary>
        /// Resets the fixed single border color to it's default value.
        /// </summary>
        [Browsable(false),
        Documentation.DocumentationExclude()]
        public void ResetFixedSingleBorderColor()
        {
            this.m_tabPanelData.FixedSingleBorderColor = Color.Empty;
        }

        /// <summary>
        /// Gets or sets the area of the control (for example, along the top) where the tabs are aligned.
        /// </summary>
        /// <value>One of the TabAlignment values. The default is Top.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"Determines whether the tabs appear on the top, bottom, left, or right side of the Control."),
        Category(@"Appearance"),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(TabAlignment.Top)
        ]
        public virtual TabAlignment Alignment
        {
            get
            {
                return this.m_tabPanelData.Alignment;
            }
            set
            {
                this.m_tabPanelData.Alignment = value;

                this.SetRegion();

                if (this.IsHandleCreated)
                {
                    ComputeTabPositions();
                }

                this.UpdateScrollButtonsStyle();
                this.PerformLayout(this, "Bounds");
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void ComputeTabPositions()
        {
            TabPanelRenderer panelRenderer = this.Renderer as TabPanelRenderer;
            if (panelRenderer != null)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    panelRenderer.ComputeTabPositions(g);
                }
            }
        }

        /// <summary>
        /// Indicates whether tabs are aligned to the top, bottom or based on the RightToLeft property when aligned vertically. 
        /// </summary>
        /// <value>One of the TabVerticalAlignment values. The default is Default.</value>
        /// <remarks>
        /// <para>This property can be used to force the tabs to align to the top or bottom of the control irrespective
        /// of the RightToLeft setting, when aligned vertically.
        /// </para>
        /// </remarks>
        [
            Description(@"Specifies whether the tabs are vertically aligned to the top, bottom, or based on the RightToLeft property
when aligned vertically."),
            Browsable(true),
            Category(@"Appearance"),
            Localizable(true),
            RefreshProperties(RefreshProperties.Repaint),
            DefaultValue(TabVerticalAlignment.Default)
        ]
        public virtual TabVerticalAlignment VerticalAlignment
        {
            get
            {
                return this.m_tabPanelData.VerticalAlignment;
            }
            set
            {
                this.m_tabPanelData.VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.BackgroundImage"/>.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false),
        ]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                if (base.BackgroundImage != value)
                {
                    base.BackgroundImage = value;
                }
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.BackgroundImageLayout"/>.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false),
        ]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }
#endif


        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.ForeColor"/>.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; return; }
        }

        /// <summary>
        /// Gets or sets the size of the control's tabs.
        /// </summary>
        /// <value>A Size object that represents the size of the
        /// tabs. The default automatically sizes the tabs to fit
        /// the icons and labels on the tabs.</value>
        /// <remarks>
        /// To change the Width of the tab, the SizeMode property must be set to Fixed.
        /// The Height however will be set irrespective of the SizeMode.
        /// </remarks>
        /// <example>
        /// </example>
        [
        Description(@"Gets or sets the size of the control's tabs."),
        Localizable(true),
        Category(@"Appearance")
        ]
        public virtual Size ItemSize
        {
            get
            {
                Size size = Size.Empty;
                if (m_tabPanelData != null)
                {
                    size = m_tabPanelData.TabSize.ToSize();

                if (size == Size.Empty)
                {
                    size = GetItemPreferredSize();
                }
				}
                return size;
            }
            set
            {
                if (value.Width < 0 || value.Height < 0)
                {
                    throw new ArgumentOutOfRangeException("Size of the TabControlAdv's tab pages can't be negative.");
                }
                this.m_tabPanelData.TabSize = value;
            }
        }

        bool ITabControl.ThemesEnabled
        {
            get { return this.m_themesEnabled; }
        }

        ThemedTabDrawing ITabControl.ThemedDrawing
        {
            get { return this.m_themedDrawing; }
        }
        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ItemSize"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        protected virtual bool ShouldSerializeItemSize()
        {
            return !m_tabPanelData.TabSize.IsEmpty;
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ItemSize"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        public virtual void ResetItemSize()
        {
            m_tabPanelData.TabSize = Size.Empty;
        }

        /// <summary>
        /// Gets the preferred size of the item.
        /// </summary>
        /// <returns></returns>
        private Size GetItemPreferredSize()
        {
            Size size = Size.Empty;
            if (m_tabPanelRenderer != null && m_tabPanelRenderer.Renderers != null
                && this.SelectedIndex >= 0 && this.SelectedIndex < m_tabPanelRenderer.Renderers.Count)
            {
                ITabRenderer renderer = m_tabPanelRenderer.Renderers[SelectedIndex] as ITabRenderer;
                if (renderer != null && this.IsHandleCreated)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        size = renderer.GetPreferredSize(g).ToSize();
                    }
                }
            }
            return size;
        }

        /// <summary>
        /// Gets or sets the space between tabs in Single Line Mode.
        /// </summary>
        /// <value>The space between the tabs in pixels. Default value is zero.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"Specifies the space between tabs in Single Line Mode."),
        Category(@"Appearance"),
        DefaultValue(0),
        Localizable(true),
        ]
        public virtual int TabGap
        {
            get { return this.m_tabPanelData.TabGap; }
            set { this.m_tabPanelData.TabGap = value; }
        }

        /// <summary>
        /// Indicates whether more than one row of tabs can be displayed.
        /// </summary>
        /// <value>True if more than one row of tabs can be displayed; false otherwise. The default is false.</value>
        /// <remarks>If Multiline is false, only one row of tabs 
        /// is displayed - even if all the tabs do not fit in the 
        /// available space. In that case, however, scroll buttons 
        /// are displayed that allow the user to navigate to the 
        /// undisplayed tabs. <para>If the Multiline property is changed 
        /// to true while the SizeMode property is set to ShrinkToFit,
        /// the SizeMode property is automatically reset to 
        /// Normal.</para></remarks>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"Indicates if more than one row of tabs is allowed."),
        Category(@"Appearance"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(false),
        Localizable(true),
        ]
        public virtual bool Multiline
        {
            get { return this.m_tabPanelData.Multiline; }
            set
            {
                this.m_tabPanelData.Multiline = value;

                if (this.m_tabPanelData.Multiline && this.SizeMode == TabSizeMode.ShrinkToFit)
                {
                    if (this.DesignMode)
                        MessageBox.Show("TabSizeMode will now be set to TabSizeMode.Normal.", "Auto Property Reset");
                    this.SizeMode = TabSizeMode.Normal;
                }
            }
        }

        /// <summary>
        /// Indicates whether the selected tab should be moved to the front row when in multiline mode.
        /// </summary>
        /// <value>
        /// True to move to front row; false otherwise.
        /// </value>
        [
        Category(@"Behavior"),
        DefaultValue(true),
        Description("Specifies whether the selected tab should be moved to the front row when in multiline mode.")
        ]
        public virtual bool KeepSelectedTabInFrontRow
        {
            get { return this.m_tabPanelData.KeepSelectedTabInFrontRow; }
            set
            {
                this.m_tabPanelData.KeepSelectedTabInFrontRow = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode on how tabs are sized.
        /// </summary>
        /// <value>One of the <see cref="TabSizeMode"/> values. The default is Normal.</value>
        /// <example>
        /// Take a look at <see cref="TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Category(@"Behavior"),
        Description(@"Indicates how tabs are sized."),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(TabSizeMode.Normal)
        ]
        public virtual TabSizeMode SizeMode
        {
            get { return this.m_tabPanelData.SizeMode; }
            set
            {
                if (value == TabSizeMode.ShrinkToFit && this.Multiline)
                {
                    if (this.DesignMode)
                        MessageBox.Show("The Multiline Property will now be set to false.", "Auto Property Reset");
                    this.Multiline = false;
                }
                this.m_tabPanelData.SizeMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the extra space that should be added around the text or image in the tab.
        /// </summary>
        /// <value>A Point structure representing the padding along the
        /// X and Y directions in pixels.</value>
        /// <example>
        /// Take a look at <see cref="TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"Indicates how much extra space should be added around the text or image in the tab."),
        Category(@"Appearance"),
        Localizable(true)
        ]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public virtual Point Padding 
#else
        public new virtual Point Padding
#endif
        {
            get { return this.m_tabPanelData.Padding; }
            set
            {
                if (value.X < 0 || value.Y < 0)
                    throw new ArgumentException("Value cannot be negative.");
                this.m_tabPanelData.Padding = value;

                if (this.IsHandleCreated)
                {
                    this.ComputeTabPositions();
                }
            }
        }

        /// <summary>
        /// Indicates whether he current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.Padding"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        protected virtual bool ShouldSerializePadding()
        {
            return ((this.Padding).Equals((object)TabControlAdv.DEFAULT_PADDING) == false);
        }

        /// <summary>
        /// Gets or sets the background color of the tab panel and tabs. The tab's color will be overriden by individual Tab BackColor in the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv"/> instance, if any.
        /// </summary>
        /// <value>The Color value.</value>
        [
        Category(@"Appearance"),
        Description("Background Color of the tab panel and tabs. The tab's Color will be overriden by individual Tab BackColor, if any."),
        ]
        public virtual Color TabPanelBackColor
        {
            get
            {
                Color color = this.m_tabPanelData.BackColor;
                if (color == Color.Empty)
                    return this.m_tabPanelDefaultProperties.DefaultTabPanelBackgroundColor();
                else
                    return color;
            }
            set
            {
                this.m_tabPanelData.BackColor = value;

                UpdateScrollButtonsStyle();
                InvalidatePanel();
            }
        }

        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabPanelBackColor"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        protected virtual bool ShouldSerializeTabPanelBackColor()
        {
            return (this.TabPanelBackColor != this.m_tabPanelDefaultProperties.DefaultTabPanelBackgroundColor());
        }
        protected virtual bool ShouldSerializeCloseButtonBackColor()
        {
            return (this.CloseButtonBackColor  != Color.White);
        }
        protected virtual bool ShouldSerializeShowCloseButtonHighLightBackColor()
        {
            return (this.ShowCloseButtonHighLightBackColor != false);
        }

        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabPanelBackColor"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetTabPanelBackColor()
        {
            if (this.TabStyle == typeof(TabRendererOffice2007))
            {
                TabPanelBackColor = this.Office2007ColorTable.TabPanelBackColor;
            }
            else if (this.TabStyle == typeof(TabRendererOffice2010))
            {
                TabPanelBackColor = this.Office2010ColorTable.TabPanelBackColor;
            }
            else
            {
                TabPanelBackColor = Color.Empty;
            }

            UpdateScrollButtonsStyle();
            InvalidatePanel();
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Font"/>.
        /// </summary>
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                this.m_tabPanelData.Font = value;
                base.Font = value;

                if (this.IsHandleCreated)
                {
                    this.ComputeTabPositions();
                }
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.ResetFont"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetFont()
        {
            this.m_tabPanelData.Font = null;
            base.ResetFont();
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the active tab.
        /// </summary>
        /// <value>The Font object to apply to the text displayed 
        /// by the control. The default is the value of the Font property.</value>
        [
        Category(@"Appearance"),
        Localizable(true),
        AmbientValue(null),
        Description(@"The font used to display text in the Active Tabs. Will be overriden by individual Tab's Fonts.")
        ]
        public virtual Font ActiveTabFont
        {
            get
            {
                Font font = this.m_tabPanelData.ActiveTabFont;
                if (font == null)
                    font = this.m_tabPanelDefaultProperties.DefaultActiveTabFont();

                return font;
            }
            set
            {
                this.m_tabPanelData.ActiveTabFont = value;

                if (this.IsHandleCreated)
                {
                    this.ComputeTabPositions();
                }
            }
        }

        /// <summary>
        /// Indicates whether XP Themes (visual styles) should be used for this control when
        /// available.
        /// </summary>
        /// <remarks>
        /// <para>XP Themes are allowed only when <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.Alignment"/>
        /// is set to Top. Setting this property to true will reset the alignment to top.</para>
        /// <para>
        /// Themes are also used only by the "3D" <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> setting.
        /// </para>
        /// </remarks>
        [
        DefaultValue(false),
        Category(@"Appearance"),
        Description("Specifies whether XP Themes (visual styles) should be used for this control when available.")
        ]
        public bool ThemesEnabled
        {
            get { return this.m_themesEnabled; }
            set
            {
                if (this.m_themesEnabled != value)
                {
                    if (value && (this.IsOffice2003Style || this.IsWhidbeyStyle))
                    {
                        if (this.DesignMode)
                        {
                            throw new ArgumentException("Themes must be disabled with Office2003 or VS2005 styles", "ThemesEnabled");
                        }
                        else
                        {
                            return;
                        }
                    }

                    this.m_themesEnabled = value;
                    if (m_sbScrollButtons != null)
                    {
                        m_sbScrollButtons.ThemesEnabled = this.m_themesEnabled;
                        this.UpdateScrollButtonsStyle();
                    }
                    this.SetRegion();
                    this.SetNeedLayout(true);
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ActiveTabFont"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeActiveTabFont()
        {
            Font defaultFont = this.m_tabPanelDefaultProperties.DefaultActiveTabFont();
            if (this.ActiveTabFont.Equals(defaultFont))
                return false;
            else
                return true;
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ActiveTabFont"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetActiveTabFont()
        {
            this.m_tabPanelData.ActiveTabFont = null;
        }

        /// <summary>
        /// Gets or sets the backcolor of the active tabs. Will be overridden by any individual Tab BackColor in the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv"/> instance, if any.
        /// </summary>
        /// <value>The Color value.</value>
        [
        Category(@"Appearance"),
        Description(@"Color of the active Tabs. Will be overriden by any individual Tab BackColor."),
        ]
        public virtual Color ActiveTabColor
        {
            get
            {
                Color color = this.m_tabPanelData.ActiveTabColor;
                if (color == Color.Empty)
                    return this.m_tabPanelDefaultProperties.DefaultActiveTabColor();
                else
                    return color;
            }
            set
            {
                this.m_tabPanelData.ActiveTabColor = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ActiveTabColor"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeActiveTabColor()
        {
            return (this.ActiveTabColor != this.m_tabPanelDefaultProperties.DefaultActiveTabColor());
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ActiveTabColor"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetActiveTabColor()
        {
            this.m_tabPanelData.ActiveTabColor = Color.Empty;
            this.Invalidate();
        }

        /// <summary>
        /// Gets or sets the color of the inactive Tabs. Will be overriden by any individual Tab BackColor in the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv"/> instance, if any.
        /// </summary>
        /// <value>The Color value.</value>
        [
        Category(@"Appearance"),
        Description(@"Color of the inactive Tabs. Will be overriden by any individual Tab BackColor."),
        ]
        public virtual Color InactiveTabColor
        {
            get
            {
                Color color = this.m_tabPanelData.InactiveTabColor;
                if (color == Color.Empty)
                    return this.m_tabPanelDefaultProperties.DefaultInactiveTabColor();
                else
                    return color;
            }
            set { this.m_tabPanelData.InactiveTabColor = value; }
        }

        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ActiveTabColor"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeInactiveTabColor()
        {
            return (this.InactiveTabColor != this.m_tabPanelDefaultProperties.DefaultInactiveTabColor());
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.InactiveTabColor"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetInactiveTabColor()
        {
            this.m_tabPanelData.InactiveTabColor = Color.Empty;
        }

        /// <summary>
        /// Gets or sets the images to be displayed on the control's tabs.
        /// </summary>
        /// <value>An ImageList that specifies the images to display on the tabs.</value>
        /// <remarks>To display an image on a tab, set the ImageIndex property of that 
        /// TabPageAdv. The ImageIndex acts as the index into the ImageList.</remarks>
        [
        Category(@"Appearance"),
        DefaultValue(null),
        Description(@"The ImageList object from which this tab takes its images.")
        ]
        public virtual ImageList ImageList
        {
            get { return this.m_tabPanelData.ImageList; }
            set { this.m_tabPanelData.ImageList = value; }
        }
        /// <summary>
        /// Gets or sets the zero based index of the currently selected item. Returns -1 if no tabs are available.
        /// </summary>
        /// <value>The zero-based index of the currently-selected tab page. The default is -1, which is also the value if no tab page is selected.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"The zero based index of the currently selected item. Returns -1 if no tabs are available."),
        Category(@"Behavior"),
        Browsable(false),
        DefaultValue(-1 /*0xFFFFFFFF*/),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual int SelectedIndex
        {
            get { return this.Disposing || this.IsDisposed ? -1 : this.m_tabPanelData.SelectedIndex; }
            set
            {
                if (value != this.m_tabPanelData.SelectedIndex)
                {
                    foreach (TabRendererBase tabRenderer in this.Renderer.Renderers)
                    {
                        tabRenderer.ShowCloseButton = this.ShouldDrawCloseButton(value,tabRenderer.ShowCloseButton);
                    }
                }

                this.m_tabPanelData.SelectedIndex = value;

                if (this.TabPages != null && this.TabPages.Count > 0)
                {
                    this.InvalidatePanel();
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected tab page.
        /// </summary>
        /// <value>The currently-selected TabPageAdv. Default value is null.</value>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description(@"The currently selected tab page."),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced),
        Category(@"Appearance"),
        DefaultValue(null)
        ]
        public virtual TabPageAdv SelectedTab
        {
            get
            {
                if (this.SelectedIndex == -1)
                    return null;

                return TabPageAdvCollection.GetTabPageAdvOfTabData(this.m_tabPagesCollection, (ITabData)this.m_tabPanelData.TabsData[this.SelectedIndex]);
            }
            set
            {
                int newindex = this.m_tabPanelData.TabsData.IndexOf(value.TabData);
                if (newindex != -1)
                {
                    this.SelectedIndex = newindex;
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal text alignment of the Tab within the layout rectangle.
        /// </summary>
        /// <value>One of the StringAlignment values. Default is StringAlignment.Center.</value>
        [
        Category(@"Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Localizable(true),
        Description("Determines the horizontal text alignment of the Tab within the layout rectangle.")
        ]
        public virtual StringAlignment TextAlignment
        {
            get { return this.m_tabPanelData.TextAlignment; }
            set
            {
                this.m_tabPanelData.TextAlignment = value;
            }
        }

        [Documentation.DocumentationExclude()]
        protected virtual bool ShouldSerializeTextAlignment()
        {
            if (TabStyle == typeof(OneNoteStyleRenderer) || TabStyle == typeof(Office2003Renderer))
                return true;
            else
                return this.m_tabPanelData.TextAlignment != StringAlignment.Center;
        }

        /// <summary>
        /// Gets or sets the vertical line alignment of the Text in the Tab in the layout rectangle.
        /// </summary>
        /// <value>One of the StringAlignment values. Default is StringAlignment.Center.</value>
        [
        Category(@"Appearance"),
        DefaultValue(StringAlignment.Center),
        Localizable(true),
        Description("Determines the vertical line alignment of the Text in the Tab in the layout rectangle.")
        ]
        public virtual StringAlignment TextLineAlignment
        {
            get { return this.m_tabPanelData.TextLineAlignment; }
            set { this.m_tabPanelData.TextLineAlignment = value; }
        }

        /// <summary>
        /// Gets or sets the relative alignment of the Image with respect to the text.
        /// </summary>
        /// <value>One of the RelativeImageAlignment values. Default is RelativeImageAlignment.LeftOfText.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"The relative alignment of the Image with respect to the text."),
        Category(@"Appearance"),
        DefaultValue(RelativeImageAlignment.LeftOfText),
        Localizable(true)
        ]
        public virtual RelativeImageAlignment ImageAlignmentR
        {
            get
            {
                return m_tabPanelData.ImageAlignmentR;
            }
            set
            {
                m_tabPanelData.ImageAlignmentR = value;
            }
        }

        /// <summary>
        /// Indicates whether image should be disabled when TabPage is not selected.
        /// </summary>
        [
        Description(@"Gets or sets value whether image should be disabled when TabPage is not selected."),
        Category(@"Appearance"),
        DefaultValue(false)
        ]
        public virtual bool DisableInactivePageImage
        {
            get
            {
                return m_tabPanelData.DisableInactivePageImage;
            }
            set
            {
                m_tabPanelData.DisableInactivePageImage = value;
            }
        }

        /// <summary>
        /// Adjust y-position of the image.
        /// </summary>
        [
        Description(@"Adjusts y-position of the image."),
        Category(@"Appearance"),
        DefaultValue(0)
        ]
        public virtual int ImageOffset
        {
            get
            {
                return m_tabPanelData.ImageOffset;
            }
            set
            {
                if (m_tabPanelData.ImageOffset != value)
                {
                    m_tabPanelData.ImageOffset = value;
                }
            }
        }

        /// <summary>
        /// Adjusts the gap between the tabControlAdv's top and the tabs.
        /// </summary>
        [
        Description(@"Adjusts the gap between the tabControlAdv's top and the tabs."),
        Category(@"Appearance"),
        DefaultValue(0)
        ]
        public virtual int AdjustTopGap
        {
            get
            {
                return m_tabPanelData.AdjustTopGap;
            }
            set
            {
                if (m_tabPanelData.AdjustTopGap != value)
                {
                    if (value < 0)
                    {
                        m_tabPanelData.AdjustTopGap = 0;
                    }
                    else
                    {
                        m_tabPanelData.AdjustTopGap = value;
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the text and the image should be in the same level.
        /// </summary>
        [
        Description(@"Indicates whether the text and the image should be in the same level."),
        Category(@"Appearance"),
        DefaultValue(false)
        ]
        public bool LevelTextAndImage
        {
            get
            {
                return m_tabPanelData.LevelTextAndImage;
            }
            set
            {
                if (m_tabPanelData.LevelTextAndImage != value)
                {
                    m_tabPanelData.LevelTextAndImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets width of the custom borders.
        /// </summary>
        [
        Description(@"Indicates width of the custom borders."),
        Category(@"Appearance"),
        DefaultValue(c_defaultBorderWidth)
        ]
        public int BorderWidth
        {
            get
            {
                return m_borderWidth;
            }
            set
            {
                if (m_borderWidth != value)
                {
                    m_borderWidth = value;
                    if (m_borderWidth < 0)
                    {
                        m_borderWidth = 0;
                    }

                    if (this.TabStyle == typeof(TabRendererVS2008))
                    {
                        this.ComputeTabPositions();
                    }

                    this.ComputeTabPanelBounds();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets visibility of the custom borders.
        /// </summary>
        [
        Description(@"Indicates visibility of the custom borders."),
        Category(@"Appearance"),
        DefaultValue(false)
        ]
        public bool BorderVisible
        {
            get
            {
                return m_borderVisible;
            }
            set
            {
                if (m_borderVisible != value)
                {
                    m_borderVisible = value;
                    this.SetRegion();
                    this.ComputeTabPanelBounds();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets color of the custom borders.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description(@"Indicates color of the custom borders.")
        ]
        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                if (m_borderColor != value)
                {
                    m_borderColor = value;
                    this.Invalidate();
                }
            }
        }

        protected virtual bool ShouldSerializeBorderColor()
        {
            return m_borderColor != this.GetRendererBorderColor();
        }

        /// <summary>
        /// Resets BorderColor to its default value.
        /// </summary>
        public virtual void ResetBorderColor()
        {
            Color borderColor = this.GetRendererBorderColor();
            if (borderColor != Color.Empty)
            {
                m_borderColor = borderColor;
                this.Invalidate();
            }
        }


        /// <summary>
        /// Indicates whether the text in the tabs should be rotated to draw horizontally when the 
        /// tab strip is aligned to the left or right border.
        /// </summary>
        /// <value>True to rotate it when aligned vertically; false otherwise. Default is false.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Description(@"Rotates the text in the tabs when the tab strip is aligned to the left of right so that the text is always drawn horizontal."),
        Category(@"Appearance"),
        DefaultValue(false)
        ]
        public virtual bool RotateTextWhenVertical
        {
            get
            {
                return this.TabPanelData.RotateTextWhenVertical;
            }
            set
            {
                if (value != this.m_tabPanelData.RotateTextWhenVertical)
                {
                    this.m_tabPanelData.RotateTextWhenVertical = value;

                    ComputeTabPanelBounds();
                    this.UpdateScrollButtonsStyle();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool NeedRotateTextWhenVertical
        {
            get
            {
                return (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right) && this.RotateTextWhenVertical;
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        internal bool RotateText180WhenLeftAligned
        {
            get { return this.TabPanelData.RotateText180WhenLeftAligned; }
            set { this.m_tabPanelData.RotateText180WhenLeftAligned = value; }
        }

        /// <summary>
        /// Indicates whether tabs change in appearance when the mouse passes over them.
        /// </summary>
        /// <value>True to turn on hot-tracking; false otherwise.
        /// Default is false.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Category(@"Behavior"),
        DefaultValue(false),
        Description("Indicates whether the tabs change in appearance when the mouse passes over them.")
        ]
        public virtual bool HotTrack
        {
            get { return this.m_tabPanelData.HotTrack; }
            set { this.m_tabPanelData.HotTrack = value; }
        }

        /// <summary>
        /// Indicates whether the Control should take focus when one of the tabs is clicked.
        /// </summary>
        /// <remarks>
        /// Note that this will however still set focus on the tab control when the user tabs
        /// around to set focus on different controls. You should then use the TabStop property to
        /// prevent focus on tab.
        /// </remarks>
        [
        Category("Behavior"),
        DefaultValue(true),
        Description("Specifies whether or not the tab control takes focus when a tab is clicked.")
        ]
        public virtual bool FocusOnTabClick
        {
            get
            {
                return m_focusOnTabClick;
            }
            set
            {
                this.m_focusOnTabClick = value;
            }
        }

        /// <summary>
        /// Indicates whether the Control should switch between tab pages when the user enters 
        /// certain keys like Ctrl+Tab or Ctrl+Shift+Tab.
        /// </summary>
        /// <value>True to switch; false otherwise. Default is true.</value>
        /// <remarks>
        /// <para>When true, the Control will also process Up, Down, Left and Right keys (if it has focus) and the 
        /// Ctrl+PageDown and Ctrl+PageUp keys to shift between the tab pages appropriately.</para>
        /// <para>Ctrl+Tab and Ctrl+Page* keys will be processed by the tab control even
        /// when the focus is within one of the children in the tab pages. Also if the tab control
        /// is within an MDI Child Form, the default behavior of Ctrl+Tab keys switching
        /// the MDI child windows will be broken.</para>
        /// </remarks>
        [
        Category("Behavior"),
        DefaultValue(true),
        Description("Specifies if the Control should switch tab pages on Ctrl+Tab or Ctrl+Shift+Tab.")
        ]
        public virtual bool SwitchPagesForDialogKeys
        {
            get { return this.m_switchPagesForDialogKeys; }
            set
            {
                this.m_switchPagesForDialogKeys = value;
            }
        }

        /// <summary>
        /// Indicates whether tooltips should be shown for tabs that have their tooltips set.
        /// </summary>
        /// <value>True to turn on tooltips; false otherwise. Default is false.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Localizable(true),
        DefaultValue(false),
        Category(@"Behavior"),
        Description("Specifies whether tooltips should be shown for tabs that have their tooltips set.")
        ]
        public virtual bool ShowToolTips
        {
            get
            {
                return m_tabPanelData.ShowToolTips;
            }
            set
            {
                if (m_tabPanelData.ShowToolTips != value)
                {
                    m_tabPanelData.ShowToolTips = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether SuperToolTips should be shown for tabs that have their tooltips set.
        /// </summary>
        [
        Localizable(true),
        DefaultValue(false),
        Category(@"Behavior"),
        Description("Specifies whether SuperToolTips should be shown for tabs that have their tooltips set.")
        ]
        public virtual bool ShowSuperToolTips
        {
            get
            {
                return m_tabPanelData.ShowSuperToolTips;
            }
            set
            {
                if (m_tabPanelData.ShowSuperToolTips != value)
                {
                    m_tabPanelData.ShowSuperToolTips = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether users can change tab position within 
        /// the tab control by drag and drop.
        /// </summary>
        /// <value>True to allow users to move tabs; false otherwise. Default is false.</value>
        /// <example>
        /// Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.TabStyle"/> property reference for sample code on how to initialize a TabControlAdv programmatically.
        /// </example>
        [
        Category("Behavior"),
        DefaultValue(false),
        Description("Specifies whether users can change tab position within the tab control by drag and drop.")
        ]
        public virtual bool UserMoveTabs
        {
            get { return this.m_tabPanelData.UserMoveTabs; }
            set { this.m_tabPanelData.UserMoveTabs = value; }
        }
        private ScrollIncrement scrollIncrement = ScrollIncrement.Tab;

        /// <summary>
        /// Specifies whether to Scroll in tabs or pages.
        /// </summary>
        /// <value>One of the ScrollIncrement values.
        /// Default is ScrollIncrement.Tab.</value>
        [
            Description("Specify whether to Scroll in tabs or pages."),
            Category("Behavior"),
            DefaultValue(ScrollIncrement.Tab)
        ]
        public virtual ScrollIncrement ScrollIncrement
        {
            get { return this.scrollIncrement; }
            set { this.scrollIncrement = value; }
        }
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.ScrollButtons"/> Control instance representing the scroll control used in the tab panel strip.
        /// </summary>
        protected ScrollButtons ScrollButtons
        {
            get { return m_sbScrollButtons; }
        }
        #endregion TABPANEL_PROPERTIES_REFLECTED
        #region HIDDEN_TO_BROWSER
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Text"/>.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Bindable(false),
        Browsable(false),
        Description("Background Color of the tab panel and tabs. The tab's Color will be overriden by individual Tab BackColor, if any.")
        ]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        #endregion HIDDEN_TO_BROWSER

        #region LISTENERS

        private void TabPanel_SelectedIndexChanging(object sender, SelectedIndexChangingEventArgs args)
        {
            this.OnSelectedIndexChanging(args);
        }
        private void TabPanel_PropertyChanged(ITabPanelData sender, TabPanelPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Multiline")
                RendererChanged(null);
            else if (e.PropertyName == "SelectedIndex")
                this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        internal void UpdateSelectedTabIndex(int index)
        {
            if (this.m_tabPanelData != null)
            {
                this.m_tabPanelData.ChangeSelectedIndex(index);
            }
        }

        internal void UpdateActiveTabFont()
        {
            if (this.m_tabPagesCollection.Count > 0)
            {
                TabPageAdv page = this.SelectedTab;

                if (null != page)
                {
                    page.TabFont = this.ActiveTabFont;

                    for (int i = 0; i < this.m_tabPagesCollection.Count; i++)
                    {
                        if (i != this.SelectedIndex)
                        {
                            TabPageAdv tabPage = m_tabPagesCollection[i] as TabPageAdv;
                            Font fontTabDefaultInactive = this.m_tabPanelDefaultProperties.DefaultInactiveTabFont();
                            if (tabPage != null)
                            {
                                tabPage.TabFont = fontTabDefaultInactive;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnSelectedIndexChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnSelectedIndexChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnSelectedIndexChanged method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            // Layout before calling the SelectedIndexChanged event because the handler
            // might expect the child Controls to be visible at this point.
            this.SetNeedLayout(true);
            if (this.IsHandleCreated)
            {
                Graphics g = this.CreateGraphics();
                this.Layout(g, false);
                g.Dispose();
            }

            InvalidatePanel();

            //int selectedIndex = this.SelectedIndex;
            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, e);
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void InvalidatePanel()
        {
            this.Invalidate(Rectangle.Round(GetPanelBounds()));
        }

        /// <summary>
        /// Raises the <see cref="SelectedIndexChanging"/> event.
        /// </summary>
        /// <param name="e">An <see cref="SelectedIndexChanging"/> instance that contains the event data.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnSelectedIndexChanging method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnSelectedIndexChanging 
        /// in a derived class, be sure to call the base class's 
        /// OnSelectedIndexChanging method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnSelectedIndexChanging(SelectedIndexChangingEventArgs e)
        {
            if (this.SelectedIndexChanging != null)
            {
                this.SelectedIndexChanging(this, e);
            }
        }

        #region ONLAYOUT_METHODS
        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="setBounds">Indicates whether the bounds should also be set on the tab page.</param>
        /// <remarks>
        /// <para>
        /// This method is called from <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.Layout"/>to 
        /// ensure that the current tab page is valid and is the requested tab.
        /// </para>
        /// <para>Sometimes bounds should not be set on the tab pages as the tab control
        /// might not have been created at this point.</para>
        /// </remarks>
        protected virtual void UpdateSelectedTabPage(bool setBounds)
        {
            int currentTabPageIndex = -1;
            // If there is a current tab page, make sure that it's still in the collection
            if (m_currentTabPage != null)
            {
                currentTabPageIndex = this.m_tabPagesCollection.IndexOf(m_currentTabPage);
                if (currentTabPageIndex == -1)
                {
                    TabPageAdv oldCurTabPage = m_currentTabPage;
                    m_currentTabPage = null;
                    oldCurTabPage.Visible = false;
                }
            }
            if (m_currentTabPage == null ||
                this.m_tabPanelData.SelectedIndex != currentTabPageIndex)
            {
                // Hide the old tab page
                if (currentTabPageIndex != -1)
                {
                    // Cache and Clear to prevent recursion
                    TabPageAdv oldCurTabPage = m_currentTabPage;
                    m_currentTabPage = null;
                    oldCurTabPage.Visible = false;
                }

                // Show the new tab page
                if (this.m_tabPanelData.SelectedIndex != -1)
                {
                    // Update current tab page
                    TabPageAdv tabPage = this.m_tabPagesCollection[this.m_tabPanelData.SelectedIndex];

                    // We used to set the visibility and then the bounds
                    // but when the tabpage had a huge control like the grid, setting visible
                    // caused the control to show with the set bounds for a while before getting adjusted
                    // to the display rectangle. So, setting visibility after setting the bounds.
                    //tabPage.Visible = true;
                    if (setBounds)
                        tabPage.Bounds = DisplayRectangle;

                    if (this.IsHandleCreated)
                        tabPage.Visible = true;

                    m_currentTabPage = tabPage;
                    //tabPage.SelectNextControl(null, true, true, true, false);
                }
            }
            else // Just reset the bounds for the current tab page
            {
                // We used to set the visibility and then the bounds, but reversed the settings for the reason mentioned above.
                if (setBounds)
                    m_currentTabPage.Bounds = this.DisplayRectangle;
                m_currentTabPage.Visible = true;
            }

            if (this.ShowTabCloseButton && this.ShowCloseButtonForActiveTabOnly && this.SelectedIndex != -1)
            {
                UpdateCloseButtons(this.SelectedIndex);
            }
        }
        #endregion ONLAYOUT_METHODS
        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="rendererNew">The new tab panel renderer.</param>
        /// <remarks>
        /// <para>This method is called when the Multiline property is toggled.
        /// Internally, a different <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> is
        /// used to render the multiline mode and the singleline mode. You can override this method
        /// and provide a custom renderer or modify the existing renderer based on the current Multiline setting.</para>
        /// </remarks>
        protected virtual void RendererChanged(TabPanelRenderer rendererNew)
        {
            if (m_tabPanelRenderer != null)
                m_tabPanelRenderer.TabPanelData = null;

            if (rendererNew != null)
                m_tabPanelRenderer = rendererNew;
            else
            {
                if (this is BackStage)
                {
                    if((this as BackStage).BackStageStyle ==RibbonStyle .Office2010 )
                        m_tabPanelRenderer = new BackStageTabRenderer(this);
                    else
                        m_tabPanelRenderer = new BackStage2013TabRenderer(this);
                }
                else if (this.m_tabPanelData.Multiline == false)
                {
                    m_tabPanelRenderer = new SingleLineTabPanelRenderer(this);
                }
                else
                {
                    DestroyScrollButtons(true);
                    m_tabPanelRenderer = new MultilineTabPanelRenderer(this);
                }
            }

            if (m_tabPanelDefaultProperties != null)
                m_tabPanelDefaultProperties.TabPanelDefaultProperties = m_tabPanelRenderer as ITabPanelDefaultProperties;

            m_tabPanelRenderer.TabPanelData = m_tabPanelData;
            OnTabPanelBoundsAffected();
        }

        private bool m_bIsDisposing = false;

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                m_bIsDisposing = true;

                if (PersistTabState)
                {
                    SaveState();
                }

                //Office2003Colors.MenuColorsChanged -= new EventHandler(this.MenuColorsChanged);
                this.DestroyScrollButtons(false);

                if (m_sbScrollButtons != null)
                {
                    m_sbScrollButtons.Dispose();
                    m_sbScrollButtons = null;
                }

                if (this.m_tabPanelData != null)
                {
                    this.m_tabPanelData.ImageList = null;
                    this.m_tabPanelData.PropertyChanged -= new TabPanelPropertyChangedEventHandler(this.TabPanel_PropertyChanged);
                    this.m_tabPanelData.SelectedIndexChanging -= new SelectedIndexChangingEventHandler(this.TabPanel_SelectedIndexChanging);
                    this.m_tabPanelData = null;
                }
                if (this.m_tabPanelRenderer != null)
                {
                    this.m_tabPanelRenderer.TabPanelData = null;
                    this.m_tabPanelRenderer = null;
                }
                if (this.m_themedDrawing != null)
                {
                    this.m_themedDrawing.Dispose();
                    this.m_themedDrawing = null;
                }
                if (this.m_tabPanelDefaultProperties != null)
                {
                    this.m_tabPanelDefaultProperties = null;
                }
                if (this.m_tabPagesCollection != null)
                {
                    this.m_tabPagesCollection.Clear();
                    this.m_tabPagesCollection = null;
                }

                if (m_tabPrimitivesHost != null)
                {
                    m_tabPrimitivesHost.Dispose();
                    m_tabPrimitivesHost = null;
                }

                Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007Colors_ManagedColorsApplied);
                Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010Colors_ManagedColorsApplied);
                Office2003Colors.MenuColorsChanged -= new EventHandler(this.m_tabControlAdvWeakContainer.Office2003ColorsChangedWeakEventHandler);

                m_bIsDisposing = false;
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Forces the tab control to re-layout its elements.
        /// </summary>
        /// <remarks>
        /// Advanced method. Need not be called under normal usage scenarios.
        /// </remarks>
        public virtual void OnTabPanelBoundsAffected()
        {
            if (this.IsEditing)
            {
                EndLabelEdit(false);
            }

            SetNeedLayout(true);
            Invalidate(false);
        }

        private void RefreshLabelEdit()
        {
            if (this.IsEditing)
            {
                m_txtLabel.Font = this.ActiveTabFont;
                m_txtLabel.BackColor = this.m_tabPanelDefaultProperties.DefaultActiveTabColor();
                m_txtLabel.Bounds = GetLabelEditBounds();
            }
        }
        void ITabControl.OnRepaint(RectangleF affectedRect)
        {
            this.Invalidate(Rectangle.Ceiling(affectedRect), false);
        }

        private void Office2007Colors_ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            if (this.Office2007ColorScheme == Office2007Theme.Managed)
            {
                OnOffice2007ColorSchemeChanged();
            }
        }

        private void Office2010Colors_ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
        {
            if (this.Office2010ColorTheme == Office2010Theme.Managed)
            {
                OnOffice2010ColorSchemeChanged();
            }
        }
        #endregion LISTENERS

        #region Label Edit implementation
        private const int DEF_LABEL_EDIT_WIDTH_OFFSET = 5;
        private const int DEF_MIN_EDIT_TEXT_LENGTH = 8;
        private const string DEF_EMPTY_TEXT = "XXXXXXXX";

        private LabelEdit m_txtLabel = null;
        private bool m_bIsEditing = false;
        private bool m_bPrevRotateTextWhenVertical = false;
        private string m_strTextBeforeEdit = null;

        /// <summary>
        /// Occurs when the LabelEdit TabPage Caption is changed.
        /// </summary>
        [Description("Occurs when Editing TabPage's text ha been changed after TabPage's Caption editing")]
        public event EventHandler LabelEditTextChanged;

        /// <summary>
        /// Occurs when the LabelEdit property is changed.
        /// </summary>
        [Description("Occurs when the LabelEdit property is changed.")]
        public event EventHandler LabelEditChanged;

        /// <summary>
        /// Occurs before Editing TabPage's Caption editing.
        /// </summary>
        [Description("Occurs before Editing TabPage's Caption editing")]
        public event EditEventHandler BeforeEdit;

        /// <summary>
        /// Occurs after Editing TabPage's Caption editing.
        /// </summary>
        [Description("Occurs after Editing TabPage's Caption editing")]
        public event EditEventHandler AfterEdit;

        /// <summary>
        /// Occurs on moving TabPage
        /// </summary>
        [Description("Occurs on moving TabPage")]
        public event TabMovingEventHandler TabMoving;

        /// <summary>
        /// Occurs when the order of tabs is changed.
        /// </summary>
        [Description("Occurs when the order of tabs is changed.")]
        public event EventHandler TabsOrderChanged;

        /// <summary>
        /// Indicates whether TabPage's captions are editable.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates, if TabPage's captions are editable or not.")
        ]
        public bool LabelEdit
        {
            get
            {
                return m_bLabelEdit;
            }
            set
            {
                if (value != m_bLabelEdit)
                {
                    m_bLabelEdit = value;

                    OnLabelEdit();
                }
            }

        }

        /// <summary>
        /// Office2007 color scheme.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies color scheme for the control."),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                return m_Office2007ColorScheme;
            }
            set
            {
                if (m_Office2007ColorScheme != value)
                {
                    m_Office2007ColorScheme = value;

                    OnOffice2007ColorSchemeChanged();
                }
            }
        }
        /// <summary>
        /// Office2007 color scheme.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies color scheme for the control."),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010ColorTheme
        {
            get
            {
                return m_Office2010ColorScheme;
            }
            set
            {
                if (m_Office2010ColorScheme != value)
                {
                    m_Office2010ColorScheme = value;

                    OnOffice2010ColorSchemeChanged();
                }
            }
        }
        private void OnOffice2007ColorSchemeChanged()
        {
            m_office2007ColorTable = Office2007Colors.GetColorTable(m_Office2007ColorScheme);
            m_sbScrollButtons.Office2007ColorScheme = m_Office2007ColorScheme;

            OnStyleChanged();

            TabPanelRenderer panelRenderer = (TabPanelRenderer)m_tabPanelRenderer;
            panelRenderer.OnTabStyleChanged();
        }
        private void OnOffice2010ColorSchemeChanged()
        {
            m_office2010ColorTable = Office2010Colors.GetColorTable(m_Office2010ColorScheme);
            m_sbScrollButtons.Office2010ColorScheme = m_Office2010ColorScheme;

            OnStyleChanged();

            TabPanelRenderer panelRenderer = (TabPanelRenderer)m_tabPanelRenderer;
            panelRenderer.OnTabStyleChanged();
        }
        /// <summary>
        /// Overridden <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.LabelEdit"/>
        /// </summary>
        protected virtual void OnLabelEdit()
        {
            if (m_bLabelEdit)
            {
                if (m_txtLabel == null)
                {
                    m_txtLabel = new LabelEdit();
                    m_txtLabel.BorderStyle = BorderStyle.None;
                    m_txtLabel.TabStop = false;
                    m_txtLabel.Visible = false;
                    m_txtLabel.KeyDown += new KeyEventHandler(LabelEditKeyDown);
                    m_txtLabel.LostFocus += new EventHandler(m_txtLabel_LostFocus);

                    this.Controls.Add(m_txtLabel);
                }
            }
            else
            {
                if (m_txtLabel != null)
                {
                    if (this.IsEditing)
                        EndLabelEdit(false);
                    m_txtLabel.KeyUp -= new KeyEventHandler(LabelEditKeyDown);
                    this.Controls.Remove(m_txtLabel);
                    m_txtLabel.Dispose();
                    m_txtLabel = null;
                }
            }

            RaiseLabelEditChanged();
        }

        /// <summary>
        /// Handles the LostFocus event of the labelEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void m_txtLabel_LostFocus(object sender, EventArgs e)
        {
            if (this.IsEditing)
            {
                m_tabPageText = this.SelectedTab.Text;

                if (m_tabPageText != m_txtLabel.Text)
                    EndLabelEdit(true);
                else
                    EndLabelEdit(false);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabControlAdv.LabelEditChanged"/> event.
        /// </summary>
        protected void RaiseLabelEditChanged()
        {
            if (LabelEditChanged != null)
            {
                LabelEditChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the tab moving.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Tools.TabMovingEventArgs"/> instance containing the event data.</param>
        protected void RaiseTabMoving(TabMovingEventArgs e)
        {
            if (TabMoving != null)
            {
                TabMoving(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabControlAdv.AfterEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Forms.Tools.EditEventArgs"/> instance containing the event data.</param>
        protected void RaiseAfterEdit(EditEventArgs e)
        {
            if (AfterEdit != null)
            {
                AfterEdit(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabControlAdv.BeforeEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Forms.Tools.EditEventArgs"/> instance containing the event data.</param>
        protected void RaiseBeforeEdit(EditEventArgs e)
        {
            if (BeforeEdit != null)
            {
                BeforeEdit(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabControlAdv.LabelEditTextChanged"/> event.
        /// </summary>
        protected void RaiseLabelEditTextChanged()
        {
            if (LabelEditTextChanged != null)
            {
                LabelEditTextChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabControlAdv.TabsOrderChanged"/> event.
        /// </summary>
        protected void RaiseTabsOrderChanged()
        {
            if (TabsOrderChanged != null)
            {
                TabsOrderChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="TabControlAdv.TabsOrderChanged"/> event.
        /// </summary>
        public virtual void OnTabsOrderChanged()
        {
            RaiseTabsOrderChanged();
        }

        /// <summary>
        /// Raises the <see cref="E:TabMoving"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Tools.TabMovingEventArgs"/> instance containing the event data.</param>
        public virtual void OnTabMoving(TabMovingEventArgs e)
        {
            RaiseTabMoving(e);
        }

        protected virtual void OnLabelEditTextChanged()
        {
            RaiseLabelEditTextChanged();
        }


        protected virtual void OnAfterEdit(EditEventArgs e)
        {
            RaiseAfterEdit(e);
        }

        protected virtual void OnBeforeEdit(EditEventArgs e)
        {
            RaiseBeforeEdit(e);
        }


        private void LabelEditKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    EndLabelEdit(true);
                    break;
                case Keys.Escape:
                    EndLabelEdit(false);
                    break;
            }
        }

        private Rectangle CorrectEditBounds(Rectangle bounds)
        {
            if (m_sbScrollButtons != null && m_sbScrollButtons.Visible)
            {
                if (this.Alignment == TabAlignment.Top || this.Alignment == TabAlignment.Bottom)
                {
                    int offset = 0;

                    if (this.IsMirrored)
                    {
                        if (bounds.Left < m_sbScrollButtons.Right)
                        {
                            offset = (m_sbScrollButtons.Right - bounds.Left + DEF_LABEL_EDIT_WIDTH_OFFSET);
                        }

                        bounds.Offset(offset, 0);
                    }
                    else
                    {
                        if (bounds.Right > m_sbScrollButtons.Left)
                        {
                            offset = (bounds.Right - m_sbScrollButtons.Left + DEF_LABEL_EDIT_WIDTH_OFFSET);
                        }
                    }

                    bounds.Width -= offset;
                }
            }

            return bounds;
        }

        private Rectangle GetLabelEditBounds()
        {
            Rectangle bounds = Rectangle.Empty;

            if (this.SelectedTab != null)
            {
                TabRendererBase selectedTabRenderer =
                    this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                if (selectedTabRenderer != null)
                {
                    bounds = CorrectEditBounds(selectedTabRenderer.TextBounds);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Gets a value indicating whether the text is in editing mode.
        /// </summary>
        protected bool IsEditing
        {
            get
            {
                return m_bIsEditing;
            }
        }

        /// <summary>
        /// Begins the label edit process.
        /// </summary>
        protected void StartLabelEdit()
        {
            if (this.DrawItem == null && this.SelectedTab != null)
            {
                m_bPrevRotateTextWhenVertical = this.RotateTextWhenVertical;
                m_strTextBeforeEdit = null;

                if (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right)
                {
                    this.RotateTextWhenVertical = true;
                }

                //	if( this.SelectedTab.Text.Length < DEF_MIN_EDIT_TEXT_LENGTH )
                //	{
                //		m_strTextBeforeEdit = this.SelectedTab.Text;
                //		this.SelectedTab.Text = DEF_EMPTY_TEXT;
                //	}

                TabRendererBase selectedTabRenderer =
                    this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                if (selectedTabRenderer != null)
                {
                    selectedTabRenderer.ShouldDrawText = false;
                }

                this.InvalidatePanel();

                m_txtLabel.Text = (m_strTextBeforeEdit == null) ?
                    this.SelectedTab.Text : m_strTextBeforeEdit;

                RefreshLabelEdit();

                NativeMethodsHelper.SetRedrawWindow(this.Handle, false, false);

                EditEventArgs e = new EditEventArgs(m_txtLabel.Text);
                OnBeforeEdit(e);

                if (e.EditText != m_txtLabel.Text)
                {
                    m_txtLabel.Text = e.EditText;
                }

                m_txtLabel.Visible = true;
                m_txtLabel.Select();
                NativeMethodsHelper.SetRedrawWindow(this.Handle, true, true);

                m_bIsEditing = true;

            }
        }

        /// <summary>
        /// Ends the label edit.
        /// </summary>
        /// <param name="success">End the editing process and sets the new text, if true.</param>
        protected void EndLabelEdit(bool success)
        {
            m_bIsEditing = false;
            m_txtLabel.Visible = false;

            if (this.SelectedTab != null)
            {
                if (success)
                {
                    EditEventArgs e = new EditEventArgs(m_txtLabel.Text);
                    OnAfterEdit(e);

                    this.SelectedTab.Text = (e.EditText != m_txtLabel.Text) ?
                        e.EditText :
                        m_txtLabel.Text;

                    OnLabelEditTextChanged();
                }
                else
                {
                    string argsText = (m_strTextBeforeEdit == null) ?
                        this.SelectedTab.Text :
                        m_strTextBeforeEdit;

                    EditEventArgs e = new EditEventArgs(argsText);

                    OnAfterEdit(e);

                    if (e.EditText != this.SelectedTab.Text)
                    {
                        this.SelectedTab.Text = e.EditText;
                    }
                    else if (m_strTextBeforeEdit != null)
                    {
                        this.SelectedTab.Text = m_strTextBeforeEdit;
                    }
                }

                TabRendererBase selectedTabRenderer =
                    this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                if (selectedTabRenderer != null)
                {
                    selectedTabRenderer.ShouldDrawText = true;
                }
            }

            this.RotateTextWhenVertical = m_bPrevRotateTextWhenVertical;
            m_strTextBeforeEdit = null;

            this.InvalidatePanel();
        }
        #endregion

        #region ITABCONTROLIMP
        Graphics ITabControl.GetGraphics()
        {
            if (m_currentGraphics != null)
                return m_currentGraphics;
            else
                return this.CreateGraphics();
        }
        Control ITabControl.GetControl()
        {
            return this;
        }

        bool ITabControl.IsDesignMode()
        {
            return this.DesignMode;
        }
        /// <summary>
        /// Returns the Rectangle region of a Tab in client co-ordinates given its tab-index.
        /// </summary>
        /// <param name="index">The tab index of the tab.</param>
        /// <returns>A Rectangle in client co-ordinates.</returns>
        public Rectangle GetTabRect(int index)
        {
            return this.m_tabPanelRenderer.GetTabBounds(index);
        }

        /// <summary>
        /// Is mouse point contains in scroll button.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal bool GetHitTestScroll(Point mousePos)
        {
            bool ret = false;

            if (this.ScrollButtons != null && this.ScrollButtons.Visible)
            {
                ret = this.ScrollButtons.Bounds.Contains(mousePos);
            }
            if (!ret && this.TabPrimitivesHost.Visible)
            {
                ret = this.TabPrimitivesHost.Bounds.Contains(mousePos);
            }

            return ret;
        }

        /// <summary>
        /// Returns the tab at the specified location.
        /// </summary>
        /// <param name="mousePos">The point where the tab is to be found.</param>
        /// <returns>The hit tab's index; -1 if none found.</returns>
        public virtual int HitTestTabs(Point mousePos)
        {
            return this.HitTestTabs(mousePos, false);
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal virtual int HitTestTabs(Point mousePos, bool inTransformedCoOrds)
        {
            return this.m_tabPanelRenderer.HitTestTabs(mousePos, inTransformedCoOrds);
        }
        /// <summary>
        /// Raises the DrawItem event.
        /// </summary>
        /// <param name="eventArgs">A DrawItemEventArgs that contains the event data.</param>
        /// <returns>True if there were listeners; false otherwise.</returns>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnDrawItem method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnDrawItem 
        /// in a derived class, be sure to call the base class's 
        /// OnDrawItem method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        public virtual bool OnDrawItem(DrawTabEventArgs eventArgs)
        {
            if (this.DrawItem != null)
            {
                this.DrawItem(this, eventArgs);
                return true;
            }
            return false;
        }
        void ITabControl.OnScrollPositionChanged()
        {
            if (!m_sbScrollButtons.Visible && !this.Multiline && m_bShowScroll)
            {
                // We need a scrollbar only if we actually CAN scroll ...
                if (this.m_tabPanelRenderer.CanScrollLeft || this.m_tabPanelRenderer.CanScrollRight)
                {
                    this.InitScrollButtons();
                }
            }
            if (m_sbScrollButtons == null)
                return;

            m_sbScrollButtons.MinButtonActive = this.m_tabPanelRenderer.CanScrollLeft;
            m_sbScrollButtons.MaxButtonActive = this.m_tabPanelRenderer.CanScrollRight;
        }
        #endregion ITABCONTROLIMP

        #region DELEGATE_MESSAGES

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseEnter"/>.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            TabPanelRenderer panelRenderer = m_tabPanelRenderer as TabPanelRenderer;

            if (panelRenderer != null)
            {
                panelRenderer.OnMouseEnter(e);
            }

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseHover"/>.
        /// </summary>
        protected override void OnMouseHover(EventArgs e)
        {
            TabPanelRenderer panelRenderer = m_tabPanelRenderer as TabPanelRenderer;

            if (panelRenderer != null && this.TopLevelControl is Form && this.TopLevelControl.ContainsFocus)
            {
                panelRenderer.OnMouseHover(e);
            }

            if (m_tabPrimitivesHost != null)
            {
                m_tabPrimitivesHost.HandledMouseHover(e);
            }

            base.OnMouseHover(e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!DesignMode)
            {
                if (this.Renderer != null && Renderer.Renderers != null &&
                    this.SelectedIndex >= 0 && this.SelectedIndex <= this.Renderer.Renderers.Count)
                {
                    TabRendererBase renderer = this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                    if (renderer != null)
                    {
                        // Get the mouse positions
                        Point mousePosition = new Point(e.X, e.Y);

                        // Transform it to the renderer co-ords
                        RectangleF rectMousePos = this.Renderer.ApplyDrawingTransform(new RectangleF(mousePosition, SizeF.Empty), true);

                        if (this.RotateTextWhenVertical && (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right))
                        {
                            rectMousePos = new RectangleF(mousePosition, SizeF.Empty);
                        }

                        mousePosition = Point.Round(rectMousePos.Location);
                        bool hitChanged = false;
                        if (renderer.CloseButtonHitTest(mousePosition))
                        {
                            if (!renderer.HitCloseButton)
                            {
                                hitChanged = true;
                                renderer.HitCloseButton = true;
                            }
                        }
                        else
                        {
                            if (renderer.HitCloseButton)
                            {
                                hitChanged = true;
                                renderer.HitCloseButton = false;
                            }
                        }
                        if ( hitChanged )
                        {
                            RectangleF rect = this.Renderer.ApplyDrawingTransform(renderer.GetCurrentBounds(), false);
                            Invalidate(Rectangle.Ceiling(rect));
                        }
                    }
                }
            }
            if (this.GetType() == typeof(DockTabControl) && !this.HotTrack )
            {
                return;
            }
            m_tabPanelRenderer.OnMouseMove(e);

            m_tabPrimitivesHost.HandledMouseMove(e);

            m_sbScrollButtons.Tracking = ButtonID.None;

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_tabPanelRenderer.OnMouseLeave(e);

            m_tabPrimitivesHost.HandledMouseLeave();

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Point mouseUpPt = new Point(e.X, e.Y);
            // OnMouseUp could be called after this control gets disposed, if it is a right
            // mouse up that follows a context menu popup (in whose handler the control could have been
            // destroyed).
            if (this.IsHandleCreated)
            {
                m_tabPanelRenderer.OnMouseUp(e);
            }

            m_tabPrimitivesHost.HandledMouseUp(e);

            base.OnMouseUp(e);

            if (m_bLabelEdit && this.SelectedIndex == m_prevSelectedIndex && this.SelectedTab != null && e.Button == MouseButtons.Left)
            {
                if (m_pt == mouseUpPt)
                {
                    this.StartLabelEdit();
                }
            }

            m_prevSelectedIndex = this.SelectedIndex;

            if (!DesignMode)
            {
                if (this.Renderer != null && Renderer.Renderers != null &&
                    this.SelectedIndex >= 0 && this.SelectedIndex <= this.Renderer.Renderers.Count)
                {
                    TabRendererBase renderer = this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                    if (renderer != null)
                    {
                        // Get the mouse positions
                        Point mousePosition = new Point(e.X, e.Y);

                        // Transform it to the renderer co-ords
                        RectangleF rectMousePos = this.Renderer.ApplyDrawingTransform(new RectangleF(mousePosition, SizeF.Empty), true);

                        if (this.RotateTextWhenVertical && (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right))
                        {
                            rectMousePos = new RectangleF(mousePosition, SizeF.Empty);
                        }

                        mousePosition = Point.Round(rectMousePos.Location);

                        if (renderer.CloseButtonHitTest(mousePosition) && e.Button == MouseButtons.Left && renderer.CloseButtonClicked)
                        {
                            Form form = this.SelectedTab.Tag as Form;

                            if (form != null)
                            {
                                int selIndex = this.SelectedIndex;

                                bool bIsMdiChild = form.IsMdiChild;

                                form.Close();

                                if (renderer.CloseButtonClicked)
                                {
                                    renderer.CloseButtonClicked = false;
                                    renderer.HitCloseButton = false;
                                    if(this.SelectedIndex >= 0)
                                        this.InvalidatePanel();
                                }
                            }
                            else
                            {
                                TabPages[SelectedIndex].Close();

                                if (renderer != null)
                                {
                                    renderer.CloseButtonClicked = false;
                                    renderer.HitCloseButton = false;

                                    this.InvalidatePanel();
                                }
                            }
                        }
                        else if (renderer.CloseButtonClicked)
                        {
                            renderer.CloseButtonClicked = false;
                        }
                    }
                }
            }
        }

        int m_prevSelectedIndex = -1;
        Point m_pt = Point.Empty;
        string m_tabPageText = string.Empty;
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!DesignMode)
            {
                if (this.Renderer != null && Renderer.Renderers != null &&
                    this.SelectedIndex >= 0 && this.SelectedIndex <= this.Renderer.Renderers.Count)
                {
                    TabRendererBase renderer = this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                    if (renderer != null)
                    {
                        if (e.Button == MouseButtons.Left && renderer.HitCloseButton)
                        {
                            renderer.CloseButtonClicked = true;
                            RectangleF rect = this.Renderer.ApplyDrawingTransform(renderer.GetCurrentBounds(), false);
                            Invalidate(Rectangle.Ceiling(rect));
                        }
                    }
                }
            }

            if (this.IsEditing)
            {
                m_prevSelectedIndex = int.MinValue;
                m_tabPageText = this.SelectedTab.Text;

                if (m_tabPageText != m_txtLabel.Text)
                    EndLabelEdit(true);
                else
                    EndLabelEdit(false);
            }

            if (e.Clicks == 2 && this.GetTabRect(this.SelectedIndex).Contains(e.X, e.Y))
                m_pt = new Point(e.X, e.Y);


            m_previousTabPage = m_currentTabPage;


            m_validateStatus = ValidateStatus.None;
            if (this.m_focusOnTabClick
                && m_tabPanelRenderer.HitTestTabs(new PointF(e.X, e.Y), false) != -1)
            {
                if (!this.Focused)
                {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )                   
                    m_previousTabPage.FireLeave(EventArgs.Empty);
#endif
                    this.Focus();
                }

                if (Focused)
                {
                    m_validateStatus = ValidateStatus.Passed;
                }
                else
                {
                    m_validateStatus = ValidateStatus.Failed;
                }
            }
            //here currentTabPage is changed
            if (Parent != null && !(this.Parent is TabHost))
            {
                NativeMethods.LockWindowUpdate(this.Handle);
                m_tabPanelRenderer.OnMouseDown(e);
                NativeMethods.LockWindowUpdate(IntPtr.Zero);
            }
            else
                m_tabPanelRenderer.OnMouseDown(e);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            FireTabPageLeaveEnter();
#endif

            m_tabPrimitivesHost.HandledMouseDown(e);

            base.OnMouseDown(e);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnGotFocus"/>.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data. </param>
        protected override void OnGotFocus(EventArgs e)
        {
            if (this.Disposing)
                return;
            m_tabPanelRenderer.OnGotFocus(e);

            base.OnGotFocus(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            // Sometimes OnVisibleChanged got called from Dispose! Hence the Renderer check.
            if (this.Visible && this.Renderer != null)
            {
                // We turn on D-B a little later (see constructor for more info)

                // With both these styles turned on painting misbehaves when you 
                // fill the Control with its BackColor, it ends up drawing with
                // a different BackColor (especially in lower color modes).
                // Hence the check in different places before drawing the BackColor
                this.SetStyle(ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.DoubleBuffer
                    , true);

                // Without this check this did not work:
                // Hiding the form on minimize followed by a showing and restoring.
                if (this.Width != 0 && this.Height != 0)
                {
                    Graphics g = this.CreateGraphics();
                    this.Layout(g, false);
                    g.Dispose();
                }
            }
            base.OnVisibleChanged(e);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            switch (base.RightToLeft)
            {
                case RightToLeft.No:
                    {
                        m_bMirrored = false;
                        break;
                    }
                case RightToLeft.Yes:
                    {
                        m_bMirrored = true;
                        break;
                    }
            }

            SetNeedLayout(true);
            Invalidate(false);
            //base.OnRightToLeftChanged (e);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnLostFocus"/>.
        /// </summary>
        /// <param name="e"> An EventArgs that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (this.Disposing)
                return;

            m_tabPanelRenderer.OnLostFocus(e);

            base.OnLostFocus(e);
        }

        /// </override>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            //Office2003Colors.MenuColorsChanged -= new EventHandler(this.MenuColorsChanged);

            if (this.m_themedDrawing != null)
            {
                this.m_themedDrawing.DetachTabControl();
            }
        }

        /// </override>
        protected override bool ProcessMnemonic(char charCode)
        {
            foreach (Control child in this.Controls)
            {
                if (IsMnemonic(charCode, child.Text))
                {
                    if (child is TabPageAdv)
                    {
                        this.SelectedTab = child as TabPageAdv;
                        return true;
                    }
                }

            }
            return false;
        }
        /// <summary>
        /// Calls the <see cref="System.Windows.Forms.ContainerControl.Validate()"/> method
        /// on the parent container control.
        /// </summary>
        /// <returns>True if validation was successful; false otherwise.</returns>
        public virtual bool ValidateFocusedTab()
        {
            IContainerControl icc = this.GetContainerControl();
            if ((!(icc is ContainerControl)) || !((ContainerControl)icc).Validate())
                return false;
            else
                return true;
        }

        /// </override>
        protected override void OnLeave(EventArgs e)
        {
            if (m_currentTabPage != null)
            {
                m_currentTabPage.FireLeave(EventArgs.Empty);
            }
            base.OnLeave(e);
        }

        /// </override>
        protected override void OnEnter(EventArgs e)
        {
            if (m_currentTabPage != null)
            {
                m_currentTabPage.FireEnter(EventArgs.Empty);
            }
            base.OnEnter(e);
        }
        #endregion DELEGATE_MESSAGES

        #region PAINTING
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnSystemColorsChanged"/>.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            Office2003Colors.SysColorsChanged(false);
            VS2005Colors.UpdateStyleColors();
            WindowsXPThemeColors.UpdateColors();

            Color borderColor = this.GetRendererBorderColor();
            if (borderColor != Color.Empty)
            {
                m_borderColor = borderColor;
            }
            this.UpdateScrollButtonsStyle();

            this.Refresh();
        }
        internal void MenuColorsChanged(object sender, EventArgs e)
        {
            // This helps if MenuColors were changed programmatically.
            this.Invalidate();
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.DisplayRectangle"/>.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                if (!this.IsHandleCreated || this.ClientRectangle == Rectangle.Empty)
                    return Rectangle.Empty;

                Rectangle tabPageBounds = Rectangle.Round(this.GetBorderRect());
                if (!this.BorderVisible && this.BorderStyle != BorderStyle.None)
                {
                    // 1 pixel on all sides for border
                    tabPageBounds.Inflate(-1, -1);
                    if (this.BorderStyle == BorderStyle.Fixed3D)
                    {
                        // additional 1 pixel to the right and bottom for 3d border
                        tabPageBounds.Height -= 1;
                        tabPageBounds.Width -= 1;

                        // For RTL bounds are shifted to the right in order to draw left 3Dstyle border without clipping
                        if (IsMirrored)
                        {
                            ++tabPageBounds.X;
                        }
                    }
                }

                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
                {
                    tabPageBounds.Inflate(-2, -2);
                }
                else if (this.BorderVisible)
                {
                    tabPageBounds.Inflate(-this.BorderWidth, -this.BorderWidth);
                    switch (this.Alignment)
                    {
                        case TabAlignment.Top:
                            tabPageBounds.Y -= this.BorderWidth - 1;
                            tabPageBounds.Height += this.BorderWidth - 1;
                            break;
                        case TabAlignment.Bottom:
                            tabPageBounds.Y -= 1;
                            tabPageBounds.Height += this.BorderWidth - 1;
                            break;
                        case TabAlignment.Left:
                            tabPageBounds.X -= this.BorderWidth - 1;
                            tabPageBounds.Width += this.BorderWidth - 1;
                            break;
                        case TabAlignment.Right:
                            tabPageBounds.X -= 1;
                            tabPageBounds.Width += this.BorderWidth - 1;
                            break;
                    }
                }

                if (tabPageBounds.Height < 0)
                {
                    tabPageBounds.Height = 0;
                }

                if (tabPageBounds.Width < 0)
                {
                    tabPageBounds.Width = 0;
                }

                return tabPageBounds;
            }
        }
        /// <summary>
        /// Returns the Top and Left border color.
        /// </summary>
        /// <returns>The Color value.</returns>
        protected virtual Color GetTopLeftBorderColor()
        {
            if (this.TabPanelData.BorderStyle == BorderStyle.Fixed3D)
                return SystemColors.ControlLightLight;
            else if (this.TabPanelData.BorderStyle == BorderStyle.FixedSingle)
                return this.FixedSingleBorderColor;
            else
                return Color.Empty;
        }

        /// <summary>
        /// Returns the Right and Bottom border color.
        /// </summary>
        /// <returns>The Color value.</returns>
        protected virtual Color GetRightBottomBorderColor()
        {
            if (this.TabPanelData.BorderStyle == BorderStyle.Fixed3D)
                return SystemColors.ControlDarkDark;
            else if (this.TabPanelData.BorderStyle == BorderStyle.FixedSingle)
                return this.FixedSingleBorderColor;
            else
                return Color.Empty;
        }

        /// <summary>
        /// Returns the Right and Bottom border shade color.
        /// </summary>
        /// <returns>The Color value.</returns>
        protected virtual Color GetRightBottomBorderShadeColor()
        {
            if (this.m_tabPanelData.BorderStyle == BorderStyle.Fixed3D)
                return SystemColors.ControlDark;
            else
                return Color.Empty;
        }

        /// <summary>
        /// Draws the 3D border around the tab control.
        /// </summary>
        /// <param name="g">The Graphics object into which the border is drawn.</param>
        /// <param name="borderBounds">The rectangular bounds within which the border is drawn.</param>
        protected virtual void Draw3DBorder(Graphics g, RectangleF borderBounds)
        {
            if (borderBounds.Height == 0 || borderBounds.Width == 0)
                return;

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                RectangleF themedBorder = borderBounds;
                themedBorder.Width += 2.0f;
                this.m_themedDrawing.DrawTabPane(g, Rectangle.Ceiling(themedBorder));
            }
            else if (this.BorderVisible)
            {
                Rectangle bounds = Rectangle.Ceiling(borderBounds);
                int width = this.BorderWidth;

                Rectangle topRect = new Rectangle(bounds.Location, new Size(bounds.Right, width));
                Rectangle bottomRect = new Rectangle(bounds.Left, bounds.Bottom - width, bounds.Right, width);
                Rectangle leftRect = new Rectangle(bounds.Location, new Size(width, bounds.Bottom - 1));
                Rectangle rightRect = new Rectangle(bounds.Right - width, bounds.Top, width, bounds.Bottom - 1);

                using (SolidBrush brush = new SolidBrush(this.ActiveTabColor))
                {
                    g.FillRectangle(brush, leftRect);
                    g.FillRectangle(brush, bottomRect);
                    g.FillRectangle(brush, rightRect);
                    g.FillRectangle(brush, topRect);
                }

                Color borderColor = SystemColors.ControlDark;

                if (this.Renderer != null && this.Renderer.Renderers != null
                    && this.SelectedIndex >= 0 && this.SelectedIndex < this.Renderer.Renderers.Count)
                {
                    TabRendererBase tabPageRenderer = this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;

                    if (this.BorderColor != this.BackColor)
                    {
                        borderColor = this.BorderColor;
                    }
                    else if (tabPageRenderer != null)
                    {
                        borderColor = tabPageRenderer.TabBorderColor;
                    }
                }

                Color lightBorderColor = WindowsXPThemeColors.TabControlAdvLightBorderColor;
                if (this.IsVistaOS)
                {
                    lightBorderColor = Color.White;
                }

                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        this.DrawTopAlignmentBorders(g, bounds, borderColor, lightBorderColor);
                        break;
                    case TabAlignment.Bottom:
                        this.DrawBottomAlignmentBorders(g, bounds, borderColor, lightBorderColor);
                        break;
                    case TabAlignment.Left:
                        this.DrawLeftAlignmentBorders(g, bounds, borderColor, lightBorderColor);
                        break;
                    case TabAlignment.Right:
                        this.DrawRightAlignmentBorders(g, bounds, borderColor, lightBorderColor);
                        break;
                }
            }
            else
            {
                // Point arrays are built in order to draw polygons from left to right for LTR
                // and vice versa
                bool bIsMirrored = IsMirrored;

                float fLeft = bIsMirrored ? borderBounds.Right - 1 : borderBounds.Left;
                float fRight = bIsMirrored ? borderBounds.Left : borderBounds.Right - 1;
                // Corner points
                PointF[] corners1 = { 
                                      new PointF( fLeft, borderBounds.Bottom - 1 ),
									  new PointF( fLeft, borderBounds.Top ),
									  new PointF( fRight, borderBounds.Top )
									};

                float fBottom = borderBounds.Bottom - 1 < borderBounds.Top ? borderBounds.Top : borderBounds.Bottom - 1;

                PointF[] corners2 = {	
                                      new PointF( fRight, borderBounds.Top ),
									  new PointF( fRight, fBottom ),
									  new PointF( fLeft, fBottom )
									};
                // Top-Left border
                using (Pen p = new Pen(this.GetTopLeftBorderColor()))
                    g.DrawLines(p, corners1);
                // Right-Bottom border
                using (Pen p = new Pen(GetRightBottomBorderColor()))
                    g.DrawLines(p, corners2);
                // Right Shade
                if (bIsMirrored)
                {
                    // Shade is shifted to the left
                    corners2[0].X += 1;
                    corners2[1].X += 1;
                    corners2[2].X -= 1;
                }
                else
                {
                    // Shade is shifted to the right
                    corners2[0].X -= 1;
                    corners2[1].X -= 1;
                    corners2[2].X += 1;
                }
                corners2[1].Y -= 1;
                corners2[2].Y -= 1;
                using (Pen p = new Pen(GetRightBottomBorderShadeColor()))
                    g.DrawLines(p, corners2);
            }
        }

        /// <summary>
        /// Sets region of the control
        /// </summary>
        protected virtual void SetRegion()
        {
            if (this.BorderVisible && !this.ThemesEnabled)
            {
                Region region = new Region(this.ClientRectangle);
                Rectangle bounds = Rectangle.Ceiling(this.GetBorderRect());

                if (this.Alignment == TabAlignment.Bottom)
                {
                    bounds = new Rectangle(bounds.Left, bounds.Top - 1, bounds.Width, bounds.Height);
                }
                else if (this.Alignment == TabAlignment.Right)
                {
                    bounds = new Rectangle(bounds.Left - 1, bounds.Top, bounds.Width, bounds.Height);
                }

                Point[] bottomLeftPoints = new Point[]
                                {
                                    new Point( bounds.Left, bounds.Bottom - c_cornerCut ),
                                    new Point( bounds.Left, bounds.Bottom ),
                                    new Point( bounds.Left + c_cornerCut, bounds.Bottom )
                                };

                Point[] bottomRightPoints = new Point[]
                                {
                                    new Point( bounds.Right, bounds.Bottom - c_cornerCut ),
                                    new Point( bounds.Right, bounds.Bottom ),
                                    new Point( bounds.Right - c_cornerCut, bounds.Bottom )
                                };

                Point[] topRightPoints = new Point[]
                                {
                                    new Point( bounds.Right, bounds.Top + c_cornerCut - 1 ),
                                    new Point( bounds.Right, bounds.Top ),
                                    new Point( bounds.Right - c_cornerCut + 1, bounds.Top )
                                };

                Point[] topLeftPoints = new Point[]
                                {
                                    new Point( bounds.Left, bounds.Top + c_cornerCut - 1 ),
                                    new Point( bounds.Left, bounds.Top ),
                                    new Point( bounds.Left + c_cornerCut - 1, bounds.Top )
                                };

                using (GraphicsPath path = new GraphicsPath())
                {
                    switch (this.Alignment)
                    {
                        case TabAlignment.Top:
                            path.AddLines(bottomLeftPoints);
                            region.Exclude(path);
                            path.Reset();
                            path.AddLines(bottomRightPoints);
                            region.Exclude(path);
                            break;

                        case TabAlignment.Bottom:
                            path.AddLines(topLeftPoints);
                            region.Exclude(path);
                            path.Reset();
                            path.AddLines(topRightPoints);
                            region.Exclude(path);
                            break;

                        case TabAlignment.Left:
                            path.AddLines(bottomRightPoints);
                            region.Exclude(path);
                            path.Reset();
                            path.AddLines(topRightPoints);
                            region.Exclude(path);
                            break;

                        case TabAlignment.Right:
                            path.AddLines(bottomLeftPoints);
                            region.Exclude(path);
                            path.Reset();
                            path.AddLines(topLeftPoints);
                            region.Exclude(path);
                            break;
                    }
                }

                this.Region = region;
            }
            else
            {
                this.Region = null;
            }
        }

        /// <summary>
        /// Draw borders for top tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawTopAlignmentBorders(Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor)
        {
            if (this.IsVS2008)
            {
                using (Pen pen = new Pen(lightBorderColor))
                using (GraphicsPath path = GetOuterTopAlignmentBordersPath(new Rectangle(bounds.X + 1, bounds.Y, bounds.Width - 2, bounds.Height - 1)))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (Pen pen = new Pen(borderColor))
            {
                using (GraphicsPath path = GetOuterTopAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
                using (GraphicsPath path = GetInnerTopAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Draw borders for bottom tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawBottomAlignmentBorders(Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor)
        {
            if (this.IsVS2008)
            {
                using (Pen pen = new Pen(lightBorderColor))
                using (GraphicsPath path = GetOuterBottomAlignmentBordersPath(new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 1)))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (Pen pen = new Pen(borderColor))
            {
                using (GraphicsPath path = GetOuterBottomAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
                using (GraphicsPath path = GetInnerBottomAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Draw borders for left tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawLeftAlignmentBorders(Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor)
        {
            if (this.IsVS2008)
            {
                using (Pen pen = new Pen(lightBorderColor))
                using (GraphicsPath path = GetOuterLeftAlignmentBordersPath(new Rectangle(bounds.X, bounds.Y + 1, bounds.Width - 1, bounds.Height - 2)))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (Pen pen = new Pen(borderColor))
            {
                using (GraphicsPath path = GetOuterLeftAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
                using (GraphicsPath path = GetInnerLeftAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Draw borders for right tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawRightAlignmentBorders(Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor)
        {
            if (this.IsVS2008)
            {
                using (Pen pen = new Pen(lightBorderColor))
                using (GraphicsPath path = GetOuterRightAlignmentBordersPath(new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 2)))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (Pen pen = new Pen(borderColor))
            {
                using (GraphicsPath path = GetOuterRightAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
                using (GraphicsPath path = GetInnerRightAlignmentBordersPath(bounds))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Gets outer borders for top tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterTopAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            if (!this.IsOffice2007Style && !this.IsOffice2010Style && !this.IsDockingWhidbeyStyle)
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }
            path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            path.AddLine(bounds.Left, bounds.Bottom - c_cornerCut, bounds.Left + c_cornerCut, bounds.Bottom);
            path.AddLine(bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            path.AddLine(bounds.Right - c_cornerCut, bounds.Bottom - 1, bounds.Right, bounds.Bottom - c_cornerCut - 1);
            path.AddLine(bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);

            return path;
        }

        /// <summary>
        /// Gets inner borders for top tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerTopAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.BorderWidth;

            path.AddLine(bounds.Left + width - 1, bounds.Top, bounds.Left + width - 1, bounds.Bottom - width);
            path.AddLine(bounds.Left + width - 1, bounds.Bottom - width, bounds.Right - width, bounds.Bottom - width);
            path.AddLine(bounds.Right - width, bounds.Top, bounds.Right - width, bounds.Bottom - width);

            return path;
        }

        /// <summary>
        /// Gets outer borders for bottom tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterBottomAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            if (!this.IsOffice2007Style && !this.IsOffice2010Style && !this.IsDockingWhidbeyStyle)
            {
                path.AddLine(bounds.Right, bounds.Bottom - 2, bounds.Left, bounds.Bottom - 2);
            }
            path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 2);
            path.AddLine(bounds.Left - 1, bounds.Top + c_cornerCut - 1, bounds.Left + c_cornerCut - 1, bounds.Top - 1);
            path.AddLine(bounds.Left, bounds.Top - 1, bounds.Right, bounds.Top - 1);
            path.AddLine(bounds.Right - c_cornerCut, bounds.Top - 1, bounds.Right, bounds.Top + c_cornerCut - 1);
            path.AddLine(bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 2);

            return path;
        }

        /// <summary>
        /// Gets inner borders for bottom tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerBottomAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.BorderWidth;

            path.AddLine(bounds.Left + width - 1, bounds.Top + width - 1, bounds.Left + width - 1, bounds.Bottom - 2);
            path.AddLine(bounds.Left + width - 1, bounds.Top + width - 2, bounds.Right - width, bounds.Top + width - 2);
            path.AddLine(bounds.Right - width, bounds.Top + width - 1, bounds.Right - width, bounds.Bottom - 2);

            return path;
        }

        /// <summary>
        /// Gets outer borders for left tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterLeftAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            if (!this.IsOffice2007Style && !this.IsOffice2010Style && !this.IsDockingWhidbeyStyle)
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            }
            path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            path.AddLine(bounds.Right - c_cornerCut, bounds.Top, bounds.Right, bounds.Top + c_cornerCut);
            path.AddLine(bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            path.AddLine(bounds.Right, bounds.Bottom - c_cornerCut - 1, bounds.Right - c_cornerCut, bounds.Bottom - 1);
            path.AddLine(bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);

            return path;
        }

        /// <summary>
        /// Gets inner borders for left tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerLeftAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.BorderWidth;

            path.AddLine(bounds.Left, bounds.Top + width - 1, bounds.Right - width, bounds.Top + width - 1);
            path.AddLine(bounds.Right - width, bounds.Top + width - 1, bounds.Right - width, bounds.Bottom - width);
            path.AddLine(bounds.Left, bounds.Bottom - width, bounds.Right - width, bounds.Bottom - width);

            return path;
        }

        /// <summary>
        /// Gets outer borders for right tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterRightAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            if (!this.IsOffice2007Style && !this.IsOffice2010Style && !this.IsDockingWhidbeyStyle)
            {
                path.AddLine(bounds.Right - 2, bounds.Top, bounds.Right - 2, bounds.Bottom);
            }
            path.AddLine(bounds.Left, bounds.Bottom - 1, bounds.Right - 2, bounds.Bottom - 1);
            path.AddLine(bounds.Left + c_cornerCut - 1, bounds.Bottom, bounds.Left - 1, bounds.Bottom - c_cornerCut);
            path.AddLine(bounds.Left - 1, bounds.Bottom, bounds.Left - 1, bounds.Top);
            path.AddLine(bounds.Left - 2, bounds.Top + c_cornerCut, bounds.Left + c_cornerCut - 2, bounds.Top);
            path.AddLine(bounds.Left, bounds.Top, bounds.Right - 2, bounds.Top);

            return path;
        }

        /// <summary>
        /// Gets inner borders for right tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerRightAlignmentBordersPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.BorderWidth;

            path.AddLine(bounds.Right - 2, bounds.Bottom - width, bounds.Left + width - 1, bounds.Bottom - width);
            path.AddLine(bounds.Left + width - 2, bounds.Bottom - width, bounds.Left + width - 2, bounds.Top + width - 1);
            path.AddLine(bounds.Left + width - 1, bounds.Top + width - 1, bounds.Right - 2, bounds.Top + width - 1);

            return path;
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            ValidateTabPagesVisibility();

            base.OnPaint(e);

            int closeButtonWidth = 0;
            MDITabPanel panel = this as MDITabPanel;
            if (panel != null && panel.IsCloseButtonActive())
            {
                closeButtonWidth = MDITabPanel.CLOSE_BUTTON_AREA_WIDTH + c_CLOSE_BUTTON_PADDING;
            }

            int dist = this.Width;
            if (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right)
            {
                dist = this.Height;
            }

            if (dist - m_sbScrollButtons.ClientRectangle.Width - closeButtonWidth <= 0)
            {
                m_bShowScroll = false;
                DestroyScrollButtons(false);
            }
            else
            {
                if (!m_bShowScroll && m_bCachedShowScroll)
                {
                    InitScrollButtons();
                }
                m_bShowScroll = m_bCachedShowScroll;
            }

            bool bNeedLayout = this.NeedLayout;

            if (bNeedLayout)
            {
                this.Layout(e.Graphics, true);
            }

            Office2003Colors.UpdateMenuColors();
            ResetChildControls();

            // Take a look at TabControlAdv.Init for notes on why we need this check.			
            // Background
            //e.Graphics.FillRectangle(new SolidBrush( Office2003Colors.MenuItemHotColorDark ), this.ClientRectangle);

            DrawPanelBackground(e.Graphics);

            if (this.Width - m_sbScrollButtons.ClientRectangle.Width - closeButtonWidth > 0)
            {
                Draw3DBorder(e.Graphics, Rectangle.Round(GetBorderRect()));
            }

            if (this.IsOffice2007Style)
            {
                DrawBorderForOffice2007(e.Graphics);
            }

            if (this.IsOffice2010Style)
            {
                DrawBorderForOffice2010(e.Graphics);
            }

            if (this.TabStyle == typeof(TabRendererVS2008))
            {
                DrawVS2008Borders(e.Graphics);
            }

            m_tabPanelRenderer.OnPaint(e.Graphics, ClientRectangle);

            if ((this.IsOffice2003Style || this.IsWhidbeyStyle) && !this.BorderVisible)
            {
                DrawAdditionalBorders(e.Graphics);
            }

            if (this.TabStyle == typeof(TabGroupRendererOffice2003))
            {
                DrawGroupOffice2003Borders(e.Graphics);
            }

            

            if (this.IsEditing && this.SelectedTab != null)
            {
                RefreshLabelEdit();
            }

            if (this.TabStyle == typeof(TabRendererMetro))
            {
                if (NeedLayout)
                    this.Layout(e.Graphics, true);
                using(Brush brush =new SolidBrush(this.BackColor))
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                DrawMetroBorders(e.Graphics);
                this.Renderer.OnPaint(e.Graphics, e.ClipRectangle);
                DrawPanelBackground(e.Graphics);
            }
            if (this.TabCount > 0)
            {
                m_tabPrimitivesHost.HandledOnPaint(e.Graphics);
            }
        }
        protected void DrawMetroBorders(Graphics g)
        {                    
            if (BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
            {
                Rectangle pRect = new Rectangle((int)this.GetPanelBounds().X, (int)this.GetPanelBounds().Y, (int)this.GetPanelBounds().Width, (int)this.GetPanelBounds().Height);
                Rectangle bRect = this.Bounds;
                Point tabBounds = Point.Empty;
                Size tabSize = Size.Empty;

                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        tabBounds.X = 0;
                        tabBounds.Y = pRect.Bottom - 1;
                        tabSize.Width = pRect.Right - 1;
                        tabSize.Height = bRect.Height - tabBounds.Y - 1;
                        break;
                    case TabAlignment.Bottom:
                        tabBounds.X = 0;
                        tabBounds.Y = 0;
                        tabSize.Width = pRect.Right - 1;
                        tabSize.Height = bRect.Bottom - tabSize.Height - 1;
                        break;
                    case TabAlignment.Right:
                        tabBounds.X = 0;
                        tabBounds.Y = 0;
                        tabSize.Width = bRect.Width - pRect.Width;
                        tabSize.Height = bRect.Height - 1;
                        break;
                    case TabAlignment.Left:
                        tabBounds.X = pRect.Right;
                        tabBounds.Y = 0;
                        tabSize.Width = bRect.Width - pRect.Right - 1;
                        tabSize.Height = pRect.Bottom - 1;
                        break;
                }
                using (Pen pen = new Pen(FixedSingleBorderColor))
                {
                    g.DrawRectangle(pen, tabBounds.X, tabBounds.Y, tabSize.Width, tabSize.Height);
                }
            }
            else if(BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D)
            {
                Draw3DBorder(g, Rectangle.Round(GetBorderRect()));
            }
        }
        protected void DrawVS2008Borders(Graphics g)
        {
            Rectangle borderBounds = Rectangle.Round(this.GetPanelBounds());

            GraphicsPath gpOuter = new GraphicsPath();
            GraphicsPath gpInner = new GraphicsPath();
            Point leftPoint = Point.Empty;
            Point rightPoint = Point.Empty;

            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    gpOuter = this.GetTopAdditionalBordersPath(borderBounds, c_cornerCut);
                    gpInner = this.GetTopAdditionalBordersPath(new Rectangle(borderBounds.Left + 1, borderBounds.Top + 1, borderBounds.Right - 2, borderBounds.Bottom), c_cornerCut - 1);
                    leftPoint = new Point(borderBounds.Left + this.BorderWidth, borderBounds.Bottom - 1);
                    rightPoint = new Point(borderBounds.Right - this.BorderWidth, borderBounds.Bottom - 1);
                    break;

                case TabAlignment.Right:
                    gpOuter = this.GetRightAdditionalBordersPath(borderBounds, c_cornerCut);
                    gpInner = this.GetRightAdditionalBordersPath(new Rectangle(borderBounds.Left - 1, borderBounds.Top + 1, borderBounds.Right - 1, borderBounds.Bottom - 2), c_cornerCut - 1);
                    leftPoint = new Point(borderBounds.Right - borderBounds.Width, borderBounds.Top + this.BorderWidth);
                    rightPoint = new Point(borderBounds.Right - borderBounds.Width, borderBounds.Bottom - this.BorderWidth);
                    break;

                case TabAlignment.Bottom:
                    borderBounds.Y--;
                    gpOuter = this.GetBottomAdditionalBordersPath(borderBounds, c_cornerCut);
                    gpInner = this.GetBottomAdditionalBordersPath(new Rectangle(borderBounds.Left + 1, borderBounds.Top - 1, borderBounds.Right - 2, borderBounds.Bottom - 2), c_cornerCut - 1);
                    leftPoint = new Point(borderBounds.Left + this.BorderWidth, borderBounds.Top + 1);
                    rightPoint = new Point(borderBounds.Right - this.BorderWidth, borderBounds.Top + 1);
                    break;

                case TabAlignment.Left:
                    borderBounds.X--;
                    gpOuter = this.GetLeftAdditionalBordersPath(borderBounds, c_cornerCut);
                    gpInner = this.GetLeftAdditionalBordersPath(new Rectangle(borderBounds.Left + 1, borderBounds.Top + 1, borderBounds.Right + 1, borderBounds.Bottom - 2), c_cornerCut - 1);
                    leftPoint = new Point(borderBounds.Left + borderBounds.Width, borderBounds.Top + this.BorderWidth);
                    rightPoint = new Point(borderBounds.Left + borderBounds.Width, borderBounds.Bottom - this.BorderWidth);
                    break;
            }

            Color borderColor = WindowsXPThemeColors.TabControlAdvActiveBorderColor;
            Color innerBorderColor = WindowsXPThemeColors.TabControlAdvLightBorderColor;
            Color fillColor = WindowsXPThemeColors.TabControlAdvActiveBottomTabColor;
            if (this.IsVistaOS)
            {
                borderColor = Color.FromArgb(127, 157, 185);
                innerBorderColor = Color.White;
                fillColor = Color.FromArgb(210, 230, 250);
            }

            using (Brush brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, gpOuter);
            }
            using (Pen borderPen = new Pen(borderColor))
            {
                g.DrawPath(borderPen, gpOuter);
                g.DrawLine(borderPen, leftPoint, rightPoint);
            }
            using (Pen borderPenInner = new Pen(innerBorderColor))
            {
                g.DrawPath(borderPenInner, gpInner);
            }

            gpOuter.Dispose();
            gpInner.Dispose();
        }

        /// <summary>
        /// Border path for additional borders for VS2008 style.
        /// </summary>
        /// <param name="borderBounds"></param>
        /// <returns></returns>
        private GraphicsPath GetTopAdditionalBordersPath(Rectangle borderBounds, int cornerCut)
        {
            GraphicsPath gp = new GraphicsPath();

            PointF[] points = new PointF[]
                                        {
                                            new PointF( borderBounds.Left, borderBounds.Bottom ),
                                            new PointF( borderBounds.Left, borderBounds.Bottom - this.BorderWidth + cornerCut - 1 ),
                                            new PointF( borderBounds.Left + cornerCut - 1, borderBounds.Bottom - this.BorderWidth ),
                                            new PointF( borderBounds.Right - cornerCut, borderBounds.Bottom - this.BorderWidth ),
                                            new PointF( borderBounds.Right - 1, borderBounds.Bottom - this.BorderWidth + cornerCut - 1 ),
                                            new PointF( borderBounds.Right - 1, borderBounds.Bottom ),
                                        };
            gp.AddLines(points);

            return gp;
        }

        /// <summary>
        /// Border path for additional borders for VS2008 style.
        /// </summary>
        /// <param name="borderBounds"></param>
        /// <returns></returns>
        private GraphicsPath GetBottomAdditionalBordersPath(Rectangle borderBounds, int cornerCut)
        {
            GraphicsPath gp = new GraphicsPath();

            PointF[] points = new PointF[]
                                        {
                                            new PointF( borderBounds.Left, borderBounds.Top + 1 ),
                                            new PointF( borderBounds.Left, borderBounds.Top + this.BorderWidth - cornerCut + 1 ),
                                            new PointF( borderBounds.Left + cornerCut - 1, borderBounds.Top + this.BorderWidth ),
                                            new PointF( borderBounds.Right - cornerCut, borderBounds.Top + this.BorderWidth ),
                                            new PointF( borderBounds.Right - 1, borderBounds.Top + this.BorderWidth - cornerCut + 1 ),
                                            new PointF( borderBounds.Right - 1, borderBounds.Top + 1 ),
                                        };
            gp.AddLines(points);

            return gp;
        }

        /// <summary>
        /// Border path for additional borders for VS2008 style.
        /// </summary>
        /// <param name="borderBounds"></param>
        /// <returns></returns>
        private GraphicsPath GetLeftAdditionalBordersPath(Rectangle borderBounds, int cornerCut)
        {
            GraphicsPath gp = new GraphicsPath();

            PointF[] points = new PointF[]
                                        {
                                            new PointF( borderBounds.Left + borderBounds.Width + 1, borderBounds.Top ),
                                            new PointF( borderBounds.Left - this.BorderWidth + borderBounds.Width + cornerCut, borderBounds.Top ),
                                            new PointF( borderBounds.Left - this.BorderWidth + borderBounds.Width + 1, borderBounds.Top + cornerCut - 1 ),
                                            new PointF( borderBounds.Left - this.BorderWidth + borderBounds.Width + 1, borderBounds.Bottom - cornerCut ),
                                            new PointF( borderBounds.Left - this.BorderWidth + borderBounds.Width + cornerCut, borderBounds.Bottom - 1 ),
                                            new PointF( borderBounds.Left + borderBounds.Width + 1, borderBounds.Bottom - 1 ),
                                        };
            gp.AddLines(points);

            return gp;
        }

        /// <summary>
        /// Border path for additional borders for VS2008 style.
        /// </summary>
        /// <param name="borderBounds"></param>
        /// <returns></returns>
        private GraphicsPath GetRightAdditionalBordersPath(Rectangle borderBounds, int cornerCut)
        {
            GraphicsPath gp = new GraphicsPath();

            PointF[] points = new PointF[]
                                        {
                                            new PointF( borderBounds.Right - borderBounds.Width, borderBounds.Top ),
                                            new PointF( borderBounds.Right + this.BorderWidth - borderBounds.Width - cornerCut, borderBounds.Top ),
                                            new PointF( borderBounds.Right + this.BorderWidth - borderBounds.Width - 1, borderBounds.Top + cornerCut - 1 ),
                                            new PointF( borderBounds.Right + this.BorderWidth - borderBounds.Width - 1, borderBounds.Bottom - cornerCut ),
                                            new PointF( borderBounds.Right + this.BorderWidth - borderBounds.Width - cornerCut, borderBounds.Bottom - 1 ),
                                            new PointF( borderBounds.Right - borderBounds.Width, borderBounds.Bottom - 1 ),
                                        };
            gp.AddLines(points);

            return gp;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void DrawGroupOffice2003Borders(Graphics g)
        {
            Rectangle borderBounds = Rectangle.Round(this.GetPanelBounds());
            Point upperLeft;
            Point lowerRight;
            Pen borderPen = new Pen(Office2003Colors.SelBorderColor);

            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    upperLeft = new Point(borderBounds.X, borderBounds.Y);
                    lowerRight = new Point(borderBounds.X + borderBounds.Width, borderBounds.Y);
                    break;

                case TabAlignment.Right:
                    upperLeft = new Point(borderBounds.X + borderBounds.Width - 1, borderBounds.Y);
                    lowerRight = new Point(upperLeft.X, borderBounds.Y + borderBounds.Height);
                    break;

                case TabAlignment.Bottom:
                    upperLeft = new Point(borderBounds.X, borderBounds.Y + borderBounds.Height - 1);
                    lowerRight = new Point(upperLeft.X + borderBounds.Width - 1, upperLeft.Y);
                    break;

                case TabAlignment.Left:
                    upperLeft = new Point(borderBounds.X, borderBounds.Y);
                    lowerRight = new Point(upperLeft.X, upperLeft.Y + borderBounds.Height);
                    break;

                default:
                    throw new ArgumentException("Unknown alignment!");
            }

            g.DrawLine(borderPen, upperLeft, lowerRight);
            borderPen.Dispose();
        }

        /// <summary>
        /// Draws the borders of Whidbey style tabs.
        /// </summary>
        protected void DrawBordersVS2005(Graphics g)
        {
            // Get Panel's bounds
            Rectangle borderBounds = Rectangle.Round(this.GetPanelBounds());
            int borderWidth = TabRendererOffice2003.DEF_BORDER_WIDTH;

            switch (Alignment)
            {
                case TabAlignment.Left:
                    borderBounds.Width -= borderWidth * 2;
                    if (this.IsMirrored)
                    {
                        borderBounds.Width -= borderWidth;
                    }
                    borderBounds.Height -= borderWidth;
                    break;

                case TabAlignment.Right:
                    borderBounds.Offset(borderWidth, 0);
                    if (!this.IsMirrored)
                    {
                        borderBounds.Offset(borderWidth, 0);
                    }
                    break;

                case TabAlignment.Top:
                    borderBounds.Height -= borderWidth * 2;
                    borderBounds.Width -= borderWidth;
                    break;

                case TabAlignment.Bottom:
                    borderBounds.Height -= borderWidth * 3;
                    borderBounds.Width -= borderWidth;

                    borderBounds.Offset(0, borderWidth * 2);
                    break;
            }

            using (Pen borderPen = new Pen(TabRendererWhidbey.DEF_SELECTED_BORDER_COLOR, borderWidth))
            {
                switch (Alignment)
                {
                    case TabAlignment.Left:
                        g.DrawLine(borderPen, borderBounds.Right, borderBounds.Top, borderBounds.Right, borderBounds.Bottom);
                        break;

                    case TabAlignment.Right:
                        g.DrawLine(borderPen, borderBounds.Left, borderBounds.Top, borderBounds.Left, borderBounds.Bottom);
                        break;

                    case TabAlignment.Top:
                        g.DrawLine(borderPen, borderBounds.Left, borderBounds.Bottom, borderBounds.Right, borderBounds.Bottom);
                        break;

                    case TabAlignment.Bottom:
                        g.DrawLine(borderPen, borderBounds.Left, borderBounds.Top, borderBounds.Right, borderBounds.Top);
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the borders of office2003 tabs.
        /// </summary>
        protected void DrawBordersOffice2003(Graphics g)
        {
            // Get Panel's bounds
            Rectangle borderBounds = Rectangle.Round(this.GetPanelBounds());
            int borderWidth = TabRendererOffice2003.DEF_BORDER_WIDTH;

            switch (Alignment)
            {
                case TabAlignment.Left:
                    borderBounds.Width -= borderWidth * 2;
                    if (this.IsMirrored)
                    {
                        borderBounds.Width -= borderWidth;
                    }
                    borderBounds.Height -= borderWidth;
                    break;

                case TabAlignment.Top:
                    borderBounds.Height -= borderWidth * 2;
                    borderBounds.Width -= borderWidth;
                    break;

                case TabAlignment.Bottom:
                    borderBounds.Height -= borderWidth * 3;
                    borderBounds.Width -= borderWidth;

                    borderBounds.Offset(0, borderWidth * 2);
                    break;

                case TabAlignment.Right:
                    borderBounds.Width -= borderWidth * 2;
                    borderBounds.Height -= borderWidth;
                    borderBounds.Offset(borderWidth, 0);
                    if (!this.IsMirrored)
                    {
                        borderBounds.Offset(borderWidth, 0);
                        borderBounds.Width -= borderWidth;
                    }
                    break;
            }

            // Draw border highlighted rectanle for Office 2003 style
            if (IsOffice2003Style)
            {
                using (Pen borderPen = new Pen(Office2003Colors.SelBorderColor, borderWidth))
                {
                    g.DrawRectangle(borderPen, borderBounds);
                }
            }
        }

        /// <summary>
        /// Draws the additional borders for Office2003 or Whidbey style.
        /// </summary>
        protected virtual void DrawAdditionalBorders(Graphics g)
        {
            if (this.IsOffice2003Style)
            {
                DrawBordersOffice2003(g);
            }
            else if (this.IsWhidbeyStyle)
            {
                DrawBordersVS2005(g);
            }
        }

        private ArrayList childControls;
        /// <summary>
        /// Holds the child controls removed by the designer, in a arraylist.
        /// </summary>
        /// <param name="childControls"></param>
        [Browsable(false),
        Syncfusion.Documentation.DocumentationExclude()]
        public void ChildControlsRemovedByDesigner(ArrayList childControls)
        {
            this.childControls = childControls;
            this.Invalidate(false);
        }
        private void ResetChildControls()
        {
            if (childControls != null)
            {
                if (childControls.Count > 0)
                {
                    foreach (Control childControl in childControls)
                        this.Controls.Add(childControl);
                }
                childControls.Clear();
            }
        }

        private RectangleF GetPanelBounds()
        {
            RectangleF panelBounds = RectangleF.Empty;

            if (this.Alignment == TabAlignment.Top || this.Alignment == TabAlignment.Bottom)
            {
                panelBounds = new RectangleF(this.ClientRectangle.Left, m_internalTabPanelBounds.Top,
                    this.ClientRectangle.Width, m_internalTabPanelBounds.Height);
            }
            else
            {
                panelBounds = new RectangleF(m_internalTabPanelBounds.Left, this.ClientRectangle.Top,
                    m_internalTabPanelBounds.Width, this.ClientRectangle.Height);
            }

            return panelBounds;
        }

        private RectangleF CorrectFillRectangle(RectangleF bounds)
        {
            int selectionLineWidth = TabRendererOffice2003.DEF_SELECTION_LINE_WIDTH;

            if (this.IsOffice2003Style)
            {
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        bounds.Height -= selectionLineWidth;
                        break;

                    case TabAlignment.Bottom:
                        bounds.Height -= (selectionLineWidth);
                        bounds.Offset(0, selectionLineWidth +
                                TabRendererOffice2003.DEF_BORDER_WIDTH);
                        break;

                    case TabAlignment.Left:
                        bounds.Width -= selectionLineWidth;
                        bounds.Width -= TabRendererOffice2003.DEF_BORDER_WIDTH;
                        break;

                    case TabAlignment.Right:
                        bounds.Width -= selectionLineWidth;
                        bounds.Offset(selectionLineWidth, 0);
                        break;
                }
            }
            else if (this.IsVS2010)
            {
                selectionLineWidth = TabRendererVS2010.DEF_SELECTION_LINE_WIDTH;
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        bounds.Offset(0, bounds.Y + bounds.Height - selectionLineWidth);
                        bounds.Height = selectionLineWidth;
                        break;

                    case TabAlignment.Bottom:
                        bounds.Height = (selectionLineWidth);
                        break;

                    case TabAlignment.Left: 
                        bounds.Offset(bounds.Width - selectionLineWidth,0);
                        bounds.Width = selectionLineWidth;
                        break;

                    case TabAlignment.Right:
                        bounds.Width = selectionLineWidth;
                        break;
                }
            }

            return bounds;
        }

        /// <summary>
        /// Draws the background for the tab panel.
        /// </summary>
        /// <param name="g">The Graphics object into which to draw.</param>
        /// <remarks>
        /// This method will paint the background of the tabs and the scroll button
        /// area, if any. However, the background of the tabs will again be repainted
        /// by the corresponding <see cref="ITabRenderer"/> (corresponding to the
        /// specified tab style).
        /// </remarks>
        protected virtual void DrawPanelBackground(Graphics g)
        {
            bool bIsOffice2003Style = this.IsOffice2003Style;

            RectangleF rectFill = GetPanelBounds();

            // fill background with selected item color
            if (bIsOffice2003Style && SelectedIndex >= 0)
            {
                using (Brush backBrush = new SolidBrush(Office2003Colors.MenuItemHotColorDark))
                {
                    g.FillRectangle(backBrush, ClientRectangle);
                }
            }
            else if (IsOffice2007Style)
            {
                using (Brush backBrush = new SolidBrush(TabPanelBackColor))
                {
                    rectFill = new RectangleF(rectFill.X, rectFill.Y + StyleRendererPropertyOffice2007.OverlapHeight,
                        rectFill.Width, rectFill.Height);
                    g.FillRectangle(backBrush, rectFill);
                }
            }
            else if (IsOffice2010Style)
            {
                using (Brush backBrush = new SolidBrush(TabPanelBackColor))
                {
                    rectFill = new RectangleF(rectFill.X, rectFill.Y + StyleRendererPropertyOffice2010.OverlapHeight,
                        rectFill.Width, rectFill.Height);
                    g.FillRectangle(backBrush, rectFill);
                }
            }
            else if (IsOneNoteStyle || IsOneNoteStyleFlatTabs)
            {
                if (SelectedIndex >= 0)
                {
                    rectFill = CorrectFillRectangle(rectFill);
                }

                if (rectFill.Width > 0 && rectFill.Height > 0)
                {
                    Color backColor = TabPanelData.BackColor;
                    if (backColor == Color.Empty)
                        backColor = m_tabPanelDefaultProperties.DefaultTabPanelBackgroundColor();

                    // Make sure to specify in the overload below that the bg is not solid.
                    using (Brush backgroundBrush = GetOffice2003FillBrush(rectFill, backColor))
                    {
                        g.FillRectangle(backgroundBrush, rectFill);
                    }
                }
            }
            else if (IsIE7Style || IsWhidbeyStyle || IsDockingWhidbeyStyleBeta || IsDockingWhidbeyStyle)
            {
                using (Brush backBrush = new SolidBrush(TabPanelBackColor))
                {
                    g.FillRectangle(backBrush, rectFill);
                }
            }
            else if(IsVS2010)
            {
                Color tabBackColor = Color.FromArgb(41, 57, 85);
                using (Brush brush = new SolidBrush(tabBackColor))
                {
                    g.FillRectangle(brush,this.ClientRectangle);
                }
            }
            // Take a look at TabControlAdv.Init for notes on why we need this check.
            if (BackColor != TabPanelBackColor
                && !IsOneNoteStyle && !IsOffice2007Style && !IsOffice2010Style && !IsOneNoteStyleFlatTabs)
            {
                Point p = new Point(0, m_sbScrollButtons.Top);

                // offset fill reftangle so that 'highlight' line is visible
                if (bIsOffice2003Style)
                {
                    if (SelectedIndex >= 0)
                    {
                        rectFill = CorrectFillRectangle(rectFill);
                    }

                    if (rectFill.Width > 0 && rectFill.Height > 0)
                    {
                        Color backColor = TabPanelData.BackColor;
                        if (backColor == Color.Empty)
                            backColor = m_tabPanelDefaultProperties.DefaultTabPanelBackgroundColor();

                        // Make sure to specify in the overload below that the bg is not solid.
                        using (Brush backgroundBrush = GetOffice2003FillBrush(rectFill, backColor))
                        {
                            g.FillRectangle(backgroundBrush, rectFill);
                        }
                    }
                }
                else if (this.IsDockingWhidbeyStyle)
                {
                    using(Brush brush =new SolidBrush(this.TabPanelBackColor))
                        g.FillRectangle(brush, rectFill);
                    DrawBorderForDockingWhidbey(g, rectFill);
                }
                else if (this.IsVS2010)
                {
                    rectFill = CorrectFillRectangle(rectFill);
                    Color bg = Color.FromArgb(255, 232, 166);
                    using (SolidBrush brush = new SolidBrush(bg))
                        g.FillRectangle(brush, rectFill);
                }
                else if (this.IsMetroStyle)
                {
                    using (Brush backBrush = new SolidBrush(Color.Blue))
                    {
                        RectangleF rect = new RectangleF(0, rectFill.Top + (rectFill.Height - 3), rectFill.Width, 3);
                        using (SolidBrush brush = new SolidBrush(this.ActiveTabColor))
                            g.FillRectangle(brush, rect);
                    }
                }
                else
                {
                    // Fill the whole panel
                    using (SolidBrush brush = new SolidBrush(this.TabPanelBackColor))
                        g.FillRectangle(brush, rectFill);
                }
            }
        }

        /// <summary>
        /// Draw border for Docking Whidbey style.
        /// </summary>
        /// <param name="pGraphics"> Graphics object. </param>
        /// <param name="pRect"> Rectangle in which we draw borders. </param>
        private void DrawBorderForDockingWhidbey(Graphics pGraphics, RectangleF pRect)
        {
            PointF ptBeginLight = PointF.Empty;
            PointF ptEndLight = PointF.Empty;
            PointF ptBeginDark = PointF.Empty;
            PointF ptEndDark = PointF.Empty;

            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    ptBeginDark.X = 0;
                    ptBeginDark.Y = pRect.Bottom - 3;
                    ptEndDark.X = pRect.Width;
                    ptEndDark.Y = pRect.Bottom - 3;
                    ptBeginLight.X = 0;
                    ptBeginLight.Y = pRect.Bottom - 2;
                    ptEndLight.X = pRect.Width;
                    ptEndLight.Y = pRect.Bottom - 2;

                    break;

                case TabAlignment.Bottom:
                    ptBeginDark.X = 0;
                    ptBeginDark.Y = pRect.Top + 2;
                    ptEndDark.X = pRect.Width;
                    ptEndDark.Y = pRect.Top + 2;
                    ptBeginLight.X = 0;
                    ptBeginLight.Y = pRect.Top + 1;
                    ptEndLight.X = pRect.Width;
                    ptEndLight.Y = pRect.Top + 1;

                    break;

                case TabAlignment.Right:
                    ptBeginDark.X = pRect.Left + 2;
                    ptBeginDark.Y = 0;
                    ptEndDark.X = pRect.Left + 2;
                    ptEndDark.Y = pRect.Height;
                    ptBeginLight.X = pRect.Left + 1;
                    ptBeginLight.Y = 0;
                    ptEndLight.X = pRect.Left + 1;
                    ptEndLight.Y = pRect.Height;

                    break;

                case TabAlignment.Left:
                    ptBeginDark.X = pRect.Right - 3;
                    ptBeginDark.Y = 0;
                    ptEndDark.X = pRect.Right - 3;
                    ptEndDark.Y = pRect.Height;
                    ptBeginLight.X = pRect.Right - 2;
                    ptBeginLight.Y = 0;
                    ptEndLight.X = pRect.Right - 2;
                    ptEndLight.Y = pRect.Height;

                    break;
            }

            // Draw borders.
            using (Pen penDark = new Pen(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                pGraphics.DrawLine(penDark, ptBeginDark, ptEndDark);
            }
            using (Pen penLight = new Pen(Color.White))
            {
                pGraphics.DrawLine(penLight, ptBeginLight, ptEndLight);
            }
        }

        /// <summary>
        /// Used for drawing borders for Office2007 style.
        /// </summary>
        /// <param name="pGraphics"> Graphics object. </param>
        private void DrawBorderForOffice2007(Graphics pGraphics)
        {
            RectangleF pRect = this.GetPanelBounds();

            PointF ptBegin = PointF.Empty;
            PointF ptEnd = PointF.Empty;

            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    ptBegin.X = 0;
                    ptBegin.Y = pRect.Bottom - 1;
                    ptEnd.X = pRect.Width;
                    ptEnd.Y = pRect.Bottom - 1;

                    break;

                case TabAlignment.Bottom:
                    ptBegin.X = 0;
                    ptBegin.Y = pRect.Top + 1;
                    ptEnd.X = pRect.Width;
                    ptEnd.Y = pRect.Top + 1;

                    break;

                case TabAlignment.Right:
                    ptBegin.X = pRect.Left + 1;
                    ptBegin.Y = 0;
                    ptEnd.X = pRect.Left + 1;
                    ptEnd.Y = pRect.Height;

                    break;

                case TabAlignment.Left:
                    ptBegin.X = pRect.Right - 1;
                    ptBegin.Y = 0;
                    ptEnd.X = pRect.Right - 1;
                    ptEnd.Y = pRect.Height;

                    break;
            }

            using (Pen pen = new Pen(this.Office2007ColorTable.TabDefaultBorderColor))
            {
                pGraphics.DrawLine(pen, ptBegin, ptEnd);
            }
        }
        /// <summary>
        /// Used for drawing borders for Office2010 style.
        /// </summary>
        /// <param name="pGraphics"> Graphics object. </param>
        private void DrawBorderForOffice2010(Graphics pGraphics)
        {
            RectangleF pRect = this.GetPanelBounds();

            PointF ptBegin = PointF.Empty;
            PointF ptEnd = PointF.Empty;

            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    ptBegin.X = 0;
                    ptBegin.Y = pRect.Bottom - 1;
                    ptEnd.X = pRect.Width;
                    ptEnd.Y = pRect.Bottom - 1;

                    break;

                case TabAlignment.Bottom:
                    ptBegin.X = 0;
                    ptBegin.Y = pRect.Top + 1;
                    ptEnd.X = pRect.Width;
                    ptEnd.Y = pRect.Top + 1;

                    break;

                case TabAlignment.Right:
                    ptBegin.X = pRect.Left + 1;
                    ptBegin.Y = 0;
                    ptEnd.X = pRect.Left + 1;
                    ptEnd.Y = pRect.Height;

                    break;

                case TabAlignment.Left:
                    ptBegin.X = pRect.Right - 1;
                    ptBegin.Y = 0;
                    ptEnd.X = pRect.Right - 1;
                    ptEnd.Y = pRect.Height;

                    break;
            }

            using (Pen pen = new Pen(this.Office2010ColorTable.TabDefaultBorderColor))
            {
                pGraphics.DrawLine(pen, ptBegin, ptEnd);
            }
        }
        private Brush GetOffice2003FillBrush(RectangleF rectFill, Color backColor)
        {
            Brush brush = null;
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    brush = new LinearGradientBrush(rectFill, backColor,
                               Color.White, LinearGradientMode.Vertical);
                    break;

                case TabAlignment.Bottom:
                    brush = new LinearGradientBrush(rectFill, Color.White,
                        backColor, LinearGradientMode.Vertical);
                    break;

                case TabAlignment.Left:
                    brush = new LinearGradientBrush(rectFill, backColor,
                         Color.White, LinearGradientMode.Horizontal);
                    break;

                case TabAlignment.Right:
                    brush = new LinearGradientBrush(rectFill, Color.White,
                        backColor, LinearGradientMode.Horizontal);
                    break;
            }

            return brush;
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <returns>The border rectangle.</returns>
        /// <remarks>
        /// <para>This is the border drawn by the tab control excluding the tab panel.</para>
        /// </remarks>
        protected virtual RectangleF GetBorderRect()
        {
            RectangleF borderRect = RectangleF.Empty;
            RectangleF CR = this.ClientRectangle;

            bool bIsMirrored = IsMirrored;
            switch (this.m_tabPanelData.Alignment)
            {
                case TabAlignment.Top:
                    float top = m_internalTabPanelBounds.Bottom - DEF_ADJUST_POS;//CR.Top + tabPanelBounds.Height - 1;
                    borderRect = new RectangleF(CR.Left, top, CR.Width, CR.Bottom - top);
                    break;

                case TabAlignment.Bottom:
                    float bottom = m_internalTabPanelBounds.Top + DEF_ADJUST_POS;//CR.Bottom - tabPanelBounds.Height + 1;
                    borderRect = new RectangleF(CR.Left, CR.Top + DEF_ADJUST_POS, CR.Width, bottom - CR.Top);
                    break;

                case TabAlignment.Left:
                    {
                        float left = m_internalTabPanelBounds.Right - DEF_ADJUST_POS;//CR.Left + tabPanelBounds.Width - 1;
                        // Shade is drawn on the left of tab control for RTL, so stretch area to the left
                        // for correct shade drawing
                        if (bIsMirrored && !this.BorderVisible)
                        {
                            left -= 1;
                        }
                        float width = CR.Right - left;
                        borderRect = new RectangleF(left, CR.Top, width, CR.Height);
                        break;
                    }

                case TabAlignment.Right:
                    {
                        float right = m_internalTabPanelBounds.Left + DEF_ADJUST_POS;//CR.Right - tabPanelBounds.Width;
                        // Shade is drawn on the right of tab control for RTL, so adjust area to the right
                        // for correct shade drawing
                        if (bIsMirrored && !this.BorderVisible)
                        {
                            right -= 1;
                        }

                        float width = right - CR.Left;
                        borderRect = new RectangleF(CR.Left + DEF_ADJUST_POS, CR.Top, width, CR.Height);
                        break;
                    }

            }

            return borderRect;
        }

        /// <summary>
        /// Forces the laying out of tab control elements.
        /// </summary>
        /// <param name="g">The Graphics object using which to calculate element sizes and positions.</param>
        /// <remarks>
        /// Advanced method. You do not have to call this directly.
        /// </remarks>
        protected new virtual void Layout(Graphics g, bool fromPaint)
        {
            if (!m_bIsDisposing)
            {
                // This would be preferrable but causes problems on initial tab display (width or height is always 0)
                //
                // Prevent layout when width or height is 0. (Noticed some problems with minimizing and hiding followed by restoring and showing)
                //if(this.Width != 0 && this.Height != 0)
                //	return;

                SetNeedLayout(false);

                this.m_tabPanelRenderer.Layout(g, fromPaint);

                ComputeTabPanelBounds();
                UpdateSelectedTabPage(true);
            }
        }

        internal bool IsDesignerLoading
        {
            get
            {
                bool isLoading = false;
                IDesignerHost host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
                isLoading = (host != null && host.Loading);

                return isLoading;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnLayout"/>.
        /// </summary>
        /// <param name="levent">A LayoutEventArgs that contains the event data.</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (levent.AffectedControl == this || this.dockJustChanged)
            {
                if (this.dockJustChanged)
                    this.dockJustChanged = false;

                // OnLayout will be called when the form gets minimized, in that scenario
                // we do not want to layout.
                // And do not create the Handle at this point (this could be called from InitializeComponent)
                if (this.Width != 0 && this.Height != 0 && this.IsHandleCreated)
                {
                    // SetNeedLayout here and layout in paint is not acceptable
                    // as when resizing happens fast, the paint messages stop firing at one point!

                    Graphics g = this.CreateGraphics();
                    this.Layout(g, false);
                    g.Dispose();
                }
            }

            base.OnLayout(levent);
        }

        /// </override>
        protected override void OnDockChanged(EventArgs e)
        {
            // Ideally the OnLayout should be called with AffectedControl == this.
            // But that does not happen. So, we will force a Layout by setting
            // this flag and querying this in OnLayout.
            this.dockJustChanged = true;

            base.OnDockChanged(e);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void ComputeTabPanelBoundsInternal()
        {
                ComputeTabPanelBounds();
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <remarks>
        /// <para>Called by the <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.Layout"/> method to 
        /// compute the tab panel bounds.</para>
        /// </remarks>
        protected virtual void ComputeTabPanelBounds()
        {
            RectangleF tabPanelBounds = RectangleF.Empty;
            if (!this.IsHandleCreated || this.ClientRectangle == Rectangle.Empty)
            {
                this.SetNeedLayout(true);
                m_internalTabPanelBounds = RectangleF.Empty;
                return;
            }
            // Cleanup cached info.
            //			this.tabPanelRenderer.RefreshLayout();

            // Available width and height are dependant upon current alignment
            SizeF preferredSize = SizeF.Empty;
            switch (this.m_tabPanelData.Alignment)
            {
                case TabAlignment.Bottom:
                case TabAlignment.Top:
                    preferredSize = new SizeF(this.ClientRectangle.Width, this.ClientRectangle.Height);
                    break;
                case TabAlignment.Left:
                case TabAlignment.Right:
                    preferredSize = new SizeF(this.ClientRectangle.Height, this.ClientRectangle.Width);
                    break;
            }

            // Get the preferred size based on current available width and height
            RectangleF CR;
            CR = this.ClientRectangle;
            using (Graphics g = this.CreateGraphics())
            {
                    this.m_tabPanelRenderer.GetPreferredSize(g, ref preferredSize);
            }

            switch (this.m_tabPanelData.Alignment)
            {
                case TabAlignment.Bottom:
                    tabPanelBounds = new RectangleF(new PointF(CR.Left, CR.Bottom - preferredSize.Height),
                        preferredSize);
                    tabPanelBounds.Width = this.ClientRectangle.Width;
                    break;
                case TabAlignment.Top:
                    tabPanelBounds = new RectangleF(new PointF(CR.Left, CR.Top),
                        preferredSize);
                    tabPanelBounds.Width = this.ClientRectangle.Width;
                    break;
                case TabAlignment.Left:
                    tabPanelBounds = new RectangleF(new PointF(CR.Left, CR.Top),
                        new SizeF(preferredSize.Height, preferredSize.Width));
                    tabPanelBounds.Height = this.ClientRectangle.Height;
                    break;
                case TabAlignment.Right:
                    tabPanelBounds = new RectangleF(new PointF(CR.Right - preferredSize.Height, CR.Top),
                        new SizeF(preferredSize.Height, preferredSize.Width));
                    tabPanelBounds.Height = this.ClientRectangle.Height;
                    break;
            }

            tabPanelBounds = AdjustTabPanelBounds(tabPanelBounds);

            if (tabPanelBounds.Height < 0)
            {
                tabPanelBounds.Height = 0;
            }

            if (tabPanelBounds.Width < 0)
            {
                tabPanelBounds.Width = 0;
            }

            if (this.TabPages.Count == 0 && this.ReserveTabSpace)
            {
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        tabPanelBounds.Height = m_reservedSpace;
                        break;

                    case TabAlignment.Right:
                        tabPanelBounds.Width = m_reservedSpace;
                        tabPanelBounds.Offset(-m_reservedSpace, 0);
                        break;

                    case TabAlignment.Left:
                        tabPanelBounds.Width = m_reservedSpace;
                        break;

                    case TabAlignment.Bottom:
                        tabPanelBounds.Height = m_reservedSpace;
                        tabPanelBounds.Offset(0, -m_reservedSpace);
                        break;
                }
            }

            this.SetTabPanelBounds(tabPanelBounds);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool ShouldDrawThemed
        {
            get
            {
                bool bShouldDrawThemed = (XPThemes.IsThemedOS &&
                    XPThemes.IsThemeActive && this.ThemesEnabled);

                return bShouldDrawThemed;
            }
        }
        /// <summary>
        /// Sets the bounds for the tab panel.
        /// </summary>
        /// <param name="tabPanelBounds">The new bounds of the tab panel.</param>
        /// <remarks>
        /// <para>Override this method and provide a new rectangle to set a
        /// custom bounds for the tab panel.</para>
        /// </remarks>
        protected virtual void SetTabPanelBounds(RectangleF tabPanelBounds)
        {
            this.Invalidate(new Region(RectangleF.Union(tabPanelBounds, this.m_tabPanelRenderer.Bounds)), false);
            this.m_tabPanelRenderer.Bounds = tabPanelBounds;
            this.m_internalTabPanelBounds = tabPanelBounds;
        }
        /// <summary>
        /// Returns the current bounds of the tab panel.
        /// </summary>
        /// <returns>A rectangle.</returns>
        protected RectangleF GetTabPanelBounds()
        {
            return this.m_internalTabPanelBounds;
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="tabPanelBounds">The computed tab panel bounds.</param>
        /// <returns>The adjusted tab panel bounds.</returns>
        /// <remarks>
        /// <para>This method is called by <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.ComputeTabPanelBounds"/>
        /// to adjust the computed tab panel bounds for custom needs. 
        /// The base class implementation inserts a scroll button if necessary and also adjusts
        /// the panel bounds to accommodate this scroll button with a call to 
        /// <see cref="Syncfusion.Windows.Forms.Tools.TabControlAdv.AdjustScrollButtonDimensions"/>.</para>
        /// </remarks>
        protected virtual RectangleF AdjustTabPanelBounds(RectangleF tabPanelBounds)
        {
            System.Drawing.SizeF preferredSize = SizeF.Empty;
            Graphics g = this.CreateGraphics();
                this.m_tabPanelRenderer.GetPreferredSize(g, ref preferredSize);
            g.Dispose();

            if (m_tabPrimitivesHost.Visible)
            {
                preferredSize.Width += (m_tabPrimitivesHost.NeedRotate) ?
                    m_tabPrimitivesHost.Size.Height : m_tabPrimitivesHost.Size.Width;
            }

            // If not enough space, init scroll buttons
            if (this.m_tabPanelData.SizeMode != TabSizeMode.ShrinkToFit
                &&
                (
                    ((this.m_tabPanelData.Alignment == TabAlignment.Top || this.m_tabPanelData.Alignment == TabAlignment.Bottom)
                        && tabPanelBounds.Width < preferredSize.Width)
                    ||
                    ((this.m_tabPanelData.Alignment == TabAlignment.Left || this.m_tabPanelData.Alignment == TabAlignment.Right)
                        && tabPanelBounds.Height < preferredSize.Width)
                    )
                )
            {
                if (m_tabPrimitivesHost.Visible)
                {
                    m_tabPrimitivesHost.SetFullModeForAllDropDownPrimitives(true);
                }

                if (m_bShowScroll)
                {
                    if (!m_sbScrollButtons.Visible && !this.Multiline)
                        InitScrollButtons();
                    AdjustScrollButtonDimensions(ref tabPanelBounds, true);
                }
            }
            else
            {
                if (m_tabPrimitivesHost.Visible)
                {
                    m_tabPrimitivesHost.SetFullModeForAllDropDownPrimitives(false);
                }

                AdjustScrollButtonDimensions(ref tabPanelBounds, false);
            }

            // NavigationCtl
            this.AdjustNavigationCtlDimensions(ref tabPanelBounds);

            return tabPanelBounds;
        }

        private const int c_iPrimitivesHostlIndent = 2;

        /// <summary>
        /// Advanced method to adjust the navigation control dimensions.
        /// </summary>
        /// <param name="tabPanelBounds">A <see cref="System.Drawing.RectangleF"/> value specifying the bounds of the tabPanel.</param>
        protected virtual void AdjustNavigationCtlDimensions(ref RectangleF tabPanelBounds)
        {
            if (m_tabPrimitivesHost.Visible)
            {
                bool needRotate = (this.Alignment == TabAlignment.Left
                    || this.Alignment == TabAlignment.Right);

                bool mirrored = m_tabPrimitivesHost.Alignment == TabPrimitiveHostAlignment.Far;
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    mirrored = !mirrored;
                }

                m_tabPrimitivesHost.NeedLayout = true;
                m_tabPrimitivesHost.Layout();
                m_tabPrimitivesHost.RefreshPrimitiveEnabled();

                Size size = m_tabPrimitivesHost.Size;

                // Correct tabPanelBounds.
                if (needRotate)
                {
                    tabPanelBounds.Height -= size.Height + c_iPrimitivesHostlIndent;
                }
                else
                {
                    tabPanelBounds.Width -= size.Width + c_iPrimitivesHostlIndent;
                }

                // Set Location in navigation control.
                if (mirrored)
                {
                    if (needRotate)
                    {
                        m_tabPrimitivesHost.SetLocation(
                            (int)(tabPanelBounds.X + (tabPanelBounds.Width - m_tabPrimitivesHost.Size.Width) / 2)
                            , (int)(tabPanelBounds.Y + c_iPrimitivesHostlIndent));

                        tabPanelBounds.Y += size.Height + c_iPrimitivesHostlIndent;
                    }
                    else
                    {
                        m_tabPrimitivesHost.SetLocation((int)tabPanelBounds.X + c_iPrimitivesHostlIndent,
                            (int)(tabPanelBounds.Y + ((tabPanelBounds.Height - m_tabPrimitivesHost.Size.Height) / 2)));

                        tabPanelBounds.X += size.Width + c_iPrimitivesHostlIndent;
                    }
                }
                else
                {
                    if (needRotate)
                    {
                        m_tabPrimitivesHost.SetLocation(
                            (int)(tabPanelBounds.X + (tabPanelBounds.Width - m_tabPrimitivesHost.Size.Width) / 2)
                            , (int)(tabPanelBounds.Y + tabPanelBounds.Height + c_iPrimitivesHostlIndent));
                    }
                    else
                    {
                        m_tabPrimitivesHost.SetLocation(
                            (int)(tabPanelBounds.X + tabPanelBounds.Width + c_iPrimitivesHostlIndent)
                            , (int)(tabPanelBounds.Y + ((tabPanelBounds.Height - m_tabPrimitivesHost.Size.Height) / 2)));
                    }
                }
            }
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="tabPanelBounds">The tab panel bounds to be adjusted.</param>
        /// <param name="scrollNeeded">True to indicate scroll buttons are needed; false otherwise.</param>
        /// <remarks>
        /// <para>
        /// The base class implementation adjusts the tab panel bounds and positions the scroll buttons
        /// appropriately.
        /// </para>
        /// </remarks>
        protected virtual void AdjustScrollButtonDimensions(ref RectangleF tabPanelBounds, bool scrollNeeded)
        {
            if (m_sbScrollButtons == null)
                return;

            if (scrollNeeded)
            {
                if (this.m_tabPanelData.Alignment == TabAlignment.Top || this.m_tabPanelData.Alignment == TabAlignment.Bottom)
                {
                    m_sbScrollButtons.Size = m_sizeScrollButtonsSize;
                    m_sbScrollButtons.ScrollButtonAppearance = ScrollButtonAppearance.Horizontal;
                    m_sbScrollButtons.IsReverseGradient = (this.m_tabPanelData.Alignment == TabAlignment.Bottom);

                    float upDownTop = tabPanelBounds.Top + tabPanelBounds.Height / 2 - m_sbScrollButtons.Height / 2;

                    if (m_bMirrored)
                    {
                        m_sbScrollButtons.Location = new Point(0, (int)upDownTop);
                        tabPanelBounds.X += (m_sizeScrollButtonsSize.Width + 4);
                    }
                    else
                    {
                        m_sbScrollButtons.Location = new Point((int)tabPanelBounds.Right - m_sizeScrollButtonsSize.Width, (int)upDownTop);//(int)tabPanelBounds.Bottom - 2 - 17);
                    }

                    tabPanelBounds.Width -= (m_sizeScrollButtonsSize.Width + 4);		// 34 for scroll buttons
                }
                else
                {
                    m_sbScrollButtons.Size = new Size(m_sizeScrollButtonsSize.Height, m_sizeScrollButtonsSize.Width);
                    m_sbScrollButtons.ScrollButtonAppearance = ScrollButtonAppearance.Vertical;
                    m_sbScrollButtons.IsReverseGradient = (this.m_tabPanelData.Alignment == TabAlignment.Right);

                    float upDownLeft = tabPanelBounds.Left + tabPanelBounds.Width / 2 - m_sbScrollButtons.Width / 2;

                    if (GetIsMirroredForVerticalAlignment())
                    {
                        m_sbScrollButtons.Location = new Point((int)upDownLeft, 0);
                        tabPanelBounds.Y += (m_sizeScrollButtonsSize.Width + 4);
                    }
                    else
                    {
                        m_sbScrollButtons.Location = new Point((int)upDownLeft/*(int)tabPanelBounds.Right - 17 - 1*/, (int)tabPanelBounds.Bottom - 30 - 2);
                    }

                    tabPanelBounds.Height -= (m_sizeScrollButtonsSize.Width + 4);		// 34 for scroll buttons
                }

                if (m_sbScrollButtons != null && !m_sbScrollButtons.Visible)
                {
                    InitScrollButtons();
                }

                m_sbScrollButtons.MinButtonActive = this.m_tabPanelRenderer.CanScrollLeft;
                m_sbScrollButtons.MaxButtonActive = this.m_tabPanelRenderer.CanScrollRight;

                this.CorrectScrollButtonsLocation();
                this.UpdateScrollButtonState();
            }
            else
            {
                if (m_sbScrollButtons != null && m_sbScrollButtons.Visible)
                {
                    DestroyScrollButtons(false);
                }
            }

            this.UpdateScrollButtonsStyle();
        }

        private void CorrectScrollButtonsLocation()
        {
            if (m_sbScrollButtons != null && m_sbScrollButtons.Visible)
            {
                int offsetX = 0;
                int offsetY = 0;

                if (this.IsWhidbeyStyle && this.Alignment == TabAlignment.Top)
                {
                    offsetY = -DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET;
                }
                else if (this.IsOffice2003Style)
                {

                    switch (this.Alignment)
                    {
                        case TabAlignment.Top:
                            if (this.IsMirrored)
                            {
                                offsetX = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET * 2;
                            }
                            else
                            {
                                offsetX = -DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET * 2;
                            }
                            break;

                        case TabAlignment.Left:
                            if (this.IsMirrored)
                            {
                                offsetX = -DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET;
                                offsetY = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET * 2;
                            }
                            else
                            {
                                offsetX = -DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET * 2;
                            }
                            break;

                        case TabAlignment.Right:
                            offsetY = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET;
                            if (this.IsMirrored)
                            {
                                offsetX = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET * 2;
                            }
                            else
                            {
                                offsetX = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET;
                            }
                            break;

                        case TabAlignment.Bottom:
                            offsetY = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET;
                            if (this.IsMirrored)
                            {
                                offsetX = DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET * 2;
                            }
                            else
                            {
                                offsetX = -DEF_SCROLL_BUTTONS_OFFICE2003_OFFSET;
                            }
                            break;

                    }
                }

                Point location = m_sbScrollButtons.Location;
                location.Offset(offsetX, offsetY);

                m_sbScrollButtons.Location = location;
            }
        }

        private Brush GetScrollButtonsBackGroundBrush()
        {
            Brush brush = null;

            if (this.IsOffice2003Style || this.IsOneNoteStyle || this.IsOneNoteStyleFlatTabs)
            {
                Color backColorTop = this.TabPanelBackColor;
                Color backColorBottom = Color.White;

                Rectangle rect = m_sbScrollButtons.ClientRectangle;

                if (this.IsOneNoteStyle || this.IsOneNoteStyleFlatTabs)
                {
                    m_sbScrollButtons.GradientInflateOffset = DEF_SCROLL_BUTTONS_GRADIENT_OFFSET;
                }

                rect.Inflate(m_sbScrollButtons.GradientInflateOffset, m_sbScrollButtons.GradientInflateOffset);

                if (rect.Width > 0 && rect.Height > 0)
                {
                    if (m_sbScrollButtons.ScrollButtonAppearance == ScrollButtonAppearance.Horizontal)
                    {
                        if (m_sbScrollButtons.IsReverseGradient)
                        {
                            brush = new LinearGradientBrush(rect, backColorBottom,
                                backColorTop, LinearGradientMode.Vertical);
                        }
                        else
                        {
                            brush = new LinearGradientBrush(rect, backColorTop,
                                backColorBottom, LinearGradientMode.Vertical);
                        }
                    }
                    else
                    {
                        if (m_sbScrollButtons.IsReverseGradient)
                        {
                            brush = new LinearGradientBrush(rect, backColorBottom,
                                backColorTop, LinearGradientMode.Horizontal);
                        }
                        else
                        {
                            brush = new LinearGradientBrush(rect, backColorTop,
                                backColorBottom, LinearGradientMode.Horizontal);
                        }
                    }
                }

                if (this.IsOffice2003Style && (this.BackColor == this.TabPanelBackColor))
                {
                    brush = new SolidBrush(Office2003Colors.MenuItemHotColorDark);
                }
            }
            else if (this.IsIE7Style || this.IsDockingWhidbeyStyleBeta || this.IsDockingWhidbeyStyle || this.IsVS2008)
            {
                brush = new SolidBrush(TabPanelBackColor);
            }

            return brush;
        }

        protected void UpdateScrollButtonsStyle()
        {
            if (m_sbScrollButtons != null)
            {
                m_sbScrollButtons.GradientInflateOffset = 0;

                if (this.Alignment == TabAlignment.Top || this.Alignment == TabAlignment.Bottom)
                {
                    m_sbScrollButtons.ScrollButtonAppearance = ScrollButtonAppearance.Horizontal;
                }
                else
                {
                    m_sbScrollButtons.ScrollButtonAppearance = ScrollButtonAppearance.Vertical;
                }

                m_sbScrollButtons.IsReverseGradient = this.Alignment == TabAlignment.Bottom || this.Alignment == TabAlignment.Right;

                if (this.IsOffice2007Style)
                {
                    m_sbScrollButtons.BackGroundBrush = new SolidBrush(TabPanelBackColor); 
                    m_sbScrollButtons.Style = VisualStyle.Office2007;
                    m_sbScrollButtons.ThemesEnabled = false;
                }
                else if (this.IsOffice2010Style)
                {
                    m_sbScrollButtons.BackGroundBrush = new SolidBrush(TabPanelBackColor);
                    m_sbScrollButtons.Style = VisualStyle.Office2010;
                    m_sbScrollButtons.ThemesEnabled = false;
                }
                else if (this.IsOffice2003Style)
                {
                    m_sbScrollButtons.GradientInflateOffset = DEF_SCROLL_BUTTONS_GRADIENT_OFFSET;
                    m_sbScrollButtons.ThemesEnabled = false;
                    m_sbScrollButtons.Style = VisualStyle.Office2003;
                    m_sbScrollButtons.BackGroundBrush = GetScrollButtonsBackGroundBrush();                    
                }
                else if (this.IsWhidbeyStyle || IsDockingWhidbeyStyleBeta)
                {
                    m_sbScrollButtons.ThemesEnabled = false;
                    m_sbScrollButtons.Style = VisualStyle.VS2005;
                    //This should have been fixed for memory leak. But this is causing an issue with the change in TabStyle.
                    //using (SolidBrush brush = new SolidBrush(TabPanelBackColor))
                    //{
                    //    m_sbScrollButtons.BackGroundBrush = brush;
                    //}
                    m_sbScrollButtons.BackGroundBrush = new SolidBrush(TabPanelBackColor);
                }
                else if (this.ThemesEnabled)
                {
                    m_sbScrollButtons.ThemesEnabled = true;
                    m_sbScrollButtons.Style = VisualStyle.OfficeXP;
                    m_sbScrollButtons.BackGroundBrush = GetScrollButtonsBackGroundBrush();
                }
                else
                {
                    m_sbScrollButtons.ThemesEnabled = true;
                    m_sbScrollButtons.Style = VisualStyle.Default;
                    m_sbScrollButtons.BackGroundBrush = GetScrollButtonsBackGroundBrush();
                }

                if (this.VSLikeScrollButton ||
                    this.IsOffice2003Style || this.IsWhidbeyStyle ||
                    this.IsOffice2007Style || this.IsOneNoteStyle || this.IsOffice2010Style ||
                    this.IsDockingWhidbeyStyleBeta || this.IsIE7Style ||
                    this.IsDockingWhidbeyStyle || this.IsVS2008 || this.IsMetroStyle)
                {
                    m_sbScrollButtons.VSLikeButton = true;
                }
                else
                {
                    m_sbScrollButtons.VSLikeButton = false;
                }

                if (this.IsVS2010)
                {
                    m_sbScrollButtons.BackGroundBrush = new SolidBrush(Color.FromArgb(41, 57, 85));
                    m_sbScrollButtons.Style = VisualStyle.VS2010;
                    m_sbScrollButtons.VSLikeButton = true;
                }
                if (this.IsMetroStyle)
                {
                    m_sbScrollButtons.Style = VisualStyle.Metro;
                }

                m_sbScrollButtons.Invalidate();
            }
        }
        /// <summary>
        /// Initializes the scroll buttons used to let user scroll the tabs.
        /// </summary>
        protected virtual void InitScrollButtons()
        {
            m_sbScrollButtons.TabStop = false;
            this.UpdateScrollButtonState();

            m_sbScrollButtons.Visible = true;
            m_sbScrollButtons.BringToFront();
            m_sbScrollButtons.UpDown += new UpDownEventHandler(this.UpDownEventHandler);
            m_sbScrollButtons.VSLikeButton = (this.VSLikeScrollButton || this.IsOffice2007Style ||
                this.IsOffice2003Style || this.IsWhidbeyStyle || this.IsVS2008 || this.IsVS2010 || this.IsOffice2010Style);
            m_sbScrollButtons.ThemesEnabled = this.ThemesEnabled;

            UpdateScrollButtonsStyle();
        }
        protected void UpdateScrollButtonState()
        {
            if (m_sbScrollButtons == null)
                return;

            if (this.m_tabPanelData.TabStyle == TabRenderer2D.TabStyleName
                || (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled && this.ThemesEnabled))
                m_sbScrollButtons.ButtonState = ButtonState.Flat;
            else
                m_sbScrollButtons.ButtonState = ButtonState.Normal;
        }

        /// <summary>
        /// Destroys the scroll buttons.
        /// </summary>
        /// <param name="multilineChanged">True if this is called because the multiline property changed; false if called from Dispose.</param>
        protected virtual void DestroyScrollButtons(bool multilineChanged)
        {
            if (m_sbScrollButtons != null)
            {
                m_sbScrollButtons.UpDown -= new UpDownEventHandler(this.UpDownEventHandler);
                m_sbScrollButtons.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Color GetRendererBorderColor()
        {
            Color borderColor = Color.Empty;

            if (this.Renderer != null && this.Renderer.Renderers != null
                && this.SelectedIndex >= 0 && this.SelectedIndex < this.Renderer.Renderers.Count)
            {
                TabRendererBase tabPageRenderer = this.Renderer.Renderers[this.SelectedIndex] as TabRendererBase;
                borderColor = tabPageRenderer.TabBorderColor;
            }

            return borderColor;
        }
        #endregion PAINTING

        #region CONTROL_OVERRIDES
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnHandleCreated"/>.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            this.UpdateSelectedTabPage(false);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.ProcessKeyPreview"/>.
        /// </summary>
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (this.ProcessKeyEventArgs(ref m))
                return true;
            else
                return base.ProcessKeyPreview(ref m);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
                return false;

            if (keyData == Keys.Escape && this.m_tabPanelRenderer.IsMovingTab())
                return true;

            if (this.SwitchPagesForDialogKeys)
            {
                Keys key = keyData & Keys.KeyCode;

                switch (key)
                {
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                }

                if (this.Focused)
                {
                    // Move left to right or right to left; do not roll over.
                    if (this.TabCount > 0
                        &&
                        (
                        ((keyData == Keys.Left || keyData == Keys.Right) && (this.Alignment == TabAlignment.Bottom || this.Alignment == TabAlignment.Top))
                        ||
                        ((keyData == Keys.Up || keyData == Keys.Down) && (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right))
                        )
                        )
                    {
                        return true;
                    }
                    // Support moving tabs across rows using Up/Down or Left/Right in Multiline mode.
                    else if (this.TabCount > 0 && this.Multiline
                        && (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right
                        || keyData == Keys.Left))
                    {
                        return true;
                    }
                }
            }

            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnKeyDown"/>.
        /// </summary>
        /// <param name="ke">A KeyEventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs ke)
        {
            if (this.m_tabPanelRenderer != null && this.m_tabPanelRenderer.IsMovingTab() && ke.KeyData == Keys.Escape)
            {
                this.m_tabPanelRenderer.CancelTabDrag();
                ke.Handled = true;
            }
            else
            {
                // This property is obsolete ever since this logic was moved from ProcessCmdKey to OnKeyDown
                // but will be useful if users want to override Ctrl+Tab switching functionality in a mdi scenario.
                if (this.SwitchPagesForDialogKeys)
                {
                    Keys keyData = ke.KeyData;
                    // If Ctrl+Tab or Ctrl+Tab+Shift or Ctrl+PageDown or Ctrl+PageUp
                    // then change selected tab appropriately roll over.
                    if (keyData == (Keys.Tab | Keys.Control)
                        || keyData == (Keys.PageDown | Keys.Control)
                        || keyData == (Keys.Tab | Keys.Control | Keys.Shift)
                        || keyData == (Keys.PageUp | Keys.Control))
                    {
                        bool shiftkey = (keyData == (Keys.Tab | Keys.Control | Keys.Shift))
                            || (keyData == (Keys.PageDown | Keys.Control));

                        int selectedIndex = this.SelectedIndex;
                        if (selectedIndex != -1)
                        {
                            int tabCount = this.m_tabPanelData.TabsData.Count;

                            bool selectableTabFound = false;
                            while (!selectableTabFound)
                            {
                                if (!shiftkey)
                                    selectedIndex = ((selectedIndex + 1) % tabCount);
                                else
                                    selectedIndex = (((selectedIndex + tabCount) - 1) % tabCount);

                                if (this.TabPanelData.IsTabSelectable(selectedIndex, true))
                                    selectableTabFound = true;

                                // We have come a full circle, so break;
                                if (selectedIndex == this.SelectedIndex)
                                    break;
                            }
                            if (this.ValidateFocusedTab())
                            {
                                // Change tab only if validation was successful in the current tab.
                                this.SelectedIndex = selectedIndex;
                            }

                            // Return true even if validation failed above.
                            ke.Handled = true;
                        }
                    }

                    if (!ke.Handled && this.Focused)
                    {
                        // Move left to right or right to left; do not roll over.
                        if (this.TabCount > 0
                            &&
                            (
                            ((keyData == Keys.Left || keyData == Keys.Right) && (this.Alignment == TabAlignment.Bottom || this.Alignment == TabAlignment.Top))
                            ||
                            ((keyData == Keys.Up || keyData == Keys.Down) && (this.Alignment == TabAlignment.Left || this.Alignment == TabAlignment.Right))
                            )
                            )
                        {


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                            m_previousTabPage = m_currentTabPage;
#endif

                            int selectedIndex = this.SelectedIndex;

                            bool bIsMirrored = GetIsMirroredForVerticalAlignment();
                            bool moveUp =
                                keyData == (bIsMirrored ? Keys.Left : Keys.Right) ||
                                keyData == (bIsMirrored ? Keys.Up : Keys.Down);

                            bool selectableTabFound = false;

                            while (!selectableTabFound)
                            {
                                int oldIndex = selectedIndex;

                                if (!moveUp && selectedIndex > 0)
                                    selectedIndex--;
                                else if (moveUp && selectedIndex < this.m_tabPanelData.TabsData.Count - 1)
                                    selectedIndex++;

                                if (this.TabPanelData.IsTabSelectable(selectedIndex, true))
                                    selectableTabFound = true;

                                // We have come a full circle, so break;
                                if (selectedIndex == this.SelectedIndex
                                    || oldIndex == selectedIndex)
                                    break;
                            }
                            if (selectableTabFound)
                                this.SelectedIndex = selectedIndex;

                            ke.Handled = true;


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                            FireTabPageLeaveEnter();
#endif
                        }
                        // Support moving tabs across rows using Up/Down or Left/Right in Multiline mode.
                        else if (this.TabCount > 0 && this.Multiline
                            && (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right
                            || keyData == Keys.Left))
                        {

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                            m_previousTabPage = m_currentTabPage;
#endif

                            int newIndex = this.SelectedIndex;
                            bool selectableTabFound = false;

                            while (!selectableTabFound)
                            {
                                Rectangle tabRect = this.GetTabRect(newIndex);

                                if (this.Alignment == TabAlignment.Top && keyData == Keys.Up)
                                {
                                    newIndex = this.HitTestTabs(new Point(tabRect.X + tabRect.Width / 2, tabRect.Y - 2));
                                }
                                else if (this.Alignment == TabAlignment.Bottom && keyData == Keys.Down)
                                {
                                    newIndex = this.HitTestTabs(new Point(tabRect.X + tabRect.Width / 2, tabRect.Bottom + 1));
                                }
                                else if (this.Alignment == TabAlignment.Left && keyData == Keys.Left)
                                {
                                    newIndex = this.HitTestTabs(new Point(tabRect.X - 2, tabRect.Y + tabRect.Height / 2));
                                }
                                else if (this.Alignment == TabAlignment.Right && keyData == Keys.Right)
                                {
                                    newIndex = this.HitTestTabs(new Point(tabRect.Right + 1, tabRect.Y + tabRect.Height / 2));
                                }

                                if (newIndex == -1 || this.TabPanelData.IsTabSelectable(newIndex, true))
                                    selectableTabFound = true;

                                // We have come a full circle, so break;
                                if (newIndex == this.SelectedIndex)
                                    break;
                            }
                            if (newIndex != -1)
                                this.SelectedIndex = newIndex;

                            ke.Handled = true;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                            FireTabPageLeaveEnter();
#endif
                        }
                    }
                }
            }
            base.OnKeyDown(ke);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnFontChanged"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            SetNeedLayout(true);
        }
        /// <summary>
        /// Overridden. See <see cref="Object.ToString"/>.
        /// </summary>
        /// <returns>The string representation of the Control.</returns>
        public override string ToString()
        {
            string textRep = base.ToString();
            if (this.TabPages.Count > 0)
            {
                int n1 = this.TabPages.Count;
                textRep = String.Concat(textRep, (string)@", TabPages.Count: ", n1.ToString());
                if (this.TabPages.Count > 0)
                    textRep = String.Concat(textRep, (string)@", TabPages[0]: ", this.TabPages[0].ToString());
            }
            return textRep;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
            this.SetRegion();
        }
        #endregion CONTROL_OVERRIDES

        #region For Touch

        bool isScaling = false;

        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true), DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
        ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            this.ItemSize = new Size((int)(ITMSIZE.Width * scaleFactor), (int)(ITMSIZE.Height * scaleFactor));
            this.ScrollButtons.Size = new Size((int)(SCROLLSIZE.Width * scaleFactor), (int)(SCROLLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        #endregion

        protected void UpDownEventHandler(object source, UpDownEventArgs e)
        {
            ScrollDirection dir = ScrollDirection.Left;
            if (e.ButtonID == 1)
                dir = ScrollDirection.Left;
            else if (e.ButtonID == 2)
                dir = ScrollDirection.Right;

            this.m_tabPanelRenderer.Scroll(this.ScrollIncrement, dir);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.CreateControlsInstance"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new TabControlAdv.ControlCollection(this);
        }

        #region Utilities
        private static RightToLeft GetActualRightToLeftValue(Control ctrl)
        {
            Debug.Assert(ctrl != null, "GetActualRightToLeftValue(): Control is null");
            while (null != (ctrl = ctrl.Parent))
            {
                RightToLeft rtlVal = ctrl.RightToLeft;
                if (RightToLeft.Inherit != rtlVal)
                {
                    return rtlVal;
                }
            }

            Debug.Assert(false, "GetActualRightToLeftValue(): Failed to determine actual value during parent treewalking");

            return RightToLeft.No;
        }
        #endregion

        /// <summary>
        /// Occurs when a control is added to the tabControlAdv control collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.ControlEventArgs"/> instance containing the event data.</param>
        private void TabControlAdv_ControlAdded(object sender, ControlEventArgs e)
        {
            TabPageAdv tabPage = e.Control as TabPageAdv;
            if (tabPage == null) return;

            tabPage.MouseDown += new MouseEventHandler(tabPage_MouseDown);
        }

        /// <summary>
        /// Handles the MouseDown event of the tabPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void tabPage_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.IsEditing)
            {
                //	m_prevSelectedIndex = int.MinValue;
                m_tabPageText = this.SelectedTab.Text;

                if (m_tabPageText != m_txtLabel.Text)
                    EndLabelEdit(true);
                else
                    EndLabelEdit(false);
            }
        }

        /// <summary>
        /// Occurs when a control is removed from the TabControlAdv control collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.ControlEventArgs"/> instance containing the event data.</param>
        private void TabControlAdv_ControlRemoved(object sender, ControlEventArgs e)
        {
            TabPageAdv tabPage = e.Control as TabPageAdv;

            if (tabPage != null)
            {
                tabPage.IsTransparent = false;
                tabPage.MouseDown -= new MouseEventHandler(tabPage_MouseDown);

                if (m_previousTabPage == tabPage)
                {
                    m_previousTabPage = null;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool handled = false;

            if (this.IsEditing)
            {
                if (keyData == Keys.Return)
                {
                    this.EndLabelEdit(true);
                    handled = true;
                }
                else if (keyData == Keys.Escape)
                {
                    this.EndLabelEdit(false);
                    handled = true;
                }
            }

            if (!handled)
            {
                handled = base.ProcessCmdKey(ref msg, keyData);
            }

            return handled;
        }

        /// <summary>
        /// If needed raises enter event on current tab page.
        /// </summary>
        private void FireTabPageLeaveEnter()
        {
            if (m_previousTabPage != m_currentTabPage)
            {
                if (m_previousTabPage != null)
                {
                    m_previousTabPage.FireLeave(EventArgs.Empty);
                }

                m_currentTabPage.FireEnter(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Invisible tabPage cannot be shown in runtime.
        /// </summary>
        private void ValidateTabPagesVisibility()
        {
            int nSelTab = this.SelectedIndex;
            TabPageAdvCollection tabPages = this.TabPages;
            int nTabCount = tabPages.Count;

            if (!this.DesignMode && nSelTab >= 0 && nSelTab < nTabCount && !tabPages[nSelTab].TabVisible)
            {
                for (int i = 0; i < nTabCount; i++)
                {
                    if (tabPages[i].TabVisible)
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        #region ISupportOffice2007Theme implementation

        Office2007Theme ISupportOffice2007Theme.Office2007ColorTheme
        {
            get
            {
                return this.Office2007ColorScheme;
            }
            set
            {
                this.Office2007ColorScheme = value;
            }
        }

        void ISupportOffice2007Theme.EnableOffice2007Style()
        {
            this.TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererOffice2007);
        }

        #endregion
    }

    public delegate void EditEventHandler(object sender, EditEventArgs e);

    public delegate void TabMovingEventHandler(object sender, TabMovingEventArgs e);

    /// <summary>
    /// Represents a single tab page in a <see cref="TabControlAdv"/>.
    /// </summary>
    /// <example>
    ///  The following example creates a <see cref="TabControlAdv"/> with one TabPageAdv object.
    ///  <para>Use the Syncfusion.Windows.Forms.Tools namespace for this example.</para>
    ///  <code lang="C#">
    ///  public Form1()
    ///  {
    ///		this.tabControl1 = new TabControlAdv();
    ///		
    ///		// Invokes the TabPage() constructor to create the tabPage1 object.
    ///		this.tabPage1 = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
    ///		this.tabControl1.Controls.AddRange(new Control[] {
    ///		         this.tabPage1});
    ///		this.tabControl1.Location = new Point(25, 25);
    ///		this.tabControl1.Size = new Size(250, 250);
    ///		this.ClientSize = new Size(300, 300);
    ///		this.Controls.AddRange(new Control[] {
    ///		        this.tabControl1});
    ///	}
    /// </code>
    /// <code lang="VB">
    /// Public Sub New()
    ///		Me.tabControl1 = New TabControlAdv()
    ///		' Invokes the TabPage() constructor to create the tabPage1 object.
    ///		Me.tabPage1 = New Syncfusion.Windows.Forms.Tools.TabPageAdv()
    ///		Me.tabControl1.Controls.AddRange(New Control() {Me.tabPage1})
    ///		Me.tabControl1.Location = New Point(25, 25)
    ///		Me.tabControl1.Size = New Size(250, 250)
    ///		Me.ClientSize = New Size(300, 300)
    ///		Me.Controls.AddRange(New Control() {Me.tabControl1})
    ///	End Sub 'New
    ///	</code>
    /// </example>
    [
    DefaultEvent(@"Click"),
    Designer(
        typeof(Syncfusion.Windows.Forms.Tools.Design.TabPageAdvDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    DesignTimeVisible(false),
    DefaultProperty(@"Text"),
    ToolboxItem(false),
    ]
    public class TabPageAdv : Panel
    {
        /// <summary>
        /// Contains the collection of controls that the TabPage uses.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public new class ControlCollection :
            Control.ControlCollection
        {
            public ControlCollection(TabPageAdv owner)
                : base((Control)owner)
            {
            }
            public override void Add(Control value)
            {
                if (value is TabPageAdv)
                    throw new ArgumentException("Cannot add TabPageAdv to TabPageAdv as child.");
                else
                    base.Add(value);
            }

            /// <summary>
            /// Overridden. See <see cref="System.Windows.Forms.Control.ControlCollection.AddRange"/>.
            /// </summary>
            /// <param name="controls">The array of controls.</param>
            public override void AddRange(Control[] controls)
            {
                base.AddRange(controls);
            }

        }

        #region Class members

        private ITabData tabData;
        private ITabPanelDefaultProperties tabPanelDefaultProperties;
        private bool customThemesEnabledFlag = true;
        private ISelectionService m_selectionService = null;
        private bool m_bIsTransparent = false;
        private bool m_enterFired = false;
        private bool m_leaveFired = false;

        #endregion

        /// <summary>
        /// Occurs when the tab page is closing.
        /// </summary>
        /// <remarks>This event is not fired when tab page is disposed or being disposed.</remarks>
        [Description("Occurs when the tab page is closing.")]
        public event TabPageAdvClosingEventHandler Closing;

        /// <summary>
        /// Occurs when the tab page is closed.
        /// </summary>
        /// <remarks>This event is not fired when tab page is disposed or being disposed.</remarks>
        [Description("Occurs when the tab page is closed.")]
        public event EventHandler Closed;

        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                if (m_selectionService != null)
                {
                    m_selectionService.SelectionChanging -= new EventHandler(selectionService_SelectionChanging);
                }

                base.Site = value;

                if (base.Site != null)
                {
                    m_selectionService = (ISelectionService)this.Site.GetService(typeof(ISelectionService));

                    if (m_selectionService != null)
                    {
                        m_selectionService.SelectionChanging += new EventHandler(selectionService_SelectionChanging);
                    }
                }
            }
        }

        private void selectionService_SelectionChanging(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                TabControlAdv tabControl = this.Parent as TabControlAdv;

                if (tabControl != null && tabControl.DisplayRectangle != Rectangle.Empty)
                {
                    this.Bounds = tabControl.DisplayRectangle;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the TabPageAdv class with its tab data and default properties.
        /// </summary>
        /// <param name="tabData">The data for this tab.</param>
        /// <param name="tabPanelDefaultProperties">The default properties for this tab.</param>
        public TabPageAdv(ITabData tabData, ITabPanelDefaultProperties tabPanelDefaultProperties)
            : this()
        {
            this.tabData = tabData;
            this.tabPanelDefaultProperties = tabPanelDefaultProperties;
        }
        /// <summary>
        /// Creates a new instance of the TabPageAdv class.
        /// </summary>
        /// <example>
        ///  The following example creates a TabControlAdv with one TabPageAdv object. 
        ///  The constructor instantiates tabPage1.
        ///  <para>Use the Syncfusion.Windows.Forms.Tools namespaces for this example.</para>
        ///  <code>
        ///  public void MyTabs()
        ///  {
        ///  	this.tabControl1 = new TabControlAdv();
        ///  	// Invokes the TabPageAdv() constructor to create the tabPage1 object.
        ///  	this.tabPage1 = new Syncfusion.Windows.Forms.ToolsTabPageAdv();
        ///  	this.tabControl1.Controls.Add(tabPage1);
        ///  	this.Controls.Add(tabControl1);
        ///  }
        ///  
        ///  public Form1()
        ///  {
        ///  	MyTabs();
        ///  }
        ///  </code>
        /// </example>
        public TabPageAdv()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            Init();
        }
        /// <summary>
        /// Creates a new instance of the TabPageAdv class qith the specified text for the tab.
        /// </summary>
        /// <param name="label">The text for the tab.</param>
        /// <example>
        /// This example creates a TabControlAdv with a TabPageAdv object. 
        /// The constructor accepts the myTabPage string as Text for tabPage1.
        ///  <para>Use the Syncfusion.Windows.Forms.Tools namespaces for this example.</para>
        ///  <code>
        ///  public void MyTabs()
        ///  {
        ///  	this.tabControl1 = new TabControlAdv();
        ///  	string tabPageName = "myTabPage";
        ///  	
        ///  	// Invokes the TabPageAdv() constructor to create the tabPage1 object.
        ///  	this.tabPage1 = new Syncfusion.Windows.Forms.Tools.TabPageAdv(tabPageName);
        ///  	
        ///  	this.tabControl1.Controls.Add(tabPage1);
        ///  	this.Controls.Add(tabControl1);
        ///  }
        ///  
        ///  public Form1()
        ///  {
        ///  	MyTabs();
        ///  }
        ///  </code>
        /// </example>
        public TabPageAdv(string label)
            : this()
        {
            this.Text = label;
            //			this.tempPropHash[text] = label;
        }
        /// <summary>
        /// Called to create the default <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/>
        /// for this TabPageAdv.
        /// </summary>
        /// <returns>An <see cref="Syncfusion.Windows.Forms.Tools.ITabData"/> instance.</returns>
        protected virtual ITabData CreateDefaultTabData()
        {
            return new TabData();
        }

        /// <summary>
        /// Indicates whether this control is transparent.
        /// </summary>

        [
        DefaultValue(false),
        Description("Indicates whether this control is transparent")
        ]
        public bool IsTransparent
        {
            get
            {
                return m_bIsTransparent;
            }
            set
            {
                if (value != m_bIsTransparent)
                {
                    m_bIsTransparent = value;

                    if (m_bIsTransparent)
                    {
                        SetTransparentBackground();
                    }
                    else
                    {
                        RestoreBackground();
                    }
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void SetSelectedAtDesignTime()
        {
            if (this.DesignMode)
            {
                ISelectionService selectionService =
                    base.GetService(typeof(ISelectionService)) as ISelectionService;

                if (selectionService != null)
                {
                    ArrayList arrControls = new ArrayList();
                    arrControls.Add(this);
                    selectionService.SetSelectedComponents(arrControls, SelectionTypes.Replace);
                }
            }
        }

        /// <summary>
        /// Adds transparent style to control styles.
        /// </summary>
        private void SetTransparentBackground()
        {
            int stylesEx = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
            stylesEx = stylesEx | NativeMethods.WS_EX_TRANSPARENT;
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, new IntPtr(stylesEx));
        }

        /// <summary>
        /// Removes Transparent style from control styles.
        /// </summary>
        private void RestoreBackground()
        {
            int stylesEx = NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_EXSTYLE);
            stylesEx = stylesEx & ~NativeMethods.WS_EX_TRANSPARENT;
            NativeMethods.SetWindowLong(Handle, NativeMethods.GWL_EXSTYLE, new IntPtr(stylesEx));
        }

        private void Init()
        {
            this.tabData = this.CreateDefaultTabData();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                if (this.tabData != null)
                {
                    this.tabData.Dispose();
                    this.tabData = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the Enter event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that has some information regarding this event.</param>
        internal void FireEnter(EventArgs e)
        {
            m_enterFired = true;
            OnEnter(e);
        }

        /// <summary>
        /// Raises the Leave event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that has some information regarding this event.</param>
        internal void FireLeave(EventArgs e)
        {
            m_leaveFired = true;
            OnLeave(e);
        }

        /// <summary>
        /// Raises the Enter event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that has some information regarding this event.</param>
        protected override void OnEnter(EventArgs e)
        {
            if (m_enterFired)
            {
                base.OnEnter(e);
            }
            m_enterFired = false;

            if (this.Controls.Count > 0) //If tab page has another tab control as a single child
            {
                if (this.Controls.Count == 1)
                {
                    //this.Controls[0].Focus();
                }
                else //else if tab page has another tab control as child with tab index as 0
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        if (ctrl is TabControlAdv && ctrl.TabIndex == 0)
                            (ctrl as TabControlAdv).Focus();
                        else if (this.Controls[0] is TabControl && ctrl.TabIndex == 0)
                            (ctrl as TabControl).Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the Leave event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that has some information regarding this event.</param>
        protected override void OnLeave(EventArgs e)
        {
            if (m_leaveFired)
            {
                base.OnLeave(e);
            }
            m_leaveFired = false;
        }

        /// <summary>
        /// Conceals the tab page from the user.
        /// </summary>
        public new void Hide()
        {
            TabVisible = false;
        }

        /// <summary>
        /// Displays the tab page to the user.
        /// </summary>
        public new void Show()
        {
            TabVisible = true;
        }

        /// <summary>
        /// Closes <see cref="TabPage"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Removes page from <see cref="TabControlAdv.TabPages"/> collection.
        /// That also removes page from parent <see cref="TabControlAdv"/> <see cref="Control.Controls"/> collection.
        /// </para>
        /// <para>
        /// This method is called by <see cref="TabControlAdv"/> only when user clicks tab page's or tab control's close button.
        /// </para>
        /// </remarks>
        /// <returns>
        /// False if page can't be closed.
        /// This happens if closing is canceled in <see cref="TabPageAdv.Closing"/> event handler,
        /// page is disposed/being disposed, page is detached from <see cref="TabControlAdv"/>,
        /// or hosted in any other (non-<see cref="TabControlAdv"/>) control.
        /// In last case, page is just removed from parent's <see cref="Control.Controls"/> collection.
        /// </returns>
        public bool Close()
        {
            bool bClosed = false;

            if (this.Parent != null)
            {
                IList tabPages = null;
                TabControlAdv tabControl = this.Parent as TabControlAdv;

                if (tabControl != null)
                {
                    tabPages = tabControl.TabPages;
                }
                else
                {
                    tabPages = this.Parent.Controls;
                }

                if (tabPages.Contains(this))
                {
                    if (!this.Disposing && !this.IsDisposed)
                    {
                        TabPageAdvClosingEventArgs tabPageAdvClosingEventArgs = new TabPageAdvClosingEventArgs(this, false);

                        OnClosing(tabPageAdvClosingEventArgs);

                        if (!tabPageAdvClosingEventArgs.Cancel)
                        {
                            tabPages.Remove(this);

                            OnClosed(EventArgs.Empty);

                            bClosed = true;
                        }
                    }
                    else
                    {
                        tabPages.Remove(this);
                    }
                }
            }

            return bClosed;
        }

        private void OnClosed(EventArgs eventArgs)
        {
            if (this.Closed != null)
            {
                this.Closed(this, eventArgs);
            }
        }

        private void OnClosing(TabPageAdvClosingEventArgs tabPageAdvClosingEventArgs)
        {
            if (this.Closing != null)
            {
                this.Closing(this, tabPageAdvClosingEventArgs);
            }
        }

        #region HIDE_IN_DESIGNER
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Anchor"/>.
        /// </summary>
        /// <remarks>Anchoring TabPageAdv instance is disabled.</remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public override AnchorStyles Anchor
        {
            get { return base.Anchor; }
            set { base.Anchor = value; }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Dock"/>.
        /// </summary>
        /// <remarks>Docking TabPageAdv instance is disabled.</remarks>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public override /*Control*/ DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Enabled"/>.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public new bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        /// <summary>
        /// Indicates whether to enable the tab.
        /// </summary>
        /// <value>True to enable; false otherwise.</value>
        /// <remarks>
        /// If disabled, the tab will be drawn disabled and the user will not be able to select
        /// the tab page through the mouse or keyboard. You can however select a tab programmatically
        /// using the <see cref="TabControlAdv.SelectedIndex"/> or <see cref="TabControlAdv.SelectedTab"/> property.
        /// </remarks>
        [
        DefaultValue(true),
        Category("Appearance"),
        Description("Specifies whether or not to enable the tab.")
        ]
        public virtual bool TabEnabled
        {
            get
            {
                if (tabData != null)
                    return this.tabData.Enabled;
                else return true;
            }
            set
            {
                if (tabData != null)
                    this.tabData.Enabled = value;

                if (!this.DesignMode)
                    this.Enabled = value;
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.TabIndex"/>.
        /// </summary>
        /// <remarks>TabIndex property for the TabPageAdv instance is disabled.</remarks>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public new int TabIndex
        {
            get { return base.TabIndex; }
            set
            {
                if (value != base.TabIndex)
                {
                    TabControlAdv tabControl = this.Parent as TabControlAdv;

                    base.TabIndex = value;

                    if (null != tabControl)
                    {
                        tabControl.OnTabsOrderChanged();
                    }
                }
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.TabStop"/>.
        /// </summary>
        /// <remarks>TabStop property for the TabPageAdv instance is disabled.</remarks>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Visible"/>.
        /// </summary>
        /// <remarks>Visible property for the TabPageAdv instance is disabled.</remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.SetBoundsCore"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            TabControlAdv tabControl;
            System.Drawing.Rectangle rectangle1;
            tabControl = this.Parent as TabControlAdv;
            if (tabControl != null && tabControl.IsHandleCreated)
            {
                bool shouldRecalculateBounds = tabControl.IsInitializing;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                shouldRecalculateBounds = (shouldRecalculateBounds || tabControl.IsDesignerLoading);

#endif
                if (shouldRecalculateBounds)
                {
                    tabControl.ComputeTabPanelBoundsInternal();
                }

                rectangle1 = tabControl.DisplayRectangle;
                base.SetBoundsCore(rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height, BoundsSpecified.All);
            }
            else
                base.SetBoundsCore(x, y, width, height, specified);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.CreateControlsInstance"/>.
        /// </summary>
        /// <returns></returns>
        protected override Control.ControlCollection CreateControlsInstance()
        {
            return (Control.ControlCollection)new TabPageAdv.ControlCollection(this);
        }
        /// <summary>
        /// Overridden. See <see cref="Object.ToString"/>
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return String.Concat((string)@"TabPageAdv: {", this.Text, (string)@"}");
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static TabPageAdv GetTabPageAdvOfComponent(object comp)
        {
            System.Windows.Forms.Control control;
            if (!(comp is System.Windows.Forms.Control))
                return null;
            else
                control = (System.Windows.Forms.Control)comp;

            while (control != null)
            {
                if (control.Parent is TabPageAdv)
                    return (TabPageAdv)control.Parent;
                else
                    control = control.Parent;
            }
            return null;
        }
        #endregion HIDE_IN_DESIGNER
        internal ITabData TabData
        {
            get { return this.tabData; }
        }
        internal ITabPanelDefaultProperties TabPanelDefaultProperties
        {
            get { return this.tabPanelDefaultProperties; }
            set
            {
                this.tabPanelDefaultProperties = value;
            }
        }
        /// <summary>
        /// Gets / sets the ToolTip text for this tab.
        /// </summary>
        /// <value>The ToolTip text for this tab.</value>
        /// <remarks>This tab page belongs to a TabControlAdv instance. 
        /// The ToolTip text appears when the user moves the mouse 
        /// over the tab - if the ShowToolTips property of the 
        /// TabControlAdv is true. For more information on ToolTips, 
        /// see the <see cref="System.Windows.Forms.ToolTip"/> class.</remarks>
        [
            DefaultValue(@""),
            Localizable(true),
            Description("Gets or sets the ToolTip text for this tab."),
            Category("Appearance")
        ]
        public string ToolTipText
        {
            get
            {
                if (tabData != null)
                {
                    return this.tabData.ToolTip;
                }
                else return "";
            }
            set
            {
                if (tabData != null)
                {
                    this.tabData.ToolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the SuperToolTip information for this tab.
        /// </summary>
        [DefaultValue(null)]
        [Localizable(true)]
        [Description("SuperToolTip information for this tab.")]
        [Category("Appearance")]
        public ToolTipInfo SuperTooltip
        {
            get
            {
                ITabData2 tabData2 = tabData as ITabData2;
                ToolTipInfo superTooltip = null;

                if (tabData2 != null)
                {
                    superTooltip = tabData2.SuperTooltip;
                }

                return superTooltip;
            }
            set
            {
                ITabData2 tabData2 = tabData as ITabData2;

                if (tabData2 != null)
                {
                    tabData2.SuperTooltip = value;
                }
            }
        }

        /// <summary>
        /// Gets / sets the index to the image displayed on this tab.
        /// </summary>
        /// <remarks>
        /// The zero-based index to the image in the TabControlAdv.ImageList 
        /// that appears on the tab. The default is -1, which signifies no image.
        /// <para>
        /// The ImageIndex points to an image in the TabControlAdv 
        /// object's associated ImageList.
        /// </para>
        /// </remarks>
        [
        Localizable(true),
        DefaultValue(-1 /*0xFFFFFFFF*/),
        Category("Appearance"),
        Description(@"Points to the image in the associated imageList that is used for this tab."),
        TypeConverter(typeof(ImageIndexConverter)),
        Editor(typeof(Syncfusion.Windows.Forms.Design.ImageIndexEditor), typeof(UITypeEditor))
        ]
        public virtual int ImageIndex
        {
            get
            {
                if (tabData != null)
                    return this.tabData.ImageIndex;
                else return -1;
            }
            set
            {
                if (tabData != null)
                    this.tabData.ImageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [Description("Image to be displayed over the tab. Animated image will be animated")]
        public Image Image
        {
            get { return this.tabData.Image; }
            set
            {
                this.tabData.Image = value;
                this.tabData.ImageChanged = true;
                Invalidate();
            }
        }
        /// <summary>
        /// Get or Set Close Button
        /// </summary>
			private bool showCloseButton = true;
			public bool ShowCloseButton
			{
			get {
					return showCloseButton;
				}
			set {
				if(showCloseButton != value )
				showCloseButton = value;
                if (this.Parent is TabControlAdv)
                {
                    if ((this.Parent as TabControlAdv).ShowTabCloseButton)
                    (this.Parent as TabControlAdv).OnShowCloseButtonChanged();
                }
				this.Invalidate();
				}
			}

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        [Description("Size of the image added in Image property")]
        public Size ImageSize
        {
             get
             {
                 return this.tabData.ImageSize;
             }
             set
             {
                 this.tabData.ImageSize = value;
                 Invalidate();
             }
        }
      
        /// <summary>
        /// Indicates whether to show a particular tab.
        /// </summary>
        /// <value>True to show the tab; false otherwise.</value>
        /// <remarks>
        /// <para>When the tab is made invisible, you can still show the tab page by setting the appropriate
        /// <see cref="TabControlAdv.SelectedIndex"/> programmatically.</para>
        /// <para>
        /// In fact, you can hide all the tabs and operate the tab like a Wizard. But also note that 
        /// Essential Tools provides a separate WizardControl for that purpose.
        /// </para>
        /// </remarks>
        [
        DefaultValue(true),
        Category("Appearance"),
        Description("Specifies whether or not to show a particular tab.")
        ]
        public virtual bool TabVisible
        {
            get
            {
                if (tabData != null)
                    return this.tabData.TabVisible;
                else return true;
            }
            set
            {
                if (tabData != null)
                {
                    this.tabData.TabVisible = value;

                    ValidateTabPagesVisibility();
                }
            }
        }

        /// <summary>
        /// Invisible tabPage cannot be shown in runtime.
        /// </summary>
        private void ValidateTabPagesVisibility()
        {
            if (!DesignMode && Parent != null && Parent is TabControlAdv)
            {
                TabControlAdv parent = Parent as TabControlAdv;

                if (parent.SelectedIndex != -1 && !parent.TabPages[parent.SelectedIndex].TabVisible)
                {
                    bool isSet = false;
                    for (int i = parent.SelectedIndex; i >= 0; i--)
                    {
                        if (parent.TabPages[i].TabVisible)
                        {
                            parent.SelectedIndex = i;
                            isSet = true;
                            break;
                        }
                    }
                    
                    if (!isSet)
                    {
                        for (int i = parent.SelectedIndex; i < parent.TabPages.Count - 1; i++)
                        {
                            if (parent.TabPages[i].TabVisible)
                            {
                                parent.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Overridden. Gets / sets the text to display on the tab.
        /// </summary>
        /// <value>The text to display on the tab.</value>
        [
        Browsable(true),
        Localizable(true),
        Description("Overridden. Gets or sets the text to display on the tab."),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))
        ]
        public override string Text
        {
            get
            {
                if (tabData != null)
                    return this.tabData.Text;
                else return "";
            }
            set
            {
                if (tabData != null)
                    this.tabData.Text = value;
            }
        }


        /// <summary>
        /// Gets / sets the font used to display text in the tab.
        /// </summary>
        /// <value>The Font object.</value>
        /// <remarks>This tab page belongs to a TabControlAdv instance. 
        /// The Font specified here will be used when rendering the 
        /// associated tab in the TabControlAdv.
        /// </remarks>
        [
        Category(@"Appearance"),
        Localizable(true),
        AmbientValue(null),
        Description(@"The font used to display text in this Tab."),
        ]
        public virtual Font TabFont
        {
            get
            {
                Font tabFont = null;
                if (tabData != null)
                {
                    tabFont = this.tabData.Font;
                }
                if (tabFont == null && this.tabPanelDefaultProperties != null)
                {
                    tabFont = this.tabPanelDefaultProperties.DefaultTabPanelFont();
                }

                return tabFont;
            }
            set
            {
                if (tabData != null)
                    this.tabData.Font = value;
            }
        }
        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv.TabFont"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeTabFont()
        {
            if (this.tabPanelDefaultProperties == null)
                return true;

            Font defaultFont = this.tabPanelDefaultProperties.DefaultTabPanelFont();
            return !this.TabFont.Equals(defaultFont);
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv.TabFont"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetTabFont()
        {
            this.TabFont = null;
        }

        /// <summary>
        /// Gets / sets the background color of this tab. Will override the Active Tab and Inactive Tab Colors.
        /// </summary>
        /// <remarks>This tab page belongs to a TabControlAdv instance. 
        /// The color specified here will be used when rendering the 
        /// associated tab in the TabControlAdv.
        /// </remarks>
        [
        Category(@"Appearance"),
        Description(@"Background Color of this Tab. Will override the Active Tab and Inactive Tab Colors."),
        ]
        public virtual Color TabBackColor
        {
            get
            {
                Color tabBackColor = Color.Empty;
                if (tabData != null)
                    tabBackColor = this.tabData.BackColor;

                if (tabBackColor == Color.Empty && this.tabPanelDefaultProperties != null)
                    tabBackColor = this.tabPanelDefaultProperties.DefaultInactiveTabColor();

                return tabBackColor;
            }
            set
            {
                if (tabData != null)
                    this.tabData.BackColor = value;
            }
        }

        /// <summary>
        /// Gets / sets the forecolor of this tab. Default is SystemColors.WindowText.
        /// </summary>
        /// <remarks>This tab page belongs to a TabControlAdv instance. 
        /// The color specified here will be used when rendering the 
        /// associated tab in the TabControlAdv. The setting will not affect the Controls in the tab page.
        /// </remarks>
        [
        Category(@"Appearance"),
        Description(@"Fore Color of this Tab. Default is SystemColors.WindowText."),
        ]
        public virtual Color TabForeColor
        {
            get
            {
                Color tabForeColor = Color.Empty;
                if (tabData != null)
                    tabForeColor = this.tabData.ForeColor;

                if (tabForeColor == Color.Empty && this.tabPanelDefaultProperties != null)
                    tabForeColor = this.tabPanelDefaultProperties.DefaultTabForeColor();

                return tabForeColor;
            }
            set
            {
                if (tabData != null)
                    this.tabData.ForeColor = value;
            }
        }
        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv.TabForeColor"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeTabForeColor()
        {
            if (this.TabForeColor == Color.Empty)
                return false;

            if (this.tabPanelDefaultProperties == null)
                return true;

            Color tabForeColor = this.tabPanelDefaultProperties.DefaultTabForeColor();
            return (this.TabForeColor != tabForeColor);
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv.TabForeColor"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetTabForeColor()
        {
            this.TabForeColor = Color.Empty;
        }
        /// <summary>
        /// Indicates whether the current value of the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv.TabBackColor"/> property is to be serialized.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeTabBackColor()
        {
            if (this.TabBackColor == Color.Empty)
                return false;

            if (this.tabPanelDefaultProperties == null)
                return true;

            Color tabColor = this.tabPanelDefaultProperties.DefaultInactiveTabColor();
            return (this.TabBackColor != tabColor);
        }
        /// <summary>
        /// Resets the <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdv.TabBackColor"/> property to its default value.
        /// </summary>
        /// <remarks>
        /// You typically use this method if you are either creating a designer for the Control or creating your own control incorporating this Control.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetTabBackColor()
        {
            this.TabBackColor = Color.Empty;
        }

        /// <override/>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            bool bgPainted = false;
            if (this.ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive
              && this.Parent is ITabControl
              )
            {
                ITabControl tabControl = this.Parent as ITabControl;
                if (tabControl.ThemedDrawing != null)
                {
                    int ox = (int)pevent.ClipRectangle.Left;
                    int oy = (int)pevent.ClipRectangle.Top;
                    int dx = (int)pevent.ClipRectangle.Width;
                    int dy = (int)pevent.ClipRectangle.Height;

                    if ((ox != 0) || (oy != 0) || (dx != this.ClientRectangle.Width) || (dy != this.ClientRectangle.Height))
                    {
                        bgPainted = this.PaintChildrenBackground(pevent.Graphics, this, pevent.ClipRectangle);
                    }
                    if (!bgPainted)
                    {
                        if (dx > this.ClientRectangle.Width
                            || dy > this.ClientRectangle.Height)
                        {
                            tabControl.ThemedDrawing.DrawTabBody(pevent.Graphics, pevent.ClipRectangle);
                        }
                        else
                        {
                            tabControl.ThemedDrawing.DrawTabBody(pevent.Graphics, this.ClientRectangle, pevent.ClipRectangle);
                        }
                    }

                    bgPainted = true;
                }
            }

            if (!bgPainted)
                base.OnPaintBackground(pevent);
        }

        private void ThemedPaintBackground(System.Drawing.Graphics graphics, Rectangle rect, Rectangle clip)
        {
            ITabControl tabControl = this.Parent as ITabControl;
            if (tabControl.ThemedDrawing != null)
            {
                tabControl.ThemedDrawing.DrawTabBody(graphics, rect, clip);
            }
        }
        private bool PaintChildrenBackground(System.Drawing.Graphics graphics, System.Windows.Forms.Control control, Rectangle clipRect)
        {
            foreach (System.Windows.Forms.Control child in control.Controls)
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if (child.BackColor == Color.Transparent) continue;
#endif
                System.Drawing.Rectangle childBounds = new System.Drawing.Rectangle(child.Location, child.Size);
                childBounds = child.Parent.RectangleToScreen(childBounds);
                childBounds = this.RectangleToClient(childBounds);

                if (childBounds.Contains(clipRect))
                {
                    if (this.PaintChildrenBackground(graphics, child, clipRect))
                    {
                        return true;
                    }

                    Rectangle client = this.ClientRectangle;
                    client = this.RectangleToScreen(client);
                    client = child.RectangleToClient(client);

                    clipRect = this.RectangleToScreen(clipRect);
                    clipRect = child.RectangleToClient(clipRect);
                    this.ThemedPaintBackground(graphics, client, clipRect);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether or not themes should be used to draw this tab page.
        /// </summary>
        /// <value>
        /// True if enabled; false otherwise.
        /// </value>
        /// <remarks>
        /// <para>
        /// By default, the value for this property is inherited from the parent TabControlAdv.
        /// You can explicitly set it to false if you want to turn off themed drawing of the background of this
        /// tab page.
        /// </para>
        /// </remarks>
        [
        Category(@"Appearance"),
        Description("Specifies whether or not themes should be used to draw this tab page.")
        ]
        public bool ThemesEnabled
        {
            get
            {
                if (this.customThemesEnabledFlag && this.Parent is TabControlAdv)
                    return ((TabControlAdv)this.Parent).ThemesEnabled;
                else
                    return false;
            }
            set
            {
                if (customThemesEnabledFlag != value)
                {
                    customThemesEnabledFlag = value;
                    this.Invalidate();
                }
            }
        }
        [Documentation.DocumentationExclude()]
        protected void ResetThemesEnabled()
        {
            this.customThemesEnabledFlag = true;
        }
        [Documentation.DocumentationExclude()]
        protected bool ShouldSerializerThemesEnabled()
        {
            return this.customThemesEnabledFlag == false;
        }
    }

    /// <summary>
    /// Contains a collection of TabPageAdv objects.
    /// </summary>
    public class TabPageAdvCollection : ArrayList
    {
        private TabDataCollection tabDataCollection;
        private TabPanelData tabPanelData;
        private Control parent;
        private ITabPanelDefaultProperties tabPanelDefaultProperties;

        /// <summary>
        /// Creates a new instance of the <see cref="TabPageAdvCollection"/> class.
        /// </summary>
        /// <param name="tabPanelData">A <see cref="TabPanelData"/> instance.</param>
        /// <param name="parent">The parent tab control.</param>
        /// <param name="tabPanelDefaultProperties">An instance of the <see cref="ITabPanelDefaultProperties"/> interface.</param>
        public TabPageAdvCollection(TabPanelData tabPanelData, Control parent, ITabPanelDefaultProperties tabPanelDefaultProperties)
        {
            this.tabPanelData = tabPanelData;
            this.tabDataCollection = tabPanelData.TabsData;
            this.parent = parent;
            this.tabPanelDefaultProperties = tabPanelDefaultProperties;

            this.tabDataCollection.CollectionAffected += new EventHandler(this.TabDataCollection_Changed);
        }

        private void TabDataCollection_Changed(object sender, EventArgs e)
        {
            // Make sure the tabpages and the tabdata are in the same order.
            int i = -1;
            foreach (ITabData tabData in tabDataCollection)
            {
                i++;
                if (((TabPageAdv)this[i]).TabData == tabData)
                    continue;
                else
                {
                    // Collections not in sync.
                    // Adjust positions
                    int curTabPos = GetTabPageIndexOfTabData(tabData);
                    if (curTabPos != -1)
                        MoveTabPageInternal(curTabPos, i);
                }
            }
        }

        /// <summary>
        /// Moves tab pages around without removing and re-inserting them.
        /// </summary>
        /// <param name="oldTabPos">The old position.</param>
        /// <param name="newTabPos">The new position.</param>
        public void MoveTabPage(int oldTabPos, int newTabPos)
        {
            if (oldTabPos >= this.Count || newTabPos >= this.Count)
                return;

            this.tabPanelData.TabsData.Move(oldTabPos, newTabPos, 1);
        }

        /// <summary>
        /// Moves tab pages around without removing and re-inserting them.
        /// </summary>
        /// <param name="oldTabPos">The old position.</param>
        /// <param name="newTabPos">The new position.</param>
        private void MoveTabPageInternal(int oldTabPos, int newTabPos)
        {
            if (oldTabPos >= this.Count || newTabPos >= this.Count)
                return;

            // Make sure to call the base class Remove and Insert
            // Remove tab page
            TabPageAdv tabPage = this[oldTabPos];
            base.RemoveAt(oldTabPos);
            base.Insert(newTabPos, tabPage);
        }

        private int GetTabPageIndexOfTabData(ITabData tabData)
        {
            int i = -1;
            foreach (TabPageAdv tabPage in this)
            {
                i++;
                if (tabPage.TabData == tabData)
                    return i;
            }
            return -1;
        }

        internal void InitTabPage(TabPageAdv tabPage)
        {
            // Init tabpage settings
            tabPage.TabPanelDefaultProperties = tabPanelDefaultProperties;
            tabPage.Visible = false;
        }

        /// <summary>
        /// Returns the <see cref="TabPageAdv"/> instance of the specified <see cref="ITabData"/>.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="tabData">The ITabData instance.</param>
        /// <returns>A <see cref="TabPageAdv"/> instance. Can be null.</returns>
        public static TabPageAdv GetTabPageAdvOfTabData(TabPageAdvCollection collection, ITabData tabData)
        {
            foreach (TabPageAdv tabPage in collection)
            {
                if (tabPage.TabData == tabData)
                    return tabPage;
            }
            return null;
        }
        #region OVERRIDEN_VIRTUALS

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.GetEnumerator()"/>.
        /// </summary>
        public override /*IEnumerable*/ IEnumerator GetEnumerator()
        {
            TabPageAdv[] tabPages;
            tabPages = (TabPageAdv[])base.ToArray(typeof(TabPageAdv));
            if (tabPages != null)
                return tabPages.GetEnumerator();
            else
                return new TabPageAdv[0].GetEnumerator();
        }

        public override IEnumerator GetEnumerator(int index, int count)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException(@"index");
            if (index + count - 1 >= this.Count)
                throw new ArgumentOutOfRangeException(@"count");

            Array subsetArray = Array.CreateInstance(typeof(TabPageAdv), count);
            this.CopyTo(index, subsetArray, 0, count);
            return subsetArray.GetEnumerator();
        }

        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.RemoveAt"/>.
        /// </summary>
        /// <param name="index">The index of the tab to remove.</param>
        public override void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException(@"index");

            TabPageAdv tabPage = this[index];
            base.RemoveAt(index);
            this.tabDataCollection.RemoveAt(index);

            this.parent.Controls.Remove(tabPage);
        }
        /// <summary>
        /// This method ensures that the obj argument is of type TabPageAdv.
        /// </summary>
        /// <param name="obj">The object to remove.</param>
        public override void Remove(object obj)
        {
            if (!(obj is TabPageAdv))
                throw new System.ArgumentException("Only objects of type TabPageAdv can be removed from the list", "value");
            base.Remove(obj);
        }
        public override void RemoveRange(int index, int count)
        {
            if (index < 0 || count < 0 || index >= this.Count || (index + count - 1) >= this.Count)
                throw new ArgumentOutOfRangeException("index", "Specified index is out of range of the array list.");

            for (int i = 0; i < count; i++)
            {
                this.RemoveAt(index);
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Insert"/>.
        /// </summary>
        public override void Insert(int index, object value)
        {
            if (!(value is TabPageAdv))
                throw new System.ArgumentException("Only objects of type TabPageAdv can be Inserted from the list", "value");

            this.InitTabPage((TabPageAdv)value);

            base.Insert(index, value);
            this.tabDataCollection.Insert(index, ((TabPageAdv)value).TabData);
            this.parent.Controls.Add((Control)value);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Collections.ArrayList.Clear"/>.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
			this.tabDataCollection.Clear();
        }
        public override int Add(object value)
        {
            this.InitTabPage((TabPageAdv)value);

            int index = base.Add(value);

            this.tabDataCollection.Add(((TabPageAdv)value).TabData);
            this.parent.Controls.Add((Control)value);

            return index;
        }

        /// <summary>
        /// Sort is disabled for this collection.
        /// </summary>
        public override void Sort(int index, int count, IComparer comparer)
        {
            // Does nothing;
        }
        /// <summary>
        /// Reverse is disabled for this collection.
        /// </summary>
        public override void Reverse(int index, int count)
        {
            // Does nothing;
        }

        /// <summary>
        /// InsertRange is disabled for this collection.
        /// </summary>
        public override void InsertRange(int index, ICollection c)
        {
            throw new NotSupportedException("This method is not supported. Use AddRange instead.");
        }
        /// <override/>
        public override void AddRange(ICollection c)
        {
            foreach (object o in c)
            {
                this.Add(o);
            }
        }
        /// <summary>
        /// SetRange is disabled for this collection.
        /// </summary>
        public override void SetRange(int index, ICollection c)
        {
            // Does nothing;
        }
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override int Count { get { return base.Count; } }
        #endregion OVERRIDEN_VIRTUALS
        #region MODIFIED_INTERFACE
        /// <summary>
        /// Gets / sets a TabPageAdv with the specified index in the collection. 
        /// In C#, this property is the indexer for the TabPageAdvCollection class.
        /// </summary>
        /// <param name="index">The zero-based index of the tab page to get / set.</param>
        /// <value>The TabPageAdv at the specified index.</value>
        public new TabPageAdv this[int index]
        {
            get
            {
                return (TabPageAdv)base[index];
            }
            set
            {
                base[index] = value;
                tabDataCollection[index] = value.TabData;
            }
        }

        /// <summary>
        /// Adds a TabPageAdv to the collection.
        /// </summary>
        /// <param name="value">The TabPageAdv to add.</param>
        public void Add(TabPageAdv value)
        {
            this.Add((object)value);
        }

        /// <summary>
        /// Indicates whether a specified tab page is in the collection.
        /// </summary>
        /// <param name="page">The TabPageAdv to locate in the collection. </param>
        /// <returns>True if the specified TabPageAdv is in the collection; false otherwise.</returns>
        public bool Contains(TabPageAdv page)
        {
            return this.Contains((object)page);
        }
        /// <summary>
        /// Returns the index of the specified tab page in the collection.
        /// </summary>
        /// <param name="page">The TabPageAdv to locate in the collection.</param>
        /// <returns>The zero-based index of the tab page; -1 if it cannot be found.</returns>
        public int IndexOf(TabPageAdv page)
        {
            return this.IndexOf((object)page);
        }
        /// <summary>
        /// Removes a TabPageAdv from the collection.
        /// </summary>
        /// <param name="value">The TabPageAdv to remove.</param>
        public void Remove(TabPageAdv value)
        {
            this.Remove((object)value);
        }
        #endregion MODIFIED_INTERFACE
    }

    /// <summary>
    /// The TabControlExt type will soon be replaced with the TabControlAdv for consistency in 
    /// Control naming in our library. 
    /// Please replace all occurrences of TabControlExt with TabControlAdv in your app.
    /// </summary>
    [Obsolete("The TabControlExt type will soon be replaced with the TabControlAdv for consistency in naming in our library. Please replace all occurences of TabControlExt with TabControlAdv in your app."),
    ToolboxItem(false)]
    public class TabControlExt : TabControlAdv
    {
        public TabControlExt() : base() { }

    }

    /// <summary>
    /// Contains information about the <see cref="TabControlAdv.TabMoving"/>
    /// event.
    /// </summary>
    public class TabMovingEventArgs : SyncfusionCancelEventArgs
    {
        private int from;
        private int target;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabMovingEventArgs"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="target">The target.</param>
        public TabMovingEventArgs(int from, int target)
        {
            this.from = from;
            this.target = target;
        }

        /// <summary>
        /// Gets the index of Tab to be moved.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get { return from; }
        }

        /// <summary>
        /// Gets the Tab index where the Tab should be inserted.
        /// </summary>
        [TraceProperty(true)]
        public int Target
        {
            get { return target; }
        }
    }

    /// <summary>
    /// Contains information about the <see cref="TabControlAdv.BeforeEdit"/> and
    /// <see cref="TabControlAdv.AfterEdit"/> events.
    /// </summary>
    public class EditEventArgs : EventArgs
    {
        /// <summary>
        /// Edit edit text
        /// </summary>
        private string m_editText;

        /// <summary>
        /// Gets edit text
        /// </summary>
        public string EditText
        {
            get
            {
                return m_editText;
            }
            set
            {
                if (value != m_editText)
                {
                    m_editText = value;
                }
            }
        }

        public EditEventArgs(string editText)
        {
            m_editText = editText;
        }
    }

    /// <summary>
    /// Handles the <see cref="TabControlAdv.Closing"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.TabPageAdvClosingEventArgs"/> that contains the event data.</param>
    public delegate void TabPageAdvClosingEventHandler(object sender, TabPageAdvClosingEventArgs args);

    /// <summary>
    /// Contains information about the <see cref="TabControlAdv.Closing"/> event.
    /// </summary>
    public class TabPageAdvClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// TabPageAdv that is closing.
        /// </summary>
        private TabPageAdv m_tabPage;

        /// <summary>
        /// Gets TabPageAdv that is closing
        /// </summary>
        public TabPageAdv TabPageAdv
        {
            get
            {
                return m_tabPage;
            }
        }

        /// <summary>
        /// Creates a new instance of this class with the specified parameters.
        /// </summary>
        public TabPageAdvClosingEventArgs(TabPageAdv tabPage)
            : base(false)
        {
            m_tabPage = tabPage;
        }

        /// <summary>
        /// Creates a new instance of this class with the specifed parameters.
        /// </summary>
        public TabPageAdvClosingEventArgs(TabPageAdv tabPage, bool cancel)
            : base(cancel)
        {
            m_tabPage = tabPage;
        }
    }

    /// <summary>
    /// Contains information about the <see cref="TabControlAdv.SelectedIndexChanging"/> event.
    /// </summary>
    public class SelectedIndexChangingEventArgs : CancelEventArgs
    {
        private int newSelectedIndex;
        /// <summary>
        /// Creates a new instance of this class with the specifed parameters.
        /// </summary>
        /// <param name="newSelectedIndex">The new selected index that will be set.</param>
        public SelectedIndexChangingEventArgs(int newSelectedIndex)
        {
            this.newSelectedIndex = newSelectedIndex;
        }
        /// <summary>
        /// Returns the newly selected tab index that is going to be set in the tab control.
        /// </summary>
        /// <example>
        /// You can get access to the corresponding <see cref="TabPageAdv"/> using the <see cref="TabControlAdv.TabPages"/>
        /// property, as follows:
        /// <code lang="C#">
        /// private void tabControlAdv1_SelectedIndexChanging(object sender, Syncfusion.Windows.Forms.Tools.SelectedIndexChangingEventArgs args)
        /// {
        /// 	TabPageAdv newPage = this.tabControlAdv1.TabPages[args.NewSelectedIndex];
        /// 	if(newPage == this.tab1)
        /// 	{
        /// 		MessageBox.Show("Cannot select tab page 1");
        /// 		args.Cancel = true;
        /// 	}
        /// }
        /// </code>
        /// <code lang="VB">
        /// Private  Sub tabControlAdv1_SelectedIndexChanging(ByVal sender As Object, ByVal args As Syncfusion.Windows.Forms.Tools.SelectedIndexChangingEventArgs)
        /// 	Dim NewPage As TabPageAdv =  Me.tabControlAdv1.TabPages(args.NewSelectedIndex) 
        /// 	If NewPage = Me.tab1 Then
        /// 		MessageBox.Show("Cannot select tab page 1")
        /// 		args.Cancel = True
        /// 	End If
        /// End Sub
        /// </code>
        /// </example>
        public int NewSelectedIndex
        {
            get { return this.newSelectedIndex; }
        }
    }
    /// <summary>
    /// The TabPageExt type will soon be replaced with the TabPageAdv for consistency in 
    /// Control naming in our library. 
    /// Please replace all occurrences of TabPageExt with TabPageAdv in your app.
    /// </summary>
    [Obsolete("The TabPageExt type will soon be replaced with the TabPageAdv for consistency in naming in our library. Please replace all occurences of TabPageExt with TabPageAdv in your app."),
    ToolboxItem(false)]
    public class TabPageExt : TabPageAdv
    {
        public TabPageExt() : base() { }
        public TabPageExt(ITabData tabData, ITabPanelDefaultProperties tabPanelDefaultProperties)
            : base(tabData, tabPanelDefaultProperties)
        {
        }
        public TabPageExt(string label) : base(label) { }
    }

    internal enum ValidateStatus { None, Failed, Passed }
}
