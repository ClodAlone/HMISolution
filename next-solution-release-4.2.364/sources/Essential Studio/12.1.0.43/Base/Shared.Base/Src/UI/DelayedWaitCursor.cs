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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// DelayedWaitCursor listens to OperationFeedback events and changes
	/// the cursor if a certain number of ticks have elapsed since the start
	/// of the operation.
	/// </summary>
	public class DelayedWaitCursor: OperationFeedbackListener
	{
		Cursor savedCursor;

		/// <summary>
		/// Overloaded. Initializes a new <see cref="DelayedWaitCursor"/> instance.
		/// </summary>
		public DelayedWaitCursor()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="DelayedWaitCursor"/> instance.
		/// </summary>
		/// <param name="provider">An object that implements the <see cref="IOperationFeedbackProvider"/> interface.
		/// </param>
		/// <remarks>
		/// Adds an <see cref="IOperationFeedbackProvider"/> that this object will listen to and
		/// provide user feedback for.
		/// </remarks>
		public DelayedWaitCursor(IOperationFeedbackProvider provider)
		{
			AddProvider(provider);
		}

		/// <summary>
		/// Overriden. Displays a wait cursor.
		/// </summary>
		/// <param name="e">An <see cref="OperationFeedbackEventArgs" /> that contains the event data.</param>
		protected override void OnProgress(OperationFeedbackEventArgs e)
		{
			if (e.Milestone == OperationMilestone.Progress)
			{
				if (savedCursor == null)
				{
					savedCursor = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;
				}
			}
			else if (e.Milestone == OperationMilestone.Finished)
			{
				if (savedCursor != null)
				{
					Cursor.Current = savedCursor;
					savedCursor = null;
				}
			}
		}
	}

	/// <summary>
	/// DelayedStatusDialog listens to OperationFeedback events and displays
	/// a modeless status dialog if a certain number of ticks have elapsed since the start
	/// of the operation.
	/// </summary>
	public class DelayedStatusDialog: OperationFeedbackListener
	{
		// Fields
		private string description = "";
		private int percent = 0;
		private BackgroundThread backgroundThread;
		private Cursor savedCursor = null;
		private int showDialogPercentRule = 25;
		private int showWaitCursorPercentRule = 75;
		private bool showWaitCursor = true;
		private bool allowCancel = true;
		private bool dialogShown = false;

		/// <summary>
		/// Overloaded. Initializes a new <see cref="DelayedStatusDialog"/> instance.
		/// </summary>
		public DelayedStatusDialog()
		{
			this.Delay = 500;
		}

		/// <summary>
		/// Initializes a new <see cref="DelayedStatusDialog"/> instance.
		/// </summary>
		/// <param name="provider">An object that implements the <see cref="IOperationFeedbackProvider"/> interface.
		/// </param>
		/// <remarks>
		/// Adds an <see cref="IOperationFeedbackProvider"/> that this object will listen to and
		/// provide user feedback for.
		/// </remarks>
		public DelayedStatusDialog(IOperationFeedbackProvider provider)
			: this()
		{
			AddProvider(provider);
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				CloseStatusDialog(null);

			base.Dispose(disposing);
		}

		/// <summary>
		/// The progress percentage value to be used for displaying a dialog bar. A dialog bar
		/// is displayed when the operation's progress in percentage is less than the specified value
		/// at the time progress should be displayed.
		/// </summary>
		/// <remarks>
		/// Typical values are 25 for <see cref="DelayedStatusDialog.ShowDialogPercentRule"/>
		/// and 75 for <see cref="DelayedStatusDialog.ShowWaitCursorPercentRule"/>.
		/// <para/>
		/// In this example, if the operation takes longer than two seconds and after two seconds
		/// only 10% of the operation have completed, a dialog is displayed. If on the other side
		/// after two seconds more than 25% have been completed and less than 75%, a wait cursor is displayed.
		/// If more than 75% have been completed, no visual feedback at all will be given.
		/// </remarks>
		/// <seealso cref="DelayedStatusDialog.ShowWaitCursorPercentRule"/>
		[DefaultValue(25)]
		public int ShowDialogPercentRule
		{
			get
			{
				return showDialogPercentRule;
			}
			set
			{
				showDialogPercentRule = value;
			}
		}

		/// <summary>
		/// The progress percentage value to be used for displaying a wait cursor. A wait cursor 
		/// is shown when the operation's progress in percentage is less than the specified value
		/// at the time progress should be displayed.
		/// </summary>
		/// <remarks>
		/// <see cref="DelayedStatusDialog.ShowWaitCursorPercentRule"/> is ignored if
		/// <see cref="DelayedStatusDialog.ShowWaitCursor"/> is false.
		/// <para/>
		/// Typical values are 25 for <see cref="DelayedStatusDialog.ShowDialogPercentRule"/>
		/// and 75 for <see cref="DelayedStatusDialog.ShowWaitCursorPercentRule"/>.
		/// <para/>
		/// In this example, if the operation takes longer than two seconds and after two seconds
		/// only 10% of the operation have completed, a dialog is displayed. If on the other side
		/// after two seconds more than 25% have been completed and less than 75%, a wait cursor is displayed.
		/// If more than 75% have been completed, no visual feedback at all will be given.
		/// </remarks>
		/// <seealso cref="DelayedStatusDialog.ShowDialogPercentRule"/>
		[DefaultValue(75)]
		public int ShowWaitCursorPercentRule
		{
			get
			{
				return showWaitCursorPercentRule;
			}
			set
			{
				showWaitCursorPercentRule = value;
			}
		}


		/// <summary>
		/// Indicates whether wait cursors should be shown.
		/// </summary>
		/// <remarks>
		/// <see cref="DelayedStatusDialog.ShowWaitCursorPercentRule"/> is ignored if
		/// <see cref="DelayedStatusDialog.ShowWaitCursor"/> is False.
		/// </remarks>
		[DefaultValue(true)]
		public bool ShowWaitCursor
		{
			get
			{
				return showWaitCursor;
			}
			set
			{
				showWaitCursor = value;
			}
		}


		/// <summary>
		/// Overriden. Displays a dialog or wait cursor during progress and closes the dialog when operation is finished.
		/// </summary>
		/// <param name="e">An <see cref="OperationFeedbackEventArgs" /> that contains the event data.</param>
		protected override void OnProgress(OperationFeedbackEventArgs e)
		{
			if (e.Milestone == OperationMilestone.Progress)
			{
				int percent = e.Percent;
				if (dialogShown || percent >= 0 && percent < showDialogPercentRule)
				{
					ShowStatusDialog(e);
					dialogShown = true;
				}
				else if (savedCursor == null && showWaitCursor && percent < showWaitCursorPercentRule)
				{
					savedCursor = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;
				}
			}
			else if (e.Milestone == OperationMilestone.Finished)
			{
				if (savedCursor != null)
				{
					Cursor.Current = savedCursor;
					savedCursor = null;
				}
				CloseStatusDialog(e);
				dialogShown = false;
			}
		}

		bool IsDialogVisible
		{
			get
			{
				return backgroundThread != null;
			}
		}

		void ShowStatusDialog(OperationFeedbackEventArgs e)
		{
			if (!SystemInformation.UserInteractive)
				return;

			if (e.Percent > -1)
				this.percent = e.Percent;
			this.description = e.Description;
			this.allowCancel = e.AllowCancel;
			if (this.backgroundThread == null) 
				this.backgroundThread = new BackgroundThread(this);
			if (this.backgroundThread != null)
			{
				this.backgroundThread.UpdateLabel();
				if (this.backgroundThread.canceled)
				{
					e.Cancel = true;
					CloseStatusDialog(e);
				}
			}
		}

		void CloseStatusDialog(OperationFeedbackEventArgs e)
		{
			if (this.backgroundThread != null) 
				this.backgroundThread.Stop();
			this.backgroundThread = null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
			private class BackgroundThread 
		{
            
			// Fields
			private DelayedStatusDialog parent;
			private StatusDialog dialog;
			private Thread thread;
			internal bool canceled = false;
			private bool alreadyStopped = false;
            
			// Constructors
			internal BackgroundThread(DelayedStatusDialog parent)
			{
				this.parent = parent;
				this.thread = new Thread(new ThreadStart(Run));
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				this.thread.SetApartmentState(ApartmentState.STA);
#else
				this.thread.ApartmentState = ApartmentState.STA;
#endif
				this.thread.Start();
			} 
            
			private void Run()
			{
				// TODO: PermissionSet(SecurityAction.Assert)
				// <PermissionSet class="System.Security.PermissionSet"
				//                version="1">
				//    <IPermission class="System.Security.Permissions.SecurityPermission"
				//                 version="1"
				//                 Flags="UnmanagedCode"/>
				//    <IPermission class="System.Security.Permissions.UIPermission"
				//                 version="1"
				//                 Window="AllWindows"/>
				// </PermissionSet>
				try 
				{
					lock(this)
					{
						if (!this.alreadyStopped) 
						{
							this.dialog = new StatusDialog();
							this.ThreadUnsafeUpdateLabel();
							this.dialog.Visible = true;
						}
					}
					if (!this.alreadyStopped) 
						Application.Run(this.dialog);
				}
				finally 
				{
					lock(this)
					{
						if (this.dialog != null)
						{
							this.dialog.Dispose();
							this.dialog = null;
						}
					}
				}
			} 
            
            
			internal void Stop()
			{
				lock(this)
				{
					if (this.dialog != null && this.dialog.IsHandleCreated) 
						this.dialog.BeginInvoke(new MethodInvoker(this.dialog.Close));
					this.alreadyStopped = true;
				}
			} 
            
			private void ThreadUnsafeUpdateLabel()
			{
				this.dialog.Label = parent.description;
				this.dialog.Percent = parent.percent;
				this.dialog.AllowCancel = parent.allowCancel;
				if (this.dialog.Canceled)
					this.canceled = true;
			} 
            
            
			internal void UpdateLabel()
			{
				if (this.dialog != null && this.dialog.IsHandleCreated) 
					this.dialog.BeginInvoke(new MethodInvoker(this.ThreadUnsafeUpdateLabel));
			}             
		} 

	
		[Syncfusion.Documentation.DocumentationExclude()]
			private class StatusDialog : System.Windows.Forms.Form
		{
			private System.Windows.Forms.ProgressBar progressBar1;
			private System.Windows.Forms.Label label1;
			private System.Windows.Forms.Button cancelButton;
			private bool canceled = false;

			public bool AllowCancel
			{
				get
				{
					return cancelButton.Visible;
				}
				set
				{
					cancelButton.Visible = value;
				}
			}


			public bool Canceled
			{
				get
				{
					return canceled;
				}
				set
				{
					canceled = value;
				}
			}

			public int Percent
			{
				get
				{
					return progressBar1.Value;
				}
				set
				{
					progressBar1.Value = value;
				}
			}

			public string Label
			{
				get
				{
					return label1.Text;
				}
				set
				{
					label1.Text = value;
				}
			}


			/// <override/>
			protected override CreateParams CreateParams
			{
				[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
				get 
				{
					CreateParams cp;
					cp = base.CreateParams;
					Version version = Environment.OSVersion.Version;
					cp.ExStyle = (cp.ExStyle | 0x80/*WS_EX_TOOLWINDOW*/);

					// Otherwise has trouble createing window in NT4.0
					if(Environment.OSVersion.Platform != PlatformID.Win32NT || 
						Environment.OSVersion.Version.Major > 5)
						cp.ExStyle |= 0x08000000/*WS_EX_NOACTIVATE*/;

					cp.ClassStyle = cp.ClassStyle | 0x0800 /*CS_SAVEBITS*/;
					return cp;
				}
			}

			public StatusDialog()
			{
				InitializeComponent();
			}

			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.progressBar1 = new System.Windows.Forms.ProgressBar();
				this.label1 = new System.Windows.Forms.Label();
				this.cancelButton = new System.Windows.Forms.Button();
				this.SuspendLayout();
				// 
				// progressBar1
				// 
				this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
					| System.Windows.Forms.AnchorStyles.Right);
				this.progressBar1.Location = new System.Drawing.Point(24, 64);
				this.progressBar1.Name = "progressBar1";
				this.progressBar1.Size = new System.Drawing.Size(336, 32);
				this.progressBar1.Step = 5;
				this.progressBar1.TabIndex = 0;
				// 
				// label1
				// 
				this.label1.Location = new System.Drawing.Point(24, 16);
				this.label1.Name = "label1";
				this.label1.Size = new System.Drawing.Size(328, 32);
				this.label1.TabIndex = 1;
				this.label1.Text = "label1";
				this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
				// 
				// cancelButton
				// 
				this.cancelButton.Location = new System.Drawing.Point(152, 112);
				this.cancelButton.Name = "cancelButton";
				this.cancelButton.TabIndex = 2;
				this.cancelButton.Text = "&Cancel";
				this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
				// 
				// StatusDialog
				// 
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
#endif
				this.CausesValidation = false;
				this.ClientSize = new System.Drawing.Size(386, 160);
				this.ControlBox = false;
				this.Controls.AddRange(new System.Windows.Forms.Control[] {
																			  this.cancelButton,
																			  this.label1,
																			  this.progressBar1});
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "StatusDialog";
				this.ShowInTaskbar = false;
				this.Text = "ProviderProgress";
				this.TopMost = true;
				this.ResumeLayout(false);

			}

			private void cancelButton_Click(object sender, System.EventArgs e)
			{
				canceled = true;		
			}
		}
	}

}
