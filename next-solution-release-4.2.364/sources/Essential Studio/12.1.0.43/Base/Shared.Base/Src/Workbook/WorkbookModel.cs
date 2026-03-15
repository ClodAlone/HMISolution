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
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    ///    Provides the data / model part for an Excel-like Workbook display. A <see cref="WorkbookModel"/> has a <see cref="WorksheetModelCollection"/>
    ///    with multiple <see cref="WorksheetModel"/>s to display in the workbook. 
    /// </summary>
    /// <remarks>
    /// This class can be serialized into a serialization stream. <para/>
    /// Multiple <see cref="WorkbookView"/> controls can share the same <see cref="WorkbookModel"/>. Each <see cref="WorksheetView"/>
    /// that is displayed in a <see cref="WorkbookView"/> is associated with a <see cref="WorksheetModel"/> from the <see cref="Worksheets"/>
    /// collection in this object.
    /// </remarks>
    [
        //TypeConverter("Syncfusion.TabBarControl.WorkbookConverter"),
        Serializable,
		ToolboxItem(false)
	]
    public class WorkbookModel : Component, ISerializable, IDeserializationCallback
    {
        // Events

		/// <summary>
		/// Occurs when the <see cref="Name"/> has changed.
		/// </summary>
        public event EventHandler NameChanged;

		/// <summary>
		/// Occurs when the <see cref="ActiveView"/> has changed.
		/// </summary>
		public event EventHandler ActiveViewChanged;

        // Fields
        private string name;
		private WorksheetModelCollection worksheets = null;
        private object activeView = null;

		/// <overload>
		/// Initializes a new <see cref="WorkbookModel"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="WorkbookModel"/> and sets a name for the workbook.
		/// </summary>
		/// <param name="name">The name of this workbook.</param>
		public WorkbookModel(string name) 
        {
            this.name = name;
        }

		// Serialize
		/// <summary>
		/// Initializes a new <see cref="WorkbookModel"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		protected WorkbookModel(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif


			name = info.GetString("Name");
			worksheets = (WorksheetModelCollection) info.GetValue("Worksheets", typeof(WorksheetModelCollection));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif


			info.AddValue("Name", name); // String
			info.AddValue("Worksheets", worksheets); // WorksheetModelCollection
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			foreach (WorksheetModel wsm in worksheets)
				wsm.workbook = this;
		}			


		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				worksheets.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Returns the collection with <see cref="WorksheetModel"/> items.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WorksheetModelCollection Worksheets 
		{
			get
			{
				if (this.worksheets == null)
					this.worksheets = new WorksheetModelCollection();
				return worksheets;
			}
		}

		/// <summary>
		/// Gets / sets the active <see cref="WorkbookView"/> for this model.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ActiveView
        {
            get
            {
                return this.activeView;
            }
            set
            {
				if (value != this.activeView)
				{
					this.activeView = value;
					OnActiveViewChanged(EventArgs.Empty);
				}
            }
        }

		/// <summary>
		/// Raises the <see cref="ActiveViewChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnActiveViewChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, this.ActiveView);
#else
			;
#endif

			if (ActiveViewChanged != null)
				ActiveViewChanged(this, e);
		}

		/// <summary>
		/// Gets / sets the name for this workbook.
		/// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    OnNameChanged(EventArgs.Empty);
                }
            }
        }

		/// <summary>
		/// Raises the <see cref="NameChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnNameChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name);
#else
			;
#endif

			try
			{
				if (NameChanged != null)
					NameChanged(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}

		/// <override/>
		public override string ToString()
        {
            return "Workbook(" + this.name + "): " + base.ToString();
        }
    }
}
