#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// CustomCollectionEditorForm for Carousel
    /// </summary>
	public class CustomCollectionEditorForm : MetroForm
	{

		#region "Variables"

		public delegate void InstanceEventHandler(object sender, object instance); 
		public event InstanceEventHandler InstanceCreated;
		public event InstanceEventHandler DestroyingInstance;
		public event InstanceEventHandler ItemRemoved;
		public event InstanceEventHandler ItemAdded;
		private IList _Collection=null;
		private ArrayList backupList=null;
        protected System.Windows.Forms.PropertyGrid pg_PropGrid;
        private System.Windows.Forms.Panel pan_ButtonsPan;
        private Button add;
        private Button remove;
        private Button down;
        private Button up;
        private Button ok;
        private Button cancel;
        private ComboBox comboBox1;
		private CustomCollectionEditor attachedEditor=null;
        private ListBox listBox1;
        private SplitContainer splitContainer1;
        private Label label1;
        private Label errlbl;
		
		#endregion

      

		#region "Properties"

		public IList Collection
		{
			get{return _Collection;}
			set
			{
				_Collection=value;
				backupList= new ArrayList(value);
				RefreshValues();
			}
		}


		#endregion

		#region "Constructors"
        Carousel carousel = null;
		public CustomCollectionEditorForm(Carousel _owner)
		{
            this.carousel = _owner;
			InitializeComponent();
			RefreshValues();
		}
		

		#endregion
			
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomCollectionEditorForm));
            this.pg_PropGrid = new System.Windows.Forms.PropertyGrid();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.add = new System.Windows.Forms.Button();
            this.remove = new System.Windows.Forms.Button();
            this.down = new System.Windows.Forms.Button();
            this.up = new System.Windows.Forms.Button();
            this.pan_ButtonsPan = new System.Windows.Forms.Panel();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.errlbl = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.pan_ButtonsPan.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pg_PropGrid
            // 
            this.pg_PropGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pg_PropGrid.BackColor = System.Drawing.Color.White;
            this.pg_PropGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pg_PropGrid.Location = new System.Drawing.Point(17, 34);
            this.pg_PropGrid.Name = "pg_PropGrid";
            this.pg_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pg_PropGrid.Size = new System.Drawing.Size(196, 300);
            this.pg_PropGrid.TabIndex = 3;
            this.pg_PropGrid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.pg_PropGrid_SelectedGridItemChanged);
            this.pg_PropGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pg_PropertyValueChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 70);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(232, 264);
            this.listBox1.TabIndex = 11;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(14, 14);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(75, 23);
            this.add.TabIndex = 10;
            this.add.Text = "&Add";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // remove
            // 
            this.remove.Location = new System.Drawing.Point(103, 14);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(75, 23);
            this.remove.TabIndex = 9;
            this.remove.Text = "&Remove";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // down
            // 
            this.down.Image = ((System.Drawing.Image)(resources.GetObject("down.Image")));
            this.down.Location = new System.Drawing.Point(250, 164);
            this.down.Name = "down";
            this.down.Size = new System.Drawing.Size(24, 29);
            this.down.TabIndex = 8;
            this.down.UseVisualStyleBackColor = true;
            this.down.Click += new System.EventHandler(this.down_Click);
            // 
            // up
            // 
            this.up.Image = ((System.Drawing.Image)(resources.GetObject("up.Image")));
            this.up.Location = new System.Drawing.Point(251, 106);
            this.up.Name = "up";
            this.up.Size = new System.Drawing.Size(23, 32);
            this.up.TabIndex = 7;
            this.up.UseVisualStyleBackColor = true;
            this.up.Click += new System.EventHandler(this.up_Click);
            // 
            // pan_ButtonsPan
            // 
            this.pan_ButtonsPan.BackColor = System.Drawing.Color.White;
            this.pan_ButtonsPan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pan_ButtonsPan.Controls.Add(this.add);
            this.pan_ButtonsPan.Controls.Add(this.remove);
            this.pan_ButtonsPan.Controls.Add(this.ok);
            this.pan_ButtonsPan.Controls.Add(this.cancel);
            this.pan_ButtonsPan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pan_ButtonsPan.Location = new System.Drawing.Point(0, 341);
            this.pan_ButtonsPan.Name = "pan_ButtonsPan";
            this.pan_ButtonsPan.Size = new System.Drawing.Size(513, 54);
            this.pan_ButtonsPan.TabIndex = 12;
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(330, 15);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 9;
            this.ok.Text = "&OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(423, 15);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 8;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // errlbl
            // 
            this.errlbl.ForeColor = System.Drawing.Color.Red;
            this.errlbl.Location = new System.Drawing.Point(9, 8);
            this.errlbl.Name = "errlbl";
            this.errlbl.Size = new System.Drawing.Size(268, 30);
            this.errlbl.TabIndex = 11;
            this.errlbl.Text = "* Disable Carousel control\'s Imageslides property to add items through editor";
            this.errlbl.Visible = false;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "ButtonAdv",
            "CheckboxAdv",
            "ComboBoxAdv",
            "Panel",
            "Label",
            "PictureBox",
            "RadiobuttonAdv",
            "TextBox"});
            this.comboBox1.Location = new System.Drawing.Point(109, 41);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.errlbl);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBox1);
            this.splitContainer1.Panel1.Controls.Add(this.down);
            this.splitContainer1.Panel1.Controls.Add(this.up);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pg_PropGrid);
            this.splitContainer1.Size = new System.Drawing.Size(513, 395);
            this.splitContainer1.SplitterDistance = 282;
            this.splitContainer1.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Selected Control";
            // 
            // CustomCollectionEditorForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.BorderColor = System.Drawing.Color.Black;
            this.CaptionAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ClientSize = new System.Drawing.Size(513, 395);
            this.ControlBox = false;
            this.Controls.Add(this.pan_ButtonsPan);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CustomCollectionEditorForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CarouselItemCollectionEditor";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CustomCollectionEditorForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomCollectionEditorForm_KeyDown);
            this.pan_ButtonsPan.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
	
		#region "Collection Item"

		/// <summary>
		/// Gets the data type of each item in the collection.
		/// </summary>
		/// <param name="coll"> The collection for which to get the item's type</param>
		/// <returns>The data type of the collection items.</returns>
		protected Type GetItemType(IList coll)
		{
			PropertyInfo pi= coll.GetType().GetProperty("Item",new Type[]{typeof(int)});
			return pi.PropertyType;
		}

		/// <summary>
		/// Gets the data types that this collection editor can contain
		/// </summary>
		/// <param name="coll">The collection for which to return the available types</param>
		/// <returns>An array of data types that this collection can contain.</returns>
		protected Type[] CreateNewItemTypes(IList coll) 
		{
			return new Type[]{ GetItemType(coll)};
		} 
		
		/// <summary>
		/// Creates a new instance of the specified collection item type.
		/// </summary>
		/// <param name="itemType">The type of item to create. </param>
		/// <returns>A new instance of the specified object.</returns>
		protected object CreateInstance(Type itemType)
		{

            /* 
            //This is just another way of how to remotely  create an object
			
            // Try to get a parameterless constructor
            ConstructorInfo ci = itemType.GetConstructor(new Type[0]);
            InstanceDescriptor id =  new InstanceDescriptor(ci,null,false);
            return id.Invoke();
            */


            object instance = Activator.CreateInstance(itemType, true);
            OnInstanceCreated(instance);
            return instance;
		}

		/// <summary>
		/// Destroys the specified instance of the object.
		/// </summary>
		/// <param name="instance">The object to destroy. </param>
		protected void DestroyInstance(	object instance	)
		{	
			OnDestroyingInstance(instance);
			if(instance is IDisposable){((IDisposable)instance).Dispose();}
			instance=null;
		}


		protected void OnDestroyingInstance(object instance)
		{
			if( DestroyingInstance!=null)
			{
				DestroyingInstance(this,instance);
			}
		}
	

		protected void OnInstanceCreated(object instance)
		{
			if (InstanceCreated!=null)
			{
				InstanceCreated(this,instance);
			}
		}
		
		protected void OnItemRemoved(object item)
		{
			if(ItemRemoved!=null)
			{
				ItemRemoved(this,item);
			}
		}

		protected void OnItemAdded(object Item)
		{
			if(ItemAdded!=null)
			{
				ItemAdded(this,Item);
			}
		}


		#endregion
		
		#region "Item Related"		

		private void MoveItem(IList list,int  index, int step)
		{

			if(index>-1 && index <list.Count && index+step >-1 && index+step<list.Count)
			{
				int poss=index+step;

				object possObject=list[poss];
				list[poss]=list[index];
				list[index]=possObject;
				possObject=null;
			}
		}


        protected internal object[] GenerateItemArray(IList collection)
		{
            object[] ti = new object[0];

			if (collection !=null && collection.Count>0)
			{
                ti = new object[collection.Count];
				
				for(int i=0;i<collection.Count;i++)
				{
					ti[i]=CreateItem(collection[i]);
				}
			}
			return ti;
		}

		/// <summary>
        /// Creates a new object for itemcollection .
		/// </summary>
		/// <param name="reffObject">The collection item for which to create an object.</param>
        protected virtual object CreateItem(object reffObject)
		{
            object ti = new object();
			SetProperties(ti,reffObject);
			return ti;
		}

		/// <summary>
        /// Cast the item into the type of reffObject to add in the item collection.
		/// </summary>
        /// <param name="titem">The object to be customized in respect to it's corresponding itemcollection.</param>
		/// <param name="reffObject">The collection item for which it customizes the created object.</param>
        protected virtual void SetProperties(object item, object reffObject)
        {
            PropertyInfo pi = reffObject.GetType().GetProperty("Name");
            item = reffObject.GetType().ToString();
        }
	
	
	
		#endregion

		#region "Implementation"

		protected virtual void RefreshValues()
		{
            if (this.Collection != null)
            {
                foreach (object c in this.Collection)
                {
                    listBox1.Items.Add(c);
                    OnItemAdded(c);
                }
            }
		}


		private   void pg_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
            object selTItem = listBox1.SelectedItem;
			SetProperties(selTItem,selTItem);
		}
		
		private void pg_PropGrid_SelectedGridItemChanged(object sender, System.Windows.Forms.SelectedGridItemChangedEventArgs e)
		{
		
			if (attachedEditor!=null)
			{
				attachedEditor.CollectionChanged-= new CustomCollectionEditor.CollectionChangedEventHandler(ValChanged);
				attachedEditor=null;
			}

			if ( e.NewSelection.Value is IList )
			{
				attachedEditor=(CustomCollectionEditor)e.NewSelection.PropertyDescriptor.GetEditor(typeof(System.Drawing.Design.UITypeEditor )) as CustomCollectionEditor;
				if (attachedEditor!=null)
				{
					attachedEditor.CollectionChanged+= new CustomCollectionEditor.CollectionChangedEventHandler(ValChanged);
				}
			}
				
				

		}


		private void ValChanged(object sender,object instance, object value )
		{
            object ti = listBox1.SelectedItem;
			SetProperties(ti,instance);	
		}

		private void UndoChanges(IList source, IList dest)
		{
			foreach(object o in dest )
			{
				if(!source.Contains(o))
				{
					DestroyInstance(o);
					OnItemRemoved(o);
				}			
			
			}
	
			dest.Clear();
			CopyItems(source,dest);
		}

		private void CopyItems(IList source, IList dest)
		{			
			foreach (object o in source)
			{
				dest.Add(o);
				OnItemAdded(o);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
		}

		#endregion

        private void cancel_Click(object sender, EventArgs e)
        {
            UndoChanges(backupList, Collection);
            this.Close();
        }

        private void up_Click(object sender, EventArgs e)
        {
            this.listBox1.BeginUpdate();
            object selItem = listBox1.SelectedItem;
            MoveItem(Collection, Collection.IndexOf(selItem), -1);
            this.listBox1.Items.Clear();
            RefreshValues();
            this.listBox1.EndUpdate();
            this.listBox1.Refresh();
            this.listBox1.SelectedItem = selItem;
            this.carousel.Populate();
        }

        private void down_Click(object sender, EventArgs e)
        {
            this.listBox1.BeginUpdate();
            object selItem = listBox1.SelectedItem;
            MoveItem(Collection, Collection.IndexOf(selItem), 1);
            this.listBox1.Items.Clear();
            RefreshValues();
            this.listBox1.EndUpdate();
            this.listBox1.Refresh();
            this.listBox1.SelectedItem = selItem;
            this.carousel.Populate();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void add_Click(object sender, EventArgs e)
        {
            //tv_Items.BeginUpdate();
            if (Collection != null && this.comboBox1.SelectedItem != null)
            {
                //create a new item to add to the Collection and a corespondent TItem to add to the treeview nodes
                Type type = null;
                switch (this.comboBox1.SelectedItem.ToString())
                {
                    case "CheckboxAdv":
                        type = typeof(CheckBoxAdv);
                        break;
                    case "ButtonAdv":
                        type = typeof(ButtonAdv);
                        break;
                    case "RadiobuttonAdv":
                        type = typeof(RadioButtonAdv);
                        break;
                    case "ComboBoxAdv":
                        type = typeof(ComboBoxAdv);
                        break;
                    case "TextBox":
                        type = typeof(TextBox);
                        break;
                    case "PictureBox":
                        type = typeof(CarouselItem);
                        break;
                    case "Label":
                        type = typeof(Label);
                        break;
                    case "Panel":
                        type = typeof(Panel);
                        break;
                    default:
                        type = typeof(Control);
                        break;

                }
              
                object newCollItem = CreateInstance(type);
                object newTItem = CreateItem(newCollItem);
                object selTItem = listBox1.SelectedItem;
                OnItemAdded(newCollItem);
                Collection.Add(newCollItem);

                listBox1.Items.Add(newCollItem);
               
                listBox1.SelectedItem = newCollItem;
                listBox1.DisplayMember = newCollItem.ToString();
            }
            this.carousel.Invalidate();
            this.carousel.Refresh();
            this.carousel.Parent.Invalidate();
            this.carousel.Parent.Refresh();
        }

        private void remove_Click(object sender, EventArgs e)
        {
            object selItem = this.listBox1.SelectedItem;
            if (selItem != null)
            {
                int nIndex = this.Collection.IndexOf(selItem);
                Collection.Remove(selItem);
                listBox1.Items.Remove(selItem);
                OnItemRemoved(selItem);
                this.carousel.Invalidate();
                this.carousel.Refresh();
                if (this.listBox1.Items.Count > nIndex)
                    this.listBox1.SelectedIndex = nIndex;
                else
                    this.listBox1.SelectedIndex = nIndex - 1;
            }
            
        }

        private void CustomCollectionEditorForm_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.Font;
            this.comboBox1.SelectedItem = "ButtonAdv";
            GetMessageTextandVisibilty();
            
            if (this.listBox1.Items.Count > 0)
                this.listBox1.SelectedItem = this.listBox1.Items[0];
        }

        private void GetMessageTextandVisibilty()
        {
            if (this.carousel.ImageSlides)
            {
                this.errlbl.Text = "* Disable Carousel control's Imageslides property to add items through editor";
                this.errlbl.Visible = true;
                this.add.Enabled = false;
                this.remove.Enabled = false;
            }
            else
            {
                this.errlbl.Visible = false;
                this.add.Enabled = true;
                this.remove.Enabled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null)
                this.pg_PropGrid.SelectedObject = this.listBox1.SelectedItem;
        }

        private void CustomCollectionEditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
	}
}

