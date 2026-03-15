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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Dialog form for quick panel customization.
	/// </summary>
	internal partial class Office2013CustomizeQuickItemsDialog:
		MetroForm
	{
        
        private static readonly Color c_borderColorOffice2007 = Color.FromArgb(200, 177, 128);
        private static readonly Color c_foreColorTopFirstOffice2007 = Color.FromArgb(255, 253, 235);
        private static readonly Color c_foreColorTopLastOffice2007 = Color.FromArgb(255, 235, 178);
        private static readonly Color c_foreColorBottomFirstOffice2007 = Color.FromArgb(255, 213, 98);
        private static readonly Color c_foreColorBottomLastOffice2007 = Color.FromArgb(255, 228, 145);
        private static readonly Color c_foreColorBottomLineOffice2007 = Color.FromArgb(255, 248, 181);
        private int btnAddLocationY = 251;
        private int btnRemoveLocationY = 278;
        private int btnUpLocationY = 251;
        private int btndownLocationY = 277;
        string QuickAccessToolBarLabel = string.Empty;

		#region Initialization
		/// <summary>
		/// Creates & initializes new instance of Office2013CustomizeQuickItemsDialog.
		/// </summary>
		/// <param name="header">RibbonControlAdvHeader instance to work with.</param>
        public Office2013CustomizeQuickItemsDialog(RibbonControlAdvHeader header)
		{
			InitializeComponent();
            this.CaptionAlign = HorizontalAlignment.Center;
            using (Bitmap bit = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if (g.DpiY > 96)
                    {
                        this.MinimumSize = new Size(879, 683);
                    }
                    else
                    {
                        this.lstAvailableItems.Size = this.lstAvailableItems.Size;
                        this.lstChosenItems.Size = this.lstChosenItems.Size;
                        this.MinimumSize = new Size(653, 547);
                    }
                }
            }
            if (header != null)
            {
                RibbonControlAdv ribbonControl = header.Parent as RibbonControlAdv;
                this.chkPlaceBelowRibbon.Checked = ribbonControl.customizeQTACheckBoxAdvChecked;
                if (ribbonControl != null && ribbonControl.QuickPanelImage != null)
                {
                    this.pictureBox1.Image = ribbonControl.QuickPanelImage;
                    this.pictureBox1.SizeMode = ribbonControl.QuickPanelImageLayout;
                }
            }
            this.comboPanels.Width = this.lstAvailableItems.Width;
			m_header = header;
            InitLocalizedResources();


			SetupHeaderDependencies();
			ApplyColorSchemeToControls();

			m_reflectedComponents = new List<Component>();

			m_Quick = new List<Component>( m_header.QuickItems.Count );

			foreach( ToolStripItem item in m_header.QuickItems )
			{
				IQuickItem reflectableItem = item as IQuickItem;

				if( reflectableItem != null )
				{
					m_reflectedComponents.Add( reflectableItem.ReflectedComponent );
					m_Quick.Add( reflectableItem.ReflectedComponent );
				}
				else
				{
					m_Quick.Add( item );
				}
			}

			ExtractPanels();

			UpdateDestinationList();
		}
        void Office2013CustomizeQuickItemsDialog_Load(object sender, System.EventArgs e)
        {
            btnAdd.UseVisualStyle = true;
            btnCancel.UseVisualStyle = true;
            btnRemove.UseVisualStyle = true;
            btnReset.UseVisualStyle = true;
            btnOK.UseVisualStyle = true;
            btnUp.UseVisualStyle = true;
            btnDown.UseVisualStyle = true;
            btnAdd.Appearance = ButtonAppearance.Metro;
            btnCancel.Appearance = ButtonAppearance.Metro;
            btnReset.Appearance = ButtonAppearance.Metro;
            btnRemove.Appearance = ButtonAppearance.Metro;
            btnOK.Appearance = ButtonAppearance.Metro;
            btnDown.Appearance = ButtonAppearance.Metro;
            btnUp.Appearance = ButtonAppearance.Metro;
            chkPlaceBelowRibbon.Style = CheckBoxAdvStyle.Metro;
            comboPanels.Style = VisualStyle.Metro;
        }
        public virtual void InitLocalizedResources()
        {
            btnAdd.Text = SR.GetString(SR.QuickAccessDialogButtonAdd, m_header.Parent as RibbonControlAdv);
            btnRemove.Text = SR.GetString(SR.QuickAccessDialogButtonRemove, m_header.Parent as RibbonControlAdv);
            btnReset.Text = SR.GetString(SR.QuickAccessDialogButtonReset, m_header.Parent as RibbonControlAdv);
            btnCancel.Text = SR.GetString(SR.QuickAccessDialogButtonCancel, m_header.Parent as RibbonControlAdv);
            chkPlaceBelowRibbon.Text = SR.GetString(SR.QuickAccessPlaceBelowRibbon, m_header.Parent as RibbonControlAdv);
            lblChooseCommandsFrom.Text = SR.GetString(SR.QuickAccessDialogCommands, m_header.Parent as RibbonControlAdv);
            label1.Text = SR.GetString(SR.CustomizationLabel, m_header.Parent as RibbonControlAdv);
            label2.Text = SR.GetString(SR.CustomizeQuickAccessLabel, m_header.Parent as RibbonControlAdv);
            string CustomizationLabel = SR.GetString(SR.CustomizationLabel, m_header.Parent as RibbonControlAdv);
            string CustomizeQuickAccessLabel = SR.GetString(SR.CustomizeQuickAccessLabel, m_header.Parent as RibbonControlAdv);
            QuickAccessToolBarLabel = SR.GetString(SR.QuickAccessToolBarLabel, m_header.Parent as RibbonControlAdv);
            label3.Text = WordWrap(QuickAccessToolBarLabel, 45);
            this.label3.Location = new System.Drawing.Point(label3.Location.X, label3.Location.Y - (this.label3.Height-15));
            OnResize(EventArgs.Empty);
        }

        private static string WordWrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();
           
            if (width < 1)
                return text;

            for (pos = 0; pos < text.Length; pos = next)
            {
                int eol = text.IndexOf(Environment.NewLine, pos);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);
                        pos += len;
                        while (pos < eol && Char.IsWhiteSpace(text[pos]))
                            pos++;
                    } while (eol > pos);
                }
                else sb.Append(Environment.NewLine); 
            }
            return sb.ToString();
        }

        private static int BreakLine(string text, int pos, int max)
        {
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max;
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;
            return i + 1;
        }

		/// <summary>
		/// 
		/// </summary>
		private void SetupHeaderDependencies()
		{
			m_header.RendererChanged += new EventHandler( HeaderRendererChanged );

			RibbonControlAdv parenRibbonCtl = m_header.Parent as RibbonControlAdv;

			if( parenRibbonCtl != null )
			{
				//this.ColorScheme = ColorSchemeConverter.ToOffice2007Theme( parenRibbonCtl.OfficeColorScheme );
			}
		}
		#endregion

		#region Static Methods
		public static void Execute( RibbonControlAdvHeader header )
		{
            using (Office2013CustomizeQuickItemsDialog dlg = new Office2013CustomizeQuickItemsDialog(header))
			{
				dlg.ShowDialog(header.Parent);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		private System.Text.RegularExpressions.Regex Regex
		{
			get
			{
				if( m_regex == null )
				{
					m_regex = new System.Text.RegularExpressions.Regex( @"\s+" );
				}
				return m_regex;
			}
		}
		#endregion

		#region Overrides

		protected override void SetVisibleCore( bool value )
		{
			if( value )
			{
				UpdateLabels();
			}
			base.SetVisibleCore( value );
		}

        //protected override void OnColorSchemeChanged()
        //{
        //    base.OnColorSchemeChanged();

        //    ApplyColorSchemeToControls();
        //}

		#endregion

		#region Event Handlers
		/// <summary>
		/// Fills source list view with new items from selected tool strip.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnComboToolstripsSelectedIndexChanged( object sender, EventArgs e )
		{
			RibbonPanelItem panelItem = comboPanels.SelectedItem as RibbonPanelItem;
			if( panelItem != null )
			{
				UpdateSourceListFromPanel( panelItem.Panel );
			}
			else
			{
				StartPanel sp = comboPanels.SelectedItem as StartPanel;
				if( sp != null )
				{
					UpdateSourceListFromDropDown( sp.DropDown );
				}
			}
		}
		/// <summary>
		/// Updates state of Add button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLstAvailableItemsSelectedIndexChanged( object sender, EventArgs e )
		{
			bool bEnable = ( lstAvailableItems.SelectedItems.Count > 0 );

			foreach( ListViewItem listViewitem in lstAvailableItems.SelectedItems )
			{
				Component comp = listViewitem.Tag as Component;

				if( m_reflectedComponents.Contains( comp ) )
				{
					bEnable = false;
					break;
				}
			}
			btnAdd.Enabled = bEnable;
		}
		/// <summary>
		/// Updates availability of Remove button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLstChosenItemsSelectedIndexChanged( object sender, EventArgs e )
		{
			btnRemove.Enabled = ( lstChosenItems.SelectedItems.Count > 0 );

			ListView.SelectedIndexCollection indexes = lstChosenItems.SelectedIndices;

			if( indexes.Count == 1 )
			{
				int index = lstChosenItems.SelectedIndices[0];

				btnDown.Enabled = !( index == lstChosenItems.Items.Count - 1 );
				btnUp.Enabled = !( index == 0 );
			}
			else
			{
				btnDown.Enabled = false;
				btnUp.Enabled = false;
			}
		}
		/// <summary>
		/// Adds items to destination list view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBtnAddClick( object sender, EventArgs e )
		{
			AddItemsToDestinationList( lstAvailableItems.SelectedItems );

			btnAdd.Enabled = false;
		}
		/// <summary>
		/// Removes items from destination list view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBtnRemoveClick( object sender, EventArgs e )
		{
			RemoveItemsFromDestinationList( lstChosenItems.SelectedItems );
			btnRemove.Enabled = false;
		}
		/// <summary>
		/// Adds items to destination list on mouse double click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLstAvailableItemsDoubleClick( object sender, EventArgs e )
		{
			Point localPoint = lstAvailableItems.PointToClient( Cursor.Position );
			ListViewItem item = lstAvailableItems.GetItemAt( localPoint.X, localPoint.Y );

			if( item != null )
			{
				AddItemsToDestinationList( new ListViewItem[] { item } );
				btnAdd.Enabled = false;
			}
		}
		/// <summary>
		/// Removed items from destination list on mouse double click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLstChosenItemsDoubleClick( object sender, EventArgs e )
		{
			RemoveItemsFromDestinationList( lstChosenItems.SelectedItems );
			btnRemove.Enabled = false;
		}
		/// <summary>
		/// Updates colection of quick buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBtnOKClick( object sender, EventArgs e )
		{
			RibbonControlAdvHeader.QuickItemsCollection quickItems = m_header.QuickItems as RibbonControlAdvHeader.QuickItemsCollection;

			if( quickItems != null )
			{
				ToolStrip owner = quickItems.Owner;

				if( owner != null )
				{
					owner.SuspendLayout();
				}

				ArrayList componentsList = new ArrayList( m_reflectedComponents );
				ArrayList quickList = new ArrayList( quickItems );

				foreach( ToolStripItem item in quickList )
				{
					IQuickItem quickItem = item as IQuickItem;

					if( quickItem != null )
					{
						int index = componentsList.IndexOf( quickItem.ReflectedComponent );

						if( index < 0 )
						{
							quickItems.Remove( item );
						}
						else
						{
							componentsList.RemoveAt( index );
						}
					}
				}
				foreach( Component c in componentsList )
				{
					ToolStripItem item = QuickToolstripReflectable.GetItemToReflect( c );
					FieldInfo eventsField = typeof(Component).GetField("events", BindingFlags.NonPublic | BindingFlags.Instance);
					EventHandlerList eventHandlerList = (EventHandlerList)eventsField.GetValue(c);
					eventsField.SetValue(item, eventHandlerList);
                    item.Tag = "added by QAT";
					m_header.AddQuickItem( item );
				}

				QuickComparer comparer = new QuickComparer( m_Quick );
				quickItems.Sort( comparer );
                if (m_header.Parent != null)
                {
                    RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
                    ribbonControl.ShowQuickPanelBelowRibbon = ribbonControl.customizeQTACheckBoxAdvCheckedSave = this.chkPlaceBelowRibbon.Checked;
                }
				quickItems.UpdateAccelerators( null );

				if( owner != null )
				{
					owner.ResumeLayout( false );
					owner.PerformLayout();
				}
			}
		}
		/// <summary> Method clears destination list. </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReset_Click( object sender, EventArgs e )
		{
			foreach( Component c in m_reflectedComponents )
			{
				m_Quick.Remove( c );
			}

			m_reflectedComponents.Clear();
            if (m_header.Parent != null)
            {
                RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
                ribbonControl.customizeQTACheckBoxAdvChecked = this.chkPlaceBelowRibbon.Checked;
            }
			UpdateDestinationList();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDown_Click( object sender, EventArgs e )
		{

			ListView.SelectedIndexCollection indexes = lstChosenItems.SelectedIndices;

			if( indexes.Count == 1 )
			{
				int index = indexes[0];

				if( index < lstChosenItems.Items.Count - 1 )
				{
					// Changing order in list
					Component c1 = m_reflectedComponents[index];
					Component c2 = m_reflectedComponents[index + 1];

					m_reflectedComponents.RemoveAt( index + 1 );
					m_reflectedComponents.Insert( index, c2 );

					m_Quick.Remove( c2 );
					m_Quick.Insert( m_Quick.IndexOf( c1 ), c2 );

					ListViewItem item = lstChosenItems.Items[index];

					lstChosenItems.Items.RemoveAt( index );
					lstChosenItems.Items.Insert( index + 1, item );

					lstChosenItems.EnsureVisible( index + 1 );
				}
			}

			lstChosenItems.Select();
		}
        /// <summary>
        /// ButtonClick event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (m_header.Parent != null)
            {
                RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
                ribbonControl.customizeQTACheckBoxAdvChecked = ribbonControl.customizeQTACheckBoxAdvCheckedSave;
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUp_Click( object sender, EventArgs e )
		{
			ListView.SelectedIndexCollection indexes = lstChosenItems.SelectedIndices;

			if( indexes.Count == 1 )
			{
				int index = indexes[0];

				if( index > 0 )
				{
					// Changing order in list
					Component c1 = m_reflectedComponents[index - 1];
					Component c2 = m_reflectedComponents[index];

					m_reflectedComponents.RemoveAt( index );
					m_reflectedComponents.Insert( index - 1, c2 );

					m_Quick.Remove( c2 );
					m_Quick.Insert( m_Quick.IndexOf( c1 ), c2 );

					ListViewItem item = lstChosenItems.Items[index];

					lstChosenItems.Items.RemoveAt( index );
					lstChosenItems.Items.Insert( index - 1, item );

					lstChosenItems.EnsureVisible( index - 1 );
				}
			}

			lstChosenItems.Select();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstChosenItems_Validating( object sender, CancelEventArgs e )
		{
			btnDown.Enabled = false;
			btnUp.Enabled = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HeaderRendererChanged( object sender, EventArgs e )
		{
			ApplyColorSchemeToControls();
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		private void UpdateLabels()
		{
			RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
			if( ribbonControl != null )
			{
				RibbonSystemText sysText = ribbonControl.SystemText;

				this.Text = sysText.QuickAccessCustomizeCaptionText;
				this.lblChooseCommandsFrom.Text = sysText.QuickAccessDialogCommandsText;
				this.btnAdd.Text = sysText.QuickAccessDialogAddText;
				this.btnRemove.Text = sysText.QuickAccessDialogRemoveText;
				this.btnOK.Text = sysText.QuickAccessDialogOkText;
				this.btnCancel.Text = sysText.QuickAccessDialogCancelText;
				this.btnReset.Text = sysText.QuickAccessDialogResetText;
				this.chkPlaceBelowRibbon.Text = sysText.QuickAccessPlaceBelowText;

				this.btnReset.Size = this.btnReset.GetPreferredSize( this.btnReset.Size );
			}
		}
		/// <summary>
		/// Updates source list view: clears it and then fills again.
		/// </summary>
		/// <param name="panel">Ribbon panel to extract items from.</param>
        private void UpdateSourceListFromPanel(RibbonPanel panel)
        {
            ClearSourceList();

            RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
            if (ribbonControl != null)
            {
                List<Component> itemsToHide = ribbonControl.ItemsToHideInCustomQuickDialog;

                List<ToolStripEx> toolStrips = new List<ToolStripEx>();

                foreach (Control c in panel.Controls)
                {
                    ToolStripEx toolStrip = c as ToolStripEx;

                    if (toolStrip != null)
                    {
                        AddImage(lstAvailableItems, (Component)toolStrip);
                        string text = GetDescription(toolStrip);

                        if (!itemsToHide.Contains(toolStrip))
                        {
                            ListViewItem listItem = new ListViewItem(text, lstAvailableItems.SmallImageList.Images.Count - 1);
                            listItem.Tag = toolStrip;
                            lstAvailableItems.Items.Add(listItem);
                        }
                        toolStrips.Add(toolStrip);
                    }
                }
                foreach (ToolStripEx toolStrip in toolStrips)
                {
                    ArrayList items = ExtractToolStripItems(toolStrip.GetItems());
                    foreach (ToolStripItem item in items)
                    {
                        if (!itemsToHide.Contains(item))
                        {
                            string text = GetDescription(item);
                            AddImage(lstAvailableItems, item);
                            ListViewItem listItem = new ListViewItem(text, lstAvailableItems.SmallImageList.Images.Count - 1);
                            listItem.Tag = item;
                            lstAvailableItems.Items.Add(listItem);
                        }
                    }
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (Bitmap bit = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if (g.DpiY > 96)
                    {

                        this.btnAdd.Location = new Point(this.Width / 2 - this.btnAdd.Size.Width + 22, btnAddLocationY + 25);
                        this.btnRemove.Location = new Point(this.Width / 2 - this.btnAdd.Size.Width + 22, btnRemoveLocationY + 40);
                        this.lstAvailableItems.Size = new Size((this.Width / 2 - this.lstAvailableItems.Location.X + 43) - ((3 * btnAdd.Width) / 2), this.Height / 2);
                        this.lstChosenItems.Location = new Point(this.Width / 2 + this.btnAdd.Width / 2, this.lstAvailableItems.Location.Y);
                        this.lstChosenItems.Size = new Size(this.lstAvailableItems.Width - 5, this.Height / 2);
                        this.btnUp.Location = new Point(this.Width - 5 * btnUp.Width / 2, btnUpLocationY + 25);
                        this.btnDown.Location = new Point(this.Width - 5 * btnDown.Width / 2, btndownLocationY + 40);
                        this.chkPlaceBelowRibbon.Location = new Point(this.lstAvailableItems.Location.X, this.lstAvailableItems.Location.Y + this.lstAvailableItems.Size.Height + 10);
                        this.label1.Location = new Point(this.lstChosenItems.Location.X - 1, this.lstChosenItems.Size.Height + this.lstChosenItems.Location.Y + 25);
                        this.btnReset.Location = new Point(this.lstChosenItems.Location.X + this.label1.Width, this.lstChosenItems.Size.Height + this.lstChosenItems.Location.Y + 20);
                        if (QuickAccessToolBarLabel != string.Empty)
                        {
                            label3.Text = WordWrap(QuickAccessToolBarLabel, this.lstChosenItems.Width / 6);
                            this.label3.Location = new System.Drawing.Point(this.lstChosenItems.Location.X, (this.lstAvailableItems.Location.Y - 30) - (this.label3.Height - 15));
                        }
                        else
                            this.label3.Location = new Point(this.lstChosenItems.Location.X, this.lstAvailableItems.Location.Y - 30);
                        this.comboPanels.Width = this.lstAvailableItems.Width;
                        this.comboPanels.Location = new Point(this.lstAvailableItems.Location.X, this.lstAvailableItems.Location.Y - 40);
                    }
                    else
                    {
                        this.btnAdd.Location = new Point(this.Width / 2 - this.btnAdd.Size.Width + 22, btnAddLocationY);
                        this.btnRemove.Location = new Point(this.Width / 2 - this.btnAdd.Size.Width + 22, btnRemoveLocationY);
                        this.lstAvailableItems.Size = new Size((this.Width / 2 - this.lstAvailableItems.Location.X + 43) - ((3 * btnAdd.Width) / 2), this.Height / 2);
                        this.lstChosenItems.Location = new Point(this.Width / 2 + this.btnAdd.Width / 2, this.lstChosenItems.Location.Y);
                        this.lstChosenItems.Size = new Size(this.lstAvailableItems.Width - 5, this.Height / 2);
                        this.btnUp.Location = new Point(this.Width - 5 * btnUp.Width / 2, btnUpLocationY);
                        this.btnDown.Location = new Point(this.Width - 5 * btnDown.Width / 2, btndownLocationY);
                        this.chkPlaceBelowRibbon.Location = new Point(this.lstAvailableItems.Location.X, this.lstAvailableItems.Location.Y + this.lstAvailableItems.Size.Height + 10);
                        this.label1.Location = new Point(this.lstChosenItems.Location.X - 1, this.lstChosenItems.Size.Height + this.lstChosenItems.Location.Y + 25);
                        this.btnReset.Location = new Point(this.lstChosenItems.Location.X + this.label1.Width, this.lstChosenItems.Size.Height + this.lstChosenItems.Location.Y + 20);
                        if (QuickAccessToolBarLabel != string.Empty)
                        {
                            label3.Text = WordWrap(QuickAccessToolBarLabel, this.lstChosenItems.Width / 5);
                            this.label3.Location = new System.Drawing.Point(lstChosenItems.Location.X, lstChosenItems.Location.Y - (this.label3.Height + 5));
                        }
                        else
                            this.label3.Location = new Point(this.lstChosenItems.Location.X, this.label3.Location.Y);

                        this.comboPanels.Width = this.lstAvailableItems.Width;
                        this.comboPanels.Location = new Point(this.lstAvailableItems.Location.X, this.lstAvailableItems.Location.Y - 40);
                    }
                }
            }
        }

        private void AddImage(QATListView listView, Component component)
        {
            Image image = m_header.GetImage(component);
            ImageList il = listView.SmallImageList;

            Size newSize = new Size(16, 16);
            using (Graphics g = this.CreateGraphics())
            {
                if (g.DpiX > 120)
                {
                    il.ImageSize = new System.Drawing.Size(25, 25);
                    newSize = new Size(25, 25);
                }
                else if (g.DpiX > 96)
                {
                    il.ImageSize = new System.Drawing.Size(20, 20);
                    newSize = new Size(20, 20);
                }
                else
                {
                    il.ImageSize = new System.Drawing.Size(16, 16);
                    newSize = new Size(16, 16);
                }
            }

            if (image.Size != newSize)
            {
                Image newImage = new Bitmap(newSize.Width, newSize.Height);

                using (Graphics g = Graphics.FromImage(newImage))
                {
                    Rectangle rcDest = new Rectangle(Point.Empty, newSize);

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, rcDest);
                }
                image = newImage;
            }

            il.Images.Add(image);
        }
        /// <summary>
        /// Updates source list view: clears it and then fills again.
        /// </summary>
        /// <param name="dropDown"> DropDown to extract items from.</param>
        private void UpdateSourceListFromDropDown(ToolStripDropDown dropDown)
        {
            ClearSourceList();
            ArrayList items = ExtractToolStripItems(dropDown.Items);
            RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
            List<Component> itemsToHide = ribbonControl.ItemsToHideInCustomQuickDialog;
            foreach (ToolStripItem item in items)
            {
                if (!itemsToHide.Contains(item))
                {
                string text = GetDescription(item);
                AddImage(lstAvailableItems, item);
                ListViewItem listItem = new ListViewItem(text, lstAvailableItems.SmallImageList.Images.Count - 1);
                listItem.Tag = item;
                lstAvailableItems.Items.Add(listItem);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        private string GetDescription(Component comp)
        {
            string text = m_header.GetText(comp);

            return this.Regex.Replace(text, " ");
        }

        /// <summary>
        /// Clears source list.
        /// </summary>
        private void ClearSourceList()
        {
            lstAvailableItems.Clear();
            lstAvailableItems.SmallImageList = CreateImageList();
            lstAvailableItems.RecreateHandle();
        }

        private static ImageList CreateImageList()
        {
            ImageList il = new ImageList();

            il.ColorDepth = ColorDepth.Depth32Bit;
            il.ImageSize = new Size(16, 16);

            return il;
        }

        /// <summary>
        /// Updates destination list view: clears it and then fills again.
        /// </summary>
        private void UpdateDestinationList()
        {
            ClearDestinationList();

            foreach (Component comp in m_reflectedComponents)
            {
                AddImage(lstChosenItems, comp);

                ListViewItem listItem = new ListViewItem(GetDescription(comp), lstChosenItems.SmallImageList.Images.Count - 1);
                listItem.Tag = comp;
                lstChosenItems.Items.Add(listItem);
            }
        }

        /// <summary>
        /// Clear destination list.
        /// </summary>
        private void ClearDestinationList()
        {
            lstChosenItems.Clear();
            lstChosenItems.SmallImageList = CreateImageList();
            lstChosenItems.RecreateHandle();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        private void AddItemsToDestinationList(ICollection items)
        {
            if (items != null && items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    Component comp = item.Tag as Component;

                    if (comp != null && !m_reflectedComponents.Contains(comp))
                    {
                        m_reflectedComponents.Add(comp);
                        m_Quick.Add(comp);
                    }
                }
                UpdateDestinationList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void RemoveItemsFromDestinationList(ICollection items)
        {
            if (items != null && items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    Component comp = item.Tag as Component;

                    if (comp != null)
                    {
                        int index = m_reflectedComponents.IndexOf(comp);

                        if (index >= 0)
                        {
                            m_reflectedComponents.RemoveAt(index);
                            int index2 = m_Quick.IndexOf(comp);
                            if (index2 >= 0)
                            {
                                m_Quick.RemoveAt(index2);
                            }
                        }
                    }
                }
                UpdateDestinationList();
            }
        }
        /// <summary>
        /// Extracts toolstrips and adds them to the collection.
        /// </summary>
        private void ExtractPanels()
        {
            comboPanels.Items.Clear();

            RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
            if (ribbonControl != null)
            {
                List<Component> itemsToHide = ribbonControl.ItemsToHideInCustomQuickDialog;

                foreach (ToolStripItem item in m_header.MainItems)
                {
                    ToolStripTabItem tabItem = item as ToolStripTabItem;

                    if (tabItem != null)
                    {
                        if (!itemsToHide.Contains(tabItem))
                            comboPanels.Items.Add(new RibbonPanelItem(tabItem, GetDescription(tabItem)));
                    }
                }

                if (m_header.MenuButtonVisible)
                {
                    ToolStripDropDown tsDropDown = m_header.MenuButtonDropDown;
                    StartPanel sp = new StartPanel(tsDropDown);

                    comboPanels.Items.Add(sp);
                }

                if (comboPanels.Items.Count > 0)
                {
                    comboPanels.SelectedIndex = 0;
                }

                comboPanels.ListBox.ItemHeight = 20;
                comboPanels.ListBox.DrawMode = DrawMode.OwnerDrawFixed;
                comboPanels.ListBox.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);
            }
        }

        void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawFocusRectangle();
            e.DrawBackground();

            string itemText = string.Empty;
            object obj = comboPanels.Items[e.Index];

            if (obj is RibbonPanelItem)
            {
                RibbonPanelItem ribbonItem = comboPanels.Items[e.Index] as RibbonPanelItem;
                itemText = ribbonItem.Panel.Text;
            }
            else if( obj is StartPanel )
            {
                StartPanel startPanel = comboPanels.Items[e.Index] as StartPanel;
                itemText = startPanel.ToString();
            }

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;

            if ((e.State & DrawItemState.Selected) != 0)
            {
            }
            else
            {
                using(Brush brush=new SolidBrush(e.BackColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);
            }
            using (Brush brush = new SolidBrush(comboPanels.ForeColor))
                e.Graphics.DrawString(itemText, comboPanels.Font, brush, e.Bounds, sf);
            sf.Dispose();
        }

		/// <summary>
		/// Extracts supported items list from specified collection.
		/// </summary>
		/// <param name="items">Source items collection</param>
		/// <returns>list of suported items</returns>
		private ArrayList ExtractToolStripItems( ToolStripItemCollection items )
		{
			ArrayList result = new ArrayList();

			foreach( ToolStripItem item in items )
			{
				if( item is ToolStripPanelItem )
				{
					ToolStripPanelItem panelItem = item as ToolStripPanelItem;
					result.AddRange( ExtractToolStripItems( panelItem.ToolStrip.GetItems() ) );
				}
				else if( IsSupportedItem( item ) )
				{
					result.Add( item );
				}
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool IsSupportedItem( ToolStripItem item )
		{
			return ( item is ToolStripButton ||
				item is ToolStripDropDownButton ||
				item is ToolStripSplitButton ||
				item is ToolStripSplitButtonEx ||
				item is ToolStripTextBox ||
				item is ToolStripComboBox ||
				item is ToolStripComboBoxEx );
		}

		/// <summary>
		/// 
		/// </summary>
		private void ApplyColorSchemeToControls()
		{
			foreach( Control ctl in this.Controls )
			{
				ISupportOffice2007Theme themed = ctl as ISupportOffice2007Theme;

				if( themed != null )
				{
			//		themed.Office2007ColorTheme = this.ColorScheme;
				}
			}

			//Office2007ColorScheme office2007ColorScheme = (Office2007ColorScheme)this.ColorScheme;

			//this.sfAvailableItems.OfficeColorScheme	= office2007ColorScheme;
			//this.sfChosenItems.OfficeColorScheme	= office2007ColorScheme;

		//	this.lblChooseCommandsFrom.ForeColor = ( this.ColorScheme == Office2007Theme.Black ) ? Color.White: Color.Black;
		}
		#endregion

		#region Fields
		/// <summary>
		/// Collection of components reflected in quick panel.
		/// </summary>
		private List<Component> m_reflectedComponents;
		/// <summary>
		/// Underlying RibbonControlAdvHeader to work with.
		/// </summary>
		private RibbonControlAdvHeader m_header;
		/// <summary>
		/// Collection of items in quick panel.
		/// </summary>
		private List<Component> m_Quick;
		/// <summary>
		/// 
		/// </summary>
		System.Text.RegularExpressions.Regex m_regex = null;
		#endregion

		#region *** StartPanel
		private class StartPanel
		{
			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			/// <param name="dropdown"></param>
			public StartPanel( ToolStripDropDown dropdown )
			{
				m_DropDown = dropdown;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				RibbonControlAdvHeader header = null;
				RibbonControlAdv ribbon = null;

				if( m_DropDown.OwnerItem != null )
				{
					header = m_DropDown.OwnerItem.Owner as RibbonControlAdvHeader;

					if( header != null )
					{
						ribbon = header.Parent as RibbonControlAdv;
					}
				}

				return ( ribbon != null ) ? ribbon.SystemText.QuickAccessDialogDropDownName : base.ToString();
			}
			#endregion

			#region Properties
			/// <summary> Gets DropDown with items. </summary>
			public ToolStripDropDown DropDown
			{
				get
				{
					return m_DropDown;
				}
			}
			#endregion

			#region Fields
			/// <summary> DropDown with items. </summary>
			private ToolStripDropDown m_DropDown = new ToolStripDropDown();
			#endregion
		}
		#endregion

		#region *** RibbonPanelItem
		class RibbonPanelItem
		{
			#region Constructors
			public RibbonPanelItem( ToolStripTabItem item, string text )
			{
				m_item = item;
				m_text = text;
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public RibbonPanel Panel
			{
				get { return m_item.Panel; }
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return m_text;
			}
			#endregion

			#region Fields
			ToolStripTabItem m_item;
			string m_text;
			#endregion
		}
		#endregion

		#region *** QuickComparer
		class QuickComparer: IComparer
		{
			#region Constructors
			public QuickComparer( IList template )
			{
				m_template = template;
			}
			#endregion

			#region IComparer Members
			public int Compare( object x, object y )
			{
				int ix = m_template.IndexOf( x is IQuickItem ? ( (IQuickItem)x ).ReflectedComponent : x );
				int iy = m_template.IndexOf( y is IQuickItem ? ( (IQuickItem)y ).ReflectedComponent : y );

				return ix - iy;
			}
			#endregion

			#region Fields
			IList m_template;
			#endregion
		}
		#endregion

        private void chkPlaceBelowRibbon_CheckStateChanged(object sender, EventArgs e)
        {
            if (m_header != null)
            {
                RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
                ribbonControl.customizeQTACheckBoxAdvChecked = (sender as CheckBoxAdv).Checked;
            }
        }
	}
}
